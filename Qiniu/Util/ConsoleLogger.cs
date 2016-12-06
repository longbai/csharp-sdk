using System;

namespace Qiniu.Util
{
	/// <summary>
	/// Console logger.
	/// </summary>
	public class ConsoleLogger:ILogger
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Util.ConsoleLogger"/> class.
		/// </summary>
		public ConsoleLogger ()
		{
		}

		/// <summary>
		/// Trace the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Trace(string s){
			Console.WriteLine(s);
		}

		/// <summary>
		/// Debug the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Debug(string s){
			Console.WriteLine(s);
		}

		/// <summary>
		/// Info the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Info(string s){
			Console.WriteLine(s);
		}

		/// <summary>
		/// Warn the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Warn(string s){
			Console.WriteLine(s);
		}

		/// <summary>
		/// Error the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Error(string s){
			Console.WriteLine(s);
		}

		/// <summary>
		/// Fatal the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		public void Fatal(string s){
			Console.WriteLine(s);
		}

	}
}

