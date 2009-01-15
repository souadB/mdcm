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
using System.Net.Sockets;
using System.Threading;

using NLog;

namespace Dicom.Network.Server {
	public enum DcmAssociateResult {
		Accept,
		RejectCalledAE,
		RejectCallingAE,
		RejectNoReason
	}

	public class DcmServiceBase : DcmNetworkBase {
		#region Protected Properties
		private bool _closeConnectionAfterRelease = true;
		public bool CloseConnectionAfterRelease {
			get { return _closeConnectionAfterRelease; }
			set { _closeConnectionAfterRelease = value; }
		}

		private bool _closedOnError = false;
		public bool ClosedOnError {
			get { return _closedOnError; }
		}

		private object _state = null;
		public object UserState {
			get { return _state; }
			set { _state = value; }
		}
		#endregion

		internal void InitializeService(DcmSocket socket) {
			InitializeNetwork(socket);
		}

		protected override void OnNetworkError(Exception e) {
			_closedOnError = true;
			Close();
		}

		protected override void OnDimseTimeout() {
			_closedOnError = true;
			Close();
		}

		protected override void OnConnected() {

		}
		
		protected override void OnConnectionClosed() {
			Close();
		}

		protected override void OnReceiveReleaseRequest() {
			SendReleaseResponse();
			if (_closeConnectionAfterRelease) {
				Thread.Sleep(1);
				Close();
			}
		}

		public void Close() {
			ShutdownNetwork();
		}
	}
}
