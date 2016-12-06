using System;
using System.Text;
using System.Collections.Generic;
using Qiniu.Util;
using Qiniu.Storage.Model;
using Qiniu.Http;

using Qiniu;

namespace Qiniu.Storage
{
	/// <summary>
	/// Bucket manager.
	/// </summary>
	public class BucketManager
	{
		private Auth auth;
		private Client client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Storage.BucketManager"/> class.
		/// </summary>
		/// <param name="auth">Auth.</param>
		public BucketManager (Auth auth)
		{
			this.auth = auth;
			this.client = new Client ();
		}

		/// <summary>
		/// Buckets this instance.
		/// </summary>
		public string[] Buckets(){
			Response r =  RsGet ("/buckets");
			Console.WriteLine (r.ToString ());
			Console.WriteLine (r.BodyString ());
			return r.JsonToObject<string[]> ();
		}

		private Response RsPost(string path, byte[] body){
			string url = Configuration.RS_HOST + path;
			return Post(url, body);
		}

		private Response RsPost(string path)  {
			return RsPost(path, null);
		}

		private Response RsGet(string path) {
			string url = Configuration.RS_HOST + path;
			return Get(url);
		}

		private Response Get(string url)  {
			StringDict headers = auth.Authorization(url);
			return client.Get(url, headers);
		}

		private Response Post(string url, byte[] body) {
			StringDict headers = auth.Authorization(url, body, Client.FormMime);
			return client.Post(url, body, headers, Client.FormMime);
		}
	}
}

