using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class DBConfigurationOptions
    {
        public string ConnectionString { get; set; }
        public string Schema { get; set; } = "dbo";
    }
}
