using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Configurations
{
    public class WebApiConfiguration
    {
        public int ParallelRequests { get; set; }
        public int RetryAttempts { get; set; }
        public int RequestTimeoutSeconds { get; set; }
    }
}
