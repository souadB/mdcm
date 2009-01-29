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
		Prefix,
		Append,
		Format,
		Chomp,
		Truncate,
		GenerateID,
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
		private string _mask;
		private DicomModifyOp _op;
		private string _input;
		private string _output;
		#endregion

		#region Public Constructors
		public DcmModify() {
		}

		public DcmModify(DcmTagMask mask, DicomModifyOp op, string input, string output) {
			_mask = mask.ToString();
			_op = op;
			_input = input;
			_output = output;
		}

		public DcmModify(string mask, DicomModifyOp op, string input, string output) {
			_mask = mask;
			_op = op;
			_input = input;
			_output = output;
		}
		#endregion

		#region Public Properties
		public string DicomMask {
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
			DcmTagMask mask = DcmTagMask.Parse(_mask);
			if (mask == null)
				return;
			foreach (DcmTag tag in dataset.GetMaskedTags(mask))
				Apply(dataset, tag);
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
			if (modifiers == null)
				modifiers = new List<DcmModify>();
			StringWriter writer = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(List<DcmModify>));
			serializer.Serialize(writer, modifiers);
			return writer.ToString();
		}

		public static List<DcmModify> DeserializeList(string data) {
			if (String.IsNullOrEmpty(data))
				return new List<DcmModify>();
			StringReader reader = new StringReader(data);
			XmlSerializer serializer = new XmlSerializer(typeof(List<DcmModify>));
			return (List<DcmModify>)serializer.Deserialize(reader);
		}

		public static IList<DcmTag> GetTags(IList<DcmModify> modifiers, DcmDataset dataset) {
			List<DcmTag> tags = new List<DcmTag>();
			foreach (DcmModify m in modifiers) {
				DcmTagMask mask = DcmTagMask.Parse(m.DicomMask);
				if (mask == null)
					continue;

				foreach (DcmTag tag in dataset.GetMaskedTags(mask)) {
					if (!tags.Contains(tag))
						tags.Add(tag);
				}
			}
			return tags;
		}

		public static void RunAll(IList<DcmModify> modifiers, DcmDataset dataset) {
			foreach (DcmModify modify in modifiers) {
				modify.Modify(dataset);
			}
		}

		public override string ToString() {
			bool enableInput = true;
			bool enableOutput = true;

			switch (_op) {
				case DicomModifyOp.Remove:
					enableInput = false;
					enableOutput = false;
					break;
				case DicomModifyOp.AddOrReplace:
					enableInput = false;
					break;
				case DicomModifyOp.Prefix:
					enableInput = false;
					break;
				case DicomModifyOp.Append:
					enableInput = false;
					break;
				case DicomModifyOp.Format:
					enableInput = false;
					break;
				case DicomModifyOp.Chomp:
					enableInput = false;
					break;
				default:
					break;
			}

			DcmTagMask mask = DcmTagMask.Parse(_mask);
			if (mask == null)
				return "Invalid DICOM Modify Mask";

			DcmTag tag = mask.Tag;
			if (tag == null)
				return "Invalid DICOM Modify Tag";

			string name = String.Empty;
			if (mask.IsFullMask)
				name = String.Format("{0} {1}", tag, tag.Entry.Name);
			else
				name = String.Format("[{0}] User Mask", _mask);

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("'{0}' {1}", name, _op);
			if (enableInput && enableOutput)
				sb.AppendFormat(" '{0}' -> '{1}'", _input, _output);
			else if (enableInput)
				sb.AppendFormat(" '{0}'", _input);
			else if (enableOutput)
				sb.AppendFormat(" '{0}'", _output);
			return sb.ToString();
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

				if (_op == DicomModifyOp.AddOrReplace) {
					dataset.AddElementWithValueString(tag, _output);
					return;
				}

				if (_op == DicomModifyOp.GenerateID) {
					dataset.AddElementWithValueString(tag, GenerateID());
					return;
				}

				if (elem == null)
					return;

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

				if (_op == DicomModifyOp.Prefix) {
					elem.SetValueString(_input + value);
					return;
				}

				if (_op == DicomModifyOp.Append) {
					elem.SetValueString(value + _input);
					return;
				}

				if (_op == DicomModifyOp.Format) {
					elem.SetValueString(String.Format(_output, value));
					return;
				}

				if (_op == DicomModifyOp.Chomp) {
					if (value.StartsWith(_output))
						elem.SetValueString(value.Substring(_output.Length));
					return;
				}

				if (_op == DicomModifyOp.Truncate) {
					int length = int.Parse(_input);
					string[] parts = value.Split('\\');
					for (int i = 0; i < parts.Length; i++) {
						if (parts[i].Length > length)
							parts[i] = parts[i].Substring(0, length);
					}
					elem.SetValueString(String.Join("\\", parts));
					return;
				}
			}
			catch (Exception e) {
				Dicom.Debug.Log.Error("Unable to modify dataset: {0}\n\trule: {1}", e.Message, ToString());
			}
		}

		private static object gidlck = new object();
		private static uint lastgid = 0;
		public static string GenerateID() {
			lock (gidlck) {
				DateTime y2k = new DateTime(2000, 01, 01);
				TimeSpan day = DateTime.Now.Subtract(y2k);
				TimeSpan min = DateTime.Now.TimeOfDay;
				String sid = String.Format("{0:0000}{1:00000}", (int)day.TotalDays, (int)min.TotalSeconds);
				uint uid = uint.Parse(sid);
				if (uid <= lastgid) uid = lastgid + 1;
				lastgid = uid;
				return String.Format("{0:000000000}", uid);
			}
		}
		#endregion
	}
}
