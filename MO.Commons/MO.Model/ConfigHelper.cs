using Microsoft.Extensions.Configuration;

namespace MO.Model
{
    public class ConfigHelper
    {
        private static readonly string _path = "dbcontext.json";
        private static readonly IConfigurationRoot _builder =
             (new ConfigurationBuilder()).AddJsonFile(_path).Build();

        public static string GetConnectionString(string name)
        {
            return _builder.GetConnectionString(name);
        }

        public static string GetAppSetting(string key, string name)
        {
            return _builder.GetSection(key)[name];
        }
    }
}
