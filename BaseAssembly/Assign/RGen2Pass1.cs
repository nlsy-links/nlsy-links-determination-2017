using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nls.BaseAssembly;

namespace Nls.BaseAssembly.Assign {
	public class RGen2Pass1 : IAssignPass1 {
		#region Fields
		private readonly ImportDataSet _dsImport;
		private readonly LinksDataSet _dsLinks;
		private readonly LinksDataSet.tblRelatedStructureRow _drLeft;
		private readonly LinksDataSet.tblRelatedStructureRow _drRight;
		private readonly LinksDataSet.tblSubjectRow _drBare1;
		private readonly LinksDataSet.tblSubjectRow _drBare2;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails1;
		private readonly LinksDataSet.tblSubjectDetailsRow _drSubjectDetails2;
		private readonly LinksDataSet.tblMarkerGen2DataTable _dtMarkersGen2;

		private readonly Int32 _idRelatedLeft = Int32.MinValue;
		private readonly Int32 _idRelatedRight = Int32.MinValue;
		private readonly Int32 _idRelatedOlderAboutYounger = Int32.MinValue;//usually equal to _idRelatedLeft
		private readonly Int32 _idRelatedYoungerAboutOlder = Int32.MinValue;//usually equal to _idRelatedRight

		private readonly Int32 _extendedID;
		private readonly MultipleBirth _multipleBirth;
		private readonly Tristate _isMZ;
		private float? _rImplicitPass1 = float.NaN;
		//private float? _rImplicit = float.NaN;
		private float? _rImplicit2004 = float.NaN;
		private float? _rExplicitOldestSibVersion = float.NaN;
		private float? _rExplicitYoungestSibVersion = float.NaN;
		private float? _rExplicitPass1 = float.NaN;
		//private float? _rExplicit = float.NaN;
		private float? _rPass1 = float.NaN;
		//private float? _r = float.NaN;
		//private float? _rPeek = float.NaN;
		#endregion
		#region IAssign Properties
		public Int32 IDLeft { get { return _idRelatedLeft; } }
		public Int32 IDRight { get { return _idRelatedRight; } }
		public MultipleBirth MultipleBirthIfSameSex { get { return _multipleBirth; } }
		public Tristate IsMZ { get { return _isMZ; } }
		//public Tristate IsRelatedInMzManual { get { return Tristate.DoNotKnow; } }
		public Tristate ImplicitShareBiomomPass1 { get { return Tristate.DoNotKnow; } }
		public Tristate ImplicitShareBiodadPass1 { get { return Tristate.DoNotKnow; } }
		public Tristate ExplicitShareBiomomPass1 { get { return Tristate.DoNotKnow; } }
		public Tristate ExplicitShareBiodadPass1 { get { return Tristate.DoNotKnow; } }
		public Tristate ShareBiomomPass1 { get { return Tristate.DoNotKnow; } }
		public Tristate ShareBiodadPass1 { get { return Tristate.DoNotKnow; } }
		public float? RImplicitPass1 { get { return _rImplicitPass1; } }
		public float? RImplicit2004 { get { return _rImplicit2004; } }
		public float? RExplicitOldestSibVersion { get { return _rExplicitOldestSibVersion; } }
		public float? RExplicitYoungestSibVersion { get { return _rExplicitYoungestSibVersion; } }
		public float? RExplicitPass1 { get { return _rExplicitPass1; } }
		public float? RPass1 { get { return _rPass1; } }
		#endregion
		#region Constructor
		public RGen2Pass1 ( ImportDataSet dsImport, LinksDataSet dsLinks, LinksDataSet.tblRelatedStructureRow drLeft, LinksDataSet.tblRelatedStructureRow drRight ) {
			if ( dsImport == null ) throw new ArgumentNullException("dsImport");
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( drLeft == null ) throw new ArgumentNullException("drLeft");
			if ( drRight == null ) throw new ArgumentNullException("drRight");
			if ( dsImport.tblLinks2004Gen2.Count == 0 ) throw new InvalidOperationException("tblLinks2004Gen2 must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblMzManual.Count == 0 ) throw new InvalidOperationException("tblMzManual must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubject.Count == 0 ) throw new InvalidOperationException("tblSubject must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblSubjectDetails.Count == 0 ) throw new InvalidOperationException("tblSubjectDetails must NOT be empty before assigning R values from it.");
			if ( dsLinks.tblMarkerGen2.Count == 0 ) throw new InvalidOperationException("tblMarkerGen2 must NOT be empty before assigning R values from it.");
			_dsImport = dsImport;
			_dsLinks = dsLinks;
			_drLeft = drLeft;
			_drRight = drRight;
			_idRelatedLeft = _drLeft.ID;
			_idRelatedRight = _drRight.ID;
			_drBare1 = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S1);
			_drBare2 = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S2);
			_drSubjectDetails1 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S1);
			_drSubjectDetails2 = _dsLinks.tblSubjectDetails.FindBySubjectTag(drLeft.SubjectTag_S2);
			_extendedID = _drLeft.tblSubjectRowByFK_tblRelatedStructure_tblSubject_Subject1.ExtendedID;

