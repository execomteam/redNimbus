using System;

namespace RedNimbus.LambdaService
{
    class Program
    {
        static void Main(string[] args)
        {
            LambdaService service = new LambdaService(new TokenManager.TokenManager());
            service.Start();
        }
    }
}
