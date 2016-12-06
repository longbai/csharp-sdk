using System;
using System.Text;

using Qiniu.Util;

namespace Qiniu.Processing
{
	/// <summary>
	/// Zip pack op.
	/// </summary>
	public class ZipPackOp : Operation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.ZipPackOp"/> class.
		/// </summary>
		public ZipPackOp() {
			args = new StringDict();
		}

		/// <summary>
		/// Append the specified url.
		/// </summary>
		/// <param name="url">URL.</param>
		public ZipPackOp Append(string url) {
			return Append(url, "");
		}

		/// <summary>
		/// Append the specified url and alias.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="alias">Alias.</param>
		public ZipPackOp Append(string url, string alias) {
			args.Put(url, alias);
			return this;
		}

		/// <summary>
		/// Build this instance.
		/// </summary>
		public String Build() {
			if (args.Size() == 0) {
				throw new ArgumentException ("zip list must have at least one part.");
			}
			StringBuilder b = new StringBuilder("mkzip");
			args.ForEach (delegate (string key, object value) {
				b.Append ("/url/");
				b.Append (UrlSafeBase64.Encode (key));
				string val = (string)value;
				if (!String.IsNullOrEmpty (val)) {
					b.Append ("/alias/");
					b.Append (UrlSafeBase64.Encode (val));
				}
			});
			return b.ToString();
		}

		private StringDict args;
	}
}
