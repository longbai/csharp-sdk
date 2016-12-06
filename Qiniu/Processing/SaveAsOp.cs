using System;
using Qiniu.Storage;

namespace Qiniu.Processing
{
	/// <summary>
	/// Save as op.
	/// </summary>
	public class SaveAsOp : Operation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.SaveAsOp"/> class.
		/// </summary>
		/// <param name="bucket">Bucket.</param>
		/// <param name="key">Key.</param>
		public SaveAsOp(string bucket, string key) {
			this.bucket = bucket;
			this.key = key;
		}

		/// <summary>
		/// Build this instance.
		/// </summary>
		public string Build() {
			return "saveas/" + ObjectManager.Entry(bucket, key);
		}

		private string bucket;
		private string key;
	}
}

