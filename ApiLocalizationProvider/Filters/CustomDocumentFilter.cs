using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Filters
{
    public class CustomDocumentFilter : IDocumentFilter
    {
        private readonly ApiLocalizationProviderOptions _options;

        public CustomDocumentFilter(IOptions<ApiLocalizationProviderOptions> options)
        {
            _options = options.Value;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var frontendRoute = _options.ApiRoutesOptions.FrontendRoute;
            var backendRoute = _options.ApiRoutesOptions.BackendRoute;


            swaggerDoc.Paths.Add($"/{frontendRoute}/{{language}}", new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new OpenApiOperation
                    {
                        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Localization Provider" } },

                        Summary = "Get Localization Module for Frontend",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse { Description = "Success" }
                        },
                        Parameters = new List<OpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "language",
                            In = ParameterLocation.Path,
                            Required = true,
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }
                    }
                    }
                }
            });


            swaggerDoc.Paths.Add($"/{backendRoute}/{{resourceName}}/{{language}}", new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new OpenApiOperation
                    {
                        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Localization Provider" } },
                        Summary = "Get Localization Module for Backend",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse { Description = "Success" }
                        },
                        Parameters = new List<OpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "resourceName",
                            In = ParameterLocation.Path,
                            Required = true,
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        },
                        new OpenApiParameter
                        {
                            Name = "language",
                            In = ParameterLocation.Path,
                            Required = true,
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }
                    }
                    }
                }
            });
        }
    }
}
