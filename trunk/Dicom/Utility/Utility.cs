using System;
using System.Collections.Generic;
using System.IO;
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
}
