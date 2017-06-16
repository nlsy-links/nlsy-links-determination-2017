using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Nls.BaseAssembly.Trend;

namespace Nls.BaseAssembly {
	public sealed class MarkerGen1 {
		#region Fields
		private readonly LinksDataSet _dsLinks;
		private readonly ImportDataSet _dsImport;
		private readonly Item[] _items = { Item.IDOfOther1979RosterGen1, Item.RosterGen1979, Item.IDCodeOfOtherSiblingGen1, Item.ShareBiomomGen1, Item.ShareBiodadGen1};
			//Unnecssary b/c they're in Retro & Current: Item.Gen1MotherDeathAge, Item.Gen1MotherBirthCountry, Item.Gen1MotherBirthYear, Item.Gen1FatherDeathAge, Item.Gen1FatherBirthCountry, Item.Gen1FatherBirthYear
		private readonly string _itemIDsString = "";
		#endregion
		#region Constructor
		public MarkerGen1 ( LinksDataSet dsLinks, ImportDataSet dsImport ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSubject.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblSubject.");
			if ( dsLinks.tblRelatedStructure.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblRelatedStructure.");
			if ( dsLinks.tblRosterGen1.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblRosterGen1.");
			if ( dsLinks.tblParentsOfGen1Current.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblParentsOfGen1Current.");
			if ( dsLinks.tblParentsOfGen1Retro.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblParentsOfGen1Retro.");
			if ( dsImport.tblGeocodeSanitized.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblGeocodeSanitized.");
			if ( dsLinks.tblMarkerGen1.Count != 0 ) throw new ArgumentException("There should be zero rows in tblMarkerGen1.");
			_dsLinks = dsLinks;
			_dsImport = dsImport;
			_itemIDsString = CommonCalculations.ConvertItemsToString(_items);
		}
		#endregion
		#region  Public Methods
		public string Go ( ) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Retrieve.VerifyResponsesExistForItem(_items, _dsLinks);
			Int16[] bioparentLiveYears = { 1980, 1979, 1978, 1977, 1976 };
			Int32 recordsAdded = 0;
			Int16[] extendedIDs = CommonFunctions.CreateExtendedFamilyIDs(_dsLinks);
			foreach ( Int16 extendedID in extendedIDs ) {
				LinksDataSet.tblRelatedStructureRow[] drsRelated = Retrieve.RelatedStructureInExtendedFamily(extendedID, RelationshipPath.Gen1Housemates, _dsLinks.tblRelatedStructure);
				LinksDataSet.tblParentsOfGen1RetroDataTable dtRetroForExtended = ParentsOfGen1Retro.RetrieveRows(extendedID, _dsLinks.tblParentsOfGen1Retro);

				foreach ( LinksDataSet.tblRelatedStructureRow drRelated in drsRelated ) {
					Int32 subject1Tag = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.SubjectTag;
					Int32 subject2Tag = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject2.SubjectTag;
					LinksDataSet.tblResponseDataTable dtSubject1 = Retrieve.SubjectsRelevantResponseRows(subject1Tag, _itemIDsString, 1, _dsLinks.tblResponse);
					LinksDataSet.tblParentsOfGen1CurrentDataTable dtParentsCurrent = ParentsOfGen1Current.RetrieveRows(subject1Tag, subject2Tag, _dsLinks);
					LinksDataSet.tblParentsOfGen1RetroDataTable dtParentsRetro = ParentsOfGen1Retro.RetrieveRows(subject1Tag, subject2Tag, dtRetroForExtended);

					recordsAdded += FromRoster(drRelated, dtSubject1);
					recordsAdded += FromShareExplicit(Item.ShareBiomomGen1, MarkerType.ShareBiomom, drRelated, dtSubject1);
					recordsAdded += FromShareExplicit(Item.ShareBiodadGen1, MarkerType.ShareBiodad, drRelated, dtSubject1);

                    recordsAdded += FromAlwaysLivedWithBothBioparents(drRelated, dtParentsCurrent);
					foreach ( Int16 year in bioparentLiveYears ) {
						recordsAdded += FromLiveWithBioparent(year, Bioparent.Mom, drRelated, dtParentsRetro);
						recordsAdded += FromLiveWithBioparent(year, Bioparent.Dad, drRelated, dtParentsRetro);
					}

					recordsAdded += FromBioparentDeathAge(Bioparent.Mom, drRelated, dtParentsCurrent);
					recordsAdded += FromBioparentDeathAge(Bioparent.Dad, drRelated, dtParentsCurrent);

					recordsAdded += FromBioparentBirthCountry(Bioparent.Mom, drRelated, _dsImport.tblGeocodeSanitized);
					recordsAdded += FromBioparentBirthCountry(Bioparent.Dad, drRelated, _dsImport.tblGeocodeSanitized);

					recordsAdded += FromBioparentBirthState(Bioparent.Mom, drRelated, _dsImport.tblGeocodeSanitized);
					recordsAdded += FromBioparentBirthState(Bioparent.Dad, drRelated, _dsImport.tblGeocodeSanitized);

					foreach ( Int16 year in ItemYears.Gen1BioparentBirthYear ) {
						recordsAdded += FromBioparentBirthYear(Bioparent.Mom, year, drRelated, dtParentsCurrent);
						recordsAdded += FromBioparentBirthYear(Bioparent.Dad, year, drRelated, dtParentsCurrent);
					}

				}
			}
			sw.Stop();
			string message = string.Format("{0:N0} Gen1 Markers were processed.\nElapsed time: {1}", recordsAdded, sw.Elapsed.ToString());
			return message;
		}
		#endregion
		#region Private Methods -Tier 1
        private Int32 FromAlwaysLivedWithBothBioparents( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblParentsOfGen1CurrentDataTable dtCurrent) {
            const Int16 year = ItemYears.Gen1BioparentInHH;
            const MarkerType markerType = MarkerType.Gen1AlwaysLivedWithBothBioparents;       
            Tristate subject1 = ParentsOfGen1Current.RetrieveAlwaysLiveWithBothBioparents(drRelated.SubjectTag_S1, dtCurrent);
            Tristate subject2 = ParentsOfGen1Current.RetrieveAlwaysLiveWithBothBioparents(drRelated.SubjectTag_S2, dtCurrent);
            
            MarkerEvidence shareBioparent = MarkerEvidence.Missing;

            if( (subject1 == Tristate.Yes) || (subject2 == Tristate.Yes) )
                shareBioparent = MarkerEvidence.StronglySupports;
            else 
                shareBioparent = MarkerEvidence.Ambiguous;

            AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, year, mzEvidence: MarkerEvidence.Irrelevant, sameGenerationEvidence: MarkerEvidence.Irrelevant,
                biomomEvidence: shareBioparent, biodadEvidence: shareBioparent, biograndparentEvidence: MarkerEvidence.Ambiguous);
            const Int32 recordsAdded = 1;
            return recordsAdded;
        }
        private Int32 FromLiveWithBioparent( Int16 year, Bioparent bioparent, LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblParentsOfGen1RetroDataTable dtRetro ) {
            Tristate subject1 = ParentsOfGen1Retro.RetrieveInHHByYear(drRelated.SubjectTag_S1, bioparent, year, dtRetro);
            Tristate subject2 = ParentsOfGen1Retro.RetrieveInHHByYear(drRelated.SubjectTag_S2, bioparent, year, dtRetro);
            MarkerEvidence shareBioparent = MarkerEvidence.Missing;

            if( (subject1 == Tristate.DoNotKnow) || (subject2 == Tristate.DoNotKnow) )
                shareBioparent = MarkerEvidence.Missing;
            else if( subject1 != subject2 )
                shareBioparent = MarkerEvidence.Unlikely;
            else if( (subject1 == Tristate.Yes) || (subject2 == Tristate.Yes) )
                shareBioparent = MarkerEvidence.StronglySupports;
            else if( (subject1 == Tristate.No) || (subject2 == Tristate.No) )
                shareBioparent = MarkerEvidence.Consistent;
            else
                throw new InvalidOperationException("All the conditions should have been caught.");

            MarkerEvidence mzEvidence = MarkerEvidence.Missing;
            MarkerEvidence shareBiomom = MarkerEvidence.Irrelevant;
            MarkerEvidence shareBiodad = MarkerEvidence.Irrelevant;

            switch( shareBioparent ) {
                case MarkerEvidence.StronglySupports:
                //case MarkerEvidence.Supports:
                case MarkerEvidence.Consistent:
                    mzEvidence = MarkerEvidence.Consistent;
                    break;
                case MarkerEvidence.Unlikely:
                    mzEvidence = MarkerEvidence.Unlikely;
                    break;
                //case MarkerEvidence.Disconfirms:
                //   mzEvidence = MarkerEvidence.Disconfirms;
                //   break;
                case MarkerEvidence.Missing:
                    mzEvidence = MarkerEvidence.Missing;
                    break;
                default:
                    throw new InvalidOperationException("The switch should not have gotten here.");
            }

            MarkerType markerType;
            switch( bioparent ) {
                case Bioparent.Dad:
                    markerType = MarkerType.Gen1BiodadInHH;
                    shareBiodad = shareBioparent;
                    break;
                case Bioparent.Mom:
                    markerType = MarkerType.Gen1BiomomInHH;
                    shareBiomom = shareBioparent;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("bioparent", bioparent, "The 'bioparent' value wasn't recognized.");
            }
            AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, year, mzEvidence: mzEvidence, sameGenerationEvidence: MarkerEvidence.Irrelevant,
                biomomEvidence: shareBiomom, biodadEvidence: shareBiodad, biograndparentEvidence: MarkerEvidence.Ambiguous);
            const Int32 recordsAdded = 1;
            return recordsAdded;
        }
        private Int32 FromBioparentBirthCountry( Bioparent bioparent, LinksDataSet.tblRelatedStructureRow drRelated, ImportDataSet.tblGeocodeSanitizedDataTable dtGeocode ) {
			const Int16 year = ItemYears.Gen1BioparentBirthCountry;
			Int32 subjectSmaller = Math.Min(drRelated.SubjectTag_S1, drRelated.SubjectTag_S2);
			Int32 subjectLarger = Math.Max(drRelated.SubjectTag_S1, drRelated.SubjectTag_S2);
			ImportDataSet.tblGeocodeSanitizedRow drGeo = dtGeocode.FindBySubjectTag_S1SubjectTag_S2(subjectSmaller, subjectLarger);

			MarkerType markerType;

			MarkerEvidence mzEvidence = MarkerEvidence.Irrelevant;
			MarkerEvidence shareBiomom = MarkerEvidence.Irrelevant;
			MarkerEvidence shareBiodad = MarkerEvidence.Irrelevant;

			switch ( bioparent ) {
				case Bioparent.Mom:
					markerType = MarkerType.Gen1BiomomBirthCountry;
					if ( drGeo.BirthMotherCountryMissing_1 || drGeo.BirthMotherCountryMissing_2 )
						shareBiomom = MarkerEvidence.Missing;
					else if ( drGeo.BirthMotherCountryEqual )
						shareBiomom = MarkerEvidence.Consistent;
					else if ( !drGeo.BirthMotherCountryEqual )
						shareBiomom = MarkerEvidence.Disconfirms;
					else
						throw new InvalidOperationException("The execution should not have gotten here.");
					mzEvidence = shareBiomom;
					break;
				case Bioparent.Dad:
					markerType = MarkerType.Gen1BiodadBirthCountry;
					if ( drGeo.BirthFatherCountryMissing_1 || drGeo.BirthFatherCountryMissing_2 )
						shareBiodad = MarkerEvidence.Missing;
					else if ( drGeo.BirthFatherCountryEqual )
						shareBiodad = MarkerEvidence.Consistent;
					else if ( !drGeo.BirthFatherCountryEqual )
						shareBiodad = MarkerEvidence.Disconfirms;
					else
						throw new InvalidOperationException("The execution should not have gotten here.");
					mzEvidence = shareBiodad;
					break;
				default:
					throw new ArgumentOutOfRangeException("bioparent", bioparent, "The 'FromShareBioparent' function does not accommodate this bioparent value.");
			}

			AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, year, mzEvidence: mzEvidence, sameGenerationEvidence: MarkerEvidence.Irrelevant,
				biomomEvidence: shareBiomom, biodadEvidence: shareBiodad, biograndparentEvidence: MarkerEvidence.Ambiguous);
			const Int32 recordsAdded = 1;
			return recordsAdded;
		}
		private Int32 FromBioparentBirthState ( Bioparent bioparent, LinksDataSet.tblRelatedStructureRow drRelated, ImportDataSet.tblGeocodeSanitizedDataTable dtGeocode ) {
			const Int16 year = ItemYears.Gen1BioparentBirthState;
			Int32 subjectSmaller = Math.Min(drRelated.SubjectTag_S1, drRelated.SubjectTag_S2);
			Int32 subjectLarger = Math.Max(drRelated.SubjectTag_S1, drRelated.SubjectTag_S2);
			ImportDataSet.tblGeocodeSanitizedRow drGeo = dtGeocode.FindBySubjectTag_S1SubjectTag_S2(subjectSmaller, subjectLarger);

			MarkerType markerType;

			MarkerEvidence mzEvidence = MarkerEvidence.Irrelevant;
			MarkerEvidence shareBiomom = MarkerEvidence.Irrelevant;
			MarkerEvidence shareBiodad = MarkerEvidence.Irrelevant;

			switch ( bioparent ) {
				case Bioparent.Mom:
					markerType = MarkerType.Gen1BiomomBirthState;
					if ( drGeo.BirthMotherStateMissing_1 || drGeo.BirthMotherStateMissing_2 )
						shareBiomom = MarkerEvidence.Missing;
					else if ( drGeo.BirthMotherStateEqual )
						shareBiomom = MarkerEvidence.Consistent;
					else if ( !drGeo.BirthMotherStateEqual )
						shareBiomom = MarkerEvidence.Disconfirms;
					else
						throw new InvalidOperationException("The execution should not have gotten here.");
					mzEvidence = shareBiomom;
					break;
				case Bioparent.Dad:
					markerType = MarkerType.Gen1BiodadBirthState;
					if ( drGeo.BirthFatherStateMissing_1 || drGeo.BirthFatherStateMissing_2 )
						shareBiodad = MarkerEvidence.Missing;
					else if ( drGeo.BirthFatherStateEqual )
						shareBiodad = MarkerEvidence.Consistent;
					else if ( !drGeo.BirthFatherStateEqual )
						shareBiodad = MarkerEvidence.Disconfirms;
					else
						throw new InvalidOperationException("The execution should not have gotten here.");
					mzEvidence = shareBiodad;
					break;
				default:
					throw new ArgumentOutOfRangeException("bioparent", bioparent, "The 'FromShareBioparent' function does not accommodate this bioparent value.");
			}

			AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, year, mzEvidence: mzEvidence, sameGenerationEvidence: MarkerEvidence.Irrelevant,
				biomomEvidence: shareBiomom, biodadEvidence: shareBiodad, biograndparentEvidence: MarkerEvidence.Ambiguous);
			const Int32 recordsAdded = 1;
			return recordsAdded;
		}
		private Int32 FromBioparentBirthYear ( Bioparent bioparent, Int16 surveyYear, LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblParentsOfGen1CurrentDataTable dtParentsOfGen1Current ) {
			//const year = ItemYears.Gen1BioparentBirthYear;
			Int16? birthYear1 = null;
			Int16? birthYear2 = null;

			MarkerType markerType;
			switch ( bioparent ) {
				case Bioparent.Mom: markerType = MarkerType.Gen1BiomomBirthYear; break;
				case Bioparent.Dad: markerType = MarkerType.Gen1BiodadBirthYear; break;
				default: throw new ArgumentOutOfRangeException("bioparent", bioparent, "The function does not accommodate this bioparent value.");
			}

			birthYear1 = ParentsOfGen1Current.RetrieveBirthYear(drRelated.SubjectTag_S1, bioparent, dtParentsOfGen1Current);
			birthYear2 = ParentsOfGen1Current.RetrieveBirthYear(drRelated.SubjectTag_S2, bioparent, dtParentsOfGen1Current);

			if ( !birthYear1.HasValue || !birthYear2.HasValue )
				return 0;

			Int32 gap = Convert.ToInt32(Math.Abs(birthYear2.Value - birthYear1.Value));

			MarkerEvidence shareBioparent = MarkerEvidence.Missing;
			if ( gap == 0 ) shareBioparent = MarkerEvidence.Supports;
			else if ( gap <= 5 ) shareBioparent = MarkerEvidence.Consistent;
			else if ( gap <= 15 ) shareBioparent = MarkerEvidence.Unlikely;
			else shareBioparent = MarkerEvidence.Disconfirms;

			MarkerEvidence mzEvidence = MarkerEvidence.Missing;
			MarkerEvidence shareBiomom = MarkerEvidence.Irrelevant;
			MarkerEvidence shareBiodad = MarkerEvidence.Irrelevant;

			switch ( shareBioparent ) {
				case MarkerEvidence.StronglySupports:
				case MarkerEvidence.Supports:
				case MarkerEvidence.Consistent:
					mzEvidence = MarkerEvidence.Consistent;
					break;
				case MarkerEvidence.Unlikely:
					mzEvidence = MarkerEvidence.Unlikely;
					break;
				case MarkerEvidence.Disconfirms:
					mzEvidence = MarkerEvidence.Disconfirms;
					break;
				default:
					throw new InvalidOperationException("The switch should not have gotten here.");
			}

			switch ( bioparent ) {
				case Bioparent.Mom: shareBiomom = shareBioparent; break;
				case Bioparent.Dad: shareBiodad = shareBioparent; break;
				default: throw new ArgumentOutOfRangeException("bioparent", bioparent, "The 'FromShareBioparent' function does not accommodate this bioparent value.");
			}
			AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, surveyYear, mzEvidence: mzEvidence, sameGenerationEvidence: MarkerEvidence.Irrelevant,
				biomomEvidence: shareBiomom, biodadEvidence: shareBiodad, biograndparentEvidence: MarkerEvidence.Ambiguous);
			return 1;
		}
		private Int32 FromBioparentDeathAge (Bioparent bioparent , LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblParentsOfGen1CurrentDataTable dtParentsOfGen1Current ) {
			const Int16 surveyYear = ItemYears.Gen1BioparentDeathAge;
			byte? deathAge1 = null;
			byte? deathAge2 = null;

			MarkerType markerType;
			switch ( bioparent ) {
				case Bioparent.Mom: markerType = MarkerType.Gen1BiomomDeathAge; break;
				case Bioparent.Dad: markerType = MarkerType.Gen1BiodadDeathAge; break;
				default: throw new ArgumentOutOfRangeException("bioparent", bioparent, "The 'FromShareBioparent' function does not accommodate this bioparent value.");
			}
			
			deathAge1 = ParentsOfGen1Current.RetrieveDeathAge(drRelated.SubjectTag_S1, bioparent, dtParentsOfGen1Current);
			deathAge2 = ParentsOfGen1Current.RetrieveDeathAge(drRelated.SubjectTag_S2, bioparent, dtParentsOfGen1Current);

			if ( !deathAge1.HasValue || !deathAge2.HasValue )
				return 0;

			Int32 gap = Convert.ToInt32(Math.Abs(deathAge2.Value - deathAge1.Value));

			MarkerEvidence shareBioparent = MarkerEvidence.Missing;
			if ( (gap == 0) && (deathAge1.Value < 55) ) shareBioparent = MarkerEvidence.StronglySupports;
			else if ( gap == 0 ) shareBioparent = MarkerEvidence.Supports;
			else if ( gap <= 5 ) shareBioparent = MarkerEvidence.Consistent;
			else if ( gap <= 25 ) shareBioparent = MarkerEvidence.Unlikely;
			else shareBioparent = MarkerEvidence.Disconfirms;

			MarkerEvidence mzEvidence = MarkerEvidence.Missing;
			MarkerEvidence shareBiomom = MarkerEvidence.Irrelevant;
			MarkerEvidence shareBiodad = MarkerEvidence.Irrelevant;

			switch ( shareBioparent ) {
				case MarkerEvidence.StronglySupports:
				case MarkerEvidence.Supports:
				case MarkerEvidence.Consistent:
					mzEvidence = MarkerEvidence.Consistent;
					break;
				case MarkerEvidence.Unlikely:
					mzEvidence = MarkerEvidence.Unlikely;
					break;
				case MarkerEvidence.Disconfirms:
					mzEvidence = MarkerEvidence.Disconfirms;
					break;
				default:
					throw new InvalidOperationException("The switch should not have gotten here.");
			}

			switch ( bioparent ) {
				case Bioparent.Mom: shareBiomom = shareBioparent; break;
				case Bioparent.Dad: shareBiodad = shareBioparent; break;
				default: throw new ArgumentOutOfRangeException("bioparent", bioparent, "The 'FromShareBioparent' function does not accommodate this bioparent value.");
			}
			AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, surveyYear, mzEvidence: mzEvidence, sameGenerationEvidence: MarkerEvidence.Irrelevant,
				biomomEvidence: shareBiomom, biodadEvidence: shareBiodad, biograndparentEvidence: MarkerEvidence.Ambiguous);
			return 1;
		}
		private Int32 FromRoster ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblResponseDataTable dtSubject1 ) {
			const MarkerType markerType = MarkerType.RosterGen1;
			MarkerGen1Summary roster = RosterGen1.RetrieveSummary(drRelated.ID, _dsLinks.tblRosterGen1);

			MarkerEvidence mzEvidence;
			if ( roster.ShareBiomom == MarkerEvidence.StronglySupports && roster.ShareBiodad == MarkerEvidence.StronglySupports )
				mzEvidence = MarkerEvidence.Consistent;
			else
				mzEvidence = MarkerEvidence.Disconfirms;

			AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, ItemYears.Gen1Roster, mzEvidence, roster.SameGeneration, roster.ShareBiomom, roster.ShareBiodad, roster.ShareBiograndparent);
			const Int32 recordsAdded = 1;
			return recordsAdded;
		}
		private Int32 FromShareExplicit ( Item itemRelationship, MarkerType markerType, LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblResponseDataTable dtSubject1 ) {
			const Item itemID = Item.IDCodeOfOtherSiblingGen1;
			Int32 surveyYearCount = ItemYears.Gen1ShareBioparent.Length;

			LinksDataSet.tblSubjectRow drSubject1 = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1;
			LinksDataSet.tblSubjectRow drSubject2 = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject2;

			//Use the other subject's ID to find the appropriate 'loop index';
			string selectToGetLoopIndex = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				drSubject1.SubjectTag, dtSubject1.SubjectTagColumn.ColumnName,
				Convert.ToInt16(itemID), dtSubject1.ItemColumn.ColumnName,
				drSubject2.SubjectID, dtSubject1.ValueColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsForLoopIndex = (LinksDataSet.tblResponseRow[])dtSubject1.Select(selectToGetLoopIndex);
			Trace.Assert(drsForLoopIndex.Length <= surveyYearCount, string.Format("No more than {0} row(s) should be returned that matches Subject2 for item '{1}'.", surveyYearCount, itemID.ToString()));

			if ( drsForLoopIndex.Length == 0 )
				return 0;

			//Use the loop index (that corresponds to the other subject) to find the ShareBiomom response.
			//LinksDataSet.tblResponseRow drResponse = drsForLoopIndex[0];
			string selectToShareResponse = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				drSubject1.SubjectTag, dtSubject1.SubjectTagColumn.ColumnName,
				Convert.ToInt16(itemRelationship), dtSubject1.ItemColumn.ColumnName,
				drsForLoopIndex[0].LoopIndex, dtSubject1.LoopIndexColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsForShareResponse = (LinksDataSet.tblResponseRow[])dtSubject1.Select(selectToShareResponse);
			Trace.Assert(drsForLoopIndex.Length <= surveyYearCount, string.Format("No more than {0} row(s) should be returned that matches Subject2 for item '{1}'.", surveyYearCount, Item.IDCodeOfOtherInterviewedBiodadGen2.ToString()));
			Int32 recordsAdded = 0;

			foreach ( LinksDataSet.tblResponseRow drResponse in drsForShareResponse ) {
				EnumResponsesGen1.ShareBioparentGen1 shareBioparent = (EnumResponsesGen1.ShareBioparentGen1)drResponse.Value;

				MarkerEvidence evidence = Assign.EvidenceGen1.ShareBioparentsForBioparents(shareBioparent);
				MarkerEvidence mzEvidence;
				if ( evidence == MarkerEvidence.StronglySupports ) mzEvidence = MarkerEvidence.Consistent;
				else mzEvidence = MarkerEvidence.Disconfirms;

				MarkerEvidence sameGeneration;
				if ( evidence == MarkerEvidence.Supports || evidence == MarkerEvidence.StronglySupports )
					sameGeneration = MarkerEvidence.StronglySupports;
				else
					sameGeneration = MarkerEvidence.Ambiguous;

				switch ( markerType ) {
					case MarkerType.ShareBiodad:
						AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, drResponse.SurveyYear, mzEvidence: mzEvidence, sameGenerationEvidence: sameGeneration,
							biomomEvidence: MarkerEvidence.Irrelevant, biodadEvidence: evidence, biograndparentEvidence: evidence);
						break;
					case MarkerType.ShareBiomom:
						AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, drResponse.SurveyYear, mzEvidence: mzEvidence, sameGenerationEvidence: sameGeneration,
							biomomEvidence: evidence, biodadEvidence: MarkerEvidence.Irrelevant, biograndparentEvidence: evidence);
						break;
					default:
						throw new ArgumentOutOfRangeException("markerType", markerType, "The 'FromShareBioparent' function does not accommodate this markerType.");
				}
				recordsAdded += 1;
			}
			return recordsAdded;
		}
		#endregion
		#region Tier 2
		private void AddMarkerRow ( Int16 extendedID, Int32 relatedID, MarkerType markerType, Int16 surveyYear, MarkerEvidence mzEvidence, MarkerEvidence sameGenerationEvidence, MarkerEvidence biomomEvidence, MarkerEvidence biodadEvidence, MarkerEvidence biograndparentEvidence ) {
			LinksDataSet.tblMarkerGen1Row drNew = _dsLinks.tblMarkerGen1.NewtblMarkerGen1Row();
			drNew.ExtendedID = extendedID;
			drNew.RelatedID = relatedID;
			drNew.MarkerType = Convert.ToByte(markerType);
			drNew.SurveyYear = surveyYear;
			drNew.MzEvidence = Convert.ToByte(mzEvidence);
			drNew.SameGeneration = Convert.ToByte(sameGenerationEvidence);
			drNew.ShareBiomomEvidence = Convert.ToByte(biomomEvidence);
			drNew.ShareBiodadEvidence = Convert.ToByte(biodadEvidence);
			drNew.ShareBioGrandparentEvidence = Convert.ToByte(biograndparentEvidence);

			_dsLinks.tblMarkerGen1.AddtblMarkerGen1Row(drNew);
		}
		#endregion
		#region Public Static Methods
		internal static MarkerEvidence RetrieveParentMarkerSingleYear ( Int64 relatedIDLeft, MarkerType markerType, Bioparent bioparent, LinksDataSet.tblMarkerGen1DataTable dtMarker ) {
			if ( dtMarker == null ) throw new ArgumentNullException("dtMarker");
			string select = string.Format("{0}={1} AND {2}={3}",
				relatedIDLeft, dtMarker.RelatedIDColumn.ColumnName,
				(byte)markerType, dtMarker.MarkerTypeColumn.ColumnName);
			string sort = dtMarker.SurveyYearColumn.ColumnName;
			LinksDataSet.tblMarkerGen1Row[] drs = (LinksDataSet.tblMarkerGen1Row[])dtMarker.Select(select, sort);
			Trace.Assert(drs.Length <= 1, "The number of returns markers should not exceed 1.");
			if ( drs.Length == 0 )
				return MarkerEvidence.Missing;
			else if ( bioparent == Bioparent.Dad )
				return (MarkerEvidence)drs[0].ShareBiodadEvidence;
			else if ( bioparent == Bioparent.Mom )
				return (MarkerEvidence)drs[0].ShareBiomomEvidence;
			else
				throw new ArgumentOutOfRangeException("markerType", markerType, "The 'bioparent' value is not accepted by this function.");
		}
		internal static MarkerEvidence RetrieveParentMarkerMultiYear ( Int64 relatedIDLeft, MarkerType markerType, Int16 year, Bioparent bioparent, LinksDataSet.tblMarkerGen1DataTable dtMarker ) {
			if ( dtMarker == null ) throw new ArgumentNullException("dtMarker");
			string select = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				relatedIDLeft, dtMarker.RelatedIDColumn.ColumnName,
				year, dtMarker.SurveyYearColumn.ColumnName,
				(byte)markerType, dtMarker.MarkerTypeColumn.ColumnName);
			string sort = dtMarker.SurveyYearColumn.ColumnName;
			LinksDataSet.tblMarkerGen1Row[] drs = (LinksDataSet.tblMarkerGen1Row[])dtMarker.Select(select, sort);
			Trace.Assert(drs.Length <= 1, "The number of returns markers should not exceed 1.");
			if ( drs.Length == 0 )
				return MarkerEvidence.Missing;
			else if ( bioparent == Bioparent.Dad )
				return (MarkerEvidence)drs[0].ShareBiodadEvidence;
			else if ( bioparent == Bioparent.Mom )
				return (MarkerEvidence)drs[0].ShareBiomomEvidence;
			else
				throw new ArgumentOutOfRangeException("markerType", markerType, "The 'bioparent' value is not accepted by this function.");
		}
		internal static MarkerGen1Summary[] RetrieveMarkers ( Int64 relatedIDLeft, MarkerType markerType, LinksDataSet.tblMarkerGen1DataTable dtMarker, Int32 maxCount ) {
			if ( dtMarker == null ) throw new ArgumentNullException("dtMarker");
			string select = string.Format("{0}={1} AND {2}={3}",
				relatedIDLeft, dtMarker.RelatedIDColumn.ColumnName,
				(byte)markerType, dtMarker.MarkerTypeColumn.ColumnName);
			LinksDataSet.tblMarkerGen1Row[] drs = (LinksDataSet.tblMarkerGen1Row[])dtMarker.Select(select);
			Trace.Assert(drs.Length <= maxCount, "The number of returns markers should not exceed " + maxCount + ".");
			MarkerGen1Summary[] evidences = new MarkerGen1Summary[drs.Length];
			for ( Int32 i = 0; i < drs.Length; i++ ) {
				evidences[i] = new MarkerGen1Summary((
					MarkerEvidence)drs[i].SameGeneration,
					(MarkerEvidence)drs[i].ShareBiomomEvidence,
					(MarkerEvidence)drs[i].ShareBiodadEvidence,
					(MarkerEvidence)drs[i].ShareBioGrandparentEvidence
				);
			}
			return evidences;
		}
		internal static LinksDataSet.tblMarkerGen1DataTable PairRelevantMarkerRows ( Int64 relatedIDLeft, Int64 relatedIDRight, LinksDataSet dsLinks, Int32 extendedID ) {
			string select = string.Format("{0}={1} AND {2} IN ({3},{4})",
				extendedID, dsLinks.tblMarkerGen1.ExtendedIDColumn.ColumnName,
				dsLinks.tblMarkerGen1.RelatedIDColumn.ColumnName, relatedIDLeft, relatedIDRight);
			LinksDataSet.tblMarkerGen1Row[] drs = (LinksDataSet.tblMarkerGen1Row[])dsLinks.tblMarkerGen1.Select(select);
			//if ( drs.Length <= 0 ) {
			//   return null;
			//}
			//else {
			LinksDataSet.tblMarkerGen1DataTable dt = new LinksDataSet.tblMarkerGen1DataTable();
			foreach ( LinksDataSet.tblMarkerGen1Row dr in drs ) {
				dt.ImportRow(dr);
			}
			return dt;
		}
		#endregion
	}
}
//private Int32 FromGen0InHH ( Bioparent bioparent, LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblParentsOfGen1RetroDataTable dtRetro ) {
//   TrendLineGen0InHH subject1 = ParentsOfGen1Retro.RetrieveTrend(bioparent, drRelated.SubjectTag_S1, dtRetro);
//   TrendLineGen0InHH subject2 = ParentsOfGen1Retro.RetrieveTrend(bioparent, drRelated.SubjectTag_S2, dtRetro);
//   //Int16?[] values2 = BabyDaddy.RetrieveInHH(drRelated.SubjectTag_S2, surveyYears, dtSubject2);

