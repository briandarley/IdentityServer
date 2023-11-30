using System.Reflection;
using Host.Infrastructure.Parsers;
using Host.Infrastructure.Validators;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Configuration.DependencyInjection.BuilderExtensions;
using IdentityServer4.EntityFramework;
using IdentityServer4.Models;
using IdentityServerHost;
using Microsoft.EntityFrameworkCore;


namespace Host.Infrastructure.DIRegistrations
{
    public static class IdentityServerDefaultRegistrations
    {
        public static void RegisterIdentityServerDefaults(this IServiceCollection services, IConfiguration configuration)
        {
            //DEFAULT
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryIdentityResources(IdentityServerHost.Configuration.Resources.IdentityResources)
            //    .AddInMemoryApiResources(IdentityServerHost.Configuration.Resources.ApiResources)
            //    .AddInMemoryApiScopes(IdentityServerHost.Configuration.Resources.ApiScopes)
            //    .AddInMemoryClients(Clients.Get())
            //    .AddAspNetIdentity<ApplicationUser>();


            //CUSTOM
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            
            var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
            if (connectionString == null)
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            var builder = services.AddIdentityServer
                (options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;

                    options.EmitScopesAsSpaceDelimitedStringInJwt = true;


                })
                //If using ASPNET Core Users add the below
                .AddAspNetIdentity<ApplicationUser>();
            builder

                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.DefaultSchema = "dbo";
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.DefaultSchema = "dbo";
                })

                .AddSigningCredential(configuration)
                .AddExtensionGrantValidator<Host.Infrastructure.Validators.ExtensionGrantValidator>()
                .AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
                .AddJwtBearerClientAuthentication()
                .AddAppAuthRedirectUriValidator()
                //.AddProfileService<DefaultProfileService>()
                //TODO, where is UserManager DI? Ans => <see cref="DbContextRegistrations.RegisterDbContexts"/>
                .AddProfileService<ProfileService<ApplicationUser>>()
                
                //.AddProfileService<ProfileService>()
                .AddCustomTokenRequestValidator<ParameterizedScopeTokenRequestValidator>()
                .AddScopeParser<ParameterizedScopeParser>()
                .AddMutualTlsSecretValidators();



        }
      
    }
}
