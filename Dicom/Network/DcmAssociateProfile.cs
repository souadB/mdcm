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
using System.IO;
using System.Text;

using Dicom.Data;
using Dicom.Utility;

namespace Dicom.Network {
	[Serializable]
	public class DcmAssociateProfile : XmlSerializable<DcmAssociateProfile> {
		public string Name { get; set; }
		public string Description { get; set; }
		public string Notes { get; set; }

		public string CalledAE { get; set; }
		public string CallingAE { get; set; }
		public string RemoteImplUID { get; set; }
		public string RemoteVersion { get; set; }

		public List<string> TransferSyntaxes { get; set; }
		public List<string> AbstractSyntaxes { get; set; }

		private int GetWeight(bool matchImplementation) {
			int weight = 0;
			if (CalledAE.Contains("*") || CalledAE.Contains("?"))
				weight++;
			if (CallingAE.Contains("*") || CallingAE.Contains("?"))
				weight++;
			if (matchImplementation && RemoteImplUID.Contains("*") || RemoteImplUID.Contains("?"))
				weight++;
			if (matchImplementation && RemoteVersion.Contains("*") || RemoteVersion.Contains("?"))
				weight++;
			return weight;
		}

		public void SetDefault() {
			TransferSyntaxes = new List<string>();
			TransferSyntaxes.AddRange(SupportedTransferSyntaxes);
			AbstractSyntaxes = new List<string>();
			AbstractSyntaxes.AddRange(SupportedStorageSyntaxes);
		}

		public void SetDefaultStorage() {
			TransferSyntaxes = new List<string>();
			TransferSyntaxes.AddRange(SupportedTransferSyntaxes);
			AbstractSyntaxes = new List<string>();
			foreach (DcmUID uid in DcmUIDs.Entries.Values) {
				if (uid.Type == UidType.SOPClass)
					SupportedStorageSyntaxes.Add(uid.UID);
			}
		}

		public void Apply(DcmAssociate associate) {
			foreach (DcmPresContext pc in associate.GetPresentationContexts()) {
				if (AbstractSyntaxes.Contains(pc.AbstractSyntax.UID)) {
					IList<DcmTS> txs = pc.GetTransfers();
					for (int i = 0; i < txs.Count; i++) {
						if (TransferSyntaxes.Contains(txs[i].UID.UID)) {
							pc.SetResult(DcmPresContextResult.Accept, txs[i]);
							break;
						}
					}
					if (pc.Result != DcmPresContextResult.Accept)
						pc.SetResult(DcmPresContextResult.RejectTransferSyntaxesNotSupported);
				}
				else {
					pc.SetResult(DcmPresContextResult.RejectAbstractSyntaxNotSupported);
				}
			}
		}

		#region Static
		public static DcmAssociateProfile GenericStorage;
		private static List<DcmAssociateProfile> Profiles;
		public static List<string> SupportedTransferSyntaxes;
		public static List<string> SupportedStorageSyntaxes;

		static DcmAssociateProfile() {
			SupportedTransferSyntaxes = new List<string>();
			SupportedTransferSyntaxes.Add(DcmTS.JPEG2000Lossless.UID.UID);
			SupportedTransferSyntaxes.Add(DcmTS.JPEGProcess14SV1.UID.UID);
			SupportedTransferSyntaxes.Add(DcmTS.RLELossless.UID.UID);
			SupportedTransferSyntaxes.Add(DcmTS.ExplicitVRLittleEndian.UID.UID);
			SupportedTransferSyntaxes.Add(DcmTS.ImplicitVRLittleEndian.UID.UID);
			SupportedTransferSyntaxes.Add(DcmTS.ExplicitVRBigEndian.UID.UID);

			SupportedStorageSyntaxes = new List<string>();
			SupportedStorageSyntaxes.Add(DcmUIDs.Verification.UID);
			foreach (DcmUID uid in DcmUIDs.Entries.Values) {
				if (uid.Description.Contains("Storage"))
					SupportedStorageSyntaxes.Add(uid.UID);
			}

			GenericStorage = new DcmAssociateProfile();
			GenericStorage.Name = "Generic Profile";
			GenericStorage.CalledAE = "*";
			GenericStorage.CallingAE = "*";
			GenericStorage.RemoteImplUID = "*";
			GenericStorage.RemoteVersion = "*";
			GenericStorage.SetDefault();

			Profiles = new List<DcmAssociateProfile>();
		}

		public static void AddProfile(string filename) {
			DcmAssociateProfile profile = Load(filename);
			AddProfile(profile);
		}

		public static void AddProfile(DcmAssociateProfile profile) {
			if (profile != null)
				Profiles.Add(profile);
		}

		public static DcmAssociateProfile Find(DcmAssociate associate, bool matchImplentation) {
			List<DcmAssociateProfile> candidates = new List<DcmAssociateProfile>();
			foreach (DcmAssociateProfile profile in Profiles) {
				if (!Wildcard.Match(profile.CalledAE, associate.CalledAE) || !Wildcard.Match(profile.CallingAE, associate.CallingAE))
					continue;
				if (matchImplentation &&
					(!Wildcard.Match(profile.RemoteImplUID, associate.ImplementationClass.UID) ||
					 !Wildcard.Match(profile.RemoteVersion, associate.ImplementationVersion)))
					continue;
				candidates.Add(profile);
			}
			if (candidates.Count > 0) {
				candidates.Sort(delegate(DcmAssociateProfile p1, DcmAssociateProfile p2) {
					return p1.GetWeight(matchImplentation) - p2.GetWeight(matchImplentation);
				});
				return candidates[0];
			}
			return GenericStorage;
		}
		#endregion
	}
}
