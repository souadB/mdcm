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
using System.Threading;

using Dicom;
using Dicom.Codec;
using Dicom.Data;
using Dicom.Network;

namespace Dicom.Network.Client {
	internal class SyntaxParams {
		public SyntaxParams(DcmTS ts, DcmCodecParameters codecParams) {
			TransferSyntax = ts;
			Params = codecParams;
		}

		public DcmTS TransferSyntax;
		public DcmCodecParameters Params;
	}

	public class CStoreInfo {
		#region Public Constructor
		public CStoreInfo(string fileName) {
			FileName = fileName;
			LoadFields();
		}

		public CStoreInfo(string fileName, object userState) {
			FileName = fileName;
			UserState = userState;
			LoadFields();
		}
		#endregion

		#region Private Members
		private static DcmTag StopTag = new DcmTag(0x0010, 0x0041);

		private object _loadLock = new object();
		private DcmTS _transferSyntax;
		private DcmTS _originalTransferSyntax;
		private DcmDataset _dataset;
		private Exception _exception;
		private object _userState;
		private DcmStatus _status = DcmStatus.Pending;
		#endregion

		#region Public Members
		[DicomField(DcmConstTags.SOPClassUID, DefaultValue = DicomFieldDefault.Default)]
		public readonly DcmUID SOPClassUID;

		[DicomField(DcmConstTags.SOPInstanceUID, DefaultValue = DicomFieldDefault.Default)]
		public readonly DcmUID SOPInstanceUID;

		[DicomField(DcmConstTags.StudyDate, DefaultValue = DicomFieldDefault.Default)]
		public readonly DateTime StudyDate;

		[DicomField(DcmConstTags.AccessionNumber, DefaultValue = DicomFieldDefault.Default)]
		public readonly string AccessionNumber;

		[DicomField(DcmConstTags.Modality, DefaultValue = DicomFieldDefault.Default)]
		public readonly string Modality;

		[DicomField(DcmConstTags.StudyDescription, DefaultValue = DicomFieldDefault.Default)]
		public readonly string StudyDescription;

		[DicomField(DcmConstTags.StudyInstanceUID, DefaultValue = DicomFieldDefault.Default)]
		public readonly DcmUID StudyInstanceUID;

		[DicomField(DcmConstTags.SeriesInstanceUID, DefaultValue = DicomFieldDefault.Default)]
		public readonly DcmUID SeriesInstanceUID;

		[DicomField(DcmConstTags.StudyID, DefaultValue = DicomFieldDefault.Default)]
		public readonly string StudyID;

		[DicomField(DcmConstTags.PatientsName, DefaultValue = DicomFieldDefault.Default)]
		public readonly string PatientsName;
		
		[DicomField(DcmConstTags.PatientID, DefaultValue = DicomFieldDefault.Default)]
		public readonly string PatientID;

		[DicomField(DcmConstTags.PatientsBirthDate, DefaultValue = DicomFieldDefault.Default)]
		public readonly DateTime PatientsBirthDate;

		[DicomField(DcmConstTags.PatientsSex, DefaultValue = DicomFieldDefault.Default)]
		public readonly string PatientsSex;

		public readonly string FileName;

		public DcmTS TransferSyntax {
			get { return _transferSyntax; }
		}

		internal bool IsLoaded {
			get { return _dataset != null; }
		}

		public bool HasError {
			get { return _exception != null; }
		}

		public Exception Error {
			get { return _exception; }
		}

		public DcmStatus Status {
			get { return _status; }
			internal set { _status = value; }
		}

		public object UserState {
			get { return _userState; }
			set { _userState = value; }
		}
		#endregion

		#region Public Methods
		internal bool CanStream(DcmTS ts) {
			return ts == _originalTransferSyntax;
		}

		internal DcmDataset GetDataset(DcmTS ts, DcmCodecParameters codecParams) {
			lock (_loadLock) {
				if (ts != _transferSyntax)
					_dataset = null;
				if (_dataset == null)
					Preload(new SyntaxParams(ts, codecParams));
				return _dataset;
			}
		}

