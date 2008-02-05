using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dicom.Utility {
	public static class StreamUtility {
		public static void Copy(Stream src, Stream dst) {
			byte[] buffer = new byte[65536];
			int read = 0;
			do {
				read = src.Read(buffer, 0, buffer.Length);
				dst.Write(buffer, 0, read);
			} while (read == buffer.Length);
		}
	}

	public static class ArrayUtility {
		public static void Shuffle(Array array) {
			Random rand = new Random();
			int n = array.Length;
			while (--n > 0) {
				int k = rand.Next(n + 1);
				Swap(array, n, k);
			}
		}

		public static void Swap(Array array, int source, int destination) {
			object obj = array.GetValue(source);
			array.SetValue(array.GetValue(destination), source);
			array.SetValue(obj, destination);
		}
	}

	public static class ByteUtility {
		public static string MD5(byte[] buffer) {
			MD5 md5 = new MD5CryptoServiceProvider();
			return BitConverter.ToString(md5.ComputeHash(buffer));
		}

		public static string SHA1(byte[] buffer) {
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			return BitConverter.ToString(sha1.ComputeHash(buffer));
		}
	}

	public static class StringUtility {
		public static string MD5(string str) {
			byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
			return ByteUtility.MD5(bytes);
		}

		public static string SHA1(string str) {
			byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
			return ByteUtility.SHA1(bytes);
		}
	}
}
