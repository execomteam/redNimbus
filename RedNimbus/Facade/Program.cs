using Newtonsoft.Json;
using System;
using System.IO;

namespace RedNimbus.Facade
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            Facade facade = new Facade();
            facade.Start();
        }
    }
}
