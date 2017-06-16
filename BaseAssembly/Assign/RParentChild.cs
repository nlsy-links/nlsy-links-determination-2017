using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nls.BaseAssembly;

namespace Nls.BaseAssembly.Assign {
	public class RParentChild : IAssignPass1, IAssignPass2 {
		#region Fields
		private readonly LinksDataSet _dsLinks;
		private readonly LinksDataSet.tblRelatedStructureRow _drLeft;
		private readonly LinksDataSet.tblSubjectRow _drBare1;
		private readonly LinksDataSet.tblSubjectRow _drBare2;

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
		public float? R{ get { return _rFull; } }
		public float? RFull { get { return _rFull; } }
		public float? RPeek { get { return _rPeek; } }
		#endregion
		#region Constructor
		public RParentChild ( LinksDataSet dsLinks, LinksDataSet.tblRelatedStructureRow drLeft ) {//ImportDataSet dsImport, , LinksDataSet.tblRelatedStructureRow drRight
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( drLeft == null ) throw new ArgumentNullException("drLeft");
			if ( dsLinks.tblSubject.Count == 0 ) throw new InvalidOperationException("tblSubject must NOT be empty before assigning R values from it.");
			_dsLinks = dsLinks;
			_drLeft = drLeft;
			_idRelatedLeft = _drLeft.ID;
			_drBare1 = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S1);
			_drBare2 = _dsLinks.tblSubject.FindBySubjectTag(drLeft.SubjectTag_S2);
			Trace.Assert(_drBare1.Generation != _drBare2.Generation, "The generation should not be the same for a parent-child relationship.");

			_multipleBirth = MultipleBirth.No;
			_isMZ = Tristate.No;

			//For IAssignPass1
			_rImplicitPass1 = RCoefficients.ParentChild;
			_rImplicit2004 = RCoefficients.ParentChild;
			_rExplicitOldestSibVersion = null;
			_rExplicitYoungestSibVersion = null;
			_rExplicitPass1 = RCoefficients.ParentChild;
			_rPass1 = RCoefficients.ParentChild;

			//For IAssignPass2
			_rImplicit = RCoefficients.ParentChild;
			_rImplicitSubject = RCoefficients.ParentChild;
			_rImplicitMother = RCoefficients.ParentChild;
			_rExplicit = RCoefficients.ParentChild;
			_rFull = RCoefficients.ParentChild;
			_rPeek = RCoefficients.ParentChild;
		}
		#endregion
	}
}
