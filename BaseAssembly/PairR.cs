using System;
using System.Data;
using System.Diagnostics;

namespace Nls.BaseAssembly {
	public struct PairR {
		#region Fields
		private readonly Int32 _subjectTag1;
		private readonly Int32 _subjectTag2;
		private readonly Int32 _relatedID;
		private float? _r;
		private bool _isInterpolated;
		#endregion 
		#region Properties
		public Int32 SubjectTag1 { get { return _subjectTag1; } }
		public Int32 SubjectTag2 { get { return _subjectTag2; } }
		public float? R { get { return _r; } }//set { _r = value; } 
		public bool IsInterpolated { get { return _isInterpolated; } }//set { _isInterpolated = value; }
		public Int32 RelatedID {
			get {
				if ( _relatedID < 0 ) throw new InvalidOperationException("The RelatedID was negative; it should not be.");
				else return _relatedID;
			}
		}
		#endregion
		#region Constructors
		public PairR ( Int32 subjectTag1, Int32 subjectTag2, float? r ) {
			if ( !(subjectTag1 < subjectTag2) ) throw new ArgumentException("subjectTag1 must be less than SubjectTag2.");
			_subjectTag1 = subjectTag1;
			_subjectTag2 = subjectTag2;
			_relatedID = Int32.MinValue;
			_r = r;
			_isInterpolated = false;
		}
		public PairR ( Int32 subjectTag1, Int32 subjectTag2, Int32 relatedID, float? r ) {
			if ( !(subjectTag1 < subjectTag2) ) throw new ArgumentException("subjectTag1 must be less than SubjectTag2.");
			_subjectTag1 = subjectTag1;
			_subjectTag2 = subjectTag2;
			_relatedID = relatedID;
			_r = r;
			_isInterpolated = false;
		}
		public static PairR[] BuildRelatedPairsOfGen1Housemates ( DataColumn dcPass1, Int32 subjectTag1, Int32 subjectTag2, Int32 extendedID, LinksDataSet ds ) {
			if ( CommonCalculations.GenerationOfSubjectTag(subjectTag1) != Generation.Gen1 ) throw new ArgumentOutOfRangeException("The generation implied by subjectTag1 isn't Gen1.");
			if ( CommonCalculations.GenerationOfSubjectTag(subjectTag2) != Generation.Gen1 ) throw new ArgumentOutOfRangeException("The generation implied by subjectTag2 isn't Gen1.");
			if ( dcPass1 == null ) throw new ArgumentNullException("dcPass1");
			switch ( dcPass1.ColumnName ) {
				case "RImplicitPass1":
				case "RExplicitPass1":
				case "RPass1":
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

			PairR[] pairs = new PairR[drsStructure.Length];
			for ( Int32 i = 0; i < drsStructure.Length; i++ ) {
				LinksDataSet.tblRelatedStructureRow dr = drsStructure[i];
				Int32 relatedID = dr.ID;
				LinksDataSet.tblRelatedValuesRow drValue = ds.tblRelatedValues.FindByID(relatedID);
				float? pass1;
				if ( DBNull.Value.Equals(drValue[dcPass1]) )//if ( drValue.IsRImplicitPass1Null() )					
					pass1 = null;
				else
					pass1 = Convert.ToSingle(drValue[dcPass1]);//pass1 = (float)drValue.RImplicitPass1;				
				pairs[i] = new PairR(dr.SubjectTag_S1, dr.SubjectTag_S2, relatedID, pass1);
			}
			return pairs;
		}
		public static PairR[] BuildRelatedPairsOfGen2Sibs ( DataColumn dcPass1, Int32 subjectTag1, Int32 subjectTag2, Int32 extendedID, LinksDataSet ds ) {
			if ( CommonCalculations.GenerationOfSubjectTag(subjectTag1) != Generation.Gen2 ) throw new ArgumentOutOfRangeException("The generation implied by subjectTag1 isn't Gen2.");
			if ( CommonCalculations.GenerationOfSubjectTag(subjectTag2) != Generation.Gen2 ) throw new ArgumentOutOfRangeException("The generation implied by subjectTag2 isn't Gen2.");
			if ( dcPass1 == null ) throw new ArgumentNullException("dcPass1");
			switch ( dcPass1.ColumnName ) {
				case "RImplicitPass1":
				case "RExplicitPass1":
				case "RPass1":
					break;
				default:
					throw new ArgumentOutOfRangeException("dcPass1", dcPass1, "The column wasn't recognized as a valid 'Pass1' column.");
			}

			if ( ds == null ) throw new ArgumentNullException("ds");
			string select = string.Format("{0}={1} AND {2}={3} AND ({4}={5} OR {6}={7} OR {6}={5} OR {4}={7}) AND {5}<{7}",
				extendedID, ds.tblRelatedStructure.ExtendedIDColumn.ColumnName,
				(byte)RelationshipPath.Gen2Siblings, ds.tblRelatedStructure.RelationshipPathColumn.ColumnName,
				subjectTag1, ds.tblRelatedStructure.SubjectTag_S1Column.ColumnName,
				subjectTag2, ds.tblRelatedStructure.SubjectTag_S2Column.ColumnName);
			LinksDataSet.tblRelatedStructureRow[] drsStructure = (LinksDataSet.tblRelatedStructureRow[])ds.tblRelatedStructure.Select(select);
			Trace.Assert(drsStructure.Length >= 1, "At least one record should be returned.");

			PairR[] pairs = new PairR[drsStructure.Length];
			for ( Int32 i = 0; i < drsStructure.Length; i++ ) {
				LinksDataSet.tblRelatedStructureRow dr = drsStructure[i];
				Int32 relatedID = dr.ID;
				LinksDataSet.tblRelatedValuesRow drValue = ds.tblRelatedValues.FindByID(relatedID);
				float? pass1;
				if ( DBNull.Value.Equals(drValue[dcPass1]) )//if ( drValue.IsRImplicitPass1Null() )					
					pass1 = null;
				else
					pass1 = Convert.ToSingle(drValue[dcPass1]);//pass1 = (float)drValue.RImplicitPass1;				
				pairs[i] = new PairR(dr.SubjectTag_S1, dr.SubjectTag_S2, relatedID, pass1);
			}
			return pairs;
		}
		#endregion
		#region Methods
		public void SetInterpolatedR ( float r ) {
			if ( _r.HasValue ) throw new InvalidOperationException(string.Format("The relationship for {0} and {1} already has an R of {2}.", _subjectTag1, _subjectTag2, _r));
			_r = r;
			_isInterpolated = true;
		}
		public static Int32 CountHalfSibs ( PairR[] pairs ) {
			Int32 tally = 0;
			foreach ( PairR pair in pairs ) {
				if (pair.R.HasValue &&  Math.Abs(pair.R.Value - RCoefficients.SiblingHalf) < 1e-5 ) 
					tally += 1;
			}
			return tally;
		}
		#endregion
		#region Overrides
		public override int GetHashCode ( ) {
			if(R.HasValue)
			return SubjectTag1 ^ SubjectTag2 ^ RelatedID  ^ Convert.ToInt32(IsInterpolated) ^ Convert.ToInt32(R *1e7);
			else
				return SubjectTag1 ^ SubjectTag2 ^ RelatedID ^ Convert.ToInt32(IsInterpolated) ^ Int32.MinValue;
		}
		public override bool Equals ( object obj ) {
			if ( !(obj is PairR) )
				return false;
			return Equals((PairR)obj);
		}
		public bool Equals ( PairR other ) {
			if ( SubjectTag1 != other.SubjectTag1 )
				return false;
			else if ( SubjectTag2 != other.SubjectTag2 )
				return false;
			else if ( R.HasValue != other.R.HasValue )
				return false;
			else if ( R.HasValue && R.Value == other.R.Value )
				return false;
			else if ( IsInterpolated == other.IsInterpolated )
				return false;
			else
				return true;
		}
		public static bool operator == ( PairR pair1, PairR pair2 ) {
			return pair1.Equals(pair2);
		}
		public static bool operator != ( PairR pair1, PairR pair2 ) {
			return !pair1.Equals(pair2);
		}  
		#endregion
	}
}
