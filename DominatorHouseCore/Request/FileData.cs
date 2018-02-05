using System.Collections.Specialized;

namespace GramDominatorCore.Request
{
    public class FileData
    {
        public FileData(NameValueCollection headers, string fileName, byte[] contents)
        {
            this.Headers = headers;
            this.FileName = fileName;
            this.Contents = contents;
        }

        public byte[] Contents { get; }

        public string FileName { get; }

        public NameValueCollection Headers { get; }
    }
}
