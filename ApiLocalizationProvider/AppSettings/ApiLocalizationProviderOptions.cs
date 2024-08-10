using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.AppSettings
{
    public class ApiLocalizationProviderOptions
    {
        /// <summary>
        /// ConsumerConfig
        /// </summary>
        public ConsumerConfig ConsumerConfig { get; set; }
        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get; set; } = "INUPCO_Localization_Mutated";
        /// <summary>
        /// CacheOptions
        /// </summary>
        public ProviderOptions ProviderOptions { get; set; }
    }
}
