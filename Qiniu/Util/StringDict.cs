using System;
using System.Text;
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
			StringBuilder b = new StringBuilder ();
			bool notFirst = false;
			foreach (KeyValuePair<string, Object> kvp in dict)  
			{  
				if (notFirst) {
					b.Append ('&');
				}
				b.Append (StringUtil.UrlEncode(kvp.Key));
				b.Append ('=');
				b.Append (StringUtil.UrlEncode(kvp.Value.ToString()));
				notFirst = true;
			}
			return b.ToString ();
		}

		public string Join(string entrySep, string kvSep, string prefix)
		{
			StringBuilder b = new StringBuilder ();
			if (prefix != null) {
				b.Append (prefix);
			}
			bool notFirst = false;
			foreach (KeyValuePair<string, Object> kvp in dict)  
			{  
				if (notFirst) {
					b.Append (entrySep);
				}
				b.Append (kvp.Key);
				b.Append (kvSep);
				b.Append (kvp.Value);
				notFirst = true;
			}
			return b.ToString ();
		}

		public delegate string consumer(string key, Object value);  

		public void ForEach(consumer c)
		{
			foreach (KeyValuePair<string, Object> kvp in dict)  
			{  
				c (kvp.Key, kvp.Value);
			}
		}

		public int Size()
		{
			return dict.Count;
		}
	}
}

