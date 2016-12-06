using System;
using System.Net;
using Qiniu.Util;
using System.Text;

namespace Qiniu.Http
{
	/// <summary>
	/// Response.
	/// </summary>
	public class Response
	{
		/// <summary>
		/// The invalid argument.
		/// </summary>
		public const int InvalidArgument = -4;
		/// <summary>
		/// The invalid file.
		/// </summary>
		public const int InvalidFile = -3;
		/// <summary>
		/// The cancelled.
		/// </summary>
		public const int Cancelled = -2;
		/// <summary>
		/// The network error.
		/// </summary>
		public const int NetworkError = -1;


	/// <summary>
	/// 状态码.
	/// </summary>
	/// <value>The status code.</value>
		public int StatusCode {
			get;
		}

		/// <summary>
		/// Gets the req identifier.
		/// </summary>
		/// <value>The req identifier.</value>
		public string ReqId {
			get;
		}

		/// <summary>
		/// Gets the xlog.
		/// </summary>
		/// <value>The xlog.</value>
		public string Xlog {
			get;
		}

		/// <summary>
		/// Gets the xvia.
		/// </summary>
		/// <value>The xvia.</value>
		public string Xvia {
			get;
		}

		/// <summary>
		/// Gets the error.
		/// </summary>
		/// <value>The error.</value>
		public string Error {
			get;
		}

		/// <summary>
		/// Gets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public long Duration {
			get;
		}

		public string ContentType {
			get;
		}

		private byte[] body;
		private HttpWebResponse response;


		internal Response (int code, string error, string reqId, string xlog, string xvia, long duration, HttpWebResponse resp, byte[] body, string ctype)
		{
			this.StatusCode = code;
			this.Error = error;
			this.Duration = duration;
			this.ReqId = reqId;
			this.Xlog = xlog;
			this.Xvia = xvia;
			this.response = resp;
			this.body = body;
			this.ContentType = ctype;
			if (StatusCode / 2 != 100 && Client.JsonMime.Equals(ctype)) {
				JsonError err = Json.Decode<JsonError> (Encoding.UTF8.GetString (body));
				Error = err.error;
			}
		}

		internal static Response CreateBy(string error, long duration)
		{
			return new Response (NetworkError, error, "", "", "", duration, null, null, null);
		}

		/// <summary>
		/// Determines whether this instance is O.
		/// </summary>
		/// <returns><c>true</c> if this instance is O; otherwise, <c>false</c>.</returns>
		public bool IsOK() {
			return StatusCode == 200 && Error == null && ReqId != null && ReqId.Length > 0;
		}

		/// <summary>
		/// Determines whether this instance is network broken.
		/// </summary>
		/// <returns><c>true</c> if this instance is network broken; otherwise, <c>false</c>.</returns>
		public bool IsNetworkBroken() {
			return StatusCode == NetworkError;
		}

		/// <summary>
		/// Determines whether this instance is server error.
		/// </summary>
		/// <returns><c>true</c> if this instance is server error; otherwise, <c>false</c>.</returns>
		public bool IsServerError() {
			return (StatusCode >= 500 && StatusCode < 600 && StatusCode != 579) || StatusCode == 996;
		}

		/// <summary>
		/// Needs the switch server.
		/// </summary>
		/// <returns><c>true</c>, if switch server was needed, <c>false</c> otherwise.</returns>
		public bool NeedSwitchServer() {
			return IsNetworkBroken() || (StatusCode >= 500 && StatusCode < 600 && StatusCode != 579);
		}

		/// <summary>
		/// Needs the retry.
		/// </summary>
		/// <returns><c>true</c>, if retry was needed, <c>false</c> otherwise.</returns>
		public bool NeedRetry() {
			return IsNetworkBroken() || IsServerError() || StatusCode == 406 || (StatusCode == 200 && Error != null);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Qiniu.Http.Response"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Qiniu.Http.Response"/>.</returns>
		public override string ToString() {
//			return string.Format("{ResponseInfo:version:{0}, status:{1}, reqId:{2}, xlog:{3}, xvia:{4}, duration:{5} s, error:{6}}",
//				Configuration.VERSION, StatusCode, ReqId, Xlog, Xvia, Duration, Error);
			return string.Format("ResponseInfo:version:{0}, status:{1}, reqId:{2}, xlog:{3}, xvia:{4}, duration:{5} s, error:{6}",
				Configuration.VERSION, StatusCode, ReqId, Xlog, Xvia, Duration, Error);
		}

		/// <summary>
		/// Jsons to object.
		/// </summary>
		/// <returns>The to object.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T JsonToObject<T>() {
			CheckArror ();
			if (!IsJson()) {
				throw new FormatException();
			}
			string b = BodyString();
			return Json.Decode<T>(b);
		}
//
//		public StringMap jsonToMap() throws QiniuException {
//			if (!isJson()) {
//				return null;
//			}
//			String b = bodyString();
//			return Json.decode(b);
//		}
//
//		public synchronized byte[] body() throws QiniuException {
//			if (body != null) {
//				return body;
//			}
//			try {
//				this.body = response.body().bytes();
//			} catch (IOException e) {
//				throw new QiniuException(e);
//			}
//			return body;
//		}
//
		/// <summary>
		/// Bodies the string.
		/// </summary>
		/// <returns>The string.</returns>
		public string BodyString(){
			return Encoding.UTF8.GetString(body);
		}

		/// <summary>
		/// Determines whether this instance is json.
		/// </summary>
		/// <returns><c>true</c> if this instance is json; otherwise, <c>false</c>.</returns>
		public bool IsJson() {
			return ContentType.Equals(Client.JsonMime);
		}

		/// <summary>
		/// URL this instance.
		/// </summary>
		public string url() {
			return response.ResponseUri.AbsoluteUri;
		}

		private string ToLog(){
			return string.Format("{0},{1},{2},{3},{4}", StatusCode, ReqId, Xvia, Error, Duration);
		}

		internal void Log(ILogger logger){
			string s = ToLog ();
			if (StatusCode / 100 == 2) {
				logger.Info (s);
			} else if (StatusCode / 100 == 4) {
				logger.Warn (s);
			} else {
				logger.Error (s);
			}
		}

		/// <summary>
		/// Checks the arror, only for 
		/// </summary>
		public void CheckArror(){
			if (Error != null) {
				throw new QiniuException (StatusCode, ToString());
			}
		}

		public struct JsonError{
			public string error;
		}
	}
}
