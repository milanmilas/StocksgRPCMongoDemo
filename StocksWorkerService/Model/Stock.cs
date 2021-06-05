using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Model
{
    public class Stock
    {
        public string DataSource { get; set; }

        public string Symbol { get; set; }

        public DateTime DateTime { get; set; }

        public string Data { get; set; }
    }
}
