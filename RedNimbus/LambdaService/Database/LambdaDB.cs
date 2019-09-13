using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RedNimbus.LambdaService.Database
{
    [Table("Lambda")]
    public class LambdaDB
    {
        public string Name { get; set; }

        public LambdaMessage.Types.TriggerType Trigger { get; set; }

        public LambdaMessage.Types.RuntimeType Runtime { get; set; }

        public string OwnerToken { get; set; }

        
        public string ImageId { get; set; }

        [Key]
        public string Guid { get; set; }

    }
}
