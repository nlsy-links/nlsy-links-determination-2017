using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nls.Base97 {
    public class SubjectDetails {
        #region Fields
        private readonly LinksDataSet _ds;
        private readonly Item[] _items = { 
            Item.DateOfBirthMonth, Item.DateOfBirthYear,
            //Item.Gen1MomOfGen2Subject, 
            Item.cross_sectional_cohort, Item.race_cohort
            //Item.BioKidCountGen1,Item.BioKidCountGen2,
            //Item.Gen1ChildsIDByBirthOrder, Item.BirthOrderInNlsGen2,
            //Item.BabyDaddyAlive, Item.BabyDaddyDeathMonth, Item.BabyDaddyDeathYearTwoDigit, Item.BabyDaddyDeathYearFourDigit
        };
        private readonly string _itemIDsString = "";
        //private static IList<OverridesGen1.SubjectYear> _overrides = OverridesGen1.InverviewDateInvalidSkip;
        #endregion
        #region Structs
        private struct LastSurvey {
            private readonly Int16? _lastSurveyYear;
            private readonly float? _ageAtLastSurvey;

            public Int16? LastSurveyYear { get { return _lastSurveyYear; } }
            public float? AgeAtLastSurvey { get { return _ageAtLastSurvey; } }

            internal LastSurvey( Int16? lastSurveyYear, float? ageAtLastSurvey ) {
                _lastSurveyYear = lastSurveyYear;
                _ageAtLastSurvey = ageAtLastSurvey;
            }
        }
        private struct BirthCondition {
            private readonly byte _order;
            private readonly byte _similarAgeCount;
            private readonly bool _hasMzPossibly;

            public byte Order { get { return _order; } }
            public byte SimilarAgeCount { get { return _similarAgeCount; } }
            public bool HasMzPossibly { get { return _hasMzPossibly; } }

            internal BirthCondition( byte order, byte similarAgeCount, bool hasMzPossibly ) {
                _order = order;
                _similarAgeCount = similarAgeCount;
                _hasMzPossibly = hasMzPossibly;
            }
        }
        private struct DeathCondition {
            private readonly bool? _isDead;
            private readonly DateTime? _deathDate;

            public bool? IsDead { get { return _isDead; } }
            public DateTime? DeathDate { get { return _deathDate; } }

            internal DeathCondition( bool? isDead, DateTime? deathDate ) {
                _isDead = isDead;
                _deathDate = deathDate;
            }
        }
        #endregion
        #region Constructor
        public SubjectDetails( LinksDataSet ds ) {
            if( ds == null ) throw new ArgumentNullException("ds");
            if( ds.tblSubject.Count <= 0 ) throw new InvalidOperationException("tblSubject must NOT be empty.");
            if( ds.tblResponse.Count <= 0 ) throw new InvalidOperationException("tblResponse must NOT be empty.");
            if( ds.tblSurveyTime.Count <= 0 ) throw new InvalidOperationException("tblSurveyTime must NOT be empty.");
            if( ds.tblSubjectDetails.Count != 0 ) throw new InvalidOperationException("tblSubjectDetails must be empty before creating rows for it.");
            _ds = ds;

            _itemIDsString = CommonCalculations.ConvertItemsToString(_items);
        }
        #endregion
        #region Public Methods
        public string Go( ) {
            const Int32 minRowCount = 1;//This is somewhat arbitrary.
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Retrieve.VerifyResponsesExistForItem(_items, _ds);
            Int32 recordsAddedTotal = 0;
            _ds.tblSubjectDetails.BeginLoadData();
            Int16[] extendedIDs = CommonFunctions.CreateExtendedFamilyIDs(_ds);
            Parallel.ForEach(extendedIDs, ( extendedID ) => {//
                //foreach( Int16 extendedID in extendedIDs ) {
                LinksDataSet.tblResponseDataTable dtExtended = Retrieve.ExtendedFamilyRelevantResponseRows(extendedID, _itemIDsString, minRowCount, _ds.tblResponse);
                LinksDataSet.tblSubjectRow[] subjectsInExtendedFamily = Retrieve.SubjectsInExtendFamily(extendedID, _ds.tblSubject);
                foreach( LinksDataSet.tblSubjectRow drSubject in subjectsInExtendedFamily ) {
                    Int32 recordsAddedForLoop = ProcessSubject(drSubject, dtExtended, subjectsInExtendedFamily);
                    Interlocked.Add(ref recordsAddedTotal, recordsAddedForLoop);
                }
                //}
            });
            _ds.tblSubjectDetails.EndLoadData();
            Trace.Assert(recordsAddedTotal == Constants.Gen1Count, "The number of 97 subjects should be correct.");

            sw.Stop();
            return string.Format("{0:N0} SubjectDetails records were created.\nElapsed time: {1}", recordsAddedTotal, sw.Elapsed.ToString());
        }
        #endregion
        #region Private Methods
        private Int32 ProcessSubject( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended, LinksDataSet.tblSubjectRow[] subjectsInExtendedFamily ) {//, LinksDataSet.tblResponseDataTable dtResponseFamily
            Int32 subjectTag = drSubject.SubjectTag;
            bool cross_section = Convert.ToBoolean(Retrieve.Response(Item.cross_sectional_cohort, drSubject.SubjectTag, dtExtended));
            EnumResponses.RaceCohort race = (EnumResponses.RaceCohort)(Retrieve.Response(Item.race_cohort, drSubject.SubjectTag, dtExtended));
            DateTime? mob = Mob.Retrieve(drSubject, dtExtended);

            byte siblingPotentialCountInNls = DetermineSiblingCountInNls(drSubject);
            BirthCondition condition = DetermineBirthCondition(drSubject, dtExtended, siblingPotentialCountInNls);
            byte birthOrderInNls = condition.Order;
            byte similarAgeCount = condition.SimilarAgeCount;
            bool hasMzPossibly = condition.HasMzPossibly;

            LastSurvey lastSurvey = LastSurveyCompleted(subjectTag);//This call takes 12 minutes total.
            Int16? lastSurveyYear = lastSurvey.LastSurveyYear;
            float? lastAge = lastSurvey.AgeAtLastSurvey;

            //byte? kidCountBio = DetermineBioKidCount(drSubject, dtExtended, lastAge);
            //byte? kidCountInNls = DetermineNlsKidCount(drSubject, subjectsInExtendedFamily);

            //DeathCondition subjectDeath = DetermineSubjectDeath();
            //bool isDead = subjectDeath.IsDead.Value;
            //DateTime? deathDate = subjectDeath.DeathDate;
            //DeathCondition biodadDeath = DetermineBiodadDeath(drSubject, dtExtended);
            //bool? isBiodadDead = biodadDeath.IsDead;
            //DateTime? biodadDeathDate = biodadDeath.DeathDate;

            AddRow(subjectTag, cross_section, race, siblingPotentialCountInNls, birthOrderInNls, similarAgeCount, hasMzPossibly, mob, lastSurveyYear, lastAge);
            //AddRow(subjectTag, cross_section, race, siblingCountInNls, birthOrderInNls, similarAgeCount, hasMzPossibly, kidCountBio, kidCountInNls, mob, lastSurveyYear, lastAge, isDead, deathDate, isBiodadDead, biodadDeathDate);
            return 1;
        }
        //private static DeathCondition DetermineSubjectDeath( ) {
        //    return new DeathCondition(false, null);
        //}
        ////private static DeathCondition DetermineBiodadDeath( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended ) {
        ////    if( (Sample)drSubject.Generation == Sample.Nlsy79Gen1 )
        ////        return new DeathCondition(null, null);
        ////    Int32 motherTag = CommonCalculations.MotherTagOfGen2Subject(drSubject.SubjectID);
        ////    byte? childLoop = Retrieve.MotherLoopIndexForChildTag(motherTag, drSubject, dtExtended);
        ////    if( !childLoop.HasValue )
        ////        return new DeathCondition(null, null);
        ////    ////TODO: retrieve an array of these values, and return the most recent
        ////    //const Item livingItem = Item.BabyDaddyAliveGen1;
        ////    ////isDead = Retrieve.ResponseNullPossible(livingItem,
        ////    //Int16[] isDeads = Retrieve.ResponseArray(livingItem, drSubject.SubjectTag, childLoop.Value, dtExtended);
        ////    //They answer the 'Living in HH' item first, and only a subset answer if he's still alive.
        ////    Int16[] itemYearsFourDigitsReversed = ItemYears.BabyDaddyDeathFourDigitYear.Reverse().ToArray();
        ////    foreach( Int16 surveyYear in itemYearsFourDigitsReversed ) {
        ////        Int32? deathYear = Retrieve.ResponseNullPossible(surveyYear, Item.BabyDaddyDeathYearFourDigit, motherTag, childLoop.Value, dtExtended);
        ////        if( (deathYear.HasValue) && (deathYear.Value > 0) ) {
        ////            Int32? deathMonth = Retrieve.ResponseNullPossible(surveyYear, Item.BabyDaddyDeathMonth, motherTag, childLoop.Value, dtExtended);
        ////            if( (!deathMonth.HasValue) || (deathMonth.Value < 1) )
        ////                deathMonth = Constants.DefaultMonthOfYear;
        ////            DateTime deathDate = new DateTime(deathYear.Value, deathMonth.Value, Constants.DefaultDayOfMonth);
        ////            return new DeathCondition(true, deathDate);
        ////        }
        ////    }

        ////    Int16[] itemYearsTwoDigitsReversed = ItemYears.BabyDaddyDeathTwoDigitYear.Reverse().ToArray();
        ////    foreach( Int16 surveyYear in itemYearsTwoDigitsReversed ) {
        ////        Int32? deathYear = Retrieve.ResponseNullPossible(surveyYear, Item.BabyDaddyDeathYearTwoDigit, motherTag, childLoop.Value, dtExtended);
        ////        if( (deathYear.HasValue) && (deathYear.Value > 0) ) {
        ////            Int32? deathMonth = Retrieve.ResponseNullPossible(surveyYear, Item.BabyDaddyDeathMonth, motherTag, childLoop.Value, dtExtended);
        ////            if( (!deathMonth.HasValue) || (deathMonth.Value < 1) )
        ////                deathMonth = Constants.DefaultMonthOfYear;
        ////            DateTime deathDate = new DateTime(1900 + deathYear.Value, deathMonth.Value, Constants.DefaultDayOfMonth);
        ////            return new DeathCondition(true, deathDate);
        ////        }
        ////    }

        ////    return new DeathCondition(null, null);
        ////}
        private byte DetermineSiblingCountInNls( LinksDataSet.tblSubjectRow drSubject ) {
            string select = string.Format("{0}={1}", drSubject.ExtendedID, _ds.tblSubject.ExtendedIDColumn.ColumnName);
            LinksDataSet.tblSubjectRow[] drs = (LinksDataSet.tblSubjectRow[])_ds.tblSubject.Select(select);
            Trace.Assert(drs.Length > 0, "At least one row should be returned.");

            return (byte)drs.Length;

        }
        private BirthCondition DetermineBirthCondition( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended, byte siblingPotentialCountInNls ) {//, LinksDataSet.tblResponseDataTable dtResponse 
            string select = string.Format("{0}={1}", drSubject.ExtendedID, dtExtended.ExtendedIDColumn.ColumnName);
            LinksDataSet.tblSubjectRow[] drs = (LinksDataSet.tblSubjectRow[])_ds.tblSubject.Select(select);
            byte orderTally = 1;
            byte similarAgeTally = 0;
            bool hasMzPossibly = false;
            DateTime? mobOfSubject = Mob.Retrieve(drSubject, dtExtended); //There shouldn't be any missing Mobs in Gen1.


            Trace.Assert(drs.Length == siblingPotentialCountInNls, "The number of returned rows should match 'siblingCountInNls'.");

            foreach( LinksDataSet.tblSubjectRow dr in drs ) {
                DateTime mobOfSibling = Mob.Retrieve(dr, dtExtended).Value;//There aren't any missings Mobs in Gen1.
                double ageDifferenceInDays = mobOfSubject.Value.Subtract(mobOfSibling).TotalDays;
                if( ageDifferenceInDays > 0 )//This should account for twins and the subject himself.
                    orderTally += 1;
                if( Math.Abs(ageDifferenceInDays) <= Constants.MaxDaysBetweenTwinBirths ) {
                    similarAgeTally += 1;//This counts themselves; eg, a only child will still have a value of one.
                    hasMzPossibly = hasMzPossibly || DetermineMzPossibility(drSubject, dr);
                }
            }

            return new BirthCondition(orderTally, similarAgeTally, hasMzPossibly);
        }
        private static bool DetermineMzPossibility( LinksDataSet.tblSubjectRow dr1, LinksDataSet.tblSubjectRow dr2 ) {
            if( dr1.SubjectTag == dr2.SubjectTag ) {
                return false;//The rows point to the same subject;
            } else if( dr1.Gender != dr2.Gender ) {
                return false; //Mzs can't have different genders.
            }
            return true;
        }
        ////private static byte? DetermineBioKidCount( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended, float? lastAge ) {
        ////    switch( (Sample)drSubject.Generation ) {
        ////        case Sample.Nlsy79Gen1:
        ////            return (byte)Retrieve.Response(Item.BioKidCountGen1, drSubject.SubjectTag, dtExtended);

        ////        case Sample.Nlsy79Gen2:
        ////            //EnumResponsesGen2.KidBioCount count = (EnumResponsesGen2.KidBioCount)Retrieve.Response(Item.BioKidCountGen2, drSubject.SubjectTag, dtExtended);
        ////            Int32? response = Retrieve.ResponseNullPossible(Item.BioKidCountGen2, drSubject.SubjectTag, dtExtended);
        ////            if( response.HasValue )
        ////                return (byte)response.Value;
        ////            else if( lastAge.HasValue && lastAge.Value <= Constants.Gen2AgeMissingHasChildrenThreshold )
        ////                return 0;
        ////            else
        ////                return null;
        ////        default:
        ////            throw new InvalidOperationException("The Generation value was not recognized.");
        ////    }
        ////}
        ////private static byte? DetermineNlsKidCount( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblSubjectRow[] subjectsInExtendedFamily ) {
        ////    switch( (Sample)drSubject.Generation ) {
        ////        case Sample.Nlsy79Gen1:
        ////            byte tally = 0;
        ////            foreach( LinksDataSet.tblSubjectRow drRelative in subjectsInExtendedFamily ) {
        ////                if( (Sample)drRelative.Generation == Sample.Nlsy79Gen2 ) {
        ////                    Int32 motherIDV1 = CommonCalculations.MotherIDOfGen2Subject(drRelative.SubjectTag);
        ////                    //Int32 motherIDV2 = Retrieve.Response(Item.Gen1MomOfGen2Subject, drSubject.SubjectTag, dtExtended);
        ////                    //Trace.Assert(motherIDV1 == motherIDV2, "The mother IDs should match.");
        ////                    if( drSubject.SubjectID == motherIDV1 )
        ////                        tally += 1;
        ////                }
        ////            }
        ////            return tally;
        ////        case Sample.Nlsy79Gen2:
        ////            return null;
        ////        default:
        ////            throw new InvalidOperationException("The Generation value was not recognized.");
        ////    }
        ////}

        private LastSurvey LastSurveyCompleted( Int32 subjectTag ) {
            string select = string.Format("{0}={1} AND {2}=1",
                subjectTag, _ds.tblSurveyTime.SubjectTagColumn.ColumnName,
                _ds.tblSurveyTime.SurveyTakenColumn.ColumnName);
            string sort = _ds.tblSurveyTime.SurveyYearColumn.ColumnName + " DESC";
            LinksDataSet.tblSurveyTimeRow[] drs = (LinksDataSet.tblSurveyTimeRow[])_ds.tblSurveyTime.Select(select, sort);
            if( drs.Length <= 0 )
                return new LastSurvey(null, null);
            else
                return new LastSurvey(drs[0].SurveyYear, (float)drs[0].AgeCalculateYears); //There's only one case where the calculated age is missing; in this case it's not their last survey, so it doesn't matter here.
        }
        //private void AddRow( Int32 subjectTag, EnumResponses.RaceCohort raceCohort, byte siblingCountInNls, byte birthOrderInNls, byte similarAgeCount, bool hasMzPossibly, byte? kidCountBio, byte? kidCountInNls,
        //    DateTime? mob, Int16? lastSurveyYearCompleted, float? ageAtLastSurvey,
        //    bool isDead, DateTime? deathDate, bool? isBiodadDead, DateTime? biodadDeathDate ) {
        private void AddRow( Int32 subjectTag, bool cross_sectional, EnumResponses.RaceCohort raceCohort, byte siblingPotentialCountInNls, byte birthOrderInNls, byte similarAgeCount, bool hasMzPossibly, //byte? kidCountBio, byte? kidCountInNls,
            DateTime? mob, Int16? lastSurveyYearCompleted, float? ageAtLastSurvey//,
            //bool isDead, DateTime? deathDate, bool? isBiodadDead, DateTime? biodadDeathDate 
            ) {

            lock( _ds.tblSubjectDetails ) {
                LinksDataSet.tblSubjectDetailsRow drNew = _ds.tblSubjectDetails.NewtblSubjectDetailsRow();
                drNew.SubjectTag = subjectTag;
                drNew.CrossSectionalCohort = cross_sectional;
                drNew.RaceCohort = Convert.ToByte(raceCohort);
                drNew.SiblingPotentialCountInNls = siblingPotentialCountInNls;
                drNew.BirthOrderInNls = birthOrderInNls;
                drNew.SimilarAgeCount = similarAgeCount;
                drNew.HasMzPossibly = hasMzPossibly;


                drNew.KidCountBio = 11;
                //if( kidCountBio.HasValue ) drNew.KidCountBio = (byte)kidCountBio;
                //else drNew.SetKidCountBioNull();


                drNew.Mob = mob.Value;
                drNew.LastSurveyYearCompleted = lastSurveyYearCompleted.Value;
                drNew.AgeAtLastSurvey = (float)ageAtLastSurvey;


                drNew.IsDead = true;// isDead;

                drNew.DeathDate = new DateTime(1982, 6, 6);
                //if( deathDate.HasValue ) drNew.DeathDate = deathDate.Value;
                //else drNew.SetDeathDateNull();

                drNew.IsBiodadDead = true;
                //if( isBiodadDead.HasValue ) drNew.IsBiodadDead = isBiodadDead.Value;
                //else drNew.SetIsBiodadDeadNull();

                drNew.BiodadDeathDate = new DateTime(1982, 6, 6);
                //if( biodadDeathDate.HasValue ) drNew.BiodadDeathDate = biodadDeathDate.Value;
                //else drNew.SetBiodadDeathDateNull();

                _ds.tblSubjectDetails.AddtblSubjectDetailsRow(drNew);
            }
        }
        #endregion
    }
}
