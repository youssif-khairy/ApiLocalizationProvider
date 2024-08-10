using ApiLocalizationProvider.AppSettings;
using ApiLocalizationProvider.BL;
using ApiLocalizationProvider.Controllers;
using ApiLocalizationProvider.DTO;
using ApiLocalizationProvider.Handlers;
using ApiLocalizationProvider.Infrastructure;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Extensions
{
    public static class ApiLocalizationProviderExtension
    {

        public static IServiceCollection AddApiLocalizationProvider<T>(this IServiceCollection services, Action<ApiLocalizationProviderOptions> action) where T : DbContext, ILocaliztionExtensionContext
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }


            services.Configure(action);

            AddServices<T>(services);

            AddControllers(services);

            return services;
        }

        public static IApplicationBuilder UseApiLocalizationProvider<T>(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            SubscribeKafkaTopic<T>(app, GetNewConsumerConfig(app));

            return app;
        }


        private static void SubscribeKafkaTopic<T>(IApplicationBuilder app, ConsumerConfig consumerConfig)
        {
            Task.Run(() =>
            {
                using var scope = app.ApplicationServices.CreateScope();

                var _localizationMutatedHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler<LocalizationMutatedWrapperDto>>();


                if (_localizationMutatedHandler == null)
                    return;


                var optionsService = scope.ServiceProvider.GetRequiredService<IOptions<ApiLocalizationProviderOptions>>();

                var options = optionsService.Value;

                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<ProviderOptions>>();

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

                                if (key != null && key.ModuleName == options.ProviderOptions.ModuleName)
                                {
                                    _localizationMutatedHandler.HandleAsync(new LocalizationMutatedWrapperDto
                                    {
                                        CacheKey = typeof(T),
                                        LocalizationMutatedDto = key
                                    });
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
            var uid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            if (injectConsumerConfig != null)
            {
                result.GroupId = $"{injectConsumerConfig.GroupId}_{Environment.MachineName}_{uid}";
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
                result.GroupId = $"{Assembly.GetEntryAssembly().GetName().FullName}_{Environment.MachineName}_{uid}";
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

        private static void AddServices<T>(IServiceCollection services) where T : DbContext, ILocaliztionExtensionContext
        {

            services.AddScoped<ILocalizationProviderService, LocalizationProviderService>();

            services.AddScoped<ILocaliztionExtensionContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IMessageHandler<LocalizationMutatedWrapperDto>, LocalizationMutatedHandler>();


        }
    }
}
