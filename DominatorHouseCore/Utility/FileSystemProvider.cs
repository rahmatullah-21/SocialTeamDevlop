using System.IO;

namespace DominatorHouseCore.Utility
{
    public interface IFileSystemProvider
    {
        byte[] ReadAllBytes(string path);
        bool Exists(string path);
    }

    public class FileSystemProvider : IFileSystemProvider
    {
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}
