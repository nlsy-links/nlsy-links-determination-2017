using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nls.BaseAssembly.Trend {
	public static class DetermineShareGen2Father {
		//public static MarkerEvidence Date ( ITrendComparison comparison ) {
		//   if ( comparison == null )
		//      throw new ArgumentNullException("comparison");
		//   else if ( comparison.Count <= 0 )
		//      throw new InvalidOperationException("There should be at least one survey year considered.");
		//   else if ( comparison.CountOfNullZeroes <= 0 ) //All the scores are null for both subjects.
		//      return MarkerEvidence.Missing;
		//   else if ( comparison.CountOfNullZeroes >= 2 && comparison.PointsAgreePerfectly )
		//      return MarkerEvidence.StronglySupports;
		//   else if ( comparison.CountOfNullZeroes >= 3 && Math.Abs(comparison.AgreementProportionExcludingNulls - 1.0) < 1e-5 )
		//   //else if ( comparison.CountOfNullZeroes >= 2 && Math.Abs(comparison.AgreementProportionExcludingNulls - 1.0) < 1e-5 )
		//      return MarkerEvidence.StronglySupports;
		//   else if ( comparison.LastMutualNonNullPointsAgree.HasValue && comparison.LastMutualNonNullPointsAgree.Value )
		//      return MarkerEvidence.Supports;
		//   else if ( comparison.CountOfNullZeroes >= 2 && comparison.AgreementProportionExcludingNulls <= .5 )
		//      return MarkerEvidence.Disconfirms;
		//   else if ( comparison.CountOfNullZeroes >= 2 && comparison.AgreementProportionExcludingNulls > .5 )
		//      return MarkerEvidence.Supports;
		//   else if ( comparison.LastMutualNonNullPointsAgree.HasValue && !comparison.LastMutualNonNullPointsAgree.Value )
		//      return MarkerEvidence.Unlikely;
		//   //else if ( DisagreementCountIncludingNulls > 0 ) 
		//   //   return MarkerEvidence.Unlikely;
		//   else
		//      return MarkerEvidence.Ambiguous;
		//}
		public static MarkerEvidence InHH ( TrendComparisonInteger comparison ) {
			if ( comparison == null )
				throw new ArgumentNullException("comparison");
			else if ( comparison.Count <= 0 )
				throw new InvalidOperationException("There should be at least one survey year considered.");
			else if ( comparison.CountOfNullZeroes <= 0 ) //All the scores are null for both subjects.
				return MarkerEvidence.Missing;
			//else if ( comparison.CountOfNullZeroes >= 2 && Math.Abs(comparison.AgreementProportionOfOnes - 0.0) < 1e-7 ) //If agree on no twice, disconfirm full sibs (ie, go half siblings);
			else if ( comparison.CountOfAtLeastOneResponseIsOne >= 2 && Math.Abs(comparison.AgreementProportionOfOnes - 0.0) < 1e-7 ) //If agree on no twice, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Disconfirms;
			else if ( comparison.AgreementCountOfOnes >= 2 && Math.Abs(comparison.AgreementProportionOfOnes - 1.0) < 1e-7 ) //If always agree on yes twice, support full siblings
				return MarkerEvidence.StronglySupports;
			else if ( comparison.CountOfNullZeroes >= 3 && comparison.AgreementProportionOfOnes > .6 ) //If 2 of 3 responses (or more) say agree on yes, support full siblings
				return MarkerEvidence.Supports;
			else if ( comparison.CountOfNullZeroes >= 2 && Math.Abs(comparison.AgreementProportionOfOnes - 0.0) < 1e-7 ) //If agree on no twice, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Unlikely;//Moved down and switched from disconfirms to unlikely
			else if ( comparison.CountOfNullZeroes == 1 && Math.Abs(comparison.AgreementProportionOfOnes - 0.0) < 1e-7 ) //If agree on no once, say unlikely full sibs (ie, go half siblings);
				return MarkerEvidence.Unlikely;
			else if ( comparison.AgreementCountOfOnes < 2 && Math.Abs(comparison.AgreementProportionOfOnes - 1.0) < 1e-7 ) //If always agree on yes, support full siblings
				return MarkerEvidence.Supports;//Moved down from BabyDaddy
			//else if ( comparison.CountOfNullZeroes >= 3 && comparison.AgreementProportionOfOnes < .3 ) //If 1 of 3 responses (or less) say disagree on yes, disconfirm full sibs (ie, go half siblings);
			else if ( comparison.CountOfAtLeastOneResponseIsOne >= 3 && comparison.AgreementProportionOfOnes < .34 ) //If 1 of 3 responses (or less) say disagree on yes, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Disconfirms;
			else
				return MarkerEvidence.Ambiguous;
		}
		public static MarkerEvidence AliveOrAsthma ( TrendComparisonInteger comparison ) {
			if ( comparison == null )
				throw new ArgumentNullException("comparison");
			else if ( comparison.Count <= 0 )
				throw new InvalidOperationException("There should be at least one survey year considered.");
			else if ( comparison.CountOfNullZeroes <= 0 ) //All the scores are null for both subjects.
				return MarkerEvidence.Missing;
			else if ( comparison.CountOfNullZeroes >= 2 && Math.Abs(comparison.AgreementProportionExcludingNulls - 0.0) < 1e-7 ) //If different values twice, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Disconfirms;
			else if ( comparison.CountOfNullZeroes >= 4 && comparison.AgreementProportionExcludingNulls <= .5 ) //If the majority of 4+ responses disagree, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Disconfirms;
			else if ( comparison.CountOfNullZeroes >= 1 && !comparison.LastMutualNonNullPointsAgree.Value ) //If different values once, unlikely full sibs (ie, go half siblings);
				return MarkerEvidence.Unlikely;
			else if ( comparison.CountOfNullZeroes >= 1 && Math.Abs(comparison.AgreementProportionExcludingNulls - 1.0) < 1e-7 ) //If always agree on yes, evidence is consistent with full siblings.
				return MarkerEvidence.Consistent;
			else if ( comparison.CountOfNullZeroes >= 4 && comparison.AgreementProportionExcludingNulls >= .75 ) //If 75% of 4+ responses agree, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Consistent;
			else
				return MarkerEvidence.Ambiguous;
		}
		public static MarkerEvidence DistanceFromHH ( TrendComparisonInteger comparison ) {
			if ( comparison == null )
				throw new ArgumentNullException("comparison");
			else if ( comparison.Count <= 0 )
				throw new InvalidOperationException("There should be at least one survey year considered.");
			else if ( comparison.CountOfNullZeroes <= 0 ) //All the scores are null for both subjects.
				return MarkerEvidence.Missing;
			else if ( comparison.CountOfNullZeroes >= 2 && Math.Abs(comparison.AgreementProportionExcludingNulls - 0.0) < 1e-7 ) //If different distances twice, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Disconfirms;
			else if ( comparison.CountOfNullZeroes >= 4 && comparison.AgreementProportionExcludingNulls <= .5 ) //If the majority of 4+ responses disagree, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Disconfirms;
			else if ( comparison.CountOfNullZeroes >= 1 && Math.Abs(comparison.AgreementProportionExcludingNulls - 0.0) < 1e-7 ) //If different distances once, unlikely full sibs (ie, go half siblings);
				return MarkerEvidence.Unlikely;
			else if ( comparison.CountOfNullZeroes >= 1 && Math.Abs(comparison.AgreementProportionExcludingNulls - 1.0) < 1e-7 ) //If always agree on yes, evidence is consistent with full siblings.
				return MarkerEvidence.Consistent;
			else if ( comparison.CountOfNullZeroes >= 4 && comparison.AgreementProportionExcludingNulls >= .75 ) //If 75% of 4+ responses agree, disconfirm full sibs (ie, go half siblings);
				return MarkerEvidence.Consistent;
			//TODO: add a clause that weakly supports with the following case. Multiple distances over the years, and the values agree.
			//	It's not great, because two biodads could live close to each other, and when the mom moves around, it could look like the two dads are the same person.
			else
				return MarkerEvidence.Ambiguous;
		}
	}

}