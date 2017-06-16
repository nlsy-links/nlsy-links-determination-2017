using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nls.BaseAssembly;

namespace Nls.BaseAssembly.Assign {
	public class RGen1Pass1 : IAssignPass1 {
		#region Fields
		private readonly ImportDataSet _dsImport;
		private readonly LinksDataSet _dsLinks;
		private readonly LinksDataSet.tblRelatedStructureRow _drLeft;
		private readonly LinksDataSet.tblRelatedStructureRow _drRight;
		//private readonly LinksDataSet.tblSubjectRow _drBare1;
		//private readonly LinksDataSet.tblSubjectRow _drBare2;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails1;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails2;
		private readonly LinksDataSet.tblMarkerGen1DataTable _dtMarkersGen1;

		private readonly Int32 _idRelatedLeft = Int32.MinValue;
		private readonly Int32 _idRelatedRight = Int32.MinValue;
		private readonly Int32 _idRelatedOlderAboutYounger = Int32.MinValue;//usually equal to _idRelatedLeft
		private readonly Int32 _idRelatedYoungerAboutOlder = Int32.MinValue;//usually equal to _idRelatedRight

		private readonly Int32 _extendedID;
		private readonly MultipleBirth _multipleBirth;
		private readonly Tristate _isMZ;
		private readonly Tristate _isRelatedInMzManual;
		//private Int16 _rosterAssignment=Int16.MinValue;
		//private float? _rRoster = float.NaN;
		private float? _rImplicitPass1 = null;// float.NaN;
		private float? _rImplicit2004 = float.NaN;

		private readonly Tristate _implicitShareBiomomPass1;
		private readonly Tristate _implicitShareBiodadPass1;

		private readonly Tristate _explicitShareBiomomPass1;
		private readonly Tristate _explicitShareBiodadPass1;

		private readonly Tristate _shareBiomomPass1;
		private readonly Tristate _shareBiodadPass1;

		private float? _rExplicitOldestSibVersion = float.NaN;
		private float? _rExplicitYoungestSibVersion = float.NaN;
		private float? _rExplicitPass1 = float.NaN;//  float.NaN;
		private float? _rPass1 = float.NaN;//  float.NaN;
		#endregion
		#region IAssign Properties
		public Int32 IDLeft { get { return _idRelatedLeft; } }
		public Int32 IDRight { get { return _idRelatedRight; } }
		public MultipleBirth MultipleBirthIfSameSex { get { return _multipleBirth; } }
		public Tristate IsMZ { get { return _isMZ; } }
		//public Tristate IsRelatedInMzManual { get { return _isRelatedInMzManual; } }
		//public Int16 RosterAssignmentID { get { return _rosterAssignment; } }
		//public float? RRoster { get { return _rRoster; } }
		public Tristate ImplicitShareBiomomPass1 { get { return _implicitShareBiomomPass1; } }
		public Tristate ImplicitShareBiodadPass1 { get { return _implicitShareBiodadPass1; } }
		public Tristate ExplicitShareBiomomPass1 { get { return _explicitShareBiomomPass1; } }
		public Tristate ExplicitShareBiodadPass1 { get { return _explicitShareBiodadPass1; } }
		public Tristate ShareBiomomPass1 { get { return _shareBiomomPass1; } }
		public Tristate ShareBiodadPass1 { get { return _shareBiodadPass1; } }
		public float? RImplicitPass1 { get { return _rImplicitPass1; } }
		public float? RImplicit2004 { get { return _rImplicit2004; } }
		public float? RExplicitOldestSibVersion { get { return _rExplicitOldestSibVersion; } }
		public float? RExplicitYoungestSibVersion { get { return _rExplicitYoungestSibVersion; } }
		public float? RExplicitPass1 { get { return _rExplicitPass1; } }
		public float? RPass1 { get { return _rPass1; } }
		#endregion
		#region Constructor
		public RGen1Pass1 ( ImportDataSet dsImport, LinksDataSet dsLinks, LinksDataSet.tblRelatedStructureRow drLeft, LinksDataSet.tblRelatedStructureRow drRight ) {
			if ( dsImport == null ) throw new ArgumentNullException("dsImport");
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( drLeft == null ) throw new ArgumentNullException("drLeft");
			if ( drRight == null ) throw new ArgumentNullException("drRight");
			if ( dsImport.tblLinks2004Gen1.Count == 0 ) throw new InvalidOperationException("tblLinks2004Gen1 must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblMzManual.Count == 0 ) throw new InvalidOperationException("tblMzManual must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubject.Count == 0 ) throw new InvalidOperationException("tblSubject must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblRosterGen1.Count == 0 ) throw new InvalidOperationException("tblRosterGen1 must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblMarkerGen1.Count == 0 ) throw new InvalidOperationException("tblMarkerGen2 must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubjectDetails.Count == 0 ) throw new InvalidOperationException("tblSubjectDetails must NOT be empty before assigning R values from it.");

			_dsImport = dsImport;
			_dsLinks = dsLinks;
			_drLeft = drLeft;
			_drRight = drRight;
			_idRelatedLeft = _drLeft.ID;
			_idRelatedRight = _drRight.ID;
			_drSubjectDetails1 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S1);
			_drSubjectDetails2 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S2);
			_extendedID = _drLeft.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.ExtendedID;

