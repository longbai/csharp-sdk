using System;

namespace Qiniu.Util
{
	/// <summary>
	/// Dummy logger.
	/// </summary>
	public class DummyLogger:ILogger
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Util.DummyLogger"/> class.
		/// </summary>
		public DummyLogger ()
		{
		}

		/// <summary>
		/// Trace the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Trace(string s){
		}

		/// <summary>
		/// Debug the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Debug(string s){
		}

		/// <summary>
		/// Info the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Info(string s){
		}

		/// <summary>
		/// Warn the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Warn(string s){
		}

		/// <summary>
		/// Error the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Error(string s){
		}

		/// <summary>
		/// Fatal the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Fatal(string s){
		}
	}
}

