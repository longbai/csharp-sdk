using System;
using System.Text;
using System.Collections.Generic;

namespace Qiniu.Processing
{
	/// <summary>
	/// Pipe.
	/// </summary>
	public class Pipe
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Processing.Pipe"/> class.
		/// </summary>
		public Pipe ()
		{
			ops = new List<Operation> ();
		}

		/// <summary>
		/// Append the specified op.
		/// </summary>
		/// <param name="op">Op.</param>
		public Pipe Append(Operation op)
		{
			ops.Add (op);
			return this;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Qiniu.Processing.Pipe"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Qiniu.Processing.Pipe"/>.</returns>
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

		private List<Operation> ops;
	}
}
