using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nls.BaseAssembly {
	public sealed class BabyDaddy {
		#region Fields
		private readonly LinksDataSet _ds;
		private readonly Item[] _items = { Item.Gen1ChildsIDByBirthOrder,
														Item.BabyDaddyInHH, Item.BabyDaddyAlive, Item.BabyDaddyEverLiveInHH, Item.BabyDaddyLeftHHMonth,  Item.BabyDaddyLeftHHYearFourDigit,
														Item.BabyDaddyDeathMonth, Item.BabyDaddyDeathYearTwoDigit, Item.BabyDaddyDeathYearFourDigit,
														Item.BabyDaddyDistanceFromMotherFuzzyCeiling,Item.BabyDaddyHasAsthma,
														Item.BabyDaddyLeftHHMonthOrNeverInHH,Item.BabyDaddyLeftHHYearTwoDigit};
		private readonly string _itemIDsString = "";
		#endregion
		#region Constructor
		public BabyDaddy ( LinksDataSet ds ) {
			if ( ds == null ) throw new ArgumentNullException("ds");
			if ( ds.tblResponse.Count <= 0 ) throw new InvalidOperationException("tblResponse must NOT be empty.");
			if ( ds.tblBabyDaddy.Count != 0 ) throw new InvalidOperationException("tblBabyDaddy must be empty before creating rows for it.");
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
			_ds.tblBabyDaddy.BeginLoadData();

			Int16[] extendedIDs = CommonFunctions.CreateExtendedFamilyIDs(_ds);
			ParallelOptions options = new ParallelOptions();
			options.MaxDegreeOfParallelism = -1;
			Parallel.ForEach(extendedIDs, options, ( extendedID ) => {//11.5 sec
				//foreach ( Int32 extendedID in extendedIDs ) {
				LinksDataSet.tblResponseDataTable dtExtendedResponse = Retrieve.ExtendedFamilyRelevantResponseRows(extendedID, _itemIDsString, minRowCount, _ds.tblResponse);
				LinksDataSet.tblSubjectRow[] subjectsInExtendedFamily = Retrieve.SubjectsInExtendFamily(extendedID, _ds.tblSubject);
				foreach ( LinksDataSet.tblSubjectRow drSubject in subjectsInExtendedFamily ) {
					if ( (Generation)drSubject.Generation == Generation.Gen2 ) {
						Int32 recordsAddedForLoop = ProcessSubjectGen2(drSubject, dtExtendedResponse);
						Interlocked.Add(ref recordsAddedTotal, recordsAddedForLoop);
					}
				}
			});
			_ds.tblBabyDaddy.EndLoadData();
			sw.Stop();
			return string.Format("{0:N0} BabyDaddy records were created.\nElapsed time: {1}", recordsAddedTotal, sw.Elapsed.ToString());
			throw new NotImplementedException();
		}
		#endregion
		#region Private Methods
		private Int32 ProcessSubjectGen2 ( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			Int32 subjectTag = drSubject.SubjectTag;
			Int32 motherTag = CommonCalculations.MotherTagOfGen2Subject(drSubject.SubjectID);
			byte? childLoopIndex = Retrieve.MotherLoopIndexForChildTag(motherTag, drSubject, dtExtendedResponse);
			if ( !childLoopIndex.HasValue )
				return 0;

			Int32 recordsAdded = 0;
			SurveyTime.SubjectSurvey[] motherSurveys = SurveyTime.RetrieveSubjectSurveys(motherTag, _ds);
			foreach ( Int16 surveyYear in ItemYears.BabyDaddyItems ) {
				SurveySource source = SurveyTime.DetermineSurveySource(surveyYear, motherSurveys);
				//if ( source == SurveySource.NoInterview ) {
				//   AddRow(subjectTag, childLoopIndex.Value, surveyYear, YesNo.ValidSkipOrNoInterviewOrNotInSurvey, YesNo.ValidSkipOrNoInterviewOrNotInSurvey, 
				//      YesNo.ValidSkipOrNoInterviewOrNotInSurvey, null, null, null, YesNo.ValidSkipOrNoInterviewOrNotInSurvey);
				//}
				//else {
				if ( source != SurveySource.NoInterview ) {
					Trace.Assert(source == SurveySource.Gen1, "Only mother responses should be considered here.");
					YesNo biodadInHH = DetermineBiodadInHH(surveyYear, motherTag, childLoopIndex.Value, dtExtendedResponse);
					YesNo biodadAlive = DetermineBiodadAlive(biodadInHH, surveyYear, motherTag, childLoopIndex.Value, dtExtendedResponse);
					YesNo biodadEverLiveInHH = DetermineBiodadEverLiveInHH(surveyYear, motherTag, childLoopIndex.Value, dtExtendedResponse);
					DateTime? biodadLeftHHDate = DetermineBiodadLeftHHDate(surveyYear, motherTag, childLoopIndex.Value, dtExtendedResponse);
					DateTime? biodadDeathDate = DetermineBiodadDeathDate(surveyYear, motherTag, childLoopIndex.Value, dtExtendedResponse);
					Int16? biodadDistanceFromHH = DetermineBiodadDistance(surveyYear, motherTag, childLoopIndex.Value, dtExtendedResponse);
					YesNo biodadAsthma = DetermineBiodadAsthma(surveyYear, motherTag, childLoopIndex.Value, dtExtendedResponse);

					if ( biodadDistanceFromHH.HasValue && biodadDistanceFromHH.Value > 0 && biodadInHH == YesNo.Yes ) {
						biodadInHH = YesNo.No;
						biodadAlive = YesNo.Yes;
					}
					AddRow(subjectTag, childLoopIndex.Value, surveyYear, biodadInHH, biodadAlive, biodadEverLiveInHH, biodadLeftHHDate, biodadDeathDate, biodadDistanceFromHH, biodadAsthma);
					recordsAdded += 1;
				}
			}
			return recordsAdded;
		}
		private static YesNo DetermineBiodadInHH ( Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			const Item item = Item.BabyDaddyInHH;
			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, motherTag, childLoopIndex, dtExtendedResponse);
			if ( !response.HasValue )
				return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
			EnumResponsesGen1.BabyDaddyLiveInHH codedResponse = (EnumResponsesGen1.BabyDaddyLiveInHH)response.Value;
			switch ( codedResponse ) {
				case EnumResponsesGen1.BabyDaddyLiveInHH.InvalidSkip: return YesNo.Refusal;
				case EnumResponsesGen1.BabyDaddyLiveInHH.DoNotKnow: return YesNo.DoNotKnow;
				case EnumResponsesGen1.BabyDaddyLiveInHH.Refusal: return YesNo.Refusal;
				case EnumResponsesGen1.BabyDaddyLiveInHH.No: return YesNo.No;
				case EnumResponsesGen1.BabyDaddyLiveInHH.Yes: return YesNo.Yes;
				default: throw new InvalidOperationException("The response " + codedResponse + " was not recognized.");
			}
		}
		private static YesNo DetermineBiodadAlive ( YesNo biobdadInHH, Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			const Item item = Item.BabyDaddyAlive;
			if ( biobdadInHH == YesNo.Yes ) return YesNo.Yes;

			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, motherTag, childLoopIndex, dtExtendedResponse);
			if ( !response.HasValue )
				return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
			EnumResponsesGen1.BabyDaddyLiving codedResponse = (EnumResponsesGen1.BabyDaddyLiving)response.Value;
			switch ( codedResponse ) {
				case EnumResponsesGen1.BabyDaddyLiving.InvalidSkip: return YesNo.Refusal;
				case EnumResponsesGen1.BabyDaddyLiving.DoNotKnow: return YesNo.DoNotKnow;
				case EnumResponsesGen1.BabyDaddyLiving.Refusal: return YesNo.Refusal;
				case EnumResponsesGen1.BabyDaddyLiving.No: return YesNo.No;
				case EnumResponsesGen1.BabyDaddyLiving.Yes: return YesNo.Yes;
				default: throw new InvalidOperationException("The response " + codedResponse + " was not recognized.");
			}
		}
		private static YesNo DetermineBiodadEverLiveInHH ( Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			if ( ItemYears.BabyDaddyInHHEver.Contains(surveyYear) ) {
				const Item itemSingleCoded = Item.BabyDaddyEverLiveInHH;
				Int32? response = Retrieve.ResponseNullPossible(surveyYear, itemSingleCoded, motherTag, childLoopIndex, dtExtendedResponse);
				if ( !response.HasValue )
					return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
				EnumResponsesGen1.BabyDaddyLeftHH codedResponse = (EnumResponsesGen1.BabyDaddyLeftHH)response.Value;
				switch ( codedResponse ) {
					case EnumResponsesGen1.BabyDaddyLeftHH.InvalidSkip: return YesNo.Refusal;
					case EnumResponsesGen1.BabyDaddyLeftHH.DoNotKnow: return YesNo.DoNotKnow;
					case EnumResponsesGen1.BabyDaddyLeftHH.Refusal: return YesNo.Refusal;
					case EnumResponsesGen1.BabyDaddyLeftHH.Yes: return YesNo.Yes;
					case EnumResponsesGen1.BabyDaddyLeftHH.NeverLivedInHH: return YesNo.No;
					default: throw new InvalidOperationException("The response " + codedResponse + " was not recognized.");
				}
			}
			else if ( ItemYears.BabyDaddyLeftHHMonthOrNeverLivedInHH.Contains(surveyYear) ) {
				const Item itemDoubleCoded = Item.BabyDaddyLeftHHMonthOrNeverInHH;
				Int32? response = Retrieve.ResponseNullPossible(surveyYear, itemDoubleCoded, motherTag, childLoopIndex, dtExtendedResponse);
				if ( !response.HasValue )
					return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
				EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded codedResponse = (EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded)response.Value;
				switch ( codedResponse ) {
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.InvalidSkip:
						return YesNo.Refusal;
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.DoNotKnow:
						return YesNo.DoNotKnow;
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.Refusal:
						return YesNo.Refusal;
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.January:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.February:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.March:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.April:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.May:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.June:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.July:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.August:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.September:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.October:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.November:
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.December:
						return YesNo.Yes;
					case EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.NeverLivedInHH:
						return YesNo.No;
					default:
						throw new InvalidOperationException("The response " + codedResponse + " was not recognized.");
				}
			}
			else {
				return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
			}
		}
		private static DateTime? DetermineBiodadLeftHHDate ( Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			//if ( biodadEverLiveInHH == YesNoGen1.No ) {
			//   return null;
			//}
			//else 
			if ( ItemYears.BabyDaddyLeftHHYearNeverAsked.Contains(surveyYear) ) {//1984-1991
				return null;
			}
			else if ( ItemYears.BabyDaddyLeftHHYearTwoDigit.Contains(surveyYear) ) {//1992 & 1993
				const Item itemYear = Item.BabyDaddyLeftHHYearTwoDigit;
				if ( ItemYears.BabyDaddyLeftHHMonthOrNeverLivedInHH.Contains(surveyYear) ) {//1992
					Trace.Assert(surveyYear == 1992, "Only 1992 surveys should be caught here.");
					const Item itemMonthDoubleCoded = Item.BabyDaddyLeftHHMonthOrNeverInHH;
					Int32? year = Retrieve.ResponseNullPossible(surveyYear, itemYear, motherTag, childLoopIndex, dtExtendedResponse);
					Int32? month = Retrieve.ResponseNullPossible(surveyYear, itemMonthDoubleCoded, motherTag, childLoopIndex, dtExtendedResponse);
					Trace.Assert(year.HasValue == month.HasValue, "If year or month is missing, the other should be missing too.");
					if ( month < 1 )
						month = Constants.DefaultMonthOfYear;
					else if ( month == (Int16)EnumResponsesGen1.BabyDaddyLeftHHMonthDoubleCoded.NeverLivedInHH )
						return null;
					//Trace.Fail("The mom reported that the Baby Daddy never lived in the house (in this double coded item).  The execution should have been diverted in the first 'if' block.");

					if ( year.HasValue && year.Value > 0 )
						return new DateTime(1900 + year.Value, month.Value, Constants.DefaultDayOfMonth);
					else
						return null;
				}
				else {//1993
					Trace.Assert(surveyYear == 1993, "Only 1993 surveys should be caught here.");
					const Item itemMonthSingledCoded = Item.BabyDaddyLeftHHMonth;
					Int32? year = Retrieve.ResponseNullPossible(surveyYear, itemYear, motherTag, childLoopIndex, dtExtendedResponse);
					Int32? month = Retrieve.ResponseNullPossible(surveyYear, itemMonthSingledCoded, motherTag, childLoopIndex, dtExtendedResponse);
					Trace.Assert(year.HasValue == month.HasValue, "If year or month is missing, the other should be missing too.");
					if ( month < 1 )
						month = Constants.DefaultMonthOfYear;

					if ( year.HasValue && year.Value > 0 )
						return new DateTime(1900 + year.Value, month.Value, Constants.DefaultDayOfMonth);
					else
						return null;
				}
			}
			else if ( ItemYears.BabyDaddyLeftHHYearFourDigit.Contains(surveyYear) ) {//1994+
				const Item itemYear = Item.BabyDaddyLeftHHYearFourDigit;
				const Item itemMonthSingledCoded = Item.BabyDaddyLeftHHMonth;
				Int32? year = Retrieve.ResponseNullPossible(surveyYear, itemYear, motherTag, childLoopIndex, dtExtendedResponse);
				Int32? month = Retrieve.ResponseNullPossible(surveyYear, itemMonthSingledCoded, motherTag, childLoopIndex, dtExtendedResponse);
				Trace.Assert(year.HasValue == month.HasValue, "If year or month is missing, the other should be missing too.");
				if ( month < 1 )
					month = Constants.DefaultMonthOfYear;

				if ( year.HasValue && year.Value > 0 )
					return new DateTime(year.Value, month.Value, Constants.DefaultDayOfMonth);
				else
					return null;
			}
			else {
				throw new ArgumentOutOfRangeException("surveyYear", surveyYear, "This item was not prepared for this survey year.");
			}
		}
		private static DateTime? DetermineBiodadDeathDate ( Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			const Item itemMonth = Item.BabyDaddyDeathMonth;
			if ( ItemYears.BabyDaddyDeathNeverAsked.Contains(surveyYear) ) {
				return null;
			}
			else if ( ItemYears.BabyDaddyDeathTwoDigitYear.Contains(surveyYear) ) {
				const Item itemYear2Digit = Item.BabyDaddyDeathYearTwoDigit;
				Int32? year = Retrieve.ResponseNullPossible(surveyYear, itemYear2Digit, motherTag, childLoopIndex, dtExtendedResponse);
				Int32? month = Retrieve.ResponseNullPossible(surveyYear, itemMonth, motherTag, childLoopIndex, dtExtendedResponse);
				Trace.Assert(year.HasValue == month.HasValue, "If year or month is missing, the other should be missing too.");
				if ( !year.HasValue || year < 0 )
					return null;
				else if ( month.HasValue && month < 1 )
					month = Constants.DefaultMonthOfYear;

				return new DateTime(1900 + year.Value, month.Value, Constants.DefaultDayOfMonth);
			}
			else if ( ItemYears.BabyDaddyDeathFourDigitYear.Contains(surveyYear) ) {
				const Item itemYear4Digit = Item.BabyDaddyDeathYearFourDigit;
				Int32? year = Retrieve.ResponseNullPossible(surveyYear, itemYear4Digit, motherTag, childLoopIndex, dtExtendedResponse);
				Int32? month = Retrieve.ResponseNullPossible(surveyYear, itemMonth, motherTag, childLoopIndex, dtExtendedResponse);
				Trace.Assert(year.HasValue == month.HasValue, "If year or month is missing, the other should be missing too.");
				if ( !year.HasValue || year < 0 )
					return null;
				else if ( month.HasValue && month < 1 )
					month = Constants.DefaultMonthOfYear;

				return new DateTime(year.Value, month.Value, Constants.DefaultDayOfMonth);
			}
			else {
				throw new ArgumentOutOfRangeException("surveyYear", surveyYear, "This item was not prepared for this survey year.");
			}
		}
		private static Int16? DetermineBiodadDistance ( Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			const Item item = Item.BabyDaddyDistanceFromMotherFuzzyCeiling;
			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, motherTag, childLoopIndex, dtExtendedResponse);
			if ( response.HasValue )
				return (Int16)response;
			else
				return null;
		}
		private static YesNo DetermineBiodadAsthma ( Int16 surveyYear, Int32 motherTag, byte childLoopIndex, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			if ( ItemYears.BabyDaddyAsthma.Contains(surveyYear) ) {
				const Item item = Item.BabyDaddyHasAsthma;
				Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, motherTag, childLoopIndex, dtExtendedResponse);
				if ( response.HasValue )
					return (YesNo)response;
				else
					return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
			}
			else {
				return YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
			}
		}
		private void AddRow ( Int32 subjectTag, byte childLoopIndex, Int16 surveyYear, YesNo biodadInHH, YesNo biodadAlive, YesNo biodadEverLiveInHH, DateTime? biodadLeftHHDate, DateTime? biodadDeathDate, Int16? biodadDistanceFromHH, YesNo biodadAsthma ) {
			lock ( _ds.tblBabyDaddy ) {
				LinksDataSet.tblBabyDaddyRow drNew = _ds.tblBabyDaddy.NewtblBabyDaddyRow();
				drNew.SubjectTag = subjectTag;
				drNew.ChildLoopIndex = childLoopIndex;
				drNew.SurveyYear = surveyYear;
				drNew.BiodadInHH = (Int16)biodadInHH;
				drNew.BiodadAlive = (Int16)biodadAlive;
				drNew.BiodadEverLiveInHH = (Int16)biodadEverLiveInHH;

				if ( biodadLeftHHDate.HasValue ) drNew.BiodadLeftHHDate = biodadLeftHHDate.Value;
				else drNew.SetBiodadLeftHHDateNull();

				if ( biodadDeathDate.HasValue ) drNew.BiodadDeathDate = biodadDeathDate.Value;
				else drNew.SetBiodadDeathDateNull();

				if ( biodadDistanceFromHH.HasValue ) drNew.BiodadDistanceFromHH = biodadDistanceFromHH.Value;
				else drNew.SetBiodadDistanceFromHHNull();

				drNew.BiodadAsthma = (Int16)biodadAsthma;

				_ds.tblBabyDaddy.AddtblBabyDaddyRow(drNew);
			}
		}
		#endregion
		#region Public Static
		public static LinksDataSet.tblBabyDaddyDataTable RetrieveRows ( Int32 subjectTag, LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblBabyDaddy.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblBabyDaddy.");

			string select = string.Format("{0}={1}", subjectTag, dsLinks.tblBabyDaddy.SubjectTagColumn.ColumnName);
			LinksDataSet.tblBabyDaddyRow[] drs = (LinksDataSet.tblBabyDaddyRow[])dsLinks.tblBabyDaddy.Select(select);
			//Trace.Assert(drs.Length >= 1, "There should be at least one row.");
			LinksDataSet.tblBabyDaddyDataTable dt = new LinksDataSet.tblBabyDaddyDataTable();
			foreach ( LinksDataSet.tblBabyDaddyRow dr in drs ) {
				dt.ImportRow(dr);
			}
			return dt;
		}

		private static LinksDataSet.tblBabyDaddyRow RetrieveRow ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblBabyDaddyDataTable dtInput, Int32 i ) {
			Int16 surveyYear = surveyYears[i];
			string select = string.Format("{0}={1} AND {2}={3}",
				subjectTag, dtInput.SubjectTagColumn.ColumnName,
				surveyYear, dtInput.SurveyYearColumn.ColumnName);
			LinksDataSet.tblBabyDaddyRow[] drs = (LinksDataSet.tblBabyDaddyRow[])dtInput.Select(select);
			if ( drs.Length <= 0 ) {
				return null;
			}
			else {
				Trace.Assert(drs.Length <= 1, "There should be no more than one row.");
				return drs[0];
			}
		}
		public static DateTime?[] RetrieveDeathDates ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblBabyDaddyDataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblBabyDaddy.");

			DateTime?[] dates = new DateTime?[surveyYears.Length];
			for ( Int32 i = 0; i < dates.Length; i++ ) {
				LinksDataSet.tblBabyDaddyRow dr = RetrieveRow(subjectTag, surveyYears, dtInput, i);
				if ( dr == null )
					dates[i] = null;
				else if ( dr.IsBiodadDeathDateNull() )
					dates[i] = null;
				else
					dates[i] = dr.BiodadDeathDate;
			}
			return dates;
		}
		public static Int16?[] RetrieveIsAlive ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblBabyDaddyDataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblBabyDaddy.");

			Int16?[] values = new Int16?[surveyYears.Length];
			for ( Int32 i = 0; i < values.Length; i++ ) {
				LinksDataSet.tblBabyDaddyRow dr = RetrieveRow(subjectTag, surveyYears, dtInput, i);
				if ( dr == null )
					values[i] = null;
				else  if ( (YesNo)dr.BiodadAlive == YesNo.ValidSkipOrNoInterviewOrNotInSurvey )
					values[i] = null;
				else if ( (YesNo)dr.BiodadAlive == YesNo.InvalidSkip )
					values[i] = null;
				else
					values[i] = dr.BiodadAlive;
			}
			return values;
		}
		public static DateTime?[] RetrieveLeftHHDates ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblBabyDaddyDataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblBabyDaddy.");

			DateTime?[] dates = new DateTime?[surveyYears.Length];
			for ( Int32 i = 0; i < dates.Length; i++ ) {
				LinksDataSet.tblBabyDaddyRow dr = RetrieveRow(subjectTag, surveyYears, dtInput, i);
				if ( dr == null )
					dates[i] = null;
				else if ( dr.IsBiodadLeftHHDateNull() )
					dates[i] = null;
				else
					dates[i] = dr.BiodadLeftHHDate;
			}
			return dates;
		}
		public static Int16?[] RetrieveInHH ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblBabyDaddyDataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblBabyDaddy.");

			Int16?[] values = new Int16?[surveyYears.Length];
			for ( Int32 i = 0; i < values.Length; i++ ) {
				LinksDataSet.tblBabyDaddyRow dr = RetrieveRow(subjectTag, surveyYears, dtInput, i);
				if ( dr == null )
					values[i] = null;
				else  if ( (YesNo)dr.BiodadInHH == YesNo.ValidSkipOrNoInterviewOrNotInSurvey )
					values[i] = null;
				else if ( (YesNo)dr.BiodadInHH == YesNo.InvalidSkip )
					values[i] = null;
				else
					values[i] = dr.BiodadInHH;
			}
			return values;
		}
		public static Int16?[] RetrieveDistanceFromHH ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblBabyDaddyDataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblBabyDaddy.");

			Int16?[] distances = new Int16?[surveyYears.Length];
			for ( Int32 i = 0; i < distances.Length; i++ ) {
				LinksDataSet.tblBabyDaddyRow dr = RetrieveRow(subjectTag, surveyYears, dtInput, i);
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
		public static Int16?[] RetrieveAsthma ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblBabyDaddyDataTable dtInput ) {
			if ( dtInput == null ) throw new ArgumentNullException("dtInput");
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblBabyDaddy.");

			Int16?[] values = new Int16?[surveyYears.Length];
			for ( Int32 i = 0; i < values.Length; i++ ) {
				LinksDataSet.tblBabyDaddyRow dr = RetrieveRow(subjectTag, surveyYears, dtInput, i);
				if ( dr == null )
					values[i] = null;
				else  if ( (YesNo)dr.BiodadAsthma == YesNo.ValidSkipOrNoInterviewOrNotInSurvey )
					values[i] = null;
				else if ( (YesNo)dr.BiodadAsthma == YesNo.InvalidSkip )
					values[i] = null;
				else
					values[i] = dr.BiodadAsthma;
			}
			return values;
		}

		#endregion
	}
}
