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

using Dicom.HL7;

using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Win32.Targets;

namespace Dicom.Utility {
	[Target("TcpLogger")]
	public class TcpLoggerTarget : TargetWithLayout {
		private static byte[] StartBlock = new byte[] { 0x0B };
		private static byte[] EndBlock = new byte[] { 0x1C, 0x0D };

		private int _port;
		private Thread _thread;
		private List<Socket> _clients;
		private object _lock = new object();

		public TcpLoggerTarget(int port) {
			_port = port;
			_clients = new List<Socket>();
			_thread = new Thread(ServerProc);
			_thread.IsBackground = true;
			_thread.Start();
			Layout = "${level:padding=1:fixedLength=True}|${message}${newline}";
		}

		protected override void Write(LogEventInfo logEvent) {
			lock (_lock) {
				string message = CompiledLayout.GetFormattedMessage(logEvent);
				byte[] bytes = Encoding.ASCII.GetBytes(message);
				for (int i = 0; i < _clients.Count; i++) {
					Socket client = _clients[i];
					try {
						client.Send(StartBlock);
						client.Send(bytes);
						client.Send(EndBlock);
					}
					catch {
						try { client.Close(); }
						catch { }
						_clients.RemoveAt(i--);
					}
				}
			}
		}

		protected void ServerProc() {
			Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			server.Bind(new IPEndPoint(IPAddress.Any, _port));
			server.Listen(5);

			List<Socket> select = new List<Socket>();

			while (true) {
				select.Clear();
				select.Add(server);
				select.AddRange(_clients);

				Socket.Select(select, null, null, -1);

				lock (_lock) {
					_clients.RemoveAll(delegate(Socket s) { return select.Contains(s); });

					if (select.Contains(server)) {
						Socket client = server.Accept();
						client.NoDelay = true;
						client.ReceiveTimeout = 2000;
						client.SendTimeout = 2000;


						_clients.Add(client);
					}
				}
			}
		}
	}

	public delegate void LogDelegate(LogLevel level, string message);

	public class TcpLoggerClient {
		private string _host;
		private int _port;
		private Thread _thread;
		private bool _stop;
		private bool _connected;

		public TcpLoggerClient(string host, int port) {
			_host = host;
			_port = port;
			OnMessage = new LogDelegate(WriteToConsole);
		}

		public bool Connected {
			get { return _connected; }
		}

		public LogDelegate OnMessage;

		public void Start() {
			if (_thread == null) {
				_stop = false;
				_thread = new Thread(ClientProc);
				_thread.Start();
			}
		}

		public void Stop() {
			if (_thread != null) {
				_stop = true;
				_thread.Join();
			}
		}

		private void WriteToConsole(LogLevel level, string msg) {
			switch (level.Name[0]) {
				case 'F':
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case 'E':
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case 'W':
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
				case 'I':
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case 'D':
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case 'T':
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
				default:
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
			}
			Console.Write(msg);
		}

		private void WriteLog(string msg) {
			if (OnMessage == null || msg == String.Empty || msg.Length < 2)
				return;

			LogLevel level = LogLevel.Off;

			switch (msg[0]) {
				case 'F':
					level = LogLevel.Fatal;
					break;
				case 'E':
					level = LogLevel.Error;
					break;
				case 'W':
					level = LogLevel.Warn;
					break;
				case 'I':
					level = LogLevel.Info;
					break;
				case 'D':
					level = LogLevel.Debug;
					break;
				case 'T':
					level = LogLevel.Trace;
					break;
				default:
					break;
			}

			OnMessage(level, msg.Substring(2));
		}

		private void ClientProc() {
			Socket socket = null;
			Stream stream = null;
			MLLP mllp = null;

			while (!_stop) {
				try {
					if (socket == null) {
						_connected = false;
						socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						socket.NoDelay = true;
						socket.ReceiveTimeout = 2000;
						socket.SendTimeout = 2000;
						socket.Connect(_host, _port);
						stream = new NetworkStream(socket);
						mllp = new MLLP(stream, false);
						_connected = true;						
					}

					if (socket.Poll(500, SelectMode.SelectRead)) {
						if (socket.Available == 0) {
							try { socket.Close(); }
							catch { }
							_connected = false;
							socket = null;
							stream = null;
							mllp = null;
						}
						else {
							WriteLog(mllp.Receive());
						}
					}
				}
				catch {
					_connected = false;
					socket = null;
					stream = null;
					mllp = null;
					Thread.Sleep(1000);
					continue;
				}
			}

			if (socket != null) {
				try { socket.Close(); }
				catch { }
			}
		}
	}

	public static class NetworkLogger {
		public static string Host = "127.0.0.1";
		public static int Port = 5555;

		public static void Initialize(bool withConsole) {
			LoggingConfiguration config = new LoggingConfiguration();

			TcpLoggerTarget target = new TcpLoggerTarget(Port);
			config.AddTarget("Network", target);

			AsyncTargetWrapper wrapper = new AsyncTargetWrapper(target);

			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, wrapper));

			if (withConsole) {
				ColoredConsoleTarget ct = new ColoredConsoleTarget();
				ct.Layout = "${message}";
				config.AddTarget("Console", ct);
				config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, ct));
			}

			LogManager.Configuration = config;
		}
	}
}
