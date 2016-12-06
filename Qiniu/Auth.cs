using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Newtonsoft.Json;

using Qiniu.Util;
using Qiniu.Http;

namespace Qiniu
{
	/// <summary>
	/// Auth. 认证签名类.
	/// </summary>
	public class Auth
	{
		private string accessKey;
		private byte[] secretKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Auth"/> class.
		/// </summary>
		/// <param name="accessKey">Access key.</param>
		/// <param name="secretKey">Secret key.</param>
		public Auth (string accessKey, string secretKey)
		{
			this.accessKey = accessKey;
			this.secretKey = Encoding.UTF8.GetBytes (secretKey);
		}

		/// <summary>
		/// Sign the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		public string Sign (byte[] data)
		{
			HMACSHA1 hmac = new HMACSHA1 (secretKey);
			byte[] digest = hmac.ComputeHash (data);
			string sign =  UrlSafeBase64.Encode (digest);
			return string.Format ("{0}:{1}", this.accessKey, sign);
		}

		/// <summary>
		/// Sign the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		public string Sign (string data)
		{
			return Sign (Encoding.UTF8.GetBytes (data));
		}

		/// <summary>
		/// Signs the with data.
		/// </summary>
		/// <returns>The signed with data.</returns>
		/// <param name="data">Data.</param>
		public string SignWithData (byte[] data)
		{
			string s = UrlSafeBase64.Encode (data);
			return string.Format ("{0}:{1}", Sign(s), s);
		}

		/// <summary>
		/// Signs the with data.
		/// </summary>
		/// <returns>The with data.</returns>
		/// <param name="data">Data.</param>
		public string SignWithData (string data)
		{
			return SignWithData(Encoding.UTF8.GetBytes(data));
		}

		/// <summary>
		/// Signs the request.
		/// </summary>
		/// <returns>The request.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="body">Body.</param>
		/// <param name="contentType">Content type.</param>
		public string SignRequest (Uri uri, byte[] body, string contentType)
		{
			byte[] pathAndQueryBytes = Encoding.UTF8.GetBytes (uri.PathAndQuery);
			using (MemoryStream buffer = new MemoryStream()) {
				buffer.Write (pathAndQueryBytes, 0, pathAndQueryBytes.Length);
				buffer.WriteByte ((byte)'\n');
				if (body != null && body.Length > 0 && Client.FormMime.Equals(contentType)) {
					buffer.Write (body, 0, body.Length);
				}
				return Sign (buffer.ToArray());
			}
		}

		/// <summary>
		/// Signs the request.
		/// </summary>
		/// <returns>The request.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="body">Body.</param>
		/// <param name="contentType">Content type.</param>
		public string SignRequest (string uri, byte[] body, string contentType)
		{
			return SignRequest(new Uri(uri), body, contentType);
		}

		/// <summary>
		/// Determines whether this instance is valid callback the specified originAuthorization uri body contentType.
		/// </summary>
		/// <returns><c>true</c> if this instance is valid callback the specified originAuthorization uri body contentType; otherwise, <c>false</c>.</returns>
		/// <param name="originAuthorization">Origin authorization.</param>
		/// <param name="uri">URI.</param>
		/// <param name="body">Body.</param>
		/// <param name="contentType">Content type.</param>
		public bool IsValidCallback(string originAuthorization, Uri uri , byte[] body, string contentType) {
			String authorization = "QBox " + SignRequest(uri, body, contentType);
			return authorization.Equals(originAuthorization);
		}

		/// <summary>
		/// Privates the download URL.
		/// </summary>
		/// <returns>The download URL.</returns>
		/// <param name="baseUrl">Base URL.</param>
		public string PrivateDownloadUrl(string baseUrl) {
			return PrivateDownloadUrl(baseUrl, 3600);
		}

		/// <summary>
		/// Privates the download URL.
		/// </summary>
		/// <returns>The download URL.</returns>
		/// <param name="baseUrl">Base URL.</param>
		/// <param name="expires">Expires.</param>
		public string PrivateDownloadUrl(string baseUrl, long expires) {
			return PrivateDownloadUrlWithDeadline(baseUrl, Deadline(expires));
		}

