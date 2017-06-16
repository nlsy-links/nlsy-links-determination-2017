//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;

//namespace Nls.BaseAssembly {
//   public class ItemYearCount {
//      #region Fields
//      private LinksDataSet _ds;
//      private Int32 _shareRosterGen1 = Int32.MinValue;
//      private Int32 _shareBiomomGen1 = Int32.MinValue;
//      private Int32 _shareBiodadGen1 = Int32.MinValue;
//      private Int32 _shareBiodadGen2 = Int32.MinValue;
//      #endregion
//      #region Constructor
//      public ItemYearCount ( LinksDataSet ds ) {
//         if ( ds == null ) throw new ArgumentNullException("ds");
//         if ( ds.tblVariable.Count <= 0 ) throw new ArgumentException("tblVariable should not be empty.");
//         _ds = ds;
//      }
//      #endregion
//      #region Public Methods
//      internal Int32 ShareRosterGen1 {
//         get {
//            if ( _shareRosterGen1 < 0 ) _shareRosterGen1 = CountItemYears(Item.RosterGen1979, Generation.Gen1);
//            return _shareRosterGen1;
//         }
//      }
//      internal Int32 ShareBiomomGen1 {
//         get {
//            if ( _shareBiomomGen1 < 0 ) _shareBiomomGen1 = CountItemYears(Item.ShareBiomomGen1, Generation.Gen1);
//            return _shareBiomomGen1;
//         }
//      }
//      internal Int32 ShareBiodadGen1 {
//         get {
//            if ( _shareBiodadGen1 < 0 ) _shareBiodadGen1 = CountItemYears(Item.ShareBiodadGen1, Generation.Gen1);
//            return _shareBiodadGen1;
//         }
//      }
//      internal Int32 ShareBiodadGen2 {
//         get {
//            if ( _shareBiodadGen2 < 0 ) _shareBiodadGen2 = CountItemYears(Item.IDCodeOfOtherInterviewedBiodadGen2, Generation.Gen2);
//            return _shareBiodadGen2;
//         }
//      }
//      #endregion
//      #region Helpers
//      private Int32 CountItemYears ( Item item, Generation generation ) {
//         IEnumerable<Int16> years = from dr in _ds.tblVariable
//                                    where (dr.Item == (byte)item) && (dr.Generation == (byte)generation)
//                                    select dr.SurveyYear;
//         Int32 count = years.Distinct().Count();
//         Trace.Assert(count > 0, string.Format("More than one SurveyYear should be returned for Item '{0}' and Generation '{1}'.", item.ToString(), generation.ToString()));
//         return count;
//      }
//      #endregion
//   }
//}
