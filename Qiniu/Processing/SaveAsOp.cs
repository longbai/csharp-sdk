using System;
using Qiniu.Storage;

namespace Qiniu.Processing
{
	public class SaveAsOp : Operation
	{
		private string bucket;
		private string key;

		public SaveAsOp(string bucket, string key) {
			this.bucket = bucket;
			this.key = key;
		}

		public string Build() {
			return "saveas/" + BucketManager.Entry(bucket, key);
		}
	}
}

