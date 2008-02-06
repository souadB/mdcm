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

using Dicom.Data;

namespace Dicom.Network.Server {
	public delegate DcmStatus DcmCStoreCallback(CStoreService client, byte presentationID, ushort messageID, DcmUID affectedInstance, 
										DcmPriority priority, string moveAE, ushort moveMessageID, DcmDataset dataset, string fileName);
	public delegate DcmAssociateResult DcmAssociationCallback(CStoreService client, DcmAssociate association);

	public class CStoreService : DcmServiceBase {
		public DcmCStoreCallback OnCStoreRequest;
		public DcmAssociationCallback OnAssociationRequest;

		public CStoreService() : base() {
			UseFileBuffer = true;
			LogID = "C-Store SCP";
		}

		protected override void OnReceiveAssociateRequest(DcmAssociate association) {
			LogID = association.CallingAE;
			Log.Info("{0} <- Association request:\n{1}", LogID, association.ToString());
			if (OnAssociationRequest != null) {
				DcmAssociateResult result = OnAssociationRequest(this, association);
				if (result == DcmAssociateResult.RejectCalledAE) {
					Log.Info("{0} -> Association reject: Rejected Called AE [{1}]", LogID, association.CalledAE);
					SendAssociateReject(DcmRejectResult.Permanent, DcmRejectSource.ServiceUser, DcmRejectReason.CalledAENotRecognized);
					return;
				}
				else if (result == DcmAssociateResult.RejectCallingAE) {
					Log.Info("{0} -> Association reject: Rejected Calling AE [{1}]", LogID, association.CallingAE);
					SendAssociateReject(DcmRejectResult.Permanent, DcmRejectSource.ServiceUser, DcmRejectReason.CallingAENotRecognized);
					return;
				}
				else {
					foreach (DcmPresContext pc in association.GetPresentationContexts()) {
						if (pc.Result == DcmPresContextResult.Proposed)
							pc.SetResult(DcmPresContextResult.RejectNoReason);
					}
				}
			}
			else {
				DcmAssociateProfile profile = DcmAssociateProfile.Find(association, true);
				profile.Apply(association);
			}
			Log.Info("{0} -> Association accept:\n{1}", LogID, association.ToString());
			SendAssociateAccept(association);
		}

		protected override void OnReceiveCEchoRequest(byte presentationID, ushort messageID, DcmPriority priority) {
			Log.Info("{0} <- C-Echo request [pc: {1}; id: {2}]", LogID, presentationID, messageID);
			Log.Info("{0} -> C-Echo response [{1}]: {2}", LogID, messageID, DcmStatus.Success);
			SendCEchoResponse(presentationID, messageID, DcmStatus.Success);
		}

		protected override void OnReceiveCStoreRequest(byte presentationID, ushort messageID, DcmUID affectedInstance, 
			DcmPriority priority, string moveAE, ushort moveMessageID, DcmDataset dataset, string fileName)
		{
			if (fileName != null)
				Log.Info("{0} <- C-Store request [pc: {1}; id: {2}] (stream)", LogID, presentationID, messageID);
			else
				Log.Info("{0} <- C-Store request [pc: {1}; id: {2}]", LogID, presentationID, messageID);

			DcmStatus status = DcmStatus.Success;

			if (OnCStoreRequest != null)
				status = OnCStoreRequest(this, presentationID, messageID, affectedInstance, priority, moveAE, moveMessageID, dataset, fileName);

			Log.Info("{0} -> C-Store response [{1}]: {2}", LogID, messageID, status);
			SendCStoreResponse(presentationID, messageID, affectedInstance, status);
		}
	}
}
