using Blockcore.DataVault.Authentication;
using Blockcore.DataVault.Authorization;
using Blockcore.DataVault.Settings;
using Blockcore.DataVault.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Conventions;
using Newtonsoft.Json;
using System;

namespace Blockcore.DataVault
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BlockcoreDataVaultSettings>(this.Configuration.GetSection("Blockcore"));

            var camelCaseConventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConventionPack, type => true);

            //BsonClassMap.RegisterClassMap<Entity>();

            //BsonClassMap.RegisterClassMap<BsonDocument>(cm => {
            //    cm.AutoMap();
            //    cm.SetIdMember(cm.GetMemberMap("Id"));
            //    //cm.SetIdMember(cm.GetMemberMapForElement("Id"));
            //    cm.IdMemberMap.SetIdGenerator(GuidGenerator.Instance);
            //});

            var config = new ServerConfig();
            Configuration.Bind(config);

            services.AddResponseCompression();
            services.AddMemoryCache();

            var todoContext = new StorageContext(config.MongoDB);
            var repo = new StorageRepository(todoContext);

            services.AddSingleton<IStorageRepository>(repo);

            // Add service and create Policy to allow Cross-Origin Requests
            services.AddCors
            (
                options =>
                {
                    options.AddPolicy
                    (
                        "CorsPolicy",

                        builder =>
                        {
                            var allowedDomains = new[] { "http://localhost", "http://localhost:4200", "http://localhost:8080" };

                            builder
                            .WithOrigins(allowedDomains)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                        }
                    );
                });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            })
            .AddApiKeySupport(options => { });

            services.AddSingleton<IAuthorizationHandler, OnlyUsersAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, OnlyAdminsAuthorizationHandler>();
            services.AddSingleton<IGetApiKeyQuery, AppSettingsGetApiKeyQuery>();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(LoggingActionFilter));
            }).AddNewtonsoftJson(options =>
            {
                // Serializer.RegisterFrontConverters(options.SerializerSettings);
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddSwaggerGen(
            options =>
            {
                string assemblyVersion = typeof(Startup).Assembly.GetName().Version.ToString();

                string description = "Access to the Blockcore Data Vault.";

                description += " Authorization is enabled on this API. You must have API key to perform calls that are not public. Future versions will require DID, not API key.";

                options.AddSecurityDefinition(ApiKeyConstants.HeaderName, new OpenApiSecurityScheme
                {
                    Description = "API key needed to access the endpoints. Vault-Api-Key: YOUR_KEY",
                    In = ParameterLocation.Header,
                    Name = ApiKeyConstants.HeaderName,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Name = ApiKeyConstants.HeaderName,
                                    Type = SecuritySchemeType.ApiKey,
                                    In = ParameterLocation.Header,
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = ApiKeyConstants.HeaderName
                                    },
                                 },
                                 new string[] {}
                             }
                    });

                options.SwaggerDoc("dv", // "dv" = Data Vault, "edv" = Encrypted Data Vault
                       new OpenApiInfo
                       {
                           Title = "Blockcore Data Vault API",
                           Version = assemblyVersion,
                           Description = description,
                           Contact = new OpenApiContact
                           {
                               Name = "Blockcore",
                               Url = new Uri("https://www.blockcore.net/")
                           }
                       });

                SwaggerApiDocumentationScaffolder.Scaffold(options);

                options.DescribeAllEnumsAsStrings();

                options.DescribeStringEnumsInCamelCase();

                options.EnableAnnotations();
            });

            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()

            // services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "docs/{documentName}/openapi.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint("/docs/dv/openapi.json", "Blockcore Data Vault API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
