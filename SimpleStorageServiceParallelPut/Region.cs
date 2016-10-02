using Amazon;

namespace SimpleStorageServiceParallelPut
{
    public class Region
    {
        public RegionEndpoint RegionEndpoint { get; set; }

        public Region(string region)
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(region);
        }
    }
}
