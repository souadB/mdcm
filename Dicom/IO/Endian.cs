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
using System.Net;
using System.Text;

namespace Dicom.IO {
	#region Endian
	public class Endian {
		public static readonly Endian Little = new Endian(false);
		public static readonly Endian Big = new Endian(true);
		public static readonly Endian LocalMachine = BitConverter.IsLittleEndian ? Little : Big;

		private bool _isBigEndian;
		private Endian(bool isBigEndian) {
			_isBigEndian = isBigEndian;
		}

		public override bool Equals(object obj) {
			if (obj is Endian)
				return this == (Endian)obj;
			return false;
		}

		public override int GetHashCode() {
			return _isBigEndian.GetHashCode();
		}

		public override string ToString() {
			if (_isBigEndian)
				return "Big Endian";
			return "Little Endian";
		}

		public static bool operator ==(Endian e1, Endian e2) {
			return e1._isBigEndian == e2._isBigEndian;
		}
		public static bool operator !=(Endian e1, Endian e2) {
			return e1._isBigEndian != e2._isBigEndian;
		}


		public static void SwapBytes(int bytesToSwap, byte[] bytes) {
			if (bytesToSwap == 1)
				return;
			if (bytesToSwap == 2) { SwapBytes2(bytes); return; }
			if (bytesToSwap == 4) { SwapBytes4(bytes); return; }
			//if (bytesToSwap == 8) { Swap8(); return; }
			int l = bytes.Length - (bytes.Length % bytesToSwap);
			for (int i = 0; i < l; i += bytesToSwap) {
				Array.Reverse(bytes, i, bytesToSwap);
			}
		}
		public static void SwapBytes2(byte[] bytes) {
			byte b;
			int l = bytes.Length - (bytes.Length % 2);
			for (int i = 0; i < l; i += 2) {
				b = bytes[i + 1];
				bytes[i + 1] = bytes[i];
				bytes[i] = b;
			}
		}
		public static void SwapBytes4(byte[] bytes) {
			byte b;
			int l = bytes.Length - (bytes.Length % 4);
			for (int i = 0; i < l; i += 4) {
				b = bytes[i + 3];
				bytes[i + 3] = bytes[i];
				bytes[i] = b;
				b = bytes[i + 2];
				bytes[i + 2] = bytes[i + 1];
				bytes[i + 1] = b;
			}
		}
		public static void SwapBytes8(byte[] bytes) {
			SwapBytes(8, bytes);
		}

		public static short Swap(short value) {
			return (short)Swap((ushort)value);
		}
		public static ushort Swap(ushort value) {
			return unchecked((ushort)((value >> 8) | (value << 8)));
		}

		public static int Swap(int value) {
			return (int)Swap((uint)value);
		}
		public static uint Swap(uint value) {
			return unchecked(
					((value & 0x000000ffU) << 24) |
					((value & 0x0000ff00U) <<  8) |
					((value & 0x00ff0000U) >>  8) |
					((value & 0xff000000U) >> 24));
		}

		public static long Swap(long value) {
			return (long)Swap((ulong)value);
		}
		public static ulong Swap(ulong value) {
			return unchecked(
					((value & 0x00000000000000ffU) << 56) |
					((value & 0x000000000000ff00U) << 40) |
					((value & 0x0000000000ff0000U) << 24) |
					((value & 0x00000000ff000000U) <<  8) |
					((value & 0x000000ff00000000U) >>  8) |
					((value & 0x0000ff0000000000U) >> 24) |
					((value & 0x00ff000000000000U) >> 40) |
					((value & 0xff00000000000000U) >> 56));
		}

		public static float Swap(float value) {
			byte[] b = BitConverter.GetBytes(value);
			Array.Reverse(b);
			return BitConverter.ToSingle(b, 0);
		}

		public static double Swap(double value) {
			byte[] b = BitConverter.GetBytes(value);
			Array.Reverse(b);
			return BitConverter.ToDouble(b, 0);
		}
	}
	#endregion

	#region EndianBinaryReader
	public class EndianBinaryReader : BinaryReader {
		#region Private Members
		private bool SwapBytes = false;
		private byte[] InternalBuffer = new byte[8];
		#endregion

		#region Public Constructors
		public EndianBinaryReader(Stream s) : base(s) {
		}
		public EndianBinaryReader(Stream s, Encoding e) : base(s, e) {
		}
		public EndianBinaryReader(Stream s, Endian e) : base(s) {
			Endian = e;
		}
		public EndianBinaryReader(Stream s, Encoding enc, Endian end) : base(s, enc) {
			Endian = end;
		}

		public static BinaryReader Create(Stream s, Endian e) {
			if (BitConverter.IsLittleEndian) {
				if (Endian.Little == e) {
					return new BinaryReader(s);
				} else {
					return new EndianBinaryReader(s, e);
				}
			} else {
				if (Endian.Big == e) {
					return new BinaryReader(s);
				} else {
					return new EndianBinaryReader(s, e);
				}
			}
		}
		#endregion

