using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nls.BaseAssembly;
using Nls.BaseAssembly.Trend;
namespace Nls.Tests.BaseFixture {
	[TestClass()]
	public class DetermineShareBabyDaddyLiveInHHFixture {
		#region Fields
		private readonly Int16[] _surveyYears1 = { 1998, 2000, 2002, 2004, 2006 };
		private readonly Int16[] _surveyYears2 = { 1998, 2000, 2002, 2004, 2006 };
		private TrendLineInteger _trend0;
		private TrendLineInteger _trend1;
		private TrendLineInteger _trend2;
		private TrendLineInteger _trend3;
		private TrendLineInteger _trend4;
		private TrendLineInteger _trend5;
		private TrendComparisonInteger _comparison0vs0;
		private TrendComparisonInteger _comparison0vs1;
		private TrendComparisonInteger _comparison0vs3;
		private TrendComparisonInteger _comparison1vs2;
		private TrendComparisonInteger _comparison1vs3;
		private TrendComparisonInteger _comparison1vs4;
		private TrendComparisonInteger _comparison1vs5;
		private TrendComparisonInteger _comparison2vs3;
		private TrendComparisonInteger _comparison3vs3;
		private TrendComparisonInteger _comparison3vs4;
		private readonly Int16?[] _values0 = { null, null, null, null, null };
		private readonly Int16?[] _values1 = { 1, 1, 0, 0, 0 };
		private readonly Int16?[] _values2 = { 1, 1, 0,0, 0 };
		private readonly Int16?[] _values3 = { null, 1, 0, null, 0 };
		private readonly Int16?[] _values4 = { null, 0, 0, 0, 0 };
		private readonly Int16?[] _values5 = { 1, 1, 0, 0, 1 };
		#endregion
		#region Additional test attributes
		[TestInitialize()]
		public void TestInitialize ( ) {
			_trend0 = new TrendLineInteger(_surveyYears2, _values0);
			_trend1 = new TrendLineInteger(_surveyYears1, _values1);
			_trend2 = new TrendLineInteger(_surveyYears2, _values2);
			_trend3 = new TrendLineInteger(_surveyYears2, _values3);
			_trend4 = new TrendLineInteger(_surveyYears2, _values4);
			_trend5 = new TrendLineInteger(_surveyYears2, _values5);
			_comparison0vs0 = new TrendComparisonInteger(_trend0, _trend0);
			_comparison0vs1 = new TrendComparisonInteger(_trend0, _trend1);
			_comparison0vs3 = new TrendComparisonInteger(_trend0, _trend3);
			_comparison1vs2 = new TrendComparisonInteger(_trend1, _trend2);
			_comparison1vs3 = new TrendComparisonInteger(_trend1, _trend3);
			_comparison1vs4 = new TrendComparisonInteger(_trend1, _trend4);
			_comparison1vs5 = new TrendComparisonInteger(_trend1, _trend5);
			_comparison2vs3 = new TrendComparisonInteger(_trend2, _trend3);
			_comparison3vs3 = new TrendComparisonInteger(_trend3, _trend3);
			_comparison3vs4 = new TrendComparisonInteger(_trend3, _trend4);
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
		private void BiodadInHH ( MarkerEvidence expected, TrendComparisonInteger comparison ) {
			MarkerEvidence actual = DetermineShareBabyDaddy.InHH(comparison);
			Assert.AreEqual(expected, actual, "The determination should be correct.");
		}
		[TestMethod()]
		public void DateTest0vs0 ( ) {
			BiodadInHH(MarkerEvidence.Missing, _comparison0vs0);
		}
		[TestMethod()]
		public void DateTest0vs1 ( ) {
			BiodadInHH(MarkerEvidence.Missing, _comparison0vs1);
		}
		[TestMethod()]
		public void DateTest0vs3 ( ) {
			BiodadInHH(MarkerEvidence.Missing, _comparison0vs3);
		}
		[TestMethod()]
		public void DateTest1vs2 ( ) {
			BiodadInHH(MarkerEvidence.StronglySupports, _comparison1vs2);
		}
		[TestMethod()]
		public void DateTest1vs3 ( ) {
			BiodadInHH(MarkerEvidence.Supports, _comparison1vs3);
		}
		[TestMethod()]
		public void DateTest1vs4 ( ) {
			//BiodadInHH(MarkerEvidence.Disconfirms, _comparison1vs4);
			BiodadInHH(MarkerEvidence.Unlikely, _comparison3vs4);
		}
		[TestMethod()]
		public void DateTest1vs5 ( ) {
			BiodadInHH(MarkerEvidence.Supports, _comparison1vs5);
		}
		[TestMethod()]
		public void DateTest2vs3 ( ) {
			BiodadInHH(MarkerEvidence.Supports, _comparison2vs3);
		}
		[TestMethod()]
		public void DateTest3vs3 ( ) {
			BiodadInHH(MarkerEvidence.Supports, _comparison3vs3);
		}
		[TestMethod()]
		public void DateTest3vs4 ( ) {
			//BiodadInHH(MarkerEvidence.Disconfirms, _comparison3vs4);
			BiodadInHH(MarkerEvidence.Unlikely, _comparison3vs4);

		}
	}
}
