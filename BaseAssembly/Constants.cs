using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nls.BaseAssembly {
	public static class Constants {
		#region  Misc
		public const Int32 TempRelatednessCoefficient = -1;
		public const Int32 DefaultDayOfMonth = 15;
		public const Int32 DefaultMonthOfYear = 7;//There's a few of these in Gen2.
		public const Int32 MaxDaysBetweenTwinBirths = 50; //We only have their month of birthday, so we have to be pretty forgiving here.
		public const Int32 MonthsPerYear = 12;
		public const double DaysPerYear = 365.25; //Account for leap year.
		//public const Int32 SurveyYearEternal = 2008;//For things that aren' associated with a survey year, like birth date or gender.
		public const string Gen1SubjectIDColumn = "R0000100"; //In tblGen1Links
		public const string Gen1ExtendedFamilyIDColumn = "R0000149";	 //In tblGen1Links	
		public const string Gen1GenderColumn = "R0214800"; //In tblGen1Links
		public const string Gen2SubjectIDColumn = "C0000100";
		public const string Gen2GenderColumn = "C0005400";
		//public static float[] Gen1RsToExcludeFromR { get { return new float[] { 0f, .375f, .75f }; } }
        public static float[] Gen1RsToExcludeFromR { get { return new float[] { 0f, .0625f, .125f, .375f, .75f }; } }
		#endregion
		#region  IDs
		public const Int32 Gen1IDMin = 1;//Sync with tblLUItem
		public const Int32 Gen1IDMax = 12686;

		public const Int32 ExtendedIDMin = Gen1IDMin;
		public const Int32 ExtendedIDMax = Gen1IDMax;
		public const Int32 Gen2IDMin = 201;
		public const Int32 Gen2IDMax = 1267501; //Sync with tblLUItem //1267302 for Year2004 females   //1251601//1268601 is the biggest for the whole NLSY
		public const Int32 SubjectIDDigitsMax = 7;    //For 1251601
		#endregion
		#region  Expected Counts
		public const Int32 Gen1Count = Gen1IDMax;
		public const Int32 Gen2Count = 11504;//11495 in 2008;
		//public const Int32 Gen2SiblingPathCount = 22150;//Double entered;

		public const Int32 Gen1HousematesPathCount = 5302;//Single entered;
        public const Int32 Gen2SiblingsPathCount = 11512; //11088 in 2010; //11075 in 2008;//Single entered;
		public const Int32 Gen2CousinsPathCount = 4995;//4972 in 2008;//Single entered;
		public const Int32 ParentChildPathCount = 11504;//Single entered;
		public const Int32 AuntNiecePathCount = 9884;//9870 in 2008;//Single entered;
		#endregion
		#region Counts
		public const Int32 Gen1IDSiblingCountMin = 1;
		public const Int32 Gen1IDSiblingCountMax = 4;//ExtendedID 9454

		public const Int32 Gen2IDSiblingCountMin = 1;
		//public const Int32 Gen2IDSisterCountMax = 7;	//ExtendedID 8693
		public const Int32 Gen2IDSiblingCountMax = 10;    //ExtendedID 6371 has 11 kids, so Gen2 Subject637111 has 10 siblings.

		//WRONG public const Int32 CousinCountMax = 16//ExtendedID 8693
        public const Int32 SurveyTimeCount = 580752; //580560 in 2010; //429718 in 2008;
		#endregion
		#region Parents Of Gen1 
		public const Int16 Gen1BioparentBirthYearReportedMin = 1900;
		public const Int16 Gen1BioparentBirthYearReportedMax = 1966;
		public const Int16 Gen1BioparentBirthYearEsimatedMin = 1891;
		public const Int16 Gen1BioparentBirthYearEsimatedMax = 1966;
		#endregion
		#region Survey Details
		public const Int16 Gen1MobSurveyYearPreferred = 1981;
		public const Int16 Gen1MobSurveyYearBackup = 1979;//See http://www.nlsinfo.org/nlsy79/docs/79html/79text/age.htm

		public const Int32 Gen1BirthYearMin = 55;//R0410300 (in 1981) is preferred over R000050 (in 1979).  See http://www.nlsinfo.org/nlsy79/docs/79html/79text/age.htm
		public const Int32 Gen1BirthYearMax = 65;
		public const Int32 Gen2BirthYearMin = 1970;//C000570
		public const Int32 Gen2BirthYearMax = 2014;

		public static readonly Int32[] Gen1PassoverResponses = {  -4, -5 };
        public static readonly Int32[] Gen1PassoverResponsesNoNegatives = { -1, -2, -3, -4, -5 };
        public static readonly Int32[] Gen2PassoverResponses = { -7 };
        public static readonly Int32[] Gen2PassoverResponseNoNegatives = { -1, -2, -3, -4, -5 , -6, - 7 };
		#endregion
		#region SurveyYears
		//See the class Nls.BaseAssembly.SurveyYears
		#endregion
		#region  DVs
		public const byte Gen2AgeMissingHasChildrenThreshold = 16;//If their last completed survey was when they were 16 or younger, assume they didn't have any biological kids yet.
		public const Int32 DVDateDifferenceThreshold = 6 * 31;//If the dates are within 6 months of each other, consider they're the same.
		//public const byte Gen1InterviewAgeMin = 14;
		//public const byte Gen1InterviewAgeMax = 48;//For 2004 survey (but cookbook says 46).

		//public const Int16 Gen2ChildrenInterviewAgeMinInMonths = 0;
		//public const Int16 Gen2ChildrenInterviewAgeMaxInMonths = 259; //For 1992 survey.

		//public const byte Gen2YoungAdultInterviewAgeMin = 13;//1996 survey.
		//public const byte Gen2YoungAdultInterviewAgeMax = 35;//For 2006 survey.


		//public const Int16 Gen1MenarcheMin = 9;   //Mother #3315
		//public const Int16 Gen1MenarcheMax = 19;//Mother #9346

		//public const Int16 Gen1IntercourseMin = 12;//10 different subjects
		//public const Int16 Gen1IntercourseMax = 27;//3 different subjects

		//public const Int16 Gen2MenarcheMin = 7;   //10 different subjects have 8, but Rodgers wants 7.
		//public const Int16 Gen2MenarcheMax = 18; //Mother #2919 (notice this is a mom)

		//public const Int16 Gen2IntercourseMin = 7;    //#325904
		//public const Int16 Gen2IntercourseMax = 28; //Only one subject is this late

		//public const Int32 Gen2FirstDateNeverHadAskedInSomeYears = 95; //They were retarded.
		//public const Int16 Gen2FirstDateMin = 0;//829803 in 96
		//public const Int16 Gen2FirstDateMax = 26;

		//public const Int16 Gen2FirstMarriageMin = 14;
		//public const Int16 Gen2FirstMarriageMax = 32;

		//public const Int16 Gen2PregnancyMin = 10;
		//public const Int16 Gen2PregnancyMax = 30;

		//public const Int16 Gen2FirstBirthMin = 10;
		//public const Int16 Gen2FirstBirthMax = 30;
		#endregion
	}
}
