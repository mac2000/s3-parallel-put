using Amazon.Runtime;
using Amazon.S3;
using CommandLine;

namespace SimpleStorageServiceParallelPut
{
    public class Options
    {
        [Option('t', "threads", Default = 10, HelpText = "Number of threads to run")]
        public int Threads { get; set; }

        [Option('f', "folder", Required = true, HelpText = "Folder with files to upload")]
        public Folder Folder { get; set; }

        [Option('k', "key", Required = true, HelpText = "AWS Access Key ID")]
        public string AwsAccessKey { get; set; }

        [Option('s', "secret", Required = true, HelpText = "AWS Secret Key")]
        public string AwsSecretKey { get; set; }

        [Option('z', "zone", Required = true, HelpText = "AWS Region e.g. eu-central-1, us-west-1")]
        public Region AwsRegion { get; set; }

        [Option('b', "bucket", Required = true, HelpText = "Bucket name to upload files into")]
        public string BucketName { get; set; }

        [Option('a', "access", Default = "public-read", HelpText = "S3 canned ACL e.g. NoACL, private, public-read")]
        public Access Access { get; set; }

        [Option('p', "pattern", Default = "*.*", HelpText = "Search pattern, e.g. *.jpg")]
        public string Pattern { get; set; }

        [Option('r', "recursive", Default = false, HelpText = "Recursively iterate")]
        public bool Recursive { get; set; }


        public AWSCredentials AwsCredentials => new BasicAWSCredentials(AwsAccessKey, AwsSecretKey);

        public AmazonS3Client AmazonS3Client => new AmazonS3Client(AwsCredentials, AwsRegion.RegionEndpoint);
    }
}
