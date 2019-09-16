using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Domain
{
    public class Lambda
    {
        public string Name { get; set; }
        public LambdaMessage.Types.TriggerType Trigger { get; set; }

        public LambdaMessage.Types.RuntimeType Runtime { get; set; }
        public Guid OwnerToken { get; set; }

        public Guid ImageId { get; set; }

        public Guid Guid { get; set; }
    }
}
