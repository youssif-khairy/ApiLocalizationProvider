using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.DTO
{
    public class ApiStringLocalizerCacheKey
    {
        public string ResourceName { get; set; }
        public string Culture { get; set; }
    }
}
