using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nls.BaseAssembly.EnumResponsesGen1;

namespace Nls.BaseAssembly {
	namespace Assign {
		public static class EvidenceGen1 {
			//public const float LowestRelatedR = (float)(1 / 64);

			public static MarkerEvidence RosterSameGeneration ( Tristate tristate ) {
				switch ( tristate ) {
					case Tristate.No: return MarkerEvidence.Disconfirms;
					case Tristate.Yes: return MarkerEvidence.StronglySupports;
					case Tristate.DoNotKnow: return MarkerEvidence.Ambiguous;
					default: throw new ArgumentOutOfRangeException("tristate", tristate, "This value is not permitted.");
				}
			}
			public static MarkerEvidence RosterShareBioParentOrGrandparent ( Tristate tristate ) {
				switch ( tristate ) {
					case Tristate.No: return MarkerEvidence.Disconfirms;
					case Tristate.Yes: return MarkerEvidence.Consistent;
					case Tristate.DoNotKnow: return MarkerEvidence.Ambiguous;
					default: throw new ArgumentOutOfRangeException("tristate", tristate, "This value is not permitted.");
				}
			}
			public static MarkerEvidence ShareBioparentsForBioparents ( EnumResponsesGen1.ShareBioparentGen1 share ) {
				switch ( share ) {
					case ShareBioparentGen1.Yes:
						return MarkerEvidence.Supports;
					case ShareBioparentGen1.No:
						return MarkerEvidence.Disconfirms;
					case ShareBioparentGen1.DoNotKnow:
					case ShareBioparentGen1.NotSure:
					case ShareBioparentGen1.Refusal:
						return MarkerEvidence.Ambiguous;
					case ShareBioparentGen1.NonInterview:
					case ShareBioparentGen1.ValidSkip:
						return MarkerEvidence.Irrelevant;
					default:
						throw new ArgumentOutOfRangeException("share", share, "The enum value for share is not recognized.");
				}
			}
		}
	}
}

//public static MarkerEvidence RosterSameGener ( Tristate tristate ) {
//   switch ( tristate ) {
//      case Tristate.No: return MarkerEvidence.Disconfirms;
//      case Tristate.Yes: return MarkerEvidence.Consistent;
//      case Tristate.DoNotKnow: return MarkerEvidence.Ambiguous;
//      default: throw new ArgumentOutOfRangeException("tristate", tristate, "This value is not permitted.");
//   }
//}

//public static MarkerGen1Summary ShareBioparentRoster1979 ( Gen1Roster roster ) {//For R0000151, R0000153, R0000155, R0000157, R0000159
//   MarkerEvidence sameGeneration;
//   MarkerEvidence shareBiodad;
//   MarkerEvidence shareBiomom;
//   MarkerEvidence shareBiograndparent;

