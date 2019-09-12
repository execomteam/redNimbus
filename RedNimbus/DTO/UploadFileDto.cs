using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace DTO
{
    public class UploadFileDto
    {
        public IFormFile File { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
    }
}
