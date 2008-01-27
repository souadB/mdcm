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

using Dicom.Data;
using Dicom.Network;

namespace Dicom.Network.Client {
	public class CMoveQuery {
		#region Private Members
		private DcmQueryRetrieveLevel _queryLevel;
		private string _instanceUid;
		private object _userState;
		#endregion

		#region Public Constructors
		public CMoveQuery() {
			_queryLevel = DcmQueryRetrieveLevel.Study;
		}

		public CMoveQuery(DcmQueryRetrieveLevel level) {
			_queryLevel = level;
		}

		public CMoveQuery(DcmQueryRetrieveLevel level, string instance) {
			_queryLevel = level;
			_instanceUid = instance;
		}
		#endregion

		#region Public Properties
		public DcmQueryRetrieveLevel QueryRetrieveLevel {
			get { return _queryLevel; }
			set { _queryLevel = value; }
		}

		public string InstanceUID {
			get { return _instanceUid; }
			set { _instanceUid = value; }
		}

		public object UserState {
			get { return _userState; }
			set { _userState = value; }
		}
		#endregion
	}

	public sealed class CMoveClient : DcmClientBase  {
		#region Private Members
		private string _destAe;
		private DcmUID _moveSopClass;
		private Queue<CMoveQuery> _moveQueries;
		private CMoveQuery _current;
		#endregion

		#region Public Constructor
		public CMoveClient() : base() {
			LogID = "C-Move SCU";
			CallingAE = "MOVE_SCU";
			CalledAE = "MOVE_SCP";
			_moveSopClass = DcmUIDs.StudyRootQueryRetrieveInformationModelMOVE;
			_moveQueries = new Queue<CMoveQuery>();
			_current = null;
		}
		#endregion

		#region Public Properties
		public delegate void CMoveResponseDelegate(CMoveQuery query, DcmStatus status, ushort remain, ushort complete, ushort warning, ushort failure);
		public CMoveResponseDelegate OnCMoveResponse;

		public string DestinationAE {
			get { return _destAe; }
			set { _destAe = value; }
		}

		public DcmUID MoveSopClassUID {
			get { return _moveSopClass; }
			set { _moveSopClass = value; }
		}
		#endregion

		#region Public Members
		public void AddQuery(CMoveQuery query) {
			_moveQueries.Enqueue(query);
		}

		public void AddQuery(DcmQueryRetrieveLevel level, string instance) {
			AddQuery(new CMoveQuery(level, instance));
		}
		#endregion

		#region Protected Overrides
		protected override void OnConnected() {
			DcmAssociate associate = new DcmAssociate();

			byte pcid = associate.AddPresentationContext(_moveSopClass);
			associate.AddTransferSyntax(pcid, DcmTS.ExplicitVRLittleEndian);
			associate.AddTransferSyntax(pcid, DcmTS.ImplicitVRLittleEndian);

			associate.CalledAE = CalledAE;
			associate.CallingAE = CallingAE;
			associate.MaximumPduLength = MaxPduSize;

			Log.Info("{0} -> Association request:\n{1}", LogID, associate.ToString());
			SendAssociateRequest(associate);
		}

		private void PerformQueryOrRelease() {
			if (_moveQueries.Count > 0) {
				byte pcid = Associate.FindAbstractSyntax(MoveSopClassUID);
				if (Associate.GetPresentationContextResult(pcid) == DcmPresContextResult.Accept) {
					CMoveQuery query = _moveQueries.Dequeue();
					Log.Info("{0} -> C-Move request", LogID);
					DcmDataset dataset = new DcmDataset(Associate.GetAcceptedTransferSyntax(pcid));
					switch (query.QueryRetrieveLevel) {
					case DcmQueryRetrieveLevel.Patient:
						dataset.AddElementWithValue(DcmTags.QueryRetrieveLevel, "PATIENT");
						dataset.AddElementWithValue(DcmTags.PatientID, query.InstanceUID);
						break;
					case DcmQueryRetrieveLevel.Study:
						dataset.AddElementWithValue(DcmTags.QueryRetrieveLevel, "STUDY");
						dataset.AddElementWithValue(DcmTags.StudyInstanceUID, query.InstanceUID);
						break;
					case DcmQueryRetrieveLevel.Series:
						dataset.AddElementWithValue(DcmTags.QueryRetrieveLevel, "SERIES");
						dataset.AddElementWithValue(DcmTags.SeriesInstanceUID, query.InstanceUID);
						break;
					case DcmQueryRetrieveLevel.Instance:
						dataset.AddElementWithValue(DcmTags.QueryRetrieveLevel, "INSTANCE");
						dataset.AddElementWithValue(DcmTags.SOPInstanceUID, query.InstanceUID);
						break;
					default:
						break;
					}
					_current = query;
					SendCMoveRequest(pcid, 1, DestinationAE, dataset);
				}
				else {
					Log.Info("{0} -> Presentation context rejected: {1}", LogID, Associate.GetPresentationContextResult(pcid));
					Log.Info("{0} -> Association release request", LogID);
					SendReleaseRequest();
				}
			}
			else {
				Log.Info("{0} -> Association release request", LogID);
				SendReleaseRequest();
			}
		}

		protected override void OnReceiveAssociateAccept(DcmAssociate association) {
			Log.Info("{0} <- Association accept:\n{1}", LogID, association.ToString());
			PerformQueryOrRelease();
		}

		protected override void OnReceiveCMoveResponse(byte presentationID, ushort messageID, DcmStatus status, 
			ushort remain, ushort complete, ushort warning, ushort failure) {
			Log.Info("{0} -> C-Move response: {1} [r: {2}; c: {3}; w: {4}; f: {5}]", LogID, status, remain, complete, warning, failure);
			if (OnCMoveResponse != null) {
				OnCMoveResponse(_current, status, remain, complete, warning, failure);
			}
			if (remain == 0 && status != DcmStatus.Pending) {
				PerformQueryOrRelease();
			}
		}
		#endregion
	}
}
