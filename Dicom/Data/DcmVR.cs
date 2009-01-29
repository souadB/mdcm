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

namespace Dicom.Data {
	public enum VrRestriction {
		NotApplicable,
		Fixed,
		Maximum,
		Any
	}

	public class DcmVR {
		#region Private Members
		private string _strval;
		private int _hash;
		private string _desc;
		private bool _is16BitLength;
		private bool _isString;
		private bool _isEncodedString;
		private byte _padding;
		private int _maxLength;
		private int _unitSize;
		private VrRestriction _restriction;

		private DcmVR() {
		}

		internal DcmVR(string value, string desc, bool isString, bool isEncodedString, bool is16BitLength, 
			byte padding, int maxLength, int unitSize, VrRestriction restriction) {
			_strval = value;
			_hash = (((int)value[0] << 8) | (int)value[1]);
			_desc = desc;
			_isString = isString;
			_isEncodedString = isEncodedString;
			_is16BitLength = is16BitLength;
			//_padding = padding;
			_padding = PadZero;
			_maxLength = maxLength;
			_unitSize = unitSize;
			_restriction = restriction;
		}
		#endregion

		#region Public Properties
		public string VR {
			get { return _strval; }
		}

		public string Description {
			get { return _desc; }
		}

		public bool IsString {
			get { return _isString; }
		}

		/// <summary>
		/// Specific Character Set applies to this VR
		/// </summary>
		public bool IsEncodedString {
			get { return _isEncodedString; }
		}

		public bool Is16BitLengthField {
			get { return _is16BitLength; }
		}

		public byte Padding {
			get { return _padding; }
		}

		public int MaxmimumLength {
			get { return _maxLength; }
		}

		public int UnitSize {
			get { return _unitSize; }
		}

		public VrRestriction Restriction {
			get { return _restriction; }
		}
		#endregion

		public override string ToString() {
			return String.Format("{0} - {1}", VR, Description);
		}

		private const byte PadSpace = (byte)' ';
		private const byte PadZero = 0;

		public static DcmVR NONE = new DcmVR("NONE", "No VR", false, false, false, PadZero, 0, 0, VrRestriction.NotApplicable);
		public static DcmVR AE = new DcmVR("AE", "Application Entity", true, false, true, PadSpace, 16, 1, VrRestriction.Maximum);
		public static DcmVR AS = new DcmVR("AS", "Age String", true, false, true, PadSpace, 4, 1, VrRestriction.Fixed);
		public static DcmVR AT = new DcmVR("AT", "Attribute Tag", false, false, true, PadZero, 4, 4, VrRestriction.Fixed);
		public static DcmVR CS = new DcmVR("CS", "Code String", true, false, true, PadSpace, 16, 1, VrRestriction.Maximum);
		public static DcmVR DA = new DcmVR("DA", "Date", true, false, true, PadSpace, 8, 1, VrRestriction.Fixed);
		public static DcmVR DS = new DcmVR("DS", "Decimal String", true, false, true, PadSpace, 16, 1, VrRestriction.Maximum);
		public static DcmVR DT = new DcmVR("DT", "Date Time", true, false, true, PadSpace, 26, 1, VrRestriction.Maximum);
		public static DcmVR FD = new DcmVR("FD", "Floating Point Double", false, false, true, PadZero, 8, 8, VrRestriction.Fixed);
		public static DcmVR FL = new DcmVR("FL", "Floating Point Single", false, false, true, PadZero, 4, 4, VrRestriction.Fixed);
		public static DcmVR IS = new DcmVR("IS", "Integer String", true, false, true, PadSpace, 12, 1, VrRestriction.Maximum);
		public static DcmVR LO = new DcmVR("LO", "Long String", true, true, true, PadSpace, 64, 1, VrRestriction.Maximum);
		public static DcmVR LT = new DcmVR("LT", "Long Text", true, true, true, PadSpace, 10240, 1, VrRestriction.Maximum);
		public static DcmVR OB = new DcmVR("OB", "Other Byte", false, false, false, PadZero, 0, 1, VrRestriction.Any);
		public static DcmVR OF = new DcmVR("OF", "Other Float", false, false, false, PadZero, 0, 4, VrRestriction.Any);
		public static DcmVR OW = new DcmVR("OW", "Other Word", false, false, false, PadZero, 0, 2, VrRestriction.Any);
		public static DcmVR PN = new DcmVR("PN", "Person Name", true, true, true, PadSpace, 64, 1, VrRestriction.Maximum);
		public static DcmVR SH = new DcmVR("SH", "Short String", true, true, true, PadSpace, 16, 1, VrRestriction.Maximum);
		public static DcmVR SL = new DcmVR("SL", "Signed Long", false, false, true, PadZero, 4, 4, VrRestriction.Fixed);
		public static DcmVR SQ = new DcmVR("SQ", "Sequence of Items", false, false, false, PadZero, 0, 0, VrRestriction.NotApplicable);
		public static DcmVR SS = new DcmVR("SS", "Signed Short", false, false, true, PadZero, 2, 2, VrRestriction.Fixed);
		public static DcmVR ST = new DcmVR("ST", "Short Text", false, true, true, PadSpace, 1024, 1, VrRestriction.Maximum);
		public static DcmVR TM = new DcmVR("TM", "Time", true, false, true, PadSpace, 16, 1, VrRestriction.Maximum);
		public static DcmVR UI = new DcmVR("UI", "Unique Identifier", true, false, true, PadSpace, 64, 1, VrRestriction.Maximum);
		public static DcmVR UL = new DcmVR("UL", "Unsigned Long", false, false, true, PadZero, 4, 4, VrRestriction.Fixed);
		public static DcmVR UN = new DcmVR("UN", "Unknown", false, false, false, PadZero, 0, 1, VrRestriction.Any);
		public static DcmVR US = new DcmVR("US", "Unsigned Short", false, false, true, PadZero, 2, 2, VrRestriction.Fixed);
		public static DcmVR UT = new DcmVR("UT", "Unlimited Text", true, true, false, PadSpace, 0, 1, VrRestriction.Any);
	}

