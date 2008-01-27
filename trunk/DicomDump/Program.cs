using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Dicom.Data;
using Dicom.Forms;

namespace DicomDump {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//Dicom.Codec.ExtDicomCodec.Initialize();

			if (File.Exists("dicom.dic"))
				DcmDictionary.ImportDictionary("dicom.dic");
			else
				DcmDictionary.LoadInternalDictionary();

			if (File.Exists("private.dic"))
				DcmDictionary.ImportDictionary("private.dic");

			DicomDumpForm form = new DicomDumpForm();
			foreach (string file in args) {
				form.AddFile(file);
			}

			Application.Run(form);
		}
	}
}
