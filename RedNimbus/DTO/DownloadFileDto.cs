using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class DownloadFileDto
    {
        public byte[] File { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}