using Microsoft.Extensions.Configuration;
using System;

namespace ApiTask.Services
{
    public static class ReadConfiguration
    {
        private static readonly string ErrorMessage = "Запрашиваемое значение не существует или задан неверный ключ";
        private static readonly IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        public static string getValueByKey(string key)
        {
            if (configuration[key] == null)
            {
                throw new ArgumentNullException(ErrorMessage);
            }
            return configuration[key];
        }
    }
}
