using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

using Dicom.Data;
using Dicom.Forms;
using Dicom.Network;
using Dicom.Network.Client;

using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Win32.Targets;

namespace DicomScu {
	public delegate void BoolDelegate(bool state);
	public delegate void MessageBoxDelegate(string message, string caption, bool isError);

	public partial class MainForm : Form {
		private ScuConfig Config = null;
		private string ConfigPath = null;

		private string[] TransferSyntaxDescriptions;
		private DcmTS[] TransferSyntaxes;

		public MainForm() {
			InitializeComponent();
			InitializeLog();

			ConfigPath = Path.Combine(Dicom.Debug.GetStartDirectory(), "dicomscu.xml");
			
			TransferSyntaxDescriptions = new string[7];
			TransferSyntaxDescriptions[0] = "Automatic";
			TransferSyntaxDescriptions[1] = "JPEG 2000 Lossless (no codec)";
			TransferSyntaxDescriptions[2] = "JPEG Lossless P14 SV1 (no codec)";
			TransferSyntaxDescriptions[3] = "RLE Lossless";
			TransferSyntaxDescriptions[4] = "Explicit VR Little Endian";
			TransferSyntaxDescriptions[5] = "Implicit VR Little Endian";
			TransferSyntaxDescriptions[6] = "Explicit VR Big Endian";

			TransferSyntaxes = new DcmTS[7];
			TransferSyntaxes[0] = null;
			TransferSyntaxes[1] = DcmTS.JPEG2000Lossless;
			TransferSyntaxes[2] = DcmTS.JPEGProcess14SV1;
			TransferSyntaxes[3] = DcmTS.RLELossless;
			TransferSyntaxes[4] = DcmTS.ExplicitVRLittleEndian;
			TransferSyntaxes[5] = DcmTS.ImplicitVRLittleEndian;
			TransferSyntaxes[6] = DcmTS.ExplicitVRBigEndian;

			foreach (string tx in TransferSyntaxDescriptions) {
				cbTransferSyntax.Items.Add(tx);
			}
			cbTransferSyntax.SelectedIndex = 0;
			
			LoadConfig();
		}

