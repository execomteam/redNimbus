using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedNimbus.Communication;

namespace RedNimbus.BucketService
{
    public class BucketService : BaseService
    {
        private string _path;

        public BucketService(string path) : base()
        {
            _path = path;
        }
    }
}
