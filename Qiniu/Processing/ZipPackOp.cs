using System;
using System.Text;

using Qiniu.Util;

namespace Qiniu.Processing
{
	public class ZipPackOp : Operation
	{
		private StringDict args;

		public ZipPackOp() {
			args = new StringDict();
		}

		public ZipPackOp Append(string url) {
			return Append(url, "");
		}

		public ZipPackOp Append(string url, string alias) {
			args.Put(url, alias);
			return this;
		}

		public String Build() {
			if (args.Size() == 0) {
				throw new ArgumentException ("zip list must have at least one part.");
			}
			StringBuilder b = new StringBuilder("mkzip");
//			args.iterate(new StringMap.Do() {
//				@Override
//				public void deal(String key, Object value) {
//					b.append("/url/");
//					b.append(UrlSafeBase64.encodeToString(key));
//					String val = (String) value;
//					if (!StringUtils.isNullOrEmpty(val)) {
//						b.append("/alias/");
//						b.append(UrlSafeBase64.encodeToString(val));
//					}
//				}
//			});
			return b.ToString();
		}
	}
}
