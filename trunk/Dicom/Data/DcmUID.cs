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
using System.Text;

namespace Dicom.Data {
	public enum UidType {
		TransferSyntax,
		SOPClass,
		MetaSOPClass,
		SOPInstance,
		ApplicationContextName,
		CodingScheme,
		SynchronizationFrameOfReference,
		Unknown
	}

	public class DcmUID {
		public readonly string UID;
		public readonly string Description;
		public readonly UidType Type;

		private DcmUID() { }

		internal DcmUID(string uid, string desc, UidType type) {
			UID = uid;
			Description = desc;
			Type = type;
		}

		public override string ToString() {
			if (Type == UidType.Unknown)
				return UID;
			return "==" + Description;
		}

		public override bool Equals(object obj) {
			if (obj is DcmUID)
				return ((DcmUID)obj).UID.Equals(UID);
			if (obj is String)
				return (String)obj == UID;
			return false;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}

	public static class DcmUIDs {
		public static Dictionary<string, DcmUID> Entries = new Dictionary<string, DcmUID>();

		static DcmUIDs() {
			#region Load Internal UIDs
			Entries.Add(DcmUIDs.JPEGXRPrivate.UID, DcmUIDs.JPEGXRPrivate);
			Entries.Add(DcmUIDs.PNGPrivate.UID, DcmUIDs.PNGPrivate);
			Entries.Add(DcmUIDs.ImplicitVRLittleEndian.UID, DcmUIDs.ImplicitVRLittleEndian);
			Entries.Add(DcmUIDs.ExplicitVRLittleEndian.UID, DcmUIDs.ExplicitVRLittleEndian);
			Entries.Add(DcmUIDs.DeflatedExplicitVRLittleEndian.UID, DcmUIDs.DeflatedExplicitVRLittleEndian);
			Entries.Add(DcmUIDs.ExplicitVRBigEndian.UID, DcmUIDs.ExplicitVRBigEndian);
			Entries.Add(DcmUIDs.JPEGProcess1.UID, DcmUIDs.JPEGProcess1);
			Entries.Add(DcmUIDs.JPEGProcess2_4.UID, DcmUIDs.JPEGProcess2_4);
			Entries.Add(DcmUIDs.JPEGProcess3_5Retired.UID, DcmUIDs.JPEGProcess3_5Retired);
			Entries.Add(DcmUIDs.JPEGProcess6_8Retired.UID, DcmUIDs.JPEGProcess6_8Retired);
			Entries.Add(DcmUIDs.JPEGProcess7_9Retired.UID, DcmUIDs.JPEGProcess7_9Retired);
			Entries.Add(DcmUIDs.JPEGProcess10_12Retired.UID, DcmUIDs.JPEGProcess10_12Retired);
			Entries.Add(DcmUIDs.JPEGProcess11_13Retired.UID, DcmUIDs.JPEGProcess11_13Retired);
			Entries.Add(DcmUIDs.JPEGProcess14.UID, DcmUIDs.JPEGProcess14);
			Entries.Add(DcmUIDs.JPEGProcess15Retired.UID, DcmUIDs.JPEGProcess15Retired);
			Entries.Add(DcmUIDs.JPEGProcess16_18Retired.UID, DcmUIDs.JPEGProcess16_18Retired);
			Entries.Add(DcmUIDs.JPEGProcess17_19Retired.UID, DcmUIDs.JPEGProcess17_19Retired);
			Entries.Add(DcmUIDs.JPEGProcess20_22Retired.UID, DcmUIDs.JPEGProcess20_22Retired);
			Entries.Add(DcmUIDs.JPEGProcess21_23Retired.UID, DcmUIDs.JPEGProcess21_23Retired);
			Entries.Add(DcmUIDs.JPEGProcess24_26Retired.UID, DcmUIDs.JPEGProcess24_26Retired);
			Entries.Add(DcmUIDs.JPEGProcess25_27Retired.UID, DcmUIDs.JPEGProcess25_27Retired);
			Entries.Add(DcmUIDs.JPEGProcess28Retired.UID, DcmUIDs.JPEGProcess28Retired);
			Entries.Add(DcmUIDs.JPEGProcess29Retired.UID, DcmUIDs.JPEGProcess29Retired);
			Entries.Add(DcmUIDs.JPEGProcess14SV1.UID, DcmUIDs.JPEGProcess14SV1);
			Entries.Add(DcmUIDs.JPEGLSLossless.UID, DcmUIDs.JPEGLSLossless);
			Entries.Add(DcmUIDs.JPEGLSNearLossless.UID, DcmUIDs.JPEGLSNearLossless);
			Entries.Add(DcmUIDs.JPEG2000Lossless.UID, DcmUIDs.JPEG2000Lossless);
			Entries.Add(DcmUIDs.JPEG2000Lossy.UID, DcmUIDs.JPEG2000Lossy);
			Entries.Add(DcmUIDs.MPEG2.UID, DcmUIDs.MPEG2);
			Entries.Add(DcmUIDs.RLELossless.UID, DcmUIDs.RLELossless);
			Entries.Add(DcmUIDs.Verification.UID, DcmUIDs.Verification);
			Entries.Add(DcmUIDs.MediaStorageDirectoryStorage.UID, DcmUIDs.MediaStorageDirectoryStorage);
			Entries.Add(DcmUIDs.BasicStudyContentNotification.UID, DcmUIDs.BasicStudyContentNotification);
			Entries.Add(DcmUIDs.StorageCommitmentPushModel.UID, DcmUIDs.StorageCommitmentPushModel);
			Entries.Add(DcmUIDs.StorageCommitmentPullModel.UID, DcmUIDs.StorageCommitmentPullModel);
			Entries.Add(DcmUIDs.ProceduralEventLoggingSOPClass.UID, DcmUIDs.ProceduralEventLoggingSOPClass);
			Entries.Add(DcmUIDs.DetachedPatientManagement.UID, DcmUIDs.DetachedPatientManagement);
			Entries.Add(DcmUIDs.DetachedVisitManagement.UID, DcmUIDs.DetachedVisitManagement);
			Entries.Add(DcmUIDs.DetachedStudyManagement.UID, DcmUIDs.DetachedStudyManagement);
			Entries.Add(DcmUIDs.StudyComponentManagement.UID, DcmUIDs.StudyComponentManagement);
			Entries.Add(DcmUIDs.ModalityPerformedProcedureStep.UID, DcmUIDs.ModalityPerformedProcedureStep);
			Entries.Add(DcmUIDs.ModalityPerformedProcedureStepRetrieve.UID, DcmUIDs.ModalityPerformedProcedureStepRetrieve);
			Entries.Add(DcmUIDs.ModalityPerformedProcedureStepNotification.UID, DcmUIDs.ModalityPerformedProcedureStepNotification);
			Entries.Add(DcmUIDs.DetachedResultsManagement.UID, DcmUIDs.DetachedResultsManagement);
			Entries.Add(DcmUIDs.DetachedInterpretationManagement.UID, DcmUIDs.DetachedInterpretationManagement);
			Entries.Add(DcmUIDs.StorageServiceClass.UID, DcmUIDs.StorageServiceClass);
			Entries.Add(DcmUIDs.BasicFilmSession.UID, DcmUIDs.BasicFilmSession);
			Entries.Add(DcmUIDs.BasicFilmBoxSOP.UID, DcmUIDs.BasicFilmBoxSOP);
			Entries.Add(DcmUIDs.BasicGrayscaleImageBox.UID, DcmUIDs.BasicGrayscaleImageBox);
			Entries.Add(DcmUIDs.BasicColorImageBox.UID, DcmUIDs.BasicColorImageBox);
			Entries.Add(DcmUIDs.ReferencedImageBoxRetired.UID, DcmUIDs.ReferencedImageBoxRetired);
			Entries.Add(DcmUIDs.PrintJob.UID, DcmUIDs.PrintJob);
			Entries.Add(DcmUIDs.BasicAnnotationBox.UID, DcmUIDs.BasicAnnotationBox);
			Entries.Add(DcmUIDs.Printer.UID, DcmUIDs.Printer);
			Entries.Add(DcmUIDs.PrinterConfigurationRetrieval.UID, DcmUIDs.PrinterConfigurationRetrieval);
			Entries.Add(DcmUIDs.VOILUTBox.UID, DcmUIDs.VOILUTBox);
			Entries.Add(DcmUIDs.PresentationLUT.UID, DcmUIDs.PresentationLUT);
			Entries.Add(DcmUIDs.ImageOverlayBox.UID, DcmUIDs.ImageOverlayBox);
			Entries.Add(DcmUIDs.BasicPrintImageOverlayBox.UID, DcmUIDs.BasicPrintImageOverlayBox);
			Entries.Add(DcmUIDs.PrintQueueManagement.UID, DcmUIDs.PrintQueueManagement);
			Entries.Add(DcmUIDs.StoredPrintStorage.UID, DcmUIDs.StoredPrintStorage);
			Entries.Add(DcmUIDs.HardcopyGrayscaleImageStorage.UID, DcmUIDs.HardcopyGrayscaleImageStorage);
			Entries.Add(DcmUIDs.HardcopyColorImageStorage.UID, DcmUIDs.HardcopyColorImageStorage);
			Entries.Add(DcmUIDs.PullPrintRequest.UID, DcmUIDs.PullPrintRequest);
			Entries.Add(DcmUIDs.MediaCreationManagementSOPClass.UID, DcmUIDs.MediaCreationManagementSOPClass);
			Entries.Add(DcmUIDs.ComputedRadiographyImageStorage.UID, DcmUIDs.ComputedRadiographyImageStorage);
			Entries.Add(DcmUIDs.DigitalXRayImageStorageForPresentation.UID, DcmUIDs.DigitalXRayImageStorageForPresentation);
			Entries.Add(DcmUIDs.DigitalXRayImageStorageForProcessing.UID, DcmUIDs.DigitalXRayImageStorageForProcessing);
			Entries.Add(DcmUIDs.DigitalMammographyXRayImageStorageForPresentation.UID, DcmUIDs.DigitalMammographyXRayImageStorageForPresentation);
			Entries.Add(DcmUIDs.DigitalMammographyXRayImageStorageForProcessing.UID, DcmUIDs.DigitalMammographyXRayImageStorageForProcessing);
			Entries.Add(DcmUIDs.DigitalIntraoralXRayImageStorageForPresentation.UID, DcmUIDs.DigitalIntraoralXRayImageStorageForPresentation);
			Entries.Add(DcmUIDs.DigitalIntraoralXRayImageStorageForProcessing.UID, DcmUIDs.DigitalIntraoralXRayImageStorageForProcessing);
			Entries.Add(DcmUIDs.CTImageStorage.UID, DcmUIDs.CTImageStorage);
			Entries.Add(DcmUIDs.EnhancedCTImageStorage.UID, DcmUIDs.EnhancedCTImageStorage);
			Entries.Add(DcmUIDs.UltrasoundMultiframeImageStorageRetired.UID, DcmUIDs.UltrasoundMultiframeImageStorageRetired);
			Entries.Add(DcmUIDs.UltrasoundMultiframeImageStorage.UID, DcmUIDs.UltrasoundMultiframeImageStorage);
			Entries.Add(DcmUIDs.MRImageStorage.UID, DcmUIDs.MRImageStorage);
			Entries.Add(DcmUIDs.EnhancedMRImageStorage.UID, DcmUIDs.EnhancedMRImageStorage);
			Entries.Add(DcmUIDs.MRSpectroscopyStorage.UID, DcmUIDs.MRSpectroscopyStorage);
			Entries.Add(DcmUIDs.NuclearMedicineImageStorageRetired.UID, DcmUIDs.NuclearMedicineImageStorageRetired);
			Entries.Add(DcmUIDs.UltrasoundImageStorageRetired.UID, DcmUIDs.UltrasoundImageStorageRetired);
			Entries.Add(DcmUIDs.UltrasoundImageStorage.UID, DcmUIDs.UltrasoundImageStorage);
			Entries.Add(DcmUIDs.SecondaryCaptureImageStorage.UID, DcmUIDs.SecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeSingleBitSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeSingleBitSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeGrayscaleByteSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeGrayscaleByteSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeGrayscaleWordSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeGrayscaleWordSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeTrueColorSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeTrueColorSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.StandaloneOverlayStorage.UID, DcmUIDs.StandaloneOverlayStorage);
			Entries.Add(DcmUIDs.StandaloneCurveStorage.UID, DcmUIDs.StandaloneCurveStorage);
			Entries.Add(DcmUIDs.TwelveLeadECGWaveformStorage.UID, DcmUIDs.TwelveLeadECGWaveformStorage);
			Entries.Add(DcmUIDs.GeneralECGWaveformStorage.UID, DcmUIDs.GeneralECGWaveformStorage);
			Entries.Add(DcmUIDs.AmbulatoryECGWaveformStorage.UID, DcmUIDs.AmbulatoryECGWaveformStorage);
			Entries.Add(DcmUIDs.HemodynamicWaveformStorage.UID, DcmUIDs.HemodynamicWaveformStorage);
			Entries.Add(DcmUIDs.CardiacElectrophysiologyWaveformStorage.UID, DcmUIDs.CardiacElectrophysiologyWaveformStorage);
			Entries.Add(DcmUIDs.BasicVoiceAudioWaveformStorage.UID, DcmUIDs.BasicVoiceAudioWaveformStorage);
			Entries.Add(DcmUIDs.StandaloneModalityLUTStorage.UID, DcmUIDs.StandaloneModalityLUTStorage);
			Entries.Add(DcmUIDs.StandaloneVOILUTStorage.UID, DcmUIDs.StandaloneVOILUTStorage);
			Entries.Add(DcmUIDs.GrayscaleSoftcopyPresentationStateStorage.UID, DcmUIDs.GrayscaleSoftcopyPresentationStateStorage);
			Entries.Add(DcmUIDs.ColorSoftcopyPresentationStateStorage.UID, DcmUIDs.ColorSoftcopyPresentationStateStorage);
			Entries.Add(DcmUIDs.PseudoColorSoftcopyPresentationStateStorage.UID, DcmUIDs.PseudoColorSoftcopyPresentationStateStorage);
			Entries.Add(DcmUIDs.BlendingSoftcopyPresentationStateStorage.UID, DcmUIDs.BlendingSoftcopyPresentationStateStorage);
			Entries.Add(DcmUIDs.XRayAngiographicImageStorage.UID, DcmUIDs.XRayAngiographicImageStorage);
			Entries.Add(DcmUIDs.EnhancedXRayAngiographicImageStorage.UID, DcmUIDs.EnhancedXRayAngiographicImageStorage);
			Entries.Add(DcmUIDs.XRayRadiofluoroscopicImageStorage.UID, DcmUIDs.XRayRadiofluoroscopicImageStorage);
			Entries.Add(DcmUIDs.EnhancedXRayRadiofluoroscopicImageStorage.UID, DcmUIDs.EnhancedXRayRadiofluoroscopicImageStorage);
			Entries.Add(DcmUIDs.XRayAngiographicBiPlaneImageStorageRetired.UID, DcmUIDs.XRayAngiographicBiPlaneImageStorageRetired);
			Entries.Add(DcmUIDs.NuclearMedicineImageStorage.UID, DcmUIDs.NuclearMedicineImageStorage);
			Entries.Add(DcmUIDs.RawDataStorage.UID, DcmUIDs.RawDataStorage);
			Entries.Add(DcmUIDs.SpatialRegistrationStorage.UID, DcmUIDs.SpatialRegistrationStorage);
			Entries.Add(DcmUIDs.SpatialFiducialsStorage.UID, DcmUIDs.SpatialFiducialsStorage);
			Entries.Add(DcmUIDs.RealWorldValueMappingStorage.UID, DcmUIDs.RealWorldValueMappingStorage);
			Entries.Add(DcmUIDs.VLImageStorageRetired.UID, DcmUIDs.VLImageStorageRetired);
			Entries.Add(DcmUIDs.VLMultiframeImageStorageRetired.UID, DcmUIDs.VLMultiframeImageStorageRetired);
			Entries.Add(DcmUIDs.VLEndoscopicImageStorage.UID, DcmUIDs.VLEndoscopicImageStorage);
			Entries.Add(DcmUIDs.VLMicroscopicImageStorage.UID, DcmUIDs.VLMicroscopicImageStorage);
			Entries.Add(DcmUIDs.VLSlideCoordinatesMicroscopicImageStorage.UID, DcmUIDs.VLSlideCoordinatesMicroscopicImageStorage);
			Entries.Add(DcmUIDs.VLPhotographicImageStorage.UID, DcmUIDs.VLPhotographicImageStorage);
			Entries.Add(DcmUIDs.VideoEndoscopicImageStorage.UID, DcmUIDs.VideoEndoscopicImageStorage);
			Entries.Add(DcmUIDs.VideoMicroscopicImageStorage.UID, DcmUIDs.VideoMicroscopicImageStorage);
			Entries.Add(DcmUIDs.VideoPhotographicImageStorage.UID, DcmUIDs.VideoPhotographicImageStorage);
			Entries.Add(DcmUIDs.OphthalmicPhotography8BitImageStorage.UID, DcmUIDs.OphthalmicPhotography8BitImageStorage);
			Entries.Add(DcmUIDs.OphthalmicPhotography16BitImageStorage.UID, DcmUIDs.OphthalmicPhotography16BitImageStorage);
			Entries.Add(DcmUIDs.StereometricRelationshipStorage.UID, DcmUIDs.StereometricRelationshipStorage);
			Entries.Add(DcmUIDs.BasicTextSR.UID, DcmUIDs.BasicTextSR);
			Entries.Add(DcmUIDs.EnhancedSR.UID, DcmUIDs.EnhancedSR);
			Entries.Add(DcmUIDs.ComprehensiveSR.UID, DcmUIDs.ComprehensiveSR);
			Entries.Add(DcmUIDs.ProcedureLogStorage.UID, DcmUIDs.ProcedureLogStorage);
			Entries.Add(DcmUIDs.MammographyCADSR.UID, DcmUIDs.MammographyCADSR);
			Entries.Add(DcmUIDs.KeyObjectSelectionDocument.UID, DcmUIDs.KeyObjectSelectionDocument);
			Entries.Add(DcmUIDs.ChestCADSR.UID, DcmUIDs.ChestCADSR);
			Entries.Add(DcmUIDs.XRayRadiationDoseSR.UID, DcmUIDs.XRayRadiationDoseSR);
			Entries.Add(DcmUIDs.EncapsulatedPDFStorage.UID, DcmUIDs.EncapsulatedPDFStorage);
			Entries.Add(DcmUIDs.PositronEmissionTomographyImageStorage.UID, DcmUIDs.PositronEmissionTomographyImageStorage);
			Entries.Add(DcmUIDs.StandalonePETCurveStorage.UID, DcmUIDs.StandalonePETCurveStorage);
			Entries.Add(DcmUIDs.RTImageStorage.UID, DcmUIDs.RTImageStorage);
			Entries.Add(DcmUIDs.RTDoseStorage.UID, DcmUIDs.RTDoseStorage);
			Entries.Add(DcmUIDs.RTStructureSetStorage.UID, DcmUIDs.RTStructureSetStorage);
			Entries.Add(DcmUIDs.RTBeamsTreatmentRecordStorage.UID, DcmUIDs.RTBeamsTreatmentRecordStorage);
			Entries.Add(DcmUIDs.RTPlanStorage.UID, DcmUIDs.RTPlanStorage);
			Entries.Add(DcmUIDs.RTBrachyTreatmentRecordStorage.UID, DcmUIDs.RTBrachyTreatmentRecordStorage);
			Entries.Add(DcmUIDs.RTTreatmentSummaryRecordStorage.UID, DcmUIDs.RTTreatmentSummaryRecordStorage);
			Entries.Add(DcmUIDs.RTIonPlanStorage.UID, DcmUIDs.RTIonPlanStorage);
			Entries.Add(DcmUIDs.RTIonBeamsTreatmentRecordStorage.UID, DcmUIDs.RTIonBeamsTreatmentRecordStorage);
			Entries.Add(DcmUIDs.PatientRootQueryRetrieveInformationModelFIND.UID, DcmUIDs.PatientRootQueryRetrieveInformationModelFIND);
			Entries.Add(DcmUIDs.PatientRootQueryRetrieveInformationModelMOVE.UID, DcmUIDs.PatientRootQueryRetrieveInformationModelMOVE);
			Entries.Add(DcmUIDs.PatientRootQueryRetrieveInformationModelGET.UID, DcmUIDs.PatientRootQueryRetrieveInformationModelGET);
			Entries.Add(DcmUIDs.StudyRootQueryRetrieveInformationModelFIND.UID, DcmUIDs.StudyRootQueryRetrieveInformationModelFIND);
			Entries.Add(DcmUIDs.StudyRootQueryRetrieveInformationModelMOVE.UID, DcmUIDs.StudyRootQueryRetrieveInformationModelMOVE);
			Entries.Add(DcmUIDs.StudyRootQueryRetrieveInformationModelGET.UID, DcmUIDs.StudyRootQueryRetrieveInformationModelGET);
			Entries.Add(DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelFIND.UID, DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelFIND);
			Entries.Add(DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelMOVE.UID, DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelMOVE);
			Entries.Add(DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelGET.UID, DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelGET);
			Entries.Add(DcmUIDs.ModalityWorklistInformationModelFIND.UID, DcmUIDs.ModalityWorklistInformationModelFIND);
			Entries.Add(DcmUIDs.GeneralPurposeWorklistInformationModelFIND.UID, DcmUIDs.GeneralPurposeWorklistInformationModelFIND);
			Entries.Add(DcmUIDs.GeneralPurposeScheduledProcedureStepSOPClass.UID, DcmUIDs.GeneralPurposeScheduledProcedureStepSOPClass);
			Entries.Add(DcmUIDs.GeneralPurposePerformedProcedureStepSOPClass.UID, DcmUIDs.GeneralPurposePerformedProcedureStepSOPClass);
			Entries.Add(DcmUIDs.InstanceAvailabilityNotificationSOPClass.UID, DcmUIDs.InstanceAvailabilityNotificationSOPClass);
			Entries.Add(DcmUIDs.PatientInformationQuery.UID, DcmUIDs.PatientInformationQuery);
			Entries.Add(DcmUIDs.BreastImagingRelevantPatientInformationQuery.UID, DcmUIDs.BreastImagingRelevantPatientInformationQuery);
			Entries.Add(DcmUIDs.CardiacRelevantPatientInformationQuery.UID, DcmUIDs.CardiacRelevantPatientInformationQuery);
			Entries.Add(DcmUIDs.HangingProtocolStorage.UID, DcmUIDs.HangingProtocolStorage);
			Entries.Add(DcmUIDs.HangingProtocolInformationModelFIND.UID, DcmUIDs.HangingProtocolInformationModelFIND);
			Entries.Add(DcmUIDs.HangingProtocolInformationModelMOVE.UID, DcmUIDs.HangingProtocolInformationModelMOVE);
			Entries.Add(DcmUIDs.DetachedPatientManagementMetaSOPClass.UID, DcmUIDs.DetachedPatientManagementMetaSOPClass);
			Entries.Add(DcmUIDs.DetachedResultsManagementMetaSOPClass.UID, DcmUIDs.DetachedResultsManagementMetaSOPClass);
			Entries.Add(DcmUIDs.DetachedStudyManagementMetaSOPClass.UID, DcmUIDs.DetachedStudyManagementMetaSOPClass);
			Entries.Add(DcmUIDs.BasicGrayscalePrintManagement.UID, DcmUIDs.BasicGrayscalePrintManagement);
			Entries.Add(DcmUIDs.ReferencedGrayscalePrintManagementRetired.UID, DcmUIDs.ReferencedGrayscalePrintManagementRetired);
			Entries.Add(DcmUIDs.BasicColorPrintManagement.UID, DcmUIDs.BasicColorPrintManagement);
			Entries.Add(DcmUIDs.ReferencedColorPrintManagementRetired.UID, DcmUIDs.ReferencedColorPrintManagementRetired);
			Entries.Add(DcmUIDs.PullStoredPrintManagement.UID, DcmUIDs.PullStoredPrintManagement);
			Entries.Add(DcmUIDs.GeneralPurposeWorklistManagementMetaSOPClass.UID, DcmUIDs.GeneralPurposeWorklistManagementMetaSOPClass);
			Entries.Add(DcmUIDs.StorageCommitmentPushModelSOPInstance.UID, DcmUIDs.StorageCommitmentPushModelSOPInstance);
			Entries.Add(DcmUIDs.StorageCommitmentPullModelSOPInstance.UID, DcmUIDs.StorageCommitmentPullModelSOPInstance);
			Entries.Add(DcmUIDs.ProceduralEventLoggingSOPInstance.UID, DcmUIDs.ProceduralEventLoggingSOPInstance);
			Entries.Add(DcmUIDs.TalairachBrainAtlasFrameOfReference.UID, DcmUIDs.TalairachBrainAtlasFrameOfReference);
			Entries.Add(DcmUIDs.SPM2T1FrameOfReference.UID, DcmUIDs.SPM2T1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2T2FrameOfReference.UID, DcmUIDs.SPM2T2FrameOfReference);
			Entries.Add(DcmUIDs.SPM2PDFrameOfReference.UID, DcmUIDs.SPM2PDFrameOfReference);
			Entries.Add(DcmUIDs.SPM2EPIFrameOfReference.UID, DcmUIDs.SPM2EPIFrameOfReference);
			Entries.Add(DcmUIDs.SPM2FILT1FrameOfReference.UID, DcmUIDs.SPM2FILT1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2PETFrameOfReference.UID, DcmUIDs.SPM2PETFrameOfReference);
			Entries.Add(DcmUIDs.SPM2TRANSMFrameOfReference.UID, DcmUIDs.SPM2TRANSMFrameOfReference);
			Entries.Add(DcmUIDs.SPM2SPECTFrameOfReference.UID, DcmUIDs.SPM2SPECTFrameOfReference);
			Entries.Add(DcmUIDs.SPM2GRAYFrameOfReference.UID, DcmUIDs.SPM2GRAYFrameOfReference);
			Entries.Add(DcmUIDs.SPM2WHITEFrameOfReference.UID, DcmUIDs.SPM2WHITEFrameOfReference);
			Entries.Add(DcmUIDs.SPM2CSFFrameOfReference.UID, DcmUIDs.SPM2CSFFrameOfReference);
			Entries.Add(DcmUIDs.SPM2BRAINMASKFrameOfReference.UID, DcmUIDs.SPM2BRAINMASKFrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG305T1FrameOfReference.UID, DcmUIDs.SPM2AVG305T1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG152T1FrameOfReference.UID, DcmUIDs.SPM2AVG152T1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG152T2FrameOfReference.UID, DcmUIDs.SPM2AVG152T2FrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG152PDFrameOfReference.UID, DcmUIDs.SPM2AVG152PDFrameOfReference);
			Entries.Add(DcmUIDs.SPM2SINGLESUBJT1FrameOfReference.UID, DcmUIDs.SPM2SINGLESUBJT1FrameOfReference);
			Entries.Add(DcmUIDs.ICBM452T1FrameOfReference.UID, DcmUIDs.ICBM452T1FrameOfReference);
			Entries.Add(DcmUIDs.ICBMSingleSubjectMRIFrameOfReference.UID, DcmUIDs.ICBMSingleSubjectMRIFrameOfReference);
			Entries.Add(DcmUIDs.PrinterSOPInstance.UID, DcmUIDs.PrinterSOPInstance);
			Entries.Add(DcmUIDs.PrinterConfigurationRetrievalSOPInstance.UID, DcmUIDs.PrinterConfigurationRetrievalSOPInstance);
			Entries.Add(DcmUIDs.PrintQueueSOPInstance.UID, DcmUIDs.PrintQueueSOPInstance);
			Entries.Add(DcmUIDs.DICOMApplicationContextName.UID, DcmUIDs.DICOMApplicationContextName);
			Entries.Add(DcmUIDs.DICOMControlledTerminologyCodingScheme.UID, DcmUIDs.DICOMControlledTerminologyCodingScheme);
			Entries.Add(DcmUIDs.UniversalCoordinatedTime.UID, DcmUIDs.UniversalCoordinatedTime);

			#endregion
		}

