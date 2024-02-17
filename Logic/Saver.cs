using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using Logic.Model;
using Newtonsoft.Json;

namespace Logic
{
    public class Saver
    {
        /// <summary>
        /// сохранение цен в Json
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void Save(string path, object data)
        {
            var text = JsonConvert.SerializeObject(data, Formatting.Indented);            
            File.WriteAllText(path, text);
        }
        
        /// <summary>
        /// загрузка цен из Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path)
        {
            // проверка на наличие файла Json
            if (File.Exists(path))
            {
                // чтение текста из Json
                var text = File.ReadAllText(path);
                // преобразование Json в объект <T>
                var data = JsonConvert.DeserializeObject<T>(text); 
                return data;
            }
            else
                return Activator.CreateInstance<T>();
        }
    }
}
