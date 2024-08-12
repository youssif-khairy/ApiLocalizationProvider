using ApiLocalizationProvider.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Infrastructure
{
    public class DbProvider : IDbProvider
    {
        private readonly string _tableName;
        private readonly string _connectionString;
        public DbProvider(IOptions<ApiLocalizationProviderOptions> options)
        {
            var dbconfig = options.Value.DBConfigurationOptions;
            _tableName = dbconfig.Schema + "." + nameof(LocalizationDetails);
            _connectionString = dbconfig.ConnectionString;
        }
        public async Task InsertLocalizationDetailsAsync(LocalizationDetails details)
        {
            var sql = @"
        INSERT INTO LocalizationDetails ([Key], TranslationEnglish, TranslationArabic, IsFrontendTranslation, ResourceName, IsDeleted, CreationDate, LastUpdatedDate)
        VALUES ( @Key, @TranslationEnglish, @TranslationArabic, @IsFrontendTranslation, @ResourceName, @IsDeleted, @CreationDate, @LastUpdatedDate)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Key", details.Key);
                    command.Parameters.AddWithValue("@TranslationEnglish", details.TranslationEnglish ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TranslationArabic", details.TranslationArabic ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsFrontendTranslation", details.IsFrontendTranslation);
                    command.Parameters.AddWithValue("@ResourceName", details.ResourceName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsDeleted", details.IsDeleted);
                    command.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                    command.Parameters.AddWithValue("@LastUpdatedDate", DateTime.Now);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task UpdateLocalizationDetailsAsync(LocalizationDetails details)
        {
            var sql = @"
        UPDATE LocalizationDetails
        SET TranslationEnglish = @TranslationEnglish, TranslationArabic = @TranslationArabic, IsDeleted = @IsDeleted, LastUpdatedDate = @LastUpdatedDate
        WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", details.Id);
                    command.Parameters.AddWithValue("@TranslationEnglish", details.TranslationEnglish ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TranslationArabic", details.TranslationArabic ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsDeleted", details.IsDeleted);
                    command.Parameters.AddWithValue("@LastUpdatedDate", DateTime.Now);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<LocalizationDetails>> GetAllLocalizationWithFilterAsync( bool isFrontend, string moduleWithPostfix)
        {
            var sql = @"
        SELECT * FROM LocalizationDetails
        WHERE IsDeleted = 0
        AND IsFrontendTranslation = @IsFrontend
        AND (@IsFrontend = 1 OR (@IsFrontend = 0 AND [Key] LIKE @ModuleWithPostfix + '%'))";

            var result = new List<LocalizationDetails>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IsFrontend", isFrontend);
                    command.Parameters.AddWithValue("@ModuleWithPostfix", moduleWithPostfix);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new LocalizationDetails
                            {
                                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                Key = reader.GetString(reader.GetOrdinal("Key")),
                                TranslationEnglish = reader.IsDBNull(reader.GetOrdinal("TranslationEnglish")) ? null : reader.GetString(reader.GetOrdinal("TranslationEnglish")),
                                TranslationArabic = reader.IsDBNull(reader.GetOrdinal("TranslationArabic")) ? null : reader.GetString(reader.GetOrdinal("TranslationArabic")),
                                IsFrontendTranslation = reader.GetBoolean(reader.GetOrdinal("IsFrontendTranslation")),
                                ResourceName = reader.IsDBNull(reader.GetOrdinal("ResourceName")) ? null : reader.GetString(reader.GetOrdinal("ResourceName")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                LastUpdatedDate = reader.GetDateTime(reader.GetOrdinal("LastUpdatedDate"))
                            });
                        }
                    }
                }
            }

            return result;
        }

        public async Task<LocalizationDetails> GetLocalizationWithKeyAndTypeAsync( string key,bool isFrontend)
        {
            var sql = @"
        SELECT * FROM LocalizationDetails
        WHERE [Key] = @Key
        AND IsFrontendTranslation = @IsFrontendTranslation";

            var result = new LocalizationDetails();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    command.Parameters.AddWithValue("@IsFrontendTranslation", isFrontend);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result = new LocalizationDetails
                            {
                                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                Key = reader.GetString(reader.GetOrdinal("Key")),
                                TranslationEnglish = reader.IsDBNull(reader.GetOrdinal("TranslationEnglish")) ? null : reader.GetString(reader.GetOrdinal("TranslationEnglish")),
                                TranslationArabic = reader.IsDBNull(reader.GetOrdinal("TranslationArabic")) ? null : reader.GetString(reader.GetOrdinal("TranslationArabic")),
                                IsFrontendTranslation = reader.GetBoolean(reader.GetOrdinal("IsFrontendTranslation")),
                                ResourceName = reader.IsDBNull(reader.GetOrdinal("ResourceName")) ? null : reader.GetString(reader.GetOrdinal("ResourceName")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                LastUpdatedDate = reader.GetDateTime(reader.GetOrdinal("LastUpdatedDate"))
                            };

                        }
                    }
                }
            }

            return result;
        }

    }
}
