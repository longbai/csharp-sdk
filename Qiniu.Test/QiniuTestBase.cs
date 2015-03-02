using System;
using System.Collections;

namespace Qiniu.Test
{
	public class QiniuTestBase
	{

		protected static string Bucket = "csharpsdk";
		protected static  string LocalKey = "gogopher.jpg";
		protected static string DOMAIN = "qiniuphotos.qiniudn.com";
		protected static string BigFile = @"";
		protected static string FileOpUrl = "http://qiniuphotos.qiniudn.com/gogopher.jpg";
		protected static string NewKey
		{
			get { return Guid.NewGuid().ToString(); }
		}
		private static bool init = false;
		private void Init()
		{
			if (init)
				return;

			init = true;
		}

		public QiniuTestBase()
		{
			Init();
		}
		protected void PrintLn(string str)
		{
			Console.WriteLine(str);
		}
	}
}

