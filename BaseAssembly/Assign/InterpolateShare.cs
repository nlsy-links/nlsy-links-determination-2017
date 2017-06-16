using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nls.BaseAssembly.Assign {
	public sealed class InterpolateShare {
		#region Fields
		private readonly PairShare[] _pairs;
		private readonly Int32[] _subjectTags;
		//private readonly Bioparent _bioparent;
		#endregion
		#region Properties
		#endregion
		#region Constructor
		public InterpolateShare ( PairShare[] pairs){ //, Bioparent bioparent ) {
			if ( pairs == null ) throw new ArgumentNullException("pairs");
			_pairs = pairs;
			//_bioparent = bioparent;
			_subjectTags = BuildTagList(pairs);
		}
		#endregion
		#region Public Instance Methods
		public Tristate Interpolate ( Int32 subjectTag1, Int32 subjectTag2 ) {//, Func<Int32, Int32, float?> retrieve ) {
			Int32 pairIndex = Validate(subjectTag1, subjectTag2, true);

			Tristate oldR = RetrieveShare(subjectTag1, subjectTag2);
			if ( oldR != Tristate.DoNotKnow ) throw new InvalidOperationException(string.Format("The relationship for {0} and {1} already has an R of {2}.", subjectTag1, subjectTag2, oldR));

			Tristate halfSibSalvage = SalvageHalfSibLoop(subjectTag1, subjectTag2, pairIndex);
			if ( halfSibSalvage != Tristate.DoNotKnow )
				return halfSibSalvage;
			else
				return SalvageFullSibLoop(subjectTag1, subjectTag2, pairIndex);//This could be DoNotKnow still.
		}
		private Tristate SalvageFullSibLoop ( Int32 subjectTag1, Int32 subjectTag2, Int32 pairIndex ) {//, Func<Int32, Int32, float?> retrieve ) {
			for ( Int32 subjectIndex = 0; subjectIndex < _subjectTags.Length; subjectIndex++ ) {
				Int32 candidateTag = _subjectTags[subjectIndex];
				if ( candidateTag != subjectTag1 && candidateTag != subjectTag2 ) {
					Tristate anchorSharesWithSubject1 = RetrieveShare(candidateTag, subjectTag1);
					Tristate anchorSharesWithSubject2 = RetrieveShare(candidateTag, subjectTag2);
					Tristate interpolatedShare = SalvageFullSib(anchorSharesWithSubject1, anchorSharesWithSubject2);
					if ( interpolatedShare != Tristate.DoNotKnow ) {
						_pairs[pairIndex].SetInterpolatedShare(interpolatedShare);
						return interpolatedShare;
					}
				}
			}
			return Tristate.DoNotKnow;
		}
		private Tristate SalvageHalfSibLoop ( Int32 subjectTag1, Int32 subjectTag2, Int32 pairIndex ) {
			for ( Int32 subjectIndex = 0; subjectIndex < _subjectTags.Length; subjectIndex++ ) {
				Int32 candidateTag = _subjectTags[subjectIndex];
				if ( candidateTag != subjectTag1 && candidateTag != subjectTag2 ) {
					Tristate anchorSharesWithSubject1 = RetrieveShare(candidateTag, subjectTag1);
					Tristate anchorSharesWithSubject2 = RetrieveShare(candidateTag, subjectTag2);
					Tristate interpolatedShare = SalvageHalfSib(anchorSharesWithSubject1, anchorSharesWithSubject2);
					if ( interpolatedShare != Tristate.DoNotKnow ) {
						_pairs[pairIndex].SetInterpolatedShare(interpolatedShare);
						return interpolatedShare;
					}
				}
			}
			return Tristate.DoNotKnow;
		}
		public static Tristate SalvageFullSib ( Tristate anchorSharesWithSubject1, Tristate anchorSharesWithSubject2 ) {
			if ( (anchorSharesWithSubject1 == Tristate.DoNotKnow) || (anchorSharesWithSubject2 == Tristate.DoNotKnow) )
				return Tristate.DoNotKnow;
			else if ( (anchorSharesWithSubject1 == Tristate.Yes) && (anchorSharesWithSubject2 == Tristate.Yes) )
				return Tristate.Yes;
			else
				return Tristate.DoNotKnow;
		}
		public static Tristate SalvageHalfSib ( Tristate anchorSharesWithSubject1, Tristate anchorSharesWithSubject2 ) {
			if ( (anchorSharesWithSubject1 == Tristate.DoNotKnow) || (anchorSharesWithSubject2 == Tristate.DoNotKnow) )
				return Tristate.DoNotKnow;
			else if ( (anchorSharesWithSubject1 == Tristate.Yes && anchorSharesWithSubject2 == Tristate.No) || (anchorSharesWithSubject1 == Tristate.No && anchorSharesWithSubject2 == Tristate.Yes) )
				return Tristate.No;
			else
				return Tristate.DoNotKnow;
		}
		public Tristate RetrieveShare ( Int32 subjectTagA, Int32 subjectTagB ) {
			Validate(subjectTagA, subjectTagB, false);

			for ( Int32 i = 0; i < _pairs.Length; i++ ) {
				if ( _pairs[i].SubjectTag1 == subjectTagA && _pairs[i].SubjectTag2 == subjectTagB ) return _pairs[i].Share;
				if ( _pairs[i].SubjectTag2 == subjectTagA && _pairs[i].SubjectTag1 == subjectTagB ) return _pairs[i].Share;
			}
			throw new ArgumentException("A relationship for subjects " + subjectTagA + " and " + subjectTagB + " does not exist in the instance Pairs array.");
		}
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
		public static Int32[] BuildTagList ( PairShare[] pairs ) {
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
