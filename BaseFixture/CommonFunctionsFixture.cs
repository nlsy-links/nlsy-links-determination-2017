using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nls.BaseAssembly;

namespace Nls.Tests.BaseFixture {

	[TestClass()]
	public class CommonFunctionsFixture {

		[TestMethod()]
		public void LastTwoDigitsOfGen2SubjectIDTest2 ( ) {
			LinksDataSet ds = new LinksDataSet();
			LinksDataSet.tblSubjectRow dr = ds.tblSubject.NewtblSubjectRow();
			dr.ExtendedID = 10;
			dr.SubjectID = 1002;
			dr.Generation = (byte)Generation.Gen2;
			dr.Gender = (byte)Gender.Male;
			Int32 expected = 2; 
			Int32 actual = CommonFunctions.LastTwoDigitsOfGen2SubjectID(dr);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod()]
		public void LastTwoDigitsOfGen2SubjectIDTest22 ( ) {
			LinksDataSet ds = new LinksDataSet();
			LinksDataSet.tblSubjectRow dr = ds.tblSubject.NewtblSubjectRow();
			dr.ExtendedID = 4000;
			dr.SubjectID = 400122;
			dr.Generation = (byte)Generation.Gen2;
			dr.Gender = (byte)Gender.Male;
			Int32 expected = 22; 
			Int32 actual = CommonFunctions.LastTwoDigitsOfGen2SubjectID(dr);
			Assert.AreEqual(expected, actual);
		}

		#region TranslateEvidenceToTristateTestSingle
		[TestMethod()]
		public void TranslateEvidenceToTristateTestSingleGreen ( ) {
			MarkerEvidence[] evidences = { MarkerEvidence.StronglySupports, MarkerEvidence.Supports };
			const Tristate expected = Tristate.Yes;
			foreach ( MarkerEvidence evidence in evidences ) {
				Tristate actual = CommonFunctions.TranslateEvidenceToTristate(evidence);
				Assert.AreEqual(expected, actual, string.Format("The actual value for {0} did not match the expectations", evidence.ToString()));
			}
		}
		[TestMethod()]
		public void TranslateEvidenceToTristateTestSingleYellow ( ) {
			MarkerEvidence[] evidences = { MarkerEvidence.Consistent, MarkerEvidence.Ambiguous, MarkerEvidence.Irrelevant, MarkerEvidence.Missing };
			const Tristate expected = Tristate.DoNotKnow;
			foreach ( MarkerEvidence evidence in evidences ) {
				Tristate actual = CommonFunctions.TranslateEvidenceToTristate(evidence);
				Assert.AreEqual(expected, actual, string.Format("The actual value for {0} did not match the expectations", evidence.ToString()));
			}
		}
		[TestMethod()]
		public void TranslateEvidenceToTristateTestSingleRed ( ) {
			MarkerEvidence[] evidences = { MarkerEvidence.Unlikely, MarkerEvidence.Disconfirms };
			const Tristate expected = Tristate.No;
			foreach ( MarkerEvidence evidence in evidences ) {
				Tristate actual = CommonFunctions.TranslateEvidenceToTristate(evidence);
				Assert.AreEqual(expected, actual, string.Format("The actual value for {0} did not match the expectations", evidence.ToString()));
			}
		}
		#endregion	
		#region TranslateEvidenceToTristateTestSingle
		[TestMethod()]
		public void TranslateEvidenceToTristateTestDoubleGreen ( ) {
			MarkerEvidence[] evidences1 = { MarkerEvidence.StronglySupports, MarkerEvidence.Supports };
			MarkerEvidence[] evidences2 = {  MarkerEvidence.StronglySupports, MarkerEvidence.Supports ,MarkerEvidence.Consistent, MarkerEvidence.Ambiguous, MarkerEvidence.Irrelevant, MarkerEvidence.Missing, MarkerEvidence.Unlikely, MarkerEvidence.Disconfirms };
			Tristate[] expecteds = { Tristate.Yes, Tristate.Yes, Tristate.Yes, Tristate.Yes, Tristate.Yes, Tristate.Yes, Tristate.DoNotKnow, Tristate.DoNotKnow };
			Trace.Assert(evidences2.Length == expecteds.Length);
			foreach ( MarkerEvidence evidence1 in evidences1 ) {
				for ( Int32 i = 0; i < evidences2.Length; i++ ) {
					Tristate actual = CommonFunctions.TranslateEvidenceToTristate(evidence1, evidences2[i]);
					Assert.AreEqual(expecteds[i], actual, string.Format("The actual value for {0} and {1} did not match the expectations", evidence1.ToString(), evidences2[i].ToString()));
				}
			}
		}
		[TestMethod()]
		public void TranslateEvidenceToTristateTestDoubleYellow ( ) {
			MarkerEvidence[] evidences1 = { MarkerEvidence.Consistent, MarkerEvidence.Ambiguous, MarkerEvidence.Irrelevant, MarkerEvidence.Missing };
			MarkerEvidence[] evidences2 = { MarkerEvidence.StronglySupports, MarkerEvidence.Supports, MarkerEvidence.Consistent, MarkerEvidence.Ambiguous, MarkerEvidence.Irrelevant, MarkerEvidence.Missing, MarkerEvidence.Unlikely, MarkerEvidence.Disconfirms };
			Tristate[] expecteds = { Tristate.Yes, Tristate.Yes, Tristate.DoNotKnow, Tristate.DoNotKnow, Tristate.DoNotKnow, Tristate.DoNotKnow, Tristate.No, Tristate.No };
			Trace.Assert(evidences2.Length == expecteds.Length);
			foreach ( MarkerEvidence evidence1 in evidences1 ) {
				for ( Int32 i = 0; i < evidences2.Length; i++ ) {
					Tristate actual = CommonFunctions.TranslateEvidenceToTristate(evidence1, evidences2[i]);
					Assert.AreEqual(expecteds[i], actual, string.Format("The actual value for {0} and {1} did not match the expectations", evidence1.ToString(), evidences2[i].ToString()));
				}
			}
		}
		[TestMethod()]
		public void TranslateEvidenceToTristateTestDoubleRed ( ) {
			MarkerEvidence[] evidences1 = { MarkerEvidence.Unlikely, MarkerEvidence.Disconfirms };
			MarkerEvidence[] evidences2 = { MarkerEvidence.StronglySupports, MarkerEvidence.Supports, MarkerEvidence.Consistent, MarkerEvidence.Ambiguous, MarkerEvidence.Irrelevant, MarkerEvidence.Missing, MarkerEvidence.Unlikely, MarkerEvidence.Disconfirms };
			Tristate[] expecteds = { Tristate.DoNotKnow, Tristate.DoNotKnow, Tristate.No, Tristate.No, Tristate.No, Tristate.No, Tristate.No, Tristate.No };
			Trace.Assert(evidences2.Length == expecteds.Length);
			foreach ( MarkerEvidence evidence1 in evidences1 ) {
				for ( Int32 i = 0; i < evidences2.Length; i++ ) {
					Tristate actual = CommonFunctions.TranslateEvidenceToTristate(evidence1, evidences2[i]);
					Assert.AreEqual(expecteds[i], actual, string.Format("The actual value for {0} and {1} did not match the expectations", evidence1.ToString(), evidences2[i].ToString()));
				}
			}
		}
		#endregion


