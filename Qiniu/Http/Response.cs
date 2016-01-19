using System;
using System.Net;

namespace Qiniu.Http
{
	public class Response
	{
		public const int InvalidArgument = -4;
		public const int InvalidFile = -3;
		public const int Cancelled = -2;
		public const int NetworkError = -1;
		/**
     * 回复状态码
     */
		public int statusCode;
		/**
     * 七牛日志扩展头
     */
		public string reqId;
		/**
     * 七牛日志扩展头
     */
		public string xlog;
		/**
     * cdn日志扩展头
     */
		public string xvia;
		/**
     * 错误信息
     */
		public string error;
		/**
     * 请求消耗时间，单位秒
     */
		public double duration;

		private byte[] body;
		private HttpWebResponse response;
		private Response (int code, string error, string reqId, string xlog, string xvia, double duration, HttpWebResponse resp)
		{
			this.statusCode = code;
			this.error = error;
			this.duration = duration;
			this.reqId = reqId;
			this.xlog = xlog;
			this.xvia = xvia;
			this.response = resp;
		}

		public static Response CreateBy(HttpWebResponse response, double duration)
		{
			string via = Via (response);
			string reqId = response.GetResponseHeader ("X-Reqid");
			string xlog = response.GetResponseHeader ("X-Log");
			return new Response (0, null, reqId, xlog, via, duration);
		}

		public static Response CreateBy(string error, double duration)
		{
			return new Response (NetworkError, error, "", "", "", duration);
		}

		public string ToString()
		{
			return "";
		}

		private static string Via(HttpWebResponse response) {
			string via;
			if (!(via = response.GetResponseHeader("X-Via", "")) == "") {
				return via;
			}

			if (!(via = response.GetResponseHeader("X-Px", "")) == "") {
				return via;
			}

			if (!(via = response.GetResponseHeader("Fw-Via", "")) == "") {
				return via;
			}

			if (!(via = response.GetResponseHeader("Via", "")) == "") {
				return via;
			}

			return via;
		}

		private static string Ctype(HttpWebResponse response) {
			return response.ContentType;
		}
	}

}

