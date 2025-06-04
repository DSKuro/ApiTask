using Microsoft.Extensions.Configuration;

namespace ApiTask.Services
{
    public static class ReadConfiguration
    {
        private static readonly IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        public static string? getValueByKey(string key)
        {
            return configuration[key];
        }
    }
}
