using System;

using NUnit.Framework;

using Qiniu.Storage;
using Qiniu.Util;

namespace Qiniu.Test
{
	[TestFixture]
	public class BucketTest
	{
		[Test]
		public void TestBuckets(){
			BucketManager b = new BucketManager (TestConfig.TestAuth);
			try{
				string[] buckets = b.Buckets ();
				Assert.IsTrue(buckets.Length > 0);
				Assert.IsTrue(StringUtil.ArrayExists(buckets, TestConfig.Bucket));
			} catch(Exception e){
				Console.WriteLine (e.ToString ());
				Assert.Fail ();
			}
		}

		[Test]
		public void TestWrongTokenBuckets(){
			BucketManager dummy = new BucketManager(TestConfig.DummyAuth);

			try {
				dummy.Buckets();
				Assert.Fail();
			} catch (QiniuException e) {
				Assert.AreEqual(401, e.Code);
			}
		}
	}
}
