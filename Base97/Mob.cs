using System;
using System.Diagnostics;

namespace Nls.Base97 {
    public static class Mob {
        internal static DateTime? Retrieve( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dt ) {
            if( dt == null ) throw new ArgumentNullException("dt");
            if( drSubject == null ) throw new ArgumentNullException("drSubject");

            const Int32 maxRows = 1;
            //if ( Overrides.MissingMobInvalidSkip.Contains(drSubject.SubjectID) ) return null;

            Int32? monthGen2 = Nls.Base97.Retrieve.ResponseNullPossible(Item.DateOfBirthMonth, drSubject.SubjectTag, maxRows, dt);
            Int32? yearGen2 = Nls.Base97.Retrieve.ResponseNullPossible(Item.DateOfBirthYear, drSubject.SubjectTag, maxRows, dt);//There's only three who don't have both year & month, so moving it won't increase speed.
            //if ( !yearGen2.HasValue ) {
            //    Trace.Assert(Overrides.MissingMobInvalidSkip.Contains(drSubject.SubjectID), "Only pre-identified Gen2 subjects can have an invalid skip Mob value.");
            //    return null;
            //}
            //else if ( monthGen2 < 0 ) {
            //    Trace.Assert(Overrides.MissingMobRefusedMonth.Contains(drSubject.SubjectID), "Only pre-identified Gen2 subjects can have a refused Mob value.");
            //    monthGen2 = Constants.DefaultMonthOfYear;
            //}
            return CalculateMob(monthGen2.Value, yearGen2.Value);

        }
        internal static DateTime? Retrieve( Int32 subjectTag, LinksDataSet.tblSubjectDetailsDataTable dt ) {
            LinksDataSet.tblSubjectDetailsRow dr = dt.FindBySubjectTag(subjectTag);
            if( dr.IsMobNull() )
                return null;
            else
                return dr.Mob;
        }

        private static DateTime? CalculateMob( Int32 reportedBirthMonth, Int32 reportedYob ) {
            if( reportedBirthMonth < 1 ) throw new ArgumentOutOfRangeException("reportedBirthMonth", reportedBirthMonth, "The reportedBirthMonth cannot be before 1 (January).");
            else if( reportedBirthMonth > 12 ) throw new ArgumentOutOfRangeException("reportedBirthMonth", reportedBirthMonth, "The reportedBirthMonth cannot be after 12 (December).");

            if( reportedYob < Constants.BirthYearMin ) throw new ArgumentOutOfRangeException("reportedYob", reportedYob, "The reportedYob cannot be before 1980, according to the NLS cookbook.");
            else if( reportedYob > Constants.BirthYearMax ) throw new ArgumentOutOfRangeException("reportedYob", reportedYob, "The reportedYob cannot be after 1984, according to the NLS cookbook.");

            return new DateTime(reportedYob, reportedBirthMonth, Constants.DefaultDayOfMonth);
        }
    }
}