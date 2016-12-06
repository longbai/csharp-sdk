using System;
using Qiniu;
using Qiniu.Http;

namespace Qiniu.Processing
{
	/// <summary>
	/// Operation manager.
	/// </summary>
	public class OperationManager
	{
		private Auth auth;
		private long tokenExpire;
		private string domain;
		private Client client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.OperationManager"/> class.
		/// </summary>
		/// <param name="domain">Domain.</param>
		/// <param name="auth">Auth.</param>
		/// <param name="tokenExpire">Token expire.</param>
		/// <param name="config">Config.</param>
		public OperationManager(string domain, Auth auth, long tokenExpire, Configuration config)
		{
			this.auth = auth;
			this.domain = domain;
			this.tokenExpire = tokenExpire;
			client = new Client();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.OperationManager"/> class.
		/// </summary>
		/// <param name="domain">Domain.</param>
		public OperationManager(string domain):this(domain, null, 0, null)
		{
		}

		private String BuildUrl(string key, Pipe pipe)
		{
			string baseUrl = "http://" + domain + "/" + key + "?" + pipe;
			if (auth == null) {
				return baseUrl;
			}
			return auth.PrivateDownloadUrl(baseUrl, tokenExpire);
		}

		/// <summary>
		/// Get the specified key and op.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="op">Op.</param>
		public Response Get(string key, Operation op) {
			return Get(key, new Pipe().Append(op));
		}

		/// <summary>
		/// Get the specified key and pipe.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="pipe">Pipe.</param>
		public Response Get(string key, Pipe pipe) {
			String url = BuildUrl(key, pipe);
			return client.Get(url);
		}
	}
}
