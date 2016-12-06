using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;

using Qiniu.Util;
using Qiniu;

namespace Qiniu.Http
{
	/// <summary>
	/// Client.
	/// </summary>
	public class Client
	{
		/// <summary>
		/// The content type header.
		/// </summary>
		public const string ContentTypeHeader = "Content-Type";
		/// <summary>
		/// The default MIME.
		/// </summary>
		public const string DefaultMime = "application/octet-stream";
		/// <summary>
		/// The json MIME.
		/// </summary>
		public const string JsonMime = "application/json";
		/// <summary>
		/// The form MIME.
		/// </summary>
		public const string FormMime = "application/x-www-form-urlencoded";

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Http.Client"/> class.
		/// </summary>
		public Client ():this(Configuration.DEFAULT_CONNECT_TIMEOUT, Configuration.DEFAULT_RESPONSE_TIMEOUT, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Http.Client"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		public Client (ILogger logger):this(Configuration.DEFAULT_CONNECT_TIMEOUT, Configuration.DEFAULT_RESPONSE_TIMEOUT, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Http.Client"/> class.
		/// </summary>
		public Client (int connectTimeout, int responseTimeout, ILogger logger)
		{
			this.connectTimeout = connectTimeout;
			this.responseTimeout = responseTimeout;
			this.logger = logger == null ? new DummyLogger () : logger;
		}

		private HttpWebRequest CreateRequest(string url, StringDict headers){
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.Timeout = connectTimeout;
			request.ReadWriteTimeout = responseTimeout;
			request.UserAgent = UserAgent();
			if (headers != null) {
				headers.ForEach(delegate (string k, Object val){
					request.Headers.Set(k, val.ToString());
				});
			}

			return request;
		}

		/// <summary>
		/// Get the specified url.
		/// </summary>
		/// <param name="url">URL.</param>
		public Response Get(string url)
		{
			return Get (url, new StringDict ());
		}

		/// <summary>
		/// Get the specified url and headers.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="headers">Headers.</param>
		public Response Get(string url, StringDict headers)
		{
			HttpWebRequest req = CreateRequest (url, headers);
			req.Method = "GET";
			Elapse elapsed = new Elapse ();
			try {
				using (HttpWebResponse response = req.GetResponse() as HttpWebResponse) {
					long end = DateTime.Now.ToUniversalTime ().Ticks;
					return HandleResponse (response, elapsed.Duration());
				}
			} catch (WebException e) {
				HttpWebResponse response = e.Response as HttpWebResponse;
				if (response != null) {
					return HandleResponse (response, elapsed.Duration());
				}
				return Response.CreateBy(e.ToString(), elapsed.Duration());
			} catch (Exception e) {
				return Response.CreateBy(e.ToString(), elapsed.Duration());
			}
		}

		/// <summary>
		/// Post the specified url, body and headers.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="body">Body.</param>
		/// <param name="headers">Headers.</param>
		public Response Post(string url, byte[] body, StringDict headers) 
		{
			return Post(url, body, headers, DefaultMime);
		}

		/// <summary>
		/// Post the specified url, body and headers.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="body">Body.</param>
		/// <param name="headers">Headers.</param>
		public Response Post(string url, string body, StringDict headers) 
		{
			return Post(url, Encoding.UTF8.GetBytes(body), headers, DefaultMime);
		}

		/// <summary>
		/// Post the specified url, body, headers and contentType.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="body">Body.</param>
		/// <param name="headers">Headers.</param>
		/// <param name="contentType">Content type.</param>
		public Response Post(string url, byte[] body, StringDict headers, string contentType) 
		{
			HttpWebRequest request = CreateRequest (url, headers);
			request.Method = "POST";
			request.ContentType = contentType;
			request.ContentLength = body.Length;
			Elapse elapsed = new Elapse ();
			using (Stream requestStream = request.GetRequestStream()) {
				requestStream.Write(body, 0, body.Length);
			}

			return Send (request, elapsed);
		}

		private static string UserAgent()
		{
			return "QiniuCSharp/"+ Configuration.VERSION + " (" + Environment.OSVersion.Version.ToString() + "; )";
		}

		private Response HandleResponse(HttpWebResponse response, long duration)
		{
			string via = Via (response);
			string reqId = response.GetResponseHeader ("X-Reqid");
			string xlog = response.GetResponseHeader ("X-Log");
			string contentType = response.ContentType;
			byte[] body = null; 
			if (response.ContentLength < 1024 * 10) {
				body = new byte[response.ContentLength];
				using (Stream stream = response.GetResponseStream()) {
					stream.Read (body, 0, body.Length);
				}
			}
			return new Response ((int)response.StatusCode, null, reqId, xlog, via, duration, response, body, contentType);
		}

		private static string MultiBoundary ()
		{
			return String.Format ("----------{0:N}--", Guid.NewGuid ());
		}

		private static string MultiContentType (string boundary)
		{
			return "multipart/form-data; boundary=" + boundary;
		}

		private static Stream BuildPostStream (Stream putStream, string fileName, NameValueCollection formData, 
			string boundary)
		{
			Stream postDataStream = new MemoryStream ();
			// form data
			string formDataHeaderTemplate =  "\r\n--" + boundary + 
				"\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n;{1}";

			foreach (string key in formData.Keys) {
				byte[] formItemBytes = Encoding.UTF8.GetBytes (string.Format (formDataHeaderTemplate,
					key, formData [key]));
				postDataStream.Write (formItemBytes, 0, formItemBytes.Length);
			}

			//adding file,Stream data
			#region adding file data

			string fileHeaderTemplate = "\r\n--" + boundary + 
				"\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
				"\r\nContent-Type: application/octet-stream\r\n\r\n";
			byte[] fileHeaderBytes = Encoding.UTF8.GetBytes (string.Format (fileHeaderTemplate,
				"file", fileName));
			postDataStream.Write (fileHeaderBytes, 0, fileHeaderBytes.Length);

			byte[] buffer = new byte[64*1024];
			int bytesRead = 0;
			while ((bytesRead = putStream.Read(buffer, 0, buffer.Length)) != 0) {
				postDataStream.Write (buffer, 0, bytesRead);
			}
			putStream.Close ();
			#endregion

			#region adding end
			byte[] endBoundaryBytes = Encoding.UTF8.GetBytes ("\r\n--" + boundary + "--\r\n");
			postDataStream.Write (endBoundaryBytes, 0, endBoundaryBytes.Length);
			#endregion

			return postDataStream;

		}

		/// <summary>
		/// Multis the post.
		/// </summary>
		/// <returns>The post.</returns>
		/// <param name="url">URL.</param>
		/// <param name="formData">Form data.</param>
		/// <param name="filePath">File path.</param>
		public Response MultiPost (string url, NameValueCollection formData, string filePath)
		{
			FileInfo fileInfo = new FileInfo (filePath);
			using (FileStream fileStream = fileInfo.OpenRead ()) {
				return MultiPost (url, formData, fileStream, fileInfo.Name);
			}
		}

		/// <summary>
		/// Multis the post.
		/// </summary>
		/// <returns>The post.</returns>
		/// <param name="url">URL.</param>
		/// <param name="formData">Form data.</param>
		/// <param name="inputStream">Input stream.</param>
		/// <param name="fileName">File name.</param>
		public Response MultiPost (string url, NameValueCollection formData, Stream inputStream, string fileName)
		{
			HttpWebRequest request = CreateRequest (url, new StringDict ()); 
			request.Method = "POST";
			Elapse elapsed = new Elapse ();

			string boundary = MultiBoundary ();
			Stream dataStream = BuildPostStream (inputStream, fileName, formData, boundary);
			request.ContentLength = dataStream.Length;
			request.ContentType = "multipart/form-data; boundary=" + boundary;

			dataStream.Position = 0;

			byte[] buffer = new byte[64*1024];
			int bytesRead = 0;
			Stream reqStream = request.GetRequestStream ();
			while ((bytesRead = dataStream.Read(buffer, 0, buffer.Length)) != 0) {
				reqStream.Write (buffer, 0, bytesRead);
			}
			dataStream.Close ();
			reqStream.Close ();
			return Send (request, elapsed);       
		}

		private Response Send(HttpWebRequest request, Elapse elapsed){
			try {
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {                   
					return HandleResponse (response, elapsed.Duration());
				}

			} catch (WebException e) {
				HttpWebResponse response = e.Response as HttpWebResponse;
				if (response != null) {
					return HandleResponse (response, elapsed.Duration());
				}
				return Response.CreateBy(e.ToString(), elapsed.Duration());
			} catch (Exception e) {
				return Response.CreateBy(e.ToString(), elapsed.Duration());
			}
		}

		private static string Via(HttpWebResponse response) {
			string via;
			if ((via = response.GetResponseHeader("X-Via")) != "") {
				return via;
			}

			if ((via = response.GetResponseHeader("X-Px")) != "") {
				return via;
			}

			if ((via = response.GetResponseHeader("Fw-Via")) != "") {
				return via;
			}

			if ((via = response.GetResponseHeader("Via")) != "") {
				return via;
			}

			return via;
		}

		private static string Ctype(HttpWebResponse response) {
			return response.ContentType;
		}

		private int connectTimeout;
		private int responseTimeout;
		private ILogger logger;
	}
}