		internal Stream GetStream(DcmTS ts) {
			lock (_loadLock) {
				Unload();
				if (CanStream(ts))
					return DicomFileFormat.GetDatasetStream(FileName);
				else
					return null;
			}
		}

		internal object LoadLock {
			get { return _loadLock; }
		}
		#endregion

		#region Private Methods
		internal void Preload(object state) {
			SyntaxParams param = (SyntaxParams)state;
			DcmTS ts = param.TransferSyntax;
			lock (_loadLock) {
				if (ts != _transferSyntax) {
					_dataset = null;
					_exception = null;
					_transferSyntax = _originalTransferSyntax;
				}
				if (ts == _originalTransferSyntax)
					return;
				if (_dataset == null && _exception == null) {
					try {
						DicomFileFormat ff = new DicomFileFormat();
						ff.Load(FileName, DicomReadOptions.Default);
						_dataset = ff.Dataset;

						if (_dataset.InternalTransferSyntax != ts) {
							_dataset.ChangeTransferSyntax(ts, param.Params);
						}
						_transferSyntax = _dataset.InternalTransferSyntax;
					}
					catch (Exception e) {
						_dataset = null;
						_exception = e;
						_transferSyntax = _originalTransferSyntax;
					}
				}
			}
		}

		internal void Unload() {
			lock (_loadLock) {
				_dataset = null;
				_transferSyntax = _originalTransferSyntax;
			}
		}

		private void LoadFields() {
			lock (_loadLock) {
				try {
					DicomFileFormat ff = new DicomFileFormat();
					ff.Load(FileName, StopTag, DicomReadOptions.Default);
					ff.Dataset.LoadDicomFields(this);
					_transferSyntax = ff.Dataset.InternalTransferSyntax;
					_originalTransferSyntax = _transferSyntax;
				}
				catch (Exception e) {
					_exception = e;
				}
			}
		}
		#endregion
	}

	public delegate void CStoreCallback(CStoreClient client, CStoreInfo info, DcmStatus status);

	public sealed class CStoreClient : DcmClientBase {
		#region Private Members
		private List<CStoreInfo> _images = new List<CStoreInfo>();
		private DcmTS _preferredTransferSyntax;
		private DcmCodecParameters _preferedSyntaxParams;
		private bool _serialPresContexts;
		private int _linger = 0;
		private Dictionary<DcmUID, List<DcmTS>> _presContextMap = new Dictionary<DcmUID, List<DcmTS>>();
		private object _lock = new object();
		private ManualResetEvent _mreStore = new ManualResetEvent(false);
		private bool _cancel = false;
		#endregion

		#region Public Constructor
		public CStoreClient() : base() {
			CallingAE = "STORE_SCU";
			CalledAE = "STORE_SCP";
		}
		#endregion

		#region Public Members
		public CStoreCallback OnCStoreResponse;

		public DcmTS PreferredTransferSyntax {
			get { return _preferredTransferSyntax; }
			set { _preferredTransferSyntax = value; }
		}

		public DcmCodecParameters PreferredSyntaxParams {
			get { return _preferedSyntaxParams; }
			set { _preferedSyntaxParams = value; }
		}

		public bool SerializedPresentationContexts {
			get { return _serialPresContexts; }
			set { _serialPresContexts = value; }
		}

		public int Linger {
			get { return _linger; }
			set { _linger = value; }
		}

		public void Add(string FileName) {
			if (!File.Exists(FileName))
				return;
			CStoreInfo info = new CStoreInfo(FileName);
			Add(info);
		}

		public void Add(CStoreInfo info) {
			lock (_lock) {
				foreach (CStoreInfo nfo in _images) {
					if (nfo.FileName == info.FileName)
						return;
				}
				_images.Add(info);
				if (!_presContextMap.ContainsKey(info.SOPClassUID)) {
					_presContextMap.Add(info.SOPClassUID, new List<DcmTS>());
				}
				if (!_presContextMap[info.SOPClassUID].Contains(info.TransferSyntax)) {
					_presContextMap[info.SOPClassUID].Add(info.TransferSyntax);
				}
			}
		}

