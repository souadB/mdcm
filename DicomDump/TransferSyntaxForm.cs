using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Dicom;
using Dicom.Data;

namespace Dicom.Forms {
	public partial class TransferSyntaxForm : Form {
		private string[] TransferSyntaxDescriptions;
		private DcmTS[] TransferSyntaxes;

		private DcmTS _selectedSyntax = null;
		private int _selectedQualityRate = 0;

		public TransferSyntaxForm() {
			InitializeComponent();

			int i = 0;
			TransferSyntaxDescriptions = new string[9];
			TransferSyntaxDescriptions[i++] = "Automatic";
			TransferSyntaxDescriptions[i++] = "JPEG 2000 Lossless";
			TransferSyntaxDescriptions[i++] = "JPEG Lossless P14 SV1";
			TransferSyntaxDescriptions[i++] = "JPEG Lossless P14";
			TransferSyntaxDescriptions[i++] = "RLE Lossless";
			TransferSyntaxDescriptions[i++] = "Deflated Explicit VR Little Endian";
			TransferSyntaxDescriptions[i++] = "Explicit VR Little Endian";
			TransferSyntaxDescriptions[i++] = "Implicit VR Little Endian";
			TransferSyntaxDescriptions[i++] = "Explicit VR Big Endian";

			i = 0;
			TransferSyntaxes = new DcmTS[9];
			TransferSyntaxes[i++] = null;
			TransferSyntaxes[i++] = DcmTS.JPEG2000Lossless;
			TransferSyntaxes[i++] = DcmTS.JPEGProcess14SV1;
			TransferSyntaxes[i++] = DcmTS.JPEGProcess14;
			TransferSyntaxes[i++] = DcmTS.RLELossless;
			TransferSyntaxes[i++] = DcmTS.DeflatedExplicitVRLittleEndian;
			TransferSyntaxes[i++] = DcmTS.ExplicitVRLittleEndian;
			TransferSyntaxes[i++] = DcmTS.ImplicitVRLittleEndian;
			TransferSyntaxes[i++] = DcmTS.ExplicitVRBigEndian;

			foreach (string tx in TransferSyntaxDescriptions) {
				cbTransferSyntax.Items.Add(tx);
			}
			cbTransferSyntax.SelectedIndex = 0;
		}

		public DcmTS SelectedTransferSyntax {
			get { return _selectedSyntax; }
		}

		private void OnSelectTransferSyntax(object sender, EventArgs e) {
			_selectedSyntax = TransferSyntaxes[cbTransferSyntax.SelectedIndex];
		}

		private void OnChangeQualityRate(object sender, EventArgs e) {
			_selectedQualityRate = (int)nuQualityRate.Value;
		}
	}
}
