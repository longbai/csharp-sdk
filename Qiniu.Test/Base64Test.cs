using System;

using NUnit.Framework;

using Qiniu.Util;

namespace Qiniu.Test
{
	[TestFixture]
	public class Base64Test
	{
		[Test]
		public void testEncode(){
			string data = "你好/+=";
			string result = UrlSafeBase64.Encode(data);
			Assert.AreEqual("5L2g5aW9Lys9", result);
		}
	}
}

