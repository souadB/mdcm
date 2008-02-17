using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Dicom;
using Dicom.Data;
using Dicom.Network;
using Dicom.Network.Server;
using Dicom.Utility;

namespace DicomPrintScp {
	class NPrintState {
		public byte PresentationID;
		public DcmPrintJob Job;
		public DcmPrintDocument Document;
	}

	class NPrintService : DcmServiceBase {
		#region Private Members
		private DcmFilmSession _session;
		private List<DcmPrintJob> _jobs;
		#endregion

		#region Public Constructors
		public NPrintService() : base() {
			LogID = "N-Print SCP";
			_jobs = new List<DcmPrintJob>();
		}
		#endregion

		#region Protected Overrides
		protected override void OnReceiveAssociateRequest(DcmAssociate association) {
			association.NegotiateAsyncOps = false;
			LogID = association.CallingAE;
			foreach (DcmPresContext pc in association.GetPresentationContexts()) {
				if (pc.AbstractSyntax == DcmUIDs.VerificationSOPClass ||
					pc.AbstractSyntax == DcmUIDs.BasicColorPrintManagementMetaSOPClass ||
					pc.AbstractSyntax == DcmUIDs.BasicGrayscalePrintManagementMetaSOPClass ||
					pc.AbstractSyntax == DcmUIDs.PrinterSOPClass ||
					//pc.AbstractSyntax == DcmUIDs.PrinterConfigurationRetrieval ||
					//pc.AbstractSyntax == DcmUIDs.PrintJob ||
					pc.AbstractSyntax == DcmUIDs.BasicFilmSessionSOPClass ||
					pc.AbstractSyntax == DcmUIDs.BasicFilmBoxSOPClass ||
					pc.AbstractSyntax == DcmUIDs.BasicGrayscaleImageBoxSOPClass ||
					pc.AbstractSyntax == DcmUIDs.BasicColorImageBoxSOPClass) {
					pc.SetResult(DcmPresContextResult.Accept);
				} else {
					pc.SetResult(DcmPresContextResult.RejectAbstractSyntaxNotSupported);
				}
			}
			SendAssociateAccept(association);
		}

		protected override void OnReceiveCEchoRequest(byte presentationID, ushort messageID, DcmPriority priority) {
			SendCEchoResponse(presentationID, messageID, DcmStatus.Success);
		}

