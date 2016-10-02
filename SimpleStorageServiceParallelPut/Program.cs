using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3.Model;
using CommandLine;

namespace SimpleStorageServiceParallelPut
{
    public class Program
    {
        private static readonly ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
        private static readonly Stopwatch Timer = Stopwatch.StartNew();
        private static long _total;

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        public static void ProcessItem(Options options, string file)
        {
            using (var client = options.AmazonS3Client)
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

        private static void Run(Options options)
        {
            var tasks = new List<Task>
            {
                Task.Run(() => CollectFilesToQueue(options))
            };

            var waitCounter = 0;
            while (Queue.Count == 0 && waitCounter++ < 10)
            {
                Thread.Sleep(100);
            }

            tasks.Add(Task.Run(() => PrintProgress()));

            for (var i = 0; i < options.Threads; i++)
            {
                tasks.Add(Task.Run(() => ProcessQueue(options)));
            }

            Task.WaitAll(tasks.ToArray());
            Timer.Stop();
            Console.Clear();
            Console.WriteLine($"Processed {_total:N0} files in {Timer.Elapsed}");
        }

        private static void PrintProgress()
        {
            while (!Queue.IsEmpty)
            {
                Console.Write($"Processed: {_total:N0} Queued: {Queue.Count:N0} Elapsed: {Timer.Elapsed}\r");
                Thread.Sleep(100);
            }
        }

        private static void ProcessQueue(Options options)
        {
            string file;
            while (Queue.TryDequeue(out file))
            {
                ProcessItem(options, file);
                Interlocked.Increment(ref _total);
            }
        }

        private static void CollectFilesToQueue(Options options)
        {
            foreach (var file in Directory.EnumerateFiles(options.Folder.ToString(), options.Pattern, SearchOption.TopDirectoryOnly))
            {
                Queue.Enqueue(file);
            }
        }
    }
}
