﻿using Microsoft.AspNetCore.Http;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class CreateLambdaDto
    {
        public string Name { get; set; }

        public LambdaMessage.Types.TriggerType Trigger { get; set; }

        public LambdaMessage.Types.RuntimeType Runtime { get; set; }

        public string OwnerToken { get; set; }

        public string ImageId { get; set; }

        public string Guid { get; set; }

        public IFormFile File { get; set; }

    }
}
