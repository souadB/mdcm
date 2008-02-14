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
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

using Dicom.Data;

namespace DicomPrintScp {
	/// <summary>Print Job</summary>
	public class DcmPrintJob {
		#region Private Members
		private DcmUID _sopInst;
		private DcmDataset _dataset;
		#endregion

		#region Public Constructors
		public DcmPrintJob(DcmUID sopInst) {
			_sopInst = sopInst;
			_dataset = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
		}
		#endregion

		#region Public Properties
		/// <summary>Print Job SOP</summary>
		public static readonly DcmUID SOPClassUID = DcmUIDs.PrintJob;

		/// <summary>SOP Instance UID</summary>
		public DcmUID SOPInstanceUID {
			get { return _sopInst; }
		}

		/// <summary>Execution status of print job.</summary>
		/// <remarks>Enumerated Values:
		/// <list type="bullet">
		/// <item><description>PENDING</description></item>
		/// <item><description>PRINTING</description></item>
		/// <item><description>DONE</description></item>
		/// <item><description>FAILURE</description></item>
		/// </list>
		/// </remarks>
		public string ExecutionStatus {
			get { return _dataset.GetString(DcmTags.ExecutionStatus, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.ExecutionStatus, value); }
		}

		/// <summary>Additional information about Execution Status (2100,0020).</summary>
		public string ExecutionStatusInfo {
			get { return _dataset.GetString(DcmTags.ExecutionStatusInfo, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.ExecutionStatusInfo, value); }
		}

		/// <summary>Specifies the priority of the print job.</summary>
		/// <remarks>
		/// Enumerated values:
		/// <list type="bullet">
		/// <item><description>HIGH</description></item>
		/// <item><description>MED</description></item>
		/// <item><description>LOW</description></item>
		/// </list>
		/// </remarks>
		public string PrintPriority {
			get { return _dataset.GetString(DcmTags.PrintPriority, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.PrintPriority, value); }
		}

		/// <summary>Date/Time of print job creation.</summary>
		public DateTime CreationDateTime {
			get { return _dataset.GetDateTime(DcmTags.CreationDate, DcmTags.CreationTime, DateTime.MinValue); }
			set { _dataset.SetDateTime(DcmTags.CreationDate, DcmTags.CreationTime, value); }
		}

		/// <summary>User defined name identifying the printer.</summary>
		public string PrinterName {
			get { return _dataset.GetString(DcmTags.PrinterName, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.PrinterName, value); }
		}

		/// <summary>DICOM Application Entity Title that issued the print operation.</summary>
		public string Originator {
			get { return _dataset.GetString(DcmTags.OriginatorAE, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.OriginatorAE, value); }
		}
		#endregion
	}

	/// <summary>Basic Film Session</summary>
	[Serializable]
	public class DcmFilmSession {
		#region Private Members
		private DcmUID _sessionClass;
		private DcmUID _sopInstance;
		private DcmDataset _dataset;
		private List<DcmFilmBox> _boxes;
		#endregion

		#region Public Constructors
		/// <summary>
		/// Initializes new Basic Film Session
		/// </summary>
		/// <param name="sessionClass">Color or Grayscale Basic Print Management UID</param>
		public DcmFilmSession(DcmUID sessionClass) {
			_sessionClass = sessionClass;
			_dataset = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
			_boxes = new List<DcmFilmBox>();
		}

		/// <summary>
		/// Initializes new Basic Film Session
		/// </summary>
		/// <param name="sessionClass">Color or Grayscale Basic Print Management UID</param>
		/// <param name="sopInstance">SOP Instance UID</param>
		/// <param name="dataset">Dataset</param>
		public DcmFilmSession(DcmUID sessionClass, DcmUID sopInstance, DcmDataset dataset) {
			_sessionClass = sessionClass;
			_sopInstance = sopInstance;
			_dataset = dataset;
			_boxes = new List<DcmFilmBox>();
		}
		#endregion

		#region Public Properties
		/// <summary>Basic Film Box SOP</summary>
		public static readonly DcmUID SOPClassUID = DcmUIDs.BasicFilmSession;

