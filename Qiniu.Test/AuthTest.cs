using System;
using System.Text;

using NUnit.Framework;

using Qiniu.Http;
using Qiniu.Util;

namespace Qiniu.Test
{
    [TestFixture]
    public class AuthTest
    {
		[TestFixtureSetUp]
		public void BeforeTest ()
		{
			#region before test
			TestConfig.DummyAuth.SetClock(delegate(){
				return 1234567890L;
			});
			#endregion
		}

		[TestFixtureTearDown]
		public void AfterTest ()
		{
		}
    [Test]
    public void TestSign() {
        string token = TestConfig.DummyAuth.Sign("test");
        Assert.AreEqual("abcdefghklmnopq:mSNBTR7uS2crJsyFr2Amwv1LaYg=", token);
    }

    [Test]
    public void TestSignWithData() {
        string token = TestConfig.DummyAuth.SignWithData("test");
        Assert.AreEqual("abcdefghklmnopq:-jP8eEV9v48MkYiBGs81aDxl60E=:dGVzdA==", token);
    }

    [Test]
    public void TestSignRequest() {
        string token = TestConfig.DummyAuth.SignRequest("http://www.qiniu.com/?go=1",
            Encoding.UTF8.GetBytes("test"), "");
			Assert.AreEqual("abcdefghklmnopq:4YsHbU_NJllJj_vA4ttNqID6_YM=", token);
        token = TestConfig.DummyAuth.SignRequest("http://www.qiniu.com/?go=1",
            Encoding.UTF8.GetBytes("test"), Client.FormMime);
			Assert.AreEqual("abcdefghklmnopq:W-eevloM1iZ2c9CwE6l066oeXLs=", token);
    }

    [Test]
    public void TestPrivateDownloadUrl() {
		string url = TestConfig.DummyAuth.PrivateDownloadUrl("http://www.qiniu.com?go=1", 3600);
        string expect = "http://www.qiniu.com?go=1&e=1234571490&token=abcdefghklmnopq:8vzBeLZ9W3E4kbBLFLW0Xe0u7v4=";
        Assert.AreEqual(expect, url);
    }

    [Test]
    public void TestDeprecatedPolicy() {
        StringDict policy = new StringDict().Put("asyncOps", 1);
        try {
				TestConfig.DummyAuth.UploadToken("1", null, 3600, policy, false);
				Assert.Fail();
        } catch (Exception e) {
				Assert.IsNotNull (e);
				//pass
        }
    }

    [Test]
    public void TestUploadToken() {
        StringDict policy = new StringDict().Put("endUser", "y");
			string token = TestConfig.DummyAuth.UploadToken("1", "2", 3600, policy, false);
        string exp = "abcdefghklmnopq:yyeexeUkPOROoTGvwBjJ0F0VLEo=:eyJlbmRVc2VyIjoieSIsInNjb3BlIjoiMToyIiwiZGVhZGxpbmUiOjEyMzQ1NzE0OTB9";
        Assert.AreEqual(exp, token);
    }
    }
}

