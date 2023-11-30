// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;
using Host.Infrastructure.ConfigurationSetup;
using Host.Infrastructure.DIRegistrations;
using Host.Infrastructure.Middleware;
using Host.Infrastructure.OpenIdConnectExtentions;

namespace IdentityServerHost
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.Title = "IdentityServer4.AspNetIdentity";
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            var logPath = Environment.GetEnvironmentVariable("LOG_PATH");

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code);
            if (!string.IsNullOrEmpty(logPath))
            {
                loggerConfiguration = loggerConfiguration.WriteTo.File(
                    logPath,
                    fileSizeLimitBytes: 1_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1));
            }
            
            loggerConfiguration.CreateLogger();

            try
            {
                Log.Information("Starting host...");
                var builder = WebApplication.CreateBuilder(args);
                WebApplication app = null;
                builder.Logging.ClearProviders();
                var configuration = ConfigurationReader.GetConfiguration();

                builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
                builder.Configuration.AddConfiguration(configuration);



                builder.Services.AddRazorPages();
                builder.Services.RegisterDbContexts(configuration);
                builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

                builder.Services.RegisterIdentityServerDefaults(configuration);
#if (!DEBUG)
                //builder.Services.RegisterIdentityServerDefaults(configuration);
#endif
#if (DEBUG)

                //builder.Services.DebugRegisterIdentityServerDefaults(configuration);
   
#endif            

                builder.Services.AddAuthentication().AddGoogleOpenIdConnect();


                app = builder.Build();

                /*
                   public ProfileService(UserManager<TUser> userManager,
                   IUserClaimsPrincipalFactory<TUser> claimsFactory)
                 */

                //app.UseAutoMapperInititialization();


                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }


                app.CongigureAppMiddleware();


                app.MapControllers();
                //var scope = app.Services.CreateScope();
                //var dbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
                //dbContext.Database.Migrate();
                app.Run();
               

                //app.Run((s) =>
                //{

                //    var dbContext = s.RequestServices.GetService<PersistedGrantDbContext>();

                //    return Task.CompletedTask;

                //});
                //CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


    }
}