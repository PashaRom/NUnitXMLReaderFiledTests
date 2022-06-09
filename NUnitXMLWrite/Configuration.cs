using System.IO;
using NUnitXMLReader.Models;
using System.Text.Json;

namespace NUnitXMLReader
{
    public sealed class Configuration
    {
        public static readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), fileConfig);
        public const string fileConfig = "setting.json";
        private static ConfigModel _config;

        private Configuration()
        {
        }

        private static ConfigModel ReadConfigFile()
        {
            var jsonSrting = File.ReadAllText(_configPath);
            var config = JsonSerializer.Deserialize<ConfigModel>(jsonSrting);
            return config;
        }

        public static ConfigModel Get
        {
            get
            {
               if(_config == null)
                {
                    _config = ReadConfigFile();
                }
                return _config;
            }
        }
    }
}
