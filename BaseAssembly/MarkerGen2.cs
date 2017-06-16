using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nls.BaseAssembly.Trend;

namespace Nls.BaseAssembly {
	public sealed class MarkerGen2 {
		#region Fields
		private readonly LinksDataSet _dsLinks;
		//private readonly ItemYearCount _itemYearCount;
		private readonly Item[] _items = { Item.ShareBiodadGen2, Item.IDCodeOfOtherInterviewedBiodadGen2 };
		private readonly string _itemIDsString = "";
		#endregion
		#region Constructor
		public MarkerGen2 ( LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSubject.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblSubject.");
			if ( dsLinks.tblRelatedStructure.Count <= 0 ) throw new ArgumentException("There shouldn't be zero rows in tblRelatedStructure.");
			if ( dsLinks.tblMarkerGen2.Count != 0 ) throw new ArgumentException("There should be zero rows in tblMarkerGen2.");
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
			const Int16 extendedFamilyBegin = 0;// 12171;
			//Parallel.ForEach(_dsLinks.tblRelatedStructure, ( drRelated ) => {//Slower than serial
			foreach ( LinksDataSet.tblRelatedStructureRow drRelated in _dsLinks.tblRelatedStructure ) {//53sec
				if ( (RelationshipPath)drRelated.RelationshipPath == RelationshipPath.Gen2Siblings ) {
					if ( drRelated.ExtendedID >= extendedFamilyBegin ) {
						LinksDataSet.tblSubjectRow drBare1 = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1;
						LinksDataSet.tblSubjectRow drBare2 = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject2;
						Int16 extendedID = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.ExtendedID;
						Int32 subject1Tag = drBare1.SubjectTag;
						Int32 subject2Tag = drBare2.SubjectTag;

						LinksDataSet.tblResponseDataTable dtSubject1 = Retrieve.SubjectsRelevantResponseRows(subject1Tag, _itemIDsString, 0, _dsLinks.tblResponse);
						//LinksDataSet.tblResponseDataTable dtSubject2 = Retrieve.SubjectsRelevantResponseRows(subject2Tag, _itemIDsString, 0, _dsLinks.tblResponse);
						//SurveyTime.SubjectSurvey[] surveysSubject1 = SurveyTime.RetrieveSubjectSurveys(subject1Tag, _dsLinks);
						//SurveyTime.SubjectSurvey[] surveysSubject2 = SurveyTime.RetrieveSubjectSurveys(subject2Tag, _dsLinks);

						LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject1 = BabyDaddy.RetrieveRows(subject1Tag, _dsLinks);
						LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject2 = BabyDaddy.RetrieveRows(subject2Tag, _dsLinks);
						recordsAdded += FromShareBiodad(drRelated, dtSubject1, extendedID);
						if ( dtBabyDaddySubject1.Count > 0 && dtBabyDaddySubject2.Count > 0 ) {
							recordsAdded += FromBabyDaddyDeathDate(drRelated, dtBabyDaddySubject1, dtBabyDaddySubject2, extendedID);
							recordsAdded += FromBabyDaddyIsAlive(drRelated, dtBabyDaddySubject1, dtBabyDaddySubject2, extendedID);
							recordsAdded += FromBabyDaddyLeftHHDate(drRelated, dtBabyDaddySubject1, dtBabyDaddySubject2, extendedID);
							recordsAdded += FromBabyDaddyInHH(drRelated, dtBabyDaddySubject1, dtBabyDaddySubject2, extendedID);
							recordsAdded += FromBabyDaddyDistanceFromHH(drRelated, dtBabyDaddySubject1, dtBabyDaddySubject2, extendedID);
							recordsAdded += FromBabyDaddyAsthma(drRelated, dtBabyDaddySubject1, dtBabyDaddySubject2, extendedID);
						}

						LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject1 = FatherOfGen2.RetrieveRows(subject1Tag, _dsLinks);
						LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject2 = FatherOfGen2.RetrieveRows(subject2Tag, _dsLinks);
						if ( dtFatherSubject1.Count > 0 && dtFatherSubject2.Count > 0 ) {
							recordsAdded += FromFatherIsAlive(drRelated, dtFatherSubject1, dtFatherSubject2, extendedID);
							recordsAdded += FromFatherInHH(drRelated, dtFatherSubject1, dtFatherSubject2, extendedID);
							recordsAdded += FromFatherDistanceFromHH(drRelated, dtFatherSubject1, dtFatherSubject2, extendedID);
						}
					}
				}
				//drRelated.ID
			}
			sw.Stop();
			string message = string.Format("{0:N0} Gen2 Markers were processed.\n\nElapsed time: {1}", recordsAdded, sw.Elapsed.ToString());
			return message;
		}
		#endregion
		#region Private Methods -Tier 1 - Baby Daddy
		private Int32 FromBabyDaddyDeathDate ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject1, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.BabyDaddyDeathDate;
			const bool fromMother = true;
			Int16[] surveyYears = ItemYears.BabyDaddyDeathDate;
			DateTime?[] dates1 = BabyDaddy.RetrieveDeathDates(drRelated.SubjectTag_S1, surveyYears, dtBabyDaddySubject1);
			DateTime?[] dates2 = BabyDaddy.RetrieveDeathDates(drRelated.SubjectTag_S2, surveyYears, dtBabyDaddySubject2);

