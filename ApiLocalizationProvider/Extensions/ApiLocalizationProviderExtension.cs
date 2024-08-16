using ApiLocalizationProvider.BL;
using ApiLocalizationProvider.Controllers;
using ApiLocalizationProvider.DTO;
using ApiLocalizationProvider.Filters;
using ApiLocalizationProvider.Handlers;
using ApiLocalizationProvider.Infrastructure;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApiLocalizationProviderExtension
    {

        public static IServiceCollection AddApiLocalizationProvider<T>(this IServiceCollection services, Action<ApiLocalizationProviderOptions> action) where T : DbContext
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            string connectionString = GetConnectionString<T>(services);




            services.Configure<ApiLocalizationProviderOptions>(opts =>
                        {
                            var dbConfig = opts.DBConfigurationOptions;

                            dbConfig.ConnectionString = string.IsNullOrEmpty(dbConfig.ConnectionString) ? connectionString : dbConfig.ConnectionString;

                            opts.DBConfigurationOptions = dbConfig;
                        });

            services.Configure(action);
            var options = new ApiLocalizationProviderOptions();
            action.Invoke(options);

            DBInitializer.Initilaize(connectionString, options.DBConfigurationOptions.Schema).GetAwaiter().GetResult();

            if (options.IncludeInSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    c.DocumentFilter<CustomDocumentFilter>();
                });
            }

            AddServices<T>(services);

            AddControllers(services);

            return services;
        }

        private static string GetConnectionString<T>(IServiceCollection services) where T : DbContext
        {
            var serviceProvider = services.BuildServiceProvider();
            var dbContextOptions = serviceProvider.GetRequiredService<DbContextOptions<T>>();

            // Extract the connection string from DbContext options if needed
            var connectionString = dbContextOptions.Extensions
                .OfType<EntityFrameworkCore.Infrastructure.RelationalOptionsExtension>()
                .FirstOrDefault()?.ConnectionString;

            return connectionString;
        }

        public static IApplicationBuilder UseApiLocalizationProvider(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseRouting();

            UseCustomRouting(app);


            SubscribeKafkaTopic(app, GetNewConsumerConfig(app));

            return app;
        }

        private static void UseCustomRouting(IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<ApiLocalizationProviderOptions>>().Value.ApiRoutesOptions;

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: options.FrontendRoute,
                    pattern: options.FrontendRoute +"/{language}",
                    defaults: new { controller = "LocalizationProvider", action = "GetLocalizationModuleForFrontend" }
                );


                endpoints.MapControllerRoute(
                    name: options.BackendRoute,
                    pattern: options.BackendRoute + "/{resourceName}/{language}",
                    defaults: new { controller = "LocalizationProvider", action = "GetLocalizationModuleForBackEnd" }
                );
            });
        }

        private static void SubscribeKafkaTopic(IApplicationBuilder app, ConsumerConfig consumerConfig)
        {
            Task.Run(() =>
            {
                using var scope = app.ApplicationServices.CreateScope();

                var _localizationMutatedHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler<LocalizationMutatedDto>>();


                if (_localizationMutatedHandler == null)
                    return;


                var optionsService = scope.ServiceProvider.GetRequiredService<IOptions<ApiLocalizationProviderOptions>>();

                var options = optionsService.Value;

                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiLocalizationProviderOptions>>();

                using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                {
                    c.Subscribe(options.Topic);

                    try
                    {
                        while (true)
                        {
                            ConsumeResult<Ignore, string> consumeResult = null;
                            try
                            {
                                consumeResult = c.Consume();
                                LocalizationMutatedDto key = default;
                                try
                                {
                                    key = JsonSerializer.Deserialize<LocalizationMutatedDto>(consumeResult?.Message?.Value);
                                }
                                catch (JsonException ex)
                                {
                                    _logger.LogDebug(ex, $"Failed to deserialize message to type '{typeof(LocalizationMutatedDto)}'. Message: '{consumeResult?.Message?.Value}'");
                                }

                                if (key != null && key.ModuleName == options.ModuleName)
                                {
                                    _localizationMutatedHandler.HandleAsync(key);
                                }
                                _logger.LogInformation($"Consumed '{options.Topic}' message '{consumeResult?.Message?.Value}' at: '{consumeResult?.TopicPartitionOffset}'.");
                            }
                            catch (ConsumeException ex)
                            {
                                _logger.LogError(ex, $"Error consuming '{options.Topic}' message '{consumeResult?.Message?.Value}' at: '{consumeResult?.TopicPartitionOffset}' reason: '{ex.Error?.Reason}'.");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        c.Close();
                    }
                }
            });
        }
        private static ConsumerConfig GetNewConsumerConfig(IApplicationBuilder app)
        {
            var injectConsumerConfig = app.ApplicationServices.GetService<ConsumerConfig>() ?? app.ApplicationServices.GetService<IOptions<ApiLocalizationProviderOptions>>()?.Value.ConsumerConfig;
            var result = new ConsumerConfig();
            if (injectConsumerConfig != null)
            {
                result.GroupId = injectConsumerConfig.GroupId;
                result.BootstrapServers = injectConsumerConfig.BootstrapServers;
                result.SaslMechanism = injectConsumerConfig.SaslMechanism;
                result.SecurityProtocol = injectConsumerConfig.SecurityProtocol;
                result.SslEndpointIdentificationAlgorithm = injectConsumerConfig.SslEndpointIdentificationAlgorithm;
                result.SslCaLocation = injectConsumerConfig.SslCaLocation;
                result.SaslUsername = injectConsumerConfig.SaslUsername;
                result.SaslPassword = injectConsumerConfig.SaslPassword;
            }
            else
            {
                result.GroupId = Assembly.GetEntryAssembly().GetName().FullName;
            }
            result.AutoOffsetReset = AutoOffsetReset.Latest;
            result.EnableAutoCommit = true;
            result.AllowAutoCreateTopics = true;

            return result;
        }

        private static void AddControllers(IServiceCollection services)
        {
            var environmentControllerAssembly = typeof(LocalizationProviderController).Assembly;

            services.AddControllers()
                .AddNewtonsoftJson().PartManager.ApplicationParts
                .Add(new AssemblyPart(environmentControllerAssembly));
        }

        private static void AddServices<T>(IServiceCollection services) where T : DbContext
        {

            services.AddScoped<ILocalizationProviderService, LocalizationProviderService>();

            services.AddScoped<IMessageHandler<LocalizationMutatedDto>, LocalizationMutatedHandler>();

            services.AddScoped<IDbProvider, DbProvider>();


        }
    }
}