		public void Cancel(bool wait) {
			_cancel = true;
			if (wait && !IsClosed)
				Wait();
		}
		#endregion

		#region Protected Overrides
		protected override void OnConnected() {
			if (_images.Count > 0) {
				DcmAssociate associate = new DcmAssociate();

				foreach (DcmUID uid in _presContextMap.Keys) {
					if (_preferredTransferSyntax != null) {
						if (!_presContextMap[uid].Contains(_preferredTransferSyntax))
							_presContextMap[uid].Remove(_preferredTransferSyntax);
						_presContextMap[uid].Insert(0, _preferredTransferSyntax);
					}
					if (!_presContextMap[uid].Contains(DcmTS.ImplicitVRLittleEndian))
						_presContextMap[uid].Add(DcmTS.ImplicitVRLittleEndian);
				}

				if (SerializedPresentationContexts) {
					lock (_lock) {
						foreach (DcmUID uid in _presContextMap.Keys) {
							foreach (DcmTS ts in _presContextMap[uid]) {
								byte pcid = associate.AddPresentationContext(uid);
								associate.AddTransferSyntax(pcid, ts);
							}
						}
					}
				} else {
					lock (_lock) {
						foreach (DcmUID uid in _presContextMap.Keys) {
							byte pcid = associate.AddOrGetPresentationContext(uid);
							foreach (DcmTS ts in _presContextMap[uid]) {
								associate.AddTransferSyntax(pcid, ts);
							}
						}
					}
				}				

				associate.CalledAE = CalledAE;
				associate.CallingAE = CallingAE;
				associate.MaximumPduLength = MaxPduSize;

				SendAssociateRequest(associate);
			} else {
				Close();
			}
		}

		protected override void OnConnectionClosed() {
			if (!ClosedOnError) {
				if (_images.Count > 0)
					Reconnect();
			}
		}

		protected override void OnReceiveAssociateAccept(DcmAssociate association) {
			SendNextCStoreRequest();
		}

		protected override void OnReceiveReleaseResponse() {
			InternalClose(_images.Count == 0);
		}

		protected override void OnReceiveCStoreResponse(byte presentationID, ushort messageIdRespondedTo, DcmUID affectedInstance, DcmStatus status) {
			_images[0].Status = status;
			if (OnCStoreResponse != null)
				OnCStoreResponse(this, _images[0], status);
			lock (_lock)
				_images.RemoveAt(0);
			SendNextCStoreRequest();
		}

		private void SendNextCStoreRequest() {
			DateTime linger = DateTime.Now.AddSeconds(Linger + 1);
			while (linger > DateTime.Now) {
				while (_images.Count > 0 && !_cancel) {
					CStoreInfo info = _images[0];
					if (Associate.FindAbstractSyntax(info.SOPClassUID) == 0) {
						//cycle association
						info.Unload();
						SendReleaseRequest();
						return;
					} else {
						List<DcmTS> tried = new List<DcmTS>();
						DcmTS tx = SelectBestTransferSyntax(info);
						while (tx != null) {
							byte pcid = Associate.FindAbstractSyntaxWithTransferSyntax(info.SOPClassUID, tx);
							if (pcid == 0)
								continue;
							DcmPresContextResult result = Associate.GetPresentationContextResult(pcid);
							if (result == DcmPresContextResult.Accept) {
								DcmCodecParameters codecParams = null;
								if (tx == _preferredTransferSyntax)
									codecParams = _preferedSyntaxParams;
								info.Preload(new SyntaxParams(tx, codecParams));
								PreloadNextCStoreRequest();
								ushort messageID = NextMessageID();
								if (info.CanStream(tx)) {
									Stream stream = info.GetStream(tx);
									SendCStoreRequest(pcid, messageID, info.SOPInstanceUID, Priority, stream);
									return;
								} else {
									codecParams = null;
									if (tx == _preferredTransferSyntax)
										codecParams = _preferedSyntaxParams;
									DcmDataset ds = info.GetDataset(tx, codecParams);
									if (ds != null) {
										SendCStoreRequest(pcid, messageID, info.SOPInstanceUID, Priority, ds);
										return;
									} else {
										Log.Error("{0} -> C-Store unable to transcode image:\n\tclass: {1}\n\told: {2}\n\tnew: {3}\n\treason: {4}\n\tcodecs: {5} - {6}", 
											LogID, info.SOPClassUID.Description, info.TransferSyntax, tx,
#if DEBUG
											(info.Error == null) ? "Unknown" : info.Error.ToString(),
#else
 											(info.Error == null) ? "Unknown" : info.Error.Message,
#endif
											DicomCodec.HasCodec(info.TransferSyntax), DicomCodec.HasCodec(tx));
									}
								}								
							}
							info.Unload();
							tried.Add(tx);
							tx = SelectAlternateTransferSyntax(info, tried);
						}
					}
					Log.Info("{0} -> C-Store request failed: No acceptable presentation context for {1}", LogID, info.SOPClassUID.Description);
					info.Status = DcmStatus.SOPClassNotSupported;
					if (OnCStoreResponse != null)
						OnCStoreResponse(this, info, DcmStatus.SOPClassNotSupported);
					lock (_lock)
						_images.RemoveAt(0);
					linger = DateTime.Now.AddSeconds(Linger + 1);
				}
				Thread.Sleep(500);
			}
			SendReleaseRequest();
		}

