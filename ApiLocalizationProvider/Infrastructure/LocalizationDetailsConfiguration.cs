using ApiLocalizationProvider.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Infrastructure
{
    /// <summary>
    /// Translation Details Fluent API Configurations
    /// </summary>
    public class LocalizationDetailsConfiguration : IEntityTypeConfiguration<LocalizationDetails>
    {
        /// <summary>
        /// Fluent API Configurations
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Configure(EntityTypeBuilder<LocalizationDetails> builder)
        {
            #region Table

            builder.ToTable("LocalizationDetails");

            builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnType("int").HasMaxLength(32)
                                       .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Key).IsRequired().HasColumnType("Nvarchar").HasMaxLength(100);

            builder.Property(e => e.OriginalTranslationEnglish).HasColumnType("Nvarchar").HasMaxLength(255);
            builder.Property(e => e.OriginalTranslationArabic).HasColumnType("Nvarchar").HasMaxLength(255);

            builder.Property(e => e.TranslationEnglish).HasColumnType("Nvarchar").HasMaxLength(255);
            builder.Property(e => e.TranslationArabic).HasColumnType("Nvarchar").HasMaxLength(255);

            builder.Property(e => e.Description).HasColumnType("Nvarchar").HasMaxLength(255);
            builder.Property(e => e.Notes).HasColumnType("Nvarchar").HasMaxLength(255);

            builder.Property(e => e.UsageCount).HasColumnType("int").HasMaxLength(32);

            builder.Property(e => e.ResourceName).HasColumnType("Nvarchar").HasMaxLength(512);


            builder.Property(e => e.UsagePaths).HasColumnType("Nvarchar").HasMaxLength(512);

            builder.Property(e => e.IsFrontendTranslation).HasColumnType("bit");
            builder.Property(e => e.CreationDate).HasColumnType("datetime");
            builder.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            builder.Property(e => e.CreatedBy).HasColumnType("Nvarchar").HasMaxLength(50);
            builder.Property(e => e.LastUpdatedBy).HasColumnType("Nvarchar").HasMaxLength(50);

            builder.Property(e => e.IsDeleted).HasDefaultValue(false).HasColumnType("bit");

            #endregion Table


        }
    }
}