		public void ShowMessageBox(string message, string caption, bool isError) {
			MessageBox.Show(this, message, caption, MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
		}

		public void InitializeLog() {
			LoggingConfiguration config = new LoggingConfiguration();

			DicomRichTextBoxTarget target = new DicomRichTextBoxTarget();
			target.UseDefaultRowColoringRules = true;
			target.Layout = "${message}";
			target.Control = rtbLog;

			config.AddTarget("DicomRichTextBox", target);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

			LogManager.Configuration = config;
		}

		public void SaveConfig() {
			if (Config == null)
				Config = new ScuConfig();
			Config.LocalAE = tbLocalAE.Text;
			Config.RemoteAE = tbRemoteAE.Text;
			Config.RemoteHost = tbRemoteHost.Text;
			Config.RemotePort = (int)nuRemotePort.Value;
			Config.MaxPdu = (uint)nuMaxPdu.Value;
			Config.Timeout = (int)nuTimeout.Value;
			Config.TransferSyntax = cbTransferSyntax.SelectedIndex;
			Config.UseTls = cbUseTls.Checked;
			XmlSerializer serializer = new XmlSerializer(Config.GetType());
			using (FileStream fs = new FileStream(ConfigPath, FileMode.Create)) {
				try {
					serializer.Serialize(fs, Config);
					fs.Flush();
				}
				catch {
				}
			}
		}

		public void LoadConfig() {
			if (!File.Exists(ConfigPath)) {
				Config = new ScuConfig();
			}
			else {
				XmlSerializer serializer = new XmlSerializer(typeof(ScuConfig));
				using (FileStream fs = new FileStream(ConfigPath, FileMode.Open)) {
					try {
						Config = (ScuConfig)serializer.Deserialize(fs);
					}
					catch {
						Config = new ScuConfig();
					}
				}
			}
			tbLocalAE.Text = Config.LocalAE;
			tbRemoteAE.Text = Config.RemoteAE;
			tbRemoteHost.Text = Config.RemoteHost;
			nuRemotePort.Value = Config.RemotePort;
			nuMaxPdu.Value = Config.MaxPdu;
			nuTimeout.Value = Config.Timeout;
			cbTransferSyntax.SelectedIndex = Config.TransferSyntax;
			cbUseTls.Checked = Config.UseTls;
		}

		private void OnClickTest(object sender, EventArgs e) {
			ToggleEchoButtons(false);

			SaveConfig();

			ThreadPool.QueueUserWorkItem(new WaitCallback(RunDicomEcho));
		}

		private void ToggleEchoButtons(bool state) {
			bttnTest.Enabled = state;
		}

		private void RunDicomEcho(object state) {
			bool success = false;
			string msg = "Timeout or unknown failure!";
			try {
				CEchoClient scu = new CEchoClient();
				scu.CallingAE = Config.LocalAE;
				scu.CalledAE = Config.RemoteAE;
				scu.MaxPduSize = Config.MaxPdu;
				scu.SocketTimeout = 5;
				scu.DimseTimeout = 5;
				scu.OnCEchoResponse += delegate(byte presentationId, ushort messageId, DcmStatus status) {
					msg = status.ToString();
				};
				scu.Connect(Config.RemoteHost, Config.RemotePort, Config.UseTls ? DcmSocketType.TLS : DcmSocketType.TCP);
				success = scu.Wait();
			}
			catch (Exception ex) {
				msg = ex.Message;
			}

			Invoke(new MessageBoxDelegate(ShowMessageBox), msg, "DICOM C-Echo Result", !success);

			Invoke(new BoolDelegate(ToggleEchoButtons), true);
		}

		private void OnSendAddImage(object sender, EventArgs e) {
			OpenFileDialog fd = new OpenFileDialog();
			fd.Multiselect = true;
			if (fd.ShowDialog(this) == DialogResult.OK) {
				foreach (string filename in fd.FileNames) {
					try {
						CStoreInfo info = new CStoreInfo(filename);

						ListViewItem item = new ListViewItem(filename, 0);
						item.SubItems.Add(info.SOPClassUID.Description);
						item.SubItems.Add(info.TransferSyntax.UID.Description);
						item.SubItems.Add(info.Status.Description);

						item.Tag = info;
						info.UserState = item;

						lvSendImages.Items.Add(item);
					}
					catch { }
				}
			}
		}

		private void OnSendRemoveImage(object sender, EventArgs e) {
			lvSendImages.BeginUpdate();
			foreach (ListViewItem lvi in lvSendImages.SelectedItems) {
				lvSendImages.Items.Remove(lvi);
			}
			lvSendImages.EndUpdate();
		}
		
		private void OnSendClear(object sender, EventArgs e) {
			lvSendImages.Items.Clear();
		}

		private void ToggleSendButtons(bool state) {
			bttnSend.Enabled = state;
			bttnSendAddImage.Enabled = state;
			bttnSendRemoveImage.Enabled = state;
			bttnSendClear.Enabled = state;
		}

		private void UpdateSendInfo(CStoreClient client, CStoreInfo info, DcmStatus status) {
			ListViewItem lvi = (ListViewItem)info.UserState;
			lvi.ImageIndex = (info.Status == DcmStatus.Success) ? 1 : 2;
			lvi.SubItems[3].Text = info.Status.Description;
			lvSendImages.EnsureVisible(lvi.Index);
		}

		private void OnSend(object sender, EventArgs e) {
			ToggleSendButtons(false);

			SaveConfig();

			CStoreClient scu = new CStoreClient();
			scu.CallingAE = Config.LocalAE;
			scu.CalledAE = Config.RemoteAE;
			scu.MaxPduSize = Config.MaxPdu;
			scu.SocketTimeout = 5;
			scu.DimseTimeout = Config.Timeout;
			scu.SerializedPresentationContexts = true;
			scu.PreferredTransferSyntax = TransferSyntaxes[Config.TransferSyntax];
			scu.OnCStoreResponse = delegate(CStoreClient client, CStoreInfo info, DcmStatus status) {
				Invoke(new CStoreCallback(UpdateSendInfo), client, info, status);
			};

			foreach (ListViewItem lvi in lvSendImages.Items) {
				lvi.ImageIndex = 0;
				lvi.SubItems[3].Text = "Pending";

				CStoreInfo info = (CStoreInfo)lvi.Tag;
				scu.Add(info);
			}

			ThreadPool.QueueUserWorkItem(new WaitCallback(RunDicomSend), scu);
		}

		private void RunDicomSend(object state) {
			try {
				CStoreClient scu = (CStoreClient)state;
				scu.Connect(Config.RemoteHost, Config.RemotePort, Config.UseTls ? DcmSocketType.TLS : DcmSocketType.TCP);
				scu.SocketTimeout = Config.Timeout;
				scu.Wait();
			}
			catch (Exception e) {
				Invoke(new MessageBoxDelegate(ShowMessageBox), e.Message, "DICOM C-Store Error", true);
			}

			Invoke(new BoolDelegate(ToggleSendButtons), true);
		}
	}

	[Serializable]
	public class ScuConfig {
		public string LocalAE = "TEST_SCU";
		public string RemoteAE = "ANY-SCP";
		public string RemoteHost = "localhost";
		public int RemotePort = 104;
		public uint MaxPdu = 16384;
		public int Timeout = 30;
		public int TransferSyntax = 0;
		public bool UseTls = false;
	}
}