			//LinksDataSet.tblRosterGen1Row drRoster = _dsLinks.tblRosterGen1.FindByRelatedID(drLeft.ID);
			//Tristate rosterShareBiomom = (Tristate)drRoster.ShareBiomom;
			//Tristate rosterShareBiodad = (Tristate)drRoster.ShareBiodad;
			//if ( drRoster.IsRNull() ) _rRoster = null;
			//else _rRoster = (float)drRoster.R;

			if ( _drSubjectDetails1.BirthOrderInNls <= _drSubjectDetails2.BirthOrderInNls ) {//This is the way it usually is.  Remember that twins were assigned tied birth orders
				_idRelatedOlderAboutYounger = _idRelatedLeft;
				_idRelatedYoungerAboutOlder = _idRelatedRight;
			}
			else if ( _drSubjectDetails1.BirthOrderInNls > _drSubjectDetails2.BirthOrderInNls ) {
				_idRelatedOlderAboutYounger = _idRelatedRight;
				_idRelatedYoungerAboutOlder = _idRelatedLeft;
			}

			_dtMarkersGen1 = MarkerGen1.PairRelevantMarkerRows(_idRelatedLeft, _idRelatedRight, _dsLinks, _extendedID);

			LinksDataSet.tblMzManualRow drMz = Retrieve.MzManualRecord(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _dsLinks);

			if ( drMz == null ) {
				_multipleBirth = MultipleBirth.No;
				_isMZ = Tristate.No;
				_isRelatedInMzManual = Tristate.DoNotKnow;
			}
			else {
				_multipleBirth = (MultipleBirth)drMz.MultipleBirthIfSameSex;
				_isMZ = (Tristate)drMz.IsMz;
				if ( drMz.IsRelatedNull() ) _isRelatedInMzManual = Tristate.DoNotKnow;
				else if ( drMz.Related ) _isRelatedInMzManual = Tristate.Yes;
				else _isRelatedInMzManual = Tristate.No;
			}

			MarkerEvidence explicitBiomomFromOlder = ReduceShareBioparentToOne(MarkerType.ShareBiomom, ItemYears.Gen1ShareBioparent.Length, _idRelatedOlderAboutYounger);
			MarkerEvidence explicitBiodadFromOlder = ReduceShareBioparentToOne(MarkerType.ShareBiodad, ItemYears.Gen1ShareBioparent.Length, _idRelatedOlderAboutYounger);
			MarkerEvidence explicitBiomomFromYounger = ReduceShareBioparentToOne(MarkerType.ShareBiomom, ItemYears.Gen1ShareBioparent.Length, _idRelatedYoungerAboutOlder);
			MarkerEvidence explicitBiodadFromYounger = ReduceShareBioparentToOne(MarkerType.ShareBiodad, ItemYears.Gen1ShareBioparent.Length, _idRelatedYoungerAboutOlder);

