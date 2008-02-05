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

using Dicom.Codec;
using Dicom.IO;

namespace Dicom.Data {
	public class DicomFileFormat {
		#region Protected Members
		protected DcmFileMetaInfo _metainfo;
		protected DcmDataset _dataset;
		#endregion

		public DicomFileFormat() {
		}

		public DicomFileFormat(DcmDataset dataset) {
			_metainfo = new DcmFileMetaInfo();
			_metainfo.FileMetaInformationVersion = DcmFileMetaInfo.Version;
			_metainfo.MediaStorageSOPClassUID = dataset.GetUID(DcmTags.SOPClassUID);
			_metainfo.MediaStorageSOPInstanceUID = dataset.GetUID(DcmTags.SOPInstanceUID);
			_metainfo.TransferSyntax = dataset.InternalTransferSyntax;
			_metainfo.ImplementationClassUID = Implementation.ClassUID;
			_metainfo.ImplementationVersionName = Implementation.Version;
			_metainfo.SourceApplicationEntityTitle = "";
			_dataset = dataset;
		}

		public DcmFileMetaInfo FileMetaInfo {
			get {
				if (_metainfo == null)
					_metainfo = new DcmFileMetaInfo();
				return _metainfo;
			}
		}
		public DcmDataset Dataset {
			get { return _dataset; }
		}

		public void ChangeTransferSytnax(DcmTS ts, DcmCodecParameters parameters) {
			Dataset.ChangeTransferSyntax(ts, parameters);
			FileMetaInfo.TransferSyntax = ts;
		}

		public static DcmFileMetaInfo LoadFileMetaInfo(String file) {
			using (FileStream fs = File.OpenRead(file)) {
				fs.Seek(128, SeekOrigin.Begin);
				CheckFileHeader(fs);
				DicomStreamReader dsr = new DicomStreamReader(fs);
				DcmFileMetaInfo metainfo = new DcmFileMetaInfo();
				dsr.Dataset = metainfo;
				dsr.Read(DcmFileMetaInfo.StopTag, DicomReadOptions.Default);
				return metainfo;
			}
		}

		public void Load(String file, DicomReadOptions options) {
			Load(file, null, options);
		}

		public void Load(String file, DcmTag stopTag, DicomReadOptions options) {
			using (FileStream fs = File.OpenRead(file)) {
				fs.Seek(128, SeekOrigin.Begin);
				CheckFileHeader(fs);
				DicomStreamReader dsr = new DicomStreamReader(fs);

				_metainfo = new DcmFileMetaInfo();
				dsr.Dataset = _metainfo;
				dsr.Read(DcmFileMetaInfo.StopTag, options);
				_dataset = new DcmDataset(_metainfo.TransferSyntax);
				dsr.Dataset = _dataset;
				dsr.Read(stopTag, options);
			}
		}

		protected static void CheckFileHeader(FileStream fs) {
			if (fs.ReadByte() != (byte)'D' ||
				fs.ReadByte() != (byte)'I' ||
				fs.ReadByte() != (byte)'C' ||
				fs.ReadByte() != (byte)'M')
				throw new DcmDataException("Invalid DICOM file: " + fs.Name);
		}

		public static FileStream GetDatasetStream(String file) {
			FileStream fs = File.OpenRead(file);
			fs.Seek(128, SeekOrigin.Begin);
			CheckFileHeader(fs);
			DicomStreamReader dsr = new DicomStreamReader(fs);
			DcmFileMetaInfo metainfo = new DcmFileMetaInfo();
			dsr.Dataset = metainfo;
			if (dsr.Read(DcmFileMetaInfo.StopTag, DicomReadOptions.Default) == DicomReadStatus.Success && fs.Position < fs.Length) {
				fs.Seek(-4, SeekOrigin.Current);
				return fs;
			}
			fs.Close();
			return null;
		}

		public void Save(String file, DicomWriteOptions options) {
			string dir = Path.GetDirectoryName(file);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			using (FileStream fs = File.Create(file)) {
				fs.Seek(128, SeekOrigin.Begin);
				fs.WriteByte((byte)'D');
				fs.WriteByte((byte)'I');
				fs.WriteByte((byte)'C');
				fs.WriteByte((byte)'M');

				DicomStreamWriter dsw = new DicomStreamWriter(fs);
				dsw.Write(_metainfo, options | DicomWriteOptions.CalculateGroupLengths);
				if (_dataset != null)
					dsw.Write(_dataset, options);
			}
		}
	}
}