			TrendLineDate trend1 = new TrendLineDate(surveyYears, dates1);
			TrendLineDate trend2 = new TrendLineDate(surveyYears, dates2);
			TrendComparisonDate comparison = new TrendComparisonDate(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareBabyDaddy.Date(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		private Int32 FromBabyDaddyIsAlive ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject1, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.BabyDaddyAlive;
			const bool fromMother = true;
			Int16[] surveyYears = ItemYears.BabyDaddyIsAlive;
			Int16?[] points1 = BabyDaddy.RetrieveIsAlive(drRelated.SubjectTag_S1, surveyYears, dtBabyDaddySubject1);
			Int16?[] points2 = BabyDaddy.RetrieveIsAlive(drRelated.SubjectTag_S2, surveyYears, dtBabyDaddySubject2);

			TrendLineInteger trend1 = new TrendLineInteger(surveyYears, points1);
			TrendLineInteger trend2 = new TrendLineInteger(surveyYears, points2);
			TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareBabyDaddy.AliveOrAsthma(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		private Int32 FromBabyDaddyInHH ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject1, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.BabyDaddyInHH;
			const bool fromMother = true;
			Int16[] surveyYears = ItemYears.BabyDaddyInHH;
			Int16?[] values1 = BabyDaddy.RetrieveInHH(drRelated.SubjectTag_S1, surveyYears, dtBabyDaddySubject1);
			Int16?[] values2 = BabyDaddy.RetrieveInHH(drRelated.SubjectTag_S2, surveyYears, dtBabyDaddySubject2);

			TrendLineInteger trend1 = new TrendLineInteger(surveyYears, values1);
			TrendLineInteger trend2 = new TrendLineInteger(surveyYears, values2);
			TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareBabyDaddy.InHH(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		private Int32 FromBabyDaddyLeftHHDate ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject1, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.BabyDaddyLeftHHDate;
			const bool fromMother = true;
			Int16[] surveyYears = ItemYears.BabyDaddyLeftHHDate;
			DateTime?[] dates1 = BabyDaddy.RetrieveLeftHHDates(drRelated.SubjectTag_S1, surveyYears, dtBabyDaddySubject1);
			DateTime?[] dates2 = BabyDaddy.RetrieveLeftHHDates(drRelated.SubjectTag_S2, surveyYears, dtBabyDaddySubject2);

			TrendLineDate trend1 = new TrendLineDate(surveyYears, dates1);
			TrendLineDate trend2 = new TrendLineDate(surveyYears, dates2);
			TrendComparisonDate comparison = new TrendComparisonDate(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareBabyDaddy.Date(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		private Int32 FromBabyDaddyDistanceFromHH ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject1, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.BabyDaddyDistanceFromHH;
			const bool fromMother = true;
			Int16[] surveyYears = ItemYears.BabyDaddyDistanceFromHHFuzzyCeiling;
			Int16?[] distances1 = BabyDaddy.RetrieveDistanceFromHH(drRelated.SubjectTag_S1, surveyYears, dtBabyDaddySubject1);
			Int16?[] distances2 = BabyDaddy.RetrieveDistanceFromHH(drRelated.SubjectTag_S2, surveyYears, dtBabyDaddySubject2);

			TrendLineInteger trend1 = new TrendLineInteger(surveyYears, distances1);
			TrendLineInteger trend2 = new TrendLineInteger(surveyYears, distances2);
			TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareBabyDaddy.DistanceFromHH(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		private Int32 FromBabyDaddyAsthma ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject1, LinksDataSet.tblBabyDaddyDataTable dtBabyDaddySubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.BabyDaddyAsthma;
			const bool fromMother = true;
			Int16[] surveyYears = ItemYears.BabyDaddyAsthma;
			Int16?[] distances1 = BabyDaddy.RetrieveAsthma(drRelated.SubjectTag_S1, surveyYears, dtBabyDaddySubject1);
			Int16?[] distances2 = BabyDaddy.RetrieveAsthma(drRelated.SubjectTag_S2, surveyYears, dtBabyDaddySubject2);

			TrendLineInteger trend1 = new TrendLineInteger(surveyYears, distances1);
			TrendLineInteger trend2 = new TrendLineInteger(surveyYears, distances2);
			TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareBabyDaddy.AliveOrAsthma(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		#endregion
		#region Private Methods -Tier 1 - Father of Gen2
		private Int32 FromShareBiodad ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblResponseDataTable dtSubject1, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.ShareBiodad;
			const bool fromMother = true;
			LinksDataSet.tblSubjectRow drSubject1 = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1;
			LinksDataSet.tblSubjectRow drSubject2 = drRelated.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject2;
			Int32 lastTwoDigitsSubject2 = CommonFunctions.LastTwoDigitsOfGen2SubjectID(drSubject2);
			Int32 surveyYearCount = ItemYears.Gen2ShareBiodad.Length;

			string selectToGetLoopIndex = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				drSubject1.SubjectTag, dtSubject1.SubjectTagColumn.ColumnName,
				(byte)Item.IDCodeOfOtherInterviewedBiodadGen2, dtSubject1.ItemColumn.ColumnName,
				lastTwoDigitsSubject2, dtSubject1.ValueColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsForLoopIndex = (LinksDataSet.tblResponseRow[])dtSubject1.Select(selectToGetLoopIndex);
			Trace.Assert(drsForLoopIndex.Length <= surveyYearCount, string.Format("No more than {0} row(s) should be returned that matches Subject2 for item '{1}'.", surveyYearCount, Item.IDCodeOfOtherInterviewedBiodadGen2.ToString()));
			Int32 recordsAdded = 0;

			foreach ( LinksDataSet.tblResponseRow drResponse in drsForLoopIndex ) {
				string selectToShareResponse = string.Format("{0}={1} AND {2}={3} AND {4}={5} AND {6}={7}",
					drSubject1.SubjectTag, dtSubject1.SubjectTagColumn.ColumnName,
					(byte)Item.ShareBiodadGen2, dtSubject1.ItemColumn.ColumnName,
					drResponse.LoopIndex, dtSubject1.LoopIndexColumn.ColumnName,
					drResponse.SurveyYear, dtSubject1.SurveyYearColumn.ColumnName);
				LinksDataSet.tblResponseRow[] drsForShareResponse = (LinksDataSet.tblResponseRow[])dtSubject1.Select(selectToShareResponse);
				Trace.Assert(drsForShareResponse.Length == 1, "Exactly one row should be returned for the ShareBiodad item to Subject2");
				EnumResponsesGen2.ShareBiodadGen2 shareBiodad = (EnumResponsesGen2.ShareBiodadGen2)drsForShareResponse[0].Value;

				MarkerEvidence mzEvidence = Assign.EvidenceGen2.ShareBiodadForBioparents(shareBiodad);
				MarkerEvidence biodadEvidence = Assign.EvidenceGen2.ShareBiodadForBioparents(shareBiodad);
				AddMarkerRow(extendedID, drRelated.ID, markerType, drResponse.SurveyYear, mzEvidence, biodadEvidence, fromMother);
				recordsAdded += 1;
			}
			return recordsAdded;
		}
		private Int32 FromFatherIsAlive ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject1, LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.Gen2CFatherAlive;
			const bool fromMother = false;
			Int16[] surveyYears = ItemYears.Gen2CFatherAlive;
			Int16?[] points1 = FatherOfGen2.RetrieveIsAlive(drRelated.SubjectTag_S1, surveyYears, dtFatherSubject1);
			Int16?[] points2 = FatherOfGen2.RetrieveIsAlive(drRelated.SubjectTag_S2, surveyYears, dtFatherSubject2);

			TrendLineInteger trend1 = new TrendLineInteger(surveyYears, points1);
			TrendLineInteger trend2 = new TrendLineInteger(surveyYears, points2);
			TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareGen2Father.AliveOrAsthma(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		private Int32 FromFatherInHH ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject1, LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.Gen2CFatherInHH;
			const bool fromMother = false;
			Int16[] surveyYears = ItemYears.Gen2CFatherInHH;
			Int16?[] values1 = FatherOfGen2.RetrieveInHH(drRelated.SubjectTag_S1, surveyYears, dtFatherSubject1);
			Int16?[] values2 = FatherOfGen2.RetrieveInHH(drRelated.SubjectTag_S2, surveyYears, dtFatherSubject2);

			TrendLineInteger trend1 = new TrendLineInteger(surveyYears, values1);
			TrendLineInteger trend2 = new TrendLineInteger(surveyYears, values2);
			TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareGen2Father.InHH(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		private Int32 FromFatherDistanceFromHH ( LinksDataSet.tblRelatedStructureRow drRelated, LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject1, LinksDataSet.tblFatherOfGen2DataTable dtFatherSubject2, Int16 extendedID ) {
			const MarkerType markerType = MarkerType.Gen2CFatherDistanceFromHH;
			const bool fromMother = false;
			Int16[] surveyYears = ItemYears.Gen2CFatherDistanceFromMotherFuzzyCeiling;
			Int16?[] distances1 = FatherOfGen2.RetrieveDistanceFromHH(drRelated.SubjectTag_S1, surveyYears, dtFatherSubject1);
			Int16?[] distances2 = FatherOfGen2.RetrieveDistanceFromHH(drRelated.SubjectTag_S2, surveyYears, dtFatherSubject2);

			TrendLineInteger trend1 = new TrendLineInteger(surveyYears, distances1);
			TrendLineInteger trend2 = new TrendLineInteger(surveyYears, distances2);
			TrendComparisonInteger comparison = new TrendComparisonInteger(trend1, trend2);
			MarkerEvidence mzEvidence = DetermineShareGen2Father.DistanceFromHH(comparison);
			MarkerEvidence biodadEvidence = mzEvidence;
			return AddMarkerRow(extendedID, drRelated.ID, markerType, comparison.LastNonMutualNullPointsYear, mzEvidence, biodadEvidence, fromMother);
		}
		#endregion
		#region Tier 2
		private Int32 AddMarkerRow ( Int16 extendedID, Int32 relatedID, MarkerType markerType, Int16? surveyYear, MarkerEvidence mzEvidence, MarkerEvidence biodadEvidence, bool fromMother ) {
			lock ( _dsLinks.tblMarkerGen2 ) {
				//if ( !surveyYear.HasValue ) return 0;

				LinksDataSet.tblMarkerGen2Row drNew = _dsLinks.tblMarkerGen2.NewtblMarkerGen2Row();
				drNew.ExtendedID = extendedID;
				drNew.RelatedID = relatedID;
				drNew.MarkerType = (byte)markerType;

				if ( surveyYear.HasValue ) drNew.SurveyYear = surveyYear.Value;
				else drNew.SetSurveyYearNull();

				drNew.MzEvidence = (byte)mzEvidence;
				drNew.ShareBiodadEvidence = (byte)biodadEvidence;
				drNew.FromMother = fromMother;

				_dsLinks.tblMarkerGen2.AddtblMarkerGen2Row(drNew);
			}
			return 1;
		}
		#endregion
		#region Static Methods
		internal static MarkerEvidence RetrieveBiodadMarkerFromGen1 ( Int64 relatedIDLeft, MarkerType markerType, LinksDataSet.tblMarkerGen2DataTable dtMarker ) {
			if ( dtMarker == null ) throw new ArgumentNullException("dtMarker");
			string select = string.Format("{0}={1} AND {2}={3}",
				relatedIDLeft, dtMarker.RelatedIDColumn.ColumnName,
				(byte)markerType, dtMarker.MarkerTypeColumn.ColumnName);
			string sort = dtMarker.SurveyYearColumn.ColumnName;
			LinksDataSet.tblMarkerGen2Row[] drs = (LinksDataSet.tblMarkerGen2Row[])dtMarker.Select(select, sort);
			Trace.Assert(drs.Length <= 1, "The number of returns markers should not exceed 1.");
			if ( drs.Length == 0 )
				return MarkerEvidence.Missing;
			else
				return (MarkerEvidence)drs[0].ShareBiodadEvidence;
		}

		internal static MarkerGen2Summary[] RetrieveMarkers ( Int64 relatedIDLeft, MarkerType markerType, LinksDataSet.tblMarkerGen2DataTable dtMarker, Int32 maxCount ) {
			if ( dtMarker == null ) throw new ArgumentNullException("dtMarker");
			string select = string.Format("{0}={1} AND {2}={3}",
				relatedIDLeft, dtMarker.RelatedIDColumn.ColumnName,
				(byte)markerType, dtMarker.MarkerTypeColumn.ColumnName);
			string sort = dtMarker.SurveyYearColumn.ColumnName;
			LinksDataSet.tblMarkerGen2Row[] drs = (LinksDataSet.tblMarkerGen2Row[])dtMarker.Select(select, sort);
			Trace.Assert(drs.Length <= maxCount, "The number of returns markers should not exceed " + maxCount + ".");
			MarkerGen2Summary[] evidences = new MarkerGen2Summary[drs.Length];
			for ( Int32 i = 0; i < drs.Length; i++ ) {
				evidences[i] = new MarkerGen2Summary(drs[i].SurveyYear, (MarkerEvidence)drs[i].MzEvidence, (MarkerEvidence)drs[i].ShareBiodadEvidence);
			}
			return evidences;
		}
		internal static LinksDataSet.tblMarkerGen2DataTable PairRelevantMarkerRows ( Int64 relatedIDLeft, Int64 relatedIDRight, LinksDataSet dsLinks, Int32 extendedID ) {
			string select = string.Format("{0}={1} AND {2} IN ({3},{4})",
				extendedID, dsLinks.tblMarkerGen2.ExtendedIDColumn.ColumnName,
				dsLinks.tblMarkerGen2.RelatedIDColumn.ColumnName, relatedIDLeft, relatedIDRight);
			LinksDataSet.tblMarkerGen2Row[] drs = (LinksDataSet.tblMarkerGen2Row[])dsLinks.tblMarkerGen2.Select(select);
			//if ( drs.Length <= 0 ) {
			//   return null;
			//}
			//else {
			LinksDataSet.tblMarkerGen2DataTable dt = new LinksDataSet.tblMarkerGen2DataTable();
			foreach ( LinksDataSet.tblMarkerGen2Row dr in drs ) {
				dt.ImportRow(dr);
			}
			return dt;
		}
		//}
		#endregion
	}
}
