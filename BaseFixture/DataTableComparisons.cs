using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nls.BaseAssembly;

namespace Nls.Tests.BaseFixture {
	[TestClass]
	public class DataTableComparisons {
		[TestMethod, Ignore]
		public void SurveyTime ( ) {
			const string expected = @"F:\Projects\Nls\Links2011\StableComparisonData\tblSurveyTimeSerial2011-07-31-22-39.xml";
			//const string actual = @"F:\Projects\Nls\Links2011\StableComparisonData\tblSurveyTimeParallel2011-07-31-15-44.xml";
			const string actual = @"F:\Projects\Nls\Links2011\StableComparisonData\tblSurveyTimeSerial2011-08-01-20-48.xml";
			LinksDataSet dsExpected = new LinksDataSet();
			dsExpected.ReadXml(expected);
			dsExpected.Relations.Clear();
			LinksDataSet dsActual = new LinksDataSet();
			dsActual.Relations.Clear();
			dsActual.ReadXml(actual);

			Assert.IsTrue(dsExpected.tblSurveyTime.Count > 0, "The DataSet should at least one row."); ;
			Assert.AreEqual(Constants.SurveyTimeCount, dsExpected.tblSurveyTime.Count, "The DataSets should have the expected number of rows.");
			Assert.AreEqual(dsExpected.tblSurveyTime.Count, dsActual.tblSurveyTime.Count, "The DataSets should have the same number of rows.");

			const string sort = "SubjectTag, SurveyYear";
			LinksDataSet.tblSurveyTimeRow[] drsExpected = (LinksDataSet.tblSurveyTimeRow[])dsExpected.tblSurveyTime.Select("", sort);
			LinksDataSet.tblSurveyTimeRow[] drsActual = (LinksDataSet.tblSurveyTimeRow[])dsActual.tblSurveyTime.Select("", sort);

			for ( Int32 i = 0; i < drsExpected.Length; i++ ) {
				Assert.AreEqual<Int32>(drsExpected[i].SubjectTag, drsActual[i].SubjectTag, "The SubjectTag should be correct for row {0}.", i);
				Assert.AreEqual<byte>(drsExpected[i].SurveySource, drsActual[i].SurveySource, "The SurveySource should be correct for row {0}.", i);

				if ( drsExpected[i].IsSurveyDateNull() )
					Assert.IsTrue(drsActual[i].IsSurveyDateNull(), "The Survey Date should be null for row {0}.", i);
				else
					Assert.AreEqual<DateTime>(drsExpected[i].SurveyDate, drsActual[i].SurveyDate, "The SurveyDate should be correct for row {0}.", i);

				if ( drsExpected[i].IsAgeSelfReportYearsNull() )
					Assert.IsTrue(drsActual[i].IsAgeSelfReportYearsNull(), "The AgeSelfReportDate should be null for row {0}.", i);
				else
					Assert.AreEqual<double>(drsExpected[i].AgeSelfReportYears, drsActual[i].AgeSelfReportYears, "The AgeSelfReportYears should be correct for row {0}.", i);

				if ( drsExpected[i].IsAgeCalculateYearsNull() )
					Assert.IsTrue(drsActual[i].IsAgeCalculateYearsNull(), "The AgeCalculateYears should be null for row {0}.", i);
				else
					Assert.AreEqual(drsExpected[i].AgeCalculateYears, drsActual[i].AgeCalculateYears, "The AgeCalculateYears should be correct for row {0}.", i);
			}
		}
	}
}