		#region Public Properties
		public Endian Endian {
			get {
				if (BitConverter.IsLittleEndian) {
					return SwapBytes ? Endian.Big : Endian.Little;
				} else {
					return SwapBytes ? Endian.Little : Endian.Big;
				}
			}
			set {
				if (BitConverter.IsLittleEndian) {
					SwapBytes = (Endian.Big == value);
				} else {
					SwapBytes = (Endian.Little == value);
				}
			}
		}

		public bool UseInternalBuffer {
			get {
				return (InternalBuffer != null);
			}
			set {
				if (value && (InternalBuffer == null)) {
					InternalBuffer = new byte[8];
				} else {
					InternalBuffer = null;
				}
			}
		}
		#endregion

		#region Private Methods
		private byte[] ReadBytesInternal(int count) {
			byte[] Buffer = null;
			if (InternalBuffer != null) {
				base.Read(InternalBuffer, 0, count);
				Buffer = InternalBuffer;
			} else {
				Buffer = base.ReadBytes(count);
			}
			if (SwapBytes) {
				Array.Reverse(Buffer, 0, count);
			}
			return Buffer;
		}
		#endregion

		#region BinaryReader Overrides
		public override short ReadInt16() {
			if (SwapBytes) {
				return Endian.Swap(base.ReadInt16());
			}
			return base.ReadInt16();
		}

		public override int ReadInt32() {
			if (SwapBytes) {
				return Endian.Swap(base.ReadInt32());
			}
			return base.ReadInt32();
		}

		public override long ReadInt64() {
			if (SwapBytes) {
				return Endian.Swap(base.ReadInt64());
			}
			return base.ReadInt64();
		}

		public override float ReadSingle() {
			if (SwapBytes) {
				byte[] b = ReadBytesInternal(4);
				return BitConverter.ToSingle(b, 0);
			}
			return base.ReadSingle();
		}

		public override double ReadDouble() {
			if (SwapBytes) {
				byte[] b = ReadBytesInternal(8);
				return BitConverter.ToDouble(b, 0);
			}
			return base.ReadDouble();
		}

		public override ushort ReadUInt16() {
			if (SwapBytes) {
				return Endian.Swap(base.ReadUInt16());
			}
			return base.ReadUInt16();
		}

		public override uint ReadUInt32() {
			if (SwapBytes) {
				return Endian.Swap(base.ReadUInt32());
			}
			return base.ReadUInt32();
		}

		public override ulong ReadUInt64() {
			if (SwapBytes) {
				return Endian.Swap(base.ReadUInt64());
			}
			return base.ReadUInt64();
		}
		#endregion
	}
	#endregion

	#region EndianBinaryWriter
	public class EndianBinaryWriter : BinaryWriter {
		#region Private Members
		private bool SwapBytes = false;
		#endregion

		#region Public Constructors
		public EndianBinaryWriter(Stream s) : base(s) {
		}
		public EndianBinaryWriter(Stream s, Encoding e) : base(s, e) {
		}
		public EndianBinaryWriter(Stream s, Endian e) : base(s) {
			Endian = e;
		}
		public EndianBinaryWriter(Stream s, Encoding enc, Endian end) : base(s, enc) {
			Endian = end;
		}

		public static BinaryWriter Create(Stream s, Endian e) {
			if (BitConverter.IsLittleEndian) {
				if (Endian.Little == e) {
					return new BinaryWriter(s);
				} else {
					return new EndianBinaryWriter(s, e);
				}
			} else {
				if (Endian.Big == e) {
					return new BinaryWriter(s);
				} else {
					return new EndianBinaryWriter(s, e);
				}
			}
		}
		#endregion

		#region Public Properties
		public Endian Endian {
			get {
				if (BitConverter.IsLittleEndian) {
					return SwapBytes ? Endian.Big : Endian.Little;
				} else {
					return SwapBytes ? Endian.Little : Endian.Big;
				}
			}
			set {
				if (BitConverter.IsLittleEndian) {
					SwapBytes = (Endian.Big == value);
				} else {
					SwapBytes = (Endian.Little == value);
				}
			}
		}
		#endregion

		#region Private Methods
		private void WriteInternal(byte[] Buffer) {
			if (SwapBytes) {
				Array.Reverse(Buffer);
			}
			base.Write(Buffer);
		}
		#endregion

		#region BinaryWriter Overrides
		public override void Write(double value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}

		public override void Write(float value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}

		public override void Write(int value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}

		public override void Write(long value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}

		public override void Write(short value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}

		public override void Write(uint value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}

		public override void Write(ulong value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}

		public override void Write(ushort value) {
			if (SwapBytes) {
				byte[] b = BitConverter.GetBytes(value);
				WriteInternal(b);
			} else {
				base.Write(value);
			}
		}
		#endregion
	}
	#endregion
}