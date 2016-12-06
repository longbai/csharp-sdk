using System;

namespace Qiniu.Util
{
	/// <summary>
	/// I logger.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Trace the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		void Trace(string s);
		/// <summary>
		/// Debug the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		void Debug(string s);
		/// <summary>
		/// Info the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		void Info(string s);
		/// <summary>
		/// Warn the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		void Warn(string s);
		/// <summary>
		/// Error the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		void Error(string s);
		/// <summary>
		/// Fatal the specified s.
		/// </summary>
		/// <param name="s">S.</param>
		void Fatal(string s);
	}
}

