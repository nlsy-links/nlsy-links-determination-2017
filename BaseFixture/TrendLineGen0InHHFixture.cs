using Nls.BaseAssembly.Trend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Nls.Tests.BaseFixture {
	[TestClass()]
	public class TrendLineGen0InHHFixture {
		#region fields
		private const Int32 _count = 20;
		private readonly Int16[] _years = { 0, 1961, 1962, 1963, 1964, 1965, 1966, 1967, 1968, 1969, 1970, 1971, 1972, 1973, 1974, 1975, 1976, 1977, 1978, 1979 };
		private readonly byte[] _ages = { 255, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
		private readonly bool?[] _values1 = { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
		private readonly bool?[] _values2 = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
		private readonly bool?[] _values3 = { false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
		private readonly bool?[] _values4 = { true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
		private readonly bool?[] _values5 = { false, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
		#endregion

		//[TestMethod()]
		//public void ConstructorTest ( ) {
		//   TrendLineGen0InHH target = new TrendLineGen0InHH(_years, _values1, _ages);
		//}
		[TestMethod()]
		public void ValidateTestHelper ( ) {
			TrendLineGen0InHH.Validate<bool?>(_years, _values1, _ages);
			TrendLineGen0InHH.Validate<bool?>(_years, _values2, _ages);
			TrendLineGen0InHH.Validate<bool?>(_years, _values3, _ages);
			TrendLineGen0InHH.Validate<bool?>(_years, _values4, _ages);
			TrendLineGen0InHH.Validate<bool?>(_years, _values5, _ages);
		}
		//[TestMethod()]
		//public void CountAllTest ( ) {
		//   TrendLineGen0InHH target = new TrendLineGen0InHH(_years, _values1, _ages);
		//   Int32 actual= target.CountAll;
		//   Assert.AreEqual(_count, actual, "The count should be accurate.");
		//   Assert.AreEqual(_count, new TrendLineGen0InHH(_years, _values1, _ages).CountAll, "The count should be accurate for _values1.");
		//   Assert.AreEqual(_count, new TrendLineGen0InHH(_years, _values2, _ages).CountAll, "The count should be accurate for _values2.");
		//   Assert.AreEqual(_count, new TrendLineGen0InHH(_years, _values3, _ages).CountAll, "The count should be accurate for _values3.");
		//   Assert.AreEqual(_count, new TrendLineGen0InHH(_years, _values4, _ages).CountAll, "The count should be accurate for _values4.");
		//   Assert.AreEqual(_count, new TrendLineGen0InHH(_years, _values5, _ages).CountAll, "The count should be accurate for _values5.");
		//}
		//[TestMethod()]
		//public void JumpCountTest ( ) {
		//   Assert.AreEqual(0, new TrendLineGen0InHH(_years, _values1, _ages).JumpCount, "The jump count should be accurate for _values1.");
		//   Assert.AreEqual(0, new TrendLineGen0InHH(_years, _values2, _ages).JumpCount, "The jump count should be accurate for _values2.");
		//   Assert.AreEqual(1, new TrendLineGen0InHH(_years, _values3, _ages).JumpCount, "The jump count should be accurate for _values3.");
		//   Assert.AreEqual(1, new TrendLineGen0InHH(_years, _values4, _ages).JumpCount, "The jump count should be accurate for _values4.");
		//   Assert.AreEqual(2, new TrendLineGen0InHH(_years, _values5, _ages).JumpCount, "The jump count should be accurate for _values5.");
		//}

		//[TestMethod()]
		//public void JumpsTest ( ) {
		//   short[] years = null; // TODO: Initialize to an appropriate value
		//   Nullable<bool>[] values = null; // TODO: Initialize to an appropriate value
		//   byte[] ages = null; // TODO: Initialize to an appropriate value
		//   TrendLineGen0InHH target = new TrendLineGen0InHH(years, values, ages); // TODO: Initialize to an appropriate value
		//   short[] actual;
		//   actual = target.Jumps;
		//   Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void SurveyYearsTest ( ) {
		//   short[] years = null; // TODO: Initialize to an appropriate value
		//   Nullable<bool>[] values = null; // TODO: Initialize to an appropriate value
		//   byte[] ages = null; // TODO: Initialize to an appropriate value
		//   TrendLineGen0InHH target = new TrendLineGen0InHH(years, values, ages); // TODO: Initialize to an appropriate value
		//   short[] actual;
		//   actual = target.SurveyYears;
		//   Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void ValuesTest ( ) {
		//   short[] years = null; // TODO: Initialize to an appropriate value
		//   Nullable<bool>[] values = null; // TODO: Initialize to an appropriate value
		//   byte[] ages = null; // TODO: Initialize to an appropriate value
		//   TrendLineGen0InHH target = new TrendLineGen0InHH(years, values, ages); // TODO: Initialize to an appropriate value
		//   Nullable<bool>[] actual;
		//   actual = target.Values;
		//   Assert.Inconclusive("Verify the correctness of this test method.");
		//}
	}
}
