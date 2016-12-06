using System;

using Newtonsoft.Json;

namespace Qiniu.Util
{
	/// <summary>
	/// Json.
	/// </summary>
	public static class Json
	{
		/// <summary>
		/// Encode the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public static string Encode(Object obj){
			return JsonConvert.SerializeObject (obj);
		}

		/// <summary>
		/// Decode the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Decode<T>(string data){
			return JsonConvert.DeserializeObject<T>(data);
		}
	}
}

