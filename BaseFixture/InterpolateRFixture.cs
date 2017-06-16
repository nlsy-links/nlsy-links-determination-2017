using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nls.BaseAssembly;
using Nls.BaseAssembly.Assign;

namespace Nls.Tests.BaseFixture {
	[TestClass()]
	public sealed class InterpolateRFixture {
		#region Full Sibling Triplet
		[TestMethod()]
		public void FullSib1vs2 ( ) {
			const float expectedR = .5f;
			PairR[] pairs = { new PairR(1, 2, null), new PairR(1, 3, .5f), new PairR(2, 3, .5f) };
			TestInterpolation(pairs, expectedR, 0);
		}
		[TestMethod()]
		public void FullSib1vs3 ( ) {
			const float expectedR = .5f;
			PairR[] pairs = { new PairR(1, 2, .5f), new PairR(1, 3, null), new PairR(2, 3, .5f) };
			TestInterpolation(pairs, expectedR, 1);
		}
		[TestMethod()]
		public void FullSib2vs3 ( ) {
			const float expectedR = .5f;
			PairR[] pairs = { new PairR(1, 2, .5f), new PairR(1, 3, .5f), new PairR(2, 3, null) };
			TestInterpolation(pairs, expectedR, 2);
		}
		#endregion
		#region Half Sibling Triplet
		[TestMethod()]
		public void HalfSib1vs2 ( ) {
			const float expectedR = .25f;
			PairR[] pairsA = { new PairR(1, 2, null), new PairR(1, 3, .5f), new PairR(2, 3, .25f) };
			PairR[] pairsB = { new PairR(1, 2, null), new PairR(1, 3, .25f), new PairR(2, 3, .5f) };
			TestInterpolation(pairsA, expectedR, 0);
			TestInterpolation(pairsB, expectedR, 0);
		}
		[TestMethod()]
		public void HalfSib1vs3 ( ) {
			const float expectedR = .25f;
			PairR[] pairsA = { new PairR(1, 2, .5f), new PairR(1, 3, null), new PairR(2, 3, .25f) };
			PairR[] pairsB = { new PairR(1, 2, .25f), new PairR(1, 3, null), new PairR(2, 3, .5f) };
			TestInterpolation(pairsA, expectedR, 1);
			TestInterpolation(pairsB, expectedR, 1);
		}
		[TestMethod()]
		public void HalfSib2vs3 ( ) {
			const float expectedR = .25f;
			PairR[] pairsA = { new PairR(1, 2, .5f), new PairR(1, 3, .25f), new PairR(2, 3, null) };
			PairR[] pairsB = { new PairR(1, 2, .25f), new PairR(1, 3, .5f), new PairR(2, 3, null) };
			TestInterpolation(pairsA, expectedR, 2);
			TestInterpolation(pairsB, expectedR, 2);
		}
		#endregion
		#region Helpers
		private void TestInterpolation ( PairR[] pairs, float expectedR, Int32 indexOfInterpolatedR ) {
			InterpolateR target = new InterpolateR(pairs);
			Int32 subjectTag1 = pairs[indexOfInterpolatedR].SubjectTag1;
			Int32 subjectTag2 = pairs[indexOfInterpolatedR].SubjectTag2;
			float? actualRV1 = target.Interpolate(subjectTag1, subjectTag2);
			Assert.AreEqual(expectedR, actualRV1, "The interpolated R should be correct.");
			float actualRV2 = pairs[indexOfInterpolatedR].R.Value;
			Assert.AreEqual(expectedR, actualRV2, "The interpolated R should be correct.");
		}
		#endregion
	}
}
