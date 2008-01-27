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
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Dicom.IO;

namespace Dicom.Data {
	public class DcmPixelData {
		#region Private Members
		private DcmTS _transferSyntax;
		private int _frames;
		private ushort _width;
		private ushort _height;
		private ushort _highBit;
		private ushort _bitsStored;
		private ushort _bitsAllocated;
		private ushort _samplesPerPixel;
		private ushort _pixelRepresentation;
		private ushort _planarConfiguration;
		private string _photometricInterpretation;
		private DcmItem _pixelDataItem;
		private uint _fragmentSize = 0xffffffff;
		private double _rescaleSlope;
		private double _rescaleIntercept;
		private bool _hasPixelPadding;
		private int _pixelPaddingValue;
		#endregion

		#region Public Contructors
		public DcmPixelData(DcmTS ts) {
			_transferSyntax = ts;
			CreatePixelDataItem();
		}

		public DcmPixelData(DcmTS ts, DcmPixelData old) {
			_transferSyntax = ts;
			_frames = 0;
			_width = old.ImageWidth;
			_height = old.ImageHeight;
			_highBit = old.HighBit;
			_bitsStored = old.BitsStored;
			_bitsAllocated = old.BitsAllocated;
			_samplesPerPixel = old.SamplesPerPixel;
			_pixelRepresentation = old.PixelRepresentation;
			_planarConfiguration = old.PlanarConfiguration;
			_photometricInterpretation = old.PhotometricInterpretation;
			_rescaleSlope = old.RescaleSlope;
			_rescaleIntercept = old.RescaleIntercept;
			_pixelPaddingValue = old.PixelPaddingValue;
			CreatePixelDataItem();
		}

		public DcmPixelData(DcmDataset dataset) {
			_transferSyntax = dataset.InternalTransferSyntax;
			_frames = dataset.GetInt32(DcmTags.NumberOfFrames, 1);
			_width = dataset.GetUInt16(DcmTags.Columns, 0);
			_height = dataset.GetUInt16(DcmTags.Rows, 0);
			_bitsStored = dataset.GetUInt16(DcmTags.BitsStored, 0);
			_bitsAllocated = dataset.GetUInt16(DcmTags.BitsAllocated, 0);
			_highBit = dataset.GetUInt16(DcmTags.HighBit, (ushort)(_bitsAllocated - _bitsStored));
			_samplesPerPixel = dataset.GetUInt16(DcmTags.SamplesPerPixel, 0);
			_pixelRepresentation = dataset.GetUInt16(DcmTags.PixelRepresentation, 0);
			_planarConfiguration = dataset.GetUInt16(DcmTags.PlanarConfiguration, 0);
			_photometricInterpretation = dataset.GetString(DcmTags.PhotometricInterpretation, "");
			_rescaleSlope = dataset.GetDouble(DcmTags.RescaleSlope, 1.0);
			_rescaleIntercept = dataset.GetDouble(DcmTags.RescaleIntercept, 0.0);
			_pixelDataItem = dataset.GetItem(DcmTags.PixelData);

			_hasPixelPadding = dataset.Contains(DcmTags.PixelPaddingValue);
			if (_hasPixelPadding) {
				DcmElement elem = dataset.GetElement(DcmTags.PixelPaddingValue);
				if (elem is DcmUnsignedShort)
					_pixelPaddingValue = (elem as DcmUnsignedShort).GetValue();
				else if (elem is DcmSignedShort) {
					_pixelPaddingValue = (elem as DcmSignedShort).GetValue();
				} else
					_pixelPaddingValue = MinimumDataValue;
			}
		}
		#endregion

		#region Public Properties
		public DcmTS TransferSyntax {
			get { return _transferSyntax; }
		}

		public int NumberOfFrames {
			get { return _frames; }
		}

		public ushort ImageWidth {
			get { return _width; }
			set { _width = value; }
		}

		public ushort ImageHeight {
			get { return _height; }
			set { _height = value; }
		}

		public ushort HighBit {
			get { return _highBit; }
			set { _highBit = value; }
		}

		public ushort BitsStored {
			get { return _bitsStored; }
			set { _bitsStored = value; }
		}

		public ushort BitsAllocated {
			get { return _bitsAllocated; }
			set { _bitsAllocated = value; }
		}

		public int BytesAllocated {
			get {
				int bytes = BitsAllocated / 8;
				if ((BitsAllocated % 8) > 0)
					bytes++;
				return bytes;
			}
		}

		public int MinimumDataValue {
			get {
				if (IsSigned)
					return -(1 << (BitsStored - 1));
				else
					return 0;
			}
		}

