using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nls.Base97 {
	public static class CommonCalculations {
		public static Int32 PermutationOf2 ( Int32 topValue ) {
			if ( topValue <= 0 ) throw new ArgumentOutOfRangeException("topValue", topValue, "The value must be larger than zero.");
			return topValue * (topValue - 1);
		}


        //public static bool Gen2SubjectsHaveCommonMother ( Int32 subject1ID, Int32 subject2ID ) {
        //    Trace.Assert(Constants.Gen2IDMin <= subject1ID && subject1ID <= Constants.Gen2IDMax, "The subject1ID should be valid for Generation 2.");
        //    Trace.Assert(Constants.Gen2IDMin <= subject2ID && subject2ID <= Constants.Gen2IDMax, "The subject2ID should be valid for Generation 2.");
        //    Int16 motherIDOfSubject1 = MotherIDOfGen2Subject(subject1ID);
        //    Int16 motherIDOfSubject2 = MotherIDOfGen2Subject(subject2ID);
        //    return Convert.ToBoolean(motherIDOfSubject1 == motherIDOfSubject2);
        //}
        public static string ConvertItemsToString( Item[] items ) {
            if( items == null ) throw new ArgumentNullException("items");
            string itemIDsString = "";
            for( Int32 i = 0; i < items.Length; i++ ) {
                if( i > 0 ) itemIDsString += ",";
                itemIDsString += Convert.ToInt16(items[i]);
            }
            Int32 distinctCount = (from item in items select item).Distinct().Count();
            if( distinctCount != items.Length ) throw new ArgumentException("The items should be unique, and not contain duplicates.", "items");
            return itemIDsString;
        }
	}
}
