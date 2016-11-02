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
        private const string Format = "Elapsed: {0} Processed: {1:N0} Uploaded: {2:N0}";
        private static readonly Stopwatch Timer = Stopwatch.StartNew();
        private static long _processed;
        private static long _uploaded;
        private static bool _done;

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            if (!options.DryRun)
            {
                Task.Run(() =>
                {
                    while (!_done)
                    {
                        Console.Write($"{Format}\r", Timer.Elapsed, _processed, _uploaded);
                        Thread.Sleep(100);
                    }
                });
            }

            Parallel.ForEach(options.Files, options.ParallelOptions,
                file =>
                {
                    if (!options.Created.HasValue || File.GetCreationTime(file) > options.Created.Value)
                    {
                        if (!options.DryRun)
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
                                        CannedACL = options.Access?.S3CannedAcl ?? S3CannedACL.PublicRead
                                    });
                                    Interlocked.Increment(ref _uploaded);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{File.GetCreationTime(file)} {Path.GetFileName(file)}");
                        }
                    }

                    Interlocked.Increment(ref _processed);
                });

            _done = true;
            Timer.Stop();
            Console.WriteLine(Format, Timer.Elapsed, _processed, _uploaded);
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
