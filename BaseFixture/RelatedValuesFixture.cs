using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nls.BaseAssembly;
using Nls.BaseAssembly.LinksDataSetTableAdapters;

namespace Nls.Tests.BaseFixture {
    [TestClass]
    public sealed class RelatedValuesFixture {
        #region Fields
        private static LinksDataSet _ds = new LinksDataSet();
        #endregion
        #region Structs
        //private struct Pair {
        //   private readonly Int32 _subjectTag1;
        //   private readonly Int32 _subjectTag2;
        //   public Int32 SubjectTag1 { get { return _subjectTag1; } }
        //   public Int32 SubjectTag2 { get { return _subjectTag2; } }
        //   public Pair ( Int32 subjectTag1, Int32 subjectTag2 ) {
        //      _subjectTag1 = subjectTag1;
        //      _subjectTag2 = subjectTag2;
        //   }
        //}
        #endregion
        #region Additional test attributes
        private TestContext testContextInstance;
        public TestContext TestContext { get { return testContextInstance; } set { testContextInstance = value; } }
        [ClassInitialize()]
        public static void ClassInitialize( TestContext testContext ) {
            tblRelatedStructureTableAdapter taStructure = new tblRelatedStructureTableAdapter();
            using( taStructure ) {
                taStructure.Fill(_ds.tblRelatedStructure);
            }
            tblRelatedValuesTableAdapter taValues = new tblRelatedValuesTableAdapter();
            using( taValues ) {
                taValues.Fill(_ds.tblRelatedValues);
            }
        }
        [ClassCleanup()]
        public static void ClassCleanup( ) {
        }
        #endregion
        #region FullSib Tests
        [TestMethod]
        public void FullSibDadDied( ) {
            CompareRImplicit(BuildFullSibDadDiedGen1());
            CompareRImplicit(BuildFullSibDadDiedGen2());
        }
        [TestMethod]
        public void FullSibDadLeft( ) {
            CompareRImplicit(BuildFullSibDadLeftGen1());
            CompareRImplicit(BuildFullSibDadLeftGen2());
        }
        public void FullSibDadDistance( ) {
            CompareRImplicit(BuildFullSibDadDistanceGen1());
            CompareRImplicit(BuildFullSibDadDistanceGen2());
        }
        [TestMethod]
        public void FullSibWhiteBread( ) {
            CompareRImplicit(BuildFullSibWhiteBreadGen1());
            CompareRImplicit(BuildFullSibWhiteBreadGen2());
        }
        #endregion
        #region HalfSib Tests
        [TestMethod]
        public void HalfSibDadDied( ) {
            CompareRImplicit(BuildHalfSibDadDiedGen1());
            CompareRImplicit(BuildHalfSibDadDiedGen2());
        }
        [TestMethod]
        public void HalfSibDadLeft( ) {
            CompareRImplicit(BuildHalfSibDadLeftGen1());
            CompareRImplicit(BuildHalfSibDadLeftGen2());
        }
        [TestMethod]
        public void HalfSibDadDistance( ) {
            CompareRImplicit(BuildHalfSibDadDistanceGen1());
            CompareRImplicit(BuildHalfSibDadDistanceGen2());
        }
        //[TestMethod]
        //public void HalfSibWhiteBread ( ) {
        //   CompareRImplicit( BuildHalfSibWhiteBread());
        //}
        #endregion
        #region Helpers
        private static void CompareRImplicit( PairR[] pairs ) {
            foreach( PairR pair in pairs ) {
                double? actualRImplicit = RetrieveRImplicit(pair.SubjectTag1, pair.SubjectTag2);
                if( !actualRImplicit.HasValue )
                    Assert.Fail(string.Format("Subjects {0} and {1} had a null RImplicit.  It should be {2}.", pair.SubjectTag1, pair.SubjectTag2, pair.R));
                else
                    Assert.AreEqual(pair.R, actualRImplicit, "The RImplicit for subject tags " + pair.SubjectTag1 + " and " + pair.SubjectTag2 + " should be equal.");
            }
        }
        private static double? RetrieveRImplicit( Int32 subjectTag1, Int32 subjectTag2 ) {
            string select = string.Format("{0}={1} AND {2}={3}",
                subjectTag1, _ds.tblRelatedStructure.SubjectTag_S1Column.ColumnName,
                subjectTag2, _ds.tblRelatedStructure.SubjectTag_S2Column.ColumnName);
            LinksDataSet.tblRelatedStructureRow[] drsStructure = (LinksDataSet.tblRelatedStructureRow[])_ds.tblRelatedStructure.Select(select);
            Trace.Assert(drsStructure.Length == 1, "There should be exactly one row returned.");
            Int32 relatedID = drsStructure[0].ID;
            LinksDataSet.tblRelatedValuesRow drValue = _ds.tblRelatedValues.FindByID(relatedID);
            if( drValue.IsRImplicitNull() )
                return null;
            else
                return drValue.RImplicit;
        }
        #endregion
        #region BuildFullSibs
        private static PairR[] BuildFullSibDadDiedGen1( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            //pairs.Add(new PairR(1700, 1800, expectedRImplicit));
            return pairs.ToArray();
        }
        private static PairR[] BuildFullSibDadDiedGen2( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            pairs.Add(new PairR(103002, 103003, expectedRImplicit));
            return pairs.ToArray();
        }

