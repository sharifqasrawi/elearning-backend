using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public string DownloadPath { get; set; }
        public string UploadPath { get; set; }
        public string OriginalFileName { get; set; }
        public string ModifiedFileName { get; set; }
        public DateTime UploadDateTime { get; set; }
        public Directory UploadDirectory { get; set; }
        public string FileType { get; set; }

    }
}
