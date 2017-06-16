using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nls.BaseAssembly {
	public static class CommonCalculations {
		public static Int32 PermutationOf2 ( Int32 topValue ) {
			if ( topValue <= 0 ) throw new ArgumentOutOfRangeException("topValue", topValue, "The value must be larger than zero.");
			return topValue * (topValue - 1);
		}
		//public static Int32 CombinationOf2 ( Int16 topValue ) { return PermutationOf2(topValue) / 2;//Integer division		 //}
		public static Generation GenerationOfSubjectTag ( Int32 subjectTag ) {
			double remainder = Math.IEEERemainder(subjectTag, 100);
			if ( remainder == 0 )
				return Generation.Gen1;
			else
				return Generation.Gen2;
		}
		public static Int16 MotherIDOfGen2Subject ( Int32 gen2SubjectID ) {
			Trace.Assert(Constants.Gen2IDMin <= gen2SubjectID && gen2SubjectID <= Constants.Gen2IDMax, "The SubjectID should be valid for Generation 2.");
			Int32 tempMotherID = gen2SubjectID / 100; //This is integer division, which basically drops the last two digits.
			Trace.Assert(Constants.Gen1IDMin <= tempMotherID && tempMotherID <= Constants.Gen1IDMax, "The inferred ID should be valid for Generation 1.");
			Trace.Assert(tempMotherID <= Int16.MaxValue, "The ID should fit in a Int16 variable.");
			return Convert.ToInt16(tempMotherID);
		}
		public static Int32 MotherTagOfGen2Subject ( Int32 gen2SubjectID ) {
			Trace.Assert(Constants.Gen2IDMin <= gen2SubjectID && gen2SubjectID <= Constants.Gen2IDMax, "The SubjectID should be valid for Generation 2.");
			Int32 tempMotherID = gen2SubjectID / 100; //This is integer division, which basically drops the last two digits.
			Trace.Assert(Constants.Gen1IDMin <= tempMotherID && tempMotherID <= Constants.Gen1IDMax, "The inferred ID should be valid for Generation 1.");
			Trace.Assert(tempMotherID <= Int16.MaxValue, "The ID should fit in a Int16 variable.");
			return tempMotherID * 100;
		}
		public static bool Gen2SubjectsHaveCommonMother ( Int32 subject1ID, Int32 subject2ID ) {
			Trace.Assert(Constants.Gen2IDMin <= subject1ID && subject1ID <= Constants.Gen2IDMax, "The subject1ID should be valid for Generation 2.");
			Trace.Assert(Constants.Gen2IDMin <= subject2ID && subject2ID <= Constants.Gen2IDMax, "The subject2ID should be valid for Generation 2.");
			Int16 motherIDOfSubject1 = MotherIDOfGen2Subject(subject1ID);
			Int16 motherIDOfSubject2 = MotherIDOfGen2Subject(subject2ID);
			return Convert.ToBoolean(motherIDOfSubject1 == motherIDOfSubject2);
		}
		public static string ConvertItemsToString ( Item[] items) {
			if ( items == null ) throw new ArgumentNullException("items");
			string itemIDsString = "";
			for ( Int32 i = 0; i < items.Length; i++ ) {
				if ( i > 0 ) itemIDsString += ",";
				itemIDsString += Convert.ToInt16(items[i]);
			}
			Int32 distinctCount = (from item in items select item).Distinct().Count();
			if ( distinctCount != items.Length ) throw new ArgumentException("The items should be unique, and not contain duplicates.", "items");
			return itemIDsString;
		}
	}
}
