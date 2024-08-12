using ApiLocalizationProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Infrastructure
{
    public interface IDbProvider
    {
        Task InsertLocalizationDetailsAsync(LocalizationDetails details);
        Task UpdateLocalizationDetailsAsync(LocalizationDetails details);
        Task<List<LocalizationDetails>> GetAllLocalizationWithFilterAsync(bool isFrontend, string moduleWithPostfix);
        Task<LocalizationDetails> GetLocalizationWithKeyAndTypeAsync(string key, bool isFrontend);
    }
}
