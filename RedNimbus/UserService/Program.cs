using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using Org.BouncyCastle.Asn1.Ocsp;
using RedNimbus.Domain;
using UserService.Database;
using UserService.Database.Model;
using UserService.Mapping;

namespace RedNimbus.UserService
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleIoc.Default.Register(() => new MapperConfiguration(mc =>{ mc.AddProfile(new MappingProfile()); }).CreateMapper());
            SimpleIoc.Default.Register<IUserRepository, UserRepository>();
            SimpleIoc.Default.Register<ITokenManager, TokenManager>();
            SimpleIoc.Default.Register<UserService>();

            var userService = SimpleIoc.Default.GetInstance<UserService>();

            userService.Start();
        }
    }
}
