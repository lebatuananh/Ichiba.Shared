namespace Shared.Services
{
    public class FileAttachment
    {
        public FileAttachment()
        {
        }

        public FileAttachment(byte[] bytes, string fileName, string contentType)
        {
            Bytes = bytes;
            FileName = fileName;
            ContentType = contentType;
        }

        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}