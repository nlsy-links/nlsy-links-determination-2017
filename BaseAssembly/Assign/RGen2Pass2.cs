using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nls.BaseAssembly;

namespace Nls.BaseAssembly.Assign {
	public class RGen2Pass2 : IAssignPass2 {
		#region Fields
		private readonly LinksDataSet _dsLinks;
		private readonly LinksDataSet.tblRelatedStructureRow _drLeft;
		private readonly LinksDataSet.tblRelatedStructureRow _drRight;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails1;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails2;
		private readonly LinksDataSet.tblMarkerGen2DataTable _dtMarkersGen2;
		private readonly LinksDataSet.tblRelatedValuesRow _drValue;

		private readonly Int32 _idRelatedLeft = Int32.MinValue;
		private readonly Int32 _idRelatedRight = Int32.MinValue;
		private readonly Int32 _idRelatedOlderAboutYounger = Int32.MinValue;//usually equal to _idRelatedLeft

		private readonly Int32 _extendedID;
		private float? _rImplicit = float.NaN;
		private float? _rImplicitSubject = float.NaN;
		private float? _rImplicitMother = float.NaN;
		private float? _rExplicit = float.NaN;
		private float? _rFull = float.NaN;
		private float? _rPeek = float.NaN;
		#endregion
		#region IAssign Properties
		public Int32 IDLeft { get { return _idRelatedLeft; } }
		public float? RImplicit { get { return _rImplicit; } }
		public float? RImplicitSubject { get { return _rImplicitSubject; } }
		public float? RImplicitMother { get { return _rImplicitMother; } }
		public float? RExplicit { get { return _rExplicit; } }
		public float? R{ get { return _rFull; } }
		public float? RFull { get { return _rFull; } }
		public float? RPeek { get { return _rPeek; } }
		#endregion
		#region Constructor
		public RGen2Pass2 ( LinksDataSet dsLinks, LinksDataSet.tblRelatedStructureRow drLeft, LinksDataSet.tblRelatedStructureRow drRight ) {//ImportDataSet dsImport,
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( drLeft == null ) throw new ArgumentNullException("drLeft");
			if ( drRight == null ) throw new ArgumentNullException("drRight");
			if ( dsLinks.tblRelatedValues.Count == 0 ) throw new InvalidOperationException("tblRelatedValues must be empty before updating rows for it.");
			if ( dsLinks.tblMzManual.Count == 0 ) throw new InvalidOperationException("tblMzManual must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubject.Count == 0 ) throw new InvalidOperationException("tblSubject must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubjectDetails.Count == 0 ) throw new InvalidOperationException("tblSubjectDetails must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblMarkerGen2.Count == 0 ) throw new InvalidOperationException("tblMarkerGen2 must NOT be empty before assigning R values from it.");
			_dsLinks = dsLinks;
			_drLeft = drLeft;
			_drRight = drRight;
			_idRelatedLeft = _drLeft.ID;
			_idRelatedRight = _drRight.ID;
			_drSubjectDetails1 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S1);
			_drSubjectDetails2 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S2);
			_extendedID = _drLeft.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.ExtendedID;

			if ( _drSubjectDetails1.BirthOrderInNls <= _drSubjectDetails2.BirthOrderInNls ) {//This is the way it usually is.  Recall twins were assigned tied birth orders
				_idRelatedOlderAboutYounger = _idRelatedLeft;
			}
			else if ( _drSubjectDetails1.BirthOrderInNls > _drSubjectDetails2.BirthOrderInNls ) {
				_idRelatedOlderAboutYounger = _idRelatedRight;
			}

			_drValue = _dsLinks.tblRelatedValues.FindByID(_idRelatedLeft);
			_dtMarkersGen2 = MarkerGen2.PairRelevantMarkerRows(_idRelatedLeft, _idRelatedRight, _dsLinks, _extendedID);

