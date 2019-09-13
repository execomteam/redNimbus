using RedNimbus.Messages;
using Microsoft.AspNetCore.Http;
using static RedNimbus.Messages.LambdaMessage.Types;

namespace DTO
{
    public class CreateLambdaDto
    {
        public string Name { get; set; }

        public TriggerType Trigger { get; set; }

        public RuntimeType Runtime { get; set; }

        public string OwnerToken { get; set; }

        public string ImageId { get; set; }

        public string Guid { get; set; }

        public IFormFile File { get; set; }

    }
}
