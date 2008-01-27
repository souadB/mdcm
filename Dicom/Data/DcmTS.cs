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

using Dicom.IO;

namespace Dicom.Data {
	public class DcmTS {
		public readonly DcmUID UID;
		public readonly bool IsBigEndian;
		public readonly bool IsExplicitVR;
		public readonly bool IsEncapsulated;
		public readonly bool IsLossy;
		public readonly bool IsDeflate;
		public readonly Endian Endian;

		public DcmTS(DcmUID uid, bool be, bool evr, bool encaps, bool lssy, bool dflt) {
			UID = uid;
			IsBigEndian = be;
			IsExplicitVR = evr;
			IsEncapsulated = encaps;
			IsLossy = lssy;
			IsDeflate = dflt;
			Endian = IsBigEndian ? Endian.Big : Endian.Little;
		}

		public override string ToString() {
			return UID.Description;
		}

		public override bool Equals(object obj) {
			if (obj is DcmTS)
				return ((DcmTS)obj).UID.Equals(UID);
			if (obj is DcmUID)
				return ((DcmUID)obj).Equals(UID);
			if (obj is String)
				return UID.Equals((String)obj);
			return false;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		#region Dicom Transfer Syntax
		public static DcmTS JPEGXRPrivate = new DcmTS(DcmUIDs.JPEGXRPrivate, false, true, true, false, false);

		public static DcmTS PNGPrivate = new DcmTS(DcmUIDs.PNGPrivate, false, true, true, false, false);

		/// <summary>Implicit VR Little Endian</summary>
		public static DcmTS ImplicitVRLittleEndian = new DcmTS(DcmUIDs.ImplicitVRLittleEndian, false, false, false, false, false);

		/// <summary>Explicit VR Little Endian</summary>
		public static DcmTS ExplicitVRLittleEndian = new DcmTS(DcmUIDs.ExplicitVRLittleEndian, false, true, false, false, false);

		/// <summary>Explicit VR Big Endian</summary>
		public static DcmTS ExplicitVRBigEndian = new DcmTS(DcmUIDs.ExplicitVRBigEndian, true, true, false, false, false);

		/// <summary>Deflated Explicit VR Little Endian</summary>
		public static DcmTS DeflatedExplicitVRLittleEndian = new DcmTS(DcmUIDs.DeflatedExplicitVRLittleEndian, false, true, false, false, true);

		/// <summary>JPEG Baseline (Process 1)</summary>
		public static DcmTS JPEGProcess1 = new DcmTS(DcmUIDs.JPEGProcess1, false, true, true, true, false);

		/// <summary>JPEG Extended (Process 2 &amp; 4)</summary>
		public static DcmTS JPEGProcess2_4 = new DcmTS(DcmUIDs.JPEGProcess2_4, false, true, true, true, false);

		/// <summary>JPEG Extended (Process 3 &amp; 5) (Retired)</summary>
		public static DcmTS JPEGProcess3_5Retired = new DcmTS(DcmUIDs.JPEGProcess3_5Retired, false, true, true, true, false);

		/// <summary>JPEG Spectral Selection, Non-Hierarchical (Process 6 &amp; 8) (Retired)</summary>
		public static DcmTS JPEGProcess6_8Retired = new DcmTS(DcmUIDs.JPEGProcess6_8Retired, false, true, true, true, false);

		/// <summary>JPEG Spectral Selection, Non-Hierarchical (Process 7 &amp; 9) (Retired)</summary>
		public static DcmTS JPEGProcess7_9Retired = new DcmTS(DcmUIDs.JPEGProcess7_9Retired, false, true, true, true, false);

		/// <summary>JPEG Full Progression, Non-Hierarchical (Process 10 &amp; 12) (Retired)</summary>
		public static DcmTS JPEGProcess10_12Retired = new DcmTS(DcmUIDs.JPEGProcess10_12Retired, false, true, true, true, false);

		/// <summary>JPEG Full Progression, Non-Hierarchical (Process 11 &amp; 13) (Retired)</summary>
		public static DcmTS JPEGProcess11_13Retired = new DcmTS(DcmUIDs.JPEGProcess11_13Retired, false, true, true, true, false);

		/// <summary>JPEG Lossless, Non-Hierarchical (Process 14)</summary>
		public static DcmTS JPEGProcess14 = new DcmTS(DcmUIDs.JPEGProcess14, false, true, true, false, false);

		/// <summary>JPEG Lossless, Non-Hierarchical (Process 15) (Retired)</summary>
		public static DcmTS JPEGProcess15Retired = new DcmTS(DcmUIDs.JPEGProcess15Retired, false, true, true, false, false);

		/// <summary>JPEG Extended, Hierarchical (Process 16 &amp; 18) (Retired)</summary>
		public static DcmTS JPEGProcess16_18Retired = new DcmTS(DcmUIDs.JPEGProcess16_18Retired, false, true, true, true, false);

		/// <summary>JPEG Extended, Hierarchical (Process 17 &amp; 19) (Retired)</summary>
		public static DcmTS JPEGProcess17_19Retired = new DcmTS(DcmUIDs.JPEGProcess17_19Retired, false, true, true, true, false);

		/// <summary>JPEG Spectral Selection, Hierarchical (Process 20 &amp; 22) (Retired)</summary>
		public static DcmTS JPEGProcess20_22Retired = new DcmTS(DcmUIDs.JPEGProcess20_22Retired, false, true, true, true, false);

		/// <summary>JPEG Spectral Selection, Hierarchical (Process 21 &amp; 23) (Retired)</summary>
		public static DcmTS JPEGProcess21_23Retired = new DcmTS(DcmUIDs.JPEGProcess21_23Retired, false, true, true, true, false);

		/// <summary>JPEG Full Progression, Hierarchical (Process 24 &amp; 26) (Retired)</summary>
		public static DcmTS JPEGProcess24_26Retired = new DcmTS(DcmUIDs.JPEGProcess24_26Retired, false, true, true, true, false);

		/// <summary>JPEG Full Progression, Hierarchical (Process 25 &amp; 27) (Retired)</summary>
		public static DcmTS JPEGProcess25_27Retired = new DcmTS(DcmUIDs.JPEGProcess25_27Retired, false, true, true, true, false);

		/// <summary>JPEG Lossless, Hierarchical (Process 28) (Retired)</summary>
		public static DcmTS JPEGProcess28Retired = new DcmTS(DcmUIDs.JPEGProcess28Retired, false, true, true, false, false);

		/// <summary>JPEG Lossless, Hierarchical (Process 29) (Retired)</summary>
		public static DcmTS JPEGProcess29Retired = new DcmTS(DcmUIDs.JPEGProcess29Retired, false, true, true, false, false);

		/// <summary>JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1])</summary>
		public static DcmTS JPEGProcess14SV1 = new DcmTS(DcmUIDs.JPEGProcess14SV1, false, true, true, false, false);

		/// <summary>JPEG-LS Lossless Image Compression</summary>
		public static DcmTS JPEGLSLossless = new DcmTS(DcmUIDs.JPEGLSLossless, false, true, true, false, false);

		/// <summary>JPEG-LS Lossy (Near-Lossless) Image Compression</summary>
		public static DcmTS JPEGLSNearLossless = new DcmTS(DcmUIDs.JPEGLSNearLossless, false, true, true, true, false);

		/// <summary>JPEG 2000 Lossless Image Compression</summary>
		public static DcmTS JPEG2000Lossless = new DcmTS(DcmUIDs.JPEG2000Lossless, false, true, true, false, false);

		/// <summary>JPEG 2000 Lossy Image Compression</summary>
		public static DcmTS JPEG2000Lossy = new DcmTS(DcmUIDs.JPEG2000Lossy, false, true, true, true, false);

		/// <summary>MPEG2 Main Profile @ Main Level</summary>
		public static DcmTS MPEG2 = new DcmTS(DcmUIDs.MPEG2, false, true, true, true, false);

		/// <summary>RLE Lossless</summary>
		public static DcmTS RLELossless = new DcmTS(DcmUIDs.RLELossless, false, true, true, false, false);
		#endregion
	}

