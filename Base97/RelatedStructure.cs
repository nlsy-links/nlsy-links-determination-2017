using System;
using System.Diagnostics;

namespace Nls.Base97 {
	public sealed class RelatedStructure {
		#region Fields
		private readonly LinksDataSet _dsLinks;
		#endregion
		#region Constructor
		public RelatedStructure ( LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSubject.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblSubject.");
			if ( dsLinks.tblRelatedStructure.Count != 0 ) throw new ArgumentException("There should be zero rows in tblRelatedStructure.");
			_dsLinks = dsLinks;
		}
		#endregion
		#region Public Methods
		public string Go ( ) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Int32 recordsAdded = 0;
			foreach ( Int16 extendedID in CommonFunctions.CreateExtendedFamilyIDs(_dsLinks) ) {
				recordsAdded += UnpackExtendedFamily(extendedID);
            }
            Trace.Assert(recordsAdded == Constants.HousematesPathCount, "The count of added housemates should be correct.");

			sw.Stop();
			string message = string.Format("{0:N0} Related paths were processed.\n\nElapsed time: {1}", recordsAdded, sw.Elapsed.ToString());
			return message;
		}
		public static LinksDataSet.tblRelatedStructureRow Retrieve ( LinksDataSet ds,  Int32 subject1Tag, Int32 subject2Tag ) {
			if ( ds.tblRelatedStructure.Count <= 0 ) throw new ArgumentException("tblRelatedStructure should have more than one row.", "ds");
			string sql = string.Format("{0}={1} AND {2}={3}",
				subject1Tag, ds.tblRelatedStructure.SubjectTag_S1Column.ColumnName,
				subject2Tag, ds.tblRelatedStructure.SubjectTag_S2Column.ColumnName);
			LinksDataSet.tblRelatedStructureRow[] drs = (LinksDataSet.tblRelatedStructureRow[])ds.tblRelatedStructure.Select(sql);
			Trace.Assert(drs.Length == 1, "There should be exactly one row retrieved.");
			return drs[0];
		}
		#endregion
		#region Private Methods
        private Int32 UnpackExtendedFamily( Int16 extendedID ) {
            string sql = string.Format("{0}={1}",
                extendedID, _dsLinks.tblSubject.ExtendedIDColumn.ColumnName);
            LinksDataSet.tblSubjectRow[] drAllExtendedFamilyMembers = (LinksDataSet.tblSubjectRow[])this._dsLinks.tblSubject.Select(sql);
            Int32 totalUpackedRowsInFamily = 0;
            foreach( LinksDataSet.tblSubjectRow drSubject1 in drAllExtendedFamilyMembers ) {
                foreach( LinksDataSet.tblSubjectRow drSubject2 in drAllExtendedFamilyMembers ) {
                    if( drSubject1.SubjectTag != drSubject2.SubjectTag )
                        totalUpackedRowsInFamily += ProcessPair(drSubject1, extendedID, drSubject2);
                }
            }

            Trace.Assert(totalUpackedRowsInFamily == CommonCalculations.PermutationOf2(drAllExtendedFamilyMembers.Length), "The number of unpacked rows for this extended family should be correct.");
            return totalUpackedRowsInFamily;
        }
		private Int32 ProcessPair ( LinksDataSet.tblSubjectRow drSubject1, Int16 extendedID, LinksDataSet.tblSubjectRow drSubject2 ) {
			Trace.Assert(drSubject1.ExtendedID == drSubject2.ExtendedID, "The ExtendedFamilyID should match.");

			LinksDataSet.tblRelatedStructureRow drNew = this._dsLinks.tblRelatedStructure.NewtblRelatedStructureRow();
			drNew.ExtendedID = extendedID;
			drNew.SubjectTag_S1 = drSubject1.SubjectTag;
			drNew.SubjectTag_S2 = drSubject2.SubjectTag;
			drNew.RelationshipPath = (byte)1;
			drNew.EverSharedHouse = true;
			_dsLinks.tblRelatedStructure.AddtblRelatedStructureRow(drNew);
			return 1;
		}


		#endregion
	}
}
