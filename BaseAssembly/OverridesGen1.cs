using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nls.BaseAssembly {
	internal static class OverridesGen1 {
		//internal static readonly Int32[] MissingYob = { 669201, 669202 }; //We don't have many values for these two brothers
		//public static Int32[] MissingYob() {  return (Int32[])(new Int32[] { 669201, 669202 }).Clone(); } //We don't have many values for these two brothers
		public struct SubjectYear {
			Int32 SubjectTag;
			Int16 SurveyYear;
			public SubjectYear ( Int32 subjectTag, Int16 surveyYear ) {
				SubjectTag = subjectTag;
				SurveyYear = surveyYear;
			}
		}
		internal static IList<SubjectYear> InverviewDateInvalidSkip { get { return new ReadOnlyCollection<SubjectYear>(new SubjectYear[] { new SubjectYear(3617, 1990) }); } }//For the 1990 interview, the date is missing (but shouldn't be).

		internal static Int32[] RosterAndExplicit = { 
			159400, //He's consistent with himself, but got dragged into this list by his brother 
			159500, //'ADOPTED OR STEP-BROTHER' on roster (ie, #59), but shares biomom with 159400 in 2006 (159400 says they share neither bioparent).
			164400, //He's consistent with himself, but got dragged into this list by his brother
			164500, //Both claim 'STEP-BROTHER' on roster (ie, #39), but shares biomom with 164400 in 2006 (164400 didn't answer explicit items).
			382100, //SPOUSE' on roster, but both share biomom in 2006 with 382200
			382200, //SPOUSE' on roster, but both share biomom in 2006 with 382100 
			485900, //'STEP-BROTHER' on roster (ie, #39), but shares (both?) bio parents in 2006
			612100, //'SPOUSE' on roster (ie, #1), but in 2006 says 611900 shares (both?) bio parents
			627000, //'ADOPTED OR STEP-BROTHER' on roster (ie, #59), but in 2006 says 627200 shares (both?) bio parents
			627100, //'ADOPTED OR STEP-BROTHER' on roster (ie, #59), but in 2006 says 627200 shares (both?) bio parents
			627200, //'ADOPTED OR STEP-SISTER' on roster (ie, #60), but in 2006 says 627000 shares (both?) bio parents
			683500, //'STEP-BROTHER' on roster (ie, #39), but in 2006 says 683600 shares biomom (but not biodad).
			683600, //'STEP-SISTER' on roster (ie, #40), but in 2006 says 683500 shares biomom (but not biodad).
			732500, //'STEP-BROTHER' on roster (ie, #39), but in 2006 says 732600 shares biomom (but not biodad).
			732600, //'STEP-SISTER' on roster (ie, #40), but in 2006 says 732500 shares biomom (but not biodad).
			891000, //'COUSIN' on roster (ie, #16), but in 2006 says 890800 shares biomom (but not biodad).
			899900, //'COUSIN' on roster (ie, #16), but in 2006 says 899800 shares biomom (but not biodad).
			950300, //'STEP-SISTER' on roster (ie, #40), but in 2006 says 950400 shares biomom (but not biodad).
			950400, //'STEP-BROTHER' on roster (ie, #39), but in 2006 says 950300 shares biomom (but not biodad).
			967300, //'STEP-BROTHER' on roster (ie, #39), but in 2006 says 967400 shares biomom (but not biodad).
			967400  //'STEP-BROTHER' on roster (ie, #39), but in 2006 says 967300 shares biomom (but not biodad).			
		};
	}
}
