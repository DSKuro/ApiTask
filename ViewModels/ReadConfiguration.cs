using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTask.ViewModels
{
    public static class ReadConfiguration
    {
        private static readonly IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        public static String getValueByKey(String key)
        {
            if (configuration[key] == null)
            {
                return "";
            }

            return configuration[key];
        }
    }
}
