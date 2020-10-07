using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using MySqlConnector;

namespace SshMySql
{
    public class SshMySqlConnection : DbConnection
    {
        private readonly SshClient sshClient;
        private readonly ForwardedPortLocal forward;
        private readonly MySqlConnection connection;

        public override string Database
        {
            get { return connection.Database; }
        }
        public override string ConnectionString
        {
            get { return connection.ConnectionString; }
            set { connection.ConnectionString = value; }
        }
        public override string DataSource
        {
            get { return connection.DataSource; }
        }
        public override ConnectionState State
        {
            get { return connection.State; }
        }
        public override string ServerVersion
        {
            get { return connection.ServerVersion; }
        }

        public SshMySqlConnection(string dbConf, SshClientConfig sshConf)
            : this(new DatabaseConfig(dbConf), sshConf)
        {
        }

        public SshMySqlConnection(SshMySqlConnectionConfig config)
            : this(config.DbConfig, config.SshConfig)
        {
        }

        public SshMySqlConnection(DatabaseConfig dbConf, SshClientConfig sshConf)
        {
            if (dbConf == null)
                throw new ArgumentNullException($"{nameof(dbConf)} must be specified.", nameof(dbConf));
            uint boundPort = (uint)dbConf.Port;
            if (sshConf != null)
            {
                if (String.IsNullOrEmpty(sshConf.HostName))
                    throw new ArgumentException($"{nameof(sshConf.HostName)} must be specified.", nameof(sshConf.HostName));
                if (String.IsNullOrEmpty(sshConf.UserName))
                    throw new ArgumentException($"{nameof(sshConf.UserName)} must be specified.", nameof(sshConf.UserName));
                if (String.IsNullOrEmpty(sshConf.Password) && String.IsNullOrEmpty(sshConf.KeyFile))
                    throw new ArgumentException($"One of {nameof(sshConf.Password)} and {nameof(sshConf.KeyFile)} must be specified.");
                if (String.IsNullOrEmpty(dbConf.Server))
                    throw new ArgumentException($"{nameof(dbConf.Server)} must be specified.", nameof(dbConf.Server));

                var authenticationMethods = new List<AuthenticationMethod>();
                if (!String.IsNullOrEmpty(sshConf.KeyFile))
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"KeyFile: {sshConf.KeyFile}");
#endif
                    authenticationMethods.Add(new PrivateKeyAuthenticationMethod(sshConf.UserName,
                        new PrivateKeyFile(sshConf.KeyFile, String.IsNullOrEmpty(sshConf.KeyPassPhrase) ? null : sshConf.KeyPassPhrase)));
                }
                if (!String.IsNullOrEmpty(sshConf.Password))
                {
                    authenticationMethods.Add(new PasswordAuthenticationMethod(sshConf.UserName, sshConf.Password));
                }

                sshClient = new SshClient(new ConnectionInfo(sshConf.HostName, sshConf.Port, sshConf.UserName, authenticationMethods.ToArray()));
                sshClient.Connect();

                // forward a local port to the database server and port, using the SSH server
                forward = new ForwardedPortLocal(Extensions.Localhost, dbConf.Server, (uint)dbConf.Port);
                sshClient.AddForwardedPort(forward);
                forward.Start();
                boundPort = forward.BoundPort;
            }
            connection = new MySqlConnection(dbConf.GetDbConnectionString(boundPort));
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return connection.BeginTransaction(isolationLevel);
        }

        public override void ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }
        public new DbCommand CreateCommand()
        {
            return connection.CreateCommand();
        }
        protected override DbCommand CreateDbCommand()
        {
            return connection.CreateCommand();
        }

        public override void Open()
        {
            connection.Open();
        }
        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return connection.OpenAsync(cancellationToken);
        }

        public override void Close()
        {
            connection.Close();
        }

        protected override void Dispose(bool disposing)
        {
            try { connection?.Dispose(); }
            catch (Exception) { }
            if (forward != null)
            {
                try { forward.Stop(); }
                catch (Exception) { }
                try { forward.Dispose(); }
                catch (Exception) { }
            }
            try { sshClient?.Dispose(); }
            catch (Exception) { }
        } // END Dispose
    }
}
