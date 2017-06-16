using Exceptionless.WebHook.Abstractions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.Linq;
using System.Reflection;
using Web.Middleware;

namespace Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            hostingEnvironment.ConfigureNLog("nlog" + (hostingEnvironment.IsProduction() ? string.Empty : $".{hostingEnvironment.EnvironmentName}") + ".config");

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.Get<AppSettings>();

            var enableModules = new[] { "Exceptionless.WebHook.Abstractions" }.Concat(appSettings.EnableModules)
                .ToDictionary(name => name, name => Assembly.Load(new AssemblyName(name)));

            foreach (var item in enableModules.Where(i => i.Value == null))
                Console.WriteLine($"无法加载模块：{item.Key}");

            var assemblies = enableModules.Values.ToArray();
            services
                .AddLogging()
                .AddMemoryCache()
                .AddInterfaceDependency(assemblies)
                .AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                .AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseMiddleware<ExceptionlessWebhookMiddleware>()
                .AddNLogWeb();
        }
    }
}