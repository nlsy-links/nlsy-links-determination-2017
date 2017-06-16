using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nls.BaseAssembly;

namespace Nls.BaseAssembly.Assign {
	public class RGen1Pass2 : IAssignPass2 {
		#region Fields
		private readonly LinksDataSet _dsLinks;
		private readonly LinksDataSet.tblRelatedStructureRow _drLeft;
		private readonly LinksDataSet.tblRelatedStructureRow _drRight;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails1;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails2;
		private readonly LinksDataSet.tblMarkerGen1DataTable _dtMarkersGen1;
		private readonly LinksDataSet.tblRelatedValuesRow _drValue;

		private readonly Int32 _idRelatedLeft = Int32.MinValue;
		private readonly Int32 _idRelatedRight = Int32.MinValue;
		private readonly Int32 _idRelatedOlderAboutYounger = Int32.MinValue;//usually equal to _idRelatedLeft

		//private readonly Tristate _implicitShareBiomom;
		//private readonly Tristate _implicitShareBiodad;
		//private readonly Tristate _explicitShareBiomom;
		//private readonly Tristate _explicitShareBiodad;
		//private readonly Tristate _shareBiomom;
		//private readonly Tristate _shareBiodad;

		private readonly Int32 _extendedID;
		private float? _rImplicit = null;// float.NaN;
		private float? _rImplicitSubject = null;//float.NaN;
		private float? _rExplicit = float.NaN;//float.NaN;
		private float? _r = float.NaN;
		private float? _rFull = float.NaN;
		private float? _rPeek = null;//float.NaN;
		#endregion
		#region IAssign Properties
		public Int32 IDLeft { get { return _idRelatedLeft; } }
		public Int32 IDRight { get { return _idRelatedRight; } }
		public float? RImplicit { get { return _rImplicit; } }
		public float? RImplicitMother { get { return null; } }
		public float? RImplicitSubject { get { return _rImplicitSubject; } }
		public float? RExplicit { get { return _rExplicit; } }
		public float? R { get { return _r; } }
		public float? RFull { get { return _rFull; } }
		public float? RPeek { get { return _rPeek; } }
		#endregion
		#region Constructor
		public RGen1Pass2 ( LinksDataSet dsLinks, LinksDataSet.tblRelatedStructureRow drLeft, LinksDataSet.tblRelatedStructureRow drRight ) {
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( drLeft == null ) throw new ArgumentNullException("drLeft");
			if ( drRight == null ) throw new ArgumentNullException("drRight");
			if ( dsLinks.tblRelatedValues.Count == 0 ) throw new InvalidOperationException("tblRelatedValues must be empty before updating rows for it.");
			if ( dsLinks.tblMzManual.Count == 0 ) throw new InvalidOperationException("tblMzManual must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubject.Count == 0 ) throw new InvalidOperationException("tblSubject must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubjectDetails.Count == 0 ) throw new InvalidOperationException("tblSubjectDetails must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblMarkerGen1.Count == 0 ) throw new InvalidOperationException("tblMarkerGen1 must NOT be empty before assigning R values from it.");
			_dsLinks = dsLinks;
			_drLeft = drLeft;
			_drRight = drRight;
			_idRelatedLeft = _drLeft.ID;
			_idRelatedRight = _drRight.ID;
			_drSubjectDetails1 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S1);
			_drSubjectDetails2 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S2);
			_extendedID = _drLeft.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.ExtendedID;

			if ( _drSubjectDetails1.BirthOrderInNls <= _drSubjectDetails2.BirthOrderInNls ) {//This is the way it usually is.  Recall twins were assigned tied birth orders
				_idRelatedOlderAboutYounger = _idRelatedLeft; //_idRelatedYoungerAboutOlder = _idRelatedRight;
			}
			else if ( _drSubjectDetails1.BirthOrderInNls > _drSubjectDetails2.BirthOrderInNls ) {
				_idRelatedOlderAboutYounger = _idRelatedRight;
			}

