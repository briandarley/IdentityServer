using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Host.Infrastructure.OpenIdConnectExtentions
{
    public static class GoogleOpenIdConnect
    {
        public static AuthenticationBuilder AddGoogleOpenIdConnect(this AuthenticationBuilder builder)
        {
            builder.AddOpenIdConnect("Google", "Google", options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();
                

                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

                options.Authority = "https://accounts.google.com/";
                options.ClientId = configuration["Google:ClientId"];//"708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";

                options.CallbackPath = "/signin-google";
                options.Scope.Add("email");
            });

            return builder;
        }

        //public static object AddGoogleOpenIdConnect(this AuthenticationBuilder builder)
        //{
        //    builder.AddOpenIdConnect("Google", "Google", options =>
        //    {
        //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        //        options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

        //        options.Authority = "https://accounts.google.com/";
        //        options.ClientId = configuration["Google:ClientId"];

        //        options.CallbackPath = "/signin-google";
        //        options.Scope.Add("email");
        //    });

        //    return builder;
        //}
    }
}
