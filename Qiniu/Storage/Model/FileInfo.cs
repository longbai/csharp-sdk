using System;

namespace Qiniu.Storage.Model
{
	/// <summary>
	/// File info.
	/// </summary>
	public struct FileInfo
	{
		/// <summary>
		/// The key.
		/// </summary>
		public string key;
		/// <summary>
		/// The hash.
		/// </summary>
		public string hash;
		/// <summary>
		/// The fsize.
		/// </summary>
		public long fsize;
		/// <summary>
		/// The put time.
		/// </summary>
		public long putTime;
		/// <summary>
		/// The type of the MIME.
		/// </summary>
		public string mimeType;
		/// <summary>
		/// The end user.
		/// </summary>
		public string endUser;
	}
}
