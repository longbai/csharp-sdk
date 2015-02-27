using System;
using System.Text;
using System.Collections.Generic;

namespace Qiniu.Processing
{
	public class Pipe
	{
		private List<Operation> ops;
		public Pipe ()
		{
			ops = new List<Operation> ();
		}

		public Pipe Append(Operation op)
		{
			ops.Add (op);
			return this;
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			bool noStart = false;
			foreach (Operation cmd in ops) {
				if (noStart) {
					b.Append("|");
				}
				b.Append(cmd.Build());
				noStart = true;
			}
			return b.ToString();
		}
	}
}
