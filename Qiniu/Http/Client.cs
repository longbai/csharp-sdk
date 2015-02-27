using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;

using Qiniu.Util;

namespace Qiniu.Http
{
	public class Client
	{
		public const string ContentTypeHeader = "Content-Type";
		public const string DefaultMime = "application/octet-stream";
		public const string JsonMime = "application/json";
		public const string FormMime = "application/x-www-form-urlencoded";
		public Client ()
		{
		}
			
		public Response Get(string url)
		{
			return Get (url, new StringDict ());
		}
		public Response Get(string url, StringDict headers)
		{
			try {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
				request.UserAgent = UserAgent();
				request.Method = "GET";
//				headers.itera
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
					return HandleResponse (response);
				}
			} catch (Exception e) {
				return new Response (-1, e.Message);
			}
		}

		public Response Post(string url, byte[] body, StringDict headers) 
		{
			return Post(url, body, headers, DefaultMime);
		}

		public Response post(string url, string body, StringDict headers) 
		{
			return Post(url, Encoding.UTF8.GetBytes(body), headers, DefaultMime);
		}

		public Response Post(string url, byte[] body, StringDict headers, string contentType) 
		{
			try {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
				request.UserAgent = UserAgent();
				request.Method = "POST";
				request.ContentType = contentType;
				request.ContentLength = body.Length;
				//				headers.itera
				using (Stream requestStream = request.GetRequestStream()) {
					requestStream.Write(body, 0, body.Length);
				}
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
					return HandleResponse (response);
				}
			} catch (Exception e) {
				return new Response (-1, e.Message);
			}
		}

		private static string UserAgent()
		{
			return "QiniuCsharp/"+ Config.VERSION + " (" + Environment.OSVersion.Version.ToString() + "; )";
		}

		private Response HandleResponse(HttpWebResponse response)
		{
			return new Response (response);
		}

		private static string MultiBoundary ()
		{
			return String.Format ("----------{0:N}", Guid.NewGuid ());
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

		public Response MultiPost (string url, NameValueCollection formData, string filePath)
		{
			FileInfo fileInfo = new FileInfo (filePath);
			using (FileStream fileStream = fileInfo.OpenRead ()) {
				return MultiPost (url, formData, fileStream, fileInfo.Name);
			}
		}

		public Response MultiPost (string url, NameValueCollection formData, Stream inputStream, string fileName)
		{
			string boundary = MultiBoundary ();
			WebRequest webRequest = WebRequest.Create (url);

			webRequest.Method = "POST";
			webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

			Stream dataStream = BuildPostStream (inputStream, fileName, formData, boundary);
			webRequest.ContentLength = dataStream.Length;
			dataStream.Position = 0;

			byte[] buffer = new byte[64*1024];
			int bytesRead = 0;
			Stream reqStream = webRequest.GetRequestStream ();
			while ((bytesRead = dataStream.Read(buffer, 0, buffer.Length)) != 0) {
				reqStream.Write (buffer, 0, bytesRead);
			}
			dataStream.Close ();
			reqStream.Close ();

			try {
				using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse) {                   
					return HandleResponse (response);
				}

			} catch (Exception e) {
				return new Response (-1, e.Message);
			}            
		}
	}
}