		/// <summary>
		/// Color or Grayscale Basic Print Management UID
		/// </summary>
		public DcmUID SessionClassUID {
			get { return _sessionClass; }
		}

		/// <summary>SOP Instance UID</summary>
		public DcmUID SOPInstanceUID {
			get { return _sopInstance; }
		}

		/// <summary>Basic Film Session data</summary>
		public DcmDataset Dataset {
			get { return _dataset; }
		}

		/// <summary>Basic Film Boxes</summary>
		public List<DcmFilmBox> BasicFilmBoxes {
			get { return _boxes; }
		}

		/// <summary>Number of copies to be printed for each film of the film session.</summary>
		public int NumberOfCopies {
			get { return _dataset.GetInt32(DcmTags.NumberOfCopies, 1); }
			set { _dataset.AddElementWithValue(DcmTags.NumberOfCopies, value); }
		}

		/// <summary>Specifies the priority of the print job.</summary>
		/// <remarks>
		/// Enumerated values:
		/// <list type="bullet">
		/// <item><description>HIGH</description></item>
		/// <item><description>MED</description></item>
		/// <item><description>LOW</description></item>
		/// </list>
		/// </remarks>
		public string PrintPriority {
			get { return _dataset.GetString(DcmTags.PrintPriority, null); }
			set { _dataset.AddElementWithValue(DcmTags.PrintPriority, value); }
		}

		/// <summary>Type of medium on which the print job will be printed.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item><description>PAPER</description></item>
		/// <item><description>CLEAR FILM</description></item>
		/// <item><description>BLUE FILM</description></item>
		/// <item><description>MAMMO CLEAR FILM</description></item>
		/// <item><description>MAMMO BLUE FILM</description></item>
		/// </list>
		/// </remarks>
		public string MediumType {
			get { return _dataset.GetString(DcmTags.MediumType, null); }
			set { _dataset.AddElementWithValue(DcmTags.MediumType, value); }
		}

		/// <summary>Film destination.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item>
		///   <term>MAGAZINE</term>
		///   <description>the exposed film is stored in film magazine</description>
		/// </item>
		/// <item>
		///   <term>PROCESSOR</term>
		///   <description>the exposed film is developed in film processor</description>
		/// </item>
		/// <item>
		///   <term>BIN_i</term>
		///   <description>the exposed film is deposited in a sorter bin where “I” represents the bin 
		///   number. Film sorter BINs shall be numbered sequentially starting from one and no maxium 
		///   is placed on the number of BINs. The encoding of the BIN number shall not contain leading
		///   zeros.</description>
		/// </item>
		/// </remarks>
		public string FilmDestination {
			get { return _dataset.GetString(DcmTags.FilmDestination, null); }
			set { _dataset.AddElementWithValue(DcmTags.FilmDestination, value); }
		}

		/// <summary>Human readable label that identifies the film session.</summary>
		public string FilmSessionLabel {
			get { return _dataset.GetString(DcmTags.FilmSessionLabel, null); }
			set { _dataset.AddElementWithValue(DcmTags.FilmSessionLabel, value); }
		}

		/// <summary>Amount of memory allocated for the film session.</summary>
		/// <remarks>Value is expressed in KB.</remarks>
		public int MemoryAllocation {
			get { return _dataset.GetInt32(DcmTags.MemoryAllocation, 0); }
			set { _dataset.AddElementWithValue(DcmTags.MemoryAllocation, value); }
		}

		/// <summary>Identification of the owner of the film session.</summary>
		public string OwnerID {
			get { return _dataset.GetString(DcmTags.OwnerID, null); }
			set { _dataset.AddElementWithValue(DcmTags.OwnerID, value); }
		}
		#endregion

		#region Public Methods
		public DcmFilmBox CreateFilmBox(DcmDataset dataset) {
			DcmUID uid = DcmUID.Generate(SOPInstanceUID, _boxes.Count + 1);
			DcmFilmBox box = new DcmFilmBox(this, uid, dataset);
			_boxes.Add(box);
			return box;
		}

