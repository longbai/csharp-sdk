using System;
using System.IO;

namespace Qiniu.Test
{
	public class TempFile
	{
		private TempFile() {
		}

		public static void Remove(string f) {
			File.Delete (f);
		}

		public static string CreateFile(int kiloSize) {
			string tempPath = Path.GetTempPath();
			long size = (long) (1024 * kiloSize);
			string fileName = tempPath + Path.GetFileName("qiniu_" + kiloSize + "k" + ".tmp");
			using(FileStream fs = new FileStream(fileName,FileMode.OpenOrCreate)){
				byte[] b = GetByte();
				long s = 0;
				while (s < size) {
					int l = (int) Math.Min(b.Length, size - s);
					fs.Write(b, 0, l);
					s += l;
				}
				fs.Flush ();
			}
			return fileName;
		}

		private static byte[] GetByte() {
			byte[] b = new byte[1024 * 4];
			b[0] = (byte)'A';
			for (int i = 1; i < 1024 * 4; i++) {
				b[i] = (byte)'b';
			}
			b[1024 * 4 - 2] = (byte)'\r';
			b[1024 * 4 - 1] = (byte)'\n';
			return b;
		}
	}
}

