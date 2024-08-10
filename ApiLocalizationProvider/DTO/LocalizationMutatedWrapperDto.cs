using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.DTO
{
    public class LocalizationMutatedWrapperDto
    {
        public Type CacheKey { get; set; }
        public LocalizationMutatedDto LocalizationMutatedDto { get; set; }
    }
}
