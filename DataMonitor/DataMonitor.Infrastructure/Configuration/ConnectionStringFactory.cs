using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitor.Infrastructure.Configuration
{
    public class ConnectionStringFactory
    {
        public static string JCJTConnectionString { get { return ConfigurationManager.ConnectionStrings["ConnJCJT"].ToString(); } }
    }
}