		public void DeleteFilmBox(DcmUID instUid) {
			for (int i = 0; i < _boxes.Count; i++) {
				if (_boxes[i].SOPInstanceUID.UID == instUid.UID) {
					_boxes.RemoveAt(i);
					return;
				}
			}
		}

		public DcmFilmBox FindFilmBox(DcmUID instUid) {
			foreach (DcmFilmBox box in _boxes) {
				if (box.SOPInstanceUID.UID == instUid.UID)
					return box;
			}
			return null;
		}

		public DcmImageBox FindImageBox(DcmUID instUid) {
			foreach (DcmFilmBox filmBox in _boxes) {
				DcmImageBox imageBox = filmBox.FindImageBox(instUid);
				if (imageBox != null)
					return imageBox;
			}
			return null;
		}

		public DcmFilmSession Clone() {
			return new DcmFilmSession(SessionClassUID, SOPInstanceUID, Dataset.Clone());
		}
		#endregion
	}

	/// <summary>Basic Film Box</summary>
	public class DcmFilmBox {
		#region Private Members
		private DcmFilmSession _session;
		private DcmUID _sopInstance;
		private DcmDataset _dataset;
		private List<DcmImageBox> _boxes;
		#endregion

		#region Public Constructors
		/// <summary>
		/// Initializes new Basic Film Box
		/// </summary>
		/// <param name="session">Basic Film Session</param>
		/// <param name="sopInstance">SOP Instance UID</param>
		public DcmFilmBox(DcmFilmSession session, DcmUID sopInstance) {
			_session = session;
			_sopInstance = sopInstance;
			_dataset = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
			_boxes = new List<DcmImageBox>();
		}

		/// <summary>
		/// Initializes new Basic Film Box
		/// </summary>
		/// <param name="session">Basic Film Session</param>
		/// <param name="sopInstance">SOP Instance UID</param>
		/// <param name="dataset">Dataset</param>
		public DcmFilmBox(DcmFilmSession session, DcmUID sopInstance, DcmDataset dataset) {
			_session = session;
			_sopInstance = sopInstance;
			_dataset = dataset;
			_boxes = new List<DcmImageBox>();
		}
		#endregion

		#region Public Properties
		/// <summary>Basic Film Session SOP</summary>
		public static readonly DcmUID SOPClassUID = DcmUIDs.BasicFilmBoxSOP;

		/// <summary>SOP Instance UID</summary>
		public DcmUID SOPInstanceUID {
			get { return _sopInstance; }
		}

		/// <summary>Basic Film Session data</summary>
		public DcmDataset Dataset {
			get { return _dataset; }
		}

		/// <summary>Basic Image Boxes</summary>
		public List<DcmImageBox> BasicImageBoxes {
			get { return _boxes; }
		}

		/// <summary>Type of image display format.</summary>
		/// <remarks>
		/// Enumerated Values:
		/// <list type="bullet">
		/// <item>
		///   <term>STANDARD\C,R</term>
		///   <description>film contains equal size rectangular image boxes with R rows of image 
		///   boxes and C columns of image boxes; C and R are integers.</description>
		/// </item>
		/// <item>
		///   <term>ROW\R1,R2,R3, etc.</term>
		///   <description>film contains rows with equal size rectangular image boxes with R1 
		///   image boxes in the first row, R2 image boxes in second row, R3 image boxes in third 
		///   row, etc.; R1, R2, R3, etc. are integers.</description>
		/// </item>
		/// <item>
		///   <term>COL\C1,C2,C3, etc.</term>
		///   <description>film contains columns with equal size rectangular image boxes with C1 
		///   image boxes in the first column, C2 image boxes in second column, C3 image boxes in 
		///   third column, etc.; C1, C2, C3, etc. are integers.</description>
		/// </item>
		/// <item>
		///   <term>SLIDE</term>
		///   <description>film contains 35mm slides; the number of slides for a particular film 
		///   size is configuration dependent.</description>
		/// </item>
		/// <item>
		///   <term>SUPERSLIDE</term>
		///   <description>film contains 40mm slides; the number of slides for a particular film 
		///   size is configuration dependent.</description>
		/// </item>
		/// <item>
		///   <term>CUSTOM\i</term>
		///   <description>film contains a customized ordering of rectangular image boxes; i identifies 
		///   the image display format; the definition of the image display formats is defined in the 
		///   Conformance Statement; i is an integer.</description>
		/// </item>
		/// </list>
 		/// </remarks>
		public string ImageDisplayFormat {
			get { return _dataset.GetValueString(DcmTags.ImageDisplayFormat); }
			set { _dataset.AddElementWithValue(DcmTags.ImageDisplayFormat, value); }
		}
		
