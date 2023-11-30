using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Host.Infrastructure.Middleware
{
    public static  class AppMiddleware
    {
        internal static WebApplication CongigureAppMiddleware(this WebApplication app)
        {

            var env = app.Services.GetRequiredService<IWebHostEnvironment>();
            _ = app.Use(async (context, next) =>
            {
                var emptyHeader = context.Request.Headers.FirstOrDefault(c => c.Key.Equals("REMOTE_USER", StringComparison.CurrentCultureIgnoreCase));

                if (emptyHeader.Key != null)
                {
                    //Remove header because, with shibboleth, they're adding an additional REMOTE_USER for some reason
                    context.Request.Headers.Remove(emptyHeader);
                }
                await next.Invoke();
            });
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/plain";

                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        var consoleLogger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
                        consoleLogger.Error(ex.Error.Message);
                        // Write the error message to the response
                        await context.Response.WriteAsync("An error occurred while processing your request.");
                    }
                });
            });



            app.Use(async (context, next) =>
            {
                // Get all server variables from the incoming request
                var serverVariables = new Dictionary<string, string>
                {
                    // Add protocol information
                    { "REQUEST_METHOD", context.Request.Method },
                    { "PROTOCOL", context.Request.Scheme },
                    { "SERVER_NAME", context.Request.Host.Host },
                    { "SERVER_PORT", context.Request.Host.Port.ToString() },

                    // Add request path and query string
                    { "PATH_INFO", context.Request.Path },
                    { "QUERY_STRING", context.Request.QueryString.Value }
                };

                // Add headers
                foreach (var header in context.Request.Headers)
                {
                    string key = $"HTTP_{header.Key.ToUpper().Replace("-", "_")}";
                    serverVariables.Add(key, header.Value);
                }

                // Add remote client information
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    serverVariables.Add("REMOTE_ADDR", remoteIpAddress.ToString());
                    serverVariables.Add("REMOTE_PORT", context.Connection.RemotePort.ToString());
                }

                // Add local server information
                var localIpAddress = context.Connection.LocalIpAddress;
                if (localIpAddress != null)
                {
                    serverVariables.Add("LOCAL_ADDR", localIpAddress.ToString());
                    serverVariables.Add("LOCAL_PORT", context.Connection.LocalPort.ToString());
                }



                // Call the next middleware in the pipeline
                await next.Invoke();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //TODO
            //app.UseVerifyKerberosAuth();
            var isDockerEnvironment = Environment.OSVersion.Platform == PlatformID.Unix;
            var requireHttps = !isDockerEnvironment;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (requireHttps)
            {
                app.UseHttpsRedirection();
            }
            //https://stackoverflow.com/questions/46772300/setup-identity-server-4-reverse-proxy
            if (isDockerEnvironment)
            {
                var forwardOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                    RequireHeaderSymmetry = false
                };

                forwardOptions.KnownNetworks.Clear();
                forwardOptions.KnownProxies.Clear();
                app.UseForwardedHeaders(forwardOptions);

                app.Use(async (ctx, next) =>
                {

                    /*
                     * Using a reverse proxy, we have to reset the base path so that the
                     * .well-known/openid-configuration reads the correct path's for the various endpoints and append with the appropriate
                     * domain whether its localhost or some other domain

                    if we're calling the server directly, i.e. 'identityserver', we don't want to append base paths

                     */
                    if (ctx.Request.Host.Value == "identityserver")
                    {
                        ctx.Request.Scheme = "http";
                    }
                    else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("REWRITE_BASE_PATH")))
                    {
                        //if using nginx and path doesn't match
                        ctx.Request.PathBase = new PathString($"/{Environment.GetEnvironmentVariable("REWRITE_BASE_PATH")}");
                        ctx.SetIdentityServerBasePath($"/{Environment.GetEnvironmentVariable("REWRITE_BASE_PATH")}");
                    }


                    await next();
                });

            }

            //TODO
            //app.UseLogoutExtension();
            app.UseCertificateForwarding();
            app.UseCookiePolicy();

            //app.UseSerilogRequestLogging();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();


            app.Use(async (context, next) =>
            {
                //This is here to remedy cosmetic errors in the browser console. 
                if (OperatingSystem.IsWindows())//todo,remove if this isn't an issue in openshift
                {
                    context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self' https://selfservice.unc.edu/ https://its-idmuat-web.ad.unc.edu/ https://its-idmtst-web.adtest.unc.edu/ https://selfservicetest.unc.edu/");
                }
                await next.Invoke();
            });
            //TODO
            //app.UseSwagger(options =>

            //TODO
            //{
            //    options.RouteTemplate = "swagger/{documentName}/swagger.json";
            //    //FROM : https://github.com/wswind/swagger-proxy
            //    //Addresses the issues of displaying swagger docs behind NGINX
            //    if (isDockerEnvironment)
            //    {
            //        //FROM : https://github.com/wswind/swagger-proxy
            //        options.PreSerializeFilters.Add((swagger, httpReq) =>
            //        {
            //            //calling API Docs/Swagger from NGINX will always be https, but when queried within Docker, the host is http, which would be rejected because http isn't exposed from outside
            //            //swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/{httpReq.Headers["X-Forwarded-Prefix"]}" } };
            //            swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"https://{httpReq.Host.Value}/{httpReq.Headers["X-Forwarded-Prefix"]}" } };
            //        });
            //    }
            //}
            //);

            //app.UseSwaggerUI(options =>
            //{
            //    //options.SwaggerEndpoint("identityServer/swagger.json", "IdentityServer");
            //    options.SwaggerEndpoint("../swagger/v1/swagger.json", "IdentityServer");

            //    //options.EnableApiKeySupport("Bearer", "header");
            //});



            app.UseAuthentication();
            app.UseAuthorization();
            //TODO
            //app.UseMiddleware<SerilogEnrichMiddleware>();
            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });

            //app.MapControllers();



            return app;
        }
    }
}
