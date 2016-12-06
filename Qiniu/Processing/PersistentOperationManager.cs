using System;
using System.Text;
using Qiniu;
using Qiniu.Http;
using Qiniu.Util;

namespace Qiniu.Processing
{
	/// <summary>
	/// Persistent operation manager.
	/// </summary>
	public class PersistentOperationManager
	{
		private Auth auth;
		private String bucket;
		private String pipeline;
		private String notifyUrl;
		private bool force;
		private Client client;
		private Configuration config;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.PersistentOperationManager"/> class.
		/// </summary>
		/// <param name="auth">Auth.</param>
		/// <param name="bucket">Bucket.</param>
		/// <param name="pipeline">Pipeline.</param>
		/// <param name="notifyUrl">Notify URL.</param>
		/// <param name="force">If set to <c>true</c> force.</param>
		/// <param name="config">Config.</param>
		public PersistentOperationManager(Auth auth, string bucket, string pipeline, string notifyUrl, bool force, Configuration config) {
			this.auth = auth;
			this.bucket = bucket;
			this.pipeline = pipeline;
			this.notifyUrl = notifyUrl;
			this.force = force;
			this.client = new Client();
			this.config = config;
		}

		/// <summary>
		/// Post the specified key and cmd.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="cmd">Cmd.</param>
		public Response Post(string key, Operation cmd) {
			return Post(key, new Pipe().Append(cmd));
		}

		/// <summary>
		/// Post the specified key and pipe.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="pipe">Pipe.</param>
		public Response Post(string key, Pipe pipe) {
			Pipe[] pipes = {pipe};
			return Post(key, pipes);
		}

		/// <summary>
		/// Post the specified key and pipe.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="pipe">Pipe.</param>
		public Response Post(string key, Pipe[] pipe) {
			 String fops = ""; //String.Join(";", (Object[])pipe);
			StringDict dict = new StringDict().Put("bucket", bucket).Put("key", key).Put("fops", fops)
				.PutNotEmpty("pipeline", pipeline).PutNotEmpty("notifyURL", notifyUrl).PutWhen("force", 1, force);

			byte[] data = Encoding.UTF8.GetBytes(dict.FormString());
			String url = Configuration.API_HOST + "/pfop/";
			StringDict headers = auth.Authorization(url, data, Client.FormMime);
			Response response = client.Post(url, data, headers, Client.FormMime);
			return response;
//			PfopStatus status = response.jsonToObject(PfopStatus.class);
//			return status.persistentId;
		}

		/// <summary>
		/// Status the specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public Response Status(string id) {
			//id is url safe
			string url = Configuration.API_HOST + "/status/get/prefop?id=" + id;
			Response response = client.Get(url);
			return response;
		}
	}
}

