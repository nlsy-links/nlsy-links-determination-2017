using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nls.BaseAssembly.Assign {
	public sealed class InterpolateR {
		#region Fields
		private readonly PairR[] _pairs;
		private readonly Int32[] _subjectTags;
		#endregion
		#region Properties
		#endregion
		#region Constructor
		public InterpolateR ( PairR[] pairs ) {
			if ( pairs == null ) throw new ArgumentNullException("pairs");
			_pairs = pairs;
			_subjectTags = BuildTagList(pairs);
		}
		#endregion
		#region Public Instance Methods
		public float? Interpolate ( Int32 subjectTag1, Int32 subjectTag2 ) {//, Func<Int32, Int32, float?> retrieve ) {
			Int32 pairIndex = Validate(subjectTag1, subjectTag2, true);

			float? oldR = RetrieveR(subjectTag1, subjectTag2);
			if ( oldR.HasValue ) throw new InvalidOperationException(string.Format("The relationship for {0} and {1} already has an R of {2}.", subjectTag1, subjectTag2, oldR));

			float? halfSibSalvage = SalvageHalfSibLoop(subjectTag1, subjectTag2, pairIndex);
			if ( halfSibSalvage.HasValue )
				return halfSibSalvage.Value;
			else
				return SalvageFullSibLoop(subjectTag1, subjectTag2, pairIndex);//This could be null still.
		}
		//public float? Interpolate ( Int32 subjectTag1, Int32 subjectTag2 ) {
		//   Int32 pairIndex = Validate(subjectTag1, subjectTag2, true);

		//   float? oldR = RetrieveR(subjectTag1, subjectTag2);
		//   if ( oldR.HasValue ) throw new InvalidOperationException(string.Format("The relationship for {0} and {1} already has an R of {2}.", subjectTag1, subjectTag2, oldR));

		//   float? halfSibSalvage = SalvageHalfSibLoop(subjectTag1, subjectTag2, pairIndex);
		//   if ( halfSibSalvage.HasValue )
		//      return halfSibSalvage.Value;
		//   else
		//      return SalvageFullSibLoop(subjectTag1, subjectTag2, pairIndex);//This could be null still.

		//   //float? fullSibSalvage = SalvageFullSibLoop(subjectTag1, subjectTag2, pairIndex);
		//   //if ( fullSibSalvage.HasValue )
		//   //   return fullSibSalvage.Value;
		//   //else
		//   //   return SalvageHalfSibLoop(subjectTag1, subjectTag2, pairIndex);//This could be null still.
		//}

		private float? SalvageFullSibLoop ( Int32 subjectTag1, Int32 subjectTag2, Int32 pairIndex){//, Func<Int32, Int32, float?> retrieve ) {
			for ( Int32 subjectIndex = 0; subjectIndex < _subjectTags.Length; subjectIndex++ ) {
				Int32 candidateTag = _subjectTags[subjectIndex];
				if ( candidateTag != subjectTag1 && candidateTag != subjectTag2 ) {
					float? rWith1 = RetrieveR(candidateTag, subjectTag1);
					float? rWith2 = RetrieveR(candidateTag, subjectTag2);
					float? interpolatedR = SalvageFullSib(rWith1, rWith2);
					if ( interpolatedR.HasValue ) {
						_pairs[pairIndex].SetInterpolatedR(interpolatedR.Value);
						return interpolatedR.Value;
					}
				}
			}
			return null;
		}
		private float? SalvageHalfSibLoop ( Int32 subjectTag1, Int32 subjectTag2, Int32 pairIndex ) {
			for ( Int32 subjectIndex = 0; subjectIndex < _subjectTags.Length; subjectIndex++ ) {
				Int32 candidateTag = _subjectTags[subjectIndex];
				if ( candidateTag != subjectTag1 && candidateTag != subjectTag2 ) {
					float? rWith1 = RetrieveR(candidateTag, subjectTag1);
					float? rWith2 = RetrieveR(candidateTag, subjectTag2);
					float? interpolatedR = SalvageHalfSib(rWith1, rWith2);
					if ( interpolatedR.HasValue ) {
						_pairs[pairIndex].SetInterpolatedR(interpolatedR.Value);
						return interpolatedR.Value;
					}
				}
			}
			return null;
		}
		public static float? SalvageFullSib ( float? rWith1, float? rWith2 ) {
			if ( !rWith1.HasValue || !rWith2.HasValue )
				return null;
			else if ( rWith1.Value >= .5f && rWith2.Value >= .5f )//Greater than accounts for twins
				return .5f;
			else
				return null;

		}
		public static float? SalvageHalfSib ( float? rWith1, float? rWith2 ) {
			if ( !rWith1.HasValue || !rWith2.HasValue )
				return null;
			else if ( (rWith1.Value >= .5f && rWith2.Value == .25f) || (rWith1.Value == .25f && rWith2.Value >= .5f) )
				return .25f;
			else
				return null;

		}
		public float? RetrieveR ( Int32 subjectTagA, Int32 subjectTagB ) {
			Validate(subjectTagA, subjectTagB, false);

			for ( Int32 i = 0; i < _pairs.Length; i++ ) {
				if ( _pairs[i].SubjectTag1 == subjectTagA && _pairs[i].SubjectTag2 == subjectTagB ) return _pairs[i].R;
				if ( _pairs[i].SubjectTag2 == subjectTagA && _pairs[i].SubjectTag1 == subjectTagB ) return _pairs[i].R;
			}
			throw new ArgumentException("A relationship for subjects " + subjectTagA + " and " + subjectTagB + " does not exist in the instance Pairs array.");
		}
		//public float? RetrieveRImplicit ( Int32 subjectTagA, Int32 subjectTagB ) {
		//   Validate(subjectTagA, subjectTagB, false);

		//   for ( Int32 i = 0; i < _pairs.Length; i++ ) {
		//      if ( _pairs[i].SubjectTag1 == subjectTagA && _pairs[i].SubjectTag2 == subjectTagB ) return _pairs[i].Ri;
		//      if ( _pairs[i].SubjectTag2 == subjectTagA && _pairs[i].SubjectTag1 == subjectTagB ) return _pairs[i].R;
		//   }
		//   throw new ArgumentException("A relationship for subjects " + subjectTagA + " and " + subjectTagB + " does not exist in the instance Pairs array.");
		//}
		public bool SubjectTagExistsInPairs ( Int32 subjectTag ) {
			for ( Int32 i = 0; i < _pairs.Length; i++ ) {
				if ( _pairs[i].SubjectTag1 == subjectTag ) return true;
				if ( _pairs[i].SubjectTag2 == subjectTag ) return true;
			}
			return false;
		}
		public Int32 RelationshipIndexOfPairs ( Int32 subjectTag1, Int32 subjectTag2 ) {
			for ( Int32 i = 0; i < _pairs.Length; i++ ) {
				if ( _pairs[i].SubjectTag1 == subjectTag1 && _pairs[i].SubjectTag2 == subjectTag2 ) return i;
				if ( _pairs[i].SubjectTag2 == subjectTag1 && _pairs[i].SubjectTag1 == subjectTag2 ) return i;
			}
			return -1;
		}
		public static Int32[] BuildTagList ( PairR[] pairs ) {
			if ( pairs == null ) throw new ArgumentNullException("pairs");
			List<Int32> list = new List<Int32>();
			for ( Int32 i = 0; i < pairs.Length; i++ ) {
				list.Add(pairs[i].SubjectTag1);
				list.Add(pairs[i].SubjectTag2);
			}
			Int32[] array = list.Distinct().ToArray();
			Array.Sort(array);
			return array;
		}
		#endregion
		#region Private Methods
		private Int32 Validate ( Int32 subjectTag1, Int32 subjectTag2, bool enforceIncreasingOrder ) {
			if ( enforceIncreasingOrder && !(subjectTag1 < subjectTag2) ) throw new ArgumentException("The value of subjectTag1 must be less than subjectTag2; subjectTag1 is " + subjectTag1 + " and subjectTag2 is " + subjectTag2 + ".");
			if ( !SubjectTagExistsInPairs(subjectTag1) ) throw new ArgumentOutOfRangeException("subjectTag1", subjectTag1, "The subject does not exist in the instance Pairs array.");
			if ( !SubjectTagExistsInPairs(subjectTag2) ) throw new ArgumentOutOfRangeException("subjectTag2", subjectTag2, "The subject does not exist in the instance Pairs array.");
			Int32 index = RelationshipIndexOfPairs(subjectTag1, subjectTag2);
			if ( index < 0 ) throw new ArgumentException("A relationship for subjects " + subjectTag1 + " and " + subjectTag2 + " does not exist in the instance Pairs array.");
			return index;
		}
		#endregion
	}
}
