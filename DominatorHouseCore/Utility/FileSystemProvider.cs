using System.IO;

namespace DominatorHouseCore.Utility
{
    public interface IFileSystemProvider
    {
        byte[] ReadAllBytes(string path);
    }

    public class FileSystemProvider : IFileSystemProvider
    {
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
