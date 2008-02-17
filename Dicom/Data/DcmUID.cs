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
		FrameOfReference,
		LDAP,
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

		public static DcmUID Generate() {
			return Generate(Implementation.ClassUID, DateTime.UtcNow.Ticks);
		}

		public static DcmUID Generate(DcmUID baseUid, long nextSeq) {
			StringBuilder uid = new StringBuilder();
			uid.Append(baseUid.UID).Append('.').Append(nextSeq);
			return new DcmUID(uid.ToString(), "SOP Instance UID", UidType.SOPInstance);
		}
	}

	public static class DcmUIDs {
		public static Dictionary<string, DcmUID> Entries = new Dictionary<string, DcmUID>();

		static DcmUIDs() {
			#region Load Internal UIDs
			Entries.Add(DcmUIDs.VerificationSOPClass.UID, DcmUIDs.VerificationSOPClass);
			Entries.Add(DcmUIDs.ImplicitVRLittleEndian.UID, DcmUIDs.ImplicitVRLittleEndian);
			Entries.Add(DcmUIDs.ExplicitVRLittleEndian.UID, DcmUIDs.ExplicitVRLittleEndian);
			Entries.Add(DcmUIDs.DeflatedExplicitVRLittleEndian.UID, DcmUIDs.DeflatedExplicitVRLittleEndian);
			Entries.Add(DcmUIDs.ExplicitVRBigEndian.UID, DcmUIDs.ExplicitVRBigEndian);
			Entries.Add(DcmUIDs.MPEG2MainProfileMainLevel.UID, DcmUIDs.MPEG2MainProfileMainLevel);
			Entries.Add(DcmUIDs.JPEGBaselineProcess1.UID, DcmUIDs.JPEGBaselineProcess1);
			Entries.Add(DcmUIDs.JPEGExtendedProcess2_4.UID, DcmUIDs.JPEGExtendedProcess2_4);
			Entries.Add(DcmUIDs.JPEGExtendedProcess3_5RETIRED.UID, DcmUIDs.JPEGExtendedProcess3_5RETIRED);
			Entries.Add(DcmUIDs.JPEGSpectralSelectionNonHierarchicalProcess6_8RETIRED.UID, DcmUIDs.JPEGSpectralSelectionNonHierarchicalProcess6_8RETIRED);
			Entries.Add(DcmUIDs.JPEGSpectralSelectionNonHierarchicalProcess7_9RETIRED.UID, DcmUIDs.JPEGSpectralSelectionNonHierarchicalProcess7_9RETIRED);
			Entries.Add(DcmUIDs.JPEGFullProgressionNonHierarchicalProcess10_12RETIRED.UID, DcmUIDs.JPEGFullProgressionNonHierarchicalProcess10_12RETIRED);
			Entries.Add(DcmUIDs.JPEGFullProgressionNonHierarchicalProcess11_13RETIRED.UID, DcmUIDs.JPEGFullProgressionNonHierarchicalProcess11_13RETIRED);
			Entries.Add(DcmUIDs.JPEGLosslessNonHierarchicalProcess14.UID, DcmUIDs.JPEGLosslessNonHierarchicalProcess14);
			Entries.Add(DcmUIDs.JPEGLosslessNonHierarchicalProcess15RETIRED.UID, DcmUIDs.JPEGLosslessNonHierarchicalProcess15RETIRED);
			Entries.Add(DcmUIDs.JPEGExtendedHierarchicalProcess16_18RETIRED.UID, DcmUIDs.JPEGExtendedHierarchicalProcess16_18RETIRED);
			Entries.Add(DcmUIDs.JPEGExtendedHierarchicalProcess17_19RETIRED.UID, DcmUIDs.JPEGExtendedHierarchicalProcess17_19RETIRED);
			Entries.Add(DcmUIDs.JPEGSpectralSelectionHierarchicalProcess20_22RETIRED.UID, DcmUIDs.JPEGSpectralSelectionHierarchicalProcess20_22RETIRED);
			Entries.Add(DcmUIDs.JPEGSpectralSelectionHierarchicalProcess21_23RETIRED.UID, DcmUIDs.JPEGSpectralSelectionHierarchicalProcess21_23RETIRED);
			Entries.Add(DcmUIDs.JPEGFullProgressionHierarchicalProcess24_26RETIRED.UID, DcmUIDs.JPEGFullProgressionHierarchicalProcess24_26RETIRED);
			Entries.Add(DcmUIDs.JPEGFullProgressionHierarchicalProcess25_27RETIRED.UID, DcmUIDs.JPEGFullProgressionHierarchicalProcess25_27RETIRED);
			Entries.Add(DcmUIDs.JPEGLosslessHierarchicalProcess28RETIRED.UID, DcmUIDs.JPEGLosslessHierarchicalProcess28RETIRED);
			Entries.Add(DcmUIDs.JPEGLosslessHierarchicalProcess29RETIRED.UID, DcmUIDs.JPEGLosslessHierarchicalProcess29RETIRED);
			Entries.Add(DcmUIDs.JPEGLosslessProcess14SV1.UID, DcmUIDs.JPEGLosslessProcess14SV1);
			Entries.Add(DcmUIDs.JPEGLSLosslessImageCompression.UID, DcmUIDs.JPEGLSLosslessImageCompression);
			Entries.Add(DcmUIDs.JPEGLSLossyNearLosslessImageCompression.UID, DcmUIDs.JPEGLSLossyNearLosslessImageCompression);
			Entries.Add(DcmUIDs.JPEG2000ImageCompressionLosslessOnly.UID, DcmUIDs.JPEG2000ImageCompressionLosslessOnly);
			Entries.Add(DcmUIDs.JPEG2000ImageCompression.UID, DcmUIDs.JPEG2000ImageCompression);
			Entries.Add(DcmUIDs.JPEG2000Part2MulticomponentImageCompressionLosslessOnly.UID, DcmUIDs.JPEG2000Part2MulticomponentImageCompressionLosslessOnly);
			Entries.Add(DcmUIDs.JPEG2000Part2MulticomponentImageCompression.UID, DcmUIDs.JPEG2000Part2MulticomponentImageCompression);
			Entries.Add(DcmUIDs.JPIPReferenced.UID, DcmUIDs.JPIPReferenced);
			Entries.Add(DcmUIDs.JPIPReferencedDeflate.UID, DcmUIDs.JPIPReferencedDeflate);
			Entries.Add(DcmUIDs.RLELossless.UID, DcmUIDs.RLELossless);
			Entries.Add(DcmUIDs.RFC2557MIMEEncapsulation.UID, DcmUIDs.RFC2557MIMEEncapsulation);
			Entries.Add(DcmUIDs.XMLEncoding.UID, DcmUIDs.XMLEncoding);
			Entries.Add(DcmUIDs.StorageCommitmentPushModelSOPClass.UID, DcmUIDs.StorageCommitmentPushModelSOPClass);
			Entries.Add(DcmUIDs.StorageCommitmentPushModelSOPInstance.UID, DcmUIDs.StorageCommitmentPushModelSOPInstance);
			Entries.Add(DcmUIDs.StorageCommitmentPullModelSOPClassRETIRED.UID, DcmUIDs.StorageCommitmentPullModelSOPClassRETIRED);
			Entries.Add(DcmUIDs.StorageCommitmentPullModelSOPInstanceRETIRED.UID, DcmUIDs.StorageCommitmentPullModelSOPInstanceRETIRED);
			Entries.Add(DcmUIDs.MediaStorageDirectoryStorage.UID, DcmUIDs.MediaStorageDirectoryStorage);
			Entries.Add(DcmUIDs.TalairachBrainAtlasFrameOfReference.UID, DcmUIDs.TalairachBrainAtlasFrameOfReference);
			Entries.Add(DcmUIDs.SPM2GRAYFrameOfReference.UID, DcmUIDs.SPM2GRAYFrameOfReference);
			Entries.Add(DcmUIDs.SPM2WHITEFrameOfReference.UID, DcmUIDs.SPM2WHITEFrameOfReference);
			Entries.Add(DcmUIDs.SPM2CSFFrameOfReference.UID, DcmUIDs.SPM2CSFFrameOfReference);
			Entries.Add(DcmUIDs.SPM2BRAINMASKFrameOfReference.UID, DcmUIDs.SPM2BRAINMASKFrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG305T1FrameOfReference.UID, DcmUIDs.SPM2AVG305T1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG152T1FrameOfReference.UID, DcmUIDs.SPM2AVG152T1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG152T2FrameOfReference.UID, DcmUIDs.SPM2AVG152T2FrameOfReference);
			Entries.Add(DcmUIDs.SPM2AVG152PDFrameOfReference.UID, DcmUIDs.SPM2AVG152PDFrameOfReference);
			Entries.Add(DcmUIDs.SPM2SINGLESUBJT1FrameOfReference.UID, DcmUIDs.SPM2SINGLESUBJT1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2T1FrameOfReference.UID, DcmUIDs.SPM2T1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2T2FrameOfReference.UID, DcmUIDs.SPM2T2FrameOfReference);
			Entries.Add(DcmUIDs.SPM2PDFrameOfReference.UID, DcmUIDs.SPM2PDFrameOfReference);
			Entries.Add(DcmUIDs.SPM2EPIFrameOfReference.UID, DcmUIDs.SPM2EPIFrameOfReference);
			Entries.Add(DcmUIDs.SPM2FILT1FrameOfReference.UID, DcmUIDs.SPM2FILT1FrameOfReference);
			Entries.Add(DcmUIDs.SPM2PETFrameOfReference.UID, DcmUIDs.SPM2PETFrameOfReference);
			Entries.Add(DcmUIDs.SPM2TRANSMFrameOfReference.UID, DcmUIDs.SPM2TRANSMFrameOfReference);
			Entries.Add(DcmUIDs.SPM2SPECTFrameOfReference.UID, DcmUIDs.SPM2SPECTFrameOfReference);
			Entries.Add(DcmUIDs.ICBM452T1FrameOfReference.UID, DcmUIDs.ICBM452T1FrameOfReference);
			Entries.Add(DcmUIDs.ICBMSingleSubjectMRIFrameOfReference.UID, DcmUIDs.ICBMSingleSubjectMRIFrameOfReference);
			Entries.Add(DcmUIDs.ProceduralEventLoggingSOPClass.UID, DcmUIDs.ProceduralEventLoggingSOPClass);
			Entries.Add(DcmUIDs.ProceduralEventLoggingSOPInstance.UID, DcmUIDs.ProceduralEventLoggingSOPInstance);
			Entries.Add(DcmUIDs.SubstanceAdministrationLoggingSOPClass.UID, DcmUIDs.SubstanceAdministrationLoggingSOPClass);
			Entries.Add(DcmUIDs.SubstanceAdministrationLoggingSOPInstance.UID, DcmUIDs.SubstanceAdministrationLoggingSOPInstance);
			Entries.Add(DcmUIDs.BasicStudyContentNotificationSOPClassRETIRED.UID, DcmUIDs.BasicStudyContentNotificationSOPClassRETIRED);
			Entries.Add(DcmUIDs.LDAPDicomDeviceName.UID, DcmUIDs.LDAPDicomDeviceName);
			Entries.Add(DcmUIDs.LDAPDicomAssociationInitiator.UID, DcmUIDs.LDAPDicomAssociationInitiator);
			Entries.Add(DcmUIDs.LDAPDicomAssociationAcceptor.UID, DcmUIDs.LDAPDicomAssociationAcceptor);
			Entries.Add(DcmUIDs.LDAPDicomHostname.UID, DcmUIDs.LDAPDicomHostname);
			Entries.Add(DcmUIDs.LDAPDicomPort.UID, DcmUIDs.LDAPDicomPort);
			Entries.Add(DcmUIDs.LDAPDicomSOPClass.UID, DcmUIDs.LDAPDicomSOPClass);
			Entries.Add(DcmUIDs.LDAPDicomTransferRole.UID, DcmUIDs.LDAPDicomTransferRole);
			Entries.Add(DcmUIDs.LDAPDicomTransferSyntax.UID, DcmUIDs.LDAPDicomTransferSyntax);
			Entries.Add(DcmUIDs.LDAPDicomPrimaryDeviceType.UID, DcmUIDs.LDAPDicomPrimaryDeviceType);
			Entries.Add(DcmUIDs.LDAPDicomRelatedDeviceReference.UID, DcmUIDs.LDAPDicomRelatedDeviceReference);
			Entries.Add(DcmUIDs.LDAPDicomPreferredCalledAETitle.UID, DcmUIDs.LDAPDicomPreferredCalledAETitle);
			Entries.Add(DcmUIDs.LDAPDicomDescription.UID, DcmUIDs.LDAPDicomDescription);
			Entries.Add(DcmUIDs.LDAPDicomTLSCyphersuite.UID, DcmUIDs.LDAPDicomTLSCyphersuite);
			Entries.Add(DcmUIDs.LDAPDicomAuthorizedNodeCertificateReference.UID, DcmUIDs.LDAPDicomAuthorizedNodeCertificateReference);
			Entries.Add(DcmUIDs.LDAPDicomThisNodeCertificateReference.UID, DcmUIDs.LDAPDicomThisNodeCertificateReference);
			Entries.Add(DcmUIDs.LDAPDicomInstalled.UID, DcmUIDs.LDAPDicomInstalled);
			Entries.Add(DcmUIDs.LDAPDicomStationName.UID, DcmUIDs.LDAPDicomStationName);
			Entries.Add(DcmUIDs.LDAPDicomDeviceSerialNumber.UID, DcmUIDs.LDAPDicomDeviceSerialNumber);
			Entries.Add(DcmUIDs.LDAPDicomInstitutionName.UID, DcmUIDs.LDAPDicomInstitutionName);
			Entries.Add(DcmUIDs.LDAPDicomInstitutionAddress.UID, DcmUIDs.LDAPDicomInstitutionAddress);
			Entries.Add(DcmUIDs.LDAPDicomInstitutionDepartmentName.UID, DcmUIDs.LDAPDicomInstitutionDepartmentName);
			Entries.Add(DcmUIDs.LDAPDicomIssuerOfPatientID.UID, DcmUIDs.LDAPDicomIssuerOfPatientID);
			Entries.Add(DcmUIDs.LDAPDicomManufacturer.UID, DcmUIDs.LDAPDicomManufacturer);
			Entries.Add(DcmUIDs.LDAPDicomPreferredCallingAETitle.UID, DcmUIDs.LDAPDicomPreferredCallingAETitle);
			Entries.Add(DcmUIDs.LDAPDicomSupportedCharacterSet.UID, DcmUIDs.LDAPDicomSupportedCharacterSet);
			Entries.Add(DcmUIDs.LDAPDicomManufacturerModelName.UID, DcmUIDs.LDAPDicomManufacturerModelName);
			Entries.Add(DcmUIDs.LDAPDicomSoftwareVersion.UID, DcmUIDs.LDAPDicomSoftwareVersion);
			Entries.Add(DcmUIDs.LDAPDicomVendorData.UID, DcmUIDs.LDAPDicomVendorData);
			Entries.Add(DcmUIDs.LDAPDicomAETitle.UID, DcmUIDs.LDAPDicomAETitle);
			Entries.Add(DcmUIDs.LDAPDicomNetworkConnectionReference.UID, DcmUIDs.LDAPDicomNetworkConnectionReference);
			Entries.Add(DcmUIDs.LDAPDicomApplicationCluster.UID, DcmUIDs.LDAPDicomApplicationCluster);
			Entries.Add(DcmUIDs.LDAPDicomConfigurationRoot.UID, DcmUIDs.LDAPDicomConfigurationRoot);
			Entries.Add(DcmUIDs.LDAPDicomDevicesRoot.UID, DcmUIDs.LDAPDicomDevicesRoot);
			Entries.Add(DcmUIDs.LDAPDicomUniqueAETitlesRegistryRoot.UID, DcmUIDs.LDAPDicomUniqueAETitlesRegistryRoot);
			Entries.Add(DcmUIDs.LDAPDicomDevice.UID, DcmUIDs.LDAPDicomDevice);
			Entries.Add(DcmUIDs.LDAPDicomNetworkAE.UID, DcmUIDs.LDAPDicomNetworkAE);
			Entries.Add(DcmUIDs.LDAPDicomNetworkConnection.UID, DcmUIDs.LDAPDicomNetworkConnection);
			Entries.Add(DcmUIDs.LDAPDicomUniqueAETitle.UID, DcmUIDs.LDAPDicomUniqueAETitle);
			Entries.Add(DcmUIDs.LDAPDicomTransferCapability.UID, DcmUIDs.LDAPDicomTransferCapability);
			Entries.Add(DcmUIDs.DICOMControlledTerminology.UID, DcmUIDs.DICOMControlledTerminology);
			Entries.Add(DcmUIDs.DICOMUIDRegistry.UID, DcmUIDs.DICOMUIDRegistry);
			Entries.Add(DcmUIDs.DICOMApplicationContextName.UID, DcmUIDs.DICOMApplicationContextName);
			Entries.Add(DcmUIDs.DetachedPatientManagementSOPClassRETIRED.UID, DcmUIDs.DetachedPatientManagementSOPClassRETIRED);
			Entries.Add(DcmUIDs.DetachedPatientManagementMetaSOPClassRETIRED.UID, DcmUIDs.DetachedPatientManagementMetaSOPClassRETIRED);
			Entries.Add(DcmUIDs.DetachedVisitManagementSOPClassRETIRED.UID, DcmUIDs.DetachedVisitManagementSOPClassRETIRED);
			Entries.Add(DcmUIDs.DetachedStudyManagementSOPClassRETIRED.UID, DcmUIDs.DetachedStudyManagementSOPClassRETIRED);
			Entries.Add(DcmUIDs.StudyComponentManagementSOPClassRETIRED.UID, DcmUIDs.StudyComponentManagementSOPClassRETIRED);
			Entries.Add(DcmUIDs.ModalityPerformedProcedureStepSOPClass.UID, DcmUIDs.ModalityPerformedProcedureStepSOPClass);
			Entries.Add(DcmUIDs.ModalityPerformedProcedureStepRetrieveSOPClass.UID, DcmUIDs.ModalityPerformedProcedureStepRetrieveSOPClass);
			Entries.Add(DcmUIDs.ModalityPerformedProcedureStepNotificationSOPClass.UID, DcmUIDs.ModalityPerformedProcedureStepNotificationSOPClass);
			Entries.Add(DcmUIDs.DetachedResultsManagementSOPClassRETIRED.UID, DcmUIDs.DetachedResultsManagementSOPClassRETIRED);
			Entries.Add(DcmUIDs.DetachedResultsManagementMetaSOPClassRETIRED.UID, DcmUIDs.DetachedResultsManagementMetaSOPClassRETIRED);
			Entries.Add(DcmUIDs.DetachedStudyManagementMetaSOPClassRETIRED.UID, DcmUIDs.DetachedStudyManagementMetaSOPClassRETIRED);
			Entries.Add(DcmUIDs.DetachedInterpretationManagementSOPClassRETIRED.UID, DcmUIDs.DetachedInterpretationManagementSOPClassRETIRED);
			Entries.Add(DcmUIDs.BasicFilmSessionSOPClass.UID, DcmUIDs.BasicFilmSessionSOPClass);
			Entries.Add(DcmUIDs.PrintJobSOPClass.UID, DcmUIDs.PrintJobSOPClass);
			Entries.Add(DcmUIDs.BasicAnnotationBoxSOPClass.UID, DcmUIDs.BasicAnnotationBoxSOPClass);
			Entries.Add(DcmUIDs.PrinterSOPClass.UID, DcmUIDs.PrinterSOPClass);
			Entries.Add(DcmUIDs.PrinterConfigurationRetrievalSOPClass.UID, DcmUIDs.PrinterConfigurationRetrievalSOPClass);
			Entries.Add(DcmUIDs.PrinterSOPInstance.UID, DcmUIDs.PrinterSOPInstance);
			Entries.Add(DcmUIDs.PrinterConfigurationRetrievalSOPInstance.UID, DcmUIDs.PrinterConfigurationRetrievalSOPInstance);
			Entries.Add(DcmUIDs.BasicColorPrintManagementMetaSOPClass.UID, DcmUIDs.BasicColorPrintManagementMetaSOPClass);
			Entries.Add(DcmUIDs.ReferencedColorPrintManagementMetaSOPClassRETIRED.UID, DcmUIDs.ReferencedColorPrintManagementMetaSOPClassRETIRED);
			Entries.Add(DcmUIDs.BasicFilmBoxSOPClass.UID, DcmUIDs.BasicFilmBoxSOPClass);
			Entries.Add(DcmUIDs.VOILUTBoxSOPClass.UID, DcmUIDs.VOILUTBoxSOPClass);
			Entries.Add(DcmUIDs.PresentationLUTSOPClass.UID, DcmUIDs.PresentationLUTSOPClass);
			Entries.Add(DcmUIDs.ImageOverlayBoxSOPClassRETIRED.UID, DcmUIDs.ImageOverlayBoxSOPClassRETIRED);
			Entries.Add(DcmUIDs.BasicPrintImageOverlayBoxSOPClassRETIRED.UID, DcmUIDs.BasicPrintImageOverlayBoxSOPClassRETIRED);
			Entries.Add(DcmUIDs.PrintQueueSOPInstanceRETIRED.UID, DcmUIDs.PrintQueueSOPInstanceRETIRED);
			Entries.Add(DcmUIDs.PrintQueueManagementSOPClassRETIRED.UID, DcmUIDs.PrintQueueManagementSOPClassRETIRED);
			Entries.Add(DcmUIDs.StoredPrintStorageSOPClassRETIRED.UID, DcmUIDs.StoredPrintStorageSOPClassRETIRED);
			Entries.Add(DcmUIDs.HardcopyGrayscaleImageStorageSOPClassRETIRED.UID, DcmUIDs.HardcopyGrayscaleImageStorageSOPClassRETIRED);
			Entries.Add(DcmUIDs.HardcopyColorImageStorageSOPClassRETIRED.UID, DcmUIDs.HardcopyColorImageStorageSOPClassRETIRED);
			Entries.Add(DcmUIDs.PullPrintRequestSOPClassRETIRED.UID, DcmUIDs.PullPrintRequestSOPClassRETIRED);
			Entries.Add(DcmUIDs.PullStoredPrintManagementMetaSOPClassRETIRED.UID, DcmUIDs.PullStoredPrintManagementMetaSOPClassRETIRED);
			Entries.Add(DcmUIDs.MediaCreationManagementSOPClassUID.UID, DcmUIDs.MediaCreationManagementSOPClassUID);
			Entries.Add(DcmUIDs.BasicGrayscaleImageBoxSOPClass.UID, DcmUIDs.BasicGrayscaleImageBoxSOPClass);
			Entries.Add(DcmUIDs.BasicColorImageBoxSOPClass.UID, DcmUIDs.BasicColorImageBoxSOPClass);
			Entries.Add(DcmUIDs.ReferencedImageBoxSOPClassRETIRED.UID, DcmUIDs.ReferencedImageBoxSOPClassRETIRED);
			Entries.Add(DcmUIDs.BasicGrayscalePrintManagementMetaSOPClass.UID, DcmUIDs.BasicGrayscalePrintManagementMetaSOPClass);
			Entries.Add(DcmUIDs.ReferencedGrayscalePrintManagementMetaSOPClassRETIRED.UID, DcmUIDs.ReferencedGrayscalePrintManagementMetaSOPClassRETIRED);
			Entries.Add(DcmUIDs.ComputedRadiographyImageStorage.UID, DcmUIDs.ComputedRadiographyImageStorage);
			Entries.Add(DcmUIDs.DigitalXRayImageStorageForPresentation.UID, DcmUIDs.DigitalXRayImageStorageForPresentation);
			Entries.Add(DcmUIDs.DigitalXRayImageStorageForProcessing.UID, DcmUIDs.DigitalXRayImageStorageForProcessing);
			Entries.Add(DcmUIDs.DigitalMammographyXRayImageStorageForPresentation.UID, DcmUIDs.DigitalMammographyXRayImageStorageForPresentation);
			Entries.Add(DcmUIDs.DigitalMammographyXRayImageStorageForProcessing.UID, DcmUIDs.DigitalMammographyXRayImageStorageForProcessing);
			Entries.Add(DcmUIDs.DigitalIntraoralXRayImageStorageForPresentation.UID, DcmUIDs.DigitalIntraoralXRayImageStorageForPresentation);
			Entries.Add(DcmUIDs.DigitalIntraoralXRayImageStorageForProcessing.UID, DcmUIDs.DigitalIntraoralXRayImageStorageForProcessing);
			Entries.Add(DcmUIDs.StandaloneModalityLUTStorageRETIRED.UID, DcmUIDs.StandaloneModalityLUTStorageRETIRED);
			Entries.Add(DcmUIDs.EncapsulatedPDFStorage.UID, DcmUIDs.EncapsulatedPDFStorage);
			Entries.Add(DcmUIDs.EncapsulatedCDAStorage.UID, DcmUIDs.EncapsulatedCDAStorage);
			Entries.Add(DcmUIDs.StandaloneVOILUTStorageRETIRED.UID, DcmUIDs.StandaloneVOILUTStorageRETIRED);
			Entries.Add(DcmUIDs.GrayscaleSoftcopyPresentationStateStorageSOPClass.UID, DcmUIDs.GrayscaleSoftcopyPresentationStateStorageSOPClass);
			Entries.Add(DcmUIDs.ColorSoftcopyPresentationStateStorageSOPClass.UID, DcmUIDs.ColorSoftcopyPresentationStateStorageSOPClass);
			Entries.Add(DcmUIDs.PseudoColorSoftcopyPresentationStateStorageSOPClass.UID, DcmUIDs.PseudoColorSoftcopyPresentationStateStorageSOPClass);
			Entries.Add(DcmUIDs.BlendingSoftcopyPresentationStateStorageSOPClass.UID, DcmUIDs.BlendingSoftcopyPresentationStateStorageSOPClass);
			Entries.Add(DcmUIDs.XRayAngiographicImageStorage.UID, DcmUIDs.XRayAngiographicImageStorage);
			Entries.Add(DcmUIDs.EnhancedXAImageStorage.UID, DcmUIDs.EnhancedXAImageStorage);
			Entries.Add(DcmUIDs.XRayRadiofluoroscopicImageStorage.UID, DcmUIDs.XRayRadiofluoroscopicImageStorage);
			Entries.Add(DcmUIDs.EnhancedXRFImageStorage.UID, DcmUIDs.EnhancedXRFImageStorage);
			Entries.Add(DcmUIDs.XRayAngiographicBiPlaneImageStorageRETIRED.UID, DcmUIDs.XRayAngiographicBiPlaneImageStorageRETIRED);
			Entries.Add(DcmUIDs.PositronEmissionTomographyImageStorage.UID, DcmUIDs.PositronEmissionTomographyImageStorage);
			Entries.Add(DcmUIDs.StandalonePETCurveStorageRETIRED.UID, DcmUIDs.StandalonePETCurveStorageRETIRED);
			Entries.Add(DcmUIDs.XRay3DAngiographicImageStorage.UID, DcmUIDs.XRay3DAngiographicImageStorage);
			Entries.Add(DcmUIDs.XRay3DCraniofacialImageStorage.UID, DcmUIDs.XRay3DCraniofacialImageStorage);
			Entries.Add(DcmUIDs.CTImageStorage.UID, DcmUIDs.CTImageStorage);
			Entries.Add(DcmUIDs.EnhancedCTImageStorage.UID, DcmUIDs.EnhancedCTImageStorage);
			Entries.Add(DcmUIDs.NuclearMedicineImageStorage.UID, DcmUIDs.NuclearMedicineImageStorage);
			Entries.Add(DcmUIDs.UltrasoundMultiframeImageStorageRETIRED.UID, DcmUIDs.UltrasoundMultiframeImageStorageRETIRED);
			Entries.Add(DcmUIDs.UltrasoundMultiframeImageStorage.UID, DcmUIDs.UltrasoundMultiframeImageStorage);
			Entries.Add(DcmUIDs.MRImageStorage.UID, DcmUIDs.MRImageStorage);
			Entries.Add(DcmUIDs.EnhancedMRImageStorage.UID, DcmUIDs.EnhancedMRImageStorage);
			Entries.Add(DcmUIDs.MRSpectroscopyStorage.UID, DcmUIDs.MRSpectroscopyStorage);
			Entries.Add(DcmUIDs.RTImageStorage.UID, DcmUIDs.RTImageStorage);
			Entries.Add(DcmUIDs.RTDoseStorage.UID, DcmUIDs.RTDoseStorage);
			Entries.Add(DcmUIDs.RTStructureSetStorage.UID, DcmUIDs.RTStructureSetStorage);
			Entries.Add(DcmUIDs.RTBeamsTreatmentRecordStorage.UID, DcmUIDs.RTBeamsTreatmentRecordStorage);
			Entries.Add(DcmUIDs.RTPlanStorage.UID, DcmUIDs.RTPlanStorage);
			Entries.Add(DcmUIDs.RTBrachyTreatmentRecordStorage.UID, DcmUIDs.RTBrachyTreatmentRecordStorage);
			Entries.Add(DcmUIDs.RTTreatmentSummaryRecordStorage.UID, DcmUIDs.RTTreatmentSummaryRecordStorage);
			Entries.Add(DcmUIDs.RTIonPlanStorage.UID, DcmUIDs.RTIonPlanStorage);
			Entries.Add(DcmUIDs.RTIonBeamsTreatmentRecordStorage.UID, DcmUIDs.RTIonBeamsTreatmentRecordStorage);
			Entries.Add(DcmUIDs.NuclearMedicineImageStorageRETIRED.UID, DcmUIDs.NuclearMedicineImageStorageRETIRED);
			Entries.Add(DcmUIDs.UltrasoundImageStorageRETIRED.UID, DcmUIDs.UltrasoundImageStorageRETIRED);
			Entries.Add(DcmUIDs.UltrasoundImageStorage.UID, DcmUIDs.UltrasoundImageStorage);
			Entries.Add(DcmUIDs.RawDataStorage.UID, DcmUIDs.RawDataStorage);
			Entries.Add(DcmUIDs.SpatialRegistrationStorage.UID, DcmUIDs.SpatialRegistrationStorage);
			Entries.Add(DcmUIDs.SpatialFiducialsStorage.UID, DcmUIDs.SpatialFiducialsStorage);
			Entries.Add(DcmUIDs.DeformableSpatialRegistrationStorage.UID, DcmUIDs.DeformableSpatialRegistrationStorage);
			Entries.Add(DcmUIDs.SegmentationStorage.UID, DcmUIDs.SegmentationStorage);
			Entries.Add(DcmUIDs.RealWorldValueMappingStorage.UID, DcmUIDs.RealWorldValueMappingStorage);
			Entries.Add(DcmUIDs.SecondaryCaptureImageStorage.UID, DcmUIDs.SecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeSingleBitSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeSingleBitSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeGrayscaleByteSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeGrayscaleByteSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeGrayscaleWordSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeGrayscaleWordSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.MultiframeTrueColorSecondaryCaptureImageStorage.UID, DcmUIDs.MultiframeTrueColorSecondaryCaptureImageStorage);
			Entries.Add(DcmUIDs.VLImageStorageTrialRETIRED.UID, DcmUIDs.VLImageStorageTrialRETIRED);
			Entries.Add(DcmUIDs.VLEndoscopicImageStorage.UID, DcmUIDs.VLEndoscopicImageStorage);
			Entries.Add(DcmUIDs.VideoEndoscopicImageStorage.UID, DcmUIDs.VideoEndoscopicImageStorage);
			Entries.Add(DcmUIDs.VLMicroscopicImageStorage.UID, DcmUIDs.VLMicroscopicImageStorage);
			Entries.Add(DcmUIDs.VideoMicroscopicImageStorage.UID, DcmUIDs.VideoMicroscopicImageStorage);
			Entries.Add(DcmUIDs.VLSlideCoordinatesMicroscopicImageStorage.UID, DcmUIDs.VLSlideCoordinatesMicroscopicImageStorage);
			Entries.Add(DcmUIDs.VLPhotographicImageStorage.UID, DcmUIDs.VLPhotographicImageStorage);
			Entries.Add(DcmUIDs.VideoPhotographicImageStorage.UID, DcmUIDs.VideoPhotographicImageStorage);
			Entries.Add(DcmUIDs.OphthalmicPhotography8BitImageStorage.UID, DcmUIDs.OphthalmicPhotography8BitImageStorage);
			Entries.Add(DcmUIDs.OphthalmicPhotography16BitImageStorage.UID, DcmUIDs.OphthalmicPhotography16BitImageStorage);
			Entries.Add(DcmUIDs.StereometricRelationshipStorage.UID, DcmUIDs.StereometricRelationshipStorage);
			Entries.Add(DcmUIDs.OphthalmicTomographyImageStorage.UID, DcmUIDs.OphthalmicTomographyImageStorage);
			Entries.Add(DcmUIDs.VLMultiframeImageStorageTrialRETIRED.UID, DcmUIDs.VLMultiframeImageStorageTrialRETIRED);
			Entries.Add(DcmUIDs.StandaloneOverlayStorageRETIRED.UID, DcmUIDs.StandaloneOverlayStorageRETIRED);
			Entries.Add(DcmUIDs.TextSRStorageTrialRETIRED.UID, DcmUIDs.TextSRStorageTrialRETIRED);
			Entries.Add(DcmUIDs.BasicTextSRStorage.UID, DcmUIDs.BasicTextSRStorage);
			Entries.Add(DcmUIDs.AudioSRStorageTrialRETIRED.UID, DcmUIDs.AudioSRStorageTrialRETIRED);
			Entries.Add(DcmUIDs.EnhancedSRStorage.UID, DcmUIDs.EnhancedSRStorage);
			Entries.Add(DcmUIDs.DetailSRStorageTrialRETIRED.UID, DcmUIDs.DetailSRStorageTrialRETIRED);
			Entries.Add(DcmUIDs.ComprehensiveSRStorage.UID, DcmUIDs.ComprehensiveSRStorage);
			Entries.Add(DcmUIDs.ComprehensiveSRStorageTrialRETIRED.UID, DcmUIDs.ComprehensiveSRStorageTrialRETIRED);
			Entries.Add(DcmUIDs.ProcedureLogStorage.UID, DcmUIDs.ProcedureLogStorage);
			Entries.Add(DcmUIDs.MammographyCADSRStorage.UID, DcmUIDs.MammographyCADSRStorage);
			Entries.Add(DcmUIDs.KeyObjectSelectionDocumentStorage.UID, DcmUIDs.KeyObjectSelectionDocumentStorage);
			Entries.Add(DcmUIDs.ChestCADSRStorage.UID, DcmUIDs.ChestCADSRStorage);
			Entries.Add(DcmUIDs.XRayRadiationDoseSRStorage.UID, DcmUIDs.XRayRadiationDoseSRStorage);
			Entries.Add(DcmUIDs.StandaloneCurveStorageRETIRED.UID, DcmUIDs.StandaloneCurveStorageRETIRED);
			Entries.Add(DcmUIDs.WaveformStorageTrialRETIRED.UID, DcmUIDs.WaveformStorageTrialRETIRED);
			Entries.Add(DcmUIDs.TwelveLeadECGWaveformStorage.UID, DcmUIDs.TwelveLeadECGWaveformStorage);
			Entries.Add(DcmUIDs.GeneralECGWaveformStorage.UID, DcmUIDs.GeneralECGWaveformStorage);
			Entries.Add(DcmUIDs.AmbulatoryECGWaveformStorage.UID, DcmUIDs.AmbulatoryECGWaveformStorage);
			Entries.Add(DcmUIDs.HemodynamicWaveformStorage.UID, DcmUIDs.HemodynamicWaveformStorage);
			Entries.Add(DcmUIDs.CardiacElectrophysiologyWaveformStorage.UID, DcmUIDs.CardiacElectrophysiologyWaveformStorage);
			Entries.Add(DcmUIDs.BasicVoiceAudioWaveformStorage.UID, DcmUIDs.BasicVoiceAudioWaveformStorage);
			Entries.Add(DcmUIDs.PatientRootQueryRetrieveInformationModelFIND.UID, DcmUIDs.PatientRootQueryRetrieveInformationModelFIND);
			Entries.Add(DcmUIDs.PatientRootQueryRetrieveInformationModelMOVE.UID, DcmUIDs.PatientRootQueryRetrieveInformationModelMOVE);
			Entries.Add(DcmUIDs.PatientRootQueryRetrieveInformationModelGET.UID, DcmUIDs.PatientRootQueryRetrieveInformationModelGET);
			Entries.Add(DcmUIDs.StudyRootQueryRetrieveInformationModelFIND.UID, DcmUIDs.StudyRootQueryRetrieveInformationModelFIND);
			Entries.Add(DcmUIDs.StudyRootQueryRetrieveInformationModelMOVE.UID, DcmUIDs.StudyRootQueryRetrieveInformationModelMOVE);
			Entries.Add(DcmUIDs.StudyRootQueryRetrieveInformationModelGET.UID, DcmUIDs.StudyRootQueryRetrieveInformationModelGET);
			Entries.Add(DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelFINDRETIRED.UID, DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelFINDRETIRED);
			Entries.Add(DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelMOVERETIRED.UID, DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelMOVERETIRED);
			Entries.Add(DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelGETRETIRED.UID, DcmUIDs.PatientStudyOnlyQueryRetrieveInformationModelGETRETIRED);
			Entries.Add(DcmUIDs.ModalityWorklistInformationModelFIND.UID, DcmUIDs.ModalityWorklistInformationModelFIND);
			Entries.Add(DcmUIDs.GeneralPurposeWorklistManagementMetaSOPClass.UID, DcmUIDs.GeneralPurposeWorklistManagementMetaSOPClass);
			Entries.Add(DcmUIDs.GeneralPurposeWorklistInformationModelFIND.UID, DcmUIDs.GeneralPurposeWorklistInformationModelFIND);
			Entries.Add(DcmUIDs.GeneralPurposeScheduledProcedureStepSOPClass.UID, DcmUIDs.GeneralPurposeScheduledProcedureStepSOPClass);
			Entries.Add(DcmUIDs.GeneralPurposePerformedProcedureStepSOPClass.UID, DcmUIDs.GeneralPurposePerformedProcedureStepSOPClass);
			Entries.Add(DcmUIDs.InstanceAvailabilityNotificationSOPClass.UID, DcmUIDs.InstanceAvailabilityNotificationSOPClass);
			Entries.Add(DcmUIDs.RTBeamsDeliveryInstructionStorageSupplement74FrozenDraft.UID, DcmUIDs.RTBeamsDeliveryInstructionStorageSupplement74FrozenDraft);
			Entries.Add(DcmUIDs.RTConventionalMachineVerificationSupplement74FrozenDraft.UID, DcmUIDs.RTConventionalMachineVerificationSupplement74FrozenDraft);
			Entries.Add(DcmUIDs.RTIonMachineVerificationSupplement74FrozenDraft.UID, DcmUIDs.RTIonMachineVerificationSupplement74FrozenDraft);
			Entries.Add(DcmUIDs.UnifiedWorklistAndProcedureStepSOPClass.UID, DcmUIDs.UnifiedWorklistAndProcedureStepSOPClass);
			Entries.Add(DcmUIDs.UnifiedProcedureStepPushSOPClass.UID, DcmUIDs.UnifiedProcedureStepPushSOPClass);
			Entries.Add(DcmUIDs.UnifiedProcedureStepWatchSOPClass.UID, DcmUIDs.UnifiedProcedureStepWatchSOPClass);
			Entries.Add(DcmUIDs.UnifiedProcedureStepPullSOPClass.UID, DcmUIDs.UnifiedProcedureStepPullSOPClass);
			Entries.Add(DcmUIDs.UnifiedProcedureStepEventSOPClass.UID, DcmUIDs.UnifiedProcedureStepEventSOPClass);
			Entries.Add(DcmUIDs.UnifiedWorklistAndProcedureStepSOPInstance.UID, DcmUIDs.UnifiedWorklistAndProcedureStepSOPInstance);
			Entries.Add(DcmUIDs.GeneralRelevantPatientInformationQuery.UID, DcmUIDs.GeneralRelevantPatientInformationQuery);
			Entries.Add(DcmUIDs.BreastImagingRelevantPatientInformationQuery.UID, DcmUIDs.BreastImagingRelevantPatientInformationQuery);
			Entries.Add(DcmUIDs.CardiacRelevantPatientInformationQuery.UID, DcmUIDs.CardiacRelevantPatientInformationQuery);
			Entries.Add(DcmUIDs.HangingProtocolStorage.UID, DcmUIDs.HangingProtocolStorage);
			Entries.Add(DcmUIDs.HangingProtocolInformationModelFIND.UID, DcmUIDs.HangingProtocolInformationModelFIND);
			Entries.Add(DcmUIDs.HangingProtocolInformationModelMOVE.UID, DcmUIDs.HangingProtocolInformationModelMOVE);
			Entries.Add(DcmUIDs.ProductCharacteristicsQuerySOPClass.UID, DcmUIDs.ProductCharacteristicsQuerySOPClass);
			Entries.Add(DcmUIDs.SubstanceApprovalQuerySOPClass.UID, DcmUIDs.SubstanceApprovalQuerySOPClass);

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
		/// <summary>SOP Class: Verification SOP Class [PS 3.4]</summary>
		public static DcmUID VerificationSOPClass = new DcmUID("1.2.840.10008.1.1", "Verification SOP Class", UidType.SOPClass);

		/// <summary>Transfer Syntax: Implicit VR Little Endian: Default Transfer Syntax for DICOM [PS 3.5]</summary>
		public static DcmUID ImplicitVRLittleEndian = new DcmUID("1.2.840.10008.1.2", "Implicit VR Little Endian: Default Transfer Syntax for DICOM", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: Explicit VR Little Endian [PS 3.5]</summary>
		public static DcmUID ExplicitVRLittleEndian = new DcmUID("1.2.840.10008.1.2.1", "Explicit VR Little Endian", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: Deflated Explicit VR Little Endian [PS 3.5]</summary>
		public static DcmUID DeflatedExplicitVRLittleEndian = new DcmUID("1.2.840.10008.1.2.1.99", "Deflated Explicit VR Little Endian", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: Explicit VR Big Endian [PS 3.5]</summary>
		public static DcmUID ExplicitVRBigEndian = new DcmUID("1.2.840.10008.1.2.2", "Explicit VR Big Endian", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: MPEG2 Main Profile @ Main Level [PS 3.5]</summary>
		public static DcmUID MPEG2MainProfileMainLevel = new DcmUID("1.2.840.10008.1.2.4.100", "MPEG2 Main Profile @ Main Level", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression [PS 3.5]</summary>
		public static DcmUID JPEGBaselineProcess1 = new DcmUID("1.2.840.10008.1.2.4.50", "JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Extended (Process 2 &amp; 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only) [PS 3.5]</summary>
		public static DcmUID JPEGExtendedProcess2_4 = new DcmUID("1.2.840.10008.1.2.4.51", "JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Extended (Process 3 &amp; 5) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGExtendedProcess3_5RETIRED = new DcmUID("1.2.840.10008.1.2.4.52", "JPEG Extended (Process 3 & 5)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Spectral Selection, Non-Hierarchical (Process 6 &amp; 8) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGSpectralSelectionNonHierarchicalProcess6_8RETIRED = new DcmUID("1.2.840.10008.1.2.4.53", "JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Spectral Selection, Non-Hierarchical (Process 7 &amp; 9) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGSpectralSelectionNonHierarchicalProcess7_9RETIRED = new DcmUID("1.2.840.10008.1.2.4.54", "JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Full Progression, Non-Hierarchical (Process 10 &amp; 12) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGFullProgressionNonHierarchicalProcess10_12RETIRED = new DcmUID("1.2.840.10008.1.2.4.55", "JPEG Full Progression, Non-Hierarchical (Process 10 & 12)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Full Progression, Non-Hierarchical (Process 11 &amp; 13) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGFullProgressionNonHierarchicalProcess11_13RETIRED = new DcmUID("1.2.840.10008.1.2.4.56", "JPEG Full Progression, Non-Hierarchical (Process 11 & 13)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Lossless, Non-Hierarchical (Process 14) [PS 3.5]</summary>
		public static DcmUID JPEGLosslessNonHierarchicalProcess14 = new DcmUID("1.2.840.10008.1.2.4.57", "JPEG Lossless, Non-Hierarchical (Process 14)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Lossless, Non-Hierarchical (Process 15) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGLosslessNonHierarchicalProcess15RETIRED = new DcmUID("1.2.840.10008.1.2.4.58", "JPEG Lossless, Non-Hierarchical (Process 15)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Extended, Hierarchical (Process 16 &amp; 18) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGExtendedHierarchicalProcess16_18RETIRED = new DcmUID("1.2.840.10008.1.2.4.59", "JPEG Extended, Hierarchical (Process 16 & 18)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Extended, Hierarchical (Process 17 &amp; 19) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGExtendedHierarchicalProcess17_19RETIRED = new DcmUID("1.2.840.10008.1.2.4.60", "JPEG Extended, Hierarchical (Process 17 & 19)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Spectral Selection, Hierarchical (Process 20 &amp; 22) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGSpectralSelectionHierarchicalProcess20_22RETIRED = new DcmUID("1.2.840.10008.1.2.4.61", "JPEG Spectral Selection, Hierarchical (Process 20 & 22)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Spectral Selection, Hierarchical (Process 21 &amp; 23) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGSpectralSelectionHierarchicalProcess21_23RETIRED = new DcmUID("1.2.840.10008.1.2.4.62", "JPEG Spectral Selection, Hierarchical (Process 21 & 23)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Full Progression, Hierarchical (Process 24 &amp; 26) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGFullProgressionHierarchicalProcess24_26RETIRED = new DcmUID("1.2.840.10008.1.2.4.63", "JPEG Full Progression, Hierarchical (Process 24 & 26)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Full Progression, Hierarchical (Process 25 &amp; 27) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGFullProgressionHierarchicalProcess25_27RETIRED = new DcmUID("1.2.840.10008.1.2.4.64", "JPEG Full Progression, Hierarchical (Process 25 & 27)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Lossless, Hierarchical (Process 28) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGLosslessHierarchicalProcess28RETIRED = new DcmUID("1.2.840.10008.1.2.4.65", "JPEG Lossless, Hierarchical (Process 28)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Lossless, Hierarchical (Process 29) [PS 3.5] (Retired)</summary>
		public static DcmUID JPEGLosslessHierarchicalProcess29RETIRED = new DcmUID("1.2.840.10008.1.2.4.66", "JPEG Lossless, Hierarchical (Process 29)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression [PS 3.5]</summary>
		public static DcmUID JPEGLosslessProcess14SV1 = new DcmUID("1.2.840.10008.1.2.4.70", "JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG-LS Lossless Image Compression [PS 3.5]</summary>
		public static DcmUID JPEGLSLosslessImageCompression = new DcmUID("1.2.840.10008.1.2.4.80", "JPEG-LS Lossless Image Compression", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG-LS Lossy (Near-Lossless) Image Compression [PS 3.5]</summary>
		public static DcmUID JPEGLSLossyNearLosslessImageCompression = new DcmUID("1.2.840.10008.1.2.4.81", "JPEG-LS Lossy (Near-Lossless) Image Compression", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG 2000 Image Compression (Lossless Only) [PS 3.5]</summary>
		public static DcmUID JPEG2000ImageCompressionLosslessOnly = new DcmUID("1.2.840.10008.1.2.4.90", "JPEG 2000 Image Compression (Lossless Only)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG 2000 Image Compression [PS 3.5]</summary>
		public static DcmUID JPEG2000ImageCompression = new DcmUID("1.2.840.10008.1.2.4.91", "JPEG 2000 Image Compression", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG 2000 Part 2 Multi-component Image Compression (Lossless Only) [PS 3.5]</summary>
		public static DcmUID JPEG2000Part2MulticomponentImageCompressionLosslessOnly = new DcmUID("1.2.840.10008.1.2.4.92", "JPEG 2000 Part 2 Multi-component Image Compression (Lossless Only)", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPEG 2000 Part 2 Multi-component Image Compression [PS 3.5]</summary>
		public static DcmUID JPEG2000Part2MulticomponentImageCompression = new DcmUID("1.2.840.10008.1.2.4.93", "JPEG 2000 Part 2 Multi-component Image Compression", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPIP Referenced [PS 3.5]</summary>
		public static DcmUID JPIPReferenced = new DcmUID("1.2.840.10008.1.2.4.94", "JPIP Referenced", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: JPIP Referenced Deflate [PS 3.5]</summary>
		public static DcmUID JPIPReferencedDeflate = new DcmUID("1.2.840.10008.1.2.4.95", "JPIP Referenced Deflate", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: RLE Lossless [PS 3.5]</summary>
		public static DcmUID RLELossless = new DcmUID("1.2.840.10008.1.2.5", "RLE Lossless", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: RFC 2557 MIME encapsulation [PS 3.10]</summary>
		public static DcmUID RFC2557MIMEEncapsulation = new DcmUID("1.2.840.10008.1.2.6.1", "RFC 2557 MIME encapsulation", UidType.TransferSyntax);

		/// <summary>Transfer Syntax: XML Encoding [PS 3.10]</summary>
		public static DcmUID XMLEncoding = new DcmUID("1.2.840.10008.1.2.6.2", "XML Encoding", UidType.TransferSyntax);

		/// <summary>SOP Class: Storage Commitment Push Model SOP Class [PS 3.4]</summary>
		public static DcmUID StorageCommitmentPushModelSOPClass = new DcmUID("1.2.840.10008.1.20.1", "Storage Commitment Push Model SOP Class", UidType.SOPClass);

		/// <summary>Well-known SOP Instance: Storage Commitment Push Model SOP Instance [PS 3.4]</summary>
		public static DcmUID StorageCommitmentPushModelSOPInstance = new DcmUID("1.2.840.10008.1.20.1.1", "Storage Commitment Push Model SOP Instance", UidType.SOPInstance);

		/// <summary>SOP Class: Storage Commitment Pull Model SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID StorageCommitmentPullModelSOPClassRETIRED = new DcmUID("1.2.840.10008.1.20.2", "Storage Commitment Pull Model SOP Class", UidType.SOPClass);

		/// <summary>Well-known SOP Instance: Storage Commitment Pull Model SOP Instance [PS 3.4] (Retired)</summary>
		public static DcmUID StorageCommitmentPullModelSOPInstanceRETIRED = new DcmUID("1.2.840.10008.1.20.2.1", "Storage Commitment Pull Model SOP Instance", UidType.SOPInstance);

		/// <summary>SOP Class: Media Storage Directory Storage [PS 3.4]</summary>
		public static DcmUID MediaStorageDirectoryStorage = new DcmUID("1.2.840.10008.1.3.10", "Media Storage Directory Storage", UidType.SOPClass);

		/// <summary>Well-known frame of reference: Talairach Brain Atlas Frame of Reference []</summary>
		public static DcmUID TalairachBrainAtlasFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.1", "Talairach Brain Atlas Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 GRAY Frame of Reference []</summary>
		public static DcmUID SPM2GRAYFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.10", "SPM2 GRAY Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 WHITE Frame of Reference []</summary>
		public static DcmUID SPM2WHITEFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.11", "SPM2 WHITE Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 CSF Frame of Reference []</summary>
		public static DcmUID SPM2CSFFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.12", "SPM2 CSF Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 BRAINMASK Frame of Reference []</summary>
		public static DcmUID SPM2BRAINMASKFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.13", "SPM2 BRAINMASK Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 AVG305T1 Frame of Reference []</summary>
		public static DcmUID SPM2AVG305T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.14", "SPM2 AVG305T1 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 AVG152T1 Frame of Reference []</summary>
		public static DcmUID SPM2AVG152T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.15", "SPM2 AVG152T1 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 AVG152T2 Frame of Reference []</summary>
		public static DcmUID SPM2AVG152T2FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.16", "SPM2 AVG152T2 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 AVG152PD Frame of Reference []</summary>
		public static DcmUID SPM2AVG152PDFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.17", "SPM2 AVG152PD Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 SINGLESUBJT1 Frame of Reference []</summary>
		public static DcmUID SPM2SINGLESUBJT1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.18", "SPM2 SINGLESUBJT1 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 T1 Frame of Reference []</summary>
		public static DcmUID SPM2T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.2", "SPM2 T1 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 T2 Frame of Reference []</summary>
		public static DcmUID SPM2T2FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.3", "SPM2 T2 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 PD Frame of Reference []</summary>
		public static DcmUID SPM2PDFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.4", "SPM2 PD Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 EPI Frame of Reference []</summary>
		public static DcmUID SPM2EPIFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.5", "SPM2 EPI Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 FIL T1 Frame of Reference []</summary>
		public static DcmUID SPM2FILT1FrameOfReference = new DcmUID("1.2.840.10008.1.4.1.6", "SPM2 FIL T1 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 PET Frame of Reference []</summary>
		public static DcmUID SPM2PETFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.7", "SPM2 PET Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 TRANSM Frame of Reference []</summary>
		public static DcmUID SPM2TRANSMFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.8", "SPM2 TRANSM Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: SPM2 SPECT Frame of Reference []</summary>
		public static DcmUID SPM2SPECTFrameOfReference = new DcmUID("1.2.840.10008.1.4.1.9", "SPM2 SPECT Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: ICBM 452 T1 Frame of Reference []</summary>
		public static DcmUID ICBM452T1FrameOfReference = new DcmUID("1.2.840.10008.1.4.2.1", "ICBM 452 T1 Frame of Reference", UidType.FrameOfReference);

		/// <summary>Well-known frame of reference: ICBM Single Subject MRI Frame of Reference []</summary>
		public static DcmUID ICBMSingleSubjectMRIFrameOfReference = new DcmUID("1.2.840.10008.1.4.2.2", "ICBM Single Subject MRI Frame of Reference", UidType.FrameOfReference);

		/// <summary>SOP Class: Procedural Event Logging SOP Class [PS 3.4]</summary>
		public static DcmUID ProceduralEventLoggingSOPClass = new DcmUID("1.2.840.10008.1.40", "Procedural Event Logging SOP Class", UidType.SOPClass);

		/// <summary>Well-known SOP Instance: Procedural Event Logging SOP Instance [PS 3.4]</summary>
		public static DcmUID ProceduralEventLoggingSOPInstance = new DcmUID("1.2.840.10008.1.40.1", "Procedural Event Logging SOP Instance", UidType.SOPInstance);

		/// <summary>SOP Class: Substance Administration Logging SOP Class [PS 3.4]</summary>
		public static DcmUID SubstanceAdministrationLoggingSOPClass = new DcmUID("1.2.840.10008.1.42", "Substance Administration Logging SOP Class", UidType.SOPClass);

		/// <summary>Well-known SOP Instance: Substance Administration Logging SOP Instance [PS 3.4]</summary>
		public static DcmUID SubstanceAdministrationLoggingSOPInstance = new DcmUID("1.2.840.10008.1.42.1", "Substance Administration Logging SOP Instance", UidType.SOPInstance);

		/// <summary>SOP Class: Basic Study Content Notification SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID BasicStudyContentNotificationSOPClassRETIRED = new DcmUID("1.2.840.10008.1.9", "Basic Study Content Notification SOP Class", UidType.SOPClass);

		/// <summary>LDAP OID: dicomDeviceName [PS 3.15]</summary>
		public static DcmUID LDAPDicomDeviceName = new DcmUID("1.2.840.10008.15.0.3.1", "dicomDeviceName", UidType.LDAP);

		/// <summary>LDAP OID: dicomAssociationInitiator [PS 3.15]</summary>
		public static DcmUID LDAPDicomAssociationInitiator = new DcmUID("1.2.840.10008.15.0.3.10", "dicomAssociationInitiator", UidType.LDAP);

		/// <summary>LDAP OID: dicomAssociationAcceptor [PS 3.15]</summary>
		public static DcmUID LDAPDicomAssociationAcceptor = new DcmUID("1.2.840.10008.15.0.3.11", "dicomAssociationAcceptor", UidType.LDAP);

		/// <summary>LDAP OID: dicomHostname [PS 3.15]</summary>
		public static DcmUID LDAPDicomHostname = new DcmUID("1.2.840.10008.15.0.3.12", "dicomHostname", UidType.LDAP);

		/// <summary>LDAP OID: dicomPort [PS 3.15]</summary>
		public static DcmUID LDAPDicomPort = new DcmUID("1.2.840.10008.15.0.3.13", "dicomPort", UidType.LDAP);

		/// <summary>LDAP OID: dicomSOPClass [PS 3.15]</summary>
		public static DcmUID LDAPDicomSOPClass = new DcmUID("1.2.840.10008.15.0.3.14", "dicomSOPClass", UidType.LDAP);

		/// <summary>LDAP OID: dicomTransferRole [PS 3.15]</summary>
		public static DcmUID LDAPDicomTransferRole = new DcmUID("1.2.840.10008.15.0.3.15", "dicomTransferRole", UidType.LDAP);

		/// <summary>LDAP OID: dicomTransferSyntax [PS 3.15]</summary>
		public static DcmUID LDAPDicomTransferSyntax = new DcmUID("1.2.840.10008.15.0.3.16", "dicomTransferSyntax", UidType.LDAP);

		/// <summary>LDAP OID: dicomPrimaryDeviceType [PS 3.15]</summary>
		public static DcmUID LDAPDicomPrimaryDeviceType = new DcmUID("1.2.840.10008.15.0.3.17", "dicomPrimaryDeviceType", UidType.LDAP);

		/// <summary>LDAP OID: dicomRelatedDeviceReference [PS 3.15]</summary>
		public static DcmUID LDAPDicomRelatedDeviceReference = new DcmUID("1.2.840.10008.15.0.3.18", "dicomRelatedDeviceReference", UidType.LDAP);

		/// <summary>LDAP OID: dicomPreferredCalledAETitle [PS 3.15]</summary>
		public static DcmUID LDAPDicomPreferredCalledAETitle = new DcmUID("1.2.840.10008.15.0.3.19", "dicomPreferredCalledAETitle", UidType.LDAP);

		/// <summary>LDAP OID: dicomDescription [PS 3.15]</summary>
		public static DcmUID LDAPDicomDescription = new DcmUID("1.2.840.10008.15.0.3.2", "dicomDescription", UidType.LDAP);

		/// <summary>LDAP OID: dicomTLSCyphersuite [PS 3.15]</summary>
		public static DcmUID LDAPDicomTLSCyphersuite = new DcmUID("1.2.840.10008.15.0.3.20", "dicomTLSCyphersuite", UidType.LDAP);

		/// <summary>LDAP OID: dicomAuthorizedNodeCertificateReference [PS 3.15]</summary>
		public static DcmUID LDAPDicomAuthorizedNodeCertificateReference = new DcmUID("1.2.840.10008.15.0.3.21", "dicomAuthorizedNodeCertificateReference", UidType.LDAP);

		/// <summary>LDAP OID: dicomThisNodeCertificateReference [PS 3.15]</summary>
		public static DcmUID LDAPDicomThisNodeCertificateReference = new DcmUID("1.2.840.10008.15.0.3.22", "dicomThisNodeCertificateReference", UidType.LDAP);

		/// <summary>LDAP OID: dicomInstalled [PS 3.15]</summary>
		public static DcmUID LDAPDicomInstalled = new DcmUID("1.2.840.10008.15.0.3.23", "dicomInstalled", UidType.LDAP);

		/// <summary>LDAP OID: dicomStationName [PS 3.15]</summary>
		public static DcmUID LDAPDicomStationName = new DcmUID("1.2.840.10008.15.0.3.24", "dicomStationName", UidType.LDAP);

		/// <summary>LDAP OID: dicomDeviceSerialNumber [PS 3.15]</summary>
		public static DcmUID LDAPDicomDeviceSerialNumber = new DcmUID("1.2.840.10008.15.0.3.25", "dicomDeviceSerialNumber", UidType.LDAP);

		/// <summary>LDAP OID: dicomInstitutionName [PS 3.15]</summary>
		public static DcmUID LDAPDicomInstitutionName = new DcmUID("1.2.840.10008.15.0.3.26", "dicomInstitutionName", UidType.LDAP);

		/// <summary>LDAP OID: dicomInstitutionAddress [PS 3.15]</summary>
		public static DcmUID LDAPDicomInstitutionAddress = new DcmUID("1.2.840.10008.15.0.3.27", "dicomInstitutionAddress", UidType.LDAP);

		/// <summary>LDAP OID: dicomInstitutionDepartmentName [PS 3.15]</summary>
		public static DcmUID LDAPDicomInstitutionDepartmentName = new DcmUID("1.2.840.10008.15.0.3.28", "dicomInstitutionDepartmentName", UidType.LDAP);

		/// <summary>LDAP OID: dicomIssuerOfPatientID [PS 3.15]</summary>
		public static DcmUID LDAPDicomIssuerOfPatientID = new DcmUID("1.2.840.10008.15.0.3.29", "dicomIssuerOfPatientID", UidType.LDAP);

		/// <summary>LDAP OID: dicomManufacturer [PS 3.15]</summary>
		public static DcmUID LDAPDicomManufacturer = new DcmUID("1.2.840.10008.15.0.3.3", "dicomManufacturer", UidType.LDAP);

		/// <summary>LDAP OID: dicomPreferredCallingAETitle [PS 3.15]</summary>
		public static DcmUID LDAPDicomPreferredCallingAETitle = new DcmUID("1.2.840.10008.15.0.3.30", "dicomPreferredCallingAETitle", UidType.LDAP);

		/// <summary>LDAP OID: dicomSupportedCharacterSet [PS 3.15]</summary>
		public static DcmUID LDAPDicomSupportedCharacterSet = new DcmUID("1.2.840.10008.15.0.3.31", "dicomSupportedCharacterSet", UidType.LDAP);

		/// <summary>LDAP OID: dicomManufacturerModelName [PS 3.15]</summary>
		public static DcmUID LDAPDicomManufacturerModelName = new DcmUID("1.2.840.10008.15.0.3.4", "dicomManufacturerModelName", UidType.LDAP);

		/// <summary>LDAP OID: dicomSoftwareVersion [PS 3.15]</summary>
		public static DcmUID LDAPDicomSoftwareVersion = new DcmUID("1.2.840.10008.15.0.3.5", "dicomSoftwareVersion", UidType.LDAP);

		/// <summary>LDAP OID: dicomVendorData [PS 3.15]</summary>
		public static DcmUID LDAPDicomVendorData = new DcmUID("1.2.840.10008.15.0.3.6", "dicomVendorData", UidType.LDAP);

		/// <summary>LDAP OID: dicomAETitle [PS 3.15]</summary>
		public static DcmUID LDAPDicomAETitle = new DcmUID("1.2.840.10008.15.0.3.7", "dicomAETitle", UidType.LDAP);

		/// <summary>LDAP OID: dicomNetworkConnectionReference [PS 3.15]</summary>
		public static DcmUID LDAPDicomNetworkConnectionReference = new DcmUID("1.2.840.10008.15.0.3.8", "dicomNetworkConnectionReference", UidType.LDAP);

		/// <summary>LDAP OID: dicomApplicationCluster [PS 3.15]</summary>
		public static DcmUID LDAPDicomApplicationCluster = new DcmUID("1.2.840.10008.15.0.3.9", "dicomApplicationCluster", UidType.LDAP);

		/// <summary>LDAP OID: dicomConfigurationRoot [PS 3.15]</summary>
		public static DcmUID LDAPDicomConfigurationRoot = new DcmUID("1.2.840.10008.15.0.4.1", "dicomConfigurationRoot", UidType.LDAP);

		/// <summary>LDAP OID: dicomDevicesRoot [PS 3.15]</summary>
		public static DcmUID LDAPDicomDevicesRoot = new DcmUID("1.2.840.10008.15.0.4.2", "dicomDevicesRoot", UidType.LDAP);

		/// <summary>LDAP OID: dicomUniqueAETitlesRegistryRoot [PS 3.15]</summary>
		public static DcmUID LDAPDicomUniqueAETitlesRegistryRoot = new DcmUID("1.2.840.10008.15.0.4.3", "dicomUniqueAETitlesRegistryRoot", UidType.LDAP);

		/// <summary>LDAP OID: dicomDevice [PS 3.15]</summary>
		public static DcmUID LDAPDicomDevice = new DcmUID("1.2.840.10008.15.0.4.4", "dicomDevice", UidType.LDAP);

		/// <summary>LDAP OID: dicomNetworkAE [PS 3.15]</summary>
		public static DcmUID LDAPDicomNetworkAE = new DcmUID("1.2.840.10008.15.0.4.5", "dicomNetworkAE", UidType.LDAP);

		/// <summary>LDAP OID: dicomNetworkConnection [PS 3.15]</summary>
		public static DcmUID LDAPDicomNetworkConnection = new DcmUID("1.2.840.10008.15.0.4.6", "dicomNetworkConnection", UidType.LDAP);

		/// <summary>LDAP OID: dicomUniqueAETitle [PS 3.15]</summary>
		public static DcmUID LDAPDicomUniqueAETitle = new DcmUID("1.2.840.10008.15.0.4.7", "dicomUniqueAETitle", UidType.LDAP);

		/// <summary>LDAP OID: dicomTransferCapability [PS 3.15]</summary>
		public static DcmUID LDAPDicomTransferCapability = new DcmUID("1.2.840.10008.15.0.4.8", "dicomTransferCapability", UidType.LDAP);

		/// <summary>Coding Scheme: DICOM Controlled Terminology [PS 3.16]</summary>
		public static DcmUID DICOMControlledTerminology = new DcmUID("1.2.840.10008.2.16.4", "DICOM Controlled Terminology", UidType.CodingScheme);

		/// <summary>DICOM UIDs as a Coding Scheme: DICOM UID Registry [PS 3.6]</summary>
		public static DcmUID DICOMUIDRegistry = new DcmUID("1.2.840.10008.2.6.1", "DICOM UID Registry", UidType.CodingScheme);

		/// <summary>Application Context Name: DICOM Application Context Name [PS 3.7]</summary>
		public static DcmUID DICOMApplicationContextName = new DcmUID("1.2.840.10008.3.1.1.1", "DICOM Application Context Name", UidType.ApplicationContextName);

		/// <summary>SOP Class: Detached Patient Management SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedPatientManagementSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.1.1", "Detached Patient Management SOP Class", UidType.SOPClass);

		/// <summary>Meta SOP Class: Detached Patient Management Meta SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedPatientManagementMetaSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.1.4", "Detached Patient Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>SOP Class: Detached Visit Management SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedVisitManagementSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.2.1", "Detached Visit Management SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Detached Study Management SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedStudyManagementSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.3.1", "Detached Study Management SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Study Component Management SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID StudyComponentManagementSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.3.2", "Study Component Management SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Modality Performed Procedure Step SOP Class [PS 3.4]</summary>
		public static DcmUID ModalityPerformedProcedureStepSOPClass = new DcmUID("1.­2.840.10008.3.1.2.3.3", "Modality Performed Procedure Step SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Modality Performed Procedure Step Retrieve SOP Class [PS 3.4]</summary>
		public static DcmUID ModalityPerformedProcedureStepRetrieveSOPClass = new DcmUID("1.­2.840.10008.3.1.2.3.4", "Modality Performed Procedure Step Retrieve SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Modality Performed Procedure Step Notification SOP Class [PS 3.4]</summary>
		public static DcmUID ModalityPerformedProcedureStepNotificationSOPClass = new DcmUID("1.­2.840.10008.3.1.2.3.5", "Modality Performed Procedure Step Notification SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Detached Results Management SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedResultsManagementSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.5.1", "Detached Results Management SOP Class", UidType.SOPClass);

		/// <summary>Meta SOP Class: Detached Results Management Meta SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedResultsManagementMetaSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.5.4", "Detached Results Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>Meta SOP Class: Detached Study Management Meta SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedStudyManagementMetaSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.5.5", "Detached Study Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>SOP Class: Detached Interpretation Management SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID DetachedInterpretationManagementSOPClassRETIRED = new DcmUID("1.2.840.10008.3.1.2.6.1", "Detached Interpretation Management SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Basic Film Session SOP Class [PS 3.4]</summary>
		public static DcmUID BasicFilmSessionSOPClass = new DcmUID("1.2.840.10008.5.1.1.1", "Basic Film Session SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Print Job SOP Class [PS 3.4]</summary>
		public static DcmUID PrintJobSOPClass = new DcmUID("1.2.840.10008.5.1.1.14", "Print Job SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Basic Annotation Box SOP Class [PS 3.4]</summary>
		public static DcmUID BasicAnnotationBoxSOPClass = new DcmUID("1.2.840.10008.5.1.1.15", "Basic Annotation Box SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Printer SOP Class [PS 3.4]</summary>
		public static DcmUID PrinterSOPClass = new DcmUID("1.2.840.10008.5.1.1.16", "Printer SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Printer Configuration Retrieval SOP Class [PS 3.4]</summary>
		public static DcmUID PrinterConfigurationRetrievalSOPClass = new DcmUID("1.2.840.10008.5.1.1.16.376", "Printer Configuration Retrieval SOP Class", UidType.SOPClass);

		/// <summary>Well-known Printer SOP Instance: Printer SOP Instance [PS 3.4]</summary>
		public static DcmUID PrinterSOPInstance = new DcmUID("1.2.840.10008.5.1.1.17", "Printer SOP Instance", UidType.SOPInstance);

		/// <summary>Well-known Printer SOP Instance: Printer Configuration Retrieval SOP Instance [PS 3.4]</summary>
		public static DcmUID PrinterConfigurationRetrievalSOPInstance = new DcmUID("1.2.840.10008.5.1.1.17.376", "Printer Configuration Retrieval SOP Instance", UidType.SOPInstance);

		/// <summary>Meta SOP Class: Basic Color Print Management Meta SOP Class [PS 3.4]</summary>
		public static DcmUID BasicColorPrintManagementMetaSOPClass = new DcmUID("1.2.840.10008.5.1.1.18", "Basic Color Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>Meta SOP Class: Referenced Color Print Management Meta SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID ReferencedColorPrintManagementMetaSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.18.1", "Referenced Color Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>SOP Class: Basic Film Box SOP Class [PS 3.4]</summary>
		public static DcmUID BasicFilmBoxSOPClass = new DcmUID("1.2.840.10008.5.1.1.2", "Basic Film Box SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: VOI LUT Box SOP Class [PS 3.4]</summary>
		public static DcmUID VOILUTBoxSOPClass = new DcmUID("1.2.840.10008.5.1.1.22", "VOI LUT Box SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Presentation LUT SOP Class [PS 3.4]</summary>
		public static DcmUID PresentationLUTSOPClass = new DcmUID("1.2.840.10008.5.1.1.23", "Presentation LUT SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Image Overlay Box SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID ImageOverlayBoxSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.24", "Image Overlay Box SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Basic Print Image Overlay Box SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID BasicPrintImageOverlayBoxSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.24.1", "Basic Print Image Overlay Box SOP Class", UidType.SOPClass);

		/// <summary>Well-known Print Queue SOP Instance: Print Queue SOP Instance [PS 3.4] (Retired)</summary>
		public static DcmUID PrintQueueSOPInstanceRETIRED = new DcmUID("1.2.840.10008.5.1.1.25", "Print Queue SOP Instance", UidType.SOPInstance);

		/// <summary>SOP Class: Print Queue Management SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID PrintQueueManagementSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.26", "Print Queue Management SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Stored Print Storage SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID StoredPrintStorageSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.27", "Stored Print Storage SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Hardcopy Grayscale Image Storage SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID HardcopyGrayscaleImageStorageSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.29", "Hardcopy Grayscale Image Storage SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Hardcopy Color Image Storage SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID HardcopyColorImageStorageSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.30", "Hardcopy Color Image Storage SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Pull Print Request SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID PullPrintRequestSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.31", "Pull Print Request SOP Class", UidType.SOPClass);

		/// <summary>Meta SOP Class: Pull Stored Print Management Meta SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID PullStoredPrintManagementMetaSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.32", "Pull Stored Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>SOP Class: Media Creation Management SOP Class UID [PS3.4]</summary>
		public static DcmUID MediaCreationManagementSOPClassUID = new DcmUID("1.2.840.10008.5.1.1.33", "Media Creation Management SOP Class UID", UidType.SOPClass);

		/// <summary>SOP Class: Basic Grayscale Image Box SOP Class [PS 3.4]</summary>
		public static DcmUID BasicGrayscaleImageBoxSOPClass = new DcmUID("1.2.840.10008.5.1.1.4", "Basic Grayscale Image Box SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Basic Color Image Box SOP Class [PS 3.4]</summary>
		public static DcmUID BasicColorImageBoxSOPClass = new DcmUID("1.2.840.10008.5.1.1.4.1", "Basic Color Image Box SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Referenced Image Box SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID ReferencedImageBoxSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.4.2", "Referenced Image Box SOP Class", UidType.SOPClass);

		/// <summary>Meta SOP Class: Basic Grayscale Print Management Meta SOP Class [PS 3.4]</summary>
		public static DcmUID BasicGrayscalePrintManagementMetaSOPClass = new DcmUID("1.2.840.10008.5.1.1.9", "Basic Grayscale Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>Meta SOP Class: Referenced Grayscale Print Management Meta SOP Class [PS 3.4] (Retired)</summary>
		public static DcmUID ReferencedGrayscalePrintManagementMetaSOPClassRETIRED = new DcmUID("1.2.840.10008.5.1.1.9.1", "Referenced Grayscale Print Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>SOP Class: Computed Radiography Image Storage [PS 3.4]</summary>
		public static DcmUID ComputedRadiographyImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.1", "Computed Radiography Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Digital X-Ray Image Storage – For Presentation [PS 3.4]</summary>
		public static DcmUID DigitalXRayImageStorageForPresentation = new DcmUID("1.2.840.10008.5.1.4.1.1.1.1", "Digital X-Ray Image Storage – For Presentation", UidType.SOPClass);

		/// <summary>SOP Class: Digital X-Ray Image Storage – For Processing [PS 3.4]</summary>
		public static DcmUID DigitalXRayImageStorageForProcessing = new DcmUID("1.2.840.10008.5.1.4.1.1.1.1.1", "Digital X-Ray Image Storage – For Processing", UidType.SOPClass);

		/// <summary>SOP Class: Digital Mammography X-Ray Image Storage – For Presentation [PS 3.4]</summary>
		public static DcmUID DigitalMammographyXRayImageStorageForPresentation = new DcmUID("1.2.840.10008.5.1.4.1.1.1.2", "Digital Mammography X-Ray Image Storage – For Presentation", UidType.SOPClass);

		/// <summary>SOP Class: Digital Mammography X-Ray Image Storage – For Processing [PS 3.4]</summary>
		public static DcmUID DigitalMammographyXRayImageStorageForProcessing = new DcmUID("1.2.840.10008.5.1.4.1.1.1.2.1", "Digital Mammography X-Ray Image Storage – For Processing", UidType.SOPClass);

		/// <summary>SOP Class: Digital Intra-oral X-Ray Image Storage – For Presentation [PS 3.4]</summary>
		public static DcmUID DigitalIntraoralXRayImageStorageForPresentation = new DcmUID("1.2.840.10008.5.1.4.1.1.1.3", "Digital Intra-oral X-Ray Image Storage – For Presentation", UidType.SOPClass);

		/// <summary>SOP Class: Digital Intra-oral X-Ray Image Storage – For Processing [PS 3.4]</summary>
		public static DcmUID DigitalIntraoralXRayImageStorageForProcessing = new DcmUID("1.2.840.10008.5.1.4.1.1.1.3.1", "Digital Intra-oral X-Ray Image Storage – For Processing", UidType.SOPClass);

		/// <summary>SOP Class: Standalone Modality LUT Storage [PS 3.4] (Retired)</summary>
		public static DcmUID StandaloneModalityLUTStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.10", "Standalone Modality LUT Storage", UidType.SOPClass);

		/// <summary>SOP Class: Encapsulated PDF Storage [PS 3.4]</summary>
		public static DcmUID EncapsulatedPDFStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.104.1", "Encapsulated PDF Storage", UidType.SOPClass);

		/// <summary>SOP Class: Encapsulated CDA Storage [PS 3.4]</summary>
		public static DcmUID EncapsulatedCDAStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.104.2", "Encapsulated CDA Storage", UidType.SOPClass);

		/// <summary>SOP Class: Standalone VOI LUT Storage [PS 3.4] (Retired)</summary>
		public static DcmUID StandaloneVOILUTStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.11", "Standalone VOI LUT Storage", UidType.SOPClass);

		/// <summary>SOP Class: Grayscale Softcopy Presentation State Storage SOP Class [PS 3.4]</summary>
		public static DcmUID GrayscaleSoftcopyPresentationStateStorageSOPClass = new DcmUID("1.2.840.10008.5.1.4.1.1.11.1", "Grayscale Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Color Softcopy Presentation State Storage SOP Class [PS 3.4]</summary>
		public static DcmUID ColorSoftcopyPresentationStateStorageSOPClass = new DcmUID("1.2.840.10008.5.1.4.1.1.11.2", "Color Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Pseudo-Color Softcopy Presentation State Storage SOP Class [PS 3.4]</summary>
		public static DcmUID PseudoColorSoftcopyPresentationStateStorageSOPClass = new DcmUID("1.2.840.10008.5.1.4.1.1.11.3", "Pseudo-Color Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Blending Softcopy Presentation State Storage SOP Class [PS 3.4]</summary>
		public static DcmUID BlendingSoftcopyPresentationStateStorageSOPClass = new DcmUID("1.2.840.10008.5.1.4.1.1.11.4", "Blending Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: X-Ray Angiographic Image Storage [PS 3.4]</summary>
		public static DcmUID XRayAngiographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.1", "X-Ray Angiographic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Enhanced XA Image Storage [PS 3.4]</summary>
		public static DcmUID EnhancedXAImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.1.1", "Enhanced XA Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: X-Ray Radiofluoroscopic Image Storage [PS 3.4]</summary>
		public static DcmUID XRayRadiofluoroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.2", "X-Ray Radiofluoroscopic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Enhanced XRF Image Storage [PS 3.4]</summary>
		public static DcmUID EnhancedXRFImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.12.2.1", "Enhanced XRF Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: X-Ray Angiographic Bi-Plane Image Storage [PS 3.4] (Retired)</summary>
		public static DcmUID XRayAngiographicBiPlaneImageStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.12.3", "X-Ray Angiographic Bi-Plane Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Positron Emission Tomography Image Storage [PS 3.4]</summary>
		public static DcmUID PositronEmissionTomographyImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.128", "Positron Emission Tomography Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Standalone PET Curve Storage [PS 3.4] (Retired)</summary>
		public static DcmUID StandalonePETCurveStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.129", "Standalone PET Curve Storage", UidType.SOPClass);

		/// <summary>SOP Class: X-Ray 3D Angiographic Image Storage [PS 3.4]</summary>
		public static DcmUID XRay3DAngiographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.13.1.1", "X-Ray 3D Angiographic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: X-Ray 3D Craniofacial Image Storage [PS 3.4]</summary>
		public static DcmUID XRay3DCraniofacialImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.13.1.2", "X-Ray 3D Craniofacial Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: CT Image Storage [PS 3.4]</summary>
		public static DcmUID CTImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.2", "CT Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Enhanced CT Image Storage [PS 3.4]</summary>
		public static DcmUID EnhancedCTImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.2.1", "Enhanced CT Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Nuclear Medicine Image Storage [PS 3.4]</summary>
		public static DcmUID NuclearMedicineImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.20", "Nuclear Medicine Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ultrasound Multi-frame Image Storage [PS 3.4] (Retired)</summary>
		public static DcmUID UltrasoundMultiframeImageStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.3", "Ultrasound Multi-frame Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ultrasound Multi-frame Image Storage [PS 3.4]</summary>
		public static DcmUID UltrasoundMultiframeImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.3.1", "Ultrasound Multi-frame Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: MR Image Storage [PS 3.4]</summary>
		public static DcmUID MRImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.4", "MR Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Enhanced MR Image Storage [PS 3.4]</summary>
		public static DcmUID EnhancedMRImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.4.1", "Enhanced MR Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: MR Spectroscopy Storage [PS 3.4]</summary>
		public static DcmUID MRSpectroscopyStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.4.2", "MR Spectroscopy Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Image Storage [PS 3.4]</summary>
		public static DcmUID RTImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.1", "RT Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Dose Storage [PS 3.4]</summary>
		public static DcmUID RTDoseStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.2", "RT Dose Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Structure Set Storage [PS 3.4]</summary>
		public static DcmUID RTStructureSetStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.3", "RT Structure Set Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Beams Treatment Record Storage [PS 3.4]</summary>
		public static DcmUID RTBeamsTreatmentRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.4", "RT Beams Treatment Record Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Plan Storage [PS 3.4]</summary>
		public static DcmUID RTPlanStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.5", "RT Plan Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Brachy Treatment Record Storage [PS 3.4]</summary>
		public static DcmUID RTBrachyTreatmentRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.6", "RT Brachy Treatment Record Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Treatment Summary Record Storage [PS 3.4]</summary>
		public static DcmUID RTTreatmentSummaryRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.7", "RT Treatment Summary Record Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Ion Plan Storage [PS 3.4]</summary>
		public static DcmUID RTIonPlanStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.8", "RT Ion Plan Storage", UidType.SOPClass);

		/// <summary>SOP Class: RT Ion Beams Treatment Record Storage [PS 3.4]</summary>
		public static DcmUID RTIonBeamsTreatmentRecordStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.481.9", "RT Ion Beams Treatment Record Storage", UidType.SOPClass);

		/// <summary>SOP Class: Nuclear Medicine Image Storage [PS 3.4] (Retired)</summary>
		public static DcmUID NuclearMedicineImageStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.5", "Nuclear Medicine Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ultrasound Image Storage [PS 3.4] (Retired)</summary>
		public static DcmUID UltrasoundImageStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.6", "Ultrasound Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ultrasound Image Storage [PS 3.4]</summary>
		public static DcmUID UltrasoundImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.6.1", "Ultrasound Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Raw Data Storage [PS 3.4]</summary>
		public static DcmUID RawDataStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66", "Raw Data Storage", UidType.SOPClass);

		/// <summary>SOP Class: Spatial Registration Storage [PS 3.4]</summary>
		public static DcmUID SpatialRegistrationStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66.1", "Spatial Registration Storage", UidType.SOPClass);

		/// <summary>SOP Class: Spatial Fiducials Storage [PS 3.4]</summary>
		public static DcmUID SpatialFiducialsStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66.2", "Spatial Fiducials Storage", UidType.SOPClass);

		/// <summary>SOP Class: Deformable Spatial Registration Storage [PS 3.4]</summary>
		public static DcmUID DeformableSpatialRegistrationStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66.3", "Deformable Spatial Registration Storage", UidType.SOPClass);

		/// <summary>SOP Class: Segmentation Storage [PS 3.4]</summary>
		public static DcmUID SegmentationStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.66.4", "Segmentation Storage", UidType.SOPClass);

		/// <summary>SOP Class: Real World Value Mapping Storage [PS 3.4]</summary>
		public static DcmUID RealWorldValueMappingStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.67", "Real World Value Mapping Storage", UidType.SOPClass);

		/// <summary>SOP Class: Secondary Capture Image Storage [PS 3.4]</summary>
		public static DcmUID SecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7", "Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Multi-frame Single Bit Secondary Capture Image Storage [PS 3.4]</summary>
		public static DcmUID MultiframeSingleBitSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.1", "Multi-frame Single Bit Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Multi-frame Grayscale Byte Secondary Capture Image Storage [PS 3.4]</summary>
		public static DcmUID MultiframeGrayscaleByteSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.2", "Multi-frame Grayscale Byte Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Multi-frame Grayscale Word Secondary Capture Image Storage [PS 3.4]</summary>
		public static DcmUID MultiframeGrayscaleWordSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.3", "Multi-frame Grayscale Word Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Multi-frame True Color Secondary Capture Image Storage [PS 3.4]</summary>
		public static DcmUID MultiframeTrueColorSecondaryCaptureImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.7.4", "Multi-frame True Color Secondary Capture Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: VL Image Storage - Trial [PS 3.4] (Retired)</summary>
		public static DcmUID VLImageStorageTrialRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1", "VL Image Storage - Trial", UidType.SOPClass);

		/// <summary>SOP Class: VL Endoscopic Image Storage [PS 3.4]</summary>
		public static DcmUID VLEndoscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.1", "VL Endoscopic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Video Endoscopic Image Storage [PS 3.4]</summary>
		public static DcmUID VideoEndoscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.1.1", "Video Endoscopic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: VL Microscopic Image Storage [PS 3.4]</summary>
		public static DcmUID VLMicroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.2", "VL Microscopic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Video Microscopic Image Storage [PS 3.4]</summary>
		public static DcmUID VideoMicroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.2.1", "Video Microscopic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: VL Slide-Coordinates Microscopic Image Storage [PS 3.4]</summary>
		public static DcmUID VLSlideCoordinatesMicroscopicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.3", "VL Slide-Coordinates Microscopic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: VL Photographic Image Storage [PS 3.4]</summary>
		public static DcmUID VLPhotographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.4", "VL Photographic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Video Photographic Image Storage [PS 3.4]</summary>
		public static DcmUID VideoPhotographicImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.4.1", "Video Photographic Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ophthalmic Photography 8 Bit Image Storage [PS 3.4]</summary>
		public static DcmUID OphthalmicPhotography8BitImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.5.1", "Ophthalmic Photography 8 Bit Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ophthalmic Photography 16 Bit Image Storage [PS 3.4]</summary>
		public static DcmUID OphthalmicPhotography16BitImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.5.2", "Ophthalmic Photography 16 Bit Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: Stereometric Relationship Storage [PS 3.4]</summary>
		public static DcmUID StereometricRelationshipStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.5.3", "Stereometric Relationship Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ophthalmic Tomography Image Storage [PS 3.4]</summary>
		public static DcmUID OphthalmicTomographyImageStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.77.1.5.4", "Ophthalmic Tomography Image Storage", UidType.SOPClass);

		/// <summary>SOP Class: VL Multi-frame Image Storage – Trial [PS 3.4] (Retired)</summary>
		public static DcmUID VLMultiframeImageStorageTrialRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.77.2", "VL Multi-frame Image Storage – Trial", UidType.SOPClass);

		/// <summary>SOP Class: Standalone Overlay Storage [PS 3.4] (Retired)</summary>
		public static DcmUID StandaloneOverlayStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.8", "Standalone Overlay Storage", UidType.SOPClass);

		/// <summary>SOP Class: Text SR Storage – Trial [PS 3.4] (Retired)</summary>
		public static DcmUID TextSRStorageTrialRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.88.1", "Text SR Storage – Trial", UidType.SOPClass);

		/// <summary>SOP Class: Basic Text SR Storage [PS 3.4]</summary>
		public static DcmUID BasicTextSRStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.11", "Basic Text SR Storage", UidType.SOPClass);

		/// <summary>SOP Class: Audio SR Storage – Trial [PS 3.4] (Retired)</summary>
		public static DcmUID AudioSRStorageTrialRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.88.2", "Audio SR Storage – Trial", UidType.SOPClass);

		/// <summary>SOP Class: Enhanced SR Storage [PS 3.4]</summary>
		public static DcmUID EnhancedSRStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.22", "Enhanced SR Storage", UidType.SOPClass);

		/// <summary>SOP Class: Detail SR Storage – Trial [PS 3.4] (Retired)</summary>
		public static DcmUID DetailSRStorageTrialRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.88.3", "Detail SR Storage – Trial", UidType.SOPClass);

		/// <summary>SOP Class: Comprehensive SR Storage [PS 3.4]</summary>
		public static DcmUID ComprehensiveSRStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.33", "Comprehensive SR Storage", UidType.SOPClass);

		/// <summary>SOP Class: Comprehensive SR Storage – Trial [PS 3.4] (Retired)</summary>
		public static DcmUID ComprehensiveSRStorageTrialRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.88.4", "Comprehensive SR Storage – Trial", UidType.SOPClass);

		/// <summary>SOP Class: Procedure Log Storage [PS 3.4]</summary>
		public static DcmUID ProcedureLogStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.40", "Procedure Log Storage", UidType.SOPClass);

		/// <summary>SOP Class: Mammography CAD SR Storage [PS 3.4]</summary>
		public static DcmUID MammographyCADSRStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.50", "Mammography CAD SR Storage", UidType.SOPClass);

		/// <summary>SOP Class: Key Object Selection Document Storage [PS 3.4]</summary>
		public static DcmUID KeyObjectSelectionDocumentStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.59", "Key Object Selection Document Storage", UidType.SOPClass);

		/// <summary>SOP Class: Chest CAD SR Storage [PS 3.4]</summary>
		public static DcmUID ChestCADSRStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.65", "Chest CAD SR Storage", UidType.SOPClass);

		/// <summary>SOP Class: X-Ray Radiation Dose SR Storage [PS 3.4]</summary>
		public static DcmUID XRayRadiationDoseSRStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.88.67", "X-Ray Radiation Dose SR Storage", UidType.SOPClass);

		/// <summary>SOP Class: Standalone Curve Storage [PS 3.4] (Retired)</summary>
		public static DcmUID StandaloneCurveStorageRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.9", "Standalone Curve Storage", UidType.SOPClass);

		/// <summary>SOP Class: Waveform Storage - Trial [PS 3.4] (Retired)</summary>
		public static DcmUID WaveformStorageTrialRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.1.9.1", "Waveform Storage - Trial", UidType.SOPClass);

		/// <summary>SOP Class: 12-lead ECG Waveform Storage [PS 3.4]</summary>
		public static DcmUID TwelveLeadECGWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.1.1", "12-lead ECG Waveform Storage", UidType.SOPClass);

		/// <summary>SOP Class: General ECG Waveform Storage [PS 3.4]</summary>
		public static DcmUID GeneralECGWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.1.2", "General ECG Waveform Storage", UidType.SOPClass);

		/// <summary>SOP Class: Ambulatory ECG Waveform Storage [PS 3.4]</summary>
		public static DcmUID AmbulatoryECGWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.1.3", "Ambulatory ECG Waveform Storage", UidType.SOPClass);

		/// <summary>SOP Class: Hemodynamic Waveform Storage [PS 3.4]</summary>
		public static DcmUID HemodynamicWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.2.1", "Hemodynamic Waveform Storage", UidType.SOPClass);

		/// <summary>SOP Class: Cardiac Electrophysiology Waveform Storage [PS 3.4]</summary>
		public static DcmUID CardiacElectrophysiologyWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.3.1", "Cardiac Electrophysiology Waveform Storage", UidType.SOPClass);

		/// <summary>SOP Class: Basic Voice Audio Waveform Storage [PS 3.4]</summary>
		public static DcmUID BasicVoiceAudioWaveformStorage = new DcmUID("1.2.840.10008.5.1.4.1.1.9.4.1", "Basic Voice Audio Waveform Storage", UidType.SOPClass);

		/// <summary>SOP Class: Patient Root Query/Retrieve Information Model – FIND [PS 3.4]</summary>
		public static DcmUID PatientRootQueryRetrieveInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.1.2.1.1", "Patient Root Query/Retrieve Information Model – FIND", UidType.SOPClass);

		/// <summary>SOP Class: Patient Root Query/Retrieve Information Model – MOVE [PS 3.4]</summary>
		public static DcmUID PatientRootQueryRetrieveInformationModelMOVE = new DcmUID("1.2.840.10008.5.1.4.1.2.1.2", "Patient Root Query/Retrieve Information Model – MOVE", UidType.SOPClass);

		/// <summary>SOP Class: Patient Root Query/Retrieve Information Model – GET [PS 3.4]</summary>
		public static DcmUID PatientRootQueryRetrieveInformationModelGET = new DcmUID("1.2.840.10008.5.1.4.1.2.1.3", "Patient Root Query/Retrieve Information Model – GET", UidType.SOPClass);

		/// <summary>SOP Class: Study Root Query/Retrieve Information Model – FIND [PS 3.4]</summary>
		public static DcmUID StudyRootQueryRetrieveInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.1.2.2.1", "Study Root Query/Retrieve Information Model – FIND", UidType.SOPClass);

		/// <summary>SOP Class: Study Root Query/Retrieve Information Model – MOVE [PS 3.4]</summary>
		public static DcmUID StudyRootQueryRetrieveInformationModelMOVE = new DcmUID("1.2.840.10008.5.1.4.1.2.2.2", "Study Root Query/Retrieve Information Model – MOVE", UidType.SOPClass);

		/// <summary>SOP Class: Study Root Query/Retrieve Information Model – GET [PS 3.4]</summary>
		public static DcmUID StudyRootQueryRetrieveInformationModelGET = new DcmUID("1.2.840.10008.5.1.4.1.2.2.3", "Study Root Query/Retrieve Information Model – GET", UidType.SOPClass);

		/// <summary>SOP Class: Patient/Study Only Query/Retrieve Information Model - FIND [PS 3.4] (Retired)</summary>
		public static DcmUID PatientStudyOnlyQueryRetrieveInformationModelFINDRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.2.3.1", "Patient/Study Only Query/Retrieve Information Model - FIND", UidType.SOPClass);

		/// <summary>SOP Class: Patient/Study Only Query/Retrieve Information Model - MOVE [PS 3.4] (Retired)</summary>
		public static DcmUID PatientStudyOnlyQueryRetrieveInformationModelMOVERETIRED = new DcmUID("1.2.840.10008.5.1.4.1.2.3.2", "Patient/Study Only Query/Retrieve Information Model - MOVE", UidType.SOPClass);

		/// <summary>SOP Class: Patient/Study Only Query/Retrieve Information Model - GET [PS 3.4] (Retired)</summary>
		public static DcmUID PatientStudyOnlyQueryRetrieveInformationModelGETRETIRED = new DcmUID("1.2.840.10008.5.1.4.1.2.3.3", "Patient/Study Only Query/Retrieve Information Model - GET", UidType.SOPClass);

		/// <summary>SOP Class: Modality Worklist Information Model – FIND [PS 3.4]</summary>
		public static DcmUID ModalityWorklistInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.31", "Modality Worklist Information Model – FIND", UidType.SOPClass);

		/// <summary>Meta SOP Class: General Purpose Worklist Management Meta SOP Class [PS 3.4]</summary>
		public static DcmUID GeneralPurposeWorklistManagementMetaSOPClass = new DcmUID("1.2.840.10008.5.1.4.32", "General Purpose Worklist Management Meta SOP Class", UidType.MetaSOPClass);

		/// <summary>SOP Class: General Purpose Worklist Information Model – FIND [PS 3.4]</summary>
		public static DcmUID GeneralPurposeWorklistInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.32.1", "General Purpose Worklist Information Model – FIND", UidType.SOPClass);

		/// <summary>SOP Class: General Purpose Scheduled Procedure Step SOP Class [PS 3.4]</summary>
		public static DcmUID GeneralPurposeScheduledProcedureStepSOPClass = new DcmUID("1.2.840.10008.5.1.4.32.2", "General Purpose Scheduled Procedure Step SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: General Purpose Performed Procedure Step SOP Class [PS 3.4]</summary>
		public static DcmUID GeneralPurposePerformedProcedureStepSOPClass = new DcmUID("1.2.840.10008.5.1.4.32.3", "General Purpose Performed Procedure Step SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Instance Availability Notification SOP Class [PS 3.4]</summary>
		public static DcmUID InstanceAvailabilityNotificationSOPClass = new DcmUID("1.2.840.10008.5.1.4.33", "Instance Availability Notification SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: RT Beams Delivery Instruction Storage (Supplement 74 Frozen Draft) [PS 3.4]</summary>
		public static DcmUID RTBeamsDeliveryInstructionStorageSupplement74FrozenDraft = new DcmUID("1.2.840.10008.5.1.4.34.1", "RT Beams Delivery Instruction Storage (Supplement 74 Frozen Draft)", UidType.SOPClass);

		/// <summary>SOP Class: RT Conventional Machine Verification (Supplement 74 Frozen Draft) [PS 3.4]</summary>
		public static DcmUID RTConventionalMachineVerificationSupplement74FrozenDraft = new DcmUID("1.2.840.10008.5.1.4.34.2", "RT Conventional Machine Verification (Supplement 74 Frozen Draft)", UidType.SOPClass);

		/// <summary>SOP Class: RT Ion Machine Verification (Supplement 74 Frozen Draft) [PS 3.4]</summary>
		public static DcmUID RTIonMachineVerificationSupplement74FrozenDraft = new DcmUID("1.2.840.10008.5.1.4.34.3", "RT Ion Machine Verification (Supplement 74 Frozen Draft)", UidType.SOPClass);

		/// <summary>Service Class: Unified Worklist and Procedure Step Service Class [PS 3.4]</summary>
		public static DcmUID UnifiedWorklistAndProcedureStepSOPClass = new DcmUID("1.2.840.10008.5.1.4.34.4", "Unified Worklist and Procedure Step Service Class", UidType.SOPClass);

		/// <summary>SOP Class: Unified Procedure Step – Push SOP Class [PS 3.4]</summary>
		public static DcmUID UnifiedProcedureStepPushSOPClass = new DcmUID("1.2.840.10008.5.1.4.34.4.1", "Unified Procedure Step – Push SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Unified Procedure Step – Watch SOP Class [PS 3.4]</summary>
		public static DcmUID UnifiedProcedureStepWatchSOPClass = new DcmUID("1.2.840.10008.5.1.4.34.4.2", "Unified Procedure Step – Watch SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Unified Procedure Step – Pull SOP Class [PS 3.4]</summary>
		public static DcmUID UnifiedProcedureStepPullSOPClass = new DcmUID("1.2.840.10008.5.1.4.34.4.3", "Unified Procedure Step – Pull SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Unified Procedure Step – Event SOP Class [PS 3.4]</summary>
		public static DcmUID UnifiedProcedureStepEventSOPClass = new DcmUID("1.2.840.10008.5.1.4.34.4.4", "Unified Procedure Step – Event SOP Class", UidType.SOPClass);

		/// <summary>Well-known SOP Instance: Unified Worklist and Procedure Step SOP Instance [PS 3.4]</summary>
		public static DcmUID UnifiedWorklistAndProcedureStepSOPInstance = new DcmUID("1.2.840.10008.5.1.4.34.5", "Unified Worklist and Procedure Step SOP Instance", UidType.SOPInstance);

		/// <summary>SOP Class: General Relevant Patient Information Query [PS 3.4]</summary>
		public static DcmUID GeneralRelevantPatientInformationQuery = new DcmUID("1.2.840.10008.5.1.4.37.1", "General Relevant Patient Information Query", UidType.SOPClass);

		/// <summary>SOP Class: Breast Imaging Relevant Patient Information Query [PS 3.4]</summary>
		public static DcmUID BreastImagingRelevantPatientInformationQuery = new DcmUID("1.2.840.10008.5.1.4.37.2", "Breast Imaging Relevant Patient Information Query", UidType.SOPClass);

		/// <summary>SOP Class: Cardiac Relevant Patient Information Query [PS 3.4]</summary>
		public static DcmUID CardiacRelevantPatientInformationQuery = new DcmUID("1.2.840.10008.5.1.4.37.3", "Cardiac Relevant Patient Information Query", UidType.SOPClass);

		/// <summary>SOP Class: Hanging Protocol Storage [PS 3.4]</summary>
		public static DcmUID HangingProtocolStorage = new DcmUID("1.2.840.10008.5.1.4.38.1", "Hanging Protocol Storage", UidType.SOPClass);

		/// <summary>SOP Class: Hanging Protocol Information Model – FIND [PS 3.4]</summary>
		public static DcmUID HangingProtocolInformationModelFIND = new DcmUID("1.2.840.10008.5.1.4.38.2", "Hanging Protocol Information Model – FIND", UidType.SOPClass);

		/// <summary>SOP Class: Hanging Protocol Information Model – MOVE [PS 3.4]</summary>
		public static DcmUID HangingProtocolInformationModelMOVE = new DcmUID("1.2.840.10008.5.1.4.38.3", "Hanging Protocol Information Model – MOVE", UidType.SOPClass);

		/// <summary>SOP Class: Product Characteristics Query SOP Class [PS 3.4]</summary>
		public static DcmUID ProductCharacteristicsQuerySOPClass = new DcmUID("1.2.840.10008.5.1.4.41", "Product Characteristics Query SOP Class", UidType.SOPClass);

		/// <summary>SOP Class: Substance Approval Query SOP Class [PS 3.4]</summary>
		public static DcmUID SubstanceApprovalQuerySOPClass = new DcmUID("1.2.840.10008.5.1.4.42", "Substance Approval Query SOP Class", UidType.SOPClass);
		#endregion
	}
}
