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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Dicom.Data;
using Dicom.IO;

namespace Dicom.Network {
	public enum DcmQueryRetrieveLevel {
		Patient,
		Study,
		Series,
		Instance,
		Worklist
	}

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

	public class DcmDimseProgress {
		public readonly DateTime Started = DateTime.Now;
		public int BytesTransfered { get; internal set; }
		public int EstimatedCommandLength { get; internal set; }
		public int EstimatedDatasetLength { get; internal set; }

		public int EstimatedBytesTotal {
			get { return EstimatedCommandLength + EstimatedDatasetLength; }
		}

		public TimeSpan TimeElapsed {
			get { return DateTime.Now.Subtract(Started); }
		}
	}

	internal class DcmDimseInfo {
		public DcmDataset Command;
		public DcmDataset Dataset;
		public ChunkStream CommandData;
		public ChunkStream DatasetData;
		public DicomStreamReader CommandReader;
		public DicomStreamReader DatasetReader;
		public DcmDimseProgress Progress;
		public string DatasetFile;
		public FileStream DatasetFileStream;
		public Stream DatasetStream;
		public bool IsNewDimse;

		public DcmDimseInfo() {
			Progress = new DcmDimseProgress();
			IsNewDimse = true;
		}

		public void Close() {
			CloseCommand();
			CloseDataset();
		}

		public void CloseCommand() {
			CommandData = null;
			CommandReader = null;
		}

		public void CloseDataset() {
			DatasetStream = null;
			DatasetData = null;
			DatasetReader = null;
			if (DatasetFileStream != null) {
				DatasetFileStream.Dispose();
				DatasetFileStream = null;
			}
		}

		public void Abort() {
			Close();
			if (DatasetFile != null)
				if (File.Exists(DatasetFile))
					File.Delete(DatasetFile);
		}
	}

	public abstract class DcmNetworkBase {
		#region Protected Members
		private ushort _messageId;
		private Stream _network;
		private DcmSocket _socket;
		private DcmAssociate _assoc;
		private DcmDimseInfo _dimse;
		private Thread _thread;
		private bool _stop;
		private int _dimseTimeout;
		private bool _disableTimeout;
		private bool _isRunning;
		private bool _useFileBuffer;
		#endregion

		#region Public Constructors
		public DcmNetworkBase() {
			_messageId = 1;
			_dimseTimeout = 180;
			_isRunning = false;
			_useFileBuffer = true;
		}
		#endregion

		#region Public Properties
		public DcmAssociate Associate {
			get { return _assoc; }
		}

		public int DimseTimeout {
			get { return _dimseTimeout; }
			set { _dimseTimeout = value; }
		}

		public DcmSocket Socket {
			get { return _socket; }
		}

		protected Stream InternalStream {
			get { return _network; }
		}

		public bool IsClosed {
			get { return !_isRunning; }
		}

		public bool UseFileBuffer {
			get { return _useFileBuffer; }
			set { _useFileBuffer = value; }
		}
		#endregion

		#region Protected Methods
		protected void InitializeNetwork(DcmSocket socket) {
			_socket = socket;
			_network = _socket.GetStream();
			_stop = false;
			_isRunning = true;
			_thread = new Thread(new ThreadStart(Process));
			_thread.IsBackground = true;
			_thread.Start();
		}

		protected void ShutdownNetwork() {
			_stop = true;
			if (_thread != null) {
				if (Thread.CurrentThread.ManagedThreadId != _thread.ManagedThreadId)
					_thread.Join();
				_thread = null;	
			}
		}

		protected virtual void OnConnected() {
		}

		protected virtual void OnConnectionClosed() {
		}

		protected virtual void OnNetworkError(Exception e) {
		}

		protected virtual void OnDimseTimeout() {
		}

		protected virtual void OnReceiveAbort(DcmAbortSource source, DcmAbortReason reason) {
			throw new NotImplementedException();
		}

		protected virtual void OnReceiveAssociateRequest(DcmAssociate association) {
			throw new NotImplementedException();
		}

		protected virtual void OnReceiveAssociateAccept(DcmAssociate association) {
			throw new NotImplementedException();
		}

