using System.Diagnostics;

namespace Nls.Base79 {
    public partial class LinksDataSet {
        //public vewSurveyTimeMostRecentRow FindBySubjectTag( int SubjectTag ) {
        //    return ((vewSurveyTimeMostRecentRow)(this.Rows.Find(new object[] {
        //                    SubjectTag})));
        //}
        public static vewSurveyTimeMostRecentRow FindBySubjectTag( LinksDataSet.vewSurveyTimeMostRecentDataTable dt, int SubjectTag ) {
            //return ((vewSurveyTimeMostRecentRow)(dt.Rows.Find(new object[] { SubjectTag }))); //This works only if SubjecTag is the primary key.
            string select = string.Format("{0}={1}", SubjectTag, dt.SubjectTagColumn.ColumnName);
            LinksDataSet.vewSurveyTimeMostRecentRow[] drs = (LinksDataSet.vewSurveyTimeMostRecentRow[])dt.Select(select);

            if( drs.Length == 0 ) {
                return null;
            } else if( drs.Length == 1 ) {
                Trace.Assert(drs.Length == 1, "There should be exactly one row returned.");
                return drs[0];
            } else {
                Trace.Fail("At most, one row should be returned.");
                return null;
            }
        }

        //private LinksDataSet.tblVariableRow[] VariablesToTranslate( ExtractSource extractSource ) {
        //    string select = string.Format("{0}={1} AND {2}={3}",
        //        (byte)extractSource, _dsLinks.tblVariable.ExtractSourceColumn.ColumnName,
        //        "TRUE", _dsLinks.tblVariable.TranslateColumn.ColumnName);
        //    LinksDataSet.tblVariableRow[] drVariablesToTranslate = (LinksDataSet.tblVariableRow[])_dsLinks.tblVariable.Select(select);
        //    return drVariablesToTranslate;
        //}
    }
}