using System.IO;

namespace SimpleStorageServiceParallelPut
{
    public class Folder
    {
        private readonly string _folder;

        public Folder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException();
            }

            _folder = folder;
        }

        public override string ToString() => _folder;
    }
}
