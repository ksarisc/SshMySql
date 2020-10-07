using System;

namespace SshMySql
{
    public class SshMySqlConnectionConfig
    {
        public SshClientConfig SshConfig { get; set; }
        public DatabaseConfig DbConfig { get; set; }
    }
}
