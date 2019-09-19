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
using UserService.Database.Model;
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
            string logEndndpoint;
            try
            {
                ConfigContainer config = LoadConfig();
                logEndndpoint = config.Endpoint;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            SimpleIoc.Default.Register(() => new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); }).CreateMapper());
            SimpleIoc.Default.Register<IUserRepository, UserRepository>();
            SimpleIoc.Default.Register<ITokenManager, TokenManager.TokenManager>();
            SimpleIoc.Default.Register<ILogSender>(()=> new LogSender(logEndndpoint));
            SimpleIoc.Default.Register<UserService>();

            var userService = SimpleIoc.Default.GetInstance<UserService>();

            userService.Start();
        }

        private ConfigContainer LoadConfig()
        {
            ConfigContainer config;
            try
            {
                using (StreamReader r = new StreamReader("LoggerConfig.json"))
                {
                    string json = r.ReadToEnd();
                    config = JsonConvert.DeserializeObject<ConfigContainer>(json);
                }
            }
            catch
            {
                throw new FileNotFoundException("Can't find configuration file!");
            }

            return config;
        }

        public class ConfigContainer
        {
            public string Endpoint { get; set; }
        }
    }
}