		/// <summary>Film orientation.</summary>
		/// <remarks>
		/// Enumerated Values:
		/// <list type="bullet">
		/// <item>
		///   <term>PORTRAIT</term>
		///   <description>vertical film position</description>
		/// </item>
		/// <item>
		///   <term>LANDSCAPE</term>
		///   <description>horizontal film position</description>
		/// </item>
		/// </list>
		/// </remarks>
		public string FilmOrientation {
			get { return _dataset.GetString(DcmTags.FilmOrientation, "PORTRAIT"); }
			set { _dataset.AddElementWithValue(DcmTags.FilmOrientation, value); }
		}
		
		/// <summary> Film size identification.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item><description>8INX10IN</description></item>
		/// <item><description>8_5INX11IN</description></item>
		/// <item><description>10INX12IN</description></item>
		/// <item><description>10INX14IN</description></item>
		/// <item><description>11INX14IN</description></item>
		/// <item><description>11INX17IN</description></item>
		/// <item><description>14INX14IN</description></item>
		/// <item><description>14INX17IN</description></item>
		/// <item><description>24CMX24CM</description></item>
		/// <item><description>24CMX30CM</description></item>
		/// <item><description>A4</description></item>
		/// <item><description>A3</description></item>
		/// </list>
		/// 
		/// Notes:
		/// 10INX14IN corresponds with 25.7CMX36.4CM
		/// A4 corresponds with 210 x 297 millimeters
		/// A3 corresponds with 297 x 420 millimeters
		/// </remarks>
		public string FilmSizeID {
			get { return _dataset.GetString(DcmTags.FilmSizeID, "8_5INX11IN"); }
			set { _dataset.AddElementWithValue(DcmTags.FilmSizeID, value); }
		}
		
		/// <summary>Interpolation type by which the printer magnifies or decimates the image 
		/// in order to fit the image in the image box on film.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item><description>REPLICATE</description></item>
		/// <item><description>BILINEAR</description></item>
		/// <item><description>CUBIC</description></item>
		/// <item><description>NONE</description></item>
		/// </list>
		/// </remarks>
		public string MagnificationType {
			get { return _dataset.GetString(DcmTags.MagnificationType, "BILINEAR"); }
			set { _dataset.AddElementWithValue(DcmTags.MagnificationType, value); }
		}
		
		/// <summary>Maximum density of the images on the film, expressed in hundredths of 
		/// OD. If Max Density is higher than maximum printer density than Max Density is set 
		/// to maximum printer density.</summary>
		public ushort MaxDensity {
			get { return _dataset.GetUInt16(DcmTags.MaxDensity, 0); }
			set { _dataset.AddElementWithValue(DcmTags.MaxDensity, value); }
		}
		
		/// <summary>Character string that contains either the ID of the printer configuration 
		/// table that contains a set of values for implementation specific print parameters 
		/// (e.g. perception LUT related parameters) or one or more configuration data values, 
		/// encoded as characters. If there are multiple configuration data values encoded in 
		/// the string, they shall be separated by backslashes. The definition of values shall 
		/// be contained in the SCP's Conformance Statement.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="">
		/// <item>
		///   <term>CS000-CS999</term>
		///   <description>Implementation specific curve type.</description></item>
		/// </list>
		/// 
		/// Note: It is recommended that for SCPs, CS000 represent the lowest contrast and CS999 
		/// the highest contrast levels available.
		/// </remarks>
		public string ConfigurationInformation {
			get { return _dataset.GetString(DcmTags.ConfigurationInformation, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.ConfigurationInformation, value); }
		}
		
