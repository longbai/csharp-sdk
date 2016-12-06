using System;
using System.Text;

namespace Qiniu.Processing
{
	/// <summary>
	/// General op.
	/// </summary>
	public class GeneralOp
	{
		/// <summary>
		/// The arguments count.
		/// </summary>
		protected int argsCount = 0;
		private StringBuilder content;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.GeneralOp"/> class.
		/// </summary>
		/// <param name="cmd">Cmd.</param>
		/// <param name="mode">Mode.</param>
		public GeneralOp(string cmd, Object mode)
		{
			content = new StringBuilder();
			content.Append (cmd);
			if (mode != null) {
				content.Append("/");
				content.Append(mode);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.GeneralOp"/> class.
		/// </summary>
		/// <param name="cmd">Cmd.</param>
		public GeneralOp(String cmd):this(cmd, null)
		{
		}
			
		/// <summary>
		/// Put the specified key and value.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public GeneralOp put(String key, Object value)
		{
			argsCount++;
			content.Append("/");
			content.Append(key);
			if (value != null) {
				content.Append("/");
				content.Append(value);
			}
			return this;
		}

		/// <summary>
		/// Build this instance.
		/// </summary>
		public String Build()
		{
			return content.ToString();
		}
	}
}