	public static class DcmVRs {
		public static List<DcmVR> Entries = new List<DcmVR>();

		static DcmVRs() {
			#region Load VRs
			Entries.Add(DcmVR.AE);
			Entries.Add(DcmVR.AS);
			Entries.Add(DcmVR.AT);
			Entries.Add(DcmVR.CS);
			Entries.Add(DcmVR.DA);
			Entries.Add(DcmVR.DS);
			Entries.Add(DcmVR.DT);
			Entries.Add(DcmVR.FD);
			Entries.Add(DcmVR.FL);
			Entries.Add(DcmVR.IS);
			Entries.Add(DcmVR.LO);
			Entries.Add(DcmVR.LT);
			Entries.Add(DcmVR.OB);
			Entries.Add(DcmVR.OF);
			Entries.Add(DcmVR.OW);
			Entries.Add(DcmVR.PN);
			Entries.Add(DcmVR.SH);
			Entries.Add(DcmVR.SL);
			Entries.Add(DcmVR.SQ);
			Entries.Add(DcmVR.SS);
			Entries.Add(DcmVR.ST);
			Entries.Add(DcmVR.TM);
			Entries.Add(DcmVR.UI);
			Entries.Add(DcmVR.UL);
			Entries.Add(DcmVR.UN);
			Entries.Add(DcmVR.US);
			Entries.Add(DcmVR.UT);
			#endregion
		}

		public static DcmVR Lookup(ushort vr) {
			if (vr == 0x0000)
				return DcmVR.NONE;
			return Lookup(new char[] { (char)(vr >> 8), (char)(vr) });
		}

		public static DcmVR Lookup(char[] vr) {
			return Lookup(new String(vr));
		}

		public static DcmVR Lookup(string vr) {
			switch (vr) {
			case "NONE": return DcmVR.NONE;
			case "AE": return DcmVR.AE;
			case "AS": return DcmVR.AS;
			case "AT": return DcmVR.AT;
			case "CS": return DcmVR.CS;
			case "DA": return DcmVR.DA;
			case "DS": return DcmVR.DS;
			case "DT": return DcmVR.DT;
			case "FD": return DcmVR.FD;
			case "FL": return DcmVR.FL;
			case "IS": return DcmVR.IS;
			case "LO": return DcmVR.LO;
			case "LT": return DcmVR.LT;
			case "OB": return DcmVR.OB;
			case "OF": return DcmVR.OF;
			case "OW": return DcmVR.OW;
			case "PN": return DcmVR.PN;
			case "SH": return DcmVR.SH;
			case "SL": return DcmVR.SL;
			case "SQ": return DcmVR.SQ;
			case "SS": return DcmVR.SS;
			case "ST": return DcmVR.ST;
			case "TM": return DcmVR.TM;
			case "UI": return DcmVR.UI;
			case "UL": return DcmVR.UL;
			case "UN": return DcmVR.UN;
			case "US": return DcmVR.US;
			case "UT": return DcmVR.UT;
			default:
				return DcmVR.UN;
			}
		}
	}
}
