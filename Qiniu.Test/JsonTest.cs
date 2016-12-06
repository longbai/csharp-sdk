using System;

using NUnit.Framework;

using Qiniu.Util;

namespace Qiniu.Test
{
	[TestFixture]
	public class JsonTest
	{
		[Test]
		public void TestDictToString() {
			StringDict dict = new StringDict().Put("k", "v").Put("n", 1);
			String j = dict.ToJson ();
			Assert.IsTrue(j.Equals("{\"k\":\"v\",\"n\":1}") || j.Equals("{\"n\":1,\"k\":\"v\"}"));
		}

		public struct Data{
			public string data;
			public int x;
		}

		[Test]
		public void TestDecode(){
			Data d1 = new Data(){data="abc", x=1};
			string m = Json.Encode (d1);
			Data d2 = Json.Decode<Data> (m);
			Assert.AreEqual (d1.data, d2.data);
			Assert.AreEqual (d1.x, d2.x);
		}
	}
}

