using System;

namespace SshMySql
{
    public static class Extensions
    {
        public static readonly string Localhost = "127.0.0.1";

        public static string GetDbConnectionString(this SshMySqlConnectionConfig config, uint boundPort)
        {
            return GetDbConnectionString(config.DbConfig, boundPort);
        }

        public static string GetDbConnectionString(this DatabaseConfig config, uint boundPort)
        {
            config.Server = Localhost;
            config.Port = boundPort;
            return config.ConnectionString;
        } // END GetDbConnectionString
    }
}
