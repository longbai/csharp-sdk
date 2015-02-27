using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Qiniu.Util;
using Qiniu.Http;

namespace Qiniu
{
	public class Auth
	{
		private string accessKey;
		private byte[] secretKey;
		private Auth (string accessKey, string secretKey)
		{
			this.accessKey = accessKey;
			this.secretKey = Encoding.UTF8.GetBytes (secretKey);
		}

		public string Sign (byte[] data)
		{
			HMACSHA1 hmac = new HMACSHA1 (secretKey);
			byte[] digest = hmac.ComputeHash (data);
			string sign =  UrlSafeBase64.Encode (digest);
			return string.Format ("{0}:{1}", this.accessKey, sign);
		}

		public string Sign (string data)
		{
			return Sign (Encoding.UTF8.GetBytes (data));
		}

		public string SignWithData (byte[] data)
		{
			string s = UrlSafeBase64.Encode (data);
			return string.Format ("{0}:{1}", Sign(s), s);
		}

		public string SignRequest (Uri uri, byte[] body, string contentType)
		{

			byte[] pathAndQueryBytes = Encoding.UTF8.GetBytes (uri.PathAndQuery);
			using (MemoryStream buffer = new MemoryStream()) {
				buffer.Write (pathAndQueryBytes, 0, pathAndQueryBytes.Length);
				buffer.WriteByte ((byte)'\n');
				if (body != null && body.Length > 0 && !String.IsNullOrEmpty(contentType) &&
					(contentType.Equals(Client.FormMime) || contentType.Equals(Client.JsonMime))) {
					buffer.Write (body, 0, body.Length);
				}
				return Sign (buffer.ToArray());
			}
		}

		public bool IsValidCallback(string originAuthorization, Uri uri , byte[] body, string contentType) {
			String authorization = "QBox " + SignRequest(uri, body, contentType);
			return authorization.Equals(originAuthorization);
		}

		public string PrivateDownloadUrl(string baseUrl) {
			return PrivateDownloadUrl(baseUrl, 3600);
		}

		public string PrivateDownloadUrl(string baseUrl, long expires) {
			return PrivateDownloadUrlWithDeadline(baseUrl, Deadline(expires));
		}

		string PrivateDownloadUrlWithDeadline(string baseUrl, long deadline) {
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

		public string UploadToken(string bucket) {
			return UploadToken(bucket, null, 3600, null, true);
		}

		public string UploadToken(string bucket, String key) {
			return UploadToken(bucket, key, 3600, null, true);
		}

		public string UploadToken(String bucket, String key, long expires, StringDict policy) {
			return UploadToken(bucket, key, expires, policy, true);
		}

		public String UploadToken(String bucket, String key, long expires, StringDict policy, bool strict) {
			return UploadTokenWithDeadline(bucket, key, Deadline(expires), policy, strict);
		}

		String UploadTokenWithDeadline(String bucket, String key, long deadline, StringDict policy, bool strict) {
			String scope = bucket;
			if (key != null) {
				scope = bucket + ":" + key;
			}
			StringDict x = new StringDict();
			CopyPolicy(x, policy, strict);
			x.Put("scope", scope).Put("deadline", deadline);

//			String s = Json.Encode(x);
			return SignWithData(Encoding.UTF8.GetBytes(""));
		}

		public StringDict Authorization(string url, byte[] body, string contentType) {
			string authorization = "QBox " + SignRequest(new Uri(url), body, contentType);
			return new StringDict().Put("Authorization", authorization);
		}

		public StringDict Authorization(String url) {
			return Authorization(url, null, null);
		}

		private long Deadline(long expires)
		{
			return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000 + expires;
		}

		private static void CopyPolicy(StringDict target, StringDict soruce, bool strict)
		{
//				if (originPolicy == null) {
//					return;
//				}
//				originPolicy.iterate(new StringMap.Do() {
//					@Override
//					public void deal(String key, Object value) {
//						if (StringUtils.inStringArray(key, deprecatedPolicyFields)) {
//							throw new IllegalArgumentException(key + " is deprecated!");
//						}
//						if (!strict || StringUtils.inStringArray(key, policyFields)) {
//							policy.put(key, value);
//						}
//					}
//				});
//			}
		}
	}
}
