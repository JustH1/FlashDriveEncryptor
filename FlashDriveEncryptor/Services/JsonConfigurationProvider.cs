using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlashDriveEncryptor.Services
{
    internal class JsonConfigurationProvider : IConfigurationProvider
    {
        private readonly string _configurationPath;
        private readonly ILogger _logger;

        private readonly Dictionary<string, object>? _configurationValues;
        
        public JsonConfigurationProvider(ILogger logger)
        {
            _logger = logger;
            _configurationPath = GetConfigFilePath();
            _configurationValues = SelectConfigurationValues() ?? new Dictionary<string, object>();
        }

        public object? this[string key] 
        {
            get 
            { 
                object? value = null;
                _configurationValues?.TryGetValue(key, out value);
                return value;
            }
        }

        private string GetConfigFilePath()
        {
            try
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "appConfiguration.json");
            }
            catch (Exception ex)
            {
                _logger.Error("Can't find the appConfiguration.json file.", ex);
                throw;
            }
        }
        private Dictionary<string, object>? SelectConfigurationValues()
        {
            Dictionary<string, object>? configurationValues = null;

            try
            {
                string jsonContent = String.Empty;

                using (StreamReader sr = new StreamReader(_configurationPath))
                {
                    jsonContent = sr.ReadToEnd();
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                configurationValues = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent, options);

                if (configurationValues == null || configurationValues?.Count == 0)
                {
                    throw new Exception("Couldn't get configuration values from the appSettings.json file.");
                }

            }
            catch (JsonException ex)
            {
                _logger.Error("Data parsing error.", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
                throw;
            }
            
            return configurationValues;
        }
    }
}
