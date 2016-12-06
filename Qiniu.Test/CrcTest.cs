using System;
using System.Text;

using NUnit.Framework;

using Qiniu.Util;

namespace Qiniu.Test
{
	[TestFixture]
	public class CrcTest
	{
		[Test]
		public void TestCrc() {
			byte[] data = Encoding.UTF8.GetBytes("Hello, World!");
			UInt32 result = Crc32.BytesSum(data);
			Assert.AreEqual(3964322768L, result);
		}
	}
}