			if ( _drSubjectDetails1.BirthOrderInNls <= _drSubjectDetails2.BirthOrderInNls ) {//This is the way it usually is.  Recall twins were assigned tied birth orders
				_idRelatedOlderAboutYounger = _idRelatedLeft;
				_idRelatedYoungerAboutOlder = _idRelatedRight;
			}
			else if ( _drSubjectDetails1.BirthOrderInNls > _drSubjectDetails2.BirthOrderInNls ) {
				_idRelatedOlderAboutYounger = _idRelatedRight;
				_idRelatedYoungerAboutOlder = _idRelatedLeft;
			}

			_dtMarkersGen2 = MarkerGen2.PairRelevantMarkerRows(_idRelatedLeft, _idRelatedRight, _dsLinks, _extendedID);

			LinksDataSet.tblMzManualRow drMzManual = Retrieve.MzManualRecord(_drBare1, _drBare2, _dsLinks);
			if ( drMzManual == null ) {
				_multipleBirth = MultipleBirth.No;
				_isMZ = Tristate.No;
			}
			else {
				_multipleBirth = (MultipleBirth)drMzManual.MultipleBirthIfSameSex;
				_isMZ = (Tristate)drMzManual.IsMz;
			}
			MarkerEvidence babyDaddyDeathDate = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyDeathDate, _dtMarkersGen2);
			MarkerEvidence babyDaddyAlive = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyAlive, _dtMarkersGen2);
			MarkerEvidence babyDaddyInHH = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyInHH, _dtMarkersGen2);
			MarkerEvidence babyDaddyLeftHHDate = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyLeftHHDate, _dtMarkersGen2);
			MarkerEvidence babyDaddyDistanceFromHH = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyDistanceFromHH, _dtMarkersGen2);
			MarkerEvidence babyDaddyAsthma = MarkerGen2.RetrieveBiodadMarkerFromGen1(_idRelatedOlderAboutYounger, MarkerType.BabyDaddyAsthma, _dtMarkersGen2);

			_rImplicitPass1 = CalculateRImplicitPass1(babyDaddyDeathDate, babyDaddyAlive, babyDaddyInHH, babyDaddyLeftHHDate, babyDaddyDistanceFromHH, babyDaddyAsthma);
			//_rImplicit not set;
			_rImplicit2004 = RetrieveRImplicit2004();
			_rExplicitOldestSibVersion = CalculateRExplicitOldestSibVersion();
			_rExplicitYoungestSibVersion = CalculateRExplicitYoungestSibVersion();
			_rExplicitPass1 = CalculateRExplicitPass1(_isMZ, _multipleBirth);
			//_rExplicit not set;
			_rPass1 = CalculateRPass1();
			//_r not set;
			//_rPeek not set;
		}
		#endregion
		#region Public Methods
		internal static float? ExplicitAgreementWithTwins ( Tristate isMz, MultipleBirth multipleBirth, float? newRExplicit ) {
			if ( !newRExplicit.HasValue ) 
				return null;
			else if ( isMz == Tristate.Yes ) {
				Trace.Assert(newRExplicit >= RCoefficients.SiblingFull, "If the pair has been identified as MZ, their RExplicit should be >= .5.");
				return RCoefficients.MzTrue;
			}
			else if ( isMz == Tristate.DoNotKnow ) {
				Trace.Assert(newRExplicit >= RCoefficients.SiblingAmbiguous, "If the pair has been identified as an ambiguous MZ, their RExplicit should be >= .5.");
				return RCoefficients.MzAmbiguous;
			}
			else if ( multipleBirth == MultipleBirth.Trip || multipleBirth == MultipleBirth.Twin || multipleBirth == MultipleBirth.TwinOrTrip ) {
				Trace.Assert(newRExplicit >= RCoefficients.SiblingFull, "If the pair has been identified as Twin/Trip, their RExplicit should be >= .5.");
				return newRExplicit;
			}
			else {
				return newRExplicit;
			}
		}
		#endregion
		#region Private Methods
		private float? ConvertSummariesToR ( MarkerType markerType, Int32 maxMarkerCount, Int32 idRelated ) {
			MarkerGen2Summary[] summaries = MarkerGen2.RetrieveMarkers(idRelated, markerType, _dtMarkersGen2, maxMarkerCount);
			if ( summaries.Length <= 0 )
				return null;
			IEnumerable<MarkerEvidence> evidences = from summary in summaries
																 select summary.ShareBiodad;
			if ( evidences.All(evidence => evidence == MarkerEvidence.Supports) ) {
				return RCoefficients.SiblingFull;
			}
			else if ( evidences.All(evidence => evidence == MarkerEvidence.Disconfirms) ) {
				return RCoefficients.SiblingHalf;
			}
			else if ( evidences.All(evidence => evidence == MarkerEvidence.Ambiguous) ) {
				return RCoefficients.SiblingAmbiguous;
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
				return RCoefficients.SiblingAmbiguous;
			}
		}
		#endregion
		#region Private Methods - Estimate R
		private float? CalculateRImplicitPass1 ( MarkerEvidence babyDaddyDeathDate, MarkerEvidence babyDaddyAlive, MarkerEvidence babyDaddyInHH, MarkerEvidence babyDaddyLeftHHDate,
			MarkerEvidence babyDaddyDistanceFromHH, MarkerEvidence babyDaddyAsthma ) {
			if ( IsMZ == BaseAssembly.Tristate.Yes ) return RCoefficients.MzTrue;
			else if ( IsMZ == BaseAssembly.Tristate.DoNotKnow ) return RCoefficients.MzAmbiguous;
			else if ( MultipleBirthIfSameSex == BaseAssembly.MultipleBirth.Twin ) return RCoefficients.SiblingFull;
			else if ( MultipleBirthIfSameSex == BaseAssembly.MultipleBirth.Trip ) return RCoefficients.SiblingFull;
			else if ( MultipleBirthIfSameSex == BaseAssembly.MultipleBirth.DoNotKnow ) throw new InvalidOperationException("There shouldn't be any 'MultipleBirth.DoNotKnow' relationships.");
			else if ( babyDaddyDeathDate == MarkerEvidence.StronglySupports ) return RCoefficients.SiblingFull;
			else if ( babyDaddyDeathDate == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			else if ( babyDaddyAlive == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			else if ( babyDaddyLeftHHDate == MarkerEvidence.StronglySupports ) return RCoefficients.SiblingFull;
			else if ( babyDaddyLeftHHDate == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			else if ( babyDaddyInHH == MarkerEvidence.StronglySupports ) return RCoefficients.SiblingFull;
			else if ( babyDaddyInHH == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			else if ( babyDaddyDistanceFromHH == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			else if ( babyDaddyAsthma == MarkerEvidence.Disconfirms ) return RCoefficients.SiblingHalf;
			else return null;
		}
		private float? RetrieveRImplicit2004 ( ) {
			ImportDataSet.tblLinks2004Gen2Row drV1 = _dsImport.tblLinks2004Gen2.FindByID1ID2(_drBare1.SubjectID, _drBare2.SubjectID);
			ImportDataSet.tblLinks2004Gen2Row drV2 = _dsImport.tblLinks2004Gen2.FindByID1ID2(_drBare2.SubjectID, _drBare1.SubjectID);
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
		private float? CalculateRExplicitOldestSibVersion ( ) {
			const MarkerType markerType = MarkerType.ShareBiodad;
			Int32 maxMarkerCount = ItemYears.Gen2ShareBiodad.Length;
			Int32 idRelated = _idRelatedOlderAboutYounger;
			return ConvertSummariesToR(markerType, maxMarkerCount, idRelated);
		}
		private float? CalculateRExplicitYoungestSibVersion ( ) {
			const MarkerType markerType = MarkerType.ShareBiodad;
			Int32 maxMarkerCount = ItemYears.Gen2ShareBiodad.Length;
			Int32 idRelated = _idRelatedYoungerAboutOlder;
			return ConvertSummariesToR(markerType, maxMarkerCount, idRelated);
		}
		private float? CalculateRExplicitPass1 ( Tristate isMZ, MultipleBirth multipleBirth ) {
			float? rExplicitPreTwin = null;
			if ( !RExplicitOldestSibVersion.HasValue && !RExplicitYoungestSibVersion.HasValue )
				rExplicitPreTwin = null;
			else if ( !RExplicitOldestSibVersion.HasValue )
				rExplicitPreTwin = RExplicitYoungestSibVersion.Value;
			else if ( !RExplicitYoungestSibVersion.HasValue )
				rExplicitPreTwin = RExplicitOldestSibVersion.Value;
			else if ( RExplicitOldestSibVersion.Value == RExplicitYoungestSibVersion.Value )
				rExplicitPreTwin = RExplicitOldestSibVersion.Value;
			else if ( RExplicitOldestSibVersion.Value == RCoefficients.SiblingAmbiguous )
				rExplicitPreTwin = RExplicitYoungestSibVersion.Value;
			else if ( RExplicitYoungestSibVersion.Value == RCoefficients.SiblingAmbiguous )
				rExplicitPreTwin = RExplicitOldestSibVersion.Value;
			else if ( RExplicitOldestSibVersion.Value == RCoefficients.SiblingFull && RExplicitYoungestSibVersion.Value == RCoefficients.SiblingHalf )
				rExplicitPreTwin = RCoefficients.SiblingAmbiguous;
			else if ( RExplicitYoungestSibVersion.Value == RCoefficients.SiblingFull && RExplicitOldestSibVersion.Value == RCoefficients.SiblingHalf )
				rExplicitPreTwin = RCoefficients.SiblingAmbiguous;
			else
				throw new InvalidOperationException("All condition should have been caught.");

			return ExplicitAgreementWithTwins(isMZ, multipleBirth, rExplicitPreTwin);
			//return rExplicitPreTwin;
		}
		private float? CalculateRPass1 ( ) {
			if ( this.IsMZ == BaseAssembly.Tristate.Yes )
				return RCoefficients.MzTrue;
			else if ( IsMZ == BaseAssembly.Tristate.DoNotKnow )
				return RCoefficients.MzAmbiguous;
			else if ( this.MultipleBirthIfSameSex == MultipleBirth.Twin || this.MultipleBirthIfSameSex == MultipleBirth.Trip ) //|| this.MultipleBirth == MultipleBirth.TwinOrTrip
				return RCoefficients.SiblingFull;
			else if ( RImplicitPass1.HasValue )
				return RImplicitPass1;
			else if ( RExplicitPass1.HasValue )
				return RExplicitPass1;
			else
				return null;
		}
		#endregion
	}
}
