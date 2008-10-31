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
using System.IO;
using System.Text;

using Dicom.Data;
using Dicom.Utility;

namespace Dicom.IO {
	/// <summary>DICOM read status</summary>
	public enum DicomReadStatus {
		/// <summary>Read operation completed successfully</summary>
		Success,

		/// <summary>Unknown error occurred during read operation</summary>
		UnknownError,

		/// <summary>More data is needed to complete dataset</summary>
		NeedMoreData,

		/// <summary>Found the tag we were looking for (internal)</summary>
		SuccessEndRead
	}

	/// <summary>
	/// Reads a DICOM dataset from a stream
	/// </summary>
	public class DicomStreamReader {
		#region Private Members
		private const uint UndefinedLength = 0xFFFFFFFF;

		private Stream _stream = null;
		private BinaryReader _reader = null;
		private DcmTS _syntax = null;
		private Encoding _encoding = Encoding.ASCII;
		private Endian _endian;
		private bool _isFile;

		private DcmDataset _dataset;

		private uint _privateCreatorCard = 0xffffffff;
		private string _privateCreatorId = String.Empty;

		private DcmTag _tag = null;
		private DcmVR _vr = null;
		private uint _len = UndefinedLength;
		private long _pos = 0;
		private long _offset = 0;

		private long _bytes = 0;
		private long _read = 0;
		private uint _need = 0;
		private long _remain = 0;

		private Stack<DcmDataset> _sds = new Stack<DcmDataset>();
		private Stack<DcmItemSequence> _sqs = new Stack<DcmItemSequence>();
		private DcmFragmentSequence _fragment = null;
		#endregion

