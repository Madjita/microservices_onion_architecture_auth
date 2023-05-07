using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WritableConfig
{
    public class WritableNewtonsoftConfig<T> : WritableJsonConfig<T> where T : class, new()
    {
        public WritableNewtonsoftConfig(
            IWebHostEnvironment environment, 
            IOptionsMonitor<T> options, 
            IConfigurationRoot configuration, 
            string section, 
            string file) 
            : base(environment, options, configuration, section, file)
        {

        }

        public void SetConfig(T config)
        {
            Update(opt =>
            {
                opt.CopyFieldsFromObject(config);
            });
        }

        private void Update(Action<T> applyChanges)
        {
            //Добавлен семафор т.к. запись в файл это критическая секция.
            _semaphoreSlim.Wait();

            try
            {
                var fileInfo = new FileInfo(_file);
                if (!File.Exists(fileInfo.FullName))
                {
                    _semaphoreSlim.Release();
                    return;
                }

                var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(fileInfo.FullName));
                if (jObject == null)
                {
                    _semaphoreSlim.Release();
                    return;
                }

                var sectionObject = jObject.TryGetValue(_section, out JToken? section) ? JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());
                if (sectionObject == null)
                {
                    throw new InvalidCastException();
                }

                applyChanges(sectionObject);

                jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
                File.WriteAllText(fileInfo.FullName, JsonConvert.SerializeObject(jObject, Formatting.Indented));
                _configuration.Reload();

                NotifyAllHandlers();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}