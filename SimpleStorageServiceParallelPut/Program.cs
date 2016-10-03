using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using CommandLine;

namespace SimpleStorageServiceParallelPut
{
    public class Program
    {
        private const string Format = "Elapsed: {0} Processed: {1:N0}";
        private static readonly Stopwatch Timer = Stopwatch.StartNew();
        private static long _total;
        private static bool _done;

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            Task.Run(() =>
            {
                while (!_done)
                {
                    Console.Write($"{Format}\r", Timer.Elapsed, _total);
                    Thread.Sleep(100);
                }
            });

            Parallel.ForEach(options.Files, options.ParallelOptions,
                file =>
                {
                    using (var client = options.AmazonS3Client)
                    {
                        if (options.Overwrite || !Exists(client, options.BucketName, file))
                        {
                            client.PutObject(new PutObjectRequest
                            {
                                BucketName = options.BucketName,
                                Key = Path.GetFileName(file),
                                FilePath = file,
                                CannedACL = options.Access.S3CannedAcl
                            });
                        }
                    }

                    Interlocked.Increment(ref _total);
                });

            _done = true;
            Timer.Stop();
            Console.WriteLine(Format, Timer.Elapsed, _total);
        }

        private static bool Exists(IAmazonS3 client, string bucketName, string file)
        {
            try
            {
                client.GetObjectMetadata(new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = Path.GetFileName(file)
                });

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw;
            }
        }
    }
}
