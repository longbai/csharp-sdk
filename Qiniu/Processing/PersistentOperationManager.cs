using System;
using System.Text;
using Qiniu;
using Qiniu.Http;
using Qiniu.Util;

namespace Qiniu.Processing
{
	public class PersistentOperationManager
	{
		private Auth auth;
		private String bucket;
		private String pipeline;
		private String notifyUrl;
		private bool force;
		private Client client;

		public PersistentOperationManager(Auth auth, string bucket, string pipeline, string notifyUrl, bool force) {
			this.auth = auth;
			this.bucket = bucket;
			this.pipeline = pipeline;
			this.notifyUrl = notifyUrl;
			this.force = force;
			this.client = new Client();
		}

		public Response Post(string key, Operation cmd) {
			return Post(key, new Pipe().Append(cmd));
		}

		public Response Post(string key, Pipe pipe) {
			Pipe[] pipes = {pipe};
			return Post(key, pipes);
		}

		public Response Post(string key, Pipe[] pipe) {
			 String fops = ""; //String.Join(";", (Object[])pipe);
			StringDict dict = new StringDict().Put("bucket", bucket).Put("key", key).Put("fops", fops)
				.PutNotEmpty("pipeline", pipeline).PutNotEmpty("notifyURL", notifyUrl).PutWhen("force", 1, force);

			byte[] data = Encoding.UTF8.GetBytes(dict.FormString());
			String url = Config.API_HOST + "/pfop/";
			StringDict headers = auth.Authorization(url, data, Client.FormMime);
			Response response = client.Post(url, data, headers, Client.FormMime);
			return response;
//			PfopStatus status = response.jsonToObject(PfopStatus.class);
//			return status.persistentId;
		}

		public Response Status(string id) {
			//id is url safe
			string url = Config.API_HOST + "/status/get/prefop?id=" + id;
			Response response = client.Get(url);
			return response;
		}
	}
}

