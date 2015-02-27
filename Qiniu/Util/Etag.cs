﻿using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace Qiniu.Util
{
	public static class Etag
	{

		public static string BytesSum(byte[] data, int offset, int length) {
			return StreamSum(new MemoryStream(data, offset, length));
		}

		public static string BytesSum(byte[] data) {
			return BytesSum(data, 0, data.Length);
		}
			
		public static string FileSum(string fileName) {
			using (FileStream fs = File.OpenRead(fileName)) {
				return StreamSum (fs);
			}
		}

		public static string StreamSum(Stream input){
			byte[] buffer = new byte[4 * 1024 * 1024];
			List<byte[]> hashs = new List<byte[]> ();
			long streamLen = 0;
			while (true) {
				int n = input.Read (buffer, 0, buffer.Length);
				if (n == 0) {
					if (streamLen == 0) {
						return "Fto5o-5ea0sNMlW_75VgGJCv2AcJ";
					}
					break;
				}
				hashs.Add (BlockHash (buffer, n));
			}

			return AllHash(hashs);
		}

		private static byte[] BlockHash(byte[] buffer, int len) {
			SHA1 sha = new SHA1CryptoServiceProvider();
			return sha.ComputeHash (buffer, 0, len);
		}
			
		private static string AllHash(List<byte[]> sha1s) {
			byte head = 0x16;
			byte[] finalHash = sha1s[0];
			int len = finalHash.Length;
			byte[] ret = new byte[len + 1];
			if (sha1s.Count != 1) {
				head = (byte) 0x96;
				SHA1 sha = new SHA1CryptoServiceProvider();
				foreach (byte[] s in sha1s) {
					sha.TransformBlock (s, 0, s.Length, s, 0);
				}
				sha.TransformFinalBlock (ret, 1, 0);
				finalHash = sha.Hash;
			}
			ret[0] = head;
			finalHash.CopyTo (ret, 1);
			return UrlSafeBase64.Encode(ret);
		}
	}
}