		public int MaximumDataValue {
			get {
				if (IsSigned)
					return (1 << (BitsStored - 1)) - 1;
				else
					return (1 << BitsStored) - 1;
			}
		}

		public ushort SamplesPerPixel {
			get { return _samplesPerPixel; }
			set { _samplesPerPixel = value; }
		}

		public ushort PixelRepresentation {
			get { return _pixelRepresentation; }
			set { _pixelRepresentation = value; }
		}

		public bool IsSigned {
			get { return _pixelRepresentation != 0; }
		}

		public ushort PlanarConfiguration {
			get { return _planarConfiguration; }
			set { _planarConfiguration = value; }
		}

		public bool IsPlanar {
			get { return _planarConfiguration != 0; }
		}

		public string PhotometricInterpretation {
			get { return _photometricInterpretation; }
			set { _photometricInterpretation = value; }
		}

		public DcmItem PixelDataItem {
			get { return _pixelDataItem; }
		}

		public DcmElement PixelDataElement {
			get { return (DcmElement)PixelDataItem; }
			set { _pixelDataItem = value; }
		}

		public DcmFragmentSequence PixelDataSequence {
			get { return (DcmFragmentSequence)PixelDataItem; }
			set { _pixelDataItem = value; }
		}

		public bool IsEncapsulated {
			get { return _transferSyntax.IsEncapsulated; }
		}

		public bool IsFragmented {
			get { return PixelDataItem.GetType() == typeof(DcmFragmentSequence); }
		}

		public uint FragmentSize {
			get { return _fragmentSize; }
			set { _fragmentSize = value; }
		}

		public int UncompressedFrameSize {
			get { return ImageWidth * ImageHeight * BytesAllocated * SamplesPerPixel; }
		}

		public double RescaleSlope {
			get { return _rescaleSlope; }
		}

		public double RescaleIntercept {
			get { return _rescaleIntercept; }
		}

		public bool HasPixelPadding {
			get { return _hasPixelPadding; }
		}

		public int PixelPaddingValue {
			get { return _pixelPaddingValue; }
		}
		#endregion

		#region Public Members
		#region Frame Access Methods
		public int GetFrameSize(int frame) {
			if (frame < 0 || frame >= NumberOfFrames)
				throw new IndexOutOfRangeException("Requested frame out of range!");

			if (!IsFragmented)
				return UncompressedFrameSize;

			List<ByteBuffer> fragments = GetFrameFragments(frame);

			int size = 0;
			foreach (ByteBuffer fragment in fragments) {
				size += fragment.Length;
			}

			return size;
		}

		public byte[] GetFrameDataU8(int frame) {
			if (frame < 0 || frame >= NumberOfFrames)
				throw new IndexOutOfRangeException("Requested frame out of range!");

			if (BitsAllocated == 8 && PixelDataElement.VR == DcmVR.OW && PixelDataElement.Endian == Endian.Little) {
				PixelDataElement.SelectByteOrder(Endian.Big);
			}

			if (IsFragmented) {
				List<ByteBuffer> fragments = GetFrameFragments(frame);

				int size = 0;
				foreach (ByteBuffer fragment in fragments) {
					size += fragment.Length;
				}

				int pos = 0;
				byte[] buffer = new byte[size];

				foreach (ByteBuffer fragment in fragments) {
					byte[] data = fragment.ToBytes();
					Buffer.BlockCopy(data, 0, buffer, pos, data.Length);
					pos += data.Length;
				}

				return buffer;
			}
			else {
				int size = UncompressedFrameSize;
				int offset = size * frame;
				byte[] buffer = new byte[size];
				ByteBuffer data = PixelDataElement.ByteBuffer;
				Buffer.BlockCopy(data.ToBytes(), offset, buffer, 0, size);
				return buffer;
			}
		}

		public ushort[] GetFrameDataU16(int frame) {
			if (frame < 0 || frame >= NumberOfFrames)
				throw new IndexOutOfRangeException("Requested frame out of range!");

			if (IsFragmented) {
				List<ByteBuffer> fragments = GetFrameFragments(frame);

				int size = 0;
				foreach (ByteBuffer fragment in fragments) {
					size += fragment.Length;
				}
				size /= 2;

				int pos = 0;
				ushort[] buffer = new ushort[size];

				foreach (ByteBuffer fragment in fragments) {
					byte[] data = fragment.ToBytes();
					Buffer.BlockCopy(data, 0, buffer, pos, data.Length);
					pos += data.Length;
				}

				return buffer;
			}
			else {
				int size = UncompressedFrameSize;
				int offset = size * frame;
				ushort[] buffer = new ushort[size / 2];
				ByteBuffer data = PixelDataElement.ByteBuffer;
				Buffer.BlockCopy(data.ToBytes(), offset, buffer, 0, size);
				return buffer;
			}
		}

