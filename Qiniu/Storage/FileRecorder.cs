using System;
using System.Text;
using System.IO;

namespace Qiniu.Storage
{
	/// <summary>
	/// File recorder.
	/// </summary>
	public class FileRecorder : Recorder
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Storage.FileRecorder"/> class.
		/// </summary>
		public FileRecorder (string directory)
		{
			this.directory = Directory.CreateDirectory (directory);
		}

		/// <summary>
		/// Keies the generate.
		/// </summary>
		/// <returns>The generate.</returns>
		/// <param name="storageKey">Storage key.</param>
		/// <param name="filePath">File path.</param>
		public string KeyGenerate (string storageKey, string filePath){
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(storageKey));
		}

		/// <summary>
		/// Put the specified key and data.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="data">Data.</param>
		public void Put(string key, byte[] data){
			File.WriteAllBytes (directory.FullName + key, data);
		}
			

		/// <summary>
		/// Fetch the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		public byte[] Fetch(string key){
			return File.ReadAllBytes (directory.FullName + key);
		}

		/// <summary>
		/// Del the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		public void del(string key){
			File.Delete (directory.FullName + key);
		}

		private DirectoryInfo directory;
	}
}
