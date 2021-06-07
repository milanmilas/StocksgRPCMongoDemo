using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Configurations
{
    public class IexServiceConfiguration : SchedulerConfiguration
    {
        public string DataSource { get; set; }
        public string Url { get; set; }
        public string[] Symbols { get; set; }

        public string Token { get; set; }
    }
}
