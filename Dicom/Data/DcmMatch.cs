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

using Dicom.Utility;

namespace Dicom.Data {
	public enum DicomMatchOp {
		Equals,
		NotEquals,
		StartsWith,
		EndsWith,
		Contains,
		OneOf,
		Wildcard,
		Regex,
		Exists,
		NotExists
	}

	[Serializable]
	public class DcmMatch {
		#region Private Members
		private string _tag;
		private DicomMatchOp _op;
		private string _value;
		#endregion

		#region Public Constructors
		public DcmMatch() {
		}

		public DcmMatch(DcmTag tag, DicomMatchOp op, string value) {
			_tag = tag.ToString();
			_op = op;
			_value = value;
		}
		#endregion

		#region Public Properties
		public string DicomTag {
			get { return _tag; }
			set { _tag = value; }
		}

		public DicomMatchOp Operator {
			get { return _op; }
			set { _op = value; }
		}

		public string Value {
			get { return _value; }
			set { _value = value; }
		}
		#endregion

		#region Public Methods
		public bool Match(DcmDataset ds) {
			DcmTag tag = DcmTag.Parse(_tag);
			DcmElement elem = ds.GetElement(tag);

			if (_op == DicomMatchOp.Exists && elem == null)
				return false;

			if (_op == DicomMatchOp.NotExists && elem != null)
				return false;

			if (elem == null)
				return false;

			string value = elem.GetValueString();

			switch (_op) {
			case DicomMatchOp.Equals:
				return value == _value;
			case DicomMatchOp.NotEquals:
				return value != _value;
			case DicomMatchOp.StartsWith:
				return value.StartsWith(_value);
			case DicomMatchOp.EndsWith:
				return value.EndsWith(_value);
			case DicomMatchOp.Contains:
				return value.Contains(_value);
			case DicomMatchOp.Wildcard:
				return Wildcard.Match(_value, value);
			case DicomMatchOp.Regex:
				return Regex.IsMatch(_value, value);
			default:
				break;
			}

			if (_op == DicomMatchOp.OneOf) {
				string[] values = value.Split('\\');
				foreach (string v in values) {
					if (v == _value)
						return true;
				}
			}

			return false;
		}

		public override string ToString() {
			DcmTag tag = DcmTag.Parse(_tag);
			if (tag == null)
				return "Invalid DICOM Match Rule";
			return string.Format("'{0} {1}' {2} '{3}'", tag, tag.Entry.Name, _op, _value);
		}

		public static string Serialize(DcmMatch match) {
			StringWriter writer = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(DcmMatch));
			serializer.Serialize(writer, match);
			return writer.ToString();
		}

		public static DcmMatch Deserialize(string data) {
			if (String.IsNullOrEmpty(data))
				return null;
			StringReader reader = new StringReader(data);
			XmlSerializer serializer = new XmlSerializer(typeof(DcmMatch));
			return (DcmMatch)serializer.Deserialize(reader);
		}

		public static string SerializeList(List<DcmMatch> matches) {
			if (matches == null)
				matches = new List<DcmMatch>();
			StringWriter writer = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(List<DcmMatch>));
			serializer.Serialize(writer, matches);
			return writer.ToString();
		}

		public static List<DcmMatch> DeserializeList(string data) {
			if (String.IsNullOrEmpty(data))
				return new List<DcmMatch>();
			StringReader reader = new StringReader(data);
			XmlSerializer serializer = new XmlSerializer(typeof(List<DcmMatch>));
			return (List<DcmMatch>)serializer.Deserialize(reader);
		}

		public static bool MatchAll(IList<DcmMatch> matches, DcmDataset dataset) {
			foreach (DcmMatch match in matches) {
				if (!match.Match(dataset)) {
					return false;
				}
			}
			return true;
		}
		#endregion
	}
}
