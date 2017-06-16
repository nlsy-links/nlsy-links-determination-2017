using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nls.BaseAssembly {
	public sealed class RosterGen1 {
		#region Fields
		private readonly LinksDataSet _dsLinks;
		//private readonly ItemYearCount _itemYearCount;
		private readonly Item[] _items = { Item.IDOfOther1979RosterGen1, Item.RosterGen1979 }; //, Item.IDCodeOfOtherSiblingGen1
		private readonly string _itemIDsString = "";
		#endregion
		#region Constructor
		public RosterGen1 ( LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSubject.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblSubject.");
			if ( dsLinks.tblRelatedStructure.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblRelatedStructure.");
			if ( dsLinks.tblRelatedStructure.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblRelatedStructure.");
			if ( dsLinks.tblRosterGen1.Count != 0 ) throw new ArgumentException("There should be zero rows in tblMarkerGen1.");
			_dsLinks = dsLinks;
			//_itemYearCount = new ItemYearCount(_dsLinks);
			_itemIDsString = CommonCalculations.ConvertItemsToString(_items);
		}
		#endregion
		#region  Public Methods
		public string Go ( ) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Retrieve.VerifyResponsesExistForItem(_items, _dsLinks);
			Int32 recordsAdded = 0;

			foreach ( LinksDataSet.tblRelatedStructureRow drRelated in _dsLinks.tblRelatedStructure ) {
				if ( (RelationshipPath)drRelated.RelationshipPath == RelationshipPath.Gen1Housemates ) {
					Int32 subject1Tag = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.SubjectTag;
					Int32 subject2Tag = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject2.SubjectTag;
					//if ( subject1Tag < subject2Tag ) {
					Int32 subject1ID = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.SubjectID;
					Int32 subject2ID = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject2.SubjectID;

					LinksDataSet.tblResponseDataTable dtFamily = Retrieve.ExtendedFamilyRelevantResponseRows(drRelated.ExtendedID, _itemIDsString, 1, _dsLinks.tblResponse);
					EnumResponsesGen1.Gen1Roster response1on2 = RetrieveResponse(subject1Tag, subject2ID, dtFamily);
					EnumResponsesGen1.Gen1Roster response2on1 = RetrieveResponse(subject2Tag, subject1ID, dtFamily);

					Int16 responseLower = Math.Min((Int16)response1on2, (Int16)response2on1);
					Int16 responseUpper = Math.Max((Int16)response1on2, (Int16)response2on1);

					LinksDataSet.tblLURosterGen1AssignmentRow drLU = RetrieveAssignmentRow(responseLower, responseUpper);
					float r = float.MinValue;
					if ( drLU.IsRNull() ) r = float.NaN;
					else r = (float)drLU.R;

					AddRosterRow(drRelated.ID, drLU.ID, responseLower, responseUpper, drLU.Resolved, r, (float)drLU.RBoundLower, (float)drLU.RBoundUpper,
						(Tristate)drLU.SameGeneration, (Tristate)drLU.ShareBiodad, (Tristate)drLU.ShareBiomom, (Tristate)drLU.ShareBiograndparent, drLU.Inconsistent);
					recordsAdded += 1;
				}
			}
			sw.Stop();
			string message = string.Format("{0:N0} Roster Records were processed.\nElapsed time: {1}", recordsAdded, sw.Elapsed.ToString());
			return message;
		}
		#endregion
		#region Public Static Methods
		internal static MarkerGen1Summary RetrieveSummary ( Int32 relatedIDLeft, LinksDataSet.tblRosterGen1DataTable dtRoster ) {
			if ( dtRoster == null ) throw new ArgumentNullException("dtRoster");
			LinksDataSet.tblRosterGen1Row dr = dtRoster.FindByRelatedID(relatedIDLeft);

			MarkerEvidence sameGeneration = Assign.EvidenceGen1.RosterSameGeneration((Tristate)dr.SameGeneration);
			MarkerEvidence shareBiodad = Assign.EvidenceGen1.RosterShareBioParentOrGrandparent((Tristate)dr.ShareBiodad);
			MarkerEvidence shareBiomom = Assign.EvidenceGen1.RosterShareBioParentOrGrandparent((Tristate)dr.ShareBiomom);
			MarkerEvidence shareBiograndparent = Assign.EvidenceGen1.RosterShareBioParentOrGrandparent((Tristate)dr.ShareBiograndparent);

			return new MarkerGen1Summary(sameGeneration, shareBiomom, shareBiodad, shareBiograndparent);
		}
		#endregion
		#region Private Methods -Tier 1
		private EnumResponsesGen1.Gen1Roster RetrieveResponse ( Int32 subject1Tag, Int32 subject2ID, LinksDataSet.tblResponseDataTable dtFamily ) {
			const Item itemID = Item.IDOfOther1979RosterGen1;
			const Item itemRelationship = Item.RosterGen1979;
			Int32 surveyYearCount = 1;  //The roster was asked only in 1979.

			//Use the other subject's ID to find the appropriate 'loop index';
			string selectToGetLoopIndex = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				subject1Tag, dtFamily.SubjectTagColumn.ColumnName,
				(byte)itemID, dtFamily.ItemColumn.ColumnName,
				subject2ID, dtFamily.ValueColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsForLoopIndex = (LinksDataSet.tblResponseRow[])dtFamily.Select(selectToGetLoopIndex);
			Trace.Assert(drsForLoopIndex.Length <= surveyYearCount, string.Format("No more than {0} row(s) should be returned that matches Subject2 for item '{1}'.", surveyYearCount, itemID.ToString()));

			//Use the loop index (that corresponds to the other subject) to find the roster response.
			LinksDataSet.tblResponseRow drResponse = drsForLoopIndex[0];
			string selectToShareResponse = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				subject1Tag, dtFamily.SubjectTagColumn.ColumnName,
				(byte)itemRelationship, dtFamily.ItemColumn.ColumnName,
				drResponse.LoopIndex, dtFamily.LoopIndexColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsForShareResponse = (LinksDataSet.tblResponseRow[])dtFamily.Select(selectToShareResponse);
			Trace.Assert(drsForShareResponse.Length == 1, "Exactly one row should be returned for the Item.RosterGen1979 item to Subject2");
			return (EnumResponsesGen1.Gen1Roster)drsForShareResponse[0].Value;
		}
		private LinksDataSet.tblLURosterGen1AssignmentRow RetrieveAssignmentRow ( Int16 responseLower, Int16 responseUpper ) {
			string select = string.Format("{0}={1} AND {2}={3}",
				responseLower, _dsLinks.tblLURosterGen1Assignment.ResponseLowerColumn,
				responseUpper, _dsLinks.tblLURosterGen1Assignment.ResponseUpperColumn);

			LinksDataSet.tblLURosterGen1AssignmentRow[] drs = (LinksDataSet.tblLURosterGen1AssignmentRow[])_dsLinks.tblLURosterGen1Assignment.Select(select);
			Trace.Assert(drs.Length == 1, "Exactly one row should be returned for the Roster assignment");
			return drs[0];
		}
		#endregion
		#region Tier 2
		private void AddRosterRow ( Int32 relatedID, byte rosterAssignmentID, Int16 responseLower, Int16 responseUpper, bool resolved, float r, float rBoundLower, float rBoundUpper,
			Tristate sameGeneration, Tristate shareBiodad, Tristate shareBiomom, Tristate shareBioGrandparent, bool inconsistent ) {
			LinksDataSet.tblRosterGen1Row drNew = _dsLinks.tblRosterGen1.NewtblRosterGen1Row();
			//drNew.ExtendedID = extendedID	;
			drNew.RelatedID = relatedID;
			drNew.RosterAssignmentID = rosterAssignmentID;
			drNew.ResponseLower = responseLower;
			drNew.ResponseUpper = responseUpper;
			drNew.Resolved = resolved;
			if ( float.IsNaN(r) ) drNew.SetRNull();
			else drNew.R = r;
			drNew.RBoundLower = rBoundLower;
			drNew.RBoundUpper = rBoundUpper;
			drNew.SameGeneration = (byte)sameGeneration;
			drNew.ShareBiodad = (byte)shareBiodad;
			drNew.ShareBiomom = (byte)shareBiomom;
			drNew.ShareBiograndparent = (byte)shareBioGrandparent;
			drNew.Inconsistent = inconsistent;
			_dsLinks.tblRosterGen1.AddtblRosterGen1Row(drNew);
		}
		#endregion
	}
}
