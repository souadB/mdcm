using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DicomScu {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Dicom.Codec.DcmRleCodec.Register();
			Dicom.Jpeg.DcmJpegCodec.Register();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
