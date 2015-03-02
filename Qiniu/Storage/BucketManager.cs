using System;
using System.Text;
using System.Collections.Generic;
using Qiniu.Util;
using Qiniu.Storage.Model;
using Qiniu.Http;

using Qiniu;

namespace Qiniu.Storage
{
	public class BucketManager
	{
		private Auth auth;
		private Client client;
		public BucketManager (Auth auth)
		{
			this.auth = auth;
			this.client = new Client ();
		}

		public static string Entry(string bucket, string key) {
			string en = bucket;
			if (key != null) {
				en = bucket + ":" + key;
			}
			return UrlSafeBase64.Encode(en);
		}
		public Response Buckets(){
			return RsGet ("/buckets");
		}

		public Response ListFiles(string bucket, string prefix, string marker, int limit, string delimiter) {
			StringDict dict = new StringDict().Put("bucket", bucket).PutNotEmpty("marker", marker)
				.PutNotEmpty("prefix", prefix).PutNotEmpty("delimiter", delimiter).PutWhen("limit", limit, limit > 0);

			string url = Config.RSF_HOST + "/list?" + dict.FormString();
			return client.Get(url);
		}

		public Response Stat(string bucket, string key) {
			return RsGet("/stat/" + Entry(bucket, key));
		}

		public Response Delete(string bucket, string key){
			return RsPost("/delete/" + Entry(bucket, key));
		}

		public Response Rename(string bucket, string oldname, string newname){
			return Move(bucket, oldname, bucket, newname);
		}

		public Response Copy(string from_bucket, string from_key, string to_bucket, string to_key){
			string from = Entry(from_bucket, from_key);
			string to = Entry(to_bucket, to_key);
			string path = "/copy/" + from + "/" + to;
			return RsPost(path);
		}

		public Response Move(string from_bucket, string from_key, string to_bucket, string to_key) {
			string from = Entry(from_bucket, from_key);
			string to = Entry(to_bucket, to_key);
			string path = "/move/" + from + "/" + to;
			return RsPost(path);
		}

		public Response ChangeMime(string bucket, string key, string mime) {
			string resource = Entry(bucket, key);
			string encode_mime = UrlSafeBase64.Encode(mime);
			string path = "/chgm/" + resource + "/mime/" + encode_mime;
			return RsPost(path);
		}

		public Response Fetch(string url, string bucket, string key) {
			string resource = UrlSafeBase64.Encode(url);
			string to = Entry(bucket, key);
			string path = "/fetch/" + resource + "/to/" + to;
			return IoPost(path);
		}

		public Response Prefetch(string bucket, string key) {
			string resource = Entry(bucket, key);
			string path = "/prefetch/" + resource;
			return IoPost(path);
		}

		public Response BatchRun(Batch operations){
			return RsPost("/batch", operations.toBody());
		}

		private Response RsPost(string path, byte[] body){
			string url = Config.RS_HOST + path;
			return Post(url, body);
		}

		private Response RsPost(string path)  {
			return RsPost(path, null);
		}

		private Response RsGet(string path) {
			string url = Config.RS_HOST + path;
			return Get(url);
		}

		private Response IoPost(string path) {
			string url = Config.IO_HOST + path;
			return Post(url, null);
		}

		private Response Get(string url)  {
			StringDict headers = auth.Authorization(url);
			return client.Get(url, headers);
		}

		private Response Post(string url, byte[] body) {
			StringDict headers = auth.Authorization(url, body, Client.FormMime);
			return client.Post(url, body, headers, Client.FormMime);
		}

		public class Batch {
			private List<string> ops;

			private Batch(List<string> ops) {
				this.ops = ops;
			}

			public static Batch copy(string source_bucket, StringDict key_pairs, string target_bucket) {
				return twoKey("copy", source_bucket, key_pairs, target_bucket);
			}

			public static Batch rename(string bucket, StringDict key_pairs) {
				return move(bucket, key_pairs, bucket);
			}

			public static Batch move(string source_bucket, StringDict key_pairs, string target_bucket) {
				return twoKey("move", source_bucket, key_pairs, target_bucket);
			}

			public static Batch delete(string bucket, string[] keys) {
				return oneKey("delete", bucket, keys);
			}

			public static Batch stat(string bucket, string[] keys) {
				return oneKey("stat", bucket, keys);
			}

			public static Batch oneKey(string operation, string bucket, string[] keys) {
				List<string> ops = new List<string>();

				foreach (string key in keys) {
					ops.Add(operation + "/" + Entry(bucket, key));
				}
				return new Batch(ops);
			}

			public static Batch twoKey(string operation, string source_bucket,
				StringDict key_pairs, String target_bucket) {

				string t_bucket = target_bucket == null ? source_bucket : target_bucket;
				List<string> ops = new List<string>();

//				key_pairs.iterate(new StringMap.Do() {
//					@Override
//					public void deal(String key, Object value) {
//						String from = entry(source_bucket, key);
//						String to = entry(t_bucket, (String) value);
//						ops.add(operation + "/" + from + "/" + to);
//					}
//				});

				return new Batch(ops);
			}

			public Batch merge(Batch batch) {
				ops.AddRange(batch.ops);
				return this;
			}

			public byte[] toBody() {
				// ops is url safe,
				string body = "&op=" + String.Join ("op=", ops.ToArray());
				return Encoding.UTF8.GetBytes (body);
			}
		}
	}
}

