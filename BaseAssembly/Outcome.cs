using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nls.BaseAssembly {
    public class Outcome {
        #region Fields
        private readonly LinksDataSet _ds;
        //private readonly Item[] _items;
        private readonly Item[] _itemsGen1 = { Item.Gen1HeightInches, Item.Gen1WeightPounds, Item.Gen1AfqtScaled3Decimals };
        private readonly Item[] _itemsGen2 = { Item.Gen2CFatherAlive};
        private readonly string _itemIDsString = "";
        private readonly string _itemIDsStringGen1 = "";
        private readonly string _itemIDsStringGen2 = "";
        //private readonly OutcomeItem[] _outcomeItemsGen1;
        //private readonly OutcomeItem[] _outcomeItemsGen2;
        #endregion
        #region Constructor
        public Outcome( LinksDataSet ds ) {
            if( ds == null ) throw new ArgumentNullException("ds");
            if( ds.tblSubject.Count <= 0 ) throw new InvalidOperationException("tblSubject must NOT be empty.");
            if( ds.tblResponse.Count <= 0 ) throw new InvalidOperationException("tblResponse must NOT be empty.");
            if( ds.tblSurveyTime.Count <= 0 ) throw new InvalidOperationException("tblSurveyTime must NOT be empty.");
            if( ds.tblOutcome.Count != 0 ) throw new InvalidOperationException("tblOutcome must be empty before creating rows for it.");
            _ds = ds;

            //_items = new Item[_itemsGen1.Length + _itemsGen2.Length];
            //_itemsGen1.CopyTo(_items, 0);
            //_itemsGen2.CopyTo(_items, _itemsGen1.Length);
            //CommonCalculations.ConvertItemsToString(_items);

            _itemIDsStringGen1     = CommonCalculations.ConvertItemsToString(_itemsGen1);
            _itemIDsStringGen2 = CommonCalculations.ConvertItemsToString(_itemsGen2);
            _itemIDsString = _itemIDsStringGen1 + ", " + _itemIDsStringGen2;

            //_outcomeItemsGen1 = BuildOutcomeYearsGen1();
            //_outcomeYearsGen2 = BuildOutcomeYearsGen2();
        }
        #endregion
        #region Public Methods
        public string Go( ) {
            const Int32 minRowCount = 1;//This is somewhat arbitrary.
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Retrieve.VerifyResponsesExistForItem(_itemsGen1, _ds);
            Retrieve.VerifyResponsesExistForItem(_itemsGen2, _ds);

            Int32 recordsAddedTotal = 0;
            _ds.tblOutcome.BeginLoadData();
            Int16[] extendedIDs = CommonFunctions.CreateExtendedFamilyIDs(_ds);
            //Parallel.ForEach(extendedIDs, ( extendedID ) => {//
            foreach( Int16 extendedID in extendedIDs ) {
                LinksDataSet.tblResponseDataTable dtExtended = Retrieve.ExtendedFamilyRelevantResponseRows(extendedID, _itemIDsString, minRowCount, _ds.tblResponse);
                LinksDataSet.tblSubjectRow[] subjectsInExtendedFamily = Retrieve.SubjectsInExtendFamily(extendedID, _ds.tblSubject);
                foreach( LinksDataSet.tblSubjectRow drSubject in subjectsInExtendedFamily ) {
                    Int32 recordsAddedForLoop = ProcessSubject(drSubject, dtExtended);//subjectsInExtendedFamily
                    Interlocked.Add(ref recordsAddedTotal, recordsAddedForLoop);
                }
            }
            _ds.tblOutcome.EndLoadData();
            Trace.Assert(recordsAddedTotal == Constants.Gen1Count + Constants.Gen2Count, "The number of Gen1+Gen2 subjects should be correct.");

            sw.Stop();
            return string.Format("{0:N0} Outcome records were created.\nElapsed time: {1}", recordsAddedTotal, sw.Elapsed.ToString());
        }
        #endregion
        #region Private Methods
        private Int32 ProcessSubject( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended ) {
            const Int32 minRowCount=0;
            string itemString = "";
            if( drSubject.Generation == (byte)Generation.Gen1 )
                itemString = _itemIDsStringGen1;
            else if( drSubject.Generation == (byte)Generation.Gen2 )
                itemString = _itemIDsStringGen2;
            else
                throw new InvalidOperationException("The execution should not have gotten here.  The value of Generation was not recognized.");

            LinksDataSet.tblResponseDataTable dt = Retrieve.SubjectsRelevantResponseRows(drSubject.SubjectTag, itemString, minRowCount, dtExtended);
            foreach( LinksDataSet.tblResponseRow dr in dt){
                AddRow(drSubject.SubjectTag, dr.Item, dr.SurveyYear, dr.Value);
            }
            return dt.Count;

        }
        private void AddRow( Int32 subjectTag, Int16 item, Int16 surveyYear, Int32 value ) {
            LinksDataSet.tblOutcomeRow drNew = _ds.tblOutcome.NewtblOutcomeRow();
            drNew.SubjectTag = subjectTag;
            drNew.Item = item;
            drNew.SurveyYear = surveyYear;
            drNew.Value = value;
            _ds.tblOutcome.AddtblOutcomeRow(drNew);
        }
        #endregion
    }
}
//#region Outcome Years

//internal struct OutcomeItem {
//    private readonly OutcomeEnum _outcome;
//    private readonly Item _item;
//    //private readonly Int16[] _surveyYears;

//    public OutcomeEnum Outcome { get { return _outcome; } }
//    public Item Item { get { return _item; } }
//    //public Int16[] SurveyYears { get { return _surveyYears; } }

//    public OutcomeItem( OutcomeEnum outcome, Item item ) {//, Int16[] surveyYears
//        _outcome = outcome;
//        _item = item;
//        //_surveyYears = surveyYears;
//    }

//}
//internal OutcomeItem[] BuildOutcomeYearsGen1(){
//    List<OutcomeItem> outcomes = new List<OutcomeItem>();
//    outcomes.Add(new OutcomeItem( OutcomeEnum.HeightAdult, Item.Gen1HeightInches));
//    outcomes.Add(new OutcomeItem( OutcomeEnum.WeightAdult, Item.Gen1WeightPounds));
//    return outcomes.ToArray();
//}
//internal OutcomeItem BuildOutcomeYearsGen2(){
//    throw new NotImplementedException();
//}

//#endregion


//private byte? DetermineHeightIn1982( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended ) {
//    const Int16 surveyYear = 1982;
//    const Item item = Item.Gen1HeightInches;
//    const SurveySource source = SurveySource.Gen1;
//    const byte maxRows = 1;
//    Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, source, drSubject.SubjectTag, maxRows, dtExtended);
//    byte? converted = (byte?)response;
//    return converted;
//}
//private Int16? DetermineWeightIn1982( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended ) {
//    const Int16 surveyYear = 1982;
//    const Item item = Item.Gen1WeightPounds;
//    const SurveySource source = SurveySource.Gen1;
//    const byte maxRows = 1;
//    Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, source, drSubject.SubjectTag, maxRows, dtExtended);
//    return (Int16?)response;
//}
//private float? DetermineAfqtIn1981( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtended ) {
//    const Int16 surveyYear = 1981;
//    const Item item = Item.Gen1AfqtScaled5Decimals;
//    const SurveySource source = SurveySource.Gen1;
//    const byte maxRows = 1;
//    Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, source, drSubject.SubjectTag, maxRows, dtExtended);

//    if( response.HasValue && response >= 0 )
//        return ((float)(response / (double)100000));
//    else
//        return null;
//}
