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

        public static IApplicationBuilder UseApiLocalizationProvider(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            SubscribeKafkaTopic(app);

            return app;
        }


        private static void SubscribeKafkaTopic(IApplicationBuilder app)
        {
            Task.Run(() =>
            {
                using var scope = app.ApplicationServices.CreateScope();

                var _localizationMutatedHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler<LocalizationMutatedDto>>();


                if (_localizationMutatedHandler == null)
                    return;


                var optionsService = scope.ServiceProvider.GetRequiredService<IOptions<ApiLocalizationProviderOptions>>();

                var options = optionsService.Value;

                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<ProviderOptions>>();

                using (var c = new ConsumerBuilder<Ignore, string>(options.ConsumerConfig).Build())
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

            services.AddScoped<IMessageHandler<LocalizationMutatedDto>, LocalizationMutatedHandler>();

            
        }
    }
}
