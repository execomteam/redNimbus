

using Newtonsoft.Json;
using System;
using System.IO;

namespace RedNimbus.LoggerService
{
    class Program
    {

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        public void Run()
        {
            try
            {
                ConfigContainer config = LoadConfig();
                Logger logger = new Logger(config.Endpoint, config.LogFilePath);
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ConfigContainer LoadConfig()
        {
            ConfigContainer tuple;
            try
            {
                using (StreamReader r = new StreamReader("LoggerConfig.json"))
                {
                    string json = r.ReadToEnd();
                    tuple = JsonConvert.DeserializeObject<ConfigContainer>(json);
                }
            }
            catch
            {
                throw new FileNotFoundException("Can't find log configuration file!");
            }

            return tuple;
        }
    }

    class ConfigContainer
    {
        public string Endpoint { get; set; }
        public string LogFilePath { get; set; }
    }
    
}
