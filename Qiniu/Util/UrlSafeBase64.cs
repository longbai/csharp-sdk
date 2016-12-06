using System;
using System.Text;

namespace Qiniu.Util
{
	/// <summary>
	/// URL safe base64.
	/// </summary>
	public static class UrlSafeBase64
	{
		/// <summary>
		/// Encode the specified text.
		/// </summary>
		/// <param name="text">Text.</param>
		public static string Encode (string text)
		{
			if (String.IsNullOrEmpty (text)) {
				return "";
			}
			return Encode(Encoding.UTF8.GetBytes (text));
		}

		/// <summary>
		/// Encode the specified bs.
		/// </summary>
		/// <param name="bs">Bs.</param>
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
