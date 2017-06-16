using System;
using System.Diagnostics;

namespace Nls.BaseAssembly {
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

			sw.Stop();
			string message = string.Format("{0:N0} Related paths were processed.\n\nElapsed time: {1}", recordsAdded, sw.Elapsed.ToString());
			return message;
		}
		public static LinksDataSet.tblRelatedStructureRow Retrieve ( LinksDataSet ds, RelationshipPath relationshipPath, Int32 subject1Tag, Int32 subject2Tag ) {
			if ( ds.tblRelatedStructure.Count <= 0 ) throw new ArgumentException("tblRelatedStructure should have more than one row.", "ds");
			string sql = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				(byte)relationshipPath, ds.tblRelatedStructure.RelationshipPathColumn.ColumnName,
				subject1Tag, ds.tblRelatedStructure.SubjectTag_S1Column.ColumnName,
				subject2Tag, ds.tblRelatedStructure.SubjectTag_S2Column.ColumnName);
			LinksDataSet.tblRelatedStructureRow[] drs = (LinksDataSet.tblRelatedStructureRow[])ds.tblRelatedStructure.Select(sql);
			Trace.Assert(drs.Length == 1, "There should be exactly one row retrieved.");
			return drs[0];
		}
		#endregion
		#region Private Methods
		private Int32 UnpackExtendedFamily ( Int16 extendedID ) {
			string sql = string.Format("{0}={1}",// AND {2}={3}",
				extendedID, _dsLinks.tblSubject.ExtendedIDColumn.ColumnName);//,				(byte)Generation.Gen1, _dsLinks.tblSubject.GenerationColumn.ColumnName
			LinksDataSet.tblSubjectRow[] drAllExtendedFamilyMembers = (LinksDataSet.tblSubjectRow[])this._dsLinks.tblSubject.Select(sql);
			Int32 totalUpackedRowsInFamily = 0;
			foreach ( LinksDataSet.tblSubjectRow drSubject1 in drAllExtendedFamilyMembers ) {
				foreach ( LinksDataSet.tblSubjectRow drSubject2 in drAllExtendedFamilyMembers ) {
					if ( CommonFunctions.BothGen1(drSubject1, drSubject2) && (drSubject1.SubjectTag != drSubject2.SubjectTag) )
						totalUpackedRowsInFamily += ProcessPair(drSubject1, extendedID, drSubject2);						
				}
			}
			foreach ( LinksDataSet.tblSubjectRow drSubject1 in drAllExtendedFamilyMembers ) {
				foreach ( LinksDataSet.tblSubjectRow drSubject2 in drAllExtendedFamilyMembers ) {
					if ( !CommonFunctions.BothGen1(drSubject1, drSubject2) && (drSubject1.SubjectTag != drSubject2.SubjectTag) )
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
			drNew.RelationshipPath = (byte)GetRelationshipPath(drSubject1, drSubject2);
			drNew.EverSharedHouse = (bool)SharedHouse((RelationshipPath)(drNew.RelationshipPath));
			_dsLinks.tblRelatedStructure.AddtblRelatedStructureRow(drNew);
			return 1;
		}
		private static RelationshipPath GetRelationshipPath ( LinksDataSet.tblSubjectRow drSubject1, LinksDataSet.tblSubjectRow drSubject2 ) {
			Trace.Assert(drSubject1.ExtendedID == drSubject2.ExtendedID, "The two subject should be in the same extended family.");
			if ( drSubject1.Generation == (byte)Generation.Gen1 && drSubject2.Generation == (byte)Generation.Gen1 ) {
				return RelationshipPath.Gen1Housemates;
			}
			else if ( drSubject1.Generation == (byte)Generation.Gen2 && drSubject2.Generation == (byte)Generation.Gen2 ) {
				if ( CommonCalculations.Gen2SubjectsHaveCommonMother(drSubject1.SubjectID, drSubject2.SubjectID) ) return RelationshipPath.Gen2Siblings;
				else return RelationshipPath.Gen2Cousins;
			}
			else {
				Int32 olderSubjectID = Math.Min(drSubject1.SubjectID, drSubject2.SubjectID);
				Int32 youngerSubjectID = Math.Max(drSubject1.SubjectID, drSubject2.SubjectID);

				Int32 motherOfYoungerSubjectID = CommonCalculations.MotherIDOfGen2Subject(youngerSubjectID);
				if ( olderSubjectID == motherOfYoungerSubjectID ) return RelationshipPath.ParentChild;
				else return RelationshipPath.AuntNiece;
			}
		}
		//private static EverSharedHouse SharedHouse ( RelationshipPath relationshipPath ) {
		private static bool SharedHouse ( RelationshipPath relationshipPath ) {

			switch ( relationshipPath ) {
				case RelationshipPath.ParentChild:
				case RelationshipPath.Gen1Housemates:
				case RelationshipPath.Gen2Siblings:
					return true; //EverSharedHouse.Yes;
				case RelationshipPath.AuntNiece:
				case RelationshipPath.Gen2Cousins:
					return false; //EverSharedHouse.No;
				default:
					throw new ArgumentOutOfRangeException("relationshipPath", relationshipPath, "The relationshipPath was not recognized.");
			}
		}
		#endregion
	}
}
