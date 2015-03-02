using System;
using System.Text;
using System.Web;

namespace Qiniu.Util
{
	public static class StringUtil
	{
		public static string UrlEncode(string src)
		{
			return HttpUtility.UrlEncode (src, Encoding.UTF8);
		}

		public static byte[] GetBytes(string src)
		{
			return Encoding.UTF8.GetBytes(src);
		}
	}
}

