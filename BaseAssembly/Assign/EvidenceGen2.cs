using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nls.BaseAssembly.EnumResponsesGen2;

namespace Nls.BaseAssembly {
	namespace Assign {
		public static class EvidenceGen2 {
			#region For Bioparents
			public static MarkerEvidence ShareBiodadForBioparents ( EnumResponsesGen2.ShareBiodadGen2 share ) {
				switch ( share ) {
					case ShareBiodadGen2.Yes:
						return MarkerEvidence.Supports;
					case ShareBiodadGen2.No:
						return MarkerEvidence.Disconfirms;
					case ShareBiodadGen2.DoNotKnow:
					case ShareBiodadGen2.NotSure:
					case ShareBiodadGen2.Refusal:
						return MarkerEvidence.Ambiguous;
					case ShareBiodadGen2.Missing:
						return MarkerEvidence.Missing;
					default:
						throw new ArgumentOutOfRangeException("share", share, "The enum value for share is not recognized.");
				}
			}
			public static MarkerEvidence BabyDaddyAsthma ( YesNo asthma1, YesNo asthma2 ) {
				switch ( asthma1 ) {
					/////////////////
					case YesNo.Yes:
						switch ( asthma2 ) {
							case YesNo.Yes:
								return MarkerEvidence.Consistent;
							case YesNo.No:
								return MarkerEvidence.Unlikely;
							case YesNo.DoNotKnow:
							case YesNo.Refusal:
							case YesNo.InvalidSkip:
								return MarkerEvidence.Ambiguous;
							case YesNo.ValidSkipOrNoInterviewOrNotInSurvey:
								return MarkerEvidence.Missing;
							default:
								throw new ArgumentOutOfRangeException("asthma2", asthma2, "The enum value for asthma2 is not recognized.");
						}
					/////////////////
					case YesNo.No:
						switch ( asthma2 ) {
							case YesNo.Yes:
								return MarkerEvidence.Unlikely;
							case YesNo.No:
								return MarkerEvidence.Consistent;
							case YesNo.DoNotKnow:
							case YesNo.Refusal:
							case YesNo.InvalidSkip:
								return MarkerEvidence.Ambiguous;
							case YesNo.ValidSkipOrNoInterviewOrNotInSurvey:
								return MarkerEvidence.Missing;
							default:
								throw new ArgumentOutOfRangeException("asthma2", asthma2, "The enum value for asthma2 is not recognized.");
						}
					/////////////////
					case YesNo.DoNotKnow:
					case YesNo.Refusal:
					case YesNo.InvalidSkip:
						return MarkerEvidence.Ambiguous;
					case YesNo.ValidSkipOrNoInterviewOrNotInSurvey:
						return MarkerEvidence.Missing;
					default:
						throw new ArgumentOutOfRangeException("asthma1", asthma1, "The enum value for asthma1 is not recognized.");
				}
			}
			public static MarkerEvidence FatherAsthmaForBioparents ( EnumResponsesGen2.FatherAsthma asthma1, EnumResponsesGen2.FatherAsthma asthma2 ) {
				switch ( asthma1 ) {
					/////////////////
					case EnumResponsesGen2.FatherAsthma.Yes:
						switch ( asthma2 ) {
							case EnumResponsesGen2.FatherAsthma.Yes:
								return MarkerEvidence.Consistent;
							case EnumResponsesGen2.FatherAsthma.No:
								return MarkerEvidence.Unlikely;
							case EnumResponsesGen2.FatherAsthma.DoNotKnow:
							case EnumResponsesGen2.FatherAsthma.Refusal:
								return MarkerEvidence.Ambiguous;
							case EnumResponsesGen2.FatherAsthma.Missing:
								return MarkerEvidence.Missing;
							default:
								throw new ArgumentOutOfRangeException("asthma2", asthma2, "The enum value for asthma2 is not recognized.");
						}
					/////////////////
					case EnumResponsesGen2.FatherAsthma.No:
						switch ( asthma2 ) {
							case EnumResponsesGen2.FatherAsthma.Yes:
								return MarkerEvidence.Unlikely;
							case EnumResponsesGen2.FatherAsthma.No:
								return MarkerEvidence.Consistent;
							case EnumResponsesGen2.FatherAsthma.DoNotKnow:
							case EnumResponsesGen2.FatherAsthma.Refusal:
								return MarkerEvidence.Ambiguous;
							case EnumResponsesGen2.FatherAsthma.Missing:
								return MarkerEvidence.Missing;
							default:
								throw new ArgumentOutOfRangeException("asthma2", asthma2, "The enum value for asthma2 is not recognized.");
						}
					/////////////////
					case EnumResponsesGen2.FatherAsthma.DoNotKnow:
					case EnumResponsesGen2.FatherAsthma.Refusal:
						return MarkerEvidence.Ambiguous;
					case EnumResponsesGen2.FatherAsthma.Missing:
						return MarkerEvidence.Missing;
					default:
						throw new ArgumentOutOfRangeException("asthma1", asthma1, "The enum value for asthma1 is not recognized.");
				}
			}
			#endregion
			//#region For MZ
			//public static MarkerEvidence GenderForMz ( Gender subject1Gender,Gender subject2Gender ) {
			//   if ( subject1Gender == Gender.InvalidSkipGen2 || subject2Gender == Gender.InvalidSkipGen2 )
			//      return MarkerEvidence.Ambiguous;
			//   else if ( subject1Gender == subject2Gender )
			//      return MarkerEvidence.Consistent;
			//   else
			//      return MarkerEvidence.Disconfirms;
			//}
			//public static MarkerEvidence DobSeparationForMz ( double separationInDays ) {
			//   //Without the restricted-access geocode data, we only have the month of birth, so the resolution is very coarse.
			//   double absoluteSeparation = Math.Abs(separationInDays);
			//   if ( absoluteSeparation < 35 )
			//      return MarkerEvidence.Consistent;
			//   else if ( absoluteSeparation < 95 )
			//      return MarkerEvidence.Unlikely;
			//   else
			//      return MarkerEvidence.Disconfirms;
			//}
			//#endregion
		}
	}
}