		/// <summary>Identification of annotation display format. The definition of the annotation 
		/// display formats and the annotation box position sequence are defined in the Conformance 
		/// Statement.</summary>
		public string AnnotationDisplayFormatID {
			get { return _dataset.GetString(DcmTags.AnnotationDisplayFormatID, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.AnnotationDisplayFormatID, value); }
		}

		/// <summary>Further specifies the type of the interpolation function. Values 
		/// are defined in Conformance Statement.
		/// 
		/// Only valid for Magnification Type (2010,0060) = CUBIC</summary>
		public string SmoothingType {
			get { return _dataset.GetString(DcmTags.SmoothingType, String.Empty); }
			set { _dataset.AddElementWithValue(DcmTags.SmoothingType, value); }
		}
		
		/// <summary>Density of the film areas surrounding and between images on the film.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item><description>BLACK</description></item>
		/// <item><description>WHITE</description></item>
		/// <item><description>i where i represents the desired density in hundredths of OD 
		/// (e.g. 150 corresponds with 1.5 OD)</description></item>
		/// </list>
		/// </remarks>
		public string BorderDensity {
			get { return _dataset.GetString(DcmTags.BorderDensity, "BLACK"); }
			set { _dataset.AddElementWithValue(DcmTags.BorderDensity, value); }
		}

		/// <summary>Density of the image box area on the film that contains no image.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item><description>BLACK</description></item>
		/// <item><description>WHITE</description></item>
		/// <item><description>i where i represents the desired density in hundredths of OD 
		/// (e.g. 150 corresponds with 1.5 OD)</description></item>
		/// </list>
		/// </remarks>
		public string EmptyImageDensity {
			get { return _dataset.GetString(DcmTags.EmptyImageDensity, "BLACK"); }
			set { _dataset.AddElementWithValue(DcmTags.EmptyImageDensity, value); }
		}

		/// <summary>Minimum density of the images on the film, expressed in hundredths of 
		/// OD. If Min Density is lower than minimum printer density than Min Density is set 
		/// to minimum printer density.</summary>
		public ushort MinDensity {
			get { return _dataset.GetUInt16(DcmTags.MinDensity, 0); }
			set { _dataset.AddElementWithValue(DcmTags.MinDensity, value); }
		}
		
		/// <summary>Specifies whether a trim box shall be printed surrounding each image 
		/// on the film.</summary>
		/// <remarks>
		/// Enumerated Values:
		/// <list type="bullet">
		/// <item><description>YES</description></item>
		/// <item><description>NO</description></item>
		/// </list>
		/// </remarks>
		public string Trim {
			get { return _dataset.GetString(DcmTags.Trim, "NO"); }
			set { _dataset.AddElementWithValue(DcmTags.Trim, value); }
		}
		
		/// <summary>Luminance of lightbox illuminating a piece of transmissive film, or for 
		/// the case of reflective media, luminance obtainable from diffuse reflection of the 
		/// illumination present. Expressed as L0, in candelas per square meter (cd/m2).</summary>
		public ushort Illumination {
			get { return _dataset.GetUInt16(DcmTags.Illumination, 0); }
			set { _dataset.AddElementWithValue(DcmTags.Illumination, value); }
		}

		/// <summary>For transmissive film, luminance contribution due to reflected ambient 
		/// light. Expressed as La, in candelas per square meter (cd/m2).</summary>
		public ushort ReflectedAmbientLight {
			get { return _dataset.GetUInt16(DcmTags.ReflectedAmbientLight, 0); }
			set { _dataset.AddElementWithValue(DcmTags.ReflectedAmbientLight, value); }
		}

		/// <summary>Specifies the resolution at which images in this Film Box are to be printed.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item>
		///   <term>STANDARD</term>
		///   <description>approximately 4k x 5k printable pixels on a 14 x 17 inch film</description>
		/// </item>
		/// <item>
		///   <term>HIGH</term>
		///   <description>Approximately twice the resolution of STANDARD.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public string RequestedResolutionID {
			get { return _dataset.GetString(DcmTags.RequestedResolutionID, "STANDARD"); }
			set { _dataset.AddElementWithValue(DcmTags.RequestedResolutionID, value); }
		}
		#endregion

