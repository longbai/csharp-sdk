using System;

using NUnit.Framework;

using Qiniu.Http;

namespace Qiniu.Test
{
	[TestFixture]
	public class HttpTest
	{
		[Test]
		public void testGet1() {
			Response r = new Client ().Get ("http://www.baidu.com");
			Assert.IsEmpty (r.ReqId);
		}

		[Test]
		public void testPost2() {
			Response r = new Client ().Post ("http://up.qiniu.com", new byte[1], null);
			Assert.IsNotEmpty (r.ReqId);
			Assert.AreEqual (400, r.StatusCode);
			Assert.IsFalse (r.IsOK ());
		}

		[Test]
		public void testPost3() {
			Response r = new Client().Post("http://httpbin.org/status/500", "hello", null);
			Assert.AreEqual (500, r.StatusCode);
			Assert.IsFalse (r.IsOK());
			Assert.IsTrue (r.IsServerError());
		}

		[Test]
		public void testPost4() {
			Response r = new Client().Post("http://httpbin.org/status/418", "hello", null);
			Assert.AreEqual (418, r.StatusCode);
			Assert.IsFalse (r.IsOK());
		}

		[Test]
		public void testPost5() {
			Response r = new Client().Post("http://httpbin.org/status/298", "hello", null);
			Assert.AreEqual (298, r.StatusCode);
			Assert.IsFalse (r.IsOK());
		}
	}
}

