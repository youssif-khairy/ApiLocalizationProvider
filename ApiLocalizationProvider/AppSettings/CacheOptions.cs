using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class CacheOptions
    {
        /// <summary>
        /// BackendMaxAge
        /// </summary>
        public TimeSpan? BackendMaxAge { get; set; }
        /// <summary>
        /// FrontendMaxAge
        /// </summary>
        public TimeSpan? FrontendMaxAge { get; set; }
    }
}
