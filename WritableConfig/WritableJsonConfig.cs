using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WritableConfig
{
    public class WritableJsonConfig<T> : IWritableConfig<T> where T : class, new()
    {
        protected readonly IWebHostEnvironment _environment;
        protected readonly IOptionsMonitor<T> _options;
        protected readonly IConfigurationRoot _configuration;
        protected readonly string _section;
        protected readonly string _file;
        protected List<EventHandler> _updateEventSubscribers = new List<EventHandler>();
        protected readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        protected T Value => _options.CurrentValue;

        T IOptions<T>.Value => throw new NotImplementedException();

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
            return _configuration.GetSection(_section).Get<T>() ?? throw new Exception($"Failed to get configuration section {_section} of type {typeof(T).FullName}");
        }
        
        public void AddHandlerToNotification(EventHandler eventHandler)
        {
            _updateEventSubscribers.Add(eventHandler);
        }
        
        public string GetSection()
        {
            return _section;
        }

        public void NotifyAllHandlers()
        {
            foreach (var eve in _updateEventSubscribers) 
            {
                eve.Invoke(this, new EventArgs());
            }
        }

        public void SetConfig(T config)
        {
            throw new NotImplementedException();
        }
    }
}
