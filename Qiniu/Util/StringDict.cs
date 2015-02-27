using System;
using System.Collections.Generic;

namespace Qiniu.Util
{
	public class StringDict
	{
		private Dictionary<string, object> dict;
		public StringDict ()
		{
			dict = new Dictionary<string, object>();
		}

		public StringDict Put(string key, Object value)
		{
			dict.Add (key, value);
			return this;
		}

		public StringDict PutNotEmpty(string key, string value)
		{
			if (String.IsNullOrEmpty (value)) {
				return this;
			}
			return Put (key, value);
		}

		public StringDict PutNotNull(string key, Object value)
		{
			if (value == null) {
				return this;
			}
			return Put (key, value);
		}

		public StringDict PutWhen(string key, Object value, bool condition)
		{
			if (!condition) {
				return this;
			}
			return Put (key, value);
		}

		public string FormString()
		{
			return null;
		}

		public string Join(string entrySep, string kvSep)
		{
			return null;
		}

		public int Size()
		{
			return dict.Count;
		}
	}
}