		#region Public Methods
		public bool Initialize() {
			_dataset.AddItem(new DcmItemSequence(DcmTags.ReferencedImageBoxSequence));
			
			// Set Defaults
			FilmOrientation = FilmOrientation;
			FilmSizeID = FilmSizeID;
			MagnificationType = MagnificationType;
			MaxDensity = MaxDensity;
			//ConfigurationInformation = ConfigurationInformation;
			//SmoothingType = SmoothingType;
			BorderDensity = BorderDensity;
			EmptyImageDensity = EmptyImageDensity;
			MinDensity = MinDensity;
			Trim = Trim;
			RequestedResolutionID = RequestedResolutionID;
			
			//_dataset.AddItem(new DcmItemSequence(DcmTags.ReferencedBasicAnnotationBoxSequence));
			//AnnotationDisplayFormatID = AnnotationDisplayFormatID;

			//_dataset.AddItem(new DcmItemSequence(DcmTags.ReferencedPresentationLUTSequence));
			//Illumination = Illumination;
			//ReflectedAmbientLight = ReflectedAmbientLight;

			string format = ImageDisplayFormat;

			if (String.IsNullOrEmpty(format)) {
				Dicom.Debug.Log.Error("No display format present in N-CREATE Basic Image Box dataset");
				return false;
			}

			string[] parts = format.Split('\\');

			if (parts[0] == "STANDARD" && parts.Length == 2) {
				parts = parts[1].Split(',');
				if (parts.Length == 2) {
					try {
						int row = int.Parse(parts[0]);
						int col = int.Parse(parts[1]);
						for (int r = 0; r < row; r++)
							for (int c = 0; c < col; c++)
								CreateImageBox();
						return true;
					}
					catch {
					}
				}
				
			}

			if ((parts[0] == "ROW" || parts[0] == "COL") && parts.Length == 2) {
				try {
					parts = parts[1].Split(',');
					foreach (string part in parts) {
						int count = int.Parse(part);
						for (int i = 0; i < count; i++)
							CreateImageBox();
					}
					return true;
				}
				catch {
				}
			}

			Dicom.Debug.Log.Error("Unsupported image display format \"{0}\"", format);
			return false;
		}

		public DcmImageBox FindImageBox(DcmUID instUid) {
			foreach (DcmImageBox box in _boxes) {
				if (box.SOPInstanceUID.UID == instUid.UID)
					return box;
			}
			return null;
		}

		public DcmFilmBox Clone() {
			DcmFilmBox box = new DcmFilmBox(_session, SOPInstanceUID, Dataset.Clone());
			foreach (DcmImageBox imageBox in BasicImageBoxes) {
				box.BasicImageBoxes.Add(imageBox.Clone());
			}
			return box;
		}
		#endregion

