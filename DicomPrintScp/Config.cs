using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace DicomPrintScp {
	[Serializable]
	public class Config {
		public static Config Instance;
		private static string ConfigPath = Path.Combine(Dicom.Debug.GetStartDirectory(), "printscp.cfg");

		public string AETitle;
		public int Port;
		public int MaxPduSize;
		public int SocketTimeout;
		public int DimseTimeout;
		public int ThrottleSpeed;
		public PrinterSettings PrinterSettings;
		public bool PreviewOnly;

		public Config() {
			AETitle = "PRINT_SCP";
			Port = 104;
			MaxPduSize = 65536;
			SocketTimeout = 30;
			DimseTimeout = 180;
			ThrottleSpeed = 0;
			PrinterSettings = null;
			PreviewOnly = false;

			PrintDocument document = new PrintDocument();
			PrinterSettings = (PrinterSettings)document.PrinterSettings.Clone();
		}

		public static Config Load() {
			if (!File.Exists(ConfigPath)) {
				Instance = new Config();
				return Instance;
			}

			try {
				BinaryFormatter serializer = new BinaryFormatter();
				using (FileStream fs = new FileStream(ConfigPath, FileMode.Open)) {
					Instance = (Config)serializer.Deserialize(fs);
				}
			}
			catch (Exception e) {
				Dicom.Debug.Log.Error(e.Message);
				Instance = new Config();
			}

			//try {
			//    XmlSerializer serializer = new XmlSerializer(typeof(Config));
			//    using (FileStream fs = new FileStream(ConfigPath, FileMode.Open)) {
			//        Instance = (Config)serializer.Deserialize(fs);
			//    }
			//}
			//catch (Exception e) {
			//    Dicom.Debug.Log.Error(e.Message);
			//    Instance = new Config();
			//}

			return Instance;
		}

		public static void Save() {
			if (Instance == null)
				Instance = new Config();

			try {
				BinaryFormatter serializer = new BinaryFormatter();
				using (FileStream fs = File.Create(ConfigPath)) {
					serializer.Serialize(fs, Instance);
					fs.Flush();
				}
			}
			catch (Exception e) {
				Dicom.Debug.Log.Error(e.Message);
			}

			//try {
			//    XmlSerializer serializer = new XmlSerializer(typeof(Config));
			//    using (FileStream fs = File.Create(ConfigPath)) {
			//        serializer.Serialize(fs, Instance);
			//        fs.Flush();
			//    }
			//}
			//catch (Exception e) {
			//    Dicom.Debug.Log.Error(e.Message);
			//}
		}
	}
}
