using ApiLocalizationProvider.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Infrastructure
{
    public static class DBInitializer
    {
        public static async Task Initilaize(string connectionString, string schema)
        {
            var sql = CreateDbTablesScript(schema);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        private static string CreateDbTablesScript(string schema)
        {
            string tableName = schema + "." + nameof(LocalizationDetails);
            return $@"
            IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{schema}')
            BEGIN
	            EXEC('CREATE SCHEMA [{schema}]')
            END;
            IF OBJECT_ID(N'{tableName}',N'U') IS NULL
            BEGIN
            CREATE TABLE {tableName}(
	            [{nameof(LocalizationDetails.Id)}] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY,
                [{nameof(LocalizationDetails.Key)}] [nvarchar](200) NOT NULL,
	            [{nameof(LocalizationDetails.TranslationEnglish)}] [nvarchar](500) NULL,
	            [{nameof(LocalizationDetails.TranslationArabic)}] [nvarchar](500) NULL,
	            [{nameof(LocalizationDetails.IsFrontendTranslation)}] [bit] NOT NULL,
	            [{nameof(LocalizationDetails.ResourceName)}] [nvarchar](500) NULL,
	            [{nameof(LocalizationDetails.IsDeleted)}] bit DEFAULT 0,
	            [{nameof(LocalizationDetails.CreationDate)}] [date] DEFAULT GETDATE(),
	            [{nameof(LocalizationDetails.LastUpdatedDate)}] [date] DEFAULT GETDATE())
            END;

"

;
        }
    }
}
