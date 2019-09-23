using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using static RedNimbus.Messages.LambdaMessage.Types;

namespace RedNimbus.Domain
{
    public class Lambda
    {
        public string Name { get; set; }
        public TriggerType Trigger { get; set; }

        public RuntimeType Runtime { get; set; }
        public Guid OwnerGuid { get; set; }
        public string Guid { get; set; }
    }
}
