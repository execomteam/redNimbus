using RedNimbus.LambdaService.Helper;

namespace RedNimbus.LambdaService
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.LambdaService service = new Services.LambdaService(new TokenManager.TokenManager(), new LambdaHelper());
            service.Start();
        }
    }
}
