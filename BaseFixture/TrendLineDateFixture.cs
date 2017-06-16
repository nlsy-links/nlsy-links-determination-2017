using Nls.BaseAssembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Nls.BaseAssembly.Trend;

namespace Nls.Tests.BaseFixture {
	[TestClass()]
	public class TrendLineDateFixture {
		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		[TestMethod()]
		public void SurveyYearsTest ( ) {
			Int16[] surveyYears = { 2000, 2002, 2004 };
			DateTime?[] points = new DateTime?[] { new DateTime(2000, 08, 20), new DateTime(2000, 08, 20), new DateTime(2000, 08, 20) };
			TrendLineDate trend = new TrendLineDate(surveyYears, points);
			Int16[] actualYears = trend.SurveyYears;
			DateTime?[] actualPoints = trend.Dates;
			Assert.AreEqual(surveyYears, actualYears);
			Assert.AreEqual(points, actualPoints);
		}
		[TestMethod()]
		public void ConsistentYes ( ) {
			Int16[] surveyYears = { 2000, 2002, 2004 };
			DateTime?[] points = new DateTime?[] { new DateTime(2000, 08, 20), new DateTime(2000, 08, 20), new DateTime(2000, 08, 20) };
			TrendLineDate trend = new TrendLineDate(surveyYears, points);
			Int16[] actualYears = trend.SurveyYears;
			DateTime?[] actualPoints = trend.Dates;
			Assert.AreEqual(surveyYears, actualYears);
			Assert.AreEqual(points, actualPoints);
		}

		#region Jump Tests
		[TestMethod()]
		public void FindJumpsTest1 ( ) {
			Int16[] surveyYears = { 2000, 2002, 2004 };
			DateTime?[] points = new DateTime?[] { new DateTime(2000, 08, 20), new DateTime(2000, 08, 20), new DateTime(2001, 01, 01) };
			Int16[] expected = { 2004 };
			TrendLineDate trend = new TrendLineDate(surveyYears, points);
			Int16[] actual = trend.Jumps;
			Helpers.CompareArray(expected, actual);
		}
		[TestMethod()]
		public void FindJumpsTest2 ( ) {
			Int16[] surveyYears = { 2000, 2002, 2004, 2006 };
			DateTime?[] points = new DateTime?[] { new DateTime(2000, 08, 20), new DateTime(2000, 08, 20), null, new DateTime(2001, 01, 01) };
			Int16[] expected = { 2006 };
			TrendLineDate trend = new TrendLineDate(surveyYears, points);
			Int16[] actual = trend.Jumps;
			Helpers.CompareArray(expected, actual);
		}
		[TestMethod()]
		public void FindJumpsTest3 ( ) {
			Int16[] surveyYears = { 2000, 2002, 2004, 2006 };
			DateTime?[] points = new DateTime?[] { null, new DateTime(2000, 08, 20), null, new DateTime(2001, 01, 01) };
			Int16[] expected = { 2006 };
			TrendLineDate trend = new TrendLineDate(surveyYears, points);
			Int16[] actual = trend.Jumps;
			Helpers.CompareArray(expected, actual);
		}
		[TestMethod()]
		public void FindJumpsTest4 ( ) {
			Int16[] surveyYears = { 2000, 2002, 2004, 2006 };
			DateTime?[] points = new DateTime?[] { null, null, null, null };
			Int16[] expected = { };
			TrendLineDate trend = new TrendLineDate(surveyYears, points);
			Int16[] actual = trend.Jumps;
			Helpers.CompareArray(expected, actual);
		}
		[TestMethod()]
		public void FindJumpsTest5 ( ) {
			Int16[] surveyYears = { 2000, 2002, 2004, 2006 };
			DateTime?[] points = new DateTime?[] { new DateTime(2000, 08, 20), new DateTime(2000, 08, 20), new DateTime(2000, 08, 20), new DateTime(2000, 08, 20) };
			Int16[] expected = { };
			TrendLineDate trend = new TrendLineDate(surveyYears, points);
			Int16[] actual = trend.Jumps;
			Helpers.CompareArray(expected, actual);
		}
		#endregion
	}
}
