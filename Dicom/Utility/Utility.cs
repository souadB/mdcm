using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dicom.Utility {
	/// <summary>
	/// Utility methods for manipulating a stream
	/// </summary>
	public static class StreamUtility {
		/// <summary>
		/// Copies data from source stream to destination stream
		/// </summary>
		/// <param name="src">Source stream</param>
		/// <param name="dst">Destination stream</param>
		public static void Copy(Stream src, Stream dst) {
			byte[] buffer = new byte[65536];
			int read = 0;
			do {
				read = src.Read(buffer, 0, buffer.Length);
				dst.Write(buffer, 0, read);
			} while (read == buffer.Length);
		}
	}

	/// <summary>
	/// Utility methods for manipulating an array
	/// </summary>
	public static class ArrayUtility {
		/// <summary>
		/// Shuffles an array
		/// </summary>
		/// <param name="array">Array</param>
		public static void Shuffle(Array array) {
			Random rand = new Random();
			int n = array.Length;
			while (--n > 0) {
				int k = rand.Next(n + 1);
				Swap(array, n, k);
			}
		}

		/// <summary>
		/// Swaps 2 items in an array
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="source">Source item</param>
		/// <param name="destination">Destination item</param>
		public static void Swap(Array array, int source, int destination) {
			object obj = array.GetValue(source);
			array.SetValue(array.GetValue(destination), source);
			array.SetValue(obj, destination);
		}
	}

	/// <summary>
	/// Utility methods for manipulating a byte array
	/// </summary>
	public static class ByteUtility {
		/// <summary>
		/// Gets MD5 hash of a byte array
		/// </summary>
		/// <param name="buffer">Byte array</param>
		/// <returns>MD5 hash as string</returns>
		public static string MD5(byte[] buffer) {
			MD5 md5 = new MD5CryptoServiceProvider();
			return BitConverter.ToString(md5.ComputeHash(buffer));
		}

		/// <summary>
		/// Gets SHA1 hash of a byte array
		/// </summary>
		/// <param name="buffer">Byte array</param>
		/// <returns>SHA1 hash as string</returns>
		public static string SHA1(byte[] buffer) {
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			return BitConverter.ToString(sha1.ComputeHash(buffer));
		}
	}

	/// <summary>
	/// Utility methods for manipulating a string
	/// </summary>
	public static class StringUtility {
		/// <summary>
		/// Gets MD5 hash of a string
		/// </summary>
		/// <param name="str">String</param>
		/// <returns>MD5 hash as string</returns>
		public static string MD5(string str) {
			byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
			return ByteUtility.MD5(bytes);
		}

		/// <summary>
		/// Gets SHA1 hash of a string
		/// </summary>
		/// <param name="str">String</param>
		/// <returns>SHA1 hash as string</returns>
		public static string SHA1(string str) {
			byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
			return ByteUtility.SHA1(bytes);
		}
	}
}
