using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nls.BaseAssembly {
	internal static class OverridesGen2 {
		internal static IList<Int32> MissingMobInvalidSkip { get { return new ReadOnlyCollection<Int32>(new Int32[] { 669201, 669202 }); } }//We don't have many values for these two brothers; they mother doesn't have their DOB values either (ie, R99000.01 & R99000.02).
		internal static IList<Int32> MissingMobRefusedMonth { get { return new ReadOnlyCollection<Int32>(new Int32[] { 851104 }); } }//(S)he refused to report her month
		internal static IList<Int32> MissingBirthOrderInvalidSkip { get { return new ReadOnlyCollection<Int32>(new Int32[] { 669201, 669202 }); } }//We don't have many values for these two brothers; they mother doesn't have their DOB values either (ie, R99000.01 & R99000.02).
		//internal static IList<Int32> ImpossibleBirthOrder { get { return new ReadOnlyCollection<Int32>(new Int32[] { 318801, 567002, 902101, 1031302 }); } }//For example, The C00058.00 reports that they're the third child, but that mom has only 1 child in the NLS records.
	}
}
