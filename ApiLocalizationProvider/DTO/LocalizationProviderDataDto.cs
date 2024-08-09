using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.DTO
{
    /// <summary>
    /// LocalizationProviderDataDto
    /// </summary>
    public class LocalizationProviderDataDto
    {
        #region PROPS
        /// <summary>
        /// Translation key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// TranslationEnglish 
        /// </summary>
        public string Value { get; set; }
        #endregion
    }
}
