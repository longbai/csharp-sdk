using System;
using System.Text;

namespace Qiniu.Util
{
	public static class UrlSafeBase64
	{
		public static string Encode (string text)
		{
			if (String.IsNullOrEmpty (text)) {
				return "";
			}
			return Encode(Encoding.UTF8.GetBytes (text));
		}

		public static string Encode (byte[] bs)
		{
			if (bs == null || bs.Length == 0)
				return "";
			string encodedStr = Convert.ToBase64String (bs);
			encodedStr = encodedStr.Replace ('+', '-').Replace ('/', '_');
			return encodedStr;
		}
	}
}