//   //TrendLineInteger trend1 = new TrendLineInteger(surveyYears, values1);
//   //TrendLineInteger trend2 = new TrendLineInteger(surveyYears, values2);
//   //TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
//   //MarkerEvidence mzEvidence = DetermineShareBabyDaddy.InHH(comparison);
//   //MarkerEvidence biodadEvidence = mzEvidence;
//   //return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
//   //throw new NotImplementedException();
//   return 0;
//}
//private Int32 FromBioparentUSBorn ( Bioparent bioparent, LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblParentsOfGen1CurrentDataTable dtCurrent ) {
//   const Int16 year = ItemYears.Gen1BioparentUSBorn;
//   Item item;
//   MarkerType markerType;
//   switch ( bioparent ) {
//      case Bioparent.Mom: item = Item.Gen1MotherBirthCountry; markerType = MarkerType.Gen1BiomomUSBorn; break;
//      case Bioparent.Dad: item = Item.Gen1FatherBirthCountry; markerType = MarkerType.Gen1BiodadUSBorn; break;
//      default: throw new ArgumentOutOfRangeException("bioparent", bioparent, "The 'FromShareBioparent' function does not accommodate this bioparent value.");
//   }

//   Tristate subject1 = ParentsOfGen1Current.RetrieveUSBorn(drRelated.SubjectTag_S1, item, dtCurrent);
//   Tristate subject2 = ParentsOfGen1Current.RetrieveUSBorn(drRelated.SubjectTag_S2, item, dtCurrent);
//   MarkerEvidence shareBioparent = MarkerEvidence.Missing;

