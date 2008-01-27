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

using Dicom.Network;

using NLog;

namespace Dicom.Network.Client {
	public delegate void DcmResponseCallback(byte presentationID, ushort messageID, DcmStatus status);

	public class DcmClientBase : DcmNetworkBase {
		#region Private Members
		private string _host;
		private int _port;
		private string _callingAe;
		private string _calledAe;
		private uint _maxPdu;
		private int _timeout;
		private int _throttle;
		private DcmPriority _priority;
		private ManualResetEvent _closedEvent;
		private string _logid = "SCU";
		private Logger _log;
		private bool _closedOnError;
		#endregion

		#region Public Constructors
		public DcmClientBase() : base() {
			_closedEvent = null;
			_maxPdu = 32768;
			_timeout = 10;
			_log = Dicom.Debug.Log;
			_priority = DcmPriority.High;
		}
		#endregion

		#region Public Properties
		public string Host {
			get { return _host; }
		}

		public int Port {
			get { return _port; }
		}

		public string CallingAE {
			get { return _callingAe; }
			set { _callingAe = value; }
		}

		public string CalledAE {
			get { return _calledAe; }
			set {
				_calledAe = value;
				LogID = _calledAe;
			}
		}

		public uint MaxPduSize {
			get { return _maxPdu; }
			set { _maxPdu = value; }
		}

		public int Timeout {
			get { return _timeout; }
			set { _timeout = value; }
		}

		public int ThrottleSpeed {
			get { return _throttle; }
			set { _throttle = value; }
		}

		public string LogID {
			get { return _logid; }
			set { _logid = value; }
		}

		public Logger Log {
			get { return _log; }
			set { _log = value; }
		}

		public bool ClosedOnError {
			get { return _closedOnError; }
		}

		public DcmPriority Priority {
			get { return _priority; }
			set { _priority = value; }
		}
		#endregion

		#region Public Members
		public void Connect(string host, int port, DcmSocketType type) {
			_host = host; _port = port;

			_closedOnError = false;
			_closedEvent = new ManualResetEvent(false);

			DcmSocket socket = DcmSocket.Create(type);
			socket.SendTimeout = _timeout * 1000;
			socket.ReceiveTimeout = _timeout * 1000;
			socket.ThrottleSpeed = _throttle;

			Log.Info("{0} -> Connecting to server at {1}:{2}", LogID, host, port);
			socket.Connect(host, port);

			if (type == DcmSocketType.TLS)
				Log.Info("{0} -> Authenticating SSL/TLS for server: {1}", LogID, socket.RemoteEndPoint);

			InitializeNetwork(socket);
		}

		protected void Reconnect() {
			Socket.Reconnect();
			InitializeNetwork(Socket);
		}

		public void Close() {
			InternalClose(true);
		}

		protected void InternalClose(bool fireClosedEvent) {
			ShutdownNetwork();
			if (_closedEvent != null && fireClosedEvent) {
				_closedEvent.Set();
				_closedEvent = null;
			}
			Log.Info("{0} -> Connection closed", LogID);
		}

		public bool Wait() {
			_closedEvent.WaitOne();
			return !_closedOnError;
		}
		#endregion

		#region DcmClientBase Virtual Methods
		protected virtual void OnClientConnected() {
		}
		#endregion

		#region DcmNetworkBase Overrides
		protected override void OnConnectionClosed() {
			Close();
		}

		protected override void OnNetworkError(Exception e) {
			Log.Info("{0} -> Network error: {1}", LogID, e.ToString());
			_closedOnError = true;
			Close();
		}

		protected override void OnDimseTimeout() {
			Log.Info("{0} -> DIMSE timeout", LogID);
			_closedOnError = true;
			Close();
		}

		protected override void OnReceiveAssociateReject(DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason) {
			Log.Info("{0} <- Association reject: {1} - {2} - {3}", LogID, result, source, reason);
			_closedOnError = true;
			Close();
		}

		protected override void OnReceiveAbort(DcmAbortSource source, DcmAbortReason reason) {
			Log.Info("{0} <- Association abort: {1} - {2}", LogID, source, reason);
			_closedOnError = true;
			Close();
		}

		protected override void OnReceiveReleaseResponse() {
			Log.Info("{0} <- Association release response", LogID);
			_closedOnError = false;
			Close();
		}

		protected override void OnReceiveReleaseRequest() {
			Log.Info("{0} <- Association release request", LogID);
			Log.Info("{0} -> Association release response", LogID);
			SendReleaseResponse();
			Close();
		}
		#endregion
	}
}
