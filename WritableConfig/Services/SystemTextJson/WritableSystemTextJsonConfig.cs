using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WritableConfig.Services.SystemTextJson
{
    public class WritableSystemTextJsonConfig<T> : WritableJsonConfig<T> where T : class, new()
    {
        public WritableSystemTextJsonConfig(
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
            _semaphoreSlim.Wait();

            try
            {
                var fileInfo = new FileInfo(_file);
                if (!File.Exists(fileInfo.FullName))
                    throw new FileNotFoundException($"Failed to find configuration file {fileInfo.FullName}");

                var jsonNode = JsonNode.Parse(File.ReadAllText(fileInfo.FullName)) ?? throw new JsonException("Failed to parse json!");
                var jsonNodeSection = jsonNode[_section] ?? throw new JsonException($"Failed to find JsonNode section {_section}!");

                var sectionObject = JsonSerializer.Deserialize<T>(jsonNodeSection.ToJsonString()) ??
                                    throw new JsonException($"Failed to deserialize section to type {typeof(T).FullName}");

                applyChanges(sectionObject);

                var options = JsonHelpers.GetDefaultOptions();
                jsonNode[_section] = JsonSerializer.SerializeToNode(sectionObject, options);

                File.WriteAllText(fileInfo.FullName, JsonSerializer.Serialize(jsonNode, options));
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