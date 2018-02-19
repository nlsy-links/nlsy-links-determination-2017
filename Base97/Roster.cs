using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nls.Base97 {
    public sealed class Roster{
        #region Fields
        private readonly LinksDataSet _dsLinks;
        //private readonly ItemYearCount _itemYearCount;
        private readonly Item[] _items = { Item.roster_relationship_1_dim, Item.pair_sister_same_bioparent, Item.pair_brother_same_bioparent }; //, Item.IDCodeOfOtherSiblingGen1, Item.IDOfOther1979RosterGen1
        private readonly string _itemIDsString = "";
        #endregion
        #region Constructor
        public Roster( LinksDataSet dsLinks ) {
            if( dsLinks == null ) throw new ArgumentNullException("dsLinks");
            if( dsLinks.tblSubject.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblSubject.");
            if( dsLinks.tblRelatedStructure.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblRelatedStructure.");
            if( dsLinks.tblRoster.Count != 0 ) throw new ArgumentException("There should be zero rows in tblMarkerGen1.");
            _dsLinks = dsLinks;
            //_itemYearCount = new ItemYearCount(_dsLinks);
            _itemIDsString = CommonCalculations.ConvertItemsToString(_items);
        }
        #endregion
        #region  Public Methods
        public string Go( ) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Retrieve.VerifyResponsesExistForItem(_items, _dsLinks);
            Int32 recordsAdded = 0;

            foreach( LinksDataSet.tblRelatedStructureRow drRelated in _dsLinks.tblRelatedStructure ) {
                Int32 subject1Tag = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.SubjectTag;
                Int32 subject2Tag = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject2.SubjectTag;

                Int32 internal_id_s1 = drRelated.hh_internal_id_s1;
                Int32 internal_id_s2 = drRelated.hh_internal_id_s2;

                LinksDataSet.tblResponseDataTable dtFamily = Retrieve.ExtendedFamilyRelevantResponseRows(drRelated.ExtendedID, _itemIDsString, 1, _dsLinks.tblResponse);
                EnumResponses.RosterChoice response1on2 = RosterResponseDeep(subject1Tag, internal_id_s2, dtFamily);
                EnumResponses.RosterChoice response2on1 = RosterResponseDeep(subject2Tag, internal_id_s1, dtFamily);

                Int16 responseLower = Math.Min((Int16)response1on2, (Int16)response2on1);
                Int16 responseUpper = Math.Max((Int16)response1on2, (Int16)response2on1);

                LinksDataSet.tblRosterAssignmentRow drLU = RetrieveAssignmentRow(responseLower, responseUpper);
                float r = float.MinValue;
                if( drLU.IsRNull() ) r = float.NaN;
                else r = (float)drLU.R;

                //const float r_dummy = .666F;

                AddRosterRow(drRelated.ID, drLU.ID, responseLower, responseUpper, drLU.Resolved, r, (float)drLU.RBoundLower, (float)drLU.RBoundUpper,
                    (Tristate)drLU.SameGeneration, (Tristate)drLU.ShareBiodad, (Tristate)drLU.ShareBiomom, (Tristate)drLU.ShareBiograndparent, drLU.Inconsistent);
                //AddRosterRow(drRelated.ID, 1, responseLower, responseUpper, false, r_dummy, r_dummy, r_dummy,
                //    Tristate.DoNotKnow, Tristate.DoNotKnow, Tristate.DoNotKnow, Tristate.DoNotKnow, true);
                recordsAdded += 1;
            }
            sw.Stop();
            string message = string.Format("{0:N0} Roster Records were processed.\nElapsed time: {1}", recordsAdded, sw.Elapsed.ToString());
            return message;
        }
        #endregion
        #region Public Static Methods
        internal static MarkerSummary RetrieveSummary( Int32 relatedIDLeft, LinksDataSet.tblRosterDataTable dtRoster ) {
            if( dtRoster == null ) throw new ArgumentNullException("dtRoster");
            LinksDataSet.tblRosterRow dr = dtRoster.FindByRelatedID(relatedIDLeft);

            MarkerEvidence sameGeneration = Assign.Evidence.RosterSameGeneration((Tristate)dr.SameGeneration);
            MarkerEvidence shareBiodad = Assign.Evidence.RosterShareBioParentOrGrandparent((Tristate)dr.ShareBiodad);
            MarkerEvidence shareBiomom = Assign.Evidence.RosterShareBioParentOrGrandparent((Tristate)dr.ShareBiomom);
            MarkerEvidence shareBiograndparent = Assign.Evidence.RosterShareBioParentOrGrandparent((Tristate)dr.ShareBiograndparent);

            return new MarkerSummary(sameGeneration, shareBiomom, shareBiodad, shareBiograndparent);
        }
        #endregion
        #region Private Methods -Tier 1

        private EnumResponses.RosterChoice RosterResponseDeep( Int32 subject_tag_a, Int32 internal_id_b, LinksDataSet.tblResponseDataTable dtFamily ) { //The tag of the respondent, and the internal id of the relative
            EnumResponses.RosterChoice shallow =  RetrieveResponse(subject_tag_a, internal_id_b, 1, dtFamily);
            switch( shallow ) {
                case EnumResponses.RosterChoice.brother_half_unsure:
                    return EnumResponses.RosterChoice.brother_half_unsure;
                case EnumResponses.RosterChoice.sister_half_unsure:
                    return EnumResponses.RosterChoice.sister_half_unsure;
                default:
                    return shallow;
            }

        }

        private EnumResponses.RosterChoice RetrieveResponse( Int32 subject1Tag, Int32 loop_index_1, Int32 loop_index_2, LinksDataSet.tblResponseDataTable dtFamily ) {
            //const Item itemID = Item.unique_id;
            const Item itemRelationship = Item.roster_relationship_1_dim;
            Int32 surveyYearCount = 1;  //The roster was asked only in 1997.

            string selectToShareResponse = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
                subject1Tag, dtFamily.SubjectTagColumn.ColumnName,
                loop_index_1, dtFamily.LoopIndex1Column.ColumnName,
                loop_index_2, dtFamily.LoopIndex2Column.ColumnName,
                (byte)itemRelationship, dtFamily.ItemColumn.ColumnName);
            LinksDataSet.tblResponseRow[] drsForShareResponse = (LinksDataSet.tblResponseRow[])dtFamily.Select(selectToShareResponse);
            Trace.Assert(drsForShareResponse.Length == surveyYearCount, "Exactly one row should be returned for the Item.Roster item to Subject2");
            
            return (EnumResponses.RosterChoice)drsForShareResponse[0].Value;
        }
        private LinksDataSet.tblRosterAssignmentRow RetrieveAssignmentRow( Int16 responseLower, Int16 responseUpper ) {
            string select = string.Format("{0}={1} AND {2}={3}",
                responseLower, _dsLinks.tblRosterAssignment.ResponseLowerColumn,
                responseUpper, _dsLinks.tblRosterAssignment.ResponseUpperColumn);

            LinksDataSet.tblRosterAssignmentRow[] drs = (LinksDataSet.tblRosterAssignmentRow[])_dsLinks.tblRosterAssignment.Select(select);
            Trace.Assert(drs.Length == 1, "Exactly one row should be returned for the Roster assignment");
            return drs[0];
        }
        #endregion
        #region Tier 2
        private void AddRosterRow( Int32 relatedID, byte rosterAssignmentID, Int16 responseLower, Int16 responseUpper, bool resolved, float r, float rBoundLower, float rBoundUpper,
            Tristate sameGeneration, Tristate shareBiodad, Tristate shareBiomom, Tristate shareBioGrandparent, bool inconsistent ) {
            LinksDataSet.tblRosterRow drNew = _dsLinks.tblRoster.NewtblRosterRow();
            //drNew.ExtendedID = extendedID	;
            drNew.RelatedID = relatedID;
            drNew.RosterAssignmentID = rosterAssignmentID;
            drNew.ResponseLower = responseLower;
            drNew.ResponseUpper = responseUpper;
            drNew.Resolved = resolved;
            if( float.IsNaN(r) ) drNew.SetRNull();
            else drNew.R = r;
            drNew.RBoundLower = rBoundLower;
            drNew.RBoundUpper = rBoundUpper;
            drNew.SameGeneration = (byte)sameGeneration;
            drNew.ShareBiodad = (byte)shareBiodad;
            drNew.ShareBiomom = (byte)shareBiomom;
            drNew.ShareBiograndparent = (byte)shareBioGrandparent;
            drNew.Inconsistent = inconsistent;
            _dsLinks.tblRoster.AddtblRosterRow(drNew);
        }
        #endregion
    }
}
