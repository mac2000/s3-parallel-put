using Amazon.S3;

namespace SimpleStorageServiceParallelPut
{
    public class Access
    {
        public S3CannedACL S3CannedAcl { get; set; }

        public Access(string access)
        {
            S3CannedAcl = S3CannedACL.FindValue(access);
        }
    }
}
