using System;

namespace SshMySql
{
	public class SshClientConfig
	{
		public int Port { get; set; } = 22;

		public string HostName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string KeyFile { get; set; }
		public string KeyPassPhrase { get; set; }
	}
}
