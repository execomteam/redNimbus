using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using RedNimbus.Domain;
using RedNimbus.LogLibrary;
using RedNimbus.TokenManager;
using System;
using System.IO;
using UserService.Database;
using UserService.Mapping;

namespace RedNimbus.UserService
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            SimpleIoc.Default.Register(() => new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); }).CreateMapper());
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