		public static DcmUID Lookup(string uid) {
			DcmUID o = null;
			Entries.TryGetValue(uid, out o);
			if (o == null) {
				o = new DcmUID(uid, "Unknown UID", UidType.Unknown);
			}
			return o;
		}

		#region Dicom UIDs
		/// <summary>TransferSyntax: Implicit VR Little Endian</summary>
		public static DcmUID ImplicitVRLittleEndian = new DcmUID("1.2.840.10008.1.2", "Implicit VR Little Endian", UidType.TransferSyntax);

		/// <summary>TransferSyntax: Explicit VR Little Endian</summary>
		public static DcmUID ExplicitVRLittleEndian = new DcmUID("1.2.840.10008.1.2.1", "Explicit VR Little Endian", UidType.TransferSyntax);

		/// <summary>TransferSyntax: Deflated Explicit VR Little Endian</summary>
		public static DcmUID DeflatedExplicitVRLittleEndian = new DcmUID("1.2.840.10008.1.2.1.99", "Deflated Explicit VR Little Endian", UidType.TransferSyntax);

		/// <summary>TransferSyntax: Explicit VR Big Endian</summary>
		public static DcmUID ExplicitVRBigEndian = new DcmUID("1.2.840.10008.1.2.2", "Explicit VR Big Endian", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Baseline (Process 1)</summary>
		public static DcmUID JPEGProcess1 = new DcmUID("1.2.840.10008.1.2.4.50", "JPEG Baseline (Process 1)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Extended (Process 2 &amp; 4)</summary>
		public static DcmUID JPEGProcess2_4 = new DcmUID("1.2.840.10008.1.2.4.51", "JPEG Extended (Process 2 & 4)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Extended (Process 3 &amp; 5) (Retired)</summary>
		public static DcmUID JPEGProcess3_5Retired = new DcmUID("1.2.840.10008.1.2.4.52", "JPEG Extended (Process 3 & 5) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Spectral Selection, Non-Hierarchical (Process 6 &amp; 8) (Retired)</summary>
		public static DcmUID JPEGProcess6_8Retired = new DcmUID("1.2.840.10008.1.2.4.53", "JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Spectral Selection, Non-Hierarchical (Process 7 &amp; 9) (Retired)</summary>
		public static DcmUID JPEGProcess7_9Retired = new DcmUID("1.2.840.10008.1.2.4.54", "JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Full Progression, Non-Hierarchical (Process 10 &amp; 12) (Retired)</summary>
		public static DcmUID JPEGProcess10_12Retired = new DcmUID("1.2.840.10008.1.2.4.55", "JPEG Full Progression, Non-Hierarchical (Process 10 & 12) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Full Progression, Non-Hierarchical (Process 11 &amp; 13) (Retired)</summary>
		public static DcmUID JPEGProcess11_13Retired = new DcmUID("1.2.840.10008.1.2.4.56", "JPEG Full Progression, Non-Hierarchical (Process 11 & 13) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Lossless, Non-Hierarchical (Process 14)</summary>
		public static DcmUID JPEGProcess14 = new DcmUID("1.2.840.10008.1.2.4.57", "JPEG Lossless, Non-Hierarchical (Process 14)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Lossless, Non-Hierarchical (Process 15) (Retired)</summary>
		public static DcmUID JPEGProcess15Retired = new DcmUID("1.2.840.10008.1.2.4.58", "JPEG Lossless, Non-Hierarchical (Process 15) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Extended, Hierarchical (Process 16 &amp; 18) (Retired)</summary>
		public static DcmUID JPEGProcess16_18Retired = new DcmUID("1.2.840.10008.1.2.4.59", "JPEG Extended, Hierarchical (Process 16 & 18) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Extended, Hierarchical (Process 17 &amp; 19) (Retired)</summary>
		public static DcmUID JPEGProcess17_19Retired = new DcmUID("1.2.840.10008.1.2.4.60", "JPEG Extended, Hierarchical (Process 17 & 19) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Spectral Selection, Hierarchical (Process 20 &amp; 22) (Retired)</summary>
		public static DcmUID JPEGProcess20_22Retired = new DcmUID("1.2.840.10008.1.2.4.61", "JPEG Spectral Selection, Hierarchical (Process 20 & 22) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Spectral Selection, Hierarchical (Process 21 &amp; 23) (Retired)</summary>
		public static DcmUID JPEGProcess21_23Retired = new DcmUID("1.2.840.10008.1.2.4.62", "JPEG Spectral Selection, Hierarchical (Process 21 & 23) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Full Progression, Hierarchical (Process 24 &amp; 26) (Retired)</summary>
		public static DcmUID JPEGProcess24_26Retired = new DcmUID("1.2.840.10008.1.2.4.63", "JPEG Full Progression, Hierarchical (Process 24 & 26) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Full Progression, Hierarchical (Process 25 &amp; 27) (Retired)</summary>
		public static DcmUID JPEGProcess25_27Retired = new DcmUID("1.2.840.10008.1.2.4.64", "JPEG Full Progression, Hierarchical (Process 25 & 27) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Lossless, Hierarchical (Process 28) (Retired)</summary>
		public static DcmUID JPEGProcess28Retired = new DcmUID("1.2.840.10008.1.2.4.65", "JPEG Lossless, Hierarchical (Process 28) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Lossless, Hierarchical (Process 29) (Retired)</summary>
		public static DcmUID JPEGProcess29Retired = new DcmUID("1.2.840.10008.1.2.4.66", "JPEG Lossless, Hierarchical (Process 29) (Retired)", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1])</summary>
		public static DcmUID JPEGProcess14SV1 = new DcmUID("1.2.840.10008.1.2.4.70", "JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1])", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG-LS Lossless Image Compression</summary>
		public static DcmUID JPEGLSLossless = new DcmUID("1.2.840.10008.1.2.4.80", "JPEG-LS Lossless Image Compression", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG-LS Lossy (Near-Lossless) Image Compression</summary>
		public static DcmUID JPEGLSNearLossless = new DcmUID("1.2.840.10008.1.2.4.81", "JPEG-LS Lossy (Near-Lossless) Image Compression", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG 2000 Lossless Image Compression</summary>
		public static DcmUID JPEG2000Lossless = new DcmUID("1.2.840.10008.1.2.4.90", "JPEG 2000 Lossless Image Compression", UidType.TransferSyntax);

		/// <summary>TransferSyntax: JPEG 2000 Lossy Image Compression</summary>
		public static DcmUID JPEG2000Lossy = new DcmUID("1.2.840.10008.1.2.4.91", "JPEG 2000 Lossy Image Compression", UidType.TransferSyntax);

		/// <summary>TransferSyntax: MPEG2 Main Profile @ Main Level</summary>
		public static DcmUID MPEG2 = new DcmUID("1.2.840.10008.1.2.4.100", "MPEG2 Main Profile @ Main Level", UidType.TransferSyntax);

		/// <summary>TransferSyntax: RLE Lossless</summary>
		public static DcmUID RLELossless = new DcmUID("1.2.840.10008.1.2.5", "RLE Lossless", UidType.TransferSyntax);

		/// <summary>SOPClass: Verification SOP Class</summary>
		public static DcmUID Verification = new DcmUID("1.2.840.10008.1.1", "Verification SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Media Storage Directory Storage</summary>
		public static DcmUID MediaStorageDirectoryStorage = new DcmUID("1.2.840.10008.1.3.10", "Media Storage Directory Storage", UidType.SOPClass);

		/// <summary>SOPClass: Basic Study Content Notification SOP Class</summary>
		public static DcmUID BasicStudyContentNotification = new DcmUID("1.2.840.10008.1.9", "Basic Study Content Notification SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Storage Commitment Push Model SOP Class</summary>
		public static DcmUID StorageCommitmentPushModel = new DcmUID("1.2.840.10008.1.20.1", "Storage Commitment Push Model SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Storage Commitment Pull Model SOP Class</summary>
		public static DcmUID StorageCommitmentPullModel = new DcmUID("1.2.840.10008.1.20.2", "Storage Commitment Pull Model SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Procedural Event Logging SOP Class</summary>
		public static DcmUID ProceduralEventLoggingSOPClass = new DcmUID("1.2.840.10008.1.40", "Procedural Event Logging SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Detached Patient Management SOP Class</summary>
		public static DcmUID DetachedPatientManagement = new DcmUID("1.2.840.10008.3.1.2.1.1", "Detached Patient Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Detached Visit Management SOP Class</summary>
		public static DcmUID DetachedVisitManagement = new DcmUID("1.2.840.10008.3.1.2.2.1", "Detached Visit Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Detached Study Management SOP Class</summary>
		public static DcmUID DetachedStudyManagement = new DcmUID("1.2.840.10008.3.1.2.3.1", "Detached Study Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Study Component Management SOP Class</summary>
		public static DcmUID StudyComponentManagement = new DcmUID("1.2.840.10008.3.1.2.3.2", "Study Component Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Modality Performed Procedure Step SOP Class</summary>
		public static DcmUID ModalityPerformedProcedureStep = new DcmUID("1.2.840.10008.3.1.2.3.3", "Modality Performed Procedure Step SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Modality Performed Procedure Step Retrieve SOP Class</summary>
		public static DcmUID ModalityPerformedProcedureStepRetrieve = new DcmUID("1.2.840.10008.3.1.2.3.4", "Modality Performed Procedure Step Retrieve SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Modality Performed Procedure Step Notification SOP Class</summary>
		public static DcmUID ModalityPerformedProcedureStepNotification = new DcmUID("1.2.840.10008.3.1.2.3.5", "Modality Performed Procedure Step Notification SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Detached Results Management SOP Class</summary>
		public static DcmUID DetachedResultsManagement = new DcmUID("1.2.840.10008.3.1.2.5.1", "Detached Results Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Detached Interpretation Management SOP Class</summary>
		public static DcmUID DetachedInterpretationManagement = new DcmUID("1.2.840.10008.3.1.2.6.1", "Detached Interpretation Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Storage Service Class</summary>
		public static DcmUID StorageServiceClass = new DcmUID("1.2.840.10008.4.2", "Storage Service Class", UidType.SOPClass);

		/// <summary>SOPClass: Basic Film Session SOP Class</summary>
		public static DcmUID BasicFilmSession = new DcmUID("1.2.840.10008.5.1.1.1", "Basic Film Session SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Basic Film Box SOP Class</summary>
		public static DcmUID BasicFilmBoxSOP = new DcmUID("1.2.840.10008.5.1.1.2", "Basic Film Box SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Basic Grayscale Image Box SOP Class</summary>
		public static DcmUID BasicGrayscaleImageBox = new DcmUID("1.2.840.10008.5.1.1.4", "Basic Grayscale Image Box SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Basic Color Image Box SOP Class</summary>
		public static DcmUID BasicColorImageBox = new DcmUID("1.2.840.10008.5.1.1.4.1", "Basic Color Image Box SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Referenced Image Box SOP Class (Retired)</summary>
		public static DcmUID ReferencedImageBoxRetired = new DcmUID("1.2.840.10008.5.1.1.4.2", "Referenced Image Box SOP Class (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: Print Job SOP Class</summary>
		public static DcmUID PrintJob = new DcmUID("1.2.840.10008.5.1.1.14", "Print Job SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Basic Annotation Box SOP Class</summary>
		public static DcmUID BasicAnnotationBox = new DcmUID("1.2.840.10008.5.1.1.15", "Basic Annotation Box SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Printer SOP Class</summary>
		public static DcmUID Printer = new DcmUID("1.2.840.10008.5.1.1.16", "Printer SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Printer Configuration Retrieval SOP Class</summary>
		public static DcmUID PrinterConfigurationRetrieval = new DcmUID("1.2.840.10008.5.1.1.16.376", "Printer Configuration Retrieval SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: VOI LUT Box SOP Class</summary>
		public static DcmUID VOILUTBox = new DcmUID("1.2.840.10008.5.1.1.22", "VOI LUT Box SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Presentation LUT SOP Class</summary>
		public static DcmUID PresentationLUT = new DcmUID("1.2.840.10008.5.1.1.23", "Presentation LUT SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Image Overlay Box SOP Class (Retired)</summary>
		public static DcmUID ImageOverlayBox = new DcmUID("1.2.840.10008.5.1.1.24", "Image Overlay Box SOP Class (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: Basic Print Image Overlay Box SOP Class</summary>
		public static DcmUID BasicPrintImageOverlayBox = new DcmUID("1.2.840.10008.5.1.1.24.1", "Basic Print Image Overlay Box SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Print Queue Management SOP Class</summary>
		public static DcmUID PrintQueueManagement = new DcmUID("1.2.840.10008.5.1.1.26", "Print Queue Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Stored Print Storage SOP Class</summary>
		public static DcmUID StoredPrintStorage = new DcmUID("1.2.840.10008.5.1.1.27", "Stored Print Storage SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Hardcopy Grayscale Image Storage SOP Class</summary>
		public static DcmUID HardcopyGrayscaleImageStorage = new DcmUID("1.2.840.10008.5.1.1.29", "Hardcopy Grayscale Image Storage SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Hardcopy Color Image Storage SOP Class</summary>
		public static DcmUID HardcopyColorImageStorage = new DcmUID("1.2.840.10008.5.1.1.30", "Hardcopy Color Image Storage SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Pull Print Request SOP Class</summary>
		public static DcmUID PullPrintRequest = new DcmUID("1.2.840.10008.5.1.1.31", "Pull Print Request SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Media Creation Management SOP Class</summary>
		public static DcmUID MediaCreationManagementSOPClass = new DcmUID("1.2.840.10008.5.1.1.33", "Media Creation Management SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Computed Radiography Image Storage</summary>
		public static DcmUID ComputedRadiographyImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.1", "Computed Radiography Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Digital X-Ray Image Storage - For Presentation</summary>
		public static DcmUID DigitalXRayImageStorageForPresentation = new DcmUID("1.2.840.10008.5.1.4.1.1.1.1", "Digital X-Ray Image Storage - For Presentation", UidType.SOPClass);

		/// <summary>SOPClass: Digital X-Ray Image Storage - For Processing</summary>
		public static DcmUID DigitalXRayImageStorageForProcessing = new DcmUID("1.2.840.10008.5.1.4.1.1.1.1.1", "Digital X-Ray Image Storage - For Processing", UidType.SOPClass);

		/// <summary>SOPClass: Digital Mammography X-Ray Image Storage - For Presentation</summary>
		public static DcmUID DigitalMammographyXRayImageStorageForPresentation = new DcmUID("1.2.840.10008.5.1.4.1.1.1.2", "Digital Mammography X-Ray Image Storage - For Presentation", UidType.SOPClass);

		/// <summary>SOPClass: Digital Mammography X-Ray Image Storage - For Processing</summary>
		public static DcmUID DigitalMammographyXRayImageStorageForProcessing = new DcmUID("1.2.840.10008.5.1.4.1.1.1.2.1", "Digital Mammography X-Ray Image Storage - For Processing", UidType.SOPClass);

		/// <summary>SOPClass: Digital Intra-oral X-Ray Image Storage - For Presentation</summary>
		public static DcmUID DigitalIntraoralXRayImageStorageForPresentation = new DcmUID("1.2.840.10008.5.1.4.1.1.1.3", "Digital Intra-oral X-Ray Image Storage - For Presentation", UidType.SOPClass);

		/// <summary>SOPClass: Digital Intra-oral X-Ray Image Storage - For Processing</summary>
		public static DcmUID DigitalIntraoralXRayImageStorageForProcessing = new DcmUID("1.2.840.10008.5.1.4.1.1.1.3.1", "Digital Intra-oral X-Ray Image Storage - For Processing", UidType.SOPClass);

		/// <summary>SOPClass: CT Image Storage</summary>
		public static DcmUID CTImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.2", "CT Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Enhanced CT Image Storage</summary>
		public static DcmUID EnhancedCTImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.2.1", "Enhanced CT Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Ultrasound Multi-frame Image Storage (Retired)</summary>
		public static DcmUID UltrasoundMultiframeImageStorageRetired = new DcmUID("1.2.840.10008.5.1.4.1.1.3", "Ultrasound Multi-frame Image Storage (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: Ultrasound Multi-frame Image Storage</summary>
		public static DcmUID UltrasoundMultiframeImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.3.1", "Ultrasound Multi-frame Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: MR Image Storage</summary>
		public static DcmUID MRImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.4", "MR Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Enhanced MR Image Storage</summary>
		public static DcmUID EnhancedMRImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.4.1", "Enhanced MR Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: MR Spectroscopy Storage</summary>
		public static DcmUID MRSpectroscopyStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.4.2", "MR Spectroscopy Storage", UidType.SOPClass);

		/// <summary>SOPClass: Nuclear Medicine Image Storage (Retired)</summary>
		public static DcmUID NuclearMedicineImageStorageRetired = new DcmUID("1.2.840.10008.5.1.4.1.1.5", "Nuclear Medicine Image Storage (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: Ultrasound Image Storage (Retired)</summary>
		public static DcmUID UltrasoundImageStorageRetired = new DcmUID("1.2.840.10008.5.1.4.1.1.6", "Ultrasound Image Storage (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: Ultrasound Image Storage</summary>
		public static DcmUID UltrasoundImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.6.1", "Ultrasound Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Secondary Capture Image Storage</summary>
		public static DcmUID SecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7", "Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Multi-frame Single Bit Secondary Capture Image Storage</summary>
		public static DcmUID MultiframeSingleBitSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.1", "Multi-frame Single Bit Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Multi-frame Grayscale Byte Secondary Capture Image Storage</summary>
		public static DcmUID MultiframeGrayscaleByteSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.2", "Multi-frame Grayscale Byte Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Multi-frame Grayscale Word Secondary Capture Image Storage</summary>
		public static DcmUID MultiframeGrayscaleWordSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.3", "Multi-frame Grayscale Word Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Multi-frame True Color Secondary Capture Image Storage</summary>
		public static DcmUID MultiframeTrueColorSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.4", "Multi-frame True Color Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Standalone Overlay Storage</summary>
		public static DcmUID StandaloneOverlayStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.8", "Standalone Overlay Storage", UidType.SOPClass);

		/// <summary>SOPClass: Standalone Curve Storage</summary>
		public static DcmUID StandaloneCurveStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9", "Standalone Curve Storage", UidType.SOPClass);

		/// <summary>SOPClass: 12-lead ECG Waveform Storage</summary>
		public static DcmUID TwelveLeadECGWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.1.1", "12-lead ECG Waveform Storage", UidType.SOPClass);

		/// <summary>SOPClass: General ECG Waveform Storage</summary>
		public static DcmUID GeneralECGWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.1.2", "General ECG Waveform Storage", UidType.SOPClass);

		/// <summary>SOPClass: Ambulatory ECG Waveform Storage</summary>
		public static DcmUID AmbulatoryECGWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.1.3", "Ambulatory ECG Waveform Storage", UidType.SOPClass);

		/// <summary>SOPClass: Hemodynamic Waveform Storage</summary>
		public static DcmUID HemodynamicWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.2.1", "Hemodynamic Waveform Storage", UidType.SOPClass);

		/// <summary>SOPClass: Cardiac Electrophysiology Waveform Storage</summary>
		public static DcmUID CardiacElectrophysiologyWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.3.1", "Cardiac Electrophysiology Waveform Storage", UidType.SOPClass);

		/// <summary>SOPClass: Basic Voice Audio Waveform Storage</summary>
		public static DcmUID BasicVoiceAudioWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.4.1", "Basic Voice Audio Waveform Storage", UidType.SOPClass);

		/// <summary>SOPClass: Standalone Modality LUT Storage</summary>
		public static DcmUID StandaloneModalityLUTStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.10", "Standalone Modality LUT Storage", UidType.SOPClass);

		/// <summary>SOPClass: Standalone VOI LUT Storage</summary>
		public static DcmUID StandaloneVOILUTStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.11", "Standalone VOI LUT Storage", UidType.SOPClass);

		/// <summary>SOPClass: Grayscale Softcopy Presentation State Storage SOP Class</summary>
		public static DcmUID GrayscaleSoftcopyPresentationStateStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.11.1", "Grayscale Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Color Softcopy Presentation State Storage SOP Class</summary>
		public static DcmUID ColorSoftcopyPresentationStateStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.11.2", "Color Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Pseudo-Color Softcopy Presentation State Storage SOP Class</summary>
		public static DcmUID PseudoColorSoftcopyPresentationStateStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.11.3", "Pseudo-Color Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Blending Softcopy Presentation State Storage SOP Class</summary>
		public static DcmUID BlendingSoftcopyPresentationStateStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.11.4", "Blending Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: X-Ray Angiographic Image Storage</summary>
		public static DcmUID XRayAngiographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.1", "X-Ray Angiographic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Enhanced X-Ray Angiographic Image Storage</summary>
		public static DcmUID EnhancedXRayAngiographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.1.1", "Enhanced X-Ray Angiographic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: X-Ray Radiofluoroscopic Image Storage</summary>
		public static DcmUID XRayRadiofluoroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.2", "X-Ray Radiofluoroscopic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Enhanced X-Ray Radiofluoroscopic Image Storage</summary>
		public static DcmUID EnhancedXRayRadiofluoroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.2.1", "Enhanced X-Ray Radiofluoroscopic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: X-Ray Angiographic Bi-Plane Image Storage (Retired)</summary>
		public static DcmUID XRayAngiographicBiPlaneImageStorageRetired = new DcmUID("1.2.840.10008.5.1.4.1.1.12.3", "X-Ray Angiographic Bi-Plane Image Storage (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: Nuclear Medicine Image Storage</summary>
		public static DcmUID NuclearMedicineImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.20", "Nuclear Medicine Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Raw Data Storage</summary>
		public static DcmUID RawDataStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66", "Raw Data Storage", UidType.SOPClass);

		/// <summary>SOPClass: Spatial Registration Storage</summary>
		public static DcmUID SpatialRegistrationStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66.1", "Spatial Registration Storage", UidType.SOPClass);

		/// <summary>SOPClass: Spatial Fiducials Storage</summary>
		public static DcmUID SpatialFiducialsStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66.2", "Spatial Fiducials Storage", UidType.SOPClass);

		/// <summary>SOPClass: Real World Value Mapping Storage</summary>
		public static DcmUID RealWorldValueMappingStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.67", "Real World Value Mapping Storage", UidType.SOPClass);

		/// <summary>SOPClass: VL Image Storage (Retired)</summary>
		public static DcmUID VLImageStorageRetired = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1", "VL Image Storage (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: VL Multi-frame Image Storage (Retired)</summary>
		public static DcmUID VLMultiframeImageStorageRetired = new DcmUID("1.2.840.10008.5.1.4.1.1.77.2", "VL Multi-frame Image Storage (Retired)", UidType.SOPClass);

		/// <summary>SOPClass: VL Endoscopic Image Storage</summary>
		public static DcmUID VLEndoscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.1", "VL Endoscopic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: VL Microscopic Image Storage</summary>
		public static DcmUID VLMicroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.2", "VL Microscopic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: VL Slide-Coordinates Microscopic Image Storage</summary>
		public static DcmUID VLSlideCoordinatesMicroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.3", "VL Slide-Coordinates Microscopic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: VL Photographic Image Storage</summary>
		public static DcmUID VLPhotographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.4", "VL Photographic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Video Endoscopic Image Storage</summary>
		public static DcmUID VideoEndoscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.1.1", "Video Endoscopic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Video Microscopic Image Storage</summary>
		public static DcmUID VideoMicroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.2.1", "Video Microscopic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Video Photographic Image Storage</summary>
		public static DcmUID VideoPhotographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.4.1", "Video Photographic Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Ophthalmic Photography 8 Bit Image Storage</summary>
		public static DcmUID OphthalmicPhotography8BitImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.5.1", "Ophthalmic Photography 8 Bit Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Ophthalmic Photography 16 Bit Image Storage</summary>
		public static DcmUID OphthalmicPhotography16BitImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.5.2", "Ophthalmic Photography 16 Bit Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Stereometric Relationship Storage</summary>
		public static DcmUID StereometricRelationshipStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.5.3", "Stereometric Relationship Storage", UidType.SOPClass);

		/// <summary>SOPClass: Basic Text SR</summary>
		public static DcmUID BasicTextSR = new DcmUID("1.2.840.10008.5.1.4.1.1.88.11", "Basic Text SR", UidType.SOPClass);

		/// <summary>SOPClass: Enhanced SR</summary>
		public static DcmUID EnhancedSR = new DcmUID("1.2.840.10008.5.1.4.1.1.88.22", "Enhanced SR", UidType.SOPClass);

		/// <summary>SOPClass: Comprehensive SR</summary>
		public static DcmUID ComprehensiveSR = new DcmUID("1.2.840.10008.5.1.4.1.1.88.33", "Comprehensive SR", UidType.SOPClass);

		/// <summary>SOPClass: Procedure Log Storage</summary>
		public static DcmUID ProcedureLogStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.40", "Procedure Log Storage", UidType.SOPClass);

		/// <summary>SOPClass: Mammography CAD SR</summary>
		public static DcmUID MammographyCADSR = new DcmUID("1.2.840.10008.5.1.4.1.1.88.50", "Mammography CAD SR", UidType.SOPClass);

		/// <summary>SOPClass: Key Object Selection Document</summary>
		public static DcmUID KeyObjectSelectionDocument = new DcmUID("1.2.840.10008.5.1.4.1.1.88.59", "Key Object Selection Document", UidType.SOPClass);

		/// <summary>SOPClass: Chest CAD SR</summary>
		public static DcmUID ChestCADSR = new DcmUID("1.2.840.10008.5.1.4.1.1.88.65", "Chest CAD SR", UidType.SOPClass);

		/// <summary>SOPClass: X-Ray Radiation Dose SR</summary>
		public static DcmUID XRayRadiationDoseSR = new DcmUID("1.2.840.10008.5.1.4.1.1.88.67", "X-Ray Radiation Dose SR", UidType.SOPClass);

		/// <summary>SOPClass: Encapsulated PDF Storage</summary>
		public static DcmUID EncapsulatedPDFStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.104.1", "Encapsulated PDF Storage", UidType.SOPClass);

		/// <summary>SOPClass: Positron Emission Tomography Image Storage</summary>
		public static DcmUID PositronEmissionTomographyImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.128", "Positron Emission Tomography Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: Standalone PET Curve Storage</summary>
		public static DcmUID StandalonePETCurveStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.129", "Standalone PET Curve Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Image Storage</summary>
		public static DcmUID RTImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.1", "RT Image Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Dose Storage</summary>
		public static DcmUID RTDoseStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.2", "RT Dose Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Structure Set Storage</summary>
		public static DcmUID RTStructureSetStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.3", "RT Structure Set Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Beams Treatment Record Storage</summary>
		public static DcmUID RTBeamsTreatmentRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.4", "RT Beams Treatment Record Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Plan Storage</summary>
		public static DcmUID RTPlanStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.5", "RT Plan Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Brachy Treatment Record Storage</summary>
		public static DcmUID RTBrachyTreatmentRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.6", "RT Brachy Treatment Record Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Treatment Summary Record Storage</summary>
		public static DcmUID RTTreatmentSummaryRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.7", "RT Treatment Summary Record Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Ion Plan Storage</summary>
		public static DcmUID RTIonPlanStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.8", "RT Ion Plan Storage", UidType.SOPClass);

		/// <summary>SOPClass: RT Ion Beams Treatment Record Storage</summary>
		public static DcmUID RTIonBeamsTreatmentRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.9", "RT Ion Beams Treatment Record Storage", UidType.SOPClass);

		/// <summary>SOPClass: Patient Root Query/Retrieve Information Model - FIND</summary>
		public static DcmUID PatientRootQueryRetrieveInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.1.2.1.1", "Patient Root Query/Retrieve Information Model - FIND", UidType.SOPClass);

		/// <summary>SOPClass: Patient Root Query/Retrieve Information Model - MOVE</summary>
		public static DcmUID PatientRootQueryRetrieveInformationModelMOVE = new DcmUID("1.2.840.10008.5.1.4.1.2.1.2", "Patient Root Query/Retrieve Information Model - MOVE", UidType.SOPClass);

		/// <summary>SOPClass: Patient Root Query/Retrieve Information Model - GET</summary>
		public static DcmUID PatientRootQueryRetrieveInformationModelGET = new DcmUID("1.2.840.10008.5.1.4.1.2.1.3", "Patient Root Query/Retrieve Information Model - GET", UidType.SOPClass);

		/// <summary>SOPClass: Study Root Query/Retrieve Information Model - FIND</summary>
		public static DcmUID StudyRootQueryRetrieveInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.1.2.2.1", "Study Root Query/Retrieve Information Model - FIND", UidType.SOPClass);

		/// <summary>SOPClass: Study Root Query/Retrieve Information Model - MOVE</summary>
		public static DcmUID StudyRootQueryRetrieveInformationModelMOVE = new DcmUID("1.2.840.10008.5.1.4.1.2.2.2", "Study Root Query/Retrieve Information Model - MOVE", UidType.SOPClass);

		/// <summary>SOPClass: Study Root Query/Retrieve Information Model - GET</summary>
		public static DcmUID StudyRootQueryRetrieveInformationModelGET = new DcmUID("1.2.840.10008.5.1.4.1.2.2.3", "Study Root Query/Retrieve Information Model - GET", UidType.SOPClass);

		/// <summary>SOPClass: Patient/Study Only Query/Retrieve Information Model - FIND</summary>
		public static DcmUID PatientStudyOnlyQueryRetrieveInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.1.2.3.1", "Patient/Study Only Query/Retrieve Information Model - FIND", UidType.SOPClass);

		/// <summary>SOPClass: Patient/Study Only Query/Retrieve Information Model - MOVE</summary>
		public static DcmUID PatientStudyOnlyQueryRetrieveInformationModelMOVE = new DcmUID("1.2.840.10008.5.1.4.1.2.3.2", "Patient/Study Only Query/Retrieve Information Model - MOVE", UidType.SOPClass);

		/// <summary>SOPClass: Patient/Study Only Query/Retrieve Information Model - GET</summary>
		public static DcmUID PatientStudyOnlyQueryRetrieveInformationModelGET = new DcmUID("1.2.840.10008.5.1.4.1.2.3.3", "Patient/Study Only Query/Retrieve Information Model - GET", UidType.SOPClass);

		/// <summary>SOPClass: Modality Worklist Information Model - FIND</summary>
		public static DcmUID ModalityWorklistInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.31", "Modality Worklist Information Model - FIND", UidType.SOPClass);

		/// <summary>SOPClass: General Purpose Worklist Information Model - FIND</summary>
		public static DcmUID GeneralPurposeWorklistInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.32.1", "General Purpose Worklist Information Model - FIND", UidType.SOPClass);

		/// <summary>SOPClass: General Purpose Scheduled Procedure Step SOP Class</summary>
		public static DcmUID GeneralPurposeScheduledProcedureStepSOPClass = new DcmUID("1.2.840.10008.5.1.4.32.2", "General Purpose Scheduled Procedure Step SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: General Purpose Performed Procedure Step SOP Class</summary>
		public static DcmUID GeneralPurposePerformedProcedureStepSOPClass = new DcmUID("1.2.840.10008.5.1.4.32.3", "General Purpose Performed Procedure Step SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: Instance Availability Notification SOP Class</summary>
		public static DcmUID InstanceAvailabilityNotificationSOPClass = new DcmUID("1.2.840.10008.5.1.4.33", "Instance Availability Notification SOP Class", UidType.SOPClass);

		/// <summary>SOPClass: General Relevant Patient Information Query General Relevant</summary>
		public static DcmUID PatientInformationQuery = new DcmUID("1.2.840.10008.5.1.4.37.1", "General Relevant Patient Information Query General Relevant", UidType.SOPClass);

		/// <summary>SOPClass: Breast Imaging Relevant Patient Information Query</summary>
		public static DcmUID BreastImagingRelevantPatientInformationQuery = new DcmUID("1.2.840.10008.5.1.4.37.2", "Breast Imaging Relevant Patient Information Query", UidType.SOPClass);

		/// <summary>SOPClass: Cardiac Relevant Patient Information Query</summary>
		public static DcmUID CardiacRelevantPatientInformationQuery = new DcmUID("1.2.840.10008.5.1.4.37.3", "Cardiac Relevant Patient Information Query", UidType.SOPClass);

		/// <summary>SOPClass: Hanging Protocol Storage</summary>
		public static DcmUID HangingProtocolStorage = new DcmUID("1.2.840.10008.5.1.4.38.1", "Hanging Protocol Storage", UidType.SOPClass);

		/// <summary>SOPClass: Hanging Protocol Information Model - FIND</summary>
		public static DcmUID HangingProtocolInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.38.2", "Hanging Protocol Information Model - FIND", UidType.SOPClass);

		/// <summary>SOPClass: Hanging Protocol Information Model - MOVE</summary>
		public static DcmUID HangingProtocolInformationModelMOVE = new DcmUID("1.2.840.10008.5.1.4.38.3", "Hanging Protocol Information Model - MOVE", UidType.SOPClass);

		/// <summary>MetaSOPClass: Detached Patient Management Meta SOP Class</summary>
		public static DcmUID DetachedPatientManagementMetaSOPClass = new DcmUID("1.2.840.10008.3.1.2.1.4", "Detached Patient Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: Detached Results Management Meta SOP Class</summary>
		public static DcmUID DetachedResultsManagementMetaSOPClass = new DcmUID("1.2.840.10008.3.1.2.5.4", "Detached Results Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: Detached Study Management Meta SOP Class</summary>
		public static DcmUID DetachedStudyManagementMetaSOPClass = new DcmUID("1.2.840.10008.3.1.2.5.5", "Detached Study Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: Basic Grayscale Print Management Meta SOP Class</summary>
		public static DcmUID BasicGrayscalePrintManagement = new DcmUID("1.2.840.10008.5.1.1.9", "Basic Grayscale Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: Referenced Grayscale Print Management Meta SOP Class (Retired)</summary>
		public static DcmUID ReferencedGrayscalePrintManagementRetired = new DcmUID("1.2.840.10008.5.1.1.9.1", "Referenced Grayscale Print Management Meta SOP Class (Retired)", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: Basic Color Print Management Meta SOP Class</summary>
		public static DcmUID BasicColorPrintManagement = new DcmUID("1.2.840.10008.5.1.1.18", "Basic Color Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: Referenced Color Print Management Meta SOP Class (Retired)</summary>
		public static DcmUID ReferencedColorPrintManagementRetired = new DcmUID("1.2.840.10008.5.1.1.18.1", "Referenced Color Print Management Meta SOP Class (Retired)", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: Pull Stored Print Management Meta SOP Class</summary>
		public static DcmUID PullStoredPrintManagement = new DcmUID("1.2.840.10008.5.1.1.32", "Pull Stored Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>MetaSOPClass: General Purpose Worklist Management Meta SOP Class</summary>
		public static DcmUID GeneralPurposeWorklistManagementMetaSOPClass = new DcmUID("1.2.840.10008.5.1.4.32", "General Purpose Worklist Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>SOPInstance: Storage Commitment Push Model SOP Instance</summary>
		public static DcmUID StorageCommitmentPushModelSOPInstance = new DcmUID("1.2.840.10008.1.20.1.1", "Storage Commitment Push Model SOP Instance", UidType.SOPInstance);

		/// <summary>SOPInstance: Storage Commitment Pull Model SOP Instance</summary>
		public static DcmUID StorageCommitmentPullModelSOPInstance = new DcmUID("1.2.840.10008.1.20.2.1", "Storage Commitment Pull Model SOP Instance", UidType.SOPInstance);

		/// <summary>SOPInstance: Procedural Event Logging SOP Instance</summary>
		public static DcmUID ProceduralEventLoggingSOPInstance = new DcmUID("1.2.840.10008.1.40.1", "Procedural Event Logging SOP Instance", UidType.SOPInstance);

		/// <summary>SOPInstance: Talairach Brain Atlas Frame of Reference</summary>
		public static DcmUID TalairachBrainAtlasFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.1", "Talairach Brain Atlas Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 T1 Frame of Reference</summary>
		public static DcmUID SPM2T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.2", "SPM2 T1 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 T2 Frame of Reference</summary>
		public static DcmUID SPM2T2FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.3", "SPM2 T2 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 PD Frame of Reference</summary>
		public static DcmUID SPM2PDFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.4", "SPM2 PD Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 EPI Frame of Reference</summary>
		public static DcmUID SPM2EPIFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.5", "SPM2 EPI Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 FIL T1 Frame of Reference</summary>
		public static DcmUID SPM2FILT1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.6", "SPM2 FIL T1 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 PET Frame of Reference</summary>
		public static DcmUID SPM2PETFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.7", "SPM2 PET Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 TRANSM Frame of Reference</summary>
		public static DcmUID SPM2TRANSMFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.8", "SPM2 TRANSM Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 SPECT Frame of Reference</summary>
		public static DcmUID SPM2SPECTFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.9", "SPM2 SPECT Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 GRAY Frame of Reference</summary>
		public static DcmUID SPM2GRAYFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.10", "SPM2 GRAY Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 WHITE Frame of Reference</summary>
		public static DcmUID SPM2WHITEFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.11", "SPM2 WHITE Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 CSF Frame of Reference</summary>
		public static DcmUID SPM2CSFFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.12", "SPM2 CSF Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 BRAINMASK Frame of Reference</summary>
		public static DcmUID SPM2BRAINMASKFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.13", "SPM2 BRAINMASK Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 AVG305T1 Frame of Reference</summary>
		public static DcmUID SPM2AVG305T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.14", "SPM2 AVG305T1 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 AVG152T1 Frame of Reference</summary>
		public static DcmUID SPM2AVG152T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.15", "SPM2 AVG152T1 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 AVG152T2 Frame of Reference</summary>
		public static DcmUID SPM2AVG152T2FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.16", "SPM2 AVG152T2 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 AVG152PD Frame of Reference</summary>
		public static DcmUID SPM2AVG152PDFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.17", "SPM2 AVG152PD Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: SPM2 SINGLESUBJT1 Frame of Reference</summary>
		public static DcmUID SPM2SINGLESUBJT1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.18", "SPM2 SINGLESUBJT1 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: ICBM 452 T1 Frame of Reference</summary>
		public static DcmUID ICBM452T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.2.1", "ICBM 452 T1 Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: ICBM Single Subject MRI Frame of Reference</summary>
		public static DcmUID ICBMSingleSubjectMRIFrameOfReference = new DcmUID("1.2.840.10008.1.4.2.2", "ICBM Single Subject MRI Frame of Reference", UidType.SOPInstance);

		/// <summary>SOPInstance: Printer SOP Instance</summary>
		public static DcmUID PrinterSOPInstance = new DcmUID("1.2.840.10008.5.1.1.17", "Printer SOP Instance", UidType.SOPInstance);

		/// <summary>SOPInstance: Printer Configuration Retrieval SOP Instance</summary>
		public static DcmUID PrinterConfigurationRetrievalSOPInstance = new DcmUID("1.2.840.10008.5.1.1.17.376", "Printer Configuration Retrieval SOP Instance", UidType.SOPInstance);

		/// <summary>SOPInstance: Print Queue SOP Instance</summary>
		public static DcmUID PrintQueueSOPInstance = new DcmUID("1.2.840.10008.5.1.1.25", "Print Queue SOP Instance", UidType.SOPInstance);

		/// <summary>ApplicationContextName: DICOM Application Context Name</summary>
		public static DcmUID DICOMApplicationContextName = new DcmUID("1.2.840.10008.3.1.1.1", "DICOM Application Context Name", UidType.ApplicationContextName);

		/// <summary>CodingScheme: DICOM Controlled Terminology Coding Scheme</summary>
		public static DcmUID DICOMControlledTerminologyCodingScheme = new DcmUID("1.2.840.10008.2.16.4", "DICOM Controlled Terminology Coding Scheme", UidType.CodingScheme);

		/// <summary>SynchronizationFrameOfReference: Universal Coordinated Time</summary>
		public static DcmUID UniversalCoordinatedTime = new DcmUID("1.2.840.10008.15.1.1", "Universal Coordinated Time", UidType.SynchronizationFrameOfReference);
		#endregion
	}
}
