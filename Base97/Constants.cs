using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nls.Base97 {
    public static class Constants {
        #region  Misc
        public const Int32 TempRelatednessCoefficient = -1;
        public const Int32 DefaultDayOfMonth = 15;
        public const Int32 DefaultMonthOfYear = 7;//There's a few of these in Gen2.
        public const Int32 MaxDaysBetweenTwinBirths = 50; //We only have their month of birthday, so we have to be pretty forgiving here.
        public const Int32 MonthsPerYear = 12;
        public const double DaysPerYear = 365.25; //Account for leap year.
        //public const Int32 SurveyYearEternal = 2008;//For things that aren' associated with a survey year, like birth date or gender.
        public const string SubjectIDColumn = "R0000100";            //In tblRoster
        public const string ExtendedFamilyIDColumn = "R1193000";	 //In tblRoster	
        public const string GenderColumn = "R0536300";               //In tblRoster

        public static float[] Gen1RsToExcludeFromR { get { return new float[] { 0f, .0625f, .125f, .375f, .75f }; } }
        #endregion
        #region  IDs
        public const Int32 IDMin = 1;//Sync with tblLUItem
        public const Int32 IDMax = 9022;

        public const Int32 ExtendedIDMin = 1;      // From R11930.00    [SIDCODE]   
        public const Int32 ExtendedIDMax = 7477;   // From R11930.00    [SIDCODE]   
        public const Int32 SubjectIDDigitsMax = 4; // Accommodate '9022'
        #endregion
        #region  Expected Counts
        public const Int32 Gen1Count = 8984;

        public const Int32 HousematesPathCount = 5038;//Single entered;

        #endregion
        #region Counts
        //public const Int32 Gen1IDSiblingCountMin = 1;
        public const Int32 SurveyTimeCount = 152728; //8,984 kids times 17 years
        #endregion
        #region Survey Details
        //public const Int16 Gen1MobSurveyYearPreferred = 1981;
        //public const Int16 Gen1MobSurveyYearBackup = 1979;//See http://www.nlsinfo.org/nlsy79/docs/79html/79text/age.htm

        public const Int32 BirthYearMin = 1980; //R05364.02
        public const Int32 BirthYearMax = 1984;

        public static readonly Int32[] PassoverResponses = { -4, -5 };
        //public static readonly Int32[] PassoverResponsesNoNegatives = { -1, -2, -3, -4, -5 };
        #endregion

        #region  DVs
        #endregion
    }
}