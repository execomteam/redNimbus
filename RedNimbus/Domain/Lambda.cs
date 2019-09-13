using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Domain
{
    public class Lambda
    {
        public string Name { get; set; }

        public string Trigger { get; set; }

        public string Runtime { get; set; }

        public string OwnerToken { get; set; }

        public string ImageId { get; set; }

        public string Guid { get; set; }
    }
}
