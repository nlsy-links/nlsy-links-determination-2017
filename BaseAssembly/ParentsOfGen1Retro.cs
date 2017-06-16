using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nls.BaseAssembly.Trend;

namespace Nls.BaseAssembly {
	public sealed class ParentsOfGen1Retro {
		#region Fields
		private readonly LinksDataSet _ds;
		private readonly Item[] _items = { Item.DateOfBirthMonth, Item.DateOfBirthYearGen1, Item.Gen1LivedWithFatherAtAgeX, Item.Gen1LivedWithMotherAtAgeX, Item.Gen1AlwaysLivedWithBothParents };
		private readonly string _itemIDsString = "";
		private const Int16 _surveyYear = ItemYears.Gen1BioparentInHH;

		#endregion
		#region Constructor
		public ParentsOfGen1Retro ( LinksDataSet ds ) {
			if ( ds == null ) throw new ArgumentNullException("ds");
			if ( ds.tblResponse.Count <= 0 ) throw new InvalidOperationException("tblResponse must NOT be empty.");
			if ( ds.tblParentsOfGen1Retro.Count != 0 ) throw new InvalidOperationException("tblParentsOfGen1Retro must be empty before creating rows for it.");
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
			_ds.tblParentsOfGen1Retro.BeginLoadData();

			Int16[] extendedIDs = CommonFunctions.CreateExtendedFamilyIDs(_ds);
			//Parallel.ForEach(extendedIDs, ( extendedID ) => {//
			foreach ( Int16 extendedID in extendedIDs ) {
				LinksDataSet.tblResponseDataTable dtExtendedResponse = Retrieve.ExtendedFamilyRelevantResponseRows(extendedID, _itemIDsString, minRowCount, _ds.tblResponse);
				LinksDataSet.tblSubjectRow[] subjectsInExtendedFamily = Retrieve.SubjectsInExtendFamily(extendedID, _ds.tblSubject);
				foreach ( LinksDataSet.tblSubjectRow drSubject in subjectsInExtendedFamily ) {
					if ( (Generation)drSubject.Generation == Generation.Gen1 ) {
						Int32 recordsAddedForLoop = ProcessSubjectGen1(drSubject, dtExtendedResponse);
						Interlocked.Add(ref recordsAddedTotal, recordsAddedForLoop);
					}
				}
			}
			_ds.tblParentsOfGen1Retro.EndLoadData();
			sw.Stop();
			return string.Format("{0:N0} tblParentsOfGen1Retro records were created.\nElapsed time: {1}", recordsAddedTotal, sw.Elapsed.ToString());
		}
		#endregion
		#region Private Methods
		private Int32 ProcessSubjectGen1 ( LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			Int32 subjectTag = drSubject.SubjectTag;
			Int16 yob = Convert.ToInt16(Mob.Retrieve(drSubject, dtExtendedResponse).Value.Year);
			Tristate bothParentsAlways = DetermineBothParentsAlwaysInHH(_surveyYear, subjectTag, yob, dtExtendedResponse);

			Int32 recordsAdded = 0;
			recordsAdded += ProcessForOneParent(bothParentsAlways, Bioparent.Dad, yob, drSubject, dtExtendedResponse);
			recordsAdded += ProcessForOneParent(bothParentsAlways, Bioparent.Mom, yob, drSubject, dtExtendedResponse);
			return recordsAdded;
		}
		private Int32 ProcessForOneParent ( Tristate bothParentsAlways, Bioparent parent,Int16 yob, LinksDataSet.tblSubjectRow drSubject, LinksDataSet.tblResponseDataTable dtExtendedResponse ) {
			const byte loopIndexForNever = 255;
			byte[] loopIndicesAndAges = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
			Int32 recordsAdded = 0;
			Item item;
			switch ( parent ) {
				case Bioparent.Dad: item = Item.Gen1LivedWithFatherAtAgeX; break;
				case Bioparent.Mom: item = Item.Gen1LivedWithMotherAtAgeX; break;
				default: throw new ArgumentOutOfRangeException("parent");
			}
			Tristate responseEver = DetermineOneParentEverInHH(item, _surveyYear, drSubject.SubjectTag, loopIndexForNever, dtExtendedResponse);

			foreach ( byte loopIndexAndAge in loopIndicesAndAges ) {
				Tristate inHH;
				if ( bothParentsAlways == Tristate.Yes ) 
					inHH =	Tristate.Yes;
				else if ( responseEver == Tristate.DoNotKnow )
					inHH = Tristate.DoNotKnow;
				else if ( responseEver == Tristate.Yes ) 
					inHH = DetermineParentInHH(Item.Gen1LivedWithFatherAtAgeX, _surveyYear, drSubject.SubjectTag, loopIndexAndAge, dtExtendedResponse);
				else if ( responseEver == Tristate.No )
					inHH = Tristate.No;
				else 
					throw new InvalidOperationException("The execution shouldn't have gotten to here in the 'ProcessForOneParent' function.");

				Int16 yearInHH = Convert.ToInt16(yob + loopIndexAndAge);
				AddRow(drSubject.SubjectTag, drSubject.ExtendedID, parent, inHH: inHH, age: loopIndexAndAge, yearInHH: yearInHH);
				recordsAdded += 1; ;
			}
			return recordsAdded;
		}
		private static Tristate DetermineBothParentsAlwaysInHH ( Int16 surveyYear, Int32 subjectTag, Int16 yob, LinksDataSet.tblResponseDataTable dtExtended ) {
			Item item = Item.Gen1AlwaysLivedWithBothParents;
			Int32? responseBothAlways = Retrieve.ResponseNullPossible(surveyYear, item, subjectTag, dtExtended);
			if ( !responseBothAlways.HasValue )
				return Tristate.DoNotKnow;
			else 
				return CommonFunctions.TranslateYesNo((YesNo)responseBothAlways.Value);
		}
		private static Tristate DetermineOneParentEverInHH ( Item item, Int16 surveyYear, Int32 subjectTag, byte loopIndex, LinksDataSet.tblResponseDataTable dtExtended ) {
			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, subjectTag, loopIndex, dtExtended);
			if ( !response.HasValue )
				return Tristate.DoNotKnow;
			else
				return CommonFunctions.TranslateYesNo(CommonFunctions.ReverseYesNo((YesNo)response));
		}
		private static Tristate DetermineParentInHH ( Item item, Int16 surveyYear, Int32 subjectTag, byte loopIndex, LinksDataSet.tblResponseDataTable dtExtended ) {
			Int32? response = Retrieve.ResponseNullPossible(surveyYear, item, subjectTag, loopIndex, dtExtended);
			if ( !response.HasValue )
				return Tristate.DoNotKnow;

			EnumResponsesGen1.BioparentOfGen1InHH codedResponse = (EnumResponsesGen1.BioparentOfGen1InHH)response.Value;
			YesNo yn;
			switch ( codedResponse ) {
				case EnumResponsesGen1.BioparentOfGen1InHH.NonInterview:
				case EnumResponsesGen1.BioparentOfGen1InHH.ValidSkip:
				case EnumResponsesGen1.BioparentOfGen1InHH.InvalidSkip:
				case EnumResponsesGen1.BioparentOfGen1InHH.DoNotKnow:
				case EnumResponsesGen1.BioparentOfGen1InHH.Refusal:
					yn = YesNo.ValidSkipOrNoInterviewOrNotInSurvey;
					break;
				case EnumResponsesGen1.BioparentOfGen1InHH.No:
					yn = YesNo.No;
					break;
				case EnumResponsesGen1.BioparentOfGen1InHH.Yes:
					yn = YesNo.Yes;
					break;
				default: throw new InvalidOperationException("The response " + codedResponse + " was not recognized.");
			}
			return CommonFunctions.TranslateYesNo(yn);
		}
		private void AddRow ( Int32 subjectTag, Int16 extendedID, Bioparent parent, Tristate inHH,  byte age, Int16 yearInHH ) {
			//lock ( _ds.tblFatherOfGen2 ) {
			LinksDataSet.tblParentsOfGen1RetroRow drNew = _ds.tblParentsOfGen1Retro.NewtblParentsOfGen1RetroRow();
			drNew.SubjectTag = subjectTag;
			drNew.ExtendedID = extendedID;
			drNew.Bioparent = (byte)parent;

			if ( inHH==Tristate.DoNotKnow ) drNew.SetInHHNull();
			else drNew.InHH = (inHH==Tristate.Yes);

			drNew.Age = age;
			drNew.Year = yearInHH;
			_ds.tblParentsOfGen1Retro.AddtblParentsOfGen1RetroRow(drNew);
			//}
		}
		#endregion
		#region Public/Private Static
		public static TrendLineGen0InHH RetrieveTrend ( Bioparent bioparent, Int32 subjectTag, LinksDataSet.tblParentsOfGen1RetroDataTable dtRetro ) { //, LinksDataSet.tblSubjectDetailsDataTable dtDetail ) {
			if ( dtRetro == null )
				return new TrendLineGen0InHH(yob: 0, hasAnyRecords: false, everAtHome: false, years: null, values: null, ages: null);
			else if ( dtRetro.Count <= 0 )
				throw new ArgumentException("There should be at least one row in tblParentsOfGen1Retro.");

			string selectYears = string.Format("{0}={1} AND {2}={3}",
				subjectTag, dtRetro.SubjectTagColumn.ColumnName,
				(byte)bioparent, dtRetro.BioparentColumn.ColumnName);
			LinksDataSet.tblParentsOfGen1RetroRow[] drs = (LinksDataSet.tblParentsOfGen1RetroRow[])dtRetro.Select(selectYears);
			Trace.Assert(drs.Length >= 0, "At least zero records should be retrieved from tblParentsOfGen1Retro.");
			Int16 yob = (from dr in drs where dr.Age == 0 select dr.Year).First();

			bool? everInHH;
			if ( drs[0].IsInHHNull() ) everInHH = null;
			else everInHH = drs[0].InHH;

			Int16[] years = new Int16[drs.Length];
			byte[] ages = new byte[drs.Length];
			bool?[] inHHs = new bool?[drs.Length];

			//values = (from dr in drsYes select (YesNo)dr.BiodadInHH).ToArray();
			for ( Int32 i = 0; i < drs.Length; i++ ) {
				years[i] = drs[i].Year;
				ages[i] = drs[i].Age;

				if ( drs[i].IsInHHNull() ) inHHs[i] = null;
				else inHHs[i] = drs[i].InHH;
			}
			//switch ( everInHH ) {
			//   case YesNo.No:
			//      Trace.Assert(drsYes.Length == 0, "If the subject says the bioparent has never lived in the HH, then there shouldn't be any more records.");
			//      break;
			//   case YesNo.Yes:
			//      //This is ok.  Execute the statements following the switch statement.
			//      break;
			//   default:
			//      throw new ArgumentOutOfRangeException("bioparent");
			//}
			return new TrendLineGen0InHH(yob: yob, hasAnyRecords: true, everAtHome: everInHH, years: years, values: inHHs, ages: ages);
		}
		public static Tristate RetrieveInHHByYear ( Int32 subjectTag, Bioparent bioparent, Int16 year, LinksDataSet.tblParentsOfGen1RetroDataTable dtRetro ) {
			if ( dtRetro == null ) throw new ArgumentNullException("dsLinks");
			if ( dtRetro.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblParentsOfGen1Retro.");

			string select = string.Format("{0}={1} AND {2}={3} AND {4}={5}",
				subjectTag, dtRetro.SubjectTagColumn.ColumnName,
				Convert.ToByte(bioparent), dtRetro.BioparentColumn.ColumnName,
				year, dtRetro.YearColumn.ColumnName);

			DataRow[] drs = dtRetro.Select(select);
			Trace.Assert(drs.Length <= 1, "There should be at most one row."); //The item asked only until they were 18.  The function could be requesting a year when they were older.
			if ( drs.Length == 0 ) return Tristate.DoNotKnow;

			LinksDataSet.tblParentsOfGen1RetroRow dr = (LinksDataSet.tblParentsOfGen1RetroRow)drs[0];
			if ( dr.IsInHHNull() )
				return Tristate.DoNotKnow;
			else if ( dr.InHH )
				return Tristate.Yes;
			else if ( !dr.InHH )
				return Tristate.No;
			else
				throw new InvalidOperationException();
		}
		public static LinksDataSet.tblParentsOfGen1RetroDataTable RetrieveRows ( Int32 subject1Tag, Int32 subject2Tag, LinksDataSet.tblParentsOfGen1RetroDataTable dtRetro ) {
			if ( dtRetro == null ) throw new ArgumentNullException("dsLinks");
			if ( dtRetro.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblParentsOfGen1Retro.");

			string select = string.Format("{0}={1} OR {2}={3}",
				subject1Tag, dtRetro.SubjectTagColumn.ColumnName,
				subject2Tag, dtRetro.SubjectTagColumn.ColumnName);
			LinksDataSet.tblParentsOfGen1RetroRow[] drs = (LinksDataSet.tblParentsOfGen1RetroRow[])dtRetro.Select(select);
			//Trace.Assert(drs.Length >= 1, "There should be at least one row.");

			LinksDataSet.tblParentsOfGen1RetroDataTable dt = new LinksDataSet.tblParentsOfGen1RetroDataTable();
			foreach ( LinksDataSet.tblParentsOfGen1RetroRow dr in drs ) {
				dt.ImportRow(dr);
			}
			return dt;
		}
		public static LinksDataSet.tblParentsOfGen1RetroDataTable RetrieveRows ( Int32 extendedID, LinksDataSet.tblParentsOfGen1RetroDataTable dtRetro ) {
		   if ( dtRetro == null ) throw new ArgumentNullException("dsLinks");
		   if ( dtRetro.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblParentsOfGen1Retro.");

		   string select = string.Format("{0}={1}", extendedID, dtRetro.ExtendedIDColumn.ColumnName);
		   LinksDataSet.tblParentsOfGen1RetroRow[] drs = (LinksDataSet.tblParentsOfGen1RetroRow[])dtRetro.Select(select);
		   Trace.Assert(drs.Length >= 1, "There should be at least one row.");
			
		   LinksDataSet.tblParentsOfGen1RetroDataTable dt = new LinksDataSet.tblParentsOfGen1RetroDataTable();
		   foreach ( LinksDataSet.tblParentsOfGen1RetroRow dr in drs ) {
		      dt.ImportRow(dr);
		   }
		   return dt;
		}
		//public static LinksDataSet.tblParentsOfGen1RetroDataTable RetrieveRows ( Int32 subjectTag, LinksDataSet dsLinks ) {
		//   if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
		//   if ( dsLinks.tblParentsOfGen1Retro.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblParentsOfGen1Retro.");

		//   string select = string.Format("{0}={1}", subjectTag, dsLinks.tblParentsOfGen1Retro.SubjectTagColumn.ColumnName);
		//   LinksDataSet.tblParentsOfGen1RetroRow[] drs = (LinksDataSet.tblParentsOfGen1RetroRow[])dsLinks.tblParentsOfGen1Retro.Select(select);
		//   //Trace.Assert(drs.Length >= 1, "There should be at least one row.");
		//   LinksDataSet.tblParentsOfGen1RetroDataTable dt = new LinksDataSet.tblParentsOfGen1RetroDataTable();
		//   foreach ( LinksDataSet.tblParentsOfGen1RetroRow dr in drs ) {
		//      dt.ImportRow(dr);
		//   }
		//   return dt;
		//}
		//public static Int16?[] RetrieveInHH ( Int32 subjectTag, Int16[] surveyYears, LinksDataSet.tblFatherOfGen2DataTable dtInput ) {
		//   if ( dtInput == null ) throw new ArgumentNullException("dtInput");
		//   if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
		//   if ( dtInput.Count <= 0 ) throw new ArgumentException("There should be at least one row in tblFatherOfGen2.");

		//   Int16?[] values = new Int16?[surveyYears.Length];
		//   for ( Int32 i = 0; i < values.Length; i++ ) {
		//      LinksDataSet.tblFatherOfGen2Row dr = RetrieveRow(subjectTag, surveyYears[i], dtInput);
		//      if ( dr == null )
		//         values[i] = null;
		//      else if ( (YesNo)dr.BiodadInHH == YesNo.ValidSkipOrNoInterviewOrNotInSurvey )
		//         values[i] = null;
		//      else if ( (YesNo)dr.BiodadInHH == YesNo.InvalidSkip )
		//         values[i] = null;
		//      else
		//         values[i] = dr.BiodadInHH;
		//   }
		//   return values;
		//}
		#endregion
	}
}
