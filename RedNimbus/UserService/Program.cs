using AutoMapper;
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
            UserService userService = new UserService(mapper);
            userService.Start();
        }
    }
}
