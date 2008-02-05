using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DicomScu {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			//Dicom.Codec.ExtDicomCodec.Initialize();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