		/// <summary>
		///A test for TranslateToR
		///</summary>
		internal struct TranslateContainer {
			public Tristate State1;
			public Tristate State2;
			public float? R;
			public TranslateContainer ( Tristate state1, Tristate state2, float? r ) {
				State1 = state1;
				State2 = state2;
				R = r;
			}
		}
		[TestMethod()]
		public void TranslateToRTest ( ) {
			TranslateContainer[] s = { 
					new TranslateContainer( Tristate.DoNotKnow, Tristate.DoNotKnow, null) ,
					new TranslateContainer( Tristate.No, Tristate.No, RCoefficients.NotRelated) ,
					new TranslateContainer( Tristate.Yes, Tristate.Yes, RCoefficients.SiblingFull) ,
					new TranslateContainer( Tristate.No, Tristate.Yes, RCoefficients.SiblingHalf) ,
					new TranslateContainer( Tristate.Yes, Tristate.No, RCoefficients.SiblingHalf) ,
					new TranslateContainer( Tristate.DoNotKnow, Tristate.No, RCoefficients.SiblingHalfOrLess) ,
					new TranslateContainer( Tristate.No, Tristate.DoNotKnow, RCoefficients.SiblingHalfOrLess) ,
					new TranslateContainer( Tristate.DoNotKnow, Tristate.Yes, RCoefficients.SiblingAmbiguous) ,
					new TranslateContainer( Tristate.Yes, Tristate.DoNotKnow, RCoefficients.SiblingAmbiguous) 
				};

			for ( Int32 i = 0; i < s.Length; i++ ) {
				float? actual = CommonFunctions.TranslateToR(s[i].State1, s[i].State2,	mustDecide: true);
				Assert.AreEqual(s[i].R, actual);
			}
		}
	}
}