	public static class DcmTSs {
		public static List<DcmTS> Entries = new List<DcmTS>();

		static DcmTSs() {
			#region Load Transfer Syntax List
			Entries.Add(DcmTS.JPEGXRPrivate);
			Entries.Add(DcmTS.PNGPrivate);
			Entries.Add(DcmTS.ImplicitVRLittleEndian);
			Entries.Add(DcmTS.ExplicitVRLittleEndian);
			Entries.Add(DcmTS.ExplicitVRBigEndian);
			Entries.Add(DcmTS.DeflatedExplicitVRLittleEndian);
			Entries.Add(DcmTS.JPEGProcess1);
			Entries.Add(DcmTS.JPEGProcess2_4);
			Entries.Add(DcmTS.JPEGProcess3_5Retired);
			Entries.Add(DcmTS.JPEGProcess6_8Retired);
			Entries.Add(DcmTS.JPEGProcess7_9Retired);
			Entries.Add(DcmTS.JPEGProcess10_12Retired);
			Entries.Add(DcmTS.JPEGProcess11_13Retired);
			Entries.Add(DcmTS.JPEGProcess14);
			Entries.Add(DcmTS.JPEGProcess15Retired);
			Entries.Add(DcmTS.JPEGProcess16_18Retired);
			Entries.Add(DcmTS.JPEGProcess17_19Retired);
			Entries.Add(DcmTS.JPEGProcess20_22Retired);
			Entries.Add(DcmTS.JPEGProcess21_23Retired);
			Entries.Add(DcmTS.JPEGProcess24_26Retired);
			Entries.Add(DcmTS.JPEGProcess25_27Retired);
			Entries.Add(DcmTS.JPEGProcess28Retired);
			Entries.Add(DcmTS.JPEGProcess29Retired);
			Entries.Add(DcmTS.JPEGProcess14SV1);
			Entries.Add(DcmTS.JPEGLSLossless);
			Entries.Add(DcmTS.JPEGLSNearLossless);
			Entries.Add(DcmTS.JPEG2000Lossless);
			Entries.Add(DcmTS.JPEG2000Lossy);
			Entries.Add(DcmTS.MPEG2);
			Entries.Add(DcmTS.RLELossless);
			#endregion
		}

		public static DcmTS Lookup(String uid) {
			return Lookup(DcmUIDs.Lookup(uid));
		}

		public static DcmTS Lookup(DcmUID uid) {
			foreach (DcmTS ts in Entries) {
				if (ts.Equals(uid))
					return ts;
			}
			return new DcmTS(uid, false, true, true, false, false);
		}
	}
}
