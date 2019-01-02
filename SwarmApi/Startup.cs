using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwarmApi.Clients;
using SwarmApi.Dtos;
using SwarmApi.Services;
using SwarmApi.Validators;
using Swashbuckle.AspNetCore.Swagger;

namespace SwarmApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug();
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Swarm API",
                    Description = "REST API for docker swarm",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Michal Marszalek",
                        Email = "mmpyro@gmail.com"
                    }
                });
                c.DescribeAllEnumsAsStrings();
            }); 
            services.Configure<SwarmConfiguration>(Configuration.GetSection("SwarmConfiguration"));
            services.AddTransient<ISwarmClient, SwarmClient>();
            services.AddTransient<INodeService, NodeService>();
            services.AddTransient<ISwarmService, SwarmService>();
            services.AddTransient<ISecretService, SecretService>();
            services.AddTransient<IValidator<SecretDto>, SecretValidator>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
	        loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SWARM API");
                c.RoutePrefix = string.Empty;
            });

            app.UseExceptionHandler(
                options => {
                    options.Run(
                    async context =>
                        {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "text/html";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null && env.IsDevelopment())
                        {
                            var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace }";
                            await context.Response.WriteAsync(err).ConfigureAwait(false);
                        }
                        else
                        {
                            await context.Response.WriteAsync("Internal server error.").ConfigureAwait(false);
                        }
                    });
            });
        }
    }
}
