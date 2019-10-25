using Microsoft.Extensions.Configuration;
using System;

namespace Svr.Infrastructure
{
    public abstract class ConfigurationBase
    {
        protected IConfigurationRoot GetConfiguration()
        {
            // ReSharper disable once StringLiteralTypo
            return new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
        }

        protected void RaiseValueNotFoundException(string configurationKey)
        {
            // ReSharper disable once StringLiteralTypo
            throw new Exception($"не удалось найти ключ appsettings ({configurationKey}).");
        }
    }
}