		private DcmTS SelectBestTransferSyntax(CStoreInfo info) {
			lock (info.LoadLock) {
				if (_preferredTransferSyntax != null) {
					byte pcid = Associate.FindAbstractSyntaxWithTransferSyntax(info.SOPClassUID, _preferredTransferSyntax);
					if (pcid != 0) {
						if (_preferredTransferSyntax.IsEncapsulated && !info.TransferSyntax.IsEncapsulated)
							return _preferredTransferSyntax;

						if (!_preferredTransferSyntax.IsEncapsulated)
							return _preferredTransferSyntax;
					}
				}

				if (info.TransferSyntax.IsEncapsulated) {
					byte pcid = Associate.FindAbstractSyntaxWithTransferSyntax(info.SOPClassUID, info.TransferSyntax);
					if (pcid != 0 && Associate.GetPresentationContextResult(pcid) == DcmPresContextResult.Accept)
						return info.TransferSyntax;
				}
							
				foreach (DcmTS tx in _presContextMap[info.SOPClassUID]) {
					if (info.TransferSyntax.IsEncapsulated && tx.IsEncapsulated)
						continue;
					byte pcid = Associate.FindAbstractSyntaxWithTransferSyntax(info.SOPClassUID, tx);
					if (pcid != 0 && Associate.GetPresentationContextResult(pcid) == DcmPresContextResult.Accept)
						return tx;
				}
			}

			return null;
		}

		private DcmTS SelectAlternateTransferSyntax(CStoreInfo info, List<DcmTS> tried) {
			lock (info.LoadLock) {
				foreach (DcmTS tx in _presContextMap[info.SOPClassUID]) {
					if (tried.Contains(tx))
						continue;
					if (info.TransferSyntax != tx && info.TransferSyntax.IsEncapsulated && tx.IsEncapsulated)
						continue;
					byte pcid = Associate.FindAbstractSyntaxWithTransferSyntax(info.SOPClassUID, tx);
					if (pcid != 0 && Associate.GetPresentationContextResult(pcid) == DcmPresContextResult.Accept)
						return tx;
				}
			}

			return null;
		}

		private void PreloadNextCStoreRequest() {
			lock (_lock) {
				if (_images.Count > 1) {
					CStoreInfo info = _images[1];
					DcmTS tx = SelectBestTransferSyntax(info);
					if (tx == null)
						return;
					if (!info.CanStream(tx)) {
						DcmCodecParameters codecParams = null;
						if (tx == _preferredTransferSyntax)
							codecParams = _preferedSyntaxParams;
						ThreadPool.QueueUserWorkItem(info.Preload, new SyntaxParams(tx, codecParams));
					}
				}
			}
		}
		#endregion
	}
}
