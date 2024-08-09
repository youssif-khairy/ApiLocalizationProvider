using ApiLocalizationProvider.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Infrastructure
{
    public interface ILocaliztionExtensionContext
    {
        DbSet<LocalizationDetails> LocalizationDetails { get; set; }


        static void AddLocalizationConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LocalizationDetailsConfiguration());
        }

    }
}
