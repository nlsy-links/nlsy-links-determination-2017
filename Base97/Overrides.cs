using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nls.Base97 {
    internal static class Overrides {
        //internal static readonly Int32[] MissingYob = { 669201, 669202 }; //We don't have many values for these two brothers
        //public static Int32[] MissingYob() {  return (Int32[])(new Int32[] { 669201, 669202 }).Clone(); } //We don't have many values for these two brothers
        public struct SubjectYear {
            Int32 SubjectTag;
            Int16 SurveyYear;
            public SubjectYear( Int32 subjectTag, Int16 surveyYear ) {
                SubjectTag = subjectTag;
                SurveyYear = surveyYear;
            }
        }
        //internal static IList<SubjectYear> InverviewDateInvalidSkip { get { return new ReadOnlyCollection<SubjectYear>(new SubjectYear[] { new SubjectYear(3617, 1990) }); } }//For the 1990 interview, the date is missing (but shouldn't be).
        internal static IList<SubjectYear> InverviewDateInvalidSkip { get { return new ReadOnlyCollection<SubjectYear>(new SubjectYear[] { }); } }

        internal static Int32[] RosterAndExplicit = { 
            //159400, //He's consistent with himself, but got dragged into this list by his brother 
            //159500, //'ADOPTED OR STEP-BROTHER' on roster (ie, #59), but shares biomom with 159400 in 2006 (159400 says they share neither bioparent).
        };
    }
}
