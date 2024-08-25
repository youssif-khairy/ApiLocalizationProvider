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
            var sql = @$"
        INSERT INTO {_tableName} ([{nameof(LocalizationDetails.Key)}], {nameof(LocalizationDetails.TranslationEnglish)}, {nameof(LocalizationDetails.TranslationArabic)}, {nameof(LocalizationDetails.IsFrontendTranslation)}, {nameof(LocalizationDetails.ResourceName)}, {nameof(LocalizationDetails.IsDeleted)}, {nameof(LocalizationDetails.CreationDate)}, {nameof(LocalizationDetails.LastUpdatedDate)})
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
            var sql = @$"
        UPDATE {_tableName}
        SET {nameof(LocalizationDetails.TranslationEnglish)} = @TranslationEnglish, {nameof(LocalizationDetails.TranslationArabic)} = @TranslationArabic, {nameof(LocalizationDetails.IsDeleted)} = @IsDeleted, {nameof(LocalizationDetails.LastUpdatedDate)} = @LastUpdatedDate
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
            var sql = @$"
        SELECT * FROM {_tableName}
        WHERE {nameof(LocalizationDetails.IsDeleted)} = 0
        AND {nameof(LocalizationDetails.IsFrontendTranslation)} = @IsFrontend
        AND (@IsFrontend = 1 OR (@IsFrontend = 0 AND [{nameof(LocalizationDetails.Key)}] LIKE @ModuleWithPostfix + '%'))";

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
                                Id = reader.GetInt64(reader.GetOrdinal($"{nameof(LocalizationDetails.Id)}")),
                                Key = reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.Key)}")),
                                TranslationEnglish = reader.IsDBNull(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationEnglish)}")) ? null : reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationEnglish)}")),
                                TranslationArabic = reader.IsDBNull(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationArabic)}")) ? null : reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationArabic)}")),
                                IsFrontendTranslation = reader.GetBoolean(reader.GetOrdinal($"{nameof(LocalizationDetails.IsFrontendTranslation)}")),
                                ResourceName = reader.IsDBNull(reader.GetOrdinal($"{nameof(LocalizationDetails.ResourceName)}")) ? null : reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.ResourceName)}")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal($"{nameof(LocalizationDetails.IsDeleted)}")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal($"{nameof(LocalizationDetails.CreationDate)}")),
                                LastUpdatedDate = reader.GetDateTime(reader.GetOrdinal($"{nameof(LocalizationDetails.LastUpdatedDate)}"))
                            });
                        }
                    }
                }
            }

            return result;
        }

        public async Task<LocalizationDetails> GetLocalizationWithKeyAndTypeAsync( string key,bool isFrontend)
        {
            var sql = @$"
        SELECT * FROM {_tableName}
        WHERE [{nameof(LocalizationDetails.Key)}] = @Key
        AND {nameof(LocalizationDetails.IsFrontendTranslation)} = @IsFrontendTranslation";

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
                                Id = reader.GetInt64(reader.GetOrdinal($"{nameof(LocalizationDetails.Id)}")),
                                Key = reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.Key)}")),
                                TranslationEnglish = reader.IsDBNull(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationEnglish)}")) ? null : reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationEnglish)}")),
                                TranslationArabic = reader.IsDBNull(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationArabic)}")) ? null : reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.TranslationArabic)}")),
                                IsFrontendTranslation = reader.GetBoolean(reader.GetOrdinal($"{nameof(LocalizationDetails.IsFrontendTranslation)}")),
                                ResourceName = reader.IsDBNull(reader.GetOrdinal($"{nameof(LocalizationDetails.ResourceName)}")) ? null : reader.GetString(reader.GetOrdinal($"{nameof(LocalizationDetails.ResourceName)}")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal($"{nameof(LocalizationDetails.IsDeleted)}")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal($"{nameof(LocalizationDetails.CreationDate)}")),
                                LastUpdatedDate = reader.GetDateTime(reader.GetOrdinal($"{nameof(LocalizationDetails.LastUpdatedDate)}"))
                            };

                        }
                    }
                }
            }

            return result;
        }

    }
}
