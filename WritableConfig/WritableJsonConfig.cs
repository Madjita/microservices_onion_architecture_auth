using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WritableConfig
{
    public class WritableJsonConfig<T> : IWritableConfig<T> where T : class, new()
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _section;
        private readonly string _file;
        private List<EventHandler> NotifyHandlers = new List<EventHandler>();
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public WritableJsonConfig(
            IWebHostEnvironment environment,
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string section,
            string file)
        {
            _environment = environment;
            _options = options;
            _configuration = configuration;
            _section = section;
            _file = file;
        }

        public T GetConfigObject()
        {
            var PossibleNull = _configuration.GetSection(_section).Get<T>();
            if (PossibleNull == null) 
            {
                throw new InvalidCastException();
            }
            return PossibleNull;
        }
        
        public void SetConfig(T config)
        {
            Update(opt =>
            {
                opt.CopyFieldsFromObject(config);
            });
        }

        public void AddHandlerToNotification(EventHandler eventHandler)
        {
            NotifyHandlers.Add(eventHandler);
        }
        
        public string GetSection()
        {
            return _section;
        }

        public void NotifyAllHandlers()
        {
            foreach (var eve in NotifyHandlers) 
            {
                eve.Invoke(this, new EventArgs());
            }
        }

        private T Value => _options.CurrentValue;

        T IOptions<T>.Value => throw new NotImplementedException();

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
    
    public static class WritableJsonConfigUtil
    {
        /// <summary>
        /// Copy all fields from objFrom to objTo, if objTo or objFrom == null throws exception
        /// </summary>
        /// <param name="objTo"> Object copy to </param>
        /// <param name="objFrom"> Object copy from </param>
        public static void CopyFieldsFromObject<T>(this T objTo, T objFrom)
        {
            if (objTo == null || objFrom == null)
            {
                throw new InvalidCastException();
            }
            
            var propInfo = objFrom.GetType().GetProperties();
            foreach (var item in propInfo)
            {
                var tmp = objTo.GetType().GetProperty(item.Name);
                if (tmp == null) continue;
                tmp.SetValue(objTo, item.GetValue(objFrom, null), null);
            }
        }
    }
}