		#region Public Constructors
		/// <summary>
		/// Initializes a new DicomStreamReader with a source stream
		/// </summary>
		/// <param name="stream">Source stream</param>
		public DicomStreamReader(Stream stream) {
			_stream = stream;
			_isFile = _stream is FileStream;
			TransferSyntax = DcmTS.ExplicitVRLittleEndian;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Transfer syntax
		/// </summary>
		public DcmTS TransferSyntax {
			get { return _syntax; }
			set {
				_syntax = value;
				_endian = _syntax.Endian;
				_reader = EndianBinaryReader.Create(_stream, _encoding, _endian);
			}
		}

		/// <summary>
		/// String encoding
		/// </summary>
		public Encoding Encoding {
			get { return _encoding; }
			set {
				_encoding = value;
				TransferSyntax = _syntax;
			}
		}

		/// <summary>
		/// DICOM dataset
		/// </summary>
		public DcmDataset Dataset {
			get { return _dataset; }
			set {
				_dataset = value;
				TransferSyntax = _dataset.InternalTransferSyntax;
			}
		}

		/// <summary>
		/// Estimated size of dataset
		/// </summary>
		public long BytesEstimated {
			get { return _bytes + _need; }
		}

		/// <summary>
		/// Number of bytes read from stream
		/// </summary>
		public long BytesRead {
			get { return _read; }
		}

		/// <summary>
		/// Number of bytes remaining in stream
		/// </summary>
		public long BytesRemaining {
			get { return _remain; }
		}

		/// <summary>
		/// Number of bytes needed to complete current read operation
		/// </summary>
		public uint BytesNeeded {
			get { return _need; }
		}

		/// <summary>
		/// Offset of this stream relative to a parent stream.
		/// </summary>
		public long PositionOffset {
			get { return _offset; }
			set { _offset = value; }
		}
		#endregion

		private ByteBuffer CurrentBuffer(DicomReadOptions options) {
			if (_isFile) {
				bool delayLoad = false;
				if (_len >= 1024 && _vr != DcmVR.SQ) {
					if (Flags.IsSet(options, DicomReadOptions.DeferLoadingLargeElements))
						delayLoad = true;
					else if (Flags.IsSet(options, DicomReadOptions.DeferLoadingPixelData) && _tag == DcmTags.PixelData)
						delayLoad = true;
					else if (Flags.IsSet(options, DicomReadOptions.DeferLoadingPixelData) && _fragment != null && _fragment.Tag == DcmTags.PixelData)
						delayLoad = true;
				}

				if (delayLoad) {
					FileStream fs = (FileStream)_stream;
					FileSegment segment = new FileSegment(fs.Name, fs.Position, _len);
					_stream.Seek(_len, SeekOrigin.Current);
					return new ByteBuffer(segment, _endian);
				}
			}

			ByteBuffer bb = new ByteBuffer(_endian);
			bb.CopyFrom(_stream, (int)_len);
			return bb;
		}

		private DicomReadStatus NeedMoreData(long count) {
			_need = (uint)count;
			return DicomReadStatus.NeedMoreData;
		}

		/// <summary>
		/// Read dataset from stream
		/// </summary>
		/// <param name="stopAtTag">End parsing at this tag</param>
		/// <param name="options">DICOM read options</param>
		/// <returns>Status code</returns>
		public DicomReadStatus Read(DcmTag stopAtTag, DicomReadOptions options) {
			// Counters:
			//  _remain - bytes remaining in stream
			//  _bytes - estimates bytes to end of dataset
			//  _read - number of bytes read from stream
			try {
				_need = 0;
				_remain = _stream.Length - _stream.Position;

				while (_remain > 0) {
					DicomReadStatus status = ParseTag(stopAtTag, options);
					if (status == DicomReadStatus.SuccessEndRead)
						return DicomReadStatus.Success;
					if (status != DicomReadStatus.Success)
						return status;

					status = ParseVR(options);
					if (status != DicomReadStatus.Success)
						return status;

					status = ParseLength(options);
					if (status != DicomReadStatus.Success)
						return status;

					// handle UN private creator id
					if (_vr == DcmVR.UN && _tag.IsPrivate && _tag.Element <= 0x00ff)
						_vr = DcmVR.LO;

					if (_fragment != null) {
						InsertFragmentItem(options);
					}
					else if (_sqs.Count > 0 &&
								(_tag == DcmTags.Item ||
								 _tag == DcmTags.ItemDelimitationItem ||
								 _tag == DcmTags.SequenceDelimitationItem)) {
						InsertSequenceItem(options);
					}
					else {
						if (_sqs.Count > 0) {
							DcmItemSequence sq = _sqs.Peek();
							if (sq.StreamLength != UndefinedLength) {
								long end = sq.StreamPosition + 8 + sq.StreamLength;
								if (_syntax.IsExplicitVR)
									end += 2 + 2;
								if ((_stream.Position - _offset) >= end) {
									if (_sds.Count == _sqs.Count)
										_sds.Pop();
									_sqs.Pop();
								}
							}
						}

						if (_len == UndefinedLength) {
							if (_vr == DcmVR.SQ) {
								DcmItemSequence sq = new DcmItemSequence(_tag, _pos, _len, _endian);
								InsertDatasetItem(sq, options);
								_sqs.Push(sq);
							}
							else {
								_fragment = new DcmFragmentSequence(_tag, _vr, _pos, _endian);
								InsertDatasetItem(_fragment, options);
							}
						}
						else {
							if (_vr == DcmVR.SQ) {
								DcmItemSequence sq = new DcmItemSequence(_tag, _pos, _len, _endian);
								InsertDatasetItem(sq, options);
								_sqs.Push(sq);
							}
							else {
								if (_remain >= _len) {
									DcmElement elem = DcmElement.Create(_tag, _vr, _pos, _endian, CurrentBuffer(options));
									_remain -= _len;
									_read += _len;

									InsertDatasetItem(elem, options);
								}
								else {
									return NeedMoreData(_len - _remain);
								}
							}
						}
					}

					_tag = null;
					_vr = null;
					_len = UndefinedLength;
				}

				return DicomReadStatus.Success;
			}
			catch (EndOfStreamException) {
				// should never happen
				return DicomReadStatus.UnknownError;
			}
		}

		private DicomReadStatus ParseTag(DcmTag stopAtTag, DicomReadOptions options) {
			if (_tag == null) {
				if (_remain >= 4) {
					_pos = _stream.Position + _offset;
					ushort g = _reader.ReadUInt16();
					if (Flags.IsSet(options, DicomReadOptions.FileMetaInfoOnly) && g != 0x0002) {
						_stream.Seek(-2, SeekOrigin.Current);
						return DicomReadStatus.SuccessEndRead;
					}
					ushort e = _reader.ReadUInt16();
					if (DcmTag.IsPrivateGroup(g) && e > 0x00ff) {
						uint card = DcmTag.GetCard(g, e);
						if ((card & _privateCreatorCard) != _privateCreatorCard) {
							_privateCreatorCard = card & 0xffffff00;
							DcmTag pct = DcmTag.GetPrivateCreatorTag(g, e);
							DcmDataset ds = _dataset;
							if (_sds.Count > 0 && _sds.Count == _sqs.Count) {
								ds = _sds.Peek();
								if (!ds.Contains(pct))
									ds = _dataset;
							}
							_privateCreatorId = ds.GetString(pct, String.Empty);
						}
						_tag = new DcmTag(g, e, _privateCreatorId);
					}
					else {
						_tag = new DcmTag(g, e);

						if (g == 0xfffe) {
							if (_tag == DcmTags.Item ||
								_tag == DcmTags.ItemDelimitationItem ||
								_tag == DcmTags.SequenceDelimitationItem)
								_vr = DcmVR.NONE;
						}
					}
					_remain -= 4;
					_bytes += 4;
					_read += 4;
				}
				else {
					return NeedMoreData(4);
				}
			}

			if (_tag >= stopAtTag)
				return DicomReadStatus.SuccessEndRead;

			return DicomReadStatus.Success;
		}

		private DicomReadStatus ParseVR(DicomReadOptions options) {
			if (_vr == null) {
				if (_syntax.IsExplicitVR) {
					if (_remain >= 2) {
						_vr = DcmVRs.Lookup(_reader.ReadChars(2));
						_remain -= 2;
						_bytes += 2;
						_read += 2;
					}
					else {
						return NeedMoreData(2);
					}
				}
				else {
					if (_tag.Element == 0x0000)
						_vr = DcmVR.UL;
					else if (Flags.IsSet(options, DicomReadOptions.ForcePrivateCreatorToLO) &&
						_tag.IsPrivate && _tag.Element <= 0x00ff)
						_vr = DcmVR.UN;
					else
						_vr = _tag.Entry.DefaultVR;
				}

				if (_vr == DcmVR.UN) {
					if (_tag.Element == 0x0000)
						_vr = DcmVR.UL; // is this needed?
					else if (_tag.IsPrivate) {
						if (_tag.Element <= 0x00ff) {
							// private creator id
						}
						else if (_stream.CanSeek && Flags.IsSet(options, DicomReadOptions.AllowSeekingForContext)) {
							// attempt to identify private sequence
							long pos = _stream.Position;
							if (_syntax.IsExplicitVR) {
								if (_remain >= 2)
									_reader.ReadUInt16();
								else {
									_vr = null;
									_stream.Position = pos;
									return NeedMoreData(2);
								}
							}

							uint l = 0;
							if (_remain >= 4) {
								l = _reader.ReadUInt32();
								if (l == UndefinedLength)
									_vr = DcmVR.SQ;
							}
							else {
								_vr = null;
								_stream.Position = pos;
								return NeedMoreData(4);
							}

							if (l != 0 && _vr == DcmVR.UN) {
								if (_remain >= 4) {
									ushort g = _reader.ReadUInt16();
									ushort e = _reader.ReadUInt16();
									DcmTag tag = new DcmTag(g, e);
									if (tag == DcmTags.Item || tag == DcmTags.SequenceDelimitationItem)
										_vr = DcmVR.SQ;
								}
								else {
									_vr = null;
									_stream.Position = pos;
									return NeedMoreData(4);
								}
							}

							_stream.Position = pos;
						}
					}
					else if (!_syntax.IsExplicitVR || Flags.IsSet(options, DicomReadOptions.UseDictionaryForExplicitUN))
						_vr = _tag.Entry.DefaultVR;
				}
			}
			return DicomReadStatus.Success;
		}

		private DicomReadStatus ParseLength(DicomReadOptions options) {
			if (_len == UndefinedLength) {
				if (_syntax.IsExplicitVR) {
					if (_tag == DcmTags.Item ||
						_tag == DcmTags.ItemDelimitationItem ||
						_tag == DcmTags.SequenceDelimitationItem) {
						if (_remain >= 4) {
							_len = _reader.ReadUInt32();
							_remain -= 4;
							_bytes += 4;
							_read += 4;
						}
						else {
							return NeedMoreData(4);
						}
					}
					else {
						if (_vr.Is16BitLengthField) {
							if (_remain >= 2) {
								_len = (uint)_reader.ReadUInt16();
								_remain -= 2;
								_bytes += 2;
								_read += 2;
							}
							else {
								return NeedMoreData(2);
							}
						}
						else {
							if (_remain >= 6) {
								_reader.ReadByte();
								_reader.ReadByte();
								_len = _reader.ReadUInt32();
								_remain -= 6;
								_bytes += 6;
								_read += 6;
							}
							else {
								return NeedMoreData(6);
							}
						}
					}
				}
				else {
					if (_remain >= 4) {
						_len = _reader.ReadUInt32();
						_remain -= 4;
						_bytes += 4;
						_read += 4;
					}
					else {
						return NeedMoreData(4);
					}
				}

				if (_len != UndefinedLength) {
					if (_vr != DcmVR.SQ && !(_tag.Equals(DcmTags.Item) && _fragment == null))
						_bytes += _len;
				}
			}
			return DicomReadStatus.Success;
		}

		private DicomReadStatus InsertFragmentItem(DicomReadOptions options) {
			if (_tag == DcmTags.Item) {
				if (_remain >= _len) {
					ByteBuffer data = CurrentBuffer(options);
					_remain -= _len;
					_read += _len;

					if (!_fragment.HasOffsetTable)
						_fragment.SetOffsetTable(data);
					else
						_fragment.AddFragment(data);

				}
				else {
					return NeedMoreData(_remain - _len);
				}
			}
			else if (_tag == DcmTags.SequenceDelimitationItem) {
				_fragment = null;
			}
			else {
				// unexpected tag
				return DicomReadStatus.UnknownError;
			}
			return DicomReadStatus.Success;
		}

		private DicomReadStatus InsertSequenceItem(DicomReadOptions options) {
			if (_tag.Equals(DcmTags.Item)) {
				if (_len != UndefinedLength && _len > _remain)
					return NeedMoreData(_remain - _len);

				if (_sds.Count > _sqs.Count)
					_sds.Pop();

				DcmDataset ds = new DcmDataset(_pos + 8, _len, TransferSyntax);
				DcmItemSequenceItem si = new DcmItemSequenceItem(_pos, _len);
				si.Dataset = ds;
				_sqs.Peek().AddSequenceItem(si);

				if (_len != UndefinedLength) {
					SegmentStream ss = new SegmentStream(_stream, _stream.Position, _len);
					_remain -= _len;
					_read += _len;

					DicomStreamReader idsr = new DicomStreamReader(ss);
					idsr.Dataset = ds;
					idsr.PositionOffset = ds.StreamPosition;
					DicomReadStatus status = idsr.Read(null, options);
					if (status != DicomReadStatus.Success)
						return status;
				}
				else {
					_sds.Push(ds);
				}
			}
			else if (_tag == DcmTags.ItemDelimitationItem) {
				if (_sds.Count == _sqs.Count)
					_sds.Pop();
			}
			else if (_tag == DcmTags.SequenceDelimitationItem) {
				if (_sds.Count == _sqs.Count)
					_sds.Pop();
				_sqs.Pop();
			}
			return DicomReadStatus.Success;
		}

		private void InsertDatasetItem(DcmItem item, DicomReadOptions options) {
			if (_sds.Count > 0 && _sds.Count == _sqs.Count) {
				DcmDataset ds = _sds.Peek();

				if (_tag.Element == 0x0000) {
					if (Flags.IsSet(options, DicomReadOptions.KeepGroupLengths))
						ds.AddItem(item);
				}
				else
					ds.AddItem(item);

				if (ds.StreamLength != UndefinedLength) {
					long end = ds.StreamPosition + ds.StreamLength;
					if ((_stream.Position - _offset) >= end)
						_sds.Pop();
				}
			}
			else {
				if (_tag.Element == 0x0000) {
					if (Flags.IsSet(options, DicomReadOptions.KeepGroupLengths))
						_dataset.AddItem(item);
				}
				else
					_dataset.AddItem(item);
			}
		}
	}
}