		public int[] GetFrameDataS32(int frame) {
			if (SamplesPerPixel == 1) {
				if (BitsAllocated != 8 && BitsAllocated != 16)
					throw new DcmDataException("BitsAllocated=" + BitsAllocated + " is unsupported!");

				if (IsSigned) {
					if (BitsAllocated == 8) {
						unchecked {
							int sign = 1 << HighBit;
							int mask = sign - 1;
							int count = ImageWidth * ImageHeight;
							int[] pixels = new int[count];
							byte[] data = GetFrameDataU8(frame);
							for (int p = 0; p < count; p++) {
								byte d = data[p];
								if ((d & sign) != 0)
									pixels[p] = -(d & mask);
								else
									pixels[p] = d & mask;
							}
							return pixels;
						}
					} else {
						unchecked {
							int sign = 1 << HighBit;
							int mask = sign - 1;
							int count = ImageWidth * ImageHeight;
							int[] pixels = new int[count];
							ushort[] data = GetFrameDataU16(frame);
							for (int p = 0; p < count; p++) {
								ushort d = data[p];
								if ((d & sign) != 0)
									pixels[p] = -(d & mask);
								else
									pixels[p] = d & mask;
							}
							return pixels;
						}
					}
				} else {
					if (BitsAllocated == 8) {
						unchecked {
							int count = ImageWidth * ImageHeight;
							int mask = (1 << (HighBit + 1)) - 1;
							int[] pixels = new int[count];
							byte[] data = GetFrameDataU8(frame);
							for (int p = 0; p < count; p++) {
								pixels[p] = data[p] & mask;
							}
							return pixels;
						}
					} else {
						unchecked {
							int count = ImageWidth * ImageHeight;
							int mask = (1 << (HighBit + 1)) - 1;
							int[] pixels = new int[count];
							ushort[] data = GetFrameDataU16(frame);
							for (int p = 0; p < count; p++) {
								pixels[p] = data[p] & mask;
							}
							return pixels;
						}
					}
				}
			} else if (SamplesPerPixel == 3) {
				if (BitsAllocated != 8)
					throw new DcmDataException("BitsAllocated=" + BitsAllocated + " is unsupported!");

				if (PhotometricInterpretation != "RGB" && PhotometricInterpretation != "YBR_FULL")
					throw new DcmDataException("PhotometricInterpretation=" + PhotometricInterpretation + " is unsupported!");

				int count = ImageWidth * ImageHeight;
				int[] pixels = new int[count];
				byte[] data = GetFrameDataU8(frame);
				if (IsPlanar) {
					unchecked {
						int c0 = 0;
						int c1 = count;
						int c2 = c1 + count;
						for (int i = 0; i < count; i++) {
							pixels[i] = (data[c0++] << 16) | (data[c1++] << 8) | data[c2++];
						}
					}
				} else {
					unchecked {
						for (int p = 0, i = 0; i < count; i++) {
							pixels[i] = (data[p++] << 16) | (data[p++] << 8) | data[p++];
						}
					}
				}
				return pixels;
			} else
				throw new DcmDataException("SamplesPerPixel=" + SamplesPerPixel + " is unsupported!");
		}

