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

namespace Dicom.Data {
	public class DcmFileMetaInfo : DcmDataset {
		#region Public Constructors
		public DcmFileMetaInfo() : base(DcmTS.ExplicitVRLittleEndian) {
		}
		#endregion

		#region Public Properties
		public readonly static byte[] Version = new byte[] { 0x00, 0x01 };
		public readonly static DcmTag StopTag = new DcmTag(0x0002, 0xFFFF);

		public byte[] FileMetaInformationVersion {
			get {
				if (Contains(DcmTags.FileMetaInformationVersion))
					return GetOB(DcmTags.FileMetaInformationVersion).GetValues();
				return null;
			}
			set {
				if (AddElement(DcmTags.FileMetaInformationVersion, DcmVR.OB))
					GetOB(DcmTags.FileMetaInformationVersion).SetValues(value);
			}
		}

		public DcmUID MediaStorageSOPClassUID {
			get {
				return GetUID(DcmTags.MediaStorageSOPClassUID);
			}
			set {
				AddElementWithValue(DcmTags.MediaStorageSOPClassUID, value);
			}
		}

		public DcmUID MediaStorageSOPInstanceUID {
			get {
				return GetUID(DcmTags.MediaStorageSOPInstanceUID);
			}
			set {
				AddElementWithValue(DcmTags.MediaStorageSOPInstanceUID, value);
			}
		}

		public DcmTS TransferSyntax {
			get {
				if (Contains(DcmTags.TransferSyntaxUID))
					return GetUI(DcmTags.TransferSyntaxUID).GetTS();
				return null;
			}
			set {
				AddElementWithValue(DcmTags.TransferSyntaxUID, value.UID);
			}
		}

		public DcmUID ImplementationClassUID {
			get {
				return GetUID(DcmTags.ImplementationClassUID);
			}
			set {
				AddElementWithValue(DcmTags.ImplementationClassUID, value);
			}
		}

		public string ImplementationVersionName {
			get {
				return GetString(DcmTags.ImplementationVersionName, null);
			}
			set {
				AddElementWithValue(DcmTags.ImplementationVersionName, value);
			}
		}

		public string SourceApplicationEntityTitle {
			get {
				return GetString(DcmTags.SourceApplicationEntityTitle, null);
			}
			set {
				AddElementWithValue(DcmTags.SourceApplicationEntityTitle, value);
			}
		}

		public DcmUID PrivateInformationCreatorUID {
			get {
				return GetUID(DcmTags.PrivateInformationCreatorUID);
			}
			set {
				AddElementWithValue(DcmTags.PrivateInformationCreatorUID, value);
			}
		}

		public byte[] PrivateInformation {
			get {
				if (Contains(DcmTags.PrivateInformation))
					return GetOB(DcmTags.PrivateInformation).GetValues();
				return null;
			}
			set {
				if (AddElement(DcmTags.PrivateInformation, DcmVR.OB))
					GetOB(DcmTags.PrivateInformation).SetValues(value);
			}
		}
		#endregion
	}
}
