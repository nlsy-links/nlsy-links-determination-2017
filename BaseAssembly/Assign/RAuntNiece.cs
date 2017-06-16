using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nls.BaseAssembly;

namespace Nls.BaseAssembly.Assign {
	public class RAuntNiece : IAssignPass1, IAssignPass2 {
		#region Fields
		//private const RelationshipPath _path = RelationshipPath.AuntNiece;
		private readonly LinksDataSet _dsLinks;
		private readonly LinksDataSet.tblSubjectRow _drBareAunt;
		private readonly LinksDataSet.tblSubjectRow _drBareNiece;
		private readonly Int32 _idRelatedLeft = Int32.MinValue;

		private readonly MultipleBirth _multipleBirth;
		private readonly Tristate _isMZ; private float? _rImplicitPass1 = null;// float.NaN;
		private float? _rImplicit2004 = float.NaN;
		private float? _rExplicitOldestSibVersion = float.NaN;
		private float? _rExplicitYoungestSibVersion = float.NaN;
		private float? _rExplicitPass1 = float.NaN;
		private float? _rPass1 = float.NaN;

		//For Pass2
		private float? _rImplicit = float.NaN;
		private float? _rImplicitSubject = float.NaN;
		private float? _rImplicitMother = float.NaN;
		private float? _rExplicit = float.NaN;
		private float? _r= float.NaN;
		private float? _rFull = float.NaN;
		private float? _rPeek = float.NaN;
		#endregion
		#region IAssign1 Properties
		public Int32 IDLeft { get { return _idRelatedLeft; } }
		public MultipleBirth MultipleBirthIfSameSex { get { return _multipleBirth; } }
		public Tristate IsMZ { get { return _isMZ; } }
		//public Tristate IsRelatedInMzManual { get { return Tristate.No; } }
		public Tristate ImplicitShareBiomomPass1 { get { return Tristate.No; } }
		public Tristate ImplicitShareBiodadPass1 { get { return Tristate.No; } }
		public Tristate ExplicitShareBiomomPass1 { get { return Tristate.No; } }
		public Tristate ExplicitShareBiodadPass1 { get { return Tristate.No; } }
		public Tristate ShareBiomomPass1 { get { return Tristate.No; } }
		public Tristate ShareBiodadPass1 { get { return Tristate.No; } }
		public float? RImplicitPass1 { get { return _rImplicitPass1; } }
		public float? RImplicit2004 { get { return _rImplicit2004; } }
		public float? RExplicitOldestSibVersion { get { return _rExplicitOldestSibVersion; } }
		public float? RExplicitYoungestSibVersion { get { return _rExplicitYoungestSibVersion; } }
		public float? RExplicitPass1 { get { return _rExplicitPass1; } }
		public float? RPass1 { get { return _rPass1; } }
		#endregion
		#region IAssign Properties
		public float? RImplicit { get { return _rImplicit; } }
		public float? RImplicitSubject { get { return _rImplicitSubject; } }
		public float? RImplicitMother { get { return _rImplicitMother; } }
		public float? RExplicit { get { return _rExplicit; } }
		public float? R{ get { return _r; } }
		public float? RFull { get { return _rFull; } }
		public float? RPeek { get { return _rPeek; } }
		#endregion
		#region Constructor
		public RAuntNiece ( LinksDataSet dsLinks, LinksDataSet.tblRelatedStructureRow drLeft ) {//ImportDataSet dsImport, , LinksDataSet.tblRelatedStructureRow drRight
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( drLeft == null ) throw new ArgumentNullException("drLeft");
			if ( dsLinks.tblSubject.Count == 0 ) throw new InvalidOperationException("tblSubject must NOT be empty before assigning R values from it.");
			_dsLinks = dsLinks;
			_idRelatedLeft = drLeft.ID;
			
			if ( CommonCalculations.GenerationOfSubjectTag(drLeft.SubjectTag_S1) == Generation.Gen1 ) {
				_drBareAunt = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S1);
				_drBareNiece = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S2);
			}
			else {
				_drBareAunt = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S2);
				_drBareNiece = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S1);
			}
			Trace.Assert(_drBareAunt.Generation == (byte)Generation.Gen1, "The generation should not be the same for a aunt-niece relationship.");
			Trace.Assert(_drBareNiece.Generation == (byte)Generation.Gen2, "The generation should not be the same for a aunt-niece relationship.");

			_multipleBirth = MultipleBirth.No;
			_isMZ = Tristate.No;

			LinksDataSet.tblRelatedValuesRow drValuesOfGen1Housemates = Gen1HousematesValues(_drBareAunt.SubjectTag, _drBareNiece.SubjectTag);//RelatedValues.Retrieve(_dsLinks, _path, 
			//For IAssignPass1
			if ( drValuesOfGen1Housemates.IsRImplicitPass1Null() ) _rImplicitPass1 = null;
			else _rImplicitPass1 = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RImplicitPass1);

			if ( drValuesOfGen1Housemates.IsRImplicit2004Null() ) _rImplicit2004 = null;
			else _rImplicit2004 = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RImplicit2004);

			_rExplicitOldestSibVersion = null;
			_rExplicitYoungestSibVersion = null;

			if ( drValuesOfGen1Housemates.IsRExplicitPass1Null() ) _rExplicitPass1 = null;
			else _rExplicitPass1 = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RExplicitPass1);

			if ( drValuesOfGen1Housemates.IsRPass1Null() ) _rPass1 = null;
			else _rPass1 = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RPass1); ;

			//For IAssignPass2
			if ( drValuesOfGen1Housemates.IsRImplicitNull() ) _rImplicit = null;
			else _rImplicit = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RImplicit);

			if ( drValuesOfGen1Housemates.IsRImplicitSubjectNull() ) _rImplicitSubject = null;
			else _rImplicitSubject = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RImplicitSubject);

			if ( drValuesOfGen1Housemates.IsRImplicitMotherNull() ) _rImplicitMother = null;
			else _rImplicitMother = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RImplicitMother);

			if ( drValuesOfGen1Housemates.IsRExplicitNull() ) _rExplicit = null;
			else _rExplicit = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RExplicit);

			if ( drValuesOfGen1Housemates.IsRNull() ) _r= null;
			else _r= (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.R);

			if ( drValuesOfGen1Housemates.IsRFullNull() ) _rFull = null;
			else _rFull = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RFull);

			if ( drValuesOfGen1Housemates.IsRPeekNull() ) _rPeek = null;
			else _rPeek = (float)(RCoefficients.ParentChild * drValuesOfGen1Housemates.RPeek);
		}
		#endregion
		#region Private Methods
		private LinksDataSet.tblRelatedValuesRow Gen1HousematesValues ( Int32 auntTag, Int32 nieceTag ) {
			RelationshipPath path = RelationshipPath.Gen1Housemates;
			Int32 motherSisterTag = CommonCalculations.MotherTagOfGen2Subject(nieceTag);
			return RelatedValues.RetrieveRRow(_dsLinks, path, auntTag, motherSisterTag);
		}
		#endregion
	}
}