		#region Private Methods
		private void CreateImageBox() {
			DcmUID classUid = DcmUIDs.BasicGrayscaleImageBox;
			if (_session.SessionClassUID == DcmUIDs.BasicColorPrintManagement)
				classUid = DcmUIDs.BasicColorImageBox;

			DcmUID instUid = DcmUID.Generate(SOPInstanceUID, _boxes.Count + 1);

			DcmImageBox box = new DcmImageBox(this, classUid, instUid);
			box.ImagePosition = (ushort)(_boxes.Count + 1);
			_boxes.Add(box);

			_dataset.AddReferenceSequenceItem(DcmTags.ReferencedImageBoxSequence, classUid, instUid);
		}
		#endregion
	}

	/// <summary>Color or Grayscale Basic Image Box</summary>
	public class DcmImageBox {
		#region Private Members
		private DcmFilmBox _filmBox;
		private DcmUID _sopClass;
		private DcmUID _sopInstance;
		private DcmDataset _dataset;
		#endregion

		#region Public Constructors
		/// <summary>
		/// Initializes new Basic Image Box
		/// </summary>
		/// <param name="filmBox">Basic Film Box</param>
		/// <param name="sopClass">SOP Class UID</param>
		/// <param name="sopInstance">SOP Instance UID</param>
		public DcmImageBox(DcmFilmBox filmBox, DcmUID sopClass, DcmUID sopInstance) {
			_filmBox = filmBox;
			_sopClass = sopClass;
			_sopInstance = sopInstance;
			_dataset = new DcmDataset(DcmTS.ImplicitVRLittleEndian);
		}

		/// <summary>
		/// Initializes new Basic Image Box
		/// </summary>
		/// <param name="filmBox">Basic Film Box</param>
		/// <param name="sopClass">SOP Class UID</param>
		/// <param name="sopInstance">SOP Instance UID</param>
		/// <param name="dataset">Dataset</param>
		public DcmImageBox(DcmFilmBox filmBox, DcmUID sopClass, DcmUID sopInstance, DcmDataset dataset) {
			_filmBox = filmBox;
			_sopClass = sopClass;
			_sopInstance = sopInstance;
			_dataset = dataset;
		}
		#endregion

		#region Public Properties
		/// <summary>Basic Color Image Box SOP</summary>
		public static readonly DcmUID ColorSOPClassUID = DcmUIDs.BasicColorImageBox;

		/// <summary>Basic Grayscale Image Box SOP</summary>
		public static readonly DcmUID GraySOPClassUID = DcmUIDs.BasicGrayscaleImageBox;

		/// <summary>SOP Class UID</summary>
		public DcmUID SOPClassUID {
			get { return _sopClass; }
		}

		/// <summary>SOP Instance UID</summary>
		public DcmUID SOPInstanceUID {
			get { return _sopInstance; }
		}

		/// <summary>Basic Film Session data</summary>
		public DcmDataset Dataset {
			get { return _dataset; }
		}

		/// <summary>Color or Grayscale Basic Image Sequence</summary>
		public DcmDataset ImageSequence {
			get {
				DcmItemSequence sq = null;
				if (_sopClass == ColorSOPClassUID)
					sq = _dataset.GetSQ(DcmTags.BasicColorImageSequence);
				else
					sq = _dataset.GetSQ(DcmTags.BasicGrayscaleImageSequence);

				if (sq != null && sq.SequenceItems.Count > 0)
					return sq.SequenceItems[0].Dataset;

				return null;
			}
		}

		/// <summary>The position of the image on the film, based on Image Display 
		/// Format (2010,0010). See C.13.5.1 for specification.</summary>
		public ushort ImagePosition {
			get { return _dataset.GetUInt16(DcmTags.ImagePosition, 1); }
			set { _dataset.AddElementWithValue(DcmTags.ImagePosition, value); }
		}

		/// <summary>Specifies whether minimum pixel values (after VOI LUT transformation) 
		/// are to printed black or white.</summary>
		/// <remarks>
		/// Enumerated Values:
		/// <list type="bullet"></list>
		/// <item>
		///   <term>NORMAL</term>
		///   <description>pixels shall be printed as specified by the Photometric Interpretation (0028,0004)</description>
		/// </item>
		/// <item>
		///   <term>REVERSE</term>
		///   <description>pixels shall be printed with the opposite polarity as specified by the Photometric 
		///   Interpretation (0028,0004)</description>
		/// </item>
		/// 
		/// If Polarity (2020,0020) is not specified by the SCU, the SCP shall print with NORMAL polarity.
		/// </remarks>
		public string Polarity {
			get { return _dataset.GetString(DcmTags.Polarity, "NORMAL"); }
			set { _dataset.AddElementWithValue(DcmTags.Polarity, value); }
		}

		/// <summary>Interpolation type by which the printer magnifies or decimates the image 
		/// in order to fit the image in the image box on film.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="bullet">
		/// <item><description>REPLICATE</description></item>
		/// <item><description>BILINEAR</description></item>
		/// <item><description>CUBIC</description></item>
		/// <item><description>NONE</description></item>
		/// </list>
		/// </remarks>
		public string MagnificationType {
			get { return _dataset.GetString(DcmTags.MagnificationType, _filmBox.MagnificationType); }
			set { _dataset.AddElementWithValue(DcmTags.MagnificationType, value); }
		}

		/// <summary>Further specifies the type of the interpolation function. Values 
		/// are defined in Conformance Statement.
		/// 
		/// Only valid for Magnification Type (2010,0060) = CUBIC</summary>
		public string SmoothingType {
			get { return _dataset.GetString(DcmTags.SmoothingType, _filmBox.SmoothingType); }
			set { _dataset.AddElementWithValue(DcmTags.SmoothingType, value); }
		}

		/// <summary>Minimum density of the images on the film, expressed in hundredths of 
		/// OD. If Min Density is lower than minimum printer density than Min Density is set 
		/// to minimum printer density.</summary>
		public ushort MinDensity {
			get { return _dataset.GetUInt16(DcmTags.MinDensity, _filmBox.MinDensity); }
			set { _dataset.AddElementWithValue(DcmTags.MinDensity, value); }
		}

		/// <summary>Maximum density of the images on the film, expressed in hundredths of 
		/// OD. If Max Density is higher than maximum printer density than Max Density is set 
		/// to maximum printer density.</summary>
		public ushort MaxDensity {
			get { return _dataset.GetUInt16(DcmTags.MaxDensity, _filmBox.MaxDensity); }
			set { _dataset.AddElementWithValue(DcmTags.MaxDensity, value); }
		}

		/// <summary>Character string that contains either the ID of the printer configuration 
		/// table that contains a set of values for implementation specific print parameters 
		/// (e.g. perception LUT related parameters) or one or more configuration data values, 
		/// encoded as characters. If there are multiple configuration data values encoded in 
		/// the string, they shall be separated by backslashes. The definition of values shall 
		/// be contained in the SCP's Conformance Statement.</summary>
		/// <remarks>
		/// Defined Terms:
		/// <list type="">
		/// <item>
		///   <term>CS000-CS999</term>
		///   <description>Implementation specific curve type.</description>
		/// </item>
		/// </list>
		/// 
		/// Note: It is recommended that for SCPs, CS000 represent the lowest contrast and CS999 
		/// the highest contrast levels available.
		/// </remarks>
		public string ConfigurationInformation {
			get { return _dataset.GetString(DcmTags.ConfigurationInformation, _filmBox.ConfigurationInformation); }
			set { _dataset.AddElementWithValue(DcmTags.ConfigurationInformation, value); }
		}

		/// <summary>Width (x-dimension) in mm of the image to be printed. This value overrides 
		/// the size that corresponds with optimal filling of the Image Box.</summary>
		public double RequestedImageSize {
			get { return _dataset.GetDouble(DcmTags.RequestedImageSize, 0.0); }
			set { _dataset.AddElementWithValue(DcmTags.RequestedImageSize, value); }
		}

		/// <summary>Specifies whether image pixels are to be decimated or cropped if the image 
		/// rows or columns is greater than the available printable pixels in an Image Box.</summary>
		/// <remarks>
		/// Decimation  means that a magnification factor &lt;1 is applied to the image. The method 
		/// of decimation shall be that specified by Magnification Type (2010,0060) or the SCP 
		/// default if not specified.
		/// 
		/// Cropping means that some image rows and/or columns are deleted before printing.
		/// 
		/// Enumerated Values:
		/// <list type="bullet">
		/// <item>
		///   <term>DECIMATE</term>
		///   <description>a magnification factor &lt;1 to be applied to the image.</description>
		/// </item>
		/// <item>
		///   <term>CROP</term>
		///   <description>some image rows and/or columns are to be deleted before printing. The 
		///   specific algorithm for cropping shall be described in the SCP Conformance Statement.</description>
		/// </item>
		/// <item>
		///   <term>FAIL</term>
		///   <description>the SCP shall not crop or decimate</description>
		/// </item>
		/// </list>
		/// </remarks>
		public string RequestedDecimateCropBehavior {
			get { return _dataset.GetString(DcmTags.RequestedDecimateCropBehavior, "DECIMATE"); }
			set { _dataset.AddElementWithValue(DcmTags.RequestedDecimateCropBehavior, value); }
		}
		#endregion

		#region Public Methods
		public DcmImageBox Clone() {
			return new DcmImageBox(_filmBox, SOPClassUID, SOPInstanceUID, Dataset.Clone());
		}
		#endregion
	}
}