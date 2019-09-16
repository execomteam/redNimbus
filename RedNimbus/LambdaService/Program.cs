using RedNimbus.LambdaService.Helper;
using RedNimbus.LambdaService.Database;
using AutoMapper;

namespace RedNimbus.LambdaService
{
    class Program
    {
        private static IMapper _mapper;
        static void Main(string[] args)
        {
            Services.LambdaService service = new Services.LambdaService(new TokenManager.TokenManager(), new LambdaHelper(), new LambdaManagment(_mapper));
            service.Start();
        }
    }
}