			MarkerEvidence biomomInHH1979 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomInHH, 1979, Bioparent.Mom, _dtMarkersGen1);
			MarkerEvidence biodadInHH1979 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadInHH, 1979, Bioparent.Dad, _dtMarkersGen1);

			MarkerEvidence biomomDeathAge = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomDeathAge, Bioparent.Mom, _dtMarkersGen1);
			MarkerEvidence biodadDeathAge = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadDeathAge, Bioparent.Dad, _dtMarkersGen1);

			_explicitShareBiomomPass1 = CommonFunctions.TranslateEvidenceToTristate(explicitBiomomFromOlder, explicitBiomomFromYounger);
			_explicitShareBiodadPass1 = CommonFunctions.TranslateEvidenceToTristate(explicitBiodadFromOlder, explicitBiodadFromYounger);

			_implicitShareBiomomPass1 = ImplicitShareBioparent(inHH1979: biomomInHH1979, deathAge: biomomDeathAge);
			_implicitShareBiodadPass1 = ImplicitShareBioparent(inHH1979: biodadInHH1979, deathAge: biodadDeathAge);

			_shareBiomomPass1 = CommonFunctions.TakePriority(_explicitShareBiomomPass1, _implicitShareBiomomPass1);
			_shareBiodadPass1 = CommonFunctions.TakePriority(_explicitShareBiodadPass1, _implicitShareBiodadPass1);

			_rExplicitOldestSibVersion = CalculateRExplicitSingleSibVersion(explicitBiomomFromOlder, explicitBiodadFromOlder);
			_rExplicitYoungestSibVersion = CalculateRExplicitSingleSibVersion(explicitBiomomFromYounger, explicitBiodadFromYounger);
			_rExplicitPass1 = CommonFunctions.TranslateToR(shareBiomom: _explicitShareBiomomPass1, shareBiodad: _explicitShareBiodadPass1, mustDecide: false);

			_rImplicitPass1 = CommonFunctions.TranslateToR(shareBiomom: _implicitShareBiomomPass1, shareBiodad: _implicitShareBiodadPass1, mustDecide: false);
			_rImplicit2004 = RetrieveRImplicit2004();

			_rPass1 = CalculateRFull(shareBiomom: _shareBiomomPass1, shareBiodad: _shareBiodadPass1,
				multiple: _multipleBirth, isMZ: _isMZ, isRelatedInMZManual: _isRelatedInMzManual, idRelated: _idRelatedLeft, dtRoster: _dsLinks.tblRosterGen1);
		}
		#endregion //#region Public Methods #endregion #region Private Methods #endregion
		#region Private Methods - Estimate R
		private Tristate ImplicitShareBioparent ( MarkerEvidence inHH1979, MarkerEvidence deathAge ) {
			if ( deathAge == MarkerEvidence.StronglySupports )
				return Tristate.Yes;
			else if ( inHH1979 == MarkerEvidence.StronglySupports )
				return Tristate.Yes;
			else if ( deathAge == MarkerEvidence.Disconfirms )
				return Tristate.No;
			else if ( inHH1979 == MarkerEvidence.Disconfirms )
				return Tristate.No;
			else 
				return Tristate.DoNotKnow;
		}
		private float? RetrieveRImplicit2004 ( ) {
			ImportDataSet.tblLinks2004Gen1Row drV1 = _dsImport.tblLinks2004Gen1.FindBySubjectTag_S1SubjectTag_S2(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
			ImportDataSet.tblLinks2004Gen1Row drV2 = _dsImport.tblLinks2004Gen1.FindBySubjectTag_S1SubjectTag_S2(_drLeft.SubjectTag_S2, _drLeft.SubjectTag_S1);
			if ( drV1 != null ) {
				if ( drV1.IsRecommendedRelatednessNull() ) return null;
				else return drV1.RecommendedRelatedness;
			}
			else if ( drV2 != null ) {
				if ( drV2.IsRecommendedRelatednessNull() ) return null;
				else return drV2.RecommendedRelatedness;
			}
			else {
				return null;//The record wasn't contained in the links created in 2004.
			}
		}
		public static float? CalculateRRoster ( Int32 idRelated, LinksDataSet.tblRosterGen1DataTable dtRoster ) {
			//TODO: Check overrides first.

			LinksDataSet.tblRosterGen1Row dr = dtRoster.FindByRelatedID(idRelated);
			Trace.Assert(dr != null, "Exactly one row should be retrieved from tblRosterGen1.");
			if ( dr.Resolved ) {
				Trace.Assert(!dr.IsRNull(), "If R is resolved by the roster, then R shouldn't be NaN.");
				return (float)dr.R;
			}
			else {
				return null;
			}
		}
		private float? CalculateRExplicitSingleSibVersion ( MarkerEvidence biomom, MarkerEvidence biodad ) {
			if ( biomom == MarkerEvidence.Missing || biodad == MarkerEvidence.Missing ) {
				return null;
			}
			else if ( biomom == MarkerEvidence.Supports && biodad == MarkerEvidence.Supports ) { //if ( !OverridesGen1.RosterAndExplicit.Contains(subjectTag) ) { //   Trace.Assert(roster.ShareBiomom != MarkerEvidence.Disconfirms); //   Trace.Assert(roster.ShareBiodad != MarkerEvidence.Disconfirms);//}
				return RCoefficients.SiblingFull;
			}
			else if ( biomom == MarkerEvidence.Disconfirms && biodad == MarkerEvidence.Supports ) {//Trace.Assert(roster.ShareBiomom != MarkerEvidence.Disconfirms); //Trace.Assert(roster.ShareBiodad != MarkerEvidence.Disconfirms);
				return RCoefficients.SiblingHalf;
			}
			else if ( biomom == MarkerEvidence.Supports && biodad == MarkerEvidence.Disconfirms ) { //if ( !OverridesGen1.RosterAndExplicit.Contains(subjectTag) ) { //   Trace.Assert(roster.ShareBiomom != MarkerEvidence.Disconfirms);//   Trace.Assert(roster.ShareBiodad != MarkerEvidence.Disconfirms); //}
				return RCoefficients.SiblingHalf;
			}
			//else if ( biomom.ShareBiomom == MarkerEvidence.Disconfirms && biodad.ShareBiodad == MarkerEvidence.Disconfirms ) {
			//   return RCoefficients.NotRelated;//The could still be cousins or something else
			//}
			else {
				return null; //The could still be cousins or something else
			}
		}
		public static float? CalculateRFull ( Tristate shareBiomom, Tristate shareBiodad, MultipleBirth multiple, Tristate isMZ, Tristate isRelatedInMZManual, Int32 idRelated, LinksDataSet.tblRosterGen1DataTable dtRoster ) {
			float? rRoster = CalculateRRoster(idRelated, dtRoster);

			if ( isMZ == BaseAssembly.Tristate.Yes ) {
				return RCoefficients.MzTrue;
			}
			else if ( isRelatedInMZManual == Tristate.No ) {
				return RCoefficients.NotRelated; //Of the 21 Gen1 subjects in tblMZManual with Related=0, 17 ended up with R=0 (as of 11/9/2012).  1 was assigned R=.5; 3 were assigned R=NULL (which I want to override now here, looking at the DOB differences).
			}
			else if ( isMZ == BaseAssembly.Tristate.DoNotKnow && isRelatedInMZManual == Tristate.Yes ) {
				Trace.Assert(multiple == MultipleBirth.Twin || multiple == MultipleBirth.Trip || multiple == MultipleBirth.TwinOrTrip, "To be assigned full sib, they've got to be assigned to be a twin/trip.");
				return RCoefficients.MzAmbiguous;
			}
			else if ( multiple == MultipleBirth.Twin || multiple == MultipleBirth.Trip || multiple == MultipleBirth.TwinOrTrip ) {
				return RCoefficients.SiblingFull;
			}
			else if ( rRoster.HasValue ) {
				return rRoster;
			}
			else {
				//The implicits & explicits were already combined to get the values of shareBiomom & shareBiodad.
				return CommonFunctions.TranslateToR(shareBiomom: shareBiomom, shareBiodad: shareBiodad, mustDecide: false);
			}
		}
		private MarkerEvidence ReduceShareBioparentToOne ( MarkerType markerType, Int32 maxMarkerCount, Int32 idRelated ) {
			MarkerGen1Summary[] summaries = MarkerGen1.RetrieveMarkers(idRelated, markerType, _dtMarkersGen1, maxMarkerCount);
			if ( summaries.Length <= 0 )
				return MarkerEvidence.Missing;

			IEnumerable<MarkerEvidence> evidences;
			if ( markerType == MarkerType.ShareBiodad )
				evidences = from summary in summaries select summary.ShareBiodad;
			else if ( markerType == MarkerType.ShareBiomom )
				evidences = from summary in summaries select summary.ShareBiomom;
			else
				throw new ArgumentOutOfRangeException("markerType", markerType, "The 'ReduceShareBiodadToOne' function does not accommodoate this markerType.");


			if ( evidences.All(evidence => evidence == MarkerEvidence.Supports) ) {
				return MarkerEvidence.Supports;
			}
			else if ( evidences.All(evidence => evidence == MarkerEvidence.Disconfirms) ) {
				return MarkerEvidence.Disconfirms;
			}
			else if ( evidences.All(evidence => evidence == MarkerEvidence.Ambiguous) ) {
				return MarkerEvidence.Ambiguous;
			}
			else if ( evidences.Any(evidence => evidence == MarkerEvidence.Irrelevant) ) {
				throw new NotImplementedException("This function was not designed to accept this evidence value.");
			}
			else if ( evidences.Any(evidence => evidence == MarkerEvidence.Consistent) ) {
				throw new NotImplementedException("This function was not designed to accept this evidence value.");
			}
			else if ( evidences.Any(evidence => evidence == MarkerEvidence.Unlikely) ) {
				throw new NotImplementedException("This function was not designed to accept this evidence value.");
			}
			else {
				return MarkerEvidence.Ambiguous;
			}
		}
		#endregion
	}
}
//private float? CalculateRImplicitPass1 ( MarkerEvidence biomomDeathAge ) {
//   throw new NotImplementedException();
//}
//private float? CalculateRExplicitPass1 ( ) {
//   if ( !RExplicitOldestSibVersion.HasValue && !RExplicitYoungestSibVersion.HasValue )
//      return null;
//   else if ( !RExplicitOldestSibVersion.HasValue )
//      return RExplicitYoungestSibVersion.Value;
//   else if ( !RExplicitYoungestSibVersion.HasValue )
//      return RExplicitOldestSibVersion.Value;
//   else if ( RExplicitOldestSibVersion.Value == RExplicitYoungestSibVersion.Value )
//      return RExplicitOldestSibVersion.Value;
//   else if ( RExplicitOldestSibVersion.Value == RCoefficients.SiblingAmbiguous )
//      return RExplicitYoungestSibVersion.Value;
//   else if ( RExplicitYoungestSibVersion.Value == RCoefficients.SiblingAmbiguous )
//      return RExplicitOldestSibVersion.Value;
//   else if ( RExplicitOldestSibVersion.Value == RCoefficients.SiblingFull && RExplicitYoungestSibVersion.Value == RCoefficients.SiblingHalf )
//      return RCoefficients.SiblingAmbiguous;
//   else if ( RExplicitYoungestSibVersion.Value == RCoefficients.SiblingFull && RExplicitOldestSibVersion.Value == RCoefficients.SiblingHalf )
//      return RCoefficients.SiblingAmbiguous;
//   else
//      throw new InvalidOperationException("All condition should have been caught.");
//}
