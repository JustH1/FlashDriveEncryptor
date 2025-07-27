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
        public Dictionary<string, object>? GetFullConfiguration()
        {
            return _configurationValues;
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

                Dictionary<string, JsonElement> jsonConfigurationValues = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent, options);

                if (jsonConfigurationValues == null || jsonConfigurationValues?.Count == 0)
                {
                    throw new Exception("Couldn't get configuration values from the appSettings.json file.");
                }

                configurationValues = new Dictionary<string, object>();

                foreach (KeyValuePair<string, JsonElement> jsonConfigurationValue in jsonConfigurationValues)
                {
                    configurationValues[jsonConfigurationValue.Key] = jsonConfigurationValue.Value.ValueKind switch
                    {
                        JsonValueKind.Number => jsonConfigurationValue.Value.GetInt32(),
                        JsonValueKind.String => jsonConfigurationValue.Value.ToString(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => jsonConfigurationValue.Value.ToString()
                    };
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