		protected virtual void OnReceiveAssociateReject(DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason) {
			throw new NotImplementedException();
		}

		protected virtual void OnReceiveReleaseRequest() {
			throw new NotImplementedException();
		}

		protected virtual void OnReceiveReleaseResponse() {
			throw new NotImplementedException();
		}



		protected virtual void OnReceiveDimseBegin(byte pcid, DcmDataset command, DcmDataset dataset, DcmDimseProgress progress) {
		}

		protected virtual void OnReceiveDimseProgress(byte pcid, DcmDataset command, DcmDataset dataset, DcmDimseProgress progress) {
		}

		protected virtual void OnReceiveDimse(byte pcid, DcmDataset command, DcmDataset dataset, DcmDimseProgress progress) {
		}

		protected virtual void OnSendDimseBegin(byte pcid, DcmDataset command, DcmDataset dataset, DcmDimseProgress progress) {
		}

		protected virtual void OnSendDimseProgress(byte pcid, DcmDataset command, DcmDataset dataset, DcmDimseProgress progress) {
		}

		protected virtual void OnSendDimse(byte pcid, DcmDataset command, DcmDataset dataset, DcmDimseProgress progress) {
		}



		protected virtual void OnPreReceiveCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance,
			DcmPriority priority, string moveAE, ushort moveMessageID, out string fileName) {
			if (UseFileBuffer) {
				fileName = Path.GetTempFileName();
			} else {
				fileName = null;
			}
		}

