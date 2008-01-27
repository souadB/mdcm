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

namespace Dicom.Network {
	//default ports:
	// 104, 11112 dicom
	// 2762 dicom-tls
	// 2761 dicom-iscl

	public enum DcmSocketType {
		Unknown,
		TCP,
		TLS,
		ISCL
	}

	public abstract class DcmSocket : IDisposable {
		public static DcmSocket Create(DcmSocketType type) {
			if (type == DcmSocketType.TLS)
				return new DcmTlsSocket();
			else if (type == DcmSocketType.TCP)
				return new DcmTcpSocket();
			else if (type == DcmSocketType.ISCL)
				return null;
			else
				return null;
		}

		public abstract DcmSocketType Type { get; }
		public abstract bool Blocking { get; set; }
		public abstract bool Connected { get; }
		public abstract int SendTimeout { get; set; }
		public abstract int ReceiveTimeout { get; set; }
		public abstract EndPoint LocalEndPoint { get; }
		public abstract EndPoint RemoteEndPoint { get; }
		public abstract int Available { get; }
		public abstract DcmSocket Accept();
		public abstract void Bind(EndPoint localEP);
		public abstract void Close();
		public abstract void Connect(EndPoint remoteEP);
		public abstract void Reconnect();
		public abstract void Listen(int backlog);
		public abstract bool Poll(int microSeconds, SelectMode mode);
		public abstract Stream GetInternalStream();
		protected abstract bool IsIncomingConnection { get; }

		protected static void RegisterSocket(DcmSocket socket) {
			lock (_sockets) {
				if (!_sockets.Contains(socket)) {
					_sockets.Add(socket);
				}
				_connections = _sockets.Count;
			}
		}

		protected static void UnregisterSocket(DcmSocket socket) {
			lock (_sockets) {
				_sockets.Remove(socket);
				_connections = _sockets.Count;
			}
		}

		private static List<DcmSocket> _sockets = new List<DcmSocket>();
		private static int _connections = 0;
		private static ConnectionStats _globalStats = new ConnectionStats();
		private ConnectionStats _localStats = new ConnectionStats();
		private Stream _stream = null;

		public static int Connections {
			get { return _connections; }
		}

		public static int IncomingConnections {
			get {
				lock (_sockets) {
					int con = 0;
					foreach (DcmSocket socket in _sockets) {
						if (socket.IsIncomingConnection)
							con++;
					}
					return con;
				}
			}
		}

		public static int OutgoingConnections {
			get {
				lock (_sockets) {
					int con = 0;
					foreach (DcmSocket socket in _sockets) {
						if (!socket.IsIncomingConnection)
							con++;
					}
					return con;
				}
			}
		}

		public static IEnumerable<DcmSocket> ConnectedSockets {
			get { return _sockets; }
		}

		public static ConnectionStats GlobalStats {
			get { return _globalStats; }
		}

		public ConnectionStats LocalStats {
			get { return _localStats; }
		}

		private int _throttleSpeed;

		public int ThrottleSpeed {
			get { return _throttleSpeed; }
			set {
				_throttleSpeed = value;
				if (_stream != null && _stream is ThrottleStream) {
					ThrottleStream ts = (ThrottleStream)_stream;
					ts.MaximumBytesPerSecond = _throttleSpeed;
				}
			}
		}

		public Stream GetStream() {
			if (_stream == null) {
				Stream stream = GetInternalStream();
				ConnectionMonitorStream mstream = new ConnectionMonitorStream(stream);
				mstream.AttachStats(GlobalStats);
				mstream.AttachStats(LocalStats);
				_stream = new ThrottleStream(mstream, _throttleSpeed);
			}
			return _stream;
		}

		public void Connect(string host, int port) {
			IPAddress[] addresses = Dns.GetHostAddresses(host);
			for (int i = 0; i < addresses.Length; i++) {
				if (addresses[i].AddressFamily == AddressFamily.InterNetwork) {
					Connect(new IPEndPoint(addresses[i], port));
					return;
				}
			}
			throw new Exception("Unable to resolve host!");
		}

		#region IDisposable Members

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			Close();
		}

