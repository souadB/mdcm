// mDCM: A C# DICOM library
//
// Copyright (c) 2006-2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)

using System;
using System.IO;

namespace Dicom.IO {
	public class FileSegment {
		#region Public Constructors
		public FileSegment(string fileName, long position, long length) {
			FileName = fileName;
			Position = position;
			Length = length;
		}
		#endregion

		#region Public Properties
		public string FileName { get; private set; }
		public long Position { get; private set; }
		public long Length { get; private set; }
		#endregion

		#region Public Methods
		public FileStream OpenStream() {
			FileStream fs = File.OpenRead(FileName);
			fs.Seek(Position, SeekOrigin.Begin);
			return fs;
		}

		public byte[] GetData() {
			byte[] data = new byte[Length];
			using (FileStream fs = OpenStream()) {
				fs.Read(data, 0, (int)Length);
			}
			return data;
		}

		public void WriteTo(Stream s) {
			using (FileStream fs = OpenStream()) {
				byte[] buffer = new byte[65536];
				int count = (int)Length;
				while (count > 0) {
					int size = Math.Min(count, buffer.Length);
					size = fs.Read(buffer, 0, size);
					s.Write(buffer, 0, size);
					count -= size;
				}
			}
		}

		public ByteBuffer GetBuffer() {
			return new ByteBuffer(GetData());
		}
		#endregion
	}
}
