using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.AppSettings
{
    /// <summary>
    /// ProviderOptions
    /// </summary>
    public class ProviderOptions
    {
        /// <summary>
        /// ModuleName
        /// </summary>
        public string ModuleName { get; set; }
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
