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
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Dicom.Data {
	public enum DicomModifyOp {
		Remove,
		AddOrReplace,
		Map,
		Split,
		Regex,
		UserOp1,
		UserOp2,
		UserOp3,
		UserOp4
	}

	public delegate void DcmModifyCallback(DcmModify op, DcmTag tag, DcmDataset dataset);

	[Serializable]
	public class DcmModify {
		public static DcmModifyCallback UserOp1Callback;
		public static DcmModifyCallback UserOp2Callback;
		public static DcmModifyCallback UserOp3Callback;
		public static DcmModifyCallback UserOp4Callback;

		#region Private Members
		private DcmTagMask _mask;
		private DicomModifyOp _op;
		private string _input;
		private string _output;
		#endregion

		#region Public Constructors
		public DcmModify() {
		}

		public DcmModify(DcmTag tag, DicomModifyOp op, string input, string output) {
			_mask = new DcmTagMask(tag.ToString());
			_op = op;
			_input = input;
			_output = output;
		}

		public DcmModify(string mask, DicomModifyOp op, string input, string output) {
			_mask = new DcmTagMask(mask);
			_op = op;
			_input = input;
			_output = output;
		}
		#endregion

		#region Public Properties
		public DcmTagMask Mask {
			get { return _mask; }
			set { _mask = value; }
		}

		public DicomModifyOp Operator {
			get { return _op; }
			set { _op = value; }
		}

		public string Input {
			get { return _input; }
			set { _input = value; }
		}

		public string Output {
			get { return _output; }
			set { _output = value; }
		}
		#endregion

		#region Public Methods
		public void Modify(DcmDataset dataset) {
			List<DcmTag> tags = new List<DcmTag>();
			tags.AddRange(dataset.GetMaskedTags(_mask));
			foreach (DcmTag tag in tags) {
				Apply(dataset, tag);
			}
		}

		public static string Serialize(DcmModify modify) {
			StringWriter writer = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(DcmModify));
			serializer.Serialize(writer, modify);
			return writer.ToString();
		}

		public static DcmModify Deserialize(string data) {
			if (String.IsNullOrEmpty(data))
				return null;
			StringReader reader = new StringReader(data);
			XmlSerializer serializer = new XmlSerializer(typeof(DcmModify));
			return (DcmModify)serializer.Deserialize(reader);
		}

		public static string SerializeList(List<DcmModify> modifiers) {
			StringWriter writer = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(List<DcmModify>));
			serializer.Serialize(writer, modifiers);
			return writer.ToString();
		}

		public static List<DcmModify> DeserializeList(string data) {
			if (String.IsNullOrEmpty(data))
				return null;
			StringReader reader = new StringReader(data);
			XmlSerializer serializer = new XmlSerializer(typeof(List<DcmModify>));
			return (List<DcmModify>)serializer.Deserialize(reader);
		}

		public override string ToString() {
			return String.Format("'{0}' {1} '{2}'->'{3}'", _mask, _op, _input, _output);
		}
		#endregion

		#region Private Methods
		private void Apply(DcmDataset dataset, DcmTag tag) {
			try {
				if (_op == DicomModifyOp.Remove) {
					dataset.Remove(tag);
					return;
				}

				if (_op == DicomModifyOp.UserOp1) {
					if (UserOp1Callback != null)
						UserOp1Callback(this, tag, dataset);
					return;
				}

				if (_op == DicomModifyOp.UserOp2) {
					if (UserOp2Callback != null)
						UserOp2Callback(this, tag, dataset);
					return;
				}

				if (_op == DicomModifyOp.UserOp3) {
					if (UserOp3Callback != null)
						UserOp3Callback(this, tag, dataset);
					return;
				}

				if (_op == DicomModifyOp.UserOp4) {
					if (UserOp4Callback != null)
						UserOp4Callback(this, tag, dataset);
					return;
				}

				DcmElement elem = dataset.GetElement(tag);

				if (elem == null) {
					if (_op == DicomModifyOp.AddOrReplace) {
						dataset.AddElementWithValueString(tag, _output);
					}
					return;
				}

				string value = elem.GetValueString();

				if (_op == DicomModifyOp.Map) {
					if (_input == value)
						elem.SetValueString(_output);
					return;
				}

				if (_op == DicomModifyOp.Split) {
					string[] parts = value.Split(_input.ToCharArray());
					elem.SetValueString(String.Format(_output, parts));
					return;
				}

				if (_op == DicomModifyOp.Regex) {
					elem.SetValueString(Regex.Replace(value, _input, _output));
					return;
				}
			}
			catch (Exception e) {
				Dicom.Debug.Log.Error("Unable to modify dataset: {0}\n\trule: {1}", e.Message, ToString());
			}
		}
		#endregion
	}
}
