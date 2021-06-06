using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWorkerService.Utils
{
    public class DateTimeWrapper : IDateTime
    {
        DateTime IDateTime.UtcNow() => DateTime.UtcNow;
    }
}
