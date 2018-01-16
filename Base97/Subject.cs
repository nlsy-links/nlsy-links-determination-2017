using System;
using System.Diagnostics;

namespace Nls.Base97 {
    public static class Subject {
        #region Public Methods
        public static string CreateSubject( ImportDataSet dsImport, LinksDataSet dsLinks ) {
            if( dsImport == null ) throw new ArgumentNullException("dsImport");
            if( dsImport.tblRoster.Rows.Count != Constants.Gen1Count ) throw new ArgumentException("There should be exactly " + Constants.Gen1Count + " Gen1 subject rows, but instead there are " + dsImport.tblRoster.Rows.Count + ".");
            if( dsLinks == null ) throw new ArgumentNullException("dsLinks");
            if( dsLinks.tblSubject.Count != 0 ) throw new InvalidOperationException("tblSubject must be empty before creating rows for it.");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Int32 subjectRecordsAddedCount = 0;
            foreach( ImportDataSet.tblDemographicsRow drGen1 in dsImport.tblDemographics ) {
                LinksDataSet.tblSubjectRow drBare = dsLinks.tblSubject.NewtblSubjectRow();
                drBare.SubjectID = (Int32)drGen1[Constants.SubjectIDColumn];
                drBare.SubjectTag = drBare.SubjectID;
                drBare.ExtendedID = Convert.ToInt16(drGen1[Constants.ExtendedFamilyIDColumn]);
                drBare.Gender = Convert.ToByte(drGen1[Constants.GenderColumn]);
                dsLinks.tblSubject.AddtblSubjectRow(drBare);
                subjectRecordsAddedCount += 1;
            }
            Trace.Assert(subjectRecordsAddedCount == Constants.Gen1Count, "The number of added Gen1 subjects should be correct.");


            sw.Stop();
            Int32 expectedRowCount = dsImport.tblRoster.Rows.Count;
            Trace.Assert(expectedRowCount == subjectRecordsAddedCount, "The correct number of subjects should be added.");
            return string.Format("{0:N0} Subject records were enumerated.\nElapsed time: {1}", subjectRecordsAddedCount, sw.Elapsed.ToString());
        }
        #endregion
        #region Private Methods
        //private static Int16 RetrieveExtendedFamilyIDFromGenMotherID ( ImportDataSet dsImport, Int32 motherID ) {
        //    ImportDataSet.tblGen1LinksRow drGen1 = dsImport.tblGen1Links.FindByR0000100(motherID);
        //    Trace.Assert(drGen1 != null, "The retrieved mother's row should not be null for Gen1 subject '" + motherID + "'.");
        //    return (Int16)drGen1.R0000149;
        //}

        #endregion
    }
}
