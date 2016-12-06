using System;
using Qiniu.Util;

namespace Qiniu.Storage.Model
{
	/// <summary>
	/// File listing.
	/// </summary>
	public struct FileListing
	{
		/// <summary>
		/// The items.
		/// </summary>
		public FileInfo[] items;

		/// <summary>
		/// The marker.
		/// </summary>
		public string marker;

		/// <summary>
		/// Determines whether this instance is EO.
		/// </summary>
		/// <returns><c>true</c> if this instance is EO; otherwise, <c>false</c>.</returns>
		public bool IsEOF() {
			return String.IsNullOrEmpty (marker);
		}
	}
}

