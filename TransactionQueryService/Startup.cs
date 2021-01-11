using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Glasswall.Administration.K8.TransactionQueryService.Business.Serialisation;
using Glasswall.Administration.K8.TransactionQueryService.Business.Services;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Configuration;
using Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Prometheus;

namespace Glasswall.Administration.K8.TransactionQueryService
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddDebug();
            })
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("*",
                    builder =>
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                    });
            });

            services.TryAddTransient<ITransactionService, TransactionService>();
            services.TryAddSingleton<ISerialiser, JsonSerialiser>();
            services.TryAddTransient<IXmlSerialiser, XmlSerialiser>();
            services.TryAddTransient<IJsonSerialiser, JsonSerialiser>();

            services.TryAddTransient<IEnumerable<IFileStore>>(s =>
            {
                // Mounted directories in /mnt/stores/[storeName]
                return System.IO.Directory.GetDirectories("/mnt/stores")
                    .Select(share => new MountedFileStore(s.GetRequiredService<ILogger<MountedFileStore>>(), share)).ToArray();
            });

            var config = ValidateAndBind(Configuration);
            services.TryAddSingleton(config);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "auth-app",
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.TokenSecret))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthorization();
            app.UseMetricServer();
            app.UseHttpMetrics();
            
            app.Use((context, next) =>
            {
                context.Response.Headers["Access-Control-Expose-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";

                if (context.Request.Method != "OPTIONS") return next.Invoke();
                
                context.Response.StatusCode = 200;
                return context.Response.WriteAsync("OK");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            app.UseCors("*");
        }

        private ITransactionQueryServiceConfiguration ValidateAndBind(IConfiguration configuration)
        {
            var username = configuration["username"];
            var password = configuration["password"];

            if (string.IsNullOrWhiteSpace(username)) throw new ConfigurationErrorsException("username was not defined");
            if (string.IsNullOrWhiteSpace(password)) throw new ConfigurationErrorsException("password was not defined");

            var businessConfig = new TransactionQueryServiceConfiguration();

            configuration.Bind(businessConfig);

            return businessConfig;
        }
    }
}
