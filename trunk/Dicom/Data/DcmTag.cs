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
using System.Globalization;
using System.Text;

namespace Dicom.Data {
	public class DcmTag {
		#region Private Members
		private ushort _g;
		private ushort _e;
		private uint _c;
		private string _p;
		private DcmDictionaryEntry _entry;
		#endregion

		#region Public Constructors
		public DcmTag(uint card) {
			_g = (ushort)(card >> 16);
			_e = (ushort)(card & 0xffff);
			_c = card;
			_p = String.Empty;
		}

		public DcmTag(ushort group, ushort element) {
			_g = group;
			_e = element;
			_c = (uint)_g << 16 | (uint)_e;
			_p = String.Empty;
		}

		public DcmTag(ushort group, ushort element, string creator) {
			_g = group;
			_e = element;
			_c = (uint)_g << 16 | (uint)_e;
			_p = creator ?? String.Empty;
		}
		#endregion

		#region Public Properties
		public ushort Group {
			get { return _g; }
		}
		public ushort Element {
			get { return _e; }
		}
		public uint Card {
			get { return _c; }
		}
		public string PrivateCreator {
			get { return _p; }
		}
		public DcmDictionaryEntry Entry {
			get {
				if (_entry == null)
					_entry = DcmDictionary.Lookup(this);
				return _entry;
			}
		}
		public bool IsPrivate {
			get { return (_g & 1) == 1; }
		}
		#endregion

		#region Operators
		public static bool operator ==(DcmTag t1, DcmTag t2) {
			if ((object)t1 == null && (object)t2 == null)
				return true;
			if ((object)t1 == null || (object)t2 == null)
				return false;
			return t1.Card == t2.Card;
		}
		public static bool operator !=(DcmTag t1, DcmTag t2) {
			return !(t1 == t2);
		}
		public static bool operator <(DcmTag t1, DcmTag t2) {
			if ((object)t1 == null || (object)t2 == null)
				return false;
			if (t1.Group == t2.Group && t1.Element < t2.Element)
				return true;
			if (t1.Group < t2.Group)
				return true;
			return false;
		}
		public static bool operator >(DcmTag t1, DcmTag t2) {
			return !(t1 < t2);
		}
		public static bool operator <=(DcmTag t1, DcmTag t2) {
			if ((object)t1 == null || (object)t2 == null)
				return false;
			if (t1.Group == t2.Group && t1.Element <= t2.Element)
				return true;
			if (t1.Group < t2.Group)
				return true;
			return false;
		}
		public static bool operator >=(DcmTag t1, DcmTag t2) {
			if ((object)t1 == null || (object)t2 == null)
				return false;
			if (t1.Group == t2.Group && t1.Element >= t2.Element)
				return true;
			if (t1.Group > t2.Group)
				return true;
			return false;
		}

		public override bool Equals(object obj) {
			if (obj is DcmTag) {
				return ((DcmTag)obj).Card == Card;
			}
			return false;
		}

		public override int GetHashCode() {
			return (int)Card;
		}
		#endregion

		public static uint GetCard(ushort group, ushort element) {
			return (uint)group << 16 | (uint)element;
		}

		public static bool IsPrivateGroup(ushort group) {
			return (group & 1) == 1;
		}

		public static DcmTag GetPrivateCreatorTag(ushort group, ushort element) {
			return new DcmTag(group, (ushort)(element >> 8));
		}

		public override string ToString() {
			return String.Format("({0:x4},{1:x4})", _g, _e);
		}

		public static DcmTag Parse(string tag) {
			try {
				string[] parts = tag.Split(',');

				if (parts.Length != 2) {
					if (tag.Length == 8) {
						parts = new string[2];
						parts[0] = tag.Substring(0, 4);
						parts[1] = tag.Substring(4, 4);
					}
					else
						return null;
				}

				parts[0] = parts[0].Trim();
				parts[1] = parts[1].Trim();

				if (parts[0].Length != 4 ||
					parts[1].Length != 4)
					return null;

				ushort g = ushort.Parse(parts[0], NumberStyles.HexNumber);
				ushort e = ushort.Parse(parts[1], NumberStyles.HexNumber);

				return new DcmTag(g, e);
			}
			catch {
				return null;
			}
		}
	}

	public static class DcmTags {
		#region Internal Dictionary Quick Tags
		/// <summary>(0000,0001) VR=UL Command Length to End (Retired)</summary>
		public static DcmTag CommandLengthToEndRETIRED = new DcmTag(0x0000, 0x0001);

		/// <summary>(0000,0002) VR=UI Affected SOP Class UID</summary>
		public static DcmTag AffectedSOPClassUID = new DcmTag(0x0000, 0x0002);

		/// <summary>(0000,0003) VR=UI Requested SOP Class UID</summary>
		public static DcmTag RequestedSOPClassUID = new DcmTag(0x0000, 0x0003);

		/// <summary>(0000,0010) VR=SH Command Recognition Code (Retired)</summary>
		public static DcmTag CommandRecognitionCodeRETIRED = new DcmTag(0x0000, 0x0010);

		/// <summary>(0000,0100) VR=US Command Field</summary>
		public static DcmTag CommandField = new DcmTag(0x0000, 0x0100);

		/// <summary>(0000,0110) VR=US Message ID (first)</summary>
		public static DcmTag MessageID = new DcmTag(0x0000, 0x0110);

		/// <summary>(0000,0120) VR=US Message ID Responded to</summary>
		public static DcmTag MessageIDRespondedTo = new DcmTag(0x0000, 0x0120);

		/// <summary>(0000,0600) VR=AE Move Destination</summary>
		public static DcmTag MoveDestination = new DcmTag(0x0000, 0x0600);

		/// <summary>(0000,0700) VR=US Priority</summary>
		public static DcmTag Priority = new DcmTag(0x0000, 0x0700);

		/// <summary>(0000,0800) VR=US Data Set Type</summary>
		public static DcmTag DataSetType = new DcmTag(0x0000, 0x0800);

		/// <summary>(0000,0900) VR=US Status</summary>
		public static DcmTag Status = new DcmTag(0x0000, 0x0900);

		/// <summary>(0000,0901) VR=AT Offending Element</summary>
		public static DcmTag OffendingElement = new DcmTag(0x0000, 0x0901);

		/// <summary>(0000,0902) VR=LO Error Comment</summary>
		public static DcmTag ErrorComment = new DcmTag(0x0000, 0x0902);

		/// <summary>(0000,0903) VR=US Error ID</summary>
		public static DcmTag ErrorID = new DcmTag(0x0000, 0x0903);

		/// <summary>(0000,1000) VR=UI SOP Affected Instance UID</summary>
		public static DcmTag AffectedSOPInstanceUID = new DcmTag(0x0000, 0x1000);

		/// <summary>(0000,1001) VR=UI SOP Requested Instance UID</summary>
		public static DcmTag RequestedSOPInstanceUID = new DcmTag(0x0000, 0x1001);

		/// <summary>(0000,1002) VR=US Event Type ID</summary>
		public static DcmTag EventTypeID = new DcmTag(0x0000, 0x1002);

		/// <summary>(0000,1005) VR=AT Attribute Identifier List</summary>
		public static DcmTag AttributeIdentifierList = new DcmTag(0x0000, 0x1005);

		/// <summary>(0000,1008) VR=US Action Type ID</summary>
		public static DcmTag ActionTypeID = new DcmTag(0x0000, 0x1008);

		/// <summary>(0000,1020) VR=US Remaining Suboperations</summary>
		public static DcmTag RemainingSuboperations = new DcmTag(0x0000, 0x1020);

		/// <summary>(0000,1021) VR=US Completed Suboperations</summary>
		public static DcmTag CompletedSuboperations = new DcmTag(0x0000, 0x1021);

		/// <summary>(0000,1022) VR=US Failed Suboperations</summary>
		public static DcmTag FailedSuboperations = new DcmTag(0x0000, 0x1022);

		/// <summary>(0000,1023) VR=US Warning Suboperations</summary>
		public static DcmTag WarningSuboperations = new DcmTag(0x0000, 0x1023);

		/// <summary>(0000,1030) VR=AE Move Originator AE Title</summary>
		public static DcmTag MoveOriginator = new DcmTag(0x0000, 0x1030);

		/// <summary>(0000,1031) VR=US Message ID (second)</summary>
		public static DcmTag MoveOriginatorMessageID = new DcmTag(0x0000, 0x1031);

		/// <summary>(0002,0001) VR=OB File Meta Information Version</summary>
		public static DcmTag FileMetaInformationVersion = new DcmTag(0x0002, 0x0001);

		/// <summary>(0002,0002) VR=UI Media Storage SOP Class UID</summary>
		public static DcmTag MediaStorageSOPClassUID = new DcmTag(0x0002, 0x0002);

		/// <summary>(0002,0003) VR=UI Media Storage SOP Instance UID</summary>
		public static DcmTag MediaStorageSOPInstanceUID = new DcmTag(0x0002, 0x0003);

		/// <summary>(0002,0010) VR=UI Transfer Syntax UID</summary>
		public static DcmTag TransferSyntaxUID = new DcmTag(0x0002, 0x0010);

		/// <summary>(0002,0012) VR=UI Implementation Class UID</summary>
		public static DcmTag ImplementationClassUID = new DcmTag(0x0002, 0x0012);

		/// <summary>(0002,0013) VR=SH Implementation Version Name</summary>
		public static DcmTag ImplementationVersionName = new DcmTag(0x0002, 0x0013);

		/// <summary>(0002,0016) VR=AE Source Application Entity Title</summary>
		public static DcmTag SourceApplicationEntityTitle = new DcmTag(0x0002, 0x0016);

		/// <summary>(0002,0100) VR=UI Private Information Creator UID</summary>
		public static DcmTag PrivateInformationCreatorUID = new DcmTag(0x0002, 0x0100);

		/// <summary>(0002,0102) VR=OB Private Information</summary>
		public static DcmTag PrivateInformation = new DcmTag(0x0002, 0x0102);

		/// <summary>(0004,1130) VR=CS File-set ID</summary>
		public static DcmTag FilesetID = new DcmTag(0x0004, 0x1130);

		/// <summary>(0004,1141) VR=CS File-set Descriptor File ID</summary>
		public static DcmTag FilesetDescriptorFileID = new DcmTag(0x0004, 0x1141);

		/// <summary>(0004,1142) VR=CS Specific Character Set of File-set Descriptor File</summary>
		public static DcmTag SpecificCharacterSetOfFilesetDescriptorFile = new DcmTag(0x0004, 0x1142);

		/// <summary>(0004,1200) VR=UL Offset of the First Directory Record of the Root Directory Entity</summary>
		public static DcmTag OffsetOfTheFirstDirectoryRecordOfTheRootDirectoryEntity = new DcmTag(0x0004, 0x1200);

		/// <summary>(0004,1202) VR=UL Offset of the Last Directory Record of the Root Directory Entity</summary>
		public static DcmTag OffsetOfTheLastDirectoryRecordOfTheRootDirectoryEntity = new DcmTag(0x0004, 0x1202);

		/// <summary>(0004,1212) VR=US File-set Consistency Flag</summary>
		public static DcmTag FilesetConsistencyFlag = new DcmTag(0x0004, 0x1212);

		/// <summary>(0004,1220) VR=SQ Directory Record Sequence</summary>
		public static DcmTag DirectoryRecordSequence = new DcmTag(0x0004, 0x1220);

		/// <summary>(0004,1400) VR=UL Offset of the Next Directory Record</summary>
		public static DcmTag OffsetOfTheNextDirectoryRecord = new DcmTag(0x0004, 0x1400);

		/// <summary>(0004,1410) VR=US Record In-use Flag</summary>
		public static DcmTag RecordInuseFlag = new DcmTag(0x0004, 0x1410);

		/// <summary>(0004,1420) VR=UL Offset of Referenced Lower-Level Directory Entity</summary>
		public static DcmTag OffsetOfReferencedLowerLevelDirectoryEntity = new DcmTag(0x0004, 0x1420);

		/// <summary>(0004,1430) VR=CS Directory Record Type</summary>
		public static DcmTag DirectoryRecordType = new DcmTag(0x0004, 0x1430);

		/// <summary>(0004,1432) VR=UI Private Record UID</summary>
		public static DcmTag PrivateRecordUID = new DcmTag(0x0004, 0x1432);

		/// <summary>(0004,1500) VR=CS Referenced File ID</summary>
		public static DcmTag ReferencedFileID = new DcmTag(0x0004, 0x1500);

		/// <summary>(0004,1504) VR=UL MRDR Directory Record Offset (Retired)</summary>
		public static DcmTag MRDRDirectoryRecordOffsetRETIRED = new DcmTag(0x0004, 0x1504);

		/// <summary>(0004,1510) VR=UI Referenced SOP Class UID in File</summary>
		public static DcmTag ReferencedSOPClassUIDInFile = new DcmTag(0x0004, 0x1510);

		/// <summary>(0004,1511) VR=UI Referenced SOP Instance UID in File</summary>
		public static DcmTag ReferencedSOPInstanceUIDInFile = new DcmTag(0x0004, 0x1511);

		/// <summary>(0004,1512) VR=UI Referenced Transfer Syntax UID in File</summary>
		public static DcmTag ReferencedTransferSyntaxUIDInFile = new DcmTag(0x0004, 0x1512);

		/// <summary>(0004,151a) VR=UI Referenced Related General SOP Class UID in File</summary>
		public static DcmTag ReferencedRelatedGeneralSOPClassUIDInFile = new DcmTag(0x0004, 0x151a);

		/// <summary>(0004,1600) VR=UL Number of References (Retired)</summary>
		public static DcmTag NumberOfReferencesRETIRED = new DcmTag(0x0004, 0x1600);

		/// <summary>(0008,0001) VR=UL Length to End (Retired)</summary>
		public static DcmTag LengthToEndRETIRED = new DcmTag(0x0008, 0x0001);

		/// <summary>(0008,0005) VR=CS Specific Character Set</summary>
		public static DcmTag SpecificCharacterSet = new DcmTag(0x0008, 0x0005);

		/// <summary>(0008,0008) VR=CS Image Type</summary>
		public static DcmTag ImageType = new DcmTag(0x0008, 0x0008);

		/// <summary>(0008,0010) VR=CS Recognition Code (Retired)</summary>
		public static DcmTag RecognitionCodeRETIRED = new DcmTag(0x0008, 0x0010);

		/// <summary>(0008,0012) VR=DA Instance Creation Date</summary>
		public static DcmTag InstanceCreationDate = new DcmTag(0x0008, 0x0012);

		/// <summary>(0008,0013) VR=TM Instance Creation Time</summary>
		public static DcmTag InstanceCreationTime = new DcmTag(0x0008, 0x0013);

		/// <summary>(0008,0014) VR=UI Instance Creator UID</summary>
		public static DcmTag InstanceCreatorUID = new DcmTag(0x0008, 0x0014);

		/// <summary>(0008,0016) VR=UI SOP Class UID</summary>
		public static DcmTag SOPClassUID = new DcmTag(0x0008, 0x0016);

		/// <summary>(0008,0018) VR=UI SOP Instance UID</summary>
		public static DcmTag SOPInstanceUID = new DcmTag(0x0008, 0x0018);

		/// <summary>(0008,001a) VR=UI Related General SOP Class UID</summary>
		public static DcmTag RelatedGeneralSOPClassUID = new DcmTag(0x0008, 0x001a);

		/// <summary>(0008,001b) VR=UI Original Specialized SOP Class UID</summary>
		public static DcmTag OriginalSpecializedSOPClassUID = new DcmTag(0x0008, 0x001b);

		/// <summary>(0008,0020) VR=DA Study Date</summary>
		public static DcmTag StudyDate = new DcmTag(0x0008, 0x0020);

		/// <summary>(0008,0021) VR=DA Series Date</summary>
		public static DcmTag SeriesDate = new DcmTag(0x0008, 0x0021);

		/// <summary>(0008,0022) VR=DA Acquisition Date</summary>
		public static DcmTag AcquisitionDate = new DcmTag(0x0008, 0x0022);

		/// <summary>(0008,0023) VR=DA Content Date</summary>
		public static DcmTag ContentDate = new DcmTag(0x0008, 0x0023);

		/// <summary>(0008,0024) VR=DA Overlay Date (Retired)</summary>
		public static DcmTag OverlayDateRETIRED = new DcmTag(0x0008, 0x0024);

		/// <summary>(0008,0025) VR=DA Curve Date (Retired)</summary>
		public static DcmTag CurveDateRETIRED = new DcmTag(0x0008, 0x0025);

		/// <summary>(0008,002a) VR=DT Acquisition Datetime</summary>
		public static DcmTag AcquisitionDatetime = new DcmTag(0x0008, 0x002a);

		/// <summary>(0008,0030) VR=TM Study Time</summary>
		public static DcmTag StudyTime = new DcmTag(0x0008, 0x0030);

		/// <summary>(0008,0031) VR=TM Series Time</summary>
		public static DcmTag SeriesTime = new DcmTag(0x0008, 0x0031);

		/// <summary>(0008,0032) VR=TM Acquisition Time</summary>
		public static DcmTag AcquisitionTime = new DcmTag(0x0008, 0x0032);

		/// <summary>(0008,0033) VR=TM Content Time</summary>
		public static DcmTag ContentTime = new DcmTag(0x0008, 0x0033);

		/// <summary>(0008,0034) VR=TM Overlay Time (Retired)</summary>
		public static DcmTag OverlayTimeRETIRED = new DcmTag(0x0008, 0x0034);

		/// <summary>(0008,0035) VR=TM Curve Time (Retired)</summary>
		public static DcmTag CurveTimeRETIRED = new DcmTag(0x0008, 0x0035);

		/// <summary>(0008,0040) VR=US Data Set Type (Retired)</summary>
		public static DcmTag DataSetTypeRETIRED = new DcmTag(0x0008, 0x0040);

		/// <summary>(0008,0041) VR=LO Data Set Subtype (Retired)</summary>
		public static DcmTag DataSetSubtypeRETIRED = new DcmTag(0x0008, 0x0041);

		/// <summary>(0008,0042) VR=CS Nuclear Medicine Series Type (Retired)</summary>
		public static DcmTag NuclearMedicineSeriesTypeRETIRED = new DcmTag(0x0008, 0x0042);

		/// <summary>(0008,0050) VR=SH Accession Number</summary>
		public static DcmTag AccessionNumber = new DcmTag(0x0008, 0x0050);

		/// <summary>(0008,0052) VR=CS Query/Retrieve Level</summary>
		public static DcmTag QueryRetrieveLevel = new DcmTag(0x0008, 0x0052);

		/// <summary>(0008,0054) VR=AE Retrieve AE Title</summary>
		public static DcmTag RetrieveAETitle = new DcmTag(0x0008, 0x0054);

		/// <summary>(0008,0056) VR=CS Instance Availability</summary>
		public static DcmTag InstanceAvailability = new DcmTag(0x0008, 0x0056);

		/// <summary>(0008,0058) VR=UI Failed SOP Instance UID List</summary>
		public static DcmTag FailedSOPInstanceUIDList = new DcmTag(0x0008, 0x0058);

		/// <summary>(0008,0060) VR=CS Modality</summary>
		public static DcmTag Modality = new DcmTag(0x0008, 0x0060);

		/// <summary>(0008,0061) VR=CS Modalities in Study</summary>
		public static DcmTag ModalitiesInStudy = new DcmTag(0x0008, 0x0061);

		/// <summary>(0008,0062) VR=UI SOP Classes in Study</summary>
		public static DcmTag SOPClassesInStudy = new DcmTag(0x0008, 0x0062);

		/// <summary>(0008,0064) VR=CS Conversion Type</summary>
		public static DcmTag ConversionType = new DcmTag(0x0008, 0x0064);

		/// <summary>(0008,0068) VR=CS Presentation Intent Type</summary>
		public static DcmTag PresentationIntentType = new DcmTag(0x0008, 0x0068);

		/// <summary>(0008,0070) VR=LO Manufacturer</summary>
		public static DcmTag Manufacturer = new DcmTag(0x0008, 0x0070);

		/// <summary>(0008,0080) VR=LO Institution Name</summary>
		public static DcmTag InstitutionName = new DcmTag(0x0008, 0x0080);

		/// <summary>(0008,0081) VR=ST Institution Address</summary>
		public static DcmTag InstitutionAddress = new DcmTag(0x0008, 0x0081);

		/// <summary>(0008,0082) VR=SQ Institution Code Sequence</summary>
		public static DcmTag InstitutionCodeSequence = new DcmTag(0x0008, 0x0082);

		/// <summary>(0008,0090) VR=PN Referring Physician's Name</summary>
		public static DcmTag ReferringPhysiciansName = new DcmTag(0x0008, 0x0090);

		/// <summary>(0008,0092) VR=ST Referring Physician's Address</summary>
		public static DcmTag ReferringPhysiciansAddress = new DcmTag(0x0008, 0x0092);

		/// <summary>(0008,0094) VR=SH Referring Physician's Telephone Numbers</summary>
		public static DcmTag ReferringPhysiciansTelephoneNumbers = new DcmTag(0x0008, 0x0094);

		/// <summary>(0008,0096) VR=SQ Referring Physician Identification Sequence</summary>
		public static DcmTag ReferringPhysicianIdentificationSequence = new DcmTag(0x0008, 0x0096);

		/// <summary>(0008,0100) VR=SH Code Value</summary>
		public static DcmTag CodeValue = new DcmTag(0x0008, 0x0100);

		/// <summary>(0008,0102) VR=SH Coding Scheme Designator</summary>
		public static DcmTag CodingSchemeDesignator = new DcmTag(0x0008, 0x0102);

		/// <summary>(0008,0103) VR=SH Coding Scheme Version</summary>
		public static DcmTag CodingSchemeVersion = new DcmTag(0x0008, 0x0103);

		/// <summary>(0008,0104) VR=LO Code Meaning</summary>
		public static DcmTag CodeMeaning = new DcmTag(0x0008, 0x0104);

		/// <summary>(0008,0105) VR=CS Mapping Resource</summary>
		public static DcmTag MappingResource = new DcmTag(0x0008, 0x0105);

		/// <summary>(0008,0106) VR=DT Context Group Version</summary>
		public static DcmTag ContextGroupVersion = new DcmTag(0x0008, 0x0106);

		/// <summary>(0008,0107) VR=DT Context Group Local Version</summary>
		public static DcmTag ContextGroupLocalVersion = new DcmTag(0x0008, 0x0107);

		/// <summary>(0008,010b) VR=CS Context Group Extension Flag</summary>
		public static DcmTag ContextGroupExtensionFlag = new DcmTag(0x0008, 0x010b);

		/// <summary>(0008,010c) VR=UI Coding Scheme UID</summary>
		public static DcmTag CodingSchemeUID = new DcmTag(0x0008, 0x010c);

		/// <summary>(0008,010d) VR=UI Context Group Extension Creator UID</summary>
		public static DcmTag ContextGroupExtensionCreatorUID = new DcmTag(0x0008, 0x010d);

		/// <summary>(0008,010f) VR=CS Context Identifier</summary>
		public static DcmTag ContextIdentifier = new DcmTag(0x0008, 0x010f);

		/// <summary>(0008,0110) VR=SQ Coding Scheme Identification Sequence</summary>
		public static DcmTag CodingSchemeIdentificationSequence = new DcmTag(0x0008, 0x0110);

		/// <summary>(0008,0112) VR=LO Coding Scheme Registry</summary>
		public static DcmTag CodingSchemeRegistry = new DcmTag(0x0008, 0x0112);

		/// <summary>(0008,0114) VR=ST Coding Scheme External ID</summary>
		public static DcmTag CodingSchemeExternalID = new DcmTag(0x0008, 0x0114);

		/// <summary>(0008,0115) VR=ST Coding Scheme Name</summary>
		public static DcmTag CodingSchemeName = new DcmTag(0x0008, 0x0115);

		/// <summary>(0008,0116) VR=ST Responsible Organization</summary>
		public static DcmTag ResponsibleOrganization = new DcmTag(0x0008, 0x0116);

		/// <summary>(0008,0201) VR=SH Timezone Offset From UTC</summary>
		public static DcmTag TimezoneOffsetFromUTC = new DcmTag(0x0008, 0x0201);

		/// <summary>(0008,1000) VR=AE Network ID (Retired)</summary>
		public static DcmTag NetworkIDRETIRED = new DcmTag(0x0008, 0x1000);

		/// <summary>(0008,1010) VR=SH Station Name</summary>
		public static DcmTag StationName = new DcmTag(0x0008, 0x1010);

		/// <summary>(0008,1030) VR=LO Study Description</summary>
		public static DcmTag StudyDescription = new DcmTag(0x0008, 0x1030);

		/// <summary>(0008,1032) VR=SQ Procedure Code Sequence</summary>
		public static DcmTag ProcedureCodeSequence = new DcmTag(0x0008, 0x1032);

		/// <summary>(0008,103e) VR=LO Series Description</summary>
		public static DcmTag SeriesDescription = new DcmTag(0x0008, 0x103e);

		/// <summary>(0008,1040) VR=LO Institutional Department Name</summary>
		public static DcmTag InstitutionalDepartmentName = new DcmTag(0x0008, 0x1040);

		/// <summary>(0008,1048) VR=PN Physician(s) of Record</summary>
		public static DcmTag PhysiciansOfRecord = new DcmTag(0x0008, 0x1048);

		/// <summary>(0008,1049) VR=SQ Physician(s) of Record Identification Sequence</summary>
		public static DcmTag PhysiciansOfRecordIdentificationSequence = new DcmTag(0x0008, 0x1049);

		/// <summary>(0008,1050) VR=PN Performing Physician's Name</summary>
		public static DcmTag PerformingPhysiciansName = new DcmTag(0x0008, 0x1050);

		/// <summary>(0008,1052) VR=SQ Performing Physician Identification Sequence</summary>
		public static DcmTag PerformingPhysicianIdentificationSequence = new DcmTag(0x0008, 0x1052);

		/// <summary>(0008,1060) VR=PN Name of Physician(s) Reading Study</summary>
		public static DcmTag NameOfPhysiciansReadingStudy = new DcmTag(0x0008, 0x1060);

		/// <summary>(0008,1062) VR=SQ Physician(s) Reading Study Identification Sequence</summary>
		public static DcmTag PhysiciansReadingStudyIdentificationSequence = new DcmTag(0x0008, 0x1062);

		/// <summary>(0008,1070) VR=PN Operators' Name</summary>
		public static DcmTag OperatorsName = new DcmTag(0x0008, 0x1070);

		/// <summary>(0008,1072) VR=SQ Operator Identification Sequence</summary>
		public static DcmTag OperatorIdentificationSequence = new DcmTag(0x0008, 0x1072);

		/// <summary>(0008,1080) VR=LO Admitting Diagnoses Description</summary>
		public static DcmTag AdmittingDiagnosesDescription = new DcmTag(0x0008, 0x1080);

		/// <summary>(0008,1084) VR=SQ Admitting Diagnoses Code Sequence</summary>
		public static DcmTag AdmittingDiagnosesCodeSequence = new DcmTag(0x0008, 0x1084);

		/// <summary>(0008,1090) VR=LO Manufacturer's Model Name</summary>
		public static DcmTag ManufacturersModelName = new DcmTag(0x0008, 0x1090);

		/// <summary>(0008,1100) VR=SQ Referenced Results Sequence (Retired)</summary>
		public static DcmTag ReferencedResultsSequenceRETIRED = new DcmTag(0x0008, 0x1100);

		/// <summary>(0008,1110) VR=SQ Referenced Study Sequence</summary>
		public static DcmTag ReferencedStudySequence = new DcmTag(0x0008, 0x1110);

		/// <summary>(0008,1111) VR=SQ Referenced Performed Procedure Step Sequence</summary>
		public static DcmTag ReferencedPerformedProcedureStepSequence = new DcmTag(0x0008, 0x1111);

		/// <summary>(0008,1115) VR=SQ Referenced Series Sequence</summary>
		public static DcmTag ReferencedSeriesSequence = new DcmTag(0x0008, 0x1115);

		/// <summary>(0008,1120) VR=SQ Referenced Patient Sequence</summary>
		public static DcmTag ReferencedPatientSequence = new DcmTag(0x0008, 0x1120);

		/// <summary>(0008,1125) VR=SQ Referenced Visit Sequence</summary>
		public static DcmTag ReferencedVisitSequence = new DcmTag(0x0008, 0x1125);

		/// <summary>(0008,1130) VR=SQ Referenced Overlay Sequence (Retired)</summary>
		public static DcmTag ReferencedOverlaySequenceRETIRED = new DcmTag(0x0008, 0x1130);

		/// <summary>(0008,113a) VR=SQ Referenced Waveform Sequence</summary>
		public static DcmTag ReferencedWaveformSequence = new DcmTag(0x0008, 0x113a);

		/// <summary>(0008,1140) VR=SQ Referenced Image Sequence</summary>
		public static DcmTag ReferencedImageSequence = new DcmTag(0x0008, 0x1140);

		/// <summary>(0008,1145) VR=SQ Referenced Curve Sequence (Retired)</summary>
		public static DcmTag ReferencedCurveSequenceRETIRED = new DcmTag(0x0008, 0x1145);

		/// <summary>(0008,114a) VR=SQ Referenced Instance Sequence</summary>
		public static DcmTag ReferencedInstanceSequence = new DcmTag(0x0008, 0x114a);

		/// <summary>(0008,114b) VR=SQ Referenced Real World Value Mapping Instance Sequence</summary>
		public static DcmTag ReferencedRealWorldValueMappingInstanceSequence = new DcmTag(0x0008, 0x114b);

		/// <summary>(0008,1150) VR=UI Referenced SOP Class UID</summary>
		public static DcmTag ReferencedSOPClassUID = new DcmTag(0x0008, 0x1150);

		/// <summary>(0008,1155) VR=UI Referenced SOP Instance UID</summary>
		public static DcmTag ReferencedSOPInstanceUID = new DcmTag(0x0008, 0x1155);

		/// <summary>(0008,115a) VR=UI SOP Classes Supported</summary>
		public static DcmTag SOPClassesSupported = new DcmTag(0x0008, 0x115a);

		/// <summary>(0008,1160) VR=IS Referenced Frame Number</summary>
		public static DcmTag ReferencedFrameNumber = new DcmTag(0x0008, 0x1160);

		/// <summary>(0008,1195) VR=UI Transaction UID</summary>
		public static DcmTag TransactionUID = new DcmTag(0x0008, 0x1195);

		/// <summary>(0008,1197) VR=US Failure Reason</summary>
		public static DcmTag FailureReason = new DcmTag(0x0008, 0x1197);

		/// <summary>(0008,1198) VR=SQ Failed SOP Sequence</summary>
		public static DcmTag FailedSOPSequence = new DcmTag(0x0008, 0x1198);

		/// <summary>(0008,1199) VR=SQ Referenced SOP Sequence</summary>
		public static DcmTag ReferencedSOPSequence = new DcmTag(0x0008, 0x1199);

		/// <summary>(0008,1200) VR=SQ Studies Containing Other Referenced Instances Sequence</summary>
		public static DcmTag StudiesContainingOtherReferencedInstancesSequence = new DcmTag(0x0008, 0x1200);

		/// <summary>(0008,1250) VR=SQ Related Series Sequence</summary>
		public static DcmTag RelatedSeriesSequence = new DcmTag(0x0008, 0x1250);

		/// <summary>(0008,2110) VR=CS Lossy Image Compression (Retired)</summary>
		public static DcmTag LossyImageCompressionRETIRED = new DcmTag(0x0008, 0x2110);

		/// <summary>(0008,2111) VR=ST Derivation Description</summary>
		public static DcmTag DerivationDescription = new DcmTag(0x0008, 0x2111);

		/// <summary>(0008,2112) VR=SQ Source Image Sequence</summary>
		public static DcmTag SourceImageSequence = new DcmTag(0x0008, 0x2112);

		/// <summary>(0008,2120) VR=SH Stage Name</summary>
		public static DcmTag StageName = new DcmTag(0x0008, 0x2120);

		/// <summary>(0008,2122) VR=IS Stage Number</summary>
		public static DcmTag StageNumber = new DcmTag(0x0008, 0x2122);

		/// <summary>(0008,2124) VR=IS Number of Stages</summary>
		public static DcmTag NumberOfStages = new DcmTag(0x0008, 0x2124);

		/// <summary>(0008,2127) VR=SH View Name</summary>
		public static DcmTag ViewName = new DcmTag(0x0008, 0x2127);

		/// <summary>(0008,2128) VR=IS View Number</summary>
		public static DcmTag ViewNumber = new DcmTag(0x0008, 0x2128);

		/// <summary>(0008,2129) VR=IS Number of Event Timers</summary>
		public static DcmTag NumberOfEventTimers = new DcmTag(0x0008, 0x2129);

		/// <summary>(0008,212a) VR=IS Number of Views in Stage</summary>
		public static DcmTag NumberOfViewsInStage = new DcmTag(0x0008, 0x212a);

		/// <summary>(0008,2130) VR=DS Event Elapsed Time(s)</summary>
		public static DcmTag EventElapsedTimes = new DcmTag(0x0008, 0x2130);

		/// <summary>(0008,2132) VR=LO Event Timer Name(s)</summary>
		public static DcmTag EventTimerNames = new DcmTag(0x0008, 0x2132);

		/// <summary>(0008,2142) VR=IS Start Trim</summary>
		public static DcmTag StartTrim = new DcmTag(0x0008, 0x2142);

		/// <summary>(0008,2143) VR=IS Stop Trim</summary>
		public static DcmTag StopTrim = new DcmTag(0x0008, 0x2143);

		/// <summary>(0008,2144) VR=IS Recommended Display Frame Rate</summary>
		public static DcmTag RecommendedDisplayFrameRate = new DcmTag(0x0008, 0x2144);

		/// <summary>(0008,2200) VR=CS Transducer Position (Retired)</summary>
		public static DcmTag TransducerPositionRETIRED = new DcmTag(0x0008, 0x2200);

		/// <summary>(0008,2204) VR=CS Transducer Orientation (Retired)</summary>
		public static DcmTag TransducerOrientationRETIRED = new DcmTag(0x0008, 0x2204);

		/// <summary>(0008,2208) VR=CS Anatomic Structure (Retired)</summary>
		public static DcmTag AnatomicStructureRETIRED = new DcmTag(0x0008, 0x2208);

		/// <summary>(0008,2218) VR=SQ Anatomic Region Sequence</summary>
		public static DcmTag AnatomicRegionSequence = new DcmTag(0x0008, 0x2218);

		/// <summary>(0008,2220) VR=SQ Anatomic Region Modifier Sequence</summary>
		public static DcmTag AnatomicRegionModifierSequence = new DcmTag(0x0008, 0x2220);

		/// <summary>(0008,2228) VR=SQ Primary Anatomic Structure Sequence</summary>
		public static DcmTag PrimaryAnatomicStructureSequence = new DcmTag(0x0008, 0x2228);

		/// <summary>(0008,2229) VR=SQ Anatomic Structure, Space or Region Sequence</summary>
		public static DcmTag AnatomicStructure, SpaceOrRegionSequence = new DcmTag(0x0008, 0x2229);

		/// <summary>(0008,2230) VR=SQ Primary Anatomic Structure Modifier Sequence</summary>
		public static DcmTag PrimaryAnatomicStructureModifierSequence = new DcmTag(0x0008, 0x2230);

		/// <summary>(0008,2240) VR=SQ Transducer Position Sequence (Retired)</summary>
		public static DcmTag TransducerPositionSequenceRETIRED = new DcmTag(0x0008, 0x2240);

		/// <summary>(0008,2242) VR=SQ Transducer Position Modifier Sequence (Retired)</summary>
		public static DcmTag TransducerPositionModifierSequenceRETIRED = new DcmTag(0x0008, 0x2242);

		/// <summary>(0008,2244) VR=SQ Transducer Orientation Sequence (Retired)</summary>
		public static DcmTag TransducerOrientationSequenceRETIRED = new DcmTag(0x0008, 0x2244);

		/// <summary>(0008,2246) VR=SQ Transducer Orientation Modifier Sequence (Retired)</summary>
		public static DcmTag TransducerOrientationModifierSequenceRETIRED = new DcmTag(0x0008, 0x2246);

		/// <summary>(0008,3001) VR=SQ Alternate Representation Sequence</summary>
		public static DcmTag AlternateRepresentationSequence = new DcmTag(0x0008, 0x3001);

		/// <summary>(0008,3010) VR=UI Irradiation Event UID</summary>
		public static DcmTag IrradiationEventUID = new DcmTag(0x0008, 0x3010);

		/// <summary>(0008,4000) VR=LT Identifying Comments (Retired)</summary>
		public static DcmTag IdentifyingCommentsRETIRED = new DcmTag(0x0008, 0x4000);

		/// <summary>(0008,9007) VR=CS Frame Type</summary>
		public static DcmTag FrameType = new DcmTag(0x0008, 0x9007);

		/// <summary>(0008,9092) VR=SQ Referenced Image Evidence Sequence</summary>
		public static DcmTag ReferencedImageEvidenceSequence = new DcmTag(0x0008, 0x9092);

		/// <summary>(0008,9121) VR=SQ Referenced Raw Data Sequence</summary>
		public static DcmTag ReferencedRawDataSequence = new DcmTag(0x0008, 0x9121);

		/// <summary>(0008,9123) VR=UI Creator-Version UID</summary>
		public static DcmTag CreatorVersionUID = new DcmTag(0x0008, 0x9123);

		/// <summary>(0008,9124) VR=SQ Derivation Image Sequence</summary>
		public static DcmTag DerivationImageSequence = new DcmTag(0x0008, 0x9124);

		/// <summary>(0008,9154) VR=SQ Source Image Evidence Sequence</summary>
		public static DcmTag SourceImageEvidenceSequence = new DcmTag(0x0008, 0x9154);

		/// <summary>(0008,9205) VR=CS Pixel Presentation</summary>
		public static DcmTag PixelPresentation = new DcmTag(0x0008, 0x9205);

		/// <summary>(0008,9206) VR=CS Volumetric Properties</summary>
		public static DcmTag VolumetricProperties = new DcmTag(0x0008, 0x9206);

		/// <summary>(0008,9207) VR=CS Volume Based Calculation Technique</summary>
		public static DcmTag VolumeBasedCalculationTechnique = new DcmTag(0x0008, 0x9207);

		/// <summary>(0008,9208) VR=CS Complex Image Component</summary>
		public static DcmTag ComplexImageComponent = new DcmTag(0x0008, 0x9208);

		/// <summary>(0008,9209) VR=CS Acquisition Contrast</summary>
		public static DcmTag AcquisitionContrast = new DcmTag(0x0008, 0x9209);

		/// <summary>(0008,9215) VR=SQ Derivation Code Sequence</summary>
		public static DcmTag DerivationCodeSequence = new DcmTag(0x0008, 0x9215);

		/// <summary>(0008,9237) VR=SQ Referenced Grayscale Presentation State Sequence</summary>
		public static DcmTag ReferencedGrayscalePresentationStateSequence = new DcmTag(0x0008, 0x9237);

		/// <summary>(0008,9410) VR=SQ Referenced Other Plane Sequence</summary>
		public static DcmTag ReferencedOtherPlaneSequence = new DcmTag(0x0008, 0x9410);

		/// <summary>(0008,9458) VR=SQ Frame Display Sequence</summary>
		public static DcmTag FrameDisplaySequence = new DcmTag(0x0008, 0x9458);

		/// <summary>(0008,9459) VR=FL Recommended Display Frame Rate in Float</summary>
		public static DcmTag RecommendedDisplayFrameRateInFloat = new DcmTag(0x0008, 0x9459);

		/// <summary>(0008,9460) VR=CS Skip Frame Range Flag</summary>
		public static DcmTag SkipFrameRangeFlag = new DcmTag(0x0008, 0x9460);

		/// <summary>(0010,0010) VR=PN Patient's Name</summary>
		public static DcmTag PatientsName = new DcmTag(0x0010, 0x0010);

		/// <summary>(0010,0020) VR=LO Patient ID</summary>
		public static DcmTag PatientID = new DcmTag(0x0010, 0x0020);

		/// <summary>(0010,0021) VR=LO Issuer of Patient ID</summary>
		public static DcmTag IssuerOfPatientID = new DcmTag(0x0010, 0x0021);

		/// <summary>(0010,0022) VR=CS Type of Patient ID</summary>
		public static DcmTag TypeOfPatientID = new DcmTag(0x0010, 0x0022);

		/// <summary>(0010,0030) VR=DA Patient's Birth Date</summary>
		public static DcmTag PatientsBirthDate = new DcmTag(0x0010, 0x0030);

		/// <summary>(0010,0032) VR=TM Patient's Birth Time</summary>
		public static DcmTag PatientsBirthTime = new DcmTag(0x0010, 0x0032);

		/// <summary>(0010,0040) VR=CS Patient's Sex</summary>
		public static DcmTag PatientsSex = new DcmTag(0x0010, 0x0040);

		/// <summary>(0010,0050) VR=SQ Patient's Insurance Plan Code Sequence</summary>
		public static DcmTag PatientsInsurancePlanCodeSequence = new DcmTag(0x0010, 0x0050);

		/// <summary>(0010,0101) VR=SQ Patient's Primary Language Code Sequence</summary>
		public static DcmTag PatientsPrimaryLanguageCodeSequence = new DcmTag(0x0010, 0x0101);

		/// <summary>(0010,0102) VR=SQ Patient's Primary Language Code Modifier Sequence</summary>
		public static DcmTag PatientsPrimaryLanguageCodeModifierSequence = new DcmTag(0x0010, 0x0102);

		/// <summary>(0010,1000) VR=LO Other Patient IDs</summary>
		public static DcmTag OtherPatientIDs = new DcmTag(0x0010, 0x1000);

		/// <summary>(0010,1001) VR=PN Other Patient Names</summary>
		public static DcmTag OtherPatientNames = new DcmTag(0x0010, 0x1001);

		/// <summary>(0010,1002) VR=SQ Other Patient IDs Sequence</summary>
		public static DcmTag OtherPatientIDsSequence = new DcmTag(0x0010, 0x1002);

		/// <summary>(0010,1005) VR=PN Patient's Birth Name</summary>
		public static DcmTag PatientsBirthName = new DcmTag(0x0010, 0x1005);

		/// <summary>(0010,1010) VR=AS Patient's Age</summary>
		public static DcmTag PatientsAge = new DcmTag(0x0010, 0x1010);

		/// <summary>(0010,1020) VR=DS Patient's Size</summary>
		public static DcmTag PatientsSize = new DcmTag(0x0010, 0x1020);

		/// <summary>(0010,1030) VR=DS Patient's Weight</summary>
		public static DcmTag PatientsWeight = new DcmTag(0x0010, 0x1030);

		/// <summary>(0010,1040) VR=LO Patient's Address</summary>
		public static DcmTag PatientsAddress = new DcmTag(0x0010, 0x1040);

		/// <summary>(0010,1050) VR=LO Insurance Plan Identification (Retired)</summary>
		public static DcmTag InsurancePlanIdentificationRETIRED = new DcmTag(0x0010, 0x1050);

		/// <summary>(0010,1060) VR=PN Patient's Mother's Birth Name</summary>
		public static DcmTag PatientsMothersBirthName = new DcmTag(0x0010, 0x1060);

		/// <summary>(0010,1080) VR=LO Military Rank</summary>
		public static DcmTag MilitaryRank = new DcmTag(0x0010, 0x1080);

		/// <summary>(0010,1081) VR=LO Branch of Service</summary>
		public static DcmTag BranchOfService = new DcmTag(0x0010, 0x1081);

		/// <summary>(0010,1090) VR=LO Medical Record Locator</summary>
		public static DcmTag MedicalRecordLocator = new DcmTag(0x0010, 0x1090);

		/// <summary>(0010,2000) VR=LO Medical Alerts</summary>
		public static DcmTag MedicalAlerts = new DcmTag(0x0010, 0x2000);

		/// <summary>(0010,2110) VR=LO Contrast Allergies</summary>
		public static DcmTag ContrastAllergies = new DcmTag(0x0010, 0x2110);

		/// <summary>(0010,2150) VR=LO Country of Residence</summary>
		public static DcmTag CountryOfResidence = new DcmTag(0x0010, 0x2150);

		/// <summary>(0010,2152) VR=LO Region of Residence</summary>
		public static DcmTag RegionOfResidence = new DcmTag(0x0010, 0x2152);

		/// <summary>(0010,2154) VR=SH Patient's Telephone Numbers</summary>
		public static DcmTag PatientsTelephoneNumbers = new DcmTag(0x0010, 0x2154);

		/// <summary>(0010,2160) VR=SH Ethnic Group</summary>
		public static DcmTag EthnicGroup = new DcmTag(0x0010, 0x2160);

		/// <summary>(0010,2180) VR=SH Occupation</summary>
		public static DcmTag Occupation = new DcmTag(0x0010, 0x2180);

		/// <summary>(0010,21a0) VR=CS Smoking Status</summary>
		public static DcmTag SmokingStatus = new DcmTag(0x0010, 0x21a0);

		/// <summary>(0010,21b0) VR=LT Additional Patient History</summary>
		public static DcmTag AdditionalPatientHistory = new DcmTag(0x0010, 0x21b0);

		/// <summary>(0010,21c0) VR=US Pregnancy Status</summary>
		public static DcmTag PregnancyStatus = new DcmTag(0x0010, 0x21c0);

		/// <summary>(0010,21d0) VR=DA Last Menstrual Date</summary>
		public static DcmTag LastMenstrualDate = new DcmTag(0x0010, 0x21d0);

		/// <summary>(0010,21f0) VR=LO Patient's Religious Preference</summary>
		public static DcmTag PatientsReligiousPreference = new DcmTag(0x0010, 0x21f0);

		/// <summary>(0010,2201) VR=LO Patient Species Description</summary>
		public static DcmTag PatientSpeciesDescription = new DcmTag(0x0010, 0x2201);

		/// <summary>(0010,2202) VR=SQ Patient Species Code Sequence</summary>
		public static DcmTag PatientSpeciesCodeSequence = new DcmTag(0x0010, 0x2202);

		/// <summary>(0010,2203) VR=CS Patient's Sex Neutered</summary>
		public static DcmTag PatientsSexNeutered = new DcmTag(0x0010, 0x2203);

		/// <summary>(0010,2292) VR=LO Patient Breed Description</summary>
		public static DcmTag PatientBreedDescription = new DcmTag(0x0010, 0x2292);

		/// <summary>(0010,2293) VR=SQ Patient Breed Code Sequence</summary>
		public static DcmTag PatientBreedCodeSequence = new DcmTag(0x0010, 0x2293);

		/// <summary>(0010,2294) VR=SQ Breed Registration Sequence</summary>
		public static DcmTag BreedRegistrationSequence = new DcmTag(0x0010, 0x2294);

		/// <summary>(0010,2295) VR=LO Breed Registration Number</summary>
		public static DcmTag BreedRegistrationNumber = new DcmTag(0x0010, 0x2295);

		/// <summary>(0010,2296) VR=SQ Breed Registry Code Sequence</summary>
		public static DcmTag BreedRegistryCodeSequence = new DcmTag(0x0010, 0x2296);

		/// <summary>(0010,2297) VR=PN Responsible Person</summary>
		public static DcmTag ResponsiblePerson = new DcmTag(0x0010, 0x2297);

		/// <summary>(0010,2298) VR=CS Responsible Person Role</summary>
		public static DcmTag ResponsiblePersonRole = new DcmTag(0x0010, 0x2298);

		/// <summary>(0010,2299) VR=LO Responsible Organization</summary>
		public static DcmTag ResponsibleOrganizationGroup10 = new DcmTag(0x0010, 0x2299);

		/// <summary>(0010,4000) VR=LT Patient Comments</summary>
		public static DcmTag PatientComments = new DcmTag(0x0010, 0x4000);

		/// <summary>(0010,9431) VR=FL Examined Body Thickness</summary>
		public static DcmTag ExaminedBodyThickness = new DcmTag(0x0010, 0x9431);

		/// <summary>(0012,0010) VR=LO Clinical Trial Sponsor Name</summary>
		public static DcmTag ClinicalTrialSponsorName = new DcmTag(0x0012, 0x0010);

		/// <summary>(0012,0020) VR=LO Clinical Trial Protocol ID</summary>
		public static DcmTag ClinicalTrialProtocolID = new DcmTag(0x0012, 0x0020);

		/// <summary>(0012,0021) VR=LO Clinical Trial Protocol Name</summary>
		public static DcmTag ClinicalTrialProtocolName = new DcmTag(0x0012, 0x0021);

		/// <summary>(0012,0030) VR=LO Clinical Trial Site ID</summary>
		public static DcmTag ClinicalTrialSiteID = new DcmTag(0x0012, 0x0030);

		/// <summary>(0012,0031) VR=LO Clinical Trial Site Name</summary>
		public static DcmTag ClinicalTrialSiteName = new DcmTag(0x0012, 0x0031);

		/// <summary>(0012,0040) VR=LO Clinical Trial Subject ID</summary>
		public static DcmTag ClinicalTrialSubjectID = new DcmTag(0x0012, 0x0040);

		/// <summary>(0012,0042) VR=LO Clinical Trial Subject Reading ID</summary>
		public static DcmTag ClinicalTrialSubjectReadingID = new DcmTag(0x0012, 0x0042);

		/// <summary>(0012,0050) VR=LO Clinical Trial Time Point ID</summary>
		public static DcmTag ClinicalTrialTimePointID = new DcmTag(0x0012, 0x0050);

		/// <summary>(0012,0051) VR=ST Clinical Trial Time Point Description</summary>
		public static DcmTag ClinicalTrialTimePointDescription = new DcmTag(0x0012, 0x0051);

		/// <summary>(0012,0060) VR=LO Clinical Trial Coordinating Center Name</summary>
		public static DcmTag ClinicalTrialCoordinatingCenterName = new DcmTag(0x0012, 0x0060);

		/// <summary>(0012,0062) VR=CS Patient Identity Removed</summary>
		public static DcmTag PatientIdentityRemoved = new DcmTag(0x0012, 0x0062);

		/// <summary>(0012,0063) VR=LO De-identification Method</summary>
		public static DcmTag DeidentificationMethod = new DcmTag(0x0012, 0x0063);

		/// <summary>(0012,0064) VR=SQ De-identification Method Code Sequence</summary>
		public static DcmTag DeidentificationMethodCodeSequence = new DcmTag(0x0012, 0x0064);

		/// <summary>(0018,0010) VR=LO Contrast/Bolus Agent</summary>
		public static DcmTag ContrastBolusAgent = new DcmTag(0x0018, 0x0010);

		/// <summary>(0018,0012) VR=SQ Contrast/Bolus Agent Sequence</summary>
		public static DcmTag ContrastBolusAgentSequence = new DcmTag(0x0018, 0x0012);

		/// <summary>(0018,0014) VR=SQ Contrast/Bolus Administration Route Sequence</summary>
		public static DcmTag ContrastBolusAdministrationRouteSequence = new DcmTag(0x0018, 0x0014);

		/// <summary>(0018,0015) VR=CS Body Part Examined</summary>
		public static DcmTag BodyPartExamined = new DcmTag(0x0018, 0x0015);

		/// <summary>(0018,0020) VR=CS Scanning Sequence</summary>
		public static DcmTag ScanningSequence = new DcmTag(0x0018, 0x0020);

		/// <summary>(0018,0021) VR=CS Sequence Variant</summary>
		public static DcmTag SequenceVariant = new DcmTag(0x0018, 0x0021);

		/// <summary>(0018,0022) VR=CS Scan Options</summary>
		public static DcmTag ScanOptions = new DcmTag(0x0018, 0x0022);

		/// <summary>(0018,0023) VR=CS MR Acquisition Type</summary>
		public static DcmTag MRAcquisitionType = new DcmTag(0x0018, 0x0023);

		/// <summary>(0018,0024) VR=SH Sequence Name</summary>
		public static DcmTag SequenceName = new DcmTag(0x0018, 0x0024);

		/// <summary>(0018,0025) VR=CS Angio Flag</summary>
		public static DcmTag AngioFlag = new DcmTag(0x0018, 0x0025);

		/// <summary>(0018,0026) VR=SQ Intervention Drug Information Sequence</summary>
		public static DcmTag InterventionDrugInformationSequence = new DcmTag(0x0018, 0x0026);

		/// <summary>(0018,0027) VR=TM Intervention Drug Stop Time</summary>
		public static DcmTag InterventionDrugStopTime = new DcmTag(0x0018, 0x0027);

		/// <summary>(0018,0028) VR=DS Intervention Drug Dose</summary>
		public static DcmTag InterventionDrugDose = new DcmTag(0x0018, 0x0028);

		/// <summary>(0018,0029) VR=SQ Intervention Drug Sequence</summary>
		public static DcmTag InterventionDrugSequence = new DcmTag(0x0018, 0x0029);

		/// <summary>(0018,002a) VR=SQ Additional Drug Sequence</summary>
		public static DcmTag AdditionalDrugSequence = new DcmTag(0x0018, 0x002a);

		/// <summary>(0018,0030) VR=LO Radionuclide (Retired)</summary>
		public static DcmTag RadionuclideRETIRED = new DcmTag(0x0018, 0x0030);

		/// <summary>(0018,0031) VR=LO Radiopharmaceutical</summary>
		public static DcmTag Radiopharmaceutical = new DcmTag(0x0018, 0x0031);

		/// <summary>(0018,0032) VR=DS Energy Window Centerline (Retired)</summary>
		public static DcmTag EnergyWindowCenterlineRETIRED = new DcmTag(0x0018, 0x0032);

		/// <summary>(0018,0033) VR=DS Energy Window Total Width (Retired)</summary>
		public static DcmTag EnergyWindowTotalWidthRETIRED = new DcmTag(0x0018, 0x0033);

		/// <summary>(0018,0034) VR=LO Intervention Drug Name</summary>
		public static DcmTag InterventionDrugName = new DcmTag(0x0018, 0x0034);

		/// <summary>(0018,0035) VR=TM Intervention Drug Start Time</summary>
		public static DcmTag InterventionDrugStartTime = new DcmTag(0x0018, 0x0035);

		/// <summary>(0018,0036) VR=SQ Intervention Sequence</summary>
		public static DcmTag InterventionSequence = new DcmTag(0x0018, 0x0036);

		/// <summary>(0018,0037) VR=CS Therapy Type (Retired)</summary>
		public static DcmTag TherapyTypeRETIRED = new DcmTag(0x0018, 0x0037);

		/// <summary>(0018,0038) VR=CS Intervention Status</summary>
		public static DcmTag InterventionStatus = new DcmTag(0x0018, 0x0038);

		/// <summary>(0018,0039) VR=CS Therapy Description (Retired)</summary>
		public static DcmTag TherapyDescriptionRETIRED = new DcmTag(0x0018, 0x0039);

		/// <summary>(0018,003a) VR=ST Intervention Description</summary>
		public static DcmTag InterventionDescription = new DcmTag(0x0018, 0x003a);

		/// <summary>(0018,0040) VR=IS Cine Rate</summary>
		public static DcmTag CineRate = new DcmTag(0x0018, 0x0040);

		/// <summary>(0018,0050) VR=DS Slice Thickness</summary>
		public static DcmTag SliceThickness = new DcmTag(0x0018, 0x0050);

		/// <summary>(0018,0060) VR=DS kVp</summary>
		public static DcmTag KVp = new DcmTag(0x0018, 0x0060);

		/// <summary>(0018,0070) VR=IS Counts Accumulated</summary>
		public static DcmTag CountsAccumulated = new DcmTag(0x0018, 0x0070);

		/// <summary>(0018,0071) VR=CS Acquisition Termination Condition</summary>
		public static DcmTag AcquisitionTerminationCondition = new DcmTag(0x0018, 0x0071);

		/// <summary>(0018,0072) VR=DS Effective Duration</summary>
		public static DcmTag EffectiveDuration = new DcmTag(0x0018, 0x0072);

		/// <summary>(0018,0073) VR=CS Acquisition Start Condition</summary>
		public static DcmTag AcquisitionStartCondition = new DcmTag(0x0018, 0x0073);

		/// <summary>(0018,0074) VR=IS Acquisition Start Condition Data</summary>
		public static DcmTag AcquisitionStartConditionData = new DcmTag(0x0018, 0x0074);

		/// <summary>(0018,0075) VR=IS Acquisition Termination Condition Data</summary>
		public static DcmTag AcquisitionTerminationConditionData = new DcmTag(0x0018, 0x0075);

		/// <summary>(0018,0080) VR=DS Repetition Time</summary>
		public static DcmTag RepetitionTime = new DcmTag(0x0018, 0x0080);

		/// <summary>(0018,0081) VR=DS Echo Time</summary>
		public static DcmTag EchoTime = new DcmTag(0x0018, 0x0081);

		/// <summary>(0018,0082) VR=DS Inversion Time</summary>
		public static DcmTag InversionTime = new DcmTag(0x0018, 0x0082);

		/// <summary>(0018,0083) VR=DS Number of Averages</summary>
		public static DcmTag NumberOfAverages = new DcmTag(0x0018, 0x0083);

		/// <summary>(0018,0084) VR=DS Imaging Frequency</summary>
		public static DcmTag ImagingFrequency = new DcmTag(0x0018, 0x0084);

		/// <summary>(0018,0085) VR=SH Imaged Nucleus</summary>
		public static DcmTag ImagedNucleus = new DcmTag(0x0018, 0x0085);

		/// <summary>(0018,0086) VR=IS Echo Number(s)</summary>
		public static DcmTag EchoNumbers = new DcmTag(0x0018, 0x0086);

		/// <summary>(0018,0087) VR=DS Magnetic Field Strength</summary>
		public static DcmTag MagneticFieldStrength = new DcmTag(0x0018, 0x0087);

		/// <summary>(0018,0088) VR=DS Spacing Between Slices</summary>
		public static DcmTag SpacingBetweenSlices = new DcmTag(0x0018, 0x0088);

		/// <summary>(0018,0089) VR=IS Number of Phase Encoding Steps</summary>
		public static DcmTag NumberOfPhaseEncodingSteps = new DcmTag(0x0018, 0x0089);

		/// <summary>(0018,0090) VR=DS Data Collection Diameter</summary>
		public static DcmTag DataCollectionDiameter = new DcmTag(0x0018, 0x0090);

		/// <summary>(0018,0091) VR=IS Echo Train Length</summary>
		public static DcmTag EchoTrainLength = new DcmTag(0x0018, 0x0091);

		/// <summary>(0018,0093) VR=DS Percent Sampling</summary>
		public static DcmTag PercentSampling = new DcmTag(0x0018, 0x0093);

		/// <summary>(0018,0094) VR=DS Percent Phase Field of View</summary>
		public static DcmTag PercentPhaseFieldOfView = new DcmTag(0x0018, 0x0094);

		/// <summary>(0018,0095) VR=DS Pixel Bandwidth</summary>
		public static DcmTag PixelBandwidth = new DcmTag(0x0018, 0x0095);

		/// <summary>(0018,1000) VR=LO Device Serial Number</summary>
		public static DcmTag DeviceSerialNumber = new DcmTag(0x0018, 0x1000);

		/// <summary>(0018,1002) VR=UI Device UID</summary>
		public static DcmTag DeviceUID = new DcmTag(0x0018, 0x1002);

		/// <summary>(0018,1003) VR=LO Device ID</summary>
		public static DcmTag DeviceID = new DcmTag(0x0018, 0x1003);

		/// <summary>(0018,1004) VR=LO Plate ID</summary>
		public static DcmTag PlateID = new DcmTag(0x0018, 0x1004);

		/// <summary>(0018,1005) VR=LO Generator ID</summary>
		public static DcmTag GeneratorID = new DcmTag(0x0018, 0x1005);

		/// <summary>(0018,1006) VR=LO Grid ID</summary>
		public static DcmTag GridID = new DcmTag(0x0018, 0x1006);

		/// <summary>(0018,1007) VR=LO Cassette ID</summary>
		public static DcmTag CassetteID = new DcmTag(0x0018, 0x1007);

		/// <summary>(0018,1008) VR=LO Gantry ID</summary>
		public static DcmTag GantryID = new DcmTag(0x0018, 0x1008);

		/// <summary>(0018,1010) VR=LO Secondary Capture Device ID</summary>
		public static DcmTag SecondaryCaptureDeviceID = new DcmTag(0x0018, 0x1010);

		/// <summary>(0018,1011) VR=LO Hardcopy Creation Device ID</summary>
		public static DcmTag HardcopyCreationDeviceID = new DcmTag(0x0018, 0x1011);

		/// <summary>(0018,1012) VR=DA Date of Secondary Capture</summary>
		public static DcmTag DateOfSecondaryCapture = new DcmTag(0x0018, 0x1012);

		/// <summary>(0018,1014) VR=TM Time of Secondary Capture</summary>
		public static DcmTag TimeOfSecondaryCapture = new DcmTag(0x0018, 0x1014);

		/// <summary>(0018,1016) VR=LO Secondary Capture Device Manufacturer</summary>
		public static DcmTag SecondaryCaptureDeviceManufacturer = new DcmTag(0x0018, 0x1016);

		/// <summary>(0018,1017) VR=LO Hardcopy Device Manufacturer</summary>
		public static DcmTag HardcopyDeviceManufacturer = new DcmTag(0x0018, 0x1017);

		/// <summary>(0018,1018) VR=LO Secondary Capture Device Manufacturer's Model Name</summary>
		public static DcmTag SecondaryCaptureDeviceManufacturersModelName = new DcmTag(0x0018, 0x1018);

		/// <summary>(0018,1019) VR=LO Secondary Capture Device Software Version(s)</summary>
		public static DcmTag SecondaryCaptureDeviceSoftwareVersions = new DcmTag(0x0018, 0x1019);

		/// <summary>(0018,101a) VR=LO Hardcopy Device Software Version</summary>
		public static DcmTag HardcopyDeviceSoftwareVersion = new DcmTag(0x0018, 0x101a);

		/// <summary>(0018,101b) VR=LO Hardcopy Device Manufacturer's Model Name</summary>
		public static DcmTag HardcopyDeviceManufacturersModelName = new DcmTag(0x0018, 0x101b);

		/// <summary>(0018,1020) VR=LO Software Version(s)</summary>
		public static DcmTag SoftwareVersions = new DcmTag(0x0018, 0x1020);

		/// <summary>(0018,1022) VR=SH Video Image Format Acquired</summary>
		public static DcmTag VideoImageFormatAcquired = new DcmTag(0x0018, 0x1022);

		/// <summary>(0018,1023) VR=LO Digital Image Format Acquired</summary>
		public static DcmTag DigitalImageFormatAcquired = new DcmTag(0x0018, 0x1023);

		/// <summary>(0018,1030) VR=LO Protocol Name</summary>
		public static DcmTag ProtocolName = new DcmTag(0x0018, 0x1030);

		/// <summary>(0018,1040) VR=LO Contrast/Bolus Route</summary>
		public static DcmTag ContrastBolusRoute = new DcmTag(0x0018, 0x1040);

		/// <summary>(0018,1041) VR=DS Contrast/Bolus Volume</summary>
		public static DcmTag ContrastBolusVolume = new DcmTag(0x0018, 0x1041);

		/// <summary>(0018,1042) VR=TM Contrast/Bolus Start Time</summary>
		public static DcmTag ContrastBolusStartTime = new DcmTag(0x0018, 0x1042);

		/// <summary>(0018,1043) VR=TM Contrast/Bolus Stop Time</summary>
		public static DcmTag ContrastBolusStopTime = new DcmTag(0x0018, 0x1043);

		/// <summary>(0018,1044) VR=DS Contrast/Bolus Total Dose</summary>
		public static DcmTag ContrastBolusTotalDose = new DcmTag(0x0018, 0x1044);

		/// <summary>(0018,1045) VR=IS Syringe Counts</summary>
		public static DcmTag SyringeCounts = new DcmTag(0x0018, 0x1045);

		/// <summary>(0018,1046) VR=DS Contrast Flow Rate</summary>
		public static DcmTag ContrastFlowRate = new DcmTag(0x0018, 0x1046);

		/// <summary>(0018,1047) VR=DS Contrast Flow Duration</summary>
		public static DcmTag ContrastFlowDuration = new DcmTag(0x0018, 0x1047);

		/// <summary>(0018,1048) VR=CS Contrast/Bolus Ingredient</summary>
		public static DcmTag ContrastBolusIngredient = new DcmTag(0x0018, 0x1048);

		/// <summary>(0018,1049) VR=DS Contrast/Bolus Ingredient Concentration</summary>
		public static DcmTag ContrastBolusIngredientConcentration = new DcmTag(0x0018, 0x1049);

		/// <summary>(0018,1050) VR=DS Spatial Resolution</summary>
		public static DcmTag SpatialResolution = new DcmTag(0x0018, 0x1050);

		/// <summary>(0018,1060) VR=DS Trigger Time</summary>
		public static DcmTag TriggerTime = new DcmTag(0x0018, 0x1060);

		/// <summary>(0018,1061) VR=LO Trigger Source or Type</summary>
		public static DcmTag TriggerSourceOrType = new DcmTag(0x0018, 0x1061);

		/// <summary>(0018,1062) VR=IS Nominal Interval</summary>
		public static DcmTag NominalInterval = new DcmTag(0x0018, 0x1062);

		/// <summary>(0018,1063) VR=DS Frame Time</summary>
		public static DcmTag FrameTime = new DcmTag(0x0018, 0x1063);

		/// <summary>(0018,1064) VR=LO Framing Type</summary>
		public static DcmTag FramingType = new DcmTag(0x0018, 0x1064);

		/// <summary>(0018,1065) VR=DS Frame Time Vector</summary>
		public static DcmTag FrameTimeVector = new DcmTag(0x0018, 0x1065);

		/// <summary>(0018,1066) VR=DS Frame Delay</summary>
		public static DcmTag FrameDelay = new DcmTag(0x0018, 0x1066);

		/// <summary>(0018,1067) VR=DS Image Trigger Delay</summary>
		public static DcmTag ImageTriggerDelay = new DcmTag(0x0018, 0x1067);

		/// <summary>(0018,1068) VR=DS Multiplex Group Time Offset</summary>
		public static DcmTag MultiplexGroupTimeOffset = new DcmTag(0x0018, 0x1068);

		/// <summary>(0018,1069) VR=DS Trigger Time Offset</summary>
		public static DcmTag TriggerTimeOffset = new DcmTag(0x0018, 0x1069);

		/// <summary>(0018,106a) VR=CS Synchronization Trigger</summary>
		public static DcmTag SynchronizationTrigger = new DcmTag(0x0018, 0x106a);

		/// <summary>(0018,106c) VR=US Synchronization Channel</summary>
		public static DcmTag SynchronizationChannel = new DcmTag(0x0018, 0x106c);

		/// <summary>(0018,106e) VR=UL Trigger Sample Position</summary>
		public static DcmTag TriggerSamplePosition = new DcmTag(0x0018, 0x106e);

		/// <summary>(0018,1070) VR=LO Radiopharmaceutical Route</summary>
		public static DcmTag RadiopharmaceuticalRoute = new DcmTag(0x0018, 0x1070);

		/// <summary>(0018,1071) VR=DS Radiopharmaceutical Volume</summary>
		public static DcmTag RadiopharmaceuticalVolume = new DcmTag(0x0018, 0x1071);

		/// <summary>(0018,1072) VR=TM Radiopharmaceutical Start Time</summary>
		public static DcmTag RadiopharmaceuticalStartTime = new DcmTag(0x0018, 0x1072);

		/// <summary>(0018,1073) VR=TM Radiopharmaceutical Stop Time</summary>
		public static DcmTag RadiopharmaceuticalStopTime = new DcmTag(0x0018, 0x1073);

		/// <summary>(0018,1074) VR=DS Radionuclide Total Dose</summary>
		public static DcmTag RadionuclideTotalDose = new DcmTag(0x0018, 0x1074);

		/// <summary>(0018,1075) VR=DS Radionuclide Half Life</summary>
		public static DcmTag RadionuclideHalfLife = new DcmTag(0x0018, 0x1075);

		/// <summary>(0018,1076) VR=DS Radionuclide Positron Fraction</summary>
		public static DcmTag RadionuclidePositronFraction = new DcmTag(0x0018, 0x1076);

		/// <summary>(0018,1077) VR=DS Radiopharmaceutical Specific Activity</summary>
		public static DcmTag RadiopharmaceuticalSpecificActivity = new DcmTag(0x0018, 0x1077);

		/// <summary>(0018,1078) VR=DT Radiopharmaceutical Start Datetime</summary>
		public static DcmTag RadiopharmaceuticalStartDatetime = new DcmTag(0x0018, 0x1078);

		/// <summary>(0018,1079) VR=DT Radiopharmaceutical Stop Datetime</summary>
		public static DcmTag RadiopharmaceuticalStopDatetime = new DcmTag(0x0018, 0x1079);

		/// <summary>(0018,1080) VR=CS Beat Rejection Flag</summary>
		public static DcmTag BeatRejectionFlag = new DcmTag(0x0018, 0x1080);

		/// <summary>(0018,1081) VR=IS Low R-R Value</summary>
		public static DcmTag LowRRValue = new DcmTag(0x0018, 0x1081);

		/// <summary>(0018,1082) VR=IS High R-R Value</summary>
		public static DcmTag HighRRValue = new DcmTag(0x0018, 0x1082);

		/// <summary>(0018,1083) VR=IS Intervals Acquired</summary>
		public static DcmTag IntervalsAcquired = new DcmTag(0x0018, 0x1083);

		/// <summary>(0018,1084) VR=IS Intervals Rejected</summary>
		public static DcmTag IntervalsRejected = new DcmTag(0x0018, 0x1084);

		/// <summary>(0018,1085) VR=LO PVC Rejection</summary>
		public static DcmTag PVCRejection = new DcmTag(0x0018, 0x1085);

		/// <summary>(0018,1086) VR=IS Skip Beats</summary>
		public static DcmTag SkipBeats = new DcmTag(0x0018, 0x1086);

		/// <summary>(0018,1088) VR=IS Heart Rate</summary>
		public static DcmTag HeartRate = new DcmTag(0x0018, 0x1088);

		/// <summary>(0018,1090) VR=IS Cardiac Number of Images</summary>
		public static DcmTag CardiacNumberOfImages = new DcmTag(0x0018, 0x1090);

		/// <summary>(0018,1094) VR=IS Trigger Window</summary>
		public static DcmTag TriggerWindow = new DcmTag(0x0018, 0x1094);

		/// <summary>(0018,1100) VR=DS Reconstruction Diameter</summary>
		public static DcmTag ReconstructionDiameter = new DcmTag(0x0018, 0x1100);

		/// <summary>(0018,1110) VR=DS Distance Source to Detector</summary>
		public static DcmTag DistanceSourceToDetector = new DcmTag(0x0018, 0x1110);

		/// <summary>(0018,1111) VR=DS Distance Source to Patient</summary>
		public static DcmTag DistanceSourceToPatient = new DcmTag(0x0018, 0x1111);

		/// <summary>(0018,1114) VR=DS Estimated Radiographic Magnification Factor</summary>
		public static DcmTag EstimatedRadiographicMagnificationFactor = new DcmTag(0x0018, 0x1114);

		/// <summary>(0018,1120) VR=DS Gantry/Detector Tilt</summary>
		public static DcmTag GantryDetectorTilt = new DcmTag(0x0018, 0x1120);

		/// <summary>(0018,1121) VR=DS Gantry/Detector Slew</summary>
		public static DcmTag GantryDetectorSlew = new DcmTag(0x0018, 0x1121);

		/// <summary>(0018,1130) VR=DS Table Height</summary>
		public static DcmTag TableHeight = new DcmTag(0x0018, 0x1130);

		/// <summary>(0018,1131) VR=DS Table Traverse</summary>
		public static DcmTag TableTraverse = new DcmTag(0x0018, 0x1131);

		/// <summary>(0018,1134) VR=CS Table Motion</summary>
		public static DcmTag TableMotion = new DcmTag(0x0018, 0x1134);

		/// <summary>(0018,1135) VR=DS Table Vertical Increment</summary>
		public static DcmTag TableVerticalIncrement = new DcmTag(0x0018, 0x1135);

		/// <summary>(0018,1136) VR=DS Table Lateral Increment</summary>
		public static DcmTag TableLateralIncrement = new DcmTag(0x0018, 0x1136);

		/// <summary>(0018,1137) VR=DS Table Longitudinal Increment</summary>
		public static DcmTag TableLongitudinalIncrement = new DcmTag(0x0018, 0x1137);

		/// <summary>(0018,1138) VR=DS Table Angle</summary>
		public static DcmTag TableAngle = new DcmTag(0x0018, 0x1138);

		/// <summary>(0018,113a) VR=CS Table Type</summary>
		public static DcmTag TableType = new DcmTag(0x0018, 0x113a);

		/// <summary>(0018,1140) VR=CS Rotation Direction</summary>
		public static DcmTag RotationDirection = new DcmTag(0x0018, 0x1140);

		/// <summary>(0018,1141) VR=DS Angular Position</summary>
		public static DcmTag AngularPosition = new DcmTag(0x0018, 0x1141);

		/// <summary>(0018,1142) VR=DS Radial Position</summary>
		public static DcmTag RadialPosition = new DcmTag(0x0018, 0x1142);

		/// <summary>(0018,1143) VR=DS Scan Arc</summary>
		public static DcmTag ScanArc = new DcmTag(0x0018, 0x1143);

		/// <summary>(0018,1144) VR=DS Angular Step</summary>
		public static DcmTag AngularStep = new DcmTag(0x0018, 0x1144);

		/// <summary>(0018,1145) VR=DS Center of Rotation Offset</summary>
		public static DcmTag CenterOfRotationOffset = new DcmTag(0x0018, 0x1145);

		/// <summary>(0018,1146) VR=DS Rotation Offset (Retired)</summary>
		public static DcmTag RotationOffsetRETIRED = new DcmTag(0x0018, 0x1146);

		/// <summary>(0018,1147) VR=CS Field of View Shape</summary>
		public static DcmTag FieldOfViewShape = new DcmTag(0x0018, 0x1147);

		/// <summary>(0018,1149) VR=IS Field of View Dimension(s)</summary>
		public static DcmTag FieldOfViewDimensions = new DcmTag(0x0018, 0x1149);

		/// <summary>(0018,1150) VR=IS Exposure Time</summary>
		public static DcmTag ExposureTime = new DcmTag(0x0018, 0x1150);

		/// <summary>(0018,1151) VR=IS X-ray Tube Current</summary>
		public static DcmTag XrayTubeCurrent = new DcmTag(0x0018, 0x1151);

		/// <summary>(0018,1152) VR=IS Exposure</summary>
		public static DcmTag Exposure = new DcmTag(0x0018, 0x1152);

		/// <summary>(0018,1153) VR=IS Exposure in µAs</summary>
		public static DcmTag ExposureInµAs = new DcmTag(0x0018, 0x1153);

		/// <summary>(0018,1154) VR=DS Average Pulse Width</summary>
		public static DcmTag AveragePulseWidth = new DcmTag(0x0018, 0x1154);

		/// <summary>(0018,1155) VR=CS Radiation Setting</summary>
		public static DcmTag RadiationSetting = new DcmTag(0x0018, 0x1155);

		/// <summary>(0018,1156) VR=CS Rectification Type</summary>
		public static DcmTag RectificationType = new DcmTag(0x0018, 0x1156);

		/// <summary>(0018,115a) VR=CS Radiation Mode</summary>
		public static DcmTag RadiationMode = new DcmTag(0x0018, 0x115a);

		/// <summary>(0018,115e) VR=DS Image and Fluoroscopy Area Dose Product</summary>
		public static DcmTag ImageAndFluoroscopyAreaDoseProduct = new DcmTag(0x0018, 0x115e);

		/// <summary>(0018,1160) VR=SH Filter Type</summary>
		public static DcmTag FilterType = new DcmTag(0x0018, 0x1160);

		/// <summary>(0018,1161) VR=LO Type of Filters</summary>
		public static DcmTag TypeOfFilters = new DcmTag(0x0018, 0x1161);

		/// <summary>(0018,1162) VR=DS Intensifier Size</summary>
		public static DcmTag IntensifierSize = new DcmTag(0x0018, 0x1162);

		/// <summary>(0018,1164) VR=DS Imager Pixel Spacing</summary>
		public static DcmTag ImagerPixelSpacing = new DcmTag(0x0018, 0x1164);

		/// <summary>(0018,1166) VR=CS Grid</summary>
		public static DcmTag Grid = new DcmTag(0x0018, 0x1166);

		/// <summary>(0018,1170) VR=IS Generator Power</summary>
		public static DcmTag GeneratorPower = new DcmTag(0x0018, 0x1170);

		/// <summary>(0018,1180) VR=SH Collimator/grid Name</summary>
		public static DcmTag CollimatorgridName = new DcmTag(0x0018, 0x1180);

		/// <summary>(0018,1181) VR=CS Collimator Type</summary>
		public static DcmTag CollimatorType = new DcmTag(0x0018, 0x1181);

		/// <summary>(0018,1182) VR=IS Focal Distance</summary>
		public static DcmTag FocalDistance = new DcmTag(0x0018, 0x1182);

		/// <summary>(0018,1183) VR=DS X Focus Center</summary>
		public static DcmTag XFocusCenter = new DcmTag(0x0018, 0x1183);

		/// <summary>(0018,1184) VR=DS Y Focus Center</summary>
		public static DcmTag YFocusCenter = new DcmTag(0x0018, 0x1184);

		/// <summary>(0018,1190) VR=DS Focal Spot(s)</summary>
		public static DcmTag FocalSpots = new DcmTag(0x0018, 0x1190);

		/// <summary>(0018,1191) VR=CS Anode Target Material</summary>
		public static DcmTag AnodeTargetMaterial = new DcmTag(0x0018, 0x1191);

		/// <summary>(0018,11a0) VR=DS Body Part Thickness</summary>
		public static DcmTag BodyPartThickness = new DcmTag(0x0018, 0x11a0);

		/// <summary>(0018,11a2) VR=DS Compression Force</summary>
		public static DcmTag CompressionForce = new DcmTag(0x0018, 0x11a2);

		/// <summary>(0018,1200) VR=DA Date of Last Calibration</summary>
		public static DcmTag DateOfLastCalibration = new DcmTag(0x0018, 0x1200);

		/// <summary>(0018,1201) VR=TM Time of Last Calibration</summary>
		public static DcmTag TimeOfLastCalibration = new DcmTag(0x0018, 0x1201);

		/// <summary>(0018,1210) VR=SH Convolution Kernel</summary>
		public static DcmTag ConvolutionKernel = new DcmTag(0x0018, 0x1210);

		/// <summary>(0018,1240) VR=IS Upper/Lower Pixel Values (Retired)</summary>
		public static DcmTag UpperLowerPixelValuesRETIRED = new DcmTag(0x0018, 0x1240);

		/// <summary>(0018,1242) VR=IS Actual Frame Duration</summary>
		public static DcmTag ActualFrameDuration = new DcmTag(0x0018, 0x1242);

		/// <summary>(0018,1243) VR=IS Count Rate</summary>
		public static DcmTag CountRate = new DcmTag(0x0018, 0x1243);

		/// <summary>(0018,1244) VR=US Preferred Playback Sequencing</summary>
		public static DcmTag PreferredPlaybackSequencing = new DcmTag(0x0018, 0x1244);

		/// <summary>(0018,1250) VR=SH Receive Coil Name</summary>
		public static DcmTag ReceiveCoilName = new DcmTag(0x0018, 0x1250);

		/// <summary>(0018,1251) VR=SH Transmit Coil Name</summary>
		public static DcmTag TransmitCoilName = new DcmTag(0x0018, 0x1251);

		/// <summary>(0018,1260) VR=SH Plate Type</summary>
		public static DcmTag PlateType = new DcmTag(0x0018, 0x1260);

		/// <summary>(0018,1261) VR=LO Phosphor Type</summary>
		public static DcmTag PhosphorType = new DcmTag(0x0018, 0x1261);

		/// <summary>(0018,1300) VR=DS Scan Velocity</summary>
		public static DcmTag ScanVelocity = new DcmTag(0x0018, 0x1300);

		/// <summary>(0018,1301) VR=CS Whole Body Technique</summary>
		public static DcmTag WholeBodyTechnique = new DcmTag(0x0018, 0x1301);

		/// <summary>(0018,1302) VR=IS Scan Length</summary>
		public static DcmTag ScanLength = new DcmTag(0x0018, 0x1302);

		/// <summary>(0018,1310) VR=US Acquisition Matrix</summary>
		public static DcmTag AcquisitionMatrix = new DcmTag(0x0018, 0x1310);

		/// <summary>(0018,1312) VR=CS In-plane Phase Encoding Direction</summary>
		public static DcmTag InplanePhaseEncodingDirection = new DcmTag(0x0018, 0x1312);

		/// <summary>(0018,1314) VR=DS Flip Angle</summary>
		public static DcmTag FlipAngle = new DcmTag(0x0018, 0x1314);

		/// <summary>(0018,1315) VR=CS Variable Flip Angle Flag</summary>
		public static DcmTag VariableFlipAngleFlag = new DcmTag(0x0018, 0x1315);

		/// <summary>(0018,1316) VR=DS SAR</summary>
		public static DcmTag SAR = new DcmTag(0x0018, 0x1316);

		/// <summary>(0018,1318) VR=DS dB/dt</summary>
		public static DcmTag DBdt = new DcmTag(0x0018, 0x1318);

		/// <summary>(0018,1400) VR=LO Acquisition Device Processing Description</summary>
		public static DcmTag AcquisitionDeviceProcessingDescription = new DcmTag(0x0018, 0x1400);

		/// <summary>(0018,1401) VR=LO Acquisition Device Processing Code</summary>
		public static DcmTag AcquisitionDeviceProcessingCode = new DcmTag(0x0018, 0x1401);

		/// <summary>(0018,1402) VR=CS Cassette Orientation</summary>
		public static DcmTag CassetteOrientation = new DcmTag(0x0018, 0x1402);

		/// <summary>(0018,1403) VR=CS Cassette Size</summary>
		public static DcmTag CassetteSize = new DcmTag(0x0018, 0x1403);

		/// <summary>(0018,1404) VR=US Exposures on Plate</summary>
		public static DcmTag ExposuresOnPlate = new DcmTag(0x0018, 0x1404);

		/// <summary>(0018,1405) VR=IS Relative X-ray Exposure</summary>
		public static DcmTag RelativeXrayExposure = new DcmTag(0x0018, 0x1405);

		/// <summary>(0018,1450) VR=DS Column Angulation</summary>
		public static DcmTag ColumnAngulation = new DcmTag(0x0018, 0x1450);

		/// <summary>(0018,1460) VR=DS Tomo Layer Height</summary>
		public static DcmTag TomoLayerHeight = new DcmTag(0x0018, 0x1460);

		/// <summary>(0018,1470) VR=DS Tomo Angle</summary>
		public static DcmTag TomoAngle = new DcmTag(0x0018, 0x1470);

		/// <summary>(0018,1480) VR=DS Tomo Time</summary>
		public static DcmTag TomoTime = new DcmTag(0x0018, 0x1480);

		/// <summary>(0018,1490) VR=CS Tomo Type</summary>
		public static DcmTag TomoType = new DcmTag(0x0018, 0x1490);

		/// <summary>(0018,1491) VR=CS Tomo Class</summary>
		public static DcmTag TomoClass = new DcmTag(0x0018, 0x1491);

		/// <summary>(0018,1495) VR=IS Number of Tomosynthesis Source Images</summary>
		public static DcmTag NumberOfTomosynthesisSourceImages = new DcmTag(0x0018, 0x1495);

		/// <summary>(0018,1500) VR=CS Positioner Motion</summary>
		public static DcmTag PositionerMotion = new DcmTag(0x0018, 0x1500);

		/// <summary>(0018,1508) VR=CS Positioner Type</summary>
		public static DcmTag PositionerType = new DcmTag(0x0018, 0x1508);

		/// <summary>(0018,1510) VR=DS Positioner Primary Angle</summary>
		public static DcmTag PositionerPrimaryAngle = new DcmTag(0x0018, 0x1510);

		/// <summary>(0018,1511) VR=DS Positioner Secondary Angle</summary>
		public static DcmTag PositionerSecondaryAngle = new DcmTag(0x0018, 0x1511);

		/// <summary>(0018,1520) VR=DS Positioner Primary Angle Increment</summary>
		public static DcmTag PositionerPrimaryAngleIncrement = new DcmTag(0x0018, 0x1520);

		/// <summary>(0018,1521) VR=DS Positioner Secondary Angle Increment</summary>
		public static DcmTag PositionerSecondaryAngleIncrement = new DcmTag(0x0018, 0x1521);

		/// <summary>(0018,1530) VR=DS Detector Primary Angle</summary>
		public static DcmTag DetectorPrimaryAngle = new DcmTag(0x0018, 0x1530);

		/// <summary>(0018,1531) VR=DS Detector Secondary Angle</summary>
		public static DcmTag DetectorSecondaryAngle = new DcmTag(0x0018, 0x1531);

		/// <summary>(0018,1600) VR=CS Shutter Shape</summary>
		public static DcmTag ShutterShape = new DcmTag(0x0018, 0x1600);

		/// <summary>(0018,1602) VR=IS Shutter Left Vertical Edge</summary>
		public static DcmTag ShutterLeftVerticalEdge = new DcmTag(0x0018, 0x1602);

		/// <summary>(0018,1604) VR=IS Shutter Right Vertical Edge</summary>
		public static DcmTag ShutterRightVerticalEdge = new DcmTag(0x0018, 0x1604);

		/// <summary>(0018,1606) VR=IS Shutter Upper Horizontal Edge</summary>
		public static DcmTag ShutterUpperHorizontalEdge = new DcmTag(0x0018, 0x1606);

		/// <summary>(0018,1608) VR=IS Shutter Lower Horizontal Edge</summary>
		public static DcmTag ShutterLowerHorizontalEdge = new DcmTag(0x0018, 0x1608);

		/// <summary>(0018,1610) VR=IS Center of Circular Shutter</summary>
		public static DcmTag CenterOfCircularShutter = new DcmTag(0x0018, 0x1610);

		/// <summary>(0018,1612) VR=IS Radius of Circular Shutter</summary>
		public static DcmTag RadiusOfCircularShutter = new DcmTag(0x0018, 0x1612);

		/// <summary>(0018,1620) VR=IS Vertices of the Polygonal Shutter</summary>
		public static DcmTag VerticesOfThePolygonalShutter = new DcmTag(0x0018, 0x1620);

		/// <summary>(0018,1622) VR=US Shutter Presentation Value</summary>
		public static DcmTag ShutterPresentationValue = new DcmTag(0x0018, 0x1622);

		/// <summary>(0018,1623) VR=US Shutter Overlay Group</summary>
		public static DcmTag ShutterOverlayGroup = new DcmTag(0x0018, 0x1623);

		/// <summary>(0018,1624) VR=US Shutter Presentation Color CIELab Value</summary>
		public static DcmTag ShutterPresentationColorCIELabValue = new DcmTag(0x0018, 0x1624);

		/// <summary>(0018,1700) VR=CS Collimator Shape</summary>
		public static DcmTag CollimatorShape = new DcmTag(0x0018, 0x1700);

		/// <summary>(0018,1702) VR=IS Collimator Left Vertical Edge</summary>
		public static DcmTag CollimatorLeftVerticalEdge = new DcmTag(0x0018, 0x1702);

		/// <summary>(0018,1704) VR=IS Collimator Right Vertical Edge</summary>
		public static DcmTag CollimatorRightVerticalEdge = new DcmTag(0x0018, 0x1704);

		/// <summary>(0018,1706) VR=IS Collimator Upper Horizontal Edge</summary>
		public static DcmTag CollimatorUpperHorizontalEdge = new DcmTag(0x0018, 0x1706);

		/// <summary>(0018,1708) VR=IS Collimator Lower Horizontal Edge</summary>
		public static DcmTag CollimatorLowerHorizontalEdge = new DcmTag(0x0018, 0x1708);

		/// <summary>(0018,1710) VR=IS Center of Circular Collimator</summary>
		public static DcmTag CenterOfCircularCollimator = new DcmTag(0x0018, 0x1710);

		/// <summary>(0018,1712) VR=IS Radius of Circular Collimator</summary>
		public static DcmTag RadiusOfCircularCollimator = new DcmTag(0x0018, 0x1712);

		/// <summary>(0018,1720) VR=IS Vertices of the Polygonal Collimator</summary>
		public static DcmTag VerticesOfThePolygonalCollimator = new DcmTag(0x0018, 0x1720);

		/// <summary>(0018,1800) VR=CS Acquisition Time Synchronized</summary>
		public static DcmTag AcquisitionTimeSynchronized = new DcmTag(0x0018, 0x1800);

		/// <summary>(0018,1801) VR=SH Time Source</summary>
		public static DcmTag TimeSource = new DcmTag(0x0018, 0x1801);

		/// <summary>(0018,1802) VR=CS Time Distribution Protocol</summary>
		public static DcmTag TimeDistributionProtocol = new DcmTag(0x0018, 0x1802);

		/// <summary>(0018,1803) VR=LO NTP Source Address</summary>
		public static DcmTag NTPSourceAddress = new DcmTag(0x0018, 0x1803);

		/// <summary>(0018,2001) VR=IS Page Number Vector</summary>
		public static DcmTag PageNumberVector = new DcmTag(0x0018, 0x2001);

		/// <summary>(0018,2002) VR=SH Frame Label Vector</summary>
		public static DcmTag FrameLabelVector = new DcmTag(0x0018, 0x2002);

		/// <summary>(0018,2003) VR=DS Frame Primary Angle Vector</summary>
		public static DcmTag FramePrimaryAngleVector = new DcmTag(0x0018, 0x2003);

		/// <summary>(0018,2004) VR=DS Frame Secondary Angle Vector</summary>
		public static DcmTag FrameSecondaryAngleVector = new DcmTag(0x0018, 0x2004);

		/// <summary>(0018,2005) VR=DS Slice Location Vector</summary>
		public static DcmTag SliceLocationVector = new DcmTag(0x0018, 0x2005);

		/// <summary>(0018,2006) VR=SH Display Window Label Vector</summary>
		public static DcmTag DisplayWindowLabelVector = new DcmTag(0x0018, 0x2006);

		/// <summary>(0018,2010) VR=DS Nominal Scanned Pixel Spacing</summary>
		public static DcmTag NominalScannedPixelSpacing = new DcmTag(0x0018, 0x2010);

		/// <summary>(0018,2020) VR=CS Digitizing Device Transport Direction</summary>
		public static DcmTag DigitizingDeviceTransportDirection = new DcmTag(0x0018, 0x2020);

		/// <summary>(0018,2030) VR=DS Rotation of Scanned Film</summary>
		public static DcmTag RotationOfScannedFilm = new DcmTag(0x0018, 0x2030);

		/// <summary>(0018,3100) VR=CS IVUS Acquisition</summary>
		public static DcmTag IVUSAcquisition = new DcmTag(0x0018, 0x3100);

		/// <summary>(0018,3101) VR=DS IVUS Pullback Rate</summary>
		public static DcmTag IVUSPullbackRate = new DcmTag(0x0018, 0x3101);

		/// <summary>(0018,3102) VR=DS IVUS Gated Rate</summary>
		public static DcmTag IVUSGatedRate = new DcmTag(0x0018, 0x3102);

		/// <summary>(0018,3103) VR=IS IVUS Pullback Start Frame Number</summary>
		public static DcmTag IVUSPullbackStartFrameNumber = new DcmTag(0x0018, 0x3103);

		/// <summary>(0018,3104) VR=IS IVUS Pullback Stop Frame Number</summary>
		public static DcmTag IVUSPullbackStopFrameNumber = new DcmTag(0x0018, 0x3104);

		/// <summary>(0018,3105) VR=IS Lesion Number</summary>
		public static DcmTag LesionNumber = new DcmTag(0x0018, 0x3105);

		/// <summary>(0018,4000) VR=LT Acquisition Comments (Retired)</summary>
		public static DcmTag AcquisitionCommentsRETIRED = new DcmTag(0x0018, 0x4000);

		/// <summary>(0018,5000) VR=SH Output Power</summary>
		public static DcmTag OutputPower = new DcmTag(0x0018, 0x5000);

		/// <summary>(0018,5010) VR=LO Transducer Data</summary>
		public static DcmTag TransducerData = new DcmTag(0x0018, 0x5010);

		/// <summary>(0018,5012) VR=DS Focus Depth</summary>
		public static DcmTag FocusDepth = new DcmTag(0x0018, 0x5012);

		/// <summary>(0018,5020) VR=LO Processing Function</summary>
		public static DcmTag ProcessingFunction = new DcmTag(0x0018, 0x5020);

		/// <summary>(0018,5021) VR=LO Postprocessing Function</summary>
		public static DcmTag PostprocessingFunction = new DcmTag(0x0018, 0x5021);

		/// <summary>(0018,5022) VR=DS Mechanical Index</summary>
		public static DcmTag MechanicalIndex = new DcmTag(0x0018, 0x5022);

		/// <summary>(0018,5024) VR=DS Bone Thermal Index</summary>
		public static DcmTag BoneThermalIndex = new DcmTag(0x0018, 0x5024);

		/// <summary>(0018,5026) VR=DS Cranial Thermal Index</summary>
		public static DcmTag CranialThermalIndex = new DcmTag(0x0018, 0x5026);

		/// <summary>(0018,5027) VR=DS Soft Tissue Thermal Index</summary>
		public static DcmTag SoftTissueThermalIndex = new DcmTag(0x0018, 0x5027);

		/// <summary>(0018,5028) VR=DS Soft Tissue-focus Thermal Index</summary>
		public static DcmTag SoftTissuefocusThermalIndex = new DcmTag(0x0018, 0x5028);

		/// <summary>(0018,5029) VR=DS Soft Tissue-surface Thermal Index</summary>
		public static DcmTag SoftTissuesurfaceThermalIndex = new DcmTag(0x0018, 0x5029);

		/// <summary>(0018,5030) VR=DS Dynamic Range (Retired)</summary>
		public static DcmTag DynamicRangeRETIRED = new DcmTag(0x0018, 0x5030);

		/// <summary>(0018,5040) VR=DS Total Gain (Retired)</summary>
		public static DcmTag TotalGainRETIRED = new DcmTag(0x0018, 0x5040);

		/// <summary>(0018,5050) VR=IS Depth of Scan Field</summary>
		public static DcmTag DepthOfScanField = new DcmTag(0x0018, 0x5050);

		/// <summary>(0018,5100) VR=CS Patient Position</summary>
		public static DcmTag PatientPosition = new DcmTag(0x0018, 0x5100);

		/// <summary>(0018,5101) VR=CS View Position</summary>
		public static DcmTag ViewPosition = new DcmTag(0x0018, 0x5101);

		/// <summary>(0018,5104) VR=SQ Projection Eponymous Name Code Sequence</summary>
		public static DcmTag ProjectionEponymousNameCodeSequence = new DcmTag(0x0018, 0x5104);

		/// <summary>(0018,5210) VR=DS Image Transformation Matrix (Retired)</summary>
		public static DcmTag ImageTransformationMatrixRETIRED = new DcmTag(0x0018, 0x5210);

		/// <summary>(0018,5212) VR=DS Image Translation Vector (Retired)</summary>
		public static DcmTag ImageTranslationVectorRETIRED = new DcmTag(0x0018, 0x5212);

		/// <summary>(0018,6000) VR=DS Sensitivity</summary>
		public static DcmTag Sensitivity = new DcmTag(0x0018, 0x6000);

		/// <summary>(0018,6011) VR=SQ Sequence of Ultrasound Regions</summary>
		public static DcmTag SequenceOfUltrasoundRegions = new DcmTag(0x0018, 0x6011);

		/// <summary>(0018,6012) VR=US Region Spatial Format</summary>
		public static DcmTag RegionSpatialFormat = new DcmTag(0x0018, 0x6012);

		/// <summary>(0018,6014) VR=US Region Data Type</summary>
		public static DcmTag RegionDataType = new DcmTag(0x0018, 0x6014);

		/// <summary>(0018,6016) VR=UL Region Flags</summary>
		public static DcmTag RegionFlags = new DcmTag(0x0018, 0x6016);

		/// <summary>(0018,6018) VR=UL Region Location Min X0</summary>
		public static DcmTag RegionLocationMinX0 = new DcmTag(0x0018, 0x6018);

		/// <summary>(0018,601a) VR=UL Region Location Min Y0</summary>
		public static DcmTag RegionLocationMinY0 = new DcmTag(0x0018, 0x601a);

		/// <summary>(0018,601c) VR=UL Region Location Max X1</summary>
		public static DcmTag RegionLocationMaxX1 = new DcmTag(0x0018, 0x601c);

		/// <summary>(0018,601e) VR=UL Region Location Max Y1</summary>
		public static DcmTag RegionLocationMaxY1 = new DcmTag(0x0018, 0x601e);

		/// <summary>(0018,6020) VR=SL Reference Pixel X0</summary>
		public static DcmTag ReferencePixelX0 = new DcmTag(0x0018, 0x6020);

		/// <summary>(0018,6022) VR=SL Reference Pixel Y0</summary>
		public static DcmTag ReferencePixelY0 = new DcmTag(0x0018, 0x6022);

		/// <summary>(0018,6024) VR=US Physical Units X Direction</summary>
		public static DcmTag PhysicalUnitsXDirection = new DcmTag(0x0018, 0x6024);

		/// <summary>(0018,6026) VR=US Physical Units Y Direction</summary>
		public static DcmTag PhysicalUnitsYDirection = new DcmTag(0x0018, 0x6026);

		/// <summary>(0018,6028) VR=FD Reference Pixel Physical Value X</summary>
		public static DcmTag ReferencePixelPhysicalValueX = new DcmTag(0x0018, 0x6028);

		/// <summary>(0018,602a) VR=FD Reference Pixel Physical Value Y</summary>
		public static DcmTag ReferencePixelPhysicalValueY = new DcmTag(0x0018, 0x602a);

		/// <summary>(0018,602c) VR=FD Physical Delta X</summary>
		public static DcmTag PhysicalDeltaX = new DcmTag(0x0018, 0x602c);

		/// <summary>(0018,602e) VR=FD Physical Delta Y</summary>
		public static DcmTag PhysicalDeltaY = new DcmTag(0x0018, 0x602e);

		/// <summary>(0018,6030) VR=UL Transducer Frequency</summary>
		public static DcmTag TransducerFrequency = new DcmTag(0x0018, 0x6030);

		/// <summary>(0018,6031) VR=CS Transducer Type</summary>
		public static DcmTag TransducerType = new DcmTag(0x0018, 0x6031);

		/// <summary>(0018,6032) VR=UL Pulse Repetition Frequency</summary>
		public static DcmTag PulseRepetitionFrequency = new DcmTag(0x0018, 0x6032);

		/// <summary>(0018,6034) VR=FD Doppler Correction Angle</summary>
		public static DcmTag DopplerCorrectionAngle = new DcmTag(0x0018, 0x6034);

		/// <summary>(0018,6036) VR=FD Steering Angle</summary>
		public static DcmTag SteeringAngle = new DcmTag(0x0018, 0x6036);

		/// <summary>(0018,6038) VR=UL Doppler Sample Volume X Position (Retired)</summary>
		public static DcmTag DopplerSampleVolumeXPositionRETIRED = new DcmTag(0x0018, 0x6038);

		/// <summary>(0018,6039) VR=SL Doppler Sample Volume X Position</summary>
		public static DcmTag DopplerSampleVolumeXPosition = new DcmTag(0x0018, 0x6039);

		/// <summary>(0018,603a) VR=UL Doppler Sample Volume Y Position (Retired)</summary>
		public static DcmTag DopplerSampleVolumeYPositionRETIRED = new DcmTag(0x0018, 0x603a);

		/// <summary>(0018,603b) VR=SL Doppler Sample Volume Y Position</summary>
		public static DcmTag DopplerSampleVolumeYPosition = new DcmTag(0x0018, 0x603b);

		/// <summary>(0018,603c) VR=UL TM-Line Position X0 (Retired)</summary>
		public static DcmTag TMLinePositionX0RETIRED = new DcmTag(0x0018, 0x603c);

		/// <summary>(0018,603d) VR=SL TM-Line Position X0</summary>
		public static DcmTag TMLinePositionX0 = new DcmTag(0x0018, 0x603d);

		/// <summary>(0018,603e) VR=UL TM-Line Position Y0 (Retired)</summary>
		public static DcmTag TMLinePositionY0RETIRED = new DcmTag(0x0018, 0x603e);

		/// <summary>(0018,603f) VR=SL TM-Line Position Y0</summary>
		public static DcmTag TMLinePositionY0 = new DcmTag(0x0018, 0x603f);

		/// <summary>(0018,6040) VR=UL TM-Line Position X1 (Retired)</summary>
		public static DcmTag TMLinePositionX1RETIRED = new DcmTag(0x0018, 0x6040);

		/// <summary>(0018,6041) VR=SL TM-Line Position X1</summary>
		public static DcmTag TMLinePositionX1 = new DcmTag(0x0018, 0x6041);

		/// <summary>(0018,6042) VR=UL TM-Line Position Y1 (Retired)</summary>
		public static DcmTag TMLinePositionY1RETIRED = new DcmTag(0x0018, 0x6042);

		/// <summary>(0018,6043) VR=SL TM-Line Position Y1</summary>
		public static DcmTag TMLinePositionY1 = new DcmTag(0x0018, 0x6043);

		/// <summary>(0018,6044) VR=US Pixel Component Organization</summary>
		public static DcmTag PixelComponentOrganization = new DcmTag(0x0018, 0x6044);

		/// <summary>(0018,6046) VR=UL Pixel Component Mask</summary>
		public static DcmTag PixelComponentMask = new DcmTag(0x0018, 0x6046);

		/// <summary>(0018,6048) VR=UL Pixel Component Range Start</summary>
		public static DcmTag PixelComponentRangeStart = new DcmTag(0x0018, 0x6048);

		/// <summary>(0018,604a) VR=UL Pixel Component Range Stop</summary>
		public static DcmTag PixelComponentRangeStop = new DcmTag(0x0018, 0x604a);

		/// <summary>(0018,604c) VR=US Pixel Component Physical Units</summary>
		public static DcmTag PixelComponentPhysicalUnits = new DcmTag(0x0018, 0x604c);

		/// <summary>(0018,604e) VR=US Pixel Component Data Type</summary>
		public static DcmTag PixelComponentDataType = new DcmTag(0x0018, 0x604e);

		/// <summary>(0018,6050) VR=UL Number of Table Break Points</summary>
		public static DcmTag NumberOfTableBreakPoints = new DcmTag(0x0018, 0x6050);

		/// <summary>(0018,6052) VR=UL Table of X Break Points</summary>
		public static DcmTag TableOfXBreakPoints = new DcmTag(0x0018, 0x6052);

		/// <summary>(0018,6054) VR=FD Table of Y Break Points</summary>
		public static DcmTag TableOfYBreakPoints = new DcmTag(0x0018, 0x6054);

		/// <summary>(0018,6056) VR=UL Number of Table Entries</summary>
		public static DcmTag NumberOfTableEntries = new DcmTag(0x0018, 0x6056);

		/// <summary>(0018,6058) VR=UL Table of Pixel Values</summary>
		public static DcmTag TableOfPixelValues = new DcmTag(0x0018, 0x6058);

		/// <summary>(0018,605a) VR=FL Table of Parameter Values</summary>
		public static DcmTag TableOfParameterValues = new DcmTag(0x0018, 0x605a);

		/// <summary>(0018,6060) VR=FL R Wave Time Vector</summary>
		public static DcmTag RWaveTimeVector = new DcmTag(0x0018, 0x6060);

		/// <summary>(0018,7000) VR=CS Detector Conditions Nominal Flag</summary>
		public static DcmTag DetectorConditionsNominalFlag = new DcmTag(0x0018, 0x7000);

		/// <summary>(0018,7001) VR=DS Detector Temperature</summary>
		public static DcmTag DetectorTemperature = new DcmTag(0x0018, 0x7001);

		/// <summary>(0018,7004) VR=CS Detector Type</summary>
		public static DcmTag DetectorType = new DcmTag(0x0018, 0x7004);

		/// <summary>(0018,7005) VR=CS Detector Configuration</summary>
		public static DcmTag DetectorConfiguration = new DcmTag(0x0018, 0x7005);

		/// <summary>(0018,7006) VR=LT Detector Description</summary>
		public static DcmTag DetectorDescription = new DcmTag(0x0018, 0x7006);

		/// <summary>(0018,7008) VR=LT Detector Mode</summary>
		public static DcmTag DetectorMode = new DcmTag(0x0018, 0x7008);

		/// <summary>(0018,700a) VR=SH Detector ID</summary>
		public static DcmTag DetectorID = new DcmTag(0x0018, 0x700a);

		/// <summary>(0018,700c) VR=DA Date of Last Detector Calibration</summary>
		public static DcmTag DateOfLastDetectorCalibration = new DcmTag(0x0018, 0x700c);

		/// <summary>(0018,700e) VR=TM Time of Last Detector Calibration</summary>
		public static DcmTag TimeOfLastDetectorCalibration = new DcmTag(0x0018, 0x700e);

		/// <summary>(0018,7010) VR=IS Exposures on Detector Since Last Calibration</summary>
		public static DcmTag ExposuresOnDetectorSinceLastCalibration = new DcmTag(0x0018, 0x7010);

		/// <summary>(0018,7011) VR=IS Exposures on Detector Since Manufactured</summary>
		public static DcmTag ExposuresOnDetectorSinceManufactured = new DcmTag(0x0018, 0x7011);

		/// <summary>(0018,7012) VR=DS Detector Time Since Last Exposure</summary>
		public static DcmTag DetectorTimeSinceLastExposure = new DcmTag(0x0018, 0x7012);

		/// <summary>(0018,7014) VR=DS Detector Active Time</summary>
		public static DcmTag DetectorActiveTime = new DcmTag(0x0018, 0x7014);

		/// <summary>(0018,7016) VR=DS Detector Activation Offset From Exposure</summary>
		public static DcmTag DetectorActivationOffsetFromExposure = new DcmTag(0x0018, 0x7016);

		/// <summary>(0018,701a) VR=DS Detector Binning</summary>
		public static DcmTag DetectorBinning = new DcmTag(0x0018, 0x701a);

		/// <summary>(0018,7020) VR=DS Detector Element Physical Size</summary>
		public static DcmTag DetectorElementPhysicalSize = new DcmTag(0x0018, 0x7020);

		/// <summary>(0018,7022) VR=DS Detector Element Spacing</summary>
		public static DcmTag DetectorElementSpacing = new DcmTag(0x0018, 0x7022);

		/// <summary>(0018,7024) VR=CS Detector Active Shape</summary>
		public static DcmTag DetectorActiveShape = new DcmTag(0x0018, 0x7024);

		/// <summary>(0018,7026) VR=DS Detector Active Dimension(s)</summary>
		public static DcmTag DetectorActiveDimensions = new DcmTag(0x0018, 0x7026);

		/// <summary>(0018,7028) VR=DS Detector Active Origin</summary>
		public static DcmTag DetectorActiveOrigin = new DcmTag(0x0018, 0x7028);

		/// <summary>(0018,702a) VR=LO Detector Manufacturer Name</summary>
		public static DcmTag DetectorManufacturerName = new DcmTag(0x0018, 0x702a);

		/// <summary>(0018,702b) VR=LO Detector Manufacturer's Model Name</summary>
		public static DcmTag DetectorManufacturersModelName = new DcmTag(0x0018, 0x702b);

		/// <summary>(0018,7030) VR=DS Field of View Origin</summary>
		public static DcmTag FieldOfViewOrigin = new DcmTag(0x0018, 0x7030);

		/// <summary>(0018,7032) VR=DS Field of View Rotation</summary>
		public static DcmTag FieldOfViewRotation = new DcmTag(0x0018, 0x7032);

		/// <summary>(0018,7034) VR=CS Field of View Horizontal Flip</summary>
		public static DcmTag FieldOfViewHorizontalFlip = new DcmTag(0x0018, 0x7034);

		/// <summary>(0018,7040) VR=LT Grid Absorbing Material</summary>
		public static DcmTag GridAbsorbingMaterial = new DcmTag(0x0018, 0x7040);

		/// <summary>(0018,7041) VR=LT Grid Spacing Material</summary>
		public static DcmTag GridSpacingMaterial = new DcmTag(0x0018, 0x7041);

		/// <summary>(0018,7042) VR=DS Grid Thickness</summary>
		public static DcmTag GridThickness = new DcmTag(0x0018, 0x7042);

		/// <summary>(0018,7044) VR=DS Grid Pitch</summary>
		public static DcmTag GridPitch = new DcmTag(0x0018, 0x7044);

		/// <summary>(0018,7046) VR=IS Grid Aspect Ratio</summary>
		public static DcmTag GridAspectRatio = new DcmTag(0x0018, 0x7046);

		/// <summary>(0018,7048) VR=DS Grid Period</summary>
		public static DcmTag GridPeriod = new DcmTag(0x0018, 0x7048);

		/// <summary>(0018,704c) VR=DS Grid Focal Distance</summary>
		public static DcmTag GridFocalDistance = new DcmTag(0x0018, 0x704c);

		/// <summary>(0018,7050) VR=CS Filter Material</summary>
		public static DcmTag FilterMaterial = new DcmTag(0x0018, 0x7050);

		/// <summary>(0018,7052) VR=DS Filter Thickness Minimum</summary>
		public static DcmTag FilterThicknessMinimum = new DcmTag(0x0018, 0x7052);

		/// <summary>(0018,7054) VR=DS Filter Thickness Maximum</summary>
		public static DcmTag FilterThicknessMaximum = new DcmTag(0x0018, 0x7054);

		/// <summary>(0018,7060) VR=CS Exposure Control Mode</summary>
		public static DcmTag ExposureControlMode = new DcmTag(0x0018, 0x7060);

		/// <summary>(0018,7062) VR=LT Exposure Control Mode Description</summary>
		public static DcmTag ExposureControlModeDescription = new DcmTag(0x0018, 0x7062);

		/// <summary>(0018,7064) VR=CS Exposure Status</summary>
		public static DcmTag ExposureStatus = new DcmTag(0x0018, 0x7064);

		/// <summary>(0018,7065) VR=DS Phototimer Setting</summary>
		public static DcmTag PhototimerSetting = new DcmTag(0x0018, 0x7065);

		/// <summary>(0018,8150) VR=DS Exposure Time in µS</summary>
		public static DcmTag ExposureTimeInµS = new DcmTag(0x0018, 0x8150);

		/// <summary>(0018,8151) VR=DS X-Ray Tube Current in µA</summary>
		public static DcmTag XRayTubeCurrentInµA = new DcmTag(0x0018, 0x8151);

		/// <summary>(0018,9004) VR=CS Content Qualification</summary>
		public static DcmTag ContentQualification = new DcmTag(0x0018, 0x9004);

		/// <summary>(0018,9005) VR=SH Pulse Sequence Name</summary>
		public static DcmTag PulseSequenceName = new DcmTag(0x0018, 0x9005);

		/// <summary>(0018,9006) VR=SQ MR Imaging Modifier Sequence</summary>
		public static DcmTag MRImagingModifierSequence = new DcmTag(0x0018, 0x9006);

		/// <summary>(0018,9008) VR=CS Echo Pulse Sequence</summary>
		public static DcmTag EchoPulseSequence = new DcmTag(0x0018, 0x9008);

		/// <summary>(0018,9009) VR=CS Inversion Recovery</summary>
		public static DcmTag InversionRecovery = new DcmTag(0x0018, 0x9009);

		/// <summary>(0018,9010) VR=CS Flow Compensation</summary>
		public static DcmTag FlowCompensation = new DcmTag(0x0018, 0x9010);

		/// <summary>(0018,9011) VR=CS Multiple Spin Echo</summary>
		public static DcmTag MultipleSpinEcho = new DcmTag(0x0018, 0x9011);

		/// <summary>(0018,9012) VR=CS Multi-planar Excitation</summary>
		public static DcmTag MultiplanarExcitation = new DcmTag(0x0018, 0x9012);

		/// <summary>(0018,9014) VR=CS Phase Contrast</summary>
		public static DcmTag PhaseContrast = new DcmTag(0x0018, 0x9014);

		/// <summary>(0018,9015) VR=CS Time of Flight Contrast</summary>
		public static DcmTag TimeOfFlightContrast = new DcmTag(0x0018, 0x9015);

		/// <summary>(0018,9016) VR=CS Spoiling</summary>
		public static DcmTag Spoiling = new DcmTag(0x0018, 0x9016);

		/// <summary>(0018,9017) VR=CS Steady State Pulse Sequence</summary>
		public static DcmTag SteadyStatePulseSequence = new DcmTag(0x0018, 0x9017);

		/// <summary>(0018,9018) VR=CS Echo Planar Pulse Sequence</summary>
		public static DcmTag EchoPlanarPulseSequence = new DcmTag(0x0018, 0x9018);

		/// <summary>(0018,9019) VR=FD Tag Angle First Axis</summary>
		public static DcmTag TagAngleFirstAxis = new DcmTag(0x0018, 0x9019);

		/// <summary>(0018,9020) VR=CS Magnetization Transfer</summary>
		public static DcmTag MagnetizationTransfer = new DcmTag(0x0018, 0x9020);

		/// <summary>(0018,9021) VR=CS T2 Preparation</summary>
		public static DcmTag T2Preparation = new DcmTag(0x0018, 0x9021);

		/// <summary>(0018,9022) VR=CS Blood Signal Nulling</summary>
		public static DcmTag BloodSignalNulling = new DcmTag(0x0018, 0x9022);

		/// <summary>(0018,9024) VR=CS Saturation Recovery</summary>
		public static DcmTag SaturationRecovery = new DcmTag(0x0018, 0x9024);

		/// <summary>(0018,9025) VR=CS Spectrally Selected Suppression</summary>
		public static DcmTag SpectrallySelectedSuppression = new DcmTag(0x0018, 0x9025);

		/// <summary>(0018,9026) VR=CS Spectrally Selected Excitation</summary>
		public static DcmTag SpectrallySelectedExcitation = new DcmTag(0x0018, 0x9026);

		/// <summary>(0018,9027) VR=CS Spatial Pre-saturation</summary>
		public static DcmTag SpatialPresaturation = new DcmTag(0x0018, 0x9027);

		/// <summary>(0018,9028) VR=CS Tagging</summary>
		public static DcmTag Tagging = new DcmTag(0x0018, 0x9028);

		/// <summary>(0018,9029) VR=CS Oversampling Phase</summary>
		public static DcmTag OversamplingPhase = new DcmTag(0x0018, 0x9029);

		/// <summary>(0018,9030) VR=FD Tag Spacing First Dimension</summary>
		public static DcmTag TagSpacingFirstDimension = new DcmTag(0x0018, 0x9030);

		/// <summary>(0018,9032) VR=CS Geometry of k-Space Traversal</summary>
		public static DcmTag GeometryOfKSpaceTraversal = new DcmTag(0x0018, 0x9032);

		/// <summary>(0018,9033) VR=CS Segmented k-Space Traversal</summary>
		public static DcmTag SegmentedKSpaceTraversal = new DcmTag(0x0018, 0x9033);

		/// <summary>(0018,9034) VR=CS Rectilinear Phase Encode Reordering</summary>
		public static DcmTag RectilinearPhaseEncodeReordering = new DcmTag(0x0018, 0x9034);

		/// <summary>(0018,9035) VR=FD Tag Thickness</summary>
		public static DcmTag TagThickness = new DcmTag(0x0018, 0x9035);

		/// <summary>(0018,9036) VR=CS Partial Fourier Direction</summary>
		public static DcmTag PartialFourierDirection = new DcmTag(0x0018, 0x9036);

		/// <summary>(0018,9037) VR=CS Cardiac Synchronization Technique</summary>
		public static DcmTag CardiacSynchronizationTechnique = new DcmTag(0x0018, 0x9037);

		/// <summary>(0018,9041) VR=LO Receive Coil Manufacturer Name</summary>
		public static DcmTag ReceiveCoilManufacturerName = new DcmTag(0x0018, 0x9041);

		/// <summary>(0018,9042) VR=SQ MR Receive Coil Sequence</summary>
		public static DcmTag MRReceiveCoilSequence = new DcmTag(0x0018, 0x9042);

		/// <summary>(0018,9043) VR=CS Receive Coil Type</summary>
		public static DcmTag ReceiveCoilType = new DcmTag(0x0018, 0x9043);

		/// <summary>(0018,9044) VR=CS Quadrature Receive Coil</summary>
		public static DcmTag QuadratureReceiveCoil = new DcmTag(0x0018, 0x9044);

		/// <summary>(0018,9045) VR=SQ Multi-Coil Definition Sequence</summary>
		public static DcmTag MultiCoilDefinitionSequence = new DcmTag(0x0018, 0x9045);

		/// <summary>(0018,9046) VR=LO Multi-Coil Configuration</summary>
		public static DcmTag MultiCoilConfiguration = new DcmTag(0x0018, 0x9046);

		/// <summary>(0018,9047) VR=SH Multi-Coil Element Name</summary>
		public static DcmTag MultiCoilElementName = new DcmTag(0x0018, 0x9047);

		/// <summary>(0018,9048) VR=CS Multi-Coil Element Used</summary>
		public static DcmTag MultiCoilElementUsed = new DcmTag(0x0018, 0x9048);

		/// <summary>(0018,9049) VR=SQ MR Transmit Coil Sequence</summary>
		public static DcmTag MRTransmitCoilSequence = new DcmTag(0x0018, 0x9049);

		/// <summary>(0018,9050) VR=LO Transmit Coil Manufacturer Name</summary>
		public static DcmTag TransmitCoilManufacturerName = new DcmTag(0x0018, 0x9050);

		/// <summary>(0018,9051) VR=CS Transmit Coil Type</summary>
		public static DcmTag TransmitCoilType = new DcmTag(0x0018, 0x9051);

		/// <summary>(0018,9052) VR=FD Spectral Width</summary>
		public static DcmTag SpectralWidth = new DcmTag(0x0018, 0x9052);

		/// <summary>(0018,9053) VR=FD Chemical Shift Reference</summary>
		public static DcmTag ChemicalShiftReference = new DcmTag(0x0018, 0x9053);

		/// <summary>(0018,9054) VR=CS Volume Localization Technique</summary>
		public static DcmTag VolumeLocalizationTechnique = new DcmTag(0x0018, 0x9054);

		/// <summary>(0018,9058) VR=US MR Acquisition Frequency Encoding Steps</summary>
		public static DcmTag MRAcquisitionFrequencyEncodingSteps = new DcmTag(0x0018, 0x9058);

		/// <summary>(0018,9059) VR=CS De-coupling</summary>
		public static DcmTag Decoupling = new DcmTag(0x0018, 0x9059);

		/// <summary>(0018,9060) VR=CS De-coupled Nucleus</summary>
		public static DcmTag DecoupledNucleus = new DcmTag(0x0018, 0x9060);

		/// <summary>(0018,9061) VR=FD De-coupling Frequency</summary>
		public static DcmTag DecouplingFrequency = new DcmTag(0x0018, 0x9061);

		/// <summary>(0018,9062) VR=CS De-coupling Method</summary>
		public static DcmTag DecouplingMethod = new DcmTag(0x0018, 0x9062);

		/// <summary>(0018,9063) VR=FD De-coupling Chemical Shift Reference</summary>
		public static DcmTag DecouplingChemicalShiftReference = new DcmTag(0x0018, 0x9063);

		/// <summary>(0018,9064) VR=CS k-space Filtering</summary>
		public static DcmTag KspaceFiltering = new DcmTag(0x0018, 0x9064);

		/// <summary>(0018,9065) VR=CS Time Domain Filtering</summary>
		public static DcmTag TimeDomainFiltering = new DcmTag(0x0018, 0x9065);

		/// <summary>(0018,9066) VR=US Number of Zero fills</summary>
		public static DcmTag NumberOfZeroFills = new DcmTag(0x0018, 0x9066);

		/// <summary>(0018,9067) VR=CS Baseline Correction</summary>
		public static DcmTag BaselineCorrection = new DcmTag(0x0018, 0x9067);

		/// <summary>(0018,9069) VR=FD Parallel Reduction Factor In-plane</summary>
		public static DcmTag ParallelReductionFactorInplane = new DcmTag(0x0018, 0x9069);

		/// <summary>(0018,9070) VR=FD Cardiac R-R Interval Specified</summary>
		public static DcmTag CardiacRRIntervalSpecified = new DcmTag(0x0018, 0x9070);

		/// <summary>(0018,9073) VR=FD Acquisition Duration</summary>
		public static DcmTag AcquisitionDuration = new DcmTag(0x0018, 0x9073);

		/// <summary>(0018,9074) VR=DT Frame Acquisition Datetime</summary>
		public static DcmTag FrameAcquisitionDatetime = new DcmTag(0x0018, 0x9074);

		/// <summary>(0018,9075) VR=CS Diffusion Directionality</summary>
		public static DcmTag DiffusionDirectionality = new DcmTag(0x0018, 0x9075);

		/// <summary>(0018,9076) VR=SQ Diffusion Gradient Direction Sequence</summary>
		public static DcmTag DiffusionGradientDirectionSequence = new DcmTag(0x0018, 0x9076);

		/// <summary>(0018,9077) VR=CS Parallel Acquisition</summary>
		public static DcmTag ParallelAcquisition = new DcmTag(0x0018, 0x9077);

		/// <summary>(0018,9078) VR=CS Parallel Acquisition Technique</summary>
		public static DcmTag ParallelAcquisitionTechnique = new DcmTag(0x0018, 0x9078);

		/// <summary>(0018,9079) VR=FD Inversion Times</summary>
		public static DcmTag InversionTimes = new DcmTag(0x0018, 0x9079);

		/// <summary>(0018,9080) VR=ST Metabolite Map Description</summary>
		public static DcmTag MetaboliteMapDescription = new DcmTag(0x0018, 0x9080);

		/// <summary>(0018,9081) VR=CS Partial Fourier</summary>
		public static DcmTag PartialFourier = new DcmTag(0x0018, 0x9081);

		/// <summary>(0018,9082) VR=FD Effective Echo Time</summary>
		public static DcmTag EffectiveEchoTime = new DcmTag(0x0018, 0x9082);

		/// <summary>(0018,9083) VR=SQ Metabolite Map Code Sequence</summary>
		public static DcmTag MetaboliteMapCodeSequence = new DcmTag(0x0018, 0x9083);

		/// <summary>(0018,9084) VR=SQ Chemical Shift Sequence</summary>
		public static DcmTag ChemicalShiftSequence = new DcmTag(0x0018, 0x9084);

		/// <summary>(0018,9085) VR=CS Cardiac Signal Source</summary>
		public static DcmTag CardiacSignalSource = new DcmTag(0x0018, 0x9085);

		/// <summary>(0018,9087) VR=FD Diffusion b-value</summary>
		public static DcmTag DiffusionBvalue = new DcmTag(0x0018, 0x9087);

		/// <summary>(0018,9089) VR=FD Diffusion Gradient Orientation</summary>
		public static DcmTag DiffusionGradientOrientation = new DcmTag(0x0018, 0x9089);

		/// <summary>(0018,9090) VR=FD Velocity Encoding Direction</summary>
		public static DcmTag VelocityEncodingDirection = new DcmTag(0x0018, 0x9090);

		/// <summary>(0018,9091) VR=FD Velocity Encoding Minimum Value</summary>
		public static DcmTag VelocityEncodingMinimumValue = new DcmTag(0x0018, 0x9091);

		/// <summary>(0018,9093) VR=US Number of k-Space Trajectories</summary>
		public static DcmTag NumberOfKSpaceTrajectories = new DcmTag(0x0018, 0x9093);

		/// <summary>(0018,9094) VR=CS Coverage of k-Space</summary>
		public static DcmTag CoverageOfKSpace = new DcmTag(0x0018, 0x9094);

		/// <summary>(0018,9095) VR=UL Spectroscopy Acquisition Phase Rows</summary>
		public static DcmTag SpectroscopyAcquisitionPhaseRows = new DcmTag(0x0018, 0x9095);

		/// <summary>(0018,9098) VR=FD Transmitter Frequency</summary>
		public static DcmTag TransmitterFrequency = new DcmTag(0x0018, 0x9098);

		/// <summary>(0018,9100) VR=CS Resonant Nucleus</summary>
		public static DcmTag ResonantNucleus = new DcmTag(0x0018, 0x9100);

		/// <summary>(0018,9101) VR=CS Frequency Correction</summary>
		public static DcmTag FrequencyCorrection = new DcmTag(0x0018, 0x9101);

		/// <summary>(0018,9103) VR=SQ MR Spectroscopy FOV/Geometry Sequence</summary>
		public static DcmTag MRSpectroscopyFOVGeometrySequence = new DcmTag(0x0018, 0x9103);

		/// <summary>(0018,9104) VR=FD Slab Thickness</summary>
		public static DcmTag SlabThickness = new DcmTag(0x0018, 0x9104);

		/// <summary>(0018,9105) VR=FD Slab Orientation</summary>
		public static DcmTag SlabOrientation = new DcmTag(0x0018, 0x9105);

		/// <summary>(0018,9106) VR=FD Mid Slab Position</summary>
		public static DcmTag MidSlabPosition = new DcmTag(0x0018, 0x9106);

		/// <summary>(0018,9107) VR=SQ MR Spatial Saturation Sequence</summary>
		public static DcmTag MRSpatialSaturationSequence = new DcmTag(0x0018, 0x9107);

		/// <summary>(0018,9112) VR=SQ MR Timing and Related Parameters Sequence</summary>
		public static DcmTag MRTimingAndRelatedParametersSequence = new DcmTag(0x0018, 0x9112);

		/// <summary>(0018,9114) VR=SQ MR Echo Sequence</summary>
		public static DcmTag MREchoSequence = new DcmTag(0x0018, 0x9114);

		/// <summary>(0018,9115) VR=SQ MR Modifier Sequence</summary>
		public static DcmTag MRModifierSequence = new DcmTag(0x0018, 0x9115);

		/// <summary>(0018,9117) VR=SQ MR Diffusion Sequence</summary>
		public static DcmTag MRDiffusionSequence = new DcmTag(0x0018, 0x9117);

		/// <summary>(0018,9118) VR=SQ Cardiac Trigger Sequence</summary>
		public static DcmTag CardiacTriggerSequence = new DcmTag(0x0018, 0x9118);

		/// <summary>(0018,9119) VR=SQ MR Averages Sequence</summary>
		public static DcmTag MRAveragesSequence = new DcmTag(0x0018, 0x9119);

		/// <summary>(0018,9125) VR=SQ MR FOV/Geometry Sequence</summary>
		public static DcmTag MRFOVGeometrySequence = new DcmTag(0x0018, 0x9125);

		/// <summary>(0018,9126) VR=SQ Volume Localization Sequence</summary>
		public static DcmTag VolumeLocalizationSequence = new DcmTag(0x0018, 0x9126);

		/// <summary>(0018,9127) VR=UL Spectroscopy Acquisition Data Columns</summary>
		public static DcmTag SpectroscopyAcquisitionDataColumns = new DcmTag(0x0018, 0x9127);

		/// <summary>(0018,9147) VR=CS Diffusion Anisotropy Type</summary>
		public static DcmTag DiffusionAnisotropyType = new DcmTag(0x0018, 0x9147);

		/// <summary>(0018,9151) VR=DT Frame Reference Datetime</summary>
		public static DcmTag FrameReferenceDatetime = new DcmTag(0x0018, 0x9151);

		/// <summary>(0018,9152) VR=SQ MR Metabolite Map Sequence</summary>
		public static DcmTag MRMetaboliteMapSequence = new DcmTag(0x0018, 0x9152);

		/// <summary>(0018,9155) VR=FD Parallel Reduction Factor out-of-plane</summary>
		public static DcmTag ParallelReductionFactorOutofplane = new DcmTag(0x0018, 0x9155);

		/// <summary>(0018,9159) VR=UL Spectroscopy Acquisition Out-of-plane Phase Steps</summary>
		public static DcmTag SpectroscopyAcquisitionOutofplanePhaseSteps = new DcmTag(0x0018, 0x9159);

		/// <summary>(0018,9166) VR=CS Bulk Motion Status</summary>
		public static DcmTag BulkMotionStatus = new DcmTag(0x0018, 0x9166);

		/// <summary>(0018,9168) VR=FD Parallel Reduction Factor Second In-plane</summary>
		public static DcmTag ParallelReductionFactorSecondInplane = new DcmTag(0x0018, 0x9168);

		/// <summary>(0018,9169) VR=CS Cardiac Beat Rejection Technique</summary>
		public static DcmTag CardiacBeatRejectionTechnique = new DcmTag(0x0018, 0x9169);

		/// <summary>(0018,9170) VR=CS Respiratory Motion Compensation Technique</summary>
		public static DcmTag RespiratoryMotionCompensationTechnique = new DcmTag(0x0018, 0x9170);

		/// <summary>(0018,9171) VR=CS Respiratory Signal Source</summary>
		public static DcmTag RespiratorySignalSource = new DcmTag(0x0018, 0x9171);

		/// <summary>(0018,9172) VR=CS Bulk Motion Compensation Technique</summary>
		public static DcmTag BulkMotionCompensationTechnique = new DcmTag(0x0018, 0x9172);

		/// <summary>(0018,9173) VR=CS Bulk Motion Signal Source</summary>
		public static DcmTag BulkMotionSignalSource = new DcmTag(0x0018, 0x9173);

		/// <summary>(0018,9174) VR=CS Applicable Safety Standard Agency</summary>
		public static DcmTag ApplicableSafetyStandardAgency = new DcmTag(0x0018, 0x9174);

		/// <summary>(0018,9175) VR=LO Applicable Safety Standard Description</summary>
		public static DcmTag ApplicableSafetyStandardDescription = new DcmTag(0x0018, 0x9175);

		/// <summary>(0018,9176) VR=SQ Operating Mode Sequence</summary>
		public static DcmTag OperatingModeSequence = new DcmTag(0x0018, 0x9176);

		/// <summary>(0018,9177) VR=CS Operating Mode Type</summary>
		public static DcmTag OperatingModeType = new DcmTag(0x0018, 0x9177);

		/// <summary>(0018,9178) VR=CS Operating Mode</summary>
		public static DcmTag OperatingMode = new DcmTag(0x0018, 0x9178);

		/// <summary>(0018,9179) VR=CS Specific Absorption Rate Definition</summary>
		public static DcmTag SpecificAbsorptionRateDefinition = new DcmTag(0x0018, 0x9179);

		/// <summary>(0018,9180) VR=CS Gradient Output Type</summary>
		public static DcmTag GradientOutputType = new DcmTag(0x0018, 0x9180);

		/// <summary>(0018,9181) VR=FD Specific Absorption Rate Value</summary>
		public static DcmTag SpecificAbsorptionRateValue = new DcmTag(0x0018, 0x9181);

		/// <summary>(0018,9182) VR=FD Gradient Output</summary>
		public static DcmTag GradientOutput = new DcmTag(0x0018, 0x9182);

		/// <summary>(0018,9183) VR=CS Flow Compensation Direction</summary>
		public static DcmTag FlowCompensationDirection = new DcmTag(0x0018, 0x9183);

		/// <summary>(0018,9184) VR=FD Tagging Delay</summary>
		public static DcmTag TaggingDelay = new DcmTag(0x0018, 0x9184);

		/// <summary>(0018,9185) VR=ST Respiratory Motion Compensation Technique Description</summary>
		public static DcmTag RespiratoryMotionCompensationTechniqueDescription = new DcmTag(0x0018, 0x9185);

		/// <summary>(0018,9186) VR=SH Respiratory Signal Source ID</summary>
		public static DcmTag RespiratorySignalSourceID = new DcmTag(0x0018, 0x9186);

		/// <summary>(0018,9195) VR=FD Chemical Shifts Minimum Integration Limit in Hz (Retired)</summary>
		public static DcmTag ChemicalShiftsMinimumIntegrationLimitInHzRETIRED = new DcmTag(0x0018, 0x9195);

		/// <summary>(0018,9196) VR=FD Chemical Shifts Maximum Integration Limit in Hz (Retired)</summary>
		public static DcmTag ChemicalShiftsMaximumIntegrationLimitInHzRETIRED = new DcmTag(0x0018, 0x9196);

		/// <summary>(0018,9197) VR=SQ MR Velocity Encoding Sequence</summary>
		public static DcmTag MRVelocityEncodingSequence = new DcmTag(0x0018, 0x9197);

		/// <summary>(0018,9198) VR=CS First Order Phase Correction</summary>
		public static DcmTag FirstOrderPhaseCorrection = new DcmTag(0x0018, 0x9198);

		/// <summary>(0018,9199) VR=CS Water Referenced Phase Correction</summary>
		public static DcmTag WaterReferencedPhaseCorrection = new DcmTag(0x0018, 0x9199);

		/// <summary>(0018,9200) VR=CS MR Spectroscopy Acquisition Type</summary>
		public static DcmTag MRSpectroscopyAcquisitionType = new DcmTag(0x0018, 0x9200);

		/// <summary>(0018,9214) VR=CS Respiratory Cycle Position</summary>
		public static DcmTag RespiratoryCyclePosition = new DcmTag(0x0018, 0x9214);

		/// <summary>(0018,9217) VR=FD Velocity Encoding Maximum Value</summary>
		public static DcmTag VelocityEncodingMaximumValue = new DcmTag(0x0018, 0x9217);

		/// <summary>(0018,9218) VR=FD Tag Spacing Second Dimension</summary>
		public static DcmTag TagSpacingSecondDimension = new DcmTag(0x0018, 0x9218);

		/// <summary>(0018,9219) VR=SS Tag Angle Second Axis</summary>
		public static DcmTag TagAngleSecondAxis = new DcmTag(0x0018, 0x9219);

		/// <summary>(0018,9220) VR=FD Frame Acquisition Duration</summary>
		public static DcmTag FrameAcquisitionDuration = new DcmTag(0x0018, 0x9220);

		/// <summary>(0018,9226) VR=SQ MR Image Frame Type Sequence</summary>
		public static DcmTag MRImageFrameTypeSequence = new DcmTag(0x0018, 0x9226);

		/// <summary>(0018,9227) VR=SQ MR Spectroscopy Frame Type Sequence</summary>
		public static DcmTag MRSpectroscopyFrameTypeSequence = new DcmTag(0x0018, 0x9227);

		/// <summary>(0018,9231) VR=US MR Acquisition Phase Encoding Steps in-plane</summary>
		public static DcmTag MRAcquisitionPhaseEncodingStepsInplane = new DcmTag(0x0018, 0x9231);

		/// <summary>(0018,9232) VR=US MR Acquisition Phase Encoding Steps out-of-plane</summary>
		public static DcmTag MRAcquisitionPhaseEncodingStepsOutofplane = new DcmTag(0x0018, 0x9232);

		/// <summary>(0018,9234) VR=UL Spectroscopy Acquisition Phase Columns</summary>
		public static DcmTag SpectroscopyAcquisitionPhaseColumns = new DcmTag(0x0018, 0x9234);

		/// <summary>(0018,9236) VR=CS Cardiac Cycle Position</summary>
		public static DcmTag CardiacCyclePosition = new DcmTag(0x0018, 0x9236);

		/// <summary>(0018,9239) VR=SQ Specific Absorption Rate Sequence</summary>
		public static DcmTag SpecificAbsorptionRateSequence = new DcmTag(0x0018, 0x9239);

		/// <summary>(0018,9240) VR=US RF Echo Train Length</summary>
		public static DcmTag RFEchoTrainLength = new DcmTag(0x0018, 0x9240);

		/// <summary>(0018,9241) VR=US Gradient Echo Train Length</summary>
		public static DcmTag GradientEchoTrainLength = new DcmTag(0x0018, 0x9241);

		/// <summary>(0018,9295) VR=FD Chemical Shifts Minimum Integration Limit in ppm</summary>
		public static DcmTag ChemicalShiftsMinimumIntegrationLimitInPpm = new DcmTag(0x0018, 0x9295);

		/// <summary>(0018,9296) VR=FD Chemical Shifts Maximum Integration Limit in ppm</summary>
		public static DcmTag ChemicalShiftsMaximumIntegrationLimitInPpm = new DcmTag(0x0018, 0x9296);

		/// <summary>(0018,9301) VR=SQ CT Acquisition Type Sequence</summary>
		public static DcmTag CTAcquisitionTypeSequence = new DcmTag(0x0018, 0x9301);

		/// <summary>(0018,9302) VR=CS Acquisition Type</summary>
		public static DcmTag AcquisitionType = new DcmTag(0x0018, 0x9302);

		/// <summary>(0018,9303) VR=FD Tube Angle</summary>
		public static DcmTag TubeAngle = new DcmTag(0x0018, 0x9303);

		/// <summary>(0018,9304) VR=SQ CT Acquisition Details Sequence</summary>
		public static DcmTag CTAcquisitionDetailsSequence = new DcmTag(0x0018, 0x9304);

		/// <summary>(0018,9305) VR=FD Revolution Time</summary>
		public static DcmTag RevolutionTime = new DcmTag(0x0018, 0x9305);

		/// <summary>(0018,9306) VR=FD Single Collimation Width</summary>
		public static DcmTag SingleCollimationWidth = new DcmTag(0x0018, 0x9306);

		/// <summary>(0018,9307) VR=FD Total Collimation Width</summary>
		public static DcmTag TotalCollimationWidth = new DcmTag(0x0018, 0x9307);

		/// <summary>(0018,9308) VR=SQ CT Table Dynamics Sequence</summary>
		public static DcmTag CTTableDynamicsSequence = new DcmTag(0x0018, 0x9308);

		/// <summary>(0018,9309) VR=FD Table Speed</summary>
		public static DcmTag TableSpeed = new DcmTag(0x0018, 0x9309);

		/// <summary>(0018,9310) VR=FD Table Feed per Rotation</summary>
		public static DcmTag TableFeedPerRotation = new DcmTag(0x0018, 0x9310);

		/// <summary>(0018,9311) VR=FD Spiral Pitch Factor</summary>
		public static DcmTag SpiralPitchFactor = new DcmTag(0x0018, 0x9311);

		/// <summary>(0018,9312) VR=SQ CT Geometry Sequence</summary>
		public static DcmTag CTGeometrySequence = new DcmTag(0x0018, 0x9312);

		/// <summary>(0018,9313) VR=FD Data Collection Center (Patient)</summary>
		public static DcmTag DataCollectionCenterPatient = new DcmTag(0x0018, 0x9313);

		/// <summary>(0018,9314) VR=SQ CT Reconstruction Sequence</summary>
		public static DcmTag CTReconstructionSequence = new DcmTag(0x0018, 0x9314);

		/// <summary>(0018,9315) VR=CS Reconstruction Algorithm</summary>
		public static DcmTag ReconstructionAlgorithm = new DcmTag(0x0018, 0x9315);

		/// <summary>(0018,9316) VR=CS Convolution Kernel Group</summary>
		public static DcmTag ConvolutionKernelGroup = new DcmTag(0x0018, 0x9316);

		/// <summary>(0018,9317) VR=FD Reconstruction Field of View</summary>
		public static DcmTag ReconstructionFieldOfView = new DcmTag(0x0018, 0x9317);

		/// <summary>(0018,9318) VR=FD Reconstruction Target Center (Patient)</summary>
		public static DcmTag ReconstructionTargetCenterPatient = new DcmTag(0x0018, 0x9318);

		/// <summary>(0018,9319) VR=FD Reconstruction Angle</summary>
		public static DcmTag ReconstructionAngle = new DcmTag(0x0018, 0x9319);

		/// <summary>(0018,9320) VR=SH Image Filter</summary>
		public static DcmTag ImageFilter = new DcmTag(0x0018, 0x9320);

		/// <summary>(0018,9321) VR=SQ CT Exposure Sequence</summary>
		public static DcmTag CTExposureSequence = new DcmTag(0x0018, 0x9321);

		/// <summary>(0018,9322) VR=FD Reconstruction Pixel Spacing</summary>
		public static DcmTag ReconstructionPixelSpacing = new DcmTag(0x0018, 0x9322);

		/// <summary>(0018,9323) VR=CS Exposure Modulation Type</summary>
		public static DcmTag ExposureModulationType = new DcmTag(0x0018, 0x9323);

		/// <summary>(0018,9324) VR=FD Estimated Dose Saving</summary>
		public static DcmTag EstimatedDoseSaving = new DcmTag(0x0018, 0x9324);

		/// <summary>(0018,9325) VR=SQ CT X-ray Details Sequence</summary>
		public static DcmTag CTXrayDetailsSequence = new DcmTag(0x0018, 0x9325);

		/// <summary>(0018,9326) VR=SQ CT Position Sequence</summary>
		public static DcmTag CTPositionSequence = new DcmTag(0x0018, 0x9326);

		/// <summary>(0018,9327) VR=FD Table Position</summary>
		public static DcmTag TablePosition = new DcmTag(0x0018, 0x9327);

		/// <summary>(0018,9328) VR=FD Exposure Time in ms</summary>
		public static DcmTag ExposureTimeInMs = new DcmTag(0x0018, 0x9328);

		/// <summary>(0018,9329) VR=SQ CT Image Frame Type Sequence</summary>
		public static DcmTag CTImageFrameTypeSequence = new DcmTag(0x0018, 0x9329);

		/// <summary>(0018,9330) VR=FD X-Ray Tube Current in mA</summary>
		public static DcmTag XRayTubeCurrentInMA = new DcmTag(0x0018, 0x9330);

		/// <summary>(0018,9332) VR=FD Exposure in mAs</summary>
		public static DcmTag ExposureInMAs = new DcmTag(0x0018, 0x9332);

		/// <summary>(0018,9333) VR=CS Constant Volume Flag</summary>
		public static DcmTag ConstantVolumeFlag = new DcmTag(0x0018, 0x9333);

		/// <summary>(0018,9334) VR=CS Fluoroscopy Flag</summary>
		public static DcmTag FluoroscopyFlag = new DcmTag(0x0018, 0x9334);

		/// <summary>(0018,9335) VR=FD Distance Source to Data Collection Center</summary>
		public static DcmTag DistanceSourceToDataCollectionCenter = new DcmTag(0x0018, 0x9335);

		/// <summary>(0018,9337) VR=US Contrast/Bolus Agent Number</summary>
		public static DcmTag ContrastBolusAgentNumber = new DcmTag(0x0018, 0x9337);

		/// <summary>(0018,9338) VR=SQ Contrast/Bolus Ingredient Code Sequence</summary>
		public static DcmTag ContrastBolusIngredientCodeSequence = new DcmTag(0x0018, 0x9338);

		/// <summary>(0018,9340) VR=SQ Contrast Administration Profile Sequence</summary>
		public static DcmTag ContrastAdministrationProfileSequence = new DcmTag(0x0018, 0x9340);

		/// <summary>(0018,9341) VR=SQ Contrast/Bolus Usage Sequence</summary>
		public static DcmTag ContrastBolusUsageSequence = new DcmTag(0x0018, 0x9341);

		/// <summary>(0018,9342) VR=CS Contrast/Bolus Agent Administered</summary>
		public static DcmTag ContrastBolusAgentAdministered = new DcmTag(0x0018, 0x9342);

		/// <summary>(0018,9343) VR=CS Contrast/Bolus Agent Detected</summary>
		public static DcmTag ContrastBolusAgentDetected = new DcmTag(0x0018, 0x9343);

		/// <summary>(0018,9344) VR=CS Contrast/Bolus Agent Phase</summary>
		public static DcmTag ContrastBolusAgentPhase = new DcmTag(0x0018, 0x9344);

		/// <summary>(0018,9345) VR=FD CTDIvol</summary>
		public static DcmTag CTDIvol = new DcmTag(0x0018, 0x9345);

		/// <summary>(0018,9401) VR=SQ Projection Pixel Calibration Sequence</summary>
		public static DcmTag ProjectionPixelCalibrationSequence = new DcmTag(0x0018, 0x9401);

		/// <summary>(0018,9402) VR=FL Distance Source to Isocenter</summary>
		public static DcmTag DistanceSourceToIsocenter = new DcmTag(0x0018, 0x9402);

		/// <summary>(0018,9403) VR=FL Distance Object to Table Top</summary>
		public static DcmTag DistanceObjectToTableTop = new DcmTag(0x0018, 0x9403);

		/// <summary>(0018,9404) VR=FL Object Pixel Spacing in Center of Beam</summary>
		public static DcmTag ObjectPixelSpacingInCenterOfBeam = new DcmTag(0x0018, 0x9404);

		/// <summary>(0018,9405) VR=SQ Positioner Position Sequence</summary>
		public static DcmTag PositionerPositionSequence = new DcmTag(0x0018, 0x9405);

		/// <summary>(0018,9406) VR=SQ Table Position Sequence</summary>
		public static DcmTag TablePositionSequence = new DcmTag(0x0018, 0x9406);

		/// <summary>(0018,9407) VR=SQ Collimator Shape Sequence</summary>
		public static DcmTag CollimatorShapeSequence = new DcmTag(0x0018, 0x9407);

		/// <summary>(0018,9412) VR=SQ XA/XRF Frame Characteristics Sequence</summary>
		public static DcmTag XAXRFFrameCharacteristicsSequence = new DcmTag(0x0018, 0x9412);

		/// <summary>(0018,9417) VR=SQ Frame Acquisition Sequence</summary>
		public static DcmTag FrameAcquisitionSequence = new DcmTag(0x0018, 0x9417);

		/// <summary>(0018,9420) VR=CS X-Ray Receptor Type</summary>
		public static DcmTag XRayReceptorType = new DcmTag(0x0018, 0x9420);

		/// <summary>(0018,9423) VR=LO Acquisition Protocol Name</summary>
		public static DcmTag AcquisitionProtocolName = new DcmTag(0x0018, 0x9423);

		/// <summary>(0018,9424) VR=LT Acquisition Protocol Description</summary>
		public static DcmTag AcquisitionProtocolDescription = new DcmTag(0x0018, 0x9424);

		/// <summary>(0018,9425) VR=CS Contrast/Bolus Ingredient Opaque</summary>
		public static DcmTag ContrastBolusIngredientOpaque = new DcmTag(0x0018, 0x9425);

		/// <summary>(0018,9426) VR=FL Distance Receptor Plane to Detector Housing</summary>
		public static DcmTag DistanceReceptorPlaneToDetectorHousing = new DcmTag(0x0018, 0x9426);

		/// <summary>(0018,9427) VR=CS Intensifier Active Shape</summary>
		public static DcmTag IntensifierActiveShape = new DcmTag(0x0018, 0x9427);

		/// <summary>(0018,9428) VR=FL Intensifier Active Dimension(s)</summary>
		public static DcmTag IntensifierActiveDimensions = new DcmTag(0x0018, 0x9428);

		/// <summary>(0018,9429) VR=FL Physical Detector Size</summary>
		public static DcmTag PhysicalDetectorSize = new DcmTag(0x0018, 0x9429);

		/// <summary>(0018,9430) VR=US Position of Isocenter Projection</summary>
		public static DcmTag PositionOfIsocenterProjection = new DcmTag(0x0018, 0x9430);

		/// <summary>(0018,9432) VR=SQ Field of View Sequence</summary>
		public static DcmTag FieldOfViewSequence = new DcmTag(0x0018, 0x9432);

		/// <summary>(0018,9433) VR=LO Field of View Description</summary>
		public static DcmTag FieldOfViewDescription = new DcmTag(0x0018, 0x9433);

		/// <summary>(0018,9434) VR=SQ Exposure Control Sensing Regions Sequence</summary>
		public static DcmTag ExposureControlSensingRegionsSequence = new DcmTag(0x0018, 0x9434);

		/// <summary>(0018,9435) VR=CS Exposure Control Sensing Region Shape</summary>
		public static DcmTag ExposureControlSensingRegionShape = new DcmTag(0x0018, 0x9435);

		/// <summary>(0018,9436) VR=SS Exposure Control Sensing Region Left Vertical Edge</summary>
		public static DcmTag ExposureControlSensingRegionLeftVerticalEdge = new DcmTag(0x0018, 0x9436);

		/// <summary>(0018,9437) VR=SS Exposure Control Sensing Region Right Vertical Edge</summary>
		public static DcmTag ExposureControlSensingRegionRightVerticalEdge = new DcmTag(0x0018, 0x9437);

		/// <summary>(0018,9438) VR=SS Exposure Control Sensing Region Upper Horizontal Edge</summary>
		public static DcmTag ExposureControlSensingRegionUpperHorizontalEdge = new DcmTag(0x0018, 0x9438);

		/// <summary>(0018,9439) VR=SS Exposure Control Sensing Region Lower Horizontal Edge</summary>
		public static DcmTag ExposureControlSensingRegionLowerHorizontalEdge = new DcmTag(0x0018, 0x9439);

		/// <summary>(0018,9440) VR=SS Center of Circular Exposure Control Sensing Region</summary>
		public static DcmTag CenterOfCircularExposureControlSensingRegion = new DcmTag(0x0018, 0x9440);

		/// <summary>(0018,9441) VR=US Radius of Circular Exposure Control Sensing Region</summary>
		public static DcmTag RadiusOfCircularExposureControlSensingRegion = new DcmTag(0x0018, 0x9441);

		/// <summary>(0018,9442) VR=SS Vertices of the Polygonal Exposure Control Sensing Region</summary>
		public static DcmTag VerticesOfThePolygonalExposureControlSensingRegion = new DcmTag(0x0018, 0x9442);

		/// <summary>(0018,9445) VR= SHALL NOT BE USED (Retired)</summary>
		public static DcmTag SHALLNOTBEUSEDRETIRED = new DcmTag(0x0018, 0x9445);

		/// <summary>(0018,9447) VR=FL Column Angulation (Patient)</summary>
		public static DcmTag ColumnAngulationPatient = new DcmTag(0x0018, 0x9447);

		/// <summary>(0018,9449) VR=FL Beam Angle</summary>
		public static DcmTag BeamAngle = new DcmTag(0x0018, 0x9449);

		/// <summary>(0018,9451) VR=SQ Frame Detector Parameters Sequence</summary>
		public static DcmTag FrameDetectorParametersSequence = new DcmTag(0x0018, 0x9451);

		/// <summary>(0018,9452) VR=FL Calculated Anatomy Thickness</summary>
		public static DcmTag CalculatedAnatomyThickness = new DcmTag(0x0018, 0x9452);

		/// <summary>(0018,9455) VR=SQ Calibration Sequence</summary>
		public static DcmTag CalibrationSequence = new DcmTag(0x0018, 0x9455);

		/// <summary>(0018,9456) VR=SQ Object Thickness Sequence</summary>
		public static DcmTag ObjectThicknessSequence = new DcmTag(0x0018, 0x9456);

		/// <summary>(0018,9457) VR=CS Plane Identification</summary>
		public static DcmTag PlaneIdentification = new DcmTag(0x0018, 0x9457);

		/// <summary>(0018,9461) VR=FL Field of View Dimension(s) in Float</summary>
		public static DcmTag FieldOfViewDimensionsInFloat = new DcmTag(0x0018, 0x9461);

		/// <summary>(0018,9462) VR=SQ Isocenter Reference System Sequence</summary>
		public static DcmTag IsocenterReferenceSystemSequence = new DcmTag(0x0018, 0x9462);

		/// <summary>(0018,9463) VR=FL Positioner Isocenter Primary Angle</summary>
		public static DcmTag PositionerIsocenterPrimaryAngle = new DcmTag(0x0018, 0x9463);

		/// <summary>(0018,9464) VR=FL Positioner Isocenter Secondary Angle</summary>
		public static DcmTag PositionerIsocenterSecondaryAngle = new DcmTag(0x0018, 0x9464);

		/// <summary>(0018,9465) VR=FL Positioner Isocenter Detector Rotation Angle</summary>
		public static DcmTag PositionerIsocenterDetectorRotationAngle = new DcmTag(0x0018, 0x9465);

		/// <summary>(0018,9466) VR=FL Table X Position to Isocenter</summary>
		public static DcmTag TableXPositionToIsocenter = new DcmTag(0x0018, 0x9466);

		/// <summary>(0018,9467) VR=FL Table Y Position to Isocenter</summary>
		public static DcmTag TableYPositionToIsocenter = new DcmTag(0x0018, 0x9467);

		/// <summary>(0018,9468) VR=FL Table Z Position to Isocenter</summary>
		public static DcmTag TableZPositionToIsocenter = new DcmTag(0x0018, 0x9468);

		/// <summary>(0018,9469) VR=FL Table Horizontal Rotation Angle</summary>
		public static DcmTag TableHorizontalRotationAngle = new DcmTag(0x0018, 0x9469);

		/// <summary>(0018,9470) VR=FL Table Head Tilt Angle</summary>
		public static DcmTag TableHeadTiltAngle = new DcmTag(0x0018, 0x9470);

		/// <summary>(0018,9471) VR=FL Table Cradle Tilt Angle</summary>
		public static DcmTag TableCradleTiltAngle = new DcmTag(0x0018, 0x9471);

		/// <summary>(0018,9472) VR=SQ Frame Display Shutter Sequence</summary>
		public static DcmTag FrameDisplayShutterSequence = new DcmTag(0x0018, 0x9472);

		/// <summary>(0018,9473) VR=FL Acquired Image Area Dose Product</summary>
		public static DcmTag AcquiredImageAreaDoseProduct = new DcmTag(0x0018, 0x9473);

		/// <summary>(0018,9474) VR=CS C-arm Positioner Tabletop Relationship</summary>
		public static DcmTag CarmPositionerTabletopRelationship = new DcmTag(0x0018, 0x9474);

		/// <summary>(0018,9476) VR=SQ X-Ray Geometry Sequence</summary>
		public static DcmTag XRayGeometrySequence = new DcmTag(0x0018, 0x9476);

		/// <summary>(0018,9477) VR=SQ Irradiation Event Identification Sequence</summary>
		public static DcmTag IrradiationEventIdentificationSequence = new DcmTag(0x0018, 0x9477);

		/// <summary>(0018,a001) VR=SQ Contributing Equipment Sequence</summary>
		public static DcmTag ContributingEquipmentSequence = new DcmTag(0x0018, 0xa001);

		/// <summary>(0018,a002) VR=DT Contribution Date Time</summary>
		public static DcmTag ContributionDateTime = new DcmTag(0x0018, 0xa002);

		/// <summary>(0018,a003) VR=ST Contribution Description</summary>
		public static DcmTag ContributionDescription = new DcmTag(0x0018, 0xa003);

		/// <summary>(0020,000d) VR=UI Study Instance UID</summary>
		public static DcmTag StudyInstanceUID = new DcmTag(0x0020, 0x000d);

		/// <summary>(0020,000e) VR=UI Series Instance UID</summary>
		public static DcmTag SeriesInstanceUID = new DcmTag(0x0020, 0x000e);

		/// <summary>(0020,0010) VR=SH Study ID</summary>
		public static DcmTag StudyID = new DcmTag(0x0020, 0x0010);

		/// <summary>(0020,0011) VR=IS Series Number</summary>
		public static DcmTag SeriesNumber = new DcmTag(0x0020, 0x0011);

		/// <summary>(0020,0012) VR=IS Acquisition Number</summary>
		public static DcmTag AcquisitionNumber = new DcmTag(0x0020, 0x0012);

		/// <summary>(0020,0013) VR=IS Instance Number</summary>
		public static DcmTag InstanceNumber = new DcmTag(0x0020, 0x0013);

		/// <summary>(0020,0014) VR=IS Isotope Number (Retired)</summary>
		public static DcmTag IsotopeNumberRETIRED = new DcmTag(0x0020, 0x0014);

		/// <summary>(0020,0015) VR=IS Phase Number (Retired)</summary>
		public static DcmTag PhaseNumberRETIRED = new DcmTag(0x0020, 0x0015);

		/// <summary>(0020,0016) VR=IS Interval Number (Retired)</summary>
		public static DcmTag IntervalNumberRETIRED = new DcmTag(0x0020, 0x0016);

		/// <summary>(0020,0017) VR=IS Time Slot Number (Retired)</summary>
		public static DcmTag TimeSlotNumberRETIRED = new DcmTag(0x0020, 0x0017);

		/// <summary>(0020,0018) VR=IS Angle Number (Retired)</summary>
		public static DcmTag AngleNumberRETIRED = new DcmTag(0x0020, 0x0018);

		/// <summary>(0020,0019) VR=IS Item Number</summary>
		public static DcmTag ItemNumber = new DcmTag(0x0020, 0x0019);

		/// <summary>(0020,0020) VR=CS Patient Orientation</summary>
		public static DcmTag PatientOrientation = new DcmTag(0x0020, 0x0020);

		/// <summary>(0020,0022) VR=IS Overlay Number (Retired)</summary>
		public static DcmTag OverlayNumberRETIRED = new DcmTag(0x0020, 0x0022);

		/// <summary>(0020,0024) VR=IS Curve Number (Retired)</summary>
		public static DcmTag CurveNumberRETIRED = new DcmTag(0x0020, 0x0024);

		/// <summary>(0020,0026) VR=IS Lookup Table Number (Retired)</summary>
		public static DcmTag LookupTableNumberRETIRED = new DcmTag(0x0020, 0x0026);

		/// <summary>(0020,0030) VR=DS Image Position (Retired)</summary>
		public static DcmTag ImagePositionRETIRED = new DcmTag(0x0020, 0x0030);

		/// <summary>(0020,0032) VR=DS Image Position (Patient)</summary>
		public static DcmTag ImagePositionPatient = new DcmTag(0x0020, 0x0032);

		/// <summary>(0020,0035) VR=DS Image Orientation (Retired)</summary>
		public static DcmTag ImageOrientationRETIRED = new DcmTag(0x0020, 0x0035);

		/// <summary>(0020,0037) VR=DS Image Orientation (Patient)</summary>
		public static DcmTag ImageOrientationPatient = new DcmTag(0x0020, 0x0037);

		/// <summary>(0020,0050) VR=DS Location (Retired)</summary>
		public static DcmTag LocationRETIRED = new DcmTag(0x0020, 0x0050);

		/// <summary>(0020,0052) VR=UI Frame of Reference UID</summary>
		public static DcmTag FrameOfReferenceUID = new DcmTag(0x0020, 0x0052);

		/// <summary>(0020,0060) VR=CS Laterality</summary>
		public static DcmTag Laterality = new DcmTag(0x0020, 0x0060);

		/// <summary>(0020,0062) VR=CS Image Laterality</summary>
		public static DcmTag ImageLaterality = new DcmTag(0x0020, 0x0062);

		/// <summary>(0020,0070) VR=LO Image Geometry Type (Retired)</summary>
		public static DcmTag ImageGeometryTypeRETIRED = new DcmTag(0x0020, 0x0070);

		/// <summary>(0020,0080) VR=CS Masking Image (Retired)</summary>
		public static DcmTag MaskingImageRETIRED = new DcmTag(0x0020, 0x0080);

		/// <summary>(0020,0100) VR=IS Temporal Position Identifier</summary>
		public static DcmTag TemporalPositionIdentifier = new DcmTag(0x0020, 0x0100);

		/// <summary>(0020,0105) VR=IS Number of Temporal Positions</summary>
		public static DcmTag NumberOfTemporalPositions = new DcmTag(0x0020, 0x0105);

		/// <summary>(0020,0110) VR=DS Temporal Resolution</summary>
		public static DcmTag TemporalResolution = new DcmTag(0x0020, 0x0110);

		/// <summary>(0020,0200) VR=UI Synchronization Frame of Reference UID</summary>
		public static DcmTag SynchronizationFrameOfReferenceUID = new DcmTag(0x0020, 0x0200);

		/// <summary>(0020,1000) VR=IS Series in Study (Retired)</summary>
		public static DcmTag SeriesInStudyRETIRED = new DcmTag(0x0020, 0x1000);

		/// <summary>(0020,1001) VR=IS Acquisitions in Series (Retired)</summary>
		public static DcmTag AcquisitionsInSeriesRETIRED = new DcmTag(0x0020, 0x1001);

		/// <summary>(0020,1002) VR=IS Images in Acquisition</summary>
		public static DcmTag ImagesInAcquisition = new DcmTag(0x0020, 0x1002);

		/// <summary>(0020,1003) VR=IS Images in Series (Retired)</summary>
		public static DcmTag ImagesInSeriesRETIRED = new DcmTag(0x0020, 0x1003);

		/// <summary>(0020,1004) VR=IS Acquisitions in Study (Retired)</summary>
		public static DcmTag AcquisitionsInStudyRETIRED = new DcmTag(0x0020, 0x1004);

		/// <summary>(0020,1005) VR=IS Images in Study (Retired)</summary>
		public static DcmTag ImagesInStudyRETIRED = new DcmTag(0x0020, 0x1005);

		/// <summary>(0020,1020) VR=CS Reference (Retired)</summary>
		public static DcmTag ReferenceRETIRED = new DcmTag(0x0020, 0x1020);

		/// <summary>(0020,1040) VR=LO Position Reference Indicator</summary>
		public static DcmTag PositionReferenceIndicator = new DcmTag(0x0020, 0x1040);

		/// <summary>(0020,1041) VR=DS Slice Location</summary>
		public static DcmTag SliceLocation = new DcmTag(0x0020, 0x1041);

		/// <summary>(0020,1070) VR=IS Other Study Numbers (Retired)</summary>
		public static DcmTag OtherStudyNumbersRETIRED = new DcmTag(0x0020, 0x1070);

		/// <summary>(0020,1200) VR=IS Number of Patient Related Studies</summary>
		public static DcmTag NumberOfPatientRelatedStudies = new DcmTag(0x0020, 0x1200);

		/// <summary>(0020,1202) VR=IS Number of Patient Related Series</summary>
		public static DcmTag NumberOfPatientRelatedSeries = new DcmTag(0x0020, 0x1202);

		/// <summary>(0020,1204) VR=IS Number of Patient Related Instances</summary>
		public static DcmTag NumberOfPatientRelatedInstances = new DcmTag(0x0020, 0x1204);

		/// <summary>(0020,1206) VR=IS Number of Study Related Series</summary>
		public static DcmTag NumberOfStudyRelatedSeries = new DcmTag(0x0020, 0x1206);

		/// <summary>(0020,1208) VR=IS Number of Study Related Instances</summary>
		public static DcmTag NumberOfStudyRelatedInstances = new DcmTag(0x0020, 0x1208);

		/// <summary>(0020,1209) VR=IS Number of Series Related Instances</summary>
		public static DcmTag NumberOfSeriesRelatedInstances = new DcmTag(0x0020, 0x1209);

		/// <summary>(0020,31xx) VR=CS Source Image IDs (Retired)</summary>
		public static DcmTag SourceImageIDsRETIRED = new DcmTag(0x0020, 0x3100);

		/// <summary>(0020,3401) VR=CS Modifying Device ID (Retired)</summary>
		public static DcmTag ModifyingDeviceIDRETIRED = new DcmTag(0x0020, 0x3401);

		/// <summary>(0020,3402) VR=CS Modified Image ID (Retired)</summary>
		public static DcmTag ModifiedImageIDRETIRED = new DcmTag(0x0020, 0x3402);

		/// <summary>(0020,3403) VR=DA Modified Image Date (Retired)</summary>
		public static DcmTag ModifiedImageDateRETIRED = new DcmTag(0x0020, 0x3403);

		/// <summary>(0020,3404) VR=LO Modifying Device Manufacturer (Retired)</summary>
		public static DcmTag ModifyingDeviceManufacturerRETIRED = new DcmTag(0x0020, 0x3404);

		/// <summary>(0020,3405) VR=TM Modified Image Time (Retired)</summary>
		public static DcmTag ModifiedImageTimeRETIRED = new DcmTag(0x0020, 0x3405);

		/// <summary>(0020,3406) VR=LO Modified Image Description (Retired)</summary>
		public static DcmTag ModifiedImageDescriptionRETIRED = new DcmTag(0x0020, 0x3406);

		/// <summary>(0020,4000) VR=LT Image Comments</summary>
		public static DcmTag ImageComments = new DcmTag(0x0020, 0x4000);

		/// <summary>(0020,5000) VR=AT Original Image Identification (Retired)</summary>
		public static DcmTag OriginalImageIdentificationRETIRED = new DcmTag(0x0020, 0x5000);

		/// <summary>(0020,5002) VR=CS Original Image Identification Nomenclature (Retired)</summary>
		public static DcmTag OriginalImageIdentificationNomenclatureRETIRED = new DcmTag(0x0020, 0x5002);

		/// <summary>(0020,9056) VR=SH Stack ID</summary>
		public static DcmTag StackID = new DcmTag(0x0020, 0x9056);

		/// <summary>(0020,9057) VR=UL In-Stack Position Number</summary>
		public static DcmTag InStackPositionNumber = new DcmTag(0x0020, 0x9057);

		/// <summary>(0020,9071) VR=SQ Frame Anatomy Sequence</summary>
		public static DcmTag FrameAnatomySequence = new DcmTag(0x0020, 0x9071);

		/// <summary>(0020,9072) VR=CS Frame Laterality</summary>
		public static DcmTag FrameLaterality = new DcmTag(0x0020, 0x9072);

		/// <summary>(0020,9111) VR=SQ Frame Content Sequence</summary>
		public static DcmTag FrameContentSequence = new DcmTag(0x0020, 0x9111);

		/// <summary>(0020,9113) VR=SQ Plane Position Sequence</summary>
		public static DcmTag PlanePositionSequence = new DcmTag(0x0020, 0x9113);

		/// <summary>(0020,9116) VR=SQ Plane Orientation Sequence</summary>
		public static DcmTag PlaneOrientationSequence = new DcmTag(0x0020, 0x9116);

		/// <summary>(0020,9128) VR=UL Temporal Position Index</summary>
		public static DcmTag TemporalPositionIndex = new DcmTag(0x0020, 0x9128);

		/// <summary>(0020,9153) VR=FD Cardiac Trigger Delay Time</summary>
		public static DcmTag CardiacTriggerDelayTime = new DcmTag(0x0020, 0x9153);

		/// <summary>(0020,9156) VR=US Frame Acquisition Number</summary>
		public static DcmTag FrameAcquisitionNumber = new DcmTag(0x0020, 0x9156);

		/// <summary>(0020,9157) VR=UL Dimension Index Values</summary>
		public static DcmTag DimensionIndexValues = new DcmTag(0x0020, 0x9157);

		/// <summary>(0020,9158) VR=LT Frame Comments</summary>
		public static DcmTag FrameComments = new DcmTag(0x0020, 0x9158);

		/// <summary>(0020,9161) VR=UI Concatenation UID</summary>
		public static DcmTag ConcatenationUID = new DcmTag(0x0020, 0x9161);

		/// <summary>(0020,9162) VR=US In-concatenation Number</summary>
		public static DcmTag InconcatenationNumber = new DcmTag(0x0020, 0x9162);

		/// <summary>(0020,9163) VR=US In-concatenation Total Number</summary>
		public static DcmTag InconcatenationTotalNumber = new DcmTag(0x0020, 0x9163);

		/// <summary>(0020,9164) VR=UI Dimension Organization UID</summary>
		public static DcmTag DimensionOrganizationUID = new DcmTag(0x0020, 0x9164);

		/// <summary>(0020,9165) VR=AT Dimension Index Pointer</summary>
		public static DcmTag DimensionIndexPointer = new DcmTag(0x0020, 0x9165);

		/// <summary>(0020,9167) VR=AT Functional Group Pointer</summary>
		public static DcmTag FunctionalGroupPointer = new DcmTag(0x0020, 0x9167);

		/// <summary>(0020,9213) VR=LO Dimension Index Private Creator</summary>
		public static DcmTag DimensionIndexPrivateCreator = new DcmTag(0x0020, 0x9213);

		/// <summary>(0020,9221) VR=SQ Dimension Organization Sequence</summary>
		public static DcmTag DimensionOrganizationSequence = new DcmTag(0x0020, 0x9221);

		/// <summary>(0020,9222) VR=SQ Dimension Index Sequence</summary>
		public static DcmTag DimensionIndexSequence = new DcmTag(0x0020, 0x9222);

		/// <summary>(0020,9228) VR=UL Concatenation Frame Offset Number</summary>
		public static DcmTag ConcatenationFrameOffsetNumber = new DcmTag(0x0020, 0x9228);

		/// <summary>(0020,9238) VR=LO Functional Group Private Creator</summary>
		public static DcmTag FunctionalGroupPrivateCreator = new DcmTag(0x0020, 0x9238);

		/// <summary>(0020,9251) VR=FD R-R Interval Time Measured</summary>
		public static DcmTag RRIntervalTimeMeasured = new DcmTag(0x0020, 0x9251);

		/// <summary>(0020,9253) VR=SQ Respiratory Trigger Sequence</summary>
		public static DcmTag RespiratoryTriggerSequence = new DcmTag(0x0020, 0x9253);

		/// <summary>(0020,9254) VR=FD Respiratory Interval Time</summary>
		public static DcmTag RespiratoryIntervalTime = new DcmTag(0x0020, 0x9254);

		/// <summary>(0020,9255) VR=FD Respiratory Trigger Delay Time</summary>
		public static DcmTag RespiratoryTriggerDelayTime = new DcmTag(0x0020, 0x9255);

		/// <summary>(0020,9256) VR=FD Respiratory Trigger Delay Threshold</summary>
		public static DcmTag RespiratoryTriggerDelayThreshold = new DcmTag(0x0020, 0x9256);

		/// <summary>(0020,9421) VR=LO Dimension Description Label</summary>
		public static DcmTag DimensionDescriptionLabel = new DcmTag(0x0020, 0x9421);

		/// <summary>(0020,9450) VR=SQ Patient Orientation in Frame Sequence</summary>
		public static DcmTag PatientOrientationInFrameSequence = new DcmTag(0x0020, 0x9450);

		/// <summary>(0020,9453) VR=LO Frame Label</summary>
		public static DcmTag FrameLabel = new DcmTag(0x0020, 0x9453);

		/// <summary>(0022,0001) VR=US Light Path Filter Pass-Through Wavelength</summary>
		public static DcmTag LightPathFilterPassThroughWavelength = new DcmTag(0x0022, 0x0001);

		/// <summary>(0022,0002) VR=US Light Path Filter Pass Band</summary>
		public static DcmTag LightPathFilterPassBand = new DcmTag(0x0022, 0x0002);

		/// <summary>(0022,0003) VR=US Image Path Filter Pass-Through Wavelength</summary>
		public static DcmTag ImagePathFilterPassThroughWavelength = new DcmTag(0x0022, 0x0003);

		/// <summary>(0022,0004) VR=US Image Path Filter Pass Band</summary>
		public static DcmTag ImagePathFilterPassBand = new DcmTag(0x0022, 0x0004);

		/// <summary>(0022,0005) VR=CS Patient Eye Movement Commanded</summary>
		public static DcmTag PatientEyeMovementCommanded = new DcmTag(0x0022, 0x0005);

		/// <summary>(0022,0006) VR=SQ Patient Eye Movement Command Code Sequence</summary>
		public static DcmTag PatientEyeMovementCommandCodeSequence = new DcmTag(0x0022, 0x0006);

		/// <summary>(0022,0007) VR=FL Spherical Lens Power</summary>
		public static DcmTag SphericalLensPower = new DcmTag(0x0022, 0x0007);

		/// <summary>(0022,0008) VR=FL Cylinder Lens Power</summary>
		public static DcmTag CylinderLensPower = new DcmTag(0x0022, 0x0008);

		/// <summary>(0022,0009) VR=FL Cylinder Axis</summary>
		public static DcmTag CylinderAxis = new DcmTag(0x0022, 0x0009);

		/// <summary>(0022,000a) VR=FL Emmetropic Magnification</summary>
		public static DcmTag EmmetropicMagnification = new DcmTag(0x0022, 0x000a);

		/// <summary>(0022,000b) VR=FL Intra Ocular Pressure</summary>
		public static DcmTag IntraOcularPressure = new DcmTag(0x0022, 0x000b);

		/// <summary>(0022,000c) VR=FL Horizontal Field of View</summary>
		public static DcmTag HorizontalFieldOfView = new DcmTag(0x0022, 0x000c);

		/// <summary>(0022,000d) VR=CS Pupil Dilated</summary>
		public static DcmTag PupilDilated = new DcmTag(0x0022, 0x000d);

		/// <summary>(0022,000e) VR=FL Degree of Dilation</summary>
		public static DcmTag DegreeOfDilation = new DcmTag(0x0022, 0x000e);

		/// <summary>(0022,0010) VR=FL Stereo Baseline Angle</summary>
		public static DcmTag StereoBaselineAngle = new DcmTag(0x0022, 0x0010);

		/// <summary>(0022,0011) VR=FL Stereo Baseline Displacement</summary>
		public static DcmTag StereoBaselineDisplacement = new DcmTag(0x0022, 0x0011);

		/// <summary>(0022,0012) VR=FL Stereo Horizontal Pixel Offset</summary>
		public static DcmTag StereoHorizontalPixelOffset = new DcmTag(0x0022, 0x0012);

		/// <summary>(0022,0013) VR=FL Stereo Vertical Pixel Offset</summary>
		public static DcmTag StereoVerticalPixelOffset = new DcmTag(0x0022, 0x0013);

		/// <summary>(0022,0014) VR=FL Stereo Rotation</summary>
		public static DcmTag StereoRotation = new DcmTag(0x0022, 0x0014);

		/// <summary>(0022,0015) VR=SQ Acquisition Device Type Code Sequence</summary>
		public static DcmTag AcquisitionDeviceTypeCodeSequence = new DcmTag(0x0022, 0x0015);

		/// <summary>(0022,0016) VR=SQ Illumination Type Code Sequence</summary>
		public static DcmTag IlluminationTypeCodeSequence = new DcmTag(0x0022, 0x0016);

		/// <summary>(0022,0017) VR=SQ Light Path Filter Type Stack Code Sequence</summary>
		public static DcmTag LightPathFilterTypeStackCodeSequence = new DcmTag(0x0022, 0x0017);

		/// <summary>(0022,0018) VR=SQ Image Path Filter Type Stack Code Sequence</summary>
		public static DcmTag ImagePathFilterTypeStackCodeSequence = new DcmTag(0x0022, 0x0018);

		/// <summary>(0022,0019) VR=SQ Lenses Code Sequence</summary>
		public static DcmTag LensesCodeSequence = new DcmTag(0x0022, 0x0019);

		/// <summary>(0022,001a) VR=SQ Channel Description Code Sequence</summary>
		public static DcmTag ChannelDescriptionCodeSequence = new DcmTag(0x0022, 0x001a);

		/// <summary>(0022,001b) VR=SQ Refractive State Sequence</summary>
		public static DcmTag RefractiveStateSequence = new DcmTag(0x0022, 0x001b);

		/// <summary>(0022,001c) VR=SQ Mydriatic Agent Code Sequence</summary>
		public static DcmTag MydriaticAgentCodeSequence = new DcmTag(0x0022, 0x001c);

		/// <summary>(0022,001d) VR=SQ Relative Image Position Code Sequence</summary>
		public static DcmTag RelativeImagePositionCodeSequence = new DcmTag(0x0022, 0x001d);

		/// <summary>(0022,0020) VR=SQ Stereo Pairs Sequence</summary>
		public static DcmTag StereoPairsSequence = new DcmTag(0x0022, 0x0020);

		/// <summary>(0022,0021) VR=SQ Left Image Sequence</summary>
		public static DcmTag LeftImageSequence = new DcmTag(0x0022, 0x0021);

		/// <summary>(0022,0022) VR=SQ Right Image Sequence</summary>
		public static DcmTag RightImageSequence = new DcmTag(0x0022, 0x0022);

		/// <summary>(0028,0002) VR=US Samples per Pixel</summary>
		public static DcmTag SamplesPerPixel = new DcmTag(0x0028, 0x0002);

		/// <summary>(0028,0003) VR=US Samples per Pixel Used</summary>
		public static DcmTag SamplesPerPixelUsed = new DcmTag(0x0028, 0x0003);

		/// <summary>(0028,0004) VR=CS Photometric Interpretation</summary>
		public static DcmTag PhotometricInterpretation = new DcmTag(0x0028, 0x0004);

		/// <summary>(0028,0005) VR=US Image Dimensions (Retired)</summary>
		public static DcmTag ImageDimensionsRETIRED = new DcmTag(0x0028, 0x0005);

		/// <summary>(0028,0006) VR=US Planar Configuration</summary>
		public static DcmTag PlanarConfiguration = new DcmTag(0x0028, 0x0006);

		/// <summary>(0028,0008) VR=IS Number of Frames</summary>
		public static DcmTag NumberOfFrames = new DcmTag(0x0028, 0x0008);

		/// <summary>(0028,0009) VR=AT Frame Increment Pointer</summary>
		public static DcmTag FrameIncrementPointer = new DcmTag(0x0028, 0x0009);

		/// <summary>(0028,000a) VR=AT Frame Dimension Pointer</summary>
		public static DcmTag FrameDimensionPointer = new DcmTag(0x0028, 0x000a);

		/// <summary>(0028,0010) VR=US Rows</summary>
		public static DcmTag Rows = new DcmTag(0x0028, 0x0010);

		/// <summary>(0028,0011) VR=US Columns</summary>
		public static DcmTag Columns = new DcmTag(0x0028, 0x0011);

		/// <summary>(0028,0012) VR=US Planes</summary>
		public static DcmTag Planes = new DcmTag(0x0028, 0x0012);

		/// <summary>(0028,0014) VR=US Ultrasound Color Data Present</summary>
		public static DcmTag UltrasoundColorDataPresent = new DcmTag(0x0028, 0x0014);

		/// <summary>(0028,0030) VR=DS Pixel Spacing</summary>
		public static DcmTag PixelSpacing = new DcmTag(0x0028, 0x0030);

		/// <summary>(0028,0031) VR=DS Zoom Factor</summary>
		public static DcmTag ZoomFactor = new DcmTag(0x0028, 0x0031);

		/// <summary>(0028,0032) VR=DS Zoom Center</summary>
		public static DcmTag ZoomCenter = new DcmTag(0x0028, 0x0032);

		/// <summary>(0028,0034) VR=IS Pixel Aspect Ratio</summary>
		public static DcmTag PixelAspectRatio = new DcmTag(0x0028, 0x0034);

		/// <summary>(0028,0040) VR=CS Image Format (Retired)</summary>
		public static DcmTag ImageFormatRETIRED = new DcmTag(0x0028, 0x0040);

		/// <summary>(0028,0050) VR=LO Manipulated Image (Retired)</summary>
		public static DcmTag ManipulatedImageRETIRED = new DcmTag(0x0028, 0x0050);

		/// <summary>(0028,0051) VR=CS Corrected Image</summary>
		public static DcmTag CorrectedImage = new DcmTag(0x0028, 0x0051);

		/// <summary>(0028,0060) VR=CS Compression Code (Retired)</summary>
		public static DcmTag CompressionCodeRETIRED = new DcmTag(0x0028, 0x0060);

		/// <summary>(0028,0100) VR=US Bits Allocated</summary>
		public static DcmTag BitsAllocated = new DcmTag(0x0028, 0x0100);

		/// <summary>(0028,0101) VR=US Bits Stored</summary>
		public static DcmTag BitsStored = new DcmTag(0x0028, 0x0101);

		/// <summary>(0028,0102) VR=US High Bit</summary>
		public static DcmTag HighBit = new DcmTag(0x0028, 0x0102);

		/// <summary>(0028,0103) VR=US Pixel Representation</summary>
		public static DcmTag PixelRepresentation = new DcmTag(0x0028, 0x0103);

		/// <summary>(0028,0104) VR=US Smallest Valid Pixel Value (Retired)</summary>
		public static DcmTag SmallestValidPixelValueRETIRED = new DcmTag(0x0028, 0x0104);

		/// <summary>(0028,0105) VR=US Largest Valid Pixel Value (Retired)</summary>
		public static DcmTag LargestValidPixelValueRETIRED = new DcmTag(0x0028, 0x0105);

		/// <summary>(0028,0106) VR=US Smallest Image Pixel Value</summary>
		public static DcmTag SmallestImagePixelValue = new DcmTag(0x0028, 0x0106);

		/// <summary>(0028,0107) VR=US Largest Image Pixel Value</summary>
		public static DcmTag LargestImagePixelValue = new DcmTag(0x0028, 0x0107);

		/// <summary>(0028,0108) VR=US Smallest Pixel Value in Series</summary>
		public static DcmTag SmallestPixelValueInSeries = new DcmTag(0x0028, 0x0108);

		/// <summary>(0028,0109) VR=US Largest Pixel Value in Series</summary>
		public static DcmTag LargestPixelValueInSeries = new DcmTag(0x0028, 0x0109);

		/// <summary>(0028,0110) VR=US Smallest Image Pixel Value in Plane</summary>
		public static DcmTag SmallestImagePixelValueInPlane = new DcmTag(0x0028, 0x0110);

		/// <summary>(0028,0111) VR=US Largest Image Pixel Value in Plane</summary>
		public static DcmTag LargestImagePixelValueInPlane = new DcmTag(0x0028, 0x0111);

		/// <summary>(0028,0120) VR=US Pixel Padding Value</summary>
		public static DcmTag PixelPaddingValue = new DcmTag(0x0028, 0x0120);

		/// <summary>(0028,0200) VR=US Image Location (Retired)</summary>
		public static DcmTag ImageLocationRETIRED = new DcmTag(0x0028, 0x0200);

		/// <summary>(0028,0300) VR=CS Quality Control Image</summary>
		public static DcmTag QualityControlImage = new DcmTag(0x0028, 0x0300);

		/// <summary>(0028,0301) VR=CS Burned In Annotation</summary>
		public static DcmTag BurnedInAnnotation = new DcmTag(0x0028, 0x0301);

		/// <summary>(0028,0402) VR=CS Pixel Spacing Calibration Type</summary>
		public static DcmTag PixelSpacingCalibrationType = new DcmTag(0x0028, 0x0402);

		/// <summary>(0028,0404) VR=LO Pixel Spacing Calibration Description</summary>
		public static DcmTag PixelSpacingCalibrationDescription = new DcmTag(0x0028, 0x0404);

		/// <summary>(0028,1040) VR=CS Pixel Intensity Relationship</summary>
		public static DcmTag PixelIntensityRelationship = new DcmTag(0x0028, 0x1040);

		/// <summary>(0028,1041) VR=SS Pixel Intensity Relationship Sign</summary>
		public static DcmTag PixelIntensityRelationshipSign = new DcmTag(0x0028, 0x1041);

		/// <summary>(0028,1050) VR=DS Window Center</summary>
		public static DcmTag WindowCenter = new DcmTag(0x0028, 0x1050);

		/// <summary>(0028,1051) VR=DS Window Width</summary>
		public static DcmTag WindowWidth = new DcmTag(0x0028, 0x1051);

		/// <summary>(0028,1052) VR=DS Rescale Intercept</summary>
		public static DcmTag RescaleIntercept = new DcmTag(0x0028, 0x1052);

		/// <summary>(0028,1053) VR=DS Rescale Slope</summary>
		public static DcmTag RescaleSlope = new DcmTag(0x0028, 0x1053);

		/// <summary>(0028,1054) VR=LO Rescale Type</summary>
		public static DcmTag RescaleType = new DcmTag(0x0028, 0x1054);

		/// <summary>(0028,1055) VR=LO Window Center &amp; Width Explanation</summary>
		public static DcmTag WindowCenterWidthExplanation = new DcmTag(0x0028, 0x1055);

		/// <summary>(0028,1056) VR=CS VOI LUT Function</summary>
		public static DcmTag VOILUTFunction = new DcmTag(0x0028, 0x1056);

		/// <summary>(0028,1080) VR=CS Gray Scale (Retired)</summary>
		public static DcmTag GrayScaleRETIRED = new DcmTag(0x0028, 0x1080);

		/// <summary>(0028,1090) VR=CS Recommended Viewing Mode</summary>
		public static DcmTag RecommendedViewingMode = new DcmTag(0x0028, 0x1090);

		/// <summary>(0028,1100) VR=US Gray Lookup Table Descriptor (Retired)</summary>
		public static DcmTag GrayLookupTableDescriptorRETIRED = new DcmTag(0x0028, 0x1100);

		/// <summary>(0028,1101) VR=US Red Palette Color Lookup Table Descriptor</summary>
		public static DcmTag RedPaletteColorLookupTableDescriptor = new DcmTag(0x0028, 0x1101);

		/// <summary>(0028,1102) VR=US Green Palette Color Lookup Table Descriptor</summary>
		public static DcmTag GreenPaletteColorLookupTableDescriptor = new DcmTag(0x0028, 0x1102);

		/// <summary>(0028,1103) VR=US Blue Palette Color Lookup Table Descriptor</summary>
		public static DcmTag BluePaletteColorLookupTableDescriptor = new DcmTag(0x0028, 0x1103);

		/// <summary>(0028,1199) VR=UI Palette Color Lookup Table UID</summary>
		public static DcmTag PaletteColorLookupTableUID = new DcmTag(0x0028, 0x1199);

		/// <summary>(0028,1200) VR=US Gray Lookup Table Data (Retired)</summary>
		public static DcmTag GrayLookupTableDataRETIRED = new DcmTag(0x0028, 0x1200);

		/// <summary>(0028,1201) VR=OW Red Palette Color Lookup Table Data</summary>
		public static DcmTag RedPaletteColorLookupTableData = new DcmTag(0x0028, 0x1201);

		/// <summary>(0028,1202) VR=OW Green Palette Color Lookup Table Data</summary>
		public static DcmTag GreenPaletteColorLookupTableData = new DcmTag(0x0028, 0x1202);

		/// <summary>(0028,1203) VR=OW Blue Palette Color Lookup Table Data</summary>
		public static DcmTag BluePaletteColorLookupTableData = new DcmTag(0x0028, 0x1203);

		/// <summary>(0028,1221) VR=OW Segmented Red Palette Color Lookup Table Data</summary>
		public static DcmTag SegmentedRedPaletteColorLookupTableData = new DcmTag(0x0028, 0x1221);

		/// <summary>(0028,1222) VR=OW Segmented Green Palette Color Lookup Table Data</summary>
		public static DcmTag SegmentedGreenPaletteColorLookupTableData = new DcmTag(0x0028, 0x1222);

		/// <summary>(0028,1223) VR=OW Segmented Blue Palette Color Lookup Table Data</summary>
		public static DcmTag SegmentedBluePaletteColorLookupTableData = new DcmTag(0x0028, 0x1223);

		/// <summary>(0028,1300) VR=CS Implant Present</summary>
		public static DcmTag ImplantPresent = new DcmTag(0x0028, 0x1300);

		/// <summary>(0028,1350) VR=CS Partial View</summary>
		public static DcmTag PartialView = new DcmTag(0x0028, 0x1350);

		/// <summary>(0028,1351) VR=ST Partial View Description</summary>
		public static DcmTag PartialViewDescription = new DcmTag(0x0028, 0x1351);

		/// <summary>(0028,1352) VR=SQ Partial View Code Sequence</summary>
		public static DcmTag PartialViewCodeSequence = new DcmTag(0x0028, 0x1352);

		/// <summary>(0028,135a) VR=CS Spatial Locations Preserved</summary>
		public static DcmTag SpatialLocationsPreserved = new DcmTag(0x0028, 0x135a);

		/// <summary>(0028,2000) VR=OB ICC Profile</summary>
		public static DcmTag ICCProfile = new DcmTag(0x0028, 0x2000);

		/// <summary>(0028,2110) VR=CS Lossy Image Compression</summary>
		public static DcmTag LossyImageCompression = new DcmTag(0x0028, 0x2110);

		/// <summary>(0028,2112) VR=DS Lossy Image Compression Ratio</summary>
		public static DcmTag LossyImageCompressionRatio = new DcmTag(0x0028, 0x2112);

		/// <summary>(0028,2114) VR=CS Lossy Image Compression Method</summary>
		public static DcmTag LossyImageCompressionMethod = new DcmTag(0x0028, 0x2114);

		/// <summary>(0028,3000) VR=SQ Modality LUT Sequence</summary>
		public static DcmTag ModalityLUTSequence = new DcmTag(0x0028, 0x3000);

		/// <summary>(0028,3002) VR=US LUT Descriptor</summary>
		public static DcmTag LUTDescriptor = new DcmTag(0x0028, 0x3002);

		/// <summary>(0028,3003) VR=LO LUT Explanation</summary>
		public static DcmTag LUTExplanation = new DcmTag(0x0028, 0x3003);

		/// <summary>(0028,3004) VR=LO Modality LUT Type</summary>
		public static DcmTag ModalityLUTType = new DcmTag(0x0028, 0x3004);

		/// <summary>(0028,3006) VR=US LUT Data</summary>
		public static DcmTag LUTData = new DcmTag(0x0028, 0x3006);

		/// <summary>(0028,3010) VR=SQ VOI LUT Sequence</summary>
		public static DcmTag VOILUTSequence = new DcmTag(0x0028, 0x3010);

		/// <summary>(0028,3110) VR=SQ Softcopy VOI LUT Sequence</summary>
		public static DcmTag SoftcopyVOILUTSequence = new DcmTag(0x0028, 0x3110);

		/// <summary>(0028,4000) VR=LT Image Presentation Comments (Retired)</summary>
		public static DcmTag ImagePresentationCommentsRETIRED = new DcmTag(0x0028, 0x4000);

		/// <summary>(0028,5000) VR=SQ Bi-Plane Acquisition Sequence</summary>
		public static DcmTag BiPlaneAcquisitionSequence = new DcmTag(0x0028, 0x5000);

		/// <summary>(0028,6010) VR=US Representative Frame Number</summary>
		public static DcmTag RepresentativeFrameNumber = new DcmTag(0x0028, 0x6010);

		/// <summary>(0028,6020) VR=US Frame Numbers of Interest (FOI)</summary>
		public static DcmTag FrameNumbersOfInterestFOI = new DcmTag(0x0028, 0x6020);

		/// <summary>(0028,6022) VR=LO Frame(s) of Interest Description</summary>
		public static DcmTag FramesOfInterestDescription = new DcmTag(0x0028, 0x6022);

		/// <summary>(0028,6023) VR=CS Frame of Interest Type</summary>
		public static DcmTag FrameOfInterestType = new DcmTag(0x0028, 0x6023);

		/// <summary>(0028,6030) VR=US Mask Pointer(s) (Retired)</summary>
		public static DcmTag MaskPointersRETIRED = new DcmTag(0x0028, 0x6030);

		/// <summary>(0028,6040) VR=US R Wave Pointer</summary>
		public static DcmTag RWavePointer = new DcmTag(0x0028, 0x6040);

		/// <summary>(0028,6100) VR=SQ Mask Subtraction Sequence</summary>
		public static DcmTag MaskSubtractionSequence = new DcmTag(0x0028, 0x6100);

		/// <summary>(0028,6101) VR=CS Mask Operation</summary>
		public static DcmTag MaskOperation = new DcmTag(0x0028, 0x6101);

		/// <summary>(0028,6102) VR=US Applicable Frame Range</summary>
		public static DcmTag ApplicableFrameRange = new DcmTag(0x0028, 0x6102);

		/// <summary>(0028,6110) VR=US Mask Frame Numbers</summary>
		public static DcmTag MaskFrameNumbers = new DcmTag(0x0028, 0x6110);

		/// <summary>(0028,6112) VR=US Contrast Frame Averaging</summary>
		public static DcmTag ContrastFrameAveraging = new DcmTag(0x0028, 0x6112);

		/// <summary>(0028,6114) VR=FL Mask Sub-pixel Shift</summary>
		public static DcmTag MaskSubpixelShift = new DcmTag(0x0028, 0x6114);

		/// <summary>(0028,6120) VR=SS TID Offset</summary>
		public static DcmTag TIDOffset = new DcmTag(0x0028, 0x6120);

		/// <summary>(0028,6190) VR=ST Mask Operation Explanation</summary>
		public static DcmTag MaskOperationExplanation = new DcmTag(0x0028, 0x6190);

		/// <summary>(0028,7fe0) VR=UT Pixel Data Provider URL</summary>
		public static DcmTag PixelDataProviderURL = new DcmTag(0x0028, 0x7fe0);

		/// <summary>(0028,9001) VR=UL Data Point Rows</summary>
		public static DcmTag DataPointRows = new DcmTag(0x0028, 0x9001);

		/// <summary>(0028,9002) VR=UL Data Point Columns</summary>
		public static DcmTag DataPointColumns = new DcmTag(0x0028, 0x9002);

		/// <summary>(0028,9003) VR=CS Signal Domain Columns</summary>
		public static DcmTag SignalDomainColumns = new DcmTag(0x0028, 0x9003);

		/// <summary>(0028,9099) VR=US Largest Monochrome Pixel Value (Retired)</summary>
		public static DcmTag LargestMonochromePixelValueRETIRED = new DcmTag(0x0028, 0x9099);

		/// <summary>(0028,9108) VR=CS Data Representation</summary>
		public static DcmTag DataRepresentation = new DcmTag(0x0028, 0x9108);

		/// <summary>(0028,9110) VR=SQ Pixel Measures Sequence</summary>
		public static DcmTag PixelMeasuresSequence = new DcmTag(0x0028, 0x9110);

		/// <summary>(0028,9132) VR=SQ Frame VOI LUT Sequence</summary>
		public static DcmTag FrameVOILUTSequence = new DcmTag(0x0028, 0x9132);

		/// <summary>(0028,9145) VR=SQ Pixel Value Transformation Sequence</summary>
		public static DcmTag PixelValueTransformationSequence = new DcmTag(0x0028, 0x9145);

		/// <summary>(0028,9235) VR=CS Signal Domain Rows</summary>
		public static DcmTag SignalDomainRows = new DcmTag(0x0028, 0x9235);

		/// <summary>(0028,9411) VR=FL Display Filter Percentage</summary>
		public static DcmTag DisplayFilterPercentage = new DcmTag(0x0028, 0x9411);

		/// <summary>(0028,9415) VR=SQ Frame Pixel Shift Sequence</summary>
		public static DcmTag FramePixelShiftSequence = new DcmTag(0x0028, 0x9415);

		/// <summary>(0028,9416) VR=US Subtraction Item ID</summary>
		public static DcmTag SubtractionItemID = new DcmTag(0x0028, 0x9416);

		/// <summary>(0028,9422) VR=SQ Pixel Intensity Relationship LUT Sequence</summary>
		public static DcmTag PixelIntensityRelationshipLUTSequence = new DcmTag(0x0028, 0x9422);

		/// <summary>(0028,9443) VR=SQ Frame Pixel Data Properties Sequence</summary>
		public static DcmTag FramePixelDataPropertiesSequence = new DcmTag(0x0028, 0x9443);

		/// <summary>(0028,9444) VR=CS Geometrical Properties</summary>
		public static DcmTag GeometricalProperties = new DcmTag(0x0028, 0x9444);

		/// <summary>(0028,9445) VR=FL Geometric Maximum Distortion</summary>
		public static DcmTag GeometricMaximumDistortion = new DcmTag(0x0028, 0x9445);

		/// <summary>(0028,9446) VR=CS Image Processing Applied</summary>
		public static DcmTag ImageProcessingApplied = new DcmTag(0x0028, 0x9446);

		/// <summary>(0028,9454) VR=CS Mask Selection Mode</summary>
		public static DcmTag MaskSelectionMode = new DcmTag(0x0028, 0x9454);

		/// <summary>(0028,9474) VR=CS LUT Function</summary>
		public static DcmTag LUTFunction = new DcmTag(0x0028, 0x9474);

		/// <summary>(0032,000a) VR=CS Study Status ID (Retired)</summary>
		public static DcmTag StudyStatusIDRETIRED = new DcmTag(0x0032, 0x000a);

		/// <summary>(0032,000c) VR=CS Study Priority ID (Retired)</summary>
		public static DcmTag StudyPriorityIDRETIRED = new DcmTag(0x0032, 0x000c);

		/// <summary>(0032,0012) VR=LO Study ID Issuer (Retired)</summary>
		public static DcmTag StudyIDIssuerRETIRED = new DcmTag(0x0032, 0x0012);

		/// <summary>(0032,0032) VR=DA Study Verified Date (Retired)</summary>
		public static DcmTag StudyVerifiedDateRETIRED = new DcmTag(0x0032, 0x0032);

		/// <summary>(0032,0033) VR=TM Study Verified Time (Retired)</summary>
		public static DcmTag StudyVerifiedTimeRETIRED = new DcmTag(0x0032, 0x0033);

		/// <summary>(0032,0034) VR=DA Study Read Date (Retired)</summary>
		public static DcmTag StudyReadDateRETIRED = new DcmTag(0x0032, 0x0034);

		/// <summary>(0032,0035) VR=TM Study Read Time (Retired)</summary>
		public static DcmTag StudyReadTimeRETIRED = new DcmTag(0x0032, 0x0035);

		/// <summary>(0032,1000) VR=DA Scheduled Study Start Date (Retired)</summary>
		public static DcmTag ScheduledStudyStartDateRETIRED = new DcmTag(0x0032, 0x1000);

		/// <summary>(0032,1001) VR=TM Scheduled Study Start Time (Retired)</summary>
		public static DcmTag ScheduledStudyStartTimeRETIRED = new DcmTag(0x0032, 0x1001);

		/// <summary>(0032,1010) VR=DA Scheduled Study Stop Date (Retired)</summary>
		public static DcmTag ScheduledStudyStopDateRETIRED = new DcmTag(0x0032, 0x1010);

		/// <summary>(0032,1011) VR=TM Scheduled Study Stop Time (Retired)</summary>
		public static DcmTag ScheduledStudyStopTimeRETIRED = new DcmTag(0x0032, 0x1011);

		/// <summary>(0032,1020) VR=LO Scheduled Study Location (Retired)</summary>
		public static DcmTag ScheduledStudyLocationRETIRED = new DcmTag(0x0032, 0x1020);

		/// <summary>(0032,1021) VR=AE Scheduled Study Location AE Title (Retired)</summary>
		public static DcmTag ScheduledStudyLocationAETitleRETIRED = new DcmTag(0x0032, 0x1021);

		/// <summary>(0032,1030) VR=LO Reason for Study (Retired)</summary>
		public static DcmTag ReasonForStudyRETIRED = new DcmTag(0x0032, 0x1030);

		/// <summary>(0032,1031) VR=SQ Requesting Physician Identification Sequence</summary>
		public static DcmTag RequestingPhysicianIdentificationSequence = new DcmTag(0x0032, 0x1031);

		/// <summary>(0032,1032) VR=PN Requesting Physician</summary>
		public static DcmTag RequestingPhysician = new DcmTag(0x0032, 0x1032);

		/// <summary>(0032,1033) VR=LO Requesting Service</summary>
		public static DcmTag RequestingService = new DcmTag(0x0032, 0x1033);

		/// <summary>(0032,1040) VR=DA Study Arrival Date (Retired)</summary>
		public static DcmTag StudyArrivalDateRETIRED = new DcmTag(0x0032, 0x1040);

		/// <summary>(0032,1041) VR=TM Study Arrival Time (Retired)</summary>
		public static DcmTag StudyArrivalTimeRETIRED = new DcmTag(0x0032, 0x1041);

		/// <summary>(0032,1050) VR=DA Study Completion Date (Retired)</summary>
		public static DcmTag StudyCompletionDateRETIRED = new DcmTag(0x0032, 0x1050);

		/// <summary>(0032,1051) VR=TM Study Completion Time (Retired)</summary>
		public static DcmTag StudyCompletionTimeRETIRED = new DcmTag(0x0032, 0x1051);

		/// <summary>(0032,1055) VR=CS Study Component Status ID (Retired)</summary>
		public static DcmTag StudyComponentStatusIDRETIRED = new DcmTag(0x0032, 0x1055);

		/// <summary>(0032,1060) VR=LO Requested Procedure Description</summary>
		public static DcmTag RequestedProcedureDescription = new DcmTag(0x0032, 0x1060);

		/// <summary>(0032,1064) VR=SQ Requested Procedure Code Sequence</summary>
		public static DcmTag RequestedProcedureCodeSequence = new DcmTag(0x0032, 0x1064);

		/// <summary>(0032,1070) VR=LO Requested Contrast Agent</summary>
		public static DcmTag RequestedContrastAgent = new DcmTag(0x0032, 0x1070);

		/// <summary>(0032,4000) VR=LT Study Comments</summary>
		public static DcmTag StudyComments = new DcmTag(0x0032, 0x4000);

		/// <summary>(0038,0004) VR=SQ Referenced Patient Alias Sequence</summary>
		public static DcmTag ReferencedPatientAliasSequence = new DcmTag(0x0038, 0x0004);

		/// <summary>(0038,0008) VR=CS Visit Status ID</summary>
		public static DcmTag VisitStatusID = new DcmTag(0x0038, 0x0008);

		/// <summary>(0038,0010) VR=LO Admission ID</summary>
		public static DcmTag AdmissionID = new DcmTag(0x0038, 0x0010);

		/// <summary>(0038,0011) VR=LO Issuer of Admission ID</summary>
		public static DcmTag IssuerOfAdmissionID = new DcmTag(0x0038, 0x0011);

		/// <summary>(0038,0016) VR=LO Route of Admissions</summary>
		public static DcmTag RouteOfAdmissions = new DcmTag(0x0038, 0x0016);

		/// <summary>(0038,001a) VR=DA Scheduled Admission Date (Retired)</summary>
		public static DcmTag ScheduledAdmissionDateRETIRED = new DcmTag(0x0038, 0x001a);

		/// <summary>(0038,001b) VR=TM Scheduled Admission Time (Retired)</summary>
		public static DcmTag ScheduledAdmissionTimeRETIRED = new DcmTag(0x0038, 0x001b);

		/// <summary>(0038,001c) VR=DA Scheduled Discharge Date (Retired)</summary>
		public static DcmTag ScheduledDischargeDateRETIRED = new DcmTag(0x0038, 0x001c);

		/// <summary>(0038,001d) VR=TM Scheduled Discharge Time (Retired)</summary>
		public static DcmTag ScheduledDischargeTimeRETIRED = new DcmTag(0x0038, 0x001d);

		/// <summary>(0038,001e) VR=LO Scheduled Patient Institution Residence (Retired)</summary>
		public static DcmTag ScheduledPatientInstitutionResidenceRETIRED = new DcmTag(0x0038, 0x001e);

		/// <summary>(0038,0020) VR=DA Admitting Date</summary>
		public static DcmTag AdmittingDate = new DcmTag(0x0038, 0x0020);

		/// <summary>(0038,0021) VR=TM Admitting Time</summary>
		public static DcmTag AdmittingTime = new DcmTag(0x0038, 0x0021);

		/// <summary>(0038,0030) VR=DA Discharge Date (Retired)</summary>
		public static DcmTag DischargeDateRETIRED = new DcmTag(0x0038, 0x0030);

		/// <summary>(0038,0032) VR=TM Discharge Time (Retired)</summary>
		public static DcmTag DischargeTimeRETIRED = new DcmTag(0x0038, 0x0032);

		/// <summary>(0038,0040) VR=LO Discharge Diagnosis Description (Retired)</summary>
		public static DcmTag DischargeDiagnosisDescriptionRETIRED = new DcmTag(0x0038, 0x0040);

		/// <summary>(0038,0044) VR=SQ Discharge Diagnosis Code Sequence (Retired)</summary>
		public static DcmTag DischargeDiagnosisCodeSequenceRETIRED = new DcmTag(0x0038, 0x0044);

		/// <summary>(0038,0050) VR=LO Special Needs</summary>
		public static DcmTag SpecialNeeds = new DcmTag(0x0038, 0x0050);

		/// <summary>(0038,0100) VR=SQ Pertinent Documents Sequence</summary>
		public static DcmTag PertinentDocumentsSequence = new DcmTag(0x0038, 0x0100);

		/// <summary>(0038,0300) VR=LO Current Patient Location</summary>
		public static DcmTag CurrentPatientLocation = new DcmTag(0x0038, 0x0300);

		/// <summary>(0038,0400) VR=LO Patient's Institution Residence</summary>
		public static DcmTag PatientsInstitutionResidence = new DcmTag(0x0038, 0x0400);

		/// <summary>(0038,0500) VR=LO Patient State</summary>
		public static DcmTag PatientState = new DcmTag(0x0038, 0x0500);

		/// <summary>(0038,0502) VR=SQ Patient Clinical Trial Participation Sequence</summary>
		public static DcmTag PatientClinicalTrialParticipationSequence = new DcmTag(0x0038, 0x0502);

		/// <summary>(0038,4000) VR=LT Visit Comments</summary>
		public static DcmTag VisitComments = new DcmTag(0x0038, 0x4000);

		/// <summary>(003a,0004) VR=CS Waveform Originality</summary>
		public static DcmTag WaveformOriginality = new DcmTag(0x003a, 0x0004);

		/// <summary>(003a,0005) VR=US Number of Waveform Channels</summary>
		public static DcmTag NumberOfWaveformChannels = new DcmTag(0x003a, 0x0005);

		/// <summary>(003a,0010) VR=UL Number of Waveform Samples</summary>
		public static DcmTag NumberOfWaveformSamples = new DcmTag(0x003a, 0x0010);

		/// <summary>(003a,001a) VR=DS Sampling Frequency</summary>
		public static DcmTag SamplingFrequency = new DcmTag(0x003a, 0x001a);

		/// <summary>(003a,0020) VR=SH Multiplex Group Label</summary>
		public static DcmTag MultiplexGroupLabel = new DcmTag(0x003a, 0x0020);

		/// <summary>(003a,0200) VR=SQ Channel Definition Sequence</summary>
		public static DcmTag ChannelDefinitionSequence = new DcmTag(0x003a, 0x0200);

		/// <summary>(003a,0202) VR=IS Waveform Channel Number</summary>
		public static DcmTag WaveformChannelNumber = new DcmTag(0x003a, 0x0202);

		/// <summary>(003a,0203) VR=SH Channel Label</summary>
		public static DcmTag ChannelLabel = new DcmTag(0x003a, 0x0203);

		/// <summary>(003a,0205) VR=CS Channel Status</summary>
		public static DcmTag ChannelStatus = new DcmTag(0x003a, 0x0205);

		/// <summary>(003a,0208) VR=SQ Channel Source Sequence</summary>
		public static DcmTag ChannelSourceSequence = new DcmTag(0x003a, 0x0208);

		/// <summary>(003a,0209) VR=SQ Channel Source Modifiers Sequence</summary>
		public static DcmTag ChannelSourceModifiersSequence = new DcmTag(0x003a, 0x0209);

		/// <summary>(003a,020a) VR=SQ Source Waveform Sequence</summary>
		public static DcmTag SourceWaveformSequence = new DcmTag(0x003a, 0x020a);

		/// <summary>(003a,020c) VR=LO Channel Derivation Description</summary>
		public static DcmTag ChannelDerivationDescription = new DcmTag(0x003a, 0x020c);

		/// <summary>(003a,0210) VR=DS Channel Sensitivity</summary>
		public static DcmTag ChannelSensitivity = new DcmTag(0x003a, 0x0210);

		/// <summary>(003a,0211) VR=SQ Channel Sensitivity Units Sequence</summary>
		public static DcmTag ChannelSensitivityUnitsSequence = new DcmTag(0x003a, 0x0211);

		/// <summary>(003a,0212) VR=DS Channel Sensitivity Correction Factor</summary>
		public static DcmTag ChannelSensitivityCorrectionFactor = new DcmTag(0x003a, 0x0212);

		/// <summary>(003a,0213) VR=DS Channel Baseline</summary>
		public static DcmTag ChannelBaseline = new DcmTag(0x003a, 0x0213);

		/// <summary>(003a,0214) VR=DS Channel Time Skew</summary>
		public static DcmTag ChannelTimeSkew = new DcmTag(0x003a, 0x0214);

		/// <summary>(003a,0215) VR=DS Channel Sample Skew</summary>
		public static DcmTag ChannelSampleSkew = new DcmTag(0x003a, 0x0215);

		/// <summary>(003a,0218) VR=DS Channel Offset</summary>
		public static DcmTag ChannelOffset = new DcmTag(0x003a, 0x0218);

		/// <summary>(003a,021a) VR=US Waveform Bits Stored</summary>
		public static DcmTag WaveformBitsStored = new DcmTag(0x003a, 0x021a);

		/// <summary>(003a,0220) VR=DS Filter Low Frequency</summary>
		public static DcmTag FilterLowFrequency = new DcmTag(0x003a, 0x0220);

		/// <summary>(003a,0221) VR=DS Filter High Frequency</summary>
		public static DcmTag FilterHighFrequency = new DcmTag(0x003a, 0x0221);

		/// <summary>(003a,0222) VR=DS Notch Filter Frequency</summary>
		public static DcmTag NotchFilterFrequency = new DcmTag(0x003a, 0x0222);

		/// <summary>(003a,0223) VR=DS Notch Filter Bandwidth</summary>
		public static DcmTag NotchFilterBandwidth = new DcmTag(0x003a, 0x0223);

		/// <summary>(003a,0300) VR=SQ Multiplexed Audio Channels Description Code Sequence</summary>
		public static DcmTag MultiplexedAudioChannelsDescriptionCodeSequence = new DcmTag(0x003a, 0x0300);

		/// <summary>(003a,0301) VR=IS Channel Identification Code</summary>
		public static DcmTag ChannelIdentificationCode = new DcmTag(0x003a, 0x0301);

		/// <summary>(003a,0302) VR=CS Channel Mode</summary>
		public static DcmTag ChannelMode = new DcmTag(0x003a, 0x0302);

		/// <summary>(0040,0001) VR=AE Scheduled Station AE Title</summary>
		public static DcmTag ScheduledStationAETitle = new DcmTag(0x0040, 0x0001);

		/// <summary>(0040,0002) VR=DA Scheduled Procedure Step Start Date</summary>
		public static DcmTag ScheduledProcedureStepStartDate = new DcmTag(0x0040, 0x0002);

		/// <summary>(0040,0003) VR=TM Scheduled Procedure Step Start Time</summary>
		public static DcmTag ScheduledProcedureStepStartTime = new DcmTag(0x0040, 0x0003);

		/// <summary>(0040,0004) VR=DA Scheduled Procedure Step End Date</summary>
		public static DcmTag ScheduledProcedureStepEndDate = new DcmTag(0x0040, 0x0004);

		/// <summary>(0040,0005) VR=TM Scheduled Procedure Step End Time</summary>
		public static DcmTag ScheduledProcedureStepEndTime = new DcmTag(0x0040, 0x0005);

		/// <summary>(0040,0006) VR=PN Scheduled Performing Physician's Name</summary>
		public static DcmTag ScheduledPerformingPhysiciansName = new DcmTag(0x0040, 0x0006);

		/// <summary>(0040,0007) VR=LO Scheduled Procedure Step Description</summary>
		public static DcmTag ScheduledProcedureStepDescription = new DcmTag(0x0040, 0x0007);

		/// <summary>(0040,0008) VR=SQ Scheduled Protocol Code Sequence</summary>
		public static DcmTag ScheduledProtocolCodeSequence = new DcmTag(0x0040, 0x0008);

		/// <summary>(0040,0009) VR=SH Scheduled Procedure Step ID</summary>
		public static DcmTag ScheduledProcedureStepID = new DcmTag(0x0040, 0x0009);

		/// <summary>(0040,000a) VR=SQ Stage Code Sequence</summary>
		public static DcmTag StageCodeSequence = new DcmTag(0x0040, 0x000a);

		/// <summary>(0040,000b) VR=SQ Scheduled Performing Physician Identification Sequence</summary>
		public static DcmTag ScheduledPerformingPhysicianIdentificationSequence = new DcmTag(0x0040, 0x000b);

		/// <summary>(0040,0010) VR=SH Scheduled Station Name</summary>
		public static DcmTag ScheduledStationName = new DcmTag(0x0040, 0x0010);

		/// <summary>(0040,0011) VR=SH Scheduled Procedure Step Location</summary>
		public static DcmTag ScheduledProcedureStepLocation = new DcmTag(0x0040, 0x0011);

		/// <summary>(0040,0012) VR=LO Pre-Medication</summary>
		public static DcmTag PreMedication = new DcmTag(0x0040, 0x0012);

		/// <summary>(0040,0020) VR=CS Scheduled Procedure Step Status</summary>
		public static DcmTag ScheduledProcedureStepStatus = new DcmTag(0x0040, 0x0020);

		/// <summary>(0040,0100) VR=SQ Scheduled Procedure Step Sequence</summary>
		public static DcmTag ScheduledProcedureStepSequence = new DcmTag(0x0040, 0x0100);

		/// <summary>(0040,0220) VR=SQ Referenced Non-Image Composite SOP Instance Sequence</summary>
		public static DcmTag ReferencedNonImageCompositeSOPInstanceSequence = new DcmTag(0x0040, 0x0220);

		/// <summary>(0040,0241) VR=AE Performed Station AE Title</summary>
		public static DcmTag PerformedStationAETitle = new DcmTag(0x0040, 0x0241);

		/// <summary>(0040,0242) VR=SH Performed Station Name</summary>
		public static DcmTag PerformedStationName = new DcmTag(0x0040, 0x0242);

		/// <summary>(0040,0243) VR=SH Performed Location</summary>
		public static DcmTag PerformedLocation = new DcmTag(0x0040, 0x0243);

		/// <summary>(0040,0244) VR=DA Performed Procedure Step Start Date</summary>
		public static DcmTag PerformedProcedureStepStartDate = new DcmTag(0x0040, 0x0244);

		/// <summary>(0040,0245) VR=TM Performed Procedure Step Start Time</summary>
		public static DcmTag PerformedProcedureStepStartTime = new DcmTag(0x0040, 0x0245);

		/// <summary>(0040,0250) VR=DA Performed Procedure Step End Date</summary>
		public static DcmTag PerformedProcedureStepEndDate = new DcmTag(0x0040, 0x0250);

		/// <summary>(0040,0251) VR=TM Performed Procedure Step End Time</summary>
		public static DcmTag PerformedProcedureStepEndTime = new DcmTag(0x0040, 0x0251);

		/// <summary>(0040,0252) VR=CS Performed Procedure Step Status</summary>
		public static DcmTag PerformedProcedureStepStatus = new DcmTag(0x0040, 0x0252);

		/// <summary>(0040,0253) VR=SH Performed Procedure Step ID</summary>
		public static DcmTag PerformedProcedureStepID = new DcmTag(0x0040, 0x0253);

		/// <summary>(0040,0254) VR=LO Performed Procedure Step Description</summary>
		public static DcmTag PerformedProcedureStepDescription = new DcmTag(0x0040, 0x0254);

		/// <summary>(0040,0255) VR=LO Performed Procedure Type Description</summary>
		public static DcmTag PerformedProcedureTypeDescription = new DcmTag(0x0040, 0x0255);

		/// <summary>(0040,0260) VR=SQ Performed Protocol Code Sequence</summary>
		public static DcmTag PerformedProtocolCodeSequence = new DcmTag(0x0040, 0x0260);

		/// <summary>(0040,0270) VR=SQ Scheduled Step Attributes Sequence</summary>
		public static DcmTag ScheduledStepAttributesSequence = new DcmTag(0x0040, 0x0270);

		/// <summary>(0040,0275) VR=SQ Request Attributes Sequence</summary>
		public static DcmTag RequestAttributesSequence = new DcmTag(0x0040, 0x0275);

		/// <summary>(0040,0280) VR=ST Comments on the Performed Procedure Step</summary>
		public static DcmTag CommentsOnThePerformedProcedureStep = new DcmTag(0x0040, 0x0280);

		/// <summary>(0040,0281) VR=SQ Performed Procedure Step Discontinuation Reason Code Sequence</summary>
		public static DcmTag PerformedProcedureStepDiscontinuationReasonCodeSequence = new DcmTag(0x0040, 0x0281);

		/// <summary>(0040,0293) VR=SQ Quantity Sequence</summary>
		public static DcmTag QuantitySequence = new DcmTag(0x0040, 0x0293);

		/// <summary>(0040,0294) VR=DS Quantity</summary>
		public static DcmTag Quantity = new DcmTag(0x0040, 0x0294);

		/// <summary>(0040,0295) VR=SQ Measuring Units Sequence</summary>
		public static DcmTag MeasuringUnitsSequence = new DcmTag(0x0040, 0x0295);

		/// <summary>(0040,0296) VR=SQ Billing Item Sequence</summary>
		public static DcmTag BillingItemSequence = new DcmTag(0x0040, 0x0296);

		/// <summary>(0040,0300) VR=US Total Time of Fluoroscopy</summary>
		public static DcmTag TotalTimeOfFluoroscopy = new DcmTag(0x0040, 0x0300);

		/// <summary>(0040,0301) VR=US Total Number of Exposures</summary>
		public static DcmTag TotalNumberOfExposures = new DcmTag(0x0040, 0x0301);

		/// <summary>(0040,0302) VR=US Entrance Dose</summary>
		public static DcmTag EntranceDose = new DcmTag(0x0040, 0x0302);

		/// <summary>(0040,0303) VR=US Exposed Area</summary>
		public static DcmTag ExposedArea = new DcmTag(0x0040, 0x0303);

		/// <summary>(0040,0306) VR=DS Distance Source to Entrance</summary>
		public static DcmTag DistanceSourceToEntrance = new DcmTag(0x0040, 0x0306);

		/// <summary>(0040,0307) VR=DS Distance Source to Support (Retired)</summary>
		public static DcmTag DistanceSourceToSupportRETIRED = new DcmTag(0x0040, 0x0307);

		/// <summary>(0040,030e) VR=SQ Exposure Dose Sequence</summary>
		public static DcmTag ExposureDoseSequence = new DcmTag(0x0040, 0x030e);

		/// <summary>(0040,0310) VR=ST Comments on Radiation Dose</summary>
		public static DcmTag CommentsOnRadiationDose = new DcmTag(0x0040, 0x0310);

		/// <summary>(0040,0312) VR=DS X-Ray Output</summary>
		public static DcmTag XRayOutput = new DcmTag(0x0040, 0x0312);

		/// <summary>(0040,0314) VR=DS Half Value Layer</summary>
		public static DcmTag HalfValueLayer = new DcmTag(0x0040, 0x0314);

		/// <summary>(0040,0316) VR=DS Organ Dose</summary>
		public static DcmTag OrganDose = new DcmTag(0x0040, 0x0316);

		/// <summary>(0040,0318) VR=CS Organ Exposed</summary>
		public static DcmTag OrganExposed = new DcmTag(0x0040, 0x0318);

		/// <summary>(0040,0320) VR=SQ Billing Procedure Step Sequence</summary>
		public static DcmTag BillingProcedureStepSequence = new DcmTag(0x0040, 0x0320);

		/// <summary>(0040,0321) VR=SQ Film Consumption Sequence</summary>
		public static DcmTag FilmConsumptionSequence = new DcmTag(0x0040, 0x0321);

		/// <summary>(0040,0324) VR=SQ Billing Supplies and Devices Sequence</summary>
		public static DcmTag BillingSuppliesAndDevicesSequence = new DcmTag(0x0040, 0x0324);

		/// <summary>(0040,0330) VR=SQ Referenced Procedure Step Sequence (Retired)</summary>
		public static DcmTag ReferencedProcedureStepSequenceRETIRED = new DcmTag(0x0040, 0x0330);

		/// <summary>(0040,0340) VR=SQ Performed Series Sequence</summary>
		public static DcmTag PerformedSeriesSequence = new DcmTag(0x0040, 0x0340);

		/// <summary>(0040,0400) VR=LT Comments on the Scheduled Procedure Step</summary>
		public static DcmTag CommentsOnTheScheduledProcedureStep = new DcmTag(0x0040, 0x0400);

		/// <summary>(0040,0440) VR=SQ Protocol Context Sequence</summary>
		public static DcmTag ProtocolContextSequence = new DcmTag(0x0040, 0x0440);

		/// <summary>(0040,0441) VR=SQ Content Item Modifier Sequence</summary>
		public static DcmTag ContentItemModifierSequence = new DcmTag(0x0040, 0x0441);

		/// <summary>(0040,050a) VR=LO Specimen Accession Number</summary>
		public static DcmTag SpecimenAccessionNumber = new DcmTag(0x0040, 0x050a);

		/// <summary>(0040,0550) VR=SQ Specimen Sequence</summary>
		public static DcmTag SpecimenSequence = new DcmTag(0x0040, 0x0550);

		/// <summary>(0040,0551) VR=LO Specimen Identifier</summary>
		public static DcmTag SpecimenIdentifier = new DcmTag(0x0040, 0x0551);

		/// <summary>(0040,0555) VR=SQ Acquisition Context Sequence</summary>
		public static DcmTag AcquisitionContextSequence = new DcmTag(0x0040, 0x0555);

		/// <summary>(0040,0556) VR=ST Acquisition Context Description</summary>
		public static DcmTag AcquisitionContextDescription = new DcmTag(0x0040, 0x0556);

		/// <summary>(0040,059a) VR=SQ Specimen Type Code Sequence</summary>
		public static DcmTag SpecimenTypeCodeSequence = new DcmTag(0x0040, 0x059a);

		/// <summary>(0040,06fa) VR=LO Slide Identifier</summary>
		public static DcmTag SlideIdentifier = new DcmTag(0x0040, 0x06fa);

		/// <summary>(0040,071a) VR=SQ Image Center Point Coordinates Sequence</summary>
		public static DcmTag ImageCenterPointCoordinatesSequence = new DcmTag(0x0040, 0x071a);

		/// <summary>(0040,072a) VR=DS X offset in Slide Coordinate System</summary>
		public static DcmTag XOffsetInSlideCoordinateSystem = new DcmTag(0x0040, 0x072a);

		/// <summary>(0040,073a) VR=DS Y offset in Slide Coordinate System</summary>
		public static DcmTag YOffsetInSlideCoordinateSystem = new DcmTag(0x0040, 0x073a);

		/// <summary>(0040,074a) VR=DS Z offset in Slide Coordinate System</summary>
		public static DcmTag ZOffsetInSlideCoordinateSystem = new DcmTag(0x0040, 0x074a);

		/// <summary>(0040,08d8) VR=SQ Pixel Spacing Sequence</summary>
		public static DcmTag PixelSpacingSequence = new DcmTag(0x0040, 0x08d8);

		/// <summary>(0040,08da) VR=SQ Coordinate System Axis Code Sequence</summary>
		public static DcmTag CoordinateSystemAxisCodeSequence = new DcmTag(0x0040, 0x08da);

		/// <summary>(0040,08ea) VR=SQ Measurement Units Code Sequence</summary>
		public static DcmTag MeasurementUnitsCodeSequence = new DcmTag(0x0040, 0x08ea);

		/// <summary>(0040,1001) VR=SH Requested Procedure ID</summary>
		public static DcmTag RequestedProcedureID = new DcmTag(0x0040, 0x1001);

		/// <summary>(0040,1002) VR=LO Reason for the Requested Procedure</summary>
		public static DcmTag ReasonForTheRequestedProcedure = new DcmTag(0x0040, 0x1002);

		/// <summary>(0040,1003) VR=SH Requested Procedure Priority</summary>
		public static DcmTag RequestedProcedurePriority = new DcmTag(0x0040, 0x1003);

		/// <summary>(0040,1004) VR=LO Patient Transport Arrangements</summary>
		public static DcmTag PatientTransportArrangements = new DcmTag(0x0040, 0x1004);

		/// <summary>(0040,1005) VR=LO Requested Procedure Location</summary>
		public static DcmTag RequestedProcedureLocation = new DcmTag(0x0040, 0x1005);

		/// <summary>(0040,1006) VR=SH Placer Order Number / Procedure (Retired)</summary>
		public static DcmTag PlacerOrderNumberProcedureRETIRED = new DcmTag(0x0040, 0x1006);

		/// <summary>(0040,1007) VR=SH Filler Order Number / Procedure (Retired)</summary>
		public static DcmTag FillerOrderNumberProcedureRETIRED = new DcmTag(0x0040, 0x1007);

		/// <summary>(0040,1008) VR=LO Confidentiality Code</summary>
		public static DcmTag ConfidentialityCode = new DcmTag(0x0040, 0x1008);

		/// <summary>(0040,1009) VR=SH Reporting Priority</summary>
		public static DcmTag ReportingPriority = new DcmTag(0x0040, 0x1009);

		/// <summary>(0040,100a) VR=SQ Reason for Requested Procedure Code Sequence</summary>
		public static DcmTag ReasonForRequestedProcedureCodeSequence = new DcmTag(0x0040, 0x100a);

		/// <summary>(0040,1010) VR=PN Names of Intended Recipients of Results</summary>
		public static DcmTag NamesOfIntendedRecipientsOfResults = new DcmTag(0x0040, 0x1010);

		/// <summary>(0040,1011) VR=SQ Intended Recipients of Results Identification Sequence</summary>
		public static DcmTag IntendedRecipientsOfResultsIdentificationSequence = new DcmTag(0x0040, 0x1011);

		/// <summary>(0040,1101) VR=SQ Person Identification Code Sequence</summary>
		public static DcmTag PersonIdentificationCodeSequence = new DcmTag(0x0040, 0x1101);

		/// <summary>(0040,1102) VR=ST Person's Address</summary>
		public static DcmTag PersonsAddress = new DcmTag(0x0040, 0x1102);

		/// <summary>(0040,1103) VR=LO Person's Telephone Numbers</summary>
		public static DcmTag PersonsTelephoneNumbers = new DcmTag(0x0040, 0x1103);

		/// <summary>(0040,1400) VR=LT Requested Procedure Comments</summary>
		public static DcmTag RequestedProcedureComments = new DcmTag(0x0040, 0x1400);

		/// <summary>(0040,2001) VR=LO Reason for the Imaging Service Request (Retired)</summary>
		public static DcmTag ReasonForTheImagingServiceRequestRETIRED = new DcmTag(0x0040, 0x2001);

		/// <summary>(0040,2004) VR=DA Issue Date of Imaging Service Request</summary>
		public static DcmTag IssueDateOfImagingServiceRequest = new DcmTag(0x0040, 0x2004);

		/// <summary>(0040,2005) VR=TM Issue Time of Imaging Service Request</summary>
		public static DcmTag IssueTimeOfImagingServiceRequest = new DcmTag(0x0040, 0x2005);

		/// <summary>(0040,2006) VR=SH Placer Order Number / Imaging Service Request (Retired)</summary>
		public static DcmTag PlacerOrderNumberImagingServiceRequestRETIRED = new DcmTag(0x0040, 0x2006);

		/// <summary>(0040,2007) VR=SH Filler Order Number / Imaging Service Request (Retired)</summary>
		public static DcmTag FillerOrderNumberImagingServiceRequestRETIRED = new DcmTag(0x0040, 0x2007);

		/// <summary>(0040,2008) VR=PN Order Entered By</summary>
		public static DcmTag OrderEnteredBy = new DcmTag(0x0040, 0x2008);

		/// <summary>(0040,2009) VR=SH Order Enterer's Location</summary>
		public static DcmTag OrderEnterersLocation = new DcmTag(0x0040, 0x2009);

		/// <summary>(0040,2010) VR=SH Order Callback Phone Number</summary>
		public static DcmTag OrderCallbackPhoneNumber = new DcmTag(0x0040, 0x2010);

		/// <summary>(0040,2016) VR=LO Placer Order Number / Imaging Service Request</summary>
		public static DcmTag PlacerOrderNumberImagingServiceRequest = new DcmTag(0x0040, 0x2016);

		/// <summary>(0040,2017) VR=LO Filler Order Number / Imaging Service Request</summary>
		public static DcmTag FillerOrderNumberImagingServiceRequest = new DcmTag(0x0040, 0x2017);

		/// <summary>(0040,2400) VR=LT Imaging Service Request Comments</summary>
		public static DcmTag ImagingServiceRequestComments = new DcmTag(0x0040, 0x2400);

		/// <summary>(0040,3001) VR=LO Confidentiality Constraint on Patient Data Description</summary>
		public static DcmTag ConfidentialityConstraintOnPatientDataDescription = new DcmTag(0x0040, 0x3001);

		/// <summary>(0040,4001) VR=CS General Purpose Scheduled Procedure Step Status</summary>
		public static DcmTag GeneralPurposeScheduledProcedureStepStatus = new DcmTag(0x0040, 0x4001);

		/// <summary>(0040,4002) VR=CS General Purpose Performed Procedure Step Status</summary>
		public static DcmTag GeneralPurposePerformedProcedureStepStatus = new DcmTag(0x0040, 0x4002);

		/// <summary>(0040,4003) VR=CS General Purpose Scheduled Procedure Step Priority</summary>
		public static DcmTag GeneralPurposeScheduledProcedureStepPriority = new DcmTag(0x0040, 0x4003);

		/// <summary>(0040,4004) VR=SQ Scheduled Processing Applications Code Sequence</summary>
		public static DcmTag ScheduledProcessingApplicationsCodeSequence = new DcmTag(0x0040, 0x4004);

		/// <summary>(0040,4005) VR=DT Scheduled Procedure Step Start Date and Time</summary>
		public static DcmTag ScheduledProcedureStepStartDateAndTime = new DcmTag(0x0040, 0x4005);

		/// <summary>(0040,4006) VR=CS Multiple Copies Flag</summary>
		public static DcmTag MultipleCopiesFlag = new DcmTag(0x0040, 0x4006);

		/// <summary>(0040,4007) VR=SQ Performed Processing Applications Code Sequence</summary>
		public static DcmTag PerformedProcessingApplicationsCodeSequence = new DcmTag(0x0040, 0x4007);

		/// <summary>(0040,4009) VR=SQ Human Performer Code Sequence</summary>
		public static DcmTag HumanPerformerCodeSequence = new DcmTag(0x0040, 0x4009);

		/// <summary>(0040,4010) VR=DT Scheduled Procedure Step Modification Date and Time</summary>
		public static DcmTag ScheduledProcedureStepModificationDateAndTime = new DcmTag(0x0040, 0x4010);

		/// <summary>(0040,4011) VR=DT Expected Completion Date and Time</summary>
		public static DcmTag ExpectedCompletionDateAndTime = new DcmTag(0x0040, 0x4011);

		/// <summary>(0040,4015) VR=SQ Resulting General Purpose Performed Procedure Steps Sequence</summary>
		public static DcmTag ResultingGeneralPurposePerformedProcedureStepsSequence = new DcmTag(0x0040, 0x4015);

		/// <summary>(0040,4016) VR=SQ Referenced General Purpose Scheduled Procedure Step Sequence</summary>
		public static DcmTag ReferencedGeneralPurposeScheduledProcedureStepSequence = new DcmTag(0x0040, 0x4016);

		/// <summary>(0040,4018) VR=SQ Scheduled Workitem Code Sequence</summary>
		public static DcmTag ScheduledWorkitemCodeSequence = new DcmTag(0x0040, 0x4018);

		/// <summary>(0040,4019) VR=SQ Performed Workitem Code Sequence</summary>
		public static DcmTag PerformedWorkitemCodeSequence = new DcmTag(0x0040, 0x4019);

		/// <summary>(0040,4020) VR=CS Input Availability Flag</summary>
		public static DcmTag InputAvailabilityFlag = new DcmTag(0x0040, 0x4020);

		/// <summary>(0040,4021) VR=SQ Input Information Sequence</summary>
		public static DcmTag InputInformationSequence = new DcmTag(0x0040, 0x4021);

		/// <summary>(0040,4022) VR=SQ Relevant Information Sequence</summary>
		public static DcmTag RelevantInformationSequence = new DcmTag(0x0040, 0x4022);

		/// <summary>(0040,4023) VR=UI Referenced General Purpose Scheduled Procedure Step Transaction UID</summary>
		public static DcmTag ReferencedGeneralPurposeScheduledProcedureStepTransactionUID = new DcmTag(0x0040, 0x4023);

		/// <summary>(0040,4025) VR=SQ Scheduled Station Name Code Sequence</summary>
		public static DcmTag ScheduledStationNameCodeSequence = new DcmTag(0x0040, 0x4025);

		/// <summary>(0040,4026) VR=SQ Scheduled Station Class Code Sequence</summary>
		public static DcmTag ScheduledStationClassCodeSequence = new DcmTag(0x0040, 0x4026);

		/// <summary>(0040,4027) VR=SQ Scheduled Station Geographic Location Code Sequence</summary>
		public static DcmTag ScheduledStationGeographicLocationCodeSequence = new DcmTag(0x0040, 0x4027);

		/// <summary>(0040,4028) VR=SQ Performed Station Name Code Sequence</summary>
		public static DcmTag PerformedStationNameCodeSequence = new DcmTag(0x0040, 0x4028);

		/// <summary>(0040,4029) VR=SQ Performed Station Class Code Sequence</summary>
		public static DcmTag PerformedStationClassCodeSequence = new DcmTag(0x0040, 0x4029);

		/// <summary>(0040,4030) VR=SQ Performed Station Geographic Location Code Sequence</summary>
		public static DcmTag PerformedStationGeographicLocationCodeSequence = new DcmTag(0x0040, 0x4030);

		/// <summary>(0040,4031) VR=SQ Requested Subsequent Workitem Code Sequence</summary>
		public static DcmTag RequestedSubsequentWorkitemCodeSequence = new DcmTag(0x0040, 0x4031);

		/// <summary>(0040,4032) VR=SQ Non-DICOM Output Code Sequence</summary>
		public static DcmTag NonDICOMOutputCodeSequence = new DcmTag(0x0040, 0x4032);

		/// <summary>(0040,4033) VR=SQ Output Information Sequence</summary>
		public static DcmTag OutputInformationSequence = new DcmTag(0x0040, 0x4033);

		/// <summary>(0040,4034) VR=SQ Scheduled Human Performers Sequence</summary>
		public static DcmTag ScheduledHumanPerformersSequence = new DcmTag(0x0040, 0x4034);

		/// <summary>(0040,4035) VR=SQ Actual Human Performers Sequence</summary>
		public static DcmTag ActualHumanPerformersSequence = new DcmTag(0x0040, 0x4035);

		/// <summary>(0040,4036) VR=LO Human Performer's Organization</summary>
		public static DcmTag HumanPerformersOrganization = new DcmTag(0x0040, 0x4036);

		/// <summary>(0040,4037) VR=PN Human Performer's Name</summary>
		public static DcmTag HumanPerformersName = new DcmTag(0x0040, 0x4037);

		/// <summary>(0040,8302) VR=DS Entrance Dose in mGy</summary>
		public static DcmTag EntranceDoseInMGy = new DcmTag(0x0040, 0x8302);

		/// <summary>(0040,9094) VR=SQ Referenced Image Real World Value Mapping Sequence</summary>
		public static DcmTag ReferencedImageRealWorldValueMappingSequence = new DcmTag(0x0040, 0x9094);

		/// <summary>(0040,9096) VR=SQ Real World Value Mapping Sequence</summary>
		public static DcmTag RealWorldValueMappingSequence = new DcmTag(0x0040, 0x9096);

		/// <summary>(0040,9098) VR=SQ Pixel Value Mapping Code Sequence</summary>
		public static DcmTag PixelValueMappingCodeSequence = new DcmTag(0x0040, 0x9098);

		/// <summary>(0040,9210) VR=SH LUT Label</summary>
		public static DcmTag LUTLabel = new DcmTag(0x0040, 0x9210);

		/// <summary>(0040,9211) VR=US Real World Value Last Value Mapped</summary>
		public static DcmTag RealWorldValueLastValueMapped = new DcmTag(0x0040, 0x9211);

		/// <summary>(0040,9212) VR=FD Real World Value LUT Data</summary>
		public static DcmTag RealWorldValueLUTData = new DcmTag(0x0040, 0x9212);

		/// <summary>(0040,9216) VR=US Real World Value First Value Mapped</summary>
		public static DcmTag RealWorldValueFirstValueMapped = new DcmTag(0x0040, 0x9216);

		/// <summary>(0040,9224) VR=FD Real World Value Intercept</summary>
		public static DcmTag RealWorldValueIntercept = new DcmTag(0x0040, 0x9224);

		/// <summary>(0040,9225) VR=FD Real World Value Slope</summary>
		public static DcmTag RealWorldValueSlope = new DcmTag(0x0040, 0x9225);

		/// <summary>(0040,a010) VR=CS Relationship Type</summary>
		public static DcmTag RelationshipType = new DcmTag(0x0040, 0xa010);

		/// <summary>(0040,a027) VR=LO Verifying Organization</summary>
		public static DcmTag VerifyingOrganization = new DcmTag(0x0040, 0xa027);

		/// <summary>(0040,a030) VR=DT Verification Date Time</summary>
		public static DcmTag VerificationDateTime = new DcmTag(0x0040, 0xa030);

		/// <summary>(0040,a032) VR=DT Observation Date Time</summary>
		public static DcmTag ObservationDateTime = new DcmTag(0x0040, 0xa032);

		/// <summary>(0040,a040) VR=CS Value Type</summary>
		public static DcmTag ValueType = new DcmTag(0x0040, 0xa040);

		/// <summary>(0040,a043) VR=SQ Concept Name Code Sequence</summary>
		public static DcmTag ConceptNameCodeSequence = new DcmTag(0x0040, 0xa043);

		/// <summary>(0040,a050) VR=CS Continuity Of Content</summary>
		public static DcmTag ContinuityOfContent = new DcmTag(0x0040, 0xa050);

		/// <summary>(0040,a073) VR=SQ Verifying Observer Sequence</summary>
		public static DcmTag VerifyingObserverSequence = new DcmTag(0x0040, 0xa073);

		/// <summary>(0040,a075) VR=PN Verifying Observer Name</summary>
		public static DcmTag VerifyingObserverName = new DcmTag(0x0040, 0xa075);

		/// <summary>(0040,a078) VR=SQ Author Observer Sequence</summary>
		public static DcmTag AuthorObserverSequence = new DcmTag(0x0040, 0xa078);

		/// <summary>(0040,a07a) VR=SQ Participant Sequence</summary>
		public static DcmTag ParticipantSequence = new DcmTag(0x0040, 0xa07a);

		/// <summary>(0040,a07c) VR=SQ Custodial Organization Sequence</summary>
		public static DcmTag CustodialOrganizationSequence = new DcmTag(0x0040, 0xa07c);

		/// <summary>(0040,a080) VR=CS Participation Type</summary>
		public static DcmTag ParticipationType = new DcmTag(0x0040, 0xa080);

		/// <summary>(0040,a082) VR=DT Participation Datetime</summary>
		public static DcmTag ParticipationDatetime = new DcmTag(0x0040, 0xa082);

		/// <summary>(0040,a084) VR=CS Observer Type</summary>
		public static DcmTag ObserverType = new DcmTag(0x0040, 0xa084);

		/// <summary>(0040,a088) VR=SQ Verifying Observer Identification Code Sequence</summary>
		public static DcmTag VerifyingObserverIdentificationCodeSequence = new DcmTag(0x0040, 0xa088);

		/// <summary>(0040,a090) VR=SQ Equivalent CDA Document Sequence (Retired)</summary>
		public static DcmTag EquivalentCDADocumentSequenceRETIRED = new DcmTag(0x0040, 0xa090);

		/// <summary>(0040,a0b0) VR=US Referenced Waveform Channels</summary>
		public static DcmTag ReferencedWaveformChannels = new DcmTag(0x0040, 0xa0b0);

		/// <summary>(0040,a120) VR=DT DateTime</summary>
		public static DcmTag DateTime = new DcmTag(0x0040, 0xa120);

		/// <summary>(0040,a121) VR=DA Date</summary>
		public static DcmTag Date = new DcmTag(0x0040, 0xa121);

		/// <summary>(0040,a122) VR=TM Time</summary>
		public static DcmTag Time = new DcmTag(0x0040, 0xa122);

		/// <summary>(0040,a123) VR=PN Person Name</summary>
		public static DcmTag PersonName = new DcmTag(0x0040, 0xa123);

		/// <summary>(0040,a124) VR=UI UID</summary>
		public static DcmTag UID = new DcmTag(0x0040, 0xa124);

		/// <summary>(0040,a130) VR=CS Temporal Range Type</summary>
		public static DcmTag TemporalRangeType = new DcmTag(0x0040, 0xa130);

		/// <summary>(0040,a132) VR=UL Referenced Sample Positions</summary>
		public static DcmTag ReferencedSamplePositions = new DcmTag(0x0040, 0xa132);

		/// <summary>(0040,a136) VR=US Referenced Frame Numbers</summary>
		public static DcmTag ReferencedFrameNumbers = new DcmTag(0x0040, 0xa136);

		/// <summary>(0040,a138) VR=DS Referenced Time Offsets</summary>
		public static DcmTag ReferencedTimeOffsets = new DcmTag(0x0040, 0xa138);

		/// <summary>(0040,a13a) VR=DT Referenced Datetime</summary>
		public static DcmTag ReferencedDatetime = new DcmTag(0x0040, 0xa13a);

		/// <summary>(0040,a160) VR=UT Text Value</summary>
		public static DcmTag TextValue = new DcmTag(0x0040, 0xa160);

		/// <summary>(0040,a168) VR=SQ Concept Code Sequence</summary>
		public static DcmTag ConceptCodeSequence = new DcmTag(0x0040, 0xa168);

		/// <summary>(0040,a170) VR=SQ Purpose of Reference Code Sequence</summary>
		public static DcmTag PurposeOfReferenceCodeSequence = new DcmTag(0x0040, 0xa170);

		/// <summary>(0040,a180) VR=US Annotation Group Number</summary>
		public static DcmTag AnnotationGroupNumber = new DcmTag(0x0040, 0xa180);

		/// <summary>(0040,a195) VR=SQ Modifier Code Sequence</summary>
		public static DcmTag ModifierCodeSequence = new DcmTag(0x0040, 0xa195);

		/// <summary>(0040,a300) VR=SQ Measured Value Sequence</summary>
		public static DcmTag MeasuredValueSequence = new DcmTag(0x0040, 0xa300);

		/// <summary>(0040,a301) VR=SQ Numeric Value Qualifier Code Sequence</summary>
		public static DcmTag NumericValueQualifierCodeSequence = new DcmTag(0x0040, 0xa301);

		/// <summary>(0040,a30a) VR=DS Numeric Value</summary>
		public static DcmTag NumericValue = new DcmTag(0x0040, 0xa30a);

		/// <summary>(0040,a360) VR=SQ Predecessor Documents Sequence</summary>
		public static DcmTag PredecessorDocumentsSequence = new DcmTag(0x0040, 0xa360);

		/// <summary>(0040,a370) VR=SQ Referenced Request Sequence</summary>
		public static DcmTag ReferencedRequestSequence = new DcmTag(0x0040, 0xa370);

		/// <summary>(0040,a372) VR=SQ Performed Procedure Code Sequence</summary>
		public static DcmTag PerformedProcedureCodeSequence = new DcmTag(0x0040, 0xa372);

		/// <summary>(0040,a375) VR=SQ Current Requested Procedure Evidence Sequence</summary>
		public static DcmTag CurrentRequestedProcedureEvidenceSequence = new DcmTag(0x0040, 0xa375);

		/// <summary>(0040,a385) VR=SQ Pertinent Other Evidence Sequence</summary>
		public static DcmTag PertinentOtherEvidenceSequence = new DcmTag(0x0040, 0xa385);

		/// <summary>(0040,a390) VR=SQ HL7 Structured Document Reference Sequence</summary>
		public static DcmTag HL7StructuredDocumentReferenceSequence = new DcmTag(0x0040, 0xa390);

		/// <summary>(0040,a491) VR=CS Completion Flag</summary>
		public static DcmTag CompletionFlag = new DcmTag(0x0040, 0xa491);

		/// <summary>(0040,a492) VR=LO Completion Flag Description</summary>
		public static DcmTag CompletionFlagDescription = new DcmTag(0x0040, 0xa492);

		/// <summary>(0040,a493) VR=CS Verification Flag</summary>
		public static DcmTag VerificationFlag = new DcmTag(0x0040, 0xa493);

		/// <summary>(0040,a504) VR=SQ Content Template Sequence</summary>
		public static DcmTag ContentTemplateSequence = new DcmTag(0x0040, 0xa504);

		/// <summary>(0040,a525) VR=SQ Identical Documents Sequence</summary>
		public static DcmTag IdenticalDocumentsSequence = new DcmTag(0x0040, 0xa525);

		/// <summary>(0040,a730) VR=SQ Content Sequence</summary>
		public static DcmTag ContentSequence = new DcmTag(0x0040, 0xa730);

		/// <summary>(0040,b020) VR=SQ Annotation Sequence</summary>
		public static DcmTag AnnotationSequence = new DcmTag(0x0040, 0xb020);

		/// <summary>(0040,db00) VR=CS Template Identifier</summary>
		public static DcmTag TemplateIdentifier = new DcmTag(0x0040, 0xdb00);

		/// <summary>(0040,db06) VR=DT Template Version (Retired)</summary>
		public static DcmTag TemplateVersionRETIRED = new DcmTag(0x0040, 0xdb06);

		/// <summary>(0040,db07) VR=DT Template Local Version (Retired)</summary>
		public static DcmTag TemplateLocalVersionRETIRED = new DcmTag(0x0040, 0xdb07);

		/// <summary>(0040,db0b) VR=CS Template Extension Flag (Retired)</summary>
		public static DcmTag TemplateExtensionFlagRETIRED = new DcmTag(0x0040, 0xdb0b);

		/// <summary>(0040,db0c) VR=UI Template Extension Organization UID (Retired)</summary>
		public static DcmTag TemplateExtensionOrganizationUIDRETIRED = new DcmTag(0x0040, 0xdb0c);

		/// <summary>(0040,db0d) VR=UI Template Extension Creator UID (Retired)</summary>
		public static DcmTag TemplateExtensionCreatorUIDRETIRED = new DcmTag(0x0040, 0xdb0d);

		/// <summary>(0040,db73) VR=UL Referenced Content Item Identifier</summary>
		public static DcmTag ReferencedContentItemIdentifier = new DcmTag(0x0040, 0xdb73);

		/// <summary>(0040,e001) VR=ST HL7 Instance Identifier</summary>
		public static DcmTag HL7InstanceIdentifier = new DcmTag(0x0040, 0xe001);

		/// <summary>(0040,e004) VR=DT HL7 Document Effective Time</summary>
		public static DcmTag HL7DocumentEffectiveTime = new DcmTag(0x0040, 0xe004);

		/// <summary>(0040,e006) VR=SQ HL7 Document Type Code Sequence</summary>
		public static DcmTag HL7DocumentTypeCodeSequence = new DcmTag(0x0040, 0xe006);

		/// <summary>(0040,e010) VR=UT Retrieve URI</summary>
		public static DcmTag RetrieveURI = new DcmTag(0x0040, 0xe010);

		/// <summary>(0042,0010) VR=ST Document Title</summary>
		public static DcmTag DocumentTitle = new DcmTag(0x0042, 0x0010);

		/// <summary>(0042,0011) VR=OB Encapsulated Document</summary>
		public static DcmTag EncapsulatedDocument = new DcmTag(0x0042, 0x0011);

		/// <summary>(0042,0012) VR=LO MIME Type of Encapsulated Document</summary>
		public static DcmTag MIMETypeOfEncapsulatedDocument = new DcmTag(0x0042, 0x0012);

		/// <summary>(0042,0013) VR=SQ Source Instance Sequence</summary>
		public static DcmTag SourceInstanceSequence = new DcmTag(0x0042, 0x0013);

		/// <summary>(0050,0004) VR=CS Calibration Image</summary>
		public static DcmTag CalibrationImage = new DcmTag(0x0050, 0x0004);

		/// <summary>(0050,0010) VR=SQ Device Sequence</summary>
		public static DcmTag DeviceSequence = new DcmTag(0x0050, 0x0010);

		/// <summary>(0050,0014) VR=DS Device Length</summary>
		public static DcmTag DeviceLength = new DcmTag(0x0050, 0x0014);

		/// <summary>(0050,0016) VR=DS Device Diameter</summary>
		public static DcmTag DeviceDiameter = new DcmTag(0x0050, 0x0016);

		/// <summary>(0050,0017) VR=CS Device Diameter Units</summary>
		public static DcmTag DeviceDiameterUnits = new DcmTag(0x0050, 0x0017);

		/// <summary>(0050,0018) VR=DS Device Volume</summary>
		public static DcmTag DeviceVolume = new DcmTag(0x0050, 0x0018);

		/// <summary>(0050,0019) VR=DS Intermarker Distance</summary>
		public static DcmTag IntermarkerDistance = new DcmTag(0x0050, 0x0019);

		/// <summary>(0050,0020) VR=LO Device Description</summary>
		public static DcmTag DeviceDescription = new DcmTag(0x0050, 0x0020);

		/// <summary>(0054,0010) VR=US Energy Window Vector</summary>
		public static DcmTag EnergyWindowVector = new DcmTag(0x0054, 0x0010);

		/// <summary>(0054,0011) VR=US Number of Energy Windows</summary>
		public static DcmTag NumberOfEnergyWindows = new DcmTag(0x0054, 0x0011);

		/// <summary>(0054,0012) VR=SQ Energy Window Information Sequence</summary>
		public static DcmTag EnergyWindowInformationSequence = new DcmTag(0x0054, 0x0012);

		/// <summary>(0054,0013) VR=SQ Energy Window Range Sequence</summary>
		public static DcmTag EnergyWindowRangeSequence = new DcmTag(0x0054, 0x0013);

		/// <summary>(0054,0014) VR=DS Energy Window Lower Limit</summary>
		public static DcmTag EnergyWindowLowerLimit = new DcmTag(0x0054, 0x0014);

		/// <summary>(0054,0015) VR=DS Energy Window Upper Limit</summary>
		public static DcmTag EnergyWindowUpperLimit = new DcmTag(0x0054, 0x0015);

		/// <summary>(0054,0016) VR=SQ Radiopharmaceutical Information Sequence</summary>
		public static DcmTag RadiopharmaceuticalInformationSequence = new DcmTag(0x0054, 0x0016);

		/// <summary>(0054,0017) VR=IS Residual Syringe Counts</summary>
		public static DcmTag ResidualSyringeCounts = new DcmTag(0x0054, 0x0017);

		/// <summary>(0054,0018) VR=SH Energy Window Name</summary>
		public static DcmTag EnergyWindowName = new DcmTag(0x0054, 0x0018);

		/// <summary>(0054,0020) VR=US Detector Vector</summary>
		public static DcmTag DetectorVector = new DcmTag(0x0054, 0x0020);

		/// <summary>(0054,0021) VR=US Number of Detectors</summary>
		public static DcmTag NumberOfDetectors = new DcmTag(0x0054, 0x0021);

		/// <summary>(0054,0022) VR=SQ Detector Information Sequence</summary>
		public static DcmTag DetectorInformationSequence = new DcmTag(0x0054, 0x0022);

		/// <summary>(0054,0030) VR=US Phase Vector</summary>
		public static DcmTag PhaseVector = new DcmTag(0x0054, 0x0030);

		/// <summary>(0054,0031) VR=US Number of Phases</summary>
		public static DcmTag NumberOfPhases = new DcmTag(0x0054, 0x0031);

		/// <summary>(0054,0032) VR=SQ Phase Information Sequence</summary>
		public static DcmTag PhaseInformationSequence = new DcmTag(0x0054, 0x0032);

		/// <summary>(0054,0033) VR=US Number of Frames in Phase</summary>
		public static DcmTag NumberOfFramesInPhase = new DcmTag(0x0054, 0x0033);

		/// <summary>(0054,0036) VR=IS Phase Delay</summary>
		public static DcmTag PhaseDelay = new DcmTag(0x0054, 0x0036);

		/// <summary>(0054,0038) VR=IS Pause Between Frames</summary>
		public static DcmTag PauseBetweenFrames = new DcmTag(0x0054, 0x0038);

		/// <summary>(0054,0039) VR=CS Phase Description</summary>
		public static DcmTag PhaseDescription = new DcmTag(0x0054, 0x0039);

		/// <summary>(0054,0050) VR=US Rotation Vector</summary>
		public static DcmTag RotationVector = new DcmTag(0x0054, 0x0050);

		/// <summary>(0054,0051) VR=US Number of Rotations</summary>
		public static DcmTag NumberOfRotations = new DcmTag(0x0054, 0x0051);

		/// <summary>(0054,0052) VR=SQ Rotation Information Sequence</summary>
		public static DcmTag RotationInformationSequence = new DcmTag(0x0054, 0x0052);

		/// <summary>(0054,0053) VR=US Number of Frames in Rotation</summary>
		public static DcmTag NumberOfFramesInRotation = new DcmTag(0x0054, 0x0053);

		/// <summary>(0054,0060) VR=US R-R Interval Vector</summary>
		public static DcmTag RRIntervalVector = new DcmTag(0x0054, 0x0060);

		/// <summary>(0054,0061) VR=US Number of R-R Intervals</summary>
		public static DcmTag NumberOfRRIntervals = new DcmTag(0x0054, 0x0061);

		/// <summary>(0054,0062) VR=SQ Gated Information Sequence</summary>
		public static DcmTag GatedInformationSequence = new DcmTag(0x0054, 0x0062);

		/// <summary>(0054,0063) VR=SQ Data Information Sequence</summary>
		public static DcmTag DataInformationSequence = new DcmTag(0x0054, 0x0063);

		/// <summary>(0054,0070) VR=US Time Slot Vector</summary>
		public static DcmTag TimeSlotVector = new DcmTag(0x0054, 0x0070);

		/// <summary>(0054,0071) VR=US Number of Time Slots</summary>
		public static DcmTag NumberOfTimeSlots = new DcmTag(0x0054, 0x0071);

		/// <summary>(0054,0072) VR=SQ Time Slot Information Sequence</summary>
		public static DcmTag TimeSlotInformationSequence = new DcmTag(0x0054, 0x0072);

		/// <summary>(0054,0073) VR=DS Time Slot Time</summary>
		public static DcmTag TimeSlotTime = new DcmTag(0x0054, 0x0073);

		/// <summary>(0054,0080) VR=US Slice Vector</summary>
		public static DcmTag SliceVector = new DcmTag(0x0054, 0x0080);

		/// <summary>(0054,0081) VR=US Number of Slices</summary>
		public static DcmTag NumberOfSlices = new DcmTag(0x0054, 0x0081);

		/// <summary>(0054,0090) VR=US Angular View Vector</summary>
		public static DcmTag AngularViewVector = new DcmTag(0x0054, 0x0090);

		/// <summary>(0054,0100) VR=US Time Slice Vector</summary>
		public static DcmTag TimeSliceVector = new DcmTag(0x0054, 0x0100);

		/// <summary>(0054,0101) VR=US Number of Time Slices</summary>
		public static DcmTag NumberOfTimeSlices = new DcmTag(0x0054, 0x0101);

		/// <summary>(0054,0200) VR=DS Start Angle</summary>
		public static DcmTag StartAngle = new DcmTag(0x0054, 0x0200);

		/// <summary>(0054,0202) VR=CS Type of Detector Motion</summary>
		public static DcmTag TypeOfDetectorMotion = new DcmTag(0x0054, 0x0202);

		/// <summary>(0054,0210) VR=IS Trigger Vector</summary>
		public static DcmTag TriggerVector = new DcmTag(0x0054, 0x0210);

		/// <summary>(0054,0211) VR=US Number of Triggers in Phase</summary>
		public static DcmTag NumberOfTriggersInPhase = new DcmTag(0x0054, 0x0211);

		/// <summary>(0054,0220) VR=SQ View Code Sequence</summary>
		public static DcmTag ViewCodeSequence = new DcmTag(0x0054, 0x0220);

		/// <summary>(0054,0222) VR=SQ View Modifier Code Sequence</summary>
		public static DcmTag ViewModifierCodeSequence = new DcmTag(0x0054, 0x0222);

		/// <summary>(0054,0300) VR=SQ Radionuclide Code Sequence</summary>
		public static DcmTag RadionuclideCodeSequence = new DcmTag(0x0054, 0x0300);

		/// <summary>(0054,0302) VR=SQ Administration Route Code Sequence</summary>
		public static DcmTag AdministrationRouteCodeSequence = new DcmTag(0x0054, 0x0302);

		/// <summary>(0054,0304) VR=SQ Radiopharmaceutical Code Sequence</summary>
		public static DcmTag RadiopharmaceuticalCodeSequence = new DcmTag(0x0054, 0x0304);

		/// <summary>(0054,0306) VR=SQ Calibration Data Sequence</summary>
		public static DcmTag CalibrationDataSequence = new DcmTag(0x0054, 0x0306);

		/// <summary>(0054,0308) VR=US Energy Window Number</summary>
		public static DcmTag EnergyWindowNumber = new DcmTag(0x0054, 0x0308);

		/// <summary>(0054,0400) VR=SH Image ID</summary>
		public static DcmTag ImageID = new DcmTag(0x0054, 0x0400);

		/// <summary>(0054,0410) VR=SQ Patient Orientation Code Sequence</summary>
		public static DcmTag PatientOrientationCodeSequence = new DcmTag(0x0054, 0x0410);

		/// <summary>(0054,0412) VR=SQ Patient Orientation Modifier Code Sequence</summary>
		public static DcmTag PatientOrientationModifierCodeSequence = new DcmTag(0x0054, 0x0412);

		/// <summary>(0054,0414) VR=SQ Patient Gantry Relationship Code Sequence</summary>
		public static DcmTag PatientGantryRelationshipCodeSequence = new DcmTag(0x0054, 0x0414);

		/// <summary>(0054,0500) VR=CS Slice Progression Direction</summary>
		public static DcmTag SliceProgressionDirection = new DcmTag(0x0054, 0x0500);

		/// <summary>(0054,1000) VR=CS Series Type</summary>
		public static DcmTag SeriesType = new DcmTag(0x0054, 0x1000);

		/// <summary>(0054,1001) VR=CS Units</summary>
		public static DcmTag Units = new DcmTag(0x0054, 0x1001);

		/// <summary>(0054,1002) VR=CS Counts Source</summary>
		public static DcmTag CountsSource = new DcmTag(0x0054, 0x1002);

		/// <summary>(0054,1004) VR=CS Reprojection Method</summary>
		public static DcmTag ReprojectionMethod = new DcmTag(0x0054, 0x1004);

		/// <summary>(0054,1100) VR=CS Randoms Correction Method</summary>
		public static DcmTag RandomsCorrectionMethod = new DcmTag(0x0054, 0x1100);

		/// <summary>(0054,1101) VR=LO Attenuation Correction Method</summary>
		public static DcmTag AttenuationCorrectionMethod = new DcmTag(0x0054, 0x1101);

		/// <summary>(0054,1102) VR=CS Decay Correction</summary>
		public static DcmTag DecayCorrection = new DcmTag(0x0054, 0x1102);

		/// <summary>(0054,1103) VR=LO Reconstruction Method</summary>
		public static DcmTag ReconstructionMethod = new DcmTag(0x0054, 0x1103);

		/// <summary>(0054,1104) VR=LO Detector Lines of Response Used</summary>
		public static DcmTag DetectorLinesOfResponseUsed = new DcmTag(0x0054, 0x1104);

		/// <summary>(0054,1105) VR=LO Scatter Correction Method</summary>
		public static DcmTag ScatterCorrectionMethod = new DcmTag(0x0054, 0x1105);

		/// <summary>(0054,1200) VR=DS Axial Acceptance</summary>
		public static DcmTag AxialAcceptance = new DcmTag(0x0054, 0x1200);

		/// <summary>(0054,1201) VR=IS Axial Mash</summary>
		public static DcmTag AxialMash = new DcmTag(0x0054, 0x1201);

		/// <summary>(0054,1202) VR=IS Transverse Mash</summary>
		public static DcmTag TransverseMash = new DcmTag(0x0054, 0x1202);

		/// <summary>(0054,1203) VR=DS Detector Element Size</summary>
		public static DcmTag DetectorElementSize = new DcmTag(0x0054, 0x1203);

		/// <summary>(0054,1210) VR=DS Coincidence Window Width</summary>
		public static DcmTag CoincidenceWindowWidth = new DcmTag(0x0054, 0x1210);

		/// <summary>(0054,1220) VR=CS Secondary Counts Type</summary>
		public static DcmTag SecondaryCountsType = new DcmTag(0x0054, 0x1220);

		/// <summary>(0054,1300) VR=DS Frame Reference Time</summary>
		public static DcmTag FrameReferenceTime = new DcmTag(0x0054, 0x1300);

		/// <summary>(0054,1310) VR=IS Primary (Prompts) Counts Accumulated</summary>
		public static DcmTag PrimaryPromptsCountsAccumulated = new DcmTag(0x0054, 0x1310);

		/// <summary>(0054,1311) VR=IS Secondary Counts Accumulated</summary>
		public static DcmTag SecondaryCountsAccumulated = new DcmTag(0x0054, 0x1311);

		/// <summary>(0054,1320) VR=DS Slice Sensitivity Factor</summary>
		public static DcmTag SliceSensitivityFactor = new DcmTag(0x0054, 0x1320);

		/// <summary>(0054,1321) VR=DS Decay Factor</summary>
		public static DcmTag DecayFactor = new DcmTag(0x0054, 0x1321);

		/// <summary>(0054,1322) VR=DS Dose Calibration Factor</summary>
		public static DcmTag DoseCalibrationFactor = new DcmTag(0x0054, 0x1322);

		/// <summary>(0054,1323) VR=DS Scatter Fraction Factor</summary>
		public static DcmTag ScatterFractionFactor = new DcmTag(0x0054, 0x1323);

		/// <summary>(0054,1324) VR=DS Dead Time Factor</summary>
		public static DcmTag DeadTimeFactor = new DcmTag(0x0054, 0x1324);

		/// <summary>(0054,1330) VR=US Image Index</summary>
		public static DcmTag ImageIndex = new DcmTag(0x0054, 0x1330);

		/// <summary>(0054,1400) VR=CS Counts Included</summary>
		public static DcmTag CountsIncluded = new DcmTag(0x0054, 0x1400);

		/// <summary>(0054,1401) VR=CS Dead Time Correction Flag</summary>
		public static DcmTag DeadTimeCorrectionFlag = new DcmTag(0x0054, 0x1401);

		/// <summary>(0060,3000) VR=SQ Histogram Sequence</summary>
		public static DcmTag HistogramSequence = new DcmTag(0x0060, 0x3000);

		/// <summary>(0060,3002) VR=US Histogram Number of Bins</summary>
		public static DcmTag HistogramNumberOfBins = new DcmTag(0x0060, 0x3002);

		/// <summary>(0060,3004) VR=US Histogram First Bin Value</summary>
		public static DcmTag HistogramFirstBinValue = new DcmTag(0x0060, 0x3004);

		/// <summary>(0060,3006) VR=US Histogram Last Bin Value</summary>
		public static DcmTag HistogramLastBinValue = new DcmTag(0x0060, 0x3006);

		/// <summary>(0060,3008) VR=US Histogram Bin Width</summary>
		public static DcmTag HistogramBinWidth = new DcmTag(0x0060, 0x3008);

		/// <summary>(0060,3010) VR=LO Histogram Explanation</summary>
		public static DcmTag HistogramExplanation = new DcmTag(0x0060, 0x3010);

		/// <summary>(0060,3020) VR=UL Histogram Data</summary>
		public static DcmTag HistogramData = new DcmTag(0x0060, 0x3020);

		/// <summary>(0062,0001) VR=CS Segmentation Type</summary>
		public static DcmTag SegmentationType = new DcmTag(0x0062, 0x0001);

		/// <summary>(0062,0002) VR=SQ Segment Sequence</summary>
		public static DcmTag SegmentSequence = new DcmTag(0x0062, 0x0002);

		/// <summary>(0062,0003) VR=SQ Segmented Property Category Code Sequence</summary>
		public static DcmTag SegmentedPropertyCategoryCodeSequence = new DcmTag(0x0062, 0x0003);

		/// <summary>(0062,0004) VR=US Segment Number</summary>
		public static DcmTag SegmentNumber = new DcmTag(0x0062, 0x0004);

		/// <summary>(0062,0005) VR=LO Segment Label</summary>
		public static DcmTag SegmentLabel = new DcmTag(0x0062, 0x0005);

		/// <summary>(0062,0006) VR=ST Segment Description</summary>
		public static DcmTag SegmentDescription = new DcmTag(0x0062, 0x0006);

		/// <summary>(0062,0008) VR=CS Segment Algorithm Type</summary>
		public static DcmTag SegmentAlgorithmType = new DcmTag(0x0062, 0x0008);

		/// <summary>(0062,0009) VR=LO Segment Algorithm Name</summary>
		public static DcmTag SegmentAlgorithmName = new DcmTag(0x0062, 0x0009);

		/// <summary>(0062,000a) VR=SQ Segment Identification Sequence</summary>
		public static DcmTag SegmentIdentificationSequence = new DcmTag(0x0062, 0x000a);

		/// <summary>(0062,000b) VR=US Referenced Segment Number</summary>
		public static DcmTag ReferencedSegmentNumber = new DcmTag(0x0062, 0x000b);

		/// <summary>(0062,000c) VR=US Recommended Display Grayscale Value</summary>
		public static DcmTag RecommendedDisplayGrayscaleValue = new DcmTag(0x0062, 0x000c);

		/// <summary>(0062,000d) VR=US Recommended Display CIELab Value</summary>
		public static DcmTag RecommendedDisplayCIELabValue = new DcmTag(0x0062, 0x000d);

		/// <summary>(0062,000e) VR=US Maximum Fractional Value</summary>
		public static DcmTag MaximumFractionalValue = new DcmTag(0x0062, 0x000e);

		/// <summary>(0062,000f) VR=SQ Segmented Property Type Code Sequence</summary>
		public static DcmTag SegmentedPropertyTypeCodeSequence = new DcmTag(0x0062, 0x000f);

		/// <summary>(0062,0010) VR=CS Segmentation Fractional Type</summary>
		public static DcmTag SegmentationFractionalType = new DcmTag(0x0062, 0x0010);

		/// <summary>(0064,0002) VR=SQ Deformable Registration Sequence</summary>
		public static DcmTag DeformableRegistrationSequence = new DcmTag(0x0064, 0x0002);

		/// <summary>(0064,0003) VR=UI Source Frame of Reference UID</summary>
		public static DcmTag SourceFrameOfReferenceUID = new DcmTag(0x0064, 0x0003);

		/// <summary>(0064,0005) VR=SQ Deformable Registration Grid Sequence</summary>
		public static DcmTag DeformableRegistrationGridSequence = new DcmTag(0x0064, 0x0005);

		/// <summary>(0064,0007) VR=UL Grid Dimensions</summary>
		public static DcmTag GridDimensions = new DcmTag(0x0064, 0x0007);

		/// <summary>(0064,0008) VR=FD Grid Resolution</summary>
		public static DcmTag GridResolution = new DcmTag(0x0064, 0x0008);

		/// <summary>(0064,0009) VR=OF Vector Grid Data</summary>
		public static DcmTag VectorGridData = new DcmTag(0x0064, 0x0009);

		/// <summary>(0064,000f) VR=SQ Pre Deformation Matrix Registration Sequence</summary>
		public static DcmTag PreDeformationMatrixRegistrationSequence = new DcmTag(0x0064, 0x000f);

		/// <summary>(0064,0010) VR=SQ Post Deformation Matrix Registration Sequence</summary>
		public static DcmTag PostDeformationMatrixRegistrationSequence = new DcmTag(0x0064, 0x0010);

		/// <summary>(0070,0001) VR=SQ Graphic Annotation Sequence</summary>
		public static DcmTag GraphicAnnotationSequence = new DcmTag(0x0070, 0x0001);

		/// <summary>(0070,0002) VR=CS Graphic Layer</summary>
		public static DcmTag GraphicLayer = new DcmTag(0x0070, 0x0002);

		/// <summary>(0070,0003) VR=CS Bounding Box Annotation Units</summary>
		public static DcmTag BoundingBoxAnnotationUnits = new DcmTag(0x0070, 0x0003);

		/// <summary>(0070,0004) VR=CS Anchor Point Annotation Units</summary>
		public static DcmTag AnchorPointAnnotationUnits = new DcmTag(0x0070, 0x0004);

		/// <summary>(0070,0005) VR=CS Graphic Annotation Units</summary>
		public static DcmTag GraphicAnnotationUnits = new DcmTag(0x0070, 0x0005);

		/// <summary>(0070,0006) VR=ST Unformatted Text Value</summary>
		public static DcmTag UnformattedTextValue = new DcmTag(0x0070, 0x0006);

		/// <summary>(0070,0008) VR=SQ Text Object Sequence</summary>
		public static DcmTag TextObjectSequence = new DcmTag(0x0070, 0x0008);

		/// <summary>(0070,0009) VR=SQ Graphic Object Sequence</summary>
		public static DcmTag GraphicObjectSequence = new DcmTag(0x0070, 0x0009);

		/// <summary>(0070,0010) VR=FL Bounding Box Top Left Hand Corner</summary>
		public static DcmTag BoundingBoxTopLeftHandCorner = new DcmTag(0x0070, 0x0010);

		/// <summary>(0070,0011) VR=FL Bounding Box Bottom Right Hand Corner</summary>
		public static DcmTag BoundingBoxBottomRightHandCorner = new DcmTag(0x0070, 0x0011);

		/// <summary>(0070,0012) VR=CS Bounding Box Text Horizontal Justification</summary>
		public static DcmTag BoundingBoxTextHorizontalJustification = new DcmTag(0x0070, 0x0012);

		/// <summary>(0070,0014) VR=FL Anchor Point</summary>
		public static DcmTag AnchorPoint = new DcmTag(0x0070, 0x0014);

		/// <summary>(0070,0015) VR=CS Anchor Point Visibility</summary>
		public static DcmTag AnchorPointVisibility = new DcmTag(0x0070, 0x0015);

		/// <summary>(0070,0020) VR=US Graphic Dimensions</summary>
		public static DcmTag GraphicDimensions = new DcmTag(0x0070, 0x0020);

		/// <summary>(0070,0021) VR=US Number of Graphic Points</summary>
		public static DcmTag NumberOfGraphicPoints = new DcmTag(0x0070, 0x0021);

		/// <summary>(0070,0022) VR=FL Graphic Data</summary>
		public static DcmTag GraphicData = new DcmTag(0x0070, 0x0022);

		/// <summary>(0070,0023) VR=CS Graphic Type</summary>
		public static DcmTag GraphicType = new DcmTag(0x0070, 0x0023);

		/// <summary>(0070,0024) VR=CS Graphic Filled</summary>
		public static DcmTag GraphicFilled = new DcmTag(0x0070, 0x0024);

		/// <summary>(0070,0041) VR=CS Image Horizontal Flip</summary>
		public static DcmTag ImageHorizontalFlip = new DcmTag(0x0070, 0x0041);

		/// <summary>(0070,0042) VR=US Image Rotation</summary>
		public static DcmTag ImageRotation = new DcmTag(0x0070, 0x0042);

		/// <summary>(0070,0052) VR=SL Displayed Area Top Left Hand Corner</summary>
		public static DcmTag DisplayedAreaTopLeftHandCorner = new DcmTag(0x0070, 0x0052);

		/// <summary>(0070,0053) VR=SL Displayed Area Bottom Right Hand Corner</summary>
		public static DcmTag DisplayedAreaBottomRightHandCorner = new DcmTag(0x0070, 0x0053);

		/// <summary>(0070,005a) VR=SQ Displayed Area Selection Sequence</summary>
		public static DcmTag DisplayedAreaSelectionSequence = new DcmTag(0x0070, 0x005a);

		/// <summary>(0070,0060) VR=SQ Graphic Layer Sequence</summary>
		public static DcmTag GraphicLayerSequence = new DcmTag(0x0070, 0x0060);

		/// <summary>(0070,0062) VR=IS Graphic Layer Order</summary>
		public static DcmTag GraphicLayerOrder = new DcmTag(0x0070, 0x0062);

		/// <summary>(0070,0066) VR=US Graphic Layer Recommended Display Grayscale Value</summary>
		public static DcmTag GraphicLayerRecommendedDisplayGrayscaleValue = new DcmTag(0x0070, 0x0066);

		/// <summary>(0070,0067) VR=US Graphic Layer Recommended Display RGB Value (Retired)</summary>
		public static DcmTag GraphicLayerRecommendedDisplayRGBValueRETIRED = new DcmTag(0x0070, 0x0067);

		/// <summary>(0070,0068) VR=LO Graphic Layer Description</summary>
		public static DcmTag GraphicLayerDescription = new DcmTag(0x0070, 0x0068);

		/// <summary>(0070,0080) VR=CS Content Label</summary>
		public static DcmTag ContentLabel = new DcmTag(0x0070, 0x0080);

		/// <summary>(0070,0081) VR=LO Content Description</summary>
		public static DcmTag ContentDescription = new DcmTag(0x0070, 0x0081);

		/// <summary>(0070,0082) VR=DA Presentation Creation Date</summary>
		public static DcmTag PresentationCreationDate = new DcmTag(0x0070, 0x0082);

		/// <summary>(0070,0083) VR=TM Presentation Creation Time</summary>
		public static DcmTag PresentationCreationTime = new DcmTag(0x0070, 0x0083);

		/// <summary>(0070,0084) VR=PN Content Creator's Name</summary>
		public static DcmTag ContentCreatorsName = new DcmTag(0x0070, 0x0084);

		/// <summary>(0070,0086) VR=SQ Content Creator's Identification Code Sequence</summary>
		public static DcmTag ContentCreatorsIdentificationCodeSequence = new DcmTag(0x0070, 0x0086);

		/// <summary>(0070,0100) VR=CS Presentation Size Mode</summary>
		public static DcmTag PresentationSizeMode = new DcmTag(0x0070, 0x0100);

		/// <summary>(0070,0101) VR=DS Presentation Pixel Spacing</summary>
		public static DcmTag PresentationPixelSpacing = new DcmTag(0x0070, 0x0101);

		/// <summary>(0070,0102) VR=IS Presentation Pixel Aspect Ratio</summary>
		public static DcmTag PresentationPixelAspectRatio = new DcmTag(0x0070, 0x0102);

		/// <summary>(0070,0103) VR=FL Presentation Pixel Magnification Ratio</summary>
		public static DcmTag PresentationPixelMagnificationRatio = new DcmTag(0x0070, 0x0103);

		/// <summary>(0070,0306) VR=CS Shape Type</summary>
		public static DcmTag ShapeType = new DcmTag(0x0070, 0x0306);

		/// <summary>(0070,0308) VR=SQ Registration Sequence</summary>
		public static DcmTag RegistrationSequence = new DcmTag(0x0070, 0x0308);

		/// <summary>(0070,0309) VR=SQ Matrix Registration Sequence</summary>
		public static DcmTag MatrixRegistrationSequence = new DcmTag(0x0070, 0x0309);

		/// <summary>(0070,030a) VR=SQ Matrix Sequence</summary>
		public static DcmTag MatrixSequence = new DcmTag(0x0070, 0x030a);

		/// <summary>(0070,030c) VR=CS Frame of Reference Transformation Matrix Type</summary>
		public static DcmTag FrameOfReferenceTransformationMatrixType = new DcmTag(0x0070, 0x030c);

		/// <summary>(0070,030d) VR=SQ Registration Type Code Sequence</summary>
		public static DcmTag RegistrationTypeCodeSequence = new DcmTag(0x0070, 0x030d);

		/// <summary>(0070,030f) VR=ST Fiducial Description</summary>
		public static DcmTag FiducialDescription = new DcmTag(0x0070, 0x030f);

		/// <summary>(0070,0310) VR=SH Fiducial Identifier</summary>
		public static DcmTag FiducialIdentifier = new DcmTag(0x0070, 0x0310);

		/// <summary>(0070,0311) VR=SQ Fiducial Identifier Code Sequence</summary>
		public static DcmTag FiducialIdentifierCodeSequence = new DcmTag(0x0070, 0x0311);

		/// <summary>(0070,0312) VR=FD Contour Uncertainty Radius</summary>
		public static DcmTag ContourUncertaintyRadius = new DcmTag(0x0070, 0x0312);

		/// <summary>(0070,0314) VR=SQ Used Fiducials Sequence</summary>
		public static DcmTag UsedFiducialsSequence = new DcmTag(0x0070, 0x0314);

		/// <summary>(0070,0318) VR=SQ Graphic Coordinates Data Sequence</summary>
		public static DcmTag GraphicCoordinatesDataSequence = new DcmTag(0x0070, 0x0318);

		/// <summary>(0070,031a) VR=UI Fiducial UID</summary>
		public static DcmTag FiducialUID = new DcmTag(0x0070, 0x031a);

		/// <summary>(0070,031c) VR=SQ Fiducial Set Sequence</summary>
		public static DcmTag FiducialSetSequence = new DcmTag(0x0070, 0x031c);

		/// <summary>(0070,031e) VR=SQ Fiducial Sequence</summary>
		public static DcmTag FiducialSequence = new DcmTag(0x0070, 0x031e);

		/// <summary>(0070,0401) VR=US Graphic Layer Recommended Display CIELab Value</summary>
		public static DcmTag GraphicLayerRecommendedDisplayCIELabValue = new DcmTag(0x0070, 0x0401);

		/// <summary>(0070,0402) VR=SQ Blending Sequence</summary>
		public static DcmTag BlendingSequence = new DcmTag(0x0070, 0x0402);

		/// <summary>(0070,0403) VR=FL Relative Opacity</summary>
		public static DcmTag RelativeOpacity = new DcmTag(0x0070, 0x0403);

		/// <summary>(0070,0404) VR=SQ Referenced Spatial Registration Sequence</summary>
		public static DcmTag ReferencedSpatialRegistrationSequence = new DcmTag(0x0070, 0x0404);

		/// <summary>(0070,0405) VR=CS Blending Position</summary>
		public static DcmTag BlendingPosition = new DcmTag(0x0070, 0x0405);

		/// <summary>(0072,0002) VR=SH Hanging Protocol Name</summary>
		public static DcmTag HangingProtocolName = new DcmTag(0x0072, 0x0002);

		/// <summary>(0072,0004) VR=LO Hanging Protocol Description</summary>
		public static DcmTag HangingProtocolDescription = new DcmTag(0x0072, 0x0004);

		/// <summary>(0072,0006) VR=CS Hanging Protocol Level</summary>
		public static DcmTag HangingProtocolLevel = new DcmTag(0x0072, 0x0006);

		/// <summary>(0072,0008) VR=LO Hanging Protocol Creator</summary>
		public static DcmTag HangingProtocolCreator = new DcmTag(0x0072, 0x0008);

		/// <summary>(0072,000a) VR=DT Hanging Protocol Creation Datetime</summary>
		public static DcmTag HangingProtocolCreationDatetime = new DcmTag(0x0072, 0x000a);

		/// <summary>(0072,000c) VR=SQ Hanging Protocol Definition Sequence</summary>
		public static DcmTag HangingProtocolDefinitionSequence = new DcmTag(0x0072, 0x000c);

		/// <summary>(0072,000e) VR=SQ Hanging Protocol User Identification Code Sequence</summary>
		public static DcmTag HangingProtocolUserIdentificationCodeSequence = new DcmTag(0x0072, 0x000e);

		/// <summary>(0072,0010) VR=LO Hanging Protocol User Group Name</summary>
		public static DcmTag HangingProtocolUserGroupName = new DcmTag(0x0072, 0x0010);

		/// <summary>(0072,0012) VR=SQ Source Hanging Protocol Sequence</summary>
		public static DcmTag SourceHangingProtocolSequence = new DcmTag(0x0072, 0x0012);

		/// <summary>(0072,0014) VR=US Number of Priors Referenced</summary>
		public static DcmTag NumberOfPriorsReferenced = new DcmTag(0x0072, 0x0014);

		/// <summary>(0072,0020) VR=SQ Image Sets Sequence</summary>
		public static DcmTag ImageSetsSequence = new DcmTag(0x0072, 0x0020);

		/// <summary>(0072,0022) VR=SQ Image Set Selector Sequence</summary>
		public static DcmTag ImageSetSelectorSequence = new DcmTag(0x0072, 0x0022);

		/// <summary>(0072,0024) VR=CS Image Set Selector Usage Flag</summary>
		public static DcmTag ImageSetSelectorUsageFlag = new DcmTag(0x0072, 0x0024);

		/// <summary>(0072,0026) VR=AT Selector Attribute</summary>
		public static DcmTag SelectorAttribute = new DcmTag(0x0072, 0x0026);

		/// <summary>(0072,0028) VR=US Selector Value Number</summary>
		public static DcmTag SelectorValueNumber = new DcmTag(0x0072, 0x0028);

		/// <summary>(0072,0030) VR=SQ Time Based Image Sets Sequence</summary>
		public static DcmTag TimeBasedImageSetsSequence = new DcmTag(0x0072, 0x0030);

		/// <summary>(0072,0032) VR=US Image Set Number</summary>
		public static DcmTag ImageSetNumber = new DcmTag(0x0072, 0x0032);

		/// <summary>(0072,0034) VR=CS Image Set Selector Category</summary>
		public static DcmTag ImageSetSelectorCategory = new DcmTag(0x0072, 0x0034);

		/// <summary>(0072,0038) VR=US Relative Time</summary>
		public static DcmTag RelativeTime = new DcmTag(0x0072, 0x0038);

		/// <summary>(0072,003a) VR=CS Relative Time Units</summary>
		public static DcmTag RelativeTimeUnits = new DcmTag(0x0072, 0x003a);

		/// <summary>(0072,003c) VR=SS Abstract Prior Value</summary>
		public static DcmTag AbstractPriorValue = new DcmTag(0x0072, 0x003c);

		/// <summary>(0072,003e) VR=SQ Abstract Prior Code Sequence</summary>
		public static DcmTag AbstractPriorCodeSequence = new DcmTag(0x0072, 0x003e);

		/// <summary>(0072,0040) VR=LO Image Set Label</summary>
		public static DcmTag ImageSetLabel = new DcmTag(0x0072, 0x0040);

		/// <summary>(0072,0050) VR=CS Selector Attribute VR</summary>
		public static DcmTag SelectorAttributeVR = new DcmTag(0x0072, 0x0050);

		/// <summary>(0072,0052) VR=AT Selector Sequence Pointer</summary>
		public static DcmTag SelectorSequencePointer = new DcmTag(0x0072, 0x0052);

		/// <summary>(0072,0054) VR=LO Selector Sequence Pointer Private Creator</summary>
		public static DcmTag SelectorSequencePointerPrivateCreator = new DcmTag(0x0072, 0x0054);

		/// <summary>(0072,0056) VR=LO Selector Attribute Private Creator</summary>
		public static DcmTag SelectorAttributePrivateCreator = new DcmTag(0x0072, 0x0056);

		/// <summary>(0072,0060) VR=AT Selector AT Value</summary>
		public static DcmTag SelectorATValue = new DcmTag(0x0072, 0x0060);

		/// <summary>(0072,0062) VR=CS Selector CS Value</summary>
		public static DcmTag SelectorCSValue = new DcmTag(0x0072, 0x0062);

		/// <summary>(0072,0064) VR=IS Selector IS Value</summary>
		public static DcmTag SelectorISValue = new DcmTag(0x0072, 0x0064);

		/// <summary>(0072,0066) VR=LO Selector LO Value</summary>
		public static DcmTag SelectorLOValue = new DcmTag(0x0072, 0x0066);

		/// <summary>(0072,0068) VR=LT Selector LT Value</summary>
		public static DcmTag SelectorLTValue = new DcmTag(0x0072, 0x0068);

		/// <summary>(0072,006a) VR=PN Selector PN Value</summary>
		public static DcmTag SelectorPNValue = new DcmTag(0x0072, 0x006a);

		/// <summary>(0072,006c) VR=SH Selector SH Value</summary>
		public static DcmTag SelectorSHValue = new DcmTag(0x0072, 0x006c);

		/// <summary>(0072,006e) VR=ST Selector ST Value</summary>
		public static DcmTag SelectorSTValue = new DcmTag(0x0072, 0x006e);

		/// <summary>(0072,0070) VR=UT Selector UT Value</summary>
		public static DcmTag SelectorUTValue = new DcmTag(0x0072, 0x0070);

		/// <summary>(0072,0072) VR=DS Selector DS Value</summary>
		public static DcmTag SelectorDSValue = new DcmTag(0x0072, 0x0072);

		/// <summary>(0072,0074) VR=FD Selector FD Value</summary>
		public static DcmTag SelectorFDValue = new DcmTag(0x0072, 0x0074);

		/// <summary>(0072,0076) VR=FL Selector FL Value</summary>
		public static DcmTag SelectorFLValue = new DcmTag(0x0072, 0x0076);

		/// <summary>(0072,0078) VR=UL Selector UL Value</summary>
		public static DcmTag SelectorULValue = new DcmTag(0x0072, 0x0078);

		/// <summary>(0072,007a) VR=US Selector US Value</summary>
		public static DcmTag SelectorUSValue = new DcmTag(0x0072, 0x007a);

		/// <summary>(0072,007c) VR=SL Selector SL Value</summary>
		public static DcmTag SelectorSLValue = new DcmTag(0x0072, 0x007c);

		/// <summary>(0072,007e) VR=SS Selector SS Value</summary>
		public static DcmTag SelectorSSValue = new DcmTag(0x0072, 0x007e);

		/// <summary>(0072,0080) VR=SQ Selector Code Sequence Value</summary>
		public static DcmTag SelectorCodeSequenceValue = new DcmTag(0x0072, 0x0080);

		/// <summary>(0072,0100) VR=US Number of Screens</summary>
		public static DcmTag NumberOfScreens = new DcmTag(0x0072, 0x0100);

		/// <summary>(0072,0102) VR=SQ Nominal Screen Definition Sequence</summary>
		public static DcmTag NominalScreenDefinitionSequence = new DcmTag(0x0072, 0x0102);

		/// <summary>(0072,0104) VR=US Number of Vertical Pixels</summary>
		public static DcmTag NumberOfVerticalPixels = new DcmTag(0x0072, 0x0104);

		/// <summary>(0072,0106) VR=US Number of Horizontal Pixels</summary>
		public static DcmTag NumberOfHorizontalPixels = new DcmTag(0x0072, 0x0106);

		/// <summary>(0072,0108) VR=FD Display Environment Spatial Position</summary>
		public static DcmTag DisplayEnvironmentSpatialPosition = new DcmTag(0x0072, 0x0108);

		/// <summary>(0072,010a) VR=US Screen Minimum Grayscale Bit Depth</summary>
		public static DcmTag ScreenMinimumGrayscaleBitDepth = new DcmTag(0x0072, 0x010a);

		/// <summary>(0072,010c) VR=US Screen Minimum Color Bit Depth</summary>
		public static DcmTag ScreenMinimumColorBitDepth = new DcmTag(0x0072, 0x010c);

		/// <summary>(0072,010e) VR=US Application Maximum Repaint Time</summary>
		public static DcmTag ApplicationMaximumRepaintTime = new DcmTag(0x0072, 0x010e);

		/// <summary>(0072,0200) VR=SQ Display Sets Sequence</summary>
		public static DcmTag DisplaySetsSequence = new DcmTag(0x0072, 0x0200);

		/// <summary>(0072,0202) VR=US Display Set Number</summary>
		public static DcmTag DisplaySetNumber = new DcmTag(0x0072, 0x0202);

		/// <summary>(0072,0203) VR=LO Display Set Label</summary>
		public static DcmTag DisplaySetLabel = new DcmTag(0x0072, 0x0203);

		/// <summary>(0072,0204) VR=US Display Set Presentation Group</summary>
		public static DcmTag DisplaySetPresentationGroup = new DcmTag(0x0072, 0x0204);

		/// <summary>(0072,0206) VR=LO Display Set Presentation Group Description</summary>
		public static DcmTag DisplaySetPresentationGroupDescription = new DcmTag(0x0072, 0x0206);

		/// <summary>(0072,0208) VR=CS Partial Data Display Handling</summary>
		public static DcmTag PartialDataDisplayHandling = new DcmTag(0x0072, 0x0208);

		/// <summary>(0072,0210) VR=SQ Synchronized Scrolling Sequence</summary>
		public static DcmTag SynchronizedScrollingSequence = new DcmTag(0x0072, 0x0210);

		/// <summary>(0072,0212) VR=US Display Set Scrolling Group</summary>
		public static DcmTag DisplaySetScrollingGroup = new DcmTag(0x0072, 0x0212);

		/// <summary>(0072,0214) VR=SQ Navigation Indicator Sequence</summary>
		public static DcmTag NavigationIndicatorSequence = new DcmTag(0x0072, 0x0214);

		/// <summary>(0072,0216) VR=US Navigation Display Set</summary>
		public static DcmTag NavigationDisplaySet = new DcmTag(0x0072, 0x0216);

		/// <summary>(0072,0218) VR=US Reference Display Sets</summary>
		public static DcmTag ReferenceDisplaySets = new DcmTag(0x0072, 0x0218);

		/// <summary>(0072,0300) VR=SQ Image Boxes Sequence</summary>
		public static DcmTag ImageBoxesSequence = new DcmTag(0x0072, 0x0300);

		/// <summary>(0072,0302) VR=US Image Box Number</summary>
		public static DcmTag ImageBoxNumber = new DcmTag(0x0072, 0x0302);

		/// <summary>(0072,0304) VR=CS Image Box Layout Type</summary>
		public static DcmTag ImageBoxLayoutType = new DcmTag(0x0072, 0x0304);

		/// <summary>(0072,0306) VR=US Image Box Tile Horizontal Dimension</summary>
		public static DcmTag ImageBoxTileHorizontalDimension = new DcmTag(0x0072, 0x0306);

		/// <summary>(0072,0308) VR=US Image Box Tile Vertical Dimension</summary>
		public static DcmTag ImageBoxTileVerticalDimension = new DcmTag(0x0072, 0x0308);

		/// <summary>(0072,0310) VR=CS Image Box Scroll Direction</summary>
		public static DcmTag ImageBoxScrollDirection = new DcmTag(0x0072, 0x0310);

		/// <summary>(0072,0312) VR=CS Image Box Small Scroll Type</summary>
		public static DcmTag ImageBoxSmallScrollType = new DcmTag(0x0072, 0x0312);

		/// <summary>(0072,0314) VR=US Image Box Small Scroll Amount</summary>
		public static DcmTag ImageBoxSmallScrollAmount = new DcmTag(0x0072, 0x0314);

		/// <summary>(0072,0316) VR=CS Image Box Large Scroll Type</summary>
		public static DcmTag ImageBoxLargeScrollType = new DcmTag(0x0072, 0x0316);

		/// <summary>(0072,0318) VR=US Image Box Large Scroll Amount</summary>
		public static DcmTag ImageBoxLargeScrollAmount = new DcmTag(0x0072, 0x0318);

		/// <summary>(0072,0320) VR=US Image Box Overlap Priority</summary>
		public static DcmTag ImageBoxOverlapPriority = new DcmTag(0x0072, 0x0320);

		/// <summary>(0072,0330) VR=FD Cine Relative to Real-Time</summary>
		public static DcmTag CineRelativeToRealTime = new DcmTag(0x0072, 0x0330);

		/// <summary>(0072,0400) VR=SQ Filter Operations Sequence</summary>
		public static DcmTag FilterOperationsSequence = new DcmTag(0x0072, 0x0400);

		/// <summary>(0072,0402) VR=CS Filter-by Category</summary>
		public static DcmTag FilterbyCategory = new DcmTag(0x0072, 0x0402);

		/// <summary>(0072,0404) VR=CS Filter-by Attribute Presence</summary>
		public static DcmTag FilterbyAttributePresence = new DcmTag(0x0072, 0x0404);

		/// <summary>(0072,0406) VR=CS Filter-by Operator</summary>
		public static DcmTag FilterbyOperator = new DcmTag(0x0072, 0x0406);

		/// <summary>(0072,0500) VR=CS Blending Operation Type</summary>
		public static DcmTag BlendingOperationType = new DcmTag(0x0072, 0x0500);

		/// <summary>(0072,0510) VR=CS Reformatting Operation Type</summary>
		public static DcmTag ReformattingOperationType = new DcmTag(0x0072, 0x0510);

		/// <summary>(0072,0512) VR=FD Reformatting Thickness</summary>
		public static DcmTag ReformattingThickness = new DcmTag(0x0072, 0x0512);

		/// <summary>(0072,0514) VR=FD Reformatting Interval</summary>
		public static DcmTag ReformattingInterval = new DcmTag(0x0072, 0x0514);

		/// <summary>(0072,0516) VR=CS Reformatting Operation Initial View Direction</summary>
		public static DcmTag ReformattingOperationInitialViewDirection = new DcmTag(0x0072, 0x0516);

		/// <summary>(0072,0520) VR=CS 3D Rendering Type</summary>
		public static DcmTag N3DRenderingType = new DcmTag(0x0072, 0x0520);

		/// <summary>(0072,0600) VR=SQ Sorting Operations Sequence</summary>
		public static DcmTag SortingOperationsSequence = new DcmTag(0x0072, 0x0600);

		/// <summary>(0072,0602) VR=CS Sort-by Category</summary>
		public static DcmTag SortbyCategory = new DcmTag(0x0072, 0x0602);

		/// <summary>(0072,0604) VR=CS Sorting Direction</summary>
		public static DcmTag SortingDirection = new DcmTag(0x0072, 0x0604);

		/// <summary>(0072,0700) VR=CS Display Set Patient Orientation</summary>
		public static DcmTag DisplaySetPatientOrientation = new DcmTag(0x0072, 0x0700);

		/// <summary>(0072,0702) VR=CS VOI Type</summary>
		public static DcmTag VOIType = new DcmTag(0x0072, 0x0702);

		/// <summary>(0072,0704) VR=CS Pseudo-color Type</summary>
		public static DcmTag PseudocolorType = new DcmTag(0x0072, 0x0704);

		/// <summary>(0072,0706) VR=CS Show Grayscale Inverted</summary>
		public static DcmTag ShowGrayscaleInverted = new DcmTag(0x0072, 0x0706);

		/// <summary>(0072,0710) VR=CS Show Image True Size Flag</summary>
		public static DcmTag ShowImageTrueSizeFlag = new DcmTag(0x0072, 0x0710);

		/// <summary>(0072,0712) VR=CS Show Graphic Annotation Flag</summary>
		public static DcmTag ShowGraphicAnnotationFlag = new DcmTag(0x0072, 0x0712);

		/// <summary>(0072,0714) VR=CS Show Patient Demographics Flag</summary>
		public static DcmTag ShowPatientDemographicsFlag = new DcmTag(0x0072, 0x0714);

		/// <summary>(0072,0716) VR=CS Show Acquisition Techniques Flag</summary>
		public static DcmTag ShowAcquisitionTechniquesFlag = new DcmTag(0x0072, 0x0716);

		/// <summary>(0072,0717) VR=CS Display Set Horizontal Justification</summary>
		public static DcmTag DisplaySetHorizontalJustification = new DcmTag(0x0072, 0x0717);

		/// <summary>(0072,0718) VR=CS Display Set Vertical Justification</summary>
		public static DcmTag DisplaySetVerticalJustification = new DcmTag(0x0072, 0x0718);

		/// <summary>(0088,0130) VR=SH Storage Media File-set ID</summary>
		public static DcmTag StorageMediaFilesetID = new DcmTag(0x0088, 0x0130);

		/// <summary>(0088,0140) VR=UI Storage Media File-set UID</summary>
		public static DcmTag StorageMediaFilesetUID = new DcmTag(0x0088, 0x0140);

		/// <summary>(0088,0200) VR=SQ Icon Image Sequence</summary>
		public static DcmTag IconImageSequence = new DcmTag(0x0088, 0x0200);

		/// <summary>(0088,0904) VR=LO Topic Title</summary>
		public static DcmTag TopicTitle = new DcmTag(0x0088, 0x0904);

		/// <summary>(0088,0906) VR=ST Topic Subject</summary>
		public static DcmTag TopicSubject = new DcmTag(0x0088, 0x0906);

		/// <summary>(0088,0910) VR=LO Topic Author</summary>
		public static DcmTag TopicAuthor = new DcmTag(0x0088, 0x0910);

		/// <summary>(0088,0912) VR=LO Topic Keywords</summary>
		public static DcmTag TopicKeywords = new DcmTag(0x0088, 0x0912);

		/// <summary>(0100,0410) VR=CS SOP Instance Status</summary>
		public static DcmTag SOPInstanceStatus = new DcmTag(0x0100, 0x0410);

		/// <summary>(0100,0420) VR=DT SOP Authorization Date and Time</summary>
		public static DcmTag SOPAuthorizationDateAndTime = new DcmTag(0x0100, 0x0420);

		/// <summary>(0100,0424) VR=LT SOP Authorization Comment</summary>
		public static DcmTag SOPAuthorizationComment = new DcmTag(0x0100, 0x0424);

		/// <summary>(0100,0426) VR=LO Authorization Equipment Certification Number</summary>
		public static DcmTag AuthorizationEquipmentCertificationNumber = new DcmTag(0x0100, 0x0426);

		/// <summary>(0400,0005) VR=US MAC ID Number</summary>
		public static DcmTag MACIDNumber = new DcmTag(0x0400, 0x0005);

		/// <summary>(0400,0010) VR=UI MAC Calculation Transfer Syntax UID</summary>
		public static DcmTag MACCalculationTransferSyntaxUID = new DcmTag(0x0400, 0x0010);

		/// <summary>(0400,0015) VR=CS MAC Algorithm</summary>
		public static DcmTag MACAlgorithm = new DcmTag(0x0400, 0x0015);

		/// <summary>(0400,0020) VR=AT Data Elements Signed</summary>
		public static DcmTag DataElementsSigned = new DcmTag(0x0400, 0x0020);

		/// <summary>(0400,0100) VR=UI Digital Signature UID</summary>
		public static DcmTag DigitalSignatureUID = new DcmTag(0x0400, 0x0100);

		/// <summary>(0400,0105) VR=DT Digital Signature DateTime</summary>
		public static DcmTag DigitalSignatureDateTime = new DcmTag(0x0400, 0x0105);

		/// <summary>(0400,0110) VR=CS Certificate Type</summary>
		public static DcmTag CertificateType = new DcmTag(0x0400, 0x0110);

		/// <summary>(0400,0115) VR=OB Certificate of Signer</summary>
		public static DcmTag CertificateOfSigner = new DcmTag(0x0400, 0x0115);

		/// <summary>(0400,0120) VR=OB Signature</summary>
		public static DcmTag Signature = new DcmTag(0x0400, 0x0120);

		/// <summary>(0400,0305) VR=CS Certified Timestamp Type</summary>
		public static DcmTag CertifiedTimestampType = new DcmTag(0x0400, 0x0305);

		/// <summary>(0400,0310) VR=OB Certified Timestamp</summary>
		public static DcmTag CertifiedTimestamp = new DcmTag(0x0400, 0x0310);

		/// <summary>(0400,0401) VR=SQ Digital Signature Purpose Code Sequence</summary>
		public static DcmTag DigitalSignaturePurposeCodeSequence = new DcmTag(0x0400, 0x0401);

		/// <summary>(0400,0402) VR=SQ Referenced Digital Signature Sequence</summary>
		public static DcmTag ReferencedDigitalSignatureSequence = new DcmTag(0x0400, 0x0402);

		/// <summary>(0400,0403) VR=SQ Referenced SOP Instance MAC Sequence</summary>
		public static DcmTag ReferencedSOPInstanceMACSequence = new DcmTag(0x0400, 0x0403);

		/// <summary>(0400,0404) VR=OB MAC</summary>
		public static DcmTag MAC = new DcmTag(0x0400, 0x0404);

		/// <summary>(0400,0500) VR=SQ Encrypted Attributes Sequence</summary>
		public static DcmTag EncryptedAttributesSequence = new DcmTag(0x0400, 0x0500);

		/// <summary>(0400,0510) VR=UI Encrypted Content Transfer Syntax UID</summary>
		public static DcmTag EncryptedContentTransferSyntaxUID = new DcmTag(0x0400, 0x0510);

		/// <summary>(0400,0520) VR=OB Encrypted Content</summary>
		public static DcmTag EncryptedContent = new DcmTag(0x0400, 0x0520);

		/// <summary>(0400,0550) VR=SQ Modified Attributes Sequence</summary>
		public static DcmTag ModifiedAttributesSequence = new DcmTag(0x0400, 0x0550);

		/// <summary>(0400,0561) VR=SQ Original Attributes Sequence</summary>
		public static DcmTag OriginalAttributesSequence = new DcmTag(0x0400, 0x0561);

		/// <summary>(0400,0562) VR=DT Attribute Modification Datetime</summary>
		public static DcmTag AttributeModificationDatetime = new DcmTag(0x0400, 0x0562);

		/// <summary>(0400,0563) VR=LO Modifying System</summary>
		public static DcmTag ModifyingSystem = new DcmTag(0x0400, 0x0563);

		/// <summary>(0400,0564) VR=LO Source of Previous Values</summary>
		public static DcmTag SourceOfPreviousValues = new DcmTag(0x0400, 0x0564);

		/// <summary>(0400,0565) VR=CS Reason for the Attribute Modification</summary>
		public static DcmTag ReasonForTheAttributeModification = new DcmTag(0x0400, 0x0565);

		/// <summary>(2000,0010) VR=IS Number of Copies</summary>
		public static DcmTag NumberOfCopies = new DcmTag(0x2000, 0x0010);

		/// <summary>(2000,001e) VR=SQ Printer Configuration Sequence</summary>
		public static DcmTag PrinterConfigurationSequence = new DcmTag(0x2000, 0x001e);

		/// <summary>(2000,0020) VR=CS Print Priority</summary>
		public static DcmTag PrintPriority = new DcmTag(0x2000, 0x0020);

		/// <summary>(2000,0030) VR=CS Medium Type</summary>
		public static DcmTag MediumType = new DcmTag(0x2000, 0x0030);

		/// <summary>(2000,0040) VR=CS Film Destination</summary>
		public static DcmTag FilmDestination = new DcmTag(0x2000, 0x0040);

		/// <summary>(2000,0050) VR=LO Film Session Label</summary>
		public static DcmTag FilmSessionLabel = new DcmTag(0x2000, 0x0050);

		/// <summary>(2000,0060) VR=IS Memory Allocation</summary>
		public static DcmTag MemoryAllocation = new DcmTag(0x2000, 0x0060);

		/// <summary>(2000,0061) VR=IS Maximum Memory Allocation</summary>
		public static DcmTag MaximumMemoryAllocation = new DcmTag(0x2000, 0x0061);

		/// <summary>(2000,0062) VR=CS Color Image Printing Flag (Retired)</summary>
		public static DcmTag ColorImagePrintingFlagRETIRED = new DcmTag(0x2000, 0x0062);

		/// <summary>(2000,0063) VR=CS Collation Flag (Retired)</summary>
		public static DcmTag CollationFlagRETIRED = new DcmTag(0x2000, 0x0063);

		/// <summary>(2000,0065) VR=CS Annotation Flag (Retired)</summary>
		public static DcmTag AnnotationFlagRETIRED = new DcmTag(0x2000, 0x0065);

		/// <summary>(2000,0067) VR=CS Image Overlay Flag (Retired)</summary>
		public static DcmTag ImageOverlayFlagRETIRED = new DcmTag(0x2000, 0x0067);

		/// <summary>(2000,0069) VR=CS Presentation LUT Flag (Retired)</summary>
		public static DcmTag PresentationLUTFlagRETIRED = new DcmTag(0x2000, 0x0069);

		/// <summary>(2000,006a) VR=CS Image Box Presentation LUT Flag (Retired)</summary>
		public static DcmTag ImageBoxPresentationLUTFlagRETIRED = new DcmTag(0x2000, 0x006a);

		/// <summary>(2000,00a0) VR=US Memory Bit Depth</summary>
		public static DcmTag MemoryBitDepth = new DcmTag(0x2000, 0x00a0);

		/// <summary>(2000,00a1) VR=US Printing Bit Depth</summary>
		public static DcmTag PrintingBitDepth = new DcmTag(0x2000, 0x00a1);

		/// <summary>(2000,00a2) VR=SQ Media Installed Sequence</summary>
		public static DcmTag MediaInstalledSequence = new DcmTag(0x2000, 0x00a2);

		/// <summary>(2000,00a4) VR=SQ Other Media Available Sequence</summary>
		public static DcmTag OtherMediaAvailableSequence = new DcmTag(0x2000, 0x00a4);

		/// <summary>(2000,00a8) VR=SQ Supported Image Display Formats Sequence</summary>
		public static DcmTag SupportedImageDisplayFormatsSequence = new DcmTag(0x2000, 0x00a8);

		/// <summary>(2000,0500) VR=SQ Referenced Film Box Sequence</summary>
		public static DcmTag ReferencedFilmBoxSequence = new DcmTag(0x2000, 0x0500);

		/// <summary>(2000,0510) VR=SQ Referenced Stored Print Sequence</summary>
		public static DcmTag ReferencedStoredPrintSequence = new DcmTag(0x2000, 0x0510);

		/// <summary>(2010,0010) VR=ST Image Display Format</summary>
		public static DcmTag ImageDisplayFormat = new DcmTag(0x2010, 0x0010);

		/// <summary>(2010,0030) VR=CS Annotation Display Format ID</summary>
		public static DcmTag AnnotationDisplayFormatID = new DcmTag(0x2010, 0x0030);

		/// <summary>(2010,0040) VR=CS Film Orientation</summary>
		public static DcmTag FilmOrientation = new DcmTag(0x2010, 0x0040);

		/// <summary>(2010,0050) VR=CS Film Size ID</summary>
		public static DcmTag FilmSizeID = new DcmTag(0x2010, 0x0050);

		/// <summary>(2010,0052) VR=CS Printer Resolution ID</summary>
		public static DcmTag PrinterResolutionID = new DcmTag(0x2010, 0x0052);

		/// <summary>(2010,0054) VR=CS Default Printer Resolution ID</summary>
		public static DcmTag DefaultPrinterResolutionID = new DcmTag(0x2010, 0x0054);

		/// <summary>(2010,0060) VR=CS Magnification Type</summary>
		public static DcmTag MagnificationType = new DcmTag(0x2010, 0x0060);

		/// <summary>(2010,0080) VR=CS Smoothing Type</summary>
		public static DcmTag SmoothingType = new DcmTag(0x2010, 0x0080);

		/// <summary>(2010,00a6) VR=CS Default Magnification Type</summary>
		public static DcmTag DefaultMagnificationType = new DcmTag(0x2010, 0x00a6);

		/// <summary>(2010,00a7) VR=CS Other Magnification Types Available</summary>
		public static DcmTag OtherMagnificationTypesAvailable = new DcmTag(0x2010, 0x00a7);

		/// <summary>(2010,00a8) VR=CS Default Smoothing Type</summary>
		public static DcmTag DefaultSmoothingType = new DcmTag(0x2010, 0x00a8);

		/// <summary>(2010,00a9) VR=CS Other Smoothing Types Available</summary>
		public static DcmTag OtherSmoothingTypesAvailable = new DcmTag(0x2010, 0x00a9);

		/// <summary>(2010,0100) VR=CS Border Density</summary>
		public static DcmTag BorderDensity = new DcmTag(0x2010, 0x0100);

		/// <summary>(2010,0110) VR=CS Empty Image Density</summary>
		public static DcmTag EmptyImageDensity = new DcmTag(0x2010, 0x0110);

		/// <summary>(2010,0120) VR=US Min Density</summary>
		public static DcmTag MinDensity = new DcmTag(0x2010, 0x0120);

		/// <summary>(2010,0130) VR=US Max Density</summary>
		public static DcmTag MaxDensity = new DcmTag(0x2010, 0x0130);

		/// <summary>(2010,0140) VR=CS Trim</summary>
		public static DcmTag Trim = new DcmTag(0x2010, 0x0140);

		/// <summary>(2010,0150) VR=ST Configuration Information</summary>
		public static DcmTag ConfigurationInformation = new DcmTag(0x2010, 0x0150);

		/// <summary>(2010,0152) VR=LT Configuration Information Description</summary>
		public static DcmTag ConfigurationInformationDescription = new DcmTag(0x2010, 0x0152);

		/// <summary>(2010,0154) VR=IS Maximum Collated Films</summary>
		public static DcmTag MaximumCollatedFilms = new DcmTag(0x2010, 0x0154);

		/// <summary>(2010,015e) VR=US Illumination</summary>
		public static DcmTag Illumination = new DcmTag(0x2010, 0x015e);

		/// <summary>(2010,0160) VR=US Reflected Ambient Light</summary>
		public static DcmTag ReflectedAmbientLight = new DcmTag(0x2010, 0x0160);

		/// <summary>(2010,0376) VR=DS Printer Pixel Spacing</summary>
		public static DcmTag PrinterPixelSpacing = new DcmTag(0x2010, 0x0376);

		/// <summary>(2010,0500) VR=SQ Referenced Film Session Sequence</summary>
		public static DcmTag ReferencedFilmSessionSequence = new DcmTag(0x2010, 0x0500);

		/// <summary>(2010,0510) VR=SQ Referenced Image Box Sequence</summary>
		public static DcmTag ReferencedImageBoxSequence = new DcmTag(0x2010, 0x0510);

		/// <summary>(2010,0520) VR=SQ Referenced Basic Annotation Box Sequence</summary>
		public static DcmTag ReferencedBasicAnnotationBoxSequence = new DcmTag(0x2010, 0x0520);

		/// <summary>(2020,0010) VR=US Image Position</summary>
		public static DcmTag ImagePosition = new DcmTag(0x2020, 0x0010);

		/// <summary>(2020,0020) VR=CS Polarity</summary>
		public static DcmTag Polarity = new DcmTag(0x2020, 0x0020);

		/// <summary>(2020,0030) VR=DS Requested Image Size</summary>
		public static DcmTag RequestedImageSize = new DcmTag(0x2020, 0x0030);

		/// <summary>(2020,0040) VR=CS Requested Decimate/Crop Behavior</summary>
		public static DcmTag RequestedDecimateCropBehavior = new DcmTag(0x2020, 0x0040);

		/// <summary>(2020,0050) VR=CS Requested Resolution ID</summary>
		public static DcmTag RequestedResolutionID = new DcmTag(0x2020, 0x0050);

		/// <summary>(2020,00a0) VR=CS Requested Image Size Flag</summary>
		public static DcmTag RequestedImageSizeFlag = new DcmTag(0x2020, 0x00a0);

		/// <summary>(2020,00a2) VR=CS Decimate/Crop Result</summary>
		public static DcmTag DecimateCropResult = new DcmTag(0x2020, 0x00a2);

		/// <summary>(2020,0110) VR=SQ Basic Grayscale Image Sequence</summary>
		public static DcmTag BasicGrayscaleImageSequence = new DcmTag(0x2020, 0x0110);

		/// <summary>(2020,0111) VR=SQ Basic Color Image Sequence</summary>
		public static DcmTag BasicColorImageSequence = new DcmTag(0x2020, 0x0111);

		/// <summary>(2020,0130) VR=SQ Referenced Image Overlay Box Sequence (Retired)</summary>
		public static DcmTag ReferencedImageOverlayBoxSequenceRETIRED = new DcmTag(0x2020, 0x0130);

		/// <summary>(2020,0140) VR=SQ Referenced VOI LUT Box Sequence (Retired)</summary>
		public static DcmTag ReferencedVOILUTBoxSequenceRETIRED = new DcmTag(0x2020, 0x0140);

		/// <summary>(2030,0010) VR=US Annotation Position</summary>
		public static DcmTag AnnotationPosition = new DcmTag(0x2030, 0x0010);

		/// <summary>(2030,0020) VR=LO Text String</summary>
		public static DcmTag TextString = new DcmTag(0x2030, 0x0020);

		/// <summary>(2040,0010) VR=SQ Referenced Overlay Plane Sequence (Retired)</summary>
		public static DcmTag ReferencedOverlayPlaneSequenceRETIRED = new DcmTag(0x2040, 0x0010);

		/// <summary>(2040,0011) VR=US Referenced Overlay Plane Groups (Retired)</summary>
		public static DcmTag ReferencedOverlayPlaneGroupsRETIRED = new DcmTag(0x2040, 0x0011);

		/// <summary>(2040,0020) VR=SQ Overlay Pixel Data Sequence (Retired)</summary>
		public static DcmTag OverlayPixelDataSequenceRETIRED = new DcmTag(0x2040, 0x0020);

		/// <summary>(2040,0060) VR=CS Overlay Magnification Type (Retired)</summary>
		public static DcmTag OverlayMagnificationTypeRETIRED = new DcmTag(0x2040, 0x0060);

		/// <summary>(2040,0070) VR=CS Overlay Smoothing Type (Retired)</summary>
		public static DcmTag OverlaySmoothingTypeRETIRED = new DcmTag(0x2040, 0x0070);

		/// <summary>(2040,0072) VR=CS Overlay or Image Magnification (Retired)</summary>
		public static DcmTag OverlayOrImageMagnificationRETIRED = new DcmTag(0x2040, 0x0072);

		/// <summary>(2040,0074) VR=US Magnify to Number of Columns (Retired)</summary>
		public static DcmTag MagnifyToNumberOfColumnsRETIRED = new DcmTag(0x2040, 0x0074);

		/// <summary>(2040,0080) VR=CS Overlay Foreground Density (Retired)</summary>
		public static DcmTag OverlayForegroundDensityRETIRED = new DcmTag(0x2040, 0x0080);

		/// <summary>(2040,0082) VR=CS Overlay Background Density (Retired)</summary>
		public static DcmTag OverlayBackgroundDensityRETIRED = new DcmTag(0x2040, 0x0082);

		/// <summary>(2040,0090) VR=CS Overlay Mode (Retired)</summary>
		public static DcmTag OverlayModeRETIRED = new DcmTag(0x2040, 0x0090);

		/// <summary>(2040,0100) VR=CS Threshold Density (Retired)</summary>
		public static DcmTag ThresholdDensityRETIRED = new DcmTag(0x2040, 0x0100);

		/// <summary>(2040,0500) VR=SQ Referenced Image Box Sequence (Retired)</summary>
		public static DcmTag ReferencedImageBoxSequenceRETIRED = new DcmTag(0x2040, 0x0500);

		/// <summary>(2050,0010) VR=SQ Presentation LUT Sequence</summary>
		public static DcmTag PresentationLUTSequence = new DcmTag(0x2050, 0x0010);

		/// <summary>(2050,0020) VR=CS Presentation LUT Shape</summary>
		public static DcmTag PresentationLUTShape = new DcmTag(0x2050, 0x0020);

		/// <summary>(2050,0500) VR=SQ Referenced Presentation LUT Sequence</summary>
		public static DcmTag ReferencedPresentationLUTSequence = new DcmTag(0x2050, 0x0500);

		/// <summary>(2100,0010) VR=SH Print Job ID</summary>
		public static DcmTag PrintJobID = new DcmTag(0x2100, 0x0010);

		/// <summary>(2100,0020) VR=CS Execution Status</summary>
		public static DcmTag ExecutionStatus = new DcmTag(0x2100, 0x0020);

		/// <summary>(2100,0030) VR=CS Execution Status Info</summary>
		public static DcmTag ExecutionStatusInfo = new DcmTag(0x2100, 0x0030);

		/// <summary>(2100,0040) VR=DA Creation Date</summary>
		public static DcmTag CreationDate = new DcmTag(0x2100, 0x0040);

		/// <summary>(2100,0050) VR=TM Creation Time</summary>
		public static DcmTag CreationTime = new DcmTag(0x2100, 0x0050);

		/// <summary>(2100,0070) VR=AE Originator</summary>
		public static DcmTag OriginatorAE = new DcmTag(0x2100, 0x0070);

		/// <summary>(2100,0140) VR=AE Destination AE</summary>
		public static DcmTag DestinationAE = new DcmTag(0x2100, 0x0140);

		/// <summary>(2100,0160) VR=SH Owner ID</summary>
		public static DcmTag OwnerID = new DcmTag(0x2100, 0x0160);

		/// <summary>(2100,0170) VR=IS Number of Films</summary>
		public static DcmTag NumberOfFilms = new DcmTag(0x2100, 0x0170);

		/// <summary>(2100,0500) VR=SQ Referenced Print Job Sequence (Pull Stored Print) (Retired)</summary>
		public static DcmTag ReferencedPrintJobSequencePullStoredPrintRETIRED = new DcmTag(0x2100, 0x0500);

		/// <summary>(2110,0010) VR=CS Printer Status</summary>
		public static DcmTag PrinterStatus = new DcmTag(0x2110, 0x0010);

		/// <summary>(2110,0020) VR=CS Printer Status Info</summary>
		public static DcmTag PrinterStatusInfo = new DcmTag(0x2110, 0x0020);

		/// <summary>(2110,0030) VR=LO Printer Name</summary>
		public static DcmTag PrinterName = new DcmTag(0x2110, 0x0030);

		/// <summary>(2110,0099) VR=SH Print Queue ID (Retired)</summary>
		public static DcmTag PrintQueueIDRETIRED = new DcmTag(0x2110, 0x0099);

		/// <summary>(2120,0010) VR=CS Queue Status (Retired)</summary>
		public static DcmTag QueueStatusRETIRED = new DcmTag(0x2120, 0x0010);

		/// <summary>(2120,0050) VR=SQ Print Job Description Sequence (Retired)</summary>
		public static DcmTag PrintJobDescriptionSequenceRETIRED = new DcmTag(0x2120, 0x0050);

		/// <summary>(2120,0070) VR=SQ Referenced Print Job Sequence (Retired)</summary>
		public static DcmTag ReferencedPrintJobSequenceRETIRED = new DcmTag(0x2120, 0x0070);

		/// <summary>(2130,0010) VR=SQ Print Management Capabilities Sequence (Retired)</summary>
		public static DcmTag PrintManagementCapabilitiesSequenceRETIRED = new DcmTag(0x2130, 0x0010);

		/// <summary>(2130,0015) VR=SQ Printer Characteristics Sequence (Retired)</summary>
		public static DcmTag PrinterCharacteristicsSequenceRETIRED = new DcmTag(0x2130, 0x0015);

		/// <summary>(2130,0030) VR=SQ Film Box Content Sequence (Retired)</summary>
		public static DcmTag FilmBoxContentSequenceRETIRED = new DcmTag(0x2130, 0x0030);

		/// <summary>(2130,0040) VR=SQ Image Box Content Sequence (Retired)</summary>
		public static DcmTag ImageBoxContentSequenceRETIRED = new DcmTag(0x2130, 0x0040);

		/// <summary>(2130,0050) VR=SQ Annotation Content Sequence (Retired)</summary>
		public static DcmTag AnnotationContentSequenceRETIRED = new DcmTag(0x2130, 0x0050);

		/// <summary>(2130,0060) VR=SQ Image Overlay Box Content Sequence (Retired)</summary>
		public static DcmTag ImageOverlayBoxContentSequenceRETIRED = new DcmTag(0x2130, 0x0060);

		/// <summary>(2130,0080) VR=SQ Presentation LUT Content Sequence (Retired)</summary>
		public static DcmTag PresentationLUTContentSequenceRETIRED = new DcmTag(0x2130, 0x0080);

		/// <summary>(2130,00a0) VR=SQ Proposed Study Sequence (Retired)</summary>
		public static DcmTag ProposedStudySequenceRETIRED = new DcmTag(0x2130, 0x00a0);

		/// <summary>(2130,00c0) VR=SQ Original Image Sequence (Retired)</summary>
		public static DcmTag OriginalImageSequenceRETIRED = new DcmTag(0x2130, 0x00c0);

		/// <summary>(2200,0001) VR=CS Label Using Information Extracted From Instances</summary>
		public static DcmTag LabelUsingInformationExtractedFromInstances = new DcmTag(0x2200, 0x0001);

		/// <summary>(2200,0002) VR=UT Label Text</summary>
		public static DcmTag LabelText = new DcmTag(0x2200, 0x0002);

		/// <summary>(2200,0003) VR=CS Label Style Selection</summary>
		public static DcmTag LabelStyleSelection = new DcmTag(0x2200, 0x0003);

		/// <summary>(2200,0004) VR=LT Media Disposition</summary>
		public static DcmTag MediaDisposition = new DcmTag(0x2200, 0x0004);

		/// <summary>(2200,0005) VR=LT Barcode Value</summary>
		public static DcmTag BarcodeValue = new DcmTag(0x2200, 0x0005);

		/// <summary>(2200,0006) VR=CS Barcode Symbology</summary>
		public static DcmTag BarcodeSymbology = new DcmTag(0x2200, 0x0006);

		/// <summary>(2200,0007) VR=CS Allow Media Splitting</summary>
		public static DcmTag AllowMediaSplitting = new DcmTag(0x2200, 0x0007);

		/// <summary>(2200,0008) VR=CS Include Non-DICOM Objects</summary>
		public static DcmTag IncludeNonDICOMObjects = new DcmTag(0x2200, 0x0008);

		/// <summary>(2200,0009) VR=CS Include Display Application</summary>
		public static DcmTag IncludeDisplayApplication = new DcmTag(0x2200, 0x0009);

		/// <summary>(2200,000a) VR=CS Preserve Composite Instances After Media Creation</summary>
		public static DcmTag PreserveCompositeInstancesAfterMediaCreation = new DcmTag(0x2200, 0x000a);

		/// <summary>(2200,000b) VR=US Total Number of Pieces of Media Created</summary>
		public static DcmTag TotalNumberOfPiecesOfMediaCreated = new DcmTag(0x2200, 0x000b);

		/// <summary>(2200,000c) VR=LO Requested Media Application Profile</summary>
		public static DcmTag RequestedMediaApplicationProfile = new DcmTag(0x2200, 0x000c);

		/// <summary>(2200,000d) VR=SQ Referenced Storage Media Sequence</summary>
		public static DcmTag ReferencedStorageMediaSequence = new DcmTag(0x2200, 0x000d);

		/// <summary>(2200,000e) VR=AT Failure Attributes</summary>
		public static DcmTag FailureAttributes = new DcmTag(0x2200, 0x000e);

		/// <summary>(2200,000f) VR=CS Allow Lossy Compression</summary>
		public static DcmTag AllowLossyCompression = new DcmTag(0x2200, 0x000f);

		/// <summary>(2200,0020) VR=CS Request Priority</summary>
		public static DcmTag RequestPriority = new DcmTag(0x2200, 0x0020);

		/// <summary>(3002,0002) VR=SH RT Image Label</summary>
		public static DcmTag RTImageLabel = new DcmTag(0x3002, 0x0002);

		/// <summary>(3002,0003) VR=LO RT Image Name</summary>
		public static DcmTag RTImageName = new DcmTag(0x3002, 0x0003);

		/// <summary>(3002,0004) VR=ST RT Image Description</summary>
		public static DcmTag RTImageDescription = new DcmTag(0x3002, 0x0004);

		/// <summary>(3002,000a) VR=CS Reported Values Origin</summary>
		public static DcmTag ReportedValuesOrigin = new DcmTag(0x3002, 0x000a);

		/// <summary>(3002,000c) VR=CS RT Image Plane</summary>
		public static DcmTag RTImagePlane = new DcmTag(0x3002, 0x000c);

		/// <summary>(3002,000d) VR=DS X-Ray Image Receptor Translation</summary>
		public static DcmTag XRayImageReceptorTranslation = new DcmTag(0x3002, 0x000d);

		/// <summary>(3002,000e) VR=DS X-Ray Image Receptor Angle</summary>
		public static DcmTag XRayImageReceptorAngle = new DcmTag(0x3002, 0x000e);

		/// <summary>(3002,0010) VR=DS RT Image Orientation</summary>
		public static DcmTag RTImageOrientation = new DcmTag(0x3002, 0x0010);

		/// <summary>(3002,0011) VR=DS Image Plane Pixel Spacing</summary>
		public static DcmTag ImagePlanePixelSpacing = new DcmTag(0x3002, 0x0011);

		/// <summary>(3002,0012) VR=DS RT Image Position</summary>
		public static DcmTag RTImagePosition = new DcmTag(0x3002, 0x0012);

		/// <summary>(3002,0020) VR=SH Radiation Machine Name</summary>
		public static DcmTag RadiationMachineName = new DcmTag(0x3002, 0x0020);

		/// <summary>(3002,0022) VR=DS Radiation Machine SAD</summary>
		public static DcmTag RadiationMachineSAD = new DcmTag(0x3002, 0x0022);

		/// <summary>(3002,0024) VR=DS Radiation Machine SSD</summary>
		public static DcmTag RadiationMachineSSD = new DcmTag(0x3002, 0x0024);

		/// <summary>(3002,0026) VR=DS RT Image SID</summary>
		public static DcmTag RTImageSID = new DcmTag(0x3002, 0x0026);

		/// <summary>(3002,0028) VR=DS Source to Reference Object Distance</summary>
		public static DcmTag SourceToReferenceObjectDistance = new DcmTag(0x3002, 0x0028);

		/// <summary>(3002,0029) VR=IS Fraction Number</summary>
		public static DcmTag FractionNumber = new DcmTag(0x3002, 0x0029);

		/// <summary>(3002,0030) VR=SQ Exposure Sequence</summary>
		public static DcmTag ExposureSequence = new DcmTag(0x3002, 0x0030);

		/// <summary>(3002,0032) VR=DS Meterset Exposure</summary>
		public static DcmTag MetersetExposure = new DcmTag(0x3002, 0x0032);

		/// <summary>(3002,0034) VR=DS Diaphragm Position</summary>
		public static DcmTag DiaphragmPosition = new DcmTag(0x3002, 0x0034);

		/// <summary>(3002,0040) VR=SQ Fluence Map Sequence</summary>
		public static DcmTag FluenceMapSequence = new DcmTag(0x3002, 0x0040);

		/// <summary>(3002,0041) VR=CS Fluence Data Source</summary>
		public static DcmTag FluenceDataSource = new DcmTag(0x3002, 0x0041);

		/// <summary>(3002,0042) VR=DS Fluence Data Scale</summary>
		public static DcmTag FluenceDataScale = new DcmTag(0x3002, 0x0042);

		/// <summary>(3004,0001) VR=CS DVH Type</summary>
		public static DcmTag DVHType = new DcmTag(0x3004, 0x0001);

		/// <summary>(3004,0002) VR=CS Dose Units</summary>
		public static DcmTag DoseUnits = new DcmTag(0x3004, 0x0002);

		/// <summary>(3004,0004) VR=CS Dose Type</summary>
		public static DcmTag DoseType = new DcmTag(0x3004, 0x0004);

		/// <summary>(3004,0006) VR=LO Dose Comment</summary>
		public static DcmTag DoseComment = new DcmTag(0x3004, 0x0006);

		/// <summary>(3004,0008) VR=DS Normalization Point</summary>
		public static DcmTag NormalizationPoint = new DcmTag(0x3004, 0x0008);

		/// <summary>(3004,000a) VR=CS Dose Summation Type</summary>
		public static DcmTag DoseSummationType = new DcmTag(0x3004, 0x000a);

		/// <summary>(3004,000c) VR=DS Grid Frame Offset Vector</summary>
		public static DcmTag GridFrameOffsetVector = new DcmTag(0x3004, 0x000c);

		/// <summary>(3004,000e) VR=DS Dose Grid Scaling</summary>
		public static DcmTag DoseGridScaling = new DcmTag(0x3004, 0x000e);

		/// <summary>(3004,0010) VR=SQ RT Dose ROI Sequence</summary>
		public static DcmTag RTDoseROISequence = new DcmTag(0x3004, 0x0010);

		/// <summary>(3004,0012) VR=DS Dose Value</summary>
		public static DcmTag DoseValue = new DcmTag(0x3004, 0x0012);

		/// <summary>(3004,0014) VR=CS Tissue Heterogeneity Correction</summary>
		public static DcmTag TissueHeterogeneityCorrection = new DcmTag(0x3004, 0x0014);

		/// <summary>(3004,0040) VR=DS DVH Normalization Point</summary>
		public static DcmTag DVHNormalizationPoint = new DcmTag(0x3004, 0x0040);

		/// <summary>(3004,0042) VR=DS DVH Normalization Dose Value</summary>
		public static DcmTag DVHNormalizationDoseValue = new DcmTag(0x3004, 0x0042);

		/// <summary>(3004,0050) VR=SQ DVH Sequence</summary>
		public static DcmTag DVHSequence = new DcmTag(0x3004, 0x0050);

		/// <summary>(3004,0052) VR=DS DVH Dose Scaling</summary>
		public static DcmTag DVHDoseScaling = new DcmTag(0x3004, 0x0052);

		/// <summary>(3004,0054) VR=CS DVH Volume Units</summary>
		public static DcmTag DVHVolumeUnits = new DcmTag(0x3004, 0x0054);

		/// <summary>(3004,0056) VR=IS DVH Number of Bins</summary>
		public static DcmTag DVHNumberOfBins = new DcmTag(0x3004, 0x0056);

		/// <summary>(3004,0058) VR=DS DVH Data</summary>
		public static DcmTag DVHData = new DcmTag(0x3004, 0x0058);

		/// <summary>(3004,0060) VR=SQ DVH Referenced ROI Sequence</summary>
		public static DcmTag DVHReferencedROISequence = new DcmTag(0x3004, 0x0060);

		/// <summary>(3004,0062) VR=CS DVH ROI Contribution Type</summary>
		public static DcmTag DVHROIContributionType = new DcmTag(0x3004, 0x0062);

		/// <summary>(3004,0070) VR=DS DVH Minimum Dose</summary>
		public static DcmTag DVHMinimumDose = new DcmTag(0x3004, 0x0070);

		/// <summary>(3004,0072) VR=DS DVH Maximum Dose</summary>
		public static DcmTag DVHMaximumDose = new DcmTag(0x3004, 0x0072);

		/// <summary>(3004,0074) VR=DS DVH Mean Dose</summary>
		public static DcmTag DVHMeanDose = new DcmTag(0x3004, 0x0074);

		/// <summary>(3006,0002) VR=SH Structure Set Label</summary>
		public static DcmTag StructureSetLabel = new DcmTag(0x3006, 0x0002);

		/// <summary>(3006,0004) VR=LO Structure Set Name</summary>
		public static DcmTag StructureSetName = new DcmTag(0x3006, 0x0004);

		/// <summary>(3006,0006) VR=ST Structure Set Description</summary>
		public static DcmTag StructureSetDescription = new DcmTag(0x3006, 0x0006);

		/// <summary>(3006,0008) VR=DA Structure Set Date</summary>
		public static DcmTag StructureSetDate = new DcmTag(0x3006, 0x0008);

		/// <summary>(3006,0009) VR=TM Structure Set Time</summary>
		public static DcmTag StructureSetTime = new DcmTag(0x3006, 0x0009);

		/// <summary>(3006,0010) VR=SQ Referenced Frame of Reference Sequence</summary>
		public static DcmTag ReferencedFrameOfReferenceSequence = new DcmTag(0x3006, 0x0010);

		/// <summary>(3006,0012) VR=SQ RT Referenced Study Sequence</summary>
		public static DcmTag RTReferencedStudySequence = new DcmTag(0x3006, 0x0012);

		/// <summary>(3006,0014) VR=SQ RT Referenced Series Sequence</summary>
		public static DcmTag RTReferencedSeriesSequence = new DcmTag(0x3006, 0x0014);

		/// <summary>(3006,0016) VR=SQ Contour Image Sequence</summary>
		public static DcmTag ContourImageSequence = new DcmTag(0x3006, 0x0016);

		/// <summary>(3006,0020) VR=SQ Structure Set ROI Sequence</summary>
		public static DcmTag StructureSetROISequence = new DcmTag(0x3006, 0x0020);

		/// <summary>(3006,0022) VR=IS ROI Number</summary>
		public static DcmTag ROINumber = new DcmTag(0x3006, 0x0022);

		/// <summary>(3006,0024) VR=UI Referenced Frame of Reference UID</summary>
		public static DcmTag ReferencedFrameOfReferenceUID = new DcmTag(0x3006, 0x0024);

		/// <summary>(3006,0026) VR=LO ROI Name</summary>
		public static DcmTag ROIName = new DcmTag(0x3006, 0x0026);

		/// <summary>(3006,0028) VR=ST ROI Description</summary>
		public static DcmTag ROIDescription = new DcmTag(0x3006, 0x0028);

		/// <summary>(3006,002a) VR=IS ROI Display Color</summary>
		public static DcmTag ROIDisplayColor = new DcmTag(0x3006, 0x002a);

		/// <summary>(3006,002c) VR=DS ROI Volume</summary>
		public static DcmTag ROIVolume = new DcmTag(0x3006, 0x002c);

		/// <summary>(3006,0030) VR=SQ RT Related ROI Sequence</summary>
		public static DcmTag RTRelatedROISequence = new DcmTag(0x3006, 0x0030);

		/// <summary>(3006,0033) VR=CS RT ROI Relationship</summary>
		public static DcmTag RTROIRelationship = new DcmTag(0x3006, 0x0033);

		/// <summary>(3006,0036) VR=CS ROI Generation Algorithm</summary>
		public static DcmTag ROIGenerationAlgorithm = new DcmTag(0x3006, 0x0036);

		/// <summary>(3006,0038) VR=LO ROI Generation Description</summary>
		public static DcmTag ROIGenerationDescription = new DcmTag(0x3006, 0x0038);

		/// <summary>(3006,0039) VR=SQ ROI Contour Sequence</summary>
		public static DcmTag ROIContourSequence = new DcmTag(0x3006, 0x0039);

		/// <summary>(3006,0040) VR=SQ Contour Sequence</summary>
		public static DcmTag ContourSequence = new DcmTag(0x3006, 0x0040);

		/// <summary>(3006,0042) VR=CS Contour Geometric Type</summary>
		public static DcmTag ContourGeometricType = new DcmTag(0x3006, 0x0042);

		/// <summary>(3006,0044) VR=DS Contour Slab Thickness</summary>
		public static DcmTag ContourSlabThickness = new DcmTag(0x3006, 0x0044);

		/// <summary>(3006,0045) VR=DS Contour Offset Vector</summary>
		public static DcmTag ContourOffsetVector = new DcmTag(0x3006, 0x0045);

		/// <summary>(3006,0046) VR=IS Number of Contour Points</summary>
		public static DcmTag NumberOfContourPoints = new DcmTag(0x3006, 0x0046);

		/// <summary>(3006,0048) VR=IS Contour Number</summary>
		public static DcmTag ContourNumber = new DcmTag(0x3006, 0x0048);

		/// <summary>(3006,0049) VR=IS Attached Contours</summary>
		public static DcmTag AttachedContours = new DcmTag(0x3006, 0x0049);

		/// <summary>(3006,0050) VR=DS Contour Data</summary>
		public static DcmTag ContourData = new DcmTag(0x3006, 0x0050);

		/// <summary>(3006,0080) VR=SQ RT ROI Observations Sequence</summary>
		public static DcmTag RTROIObservationsSequence = new DcmTag(0x3006, 0x0080);

		/// <summary>(3006,0082) VR=IS Observation Number</summary>
		public static DcmTag ObservationNumber = new DcmTag(0x3006, 0x0082);

		/// <summary>(3006,0084) VR=IS Referenced ROI Number</summary>
		public static DcmTag ReferencedROINumber = new DcmTag(0x3006, 0x0084);

		/// <summary>(3006,0085) VR=SH ROI Observation Label</summary>
		public static DcmTag ROIObservationLabel = new DcmTag(0x3006, 0x0085);

		/// <summary>(3006,0086) VR=SQ RT ROI Identification Code Sequence</summary>
		public static DcmTag RTROIIdentificationCodeSequence = new DcmTag(0x3006, 0x0086);

		/// <summary>(3006,0088) VR=ST ROI Observation Description</summary>
		public static DcmTag ROIObservationDescription = new DcmTag(0x3006, 0x0088);

		/// <summary>(3006,00a0) VR=SQ Related RT ROI Observations Sequence</summary>
		public static DcmTag RelatedRTROIObservationsSequence = new DcmTag(0x3006, 0x00a0);

		/// <summary>(3006,00a4) VR=CS RT ROI Interpreted Type</summary>
		public static DcmTag RTROIInterpretedType = new DcmTag(0x3006, 0x00a4);

		/// <summary>(3006,00a6) VR=PN ROI Interpreter</summary>
		public static DcmTag ROIInterpreter = new DcmTag(0x3006, 0x00a6);

		/// <summary>(3006,00b0) VR=SQ ROI Physical Properties Sequence</summary>
		public static DcmTag ROIPhysicalPropertiesSequence = new DcmTag(0x3006, 0x00b0);

		/// <summary>(3006,00b2) VR=CS ROI Physical Property</summary>
		public static DcmTag ROIPhysicalProperty = new DcmTag(0x3006, 0x00b2);

		/// <summary>(3006,00b4) VR=DS ROI Physical Property Value</summary>
		public static DcmTag ROIPhysicalPropertyValue = new DcmTag(0x3006, 0x00b4);

		/// <summary>(3006,00c0) VR=SQ Frame of Reference Relationship Sequence</summary>
		public static DcmTag FrameOfReferenceRelationshipSequence = new DcmTag(0x3006, 0x00c0);

		/// <summary>(3006,00c2) VR=UI Related Frame of Reference UID</summary>
		public static DcmTag RelatedFrameOfReferenceUID = new DcmTag(0x3006, 0x00c2);

		/// <summary>(3006,00c4) VR=CS Frame of Reference Transformation Type</summary>
		public static DcmTag FrameOfReferenceTransformationType = new DcmTag(0x3006, 0x00c4);

		/// <summary>(3006,00c6) VR=DS Frame of Reference Transformation Matrix</summary>
		public static DcmTag FrameOfReferenceTransformationMatrix = new DcmTag(0x3006, 0x00c6);

		/// <summary>(3006,00c8) VR=LO Frame of Reference Transformation Comment</summary>
		public static DcmTag FrameOfReferenceTransformationComment = new DcmTag(0x3006, 0x00c8);

		/// <summary>(3008,0010) VR=SQ Measured Dose Reference Sequence</summary>
		public static DcmTag MeasuredDoseReferenceSequence = new DcmTag(0x3008, 0x0010);

		/// <summary>(3008,0012) VR=ST Measured Dose Description</summary>
		public static DcmTag MeasuredDoseDescription = new DcmTag(0x3008, 0x0012);

		/// <summary>(3008,0014) VR=CS Measured Dose Type</summary>
		public static DcmTag MeasuredDoseType = new DcmTag(0x3008, 0x0014);

		/// <summary>(3008,0016) VR=DS Measured Dose Value</summary>
		public static DcmTag MeasuredDoseValue = new DcmTag(0x3008, 0x0016);

		/// <summary>(3008,0020) VR=SQ Treatment Session Beam Sequence</summary>
		public static DcmTag TreatmentSessionBeamSequence = new DcmTag(0x3008, 0x0020);

		/// <summary>(3008,0021) VR=SQ Treatment Session Ion Beam Sequence</summary>
		public static DcmTag TreatmentSessionIonBeamSequence = new DcmTag(0x3008, 0x0021);

		/// <summary>(3008,0022) VR=IS Current Fraction Number</summary>
		public static DcmTag CurrentFractionNumber = new DcmTag(0x3008, 0x0022);

		/// <summary>(3008,0024) VR=DA Treatment Control Point Date</summary>
		public static DcmTag TreatmentControlPointDate = new DcmTag(0x3008, 0x0024);

		/// <summary>(3008,0025) VR=TM Treatment Control Point Time</summary>
		public static DcmTag TreatmentControlPointTime = new DcmTag(0x3008, 0x0025);

		/// <summary>(3008,002a) VR=CS Treatment Termination Status</summary>
		public static DcmTag TreatmentTerminationStatus = new DcmTag(0x3008, 0x002a);

		/// <summary>(3008,002b) VR=SH Treatment Termination Code</summary>
		public static DcmTag TreatmentTerminationCode = new DcmTag(0x3008, 0x002b);

		/// <summary>(3008,002c) VR=CS Treatment Verification Status</summary>
		public static DcmTag TreatmentVerificationStatus = new DcmTag(0x3008, 0x002c);

		/// <summary>(3008,0030) VR=SQ Referenced Treatment Record Sequence</summary>
		public static DcmTag ReferencedTreatmentRecordSequence = new DcmTag(0x3008, 0x0030);

		/// <summary>(3008,0032) VR=DS Specified Primary Meterset</summary>
		public static DcmTag SpecifiedPrimaryMeterset = new DcmTag(0x3008, 0x0032);

		/// <summary>(3008,0033) VR=DS Specified Secondary Meterset</summary>
		public static DcmTag SpecifiedSecondaryMeterset = new DcmTag(0x3008, 0x0033);

		/// <summary>(3008,0036) VR=DS Delivered Primary Meterset</summary>
		public static DcmTag DeliveredPrimaryMeterset = new DcmTag(0x3008, 0x0036);

		/// <summary>(3008,0037) VR=DS Delivered Secondary Meterset</summary>
		public static DcmTag DeliveredSecondaryMeterset = new DcmTag(0x3008, 0x0037);

		/// <summary>(3008,003a) VR=DS Specified Treatment Time</summary>
		public static DcmTag SpecifiedTreatmentTime = new DcmTag(0x3008, 0x003a);

		/// <summary>(3008,003b) VR=DS Delivered Treatment Time</summary>
		public static DcmTag DeliveredTreatmentTime = new DcmTag(0x3008, 0x003b);

		/// <summary>(3008,0040) VR=SQ Control Point Delivery Sequence</summary>
		public static DcmTag ControlPointDeliverySequence = new DcmTag(0x3008, 0x0040);

		/// <summary>(3008,0041) VR=SQ Ion Control Point Delivery Sequence</summary>
		public static DcmTag IonControlPointDeliverySequence = new DcmTag(0x3008, 0x0041);

		/// <summary>(3008,0042) VR=DS Specified Meterset</summary>
		public static DcmTag SpecifiedMeterset = new DcmTag(0x3008, 0x0042);

		/// <summary>(3008,0044) VR=DS Delivered Meterset</summary>
		public static DcmTag DeliveredMeterset = new DcmTag(0x3008, 0x0044);

		/// <summary>(3008,0045) VR=FL Meterset Rate Set</summary>
		public static DcmTag MetersetRateSet = new DcmTag(0x3008, 0x0045);

		/// <summary>(3008,0046) VR=FL Meterset Rate Delivered</summary>
		public static DcmTag MetersetRateDelivered = new DcmTag(0x3008, 0x0046);

		/// <summary>(3008,0047) VR=FL Scan Spot Metersets Delivered</summary>
		public static DcmTag ScanSpotMetersetsDelivered = new DcmTag(0x3008, 0x0047);

		/// <summary>(3008,0048) VR=DS Dose Rate Delivered</summary>
		public static DcmTag DoseRateDelivered = new DcmTag(0x3008, 0x0048);

		/// <summary>(3008,0050) VR=SQ Treatment Summary Calculated Dose Reference Sequence</summary>
		public static DcmTag TreatmentSummaryCalculatedDoseReferenceSequence = new DcmTag(0x3008, 0x0050);

		/// <summary>(3008,0052) VR=DS Cumulative Dose to Dose Reference</summary>
		public static DcmTag CumulativeDoseToDoseReference = new DcmTag(0x3008, 0x0052);

		/// <summary>(3008,0054) VR=DA First Treatment Date</summary>
		public static DcmTag FirstTreatmentDate = new DcmTag(0x3008, 0x0054);

		/// <summary>(3008,0056) VR=DA Most Recent Treatment Date</summary>
		public static DcmTag MostRecentTreatmentDate = new DcmTag(0x3008, 0x0056);

		/// <summary>(3008,005a) VR=IS Number of Fractions Delivered</summary>
		public static DcmTag NumberOfFractionsDelivered = new DcmTag(0x3008, 0x005a);

		/// <summary>(3008,0060) VR=SQ Override Sequence</summary>
		public static DcmTag OverrideSequence = new DcmTag(0x3008, 0x0060);

		/// <summary>(3008,0061) VR=AT Parameter Sequence Pointer</summary>
		public static DcmTag ParameterSequencePointer = new DcmTag(0x3008, 0x0061);

		/// <summary>(3008,0062) VR=AT Override Parameter Pointer</summary>
		public static DcmTag OverrideParameterPointer = new DcmTag(0x3008, 0x0062);

		/// <summary>(3008,0063) VR=IS Parameter Item Index</summary>
		public static DcmTag ParameterItemIndex = new DcmTag(0x3008, 0x0063);

		/// <summary>(3008,0064) VR=IS Measured Dose Reference Number</summary>
		public static DcmTag MeasuredDoseReferenceNumber = new DcmTag(0x3008, 0x0064);

		/// <summary>(3008,0065) VR=AT Parameter Pointer</summary>
		public static DcmTag ParameterPointer = new DcmTag(0x3008, 0x0065);

		/// <summary>(3008,0066) VR=ST Override Reason</summary>
		public static DcmTag OverrideReason = new DcmTag(0x3008, 0x0066);

		/// <summary>(3008,0068) VR=SQ Corrected Parameter Sequence</summary>
		public static DcmTag CorrectedParameterSequence = new DcmTag(0x3008, 0x0068);

		/// <summary>(3008,006a) VR=FL Correction Value</summary>
		public static DcmTag CorrectionValue = new DcmTag(0x3008, 0x006a);

		/// <summary>(3008,0070) VR=SQ Calculated Dose Reference Sequence</summary>
		public static DcmTag CalculatedDoseReferenceSequence = new DcmTag(0x3008, 0x0070);

		/// <summary>(3008,0072) VR=IS Calculated Dose Reference Number</summary>
		public static DcmTag CalculatedDoseReferenceNumber = new DcmTag(0x3008, 0x0072);

		/// <summary>(3008,0074) VR=ST Calculated Dose Reference Description</summary>
		public static DcmTag CalculatedDoseReferenceDescription = new DcmTag(0x3008, 0x0074);

		/// <summary>(3008,0076) VR=DS Calculated Dose Reference Dose Value</summary>
		public static DcmTag CalculatedDoseReferenceDoseValue = new DcmTag(0x3008, 0x0076);

		/// <summary>(3008,0078) VR=DS Start Meterset</summary>
		public static DcmTag StartMeterset = new DcmTag(0x3008, 0x0078);

		/// <summary>(3008,007a) VR=DS End Meterset</summary>
		public static DcmTag EndMeterset = new DcmTag(0x3008, 0x007a);

		/// <summary>(3008,0080) VR=SQ Referenced Measured Dose Reference Sequence</summary>
		public static DcmTag ReferencedMeasuredDoseReferenceSequence = new DcmTag(0x3008, 0x0080);

		/// <summary>(3008,0082) VR=IS Referenced Measured Dose Reference Number</summary>
		public static DcmTag ReferencedMeasuredDoseReferenceNumber = new DcmTag(0x3008, 0x0082);

		/// <summary>(3008,0090) VR=SQ Referenced Calculated Dose Reference Sequence</summary>
		public static DcmTag ReferencedCalculatedDoseReferenceSequence = new DcmTag(0x3008, 0x0090);

		/// <summary>(3008,0092) VR=IS Referenced Calculated Dose Reference Number</summary>
		public static DcmTag ReferencedCalculatedDoseReferenceNumber = new DcmTag(0x3008, 0x0092);

		/// <summary>(3008,00a0) VR=SQ Beam Limiting Device Leaf Pairs Sequence</summary>
		public static DcmTag BeamLimitingDeviceLeafPairsSequence = new DcmTag(0x3008, 0x00a0);

		/// <summary>(3008,00b0) VR=SQ Recorded Wedge Sequence</summary>
		public static DcmTag RecordedWedgeSequence = new DcmTag(0x3008, 0x00b0);

		/// <summary>(3008,00c0) VR=SQ Recorded Compensator Sequence</summary>
		public static DcmTag RecordedCompensatorSequence = new DcmTag(0x3008, 0x00c0);

		/// <summary>(3008,00d0) VR=SQ Recorded Block Sequence</summary>
		public static DcmTag RecordedBlockSequence = new DcmTag(0x3008, 0x00d0);

		/// <summary>(3008,00e0) VR=SQ Treatment Summary Measured Dose Reference Sequence</summary>
		public static DcmTag TreatmentSummaryMeasuredDoseReferenceSequence = new DcmTag(0x3008, 0x00e0);

		/// <summary>(3008,00f0) VR=SQ Recorded Snout Sequence</summary>
		public static DcmTag RecordedSnoutSequence = new DcmTag(0x3008, 0x00f0);

		/// <summary>(3008,00f2) VR=SQ Recorded Range Shifter Sequence</summary>
		public static DcmTag RecordedRangeShifterSequence = new DcmTag(0x3008, 0x00f2);

		/// <summary>(3008,00f4) VR=SQ Recorded Lateral Spreading Device Sequence</summary>
		public static DcmTag RecordedLateralSpreadingDeviceSequence = new DcmTag(0x3008, 0x00f4);

		/// <summary>(3008,00f6) VR=SQ Recorded Range Modulator Sequence</summary>
		public static DcmTag RecordedRangeModulatorSequence = new DcmTag(0x3008, 0x00f6);

		/// <summary>(3008,0100) VR=SQ Recorded Source Sequence</summary>
		public static DcmTag RecordedSourceSequence = new DcmTag(0x3008, 0x0100);

		/// <summary>(3008,0105) VR=LO Source Serial Number</summary>
		public static DcmTag SourceSerialNumber = new DcmTag(0x3008, 0x0105);

		/// <summary>(3008,0110) VR=SQ Treatment Session Application Setup Sequence</summary>
		public static DcmTag TreatmentSessionApplicationSetupSequence = new DcmTag(0x3008, 0x0110);

		/// <summary>(3008,0116) VR=CS Application Setup Check</summary>
		public static DcmTag ApplicationSetupCheck = new DcmTag(0x3008, 0x0116);

		/// <summary>(3008,0120) VR=SQ Recorded Brachy Accessory Device Sequence</summary>
		public static DcmTag RecordedBrachyAccessoryDeviceSequence = new DcmTag(0x3008, 0x0120);

		/// <summary>(3008,0122) VR=IS Referenced Brachy Accessory Device Number</summary>
		public static DcmTag ReferencedBrachyAccessoryDeviceNumber = new DcmTag(0x3008, 0x0122);

		/// <summary>(3008,0130) VR=SQ Recorded Channel Sequence</summary>
		public static DcmTag RecordedChannelSequence = new DcmTag(0x3008, 0x0130);

		/// <summary>(3008,0132) VR=DS Specified Channel Total Time</summary>
		public static DcmTag SpecifiedChannelTotalTime = new DcmTag(0x3008, 0x0132);

		/// <summary>(3008,0134) VR=DS Delivered Channel Total Time</summary>
		public static DcmTag DeliveredChannelTotalTime = new DcmTag(0x3008, 0x0134);

		/// <summary>(3008,0136) VR=IS Specified Number of Pulses</summary>
		public static DcmTag SpecifiedNumberOfPulses = new DcmTag(0x3008, 0x0136);

		/// <summary>(3008,0138) VR=IS Delivered Number of Pulses</summary>
		public static DcmTag DeliveredNumberOfPulses = new DcmTag(0x3008, 0x0138);

		/// <summary>(3008,013a) VR=DS Specified Pulse Repetition Interval</summary>
		public static DcmTag SpecifiedPulseRepetitionInterval = new DcmTag(0x3008, 0x013a);

		/// <summary>(3008,013c) VR=DS Delivered Pulse Repetition Interval</summary>
		public static DcmTag DeliveredPulseRepetitionInterval = new DcmTag(0x3008, 0x013c);

		/// <summary>(3008,0140) VR=SQ Recorded Source Applicator Sequence</summary>
		public static DcmTag RecordedSourceApplicatorSequence = new DcmTag(0x3008, 0x0140);

		/// <summary>(3008,0142) VR=IS Referenced Source Applicator Number</summary>
		public static DcmTag ReferencedSourceApplicatorNumber = new DcmTag(0x3008, 0x0142);

		/// <summary>(3008,0150) VR=SQ Recorded Channel Shield Sequence</summary>
		public static DcmTag RecordedChannelShieldSequence = new DcmTag(0x3008, 0x0150);

		/// <summary>(3008,0152) VR=IS Referenced Channel Shield Number</summary>
		public static DcmTag ReferencedChannelShieldNumber = new DcmTag(0x3008, 0x0152);

		/// <summary>(3008,0160) VR=SQ Brachy Control Point Delivered Sequence</summary>
		public static DcmTag BrachyControlPointDeliveredSequence = new DcmTag(0x3008, 0x0160);

		/// <summary>(3008,0162) VR=DA Safe Position Exit Date</summary>
		public static DcmTag SafePositionExitDate = new DcmTag(0x3008, 0x0162);

		/// <summary>(3008,0164) VR=TM Safe Position Exit Time</summary>
		public static DcmTag SafePositionExitTime = new DcmTag(0x3008, 0x0164);

		/// <summary>(3008,0166) VR=DA Safe Position Return Date</summary>
		public static DcmTag SafePositionReturnDate = new DcmTag(0x3008, 0x0166);

		/// <summary>(3008,0168) VR=TM Safe Position Return Time</summary>
		public static DcmTag SafePositionReturnTime = new DcmTag(0x3008, 0x0168);

		/// <summary>(3008,0200) VR=CS Current Treatment Status</summary>
		public static DcmTag CurrentTreatmentStatus = new DcmTag(0x3008, 0x0200);

		/// <summary>(3008,0202) VR=ST Treatment Status Comment</summary>
		public static DcmTag TreatmentStatusComment = new DcmTag(0x3008, 0x0202);

		/// <summary>(3008,0220) VR=SQ Fraction Group Summary Sequence</summary>
		public static DcmTag FractionGroupSummarySequence = new DcmTag(0x3008, 0x0220);

		/// <summary>(3008,0223) VR=IS Referenced Fraction Number</summary>
		public static DcmTag ReferencedFractionNumber = new DcmTag(0x3008, 0x0223);

		/// <summary>(3008,0224) VR=CS Fraction Group Type</summary>
		public static DcmTag FractionGroupType = new DcmTag(0x3008, 0x0224);

		/// <summary>(3008,0230) VR=CS Beam Stopper Position</summary>
		public static DcmTag BeamStopperPosition = new DcmTag(0x3008, 0x0230);

		/// <summary>(3008,0240) VR=SQ Fraction Status Summary Sequence</summary>
		public static DcmTag FractionStatusSummarySequence = new DcmTag(0x3008, 0x0240);

		/// <summary>(3008,0250) VR=DA Treatment Date</summary>
		public static DcmTag TreatmentDate = new DcmTag(0x3008, 0x0250);

		/// <summary>(3008,0251) VR=TM Treatment Time</summary>
		public static DcmTag TreatmentTime = new DcmTag(0x3008, 0x0251);

		/// <summary>(300a,0002) VR=SH RT Plan Label</summary>
		public static DcmTag RTPlanLabel = new DcmTag(0x300a, 0x0002);

		/// <summary>(300a,0003) VR=LO RT Plan Name</summary>
		public static DcmTag RTPlanName = new DcmTag(0x300a, 0x0003);

		/// <summary>(300a,0004) VR=ST RT Plan Description</summary>
		public static DcmTag RTPlanDescription = new DcmTag(0x300a, 0x0004);

		/// <summary>(300a,0006) VR=DA RT Plan Date</summary>
		public static DcmTag RTPlanDate = new DcmTag(0x300a, 0x0006);

		/// <summary>(300a,0007) VR=TM RT Plan Time</summary>
		public static DcmTag RTPlanTime = new DcmTag(0x300a, 0x0007);

		/// <summary>(300a,0009) VR=LO Treatment Protocols</summary>
		public static DcmTag TreatmentProtocols = new DcmTag(0x300a, 0x0009);

		/// <summary>(300a,000a) VR=CS Plan Intent</summary>
		public static DcmTag PlanIntent = new DcmTag(0x300a, 0x000a);

		/// <summary>(300a,000b) VR=LO Treatment Sites</summary>
		public static DcmTag TreatmentSites = new DcmTag(0x300a, 0x000b);

		/// <summary>(300a,000c) VR=CS RT Plan Geometry</summary>
		public static DcmTag RTPlanGeometry = new DcmTag(0x300a, 0x000c);

		/// <summary>(300a,000e) VR=ST Prescription Description</summary>
		public static DcmTag PrescriptionDescription = new DcmTag(0x300a, 0x000e);

		/// <summary>(300a,0010) VR=SQ Dose Reference Sequence</summary>
		public static DcmTag DoseReferenceSequence = new DcmTag(0x300a, 0x0010);

		/// <summary>(300a,0012) VR=IS Dose Reference Number</summary>
		public static DcmTag DoseReferenceNumber = new DcmTag(0x300a, 0x0012);

		/// <summary>(300a,0013) VR=UI Dose Reference UID</summary>
		public static DcmTag DoseReferenceUID = new DcmTag(0x300a, 0x0013);

		/// <summary>(300a,0014) VR=CS Dose Reference Structure Type</summary>
		public static DcmTag DoseReferenceStructureType = new DcmTag(0x300a, 0x0014);

		/// <summary>(300a,0015) VR=CS Nominal Beam Energy Unit</summary>
		public static DcmTag NominalBeamEnergyUnit = new DcmTag(0x300a, 0x0015);

		/// <summary>(300a,0016) VR=LO Dose Reference Description</summary>
		public static DcmTag DoseReferenceDescription = new DcmTag(0x300a, 0x0016);

		/// <summary>(300a,0018) VR=DS Dose Reference Point Coordinates</summary>
		public static DcmTag DoseReferencePointCoordinates = new DcmTag(0x300a, 0x0018);

		/// <summary>(300a,001a) VR=DS Nominal Prior Dose</summary>
		public static DcmTag NominalPriorDose = new DcmTag(0x300a, 0x001a);

		/// <summary>(300a,0020) VR=CS Dose Reference Type</summary>
		public static DcmTag DoseReferenceType = new DcmTag(0x300a, 0x0020);

		/// <summary>(300a,0021) VR=DS Constraint Weight</summary>
		public static DcmTag ConstraintWeight = new DcmTag(0x300a, 0x0021);

		/// <summary>(300a,0022) VR=DS Delivery Warning Dose</summary>
		public static DcmTag DeliveryWarningDose = new DcmTag(0x300a, 0x0022);

		/// <summary>(300a,0023) VR=DS Delivery Maximum Dose</summary>
		public static DcmTag DeliveryMaximumDose = new DcmTag(0x300a, 0x0023);

		/// <summary>(300a,0025) VR=DS Target Minimum Dose</summary>
		public static DcmTag TargetMinimumDose = new DcmTag(0x300a, 0x0025);

		/// <summary>(300a,0026) VR=DS Target Prescription Dose</summary>
		public static DcmTag TargetPrescriptionDose = new DcmTag(0x300a, 0x0026);

		/// <summary>(300a,0027) VR=DS Target Maximum Dose</summary>
		public static DcmTag TargetMaximumDose = new DcmTag(0x300a, 0x0027);

		/// <summary>(300a,0028) VR=DS Target Underdose Volume Fraction</summary>
		public static DcmTag TargetUnderdoseVolumeFraction = new DcmTag(0x300a, 0x0028);

		/// <summary>(300a,002a) VR=DS Organ at Risk Full-volume Dose</summary>
		public static DcmTag OrganAtRiskFullvolumeDose = new DcmTag(0x300a, 0x002a);

		/// <summary>(300a,002b) VR=DS Organ at Risk Limit Dose</summary>
		public static DcmTag OrganAtRiskLimitDose = new DcmTag(0x300a, 0x002b);

		/// <summary>(300a,002c) VR=DS Organ at Risk Maximum Dose</summary>
		public static DcmTag OrganAtRiskMaximumDose = new DcmTag(0x300a, 0x002c);

		/// <summary>(300a,002d) VR=DS Organ at Risk Overdose Volume Fraction</summary>
		public static DcmTag OrganAtRiskOverdoseVolumeFraction = new DcmTag(0x300a, 0x002d);

		/// <summary>(300a,0040) VR=SQ Tolerance Table Sequence</summary>
		public static DcmTag ToleranceTableSequence = new DcmTag(0x300a, 0x0040);

		/// <summary>(300a,0042) VR=IS Tolerance Table Number</summary>
		public static DcmTag ToleranceTableNumber = new DcmTag(0x300a, 0x0042);

		/// <summary>(300a,0043) VR=SH Tolerance Table Label</summary>
		public static DcmTag ToleranceTableLabel = new DcmTag(0x300a, 0x0043);

		/// <summary>(300a,0044) VR=DS Gantry Angle Tolerance</summary>
		public static DcmTag GantryAngleTolerance = new DcmTag(0x300a, 0x0044);

		/// <summary>(300a,0046) VR=DS Beam Limiting Device Angle Tolerance</summary>
		public static DcmTag BeamLimitingDeviceAngleTolerance = new DcmTag(0x300a, 0x0046);

		/// <summary>(300a,0048) VR=SQ Beam Limiting Device Tolerance Sequence</summary>
		public static DcmTag BeamLimitingDeviceToleranceSequence = new DcmTag(0x300a, 0x0048);

		/// <summary>(300a,004a) VR=DS Beam Limiting Device Position Tolerance</summary>
		public static DcmTag BeamLimitingDevicePositionTolerance = new DcmTag(0x300a, 0x004a);

		/// <summary>(300a,004b) VR=FL Snout Position Tolerance</summary>
		public static DcmTag SnoutPositionTolerance = new DcmTag(0x300a, 0x004b);

		/// <summary>(300a,004c) VR=DS Patient Support Angle Tolerance</summary>
		public static DcmTag PatientSupportAngleTolerance = new DcmTag(0x300a, 0x004c);

		/// <summary>(300a,004e) VR=DS Table Top Eccentric Angle Tolerance</summary>
		public static DcmTag TableTopEccentricAngleTolerance = new DcmTag(0x300a, 0x004e);

		/// <summary>(300a,004f) VR=FL Table Top Pitch Angle Tolerance</summary>
		public static DcmTag TableTopPitchAngleTolerance = new DcmTag(0x300a, 0x004f);

		/// <summary>(300a,0050) VR=FL Table Top Roll Angle Tolerance</summary>
		public static DcmTag TableTopRollAngleTolerance = new DcmTag(0x300a, 0x0050);

		/// <summary>(300a,0051) VR=DS Table Top Vertical Position Tolerance</summary>
		public static DcmTag TableTopVerticalPositionTolerance = new DcmTag(0x300a, 0x0051);

		/// <summary>(300a,0052) VR=DS Table Top Longitudinal Position Tolerance</summary>
		public static DcmTag TableTopLongitudinalPositionTolerance = new DcmTag(0x300a, 0x0052);

		/// <summary>(300a,0053) VR=DS Table Top Lateral Position Tolerance</summary>
		public static DcmTag TableTopLateralPositionTolerance = new DcmTag(0x300a, 0x0053);

		/// <summary>(300a,0055) VR=CS RT Plan Relationship</summary>
		public static DcmTag RTPlanRelationship = new DcmTag(0x300a, 0x0055);

		/// <summary>(300a,0070) VR=SQ Fraction Group Sequence</summary>
		public static DcmTag FractionGroupSequence = new DcmTag(0x300a, 0x0070);

		/// <summary>(300a,0071) VR=IS Fraction Group Number</summary>
		public static DcmTag FractionGroupNumber = new DcmTag(0x300a, 0x0071);

		/// <summary>(300a,0072) VR=LO Fraction Group Description</summary>
		public static DcmTag FractionGroupDescription = new DcmTag(0x300a, 0x0072);

		/// <summary>(300a,0078) VR=IS Number of Fractions Planned</summary>
		public static DcmTag NumberOfFractionsPlanned = new DcmTag(0x300a, 0x0078);

		/// <summary>(300a,0079) VR=IS Number of Fraction Pattern Digits Per Day</summary>
		public static DcmTag NumberOfFractionPatternDigitsPerDay = new DcmTag(0x300a, 0x0079);

		/// <summary>(300a,007a) VR=IS Repeat Fraction Cycle Length</summary>
		public static DcmTag RepeatFractionCycleLength = new DcmTag(0x300a, 0x007a);

		/// <summary>(300a,007b) VR=LT Fraction Pattern</summary>
		public static DcmTag FractionPattern = new DcmTag(0x300a, 0x007b);

		/// <summary>(300a,0080) VR=IS Number of Beams</summary>
		public static DcmTag NumberOfBeams = new DcmTag(0x300a, 0x0080);

		/// <summary>(300a,0082) VR=DS Beam Dose Specification Point</summary>
		public static DcmTag BeamDoseSpecificationPoint = new DcmTag(0x300a, 0x0082);

		/// <summary>(300a,0084) VR=DS Beam Dose</summary>
		public static DcmTag BeamDose = new DcmTag(0x300a, 0x0084);

		/// <summary>(300a,0086) VR=DS Beam Meterset</summary>
		public static DcmTag BeamMeterset = new DcmTag(0x300a, 0x0086);

		/// <summary>(300a,0088) VR=FL Beam Dose Point Depth</summary>
		public static DcmTag BeamDosePointDepth = new DcmTag(0x300a, 0x0088);

		/// <summary>(300a,0089) VR=FL Beam Dose Point Equivalent Depth</summary>
		public static DcmTag BeamDosePointEquivalentDepth = new DcmTag(0x300a, 0x0089);

		/// <summary>(300a,008a) VR=FL Beam Dose Point SSD</summary>
		public static DcmTag BeamDosePointSSD = new DcmTag(0x300a, 0x008a);

		/// <summary>(300a,00a0) VR=IS Number of Brachy Application Setups</summary>
		public static DcmTag NumberOfBrachyApplicationSetups = new DcmTag(0x300a, 0x00a0);

		/// <summary>(300a,00a2) VR=DS Brachy Application Setup Dose Specification Point</summary>
		public static DcmTag BrachyApplicationSetupDoseSpecificationPoint = new DcmTag(0x300a, 0x00a2);

		/// <summary>(300a,00a4) VR=DS Brachy Application Setup Dose</summary>
		public static DcmTag BrachyApplicationSetupDose = new DcmTag(0x300a, 0x00a4);

		/// <summary>(300a,00b0) VR=SQ Beam Sequence</summary>
		public static DcmTag BeamSequence = new DcmTag(0x300a, 0x00b0);

		/// <summary>(300a,00b2) VR=SH Treatment Machine Name</summary>
		public static DcmTag TreatmentMachineName = new DcmTag(0x300a, 0x00b2);

		/// <summary>(300a,00b3) VR=CS Primary Dosimeter Unit</summary>
		public static DcmTag PrimaryDosimeterUnit = new DcmTag(0x300a, 0x00b3);

		/// <summary>(300a,00b4) VR=DS Source-Axis Distance</summary>
		public static DcmTag SourceAxisDistance = new DcmTag(0x300a, 0x00b4);

		/// <summary>(300a,00b6) VR=SQ Beam Limiting Device Sequence</summary>
		public static DcmTag BeamLimitingDeviceSequence = new DcmTag(0x300a, 0x00b6);

		/// <summary>(300a,00b8) VR=CS RT Beam Limiting Device Type</summary>
		public static DcmTag RTBeamLimitingDeviceType = new DcmTag(0x300a, 0x00b8);

		/// <summary>(300a,00ba) VR=DS Source to Beam Limiting Device Distance</summary>
		public static DcmTag SourceToBeamLimitingDeviceDistance = new DcmTag(0x300a, 0x00ba);

		/// <summary>(300a,00bb) VR=FL Isocenter to Beam Limiting Device Distance</summary>
		public static DcmTag IsocenterToBeamLimitingDeviceDistance = new DcmTag(0x300a, 0x00bb);

		/// <summary>(300a,00bc) VR=IS Number of Leaf/Jaw Pairs</summary>
		public static DcmTag NumberOfLeafJawPairs = new DcmTag(0x300a, 0x00bc);

		/// <summary>(300a,00be) VR=DS Leaf Position Boundaries</summary>
		public static DcmTag LeafPositionBoundaries = new DcmTag(0x300a, 0x00be);

		/// <summary>(300a,00c0) VR=IS Beam Number</summary>
		public static DcmTag BeamNumber = new DcmTag(0x300a, 0x00c0);

		/// <summary>(300a,00c2) VR=LO Beam Name</summary>
		public static DcmTag BeamName = new DcmTag(0x300a, 0x00c2);

		/// <summary>(300a,00c3) VR=ST Beam Description</summary>
		public static DcmTag BeamDescription = new DcmTag(0x300a, 0x00c3);

		/// <summary>(300a,00c4) VR=CS Beam Type</summary>
		public static DcmTag BeamType = new DcmTag(0x300a, 0x00c4);

		/// <summary>(300a,00c6) VR=CS Radiation Type</summary>
		public static DcmTag RadiationType = new DcmTag(0x300a, 0x00c6);

		/// <summary>(300a,00c7) VR=CS High-Dose Technique Type</summary>
		public static DcmTag HighDoseTechniqueType = new DcmTag(0x300a, 0x00c7);

		/// <summary>(300a,00c8) VR=IS Reference Image Number</summary>
		public static DcmTag ReferenceImageNumber = new DcmTag(0x300a, 0x00c8);

		/// <summary>(300a,00ca) VR=SQ Planned Verification Image Sequence</summary>
		public static DcmTag PlannedVerificationImageSequence = new DcmTag(0x300a, 0x00ca);

		/// <summary>(300a,00cc) VR=LO Imaging Device-Specific Acquisition Parameters</summary>
		public static DcmTag ImagingDeviceSpecificAcquisitionParameters = new DcmTag(0x300a, 0x00cc);

		/// <summary>(300a,00ce) VR=CS Treatment Delivery Type</summary>
		public static DcmTag TreatmentDeliveryType = new DcmTag(0x300a, 0x00ce);

		/// <summary>(300a,00d0) VR=IS Number of Wedges</summary>
		public static DcmTag NumberOfWedges = new DcmTag(0x300a, 0x00d0);

		/// <summary>(300a,00d1) VR=SQ Wedge Sequence</summary>
		public static DcmTag WedgeSequence = new DcmTag(0x300a, 0x00d1);

		/// <summary>(300a,00d2) VR=IS Wedge Number</summary>
		public static DcmTag WedgeNumber = new DcmTag(0x300a, 0x00d2);

		/// <summary>(300a,00d3) VR=CS Wedge Type</summary>
		public static DcmTag WedgeType = new DcmTag(0x300a, 0x00d3);

		/// <summary>(300a,00d4) VR=SH Wedge ID</summary>
		public static DcmTag WedgeID = new DcmTag(0x300a, 0x00d4);

		/// <summary>(300a,00d5) VR=IS Wedge Angle</summary>
		public static DcmTag WedgeAngle = new DcmTag(0x300a, 0x00d5);

		/// <summary>(300a,00d6) VR=DS Wedge Factor</summary>
		public static DcmTag WedgeFactor = new DcmTag(0x300a, 0x00d6);

		/// <summary>(300a,00d7) VR=FL Total Wedge Tray Water-Equivalent Thickness</summary>
		public static DcmTag TotalWedgeTrayWaterEquivalentThickness = new DcmTag(0x300a, 0x00d7);

		/// <summary>(300a,00d8) VR=DS Wedge Orientation</summary>
		public static DcmTag WedgeOrientation = new DcmTag(0x300a, 0x00d8);

		/// <summary>(300a,00d9) VR=FL Isocenter to Wedge Tray Distance</summary>
		public static DcmTag IsocenterToWedgeTrayDistance = new DcmTag(0x300a, 0x00d9);

		/// <summary>(300a,00da) VR=DS Source to Wedge Tray Distance</summary>
		public static DcmTag SourceToWedgeTrayDistance = new DcmTag(0x300a, 0x00da);

		/// <summary>(300a,00db) VR=FL Wedge Thin Edge Position</summary>
		public static DcmTag WedgeThinEdgePosition = new DcmTag(0x300a, 0x00db);

		/// <summary>(300a,00dc) VR=SH Bolus ID</summary>
		public static DcmTag BolusID = new DcmTag(0x300a, 0x00dc);

		/// <summary>(300a,00dd) VR=ST Bolus Description</summary>
		public static DcmTag BolusDescription = new DcmTag(0x300a, 0x00dd);

		/// <summary>(300a,00e0) VR=IS Number of Compensators</summary>
		public static DcmTag NumberOfCompensators = new DcmTag(0x300a, 0x00e0);

		/// <summary>(300a,00e1) VR=SH Material ID</summary>
		public static DcmTag MaterialID = new DcmTag(0x300a, 0x00e1);

		/// <summary>(300a,00e2) VR=DS Total Compensator Tray Factor</summary>
		public static DcmTag TotalCompensatorTrayFactor = new DcmTag(0x300a, 0x00e2);

		/// <summary>(300a,00e3) VR=SQ Compensator Sequence</summary>
		public static DcmTag CompensatorSequence = new DcmTag(0x300a, 0x00e3);

		/// <summary>(300a,00e4) VR=IS Compensator Number</summary>
		public static DcmTag CompensatorNumber = new DcmTag(0x300a, 0x00e4);

		/// <summary>(300a,00e5) VR=SH Compensator ID</summary>
		public static DcmTag CompensatorID = new DcmTag(0x300a, 0x00e5);

		/// <summary>(300a,00e6) VR=DS Source to Compensator Tray Distance</summary>
		public static DcmTag SourceToCompensatorTrayDistance = new DcmTag(0x300a, 0x00e6);

		/// <summary>(300a,00e7) VR=IS Compensator Rows</summary>
		public static DcmTag CompensatorRows = new DcmTag(0x300a, 0x00e7);

		/// <summary>(300a,00e8) VR=IS Compensator Columns</summary>
		public static DcmTag CompensatorColumns = new DcmTag(0x300a, 0x00e8);

		/// <summary>(300a,00e9) VR=DS Compensator Pixel Spacing</summary>
		public static DcmTag CompensatorPixelSpacing = new DcmTag(0x300a, 0x00e9);

		/// <summary>(300a,00ea) VR=DS Compensator Position</summary>
		public static DcmTag CompensatorPosition = new DcmTag(0x300a, 0x00ea);

		/// <summary>(300a,00eb) VR=DS Compensator Transmission Data</summary>
		public static DcmTag CompensatorTransmissionData = new DcmTag(0x300a, 0x00eb);

		/// <summary>(300a,00ec) VR=DS Compensator Thickness Data</summary>
		public static DcmTag CompensatorThicknessData = new DcmTag(0x300a, 0x00ec);

		/// <summary>(300a,00ed) VR=IS Number of Boli</summary>
		public static DcmTag NumberOfBoli = new DcmTag(0x300a, 0x00ed);

		/// <summary>(300a,00ee) VR=CS Compensator Type</summary>
		public static DcmTag CompensatorType = new DcmTag(0x300a, 0x00ee);

		/// <summary>(300a,00f0) VR=IS Number of Blocks</summary>
		public static DcmTag NumberOfBlocks = new DcmTag(0x300a, 0x00f0);

		/// <summary>(300a,00f2) VR=DS Total Block Tray Factor</summary>
		public static DcmTag TotalBlockTrayFactor = new DcmTag(0x300a, 0x00f2);

		/// <summary>(300a,00f3) VR=FL Total Block Tray Water-Equivalent Thickness</summary>
		public static DcmTag TotalBlockTrayWaterEquivalentThickness = new DcmTag(0x300a, 0x00f3);

		/// <summary>(300a,00f4) VR=SQ Block Sequence</summary>
		public static DcmTag BlockSequence = new DcmTag(0x300a, 0x00f4);

		/// <summary>(300a,00f5) VR=SH Block Tray ID</summary>
		public static DcmTag BlockTrayID = new DcmTag(0x300a, 0x00f5);

		/// <summary>(300a,00f6) VR=DS Source to Block Tray Distance</summary>
		public static DcmTag SourceToBlockTrayDistance = new DcmTag(0x300a, 0x00f6);

		/// <summary>(300a,00f7) VR=FL Isocenter to Block Tray Distance</summary>
		public static DcmTag IsocenterToBlockTrayDistance = new DcmTag(0x300a, 0x00f7);

		/// <summary>(300a,00f8) VR=CS Block Type</summary>
		public static DcmTag BlockType = new DcmTag(0x300a, 0x00f8);

		/// <summary>(300a,00f9) VR=LO Accessory Code</summary>
		public static DcmTag AccessoryCode = new DcmTag(0x300a, 0x00f9);

		/// <summary>(300a,00fa) VR=CS Block Divergence</summary>
		public static DcmTag BlockDivergence = new DcmTag(0x300a, 0x00fa);

		/// <summary>(300a,00fb) VR=CS Block Mounting Position</summary>
		public static DcmTag BlockMountingPosition = new DcmTag(0x300a, 0x00fb);

		/// <summary>(300a,00fc) VR=IS Block Number</summary>
		public static DcmTag BlockNumber = new DcmTag(0x300a, 0x00fc);

		/// <summary>(300a,00fe) VR=LO Block Name</summary>
		public static DcmTag BlockName = new DcmTag(0x300a, 0x00fe);

		/// <summary>(300a,0100) VR=DS Block Thickness</summary>
		public static DcmTag BlockThickness = new DcmTag(0x300a, 0x0100);

		/// <summary>(300a,0102) VR=DS Block Transmission</summary>
		public static DcmTag BlockTransmission = new DcmTag(0x300a, 0x0102);

		/// <summary>(300a,0104) VR=IS Block Number of Points</summary>
		public static DcmTag BlockNumberOfPoints = new DcmTag(0x300a, 0x0104);

		/// <summary>(300a,0106) VR=DS Block Data</summary>
		public static DcmTag BlockData = new DcmTag(0x300a, 0x0106);

		/// <summary>(300a,0107) VR=SQ Applicator Sequence</summary>
		public static DcmTag ApplicatorSequence = new DcmTag(0x300a, 0x0107);

		/// <summary>(300a,0108) VR=SH Applicator ID</summary>
		public static DcmTag ApplicatorID = new DcmTag(0x300a, 0x0108);

		/// <summary>(300a,0109) VR=CS Applicator Type</summary>
		public static DcmTag ApplicatorType = new DcmTag(0x300a, 0x0109);

		/// <summary>(300a,010a) VR=LO Applicator Description</summary>
		public static DcmTag ApplicatorDescription = new DcmTag(0x300a, 0x010a);

		/// <summary>(300a,010c) VR=DS Cumulative Dose Reference Coefficient</summary>
		public static DcmTag CumulativeDoseReferenceCoefficient = new DcmTag(0x300a, 0x010c);

		/// <summary>(300a,010e) VR=DS Final Cumulative Meterset Weight</summary>
		public static DcmTag FinalCumulativeMetersetWeight = new DcmTag(0x300a, 0x010e);

		/// <summary>(300a,0110) VR=IS Number of Control Points</summary>
		public static DcmTag NumberOfControlPoints = new DcmTag(0x300a, 0x0110);

		/// <summary>(300a,0111) VR=SQ Control Point Sequence</summary>
		public static DcmTag ControlPointSequence = new DcmTag(0x300a, 0x0111);

		/// <summary>(300a,0112) VR=IS Control Point Index</summary>
		public static DcmTag ControlPointIndex = new DcmTag(0x300a, 0x0112);

		/// <summary>(300a,0114) VR=DS Nominal Beam Energy</summary>
		public static DcmTag NominalBeamEnergy = new DcmTag(0x300a, 0x0114);

		/// <summary>(300a,0115) VR=DS Dose Rate Set</summary>
		public static DcmTag DoseRateSet = new DcmTag(0x300a, 0x0115);

		/// <summary>(300a,0116) VR=SQ Wedge Position Sequence</summary>
		public static DcmTag WedgePositionSequence = new DcmTag(0x300a, 0x0116);

		/// <summary>(300a,0118) VR=CS Wedge Position</summary>
		public static DcmTag WedgePosition = new DcmTag(0x300a, 0x0118);

		/// <summary>(300a,011a) VR=SQ Beam Limiting Device Position Sequence</summary>
		public static DcmTag BeamLimitingDevicePositionSequence = new DcmTag(0x300a, 0x011a);

		/// <summary>(300a,011c) VR=DS Leaf/Jaw Positions</summary>
		public static DcmTag LeafJawPositions = new DcmTag(0x300a, 0x011c);

		/// <summary>(300a,011e) VR=DS Gantry Angle</summary>
		public static DcmTag GantryAngle = new DcmTag(0x300a, 0x011e);

		/// <summary>(300a,011f) VR=CS Gantry Rotation Direction</summary>
		public static DcmTag GantryRotationDirection = new DcmTag(0x300a, 0x011f);

		/// <summary>(300a,0120) VR=DS Beam Limiting Device Angle</summary>
		public static DcmTag BeamLimitingDeviceAngle = new DcmTag(0x300a, 0x0120);

		/// <summary>(300a,0121) VR=CS Beam Limiting Device Rotation Direction</summary>
		public static DcmTag BeamLimitingDeviceRotationDirection = new DcmTag(0x300a, 0x0121);

		/// <summary>(300a,0122) VR=DS Patient Support Angle</summary>
		public static DcmTag PatientSupportAngle = new DcmTag(0x300a, 0x0122);

		/// <summary>(300a,0123) VR=CS Patient Support Rotation Direction</summary>
		public static DcmTag PatientSupportRotationDirection = new DcmTag(0x300a, 0x0123);

		/// <summary>(300a,0124) VR=DS Table Top Eccentric Axis Distance</summary>
		public static DcmTag TableTopEccentricAxisDistance = new DcmTag(0x300a, 0x0124);

		/// <summary>(300a,0125) VR=DS Table Top Eccentric Angle</summary>
		public static DcmTag TableTopEccentricAngle = new DcmTag(0x300a, 0x0125);

		/// <summary>(300a,0126) VR=CS Table Top Eccentric Rotation Direction</summary>
		public static DcmTag TableTopEccentricRotationDirection = new DcmTag(0x300a, 0x0126);

		/// <summary>(300a,0128) VR=DS Table Top Vertical Position</summary>
		public static DcmTag TableTopVerticalPosition = new DcmTag(0x300a, 0x0128);

		/// <summary>(300a,0129) VR=DS Table Top Longitudinal Position</summary>
		public static DcmTag TableTopLongitudinalPosition = new DcmTag(0x300a, 0x0129);

		/// <summary>(300a,012a) VR=DS Table Top Lateral Position</summary>
		public static DcmTag TableTopLateralPosition = new DcmTag(0x300a, 0x012a);

		/// <summary>(300a,012c) VR=DS Isocenter Position</summary>
		public static DcmTag IsocenterPosition = new DcmTag(0x300a, 0x012c);

		/// <summary>(300a,012e) VR=DS Surface Entry Point</summary>
		public static DcmTag SurfaceEntryPoint = new DcmTag(0x300a, 0x012e);

		/// <summary>(300a,0130) VR=DS Source to Surface Distance</summary>
		public static DcmTag SourceToSurfaceDistance = new DcmTag(0x300a, 0x0130);

		/// <summary>(300a,0134) VR=DS Cumulative Meterset Weight</summary>
		public static DcmTag CumulativeMetersetWeight = new DcmTag(0x300a, 0x0134);

		/// <summary>(300a,0140) VR=FL Table Top Pitch Angle</summary>
		public static DcmTag TableTopPitchAngle = new DcmTag(0x300a, 0x0140);

		/// <summary>(300a,0142) VR=CS Table Top Pitch Rotation Direction</summary>
		public static DcmTag TableTopPitchRotationDirection = new DcmTag(0x300a, 0x0142);

		/// <summary>(300a,0144) VR=FL Table Top Roll Angle</summary>
		public static DcmTag TableTopRollAngle = new DcmTag(0x300a, 0x0144);

		/// <summary>(300a,0146) VR=CS Table Top Roll Rotation Direction</summary>
		public static DcmTag TableTopRollRotationDirection = new DcmTag(0x300a, 0x0146);

		/// <summary>(300a,0148) VR=FL Head Fixation Angle</summary>
		public static DcmTag HeadFixationAngle = new DcmTag(0x300a, 0x0148);

		/// <summary>(300a,014a) VR=FL Gantry Pitch Angle</summary>
		public static DcmTag GantryPitchAngle = new DcmTag(0x300a, 0x014a);

		/// <summary>(300a,014c) VR=CS Gantry Pitch Rotation Direction</summary>
		public static DcmTag GantryPitchRotationDirection = new DcmTag(0x300a, 0x014c);

		/// <summary>(300a,014e) VR=FL Gantry Pitch Angle Tolerance</summary>
		public static DcmTag GantryPitchAngleTolerance = new DcmTag(0x300a, 0x014e);

		/// <summary>(300a,0180) VR=SQ Patient Setup Sequence</summary>
		public static DcmTag PatientSetupSequence = new DcmTag(0x300a, 0x0180);

		/// <summary>(300a,0182) VR=IS Patient Setup Number</summary>
		public static DcmTag PatientSetupNumber = new DcmTag(0x300a, 0x0182);

		/// <summary>(300a,0183) VR=LO Patient Setup Label</summary>
		public static DcmTag PatientSetupLabel = new DcmTag(0x300a, 0x0183);

		/// <summary>(300a,0184) VR=LO Patient Additional Position</summary>
		public static DcmTag PatientAdditionalPosition = new DcmTag(0x300a, 0x0184);

		/// <summary>(300a,0190) VR=SQ Fixation Device Sequence</summary>
		public static DcmTag FixationDeviceSequence = new DcmTag(0x300a, 0x0190);

		/// <summary>(300a,0192) VR=CS Fixation Device Type</summary>
		public static DcmTag FixationDeviceType = new DcmTag(0x300a, 0x0192);

		/// <summary>(300a,0194) VR=SH Fixation Device Label</summary>
		public static DcmTag FixationDeviceLabel = new DcmTag(0x300a, 0x0194);

		/// <summary>(300a,0196) VR=ST Fixation Device Description</summary>
		public static DcmTag FixationDeviceDescription = new DcmTag(0x300a, 0x0196);

		/// <summary>(300a,0198) VR=SH Fixation Device Position</summary>
		public static DcmTag FixationDevicePosition = new DcmTag(0x300a, 0x0198);

		/// <summary>(300a,0199) VR=FL Fixation Device Pitch Angle</summary>
		public static DcmTag FixationDevicePitchAngle = new DcmTag(0x300a, 0x0199);

		/// <summary>(300a,019a) VR=FL Fixation Device Roll Angle</summary>
		public static DcmTag FixationDeviceRollAngle = new DcmTag(0x300a, 0x019a);

		/// <summary>(300a,01a0) VR=SQ Shielding Device Sequence</summary>
		public static DcmTag ShieldingDeviceSequence = new DcmTag(0x300a, 0x01a0);

		/// <summary>(300a,01a2) VR=CS Shielding Device Type</summary>
		public static DcmTag ShieldingDeviceType = new DcmTag(0x300a, 0x01a2);

		/// <summary>(300a,01a4) VR=SH Shielding Device Label</summary>
		public static DcmTag ShieldingDeviceLabel = new DcmTag(0x300a, 0x01a4);

		/// <summary>(300a,01a6) VR=ST Shielding Device Description</summary>
		public static DcmTag ShieldingDeviceDescription = new DcmTag(0x300a, 0x01a6);

		/// <summary>(300a,01a8) VR=SH Shielding Device Position</summary>
		public static DcmTag ShieldingDevicePosition = new DcmTag(0x300a, 0x01a8);

		/// <summary>(300a,01b0) VR=CS Setup Technique</summary>
		public static DcmTag SetupTechnique = new DcmTag(0x300a, 0x01b0);

		/// <summary>(300a,01b2) VR=ST Setup Technique Description</summary>
		public static DcmTag SetupTechniqueDescription = new DcmTag(0x300a, 0x01b2);

		/// <summary>(300a,01b4) VR=SQ Setup Device Sequence</summary>
		public static DcmTag SetupDeviceSequence = new DcmTag(0x300a, 0x01b4);

		/// <summary>(300a,01b6) VR=CS Setup Device Type</summary>
		public static DcmTag SetupDeviceType = new DcmTag(0x300a, 0x01b6);

		/// <summary>(300a,01b8) VR=SH Setup Device Label</summary>
		public static DcmTag SetupDeviceLabel = new DcmTag(0x300a, 0x01b8);

		/// <summary>(300a,01ba) VR=ST Setup Device Description</summary>
		public static DcmTag SetupDeviceDescription = new DcmTag(0x300a, 0x01ba);

		/// <summary>(300a,01bc) VR=DS Setup Device Parameter</summary>
		public static DcmTag SetupDeviceParameter = new DcmTag(0x300a, 0x01bc);

		/// <summary>(300a,01d0) VR=ST Setup Reference Description</summary>
		public static DcmTag SetupReferenceDescription = new DcmTag(0x300a, 0x01d0);

		/// <summary>(300a,01d2) VR=DS Table Top Vertical Setup Displacement</summary>
		public static DcmTag TableTopVerticalSetupDisplacement = new DcmTag(0x300a, 0x01d2);

		/// <summary>(300a,01d4) VR=DS Table Top Longitudinal Setup Displacement</summary>
		public static DcmTag TableTopLongitudinalSetupDisplacement = new DcmTag(0x300a, 0x01d4);

		/// <summary>(300a,01d6) VR=DS Table Top Lateral Setup Displacement</summary>
		public static DcmTag TableTopLateralSetupDisplacement = new DcmTag(0x300a, 0x01d6);

		/// <summary>(300a,0200) VR=CS Brachy Treatment Technique</summary>
		public static DcmTag BrachyTreatmentTechnique = new DcmTag(0x300a, 0x0200);

		/// <summary>(300a,0202) VR=CS Brachy Treatment Type</summary>
		public static DcmTag BrachyTreatmentType = new DcmTag(0x300a, 0x0202);

		/// <summary>(300a,0206) VR=SQ Treatment Machine Sequence</summary>
		public static DcmTag TreatmentMachineSequence = new DcmTag(0x300a, 0x0206);

		/// <summary>(300a,0210) VR=SQ Source Sequence</summary>
		public static DcmTag SourceSequence = new DcmTag(0x300a, 0x0210);

		/// <summary>(300a,0212) VR=IS Source Number</summary>
		public static DcmTag SourceNumber = new DcmTag(0x300a, 0x0212);

		/// <summary>(300a,0214) VR=CS Source Type</summary>
		public static DcmTag SourceType = new DcmTag(0x300a, 0x0214);

		/// <summary>(300a,0216) VR=LO Source Manufacturer</summary>
		public static DcmTag SourceManufacturer = new DcmTag(0x300a, 0x0216);

		/// <summary>(300a,0218) VR=DS Active Source Diameter</summary>
		public static DcmTag ActiveSourceDiameter = new DcmTag(0x300a, 0x0218);

		/// <summary>(300a,021a) VR=DS Active Source Length</summary>
		public static DcmTag ActiveSourceLength = new DcmTag(0x300a, 0x021a);

		/// <summary>(300a,0222) VR=DS Source Encapsulation Nominal Thickness</summary>
		public static DcmTag SourceEncapsulationNominalThickness = new DcmTag(0x300a, 0x0222);

		/// <summary>(300a,0224) VR=DS Source Encapsulation Nominal Transmission</summary>
		public static DcmTag SourceEncapsulationNominalTransmission = new DcmTag(0x300a, 0x0224);

		/// <summary>(300a,0226) VR=LO Source Isotope Name</summary>
		public static DcmTag SourceIsotopeName = new DcmTag(0x300a, 0x0226);

		/// <summary>(300a,0228) VR=DS Source Isotope Half Life</summary>
		public static DcmTag SourceIsotopeHalfLife = new DcmTag(0x300a, 0x0228);

		/// <summary>(300a,0229) VR=CS Source Strength Units</summary>
		public static DcmTag SourceStrengthUnits = new DcmTag(0x300a, 0x0229);

		/// <summary>(300a,022a) VR=DS Reference Air Kerma Rate</summary>
		public static DcmTag ReferenceAirKermaRate = new DcmTag(0x300a, 0x022a);

		/// <summary>(300a,022b) VR=DS Source Strength</summary>
		public static DcmTag SourceStrength = new DcmTag(0x300a, 0x022b);

		/// <summary>(300a,022c) VR=DA Source Strength Reference Date</summary>
		public static DcmTag SourceStrengthReferenceDate = new DcmTag(0x300a, 0x022c);

		/// <summary>(300a,022e) VR=TM Source Strength Reference Time</summary>
		public static DcmTag SourceStrengthReferenceTime = new DcmTag(0x300a, 0x022e);

		/// <summary>(300a,0230) VR=SQ Application Setup Sequence</summary>
		public static DcmTag ApplicationSetupSequence = new DcmTag(0x300a, 0x0230);

		/// <summary>(300a,0232) VR=CS Application Setup Type</summary>
		public static DcmTag ApplicationSetupType = new DcmTag(0x300a, 0x0232);

		/// <summary>(300a,0234) VR=IS Application Setup Number</summary>
		public static DcmTag ApplicationSetupNumber = new DcmTag(0x300a, 0x0234);

		/// <summary>(300a,0236) VR=LO Application Setup Name</summary>
		public static DcmTag ApplicationSetupName = new DcmTag(0x300a, 0x0236);

		/// <summary>(300a,0238) VR=LO Application Setup Manufacturer</summary>
		public static DcmTag ApplicationSetupManufacturer = new DcmTag(0x300a, 0x0238);

		/// <summary>(300a,0240) VR=IS Template Number</summary>
		public static DcmTag TemplateNumber = new DcmTag(0x300a, 0x0240);

		/// <summary>(300a,0242) VR=SH Template Type</summary>
		public static DcmTag TemplateType = new DcmTag(0x300a, 0x0242);

		/// <summary>(300a,0244) VR=LO Template Name</summary>
		public static DcmTag TemplateName = new DcmTag(0x300a, 0x0244);

		/// <summary>(300a,0250) VR=DS Total Reference Air Kerma</summary>
		public static DcmTag TotalReferenceAirKerma = new DcmTag(0x300a, 0x0250);

		/// <summary>(300a,0260) VR=SQ Brachy Accessory Device Sequence</summary>
		public static DcmTag BrachyAccessoryDeviceSequence = new DcmTag(0x300a, 0x0260);

		/// <summary>(300a,0262) VR=IS Brachy Accessory Device Number</summary>
		public static DcmTag BrachyAccessoryDeviceNumber = new DcmTag(0x300a, 0x0262);

		/// <summary>(300a,0263) VR=SH Brachy Accessory Device ID</summary>
		public static DcmTag BrachyAccessoryDeviceID = new DcmTag(0x300a, 0x0263);

		/// <summary>(300a,0264) VR=CS Brachy Accessory Device Type</summary>
		public static DcmTag BrachyAccessoryDeviceType = new DcmTag(0x300a, 0x0264);

		/// <summary>(300a,0266) VR=LO Brachy Accessory Device Name</summary>
		public static DcmTag BrachyAccessoryDeviceName = new DcmTag(0x300a, 0x0266);

		/// <summary>(300a,026a) VR=DS Brachy Accessory Device Nominal Thickness</summary>
		public static DcmTag BrachyAccessoryDeviceNominalThickness = new DcmTag(0x300a, 0x026a);

		/// <summary>(300a,026c) VR=DS Brachy Accessory Device Nominal Transmission</summary>
		public static DcmTag BrachyAccessoryDeviceNominalTransmission = new DcmTag(0x300a, 0x026c);

		/// <summary>(300a,0280) VR=SQ Channel Sequence</summary>
		public static DcmTag ChannelSequence = new DcmTag(0x300a, 0x0280);

		/// <summary>(300a,0282) VR=IS Channel Number</summary>
		public static DcmTag ChannelNumber = new DcmTag(0x300a, 0x0282);

		/// <summary>(300a,0284) VR=DS Channel Length</summary>
		public static DcmTag ChannelLength = new DcmTag(0x300a, 0x0284);

		/// <summary>(300a,0286) VR=DS Channel Total Time</summary>
		public static DcmTag ChannelTotalTime = new DcmTag(0x300a, 0x0286);

		/// <summary>(300a,0288) VR=CS Source Movement Type</summary>
		public static DcmTag SourceMovementType = new DcmTag(0x300a, 0x0288);

		/// <summary>(300a,028a) VR=IS Number of Pulses</summary>
		public static DcmTag NumberOfPulses = new DcmTag(0x300a, 0x028a);

		/// <summary>(300a,028c) VR=DS Pulse Repetition Interval</summary>
		public static DcmTag PulseRepetitionInterval = new DcmTag(0x300a, 0x028c);

		/// <summary>(300a,0290) VR=IS Source Applicator Number</summary>
		public static DcmTag SourceApplicatorNumber = new DcmTag(0x300a, 0x0290);

		/// <summary>(300a,0291) VR=SH Source Applicator ID</summary>
		public static DcmTag SourceApplicatorID = new DcmTag(0x300a, 0x0291);

		/// <summary>(300a,0292) VR=CS Source Applicator Type</summary>
		public static DcmTag SourceApplicatorType = new DcmTag(0x300a, 0x0292);

		/// <summary>(300a,0294) VR=LO Source Applicator Name</summary>
		public static DcmTag SourceApplicatorName = new DcmTag(0x300a, 0x0294);

		/// <summary>(300a,0296) VR=DS Source Applicator Length</summary>
		public static DcmTag SourceApplicatorLength = new DcmTag(0x300a, 0x0296);

		/// <summary>(300a,0298) VR=LO Source Applicator Manufacturer</summary>
		public static DcmTag SourceApplicatorManufacturer = new DcmTag(0x300a, 0x0298);

		/// <summary>(300a,029c) VR=DS Source Applicator Wall Nominal Thickness</summary>
		public static DcmTag SourceApplicatorWallNominalThickness = new DcmTag(0x300a, 0x029c);

		/// <summary>(300a,029e) VR=DS Source Applicator Wall Nominal Transmission</summary>
		public static DcmTag SourceApplicatorWallNominalTransmission = new DcmTag(0x300a, 0x029e);

		/// <summary>(300a,02a0) VR=DS Source Applicator Step Size</summary>
		public static DcmTag SourceApplicatorStepSize = new DcmTag(0x300a, 0x02a0);

		/// <summary>(300a,02a2) VR=IS Transfer Tube Number</summary>
		public static DcmTag TransferTubeNumber = new DcmTag(0x300a, 0x02a2);

		/// <summary>(300a,02a4) VR=DS Transfer Tube Length</summary>
		public static DcmTag TransferTubeLength = new DcmTag(0x300a, 0x02a4);

		/// <summary>(300a,02b0) VR=SQ Channel Shield Sequence</summary>
		public static DcmTag ChannelShieldSequence = new DcmTag(0x300a, 0x02b0);

		/// <summary>(300a,02b2) VR=IS Channel Shield Number</summary>
		public static DcmTag ChannelShieldNumber = new DcmTag(0x300a, 0x02b2);

		/// <summary>(300a,02b3) VR=SH Channel Shield ID</summary>
		public static DcmTag ChannelShieldID = new DcmTag(0x300a, 0x02b3);

		/// <summary>(300a,02b4) VR=LO Channel Shield Name</summary>
		public static DcmTag ChannelShieldName = new DcmTag(0x300a, 0x02b4);

		/// <summary>(300a,02b8) VR=DS Channel Shield Nominal Thickness</summary>
		public static DcmTag ChannelShieldNominalThickness = new DcmTag(0x300a, 0x02b8);

		/// <summary>(300a,02ba) VR=DS Channel Shield Nominal Transmission</summary>
		public static DcmTag ChannelShieldNominalTransmission = new DcmTag(0x300a, 0x02ba);

		/// <summary>(300a,02c8) VR=DS Final Cumulative Time Weight</summary>
		public static DcmTag FinalCumulativeTimeWeight = new DcmTag(0x300a, 0x02c8);

		/// <summary>(300a,02d0) VR=SQ Brachy Control Point Sequence</summary>
		public static DcmTag BrachyControlPointSequence = new DcmTag(0x300a, 0x02d0);

		/// <summary>(300a,02d2) VR=DS Control Point Relative Position</summary>
		public static DcmTag ControlPointRelativePosition = new DcmTag(0x300a, 0x02d2);

		/// <summary>(300a,02d4) VR=DS Control Point 3D Position</summary>
		public static DcmTag ControlPoint3DPosition = new DcmTag(0x300a, 0x02d4);

		/// <summary>(300a,02d6) VR=DS Cumulative Time Weight</summary>
		public static DcmTag CumulativeTimeWeight = new DcmTag(0x300a, 0x02d6);

		/// <summary>(300a,02e0) VR=CS Compensator Divergence</summary>
		public static DcmTag CompensatorDivergence = new DcmTag(0x300a, 0x02e0);

		/// <summary>(300a,02e1) VR=CS Compensator Mounting Position</summary>
		public static DcmTag CompensatorMountingPosition = new DcmTag(0x300a, 0x02e1);

		/// <summary>(300a,02e2) VR=DS Source to Compensator Distance</summary>
		public static DcmTag SourceToCompensatorDistance = new DcmTag(0x300a, 0x02e2);

		/// <summary>(300a,02e3) VR=FL Total Compensator Tray Water-Equivalent Thickness</summary>
		public static DcmTag TotalCompensatorTrayWaterEquivalentThickness = new DcmTag(0x300a, 0x02e3);

		/// <summary>(300a,02e4) VR=FL Isocenter to Compensator Tray Distance</summary>
		public static DcmTag IsocenterToCompensatorTrayDistance = new DcmTag(0x300a, 0x02e4);

		/// <summary>(300a,02e5) VR=FL Compensator Column Offset</summary>
		public static DcmTag CompensatorColumnOffset = new DcmTag(0x300a, 0x02e5);

		/// <summary>(300a,02e6) VR=FL Isocenter to Compensator Distances</summary>
		public static DcmTag IsocenterToCompensatorDistances = new DcmTag(0x300a, 0x02e6);

		/// <summary>(300a,02e7) VR=FL Compensator Relative Stopping Power Ratio</summary>
		public static DcmTag CompensatorRelativeStoppingPowerRatio = new DcmTag(0x300a, 0x02e7);

		/// <summary>(300a,02e8) VR=FL Compensator Milling Tool Diameter</summary>
		public static DcmTag CompensatorMillingToolDiameter = new DcmTag(0x300a, 0x02e8);

		/// <summary>(300a,02ea) VR=SQ Ion Range Compensator Sequence</summary>
		public static DcmTag IonRangeCompensatorSequence = new DcmTag(0x300a, 0x02ea);

		/// <summary>(300a,0302) VR=IS Radiation Mass Number</summary>
		public static DcmTag RadiationMassNumber = new DcmTag(0x300a, 0x0302);

		/// <summary>(300a,0304) VR=IS Radiation Atomic Number</summary>
		public static DcmTag RadiationAtomicNumber = new DcmTag(0x300a, 0x0304);

		/// <summary>(300a,0306) VR=SS Radiation Charge State</summary>
		public static DcmTag RadiationChargeState = new DcmTag(0x300a, 0x0306);

		/// <summary>(300a,0308) VR=CS Scan Mode</summary>
		public static DcmTag ScanMode = new DcmTag(0x300a, 0x0308);

		/// <summary>(300a,030a) VR=FL Virtual Source-Axis Distances</summary>
		public static DcmTag VirtualSourceAxisDistances = new DcmTag(0x300a, 0x030a);

		/// <summary>(300a,030c) VR=SQ Snout Sequence</summary>
		public static DcmTag SnoutSequence = new DcmTag(0x300a, 0x030c);

		/// <summary>(300a,030d) VR=FL Snout Position</summary>
		public static DcmTag SnoutPosition = new DcmTag(0x300a, 0x030d);

		/// <summary>(300a,030f) VR=SH Snout ID</summary>
		public static DcmTag SnoutID = new DcmTag(0x300a, 0x030f);

		/// <summary>(300a,0312) VR=IS Number of Range Shifters</summary>
		public static DcmTag NumberOfRangeShifters = new DcmTag(0x300a, 0x0312);

		/// <summary>(300a,0314) VR=SQ Range Shifter Sequence</summary>
		public static DcmTag RangeShifterSequence = new DcmTag(0x300a, 0x0314);

		/// <summary>(300a,0316) VR=IS Range Shifter Number</summary>
		public static DcmTag RangeShifterNumber = new DcmTag(0x300a, 0x0316);

		/// <summary>(300a,0318) VR=SH Range Shifter ID</summary>
		public static DcmTag RangeShifterID = new DcmTag(0x300a, 0x0318);

		/// <summary>(300a,0320) VR=CS Range Shifter Type</summary>
		public static DcmTag RangeShifterType = new DcmTag(0x300a, 0x0320);

		/// <summary>(300a,0322) VR=LO Range Shifter Description</summary>
		public static DcmTag RangeShifterDescription = new DcmTag(0x300a, 0x0322);

		/// <summary>(300a,0330) VR=IS Number of Lateral Spreading Devices</summary>
		public static DcmTag NumberOfLateralSpreadingDevices = new DcmTag(0x300a, 0x0330);

		/// <summary>(300a,0332) VR=SQ Lateral Spreading Device Sequence</summary>
		public static DcmTag LateralSpreadingDeviceSequence = new DcmTag(0x300a, 0x0332);

		/// <summary>(300a,0334) VR=IS Lateral Spreading Device Number</summary>
		public static DcmTag LateralSpreadingDeviceNumber = new DcmTag(0x300a, 0x0334);

		/// <summary>(300a,0336) VR=SH Lateral Spreading Device ID</summary>
		public static DcmTag LateralSpreadingDeviceID = new DcmTag(0x300a, 0x0336);

		/// <summary>(300a,0338) VR=CS Lateral Spreading Device Type</summary>
		public static DcmTag LateralSpreadingDeviceType = new DcmTag(0x300a, 0x0338);

		/// <summary>(300a,033a) VR=LO Lateral Spreading Device Description</summary>
		public static DcmTag LateralSpreadingDeviceDescription = new DcmTag(0x300a, 0x033a);

		/// <summary>(300a,033c) VR=FL Lateral Spreading Device Water Equivalent Thickness</summary>
		public static DcmTag LateralSpreadingDeviceWaterEquivalentThickness = new DcmTag(0x300a, 0x033c);

		/// <summary>(300a,0340) VR=IS Number of Range Modulators</summary>
		public static DcmTag NumberOfRangeModulators = new DcmTag(0x300a, 0x0340);

		/// <summary>(300a,0342) VR=SQ Range Modulator Sequence</summary>
		public static DcmTag RangeModulatorSequence = new DcmTag(0x300a, 0x0342);

		/// <summary>(300a,0344) VR=IS Range Modulator Number</summary>
		public static DcmTag RangeModulatorNumber = new DcmTag(0x300a, 0x0344);

		/// <summary>(300a,0346) VR=SH Range Modulator ID</summary>
		public static DcmTag RangeModulatorID = new DcmTag(0x300a, 0x0346);

		/// <summary>(300a,0348) VR=CS Range Modulator Type</summary>
		public static DcmTag RangeModulatorType = new DcmTag(0x300a, 0x0348);

		/// <summary>(300a,034a) VR=LO Range Modulator Description</summary>
		public static DcmTag RangeModulatorDescription = new DcmTag(0x300a, 0x034a);

		/// <summary>(300a,034c) VR=SH Beam Current Modulation ID</summary>
		public static DcmTag BeamCurrentModulationID = new DcmTag(0x300a, 0x034c);

		/// <summary>(300a,0350) VR=CS Patient Support Type</summary>
		public static DcmTag PatientSupportType = new DcmTag(0x300a, 0x0350);

		/// <summary>(300a,0352) VR=SH Patient Support ID</summary>
		public static DcmTag PatientSupportID = new DcmTag(0x300a, 0x0352);

		/// <summary>(300a,0354) VR=LO Patient Support Accessory Code</summary>
		public static DcmTag PatientSupportAccessoryCode = new DcmTag(0x300a, 0x0354);

		/// <summary>(300a,0356) VR=FL Fixation Light Azimuthal Angle</summary>
		public static DcmTag FixationLightAzimuthalAngle = new DcmTag(0x300a, 0x0356);

		/// <summary>(300a,0358) VR=FL Fixation Light Polar Angle</summary>
		public static DcmTag FixationLightPolarAngle = new DcmTag(0x300a, 0x0358);

		/// <summary>(300a,035a) VR=FL Meterset Rate</summary>
		public static DcmTag MetersetRate = new DcmTag(0x300a, 0x035a);

		/// <summary>(300a,0360) VR=SQ Range Shifter Settings Sequence</summary>
		public static DcmTag RangeShifterSettingsSequence = new DcmTag(0x300a, 0x0360);

		/// <summary>(300a,0362) VR=LO Range Shifter Setting</summary>
		public static DcmTag RangeShifterSetting = new DcmTag(0x300a, 0x0362);

		/// <summary>(300a,0364) VR=FL Isocenter to Range Shifter Distance</summary>
		public static DcmTag IsocenterToRangeShifterDistance = new DcmTag(0x300a, 0x0364);

		/// <summary>(300a,0366) VR=FL Range Shifter Water Equivalent Thickness</summary>
		public static DcmTag RangeShifterWaterEquivalentThickness = new DcmTag(0x300a, 0x0366);

		/// <summary>(300a,0370) VR=SQ Lateral Spreading Device Settings Sequence</summary>
		public static DcmTag LateralSpreadingDeviceSettingsSequence = new DcmTag(0x300a, 0x0370);

		/// <summary>(300a,0372) VR=LO Lateral Spreading Device Setting</summary>
		public static DcmTag LateralSpreadingDeviceSetting = new DcmTag(0x300a, 0x0372);

		/// <summary>(300a,0374) VR=FL Isocenter to Lateral Spreading Device Distance</summary>
		public static DcmTag IsocenterToLateralSpreadingDeviceDistance = new DcmTag(0x300a, 0x0374);

		/// <summary>(300a,0380) VR=SQ Range Modulator Settings Sequence</summary>
		public static DcmTag RangeModulatorSettingsSequence = new DcmTag(0x300a, 0x0380);

		/// <summary>(300a,0382) VR=FL Range Modulator Gating Start Value</summary>
		public static DcmTag RangeModulatorGatingStartValue = new DcmTag(0x300a, 0x0382);

		/// <summary>(300a,0384) VR=FL Range Modulator Gating Stop Value</summary>
		public static DcmTag RangeModulatorGatingStopValue = new DcmTag(0x300a, 0x0384);

		/// <summary>(300a,0386) VR=FL Range Modulator Gating Start Water Equivalent Thickness</summary>
		public static DcmTag RangeModulatorGatingStartWaterEquivalentThickness = new DcmTag(0x300a, 0x0386);

		/// <summary>(300a,0388) VR=FL Range Modulator Gating Stop Water Equivalent Thickness</summary>
		public static DcmTag RangeModulatorGatingStopWaterEquivalentThickness = new DcmTag(0x300a, 0x0388);

		/// <summary>(300a,038a) VR=FL Isocenter to Range Modulator Distance</summary>
		public static DcmTag IsocenterToRangeModulatorDistance = new DcmTag(0x300a, 0x038a);

		/// <summary>(300a,0390) VR=SH Scan Spot Tune ID</summary>
		public static DcmTag ScanSpotTuneID = new DcmTag(0x300a, 0x0390);

		/// <summary>(300a,0392) VR=IS Number of Scan Spot Positions</summary>
		public static DcmTag NumberOfScanSpotPositions = new DcmTag(0x300a, 0x0392);

		/// <summary>(300a,0394) VR=FL Scan Spot Position Map</summary>
		public static DcmTag ScanSpotPositionMap = new DcmTag(0x300a, 0x0394);

		/// <summary>(300a,0396) VR=FL Scan Spot Meterset Weights</summary>
		public static DcmTag ScanSpotMetersetWeights = new DcmTag(0x300a, 0x0396);

		/// <summary>(300a,0398) VR=FL Scanning Spot Size</summary>
		public static DcmTag ScanningSpotSize = new DcmTag(0x300a, 0x0398);

		/// <summary>(300a,039a) VR=IS Number of Paintings</summary>
		public static DcmTag NumberOfPaintings = new DcmTag(0x300a, 0x039a);

		/// <summary>(300a,03a0) VR=SQ Ion Tolerance Table Sequence</summary>
		public static DcmTag IonToleranceTableSequence = new DcmTag(0x300a, 0x03a0);

		/// <summary>(300a,03a2) VR=SQ Ion Beam Sequence</summary>
		public static DcmTag IonBeamSequence = new DcmTag(0x300a, 0x03a2);

		/// <summary>(300a,03a4) VR=SQ Ion Beam Limiting Device Sequence</summary>
		public static DcmTag IonBeamLimitingDeviceSequence = new DcmTag(0x300a, 0x03a4);

		/// <summary>(300a,03a6) VR=SQ Ion Block Sequence</summary>
		public static DcmTag IonBlockSequence = new DcmTag(0x300a, 0x03a6);

		/// <summary>(300a,03a8) VR=SQ Ion Control Point Sequence</summary>
		public static DcmTag IonControlPointSequence = new DcmTag(0x300a, 0x03a8);

		/// <summary>(300a,03aa) VR=SQ Ion Wedge Sequence</summary>
		public static DcmTag IonWedgeSequence = new DcmTag(0x300a, 0x03aa);

		/// <summary>(300a,03ac) VR=SQ Ion Wedge Position Sequence</summary>
		public static DcmTag IonWedgePositionSequence = new DcmTag(0x300a, 0x03ac);

		/// <summary>(300a,0401) VR=SQ Referenced Setup Image Sequence</summary>
		public static DcmTag ReferencedSetupImageSequence = new DcmTag(0x300a, 0x0401);

		/// <summary>(300a,0402) VR=ST Setup Image Comment</summary>
		public static DcmTag SetupImageComment = new DcmTag(0x300a, 0x0402);

		/// <summary>(300a,0410) VR=SQ Motion Synchronization Sequence</summary>
		public static DcmTag MotionSynchronizationSequence = new DcmTag(0x300a, 0x0410);

		/// <summary>(300c,0002) VR=SQ Referenced RT Plan Sequence</summary>
		public static DcmTag ReferencedRTPlanSequence = new DcmTag(0x300c, 0x0002);

		/// <summary>(300c,0004) VR=SQ Referenced Beam Sequence</summary>
		public static DcmTag ReferencedBeamSequence = new DcmTag(0x300c, 0x0004);

		/// <summary>(300c,0006) VR=IS Referenced Beam Number</summary>
		public static DcmTag ReferencedBeamNumber = new DcmTag(0x300c, 0x0006);

		/// <summary>(300c,0007) VR=IS Referenced Reference Image Number</summary>
		public static DcmTag ReferencedReferenceImageNumber = new DcmTag(0x300c, 0x0007);

		/// <summary>(300c,0008) VR=DS Start Cumulative Meterset Weight</summary>
		public static DcmTag StartCumulativeMetersetWeight = new DcmTag(0x300c, 0x0008);

		/// <summary>(300c,0009) VR=DS End Cumulative Meterset Weight</summary>
		public static DcmTag EndCumulativeMetersetWeight = new DcmTag(0x300c, 0x0009);

		/// <summary>(300c,000a) VR=SQ Referenced Brachy Application Setup Sequence</summary>
		public static DcmTag ReferencedBrachyApplicationSetupSequence = new DcmTag(0x300c, 0x000a);

		/// <summary>(300c,000c) VR=IS Referenced Brachy Application Setup Number</summary>
		public static DcmTag ReferencedBrachyApplicationSetupNumber = new DcmTag(0x300c, 0x000c);

		/// <summary>(300c,000e) VR=IS Referenced Source Number</summary>
		public static DcmTag ReferencedSourceNumber = new DcmTag(0x300c, 0x000e);

		/// <summary>(300c,0020) VR=SQ Referenced Fraction Group Sequence</summary>
		public static DcmTag ReferencedFractionGroupSequence = new DcmTag(0x300c, 0x0020);

		/// <summary>(300c,0022) VR=IS Referenced Fraction Group Number</summary>
		public static DcmTag ReferencedFractionGroupNumber = new DcmTag(0x300c, 0x0022);

		/// <summary>(300c,0040) VR=SQ Referenced Verification Image Sequence</summary>
		public static DcmTag ReferencedVerificationImageSequence = new DcmTag(0x300c, 0x0040);

		/// <summary>(300c,0042) VR=SQ Referenced Reference Image Sequence</summary>
		public static DcmTag ReferencedReferenceImageSequence = new DcmTag(0x300c, 0x0042);

		/// <summary>(300c,0050) VR=SQ Referenced Dose Reference Sequence</summary>
		public static DcmTag ReferencedDoseReferenceSequence = new DcmTag(0x300c, 0x0050);

		/// <summary>(300c,0051) VR=IS Referenced Dose Reference Number</summary>
		public static DcmTag ReferencedDoseReferenceNumber = new DcmTag(0x300c, 0x0051);

		/// <summary>(300c,0055) VR=SQ Brachy Referenced Dose Reference Sequence</summary>
		public static DcmTag BrachyReferencedDoseReferenceSequence = new DcmTag(0x300c, 0x0055);

		/// <summary>(300c,0060) VR=SQ Referenced Structure Set Sequence</summary>
		public static DcmTag ReferencedStructureSetSequence = new DcmTag(0x300c, 0x0060);

		/// <summary>(300c,006a) VR=IS Referenced Patient Setup Number</summary>
		public static DcmTag ReferencedPatientSetupNumber = new DcmTag(0x300c, 0x006a);

		/// <summary>(300c,0080) VR=SQ Referenced Dose Sequence</summary>
		public static DcmTag ReferencedDoseSequence = new DcmTag(0x300c, 0x0080);

		/// <summary>(300c,00a0) VR=IS Referenced Tolerance Table Number</summary>
		public static DcmTag ReferencedToleranceTableNumber = new DcmTag(0x300c, 0x00a0);

		/// <summary>(300c,00b0) VR=SQ Referenced Bolus Sequence</summary>
		public static DcmTag ReferencedBolusSequence = new DcmTag(0x300c, 0x00b0);

		/// <summary>(300c,00c0) VR=IS Referenced Wedge Number</summary>
		public static DcmTag ReferencedWedgeNumber = new DcmTag(0x300c, 0x00c0);

		/// <summary>(300c,00d0) VR=IS Referenced Compensator Number</summary>
		public static DcmTag ReferencedCompensatorNumber = new DcmTag(0x300c, 0x00d0);

		/// <summary>(300c,00e0) VR=IS Referenced Block Number</summary>
		public static DcmTag ReferencedBlockNumber = new DcmTag(0x300c, 0x00e0);

		/// <summary>(300c,00f0) VR=IS Referenced Control Point Index</summary>
		public static DcmTag ReferencedControlPointIndex = new DcmTag(0x300c, 0x00f0);

		/// <summary>(300c,00f2) VR=SQ Referenced Control Point Sequence</summary>
		public static DcmTag ReferencedControlPointSequence = new DcmTag(0x300c, 0x00f2);

		/// <summary>(300c,00f4) VR=IS Referenced Start Control Point Index</summary>
		public static DcmTag ReferencedStartControlPointIndex = new DcmTag(0x300c, 0x00f4);

		/// <summary>(300c,00f6) VR=IS Referenced Stop Control Point Index</summary>
		public static DcmTag ReferencedStopControlPointIndex = new DcmTag(0x300c, 0x00f6);

		/// <summary>(300c,0100) VR=IS Referenced Range Shifter Number</summary>
		public static DcmTag ReferencedRangeShifterNumber = new DcmTag(0x300c, 0x0100);

		/// <summary>(300c,0102) VR=IS Referenced Lateral Spreading Device Number</summary>
		public static DcmTag ReferencedLateralSpreadingDeviceNumber = new DcmTag(0x300c, 0x0102);

		/// <summary>(300c,0104) VR=IS Referenced Range Modulator Number</summary>
		public static DcmTag ReferencedRangeModulatorNumber = new DcmTag(0x300c, 0x0104);

		/// <summary>(300e,0002) VR=CS Approval Status</summary>
		public static DcmTag ApprovalStatus = new DcmTag(0x300e, 0x0002);

		/// <summary>(300e,0004) VR=DA Review Date</summary>
		public static DcmTag ReviewDate = new DcmTag(0x300e, 0x0004);

		/// <summary>(300e,0005) VR=TM Review Time</summary>
		public static DcmTag ReviewTime = new DcmTag(0x300e, 0x0005);

		/// <summary>(300e,0008) VR=PN Reviewer Name</summary>
		public static DcmTag ReviewerName = new DcmTag(0x300e, 0x0008);

		/// <summary>(4000,0010) VR=LT Arbitrary (Retired)</summary>
		public static DcmTag ArbitraryRETIRED = new DcmTag(0x4000, 0x0010);

		/// <summary>(4000,4000) VR=LT Text Comments (Retired)</summary>
		public static DcmTag TextCommentsRETIRED = new DcmTag(0x4000, 0x4000);

		/// <summary>(4008,0040) VR=SH Results ID (Retired)</summary>
		public static DcmTag ResultsIDRETIRED = new DcmTag(0x4008, 0x0040);

		/// <summary>(4008,0042) VR=LO Results ID Issuer (Retired)</summary>
		public static DcmTag ResultsIDIssuerRETIRED = new DcmTag(0x4008, 0x0042);

		/// <summary>(4008,0050) VR=SQ Referenced Interpretation Sequence (Retired)</summary>
		public static DcmTag ReferencedInterpretationSequenceRETIRED = new DcmTag(0x4008, 0x0050);

		/// <summary>(4008,0100) VR=DA Interpretation Recorded Date (Retired)</summary>
		public static DcmTag InterpretationRecordedDateRETIRED = new DcmTag(0x4008, 0x0100);

		/// <summary>(4008,0101) VR=TM Interpretation Recorded Time (Retired)</summary>
		public static DcmTag InterpretationRecordedTimeRETIRED = new DcmTag(0x4008, 0x0101);

		/// <summary>(4008,0102) VR=PN Interpretation Recorder (Retired)</summary>
		public static DcmTag InterpretationRecorderRETIRED = new DcmTag(0x4008, 0x0102);

		/// <summary>(4008,0103) VR=LO Reference to Recorded Sound (Retired)</summary>
		public static DcmTag ReferenceToRecordedSoundRETIRED = new DcmTag(0x4008, 0x0103);

		/// <summary>(4008,0108) VR=DA Interpretation Transcription Date (Retired)</summary>
		public static DcmTag InterpretationTranscriptionDateRETIRED = new DcmTag(0x4008, 0x0108);

		/// <summary>(4008,0109) VR=TM Interpretation Transcription Time (Retired)</summary>
		public static DcmTag InterpretationTranscriptionTimeRETIRED = new DcmTag(0x4008, 0x0109);

		/// <summary>(4008,010a) VR=PN Interpretation Transcriber (Retired)</summary>
		public static DcmTag InterpretationTranscriberRETIRED = new DcmTag(0x4008, 0x010a);

		/// <summary>(4008,010b) VR=ST Interpretation Text (Retired)</summary>
		public static DcmTag InterpretationTextRETIRED = new DcmTag(0x4008, 0x010b);

		/// <summary>(4008,010c) VR=PN Interpretation Author (Retired)</summary>
		public static DcmTag InterpretationAuthorRETIRED = new DcmTag(0x4008, 0x010c);

		/// <summary>(4008,0111) VR=SQ Interpretation Approver Sequence (Retired)</summary>
		public static DcmTag InterpretationApproverSequenceRETIRED = new DcmTag(0x4008, 0x0111);

		/// <summary>(4008,0112) VR=DA Interpretation Approval Date (Retired)</summary>
		public static DcmTag InterpretationApprovalDateRETIRED = new DcmTag(0x4008, 0x0112);

		/// <summary>(4008,0113) VR=TM Interpretation Approval Time (Retired)</summary>
		public static DcmTag InterpretationApprovalTimeRETIRED = new DcmTag(0x4008, 0x0113);

		/// <summary>(4008,0114) VR=PN Physician Approving Interpretation (Retired)</summary>
		public static DcmTag PhysicianApprovingInterpretationRETIRED = new DcmTag(0x4008, 0x0114);

		/// <summary>(4008,0115) VR=LT Interpretation Diagnosis Description (Retired)</summary>
		public static DcmTag InterpretationDiagnosisDescriptionRETIRED = new DcmTag(0x4008, 0x0115);

		/// <summary>(4008,0117) VR=SQ Interpretation Diagnosis Code Sequence (Retired)</summary>
		public static DcmTag InterpretationDiagnosisCodeSequenceRETIRED = new DcmTag(0x4008, 0x0117);

		/// <summary>(4008,0118) VR=SQ Results Distribution List Sequence (Retired)</summary>
		public static DcmTag ResultsDistributionListSequenceRETIRED = new DcmTag(0x4008, 0x0118);

		/// <summary>(4008,0119) VR=PN Distribution Name (Retired)</summary>
		public static DcmTag DistributionNameRETIRED = new DcmTag(0x4008, 0x0119);

		/// <summary>(4008,011a) VR=LO Distribution Address (Retired)</summary>
		public static DcmTag DistributionAddressRETIRED = new DcmTag(0x4008, 0x011a);

		/// <summary>(4008,0200) VR=SH Interpretation ID (Retired)</summary>
		public static DcmTag InterpretationIDRETIRED = new DcmTag(0x4008, 0x0200);

		/// <summary>(4008,0202) VR=LO Interpretation ID Issuer (Retired)</summary>
		public static DcmTag InterpretationIDIssuerRETIRED = new DcmTag(0x4008, 0x0202);

		/// <summary>(4008,0210) VR=CS Interpretation Type ID (Retired)</summary>
		public static DcmTag InterpretationTypeIDRETIRED = new DcmTag(0x4008, 0x0210);

		/// <summary>(4008,0212) VR=CS Interpretation Status ID (Retired)</summary>
		public static DcmTag InterpretationStatusIDRETIRED = new DcmTag(0x4008, 0x0212);

		/// <summary>(4008,0300) VR=ST Impressions (Retired)</summary>
		public static DcmTag ImpressionsRETIRED = new DcmTag(0x4008, 0x0300);

		/// <summary>(4008,4000) VR=ST Results Comments (Retired)</summary>
		public static DcmTag ResultsCommentsRETIRED = new DcmTag(0x4008, 0x4000);

		/// <summary>(4ffe,0001) VR=SQ MAC Parameters Sequence</summary>
		public static DcmTag MACParametersSequence = new DcmTag(0x4ffe, 0x0001);

		/// <summary>(50xx,0005) VR=US Curve Dimensions (Retired)</summary>
		public static DcmTag CurveDimensionsRETIRED = new DcmTag(0x5000, 0x0005);

		/// <summary>(50xx,0010) VR=US Number of Points (Retired)</summary>
		public static DcmTag NumberOfPointsRETIRED = new DcmTag(0x5000, 0x0010);

		/// <summary>(50xx,0020) VR=CS Type of Data (Retired)</summary>
		public static DcmTag TypeOfDataRETIRED = new DcmTag(0x5000, 0x0020);

		/// <summary>(50xx,0022) VR=LO Curve Description (Retired)</summary>
		public static DcmTag CurveDescriptionRETIRED = new DcmTag(0x5000, 0x0022);

		/// <summary>(50xx,0030) VR=SH Axis Units (Retired)</summary>
		public static DcmTag AxisUnitsRETIRED = new DcmTag(0x5000, 0x0030);

		/// <summary>(50xx,0040) VR=SH Axis Labels (Retired)</summary>
		public static DcmTag AxisLabelsRETIRED = new DcmTag(0x5000, 0x0040);

		/// <summary>(50xx,0103) VR=US Data Value Representation (Retired)</summary>
		public static DcmTag DataValueRepresentationRETIRED = new DcmTag(0x5000, 0x0103);

		/// <summary>(50xx,0104) VR=US Minimum Coordinate Value (Retired)</summary>
		public static DcmTag MinimumCoordinateValueRETIRED = new DcmTag(0x5000, 0x0104);

		/// <summary>(50xx,0105) VR=US Maximum Coordinate Value (Retired)</summary>
		public static DcmTag MaximumCoordinateValueRETIRED = new DcmTag(0x5000, 0x0105);

		/// <summary>(50xx,0106) VR=SH Curve Range (Retired)</summary>
		public static DcmTag CurveRangeRETIRED = new DcmTag(0x5000, 0x0106);

		/// <summary>(50xx,0110) VR=US Curve Data Descriptor (Retired)</summary>
		public static DcmTag CurveDataDescriptorRETIRED = new DcmTag(0x5000, 0x0110);

		/// <summary>(50xx,0112) VR=US Coordinate Start Value (Retired)</summary>
		public static DcmTag CoordinateStartValueRETIRED = new DcmTag(0x5000, 0x0112);

		/// <summary>(50xx,0114) VR=US Coordinate Step Value (Retired)</summary>
		public static DcmTag CoordinateStepValueRETIRED = new DcmTag(0x5000, 0x0114);

		/// <summary>(50xx,1001) VR=CS Curve Activation Layer (Retired)</summary>
		public static DcmTag CurveActivationLayerRETIRED = new DcmTag(0x5000, 0x1001);

		/// <summary>(50xx,2000) VR=US Audio Type (Retired)</summary>
		public static DcmTag AudioTypeRETIRED = new DcmTag(0x5000, 0x2000);

		/// <summary>(50xx,2002) VR=US Audio Sample Format (Retired)</summary>
		public static DcmTag AudioSampleFormatRETIRED = new DcmTag(0x5000, 0x2002);

		/// <summary>(50xx,2004) VR=US Number of Channels (Retired)</summary>
		public static DcmTag NumberOfChannelsRETIRED = new DcmTag(0x5000, 0x2004);

		/// <summary>(50xx,2006) VR=UL Number of Samples (Retired)</summary>
		public static DcmTag NumberOfSamplesRETIRED = new DcmTag(0x5000, 0x2006);

		/// <summary>(50xx,2008) VR=UL Sample Rate (Retired)</summary>
		public static DcmTag SampleRateRETIRED = new DcmTag(0x5000, 0x2008);

		/// <summary>(50xx,200a) VR=UL Total Time (Retired)</summary>
		public static DcmTag TotalTimeRETIRED = new DcmTag(0x5000, 0x200a);

		/// <summary>(50xx,200c) VR=OB Audio Sample Data (Retired)</summary>
		public static DcmTag AudioSampleDataRETIRED = new DcmTag(0x5000, 0x200c);

		/// <summary>(50xx,200e) VR=LT Audio Comments (Retired)</summary>
		public static DcmTag AudioCommentsRETIRED = new DcmTag(0x5000, 0x200e);

		/// <summary>(50xx,2500) VR=LO Curve Label (Retired)</summary>
		public static DcmTag CurveLabelRETIRED = new DcmTag(0x5000, 0x2500);

		/// <summary>(50xx,2600) VR=SQ Curve Referenced Overlay Sequence (Retired)</summary>
		public static DcmTag CurveReferencedOverlaySequenceRETIRED = new DcmTag(0x5000, 0x2600);

		/// <summary>(50xx,2610) VR=US Curve Referenced Overlay Group (Retired)</summary>
		public static DcmTag CurveReferencedOverlayGroupRETIRED = new DcmTag(0x5000, 0x2610);

		/// <summary>(50xx,3000) VR=OB Curve Data (Retired)</summary>
		public static DcmTag CurveDataRETIRED = new DcmTag(0x5000, 0x3000);

		/// <summary>(5200,9229) VR=SQ Shared Functional Groups Sequence</summary>
		public static DcmTag SharedFunctionalGroupsSequence = new DcmTag(0x5200, 0x9229);

		/// <summary>(5200,9230) VR=SQ Per-frame Functional Groups Sequence</summary>
		public static DcmTag PerframeFunctionalGroupsSequence = new DcmTag(0x5200, 0x9230);

		/// <summary>(5400,0100) VR=SQ Waveform Sequence</summary>
		public static DcmTag WaveformSequence = new DcmTag(0x5400, 0x0100);

		/// <summary>(5400,0110) VR=OB Channel Minimum Value</summary>
		public static DcmTag ChannelMinimumValue = new DcmTag(0x5400, 0x0110);

		/// <summary>(5400,0112) VR=OB Channel Maximum Value</summary>
		public static DcmTag ChannelMaximumValue = new DcmTag(0x5400, 0x0112);

		/// <summary>(5400,1004) VR=US Waveform Bits Allocated</summary>
		public static DcmTag WaveformBitsAllocated = new DcmTag(0x5400, 0x1004);

		/// <summary>(5400,1006) VR=CS Waveform Sample Interpretation</summary>
		public static DcmTag WaveformSampleInterpretation = new DcmTag(0x5400, 0x1006);

		/// <summary>(5400,100a) VR=OB Waveform Padding Value</summary>
		public static DcmTag WaveformPaddingValue = new DcmTag(0x5400, 0x100a);

		/// <summary>(5400,1010) VR=OB Waveform Data</summary>
		public static DcmTag WaveformData = new DcmTag(0x5400, 0x1010);

		/// <summary>(5600,0010) VR=OF First Order Phase Correction Angle</summary>
		public static DcmTag FirstOrderPhaseCorrectionAngle = new DcmTag(0x5600, 0x0010);

		/// <summary>(5600,0020) VR=OF Spectroscopy Data</summary>
		public static DcmTag SpectroscopyData = new DcmTag(0x5600, 0x0020);

		/// <summary>(60xx,0010) VR=US Overlay Rows</summary>
		public static DcmTag OverlayRows = new DcmTag(0x6000, 0x0010);

		/// <summary>(60xx,0011) VR=US Overlay Columns</summary>
		public static DcmTag OverlayColumns = new DcmTag(0x6000, 0x0011);

		/// <summary>(60xx,0012) VR=US Overlay Planes</summary>
		public static DcmTag OverlayPlanes = new DcmTag(0x6000, 0x0012);

		/// <summary>(60xx,0015) VR=IS Number of Frames in Overlay</summary>
		public static DcmTag NumberOfFramesInOverlay = new DcmTag(0x6000, 0x0015);

		/// <summary>(60xx,0022) VR=LO Overlay Description</summary>
		public static DcmTag OverlayDescription = new DcmTag(0x6000, 0x0022);

		/// <summary>(60xx,0040) VR=CS Overlay Type</summary>
		public static DcmTag OverlayType = new DcmTag(0x6000, 0x0040);

		/// <summary>(60xx,0045) VR=LO Overlay Subtype</summary>
		public static DcmTag OverlaySubtype = new DcmTag(0x6000, 0x0045);

		/// <summary>(60xx,0050) VR=SS Overlay Origin</summary>
		public static DcmTag OverlayOrigin = new DcmTag(0x6000, 0x0050);

		/// <summary>(60xx,0051) VR=US Image Frame Origin</summary>
		public static DcmTag ImageFrameOrigin = new DcmTag(0x6000, 0x0051);

		/// <summary>(60xx,0052) VR=US Overlay Plane Origin</summary>
		public static DcmTag OverlayPlaneOrigin = new DcmTag(0x6000, 0x0052);

		/// <summary>(60xx,0060) VR=CS Overlay Compression Code (Retired)</summary>
		public static DcmTag OverlayCompressionCodeRETIRED = new DcmTag(0x6000, 0x0060);

		/// <summary>(60xx,0100) VR=US Overlay Bits Allocated</summary>
		public static DcmTag OverlayBitsAllocated = new DcmTag(0x6000, 0x0100);

		/// <summary>(60xx,0102) VR=US Overlay Bit Position</summary>
		public static DcmTag OverlayBitPosition = new DcmTag(0x6000, 0x0102);

		/// <summary>(60xx,0110) VR=CS Overlay Format (Retired)</summary>
		public static DcmTag OverlayFormatRETIRED = new DcmTag(0x6000, 0x0110);

		/// <summary>(60xx,0200) VR=US Overlay Location (Retired)</summary>
		public static DcmTag OverlayLocationRETIRED = new DcmTag(0x6000, 0x0200);

		/// <summary>(60xx,1001) VR=CS Overlay Activation Layer</summary>
		public static DcmTag OverlayActivationLayer = new DcmTag(0x6000, 0x1001);

		/// <summary>(60xx,1100) VR=US Overlay Descriptor - Gray (Retired)</summary>
		public static DcmTag OverlayDescriptorGrayRETIRED = new DcmTag(0x6000, 0x1100);

		/// <summary>(60xx,1101) VR=US Overlay Descriptor - Red (Retired)</summary>
		public static DcmTag OverlayDescriptorRedRETIRED = new DcmTag(0x6000, 0x1101);

		/// <summary>(60xx,1102) VR=US Overlay Descriptor - Green (Retired)</summary>
		public static DcmTag OverlayDescriptorGreenRETIRED = new DcmTag(0x6000, 0x1102);

		/// <summary>(60xx,1103) VR=US Overlay Descriptor - Blue (Retired)</summary>
		public static DcmTag OverlayDescriptorBlueRETIRED = new DcmTag(0x6000, 0x1103);

		/// <summary>(60xx,1200) VR=US Overlays - Gray (Retired)</summary>
		public static DcmTag OverlaysGrayRETIRED = new DcmTag(0x6000, 0x1200);

		/// <summary>(60xx,1201) VR=US Overlays - Red (Retired)</summary>
		public static DcmTag OverlaysRedRETIRED = new DcmTag(0x6000, 0x1201);

		/// <summary>(60xx,1202) VR=US Overlays - Green (Retired)</summary>
		public static DcmTag OverlaysGreenRETIRED = new DcmTag(0x6000, 0x1202);

		/// <summary>(60xx,1203) VR=US Overlays - Blue (Retired)</summary>
		public static DcmTag OverlaysBlueRETIRED = new DcmTag(0x6000, 0x1203);

		/// <summary>(60xx,1301) VR=IS ROI Area</summary>
		public static DcmTag ROIArea = new DcmTag(0x6000, 0x1301);

		/// <summary>(60xx,1302) VR=DS ROI Mean</summary>
		public static DcmTag ROIMean = new DcmTag(0x6000, 0x1302);

		/// <summary>(60xx,1303) VR=DS ROI Standard Deviation</summary>
		public static DcmTag ROIStandardDeviation = new DcmTag(0x6000, 0x1303);

		/// <summary>(60xx,1500) VR=LO Overlay Label</summary>
		public static DcmTag OverlayLabel = new DcmTag(0x6000, 0x1500);

		/// <summary>(60xx,3000) VR=OB Overlay Data</summary>
		public static DcmTag OverlayData = new DcmTag(0x6000, 0x3000);

		/// <summary>(60xx,4000) VR=LT Overlay Comments (Retired)</summary>
		public static DcmTag OverlayCommentsRETIRED = new DcmTag(0x6000, 0x4000);

		/// <summary>(7fe0,0010) VR=OB Pixel Data</summary>
		public static DcmTag PixelData = new DcmTag(0x7fe0, 0x0010);

		/// <summary>(fffa,fffa) VR=SQ Digital Signatures Sequence</summary>
		public static DcmTag DigitalSignaturesSequence = new DcmTag(0xfffa, 0xfffa);

		/// <summary>(fffc,fffc) VR=OB Data Set Trailing Padding</summary>
		public static DcmTag DataSetTrailingPadding = new DcmTag(0xfffc, 0xfffc);

		/// <summary>(fffe,e000) VR= Item</summary>
		public static DcmTag Item = new DcmTag(0xfffe, 0xe000);

		/// <summary>(fffe,e00d) VR= Item Delimitation Item</summary>
		public static DcmTag ItemDelimitationItem = new DcmTag(0xfffe, 0xe00d);

		/// <summary>(fffe,e0dd) VR= Sequence Delimitation Item</summary>
		public static DcmTag SequenceDelimitationItem = new DcmTag(0xfffe, 0xe0dd);
		#endregion
	}

	public static class DcmConstTags {
		#region Const Tags
		public const uint CommandLengthToEndRETIRED = 0x00000001;
		public const uint AffectedSOPClassUID = 0x00000002;
		public const uint RequestedSOPClassUID = 0x00000003;
		public const uint CommandRecognitionCodeRETIRED = 0x00000010;
		public const uint CommandField = 0x00000100;
		public const uint MessageIDFirst = 0x00000110;
		public const uint MessageIDRespondedTo = 0x00000120;
		public const uint MoveDestination = 0x00000600;
		public const uint Priority = 0x00000700;
		public const uint DataSetType = 0x00000800;
		public const uint Status = 0x00000900;
		public const uint OffendingElement = 0x00000901;
		public const uint ErrorComment = 0x00000902;
		public const uint ErrorID = 0x00000903;
		public const uint SOPAffectedInstanceUID = 0x00001000;
		public const uint SOPRequestedInstanceUID = 0x00001001;
		public const uint EventTypeID = 0x00001002;
		public const uint AttributeIdentifierList = 0x00001005;
		public const uint ActionTypeID = 0x00001008;
		public const uint RemainingSuboperations = 0x00001020;
		public const uint CompletedSuboperations = 0x00001021;
		public const uint FailedSuboperations = 0x00001022;
		public const uint WarningSuboperations = 0x00001023;
		public const uint AETitle = 0x00001030;
		public const uint MessageIDSecond = 0x00001031;
		public const uint FileMetaInformationVersion = 0x00020001;
		public const uint MediaStorageSOPClassUID = 0x00020002;
		public const uint MediaStorageSOPInstanceUID = 0x00020003;
		public const uint TransferSyntaxUID = 0x00020010;
		public const uint ImplementationClassUID = 0x00020012;
		public const uint ImplementationVersionName = 0x00020013;
		public const uint SourceApplicationEntityTitle = 0x00020016;
		public const uint PrivateInformationCreatorUID = 0x00020100;
		public const uint PrivateInformation = 0x00020102;
		public const uint FilesetID = 0x00041130;
		public const uint FilesetDescriptorFileID = 0x00041141;
		public const uint SpecificCharacterSetOfFilesetDescriptorFile = 0x00041142;
		public const uint OffsetOfTheFirstDirectoryRecordOfTheRootDirectoryEntity = 0x00041200;
		public const uint OffsetOfTheLastDirectoryRecordOfTheRootDirectoryEntity = 0x00041202;
		public const uint FilesetConsistencyFlag = 0x00041212;
		public const uint DirectoryRecordSequence = 0x00041220;
		public const uint OffsetOfTheNextDirectoryRecord = 0x00041400;
		public const uint RecordInuseFlag = 0x00041410;
		public const uint OffsetOfReferencedLowerLevelDirectoryEntity = 0x00041420;
		public const uint DirectoryRecordType = 0x00041430;
		public const uint PrivateRecordUID = 0x00041432;
		public const uint ReferencedFileID = 0x00041500;
		public const uint MRDRDirectoryRecordOffsetRETIRED = 0x00041504;
		public const uint ReferencedSOPClassUIDInFile = 0x00041510;
		public const uint ReferencedSOPInstanceUIDInFile = 0x00041511;
		public const uint ReferencedTransferSyntaxUIDInFile = 0x00041512;
		public const uint ReferencedRelatedGeneralSOPClassUIDInFile = 0x0004151a;
		public const uint NumberOfReferencesRETIRED = 0x00041600;
		public const uint LengthToEndRETIRED = 0x00080001;
		public const uint SpecificCharacterSet = 0x00080005;
		public const uint ImageType = 0x00080008;
		public const uint RecognitionCodeRETIRED = 0x00080010;
		public const uint InstanceCreationDate = 0x00080012;
		public const uint InstanceCreationTime = 0x00080013;
		public const uint InstanceCreatorUID = 0x00080014;
		public const uint SOPClassUID = 0x00080016;
		public const uint SOPInstanceUID = 0x00080018;
		public const uint RelatedGeneralSOPClassUID = 0x0008001a;
		public const uint OriginalSpecializedSOPClassUID = 0x0008001b;
		public const uint StudyDate = 0x00080020;
		public const uint SeriesDate = 0x00080021;
		public const uint AcquisitionDate = 0x00080022;
		public const uint ContentDate = 0x00080023;
		public const uint OverlayDateRETIRED = 0x00080024;
		public const uint CurveDateRETIRED = 0x00080025;
		public const uint AcquisitionDatetime = 0x0008002a;
		public const uint StudyTime = 0x00080030;
		public const uint SeriesTime = 0x00080031;
		public const uint AcquisitionTime = 0x00080032;
		public const uint ContentTime = 0x00080033;
		public const uint OverlayTimeRETIRED = 0x00080034;
		public const uint CurveTimeRETIRED = 0x00080035;
		public const uint DataSetTypeRETIRED = 0x00080040;
		public const uint DataSetSubtypeRETIRED = 0x00080041;
		public const uint NuclearMedicineSeriesTypeRETIRED = 0x00080042;
		public const uint AccessionNumber = 0x00080050;
		public const uint QueryRetrieveLevel = 0x00080052;
		public const uint RetrieveAETitle = 0x00080054;
		public const uint InstanceAvailability = 0x00080056;
		public const uint FailedSOPInstanceUIDList = 0x00080058;
		public const uint Modality = 0x00080060;
		public const uint ModalitiesInStudy = 0x00080061;
		public const uint SOPClassesInStudy = 0x00080062;
		public const uint ConversionType = 0x00080064;
		public const uint PresentationIntentType = 0x00080068;
		public const uint Manufacturer = 0x00080070;
		public const uint InstitutionName = 0x00080080;
		public const uint InstitutionAddress = 0x00080081;
		public const uint InstitutionCodeSequence = 0x00080082;
		public const uint ReferringPhysiciansName = 0x00080090;
		public const uint ReferringPhysiciansAddress = 0x00080092;
		public const uint ReferringPhysiciansTelephoneNumbers = 0x00080094;
		public const uint ReferringPhysicianIdentificationSequence = 0x00080096;
		public const uint CodeValue = 0x00080100;
		public const uint CodingSchemeDesignator = 0x00080102;
		public const uint CodingSchemeVersion = 0x00080103;
		public const uint CodeMeaning = 0x00080104;
		public const uint MappingResource = 0x00080105;
		public const uint ContextGroupVersion = 0x00080106;
		public const uint ContextGroupLocalVersion = 0x00080107;
		public const uint ContextGroupExtensionFlag = 0x0008010b;
		public const uint CodingSchemeUID = 0x0008010c;
		public const uint ContextGroupExtensionCreatorUID = 0x0008010d;
		public const uint ContextIdentifier = 0x0008010f;
		public const uint CodingSchemeIdentificationSequence = 0x00080110;
		public const uint CodingSchemeRegistry = 0x00080112;
		public const uint CodingSchemeExternalID = 0x00080114;
		public const uint CodingSchemeName = 0x00080115;
		public const uint ResponsibleOrganization = 0x00080116;
		public const uint TimezoneOffsetFromUTC = 0x00080201;
		public const uint NetworkIDRETIRED = 0x00081000;
		public const uint StationName = 0x00081010;
		public const uint StudyDescription = 0x00081030;
		public const uint ProcedureCodeSequence = 0x00081032;
		public const uint SeriesDescription = 0x0008103e;
		public const uint InstitutionalDepartmentName = 0x00081040;
		public const uint PhysiciansOfRecord = 0x00081048;
		public const uint PhysiciansOfRecordIdentificationSequence = 0x00081049;
		public const uint PerformingPhysiciansName = 0x00081050;
		public const uint PerformingPhysicianIdentificationSequence = 0x00081052;
		public const uint NameOfPhysiciansReadingStudy = 0x00081060;
		public const uint PhysiciansReadingStudyIdentificationSequence = 0x00081062;
		public const uint OperatorsName = 0x00081070;
		public const uint OperatorIdentificationSequence = 0x00081072;
		public const uint AdmittingDiagnosesDescription = 0x00081080;
		public const uint AdmittingDiagnosesCodeSequence = 0x00081084;
		public const uint ManufacturersModelName = 0x00081090;
		public const uint ReferencedResultsSequenceRETIRED = 0x00081100;
		public const uint ReferencedStudySequence = 0x00081110;
		public const uint ReferencedPerformedProcedureStepSequence = 0x00081111;
		public const uint ReferencedSeriesSequence = 0x00081115;
		public const uint ReferencedPatientSequence = 0x00081120;
		public const uint ReferencedVisitSequence = 0x00081125;
		public const uint ReferencedOverlaySequenceRETIRED = 0x00081130;
		public const uint ReferencedWaveformSequence = 0x0008113a;
		public const uint ReferencedImageSequence = 0x00081140;
		public const uint ReferencedCurveSequenceRETIRED = 0x00081145;
		public const uint ReferencedInstanceSequence = 0x0008114a;
		public const uint ReferencedRealWorldValueMappingInstanceSequence = 0x0008114b;
		public const uint ReferencedSOPClassUID = 0x00081150;
		public const uint ReferencedSOPInstanceUID = 0x00081155;
		public const uint SOPClassesSupported = 0x0008115a;
		public const uint ReferencedFrameNumber = 0x00081160;
		public const uint TransactionUID = 0x00081195;
		public const uint FailureReason = 0x00081197;
		public const uint FailedSOPSequence = 0x00081198;
		public const uint ReferencedSOPSequence = 0x00081199;
		public const uint StudiesContainingOtherReferencedInstancesSequence = 0x00081200;
		public const uint RelatedSeriesSequence = 0x00081250;
		public const uint LossyImageCompressionRETIRED = 0x00082110;
		public const uint DerivationDescription = 0x00082111;
		public const uint SourceImageSequence = 0x00082112;
		public const uint StageName = 0x00082120;
		public const uint StageNumber = 0x00082122;
		public const uint NumberOfStages = 0x00082124;
		public const uint ViewName = 0x00082127;
		public const uint ViewNumber = 0x00082128;
		public const uint NumberOfEventTimers = 0x00082129;
		public const uint NumberOfViewsInStage = 0x0008212a;
		public const uint EventElapsedTimes = 0x00082130;
		public const uint EventTimerNames = 0x00082132;
		public const uint StartTrim = 0x00082142;
		public const uint StopTrim = 0x00082143;
		public const uint RecommendedDisplayFrameRate = 0x00082144;
		public const uint TransducerPositionRETIRED = 0x00082200;
		public const uint TransducerOrientationRETIRED = 0x00082204;
		public const uint AnatomicStructureRETIRED = 0x00082208;
		public const uint AnatomicRegionSequence = 0x00082218;
		public const uint AnatomicRegionModifierSequence = 0x00082220;
		public const uint PrimaryAnatomicStructureSequence = 0x00082228;
		public const uint AnatomicStructureSpaceOrRegionSequence = 0x00082229;
		public const uint PrimaryAnatomicStructureModifierSequence = 0x00082230;
		public const uint TransducerPositionSequenceRETIRED = 0x00082240;
		public const uint TransducerPositionModifierSequenceRETIRED = 0x00082242;
		public const uint TransducerOrientationSequenceRETIRED = 0x00082244;
		public const uint TransducerOrientationModifierSequenceRETIRED = 0x00082246;
		public const uint AlternateRepresentationSequence = 0x00083001;
		public const uint IrradiationEventUID = 0x00083010;
		public const uint IdentifyingCommentsRETIRED = 0x00084000;
		public const uint FrameType = 0x00089007;
		public const uint ReferencedImageEvidenceSequence = 0x00089092;
		public const uint ReferencedRawDataSequence = 0x00089121;
		public const uint CreatorVersionUID = 0x00089123;
		public const uint DerivationImageSequence = 0x00089124;
		public const uint SourceImageEvidenceSequence = 0x00089154;
		public const uint PixelPresentation = 0x00089205;
		public const uint VolumetricProperties = 0x00089206;
		public const uint VolumeBasedCalculationTechnique = 0x00089207;
		public const uint ComplexImageComponent = 0x00089208;
		public const uint AcquisitionContrast = 0x00089209;
		public const uint DerivationCodeSequence = 0x00089215;
		public const uint ReferencedGrayscalePresentationStateSequence = 0x00089237;
		public const uint ReferencedOtherPlaneSequence = 0x00089410;
		public const uint FrameDisplaySequence = 0x00089458;
		public const uint RecommendedDisplayFrameRateInFloat = 0x00089459;
		public const uint SkipFrameRangeFlag = 0x00089460;
		public const uint PatientsName = 0x00100010;
		public const uint PatientID = 0x00100020;
		public const uint IssuerOfPatientID = 0x00100021;
		public const uint TypeOfPatientID = 0x00100022;
		public const uint PatientsBirthDate = 0x00100030;
		public const uint PatientsBirthTime = 0x00100032;
		public const uint PatientsSex = 0x00100040;
		public const uint PatientsInsurancePlanCodeSequence = 0x00100050;
		public const uint PatientsPrimaryLanguageCodeSequence = 0x00100101;
		public const uint PatientsPrimaryLanguageCodeModifierSequence = 0x00100102;
		public const uint OtherPatientIDs = 0x00101000;
		public const uint OtherPatientNames = 0x00101001;
		public const uint OtherPatientIDsSequence = 0x00101002;
		public const uint PatientsBirthName = 0x00101005;
		public const uint PatientsAge = 0x00101010;
		public const uint PatientsSize = 0x00101020;
		public const uint PatientsWeight = 0x00101030;
		public const uint PatientsAddress = 0x00101040;
		public const uint InsurancePlanIdentificationRETIRED = 0x00101050;
		public const uint PatientsMothersBirthName = 0x00101060;
		public const uint MilitaryRank = 0x00101080;
		public const uint BranchOfService = 0x00101081;
		public const uint MedicalRecordLocator = 0x00101090;
		public const uint MedicalAlerts = 0x00102000;
		public const uint ContrastAllergies = 0x00102110;
		public const uint CountryOfResidence = 0x00102150;
		public const uint RegionOfResidence = 0x00102152;
		public const uint PatientsTelephoneNumbers = 0x00102154;
		public const uint EthnicGroup = 0x00102160;
		public const uint Occupation = 0x00102180;
		public const uint SmokingStatus = 0x001021a0;
		public const uint AdditionalPatientHistory = 0x001021b0;
		public const uint PregnancyStatus = 0x001021c0;
		public const uint LastMenstrualDate = 0x001021d0;
		public const uint PatientsReligiousPreference = 0x001021f0;
		public const uint PatientSpeciesDescription = 0x00102201;
		public const uint PatientSpeciesCodeSequence = 0x00102202;
		public const uint PatientsSexNeutered = 0x00102203;
		public const uint PatientBreedDescription = 0x00102292;
		public const uint PatientBreedCodeSequence = 0x00102293;
		public const uint BreedRegistrationSequence = 0x00102294;
		public const uint BreedRegistrationNumber = 0x00102295;
		public const uint BreedRegistryCodeSequence = 0x00102296;
		public const uint ResponsiblePerson = 0x00102297;
		public const uint ResponsiblePersonRole = 0x00102298;
		public const uint ResponsibleOrganization2 = 0x00102299;
		public const uint PatientComments = 0x00104000;
		public const uint ExaminedBodyThickness = 0x00109431;
		public const uint ClinicalTrialSponsorName = 0x00120010;
		public const uint ClinicalTrialProtocolID = 0x00120020;
		public const uint ClinicalTrialProtocolName = 0x00120021;
		public const uint ClinicalTrialSiteID = 0x00120030;
		public const uint ClinicalTrialSiteName = 0x00120031;
		public const uint ClinicalTrialSubjectID = 0x00120040;
		public const uint ClinicalTrialSubjectReadingID = 0x00120042;
		public const uint ClinicalTrialTimePointID = 0x00120050;
		public const uint ClinicalTrialTimePointDescription = 0x00120051;
		public const uint ClinicalTrialCoordinatingCenterName = 0x00120060;
		public const uint PatientIdentityRemoved = 0x00120062;
		public const uint DeidentificationMethod = 0x00120063;
		public const uint DeidentificationMethodCodeSequence = 0x00120064;
		public const uint ContrastBolusAgent = 0x00180010;
		public const uint ContrastBolusAgentSequence = 0x00180012;
		public const uint ContrastBolusAdministrationRouteSequence = 0x00180014;
		public const uint BodyPartExamined = 0x00180015;
		public const uint ScanningSequence = 0x00180020;
		public const uint SequenceVariant = 0x00180021;
		public const uint ScanOptions = 0x00180022;
		public const uint MRAcquisitionType = 0x00180023;
		public const uint SequenceName = 0x00180024;
		public const uint AngioFlag = 0x00180025;
		public const uint InterventionDrugInformationSequence = 0x00180026;
		public const uint InterventionDrugStopTime = 0x00180027;
		public const uint InterventionDrugDose = 0x00180028;
		public const uint InterventionDrugSequence = 0x00180029;
		public const uint AdditionalDrugSequence = 0x0018002a;
		public const uint RadionuclideRETIRED = 0x00180030;
		public const uint Radiopharmaceutical = 0x00180031;
		public const uint EnergyWindowCenterlineRETIRED = 0x00180032;
		public const uint EnergyWindowTotalWidthRETIRED = 0x00180033;
		public const uint InterventionDrugName = 0x00180034;
		public const uint InterventionDrugStartTime = 0x00180035;
		public const uint InterventionSequence = 0x00180036;
		public const uint TherapyTypeRETIRED = 0x00180037;
		public const uint InterventionStatus = 0x00180038;
		public const uint TherapyDescriptionRETIRED = 0x00180039;
		public const uint InterventionDescription = 0x0018003a;
		public const uint CineRate = 0x00180040;
		public const uint SliceThickness = 0x00180050;
		public const uint KVp = 0x00180060;
		public const uint CountsAccumulated = 0x00180070;
		public const uint AcquisitionTerminationCondition = 0x00180071;
		public const uint EffectiveDuration = 0x00180072;
		public const uint AcquisitionStartCondition = 0x00180073;
		public const uint AcquisitionStartConditionData = 0x00180074;
		public const uint AcquisitionTerminationConditionData = 0x00180075;
		public const uint RepetitionTime = 0x00180080;
		public const uint EchoTime = 0x00180081;
		public const uint InversionTime = 0x00180082;
		public const uint NumberOfAverages = 0x00180083;
		public const uint ImagingFrequency = 0x00180084;
		public const uint ImagedNucleus = 0x00180085;
		public const uint EchoNumbers = 0x00180086;
		public const uint MagneticFieldStrength = 0x00180087;
		public const uint SpacingBetweenSlices = 0x00180088;
		public const uint NumberOfPhaseEncodingSteps = 0x00180089;
		public const uint DataCollectionDiameter = 0x00180090;
		public const uint EchoTrainLength = 0x00180091;
		public const uint PercentSampling = 0x00180093;
		public const uint PercentPhaseFieldOfView = 0x00180094;
		public const uint PixelBandwidth = 0x00180095;
		public const uint DeviceSerialNumber = 0x00181000;
		public const uint DeviceUID = 0x00181002;
		public const uint DeviceID = 0x00181003;
		public const uint PlateID = 0x00181004;
		public const uint GeneratorID = 0x00181005;
		public const uint GridID = 0x00181006;
		public const uint CassetteID = 0x00181007;
		public const uint GantryID = 0x00181008;
		public const uint SecondaryCaptureDeviceID = 0x00181010;
		public const uint HardcopyCreationDeviceID = 0x00181011;
		public const uint DateOfSecondaryCapture = 0x00181012;
		public const uint TimeOfSecondaryCapture = 0x00181014;
		public const uint SecondaryCaptureDeviceManufacturer = 0x00181016;
		public const uint HardcopyDeviceManufacturer = 0x00181017;
		public const uint SecondaryCaptureDeviceManufacturersModelName = 0x00181018;
		public const uint SecondaryCaptureDeviceSoftwareVersions = 0x00181019;
		public const uint HardcopyDeviceSoftwareVersion = 0x0018101a;
		public const uint HardcopyDeviceManufacturersModelName = 0x0018101b;
		public const uint SoftwareVersions = 0x00181020;
		public const uint VideoImageFormatAcquired = 0x00181022;
		public const uint DigitalImageFormatAcquired = 0x00181023;
		public const uint ProtocolName = 0x00181030;
		public const uint ContrastBolusRoute = 0x00181040;
		public const uint ContrastBolusVolume = 0x00181041;
		public const uint ContrastBolusStartTime = 0x00181042;
		public const uint ContrastBolusStopTime = 0x00181043;
		public const uint ContrastBolusTotalDose = 0x00181044;
		public const uint SyringeCounts = 0x00181045;
		public const uint ContrastFlowRate = 0x00181046;
		public const uint ContrastFlowDuration = 0x00181047;
		public const uint ContrastBolusIngredient = 0x00181048;
		public const uint ContrastBolusIngredientConcentration = 0x00181049;
		public const uint SpatialResolution = 0x00181050;
		public const uint TriggerTime = 0x00181060;
		public const uint TriggerSourceOrType = 0x00181061;
		public const uint NominalInterval = 0x00181062;
		public const uint FrameTime = 0x00181063;
		public const uint FramingType = 0x00181064;
		public const uint FrameTimeVector = 0x00181065;
		public const uint FrameDelay = 0x00181066;
		public const uint ImageTriggerDelay = 0x00181067;
		public const uint MultiplexGroupTimeOffset = 0x00181068;
		public const uint TriggerTimeOffset = 0x00181069;
		public const uint SynchronizationTrigger = 0x0018106a;
		public const uint SynchronizationChannel = 0x0018106c;
		public const uint TriggerSamplePosition = 0x0018106e;
		public const uint RadiopharmaceuticalRoute = 0x00181070;
		public const uint RadiopharmaceuticalVolume = 0x00181071;
		public const uint RadiopharmaceuticalStartTime = 0x00181072;
		public const uint RadiopharmaceuticalStopTime = 0x00181073;
		public const uint RadionuclideTotalDose = 0x00181074;
		public const uint RadionuclideHalfLife = 0x00181075;
		public const uint RadionuclidePositronFraction = 0x00181076;
		public const uint RadiopharmaceuticalSpecificActivity = 0x00181077;
		public const uint RadiopharmaceuticalStartDatetime = 0x00181078;
		public const uint RadiopharmaceuticalStopDatetime = 0x00181079;
		public const uint BeatRejectionFlag = 0x00181080;
		public const uint LowRRValue = 0x00181081;
		public const uint HighRRValue = 0x00181082;
		public const uint IntervalsAcquired = 0x00181083;
		public const uint IntervalsRejected = 0x00181084;
		public const uint PVCRejection = 0x00181085;
		public const uint SkipBeats = 0x00181086;
		public const uint HeartRate = 0x00181088;
		public const uint CardiacNumberOfImages = 0x00181090;
		public const uint TriggerWindow = 0x00181094;
		public const uint ReconstructionDiameter = 0x00181100;
		public const uint DistanceSourceToDetector = 0x00181110;
		public const uint DistanceSourceToPatient = 0x00181111;
		public const uint EstimatedRadiographicMagnificationFactor = 0x00181114;
		public const uint GantryDetectorTilt = 0x00181120;
		public const uint GantryDetectorSlew = 0x00181121;
		public const uint TableHeight = 0x00181130;
		public const uint TableTraverse = 0x00181131;
		public const uint TableMotion = 0x00181134;
		public const uint TableVerticalIncrement = 0x00181135;
		public const uint TableLateralIncrement = 0x00181136;
		public const uint TableLongitudinalIncrement = 0x00181137;
		public const uint TableAngle = 0x00181138;
		public const uint TableType = 0x0018113a;
		public const uint RotationDirection = 0x00181140;
		public const uint AngularPosition = 0x00181141;
		public const uint RadialPosition = 0x00181142;
		public const uint ScanArc = 0x00181143;
		public const uint AngularStep = 0x00181144;
		public const uint CenterOfRotationOffset = 0x00181145;
		public const uint RotationOffsetRETIRED = 0x00181146;
		public const uint FieldOfViewShape = 0x00181147;
		public const uint FieldOfViewDimensions = 0x00181149;
		public const uint ExposureTime = 0x00181150;
		public const uint XrayTubeCurrent = 0x00181151;
		public const uint Exposure = 0x00181152;
		public const uint ExposureInµAs = 0x00181153;
		public const uint AveragePulseWidth = 0x00181154;
		public const uint RadiationSetting = 0x00181155;
		public const uint RectificationType = 0x00181156;
		public const uint RadiationMode = 0x0018115a;
		public const uint ImageAndFluoroscopyAreaDoseProduct = 0x0018115e;
		public const uint FilterType = 0x00181160;
		public const uint TypeOfFilters = 0x00181161;
		public const uint IntensifierSize = 0x00181162;
		public const uint ImagerPixelSpacing = 0x00181164;
		public const uint Grid = 0x00181166;
		public const uint GeneratorPower = 0x00181170;
		public const uint CollimatorgridName = 0x00181180;
		public const uint CollimatorType = 0x00181181;
		public const uint FocalDistance = 0x00181182;
		public const uint XFocusCenter = 0x00181183;
		public const uint YFocusCenter = 0x00181184;
		public const uint FocalSpots = 0x00181190;
		public const uint AnodeTargetMaterial = 0x00181191;
		public const uint BodyPartThickness = 0x001811a0;
		public const uint CompressionForce = 0x001811a2;
		public const uint DateOfLastCalibration = 0x00181200;
		public const uint TimeOfLastCalibration = 0x00181201;
		public const uint ConvolutionKernel = 0x00181210;
		public const uint UpperLowerPixelValuesRETIRED = 0x00181240;
		public const uint ActualFrameDuration = 0x00181242;
		public const uint CountRate = 0x00181243;
		public const uint PreferredPlaybackSequencing = 0x00181244;
		public const uint ReceiveCoilName = 0x00181250;
		public const uint TransmitCoilName = 0x00181251;
		public const uint PlateType = 0x00181260;
		public const uint PhosphorType = 0x00181261;
		public const uint ScanVelocity = 0x00181300;
		public const uint WholeBodyTechnique = 0x00181301;
		public const uint ScanLength = 0x00181302;
		public const uint AcquisitionMatrix = 0x00181310;
		public const uint InplanePhaseEncodingDirection = 0x00181312;
		public const uint FlipAngle = 0x00181314;
		public const uint VariableFlipAngleFlag = 0x00181315;
		public const uint SAR = 0x00181316;
		public const uint DBdt = 0x00181318;
		public const uint AcquisitionDeviceProcessingDescription = 0x00181400;
		public const uint AcquisitionDeviceProcessingCode = 0x00181401;
		public const uint CassetteOrientation = 0x00181402;
		public const uint CassetteSize = 0x00181403;
		public const uint ExposuresOnPlate = 0x00181404;
		public const uint RelativeXrayExposure = 0x00181405;
		public const uint ColumnAngulation = 0x00181450;
		public const uint TomoLayerHeight = 0x00181460;
		public const uint TomoAngle = 0x00181470;
		public const uint TomoTime = 0x00181480;
		public const uint TomoType = 0x00181490;
		public const uint TomoClass = 0x00181491;
		public const uint NumberOfTomosynthesisSourceImages = 0x00181495;
		public const uint PositionerMotion = 0x00181500;
		public const uint PositionerType = 0x00181508;
		public const uint PositionerPrimaryAngle = 0x00181510;
		public const uint PositionerSecondaryAngle = 0x00181511;
		public const uint PositionerPrimaryAngleIncrement = 0x00181520;
		public const uint PositionerSecondaryAngleIncrement = 0x00181521;
		public const uint DetectorPrimaryAngle = 0x00181530;
		public const uint DetectorSecondaryAngle = 0x00181531;
		public const uint ShutterShape = 0x00181600;
		public const uint ShutterLeftVerticalEdge = 0x00181602;
		public const uint ShutterRightVerticalEdge = 0x00181604;
		public const uint ShutterUpperHorizontalEdge = 0x00181606;
		public const uint ShutterLowerHorizontalEdge = 0x00181608;
		public const uint CenterOfCircularShutter = 0x00181610;
		public const uint RadiusOfCircularShutter = 0x00181612;
		public const uint VerticesOfThePolygonalShutter = 0x00181620;
		public const uint ShutterPresentationValue = 0x00181622;
		public const uint ShutterOverlayGroup = 0x00181623;
		public const uint ShutterPresentationColorCIELabValue = 0x00181624;
		public const uint CollimatorShape = 0x00181700;
		public const uint CollimatorLeftVerticalEdge = 0x00181702;
		public const uint CollimatorRightVerticalEdge = 0x00181704;
		public const uint CollimatorUpperHorizontalEdge = 0x00181706;
		public const uint CollimatorLowerHorizontalEdge = 0x00181708;
		public const uint CenterOfCircularCollimator = 0x00181710;
		public const uint RadiusOfCircularCollimator = 0x00181712;
		public const uint VerticesOfThePolygonalCollimator = 0x00181720;
		public const uint AcquisitionTimeSynchronized = 0x00181800;
		public const uint TimeSource = 0x00181801;
		public const uint TimeDistributionProtocol = 0x00181802;
		public const uint NTPSourceAddress = 0x00181803;
		public const uint PageNumberVector = 0x00182001;
		public const uint FrameLabelVector = 0x00182002;
		public const uint FramePrimaryAngleVector = 0x00182003;
		public const uint FrameSecondaryAngleVector = 0x00182004;
		public const uint SliceLocationVector = 0x00182005;
		public const uint DisplayWindowLabelVector = 0x00182006;
		public const uint NominalScannedPixelSpacing = 0x00182010;
		public const uint DigitizingDeviceTransportDirection = 0x00182020;
		public const uint RotationOfScannedFilm = 0x00182030;
		public const uint IVUSAcquisition = 0x00183100;
		public const uint IVUSPullbackRate = 0x00183101;
		public const uint IVUSGatedRate = 0x00183102;
		public const uint IVUSPullbackStartFrameNumber = 0x00183103;
		public const uint IVUSPullbackStopFrameNumber = 0x00183104;
		public const uint LesionNumber = 0x00183105;
		public const uint AcquisitionCommentsRETIRED = 0x00184000;
		public const uint OutputPower = 0x00185000;
		public const uint TransducerData = 0x00185010;
		public const uint FocusDepth = 0x00185012;
		public const uint ProcessingFunction = 0x00185020;
		public const uint PostprocessingFunction = 0x00185021;
		public const uint MechanicalIndex = 0x00185022;
		public const uint BoneThermalIndex = 0x00185024;
		public const uint CranialThermalIndex = 0x00185026;
		public const uint SoftTissueThermalIndex = 0x00185027;
		public const uint SoftTissuefocusThermalIndex = 0x00185028;
		public const uint SoftTissuesurfaceThermalIndex = 0x00185029;
		public const uint DynamicRangeRETIRED = 0x00185030;
		public const uint TotalGainRETIRED = 0x00185040;
		public const uint DepthOfScanField = 0x00185050;
		public const uint PatientPosition = 0x00185100;
		public const uint ViewPosition = 0x00185101;
		public const uint ProjectionEponymousNameCodeSequence = 0x00185104;
		public const uint ImageTransformationMatrixRETIRED = 0x00185210;
		public const uint ImageTranslationVectorRETIRED = 0x00185212;
		public const uint Sensitivity = 0x00186000;
		public const uint SequenceOfUltrasoundRegions = 0x00186011;
		public const uint RegionSpatialFormat = 0x00186012;
		public const uint RegionDataType = 0x00186014;
		public const uint RegionFlags = 0x00186016;
		public const uint RegionLocationMinX0 = 0x00186018;
		public const uint RegionLocationMinY0 = 0x0018601a;
		public const uint RegionLocationMaxX1 = 0x0018601c;
		public const uint RegionLocationMaxY1 = 0x0018601e;
		public const uint ReferencePixelX0 = 0x00186020;
		public const uint ReferencePixelY0 = 0x00186022;
		public const uint PhysicalUnitsXDirection = 0x00186024;
		public const uint PhysicalUnitsYDirection = 0x00186026;
		public const uint ReferencePixelPhysicalValueX = 0x00186028;
		public const uint ReferencePixelPhysicalValueY = 0x0018602a;
		public const uint PhysicalDeltaX = 0x0018602c;
		public const uint PhysicalDeltaY = 0x0018602e;
		public const uint TransducerFrequency = 0x00186030;
		public const uint TransducerType = 0x00186031;
		public const uint PulseRepetitionFrequency = 0x00186032;
		public const uint DopplerCorrectionAngle = 0x00186034;
		public const uint SteeringAngle = 0x00186036;
		public const uint DopplerSampleVolumeXPositionRETIRED = 0x00186038;
		public const uint DopplerSampleVolumeXPosition = 0x00186039;
		public const uint DopplerSampleVolumeYPositionRETIRED = 0x0018603a;
		public const uint DopplerSampleVolumeYPosition = 0x0018603b;
		public const uint TMLinePositionX0RETIRED = 0x0018603c;
		public const uint TMLinePositionX0 = 0x0018603d;
		public const uint TMLinePositionY0RETIRED = 0x0018603e;
		public const uint TMLinePositionY0 = 0x0018603f;
		public const uint TMLinePositionX1RETIRED = 0x00186040;
		public const uint TMLinePositionX1 = 0x00186041;
		public const uint TMLinePositionY1RETIRED = 0x00186042;
		public const uint TMLinePositionY1 = 0x00186043;
		public const uint PixelComponentOrganization = 0x00186044;
		public const uint PixelComponentMask = 0x00186046;
		public const uint PixelComponentRangeStart = 0x00186048;
		public const uint PixelComponentRangeStop = 0x0018604a;
		public const uint PixelComponentPhysicalUnits = 0x0018604c;
		public const uint PixelComponentDataType = 0x0018604e;
		public const uint NumberOfTableBreakPoints = 0x00186050;
		public const uint TableOfXBreakPoints = 0x00186052;
		public const uint TableOfYBreakPoints = 0x00186054;
		public const uint NumberOfTableEntries = 0x00186056;
		public const uint TableOfPixelValues = 0x00186058;
		public const uint TableOfParameterValues = 0x0018605a;
		public const uint RWaveTimeVector = 0x00186060;
		public const uint DetectorConditionsNominalFlag = 0x00187000;
		public const uint DetectorTemperature = 0x00187001;
		public const uint DetectorType = 0x00187004;
		public const uint DetectorConfiguration = 0x00187005;
		public const uint DetectorDescription = 0x00187006;
		public const uint DetectorMode = 0x00187008;
		public const uint DetectorID = 0x0018700a;
		public const uint DateOfLastDetectorCalibration = 0x0018700c;
		public const uint TimeOfLastDetectorCalibration = 0x0018700e;
		public const uint ExposuresOnDetectorSinceLastCalibration = 0x00187010;
		public const uint ExposuresOnDetectorSinceManufactured = 0x00187011;
		public const uint DetectorTimeSinceLastExposure = 0x00187012;
		public const uint DetectorActiveTime = 0x00187014;
		public const uint DetectorActivationOffsetFromExposure = 0x00187016;
		public const uint DetectorBinning = 0x0018701a;
		public const uint DetectorElementPhysicalSize = 0x00187020;
		public const uint DetectorElementSpacing = 0x00187022;
		public const uint DetectorActiveShape = 0x00187024;
		public const uint DetectorActiveDimensions = 0x00187026;
		public const uint DetectorActiveOrigin = 0x00187028;
		public const uint DetectorManufacturerName = 0x0018702a;
		public const uint DetectorManufacturersModelName = 0x0018702b;
		public const uint FieldOfViewOrigin = 0x00187030;
		public const uint FieldOfViewRotation = 0x00187032;
		public const uint FieldOfViewHorizontalFlip = 0x00187034;
		public const uint GridAbsorbingMaterial = 0x00187040;
		public const uint GridSpacingMaterial = 0x00187041;
		public const uint GridThickness = 0x00187042;
		public const uint GridPitch = 0x00187044;
		public const uint GridAspectRatio = 0x00187046;
		public const uint GridPeriod = 0x00187048;
		public const uint GridFocalDistance = 0x0018704c;
		public const uint FilterMaterial = 0x00187050;
		public const uint FilterThicknessMinimum = 0x00187052;
		public const uint FilterThicknessMaximum = 0x00187054;
		public const uint ExposureControlMode = 0x00187060;
		public const uint ExposureControlModeDescription = 0x00187062;
		public const uint ExposureStatus = 0x00187064;
		public const uint PhototimerSetting = 0x00187065;
		public const uint ExposureTimeInµS = 0x00188150;
		public const uint XRayTubeCurrentInµA = 0x00188151;
		public const uint ContentQualification = 0x00189004;
		public const uint PulseSequenceName = 0x00189005;
		public const uint MRImagingModifierSequence = 0x00189006;
		public const uint EchoPulseSequence = 0x00189008;
		public const uint InversionRecovery = 0x00189009;
		public const uint FlowCompensation = 0x00189010;
		public const uint MultipleSpinEcho = 0x00189011;
		public const uint MultiplanarExcitation = 0x00189012;
		public const uint PhaseContrast = 0x00189014;
		public const uint TimeOfFlightContrast = 0x00189015;
		public const uint Spoiling = 0x00189016;
		public const uint SteadyStatePulseSequence = 0x00189017;
		public const uint EchoPlanarPulseSequence = 0x00189018;
		public const uint TagAngleFirstAxis = 0x00189019;
		public const uint MagnetizationTransfer = 0x00189020;
		public const uint T2Preparation = 0x00189021;
		public const uint BloodSignalNulling = 0x00189022;
		public const uint SaturationRecovery = 0x00189024;
		public const uint SpectrallySelectedSuppression = 0x00189025;
		public const uint SpectrallySelectedExcitation = 0x00189026;
		public const uint SpatialPresaturation = 0x00189027;
		public const uint Tagging = 0x00189028;
		public const uint OversamplingPhase = 0x00189029;
		public const uint TagSpacingFirstDimension = 0x00189030;
		public const uint GeometryOfKSpaceTraversal = 0x00189032;
		public const uint SegmentedKSpaceTraversal = 0x00189033;
		public const uint RectilinearPhaseEncodeReordering = 0x00189034;
		public const uint TagThickness = 0x00189035;
		public const uint PartialFourierDirection = 0x00189036;
		public const uint CardiacSynchronizationTechnique = 0x00189037;
		public const uint ReceiveCoilManufacturerName = 0x00189041;
		public const uint MRReceiveCoilSequence = 0x00189042;
		public const uint ReceiveCoilType = 0x00189043;
		public const uint QuadratureReceiveCoil = 0x00189044;
		public const uint MultiCoilDefinitionSequence = 0x00189045;
		public const uint MultiCoilConfiguration = 0x00189046;
		public const uint MultiCoilElementName = 0x00189047;
		public const uint MultiCoilElementUsed = 0x00189048;
		public const uint MRTransmitCoilSequence = 0x00189049;
		public const uint TransmitCoilManufacturerName = 0x00189050;
		public const uint TransmitCoilType = 0x00189051;
		public const uint SpectralWidth = 0x00189052;
		public const uint ChemicalShiftReference = 0x00189053;
		public const uint VolumeLocalizationTechnique = 0x00189054;
		public const uint MRAcquisitionFrequencyEncodingSteps = 0x00189058;
		public const uint Decoupling = 0x00189059;
		public const uint DecoupledNucleus = 0x00189060;
		public const uint DecouplingFrequency = 0x00189061;
		public const uint DecouplingMethod = 0x00189062;
		public const uint DecouplingChemicalShiftReference = 0x00189063;
		public const uint KspaceFiltering = 0x00189064;
		public const uint TimeDomainFiltering = 0x00189065;
		public const uint NumberOfZeroFills = 0x00189066;
		public const uint BaselineCorrection = 0x00189067;
		public const uint ParallelReductionFactorInplane = 0x00189069;
		public const uint CardiacRRIntervalSpecified = 0x00189070;
		public const uint AcquisitionDuration = 0x00189073;
		public const uint FrameAcquisitionDatetime = 0x00189074;
		public const uint DiffusionDirectionality = 0x00189075;
		public const uint DiffusionGradientDirectionSequence = 0x00189076;
		public const uint ParallelAcquisition = 0x00189077;
		public const uint ParallelAcquisitionTechnique = 0x00189078;
		public const uint InversionTimes = 0x00189079;
		public const uint MetaboliteMapDescription = 0x00189080;
		public const uint PartialFourier = 0x00189081;
		public const uint EffectiveEchoTime = 0x00189082;
		public const uint MetaboliteMapCodeSequence = 0x00189083;
		public const uint ChemicalShiftSequence = 0x00189084;
		public const uint CardiacSignalSource = 0x00189085;
		public const uint DiffusionBvalue = 0x00189087;
		public const uint DiffusionGradientOrientation = 0x00189089;
		public const uint VelocityEncodingDirection = 0x00189090;
		public const uint VelocityEncodingMinimumValue = 0x00189091;
		public const uint NumberOfKSpaceTrajectories = 0x00189093;
		public const uint CoverageOfKSpace = 0x00189094;
		public const uint SpectroscopyAcquisitionPhaseRows = 0x00189095;
		public const uint TransmitterFrequency = 0x00189098;
		public const uint ResonantNucleus = 0x00189100;
		public const uint FrequencyCorrection = 0x00189101;
		public const uint MRSpectroscopyFOVGeometrySequence = 0x00189103;
		public const uint SlabThickness = 0x00189104;
		public const uint SlabOrientation = 0x00189105;
		public const uint MidSlabPosition = 0x00189106;
		public const uint MRSpatialSaturationSequence = 0x00189107;
		public const uint MRTimingAndRelatedParametersSequence = 0x00189112;
		public const uint MREchoSequence = 0x00189114;
		public const uint MRModifierSequence = 0x00189115;
		public const uint MRDiffusionSequence = 0x00189117;
		public const uint CardiacTriggerSequence = 0x00189118;
		public const uint MRAveragesSequence = 0x00189119;
		public const uint MRFOVGeometrySequence = 0x00189125;
		public const uint VolumeLocalizationSequence = 0x00189126;
		public const uint SpectroscopyAcquisitionDataColumns = 0x00189127;
		public const uint DiffusionAnisotropyType = 0x00189147;
		public const uint FrameReferenceDatetime = 0x00189151;
		public const uint MRMetaboliteMapSequence = 0x00189152;
		public const uint ParallelReductionFactorOutofplane = 0x00189155;
		public const uint SpectroscopyAcquisitionOutofplanePhaseSteps = 0x00189159;
		public const uint BulkMotionStatus = 0x00189166;
		public const uint ParallelReductionFactorSecondInplane = 0x00189168;
		public const uint CardiacBeatRejectionTechnique = 0x00189169;
		public const uint RespiratoryMotionCompensationTechnique = 0x00189170;
		public const uint RespiratorySignalSource = 0x00189171;
		public const uint BulkMotionCompensationTechnique = 0x00189172;
		public const uint BulkMotionSignalSource = 0x00189173;
		public const uint ApplicableSafetyStandardAgency = 0x00189174;
		public const uint ApplicableSafetyStandardDescription = 0x00189175;
		public const uint OperatingModeSequence = 0x00189176;
		public const uint OperatingModeType = 0x00189177;
		public const uint OperatingMode = 0x00189178;
		public const uint SpecificAbsorptionRateDefinition = 0x00189179;
		public const uint GradientOutputType = 0x00189180;
		public const uint SpecificAbsorptionRateValue = 0x00189181;
		public const uint GradientOutput = 0x00189182;
		public const uint FlowCompensationDirection = 0x00189183;
		public const uint TaggingDelay = 0x00189184;
		public const uint RespiratoryMotionCompensationTechniqueDescription = 0x00189185;
		public const uint RespiratorySignalSourceID = 0x00189186;
		public const uint ChemicalShiftsMinimumIntegrationLimitInHzRETIRED = 0x00189195;
		public const uint ChemicalShiftsMaximumIntegrationLimitInHzRETIRED = 0x00189196;
		public const uint MRVelocityEncodingSequence = 0x00189197;
		public const uint FirstOrderPhaseCorrection = 0x00189198;
		public const uint WaterReferencedPhaseCorrection = 0x00189199;
		public const uint MRSpectroscopyAcquisitionType = 0x00189200;
		public const uint RespiratoryCyclePosition = 0x00189214;
		public const uint VelocityEncodingMaximumValue = 0x00189217;
		public const uint TagSpacingSecondDimension = 0x00189218;
		public const uint TagAngleSecondAxis = 0x00189219;
		public const uint FrameAcquisitionDuration = 0x00189220;
		public const uint MRImageFrameTypeSequence = 0x00189226;
		public const uint MRSpectroscopyFrameTypeSequence = 0x00189227;
		public const uint MRAcquisitionPhaseEncodingStepsInplane = 0x00189231;
		public const uint MRAcquisitionPhaseEncodingStepsOutofplane = 0x00189232;
		public const uint SpectroscopyAcquisitionPhaseColumns = 0x00189234;
		public const uint CardiacCyclePosition = 0x00189236;
		public const uint SpecificAbsorptionRateSequence = 0x00189239;
		public const uint RFEchoTrainLength = 0x00189240;
		public const uint GradientEchoTrainLength = 0x00189241;
		public const uint ChemicalShiftsMinimumIntegrationLimitInPpm = 0x00189295;
		public const uint ChemicalShiftsMaximumIntegrationLimitInPpm = 0x00189296;
		public const uint CTAcquisitionTypeSequence = 0x00189301;
		public const uint AcquisitionType = 0x00189302;
		public const uint TubeAngle = 0x00189303;
		public const uint CTAcquisitionDetailsSequence = 0x00189304;
		public const uint RevolutionTime = 0x00189305;
		public const uint SingleCollimationWidth = 0x00189306;
		public const uint TotalCollimationWidth = 0x00189307;
		public const uint CTTableDynamicsSequence = 0x00189308;
		public const uint TableSpeed = 0x00189309;
		public const uint TableFeedPerRotation = 0x00189310;
		public const uint SpiralPitchFactor = 0x00189311;
		public const uint CTGeometrySequence = 0x00189312;
		public const uint DataCollectionCenterPatient = 0x00189313;
		public const uint CTReconstructionSequence = 0x00189314;
		public const uint ReconstructionAlgorithm = 0x00189315;
		public const uint ConvolutionKernelGroup = 0x00189316;
		public const uint ReconstructionFieldOfView = 0x00189317;
		public const uint ReconstructionTargetCenterPatient = 0x00189318;
		public const uint ReconstructionAngle = 0x00189319;
		public const uint ImageFilter = 0x00189320;
		public const uint CTExposureSequence = 0x00189321;
		public const uint ReconstructionPixelSpacing = 0x00189322;
		public const uint ExposureModulationType = 0x00189323;
		public const uint EstimatedDoseSaving = 0x00189324;
		public const uint CTXrayDetailsSequence = 0x00189325;
		public const uint CTPositionSequence = 0x00189326;
		public const uint TablePosition = 0x00189327;
		public const uint ExposureTimeInMs = 0x00189328;
		public const uint CTImageFrameTypeSequence = 0x00189329;
		public const uint XRayTubeCurrentInMA = 0x00189330;
		public const uint ExposureInMAs = 0x00189332;
		public const uint ConstantVolumeFlag = 0x00189333;
		public const uint FluoroscopyFlag = 0x00189334;
		public const uint DistanceSourceToDataCollectionCenter = 0x00189335;
		public const uint ContrastBolusAgentNumber = 0x00189337;
		public const uint ContrastBolusIngredientCodeSequence = 0x00189338;
		public const uint ContrastAdministrationProfileSequence = 0x00189340;
		public const uint ContrastBolusUsageSequence = 0x00189341;
		public const uint ContrastBolusAgentAdministered = 0x00189342;
		public const uint ContrastBolusAgentDetected = 0x00189343;
		public const uint ContrastBolusAgentPhase = 0x00189344;
		public const uint CTDIvol = 0x00189345;
		public const uint ProjectionPixelCalibrationSequence = 0x00189401;
		public const uint DistanceSourceToIsocenter = 0x00189402;
		public const uint DistanceObjectToTableTop = 0x00189403;
		public const uint ObjectPixelSpacingInCenterOfBeam = 0x00189404;
		public const uint PositionerPositionSequence = 0x00189405;
		public const uint TablePositionSequence = 0x00189406;
		public const uint CollimatorShapeSequence = 0x00189407;
		public const uint XAXRFFrameCharacteristicsSequence = 0x00189412;
		public const uint FrameAcquisitionSequence = 0x00189417;
		public const uint XRayReceptorType = 0x00189420;
		public const uint AcquisitionProtocolName = 0x00189423;
		public const uint AcquisitionProtocolDescription = 0x00189424;
		public const uint ContrastBolusIngredientOpaque = 0x00189425;
		public const uint DistanceReceptorPlaneToDetectorHousing = 0x00189426;
		public const uint IntensifierActiveShape = 0x00189427;
		public const uint IntensifierActiveDimensions = 0x00189428;
		public const uint PhysicalDetectorSize = 0x00189429;
		public const uint PositionOfIsocenterProjection = 0x00189430;
		public const uint FieldOfViewSequence = 0x00189432;
		public const uint FieldOfViewDescription = 0x00189433;
		public const uint ExposureControlSensingRegionsSequence = 0x00189434;
		public const uint ExposureControlSensingRegionShape = 0x00189435;
		public const uint ExposureControlSensingRegionLeftVerticalEdge = 0x00189436;
		public const uint ExposureControlSensingRegionRightVerticalEdge = 0x00189437;
		public const uint ExposureControlSensingRegionUpperHorizontalEdge = 0x00189438;
		public const uint ExposureControlSensingRegionLowerHorizontalEdge = 0x00189439;
		public const uint CenterOfCircularExposureControlSensingRegion = 0x00189440;
		public const uint RadiusOfCircularExposureControlSensingRegion = 0x00189441;
		public const uint VerticesOfThePolygonalExposureControlSensingRegion = 0x00189442;
		public const uint SHALLNOTBEUSEDRETIRED = 0x00189445;
		public const uint ColumnAngulationPatient = 0x00189447;
		public const uint BeamAngle = 0x00189449;
		public const uint FrameDetectorParametersSequence = 0x00189451;
		public const uint CalculatedAnatomyThickness = 0x00189452;
		public const uint CalibrationSequence = 0x00189455;
		public const uint ObjectThicknessSequence = 0x00189456;
		public const uint PlaneIdentification = 0x00189457;
		public const uint FieldOfViewDimensionsInFloat = 0x00189461;
		public const uint IsocenterReferenceSystemSequence = 0x00189462;
		public const uint PositionerIsocenterPrimaryAngle = 0x00189463;
		public const uint PositionerIsocenterSecondaryAngle = 0x00189464;
		public const uint PositionerIsocenterDetectorRotationAngle = 0x00189465;
		public const uint TableXPositionToIsocenter = 0x00189466;
		public const uint TableYPositionToIsocenter = 0x00189467;
		public const uint TableZPositionToIsocenter = 0x00189468;
		public const uint TableHorizontalRotationAngle = 0x00189469;
		public const uint TableHeadTiltAngle = 0x00189470;
		public const uint TableCradleTiltAngle = 0x00189471;
		public const uint FrameDisplayShutterSequence = 0x00189472;
		public const uint AcquiredImageAreaDoseProduct = 0x00189473;
		public const uint CarmPositionerTabletopRelationship = 0x00189474;
		public const uint XRayGeometrySequence = 0x00189476;
		public const uint IrradiationEventIdentificationSequence = 0x00189477;
		public const uint ContributingEquipmentSequence = 0x0018a001;
		public const uint ContributionDateTime = 0x0018a002;
		public const uint ContributionDescription = 0x0018a003;
		public const uint StudyInstanceUID = 0x0020000d;
		public const uint SeriesInstanceUID = 0x0020000e;
		public const uint StudyID = 0x00200010;
		public const uint SeriesNumber = 0x00200011;
		public const uint AcquisitionNumber = 0x00200012;
		public const uint InstanceNumber = 0x00200013;
		public const uint IsotopeNumberRETIRED = 0x00200014;
		public const uint PhaseNumberRETIRED = 0x00200015;
		public const uint IntervalNumberRETIRED = 0x00200016;
		public const uint TimeSlotNumberRETIRED = 0x00200017;
		public const uint AngleNumberRETIRED = 0x00200018;
		public const uint ItemNumber = 0x00200019;
		public const uint PatientOrientation = 0x00200020;
		public const uint OverlayNumberRETIRED = 0x00200022;
		public const uint CurveNumberRETIRED = 0x00200024;
		public const uint LookupTableNumberRETIRED = 0x00200026;
		public const uint ImagePositionRETIRED = 0x00200030;
		public const uint ImagePositionPatient = 0x00200032;
		public const uint ImageOrientationRETIRED = 0x00200035;
		public const uint ImageOrientationPatient = 0x00200037;
		public const uint LocationRETIRED = 0x00200050;
		public const uint FrameOfReferenceUID = 0x00200052;
		public const uint Laterality = 0x00200060;
		public const uint ImageLaterality = 0x00200062;
		public const uint ImageGeometryTypeRETIRED = 0x00200070;
		public const uint MaskingImageRETIRED = 0x00200080;
		public const uint TemporalPositionIdentifier = 0x00200100;
		public const uint NumberOfTemporalPositions = 0x00200105;
		public const uint TemporalResolution = 0x00200110;
		public const uint SynchronizationFrameOfReferenceUID = 0x00200200;
		public const uint SeriesInStudyRETIRED = 0x00201000;
		public const uint AcquisitionsInSeriesRETIRED = 0x00201001;
		public const uint ImagesInAcquisition = 0x00201002;
		public const uint ImagesInSeriesRETIRED = 0x00201003;
		public const uint AcquisitionsInStudyRETIRED = 0x00201004;
		public const uint ImagesInStudyRETIRED = 0x00201005;
		public const uint ReferenceRETIRED = 0x00201020;
		public const uint PositionReferenceIndicator = 0x00201040;
		public const uint SliceLocation = 0x00201041;
		public const uint OtherStudyNumbersRETIRED = 0x00201070;
		public const uint NumberOfPatientRelatedStudies = 0x00201200;
		public const uint NumberOfPatientRelatedSeries = 0x00201202;
		public const uint NumberOfPatientRelatedInstances = 0x00201204;
		public const uint NumberOfStudyRelatedSeries = 0x00201206;
		public const uint NumberOfStudyRelatedInstances = 0x00201208;
		public const uint NumberOfSeriesRelatedInstances = 0x00201209;
		public const uint ModifyingDeviceIDRETIRED = 0x00203401;
		public const uint ModifiedImageIDRETIRED = 0x00203402;
		public const uint ModifiedImageDateRETIRED = 0x00203403;
		public const uint ModifyingDeviceManufacturerRETIRED = 0x00203404;
		public const uint ModifiedImageTimeRETIRED = 0x00203405;
		public const uint ModifiedImageDescriptionRETIRED = 0x00203406;
		public const uint ImageComments = 0x00204000;
		public const uint OriginalImageIdentificationRETIRED = 0x00205000;
		public const uint OriginalImageIdentificationNomenclatureRETIRED = 0x00205002;
		public const uint StackID = 0x00209056;
		public const uint InStackPositionNumber = 0x00209057;
		public const uint FrameAnatomySequence = 0x00209071;
		public const uint FrameLaterality = 0x00209072;
		public const uint FrameContentSequence = 0x00209111;
		public const uint PlanePositionSequence = 0x00209113;
		public const uint PlaneOrientationSequence = 0x00209116;
		public const uint TemporalPositionIndex = 0x00209128;
		public const uint CardiacTriggerDelayTime = 0x00209153;
		public const uint FrameAcquisitionNumber = 0x00209156;
		public const uint DimensionIndexValues = 0x00209157;
		public const uint FrameComments = 0x00209158;
		public const uint ConcatenationUID = 0x00209161;
		public const uint InconcatenationNumber = 0x00209162;
		public const uint InconcatenationTotalNumber = 0x00209163;
		public const uint DimensionOrganizationUID = 0x00209164;
		public const uint DimensionIndexPointer = 0x00209165;
		public const uint FunctionalGroupPointer = 0x00209167;
		public const uint DimensionIndexPrivateCreator = 0x00209213;
		public const uint DimensionOrganizationSequence = 0x00209221;
		public const uint DimensionIndexSequence = 0x00209222;
		public const uint ConcatenationFrameOffsetNumber = 0x00209228;
		public const uint FunctionalGroupPrivateCreator = 0x00209238;
		public const uint RRIntervalTimeMeasured = 0x00209251;
		public const uint RespiratoryTriggerSequence = 0x00209253;
		public const uint RespiratoryIntervalTime = 0x00209254;
		public const uint RespiratoryTriggerDelayTime = 0x00209255;
		public const uint RespiratoryTriggerDelayThreshold = 0x00209256;
		public const uint DimensionDescriptionLabel = 0x00209421;
		public const uint PatientOrientationInFrameSequence = 0x00209450;
		public const uint FrameLabel = 0x00209453;
		public const uint LightPathFilterPassThroughWavelength = 0x00220001;
		public const uint LightPathFilterPassBand = 0x00220002;
		public const uint ImagePathFilterPassThroughWavelength = 0x00220003;
		public const uint ImagePathFilterPassBand = 0x00220004;
		public const uint PatientEyeMovementCommanded = 0x00220005;
		public const uint PatientEyeMovementCommandCodeSequence = 0x00220006;
		public const uint SphericalLensPower = 0x00220007;
		public const uint CylinderLensPower = 0x00220008;
		public const uint CylinderAxis = 0x00220009;
		public const uint EmmetropicMagnification = 0x0022000a;
		public const uint IntraOcularPressure = 0x0022000b;
		public const uint HorizontalFieldOfView = 0x0022000c;
		public const uint PupilDilated = 0x0022000d;
		public const uint DegreeOfDilation = 0x0022000e;
		public const uint StereoBaselineAngle = 0x00220010;
		public const uint StereoBaselineDisplacement = 0x00220011;
		public const uint StereoHorizontalPixelOffset = 0x00220012;
		public const uint StereoVerticalPixelOffset = 0x00220013;
		public const uint StereoRotation = 0x00220014;
		public const uint AcquisitionDeviceTypeCodeSequence = 0x00220015;
		public const uint IlluminationTypeCodeSequence = 0x00220016;
		public const uint LightPathFilterTypeStackCodeSequence = 0x00220017;
		public const uint ImagePathFilterTypeStackCodeSequence = 0x00220018;
		public const uint LensesCodeSequence = 0x00220019;
		public const uint ChannelDescriptionCodeSequence = 0x0022001a;
		public const uint RefractiveStateSequence = 0x0022001b;
		public const uint MydriaticAgentCodeSequence = 0x0022001c;
		public const uint RelativeImagePositionCodeSequence = 0x0022001d;
		public const uint StereoPairsSequence = 0x00220020;
		public const uint LeftImageSequence = 0x00220021;
		public const uint RightImageSequence = 0x00220022;
		public const uint SamplesPerPixel = 0x00280002;
		public const uint SamplesPerPixelUsed = 0x00280003;
		public const uint PhotometricInterpretation = 0x00280004;
		public const uint ImageDimensionsRETIRED = 0x00280005;
		public const uint PlanarConfiguration = 0x00280006;
		public const uint NumberOfFrames = 0x00280008;
		public const uint FrameIncrementPointer = 0x00280009;
		public const uint FrameDimensionPointer = 0x0028000a;
		public const uint Rows = 0x00280010;
		public const uint Columns = 0x00280011;
		public const uint Planes = 0x00280012;
		public const uint UltrasoundColorDataPresent = 0x00280014;
		public const uint PixelSpacing = 0x00280030;
		public const uint ZoomFactor = 0x00280031;
		public const uint ZoomCenter = 0x00280032;
		public const uint PixelAspectRatio = 0x00280034;
		public const uint ImageFormatRETIRED = 0x00280040;
		public const uint ManipulatedImageRETIRED = 0x00280050;
		public const uint CorrectedImage = 0x00280051;
		public const uint CompressionCodeRETIRED = 0x00280060;
		public const uint BitsAllocated = 0x00280100;
		public const uint BitsStored = 0x00280101;
		public const uint HighBit = 0x00280102;
		public const uint PixelRepresentation = 0x00280103;
		public const uint SmallestValidPixelValueRETIRED = 0x00280104;
		public const uint LargestValidPixelValueRETIRED = 0x00280105;
		public const uint SmallestImagePixelValue = 0x00280106;
		public const uint LargestImagePixelValue = 0x00280107;
		public const uint SmallestPixelValueInSeries = 0x00280108;
		public const uint LargestPixelValueInSeries = 0x00280109;
		public const uint SmallestImagePixelValueInPlane = 0x00280110;
		public const uint LargestImagePixelValueInPlane = 0x00280111;
		public const uint PixelPaddingValue = 0x00280120;
		public const uint ImageLocationRETIRED = 0x00280200;
		public const uint QualityControlImage = 0x00280300;
		public const uint BurnedInAnnotation = 0x00280301;
		public const uint PixelSpacingCalibrationType = 0x00280402;
		public const uint PixelSpacingCalibrationDescription = 0x00280404;
		public const uint PixelIntensityRelationship = 0x00281040;
		public const uint PixelIntensityRelationshipSign = 0x00281041;
		public const uint WindowCenter = 0x00281050;
		public const uint WindowWidth = 0x00281051;
		public const uint RescaleIntercept = 0x00281052;
		public const uint RescaleSlope = 0x00281053;
		public const uint RescaleType = 0x00281054;
		public const uint WindowCenterWidthExplanation = 0x00281055;
		public const uint VOILUTFunction = 0x00281056;
		public const uint GrayScaleRETIRED = 0x00281080;
		public const uint RecommendedViewingMode = 0x00281090;
		public const uint GrayLookupTableDescriptorRETIRED = 0x00281100;
		public const uint RedPaletteColorLookupTableDescriptor = 0x00281101;
		public const uint GreenPaletteColorLookupTableDescriptor = 0x00281102;
		public const uint BluePaletteColorLookupTableDescriptor = 0x00281103;
		public const uint PaletteColorLookupTableUID = 0x00281199;
		public const uint GrayLookupTableDataRETIRED = 0x00281200;
		public const uint RedPaletteColorLookupTableData = 0x00281201;
		public const uint GreenPaletteColorLookupTableData = 0x00281202;
		public const uint BluePaletteColorLookupTableData = 0x00281203;
		public const uint SegmentedRedPaletteColorLookupTableData = 0x00281221;
		public const uint SegmentedGreenPaletteColorLookupTableData = 0x00281222;
		public const uint SegmentedBluePaletteColorLookupTableData = 0x00281223;
		public const uint ImplantPresent = 0x00281300;
		public const uint PartialView = 0x00281350;
		public const uint PartialViewDescription = 0x00281351;
		public const uint PartialViewCodeSequence = 0x00281352;
		public const uint SpatialLocationsPreserved = 0x0028135a;
		public const uint ICCProfile = 0x00282000;
		public const uint LossyImageCompression = 0x00282110;
		public const uint LossyImageCompressionRatio = 0x00282112;
		public const uint LossyImageCompressionMethod = 0x00282114;
		public const uint ModalityLUTSequence = 0x00283000;
		public const uint LUTDescriptor = 0x00283002;
		public const uint LUTExplanation = 0x00283003;
		public const uint ModalityLUTType = 0x00283004;
		public const uint LUTData = 0x00283006;
		public const uint VOILUTSequence = 0x00283010;
		public const uint SoftcopyVOILUTSequence = 0x00283110;
		public const uint ImagePresentationCommentsRETIRED = 0x00284000;
		public const uint BiPlaneAcquisitionSequence = 0x00285000;
		public const uint RepresentativeFrameNumber = 0x00286010;
		public const uint FrameNumbersOfInterestFOI = 0x00286020;
		public const uint FramesOfInterestDescription = 0x00286022;
		public const uint FrameOfInterestType = 0x00286023;
		public const uint MaskPointersRETIRED = 0x00286030;
		public const uint RWavePointer = 0x00286040;
		public const uint MaskSubtractionSequence = 0x00286100;
		public const uint MaskOperation = 0x00286101;
		public const uint ApplicableFrameRange = 0x00286102;
		public const uint MaskFrameNumbers = 0x00286110;
		public const uint ContrastFrameAveraging = 0x00286112;
		public const uint MaskSubpixelShift = 0x00286114;
		public const uint TIDOffset = 0x00286120;
		public const uint MaskOperationExplanation = 0x00286190;
		public const uint PixelDataProviderURL = 0x00287fe0;
		public const uint DataPointRows = 0x00289001;
		public const uint DataPointColumns = 0x00289002;
		public const uint SignalDomainColumns = 0x00289003;
		public const uint LargestMonochromePixelValueRETIRED = 0x00289099;
		public const uint DataRepresentation = 0x00289108;
		public const uint PixelMeasuresSequence = 0x00289110;
		public const uint FrameVOILUTSequence = 0x00289132;
		public const uint PixelValueTransformationSequence = 0x00289145;
		public const uint SignalDomainRows = 0x00289235;
		public const uint DisplayFilterPercentage = 0x00289411;
		public const uint FramePixelShiftSequence = 0x00289415;
		public const uint SubtractionItemID = 0x00289416;
		public const uint PixelIntensityRelationshipLUTSequence = 0x00289422;
		public const uint FramePixelDataPropertiesSequence = 0x00289443;
		public const uint GeometricalProperties = 0x00289444;
		public const uint GeometricMaximumDistortion = 0x00289445;
		public const uint ImageProcessingApplied = 0x00289446;
		public const uint MaskSelectionMode = 0x00289454;
		public const uint LUTFunction = 0x00289474;
		public const uint StudyStatusIDRETIRED = 0x0032000a;
		public const uint StudyPriorityIDRETIRED = 0x0032000c;
		public const uint StudyIDIssuerRETIRED = 0x00320012;
		public const uint StudyVerifiedDateRETIRED = 0x00320032;
		public const uint StudyVerifiedTimeRETIRED = 0x00320033;
		public const uint StudyReadDateRETIRED = 0x00320034;
		public const uint StudyReadTimeRETIRED = 0x00320035;
		public const uint ScheduledStudyStartDateRETIRED = 0x00321000;
		public const uint ScheduledStudyStartTimeRETIRED = 0x00321001;
		public const uint ScheduledStudyStopDateRETIRED = 0x00321010;
		public const uint ScheduledStudyStopTimeRETIRED = 0x00321011;
		public const uint ScheduledStudyLocationRETIRED = 0x00321020;
		public const uint ScheduledStudyLocationAETitleRETIRED = 0x00321021;
		public const uint ReasonForStudyRETIRED = 0x00321030;
		public const uint RequestingPhysicianIdentificationSequence = 0x00321031;
		public const uint RequestingPhysician = 0x00321032;
		public const uint RequestingService = 0x00321033;
		public const uint StudyArrivalDateRETIRED = 0x00321040;
		public const uint StudyArrivalTimeRETIRED = 0x00321041;
		public const uint StudyCompletionDateRETIRED = 0x00321050;
		public const uint StudyCompletionTimeRETIRED = 0x00321051;
		public const uint StudyComponentStatusIDRETIRED = 0x00321055;
		public const uint RequestedProcedureDescription = 0x00321060;
		public const uint RequestedProcedureCodeSequence = 0x00321064;
		public const uint RequestedContrastAgent = 0x00321070;
		public const uint StudyComments = 0x00324000;
		public const uint ReferencedPatientAliasSequence = 0x00380004;
		public const uint VisitStatusID = 0x00380008;
		public const uint AdmissionID = 0x00380010;
		public const uint IssuerOfAdmissionID = 0x00380011;
		public const uint RouteOfAdmissions = 0x00380016;
		public const uint ScheduledAdmissionDateRETIRED = 0x0038001a;
		public const uint ScheduledAdmissionTimeRETIRED = 0x0038001b;
		public const uint ScheduledDischargeDateRETIRED = 0x0038001c;
		public const uint ScheduledDischargeTimeRETIRED = 0x0038001d;
		public const uint ScheduledPatientInstitutionResidenceRETIRED = 0x0038001e;
		public const uint AdmittingDate = 0x00380020;
		public const uint AdmittingTime = 0x00380021;
		public const uint DischargeDateRETIRED = 0x00380030;
		public const uint DischargeTimeRETIRED = 0x00380032;
		public const uint DischargeDiagnosisDescriptionRETIRED = 0x00380040;
		public const uint DischargeDiagnosisCodeSequenceRETIRED = 0x00380044;
		public const uint SpecialNeeds = 0x00380050;
		public const uint PertinentDocumentsSequence = 0x00380100;
		public const uint CurrentPatientLocation = 0x00380300;
		public const uint PatientsInstitutionResidence = 0x00380400;
		public const uint PatientState = 0x00380500;
		public const uint PatientClinicalTrialParticipationSequence = 0x00380502;
		public const uint VisitComments = 0x00384000;
		public const uint WaveformOriginality = 0x003a0004;
		public const uint NumberOfWaveformChannels = 0x003a0005;
		public const uint NumberOfWaveformSamples = 0x003a0010;
		public const uint SamplingFrequency = 0x003a001a;
		public const uint MultiplexGroupLabel = 0x003a0020;
		public const uint ChannelDefinitionSequence = 0x003a0200;
		public const uint WaveformChannelNumber = 0x003a0202;
		public const uint ChannelLabel = 0x003a0203;
		public const uint ChannelStatus = 0x003a0205;
		public const uint ChannelSourceSequence = 0x003a0208;
		public const uint ChannelSourceModifiersSequence = 0x003a0209;
		public const uint SourceWaveformSequence = 0x003a020a;
		public const uint ChannelDerivationDescription = 0x003a020c;
		public const uint ChannelSensitivity = 0x003a0210;
		public const uint ChannelSensitivityUnitsSequence = 0x003a0211;
		public const uint ChannelSensitivityCorrectionFactor = 0x003a0212;
		public const uint ChannelBaseline = 0x003a0213;
		public const uint ChannelTimeSkew = 0x003a0214;
		public const uint ChannelSampleSkew = 0x003a0215;
		public const uint ChannelOffset = 0x003a0218;
		public const uint WaveformBitsStored = 0x003a021a;
		public const uint FilterLowFrequency = 0x003a0220;
		public const uint FilterHighFrequency = 0x003a0221;
		public const uint NotchFilterFrequency = 0x003a0222;
		public const uint NotchFilterBandwidth = 0x003a0223;
		public const uint MultiplexedAudioChannelsDescriptionCodeSequence = 0x003a0300;
		public const uint ChannelIdentificationCode = 0x003a0301;
		public const uint ChannelMode = 0x003a0302;
		public const uint ScheduledStationAETitle = 0x00400001;
		public const uint ScheduledProcedureStepStartDate = 0x00400002;
		public const uint ScheduledProcedureStepStartTime = 0x00400003;
		public const uint ScheduledProcedureStepEndDate = 0x00400004;
		public const uint ScheduledProcedureStepEndTime = 0x00400005;
		public const uint ScheduledPerformingPhysiciansName = 0x00400006;
		public const uint ScheduledProcedureStepDescription = 0x00400007;
		public const uint ScheduledProtocolCodeSequence = 0x00400008;
		public const uint ScheduledProcedureStepID = 0x00400009;
		public const uint StageCodeSequence = 0x0040000a;
		public const uint ScheduledPerformingPhysicianIdentificationSequence = 0x0040000b;
		public const uint ScheduledStationName = 0x00400010;
		public const uint ScheduledProcedureStepLocation = 0x00400011;
		public const uint PreMedication = 0x00400012;
		public const uint ScheduledProcedureStepStatus = 0x00400020;
		public const uint ScheduledProcedureStepSequence = 0x00400100;
		public const uint ReferencedNonImageCompositeSOPInstanceSequence = 0x00400220;
		public const uint PerformedStationAETitle = 0x00400241;
		public const uint PerformedStationName = 0x00400242;
		public const uint PerformedLocation = 0x00400243;
		public const uint PerformedProcedureStepStartDate = 0x00400244;
		public const uint PerformedProcedureStepStartTime = 0x00400245;
		public const uint PerformedProcedureStepEndDate = 0x00400250;
		public const uint PerformedProcedureStepEndTime = 0x00400251;
		public const uint PerformedProcedureStepStatus = 0x00400252;
		public const uint PerformedProcedureStepID = 0x00400253;
		public const uint PerformedProcedureStepDescription = 0x00400254;
		public const uint PerformedProcedureTypeDescription = 0x00400255;
		public const uint PerformedProtocolCodeSequence = 0x00400260;
		public const uint ScheduledStepAttributesSequence = 0x00400270;
		public const uint RequestAttributesSequence = 0x00400275;
		public const uint CommentsOnThePerformedProcedureStep = 0x00400280;
		public const uint PerformedProcedureStepDiscontinuationReasonCodeSequence = 0x00400281;
		public const uint QuantitySequence = 0x00400293;
		public const uint Quantity = 0x00400294;
		public const uint MeasuringUnitsSequence = 0x00400295;
		public const uint BillingItemSequence = 0x00400296;
		public const uint TotalTimeOfFluoroscopy = 0x00400300;
		public const uint TotalNumberOfExposures = 0x00400301;
		public const uint EntranceDose = 0x00400302;
		public const uint ExposedArea = 0x00400303;
		public const uint DistanceSourceToEntrance = 0x00400306;
		public const uint DistanceSourceToSupportRETIRED = 0x00400307;
		public const uint ExposureDoseSequence = 0x0040030e;
		public const uint CommentsOnRadiationDose = 0x00400310;
		public const uint XRayOutput = 0x00400312;
		public const uint HalfValueLayer = 0x00400314;
		public const uint OrganDose = 0x00400316;
		public const uint OrganExposed = 0x00400318;
		public const uint BillingProcedureStepSequence = 0x00400320;
		public const uint FilmConsumptionSequence = 0x00400321;
		public const uint BillingSuppliesAndDevicesSequence = 0x00400324;
		public const uint ReferencedProcedureStepSequenceRETIRED = 0x00400330;
		public const uint PerformedSeriesSequence = 0x00400340;
		public const uint CommentsOnTheScheduledProcedureStep = 0x00400400;
		public const uint ProtocolContextSequence = 0x00400440;
		public const uint ContentItemModifierSequence = 0x00400441;
		public const uint SpecimenAccessionNumber = 0x0040050a;
		public const uint SpecimenSequence = 0x00400550;
		public const uint SpecimenIdentifier = 0x00400551;
		public const uint AcquisitionContextSequence = 0x00400555;
		public const uint AcquisitionContextDescription = 0x00400556;
		public const uint SpecimenTypeCodeSequence = 0x0040059a;
		public const uint SlideIdentifier = 0x004006fa;
		public const uint ImageCenterPointCoordinatesSequence = 0x0040071a;
		public const uint XOffsetInSlideCoordinateSystem = 0x0040072a;
		public const uint YOffsetInSlideCoordinateSystem = 0x0040073a;
		public const uint ZOffsetInSlideCoordinateSystem = 0x0040074a;
		public const uint PixelSpacingSequence = 0x004008d8;
		public const uint CoordinateSystemAxisCodeSequence = 0x004008da;
		public const uint MeasurementUnitsCodeSequence = 0x004008ea;
		public const uint RequestedProcedureID = 0x00401001;
		public const uint ReasonForTheRequestedProcedure = 0x00401002;
		public const uint RequestedProcedurePriority = 0x00401003;
		public const uint PatientTransportArrangements = 0x00401004;
		public const uint RequestedProcedureLocation = 0x00401005;
		public const uint PlacerOrderNumberProcedureRETIRED = 0x00401006;
		public const uint FillerOrderNumberProcedureRETIRED = 0x00401007;
		public const uint ConfidentialityCode = 0x00401008;
		public const uint ReportingPriority = 0x00401009;
		public const uint ReasonForRequestedProcedureCodeSequence = 0x0040100a;
		public const uint NamesOfIntendedRecipientsOfResults = 0x00401010;
		public const uint IntendedRecipientsOfResultsIdentificationSequence = 0x00401011;
		public const uint PersonIdentificationCodeSequence = 0x00401101;
		public const uint PersonsAddress = 0x00401102;
		public const uint PersonsTelephoneNumbers = 0x00401103;
		public const uint RequestedProcedureComments = 0x00401400;
		public const uint ReasonForTheImagingServiceRequestRETIRED = 0x00402001;
		public const uint IssueDateOfImagingServiceRequest = 0x00402004;
		public const uint IssueTimeOfImagingServiceRequest = 0x00402005;
		public const uint PlacerOrderNumberImagingServiceRequestRETIRED = 0x00402006;
		public const uint FillerOrderNumberImagingServiceRequestRETIRED = 0x00402007;
		public const uint OrderEnteredBy = 0x00402008;
		public const uint OrderEnterersLocation = 0x00402009;
		public const uint OrderCallbackPhoneNumber = 0x00402010;
		public const uint PlacerOrderNumberImagingServiceRequest = 0x00402016;
		public const uint FillerOrderNumberImagingServiceRequest = 0x00402017;
		public const uint ImagingServiceRequestComments = 0x00402400;
		public const uint ConfidentialityConstraintOnPatientDataDescription = 0x00403001;
		public const uint GeneralPurposeScheduledProcedureStepStatus = 0x00404001;
		public const uint GeneralPurposePerformedProcedureStepStatus = 0x00404002;
		public const uint GeneralPurposeScheduledProcedureStepPriority = 0x00404003;
		public const uint ScheduledProcessingApplicationsCodeSequence = 0x00404004;
		public const uint ScheduledProcedureStepStartDateAndTime = 0x00404005;
		public const uint MultipleCopiesFlag = 0x00404006;
		public const uint PerformedProcessingApplicationsCodeSequence = 0x00404007;
		public const uint HumanPerformerCodeSequence = 0x00404009;
		public const uint ScheduledProcedureStepModificationDateAndTime = 0x00404010;
		public const uint ExpectedCompletionDateAndTime = 0x00404011;
		public const uint ResultingGeneralPurposePerformedProcedureStepsSequence = 0x00404015;
		public const uint ReferencedGeneralPurposeScheduledProcedureStepSequence = 0x00404016;
		public const uint ScheduledWorkitemCodeSequence = 0x00404018;
		public const uint PerformedWorkitemCodeSequence = 0x00404019;
		public const uint InputAvailabilityFlag = 0x00404020;
		public const uint InputInformationSequence = 0x00404021;
		public const uint RelevantInformationSequence = 0x00404022;
		public const uint ReferencedGeneralPurposeScheduledProcedureStepTransactionUID = 0x00404023;
		public const uint ScheduledStationNameCodeSequence = 0x00404025;
		public const uint ScheduledStationClassCodeSequence = 0x00404026;
		public const uint ScheduledStationGeographicLocationCodeSequence = 0x00404027;
		public const uint PerformedStationNameCodeSequence = 0x00404028;
		public const uint PerformedStationClassCodeSequence = 0x00404029;
		public const uint PerformedStationGeographicLocationCodeSequence = 0x00404030;
		public const uint RequestedSubsequentWorkitemCodeSequence = 0x00404031;
		public const uint NonDICOMOutputCodeSequence = 0x00404032;
		public const uint OutputInformationSequence = 0x00404033;
		public const uint ScheduledHumanPerformersSequence = 0x00404034;
		public const uint ActualHumanPerformersSequence = 0x00404035;
		public const uint HumanPerformersOrganization = 0x00404036;
		public const uint HumanPerformersName = 0x00404037;
		public const uint EntranceDoseInMGy = 0x00408302;
		public const uint ReferencedImageRealWorldValueMappingSequence = 0x00409094;
		public const uint RealWorldValueMappingSequence = 0x00409096;
		public const uint PixelValueMappingCodeSequence = 0x00409098;
		public const uint LUTLabel = 0x00409210;
		public const uint RealWorldValueLastValueMapped = 0x00409211;
		public const uint RealWorldValueLUTData = 0x00409212;
		public const uint RealWorldValueFirstValueMapped = 0x00409216;
		public const uint RealWorldValueIntercept = 0x00409224;
		public const uint RealWorldValueSlope = 0x00409225;
		public const uint RelationshipType = 0x0040a010;
		public const uint VerifyingOrganization = 0x0040a027;
		public const uint VerificationDateTime = 0x0040a030;
		public const uint ObservationDateTime = 0x0040a032;
		public const uint ValueType = 0x0040a040;
		public const uint ConceptNameCodeSequence = 0x0040a043;
		public const uint ContinuityOfContent = 0x0040a050;
		public const uint VerifyingObserverSequence = 0x0040a073;
		public const uint VerifyingObserverName = 0x0040a075;
		public const uint AuthorObserverSequence = 0x0040a078;
		public const uint ParticipantSequence = 0x0040a07a;
		public const uint CustodialOrganizationSequence = 0x0040a07c;
		public const uint ParticipationType = 0x0040a080;
		public const uint ParticipationDatetime = 0x0040a082;
		public const uint ObserverType = 0x0040a084;
		public const uint VerifyingObserverIdentificationCodeSequence = 0x0040a088;
		public const uint EquivalentCDADocumentSequenceRETIRED = 0x0040a090;
		public const uint ReferencedWaveformChannels = 0x0040a0b0;
		public const uint DateTime = 0x0040a120;
		public const uint Date = 0x0040a121;
		public const uint Time = 0x0040a122;
		public const uint PersonName = 0x0040a123;
		public const uint UID = 0x0040a124;
		public const uint TemporalRangeType = 0x0040a130;
		public const uint ReferencedSamplePositions = 0x0040a132;
		public const uint ReferencedFrameNumbers = 0x0040a136;
		public const uint ReferencedTimeOffsets = 0x0040a138;
		public const uint ReferencedDatetime = 0x0040a13a;
		public const uint TextValue = 0x0040a160;
		public const uint ConceptCodeSequence = 0x0040a168;
		public const uint PurposeOfReferenceCodeSequence = 0x0040a170;
		public const uint AnnotationGroupNumber = 0x0040a180;
		public const uint ModifierCodeSequence = 0x0040a195;
		public const uint MeasuredValueSequence = 0x0040a300;
		public const uint NumericValueQualifierCodeSequence = 0x0040a301;
		public const uint NumericValue = 0x0040a30a;
		public const uint PredecessorDocumentsSequence = 0x0040a360;
		public const uint ReferencedRequestSequence = 0x0040a370;
		public const uint PerformedProcedureCodeSequence = 0x0040a372;
		public const uint CurrentRequestedProcedureEvidenceSequence = 0x0040a375;
		public const uint PertinentOtherEvidenceSequence = 0x0040a385;
		public const uint HL7StructuredDocumentReferenceSequence = 0x0040a390;
		public const uint CompletionFlag = 0x0040a491;
		public const uint CompletionFlagDescription = 0x0040a492;
		public const uint VerificationFlag = 0x0040a493;
		public const uint ContentTemplateSequence = 0x0040a504;
		public const uint IdenticalDocumentsSequence = 0x0040a525;
		public const uint ContentSequence = 0x0040a730;
		public const uint AnnotationSequence = 0x0040b020;
		public const uint TemplateIdentifier = 0x0040db00;
		public const uint TemplateVersionRETIRED = 0x0040db06;
		public const uint TemplateLocalVersionRETIRED = 0x0040db07;
		public const uint TemplateExtensionFlagRETIRED = 0x0040db0b;
		public const uint TemplateExtensionOrganizationUIDRETIRED = 0x0040db0c;
		public const uint TemplateExtensionCreatorUIDRETIRED = 0x0040db0d;
		public const uint ReferencedContentItemIdentifier = 0x0040db73;
		public const uint HL7InstanceIdentifier = 0x0040e001;
		public const uint HL7DocumentEffectiveTime = 0x0040e004;
		public const uint HL7DocumentTypeCodeSequence = 0x0040e006;
		public const uint RetrieveURI = 0x0040e010;
		public const uint DocumentTitle = 0x00420010;
		public const uint EncapsulatedDocument = 0x00420011;
		public const uint MIMETypeOfEncapsulatedDocument = 0x00420012;
		public const uint SourceInstanceSequence = 0x00420013;
		public const uint CalibrationImage = 0x00500004;
		public const uint DeviceSequence = 0x00500010;
		public const uint DeviceLength = 0x00500014;
		public const uint DeviceDiameter = 0x00500016;
		public const uint DeviceDiameterUnits = 0x00500017;
		public const uint DeviceVolume = 0x00500018;
		public const uint IntermarkerDistance = 0x00500019;
		public const uint DeviceDescription = 0x00500020;
		public const uint EnergyWindowVector = 0x00540010;
		public const uint NumberOfEnergyWindows = 0x00540011;
		public const uint EnergyWindowInformationSequence = 0x00540012;
		public const uint EnergyWindowRangeSequence = 0x00540013;
		public const uint EnergyWindowLowerLimit = 0x00540014;
		public const uint EnergyWindowUpperLimit = 0x00540015;
		public const uint RadiopharmaceuticalInformationSequence = 0x00540016;
		public const uint ResidualSyringeCounts = 0x00540017;
		public const uint EnergyWindowName = 0x00540018;
		public const uint DetectorVector = 0x00540020;
		public const uint NumberOfDetectors = 0x00540021;
		public const uint DetectorInformationSequence = 0x00540022;
		public const uint PhaseVector = 0x00540030;
		public const uint NumberOfPhases = 0x00540031;
		public const uint PhaseInformationSequence = 0x00540032;
		public const uint NumberOfFramesInPhase = 0x00540033;
		public const uint PhaseDelay = 0x00540036;
		public const uint PauseBetweenFrames = 0x00540038;
		public const uint PhaseDescription = 0x00540039;
		public const uint RotationVector = 0x00540050;
		public const uint NumberOfRotations = 0x00540051;
		public const uint RotationInformationSequence = 0x00540052;
		public const uint NumberOfFramesInRotation = 0x00540053;
		public const uint RRIntervalVector = 0x00540060;
		public const uint NumberOfRRIntervals = 0x00540061;
		public const uint GatedInformationSequence = 0x00540062;
		public const uint DataInformationSequence = 0x00540063;
		public const uint TimeSlotVector = 0x00540070;
		public const uint NumberOfTimeSlots = 0x00540071;
		public const uint TimeSlotInformationSequence = 0x00540072;
		public const uint TimeSlotTime = 0x00540073;
		public const uint SliceVector = 0x00540080;
		public const uint NumberOfSlices = 0x00540081;
		public const uint AngularViewVector = 0x00540090;
		public const uint TimeSliceVector = 0x00540100;
		public const uint NumberOfTimeSlices = 0x00540101;
		public const uint StartAngle = 0x00540200;
		public const uint TypeOfDetectorMotion = 0x00540202;
		public const uint TriggerVector = 0x00540210;
		public const uint NumberOfTriggersInPhase = 0x00540211;
		public const uint ViewCodeSequence = 0x00540220;
		public const uint ViewModifierCodeSequence = 0x00540222;
		public const uint RadionuclideCodeSequence = 0x00540300;
		public const uint AdministrationRouteCodeSequence = 0x00540302;
		public const uint RadiopharmaceuticalCodeSequence = 0x00540304;
		public const uint CalibrationDataSequence = 0x00540306;
		public const uint EnergyWindowNumber = 0x00540308;
		public const uint ImageID = 0x00540400;
		public const uint PatientOrientationCodeSequence = 0x00540410;
		public const uint PatientOrientationModifierCodeSequence = 0x00540412;
		public const uint PatientGantryRelationshipCodeSequence = 0x00540414;
		public const uint SliceProgressionDirection = 0x00540500;
		public const uint SeriesType = 0x00541000;
		public const uint Units = 0x00541001;
		public const uint CountsSource = 0x00541002;
		public const uint ReprojectionMethod = 0x00541004;
		public const uint RandomsCorrectionMethod = 0x00541100;
		public const uint AttenuationCorrectionMethod = 0x00541101;
		public const uint DecayCorrection = 0x00541102;
		public const uint ReconstructionMethod = 0x00541103;
		public const uint DetectorLinesOfResponseUsed = 0x00541104;
		public const uint ScatterCorrectionMethod = 0x00541105;
		public const uint AxialAcceptance = 0x00541200;
		public const uint AxialMash = 0x00541201;
		public const uint TransverseMash = 0x00541202;
		public const uint DetectorElementSize = 0x00541203;
		public const uint CoincidenceWindowWidth = 0x00541210;
		public const uint SecondaryCountsType = 0x00541220;
		public const uint FrameReferenceTime = 0x00541300;
		public const uint PrimaryPromptsCountsAccumulated = 0x00541310;
		public const uint SecondaryCountsAccumulated = 0x00541311;
		public const uint SliceSensitivityFactor = 0x00541320;
		public const uint DecayFactor = 0x00541321;
		public const uint DoseCalibrationFactor = 0x00541322;
		public const uint ScatterFractionFactor = 0x00541323;
		public const uint DeadTimeFactor = 0x00541324;
		public const uint ImageIndex = 0x00541330;
		public const uint CountsIncluded = 0x00541400;
		public const uint DeadTimeCorrectionFlag = 0x00541401;
		public const uint HistogramSequence = 0x00603000;
		public const uint HistogramNumberOfBins = 0x00603002;
		public const uint HistogramFirstBinValue = 0x00603004;
		public const uint HistogramLastBinValue = 0x00603006;
		public const uint HistogramBinWidth = 0x00603008;
		public const uint HistogramExplanation = 0x00603010;
		public const uint HistogramData = 0x00603020;
		public const uint SegmentationType = 0x00620001;
		public const uint SegmentSequence = 0x00620002;
		public const uint SegmentedPropertyCategoryCodeSequence = 0x00620003;
		public const uint SegmentNumber = 0x00620004;
		public const uint SegmentLabel = 0x00620005;
		public const uint SegmentDescription = 0x00620006;
		public const uint SegmentAlgorithmType = 0x00620008;
		public const uint SegmentAlgorithmName = 0x00620009;
		public const uint SegmentIdentificationSequence = 0x0062000a;
		public const uint ReferencedSegmentNumber = 0x0062000b;
		public const uint RecommendedDisplayGrayscaleValue = 0x0062000c;
		public const uint RecommendedDisplayCIELabValue = 0x0062000d;
		public const uint MaximumFractionalValue = 0x0062000e;
		public const uint SegmentedPropertyTypeCodeSequence = 0x0062000f;
		public const uint SegmentationFractionalType = 0x00620010;
		public const uint DeformableRegistrationSequence = 0x00640002;
		public const uint SourceFrameOfReferenceUID = 0x00640003;
		public const uint DeformableRegistrationGridSequence = 0x00640005;
		public const uint GridDimensions = 0x00640007;
		public const uint GridResolution = 0x00640008;
		public const uint VectorGridData = 0x00640009;
		public const uint PreDeformationMatrixRegistrationSequence = 0x0064000f;
		public const uint PostDeformationMatrixRegistrationSequence = 0x00640010;
		public const uint GraphicAnnotationSequence = 0x00700001;
		public const uint GraphicLayer = 0x00700002;
		public const uint BoundingBoxAnnotationUnits = 0x00700003;
		public const uint AnchorPointAnnotationUnits = 0x00700004;
		public const uint GraphicAnnotationUnits = 0x00700005;
		public const uint UnformattedTextValue = 0x00700006;
		public const uint TextObjectSequence = 0x00700008;
		public const uint GraphicObjectSequence = 0x00700009;
		public const uint BoundingBoxTopLeftHandCorner = 0x00700010;
		public const uint BoundingBoxBottomRightHandCorner = 0x00700011;
		public const uint BoundingBoxTextHorizontalJustification = 0x00700012;
		public const uint AnchorPoint = 0x00700014;
		public const uint AnchorPointVisibility = 0x00700015;
		public const uint GraphicDimensions = 0x00700020;
		public const uint NumberOfGraphicPoints = 0x00700021;
		public const uint GraphicData = 0x00700022;
		public const uint GraphicType = 0x00700023;
		public const uint GraphicFilled = 0x00700024;
		public const uint ImageHorizontalFlip = 0x00700041;
		public const uint ImageRotation = 0x00700042;
		public const uint DisplayedAreaTopLeftHandCorner = 0x00700052;
		public const uint DisplayedAreaBottomRightHandCorner = 0x00700053;
		public const uint DisplayedAreaSelectionSequence = 0x0070005a;
		public const uint GraphicLayerSequence = 0x00700060;
		public const uint GraphicLayerOrder = 0x00700062;
		public const uint GraphicLayerRecommendedDisplayGrayscaleValue = 0x00700066;
		public const uint GraphicLayerRecommendedDisplayRGBValueRETIRED = 0x00700067;
		public const uint GraphicLayerDescription = 0x00700068;
		public const uint ContentLabel = 0x00700080;
		public const uint ContentDescription = 0x00700081;
		public const uint PresentationCreationDate = 0x00700082;
		public const uint PresentationCreationTime = 0x00700083;
		public const uint ContentCreatorsName = 0x00700084;
		public const uint ContentCreatorsIdentificationCodeSequence = 0x00700086;
		public const uint PresentationSizeMode = 0x00700100;
		public const uint PresentationPixelSpacing = 0x00700101;
		public const uint PresentationPixelAspectRatio = 0x00700102;
		public const uint PresentationPixelMagnificationRatio = 0x00700103;
		public const uint ShapeType = 0x00700306;
		public const uint RegistrationSequence = 0x00700308;
		public const uint MatrixRegistrationSequence = 0x00700309;
		public const uint MatrixSequence = 0x0070030a;
		public const uint FrameOfReferenceTransformationMatrixType = 0x0070030c;
		public const uint RegistrationTypeCodeSequence = 0x0070030d;
		public const uint FiducialDescription = 0x0070030f;
		public const uint FiducialIdentifier = 0x00700310;
		public const uint FiducialIdentifierCodeSequence = 0x00700311;
		public const uint ContourUncertaintyRadius = 0x00700312;
		public const uint UsedFiducialsSequence = 0x00700314;
		public const uint GraphicCoordinatesDataSequence = 0x00700318;
		public const uint FiducialUID = 0x0070031a;
		public const uint FiducialSetSequence = 0x0070031c;
		public const uint FiducialSequence = 0x0070031e;
		public const uint GraphicLayerRecommendedDisplayCIELabValue = 0x00700401;
		public const uint BlendingSequence = 0x00700402;
		public const uint RelativeOpacity = 0x00700403;
		public const uint ReferencedSpatialRegistrationSequence = 0x00700404;
		public const uint BlendingPosition = 0x00700405;
		public const uint HangingProtocolName = 0x00720002;
		public const uint HangingProtocolDescription = 0x00720004;
		public const uint HangingProtocolLevel = 0x00720006;
		public const uint HangingProtocolCreator = 0x00720008;
		public const uint HangingProtocolCreationDatetime = 0x0072000a;
		public const uint HangingProtocolDefinitionSequence = 0x0072000c;
		public const uint HangingProtocolUserIdentificationCodeSequence = 0x0072000e;
		public const uint HangingProtocolUserGroupName = 0x00720010;
		public const uint SourceHangingProtocolSequence = 0x00720012;
		public const uint NumberOfPriorsReferenced = 0x00720014;
		public const uint ImageSetsSequence = 0x00720020;
		public const uint ImageSetSelectorSequence = 0x00720022;
		public const uint ImageSetSelectorUsageFlag = 0x00720024;
		public const uint SelectorAttribute = 0x00720026;
		public const uint SelectorValueNumber = 0x00720028;
		public const uint TimeBasedImageSetsSequence = 0x00720030;
		public const uint ImageSetNumber = 0x00720032;
		public const uint ImageSetSelectorCategory = 0x00720034;
		public const uint RelativeTime = 0x00720038;
		public const uint RelativeTimeUnits = 0x0072003a;
		public const uint AbstractPriorValue = 0x0072003c;
		public const uint AbstractPriorCodeSequence = 0x0072003e;
		public const uint ImageSetLabel = 0x00720040;
		public const uint SelectorAttributeVR = 0x00720050;
		public const uint SelectorSequencePointer = 0x00720052;
		public const uint SelectorSequencePointerPrivateCreator = 0x00720054;
		public const uint SelectorAttributePrivateCreator = 0x00720056;
		public const uint SelectorATValue = 0x00720060;
		public const uint SelectorCSValue = 0x00720062;
		public const uint SelectorISValue = 0x00720064;
		public const uint SelectorLOValue = 0x00720066;
		public const uint SelectorLTValue = 0x00720068;
		public const uint SelectorPNValue = 0x0072006a;
		public const uint SelectorSHValue = 0x0072006c;
		public const uint SelectorSTValue = 0x0072006e;
		public const uint SelectorUTValue = 0x00720070;
		public const uint SelectorDSValue = 0x00720072;
		public const uint SelectorFDValue = 0x00720074;
		public const uint SelectorFLValue = 0x00720076;
		public const uint SelectorULValue = 0x00720078;
		public const uint SelectorUSValue = 0x0072007a;
		public const uint SelectorSLValue = 0x0072007c;
		public const uint SelectorSSValue = 0x0072007e;
		public const uint SelectorCodeSequenceValue = 0x00720080;
		public const uint NumberOfScreens = 0x00720100;
		public const uint NominalScreenDefinitionSequence = 0x00720102;
		public const uint NumberOfVerticalPixels = 0x00720104;
		public const uint NumberOfHorizontalPixels = 0x00720106;
		public const uint DisplayEnvironmentSpatialPosition = 0x00720108;
		public const uint ScreenMinimumGrayscaleBitDepth = 0x0072010a;
		public const uint ScreenMinimumColorBitDepth = 0x0072010c;
		public const uint ApplicationMaximumRepaintTime = 0x0072010e;
		public const uint DisplaySetsSequence = 0x00720200;
		public const uint DisplaySetNumber = 0x00720202;
		public const uint DisplaySetLabel = 0x00720203;
		public const uint DisplaySetPresentationGroup = 0x00720204;
		public const uint DisplaySetPresentationGroupDescription = 0x00720206;
		public const uint PartialDataDisplayHandling = 0x00720208;
		public const uint SynchronizedScrollingSequence = 0x00720210;
		public const uint DisplaySetScrollingGroup = 0x00720212;
		public const uint NavigationIndicatorSequence = 0x00720214;
		public const uint NavigationDisplaySet = 0x00720216;
		public const uint ReferenceDisplaySets = 0x00720218;
		public const uint ImageBoxesSequence = 0x00720300;
		public const uint ImageBoxNumber = 0x00720302;
		public const uint ImageBoxLayoutType = 0x00720304;
		public const uint ImageBoxTileHorizontalDimension = 0x00720306;
		public const uint ImageBoxTileVerticalDimension = 0x00720308;
		public const uint ImageBoxScrollDirection = 0x00720310;
		public const uint ImageBoxSmallScrollType = 0x00720312;
		public const uint ImageBoxSmallScrollAmount = 0x00720314;
		public const uint ImageBoxLargeScrollType = 0x00720316;
		public const uint ImageBoxLargeScrollAmount = 0x00720318;
		public const uint ImageBoxOverlapPriority = 0x00720320;
		public const uint CineRelativeToRealTime = 0x00720330;
		public const uint FilterOperationsSequence = 0x00720400;
		public const uint FilterbyCategory = 0x00720402;
		public const uint FilterbyAttributePresence = 0x00720404;
		public const uint FilterbyOperator = 0x00720406;
		public const uint BlendingOperationType = 0x00720500;
		public const uint ReformattingOperationType = 0x00720510;
		public const uint ReformattingThickness = 0x00720512;
		public const uint ReformattingInterval = 0x00720514;
		public const uint ReformattingOperationInitialViewDirection = 0x00720516;
		public const uint _3DRenderingType = 0x00720520;
		public const uint SortingOperationsSequence = 0x00720600;
		public const uint SortbyCategory = 0x00720602;
		public const uint SortingDirection = 0x00720604;
		public const uint DisplaySetPatientOrientation = 0x00720700;
		public const uint VOIType = 0x00720702;
		public const uint PseudocolorType = 0x00720704;
		public const uint ShowGrayscaleInverted = 0x00720706;
		public const uint ShowImageTrueSizeFlag = 0x00720710;
		public const uint ShowGraphicAnnotationFlag = 0x00720712;
		public const uint ShowPatientDemographicsFlag = 0x00720714;
		public const uint ShowAcquisitionTechniquesFlag = 0x00720716;
		public const uint DisplaySetHorizontalJustification = 0x00720717;
		public const uint DisplaySetVerticalJustification = 0x00720718;
		public const uint StorageMediaFilesetID = 0x00880130;
		public const uint StorageMediaFilesetUID = 0x00880140;
		public const uint IconImageSequence = 0x00880200;
		public const uint TopicTitle = 0x00880904;
		public const uint TopicSubject = 0x00880906;
		public const uint TopicAuthor = 0x00880910;
		public const uint TopicKeywords = 0x00880912;
		public const uint SOPInstanceStatus = 0x01000410;
		public const uint SOPAuthorizationDateAndTime = 0x01000420;
		public const uint SOPAuthorizationComment = 0x01000424;
		public const uint AuthorizationEquipmentCertificationNumber = 0x01000426;
		public const uint MACIDNumber = 0x04000005;
		public const uint MACCalculationTransferSyntaxUID = 0x04000010;
		public const uint MACAlgorithm = 0x04000015;
		public const uint DataElementsSigned = 0x04000020;
		public const uint DigitalSignatureUID = 0x04000100;
		public const uint DigitalSignatureDateTime = 0x04000105;
		public const uint CertificateType = 0x04000110;
		public const uint CertificateOfSigner = 0x04000115;
		public const uint Signature = 0x04000120;
		public const uint CertifiedTimestampType = 0x04000305;
		public const uint CertifiedTimestamp = 0x04000310;
		public const uint DigitalSignaturePurposeCodeSequence = 0x04000401;
		public const uint ReferencedDigitalSignatureSequence = 0x04000402;
		public const uint ReferencedSOPInstanceMACSequence = 0x04000403;
		public const uint MAC = 0x04000404;
		public const uint EncryptedAttributesSequence = 0x04000500;
		public const uint EncryptedContentTransferSyntaxUID = 0x04000510;
		public const uint EncryptedContent = 0x04000520;
		public const uint ModifiedAttributesSequence = 0x04000550;
		public const uint OriginalAttributesSequence = 0x04000561;
		public const uint AttributeModificationDatetime = 0x04000562;
		public const uint ModifyingSystem = 0x04000563;
		public const uint SourceOfPreviousValues = 0x04000564;
		public const uint ReasonForTheAttributeModification = 0x04000565;
		public const uint NumberOfCopies = 0x20000010;
		public const uint PrinterConfigurationSequence = 0x2000001e;
		public const uint PrintPriority = 0x20000020;
		public const uint MediumType = 0x20000030;
		public const uint FilmDestination = 0x20000040;
		public const uint FilmSessionLabel = 0x20000050;
		public const uint MemoryAllocation = 0x20000060;
		public const uint MaximumMemoryAllocation = 0x20000061;
		public const uint ColorImagePrintingFlagRETIRED = 0x20000062;
		public const uint CollationFlagRETIRED = 0x20000063;
		public const uint AnnotationFlagRETIRED = 0x20000065;
		public const uint ImageOverlayFlagRETIRED = 0x20000067;
		public const uint PresentationLUTFlagRETIRED = 0x20000069;
		public const uint ImageBoxPresentationLUTFlagRETIRED = 0x2000006a;
		public const uint MemoryBitDepth = 0x200000a0;
		public const uint PrintingBitDepth = 0x200000a1;
		public const uint MediaInstalledSequence = 0x200000a2;
		public const uint OtherMediaAvailableSequence = 0x200000a4;
		public const uint SupportedImageDisplayFormatsSequence = 0x200000a8;
		public const uint ReferencedFilmBoxSequence = 0x20000500;
		public const uint ReferencedStoredPrintSequence = 0x20000510;
		public const uint ImageDisplayFormat = 0x20100010;
		public const uint AnnotationDisplayFormatID = 0x20100030;
		public const uint FilmOrientation = 0x20100040;
		public const uint FilmSizeID = 0x20100050;
		public const uint PrinterResolutionID = 0x20100052;
		public const uint DefaultPrinterResolutionID = 0x20100054;
		public const uint MagnificationType = 0x20100060;
		public const uint SmoothingType = 0x20100080;
		public const uint DefaultMagnificationType = 0x201000a6;
		public const uint OtherMagnificationTypesAvailable = 0x201000a7;
		public const uint DefaultSmoothingType = 0x201000a8;
		public const uint OtherSmoothingTypesAvailable = 0x201000a9;
		public const uint BorderDensity = 0x20100100;
		public const uint EmptyImageDensity = 0x20100110;
		public const uint MinDensity = 0x20100120;
		public const uint MaxDensity = 0x20100130;
		public const uint Trim = 0x20100140;
		public const uint ConfigurationInformation = 0x20100150;
		public const uint ConfigurationInformationDescription = 0x20100152;
		public const uint MaximumCollatedFilms = 0x20100154;
		public const uint Illumination = 0x2010015e;
		public const uint ReflectedAmbientLight = 0x20100160;
		public const uint PrinterPixelSpacing = 0x20100376;
		public const uint ReferencedFilmSessionSequence = 0x20100500;
		public const uint ReferencedImageBoxSequence = 0x20100510;
		public const uint ReferencedBasicAnnotationBoxSequence = 0x20100520;
		public const uint ImagePosition = 0x20200010;
		public const uint Polarity = 0x20200020;
		public const uint RequestedImageSize = 0x20200030;
		public const uint RequestedDecimateCropBehavior = 0x20200040;
		public const uint RequestedResolutionID = 0x20200050;
		public const uint RequestedImageSizeFlag = 0x202000a0;
		public const uint DecimateCropResult = 0x202000a2;
		public const uint BasicGrayscaleImageSequence = 0x20200110;
		public const uint BasicColorImageSequence = 0x20200111;
		public const uint ReferencedImageOverlayBoxSequenceRETIRED = 0x20200130;
		public const uint ReferencedVOILUTBoxSequenceRETIRED = 0x20200140;
		public const uint AnnotationPosition = 0x20300010;
		public const uint TextString = 0x20300020;
		public const uint ReferencedOverlayPlaneSequenceRETIRED = 0x20400010;
		public const uint ReferencedOverlayPlaneGroupsRETIRED = 0x20400011;
		public const uint OverlayPixelDataSequenceRETIRED = 0x20400020;
		public const uint OverlayMagnificationTypeRETIRED = 0x20400060;
		public const uint OverlaySmoothingTypeRETIRED = 0x20400070;
		public const uint OverlayOrImageMagnificationRETIRED = 0x20400072;
		public const uint MagnifyToNumberOfColumnsRETIRED = 0x20400074;
		public const uint OverlayForegroundDensityRETIRED = 0x20400080;
		public const uint OverlayBackgroundDensityRETIRED = 0x20400082;
		public const uint OverlayModeRETIRED = 0x20400090;
		public const uint ThresholdDensityRETIRED = 0x20400100;
		public const uint ReferencedImageBoxSequenceRETIRED = 0x20400500;
		public const uint PresentationLUTSequence = 0x20500010;
		public const uint PresentationLUTShape = 0x20500020;
		public const uint ReferencedPresentationLUTSequence = 0x20500500;
		public const uint PrintJobID = 0x21000010;
		public const uint ExecutionStatus = 0x21000020;
		public const uint ExecutionStatusInfo = 0x21000030;
		public const uint CreationDate = 0x21000040;
		public const uint CreationTime = 0x21000050;
		public const uint Originator = 0x21000070;
		public const uint DestinationAE = 0x21000140;
		public const uint OwnerID = 0x21000160;
		public const uint NumberOfFilms = 0x21000170;
		public const uint ReferencedPrintJobSequencePullStoredPrintRETIRED = 0x21000500;
		public const uint PrinterStatus = 0x21100010;
		public const uint PrinterStatusInfo = 0x21100020;
		public const uint PrinterName = 0x21100030;
		public const uint PrintQueueIDRETIRED = 0x21100099;
		public const uint QueueStatusRETIRED = 0x21200010;
		public const uint PrintJobDescriptionSequenceRETIRED = 0x21200050;
		public const uint ReferencedPrintJobSequenceRETIRED = 0x21200070;
		public const uint PrintManagementCapabilitiesSequenceRETIRED = 0x21300010;
		public const uint PrinterCharacteristicsSequenceRETIRED = 0x21300015;
		public const uint FilmBoxContentSequenceRETIRED = 0x21300030;
		public const uint ImageBoxContentSequenceRETIRED = 0x21300040;
		public const uint AnnotationContentSequenceRETIRED = 0x21300050;
		public const uint ImageOverlayBoxContentSequenceRETIRED = 0x21300060;
		public const uint PresentationLUTContentSequenceRETIRED = 0x21300080;
		public const uint ProposedStudySequenceRETIRED = 0x213000a0;
		public const uint OriginalImageSequenceRETIRED = 0x213000c0;
		public const uint LabelUsingInformationExtractedFromInstances = 0x22000001;
		public const uint LabelText = 0x22000002;
		public const uint LabelStyleSelection = 0x22000003;
		public const uint MediaDisposition = 0x22000004;
		public const uint BarcodeValue = 0x22000005;
		public const uint BarcodeSymbology = 0x22000006;
		public const uint AllowMediaSplitting = 0x22000007;
		public const uint IncludeNonDICOMObjects = 0x22000008;
		public const uint IncludeDisplayApplication = 0x22000009;
		public const uint PreserveCompositeInstancesAfterMediaCreation = 0x2200000a;
		public const uint TotalNumberOfPiecesOfMediaCreated = 0x2200000b;
		public const uint RequestedMediaApplicationProfile = 0x2200000c;
		public const uint ReferencedStorageMediaSequence = 0x2200000d;
		public const uint FailureAttributes = 0x2200000e;
		public const uint AllowLossyCompression = 0x2200000f;
		public const uint RequestPriority = 0x22000020;
		public const uint RTImageLabel = 0x30020002;
		public const uint RTImageName = 0x30020003;
		public const uint RTImageDescription = 0x30020004;
		public const uint ReportedValuesOrigin = 0x3002000a;
		public const uint RTImagePlane = 0x3002000c;
		public const uint XRayImageReceptorTranslation = 0x3002000d;
		public const uint XRayImageReceptorAngle = 0x3002000e;
		public const uint RTImageOrientation = 0x30020010;
		public const uint ImagePlanePixelSpacing = 0x30020011;
		public const uint RTImagePosition = 0x30020012;
		public const uint RadiationMachineName = 0x30020020;
		public const uint RadiationMachineSAD = 0x30020022;
		public const uint RadiationMachineSSD = 0x30020024;
		public const uint RTImageSID = 0x30020026;
		public const uint SourceToReferenceObjectDistance = 0x30020028;
		public const uint FractionNumber = 0x30020029;
		public const uint ExposureSequence = 0x30020030;
		public const uint MetersetExposure = 0x30020032;
		public const uint DiaphragmPosition = 0x30020034;
		public const uint FluenceMapSequence = 0x30020040;
		public const uint FluenceDataSource = 0x30020041;
		public const uint FluenceDataScale = 0x30020042;
		public const uint DVHType = 0x30040001;
		public const uint DoseUnits = 0x30040002;
		public const uint DoseType = 0x30040004;
		public const uint DoseComment = 0x30040006;
		public const uint NormalizationPoint = 0x30040008;
		public const uint DoseSummationType = 0x3004000a;
		public const uint GridFrameOffsetVector = 0x3004000c;
		public const uint DoseGridScaling = 0x3004000e;
		public const uint RTDoseROISequence = 0x30040010;
		public const uint DoseValue = 0x30040012;
		public const uint TissueHeterogeneityCorrection = 0x30040014;
		public const uint DVHNormalizationPoint = 0x30040040;
		public const uint DVHNormalizationDoseValue = 0x30040042;
		public const uint DVHSequence = 0x30040050;
		public const uint DVHDoseScaling = 0x30040052;
		public const uint DVHVolumeUnits = 0x30040054;
		public const uint DVHNumberOfBins = 0x30040056;
		public const uint DVHData = 0x30040058;
		public const uint DVHReferencedROISequence = 0x30040060;
		public const uint DVHROIContributionType = 0x30040062;
		public const uint DVHMinimumDose = 0x30040070;
		public const uint DVHMaximumDose = 0x30040072;
		public const uint DVHMeanDose = 0x30040074;
		public const uint StructureSetLabel = 0x30060002;
		public const uint StructureSetName = 0x30060004;
		public const uint StructureSetDescription = 0x30060006;
		public const uint StructureSetDate = 0x30060008;
		public const uint StructureSetTime = 0x30060009;
		public const uint ReferencedFrameOfReferenceSequence = 0x30060010;
		public const uint RTReferencedStudySequence = 0x30060012;
		public const uint RTReferencedSeriesSequence = 0x30060014;
		public const uint ContourImageSequence = 0x30060016;
		public const uint StructureSetROISequence = 0x30060020;
		public const uint ROINumber = 0x30060022;
		public const uint ReferencedFrameOfReferenceUID = 0x30060024;
		public const uint ROIName = 0x30060026;
		public const uint ROIDescription = 0x30060028;
		public const uint ROIDisplayColor = 0x3006002a;
		public const uint ROIVolume = 0x3006002c;
		public const uint RTRelatedROISequence = 0x30060030;
		public const uint RTROIRelationship = 0x30060033;
		public const uint ROIGenerationAlgorithm = 0x30060036;
		public const uint ROIGenerationDescription = 0x30060038;
		public const uint ROIContourSequence = 0x30060039;
		public const uint ContourSequence = 0x30060040;
		public const uint ContourGeometricType = 0x30060042;
		public const uint ContourSlabThickness = 0x30060044;
		public const uint ContourOffsetVector = 0x30060045;
		public const uint NumberOfContourPoints = 0x30060046;
		public const uint ContourNumber = 0x30060048;
		public const uint AttachedContours = 0x30060049;
		public const uint ContourData = 0x30060050;
		public const uint RTROIObservationsSequence = 0x30060080;
		public const uint ObservationNumber = 0x30060082;
		public const uint ReferencedROINumber = 0x30060084;
		public const uint ROIObservationLabel = 0x30060085;
		public const uint RTROIIdentificationCodeSequence = 0x30060086;
		public const uint ROIObservationDescription = 0x30060088;
		public const uint RelatedRTROIObservationsSequence = 0x300600a0;
		public const uint RTROIInterpretedType = 0x300600a4;
		public const uint ROIInterpreter = 0x300600a6;
		public const uint ROIPhysicalPropertiesSequence = 0x300600b0;
		public const uint ROIPhysicalProperty = 0x300600b2;
		public const uint ROIPhysicalPropertyValue = 0x300600b4;
		public const uint FrameOfReferenceRelationshipSequence = 0x300600c0;
		public const uint RelatedFrameOfReferenceUID = 0x300600c2;
		public const uint FrameOfReferenceTransformationType = 0x300600c4;
		public const uint FrameOfReferenceTransformationMatrix = 0x300600c6;
		public const uint FrameOfReferenceTransformationComment = 0x300600c8;
		public const uint MeasuredDoseReferenceSequence = 0x30080010;
		public const uint MeasuredDoseDescription = 0x30080012;
		public const uint MeasuredDoseType = 0x30080014;
		public const uint MeasuredDoseValue = 0x30080016;
		public const uint TreatmentSessionBeamSequence = 0x30080020;
		public const uint TreatmentSessionIonBeamSequence = 0x30080021;
		public const uint CurrentFractionNumber = 0x30080022;
		public const uint TreatmentControlPointDate = 0x30080024;
		public const uint TreatmentControlPointTime = 0x30080025;
		public const uint TreatmentTerminationStatus = 0x3008002a;
		public const uint TreatmentTerminationCode = 0x3008002b;
		public const uint TreatmentVerificationStatus = 0x3008002c;
		public const uint ReferencedTreatmentRecordSequence = 0x30080030;
		public const uint SpecifiedPrimaryMeterset = 0x30080032;
		public const uint SpecifiedSecondaryMeterset = 0x30080033;
		public const uint DeliveredPrimaryMeterset = 0x30080036;
		public const uint DeliveredSecondaryMeterset = 0x30080037;
		public const uint SpecifiedTreatmentTime = 0x3008003a;
		public const uint DeliveredTreatmentTime = 0x3008003b;
		public const uint ControlPointDeliverySequence = 0x30080040;
		public const uint IonControlPointDeliverySequence = 0x30080041;
		public const uint SpecifiedMeterset = 0x30080042;
		public const uint DeliveredMeterset = 0x30080044;
		public const uint MetersetRateSet = 0x30080045;
		public const uint MetersetRateDelivered = 0x30080046;
		public const uint ScanSpotMetersetsDelivered = 0x30080047;
		public const uint DoseRateDelivered = 0x30080048;
		public const uint TreatmentSummaryCalculatedDoseReferenceSequence = 0x30080050;
		public const uint CumulativeDoseToDoseReference = 0x30080052;
		public const uint FirstTreatmentDate = 0x30080054;
		public const uint MostRecentTreatmentDate = 0x30080056;
		public const uint NumberOfFractionsDelivered = 0x3008005a;
		public const uint OverrideSequence = 0x30080060;
		public const uint ParameterSequencePointer = 0x30080061;
		public const uint OverrideParameterPointer = 0x30080062;
		public const uint ParameterItemIndex = 0x30080063;
		public const uint MeasuredDoseReferenceNumber = 0x30080064;
		public const uint ParameterPointer = 0x30080065;
		public const uint OverrideReason = 0x30080066;
		public const uint CorrectedParameterSequence = 0x30080068;
		public const uint CorrectionValue = 0x3008006a;
		public const uint CalculatedDoseReferenceSequence = 0x30080070;
		public const uint CalculatedDoseReferenceNumber = 0x30080072;
		public const uint CalculatedDoseReferenceDescription = 0x30080074;
		public const uint CalculatedDoseReferenceDoseValue = 0x30080076;
		public const uint StartMeterset = 0x30080078;
		public const uint EndMeterset = 0x3008007a;
		public const uint ReferencedMeasuredDoseReferenceSequence = 0x30080080;
		public const uint ReferencedMeasuredDoseReferenceNumber = 0x30080082;
		public const uint ReferencedCalculatedDoseReferenceSequence = 0x30080090;
		public const uint ReferencedCalculatedDoseReferenceNumber = 0x30080092;
		public const uint BeamLimitingDeviceLeafPairsSequence = 0x300800a0;
		public const uint RecordedWedgeSequence = 0x300800b0;
		public const uint RecordedCompensatorSequence = 0x300800c0;
		public const uint RecordedBlockSequence = 0x300800d0;
		public const uint TreatmentSummaryMeasuredDoseReferenceSequence = 0x300800e0;
		public const uint RecordedSnoutSequence = 0x300800f0;
		public const uint RecordedRangeShifterSequence = 0x300800f2;
		public const uint RecordedLateralSpreadingDeviceSequence = 0x300800f4;
		public const uint RecordedRangeModulatorSequence = 0x300800f6;
		public const uint RecordedSourceSequence = 0x30080100;
		public const uint SourceSerialNumber = 0x30080105;
		public const uint TreatmentSessionApplicationSetupSequence = 0x30080110;
		public const uint ApplicationSetupCheck = 0x30080116;
		public const uint RecordedBrachyAccessoryDeviceSequence = 0x30080120;
		public const uint ReferencedBrachyAccessoryDeviceNumber = 0x30080122;
		public const uint RecordedChannelSequence = 0x30080130;
		public const uint SpecifiedChannelTotalTime = 0x30080132;
		public const uint DeliveredChannelTotalTime = 0x30080134;
		public const uint SpecifiedNumberOfPulses = 0x30080136;
		public const uint DeliveredNumberOfPulses = 0x30080138;
		public const uint SpecifiedPulseRepetitionInterval = 0x3008013a;
		public const uint DeliveredPulseRepetitionInterval = 0x3008013c;
		public const uint RecordedSourceApplicatorSequence = 0x30080140;
		public const uint ReferencedSourceApplicatorNumber = 0x30080142;
		public const uint RecordedChannelShieldSequence = 0x30080150;
		public const uint ReferencedChannelShieldNumber = 0x30080152;
		public const uint BrachyControlPointDeliveredSequence = 0x30080160;
		public const uint SafePositionExitDate = 0x30080162;
		public const uint SafePositionExitTime = 0x30080164;
		public const uint SafePositionReturnDate = 0x30080166;
		public const uint SafePositionReturnTime = 0x30080168;
		public const uint CurrentTreatmentStatus = 0x30080200;
		public const uint TreatmentStatusComment = 0x30080202;
		public const uint FractionGroupSummarySequence = 0x30080220;
		public const uint ReferencedFractionNumber = 0x30080223;
		public const uint FractionGroupType = 0x30080224;
		public const uint BeamStopperPosition = 0x30080230;
		public const uint FractionStatusSummarySequence = 0x30080240;
		public const uint TreatmentDate = 0x30080250;
		public const uint TreatmentTime = 0x30080251;
		public const uint RTPlanLabel = 0x300a0002;
		public const uint RTPlanName = 0x300a0003;
		public const uint RTPlanDescription = 0x300a0004;
		public const uint RTPlanDate = 0x300a0006;
		public const uint RTPlanTime = 0x300a0007;
		public const uint TreatmentProtocols = 0x300a0009;
		public const uint PlanIntent = 0x300a000a;
		public const uint TreatmentSites = 0x300a000b;
		public const uint RTPlanGeometry = 0x300a000c;
		public const uint PrescriptionDescription = 0x300a000e;
		public const uint DoseReferenceSequence = 0x300a0010;
		public const uint DoseReferenceNumber = 0x300a0012;
		public const uint DoseReferenceUID = 0x300a0013;
		public const uint DoseReferenceStructureType = 0x300a0014;
		public const uint NominalBeamEnergyUnit = 0x300a0015;
		public const uint DoseReferenceDescription = 0x300a0016;
		public const uint DoseReferencePointCoordinates = 0x300a0018;
		public const uint NominalPriorDose = 0x300a001a;
		public const uint DoseReferenceType = 0x300a0020;
		public const uint ConstraintWeight = 0x300a0021;
		public const uint DeliveryWarningDose = 0x300a0022;
		public const uint DeliveryMaximumDose = 0x300a0023;
		public const uint TargetMinimumDose = 0x300a0025;
		public const uint TargetPrescriptionDose = 0x300a0026;
		public const uint TargetMaximumDose = 0x300a0027;
		public const uint TargetUnderdoseVolumeFraction = 0x300a0028;
		public const uint OrganAtRiskFullvolumeDose = 0x300a002a;
		public const uint OrganAtRiskLimitDose = 0x300a002b;
		public const uint OrganAtRiskMaximumDose = 0x300a002c;
		public const uint OrganAtRiskOverdoseVolumeFraction = 0x300a002d;
		public const uint ToleranceTableSequence = 0x300a0040;
		public const uint ToleranceTableNumber = 0x300a0042;
		public const uint ToleranceTableLabel = 0x300a0043;
		public const uint GantryAngleTolerance = 0x300a0044;
		public const uint BeamLimitingDeviceAngleTolerance = 0x300a0046;
		public const uint BeamLimitingDeviceToleranceSequence = 0x300a0048;
		public const uint BeamLimitingDevicePositionTolerance = 0x300a004a;
		public const uint SnoutPositionTolerance = 0x300a004b;
		public const uint PatientSupportAngleTolerance = 0x300a004c;
		public const uint TableTopEccentricAngleTolerance = 0x300a004e;
		public const uint TableTopPitchAngleTolerance = 0x300a004f;
		public const uint TableTopRollAngleTolerance = 0x300a0050;
		public const uint TableTopVerticalPositionTolerance = 0x300a0051;
		public const uint TableTopLongitudinalPositionTolerance = 0x300a0052;
		public const uint TableTopLateralPositionTolerance = 0x300a0053;
		public const uint RTPlanRelationship = 0x300a0055;
		public const uint FractionGroupSequence = 0x300a0070;
		public const uint FractionGroupNumber = 0x300a0071;
		public const uint FractionGroupDescription = 0x300a0072;
		public const uint NumberOfFractionsPlanned = 0x300a0078;
		public const uint NumberOfFractionPatternDigitsPerDay = 0x300a0079;
		public const uint RepeatFractionCycleLength = 0x300a007a;
		public const uint FractionPattern = 0x300a007b;
		public const uint NumberOfBeams = 0x300a0080;
		public const uint BeamDoseSpecificationPoint = 0x300a0082;
		public const uint BeamDose = 0x300a0084;
		public const uint BeamMeterset = 0x300a0086;
		public const uint BeamDosePointDepth = 0x300a0088;
		public const uint BeamDosePointEquivalentDepth = 0x300a0089;
		public const uint BeamDosePointSSD = 0x300a008a;
		public const uint NumberOfBrachyApplicationSetups = 0x300a00a0;
		public const uint BrachyApplicationSetupDoseSpecificationPoint = 0x300a00a2;
		public const uint BrachyApplicationSetupDose = 0x300a00a4;
		public const uint BeamSequence = 0x300a00b0;
		public const uint TreatmentMachineName = 0x300a00b2;
		public const uint PrimaryDosimeterUnit = 0x300a00b3;
		public const uint SourceAxisDistance = 0x300a00b4;
		public const uint BeamLimitingDeviceSequence = 0x300a00b6;
		public const uint RTBeamLimitingDeviceType = 0x300a00b8;
		public const uint SourceToBeamLimitingDeviceDistance = 0x300a00ba;
		public const uint IsocenterToBeamLimitingDeviceDistance = 0x300a00bb;
		public const uint NumberOfLeafJawPairs = 0x300a00bc;
		public const uint LeafPositionBoundaries = 0x300a00be;
		public const uint BeamNumber = 0x300a00c0;
		public const uint BeamName = 0x300a00c2;
		public const uint BeamDescription = 0x300a00c3;
		public const uint BeamType = 0x300a00c4;
		public const uint RadiationType = 0x300a00c6;
		public const uint HighDoseTechniqueType = 0x300a00c7;
		public const uint ReferenceImageNumber = 0x300a00c8;
		public const uint PlannedVerificationImageSequence = 0x300a00ca;
		public const uint ImagingDeviceSpecificAcquisitionParameters = 0x300a00cc;
		public const uint TreatmentDeliveryType = 0x300a00ce;
		public const uint NumberOfWedges = 0x300a00d0;
		public const uint WedgeSequence = 0x300a00d1;
		public const uint WedgeNumber = 0x300a00d2;
		public const uint WedgeType = 0x300a00d3;
		public const uint WedgeID = 0x300a00d4;
		public const uint WedgeAngle = 0x300a00d5;
		public const uint WedgeFactor = 0x300a00d6;
		public const uint TotalWedgeTrayWaterEquivalentThickness = 0x300a00d7;
		public const uint WedgeOrientation = 0x300a00d8;
		public const uint IsocenterToWedgeTrayDistance = 0x300a00d9;
		public const uint SourceToWedgeTrayDistance = 0x300a00da;
		public const uint WedgeThinEdgePosition = 0x300a00db;
		public const uint BolusID = 0x300a00dc;
		public const uint BolusDescription = 0x300a00dd;
		public const uint NumberOfCompensators = 0x300a00e0;
		public const uint MaterialID = 0x300a00e1;
		public const uint TotalCompensatorTrayFactor = 0x300a00e2;
		public const uint CompensatorSequence = 0x300a00e3;
		public const uint CompensatorNumber = 0x300a00e4;
		public const uint CompensatorID = 0x300a00e5;
		public const uint SourceToCompensatorTrayDistance = 0x300a00e6;
		public const uint CompensatorRows = 0x300a00e7;
		public const uint CompensatorColumns = 0x300a00e8;
		public const uint CompensatorPixelSpacing = 0x300a00e9;
		public const uint CompensatorPosition = 0x300a00ea;
		public const uint CompensatorTransmissionData = 0x300a00eb;
		public const uint CompensatorThicknessData = 0x300a00ec;
		public const uint NumberOfBoli = 0x300a00ed;
		public const uint CompensatorType = 0x300a00ee;
		public const uint NumberOfBlocks = 0x300a00f0;
		public const uint TotalBlockTrayFactor = 0x300a00f2;
		public const uint TotalBlockTrayWaterEquivalentThickness = 0x300a00f3;
		public const uint BlockSequence = 0x300a00f4;
		public const uint BlockTrayID = 0x300a00f5;
		public const uint SourceToBlockTrayDistance = 0x300a00f6;
		public const uint IsocenterToBlockTrayDistance = 0x300a00f7;
		public const uint BlockType = 0x300a00f8;
		public const uint AccessoryCode = 0x300a00f9;
		public const uint BlockDivergence = 0x300a00fa;
		public const uint BlockMountingPosition = 0x300a00fb;
		public const uint BlockNumber = 0x300a00fc;
		public const uint BlockName = 0x300a00fe;
		public const uint BlockThickness = 0x300a0100;
		public const uint BlockTransmission = 0x300a0102;
		public const uint BlockNumberOfPoints = 0x300a0104;
		public const uint BlockData = 0x300a0106;
		public const uint ApplicatorSequence = 0x300a0107;
		public const uint ApplicatorID = 0x300a0108;
		public const uint ApplicatorType = 0x300a0109;
		public const uint ApplicatorDescription = 0x300a010a;
		public const uint CumulativeDoseReferenceCoefficient = 0x300a010c;
		public const uint FinalCumulativeMetersetWeight = 0x300a010e;
		public const uint NumberOfControlPoints = 0x300a0110;
		public const uint ControlPointSequence = 0x300a0111;
		public const uint ControlPointIndex = 0x300a0112;
		public const uint NominalBeamEnergy = 0x300a0114;
		public const uint DoseRateSet = 0x300a0115;
		public const uint WedgePositionSequence = 0x300a0116;
		public const uint WedgePosition = 0x300a0118;
		public const uint BeamLimitingDevicePositionSequence = 0x300a011a;
		public const uint LeafJawPositions = 0x300a011c;
		public const uint GantryAngle = 0x300a011e;
		public const uint GantryRotationDirection = 0x300a011f;
		public const uint BeamLimitingDeviceAngle = 0x300a0120;
		public const uint BeamLimitingDeviceRotationDirection = 0x300a0121;
		public const uint PatientSupportAngle = 0x300a0122;
		public const uint PatientSupportRotationDirection = 0x300a0123;
		public const uint TableTopEccentricAxisDistance = 0x300a0124;
		public const uint TableTopEccentricAngle = 0x300a0125;
		public const uint TableTopEccentricRotationDirection = 0x300a0126;
		public const uint TableTopVerticalPosition = 0x300a0128;
		public const uint TableTopLongitudinalPosition = 0x300a0129;
		public const uint TableTopLateralPosition = 0x300a012a;
		public const uint IsocenterPosition = 0x300a012c;
		public const uint SurfaceEntryPoint = 0x300a012e;
		public const uint SourceToSurfaceDistance = 0x300a0130;
		public const uint CumulativeMetersetWeight = 0x300a0134;
		public const uint TableTopPitchAngle = 0x300a0140;
		public const uint TableTopPitchRotationDirection = 0x300a0142;
		public const uint TableTopRollAngle = 0x300a0144;
		public const uint TableTopRollRotationDirection = 0x300a0146;
		public const uint HeadFixationAngle = 0x300a0148;
		public const uint GantryPitchAngle = 0x300a014a;
		public const uint GantryPitchRotationDirection = 0x300a014c;
		public const uint GantryPitchAngleTolerance = 0x300a014e;
		public const uint PatientSetupSequence = 0x300a0180;
		public const uint PatientSetupNumber = 0x300a0182;
		public const uint PatientSetupLabel = 0x300a0183;
		public const uint PatientAdditionalPosition = 0x300a0184;
		public const uint FixationDeviceSequence = 0x300a0190;
		public const uint FixationDeviceType = 0x300a0192;
		public const uint FixationDeviceLabel = 0x300a0194;
		public const uint FixationDeviceDescription = 0x300a0196;
		public const uint FixationDevicePosition = 0x300a0198;
		public const uint FixationDevicePitchAngle = 0x300a0199;
		public const uint FixationDeviceRollAngle = 0x300a019a;
		public const uint ShieldingDeviceSequence = 0x300a01a0;
		public const uint ShieldingDeviceType = 0x300a01a2;
		public const uint ShieldingDeviceLabel = 0x300a01a4;
		public const uint ShieldingDeviceDescription = 0x300a01a6;
		public const uint ShieldingDevicePosition = 0x300a01a8;
		public const uint SetupTechnique = 0x300a01b0;
		public const uint SetupTechniqueDescription = 0x300a01b2;
		public const uint SetupDeviceSequence = 0x300a01b4;
		public const uint SetupDeviceType = 0x300a01b6;
		public const uint SetupDeviceLabel = 0x300a01b8;
		public const uint SetupDeviceDescription = 0x300a01ba;
		public const uint SetupDeviceParameter = 0x300a01bc;
		public const uint SetupReferenceDescription = 0x300a01d0;
		public const uint TableTopVerticalSetupDisplacement = 0x300a01d2;
		public const uint TableTopLongitudinalSetupDisplacement = 0x300a01d4;
		public const uint TableTopLateralSetupDisplacement = 0x300a01d6;
		public const uint BrachyTreatmentTechnique = 0x300a0200;
		public const uint BrachyTreatmentType = 0x300a0202;
		public const uint TreatmentMachineSequence = 0x300a0206;
		public const uint SourceSequence = 0x300a0210;
		public const uint SourceNumber = 0x300a0212;
		public const uint SourceType = 0x300a0214;
		public const uint SourceManufacturer = 0x300a0216;
		public const uint ActiveSourceDiameter = 0x300a0218;
		public const uint ActiveSourceLength = 0x300a021a;
		public const uint SourceEncapsulationNominalThickness = 0x300a0222;
		public const uint SourceEncapsulationNominalTransmission = 0x300a0224;
		public const uint SourceIsotopeName = 0x300a0226;
		public const uint SourceIsotopeHalfLife = 0x300a0228;
		public const uint SourceStrengthUnits = 0x300a0229;
		public const uint ReferenceAirKermaRate = 0x300a022a;
		public const uint SourceStrength = 0x300a022b;
		public const uint SourceStrengthReferenceDate = 0x300a022c;
		public const uint SourceStrengthReferenceTime = 0x300a022e;
		public const uint ApplicationSetupSequence = 0x300a0230;
		public const uint ApplicationSetupType = 0x300a0232;
		public const uint ApplicationSetupNumber = 0x300a0234;
		public const uint ApplicationSetupName = 0x300a0236;
		public const uint ApplicationSetupManufacturer = 0x300a0238;
		public const uint TemplateNumber = 0x300a0240;
		public const uint TemplateType = 0x300a0242;
		public const uint TemplateName = 0x300a0244;
		public const uint TotalReferenceAirKerma = 0x300a0250;
		public const uint BrachyAccessoryDeviceSequence = 0x300a0260;
		public const uint BrachyAccessoryDeviceNumber = 0x300a0262;
		public const uint BrachyAccessoryDeviceID = 0x300a0263;
		public const uint BrachyAccessoryDeviceType = 0x300a0264;
		public const uint BrachyAccessoryDeviceName = 0x300a0266;
		public const uint BrachyAccessoryDeviceNominalThickness = 0x300a026a;
		public const uint BrachyAccessoryDeviceNominalTransmission = 0x300a026c;
		public const uint ChannelSequence = 0x300a0280;
		public const uint ChannelNumber = 0x300a0282;
		public const uint ChannelLength = 0x300a0284;
		public const uint ChannelTotalTime = 0x300a0286;
		public const uint SourceMovementType = 0x300a0288;
		public const uint NumberOfPulses = 0x300a028a;
		public const uint PulseRepetitionInterval = 0x300a028c;
		public const uint SourceApplicatorNumber = 0x300a0290;
		public const uint SourceApplicatorID = 0x300a0291;
		public const uint SourceApplicatorType = 0x300a0292;
		public const uint SourceApplicatorName = 0x300a0294;
		public const uint SourceApplicatorLength = 0x300a0296;
		public const uint SourceApplicatorManufacturer = 0x300a0298;
		public const uint SourceApplicatorWallNominalThickness = 0x300a029c;
		public const uint SourceApplicatorWallNominalTransmission = 0x300a029e;
		public const uint SourceApplicatorStepSize = 0x300a02a0;
		public const uint TransferTubeNumber = 0x300a02a2;
		public const uint TransferTubeLength = 0x300a02a4;
		public const uint ChannelShieldSequence = 0x300a02b0;
		public const uint ChannelShieldNumber = 0x300a02b2;
		public const uint ChannelShieldID = 0x300a02b3;
		public const uint ChannelShieldName = 0x300a02b4;
		public const uint ChannelShieldNominalThickness = 0x300a02b8;
		public const uint ChannelShieldNominalTransmission = 0x300a02ba;
		public const uint FinalCumulativeTimeWeight = 0x300a02c8;
		public const uint BrachyControlPointSequence = 0x300a02d0;
		public const uint ControlPointRelativePosition = 0x300a02d2;
		public const uint ControlPoint3DPosition = 0x300a02d4;
		public const uint CumulativeTimeWeight = 0x300a02d6;
		public const uint CompensatorDivergence = 0x300a02e0;
		public const uint CompensatorMountingPosition = 0x300a02e1;
		public const uint SourceToCompensatorDistance = 0x300a02e2;
		public const uint TotalCompensatorTrayWaterEquivalentThickness = 0x300a02e3;
		public const uint IsocenterToCompensatorTrayDistance = 0x300a02e4;
		public const uint CompensatorColumnOffset = 0x300a02e5;
		public const uint IsocenterToCompensatorDistances = 0x300a02e6;
		public const uint CompensatorRelativeStoppingPowerRatio = 0x300a02e7;
		public const uint CompensatorMillingToolDiameter = 0x300a02e8;
		public const uint IonRangeCompensatorSequence = 0x300a02ea;
		public const uint RadiationMassNumber = 0x300a0302;
		public const uint RadiationAtomicNumber = 0x300a0304;
		public const uint RadiationChargeState = 0x300a0306;
		public const uint ScanMode = 0x300a0308;
		public const uint VirtualSourceAxisDistances = 0x300a030a;
		public const uint SnoutSequence = 0x300a030c;
		public const uint SnoutPosition = 0x300a030d;
		public const uint SnoutID = 0x300a030f;
		public const uint NumberOfRangeShifters = 0x300a0312;
		public const uint RangeShifterSequence = 0x300a0314;
		public const uint RangeShifterNumber = 0x300a0316;
		public const uint RangeShifterID = 0x300a0318;
		public const uint RangeShifterType = 0x300a0320;
		public const uint RangeShifterDescription = 0x300a0322;
		public const uint NumberOfLateralSpreadingDevices = 0x300a0330;
		public const uint LateralSpreadingDeviceSequence = 0x300a0332;
		public const uint LateralSpreadingDeviceNumber = 0x300a0334;
		public const uint LateralSpreadingDeviceID = 0x300a0336;
		public const uint LateralSpreadingDeviceType = 0x300a0338;
		public const uint LateralSpreadingDeviceDescription = 0x300a033a;
		public const uint LateralSpreadingDeviceWaterEquivalentThickness = 0x300a033c;
		public const uint NumberOfRangeModulators = 0x300a0340;
		public const uint RangeModulatorSequence = 0x300a0342;
		public const uint RangeModulatorNumber = 0x300a0344;
		public const uint RangeModulatorID = 0x300a0346;
		public const uint RangeModulatorType = 0x300a0348;
		public const uint RangeModulatorDescription = 0x300a034a;
		public const uint BeamCurrentModulationID = 0x300a034c;
		public const uint PatientSupportType = 0x300a0350;
		public const uint PatientSupportID = 0x300a0352;
		public const uint PatientSupportAccessoryCode = 0x300a0354;
		public const uint FixationLightAzimuthalAngle = 0x300a0356;
		public const uint FixationLightPolarAngle = 0x300a0358;
		public const uint MetersetRate = 0x300a035a;
		public const uint RangeShifterSettingsSequence = 0x300a0360;
		public const uint RangeShifterSetting = 0x300a0362;
		public const uint IsocenterToRangeShifterDistance = 0x300a0364;
		public const uint RangeShifterWaterEquivalentThickness = 0x300a0366;
		public const uint LateralSpreadingDeviceSettingsSequence = 0x300a0370;
		public const uint LateralSpreadingDeviceSetting = 0x300a0372;
		public const uint IsocenterToLateralSpreadingDeviceDistance = 0x300a0374;
		public const uint RangeModulatorSettingsSequence = 0x300a0380;
		public const uint RangeModulatorGatingStartValue = 0x300a0382;
		public const uint RangeModulatorGatingStopValue = 0x300a0384;
		public const uint RangeModulatorGatingStartWaterEquivalentThickness = 0x300a0386;
		public const uint RangeModulatorGatingStopWaterEquivalentThickness = 0x300a0388;
		public const uint IsocenterToRangeModulatorDistance = 0x300a038a;
		public const uint ScanSpotTuneID = 0x300a0390;
		public const uint NumberOfScanSpotPositions = 0x300a0392;
		public const uint ScanSpotPositionMap = 0x300a0394;
		public const uint ScanSpotMetersetWeights = 0x300a0396;
		public const uint ScanningSpotSize = 0x300a0398;
		public const uint NumberOfPaintings = 0x300a039a;
		public const uint IonToleranceTableSequence = 0x300a03a0;
		public const uint IonBeamSequence = 0x300a03a2;
		public const uint IonBeamLimitingDeviceSequence = 0x300a03a4;
		public const uint IonBlockSequence = 0x300a03a6;
		public const uint IonControlPointSequence = 0x300a03a8;
		public const uint IonWedgeSequence = 0x300a03aa;
		public const uint IonWedgePositionSequence = 0x300a03ac;
		public const uint ReferencedSetupImageSequence = 0x300a0401;
		public const uint SetupImageComment = 0x300a0402;
		public const uint MotionSynchronizationSequence = 0x300a0410;
		public const uint ReferencedRTPlanSequence = 0x300c0002;
		public const uint ReferencedBeamSequence = 0x300c0004;
		public const uint ReferencedBeamNumber = 0x300c0006;
		public const uint ReferencedReferenceImageNumber = 0x300c0007;
		public const uint StartCumulativeMetersetWeight = 0x300c0008;
		public const uint EndCumulativeMetersetWeight = 0x300c0009;
		public const uint ReferencedBrachyApplicationSetupSequence = 0x300c000a;
		public const uint ReferencedBrachyApplicationSetupNumber = 0x300c000c;
		public const uint ReferencedSourceNumber = 0x300c000e;
		public const uint ReferencedFractionGroupSequence = 0x300c0020;
		public const uint ReferencedFractionGroupNumber = 0x300c0022;
		public const uint ReferencedVerificationImageSequence = 0x300c0040;
		public const uint ReferencedReferenceImageSequence = 0x300c0042;
		public const uint ReferencedDoseReferenceSequence = 0x300c0050;
		public const uint ReferencedDoseReferenceNumber = 0x300c0051;
		public const uint BrachyReferencedDoseReferenceSequence = 0x300c0055;
		public const uint ReferencedStructureSetSequence = 0x300c0060;
		public const uint ReferencedPatientSetupNumber = 0x300c006a;
		public const uint ReferencedDoseSequence = 0x300c0080;
		public const uint ReferencedToleranceTableNumber = 0x300c00a0;
		public const uint ReferencedBolusSequence = 0x300c00b0;
		public const uint ReferencedWedgeNumber = 0x300c00c0;
		public const uint ReferencedCompensatorNumber = 0x300c00d0;
		public const uint ReferencedBlockNumber = 0x300c00e0;
		public const uint ReferencedControlPointIndex = 0x300c00f0;
		public const uint ReferencedControlPointSequence = 0x300c00f2;
		public const uint ReferencedStartControlPointIndex = 0x300c00f4;
		public const uint ReferencedStopControlPointIndex = 0x300c00f6;
		public const uint ReferencedRangeShifterNumber = 0x300c0100;
		public const uint ReferencedLateralSpreadingDeviceNumber = 0x300c0102;
		public const uint ReferencedRangeModulatorNumber = 0x300c0104;
		public const uint ApprovalStatus = 0x300e0002;
		public const uint ReviewDate = 0x300e0004;
		public const uint ReviewTime = 0x300e0005;
		public const uint ReviewerName = 0x300e0008;
		public const uint ArbitraryRETIRED = 0x40000010;
		public const uint TextCommentsRETIRED = 0x40004000;
		public const uint ResultsIDRETIRED = 0x40080040;
		public const uint ResultsIDIssuerRETIRED = 0x40080042;
		public const uint ReferencedInterpretationSequenceRETIRED = 0x40080050;
		public const uint InterpretationRecordedDateRETIRED = 0x40080100;
		public const uint InterpretationRecordedTimeRETIRED = 0x40080101;
		public const uint InterpretationRecorderRETIRED = 0x40080102;
		public const uint ReferenceToRecordedSoundRETIRED = 0x40080103;
		public const uint InterpretationTranscriptionDateRETIRED = 0x40080108;
		public const uint InterpretationTranscriptionTimeRETIRED = 0x40080109;
		public const uint InterpretationTranscriberRETIRED = 0x4008010a;
		public const uint InterpretationTextRETIRED = 0x4008010b;
		public const uint InterpretationAuthorRETIRED = 0x4008010c;
		public const uint InterpretationApproverSequenceRETIRED = 0x40080111;
		public const uint InterpretationApprovalDateRETIRED = 0x40080112;
		public const uint InterpretationApprovalTimeRETIRED = 0x40080113;
		public const uint PhysicianApprovingInterpretationRETIRED = 0x40080114;
		public const uint InterpretationDiagnosisDescriptionRETIRED = 0x40080115;
		public const uint InterpretationDiagnosisCodeSequenceRETIRED = 0x40080117;
		public const uint ResultsDistributionListSequenceRETIRED = 0x40080118;
		public const uint DistributionNameRETIRED = 0x40080119;
		public const uint DistributionAddressRETIRED = 0x4008011a;
		public const uint InterpretationIDRETIRED = 0x40080200;
		public const uint InterpretationIDIssuerRETIRED = 0x40080202;
		public const uint InterpretationTypeIDRETIRED = 0x40080210;
		public const uint InterpretationStatusIDRETIRED = 0x40080212;
		public const uint ImpressionsRETIRED = 0x40080300;
		public const uint ResultsCommentsRETIRED = 0x40084000;
		public const uint MACParametersSequence = 0x4ffe0001;
		public const uint SharedFunctionalGroupsSequence = 0x52009229;
		public const uint PerframeFunctionalGroupsSequence = 0x52009230;
		public const uint WaveformSequence = 0x54000100;
		public const uint ChannelMinimumValue = 0x54000110;
		public const uint ChannelMaximumValue = 0x54000112;
		public const uint WaveformBitsAllocated = 0x54001004;
		public const uint WaveformSampleInterpretation = 0x54001006;
		public const uint WaveformPaddingValue = 0x5400100a;
		public const uint WaveformData = 0x54001010;
		public const uint FirstOrderPhaseCorrectionAngle = 0x56000010;
		public const uint SpectroscopyData = 0x56000020;
		public const uint PixelData = 0x7fe00010;
		public const uint DigitalSignaturesSequence = 0xfffafffa;
		public const uint DataSetTrailingPadding = 0xfffcfffc;
		public const uint Item = 0xfffee000;
		public const uint ItemDelimitationItem = 0xfffee00d;
		public const uint SequenceDelimitationItem = 0xfffee0dd;
		#endregion
	}

	public class DcmTagMask {
		#region Private Members
		private uint _c;
		private uint _m;
		#endregion

		#region Public Constructor
		public DcmTagMask(string mask) {
			mask = mask.Trim('(', ')');
			mask = mask.Replace(",", "").ToLower();

			StringBuilder sb = new StringBuilder();
			sb.Replace('x', '0');
			_c = uint.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber);

			sb = new StringBuilder(mask);
			sb.Replace('0', 'F').Replace('1', 'F').Replace('2', 'F')
				.Replace('3', 'F').Replace('4', 'F').Replace('5', 'F')
				.Replace('6', 'F').Replace('7', 'F').Replace('8', 'F')
				.Replace('9', 'F').Replace('a', 'F').Replace('b', 'F')
				.Replace('c', 'F').Replace('d', 'F').Replace('e', 'F')
				.Replace('f', 'F').Replace('x', '0');
			_m = uint.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber);
		}
		#endregion

		#region Public Properties
		public uint Card {
			get { return _c; }
		}

		public uint Mask {
			get { return _m; }
		}

		public bool IsFullMask {
			get { return _m == 0xFFFFFFFF; }
		}

		public DcmTag Tag {
			get { return new DcmTag(_c); }
		}
		#endregion

		#region Public Members
		public bool IsMatch(DcmTag tag) {
			return (tag.Card & Mask) == Card;
		}

		public override string ToString() {
			string tag = String.Empty;
			string card = _c.ToString("X8");
			uint mask = 0xF0000000;
			for (int i = 0; i < 8; i++) {
				if ((_m & mask) != 0)
					tag += card[i];
				else
					tag += 'X';
				mask >>= 4;
			}
			return tag;
		}
		#endregion
	}
}
