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
	/// Object manager.
	/// </summary>
	public class ObjectManager
	{
		private Auth auth;
		private Client client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Storage.ObjectManager"/> class.
		/// </summary>
		/// <param name="auth">Auth.</param>
		/// <param name="config">Config.</param>
		public ObjectManager (Auth auth, Configuration config)
		{
			this.auth = auth;
			this.client = new Client ();
		}

		/// <summary>
		/// Entry the specified bucket and key.
		/// </summary>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		public static string Entry(string bucket, string key) {
			string en = bucket;
			if (key != null) {
				en = bucket + ":" + key;
			}
			return UrlSafeBase64.Encode(en);
		}

		/// <summary>
		/// Lists the files.
		/// </summary>
		/// <returns>The files.</returns>
		/// <param name="bucket">Bucket.</param>
		/// <param name="prefix">Prefix.</param>
		/// <param name="marker">Marker.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="delimiter">Delimiter.</param>
		public FileListing ListFiles(string bucket, string prefix, string marker, int limit, string delimiter) {
			StringDict dict = new StringDict().Put("bucket", bucket).PutNotEmpty("marker", marker)
				.PutNotEmpty("prefix", prefix).PutNotEmpty("delimiter", delimiter).PutWhen("limit", limit, limit > 0);

			string url = Configuration.RSF_HOST + "/list?" + dict.FormString();
			Response r =  client.Get(url);
			return r.JsonToObject<FileListing> ();
		}

		/// <summary>
		/// Stat the specified bucket and key.
		/// </summary>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		public FileInfo Stat(string bucket, string key) {
			Response r = RsGet("/stat/" + Entry(bucket, key));
			return r.JsonToObject<FileInfo> ();
		}

		/// <summary>
		/// Delete the specified bucket and key.
		/// </summary>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		public void Delete(string bucket, string key){
			RsPost("/delete/" + Entry(bucket, key));
		}

		/// <summary>
		/// Rename the specified bucket, oldname and newname.
		/// </summary>
		/// <param name="bucket">Bucket.</param>
		/// <param name="oldname">Oldname.</param>
		/// <param name="newname">Newname.</param>
		public void Rename(string bucket, string oldname, string newname){
			Move(bucket, oldname, bucket, newname);
		}

		/// <summary>
		/// Copy the specified from_bucket, from_key, to_bucket and to_key.
		/// </summary>
		/// <param name="from_bucket">From bucket.</param>
		/// <param name="from_key">From key.</param>
		/// <param name="to_bucket">To bucket.</param>
		/// <param name="to_key">To key.</param>
		public void Copy(string from_bucket, string from_key, string to_bucket, string to_key){
			string from = Entry(from_bucket, from_key);
			string to = Entry(to_bucket, to_key);
			string path = "/copy/" + from + "/" + to;
			RsPost(path);
		}

		/// <summary>
		/// Move the specified from_bucket, from_key, to_bucket and to_key.
		/// </summary>
		/// <param name="from_bucket">From bucket.</param>
		/// <param name="from_key">From key.</param>
		/// <param name="to_bucket">To bucket.</param>
		/// <param name="to_key">To key.</param>
		public void Move(string from_bucket, string from_key, string to_bucket, string to_key) {
			string from = Entry(from_bucket, from_key);
			string to = Entry(to_bucket, to_key);
			string path = "/move/" + from + "/" + to;
			RsPost(path);
		}

		/// <summary>
		/// Changes the MIME.
		/// </summary>
		/// <returns>The MIME.</returns>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		/// <param name="mime">MIME.</param>
		public void ChangeMime(string bucket, string key, string mime) {
			string resource = Entry(bucket, key);
			string encode_mime = UrlSafeBase64.Encode(mime);
			string path = "/chgm/" + resource + "/mime/" + encode_mime;
			RsPost(path);

		}

		/// <summary>
		/// Fetch the specified url, bucket and key.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		public FileInfo Fetch(string url, string bucket, string key) {
			string resource = UrlSafeBase64.Encode(url);
			string to = Entry(bucket, key);
			string path = "/fetch/" + resource + "/to/" + to;
			Response r =  IoPost(path);
			return r.JsonToObject<FileInfo> ();
		}

		/// <summary>
		/// Prefetch the specified bucket and key.
		/// </summary>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		public void Prefetch(string bucket, string key) {
			string resource = Entry(bucket, key);
			string path = "/prefetch/" + resource;
			IoPost(path);
		}

		/// <summary>
		/// Batchs the run.
		/// </summary>
		/// <returns>The run.</returns>
		/// <param name="operations">Operations.</param>
		public Response BatchRun(Batch operations){
			return RsPost("/batch", operations.ToBody());
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

		private Response IoPost(string path) {
			string url = Configuration.IO_HOST + path;
			return Post(url, null);
		}

		private Response Get(string url)  {
			StringDict headers = auth.Authorization(url);
			Response r =  client.Get(url, headers);
			r.CheckArror ();
			return r;
		}

		private Response Post(string url, byte[] body) {
			StringDict headers = auth.Authorization(url, body, Client.FormMime);
			Response r =  client.Post(url, body, headers, Client.FormMime);
			r.CheckArror ();
			return r;
		}

		/// <summary>
		/// Batch.
		/// </summary>
		public class Batch {
			private List<string> ops;

			private Batch(List<string> ops) {
				this.ops = ops;
			}

			/// <summary>
			/// Copy the specified source_bucket, key_pairs and target_bucket.
			/// </summary>
			/// <param name="source_bucket">Source bucket.</param>
			/// <param name="key_pairs">Key pairs.</param>
			/// <param name="target_bucket">Target bucket.</param>
			public static Batch Copy(string source_bucket, StringDict key_pairs, string target_bucket) {
				return TwoKey("copy", source_bucket, key_pairs, target_bucket);
			}

			/// <summary>
			/// Rename the specified bucket and key_pairs.
			/// </summary>
			/// <param name="bucket">Bucket.</param>
			/// <param name="key_pairs">Key pairs.</param>
			public static Batch Rename(string bucket, StringDict key_pairs) {
				return Move(bucket, key_pairs, bucket);
			}

			/// <summary>
			/// Move the specified source_bucket, key_pairs and target_bucket.
			/// </summary>
			/// <param name="source_bucket">Source bucket.</param>
			/// <param name="key_pairs">Key pairs.</param>
			/// <param name="target_bucket">Target bucket.</param>
			public static Batch Move(string source_bucket, StringDict key_pairs, string target_bucket) {
				return TwoKey("move", source_bucket, key_pairs, target_bucket);
			}

			/// <summary>
			/// Delete the specified bucket and keys.
			/// </summary>
			/// <param name="bucket">Bucket.</param>
			/// <param name="keys">Keys.</param>
			public static Batch Delete(string bucket, string[] keys) {
				return OneKey("delete", bucket, keys);
			}

			/// <summary>
			/// Stat the specified bucket and keys.
			/// </summary>
			/// <param name="bucket">Bucket.</param>
			/// <param name="keys">Keys.</param>
			public static Batch Stat(string bucket, string[] keys) {
				return OneKey("stat", bucket, keys);
			}

			/// <summary>
			/// Ones the key.
			/// </summary>
			/// <returns>The key.</returns>
			/// <param name="operation">Operation.</param>
			/// <param name="bucket">Bucket.</param>
			/// <param name="keys">Keys.</param>
			public static Batch OneKey(string operation, string bucket, string[] keys) {
				List<string> ops = new List<string>();

				foreach (string key in keys) {
					ops.Add(operation + "/" + Entry(bucket, key));
				}
				return new Batch(ops);
			}

			/// <summary>
			/// Twos the key.
			/// </summary>
			/// <returns>The key.</returns>
			/// <param name="operation">Operation.</param>
			/// <param name="source_bucket">Source bucket.</param>
			/// <param name="key_pairs">Key pairs.</param>
			/// <param name="target_bucket">Target bucket.</param>
			public static Batch TwoKey(string operation, string source_bucket,
				StringDict key_pairs, String target_bucket) {

				string t_bucket = target_bucket == null ? source_bucket : target_bucket;
				List<string> ops = new List<string>();

				key_pairs.ForEach (delegate(string key, object value) {
					string src = Entry(source_bucket, key);
					string target = Entry(t_bucket, (string) value);
					ops.Add(operation + "/" + src + "/" + target);
				}); 
				return new Batch(ops);
			}

			/// <summary>
			/// Merge the specified batch.
			/// </summary>
			/// <param name="batch">Batch.</param>
			public Batch Merge(Batch batch) {
				ops.AddRange(batch.ops);
				return this;
			}

			/// <summary>
			/// Tos the body.
			/// </summary>
			/// <returns>The body.</returns>
			public byte[] ToBody() {
				// ops is url safe,
				string body = "&op=" + String.Join ("op=", ops.ToArray());
				return Encoding.UTF8.GetBytes (body);
			}
		}

		/**
     * 获取文件列表迭代器
     */
		public class FileListIterator: Iterator<FileInfo[]> {
			private String marker = null;
			private String bucket;
			private String delimiter;
			private int limit;
			private String prefix;
			private QiniuException exception = null;

			public FileListIterator(String bucket, String prefix, int limit, String delimiter) {
				if (limit <= 0) {
					throw new IllegalArgumentException("limit must great than 0");
				}
				this.bucket = bucket;
				this.prefix = prefix;
				this.limit = limit;
				this.delimiter = delimiter;
			}

			public QiniuException error() {
				return exception;
			}

			public boolean hasNext() {
				return exception == null && !"".equals(marker);
			}

			public FileInfo[] next() {
				try {
					FileListing f = listFiles(bucket, prefix, marker, limit, delimiter);
					this.marker = f.marker == null ? "" : f.marker;
					return f.items;
				} catch (QiniuException e) {
					this.exception = e;
					return null;
				}
			}

			public void remove() {
				throw new UnsupportedOperationException("remove");
			}
		}
	}
}
