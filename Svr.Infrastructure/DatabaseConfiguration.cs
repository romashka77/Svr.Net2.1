using Microsoft.Extensions.Configuration;

namespace Svr.Infrastructure
{
    public class DatabaseConfiguration : ConfigurationBase
    {
        private const string DataConnectionKey = "svrDataConnection";
        private const string AuthConnectionKey = "svrIdentityConnection";

        public string GetDataConnectionString() => GetConfiguration().GetConnectionString(DataConnectionKey);

        public string GetAuthConnectionString() => GetConfiguration().GetConnectionString(AuthConnectionKey);
    }
}
