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
		RejectCallingAE
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

		private int _timeout = 30;
		public int Timeout {
			get { return _timeout; }
			set { _timeout = value; }
		}

		private string _logid = "SCP";
		public string LogID {
			get { return _logid; }
			set { _logid = value; }
		}

		private Logger _log = Debug.Log;
		public Logger Log {
			get { return _log; }
			set { _log = value; }
		}

		private object _state = null;
		public object UserState {
			get { return _state; }
			set { _state = value; }
		}
		#endregion

		internal void InitializeService(DcmSocket socket) {
			socket.SendTimeout = Timeout * 1000;
			socket.ReceiveTimeout = Timeout * 1000;
			InitializeNetwork(socket);
		}

		protected override void OnNetworkError(Exception e) {
			if (e != null)
				Log.Error("{0} -> Network error: {1}", LogID, e.ToString());
			else
				Log.Error("{0} -> Unknown network error", LogID);
			_closedOnError = true;
			Close();
		}

		protected override void OnDimseTimeout() {
			Log.Error("{0} -> DIMSE Timeout after {1} seconds", LogID, DimseTimeout);
			_closedOnError = true;
			Close();
		}

		protected override void OnConnected() {

		}
		
		protected override void OnConnectionClosed() {
			Close();
		}

		protected override void OnReceiveReleaseRequest() {
			Log.Info("{0} <- Associate release request", LogID);
			Log.Info("{0} -> Associate release response", LogID);
			SendReleaseResponse();
			if (_closeConnectionAfterRelease) {
				Thread.Sleep(1);
				Close();
			}
		}

		public void Close() {
			ShutdownNetwork();
			Log.Info("{0} <- Connection closed", LogID);
		}
	}
}
