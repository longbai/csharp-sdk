using System;
using System.Text;

namespace Qiniu.Processing
{
	public class GeneralOp
	{
		protected int argsCount = 0;
		private StringBuilder content;

		public GeneralOp(string cmd, Object mode)
		{
			content = new StringBuilder();
			content.Append (cmd);
			if (mode != null) {
				content.Append("/");
				content.Append(mode);
			}
		}

		public GeneralOp(String cmd):this(cmd, null)
		{
		}
			
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

		public String Build()
		{
			return content.ToString();
		}
	}
}
