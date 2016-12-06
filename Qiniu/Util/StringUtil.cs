using System;
using System.Text;
using System.Web;

namespace Qiniu.Util
{
	/// <summary>
	/// String util.
	/// </summary>
	public static class StringUtil
	{
		/// <summary>
		/// URLs the encode.
		/// </summary>
		/// <returns>The encode.</returns>
		/// <param name="src">Source.</param>
		public static string UrlEncode(string src)
		{
			return HttpUtility.UrlEncode (src, Encoding.UTF8);
		}

		/// <summary>
		/// Gets the bytes.
		/// </summary>
		/// <returns>The bytes.</returns>
		/// <param name="src">Source.</param>
		public static byte[] GetBytes(string src)
		{
			return Encoding.UTF8.GetBytes(src);
		}

		/// <summary>
		/// Arraies the exists.
		/// </summary>
		/// <returns><c>true</c>, if exists was arrayed, <c>false</c> otherwise.</returns>
		/// <param name="array">Array.</param>
		/// <param name="query">Query.</param>
		public static bool ArrayExists(string[] array, string query)
		{
			return Array.Exists (array, delegate (string obj) {
				return obj.Equals (query);
			});
		}
	}
}

