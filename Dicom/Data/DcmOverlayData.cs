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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Data {
	public class DcmOverlayData {
		#region Private Members
		private ushort _group;

		private int _rows;
		private int _columns;
		private string _type;
		private int _originX;
		private int _originY;
		private int _bitsAllocated;
		private int _bitPosition;
		private byte[] _data;

		private string _description;
		private string _subtype;
		private string _label;

		private int _frames;
		private int _frameOrigin;
		#endregion

		#region Public Constructors
		public DcmOverlayData(DcmDataset ds, ushort group) {
			_group = group;
			Load(ds);
		}
		#endregion

		#region Public Properties
		public ushort Group {
			get { return _group; }
		}

		public int Rows {
			get { return _rows; }
		}

		public int Columns {
			get { return _columns; }
		}

		public string Type {
			get { return _type; }
		}

		public int OriginX {
			get { return _originX; }
		}

		public int OriginY {
			get { return _originY; }
		}

		public int BitsAllocated {
			get { return _bitsAllocated; }
		}

		public int BitPosition {
			get { return _bitPosition; }
		}

		public byte[] Data {
			get { return _data; }
		}

		public string Description {
			get { return _description; }
		}

		public string Subtype {
			get { return _subtype; }
		}

		public string Label {
			get { return _label; }
		}

		public int NumberOfFrames {
			get { return _frames; }
		}

		public int OriginFrame {
			get { return _frameOrigin; }
		}
		#endregion

		#region Public Members
		public int[] GetOverlayDataS32(int bg, int fg) {
			int[] overlay = new int[Rows * Columns];
			BitArray bits = new BitArray(_data);
			if (bits.Length < overlay.Length)
				throw new DcmDataException("Invalid overlay length: " + bits.Length);
			for (int i = 0, c = overlay.Length; i < c; i++) {
				if (bits.Get(i))
					overlay[i] = fg;
				else
					overlay[i] = bg;
			}
			return overlay;
		}

		public static DcmOverlayData[] GetOverlays(DcmDataset ds) {
			List<ushort> groups = new List<ushort>();
			foreach (DcmItem elem in ds.Elements) {
				if (elem.Tag.Element == 0x0010) {
					if (elem.Tag.Group >= 0x6000 && elem.Tag.Group <= 0x60FF) {
						groups.Add(elem.Tag.Group);
					}
				}
			}
			List<DcmOverlayData> overlays = new List<DcmOverlayData>();
			foreach (ushort group in groups) {
				DcmOverlayData overlay = new DcmOverlayData(ds, group);
				overlays.Add(overlay);
			}
			return overlays.ToArray();
		}
		#endregion

		#region Private Methods
		private DcmTag OverlayTag(DcmTag tag) {
			return new DcmTag(_group, tag.Element);
		}

		private void Load(DcmDataset ds) {
			_rows = ds.GetUInt16(OverlayTag(DcmTags.OverlayRows), 0);
			_columns = ds.GetUInt16(OverlayTag(DcmTags.OverlayColumns), 0);
			_type = ds.GetString(OverlayTag(DcmTags.OverlayType), "Unknown");

			DcmTag tag = OverlayTag(DcmTags.OverlayOrigin);
			if (ds.Contains(tag)) {
				short[] xy = ds.GetSS(tag).GetValues();
				if (xy != null && xy.Length == 2) {
					_originX = xy[0];
					_originY = xy[1];
				}
			}

			_bitsAllocated = ds.GetUInt16(OverlayTag(DcmTags.OverlayBitsAllocated), 1);
			_bitPosition = ds.GetUInt16(OverlayTag(DcmTags.OverlayBitPosition), 0);

			tag = OverlayTag(DcmTags.OverlayData);
			if (ds.Contains(tag)) {
				DcmElement elem = ds.GetElement(tag);
				_data = elem.ByteBuffer.ToBytes();
			}

			_description = ds.GetString(OverlayTag(DcmTags.OverlayDescription), String.Empty);
			_subtype = ds.GetString(OverlayTag(DcmTags.OverlaySubtype), String.Empty);
			_label = ds.GetString(OverlayTag(DcmTags.OverlayLabel), String.Empty);

			_frames = ds.GetInt32(OverlayTag(DcmTags.NumberOfFramesInOverlay), 1);
			_frameOrigin = ds.GetUInt16(OverlayTag(DcmTags.ImageFrameOrigin), 1);

			//TODO: include ROI
		}
		#endregion
	}
}