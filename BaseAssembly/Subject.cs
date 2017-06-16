using System;
using System.Diagnostics;

namespace Nls.BaseAssembly {
	public static class Subject {
		#region Public Methods
		public static string CreateSubject ( ImportDataSet dsImport, LinksDataSet dsLinks ) {
			if ( dsImport == null ) throw new ArgumentNullException("dsImport");
			if ( dsImport.tblGen1Links.Rows.Count != Constants.Gen1Count ) throw new ArgumentException("There should be exactly " + Constants.Gen1Count + " Gen1 subject rows, but instead there are " + dsImport.tblGen1Links.Rows.Count + ".");
			if ( dsImport.tblGen2Links.Rows.Count != Constants.Gen2Count ) throw new ArgumentException("There should be exactly " + Constants.Gen2Count + " Gen2 subject rows, but instead there are " + dsImport.tblGen2Links.Rows.Count + ".");
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSubject.Count != 0 ) throw new InvalidOperationException("tblSubject must be empty before creating rows for it.");
			Stopwatch sw = new Stopwatch();
			sw.Start();

			Int32 subjectRecordsAddedCount = 0;
			foreach ( ImportDataSet.tblGen1LinksRow drGen1 in dsImport.tblGen1Links ) {
				LinksDataSet.tblSubjectRow drBare = dsLinks.tblSubject.NewtblSubjectRow();
				drBare.SubjectID = (Int32)drGen1[Constants.Gen1SubjectIDColumn];
				drBare.SubjectTag = drBare.SubjectID * 100;//For Gen1 subjects, append "00" at the end.
				drBare.ExtendedID = Convert.ToInt16(drGen1[Constants.Gen1ExtendedFamilyIDColumn]);
				drBare.Generation = (byte)Generation.Gen1;
				drBare.Gender = Convert.ToByte(drGen1[Constants.Gen1GenderColumn]);
				dsLinks.tblSubject.AddtblSubjectRow(drBare);
				subjectRecordsAddedCount += 1;
			}
			Trace.Assert(subjectRecordsAddedCount == Constants.Gen1Count, "The number of added Gen1 subjects should be correct.");

			foreach ( ImportDataSet.tblGen2LinksRow drGen2 in dsImport.tblGen2Links ) {
				LinksDataSet.tblSubjectRow drBare = dsLinks.tblSubject.NewtblSubjectRow();
				drBare.SubjectID = drGen2.C0000100;
				drBare.SubjectTag = drBare.SubjectID;
				Int32 motherID = drGen2.C0000200;
				drBare.ExtendedID = RetrieveExtendedFamilyIDFromGenMotherID(dsImport, motherID);
				drBare.Generation = (byte)Generation.Gen2;
				
				Int32 genderTemp = drGen2.C0005400;
				if ( genderTemp == -3 ) genderTemp = (byte)Gender.InvalidSkipGen2;
				drBare.Gender = (byte)genderTemp;
				
				dsLinks.tblSubject.AddtblSubjectRow(drBare);
				subjectRecordsAddedCount += 1;
			}
			Trace.Assert(subjectRecordsAddedCount == Constants.Gen1Count+ Constants.Gen2Count, "The number of added Gen2 subjects should be correct.");

			sw.Stop();
			Int32 expectedRowCount = dsImport.tblGen1Links.Rows.Count + dsImport.tblGen2Links.Rows.Count;
			Trace.Assert(expectedRowCount == subjectRecordsAddedCount, "The correct number of subjects should be added.");
			return string.Format("{0:N0} Subject records were enumerated.\nElapsed time: {1}", subjectRecordsAddedCount, sw.Elapsed.ToString());
		}
		#endregion
		#region Private Methods
		private static Int16 RetrieveExtendedFamilyIDFromGenMotherID ( ImportDataSet dsImport, Int32 motherID ) {
			ImportDataSet.tblGen1LinksRow drGen1 = dsImport.tblGen1Links.FindByR0000100(motherID);
			Trace.Assert(drGen1 != null, "The retrieved mother's row should not be null for Gen1 subject '" + motherID + "'.");
			return (Int16)drGen1.R0000149;
		}

		#endregion
	}
}