//   switch ( roster ) {
//      case Gen1Roster.ValidSkip:
//      case Gen1Roster.InvalidSkip:
//      case Gen1Roster.Refusal:
//         sameGeneration = MarkerEvidence.Missing;
//         shareBiodad = MarkerEvidence.Ambiguous;
//         shareBiomom = MarkerEvidence.Ambiguous;
//         shareBiograndparent = MarkerEvidence.Ambiguous;
//         break;
//      //case Gen1Relationship.Respondent:
//      case Gen1Roster.Spouse:
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      //case Gen1Relationship.Son: 
//      //case Gen1Relationship.Daughter:
//      //case Gen1Relationship.Father:
//      //case Gen1Relationship.Mother:
//      case Gen1Roster.Brother:
//      case Gen1Roster.Sister:
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Consistent;
//         shareBiomom = MarkerEvidence.Consistent;
//         shareBiograndparent = MarkerEvidence.Consistent;
//         break;
//      //case Gen1Relationship.Grandfather:
//      //case Gen1Relationship.Grandmother:
//      //case Gen1Relationship.Grandson:
//      //case Gen1Relationship.Granddaughter:
//      case Gen1Roster.UncleEtc:
//      case Gen1Roster.AuntEtc:
//         sameGeneration = MarkerEvidence.Disconfirms;
//         shareBiodad = MarkerEvidence.Unlikely;
//         shareBiomom = MarkerEvidence.Unlikely;
//         shareBiograndparent = MarkerEvidence.Unlikely;
//         break;
//      //case Gen1Relationship.GreatUncle:
//      //case Gen1Relationship.GreatAunt:
//      case Gen1Roster.Cousin://Weaken b/c might not be a real 1/8 cousin?
//         sameGeneration = MarkerEvidence.Supports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Supports;
//         break;
//      case Gen1Roster.Nephew:
//      case Gen1Roster.Niece:
//         sameGeneration = MarkerEvidence.Disconfirms;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Supports;
//         break;
//      //case Gen1Relationship.OtherBloodRelative:
//      case Gen1Roster.AdoptedOrStepson:
//      case Gen1Roster.AdoptedOrStepdaughter:
//         sameGeneration = MarkerEvidence.Disconfirms;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      //case Gen1Relationship.SonInLaw:
//      //case Gen1Relationship.DaughterInLaw:
//      //case Gen1Relationship.FatherInLaw:
//      //case Gen1Relationship.MotherInLaw:
//      case Gen1Roster.BrotherInLaw:
//      case Gen1Roster.SisterInLaw:
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      case Gen1Roster.GrandfatherInLaw:
//      case Gen1Roster.GrandsonInLaw:
//         sameGeneration = MarkerEvidence.Disconfirms;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      //case Gen1Relationship.GrandmotherInLaw:
//      //case Gen1Relationship.GranddaugherInLaw:
//      case Gen1Roster.OtherInLawRelative:
//         sameGeneration = MarkerEvidence.Ambiguous;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      case Gen1Roster.Partner:
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      case Gen1Roster.Boarder:
//         sameGeneration = MarkerEvidence.Ambiguous;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      //case Gen1Relationship.FosterChild:
//      case Gen1Roster.OtherNonRelative:
//         sameGeneration = MarkerEvidence.Ambiguous;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      //case Gen1Relationship.Stepfather:
//      case Gen1Roster.Stepmother:
//         sameGeneration = MarkerEvidence.Disconfirms;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      case Gen1Roster.Stepbrother:
//      case Gen1Roster.Stepsister:
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      //case Gen1Relationship.GreatGrandson
//      //case Gen1Relationship.GreatGranddaughter:
//      //case Gen1Relationship.StepGrandson:
//      //case Gen1Relationship.StepGranddaughter:
//      //case Gen1Relationship.FosterSon:
//      //case Gen1Relationship.FosterDaughter
//      //case Gen1Relationship.Parents:
//      //case Gen1Relationship.Grandparents:
//      //case Gen1Relationship.AuntOrUncle:
//      //case Gen1Relationship.FosterFather:
//      //case Gen1Relationship.FosterMother:
//      case Gen1Roster.FosterBrother:
//      case Gen1Roster.FosterSister:
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      //case Gen1Relationship.Guardian:
//      case Gen1Roster.HusbandOrBrotherInLaw:
//      case Gen1Roster.WifeOrSisterInLaw:
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      case Gen1Roster.AdoptedOrStepbrother:
//      case Gen1Roster.AdoptedOrStepsister: //Watch out, 'AdoptedOrStepsister' is listed at the bottom of the NlsInvestigator (ie, it's out of order in the cookbook)
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Disconfirms;
//         shareBiomom = MarkerEvidence.Disconfirms;
//         shareBiograndparent = MarkerEvidence.Disconfirms;
//         break;
//      case Gen1Roster.BrotherOrCousin:
//      case Gen1Roster.SisterOrCousin: //r = 0.3125f; break;// = (1/2 + 1/8) / 2
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Ambiguous;
//         shareBiomom = MarkerEvidence.Ambiguous;
//         shareBiograndparent = MarkerEvidence.StronglySupports;
//         break;
//      case Gen1Roster.BrotherNaturalStepOrAdopted:
//      case Gen1Roster.SisterNaturalStepOrAdopted: //r = 0.125f; break; //I weighted this towards 0, because I think the real .5 siblings would be caught with the previous categories.
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Unlikely;
//         shareBiomom = MarkerEvidence.Unlikely;
//         shareBiograndparent = MarkerEvidence.Unlikely;
//         break;
//      case Gen1Roster.SiblingOrSpouseOfInLaws: //r = 0.125f; break; //I weighted this towards 0, because I think the real .5 siblings would be caught with the previous categories.
//         sameGeneration = MarkerEvidence.StronglySupports;
//         shareBiodad = MarkerEvidence.Unlikely;
//         shareBiomom = MarkerEvidence.Unlikely;
//         shareBiograndparent = MarkerEvidence.Unlikely;
//         break;
//      default: throw new ArgumentOutOfRangeException("roster", roster, "The relationship category was not recognized.");
//   }
//   return new MarkerGen1Summary(sameGeneration, shareBiomom, shareBiodad, shareBiograndparent);
//   //rosterResolved, rosterR, rosterRBoundLower, rosterRBoundUpper, 
//}
