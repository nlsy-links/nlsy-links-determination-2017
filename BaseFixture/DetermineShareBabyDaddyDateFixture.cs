using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nls.BaseAssembly;
using Nls.BaseAssembly.Trend;
namespace Nls.Tests.BaseFixture {
	[TestClass()]
	public class DetermineShareBabyDaddyDateFixture {
		#region Fields
		private readonly Int16[] _surveyYears1 = { 1998, 2000, 2002, 2004, 2006 };
		private readonly Int16[] _surveyYears2 = { 1998, 2000, 2002, 2004, 2006 };
		private TrendLineDate _trend0;
		private TrendLineDate _trend1;
		private TrendLineDate _trend2;
		private TrendLineDate _trend3;
		private TrendLineDate _trend4;
		private TrendLineDate _trend5;
		private ITrendComparison _comparison0vs0;
		private ITrendComparison _comparison0vs1;
		private ITrendComparison _comparison0vs3;
		private ITrendComparison _comparison1vs2;
		private ITrendComparison _comparison1vs3;
		private ITrendComparison _comparison1vs4;
		private ITrendComparison _comparison1vs5;
		private ITrendComparison _comparison2vs3;
		private ITrendComparison _comparison3vs3;
		private ITrendComparison _comparison3vs4;
		private readonly DateTime?[] _dates0 = { null, null, null, null, null };
		private readonly DateTime?[] _dates1 = { new DateTime(2000, 1, 15), new DateTime(2000, 1, 15), new DateTime(2002, 2, 15), new DateTime(2002, 2, 15), new DateTime(2002, 2, 15) };
		private readonly DateTime?[] _dates2 = { new DateTime(2000, 1, 15), new DateTime(2000, 1, 15), new DateTime(2002, 2, 15), new DateTime(2002, 2, 15), new DateTime(2002, 2, 15) };
		private readonly DateTime?[] _dates3 = { null, new DateTime(2000, 1, 15), new DateTime(2002, 2, 15), null, new DateTime(2002, 2, 15) };
		private readonly DateTime?[] _dates4 = { null, new DateTime(1999, 1, 15), new DateTime(1999, 2, 15), new DateTime(1999, 3, 15), new DateTime(1999, 4, 15) };
		private readonly DateTime?[] _dates5 = { new DateTime(2000, 1, 15), new DateTime(2000, 1, 15), new DateTime(2002, 2, 15), new DateTime(2002, 2, 15), new DateTime(2003, 3, 15) };
		#endregion
		#region Additional test attributes
		[TestInitialize()]
		public void TestInitialize ( ) {
			_trend0 = new TrendLineDate(_surveyYears2, _dates0);
			_trend1 = new TrendLineDate(_surveyYears1, _dates1);
			_trend2 = new TrendLineDate(_surveyYears2, _dates2);
			_trend3 = new TrendLineDate(_surveyYears2, _dates3);
			_trend4 = new TrendLineDate(_surveyYears2, _dates4);
			_trend5 = new TrendLineDate(_surveyYears2, _dates5);
			_comparison0vs0 = new TrendComparisonDate(_trend0, _trend0);
			_comparison0vs1 = new TrendComparisonDate(_trend0, _trend1);
			_comparison0vs3 = new TrendComparisonDate(_trend0, _trend3);
			_comparison1vs2 = new TrendComparisonDate(_trend1, _trend2);
			_comparison1vs3 = new TrendComparisonDate(_trend1, _trend3);
			_comparison1vs4 = new TrendComparisonDate(_trend1, _trend4);
			_comparison1vs5 = new TrendComparisonDate(_trend1, _trend5);
			_comparison2vs3 = new TrendComparisonDate(_trend2, _trend3);
			_comparison3vs3 = new TrendComparisonDate(_trend3, _trend3);
			_comparison3vs4 = new TrendComparisonDate(_trend3, _trend4);
		}
		[TestCleanup()]
		public void TestCleanup ( ) {
			_trend0 = null;
			_trend1 = null;
			_trend2 = null;
			_trend3 = null;
			_trend4 = null;
			_trend5 = null;
			_comparison0vs0 = null;
			_comparison0vs1 = null;
			_comparison0vs3 = null;
			_comparison1vs2 = null;
			_comparison1vs3 = null;
			_comparison1vs4 = null;
			_comparison1vs5 = null;
			_comparison2vs3 = null;
			_comparison3vs3 = null;
			_comparison3vs4 = null;
		}
		#endregion
		private void Date ( MarkerEvidence expected, ITrendComparison comparison ) {
			MarkerEvidence actual = DetermineShareBabyDaddy.Date(comparison);
			Assert.AreEqual(expected, actual, "The determination should be correct.");
		}
		[TestMethod()]
		public void DateTest0vs0 ( ) {
			Date(MarkerEvidence.Missing, _comparison0vs0);
		}
		[TestMethod()]
		public void DateTest0vs1 ( ) {
			Date(MarkerEvidence.Missing, _comparison0vs1);
		}
		[TestMethod()]
		public void DateTest0vs3 ( ) {
			Date(MarkerEvidence.Missing, _comparison0vs3);
		}
		[TestMethod()]
		public void DateTest1vs2 ( ) {
			Date(MarkerEvidence.StronglySupports, _comparison1vs2);
		}
		[TestMethod()]
		public void DateTest1vs3 ( ) {
			Date(MarkerEvidence.StronglySupports, _comparison1vs3);
		}
		[TestMethod()]
		public void DateTest1vs4 ( ) {
			Date(MarkerEvidence.Disconfirms, _comparison1vs4);
		}
		[TestMethod()]
		public void DateTest1vs5 ( ) {
			Date(MarkerEvidence.Supports, _comparison1vs5);
		}
		[TestMethod()]
		public void DateTest2vs3 ( ) {
			Date(MarkerEvidence.StronglySupports, _comparison2vs3);
		}
		[TestMethod()]
		public void DateTest3vs3 ( ) {
			Date(MarkerEvidence.StronglySupports, _comparison3vs3);
		}
		[TestMethod()]
		public void DateTest3vs4 ( ) {
			Date(MarkerEvidence.Disconfirms, _comparison3vs4);
		}

