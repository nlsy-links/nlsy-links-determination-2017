using System;
using System.Data;
using System.Diagnostics;

namespace Nls.BaseAssembly {
	public struct PairShare {
		#region Fields
		private readonly Int32 _subjectTag1;
		private readonly Int32 _subjectTag2;
		private readonly Int32 _relatedID;
		private Tristate _share;
		private bool _isInterpolated;
		#endregion 
		#region Properties
		public Int32 SubjectTag1 { get { return _subjectTag1; } }
		public Int32 SubjectTag2 { get { return _subjectTag2; } }
		public Tristate Share { get { return _share; } }//set { _r = value; } 
		public bool IsInterpolated { get { return _isInterpolated; } }//set { _isInterpolated = value; }
		public Int32 RelatedID {
			get {
				if ( _relatedID < 0 ) throw new InvalidOperationException("The RelatedID was negative; it should not be.");
				else return _relatedID;
			}
		}
		#endregion
		#region Constructors
		public PairShare ( Int32 subjectTag1, Int32 subjectTag2, Tristate share ) {
			if ( !(subjectTag1 < subjectTag2) ) throw new ArgumentException("subjectTag1 must be less than SubjectTag2.");
			_subjectTag1 = subjectTag1;
			_subjectTag2 = subjectTag2;
			_relatedID = Int32.MinValue;
			_share = share;
			_isInterpolated = false;
		}
		public PairShare ( Int32 subjectTag1, Int32 subjectTag2, Int32 relatedID, Tristate share ) {
			if ( !(subjectTag1 < subjectTag2) ) throw new ArgumentException("subjectTag1 must be less than SubjectTag2.");
			_subjectTag1 = subjectTag1;
			_subjectTag2 = subjectTag2;
			_relatedID = relatedID;
			_share = share;
			_isInterpolated = false;
		}
		public static PairShare[] BuildRelatedPairsOfGen1Housemates ( DataColumn dcPass1, Int32 subjectTag1, Int32 subjectTag2, Int32 extendedID, LinksDataSet ds ) {
			if ( CommonCalculations.GenerationOfSubjectTag(subjectTag1) != Generation.Gen1 ) throw new ArgumentOutOfRangeException("The generation implied by subjectTag1 isn't Gen1.");
			if ( CommonCalculations.GenerationOfSubjectTag(subjectTag2) != Generation.Gen1 ) throw new ArgumentOutOfRangeException("The generation implied by subjectTag2 isn't Gen1.");
			if ( dcPass1 == null ) throw new ArgumentNullException("dcPass1");
			switch ( dcPass1.ColumnName ) {
				case "ImplicitShareBiomomPass1":
				case "ImplicitShareBiodadPass1":
				case "ExplicitShareBiomomPass1":
				case "ExplicitShareBiodadPass1":
				case "ShareBiomomPass1":
				case "ShareBiodadPass1":
					break;
				default:
					throw new ArgumentOutOfRangeException("dcPass1", dcPass1, "The column wasn't recognized as a valid 'Pass1' column.");
			}

			if ( ds == null ) throw new ArgumentNullException("ds");
			string select = string.Format("{0}={1} AND {2}={3} AND ({4}={5} OR {6}={7} OR {6}={5} OR {4}={7}) AND {5}<{7}",
				extendedID, ds.tblRelatedStructure.ExtendedIDColumn.ColumnName,
				(byte)RelationshipPath.Gen1Housemates, ds.tblRelatedStructure.RelationshipPathColumn.ColumnName,
				subjectTag1, ds.tblRelatedStructure.SubjectTag_S1Column.ColumnName,
				subjectTag2, ds.tblRelatedStructure.SubjectTag_S2Column.ColumnName);
			LinksDataSet.tblRelatedStructureRow[] drsStructure = (LinksDataSet.tblRelatedStructureRow[])ds.tblRelatedStructure.Select(select);
			Trace.Assert(drsStructure.Length >= 1, "At least one record should be returned.");

			PairShare[] pairs = new PairShare[drsStructure.Length];
			for ( Int32 i = 0; i < drsStructure.Length; i++ ) {
				LinksDataSet.tblRelatedStructureRow dr = drsStructure[i];
				Int32 relatedID = dr.ID;
				LinksDataSet.tblRelatedValuesRow drValue = ds.tblRelatedValues.FindByID(relatedID);
				Tristate pass1;
				if ( DBNull.Value.Equals(drValue[dcPass1]) )//if ( drValue.IsRImplicitPass1Null() )					
					pass1 = Tristate.DoNotKnow;
				else
					pass1 = (Tristate)(drValue[dcPass1]);//pass1 = (float)drValue.RImplicitPass1;				
				pairs[i] = new PairShare(dr.SubjectTag_S1, dr.SubjectTag_S2, relatedID, pass1);
			}
			return pairs;
		}
		
		#endregion
		#region Methods
		public void SetInterpolatedShare ( Tristate share ) {
			if ( _share !=  Tristate.DoNotKnow) throw new InvalidOperationException(string.Format("The relationship for {0} and {1} already has a share value of {2}.", _subjectTag1, _subjectTag2, _share));
			_share = share;
			_isInterpolated = true;
		}
		//public static Int32 CountHalfSibs ( PairShare[] pairs ) {
		//   Int32 tally = 0;
		//   foreach ( PairR pair in pairs ) {
		//      if (pair.R.HasValue &&  Math.Abs(pair.R.Value - RCoefficients.SiblingHalf) < 1e-5 ) 
		//         tally += 1;
		//   }
		//   return tally;
		//}
		#endregion
	}
}
