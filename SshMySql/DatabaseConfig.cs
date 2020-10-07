using System;
using System.Runtime.Serialization;
using MySqlConnector;

namespace SshMySql
{
    public class DatabaseConfig
    {
        private readonly MySqlConnectionStringBuilder builder;
        private string server;
        public string Server
        {
            get { return server; }
            set
            {
                builder.Server = value;
                server = value;
            }
        }
        private string username;
        public string UserName
        {
            get { return username; }
            set
            {
                builder.UserID = value;
                username = value;
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                builder.Password = value;
                password = value;
            }
        }
        private string database;
        public string Database
        {
            get { return database; }
            set
            {
                builder.Database = value;
                database = value;
            }
        }
        private uint port = 3306;
        public uint Port
        {
            get { return port; }
            set
            {
                builder.Port = value;
                port = value;
            }
        }
        
        public DatabaseConfig()
        {
            builder = new MySqlConnectionStringBuilder();
        }
        public DatabaseConfig(string connection)
        {
            // what if connection is json encoded DatabaseConfig?
            builder = new MySqlConnectionStringBuilder(connection);
        }

        [IgnoreDataMember]
        public string ConnectionString
        {
            get { return builder.ConnectionString; }
            set
            {
                builder.ConnectionString = value;
                server = builder.Server;
                username = builder.UserID;
                password = builder.Password;
                database = builder.Database;
                port = builder.Port;
            }
        } // END ConnectionString
    }
}
