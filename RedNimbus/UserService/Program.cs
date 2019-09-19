using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using RedNimbus.TokenManager;
using UserService.Database;
using UserService.Mapping;

namespace RedNimbus.UserService
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleIoc.Default.Register(() => new MapperConfiguration(mc =>{ mc.AddProfile(new MappingProfile()); }).CreateMapper());
            SimpleIoc.Default.Register<IUserRepository, UserRepository>();
            SimpleIoc.Default.Register<ITokenManager, TokenManager.TokenManager>();
            SimpleIoc.Default.Register<UserService>();
            
            SimpleIoc.Default.Register<UserService>();
            SimpleIoc.Default.Register<IUserCommunicationService,UserCommunicationService>();

            var userService = SimpleIoc.Default.GetInstance<UserService>();

            userService.Start();
        }
    }
}
