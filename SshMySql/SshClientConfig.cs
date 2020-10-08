using Renci.SshNet;
using System;
using System.Collections.Generic;

namespace SshMySql
{
	public class SshClientConfig
	{
        private readonly List<AuthenticationMethod> authenticationMethods = new List<AuthenticationMethod>();

        public int Port { get; set; } = 22;

		public string HostName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string KeyFile { get; set; }
		public string KeyPassPhrase { get; set; }

		public ConnectionInfo GetConnectionInfo()
        {
            authenticationMethods.Clear();
            if (!String.IsNullOrEmpty(KeyFile))
            {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"KeyFile: {KeyFile}");
#endif
                var keyFile = new PrivateKeyFile(KeyFile,
                    String.IsNullOrEmpty(KeyPassPhrase) ? null : KeyPassPhrase);
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(UserName, keyFile));
            }
            if (!String.IsNullOrEmpty(Password))
            {
                authenticationMethods.Add(new PasswordAuthenticationMethod(UserName, Password));
            }

            return new ConnectionInfo(HostName, Port, UserName, authenticationMethods.ToArray());
        }
	}
}
