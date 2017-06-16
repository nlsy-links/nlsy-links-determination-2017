using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nls.BaseAssembly {
	public class SurveyTime {
		public struct SubjectSurvey {
			internal readonly Int16 SurveyYear;
			internal readonly SurveySource SurveySource;
			internal SubjectSurvey ( Int16 surveyYear, SurveySource source ) {
				SurveyYear = surveyYear;
				SurveySource = source;
			}
		}
		#region Fields
		private readonly LinksDataSet _ds;
		private readonly Item[] _items = { Item.AgeAtInterviewDateMonths, Item.AgeAtInterviewDateYears, 
													Item.DateOfBirthMonth, Item.DateOfBirthYearGen1, Item.DateOfBirthYearGen2,
													Item.InterviewDateDay, Item.InterviewDateMonth, Item.InterviewDateYear};
		private readonly string _itemIDsString = "";
        private static IList<OverridesGen1.SubjectYear> _overrides = OverridesGen1.InverviewDateInvalidSkip;
        private const float ageBiasCorrectionInYears = 0.5f;
        private const float ageBiasCorrectionInMonths = 0.5f;
		#endregion
		#region Constructor
		public SurveyTime ( LinksDataSet ds ) {
			if ( ds == null ) throw new ArgumentNullException("ds");
			if ( ds.tblResponse.Count <= 0 ) throw new InvalidOperationException("tblResponse must NOT be empty.");
			if ( ds.tblSurveyTime.Count != 0 ) throw new InvalidOperationException("tblSurveyTime must be empty before creating rows for it.");
			_ds = ds;

			_itemIDsString = CommonCalculations.ConvertItemsToString(_items);
		}
		#endregion
		#region Public Methods
		public string Go ( ) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Retrieve.VerifyResponsesExistForItem(_items, _ds);
			Int32 recordsAddedTotal = 0;
			_ds.tblSurveyTime.BeginLoadData();
			ParallelOptions options = new ParallelOptions();
			options.MaxDegreeOfParallelism = -1;
			Parallel.ForEach(_ds.tblSubject, options, ( drSubject ) => {
				//foreach ( LinksDataSet.tblSubjectRow drSubject in _ds.tblSubject ) {//Elapsed time: 00:01:07.4203198
				//if ( recordsAddedTotal < 100 ) {
				Int32 recordsAddedForLoop = ProcessSubject(drSubject);
				Interlocked.Add(ref recordsAddedTotal, recordsAddedForLoop);
				//}
			});
			_ds.tblSurveyTime.EndLoadData();
			sw.Stop();
			Trace.Assert(recordsAddedTotal == Constants.SurveyTimeCount, "The number of individual surveys (for both Gen1 & Gen2 combined) should be correct.");
			return string.Format("{0:N0} SurveyTime records were created.\nElapsed time: {1}", recordsAddedTotal, sw.Elapsed.ToString());
		}
		#endregion
		#region Private Methods
		private Int32 ProcessSubject ( LinksDataSet.tblSubjectRow drSubject ) {
			Int32 subjectTag = drSubject.SubjectTag;
			Int32 recordsProcessed = 0;
			Int16[] surveyYears = ItemYears.Gen1AndGen2;// SubjectWaves(subjectTag);
			LinksDataSet.tblResponseDataTable dtResponse = Retrieve.SubjectsRelevantResponseRows(drSubject.SubjectTag, _itemIDsString, 1, _ds.tblResponse);
			DateTime? mob = Mob.Retrieve(drSubject, dtResponse);

			foreach ( Int16 surveyYear in surveyYears ) {
				DateTime? surveyDate = null;//CalculateSurveyDate(;
				float? ageSelfReport = null;
				float? ageCalculated = null;

				SurveySource source = DetermineSurveySource(surveyYear, drSubject, dtResponse);
				OverridesGen1.SubjectYear subjectYear = new OverridesGen1.SubjectYear(drSubject.SubjectID, surveyYear);
				bool invalidSkipOverride = false;
				if ( (drSubject.Generation == (byte)Generation.Gen1) && (_overrides.Contains(subjectYear)) ) {
					source = SurveySource.Gen1;
					invalidSkipOverride = true;
				}

				if ( source != SurveySource.NoInterview ) {
					surveyDate = DetermineSurveyDate(source, surveyYear, drSubject, dtResponse);
					ageSelfReport = DetermineAgeSelfReport(source, subjectTag, dtResponse, surveyYear);
					if ( !invalidSkipOverride )
						ageCalculated = CalculateAge(surveyDate, mob);
				}
				AddRow(subjectTag, source, surveyYear, surveyDate, ageSelfReport, ageCalculated);
				recordsProcessed += 1;
			}
			return recordsProcessed;
		}
		private SurveySource DetermineSurveySource ( Int16 surveyYear, LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtResponseForSubject ) {
			string select = string.Format("{0}={1} AND {2}={3} AND {4}>0",
				surveyYear, _ds.tblResponse.SurveyYearColumn.ColumnName,
				(byte)Item.InterviewDateMonth, _ds.tblResponse.ItemColumn.ColumnName,
				_ds.tblResponse.ValueColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsResponse = (LinksDataSet.tblResponseRow[])dtResponseForSubject.Select(select);
			Trace.Assert(drsResponse.Length <= 1, string.Format("There should be at most one row with a positive value for InterviewDateMonth (SubjectTag:{0}, SurveyYear:{1}).", drSubject.SubjectTag, surveyYear));
			if ( drsResponse.Length == 0 ) {
				return SurveySource.NoInterview;
			}
			else {
				SurveySource source = (SurveySource)drsResponse[0].SurveySource;
				switch ( source ) {
					case SurveySource.Gen1: Trace.Assert(drSubject.Generation == (byte)Generation.Gen1, "The subject should be Gen1."); break;
					case SurveySource.Gen2C: Trace.Assert(drSubject.Generation == (byte)Generation.Gen2, "The subject should be Gen2."); break;
					case SurveySource.Gen2YA: Trace.Assert(drSubject.Generation == (byte)Generation.Gen2, "The subject should be Gen2."); break;
					default: throw new InvalidOperationException("The determined SurveySource was not recognized.");//The NotInterviewed shouldn't be possible for this switch.
				}
				return source;
			}
		}
		private static DateTime? DetermineSurveyDate ( SurveySource source, Int16 surveyYear, LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtResponseForSubject ) {
			Int32 maxRecords = 1;
			Int32? monthReported = Retrieve.ResponseNullPossible(surveyYear, Item.InterviewDateMonth, source, drSubject.SubjectTag, maxRecords, dtResponseForSubject);
			if ( !monthReported.HasValue || monthReported < 0 ) return null;

			Int32? dayReported = Retrieve.ResponseNullPossible(surveyYear, Item.InterviewDateDay, source, drSubject.SubjectTag, maxRecords, dtResponseForSubject);
			if ( !dayReported.HasValue || dayReported < 0 ) dayReported = Constants.DefaultDayOfMonth;
			//Trace.Fail("There shouldn't be any interview date that missing a day, but not a month.");

			Int32? yearReported = Retrieve.ResponseNullPossible(surveyYear, Item.InterviewDateYear, source, drSubject.SubjectTag, maxRecords, dtResponseForSubject);
			if ( yearReported < 0 || !yearReported.HasValue ) yearReported = surveyYear;
			else if ( 0 < yearReported && yearReported < 1900 ) yearReported = 1900 + yearReported;//The 1993 Gen1 Survey reports it like '93', instead of '1993'.

			dayReported = Math.Min(28, dayReported.Value);
			DateTime interviewDate = new DateTime(yearReported.Value, monthReported.Value, dayReported.Value);
			return interviewDate;
		}
		private static float? DetermineAgeSelfReport ( SurveySource source, Int32 subjectTag, LinksDataSet.tblResponseDataTable dtResponse, Int16 surveyYear ) {
			float? ageSelfReportYears;
			switch ( source ) {
				case SurveySource.Gen2C:
					ageSelfReportYears = AgeSelfReportMonths(subjectTag, surveyYear, dtResponse);
					break;
				case SurveySource.Gen1:
				case SurveySource.Gen2YA:
					ageSelfReportYears = AgeSelfReportYears(subjectTag, surveyYear, dtResponse);
					break;
				default: throw new ArgumentOutOfRangeException("source", source, "The SurveySource value was not recognized.");
			}
			if ( ageSelfReportYears.HasValue && ageSelfReportYears.Value < 0 ) throw new InvalidOperationException(string.Format("The self-reported age for SubjectTag {0} is {1}.", subjectTag, ageSelfReportYears.Value));
			return ageSelfReportYears;
		}
		private static float? CalculateAge ( DateTime? interviewDate, DateTime? mob ) {
			if ( !mob.HasValue )
				return null;

			float ageInYears = (float)(interviewDate.Value.Subtract(mob.Value).TotalDays / Constants.DaysPerYear);
			const double roundingError = 17 / Constants.DaysPerYear;
			Trace.Assert(ageInYears > -roundingError, "Age should be positive (non-null) value, at this point of the execution.");
			ageInYears = Math.Max(0, ageInYears);
			return ageInYears;
		}
		private static float? AgeSelfReportYears ( Int32 subjectTag, Int16 surveyYear, LinksDataSet.tblResponseDataTable dtResponseForSubject ) {
			string select = string.Format("{0}={1} AND {2}={3} AND {4}={5} AND {6}>0",
				subjectTag, dtResponseForSubject.SubjectTagColumn.ColumnName,
				surveyYear, dtResponseForSubject.SurveyYearColumn.ColumnName,
				(byte)Item.AgeAtInterviewDateYears, dtResponseForSubject.ItemColumn.ColumnName,
				dtResponseForSubject.ValueColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsResponse = (LinksDataSet.tblResponseRow[])dtResponseForSubject.Select(select);
			Trace.Assert(drsResponse.Length <= 1, "No more than one row should be returned.");

			if ( drsResponse.Length == 0 ) {
				return null;
			}
			else {
				float ageInYears = (float)drsResponse[0].Value + ageBiasCorrectionInYears;
				Trace.Assert(ageInYears > 0, "Age should be positive (non-null) value, at this point of the execution.");
				return ageInYears;
			}
		}
		private static float? AgeSelfReportMonths ( Int32 subjectTag, Int16 surveyYear, LinksDataSet.tblResponseDataTable dtResponseForSubject ) {
			string select = string.Format("{0}={1} AND {2}={3} AND {4}={5} AND {6}>0",
				subjectTag, dtResponseForSubject.SubjectTagColumn.ColumnName,
				surveyYear, dtResponseForSubject.SurveyYearColumn.ColumnName,
				(byte)Item.AgeAtInterviewDateMonths, dtResponseForSubject.ItemColumn.ColumnName,
				dtResponseForSubject.ValueColumn.ColumnName);
			LinksDataSet.tblResponseRow[] drsResponse = (LinksDataSet.tblResponseRow[])dtResponseForSubject.Select(select);
			Trace.Assert(drsResponse.Length <= 1, "No more than one row should be returned.");

			const double monthsPerYear = 12.0;
			if ( drsResponse.Length == 0 ) {
				return null;
			}
			else {
				Trace.Assert(drsResponse[0].Generation == (byte)Generation.Gen2, "Only Gen2 subjects should be answering this item (specifically the Cs -not the YAs).");
				float ageInYears = (float)((drsResponse[0].Value + ageBiasCorrectionInMonths) / monthsPerYear);
				Trace.Assert(ageInYears > 0, "Age should be positive (non-null) value, at this point of the execution.");
				return ageInYears;
			}
		}
		private void AddRow ( Int32 subjectTag, SurveySource surveySource, Int16 surveyYear, DateTime? surveyDate, float? ageSelfReport, float? calculatedAge ) {
			lock ( _ds.tblSurveyTime ) {
				LinksDataSet.tblSurveyTimeRow drNew = _ds.tblSurveyTime.NewtblSurveyTimeRow();
				drNew.SubjectTag = subjectTag;
				drNew.SurveySource = (byte)surveySource;
				drNew.SurveyYear = surveyYear;

				if ( surveyDate.HasValue ) drNew.SurveyDate = surveyDate.Value;
				else drNew.SetSurveyDateNull();

				if ( ageSelfReport.HasValue ) drNew.AgeSelfReportYears = ageSelfReport.Value;
				else drNew.SetAgeSelfReportYearsNull();

				if ( calculatedAge.HasValue ) drNew.AgeCalculateYears = calculatedAge.Value;
				else drNew.SetAgeCalculateYearsNull();

				_ds.tblSurveyTime.AddtblSurveyTimeRow(drNew);
			}
		}
		#endregion
		#region Public Static
		//public static LinksDataSet.tblSurveyTimeDataTable ExtendedFamilySurveyTime ( Int16 extendedID, LinksDataSet dsLinks ) {
		//}
		public static SubjectSurvey[] RetrieveSubjectSurveys ( Int32 subjectTag, LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSurveyTime.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblSurveyTime.");

			string select = string.Format("{0}={1} AND {2}>0",
				subjectTag, dsLinks.tblSurveyTime.SubjectTagColumn.ColumnName,
				dsLinks.tblSurveyTime.SurveySourceColumn.ColumnName);
			LinksDataSet.tblSurveyTimeRow[] drs = (LinksDataSet.tblSurveyTimeRow[])dsLinks.tblSurveyTime.Select(select);
			//Trace.Assert(drs.Length > 0, "There should be at least one row returned.");
			SubjectSurvey[] ss = new SubjectSurvey[drs.Length];
			for ( Int32 i = 0; i < drs.Length; i++ ) {
				ss[i] = new SubjectSurvey(drs[i].SurveyYear, (SurveySource)drs[i].SurveySource);
			}
			return ss;
		}
		public static DateTime? RetrieveSubjectSurveyDate ( Int32 subjectTag, Int16 surveyYear, LinksDataSet dsLinks ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblSurveyTime.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblSurveyTime.");

			string select = string.Format("{0}={1} AND {2}={3}", // AND {4}>0",
				subjectTag, dsLinks.tblSurveyTime.SubjectTagColumn.ColumnName,
				surveyYear, dsLinks.tblSurveyTime.SurveyYearColumn.ColumnName); //dsLinks.tblSurveyTime.SurveySourceColumn.ColumnName
			LinksDataSet.tblSurveyTimeRow[] drs = (LinksDataSet.tblSurveyTimeRow[])dsLinks.tblSurveyTime.Select(select);
			Trace.Assert(drs.Length == 1, "There should be exactly one row returned.");
			
			if( drs[0].IsSurveyDateNull() ) {
				Trace.Assert(drs[0].SurveySource == 0, "If the Survey Date is null, the Survey Source should be zero.");
				return null;
			}
			else {
				Trace.Assert(drs[0].SurveySource > 0, "If the Survey Date is not null, the Survey Source should be nonzero.");
				return drs[0].SurveyDate;
			}
		}
		public static SurveySource DetermineSurveySource ( Int16 surveyYear, SubjectSurvey[] subjectSurveys ) {
			IEnumerable<SurveySource> sources = from ss in subjectSurveys
															where ss.SurveyYear == surveyYear
															select ss.SurveySource;
			if ( sources.Count() <= 0 )
				return SurveySource.NoInterview;
			else
				return sources.Single();
		}
		#endregion
	}
}
//private float? ResolveCAndYA ( float? valueC, float? valueYA ) {
//   if ( valueC.HasValue && valueYA.HasValue )
//      throw new ArgumentException("The subject should not have nonzero values for both valueC and valueYA.");
//   else if ( valueC.HasValue )
//      return valueC;
//   else if ( valueYA.HasValue )
//      return valueYA;
//   else
//      return null;
//}