			_drValue = _dsLinks.tblRelatedValues.FindByID(_idRelatedLeft);
			_dtMarkersGen1 = MarkerGen1.PairRelevantMarkerRows(_idRelatedLeft, _idRelatedRight, _dsLinks, _extendedID);

			Tristate explicitShareBiomom = AddressExplicitBiomom();
			Tristate explicitShareBiodad = AddressExplicitBiodad();

			Tristate implicitShareBiomom = AddressImplicitBiomom();
			Tristate implicitShareBiodad = AddressImplicitBiodad();

			_rExplicit = CommonFunctions.TranslateToR(shareBiomom: explicitShareBiomom, shareBiodad: explicitShareBiodad, mustDecide: true);
			_rImplicit = CommonFunctions.TranslateToR(shareBiomom: implicitShareBiomom, shareBiodad: implicitShareBiodad, mustDecide: false);//Possibly 'true' later one

			//_rFull = RGen1Pass1.CalculateRFull(shareBiomom: shareBiomom, shareBiodad: shareBiodad,
			//   multiple: (MultipleBirth)_drValue.MultipleBirthIfSameSex, isMZ: (Tristate)_drValue.IsMz, isRelatedInMZManual: (Tristate)_drValue.IsRelatedInMzManual,
			//   idRelated: _idRelatedLeft, dtRoster: _dsLinks.tblRosterGen1);

			if ( !_drValue.IsRPass1Null() ) {
				_rFull = (float)_drValue.RPass1;
			}
			else { //Don't do the expensive interpolation unless it will be used
				Tristate shareBiomom = AddressBiomom(explicitShareBiomom, implicitShareBiomom);
				Tristate shareBiodad = AddressBiodad(explicitShareBiodad, implicitShareBiodad);
				_rFull = CommonFunctions.TranslateToR(shareBiomom: shareBiomom, shareBiodad: shareBiodad, mustDecide: true);
			}

