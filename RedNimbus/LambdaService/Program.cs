using RedNimbus.LambdaService.Helper;
using RedNimbus.LambdaService.Database;
using AutoMapper;
using RedNimbus.LambdaService.Mappings;

namespace RedNimbus.LambdaService
{
    class Program
    {
        private static IMapper _mapper;


        static void Main(string[] args)
        {
            var mc = new MapperConfiguration(m => { m.AddProfile(new Mapping()); });
            _mapper = mc.CreateMapper();
            Services.LambdaService service = new Services.LambdaService(new TokenManager.TokenManager(), new LambdaHelper(), new LambdaManagment(_mapper));
            service.Start();
        }
    }
}