        private static PairR[] BuildFullSibDadLeftGen1( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            return pairs.ToArray();
        }
        private static PairR[] BuildFullSibDadLeftGen2( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            pairs.Add(new PairR(801, 802, expectedRImplicit));
            pairs.Add(new PairR(801, 803, expectedRImplicit));
            pairs.Add(new PairR(802, 803, expectedRImplicit));
            pairs.Add(new PairR(4303, 4304, expectedRImplicit));
            pairs.Add(new PairR(66802, 66803, expectedRImplicit));
            pairs.Add(new PairR(66802, 66804, expectedRImplicit));
            pairs.Add(new PairR(66803, 66804, expectedRImplicit));
            pairs.Add(new PairR(533801, 533802, expectedRImplicit));
            pairs.Add(new PairR(533801, 533803, expectedRImplicit));
            pairs.Add(new PairR(533802, 533803, expectedRImplicit));
            return pairs.ToArray();
        }

        private static PairR[] BuildFullSibDadDistanceGen1( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            return pairs.ToArray();
        }
        private static PairR[] BuildFullSibDadDistanceGen2( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            pairs.Add(new PairR(1217202, 1217203, expectedRImplicit));
            pairs.Add(new PairR(1217202, 1217204, expectedRImplicit));
            return pairs.ToArray();
        }

        private static PairR[] BuildFullSibWhiteBreadGen1( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            return pairs.ToArray();
        }
        private static PairR[] BuildFullSibWhiteBreadGen2( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .5f;
            pairs.Add(new PairR(17401, 17402, expectedRImplicit));
            pairs.Add(new PairR(495301, 495302, expectedRImplicit));
            pairs.Add(new PairR(495301, 495303, expectedRImplicit));
            pairs.Add(new PairR(495301, 495304, expectedRImplicit));
            pairs.Add(new PairR(495302, 495303, expectedRImplicit));
            pairs.Add(new PairR(495302, 495304, expectedRImplicit));
            pairs.Add(new PairR(495303, 495304, expectedRImplicit));
            pairs.Add(new PairR(914501, 914502, expectedRImplicit));
            pairs.Add(new PairR(914501, 914503, expectedRImplicit));
            pairs.Add(new PairR(914501, 914504, expectedRImplicit));
            pairs.Add(new PairR(914502, 914503, expectedRImplicit));
            pairs.Add(new PairR(914502, 914504, expectedRImplicit));
            pairs.Add(new PairR(914503, 914504, expectedRImplicit));
            return pairs.ToArray();
        }
        #endregion
        #region BuildHalfSibs
        private static PairR[] BuildHalfSibDadDiedGen1( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .25f;
            return pairs.ToArray();
        }
        private static PairR[] BuildHalfSibDadDiedGen2( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .25f;
            pairs.Add(new PairR(103001, 103002, expectedRImplicit));
            pairs.Add(new PairR(103001, 103003, expectedRImplicit));
            pairs.Add(new PairR(103001, 103004, expectedRImplicit));
            pairs.Add(new PairR(103002, 103004, expectedRImplicit));//103002, 103003 are full sibs, confirmed by DeathDate & Explicit
            pairs.Add(new PairR(103003, 103004, expectedRImplicit));
            return pairs.ToArray();
        }

        private static PairR[] BuildHalfSibDadLeftGen1( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .25f;
            return pairs.ToArray();
        }
        private static PairR[] BuildHalfSibDadLeftGen2( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .25f;
            pairs.Add(new PairR(4301, 4302, expectedRImplicit));
            pairs.Add(new PairR(4301, 4303, expectedRImplicit));
            pairs.Add(new PairR(4301, 4304, expectedRImplicit));//Years don't match up
            pairs.Add(new PairR(4302, 4303, expectedRImplicit));
            pairs.Add(new PairR(4302, 4304, expectedRImplicit));
            pairs.Add(new PairR(66801, 66802, expectedRImplicit));
            pairs.Add(new PairR(66801, 66803, expectedRImplicit));
            pairs.Add(new PairR(66801, 66804, expectedRImplicit));
            //pairs.Add(new Pair(66801, 66805, expectedRImplicit));//Years don't match up
            pairs.Add(new PairR(66802, 66805, expectedRImplicit));
            pairs.Add(new PairR(66803, 66805, expectedRImplicit));
            pairs.Add(new PairR(66804, 66805, expectedRImplicit));
            pairs.Add(new PairR(1217202, 1217205, expectedRImplicit));//The explicits don't agree; I could see it either way.
            pairs.Add(new PairR(1217203, 1217205, expectedRImplicit));//The explicits don't agree; I could see it either way.
            pairs.Add(new PairR(1217204, 1217205, expectedRImplicit));//The explicits don't agree; I could see it either way.
            pairs.Add(new PairR(1254201, 1254202, expectedRImplicit));
            pairs.Add(new PairR(1254201, 1254203, expectedRImplicit));
            return pairs.ToArray();
        }

        private static PairR[] BuildHalfSibDadDistanceGen1( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .25f;
            return pairs.ToArray();
        }
        private static PairR[] BuildHalfSibDadDistanceGen2( ) {
            List<PairR> pairs = new List<PairR>();
            const float expectedRImplicit = .25f;
            pairs.Add(new PairR(1217201, 1217202, expectedRImplicit));
            pairs.Add(new PairR(1217201, 1217203, expectedRImplicit));
            pairs.Add(new PairR(1217201, 1217204, expectedRImplicit));
            pairs.Add(new PairR(1217201, 1217205, expectedRImplicit));
            return pairs.ToArray();
        }
        //private static Pair[] BuildHalfSibWhiteBread ( ) {
        //   //const float expectedRImplicit = .25f;
        //   List<Pair> pairs = new List<Pair>();
        //   return pairs.ToArray();
        //}
        #endregion
    }
}
