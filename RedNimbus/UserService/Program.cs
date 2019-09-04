using AutoMapper;
using RedNimbus.TokenManager;
using UserService.Mapping;

namespace RedNimbus.UserService
{
    class Program
    {
        static void Main(string[] args)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            ITokenManager tokenManager = new TokenManager.TokenManager();
            UserService userService = new UserService(mapper, tokenManager);
            userService.Start();
        }
    }
}
