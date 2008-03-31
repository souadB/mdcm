using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using NLog;
using NLog.Targets;

namespace Dicom.Utility {
	public enum SyslogLevel {
		Emergency = 0,
		Alert = 1,
		Critical = 2,
		Error = 3,
		Warning = 4,
		Notice = 5,
		Information = 6,
		Debug = 7
	}

	public enum SyslogFacility {
		Kernel = 0,
		User = 1,
		Mail = 2,
		Daemon = 3,
		Auth = 4,
		Syslog = 5,
		Lpr = 6,
		News = 7,
		UUCP = 8,
		Cron = 9,
		Local0 = 10,
		Local1 = 11,
		Local2 = 12,
		Local3 = 13,
		Local4 = 14,
		Local5 = 15,
		Local6 = 16,
		Local7 = 17
	}

	[Target("Syslog")]
	public class SyslogTarget : TargetWithLayout {
		private int _port;
		private string _host;
		private IPEndPoint _endpoint;
		private Socket _socket;
		private SyslogFacility _facility;

		public SyslogTarget() {
			Host = "127.0.0.1";
			Port = 514;
			Facility = SyslogFacility.User;
			Layout = "${message}";
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		}

		public string Host {
			get { return _host; }
			set { _host = value; }
		}

		public int Port {
			get { return _port; }
			set { _port = value; }
		}

		public SyslogFacility Facility {
			get { return _facility; }
			set { _facility = value; }
		}

		protected override void Write(LogEventInfo logEvent) {
			if (_endpoint == null) {
				try {
					IPHostEntry entry = Dns.GetHostEntry(_host);
					for (int i = 0; i < entry.AddressList.Length; i++) {
						if (entry.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
							_endpoint = new IPEndPoint(entry.AddressList[i], _port);
							break;
						}
					}
				}
				catch {
					_endpoint = null;
				}

				if (_endpoint == null) {
					_endpoint = new IPEndPoint(IPAddress.Loopback, _port);
				}
			}

			int facility = (int)_facility;

			int level = (int)SyslogLevel.Debug;
			if (logEvent.Level == LogLevel.Info)
				level = (int)SyslogLevel.Information;
			else if (logEvent.Level == LogLevel.Warn)
				level = (int)SyslogLevel.Warning;
			else if (logEvent.Level == LogLevel.Error)
				level = (int)SyslogLevel.Error;
			else if (logEvent.Level == LogLevel.Fatal)
				level = (int)SyslogLevel.Critical;

			int priority = (facility * 8) + level;

			string message = String.Format("<{0}>{1}", priority, CompiledLayout.GetFormattedMessage(logEvent));
			byte[] buffer = Encoding.ASCII.GetBytes(message);

			try {
				_socket.SendTo(buffer, _endpoint);
			}
			catch {
			}
		}
	}
}
