using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nls.BaseAssembly {
	public sealed class FatherOfGen2 {
		#region Fields
		private readonly LinksDataSet _ds;
		private readonly Item[] _items = { Item.Gen2CFatherLivingInHH, Item.Gen2CFatherAlive, Item.Gen2CFatherDistanceFromMotherFuzzyCeiling };
		private readonly string _itemIDsString = "";
		#endregion
		#region Constructor
		public FatherOfGen2 ( LinksDataSet ds ) {
			if ( ds == null ) throw new ArgumentNullException("ds");
			if ( ds.tblResponse.Count <= 0 ) throw new InvalidOperationException("tblResponse must NOT be empty.");
			if ( ds.tblFatherOfGen2.Count != 0 ) throw new InvalidOperationException("tblFatherOfGen2 must be empty before creating rows for it.");
			_ds = ds;

			_itemIDsString = CommonCalculations.ConvertItemsToString(_items);
		}
		#endregion
		#region Public Methods
		public string Go ( ) {
			const Int32 minRowCount = 0;//There are some extended families with no children.
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Retrieve.VerifyResponsesExistForItem(_items, _ds);
			Int32 recordsAddedTotal = 0;
			_ds.tblFatherOfGen2.BeginLoadData();

			Int16[] extendedIDs = CommonFunctions.CreateExtendedFamilyIDs(_ds);
			//Parallel.ForEach(extendedIDs, ( extendedID ) => {//
			foreach ( Int16 extendedID in extendedIDs ) {
				LinksDataSet.tblResponseDataTable dtExtendedResponse = Retrieve.ExtendedFamilyRelevantResponseRows(extendedID, _itemIDsString, minRowCount, _ds.tblResponse);
				LinksDataSet.tblSubjectRow[] subjectsInExtendedFamily = Retrieve.SubjectsInExtendFamily(extendedID, _ds.tblSubject);
				foreach ( LinksDataSet.tblSubjectRow drSubject in subjectsInExtendedFamily ) {
					if ( (Generation)drSubject.Generation == Generation.Gen2 ) {
						Int32 recordsAddedForLoop = ProcessSubjectGen2(drSubject, dtExtendedResponse);
						Interlocked.Add(ref recordsAddedTotal, recordsAddedForLoop);
					}
				}
			}
			_ds.tblFatherOfGen2.EndLoadData();
			sw.Stop();
			return string.Format("{0:N0} FatherOfGen2 records were created.\nElapsed time: {1}", recordsAddedTotal, sw.Elapsed.ToString());
		}
		#endregion
		#region Private Methods
		private Int32 ProcessSubjectGen2 ( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			Int32 subjectTag = drSubject.SubjectTag;

			Int32 recordsAdded = 0;
			SurveyTime.SubjectSurvey[] subjectSurveys = SurveyTime.RetrieveSubjectSurveys(subjectTag, _ds);
			foreach ( Int16 surveyYear in ItemYears.Gen2CFatherItems ) {
				SurveySource source = SurveyTime.DetermineSurveySource(surveyYear, subjectSurveys);
				if ( source == SurveySource.Gen2C ) {
					YesNo biodadInHH = DetermineBiodadInHH(source, surveyYear, subjectTag, dtExtendedResponse);
					YesNo biodadAlive = DetermineBiodadAlive(biodadInHH, surveyYear, subjectTag, dtExtendedResponse);
					Int16? biodadDistanceFromHH = DetermineBiodadDistance(surveyYear, subjectTag, dtExtendedResponse);
					YesNo biodadAsthma = YesNo.ValidSkipOrNoInterviewOrNotInSurvey;//DetermineBiodadAsthma(surveyYear, motherTag, childLoopIndex.Value, dtExtended);

					if ( biodadDistanceFromHH.HasValue && biodadDistanceFromHH.Value > 0 && biodadInHH == YesNo.Yes ) {
						biodadInHH = YesNo.No;
						biodadAlive = YesNo.Yes;
					}

					AddRow(subjectTag, surveyYear, biodadInHH, biodadAlive, biodadDistanceFromHH, biodadAsthma);
					recordsAdded += 1;
				}
			}
			return recordsAdded;
		}
		private static YesNo DetermineBiodadInHH ( SurveySource source, Int16 surveyYear, Int32 subjectTag, LinksDataSet.tblResponseDataTable dtExtended ) {
			Item item;
			switch ( source ) {
				case SurveySource.Gen2C: item = Item.Gen2CFatherLivingInHH; break;
				//case SurveySource.Gen2YA: item = Item.Gen2CFathedrLivingInHH;
				default: throw new ArgumentOutOfRangeException("source", source, "Only (nonmissing) Gen2 sources are allowed.");
			}
			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, subjectTag, dtExtended);
			if ( !response.HasValue )
				return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
			EnumResponsesGen2.FatherOfGen2LiveInHH codedResponse = (EnumResponsesGen2.FatherOfGen2LiveInHH)response.Value;
			switch ( codedResponse ) {
				case EnumResponsesGen2.FatherOfGen2LiveInHH.InvalidSkip: return YesNo.Refusal;
				case EnumResponsesGen2.FatherOfGen2LiveInHH.DoNotKnow: return YesNo.DoNotKnow;
				case EnumResponsesGen2.FatherOfGen2LiveInHH.Refusal: return YesNo.Refusal;
				case EnumResponsesGen2.FatherOfGen2LiveInHH.No: return YesNo.No;
				case EnumResponsesGen2.FatherOfGen2LiveInHH.Yes: return YesNo.Yes;
				default: throw new InvalidOperationException("The response " + codedResponse + " was not recognized.");
			}
		}
		private static YesNo DetermineBiodadAlive ( YesNo biobdadInHH, Int16 surveyYear, Int32 subjectTag, LinksDataSet.tblResponseDataTable dtMother ) {
			const Item item = Item.Gen2CFatherAlive;
			if ( biobdadInHH == YesNo.Yes ) return YesNo.Yes;

			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, subjectTag, dtMother);
			if ( !response.HasValue )
				return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
			EnumResponsesGen2.FatherOfGen2Living codedResponse = (EnumResponsesGen2.FatherOfGen2Living)response.Value;
			switch ( codedResponse ) {
				case EnumResponsesGen2.FatherOfGen2Living.InvalidSkip: return YesNo.Refusal;
				case EnumResponsesGen2.FatherOfGen2Living.DoNotKnow: return YesNo.DoNotKnow;
				case EnumResponsesGen2.FatherOfGen2Living.Refusal: return YesNo.Refusal;
				case EnumResponsesGen2.FatherOfGen2Living.No: return YesNo.No;
				case EnumResponsesGen2.FatherOfGen2Living.Yes: return YesNo.Yes;
				default: throw new InvalidOperationException("The response " + codedResponse + " was not recognized.");
			}
		}
		private static Int16? DetermineBiodadDistance ( Int16 surveyYear, Int32 subjectTag, LinksDataSet.tblResponseDataTable dtMother ) {
			const Item item = Item.Gen2CFatherDistanceFromMotherFuzzyCeiling;
			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, subjectTag, dtMother);
			if ( response.HasValue )
				return (Int16)response;
			else
				return null;
		}
		//private static YesNo DetermineBiodadAsthma ( Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtMother ) {
		//   if ( SurveyYears.FatherOfGen2Asthma.Contains(surveyYear) ) {
		//      const Item item = Item.FatherOfGen2HasAsthmaGen1;
		//      Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, motherTag, childLoopIndex, dtMother);
		//      if ( response.HasValue )
		//         return (YesNo)response;
		//      else
		//         return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
		//   }
		//   else {
		//      return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
		//   }
		//}
		private void AddRow ( Int32 subjectTag, Int16 surveyYear, YesNo biodadInHH, YesNo biodadAlive, Int16? biodadDistanceFromHH, YesNo biodadAsthma ) {
			lock ( _ds.tblFatherOfGen2 ) {
				LinksDataSet.tblFatherOfGen2Row drNew = _ds.tblFatherOfGen2.NewtblFatherOfGen2Row();
				drNew.SubjectTag = subjectTag;
				drNew.SurveyYear = surveyYear;
				drNew.BiodadInHH = (Int16)biodadInHH;
				drNew.BiodadAlive = (Int16)biodadAlive;

				if ( biodadDistanceFromHH.HasValue ) drNew.BiodadDistanceFromHH = biodadDistanceFromHH.Value;
				else drNew.SetBiodadDistanceFromHHNull();

				drNew.BiodadAsthma = (Int16)biodadAsthma;

				_ds.tblFatherOfGen2.AddtblFatherOfGen2Row(drNew);
			}
		}
		#endregion
		#region Public/Private Static
		public static LinksDataSet.tblFatherOfGen2DataTable RetrieveRows ( Int32 subjectTag, LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblFatherOfGen2.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblFatherOfGen2.");

			string select = string.Format("{0}={1}", subjectTag, dsLinks.tblFatherOfGen2.SubjectTagColumn.ColumnName);
			LinksDataSet.tblFatherOfGen2Row[] drs = (LinksDataSet.tblFatherOfGen2Row[])dsLinks.tblFatherOfGen2.Select(select);
			//Trace.Assert(drs.Length >= 1, "There should be at least one row.");
			LinksDataSet.tblFatherOfGen2DataTable dt = new LinksDataSet.tblFatherOfGen2DataTable();
			foreach ( LinksDataSet.tblFatherOfGen2Row dr in drs ) {
				dt.ImportRow(dr);
			}
			return dt;
		}
		private static LinksDataSet.tblFatherOfGen2Row RetrieveRow ( Int32 subjectTag, Int16 surveyYear, LinksDataSet.tblFatherOfGen2DataTable dtInput ) {
			string select = string.Format("{0}={1} AND {2}={3}",
				subjectTag, dtInput.SubjectTagColumn.ColumnName,
				surveyYear, dtInput.SurveyYearColumn.ColumnName);
			LinksDataSet.tblFatherOfGen2Row[] drs = (LinksDataSet.tblFatherOfGen2Row[])dtInput.Select(select);
			//if ( drs == null ) {
			if ( drs.Length <=0 ) {
				return null;
			}
			else {
				Trace.Assert(drs.Length <= 1, "There should be no more than one row.");
				return drs[0];
			}
		}
		public static Int16?[] RetrieveIsAlive ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblFatherOfGen2DataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblFatherOfGen2.");

			Int16?[] values = new Int16?[surveyYears.Length];
			for ( Int32 i = 0; i < values.Length; i++ ) {
				LinksDataSet.tblFatherOfGen2Row dr = RetrieveRow(subjectTag, surveyYears[i], dtInput);
				if ( dr == null )
					values[i] = null;
				else if ( (YesNo)dr.BiodadAlive == YesNo.ValidSkipOrNoInterviewOrNotInSurvey )
					values[i] = null;
				else if ( (YesNo)dr.BiodadAlive == YesNo.InvalidSkip )
					values[i] = null;
				else
					values[i] = dr.BiodadAlive;
			}
			return values;
		}
		public static Int16?[] RetrieveInHH ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblFatherOfGen2DataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblFatherOfGen2.");

			Int16?[] values = new Int16?[surveyYears.Length];
			for ( Int32 i = 0; i < values.Length; i++ ) {
				LinksDataSet.tblFatherOfGen2Row dr = RetrieveRow(subjectTag, surveyYears[i], dtInput);
				if ( dr == null )
					values[i] = null;
				else if ( (YesNo)dr.BiodadInHH == YesNo.ValidSkipOrNoInterviewOrNotInSurvey )
					values[i] = null;
				else if ( (YesNo)dr.BiodadInHH == YesNo.InvalidSkip )
					values[i] = null;
				else
					values[i] = dr.BiodadInHH;
			}
			return values;
		}
		public static Int16?[] RetrieveDistanceFromHH ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblFatherOfGen2DataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblFatherOfGen2.");

			Int16?[] distances = new Int16?[surveyYears.Length];
			for ( Int32 i = 0; i < distances.Length; i++ ) {
				LinksDataSet.tblFatherOfGen2Row dr = RetrieveRow(subjectTag, surveyYears[i], dtInput);
				if ( dr == null )
					distances[i] = null;
				else if ( dr.IsBiodadDistanceFromHHNull() )
					distances[i] = null;
				else if ( (YesNo)dr.BiodadDistanceFromHH == YesNo.InvalidSkip )
					distances[i] = null;
				else
					distances[i] = dr.BiodadDistanceFromHH;
			}
			return distances;
		}

		#endregion
	}
}