            LinksDataSet.tblRosterGen1Row drRoster = _dsLinks.tblRosterGen1.FindByRelatedID(_drValue.ID);
            _r = CalculateR(_rFull, (Tristate)drRoster.SameGeneration);
		}
		#endregion
		#region Public Methods
		#endregion
		#region Private Methods - Estimate R
		private Tristate AddressImplicitBiomom ( ) {
			Tristate implicitBiomomPass1 = (Tristate)_drValue.ImplicitShareBiomomPass1;
			if ( implicitBiomomPass1 != Tristate.DoNotKnow ) return implicitBiomomPass1;
			const Bioparent bioparent = Bioparent.Mom;

			DataColumn dcPass1 = _dsLinks.tblRelatedValues.ImplicitShareBiomomPass1Column;
			PairShare[] pairs = PairShare.BuildRelatedPairsOfGen1Housemates(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateShare interpolate = new InterpolateShare(pairs);
			Tristate newShare = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);

			if ( newShare != Tristate.DoNotKnow ) {
				return newShare;
			}
			else {
                MarkerEvidence alwaysWithBothBioparents = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1AlwaysLivedWithBothBioparents, bioparent, _dtMarkersGen1);
                MarkerEvidence inHH1980 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomInHH, 1980, bioparent, _dtMarkersGen1);
				MarkerEvidence inHH1978 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomInHH, 1978, bioparent, _dtMarkersGen1);
				MarkerEvidence inHH1977 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomInHH, 1977, bioparent, _dtMarkersGen1);
				MarkerEvidence inHH1976 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomInHH, 1976, bioparent, _dtMarkersGen1);
				MarkerEvidence birthCountry = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomBirthCountry, bioparent, _dtMarkersGen1);
				MarkerEvidence birthState = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomBirthState, bioparent, _dtMarkersGen1);
				MarkerEvidence birthYearAskedIn1988 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomBirthYear, 1988, bioparent, _dtMarkersGen1);
				MarkerEvidence birthYearAskedIn1987 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiomomBirthYear, 1987, bioparent, _dtMarkersGen1);
                return ImplicitShareBioparent(alwaysWithBothBioparents, inHH1980, inHH1978, inHH1977, inHH1976, birthCountry, birthState, birthYearAskedIn1988, birthYearAskedIn1987);
			}
		}
		private Tristate AddressImplicitBiodad ( ) {
			Tristate implicitBiodadPass1 = (Tristate)_drValue.ImplicitShareBiodadPass1;
			if ( implicitBiodadPass1 != Tristate.DoNotKnow ) return implicitBiodadPass1;
			const Bioparent bioparent = Bioparent.Dad;

			DataColumn dcPass1 = _dsLinks.tblRelatedValues.ImplicitShareBiodadPass1Column;
			PairShare[] pairs = PairShare.BuildRelatedPairsOfGen1Housemates(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateShare interpolate = new InterpolateShare(pairs);
			Tristate newShare = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);

			if ( newShare != Tristate.DoNotKnow ) {
				return newShare;
			}
			else {
                MarkerEvidence alwaysWithBothBioparents = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1AlwaysLivedWithBothBioparents, bioparent, _dtMarkersGen1);
                MarkerEvidence inHH1980 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadInHH, 1980, bioparent, _dtMarkersGen1);
				MarkerEvidence inHH1978 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadInHH, 1978, bioparent, _dtMarkersGen1);
				MarkerEvidence inHH1977 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadInHH, 1977, bioparent, _dtMarkersGen1);
				MarkerEvidence inHH1976 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadInHH, 1976, bioparent, _dtMarkersGen1);
				MarkerEvidence birthCountry = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadBirthCountry, bioparent, _dtMarkersGen1);
				MarkerEvidence birthState = MarkerGen1.RetrieveParentMarkerSingleYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadBirthState, bioparent, _dtMarkersGen1);
				MarkerEvidence birthYearAskedIn1988 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadBirthYear, 1988, bioparent, _dtMarkersGen1);
				MarkerEvidence birthYearAskedIn1987 = MarkerGen1.RetrieveParentMarkerMultiYear(_idRelatedOlderAboutYounger, MarkerType.Gen1BiodadBirthYear, 1987, bioparent, _dtMarkersGen1);
                return ImplicitShareBioparent(alwaysWithBothBioparents, inHH1980, inHH1978, inHH1977, inHH1976, birthCountry, birthState, birthYearAskedIn1988, birthYearAskedIn1987);
			}
		}
		private Tristate AddressExplicitBiomom ( ) {
			Tristate explicitBiomomPass1 = (Tristate)_drValue.ExplicitShareBiomomPass1;
			if ( explicitBiomomPass1 != Tristate.DoNotKnow ) return explicitBiomomPass1;

			DataColumn dcPass1 = _dsLinks.tblRelatedValues.ExplicitShareBiomomPass1Column;
			PairShare[] pairs = PairShare.BuildRelatedPairsOfGen1Housemates(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateShare interpolate = new InterpolateShare(pairs);
			return interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
		}
		private Tristate AddressExplicitBiodad ( ) {
			Tristate explicitBiodadPass1 = (Tristate)_drValue.ExplicitShareBiodadPass1;
			if ( explicitBiodadPass1 != Tristate.DoNotKnow ) return explicitBiodadPass1;

			DataColumn dcPass1 = _dsLinks.tblRelatedValues.ExplicitShareBiodadPass1Column;
			PairShare[] pairs = PairShare.BuildRelatedPairsOfGen1Housemates(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateShare interpolate = new InterpolateShare(pairs);
			return interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
		}
		private Tristate AddressBiomom ( Tristate explicitShare, Tristate implicitShare ) {
			Tristate biomomPass1 = (Tristate)_drValue.ShareBiomomPass1;
			if ( biomomPass1 != Tristate.DoNotKnow ) return biomomPass1;

			DataColumn dcPass1 = _dsLinks.tblRelatedValues.ShareBiomomPass1Column;
			PairShare[] pairs = PairShare.BuildRelatedPairsOfGen1Housemates(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateShare interpolate = new InterpolateShare(pairs);
			Tristate newShare = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
			if ( newShare != Tristate.DoNotKnow )
				return newShare;
			else
				return CommonFunctions.TakePriority(explicitShare, implicitShare);
		}
		private Tristate AddressBiodad ( Tristate explicitShare, Tristate implicitShare ) {
			Tristate biodadPass1 = (Tristate)_drValue.ShareBiodadPass1;
			if ( biodadPass1 != Tristate.DoNotKnow ) return biodadPass1;

			DataColumn dcPass1 = _dsLinks.tblRelatedValues.ShareBiodadPass1Column;
			PairShare[] pairs = PairShare.BuildRelatedPairsOfGen1Housemates(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateShare interpolate = new InterpolateShare(pairs);
			Tristate newShare = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
			if ( newShare != Tristate.DoNotKnow )
				return newShare;
			else
				return CommonFunctions.TakePriority(explicitShare, implicitShare);
		}
        private Tristate ImplicitShareBioparent( MarkerEvidence alwaysWithBothBioparents, MarkerEvidence inHH1980, MarkerEvidence inHH1978, MarkerEvidence inHH1977, MarkerEvidence inHH1976, MarkerEvidence birthCountry, MarkerEvidence birthState,
            MarkerEvidence birthYearAskedIn1988, MarkerEvidence birthYearAskedIn1987 ) {

            if( inHH1980 == MarkerEvidence.StronglySupports )
                return Tristate.Yes;
            else if( inHH1980 == MarkerEvidence.Disconfirms )
                return Tristate.No;
            else if( inHH1978 == MarkerEvidence.StronglySupports )
                return Tristate.Yes;
            else if( inHH1978 == MarkerEvidence.Disconfirms )
                return Tristate.No;
            else if( inHH1977 == MarkerEvidence.StronglySupports )
                return Tristate.Yes;
            else if( inHH1977 == MarkerEvidence.Disconfirms )
                return Tristate.No;
            else if( inHH1976 == MarkerEvidence.StronglySupports )
                return Tristate.Yes;
            else if( inHH1976 == MarkerEvidence.Disconfirms )
                return Tristate.No;
            else if( birthCountry == MarkerEvidence.Disconfirms )
                return Tristate.No;
            else if( birthState == MarkerEvidence.Disconfirms )
                return Tristate.No;
            else if( alwaysWithBothBioparents == MarkerEvidence.StronglySupports )
                return Tristate.Yes;
            else if( birthYearAskedIn1988 == MarkerEvidence.Disconfirms )
                return Tristate.No;
            //else if ( birthYearAskedIn1988 == MarkerEvidence.Supports )
            //   return Tristate.Yes;
            else if( birthYearAskedIn1987 == MarkerEvidence.Disconfirms )
                return Tristate.No;
            //else if ( birthYearAskedIn1987 == MarkerEvidence.Supports )
            //   return Tristate.Yes;
            //else if ( birthYearAskedIn1988 == MarkerEvidence.Unlikely )
            //   return Tristate.No;
            //else if ( birthYearAskedIn1987 == MarkerEvidence.Unlikely )
            //   return Tristate.No;
            else 
                return Tristate.DoNotKnow;
        }
        private static float? CalculateR( float? rFull, Tristate sameGeneration ) {
            if( !rFull.HasValue )
                return null;
            else if( Constants.Gen1RsToExcludeFromR.Contains(rFull.Value) )
                return null;
            else if( sameGeneration == Tristate.No )
                return null;
            else if( sameGeneration == Tristate.DoNotKnow )
                return null;
            else
                return (float)rFull.Value;
        }
		private static float? CalculateRPeek ( ) {
			return null;
		}
		#endregion
	}
}
//private float? CalculateRExplicit ( ) {//Int32 idRelated
//   if ( !_drValue.IsRExplicitPass1Null() ) return (float?)_drValue.RExplicitPass1;
//   DataColumn dcPass1 = _dsLinks.tblRelatedValues.RExplicitPass1Column;
//   PairR[] pairs = PairR.BuildRelatedPairsOfGen1Housemates(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

//   //InterpolateBioparent interpolate = new InterpolateBioparent(pairs);
//   //float? newRExplicit = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
//   //if ( newRExplicit.HasValue ) 
//   //   return newRExplicit;
//   //else 
//      return null;
//}
