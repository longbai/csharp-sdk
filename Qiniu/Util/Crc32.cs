using System;
using System.IO;

namespace Qiniu.Util
{
	/// <summary>
	/// Crc32.
	/// </summary>
	public static class Crc32
	{
		/// <summary>
		/// Byteses the sum.
		/// </summary>
		/// <returns>The sum.</returns>
		/// <param name="data">Data.</param>
		public static UInt32 BytesSum(byte[] data){
			return BytesSum (data, data.Length);
		}
		/// <summary>
		/// Byteses the sum.
		/// </summary>
		/// <returns>The sum.</returns>
		/// <param name="data">Data.</param>
		/// <param name="length">Length.</param>
		public static UInt32 BytesSum (byte[] data,int length)
		{
			Imp crc = new Imp ();
			crc.Write (data, 0, length);
			return crc.Sum32 ();
		}

		/// <summary>
		/// Files the sum.
		/// </summary>
		/// <returns>The sum.</returns>
		/// <param name="fileName">File name.</param>
		public static UInt32 FileSum (string fileName)
		{
			Imp crc = new Imp ();
			int bufferLen = 32 * 1024;
			using (FileStream fs = File.OpenRead(fileName)) {
				byte[] buffer = new byte[bufferLen];
				while (true) {
					int n = fs.Read (buffer, 0, bufferLen);
					if (n == 0)
						break;
					crc.Write (buffer, 0, n);
				}
			}
			return crc.Sum32 ();
		}

		private class Imp
		{
			public const UInt32 IEEE = 0xedb88320;
			private UInt32[] Table;
			private UInt32 Value;

			public Imp ()
			{
				Value = 0;
				Table = MakeTable (IEEE);
			}

			public void Write (byte[] p, int offset, int count)
			{
				this.Value = Update (this.Value, this.Table, p, offset, count);
			}

			public UInt32 Sum32 ()
			{
				return this.Value;
			}

			private static UInt32[] MakeTable (UInt32 poly)
			{
				UInt32[] table = new UInt32[256];
				for (int i = 0; i < 256; i++) {
					UInt32 crc = (UInt32)i;
					for (int j = 0; j < 8; j++) {
						if ((crc & 1) == 1)
							crc = (crc >> 1) ^ poly;
						else
							crc >>= 1;
					}
					table [i] = crc;
				}
				return table;
			}

			public static UInt32 Update (UInt32 crc, UInt32[] table, byte[] p, int offset, int count)
			{
				crc = ~crc;
				for (int i = 0; i < count; i++) {
					crc = table [((byte)crc) ^ p [offset + i]] ^ (crc >> 8);
				}
				return ~crc;
			}
		}
	}
}

