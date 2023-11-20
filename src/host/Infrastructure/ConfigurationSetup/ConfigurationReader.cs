using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Host.Infrastructure.ConfigurationSetup
{
    public class ConfigurationReader
    {
        internal static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(delegate (JsonConfigurationSource c)
                {
                    c.Path = "appsettings.json";
                    c.ReloadOnChange = true;
                    c.Optional = false;
                })
                //TODO add config and config.{environment}.json
                //.AddJsonFile(delegate (JsonConfigurationSource c)
                //{
                //    c.Path = "config.json";
                //    c.ReloadOnChange = true;
                //    c.Optional = false;
                //})
                //.AddJsonFile(delegate (JsonConfigurationSource c)
                //    {
                //        string environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                //        c.Path = "config." + environmentVariable.ToLower() + ".json";
                //        c.ReloadOnChange = true;
                //        c.Optional = false;
                //    }
                //)
                .Build();
        }
    }
}
