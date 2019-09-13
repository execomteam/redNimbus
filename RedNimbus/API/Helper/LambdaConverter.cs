using DTO;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Helper
{
    public static class LambdaConverter
    {
        public static CreateLambdaDto LambdaMessageToDto(LambdaMessage message)
        {
            return new CreateLambdaDto
            {
                Name = message.Name,
                Trigger = message.Trigger,
                Runtime = message.Runtime,
                OwnerToken = message.OwnerId,
                ImageId = message.ImageId,
                Guid = message.Guid
            };
        }
    }
}