			MarkerEvidence babyDaddyDeathDate = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyDeathDate, _dtMarkersGen2);
			MarkerEvidence babyDaddyAlive = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyAlive, _dtMarkersGen2);
			MarkerEvidence babyDaddyInHH = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyInHH, _dtMarkersGen2);
			MarkerEvidence babyDaddyLeftHHDate = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyLeftHHDate, _dtMarkersGen2);
			MarkerEvidence babyDaddyDistanceFromHH = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyDistanceFromHH, _dtMarkersGen2);

			MarkerEvidence fatherAlive = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.Gen2CFatherAlive, _dtMarkersGen2);
			MarkerEvidence fatherInHH = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.Gen2CFatherInHH, _dtMarkersGen2);
			MarkerEvidence fatherDistanceFromHH = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.Gen2CFatherDistanceFromHH, _dtMarkersGen2);

			_rImplicitMother = CalculateRImplicitMother(babyDaddyDeathDate, babyDaddyAlive, babyDaddyInHH, babyDaddyLeftHHDate, babyDaddyDistanceFromHH);
			_rImplicitSubject = CalculateRImplicitSubject(fatherAlive, fatherInHH, fatherDistanceFromHH);
			_rImplicit = CalculateRImplicit(_rImplicitMother, _rImplicitSubject);
			_rExplicit = CalculateRExplicit();
			_rFull = CalculateRFull();
			_rPeek = CalculateRPeek();
		}
		#endregion
		#region Public Methods
		#endregion
		#region Private Methods - Estimate R
		private float? CalculateRImplicitMother ( MarkerEvidence babyDaddyDeathDate, MarkerEvidence babyDaddyAlive, MarkerEvidence babyDaddyInHH, MarkerEvidence babyDaddyLeftHHDate, MarkerEvidence babyDaddyDistanceFromHH ) {
			if ( !_drValue.IsRImplicitPass1Null() ) return (float?)_drValue.RImplicitPass1;
			DataColumn dcPass1 = _dsLinks.tblRelatedValues.RImplicitPass1Column;
			PairR[] pairs = PairR.BuildRelatedPairsOfGen2Sibs(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateR interpolate = new InterpolateR(pairs);
			float? newRImplicit = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
			if ( newRImplicit.HasValue ) {
				return newRImplicit;
			}
			else {
				if ( babyDaddyDeathDate == MarkerEvidence.Supports ) return RCoefficients.SiblingFull;
				else if ( babyDaddyDeathDate == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
				else if ( babyDaddyInHH == MarkerEvidence.Supports ) return RCoefficients.SiblingFull;
				else if ( babyDaddyInHH == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
				else if ( babyDaddyLeftHHDate == MarkerEvidence.Supports ) return RCoefficients.SiblingFull;
				else if ( babyDaddyLeftHHDate == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
				else if ( babyDaddyAlive == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
				else if ( babyDaddyDistanceFromHH == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
				else if ( HasLargeMetaphoricalDistance(pairs) ) return RCoefficients.SiblingHalf;
				else return null;
			}
		}
		private float? CalculateRImplicitSubject ( MarkerEvidence fatherAlive, MarkerEvidence fatherInHH, MarkerEvidence fatherDistanceFromHH ) {
			//else if ( babyDaddyDeathDate == MarkerEvidence.StronglySupports ) return RCoefficientsSibling.SiblingFull;
			//else if ( babyDaddyDeathDate == MarkerEvidence.Disconfirms ) return RCoefficientsSibling.SiblingHalf;
			if ( fatherAlive == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			//else if ( babyDaddyLeftHHDate == MarkerEvidence.StronglySupports ) return RCoefficientsSibling.SiblingFull;
			//else if ( babyDaddyLeftHHDate == MarkerEvidence.Disconfirms ) return RCoefficientsSibling.SiblingHalf;
			else if ( fatherInHH == MarkerEvidence.StronglySupports ) return RCoefficients.SiblingFull;
			else if ( fatherInHH == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			else if ( fatherDistanceFromHH == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			//else if ( babyDaddyAsthma == MarkerEvidence.Disconfirms ) return RCoefficientsSibling.SiblingHalf;


			//if ( babyDaddyDeathDate == MarkerEvidence.Supports ) return RCoefficientsSibling.SiblingFull;
			//else if ( babyDaddyDeathDate == MarkerEvidence.Unlikely ) return RCoefficientsSibling.SiblingHalf;
			else if ( fatherInHH == MarkerEvidence.Supports ) return RCoefficients.SiblingFull;
			else if ( fatherInHH == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
			//else if ( babyDaddyLeftHHDate == MarkerEvidence.Supports ) return RCoefficientsSibling.SiblingFull;
			//else if ( babyDaddyLeftHHDate == MarkerEvidence.Unlikely ) return RCoefficientsSibling.SiblingHalf;
			else if ( fatherAlive == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
			else if ( fatherDistanceFromHH == MarkerEvidence.Unlikely ) return RCoefficients.SiblingHalf;
			//else if ( HasLargeMetaphoricalDistance(pairs) ) return RCoefficientsSibling.SiblingHalf;
			else return null;
		}
		private static float? CalculateRImplicit ( float? rImplicitMother, float? rImplicitSubject ) {
			if ( rImplicitMother.HasValue )
				return rImplicitMother;
			else
				return rImplicitSubject;//This could still be null;
		}
		private bool HasLargeMetaphoricalDistance ( PairR[] pairs ) {
			if ( _drSubjectDetails1.IsMobNull() || _drSubjectDetails2.IsMobNull() )
				return false;
			double mobDifferenceInYears = Math.Abs(_drSubjectDetails1.Mob.Subtract(_drSubjectDetails2.Mob).TotalDays / Constants.DaysPerYear);
			if ( mobDifferenceInYears > 8 && PairR.CountHalfSibs(pairs) >= 2 )
				return true;
			else if ( mobDifferenceInYears > 5 && PairR.CountHalfSibs(pairs) >= 3 )
				return true;
			else
				return false;
		}
		private float? CalculateRExplicit ( ) {
			if ( !_drValue.IsRExplicitPass1Null() ) return (float?)_drValue.RExplicitPass1;
			DataColumn dcPass1 = _dsLinks.tblRelatedValues.RExplicitPass1Column;
			PairR[] pairs = PairR.BuildRelatedPairsOfGen2Sibs(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateR interpolate = new InterpolateR(pairs);
			float? newRExplicit = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
			return RGen2Pass1.ExplicitAgreementWithTwins((Tristate)_drValue.IsMz, (MultipleBirth)_drValue.MultipleBirthIfSameSex, newRExplicit);
		}
		private float? CalculateRFull ( ) {
			if ( !_drValue.IsRPass1Null() ) return (float?)_drValue.RPass1;
			DataColumn dcPass1 = _dsLinks.tblRelatedValues.RPass1Column;
			PairR[] pairs = PairR.BuildRelatedPairsOfGen2Sibs(dcPass1, _drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2, _drLeft.ExtendedID, _dsLinks);

			InterpolateR interpolate = new InterpolateR(pairs);
			float? newR = interpolate.Interpolate(_drLeft.SubjectTag_S1, _drLeft.SubjectTag_S2);
			if ( newR.HasValue )
				return newR;
			else if ( _rImplicit.HasValue )
				return _rImplicit;
			else
				return RCoefficients.SiblingAmbiguous;
		}
		private static float? CalculateRPeek ( ) {
			return null;
		}
		#endregion
	}
}