		private string PrivateDownloadUrlWithDeadline(string baseUrl, long deadline) {
			StringBuilder b = new StringBuilder();
			b.Append(baseUrl);
			int pos = baseUrl.IndexOf("?");
			if (pos > 0) {
				b.Append("&e=");
			} else {
				b.Append("?e=");
			}
			b.Append(deadline);

			String token = Sign(b.ToString());
			b.Append("&token=");
			b.Append(token);
			return b.ToString();
		}

		/// <summary>
		/// Uploads the token.
		/// </summary>
		/// <returns>The token.</returns>
		/// <param name="bucket">Bucket.</param>
		public string UploadToken(string bucket) {
			return UploadToken(bucket, null, 3600, null, true);
		}

		/// <summary>
		/// Uploads the token.
		/// </summary>
		/// <returns>The token.</returns>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		public string UploadToken(string bucket, String key) {
			return UploadToken(bucket, key, 3600, null, true);
		}

		/// <summary>
		/// Uploads the token.
		/// </summary>
		/// <returns>The token.</returns>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		/// <param name="expires">Expires.</param>
		/// <param name="policy">Policy.</param>
		public string UploadToken(String bucket, String key, long expires, StringDict policy) {
			return UploadToken(bucket, key, expires, policy, true);
		}

		/// <summary>
		/// Uploads the token.
		/// </summary>
		/// <returns>The token.</returns>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		/// <param name="expires">Expires.</param>
		/// <param name="policy">Policy.</param>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		public String UploadToken(String bucket, String key, long expires, StringDict policy, bool strict) {
			return UploadTokenWithDeadline(bucket, key, Deadline(expires), policy, strict);
		}

		private String UploadTokenWithDeadline(String bucket, String key, long deadline, StringDict policy, bool strict) {
			String scope = bucket;
			if (key != null) {
				scope = bucket + ":" + key;
			}
			StringDict x = new StringDict();
			CopyPolicy(x, policy, strict);
			x.Put("scope", scope).Put("deadline", deadline);

			return SignWithData(Encoding.UTF8.GetBytes(x.ToJson()));
		}

		/// <summary>
		/// Authorization the specified url, body and contentType.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="body">Body.</param>
		/// <param name="contentType">Content type.</param>
		public StringDict Authorization(string url, byte[] body, string contentType) {
			string authorization = "QBox " + SignRequest(new Uri(url), body, contentType);
			return new StringDict().Put("Authorization", authorization);
		}

		/// <summary>
		/// Authorization the specified url.
		/// </summary>
		/// <param name="url">URL.</param>
		public StringDict Authorization(String url) {
			return Authorization(url, null, null);
		}

		/// <summary>
		/// Clock. Only for test.
		/// </summary>
		public delegate long Clock(); 

		/// <summary>
		/// Sets the clock. Only for test
		/// </summary>
		/// <returns>the Auth instance.</returns>
		/// <param name="c">C.</param>
		public Auth SetClock(Clock c){
			clock = c;
			return this;
		}

		private Clock clock = delegate(){
			return (DateTime.Now.ToUniversalTime ().Ticks - 621355968000000000) / 10000000;
		};

		private long Deadline(long expires)
		{
			return clock() + expires;
		}

		private static void CopyPolicy(StringDict target, StringDict soruce, bool strict)
		{
			if (soruce == null) {
				return;
			}
			soruce.ForEach (delegate(string key, object val) {
				if (StringUtil.ArrayExists(deprecatedPolicyFields, key)){
					throw new ArgumentException (key + " is deprecated!");
				}

				if (!strict || StringUtil.ArrayExists (policyFields, key)) {
					target.Put (key, val);
				}
			});
		}

		private static string[] policyFields = {
			"callbackUrl",
			"callbackBody",
			"callbackHost",
			"callbackBodyType",
			"callbackFetchKey",

			"returnUrl",
			"returnBody",

			"endUser",
			"saveKey",
			"insertOnly",

			"detectMime",
			"mimeLimit",
			"fsizeLimit",

			"persistentOps",
			"persistentNotifyUrl",
			"persistentPipeline",
		};

		private static string[] deprecatedPolicyFields = {
			"asyncOps",
		};
	}
}
