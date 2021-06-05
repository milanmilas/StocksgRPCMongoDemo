using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Configurations
{
    public class AlphavantageServiceConfiguration
    {
        public string DataSource { get; set; }
        public string Url { get; set; }
        public string[] Symbols { get; set; }

        public string Interval { get; set; }

        public string ApiKey { get; set; }

        public int ParallelRequests { get; set; }
    }
}