		protected override void OnReceiveNCreateRequest(byte presentationID, ushort messageID, 
			DcmUID affectedClass, DcmUID affectedInstance, DcmDataset dataset) {
			DcmUID sopClass = Associate.GetAbstractSyntax(presentationID);

			if (affectedClass == DcmUIDs.BasicFilmSessionSOPClass) {
				if (_session != null) {
					Log.Error("{0} -> Attempted to create second Basic Film Session on association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session = new DcmFilmSession(sopClass, affectedInstance, dataset);

				SendNCreateResponse(presentationID, messageID, affectedClass, _session.SOPInstanceUID, null, DcmStatus.Success);
				return;
			}

			if (affectedClass == DcmUIDs.BasicFilmBoxSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				DcmFilmBox box = _session.CreateFilmBox(affectedInstance, dataset);
				if (!box.Initialize()) {
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				SendNCreateResponse(presentationID, messageID, affectedClass, box.SOPInstanceUID, dataset, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNDeleteRequest(byte presentationID, ushort messageID, 
			DcmUID requestedClass, DcmUID requestedInstance) {

			if (requestedClass == DcmUIDs.BasicFilmSessionSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session = null;

				SendNDeleteResponse(presentationID, messageID, requestedClass, requestedInstance, DcmStatus.Success);
				return;
			}

			if (requestedClass == DcmUIDs.BasicFilmBoxSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session.DeleteFilmBox(requestedInstance);

				SendNDeleteResponse(presentationID, messageID, requestedClass, requestedInstance, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNSetRequest(byte presentationID, ushort messageID, 
			DcmUID requestedClass, DcmUID requestedInstance, DcmDataset dataset) {

			if (requestedClass == DcmUIDs.BasicFilmSessionSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session.Dataset.Merge(dataset);

				SendNSetResponse(presentationID, messageID, requestedClass, requestedInstance, null, DcmStatus.Success);
				return;
			}

			if (requestedClass == DcmUIDs.BasicFilmBoxSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				DcmFilmBox box = _session.FindFilmBox(requestedInstance);
				if (box == null) {
					Log.Error("{0} -> Received N-SET request for invalid film box", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				box.Dataset.Merge(dataset);

				SendNSetResponse(presentationID, messageID, requestedClass, requestedInstance, null, DcmStatus.Success);
				return;
			}

			if (requestedClass == DcmUIDs.BasicColorImageBoxSOPClass ||
				requestedClass == DcmUIDs.BasicGrayscaleImageBoxSOPClass)
			{
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				DcmImageBox box = _session.FindImageBox(requestedInstance);
				if (box == null) {
					Log.Error("{0} -> Received N-SET request for invalid image box", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				box.Dataset.Merge(dataset);

				SendNSetResponse(presentationID, messageID, requestedClass, requestedInstance, null, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNGetRequest(byte presentationID, ushort messageID, 
			DcmUID requestedClass, DcmUID requestedInstance, DcmTag[] attributes) {

			if (requestedClass == DcmUIDs.PrinterSOPClass && requestedInstance == DcmUIDs.PrinterSOPInstance) {
				DcmDataset ds = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
				ds.AddElementWithValue(DcmTags.PrinterStatus, "NORMAL");
				ds.AddElementWithValue(DcmTags.PrinterStatus, "NORMAL");
				ds.AddElementWithValue(DcmTags.PrinterName, Config.Instance.PrinterSettings.PrinterName);
				ds.AddElementWithValue(DcmTags.Manufacturer, "N/A");
				ds.AddElementWithValue(DcmTags.ManufacturersModelName, "N/A");
				ds.AddElementWithValue(DcmTags.DeviceSerialNumber, "N/A");
				ds.AddElementWithValue(DcmTags.SoftwareVersions, "N/A");
				ds.SetDateTime(DcmTags.DateOfLastCalibration, DcmTags.TimeOfLastCalibration, DateTime.Now);

				SendNGetResponse(presentationID, messageID, requestedClass, requestedInstance, ds, DcmStatus.Success);
				return;
			}

			if (requestedClass == DcmUIDs.PrintJobSOPClass) {
				DcmPrintJob job = null;

				foreach (DcmPrintJob pj in _jobs) {
					if (pj.SOPInstanceUID == requestedInstance) {
						job = pj;
						break;
					}
				}

				if (job == null) {
					job = new DcmPrintJob(requestedInstance);
					job.ExecutionStatus = "DONE";
					job.CreationDateTime = DateTime.Today;
					job.PrintPriority = _session.PrintPriority;
					job.PrinterName = Config.Instance.PrinterSettings.PrinterName;
					job.Originator = Associate.CallingAE;
				}

				SendNGetResponse(presentationID, messageID, requestedClass, requestedInstance, job.Dataset, DcmStatus.Success);
				return;
			}

			if (requestedClass == DcmUIDs.PrinterConfigurationRetrievalSOPClass && requestedInstance == DcmUIDs.PrinterConfigurationRetrievalSOPInstance) {
				DcmDataset ds = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
				DcmDataset config = new DcmDataset(DcmTS.ImplicitVRLittleEndian);


				DcmItemSequence sq = new DcmItemSequence(DcmTags.PrinterConfigurationSequence);
				sq.AddSequenceItem(config);
				ds.AddItem(sq);

				SendNGetResponse(presentationID, messageID, requestedClass, requestedInstance, ds, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNActionRequest(byte presentationID, ushort messageID, 
			DcmUID requestedClass, DcmUID requestedInstance, ushort actionTypeID, DcmDataset dataset) {

			if (_session == null) {
				Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
				SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
				return;
			}

			DcmPrintDocument document = new DcmPrintDocument(_session);

			if (requestedClass == DcmUIDs.BasicFilmSessionSOPClass && actionTypeID == 0x0001) {
				foreach (DcmFilmBox box in _session.BasicFilmBoxes)
					document.AddFilmBox(box);
			}
			
			else if (requestedClass == DcmUIDs.BasicFilmBoxSOPClass && actionTypeID == 0x0001) {
				DcmFilmBox box = _session.FindFilmBox(requestedInstance);
				if (box == null) {
					Log.Error("{0} -> Received N-ACTION request for invalid film box", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				document.AddFilmBox(box);
			}

			else {
				SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
			}

			DcmDataset result = null;

			NPrintState state = new NPrintState();
			state.PresentationID = presentationID;
			state.Document = document;

			//DcmUID jobUid = DcmUID.Generate(_session.SOPInstanceUID, 9999);
			//jobUid = DcmUID.Generate(jobUid, _jobs.Count + 1);

			//DcmPrintJob job = new DcmPrintJob(jobUid);
			//job.ExecutionStatus = "PENDING";
			//job.ExecutionStatusInfo = "QUEUED";
			//job.CreationDateTime = DateTime.Now;
			//job.PrintPriority = _session.PrintPriority;
			//job.PrinterName = Config.Instance.PrinterSettings.PrinterName;
			//job.Originator = Associate.CallingAE;

			//result = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
			//result.AddReferenceSequenceItem(DcmTags.ReferencedPrintJobSequenceRETIRED, DcmUIDs.PrintJob, job.SOPInstanceUID);

			//state.Job = job;
			//_jobs.Add(job);
			
			new Thread(PrintJobProc).Start(state);

			SendNActionResponse(presentationID, messageID, requestedClass, requestedInstance, actionTypeID, result, DcmStatus.Success);

			if (state.Job != null) {
				SendNEventReportRequest(presentationID, NextMessageID(), DcmUIDs.PrintJobSOPClass, state.Job.SOPInstanceUID, 1, state.Job.Dataset);
			}
		}

		protected override void OnReceiveNEventReportResponse(byte presentationID, ushort messageIdRespondedTo, 
			DcmUID affectedClass, DcmUID affectedInstance, ushort eventTypeID, DcmDataset dataset, DcmStatus status) {
			if (affectedClass == DcmUIDs.PrintJobSOPClass) {
				DcmPrintJob job = null;

				foreach (DcmPrintJob pj in _jobs) {
					if (pj.SOPInstanceUID == affectedInstance) {
						job = pj;
						break;
					}
				}

				if (job != null && (job.ExecutionStatus == "DONE" || job.ExecutionStatus == "FAILURE"))
					_jobs.Remove(job);

				return;
			}
		}
		#endregion

		#region Private Methods
		private void PrintJobProc(object state) {
			NPrintState print = (NPrintState)state;

			if (print.Job != null) {
				print.Job.ExecutionStatus = "PRINTING";
				print.Job.ExecutionStatusInfo = "PRINTING";
				SendNEventReportRequest(print.PresentationID, NextMessageID(), DcmUIDs.PrintJobSOPClass, print.Job.SOPInstanceUID, 2, print.Job.Dataset);
			}

			try {
				print.Document.Print();

				if (print.Job != null) {
					print.Job.ExecutionStatus = "DONE";
					print.Job.ExecutionStatusInfo = "SUCCESS";
					SendNEventReportRequest(print.PresentationID, NextMessageID(), DcmUIDs.PrintJobSOPClass, print.Job.SOPInstanceUID, 3, print.Job.Dataset);
				}
			} catch (Exception e) {
				if (print.Job != null) {
					print.Job.ExecutionStatus = "FAILURE";
					print.Job.ExecutionStatusInfo = e.Message;
					SendNEventReportRequest(print.PresentationID, NextMessageID(), DcmUIDs.PrintJobSOPClass, print.Job.SOPInstanceUID, 4, print.Job.Dataset);
				}
			}
		}
		#endregion
	}

	public class DcmPrintDocument {
		#region Private Members
		private DcmFilmSession _session;
		private List<DcmFilmBox> _filmBoxes;
		private int _current;
		private PrintPreviewDialog _previewDialog;
		#endregion

		#region Public Constructors
		public DcmPrintDocument(DcmFilmSession session) {
			_session = session;
			_filmBoxes = new List<DcmFilmBox>();
		}
		#endregion

		#region Public Properties

		#endregion

		#region Public Methods
		public void AddFilmBox(DcmFilmBox box) {
			_filmBoxes.Add(box);
		}

		public void Print() {
			_current = 0;

			PrintDocument document = new PrintDocument();
			document.PrinterSettings = (PrinterSettings)Config.Instance.PrinterSettings.Clone();
			document.PrinterSettings.Collate = true;
			document.PrinterSettings.Copies = (short)_session.NumberOfCopies;
			document.QueryPageSettings += OnQueryPageSettings;
			document.PrintPage += OnPrintPage;

			if (Config.Instance.PreviewOnly) {
				if (Application.OpenForms.Count > 0)
					Application.OpenForms[0].BeginInvoke(new WaitCallback(PreviewProc), document);
			} else {
				document.Print();
			}
		}
		#endregion

		#region Private Methods
		private void PreviewProc(object state) {
			PrintDocument document = (PrintDocument)state;

			_previewDialog = new PrintPreviewDialog();
			_previewDialog.Text = "DICOM Print Preview";
			_previewDialog.ShowInTaskbar = true;
			_previewDialog.WindowState = FormWindowState.Maximized;
			_previewDialog.Document = document;
			_previewDialog.FormClosed += delegate(object sender, FormClosedEventArgs e) {
				_previewDialog = null;
			};
			_previewDialog.Show(Application.OpenForms[0]);
			_previewDialog.BringToFront();
			_previewDialog.Focus();
		}

		private void OnQueryPageSettings(object sender, QueryPageSettingsEventArgs e) {
			DcmFilmBox filmBox = _filmBoxes[_current];

			e.PageSettings.Landscape = (filmBox.FilmOrientation == "LANDSCAPE");
		}

		private void OnPrintPage(object sender, PrintPageEventArgs e) {
			DcmFilmBox filmBox = _filmBoxes[_current];

			if (filmBox.MagnificationType == "CUBIC")
				e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			else
				e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

			// what is the correct way to calculate this???
			double resX = e.PageSettings.Bounds.Width / e.Graphics.VisibleClipBounds.Width;
			double resY = e.PageSettings.Bounds.Height / e.Graphics.VisibleClipBounds.Height;

			Rectangle bounds = new Rectangle();
			bounds.X = (int)(e.PageSettings.PrintableArea.X * resX);
			bounds.Width = (int)(e.PageSettings.PrintableArea.Width * resX) - (bounds.X * 2);
			bounds.Y = (int)(e.PageSettings.PrintableArea.Y * resY);
			bounds.Height = (int)(e.PageSettings.PrintableArea.Height * resY) - (bounds.Y * 2);

			string format = filmBox.ImageDisplayFormat;

			if (String.IsNullOrEmpty(format))
				return;

			string[] parts = format.Split('\\');

			if (parts[0] == "STANDARD" && parts.Length == 2) {
				parts = parts[1].Split(',');
				if (parts.Length == 2) {
					try {
						int rows = int.Parse(parts[0]);
						int cols = int.Parse(parts[1]);

						int rowSize = bounds.Height / rows;
						int colSize = bounds.Width / cols;

						int imageBox = 0;

						for (int r = 0; r < rows; r++) {
							for (int c = 0; c < cols; c++) {
								Point position = new Point();
								position.Y = bounds.Top + (r * rowSize);
								position.X = bounds.Left + (c * colSize);

								if (imageBox < filmBox.BasicImageBoxes.Count)
									DrawImageBox(filmBox.BasicImageBoxes[imageBox], e.Graphics, position, colSize, rowSize);

								imageBox++;
							}
						}
					}
					catch {
					}
				}

			}

			if (parts[0] == "ROW" && parts.Length == 2) {
				try {
					parts = parts[1].Split(',');

					int rows = parts.Length;
					int rowSize = bounds.Height / rows;

					int imageBox = 0;

					for (int r = 0; r < rows; r++) {
						int cols = int.Parse(parts[r]);
						int colSize = bounds.Width / cols;

						for (int c = 0; c < cols; c++) {
                            Point position = new Point();
                            position.Y = bounds.Top + (r * rowSize);
                            position.X = bounds.Left + (c * colSize);

							if (imageBox < filmBox.BasicImageBoxes.Count)
								DrawImageBox(filmBox.BasicImageBoxes[imageBox], e.Graphics, position, colSize, rowSize);

							imageBox++;
						}
					}
				}
				catch {
				}
			}

			if (parts[0] == "COL" && parts.Length == 2) {
				try {
					parts = parts[1].Split(',');

					int cols = parts.Length;
					int colSize = bounds.Width / cols;

					int imageBox = 0;

					for (int c = 0; c < cols; c++) {
						int rows = int.Parse(parts[c]);
						int rowSize = bounds.Height / rows;

						for (int r = 0; r < rows; r++) {
							Point position = new Point();
                            position.Y = bounds.Top + (r * rowSize);
                            position.X = bounds.Left + (c * colSize);

							if (imageBox < filmBox.BasicImageBoxes.Count)
								DrawImageBox(filmBox.BasicImageBoxes[imageBox], e.Graphics, position, colSize, rowSize);

							imageBox++;
						}
					}
				}
				catch {
				}
			}

			_current++;
			e.HasMorePages = _current < _filmBoxes.Count;
			if (!e.HasMorePages)
				_current = 0;
		}

		private void DrawImageBox(DcmImageBox imageBox, Graphics graphics, Point position, int width, int height) {
			DcmDataset dataset = imageBox.ImageSequence;
			if (!dataset.Contains(DcmTags.PixelData))
				return;

			DcmPixelData pixelData = new DcmPixelData(dataset);

			PinnedIntArray pixelBuffer = null;
			Bitmap bitmap = null;

			if (pixelData.SamplesPerPixel == 3) {
				pixelBuffer = new PinnedIntArray(pixelData.GetFrameDataS32(0));
			} else {
                pixelBuffer = new PinnedIntArray(pixelData.ImageWidth * pixelData.ImageHeight);
                bool invert = (pixelData.PhotometricInterpretation == "MONOCHROME1");
				if (imageBox.Polarity == "REVERSE")
					invert = !invert;

				if (pixelData.BitsAllocated == 8) {					
					byte[] pixels = pixelData.GetFrameDataU8(0);
					
					int pixel = 0;
					for (int y = 0; y < pixelData.ImageHeight; y++) {
						for (int x = 0; x < pixelData.ImageWidth; x++) {
							byte b = pixels[pixel];
							if (invert) b = (byte)(255 - b);
							pixelBuffer[pixel] = (b << 16) | (b << 8) | (b);
							pixel++;
						}
					}

					pixels = null;
				} else {
                    ushort[] pixels = pixelData.GetFrameDataU16(0);
					double scale = 256.0 / 4096.0;

                    int pixel = 0;
                    for (int y = 0; y < pixelData.ImageHeight; y++) {
                        for (int x = 0; x < pixelData.ImageWidth; x++) {
                            byte b = (byte)(pixels[pixel] * scale);
							if (invert) b = (byte)(255 - b);
                            pixelBuffer[pixel] = (b << 16) | (b << 8) | (b);
                            pixel++;
                        }
                    }

					pixels = null;
				}
			}

			bitmap = new Bitmap(pixelData.ImageWidth, pixelData.ImageHeight, 
				pixelData.ImageWidth * sizeof(int), PixelFormat.Format32bppRgb, pixelBuffer.Pointer);

			int border = 3;

			double factor = Math.Min((double)(height - (border * 2)) / (double)bitmap.Height,
									 (double)(width - (border * 2)) / (double)bitmap.Width);

			int drawWidth = (int)(bitmap.Width * factor);
			int drawHeight = (int)(bitmap.Height * factor);

			int drawX = Math.Max(position.X, position.X + ((width - drawWidth) / 2));
			int drawY = Math.Max(position.Y, position.Y + ((height - drawHeight) / 2));

			graphics.DrawImage(bitmap, drawX, drawY, drawWidth, drawHeight);
		}
		#endregion
	}
}
