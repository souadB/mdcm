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

using Dicom.Data;

namespace Dicom.Network {
	public enum DcmPriority : ushort {
		Low = 0x0002,
		Medium = 0x0000,
		High = 0x0001
	}

	public enum DcmCommandField : ushort {
		CStoreRequest = 0x0001,
		CStoreResponse = 0x8001,
		CGetRequest = 0x0010,
		CGetResponse = 0x8010,
		CFindRequest = 0x0020,
		CFindResponse = 0x8020,
		CMoveRequest = 0x0021,
		CMoveResponse = 0x8021,
		CEchoRequest = 0x0030,
		CEchoResponse = 0x8030,
		NEventReportRequest = 0x0100,
		NEventReportResponse = 0x8100,
		NGetRequest = 0x0110,
		NGetResponse = 0x8110,
		NSetRequest = 0x0120,
		NSetResponse = 0x8120,
		NActionRequest = 0x0130,
		NActionResponse = 0x8130,
		NCreateRequest = 0x0140,
		NCreateResponse = 0x8140,
		NDeleteRequest = 0x0150,
		NDeleteResponse = 0x8150,
		CCancelRequest = 0x0FFF
	}

	public class DcmCommand : DcmDataset {
		#region Public Constructors
		public DcmCommand() : base(DcmTS.ImplicitVRLittleEndian) {
		}
		#endregion

		#region Public Properties
		public DcmUID AffectedSOPClassUID {
			get {
				return GetUID(DcmTags.AffectedSOPClassUID);
			}
			set {
				AddElementWithValue(DcmTags.AffectedSOPClassUID, value);
			}
		}

		public DcmUID RequestedSOPClassUID {
			get {
				return GetUID(DcmTags.RequestedSOPClassUID);
			}
			set {
				AddElementWithValue(DcmTags.RequestedSOPClassUID, value);
			}
		}

		public DcmCommandField CommandField {
			get {
				return (DcmCommandField)GetUInt16(DcmTags.CommandField, 0);
			}
			set {
				AddElementWithValue(DcmTags.CommandField, (ushort)value);
			}
		}

		public ushort MessageID {
			get {
				return GetUInt16(DcmTags.MessageID, 0);
			}
			set {
				AddElementWithValue(DcmTags.MessageID, value);
			}
		}

		public ushort MessageIDBeingRespondedTo {
			get {
				return GetUInt16(DcmTags.MessageIDBeingRespondedTo, 0);
			}
			set {
				AddElementWithValue(DcmTags.MessageIDBeingRespondedTo, value);
			}
		}

		public string MoveDestinationAE {
			get {
				return GetString(DcmTags.MoveDestination, null);
			}
			set {
				AddElementWithValue(DcmTags.MoveDestination, value);
			}
		}

		public DcmPriority Priority {
			get {
				return (DcmPriority)GetUInt16(DcmTags.Priority, 0);
			}
			set {
				AddElementWithValue(DcmTags.Priority, (ushort)value);
			}
		}

		public bool HasDataset {
			get {
				return DataSetType != (ushort)0x0101;
			}
			set {
				DataSetType = value ? (ushort)0x0202 : (ushort)0x0101;
			}
		}

		public ushort DataSetType {
			get {
				return GetUInt16(DcmTags.DataSetType, (ushort)0x0101);
			}
			set {
				AddElementWithValue(DcmTags.DataSetType, value);
			}
		}

		public DcmStatus Status {
			get {
				return DcmStatus.Lookup(GetUInt16(DcmTags.Status, 0x0211));
			}
			set {
				AddElementWithValue(DcmTags.Status, value.Code);
			}
		}

		public DcmTag OffendingElement {
			get {
				return GetDcmTag(DcmTags.OffendingElement);
			}
			set {
				AddElementWithValue(DcmTags.OffendingElement, value);
			}
		}

		public string ErrorComment {
			get {
				return GetString(DcmTags.ErrorComment, null);
			}
			set {
				AddElementWithValue(DcmTags.ErrorComment, value);
			}
		}

		public ushort ErrorID {
			get {
				return GetUInt16(DcmTags.ErrorID, 0);
			}
			set {
				AddElementWithValue(DcmTags.ErrorID, value);
			}
		}

		public DcmUID AffectedSOPInstanceUID {
			get {
				return GetUID(DcmTags.AffectedSOPInstanceUID);
			}
			set {
				AddElementWithValue(DcmTags.AffectedSOPInstanceUID, value);
			}
		}

		public DcmUID RequestedSOPInstanceUID {
			get {
				return GetUID(DcmTags.RequestedSOPInstanceUID);
			}
			set {
				AddElementWithValue(DcmTags.RequestedSOPInstanceUID, value);
			}
		}

		public ushort EventTypeID {
			get {
				return GetUInt16(DcmTags.EventTypeID, 0);
			}
			set {
				AddElementWithValue(DcmTags.EventTypeID, value);
			}
		}

		public DcmAttributeTag AttributeIdentifierList {
			get {
				return GetAT(DcmTags.AttributeIdentifierList);
			}
			set {
				AddItem(value);
			}
		}

		public ushort ActionTypeID {
			get {
				return GetUInt16(DcmTags.ActionTypeID, 0);
			}
			set {
				AddElementWithValue(DcmTags.ActionTypeID, value);
			}
		}

		public ushort NumberOfRemainingSuboperations {
			get {
				return GetUInt16(DcmTags.NumberOfRemainingSuboperations, 0);
			}
			set {
				AddElementWithValue(DcmTags.NumberOfRemainingSuboperations, value);
			}
		}

		public ushort NumberOfCompletedSuboperations {
			get {
				return GetUInt16(DcmTags.NumberOfCompletedSuboperations, 0);
			}
			set {
				AddElementWithValue(DcmTags.NumberOfCompletedSuboperations, value);
			}
		}

		public ushort NumberOfFailedSuboperations {
			get {
				return GetUInt16(DcmTags.NumberOfFailedSuboperations, 0);
			}
			set {
				AddElementWithValue(DcmTags.NumberOfFailedSuboperations, value);
			}
		}

		public ushort NumberOfWarningSuboperations {
			get {
				return GetUInt16(DcmTags.NumberOfWarningSuboperations, 0);
			}
			set {
				AddElementWithValue(DcmTags.NumberOfWarningSuboperations, value);
			}
		}

		public string MoveOriginatorAE {
			get {
				return GetString(DcmTags.MoveOriginatorApplicationEntityTitle, null);
			}
			set {
				AddElementWithValue(DcmTags.MoveOriginatorApplicationEntityTitle, value);
			}
		}

		public ushort MoveOriginatorMessageID {
			get {
				return GetUInt16(DcmTags.MoveOriginatorMessageID, 0);
			}
			set {
				AddElementWithValue(DcmTags.MoveOriginatorMessageID, value);
			}
		}
		#endregion
	}
}
