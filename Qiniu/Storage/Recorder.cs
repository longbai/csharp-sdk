using System;

namespace Qiniu.Storage
{
	/// <summary>
	/// Recorder.
	/// </summary>
	public interface Recorder
	{
		/// <summary>
		/// Keies the generate.
		/// </summary>
		/// <returns>The generate.</returns>
		/// <param name="storageKey">Storage key.</param>
		/// <param name="filePath">File path.</param>
		string KeyGenerate (string storageKey, string filePath);

		/// <summary>
		/// Put the specified key and data.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="data">Data.</param>
		void Put(string key, byte[] data);

		/// <summary>
		/// Fetch the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		byte[] Fetch(string key);

		/// <summary>
		/// Del the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		void del(string key);
	}
}

