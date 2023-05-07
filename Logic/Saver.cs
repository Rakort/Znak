using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

namespace Logic
{
    public class Saver
    {
        public static void Save(string path, object data)
        {
            var text = JsonConvert.SerializeObject(data, Formatting.Indented);            
            File.WriteAllText(path, text);
        }

        public static T Load<T>(string path)
        {
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject<T>(text);
                return data;
            }
            else
                return default(T);
        }
    }
}