		protected virtual void OnReceiveCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance, 
			DcmPriority priority, string moveAE, ushort moveMessageID, DcmDataset dataset, string fileName) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected virtual void OnPostReceiveCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance, 
			DcmDataset dataset, string fileName) {
			if (fileName != null)
				if (File.Exists(fileName))
					File.Delete(fileName);
		}

		protected virtual void OnReceiveCStoreResponse(byte presentationID, ushort messageID, DcmUID affectedInstance, DcmStatus status) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected virtual void OnReceiveCEchoRequest(byte presentationID, ushort messageID) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected virtual void OnReceiveCEchoResponse(byte presentationID, ushort messageID, DcmStatus status) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected virtual void OnReceiveCFindRequest(byte presentationID, ushort messageID, DcmDataset dataset) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected virtual void OnReceiveCFindResponse(byte presentationID, ushort messageID, DcmDataset dataset, DcmStatus status) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected virtual void OnReceiveCMoveRequest(byte presentationID, ushort messageID, DcmDataset dataset) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected virtual void OnReceiveCMoveResponse(byte presentationID, ushort messageID, DcmStatus status, 
			ushort remain, ushort complete, ushort warning, ushort failure) {
			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}


		protected ushort NextMessageID() {
			return _messageId++;
		}

		protected void SendAssociateRequest(DcmAssociate associate) {
			_assoc = associate;
			AAssociateRQ pdu = new AAssociateRQ(_assoc);
			SendRawPDU(pdu.Write());
		}

		protected void SendAssociateAccept(DcmAssociate associate) {
			AAssociateAC pdu = new AAssociateAC(_assoc);
			SendRawPDU(pdu.Write());
		}

		protected void SendAssociateReject(DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason) {
			AAssociateRJ pdu = new AAssociateRJ(result, source, reason);
			SendRawPDU(pdu.Write());
		}

		protected void SendReleaseRequest() {
			AReleaseRQ pdu = new AReleaseRQ();
			SendRawPDU(pdu.Write());
		}

		protected void SendReleaseResponse() {
			AReleaseRP pdu = new AReleaseRP();
			SendRawPDU(pdu.Write());
		}

		protected void SendAbort(DcmAbortSource source, DcmAbortReason reason) {
			AAbort pdu = new AAbort(source, reason);
			SendRawPDU(pdu.Write());
		}

		protected void SendCEchoRequest(byte presentationID, ushort messageID) {
			DcmUID affectedClass = Associate.GetAbstractSyntax(presentationID);
			DcmDataset command = CreateRequest(messageID, DcmCommandField.CEchoRequest, affectedClass, false);
			SendDimse(presentationID, command, null);
		}

		protected void SendCEchoResponse(byte presentationID, ushort messageID, DcmStatus status) {
			DcmUID affectedClass = Associate.GetAbstractSyntax(presentationID);
			DcmDataset command = CreateResponse(messageID, DcmCommandField.CEchoResponse, affectedClass, status);
			SendDimse(presentationID, command, null);
		}

		protected void SendCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance,
			DcmPriority priority, DcmDataset dataset) {
			SendCStoreRequest(presentationID, messageID, affectedInstance, priority, null, 0, dataset);
		}

		protected void SendCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance, 
			DcmPriority priority, string moveAE, ushort moveMessageID, DcmDataset dataset) {
			DcmUID affectedClass = Associate.GetAbstractSyntax(presentationID);

			DcmDataset command = CreateRequest(messageID, DcmCommandField.CStoreRequest, affectedClass, true);
			command.AddElementWithValue(DcmTags.Priority, (ushort)priority);
			command.AddElementWithValue(DcmTags.AffectedSOPInstanceUID, affectedInstance.UID);
			if (moveAE != null && moveAE != String.Empty) {
				command.AddElementWithValue(DcmTags.MoveOriginatorAE, moveAE);
				command.AddElementWithValue(DcmTags.MoveOriginatorMessageID, moveMessageID);
			}

			SendDimse(presentationID, command, dataset);
		}

		protected void SendCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance,
			DcmPriority priority, Stream datastream) {
			SendCStoreRequest(presentationID, messageID, affectedInstance, priority, null, 0, datastream);
		}

		protected void SendCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance,
			DcmPriority priority, string moveAE, ushort moveMessageID, Stream datastream) {
			DcmUID affectedClass = Associate.GetAbstractSyntax(presentationID);

			DcmDataset command = CreateRequest(messageID, DcmCommandField.CStoreRequest, affectedClass, true);
			command.AddElementWithValue(DcmTags.Priority, (ushort)priority);
			command.AddElementWithValue(DcmTags.AffectedSOPInstanceUID, affectedInstance.UID);
			if (moveAE != null && moveAE != String.Empty) {
				command.AddElementWithValue(DcmTags.MoveOriginatorAE, moveAE);
				command.AddElementWithValue(DcmTags.MoveOriginatorMessageID, moveMessageID);
			}

			SendDimseStream(presentationID, command, datastream);
		}

		protected void SendCStoreResponse(byte presentationID, ushort messageID, DcmUID affectedInstance, DcmStatus status) {
			DcmUID affectedClass = Associate.GetAbstractSyntax(presentationID);
			DcmDataset command = CreateResponse(messageID, DcmCommandField.CStoreResponse, affectedClass, status);
			command.AddElementWithValue(DcmTags.AffectedSOPInstanceUID, affectedInstance.UID);
			SendDimse(presentationID, command, null);
		}

		protected void SendCFindRequest(byte presentationID, ushort messageID, DcmDataset dataset) {
			DcmUID affectedClass = Associate.GetAbstractSyntax(presentationID);
			DcmDataset command = CreateRequest(messageID, DcmCommandField.CFindRequest, affectedClass, true);
			SendDimse(presentationID, command, dataset);
		}

		protected void SendCMoveRequest(byte presentationID, ushort messageID, string destinationAE, DcmDataset dataset) {
			DcmUID affectedClass = Associate.GetAbstractSyntax(presentationID);
			DcmDataset command = CreateRequest(messageID, DcmCommandField.CMoveRequest, affectedClass, true);
			command.AddElementWithValue(DcmTags.MoveDestination, destinationAE);
			SendDimse(presentationID, command, dataset);
		}
		#endregion

		#region Private Methods
		private DcmDataset CreateRequest(ushort messageID, DcmCommandField commandField, DcmUID affectedClass, bool hasDataset) {
			DcmDataset command = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
			command.AddElementWithValue(DcmTags.MessageID, messageID);
			command.AddElementWithValue(DcmTags.CommandField, (ushort)commandField);
			command.AddElementWithValue(DcmTags.AffectedSOPClassUID, affectedClass.UID);
			command.AddElementWithValue(DcmTags.DataSetType, hasDataset ? (ushort)0x0202 : (ushort)0x0101);
			return command;
		}

		private DcmDataset CreateResponse(ushort messageIdRespondedTo, DcmCommandField commandField, DcmUID affectedClass, DcmStatus status) {
			DcmDataset command = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
			command.AddElementWithValue(DcmTags.MessageIDRespondedTo, messageIdRespondedTo);
			command.AddElementWithValue(DcmTags.CommandField, (ushort)commandField);
			command.AddElementWithValue(DcmTags.AffectedSOPClassUID, affectedClass.UID);
			command.AddElementWithValue(DcmTags.DataSetType, (ushort)0x0101);
			command.AddElementWithValue(DcmTags.Status, status.Code);
			return command;
		}

		private void Process() {
			try {
				OnConnected();
				_disableTimeout = false;
				DateTime timeout = DateTime.Now.AddSeconds(DimseTimeout);
				while (!_stop) {
					if (_socket.Poll(500, SelectMode.SelectRead)) {
						if (_socket.Available == 0) {
							// connection closed
							break;
						}
						ProcessNextPDU();
						timeout = DateTime.Now.AddSeconds(DimseTimeout);
					} else if (_disableTimeout) {
						timeout = DateTime.Now.AddSeconds(DimseTimeout);
					} else if (DateTime.Now > timeout) {
						OnDimseTimeout();
						_stop = true;
					} else {
						Thread.Sleep(100);
					}
				}
				OnConnectionClosed();
			}
			catch (Exception e) {
				OnNetworkError(e);
				OnConnectionClosed();
			}
			finally {
				try { _network.Close(); } catch { }
				_network = null;
				try { _socket.Close(); } catch { }
				_socket = null;
				_isRunning = false;
			}
		}

		private bool ProcessNextPDU() {
			RawPDU raw = new RawPDU(_network);

			if (raw.Type == 0x04) {
				if (_dimse == null) {
					_dimse = new DcmDimseInfo();
				}
			}

			raw.ReadPDU();

			try {
				switch (raw.Type) {
				case 0x01: {
						_assoc = new DcmAssociate();
						AAssociateRQ pdu = new AAssociateRQ(_assoc);
						pdu.Read(raw);
						OnReceiveAssociateRequest(_assoc);
						return true;
					}
				case 0x02: {
						AAssociateAC pdu = new AAssociateAC(_assoc);
						pdu.Read(raw);
						OnReceiveAssociateAccept(_assoc);
						return true;
					}
				case 0x03: {
						AAssociateRJ pdu = new AAssociateRJ();
						pdu.Read(raw);
						OnReceiveAssociateReject(pdu.Result, pdu.Source, pdu.Reason);
						return true;
					}
				case 0x04: {
						PDataTF pdu = new PDataTF();
						pdu.Read(raw);
						return ProcessPDataTF(pdu);
					}
				case 0x05: {
						AReleaseRQ pdu = new AReleaseRQ();
						pdu.Read(raw);
						OnReceiveReleaseRequest();
						return true;
					}
				case 0x06: {
						AReleaseRP pdu = new AReleaseRP();
						pdu.Read(raw);
						OnReceiveReleaseResponse();
						return true;
					}
				case 0x07: {
						AAbort pdu = new AAbort();
						pdu.Read(raw);
						OnReceiveAbort(pdu.Source, pdu.Reason);
						return true;
					}
				case 0xFF: {
						return false;
					}
				default:
					throw new DcmNetworkException("Unknown PDU type");
				}
			} catch (Exception e) {
				OnNetworkError(e);
				String file = String.Format(@"{0}\Errors\{1}.pdu",
					Dicom.Debug.GetStartDirectory(), DateTime.Now.Ticks);
				Directory.CreateDirectory(Dicom.Debug.GetStartDirectory() + @"\Errors");
				raw.Save(file);
				return false;
			}
		}

		private bool ProcessPDataTF(PDataTF pdu) {
			try {
				byte pcid = 0;
				foreach (PDV pdv in pdu.PDVs) {
					pcid = pdv.PCID;
					if (pdv.IsCommand) {
						if (_dimse.CommandData == null)
							_dimse.CommandData = new ChunkStream();

						_dimse.CommandData.AddChunk(pdv.Value);

						if (_dimse.Command == null) {
							_dimse.Command = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
						}

						if (_dimse.CommandReader == null) {
							_dimse.CommandReader = new DicomStreamReader(_dimse.CommandData);
							_dimse.CommandReader.Dataset = _dimse.Command;
						}

						_dimse.CommandReader.Read(null, DicomReadOptions.Default);

						_dimse.Progress.BytesTransfered += pdv.Value.Length;
						_dimse.Progress.EstimatedCommandLength = (int)_dimse.CommandReader.BytesEstimated;

						if (pdv.IsLastFragment) {
							_dimse.CloseCommand();

							bool isLast = true;
							if (_dimse.Command.Contains(DcmTags.DataSetType)) {
								if (_dimse.Command.GetUInt16(DcmTags.DataSetType, 0x0101) != 0x0101) {
									isLast = false;

									DcmCommandField commandField = (DcmCommandField)_dimse.Command.GetUInt16(DcmTags.CommandField, 0);
									if (commandField == DcmCommandField.CStoreRequest) {
										ushort messageID = _dimse.Command.GetUInt16(DcmTags.MessageID, 1);
										DcmPriority priority = (DcmPriority)_dimse.Command.GetUInt16(DcmTags.Priority, 0);
										DcmUID affectedInstance = _dimse.Command.GetUID(DcmTags.AffectedSOPInstanceUID);
										string moveAE = _dimse.Command.GetString(DcmTags.MoveOriginatorAE, null);
										ushort moveMessageID = _dimse.Command.GetUInt16(DcmTags.MoveOriginatorMessageID, 1);
										OnPreReceiveCStoreRequest(pcid, messageID, affectedInstance, priority, 
											moveAE, moveMessageID, out _dimse.DatasetFile);

										if (_dimse.DatasetFile != null) {
											DcmPresContext pres = Associate.GetPresentationContext(pcid);

											DicomFileFormat ff = new DicomFileFormat();
											ff.FileMetaInfo.FileMetaInformationVersion = DcmFileMetaInfo.Version;
											ff.FileMetaInfo.MediaStorageSOPClassUID = pres.AbstractSyntax;
											ff.FileMetaInfo.MediaStorageSOPInstanceUID = affectedInstance;
											ff.FileMetaInfo.TransferSyntax = pres.AcceptedTransferSyntax;
											ff.FileMetaInfo.ImplementationClassUID = Implementation.ClassUID;
											ff.FileMetaInfo.ImplementationVersionName = Implementation.Version;
											ff.FileMetaInfo.SourceApplicationEntityTitle = Associate.CalledAE;
											ff.Save(_dimse.DatasetFile, DicomWriteOptions.Default);

											_dimse.DatasetFileStream = new FileStream(_dimse.DatasetFile, FileMode.Open);
											_dimse.DatasetFileStream.Seek(0, SeekOrigin.End);
											_dimse.DatasetStream = _dimse.DatasetFileStream;
										}
									}
								}
							}
							if (isLast) {
								if (_dimse.IsNewDimse)
									OnReceiveDimseBegin(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
								OnReceiveDimseProgress(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
								OnReceiveDimse(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
								ProcessDimse(pcid);
								_dimse = null;
								return true;
							}
						}
					} else {
						if (_dimse.DatasetFile != null) {
							long pos = _dimse.DatasetFileStream.Position;
							_dimse.DatasetFileStream.Seek(0, SeekOrigin.End);
							_dimse.DatasetFileStream.Write(pdv.Value, 0, pdv.Value.Length);
							_dimse.DatasetFileStream.Position = pos;
						} else {
							if (_dimse.DatasetData == null) {
								_dimse.DatasetData = new ChunkStream();
								_dimse.DatasetStream = _dimse.DatasetData;
							}
							_dimse.DatasetData.AddChunk(pdv.Value);
						}

						if (_dimse.Dataset == null) {
							DcmTS ts = _assoc.GetAcceptedTransferSyntax(pdv.PCID);
							_dimse.Dataset = new DcmDataset(ts);
						}

						if (_dimse.DatasetReader == null) {
							_dimse.DatasetReader = new DicomStreamReader(_dimse.DatasetStream);
							_dimse.DatasetReader.Dataset = _dimse.Dataset;
						}
						
						_dimse.Progress.BytesTransfered += pdv.Value.Length;

						long remaining = _dimse.DatasetReader.BytesRemaining + pdv.Value.Length;
						if (remaining >= _dimse.DatasetReader.BytesNeeded || pdv.IsLastFragment) {
							_dimse.DatasetReader.Read(null, DicomReadOptions.Default |
															DicomReadOptions.DeferLoadingLargeElements |
															DicomReadOptions.DeferLoadingPixelData);

							_dimse.Progress.EstimatedDatasetLength = (int)_dimse.DatasetReader.BytesEstimated;
						}

						if (pdv.IsLastFragment) {
							_dimse.Close();

							if (_dimse.IsNewDimse)
								OnReceiveDimseBegin(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
							OnReceiveDimseProgress(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
							OnReceiveDimse(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
							ProcessDimse(pcid);
							_dimse = null;
							return true;
						}
					}
				}

				if (_dimse.IsNewDimse) {
					OnReceiveDimseBegin(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
					_dimse.IsNewDimse = false;
				} else {
					OnReceiveDimseProgress(pcid, _dimse.Command, _dimse.Dataset, _dimse.Progress);
				}

				return true;
			} catch (Exception e) {
				_dimse.Abort();
				_dimse = null;
				Debug.Log.Error(e.ToString());
				return false;
			}
		}

		private bool ProcessDimse(byte pcid) {
			ushort messageID = _dimse.Command.GetUInt16(DcmTags.MessageID, 1);
			DcmPriority priority = (DcmPriority)_dimse.Command.GetUInt16(DcmTags.Priority, 0);
			DcmCommandField commandField = (DcmCommandField)_dimse.Command.GetUInt16(DcmTags.CommandField, 0);

			if (commandField == DcmCommandField.CStoreRequest) {
				DcmUID affectedInstance = _dimse.Command.GetUID(DcmTags.AffectedSOPInstanceUID);
				string moveAE = _dimse.Command.GetString(DcmTags.MoveOriginatorAE, null);
				ushort moveMessageID = _dimse.Command.GetUInt16(DcmTags.MoveOriginatorMessageID, 1);
				try {
					OnReceiveCStoreRequest(pcid, messageID, affectedInstance, priority, moveAE, moveMessageID, _dimse.Dataset, _dimse.DatasetFile);
				} finally {
					OnPostReceiveCStoreRequest(pcid, messageID, affectedInstance, _dimse.Dataset, _dimse.DatasetFile);
				}
				return true;
			}

			if (commandField == DcmCommandField.CStoreResponse) {
				DcmUID affectedInstance = _dimse.Command.GetUID(DcmTags.AffectedSOPInstanceUID);
				DcmStatus status = DcmStatus.Lookup(_dimse.Command.GetUInt16(DcmTags.Status, 0x0211));
				OnReceiveCStoreResponse(pcid, messageID, affectedInstance, status);
				return true;
			}

			if (commandField == DcmCommandField.CEchoRequest) {
				OnReceiveCEchoRequest(pcid, messageID);
				return true;
			}

			if (commandField == DcmCommandField.CEchoResponse) {
				DcmStatus status = DcmStatus.Lookup(_dimse.Command.GetUInt16(DcmTags.Status, 0x0211));
				OnReceiveCEchoResponse(pcid, messageID, status);
				return true;
			}

			if (commandField == DcmCommandField.CFindRequest) {
				OnReceiveCFindRequest(pcid, messageID, _dimse.Dataset);
				return true;
			}

			if (commandField == DcmCommandField.CFindResponse) {
				DcmStatus status = DcmStatus.Lookup(_dimse.Command.GetUInt16(DcmTags.Status, 0x0211));
				OnReceiveCFindResponse(pcid, messageID, _dimse.Dataset, status);
				return true;
			}

			if (commandField == DcmCommandField.CMoveRequest) {
				OnReceiveCMoveRequest(pcid, messageID, _dimse.Dataset);
				return true;
			}

			if (commandField == DcmCommandField.CMoveResponse) {
				DcmStatus status = DcmStatus.Lookup(_dimse.Command.GetUInt16(DcmTags.Status, 0x0211));
				ushort remain = _dimse.Command.GetUInt16(DcmTags.RemainingSuboperations, 0);
				ushort complete = _dimse.Command.GetUInt16(DcmTags.CompletedSuboperations, 0);
				ushort warning = _dimse.Command.GetUInt16(DcmTags.WarningSuboperations, 0);
				ushort failure = _dimse.Command.GetUInt16(DcmTags.FailedSuboperations, 0);
				OnReceiveCMoveResponse(pcid, messageID, status, remain, complete, warning, failure);
				return true;
			}

			return false;
		}

		private void SendRawPDU(RawPDU pdu) {
			try {
				_disableTimeout = true;
				pdu.WritePDU(_network);
				_disableTimeout = false;
			}
			catch (Exception e) {
				OnNetworkError(e);
			}
		}

		private bool SendDimse(byte pcid, DcmDataset command, DcmDataset dataset) {
			try {
				_disableTimeout = true;

				DcmTS ts = _assoc.GetAcceptedTransferSyntax(pcid);

				DcmDimseProgress progress = new DcmDimseProgress();

				progress.EstimatedCommandLength = (int)command.CalculateWriteLength(DcmTS.ImplicitVRLittleEndian, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

				if (dataset != null)
					progress.EstimatedDatasetLength = (int)dataset.CalculateWriteLength(ts, DicomWriteOptions.Default);

				PDataTFStream pdustream = new PDataTFStream(_network, pcid, (int)_assoc.MaximumPduLength);
				pdustream.OnPduSent += delegate() {
					progress.BytesTransfered = pdustream.BytesSent;
					OnSendDimseProgress(pcid, command, dataset, progress);
				};

				OnSendDimseBegin(pcid, command, dataset, progress);

				DicomStreamWriter dsw = new DicomStreamWriter(pdustream);
				dsw.Write(command, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

				if (dataset != null) {
					pdustream.IsCommand = false;
					dsw.Write(dataset, DicomWriteOptions.Default);
				}

				// flush last pdu
				pdustream.Flush(true);

				OnSendDimse(pcid, command, dataset, progress);

				return true;
			}
			catch (Exception e) {
				OnNetworkError(e);
				return false;
			}
			finally {
				_disableTimeout = false;
			}
		}

		private bool SendDimseStream(byte pcid, DcmDataset command, Stream datastream) {
			try {
				_disableTimeout = true;

				DcmTS ts = _assoc.GetAcceptedTransferSyntax(pcid);

				DcmDimseProgress progress = new DcmDimseProgress();

				progress.EstimatedCommandLength = (int)command.CalculateWriteLength(DcmTS.ImplicitVRLittleEndian, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

				if (datastream != null)
					progress.EstimatedDatasetLength = (int)datastream.Length - (int)datastream.Position;

				PDataTFStream pdustream = new PDataTFStream(_network, pcid, (int)_assoc.MaximumPduLength);
				pdustream.OnPduSent += delegate() {
					progress.BytesTransfered = pdustream.BytesSent;
					OnSendDimseProgress(pcid, command, null, progress);
				};

				OnSendDimseBegin(pcid, command, null, progress);

				DicomStreamWriter dsw = new DicomStreamWriter(pdustream);
				dsw.Write(command, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);
				dsw = null;

				if (datastream != null) {
					pdustream.IsCommand = false;
					pdustream.Write(datastream);
				}

				// flush last pdu
				pdustream.Flush(true);

				OnSendDimse(pcid, command, null, progress);

				return true;
			}
			catch (Exception e) {
				OnNetworkError(e);
				return false;
			}
			finally {
				_disableTimeout = false;
			}
		}
		#endregion
	}
}
