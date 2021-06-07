using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksGrpcService.Configurations
{
    public class DatabaseConfiguration
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
