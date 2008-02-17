using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using Dicom.Forms;
using Dicom.Network;
using Dicom.Network.Server;

namespace DicomPrintScp {
	public partial class MainForm : Form {
		private LogForm _log;
		private DcmServer<NPrintService> _server;
		private bool _closing;

		public MainForm() {
			InitializeComponent();
			InitializeLog();
		}

		private void InitializeLog() {
			_log = new LogForm();
			_log.Show();
			_log.Hide();
		}

		private void LoadSettings() {
			Config.Load();
			nuDicomPort.Value = Config.Instance.Port;
			tbAETitle.Text = Config.Instance.AETitle;
			nuMaxPduSize.Value = Config.Instance.MaxPduSize;
			nuSocketTo.Value = Config.Instance.SocketTimeout;
			nuDimseTo.Value = Config.Instance.DimseTimeout;
			nuThrottle.Value = Config.Instance.ThrottleSpeed;
			cbPreviewOnly.Checked = Config.Instance.PreviewOnly;
			cbAutoStart.Checked = AppUtility.IsAutoStartup("DICOM Print SCP");
		}

		private void SaveSettings() {
			Config.Instance.Port = (int)nuDicomPort.Value;
			Config.Instance.AETitle = tbAETitle.Text;
			Config.Instance.MaxPduSize = (int)nuMaxPduSize.Value;
			Config.Instance.SocketTimeout = (int)nuSocketTo.Value;
			Config.Instance.DimseTimeout = (int)nuDimseTo.Value;
			Config.Instance.ThrottleSpeed = (int)nuThrottle.Value;
			Config.Instance.PreviewOnly = cbPreviewOnly.Checked;
			Config.Save();

			AppUtility.SetAutoStartup("DICOM Print SCP", cbAutoStart.Checked);
		}

		private void EnableControls(bool enabled) {
			cbAutoStart.Enabled = enabled;
			bttnPrinterSettings.Enabled = enabled;
			cbPreviewOnly.Enabled = enabled;
			gbDicomSettings.Enabled = enabled;
		}

		private void ToggleService() {
			if (_server == null) {
				SaveSettings();

				if (Config.Instance.PrinterSettings == null) {
					MessageBox.Show(this, "Please configure your printer before starting the DICOM server.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				EnableControls(false);
				bttnStartStop.Text = "Stop";

				try {
					_server = new DcmServer<NPrintService>();
					_server.AddPort((int)nuDicomPort.Value, DcmSocketType.TCP);
					_server.Start();
					return;
				}
				catch (Exception ex) {
					MessageBox.Show(this, ex.Message, "DICOM Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			_server.Stop();
			_server = null;

			EnableControls(true);
			bttnStartStop.Text = "Start";
		}

		private void OnLoad(object sender, EventArgs e) {
			LoadSettings();

			if (Config.Instance.PrinterSettings == null)
				OnClickPrinterSettings(sender, e);

			ToggleService();

			Hide();
		}

		private void OnDicomLogClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			_log.Show();
			_log.Focus();
		}

		private void OnToggleServiceClick(object sender, EventArgs e) {
			ToggleService();
		}

		private void OnClosing(object sender, FormClosingEventArgs e) {
			if (_closing || 
				e.CloseReason == CloseReason.WindowsShutDown || 
				e.CloseReason == CloseReason.TaskManagerClosing || 
				e.CloseReason == CloseReason.ApplicationExitCall)
			{
				if (_server != null) {
					ToggleService();
				} else {
					Config.Save();
				}
			} else {
				if (_server != null) {
					e.Cancel = true;
					Hide();
				} else {
					Config.Save();
				}
			}
		}

		private void OnClickPrinterSettings(object sender, EventArgs e) {
			PrintDialog pd = new PrintDialog();
			if (Config.Instance.PrinterSettings != null)
				pd.PrinterSettings = Config.Instance.PrinterSettings;
			if (pd.ShowDialog(this) == DialogResult.OK) {
				Config.Instance.PrinterSettings = pd.PrinterSettings;
			}
		}

		private void OnClickNotifyMenuExit(object sender, EventArgs e) {
			_closing = true;
			Application.Exit();
		}

		private void OnNotifyMenuOpen(object sender, EventArgs e) {
			Show();
			Focus();
		}
	}
}
