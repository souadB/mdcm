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
	public class DicomTemplateAdapter {
		private static Dictionary<string, DcmTag> _map;

		static DicomTemplateAdapter() {
			_map = new Dictionary<string, DcmTag>();
			_map.Add("StudyUID", DcmTags.StudyInstanceUID);
			_map.Add("SeriesUID", DcmTags.SeriesInstanceUID);
			_map.Add("ImageUID", DcmTags.SOPInstanceUID);
			_map.Add("PatientID", DcmTags.PatientID);
			_map.Add("PatientName", DcmTags.PatientsName);
			_map.Add("PatientSex", DcmTags.PatientsSex);
			_map.Add("Modality", DcmTags.Modality);
			_map.Add("Description", DcmTags.StudyDescription);
			_map.Add("BodyPart", DcmTags.BodyPartExamined);
			_map.Add("StudyID", DcmTags.StudyID);
			_map.Add("AccessionNumber", DcmTags.AccessionNumber);
			_map.Add("StudyDate", DcmTags.StudyDate);
			_map.Add("StudyTime", DcmTags.StudyTime);
		}

		private DcmDataset _dataset;

		public DicomTemplateAdapter(DcmDataset dataset) {
			_dataset = dataset;
		}

		public object this[string index] {
			get {
				if (_dataset == null)
					return String.Empty;

				DcmTag tag = null;
				if (!_map.TryGetValue(index, out tag)) {
					tag = DcmTag.Parse(index);
				}

				if (tag != null) {
					DcmElement elem = _dataset.GetElement(tag);
					if (elem.Length == 0)
						return String.Empty;
					if (elem != null) {
						return elem.GetValueObject();
					}
				}

				return String.Empty;
			}
		}
	}
}