		public List<ByteBuffer> GetFrameFragments(int frame) {
			if (frame < 0 || frame >= NumberOfFrames)
				throw new IndexOutOfRangeException("Requested frame out of range!");

			if (!IsFragmented)
				throw new InvalidOperationException("No fragmented pixel data!");

			List<ByteBuffer> fragments = new List<ByteBuffer>();
			DcmFragmentSequence sequence = PixelDataSequence;
			int fragmentCount = sequence.Fragments.Count;

			if (NumberOfFrames == 1) {
				fragments.AddRange(sequence.Fragments);
				return fragments;
			}

			if (fragmentCount == NumberOfFrames) {
				fragments.Add(sequence.Fragments[frame]);
				return fragments;
			}

			if (sequence.HasOffsetTable && sequence.OffsetTable.Count == NumberOfFrames) {
				uint offset = sequence.OffsetTable[frame];
				uint stop = 0xffffffff;
				uint pos = 0;

				if ((frame + 1) < NumberOfFrames) {
					stop = sequence.OffsetTable[frame + 1];
				}

				int i = 0;
				while (pos < offset && i < fragmentCount) {
					pos += (uint)(8 + sequence.Fragments[i].Length);
					i++;
				}

				// check for invalid offset table
				if (pos != offset)
					goto GUESS_FRAME_OFFSET;

				while (offset < stop && i < fragmentCount) {
					fragments.Add(sequence.Fragments[i]);
					offset += (uint)(8 + sequence.Fragments[i].Length);
					i++;
				}

				return fragments;
			}

		GUESS_FRAME_OFFSET:
			if (sequence.Fragments.Count > 0) {
				int fragmentSize = sequence.Fragments[0].Length;

				bool allSameLength = true;
				for (int i = 0; i < fragmentCount; i++) {
					if (sequence.Fragments[i].Length != fragmentSize) {
						allSameLength = false;
						break;
					}
				}

				if (allSameLength) {
					if ((fragmentCount % NumberOfFrames) != 0)
						throw new DcmDataException("Unable to determine frame length from pixel data sequence!");

					int count = fragmentCount / NumberOfFrames;
					int start = frame * count;

					for (int i = 0; i < fragmentCount; i++) {
						fragments.Add(sequence.Fragments[i]);
					}

					return fragments;
				}
				else {
					// what if a single frame ends on a fragment boundary?

					int count = 0;
					int start = 0;

					for (int i = 0; i < fragmentCount && count < frame; i++, start++) {
						if (sequence.Fragments[i].Length != fragmentSize)
							count++;
					}

					for (int i = start; i < fragmentCount; i++) {
						fragments.Add(sequence.Fragments[i]);
						if (sequence.Fragments[i].Length != fragmentSize)
							break;
					}

					return fragments;
				}
			}

			return fragments;
		}
		#endregion

		#region Frame Creation Methods
		public void AddFrame(byte[] data) {
			_frames++;
			if (IsFragmented) {
				DcmFragmentSequence sequence = PixelDataSequence;

				uint offset = 0;
				foreach (ByteBuffer fragment in sequence.Fragments) {
					offset += (uint)(8 + fragment.Length);
				}
				sequence.OffsetTable.Add(offset);

				int pos = 0;
				while (pos < data.Length) {
					int count = (int)Math.Min(FragmentSize, (uint)(data.Length - pos));
					ByteBuffer buffer = new ByteBuffer();
					buffer.Append(data, pos, count);
					sequence.Fragments.Add(buffer);
					pos += count;
				}
			}
			else {
				PixelDataElement.ByteBuffer.Append(data, 0, data.Length);
			}
		}
		#endregion

		#region Dataset Methods
		public void UpdateDataset(DcmDataset dataset) {
			dataset.AddElementWithValue(DcmTags.NumberOfFrames, _frames);
			dataset.AddElementWithValue(DcmTags.Columns, _width);
			dataset.AddElementWithValue(DcmTags.Rows, _height);
			dataset.AddElementWithValue(DcmTags.HighBit, _highBit);
			dataset.AddElementWithValue(DcmTags.BitsStored, _bitsStored);
			dataset.AddElementWithValue(DcmTags.BitsAllocated, _bitsAllocated);
			dataset.AddElementWithValue(DcmTags.SamplesPerPixel, _samplesPerPixel);
			dataset.AddElementWithValue(DcmTags.PixelRepresentation, _pixelRepresentation);
			dataset.AddElementWithValue(DcmTags.PlanarConfiguration, _planarConfiguration);
			dataset.AddElementWithValue(DcmTags.PhotometricInterpretation, _photometricInterpretation);
			if (SamplesPerPixel == 1) {
				dataset.AddElementWithValue(DcmTags.RescaleSlope, _rescaleSlope);
				dataset.AddElementWithValue(DcmTags.RescaleIntercept, _rescaleIntercept);
				//if (_pixelPaddingValue != 0)
				//    dataset.AddElementWithValue(DcmTags.PixelPaddingValue, _pixelPaddingValue);
			}
			dataset.AddItem(_pixelDataItem);
		}
		#endregion
		#endregion

		#region Protected Members
		protected void CreatePixelDataItem() {
			_frames = 0;
			if (IsEncapsulated) {
				_pixelDataItem = new DcmFragmentSequence(DcmTags.PixelData, DcmVR.OB);
			}
			else {
				if (_bitsAllocated > 8 && _bitsAllocated <= 16) {
					_pixelDataItem = new DcmOtherByte(DcmTags.PixelData);
				}
				else {
					_pixelDataItem = new DcmOtherWord(DcmTags.PixelData);
				}
			}
		}
		#endregion
	}
}
