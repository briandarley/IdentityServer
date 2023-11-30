using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System;
using UNC.IdentityModel;
using IdentityServer4.Configuration.DependencyInjection.BuilderExtensions;
namespace Host.Infrastructure.DIRegistrations
{
    public static class SigningCredentialsRegistrations
    {
        public static IIdentityServerBuilder AddSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            var position = 0;

            try
            {
                var contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey) ?? "";
                contentRoot = string.IsNullOrEmpty(contentRoot) ? "" : $"{contentRoot}/";

                if (OperatingSystem.IsLinux())
                {
                    contentRoot = "/";
                }
                
                
                //TODO fix this
                var rsaCert = new X509Certificate2($"{contentRoot}IdKeys/idsrv.x509.rsa.p12", "aQd8WHpQ3wGn", X509KeyStorageFlags.MachineKeySet
                                                                                                                 | X509KeyStorageFlags.UserKeySet
                                                                                                                 | X509KeyStorageFlags.PersistKeySet
                                                                                                                 | X509KeyStorageFlags.Exportable);
                //10/4/2022 Add Signing Credentials
                builder.AddSigningCredential(rsaCert, "RS256");
                position = 3;
                // ...and PS256
                builder.AddSigningCredential(rsaCert, "PS256");
                position = 4;
                //TODO fix this
                // or manually extract ECDSA key from certificate (directly using the certificate is not support by Microsoft right now)
                var ecCert = new X509Certificate2($"{contentRoot}IdKeys/idsrv.x509.ecdsa.p12", "aQd8WHpQ3wGn", X509KeyStorageFlags.MachineKeySet
                                                                                                                  | X509KeyStorageFlags.UserKeySet
                                                                                                                  | X509KeyStorageFlags.PersistKeySet
                                                                                                                  | X509KeyStorageFlags.Exportable);
                position = 5;

                var key = new ECDsaSecurityKey(ecCert.GetECDsaPrivateKey())
                {
                    KeyId = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)
                };
                position = 7;

                return builder.AddSigningCredential(
                    key,
                    IdentityServerConstants.ECDsaSigningAlgorithm.ES256);


            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Failed AddSigningCredential, last line {position}");
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                Log.Logger.Information("End AddSigningCredential");
            }
        }

    }
}
