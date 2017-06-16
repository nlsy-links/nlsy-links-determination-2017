using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Nls.BaseAssembly {
	public static class CommonFunctions {

		private readonly static MarkerEvidence[] _green = { MarkerEvidence.StronglySupports, MarkerEvidence.Supports };
		private readonly static MarkerEvidence[] _yellow = { MarkerEvidence.Consistent, MarkerEvidence.Ambiguous, MarkerEvidence.Irrelevant, MarkerEvidence.Missing };
		private readonly static MarkerEvidence[] _red = { MarkerEvidence.Unlikely, MarkerEvidence.Disconfirms };

		public static Tristate TranslateEvidenceToTristate ( MarkerEvidence source1, MarkerEvidence source2 ) {
			if ( _green.Contains(source1) && _green.Contains(source2) ) return Tristate.Yes;
			else if ( _red.Contains(source1) && _red.Contains(source2) ) return Tristate.No;
			else if ( _yellow.Contains(source1) ) return TranslateEvidenceToTristate(source2);
			else if ( _yellow.Contains(source2) ) return TranslateEvidenceToTristate(source1);
			else if ( (_green.Contains(source1) && _red.Contains(source2)) || (_green.Contains(source2) && _red.Contains(source1)) ) return Tristate.DoNotKnow;
			else throw new ArgumentOutOfRangeException("The 'TranslateEvidenceToTristate' function was not designed to accommodate these two values of MarkerEvidence.");
		}
		public static Tristate TranslateEvidenceToTristate ( MarkerEvidence evidence ) {
			switch ( evidence ) {
				case MarkerEvidence.StronglySupports:
				case MarkerEvidence.Supports:
					return Tristate.Yes;
				case MarkerEvidence.Consistent:
				case MarkerEvidence.Ambiguous:
				case MarkerEvidence.Irrelevant:
				case MarkerEvidence.Missing:
					return Tristate.DoNotKnow;
				case MarkerEvidence.Unlikely:
				case MarkerEvidence.Disconfirms:
					return Tristate.No;
				default:
					throw new ArgumentOutOfRangeException("evidence", evidence, "This function does not support this value of MarkerEvidence.");
			}
		}
		public static Tristate TakePriority ( Tristate priority1, Tristate priority2 ) {
			if ( priority1 != Tristate.DoNotKnow )
				return priority1;
			else
				return priority2;//Which still may be 'DoNotKnow'
		}
		public static float? TranslateToR ( Tristate shareBiomom, Tristate shareBiodad, bool mustDecide ) {
			if ( shareBiomom == Tristate.DoNotKnow && shareBiodad == Tristate.DoNotKnow ) return null;
			else if ( shareBiomom == Tristate.No && shareBiodad == Tristate.No ) return RCoefficients.NotRelated;
			else if ( shareBiomom == Tristate.Yes && shareBiodad == Tristate.Yes ) return RCoefficients.SiblingFull;
			else if ( shareBiomom == Tristate.No && shareBiodad == Tristate.Yes ) return RCoefficients.SiblingHalf;
			else if ( shareBiomom == Tristate.Yes && shareBiodad == Tristate.No ) return RCoefficients.SiblingHalf;
			else if ( !mustDecide ) return null;
			else if ( shareBiomom == Tristate.DoNotKnow && shareBiodad == Tristate.No ) return RCoefficients.SiblingHalfOrLess; //What does the team think about this?
			else if ( shareBiomom == Tristate.No && shareBiodad == Tristate.DoNotKnow ) return RCoefficients.SiblingHalfOrLess; //What does the team think about this?
			else if ( shareBiomom == Tristate.DoNotKnow && shareBiodad == Tristate.Yes ) return RCoefficients.SiblingAmbiguous;
			else if ( shareBiomom == Tristate.Yes && shareBiodad == Tristate.DoNotKnow ) return RCoefficients.SiblingAmbiguous;
			else throw new InvalidOperationException("All conditions should have been caught.");
		}
		public static YesNo ReverseYesNo ( YesNo yn ) {
			switch ( yn ) {
				case YesNo.Yes: return YesNo.No;
				case YesNo.No: return YesNo.Yes;
				default: return yn;  //Don't modify the other values.
			}
		}
		public static Tristate TranslateYesNo ( YesNo yn ) {
			switch ( yn ) {
				case YesNo.Yes: return  Tristate.Yes;
				case YesNo.No: return Tristate.No;
				default: return Tristate.DoNotKnow;
			}
		}
		//public static bool? TranslateYesNo ( YesNo yn ) {
		//   switch ( yn ) {
		//      case YesNo.Yes: return true;
		//      case YesNo.No: return false;
		//      default: return null;
		//   }
		//}
		public static byte LastTwoDigitsOfGen2SubjectID ( LinksDataSet.tblSubjectRow drSubject ) {
			if ( drSubject == null ) throw new ArgumentNullException("drSubject");
			if ( drSubject.Generation != (byte)Generation.Gen2 ) throw new ArgumentOutOfRangeException("drSubject", drSubject.Generation, "This function is valid for only Gen2 subjects.");
			string subjectIDString = drSubject.SubjectID.ToString();
			Int32 startIndex = subjectIDString.Length - 2;
			return Convert.ToByte(subjectIDString.Substring(startIndex));
		}
		public static Int16[] CreateExtendedFamilyIDs ( LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSubject.Count <= 0 ) throw new ArgumentException("The tblSubject is empty.", "dsLinks");
			IEnumerable<Int16> ids = (from dr in dsLinks.tblSubject
											  select dr.ExtendedID).Distinct();
			return ids.ToArray();
		}
		internal static bool BothGen1 ( LinksDataSet.tblSubjectRow drSubject1, LinksDataSet.tblSubjectRow drSubject2 ) {
			if ( drSubject1 == null ) throw new ArgumentNullException("drSubject1");
			if ( drSubject2 == null ) throw new ArgumentNullException("drSubject2");
			return drSubject1.Generation == (byte)Generation.Gen1 && drSubject2.Generation == (byte)Generation.Gen1;
		}
		//internal static bool BothGen1 ( LinksDataSet.tblRelatedRow drRelated ) {
		//   if ( drRelated == null ) throw new ArgumentNullException("drRelated");
		//   LinksDataSet.tblSubjectRow drSubject1 = drRelated.tblSubjectRowByFK_tblRelated_tblSubject_Subject1;
		//   LinksDataSet.tblSubjectRow drSubject2 = drRelated.tblSubjectRowByFK_tblRelated_tblSubject_Subject2;
		//   return BothGen1(drSubject1, drSubject2);
		//}
		//internal static bool BothGen2 ( LinksDataSet.tblSubjectRow drSubject1, LinksDataSet.tblSubjectRow drSubject2 ) {
		//   if ( drSubject1 == null ) throw new ArgumentNullException("drSubject1");
		//   if ( drSubject2 == null ) throw new ArgumentNullException("drSubject2");
		//   return drSubject1.Generation == (byte)Generation.Gen2 && drSubject2.Generation == (byte)Generation.Gen2;
		//}		
	}
}
