using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.DTO
{
    /// <summary>
    /// LocalizationMutatedDto
    /// </summary>
    public class LocalizationMutatedDto
    {
        /// <summary>
        /// ModuleName
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// Localizations
        /// </summary>
        public List<LocalizationMutatedDetailsDto> Localizations { get; set; }
    }
}
