using System;

namespace Qiniu
{
	/// <summary>
	/// Qiniu exception.
	/// </summary>
	public class QiniuException:System.IO.IOException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.QiniuException"/> class.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <param name="s">S.</param>
		public QiniuException (int code, string s):base(s)
		{
			this.Code = code;
		}

		/// <summary>
		/// Gets the code.
		/// </summary>
		/// <value>The code.</value>
		public int Code {
			get;
		}
	}
}

