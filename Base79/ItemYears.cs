using System;

namespace Nls.Base79 {
    public static class ItemYears {
        public const Int16 Gen1Roster = 1979;

        public readonly static Int16[] Gen1AndGen2 = { 1979, 1980, 1981, 1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };

        public readonly static Int16[] Gen1ShareBioparent = { 2006, 2010 };

        public readonly static Int16[] Gen1Height = { 1982 };
        public readonly static Int16[] Gen1Weight = { 1982 };

        public const Int16 Gen1BioparentInHH = 1988;

        public const Int16 Gen1BioparentHighestGrade = 1979;
        public const Int16 Gen1BioparentBirthCountry = 1979; //And also the Gen1's biodad's dad; It's in the sanitized geocode datset
        public const Int16 Gen1BioparentBirthState = 1979; //And also the Gen1's biodad's dad; It's in the sanitized geocode datset
        public const Int16 Gen1BioparentAlive = 0; //It's an XRND.
        public const Int16 Gen1BioparentDeathAge = 0; //It's an XRND.
        public const Int16 Gen1BioparentDeathCause = 0; //It's an XRND.
        public readonly static Int16[] Gen1BioparentBirthYear = { 1987, 1988 };
        //public readonly static Int16[] Gen1BioparentBirthMonth = ItemYears.Gen1BioparentBirthYear;
        public readonly static Int16[] Gen1BioparentAge = ItemYears.Gen1BioparentBirthYear;

        public readonly static Int16[] Gen2ShareBiodad = { 2006, 2008, 2010, 2012, 2014 };

        //public readonly static Int16[] FatherDeadGen2 = { 2006, 2008, 2010, 2012, 2014 }; // Not incorporated yet. THrees items: (a) alive/dead, (b) month of death, (c) year of death. May not be biodad?
        public readonly static Int16[] FatherAsthmaGen2 = { 2004, 2006, 2008, 2010, 2010, 2012, 2014 };

        // Baby Daddy: Questions answered by the Gen1 Mother about the father of her Gen2 child (ie, the items are located in the NLSY Gen1 data source)
        public readonly static Int16[] BabyDaddyItems = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyInHH = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyIsAlive = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyInHHEver = { 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyLeftHHMonthOrNeverLivedInHH = { 1992 };
        public readonly static Int16[] BabyDaddyLeftHHMonth = { 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyLeftHHYearNeverAsked = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991 };
        public readonly static Int16[] BabyDaddyLeftHHYearTwoDigit = { 1992, 1993 };
        public readonly static Int16[] BabyDaddyLeftHHYearFourDigit = { 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyLeftHHDate = { 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010 };
        public readonly static Int16[] BabyDaddyDeathNeverAsked = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991 };
        public readonly static Int16[] BabyDaddyDeathTwoDigitYear = { 1992, 1993 };
        public readonly static Int16[] BabyDaddyDeathFourDigitYear = { 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyDeathDate = { 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyDistanceFromHHFuzzyCeiling = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] BabyDaddyAsthma = { 2004, 2006, 2008, 2010, 2012, 2014 };

        public readonly static Int16[] Gen2CFatherItems = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };
        public readonly static Int16[] Gen2CFatherInHH = { 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };//No 1991
        public readonly static Int16[] Gen2CFatherAlive = { 1984, 1985, 1986, 1988, 1990, 1992, 1993, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 }; //No 1987, 1989, 1991 (but 1993 is present)
        public readonly static Int16[] Gen2CFatherDistanceFromMotherFuzzyCeiling = { 1984, 1985, 1986, 1988, 1990, 1992, 1994, 1996, 1998, 2000, 2002, 2004, 2006, 2008, 2010, 2012, 2014 };//No odd years after 1985
    }
}
