using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ApiLocalizationProviderOptions
    {
        public ConsumerConfig ConsumerConfig { get; set; }
        public string Topic { get; set; } = "INUPCO_Localization_Mutated";
        public string ModuleName { get; set; }
        public ApiRoutesOptions ApiRoutesOptions { get; set; } = new ApiRoutesOptions();
        public CacheOptions CacheOptions { get; set; }
        public DBConfigurationOptions DBConfigurationOptions { get; set; } = new DBConfigurationOptions();
    }
}
