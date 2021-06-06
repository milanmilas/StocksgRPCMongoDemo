using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Utils
{
    public static class IConfigurationExtension
    {
        public static T RegisterConfiguration<T>(this IConfiguration configuration)
        {
            return configuration
                    .GetSection(typeof(T).Name)
                    .Get<T>();
        }
    }
}
