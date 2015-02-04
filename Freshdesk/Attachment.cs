using System.IO;

namespace Freshdesk
{
    public class Attachment
    {
        public Stream Content { get; set; }
        public string FileName { get; set; }
    }
}
