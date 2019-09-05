using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedNimbus.BucketService.Helper
{
    public static class ConfigHelper
    {
        public static string LoadConfig()
        {
            Item item;
            try
            {
                using (StreamReader r = new StreamReader("bucketConfig.json"))
                {
                    string json = r.ReadToEnd();
                    item = JsonConvert.DeserializeObject<Item>(json);
                }
            }
            catch
            {
                throw new FileNotFoundException("Can't find Bucket configuration file!");
            }

            return item.Path;
        }
    }

    public class Item
    {
        public string Path { get; set; }
    }
}
