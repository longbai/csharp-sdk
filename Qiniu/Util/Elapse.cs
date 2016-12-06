using System;

namespace Qiniu.Util
{
	/// <summary>
	/// Elapse.
	/// </summary>
	public class Elapse
	{
		private long start;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Util.Elapse"/> class.
		/// </summary>
		public Elapse ()
		{
			start = DateTime.Now.ToUniversalTime ().Ticks;
		}

		/// <summary>
		/// Duration this instance.
		/// </summary>
		public long Duration(){
			return (DateTime.Now.ToUniversalTime ().Ticks - start) / 10000000;
		}
	}
}

