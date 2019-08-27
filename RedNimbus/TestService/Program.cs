using System;

namespace TestService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TEST SERVICE STARTING");
            TestService svc = new TestService();
            svc.Start();
            Console.WriteLine("TEST SERVICE STARTED");
        }
    }
}
