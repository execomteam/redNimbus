using Newtonsoft.Json;
using System;
using System.IO;

namespace RedNimbus.Facade
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            string logEndpoint = null;

            try
            {
                logEndpoint = LoadConfig().Endpoint;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("File can not be found");
                return;
            }

            Facade facade = new Facade(logEndpoint);
            facade.Start();
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
