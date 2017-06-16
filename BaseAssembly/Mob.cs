using System;
using System.Diagnostics;

namespace Nls.BaseAssembly {
	public static class Mob {
		internal static DateTime? Retrieve ( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dt ) {
			if ( dt == null ) throw new ArgumentNullException("dt");
			if ( drSubject == null ) throw new ArgumentNullException("drSubject");

			const Int32 maxRows = 1;
			switch ( (Generation)drSubject.Generation ) {
				case Generation.Gen1:
					const SurveySource sourceGen1 = SurveySource.Gen1;
					Int32? monthGen1 = Nls.BaseAssembly.Retrieve.ResponseNullPossible(Constants.Gen1MobSurveyYearPreferred, Item.DateOfBirthMonth, sourceGen1, drSubject.SubjectTag, maxRows, dt);
					Int32 yearGen1 = Int32.MinValue;
					if ( monthGen1.HasValue && monthGen1.Value > 0 ) {
						yearGen1 = Nls.BaseAssembly.Retrieve.Response(Constants.Gen1MobSurveyYearPreferred, Item.DateOfBirthYearGen1, drSubject.SubjectTag, maxRows, dt);
						Trace.Assert(yearGen1 > 0, "If month isn't missing, neither should year.");
					}
					else {
						monthGen1 = Nls.BaseAssembly.Retrieve.Response(Constants.Gen1MobSurveyYearBackup, Item.DateOfBirthMonth, drSubject.SubjectTag, maxRows, dt);
						yearGen1 = Nls.BaseAssembly.Retrieve.Response(Constants.Gen1MobSurveyYearBackup, Item.DateOfBirthYearGen1, drSubject.SubjectTag, maxRows, dt);
					}
					return CalculateMobForGen1(monthGen1.Value, yearGen1);
				case Generation.Gen2:
					if ( OverridesGen2.MissingMobInvalidSkip.Contains(drSubject.SubjectID) ) return null;

					const SurveySource sourceGen2 = SurveySource.Gen2C;
					Int32? monthGen2 = Nls.BaseAssembly.Retrieve.ResponseNullPossible(Item.DateOfBirthMonth, sourceGen2, drSubject.SubjectTag, maxRows, dt);
					Int32? yearGen2 = Nls.BaseAssembly.Retrieve.ResponseNullPossible(Item.DateOfBirthYearGen2, sourceGen2, drSubject.SubjectTag, maxRows, dt);//There's only three who don't have both year & month, so moving it won't increase speed.
					if ( !yearGen2.HasValue ) {
						Trace.Assert(OverridesGen2.MissingMobInvalidSkip.Contains(drSubject.SubjectID), "Only pre-identified Gen2 subjects can have an invalid skip Mob value.");
						return null;
					}
					else if ( monthGen2 < 0 ) {
						Trace.Assert(OverridesGen2.MissingMobRefusedMonth.Contains(drSubject.SubjectID), "Only pre-identified Gen2 subjects can have a refused Mob value.");
						monthGen2 = Constants.DefaultMonthOfYear;
					}
					return CalculateMobForGen2(monthGen2.Value, yearGen2.Value);
				default:
					Trace.Fail("The Generation value is not recognized.");
					return null;
			}
		}
		internal static DateTime? Retrieve ( Int32 subjectTag, LinksDataSet.tblSubjectDetailsDataTable dt ) {
			LinksDataSet.tblSubjectDetailsRow dr = dt.FindBySubjectTag(subjectTag);
			if ( dr.IsMobNull() )
				return null;
			else
				return dr.Mob;
		}
		private static DateTime? CalculateMobForGen1 ( Int32 reportedBirthMonth, Int32 reportedYob ) {
			if ( reportedBirthMonth == (Int32)EnumResponsesGen1.TypicalItem.Refusal ) return null;//Value of -1
			else if ( reportedBirthMonth == (Int32)EnumResponsesGen1.TypicalItem.InvalidSkip ) return null; //Value of -3
			else if ( reportedBirthMonth < 1 ) throw new ArgumentOutOfRangeException("reportedBirthMonth", reportedBirthMonth, "The reportedBirthMonth cannot be before 1 (January).");
			else if ( reportedBirthMonth > 12 ) throw new ArgumentOutOfRangeException("reportedBirthMonth", reportedBirthMonth, "The reportedBirthMonth cannot be after 12 (December).");

			if ( reportedYob == (Int32)EnumResponsesGen1.TypicalItem.Refusal ) return null;//Value of -1
			else if ( reportedYob == (Int32)EnumResponsesGen1.TypicalItem.InvalidSkip ) return null; //Value of -3
			else if ( reportedYob < Constants.Gen1BirthYearMin ) throw new ArgumentOutOfRangeException("reportedYob", reportedYob, "The reportedYob cannot be before (19)55, according to the NLS cookbook (asked in 1981).");
			else if ( reportedYob > Constants.Gen1BirthYearMax ) throw new ArgumentOutOfRangeException("reportedYob", reportedYob, "The reportedYob cannot be after (19)65, according to the NLS cookbook (asked in 1981).");

			return new DateTime(1900 + reportedYob, reportedBirthMonth, Constants.DefaultDayOfMonth);
		}
		private static DateTime? CalculateMobForGen2 ( Int32 reportedBirthMonth, Int32 reportedYob ) {
			if ( reportedBirthMonth == (Int32)EnumResponsesGen1.TypicalItem.Refusal ) return null;//Value of -1
			else if ( reportedBirthMonth == (Int32)EnumResponsesGen1.TypicalItem.InvalidSkip ) return null;//Value of -3
			else if ( reportedBirthMonth < 1 ) throw new ArgumentOutOfRangeException("reportedBirthMonth", reportedBirthMonth, "The reportedBirthMonth cannot be before 1 (January).");
			else if ( reportedBirthMonth > 12 ) throw new ArgumentOutOfRangeException("reportedBirthMonth", reportedBirthMonth, "The reportedBirthMonth cannot be after 12 (December).");

			if ( reportedYob == (Int32)EnumResponsesGen1.TypicalItem.Refusal ) return null; //Value of -1
			else if ( reportedYob == (Int32)EnumResponsesGen1.TypicalItem.InvalidSkip ) return null; //Value of -3
			else if ( reportedYob < Constants.Gen2BirthYearMin ) throw new ArgumentOutOfRangeException("reportedYob", reportedYob, "The reportedYob cannot be before 1970, according to the NLS cookbook.");
			else if ( reportedYob > Constants.Gen2BirthYearMax ) throw new ArgumentOutOfRangeException("reportedYob", reportedYob, "The reportedYob cannot be after the data was collected.");

			return new DateTime(reportedYob, reportedBirthMonth, Constants.DefaultDayOfMonth);
		}
	}
}
