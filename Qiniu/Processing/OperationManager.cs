using System;
using Qiniu;
using Qiniu.Http;

namespace Qiniu.Processing
{
	public class OperationManager
	{
		private Auth auth;
		private long tokenExpire;
		private string domain;
		private Client client;

		public OperationManager(string domain, Auth auth, long tokenExpire)
		{
			this.auth = auth;
			this.domain = domain;
			this.tokenExpire = tokenExpire;
			client = new Client();
		}

		public OperationManager(string domain):this(domain, null, 0)
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

		public Response Get(string key, Operation op) {
			return Get(key, new Pipe().Append(op));
		}

		public Response Get(string key, Pipe pipe) {
			String url = BuildUrl(key, pipe);
			return client.Get(url);
		}
	}
}
