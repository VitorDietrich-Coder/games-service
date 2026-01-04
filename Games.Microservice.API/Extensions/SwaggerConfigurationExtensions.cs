using FGC.Api.Swagger.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace Games.Microservice.API.Extensions
{
    
    public static class SwaggerConfigurationExtensions
    {
        /// <summary>
        /// Adds Swagger middleware to the application pipeline, including Swagger UI.
        /// </summary>
        /// <param name="app">The application builder instance.</param>
        public static void UseSwaggerWithUI(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Games Micro serviço");
                options.RoutePrefix = "swagger";
            });

            app.UseReDoc(c =>
            {
                c.DocumentTitle = "REDOC API Documentation";
                c.SpecUrl = "/swagger/v1/swagger.json";
            }
            );
        }

        /// <summary>
        /// Adds and configures Swagger-related services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FGC API",
                    Version = "v1",
                    Description = "Documentação do Games Micro serviço"
                });
                options.OperationFilter<SwaggerResponseProfileOperationFilter>();

                ConfigureSecurity(options);
                ConfigureXmlComments(options);
                ConfigureFilters(options);
            });

            services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
        }


        /// <summary>
        /// Configures JWT Bearer authentication for Swagger.
        /// </summary>
        /// <param name="options">The SwaggerGen options.</param>
        private static void ConfigureSecurity(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Autentication JWT using Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        /// <summary>
        /// Adiciona os comentários XML da API ao Swagger.
        /// </summary>
        private static void ConfigureXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        }

        /// <summary>
        /// Configura filtros personalizados, exemplos e anotations para o Swagger.
        /// </summary>
        private static void ConfigureFilters(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            options.ExampleFilters();
            options.OperationFilter<SetApplicationJsonAsDefaultFilter>();
            options.EnableAnnotations();
            options.SchemaFilter<SuccessResponseSchemaFilter>();

            options.DocInclusionPredicate((docName, apiDesc) =>
                apiDesc.GroupName == null || apiDesc.GroupName == docName
            );
        }
    }
}
