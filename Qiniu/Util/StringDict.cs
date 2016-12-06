using System;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Qiniu.Util
{
	/// <summary>
	/// String dict.
	/// </summary>
	public class StringDict
	{
		private Dictionary<string, object> dict;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qiniu.Util.StringDict"/> class.
		/// </summary>
		public StringDict ()
		{
			dict = new Dictionary<string, object>();
		}

		/// <summary>
		/// Put the specified key and value.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public StringDict Put(string key, Object value)
		{
			dict.Add (key, value);
			return this;
		}

		/// <summary>
		/// Puts the not empty.
		/// </summary>
		/// <returns>The not empty.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public StringDict PutNotEmpty(string key, string value)
		{
			if (String.IsNullOrEmpty (value)) {
				return this;
			}
			return Put (key, value);
		}

		/// <summary>
		/// Puts the not null.
		/// </summary>
		/// <returns>The not null.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public StringDict PutNotNull(string key, Object value)
		{
			if (value == null) {
				return this;
			}
			return Put (key, value);
		}

		/// <summary>
		/// Puts the when.
		/// </summary>
		/// <returns>The when.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <param name="condition">If set to <c>true</c> condition.</param>
		public StringDict PutWhen(string key, Object value, bool condition)
		{
			if (!condition) {
				return this;
			}
			return Put (key, value);
		}

		/// <summary>
		/// Forms the string.
		/// </summary>
		/// <returns>The string.</returns>
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

		/// <summary>
		/// Join the specified entrySep, kvSep and prefix.
		/// </summary>
		/// <param name="entrySep">Entry sep.</param>
		/// <param name="kvSep">Kv sep.</param>
		/// <param name="prefix">Prefix.</param>
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

		/// <summary>
		/// Consumer.
		/// </summary>
		public delegate void consumer(string key, Object value);  

		/// <summary>
		/// Fors the each.
		/// </summary>
		/// <param name="c">C.</param>
		public void ForEach(consumer c)
		{
			foreach (KeyValuePair<string, Object> kvp in dict)  
			{  
				c (kvp.Key, kvp.Value);
			}
		}

		/// <summary>
		/// Size this instance.
		/// </summary>
		public int Size()
		{
			return dict.Count;
		}

		/// <summary>
		/// Tos the json.
		/// </summary>
		/// <returns>The json.</returns>
		public string ToJson(){
			return Json.Encode (dict);
		}
	}
}