		#endregion
	}

	#region TCP
	public class DcmTcpSocket : DcmSocket {
		public DcmTcpSocket() {
			_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_Socket.NoDelay = true;
		}

		private DcmTcpSocket(Socket socket) {
			_Socket = socket;
			_Socket.NoDelay = true;
			_Incoming = true;
			RegisterSocket(this);
		}

		public override DcmSocketType Type {
			get { return DcmSocketType.TCP; }
		}

		public override bool Blocking {
			get { return _Socket.Blocking; }
			set { _Socket.Blocking = value; }
		}

		public override bool Connected {
			get { return _Socket.Connected; }
		}

		public override int SendTimeout {
			get { return _Socket.SendTimeout; }
			set { _Socket.SendTimeout = value; }
		}

		public override int ReceiveTimeout {
			get { return _Socket.ReceiveTimeout; }
			set { _Socket.ReceiveTimeout = value; }
		}

		public override EndPoint LocalEndPoint {
			get { return _Socket.LocalEndPoint; }
		}

		public override EndPoint RemoteEndPoint {
			get { return _Socket.RemoteEndPoint; }
		}

		public override int Available {
			get { return _Socket.Available; }
		}

		public override DcmSocket Accept() {
			Socket socket = _Socket.Accept();
			return new DcmTcpSocket(socket);
		}

		public override void Bind(EndPoint localEP) {
			_Socket.Bind(localEP);
		}

		public override void Close() {
			if (_Socket != null) {
				UnregisterSocket(this);
				_Socket.Close();
				_Socket = null;
			}
		}

		public override void Connect(EndPoint remoteEP) {
			_Socket.Connect(remoteEP);
			_RemoteEP = remoteEP;
			RegisterSocket(this);
		}

		public override void Reconnect() {
			Close();
			Connect(_RemoteEP);
		}

		public override void Listen(int backlog) {
			_Socket.Listen(backlog);
		}

		public override bool Poll(int microSeconds, SelectMode mode) {
			return _Socket.Poll(microSeconds, mode);
		}

		public override Stream GetInternalStream() {
			return new NetworkStream(_Socket);
		}

		protected override bool IsIncomingConnection {
			get { return _Incoming; }
		}

		private EndPoint _RemoteEP;
		private Socket _Socket;
		private bool _Incoming;
	}
	#endregion

	#region TLS
	public class DcmTlsSocket : DcmSocket {
		public DcmTlsSocket() {
			_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_Socket.NoDelay = true;
		}

		private DcmTlsSocket(Socket socket) {
			_IsServer = true;
			_Socket = socket;
			_Socket.NoDelay = true;
			RegisterSocket(this);
		}

		public override DcmSocketType Type {
			get { return DcmSocketType.TLS; }
		}

		public override bool Blocking {
			get { return _Socket.Blocking; }
			set { _Socket.Blocking = value; }
		}

		public override bool Connected {
			get { return _Socket.Connected; }
		}

		public override int SendTimeout {
			get { return _Socket.SendTimeout; }
			set { _Socket.SendTimeout = value; }
		}

		public override int ReceiveTimeout {
			get { return _Socket.ReceiveTimeout; }
			set { _Socket.ReceiveTimeout = value; }
		}

		public override EndPoint LocalEndPoint {
			get { return _Socket.LocalEndPoint; }
		}

		public override EndPoint RemoteEndPoint {
			get { return _Socket.RemoteEndPoint; }
		}

		public override int Available {
			get { return _Socket.Available; }
		}

		public override DcmSocket Accept() {
			Socket socket = _Socket.Accept();
			return new DcmTlsSocket(socket);
		}

		public override void Bind(EndPoint localEP) {
			_Socket.Bind(localEP);
		}

		public override void Close() {
			if (_Socket != null) {
				UnregisterSocket(this);
				_Socket.Close();
				_Socket = null;
			}
		}

		public override void Connect(EndPoint remoteEP) {
			_IsServer = false;
			_Socket.Connect(remoteEP);
			_RemoteEP = remoteEP;
			RegisterSocket(this);
		}

		public override void Reconnect() {
			Close();
			Connect(_RemoteEP);
		}

		public override void Listen(int backlog) {
			_IsServer = true;
			_Socket.Listen(backlog);
		}

		public override bool Poll(int microSeconds, SelectMode mode) {
			return _Socket.Poll(microSeconds, mode);
		}

		public override Stream GetInternalStream() {
			if (_IsServer)
				return new TlsServerStream(_Socket);
			else
				return new TlsClientStream(_Socket);
		}

		protected override bool IsIncomingConnection {
			get { return _IsServer; }
		}

		private bool _IsServer;
		private EndPoint _RemoteEP;
		private Socket _Socket;
	}
	#endregion
}