		#region SpecialCases
		[TestMethod]
		public void Sibs608401Vs608402 ( ) {
			Int16[] surveyYears = { 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008 };
			DateTime?[] datesA = { new DateTime(1980, 7, 15), new DateTime(1986, 7, 15), null, new DateTime(1994, 5, 15), null, null, null, new DateTime(1989, 7, 15), null, new DateTime(1999, 7, 15) };
			DateTime?[] datesB = { null, null, null, new DateTime(1994, 5, 15), null, null, null, new DateTime(1989, 7, 15), new DateTime(2005, 12, 15), new DateTime(1999, 7, 15) };

			TrendLineDate trendA = new TrendLineDate(surveyYears, datesA);
			TrendLineDate trendB = new TrendLineDate(surveyYears, datesB);
			TrendComparisonDate comparison = new TrendComparisonDate(trendA, trendB);
			Date(MarkerEvidence.StronglySupports, comparison);
		}
		[TestMethod]
		public void Sibs608401Vs608405 ( ) {
			Int16[] surveyYears = { 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008 };
			DateTime?[] datesA = { new DateTime(1980, 7, 15), new DateTime(1986, 7, 15), null, new DateTime(1994, 5, 15), null, null, null, new DateTime(1989, 7, 15), null, new DateTime(1999, 7, 15) };
			DateTime?[] datesB = { null, null, null, null, null, null, null, new DateTime(1989, 7, 15), new DateTime(2005, 12, 15), new DateTime(1999, 7, 15) };

			TrendLineDate trendA = new TrendLineDate(surveyYears, datesA);
			TrendLineDate trendB = new TrendLineDate(surveyYears, datesB);
			TrendComparisonDate comparison = new TrendComparisonDate(trendA, trendB);
			Date(MarkerEvidence.Supports, comparison);
		}

		#endregion
	}
}
