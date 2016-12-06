using System;
using System.Text;

using NUnit.Framework;

using Qiniu.Util;

namespace Qiniu.Test
{
	[TestFixture]
	public class EtagTest
	{
		[Test]
		public void TestData() {
			string zero = Etag.BytesSum(new byte[0]);
			Assert.AreEqual("Fto5o-5ea0sNMlW_75VgGJCv2AcJ", zero);

			string etag = Etag.BytesSum(Encoding.UTF8.GetBytes("etag"));
			Assert.AreEqual("FpLiADEaVoALPkdb8tJEJyRTXoe_", etag);
		}

		[Test]
		public void TestFile(){
			string f = TempFile.CreateFile(1024);
			Assert.AreEqual("Foyl8onxBLWeRLL5oItRJphv6i4b", Etag.FileSum(f));
			TempFile.Remove (f);
			f = TempFile.CreateFile(4 * 1024);
			Assert.AreEqual("FicHOveBNs5Kn9d74M3b9tI4D-8r", Etag.FileSum(f));
			TempFile.Remove(f);
			f = TempFile.CreateFile(5 * 1024);
			Assert.AreEqual("lg-Eb5KFCuZn-cUfj_oS2PPOU9xy", Etag.FileSum(f));
			TempFile.Remove(f);
			f = TempFile.CreateFile(8 * 1024);
			Assert.AreEqual("lkSKZOMToDp-EqLDVuT1pyjQssl-", Etag.FileSum(f));
			TempFile.Remove(f);
			f = TempFile.CreateFile(9 * 1024);
			Assert.AreEqual("ljgVjMtyMsOgIySv79U8Qz4TrUO4", Etag.FileSum(f));
			TempFile.Remove(f);
		}
	}
}
