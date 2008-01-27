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
	public sealed class CEchoClient : DcmClientBase {
		#region Public Constructor
		public CEchoClient() : base() {
			CallingAE = "ECHO_SCU";
			CalledAE = "ECHO_SCP";
		}
		#endregion

		public DcmResponseCallback OnCEchoResponse;

		#region Protected Overrides
		protected override void OnConnected() {
			DcmAssociate associate = new DcmAssociate();

			byte pcid = associate.AddPresentationContext(DcmUIDs.Verification);
			associate.AddTransferSyntax(pcid, DcmTS.ExplicitVRLittleEndian);
			associate.AddTransferSyntax(pcid, DcmTS.ImplicitVRLittleEndian);

			associate.CalledAE = CalledAE;
			associate.CallingAE = CallingAE;
			associate.MaximumPduLength = MaxPduSize;

			Log.Info("{0} -> Association request:\n{1}", LogID, associate.ToString());
			SendAssociateRequest(associate);
		}

		protected override void OnReceiveAssociateAccept(DcmAssociate association) {
			Log.Info("{0} <- Association accept:\n{1}", LogID, association.ToString());
			byte pcid = association.FindAbstractSyntax(DcmUIDs.Verification);
			Log.Info("{0} -> C-Echo request", LogID);
			SendCEchoRequest(pcid, 1);
		}

		protected override void OnReceiveCEchoResponse(byte presentationID, ushort messageID, DcmStatus status) {
			Log.Info("{0} <- C-Echo response: {1}", LogID, status);
			if (OnCEchoResponse != null)
				OnCEchoResponse(presentationID, messageID, status);
			Log.Info("{0} -> Association release request", LogID);
			SendReleaseRequest();
		}
		#endregion
	}
}