//   if ( (subject1 == Tristate.DoNotKnow) || (subject2 == Tristate.DoNotKnow) )
//      shareBioparent = MarkerEvidence.Missing;
//   else if ( subject1 != subject2 )
//      shareBioparent = MarkerEvidence.Disconfirms;
//   else if ( (subject1 == subject2) )
//      shareBioparent = MarkerEvidence.Consistent;
//   else
//      throw new InvalidOperationException("All the conditions should have been caught.");

//   MarkerEvidence mzEvidence = shareBioparent;
//   MarkerEvidence shareBiomom = MarkerEvidence.Irrelevant;
//   MarkerEvidence shareBiodad = MarkerEvidence.Irrelevant;

//   switch ( item ) {
//      case Item.Gen1MotherBirthCountry: shareBiomom = shareBioparent; break;
//      case Item.Gen1FatherBirthCountry: shareBiodad = shareBioparent; break;
//      default: throw new ArgumentOutOfRangeException("item", item, "The 'item' value isn't accommodated by this function.");
//   }
//   AddMarkerRow(drRelated.ExtendedID, drRelated.ID, markerType, year, mzEvidence: mzEvidence, sameGenerationEvidence: MarkerEvidence.Irrelevant,
//      biomomEvidence: shareBiomom, biodadEvidence: shareBiodad, biograndparentEvidence: MarkerEvidence.Ambiguous);
//   const Int32 recordsAdded = 1;
//   return recordsAdded;
//}
