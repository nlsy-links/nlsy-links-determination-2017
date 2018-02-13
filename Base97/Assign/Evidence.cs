using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nls.Base97.EnumResponses;

namespace Nls.Base97 {
    namespace Assign {
        public static class Evidence {
            //public const float LowestRelatedR = (float)(1 / 64);

            public static MarkerEvidence RosterSameGeneration( Tristate tristate ) {
                switch( tristate ) {
                    case Tristate.No: return MarkerEvidence.Disconfirms;
                    case Tristate.Yes: return MarkerEvidence.StronglySupports;
                    case Tristate.DoNotKnow: return MarkerEvidence.Ambiguous;
                    default: throw new ArgumentOutOfRangeException("tristate", tristate, "This value is not permitted.");
                }
            }
            public static MarkerEvidence RosterShareBioParentOrGrandparent( Tristate tristate ) {
                switch( tristate ) {
                    case Tristate.No: return MarkerEvidence.Disconfirms;
                    case Tristate.Yes: return MarkerEvidence.Consistent;
                    case Tristate.DoNotKnow: return MarkerEvidence.Ambiguous;
                    default: throw new ArgumentOutOfRangeException("tristate", tristate, "This value is not permitted.");
                }
            }
            //public static MarkerEvidence ShareBioparentsForBioparents( EnumResponses.ShareBioparent share ) {
            //    switch( share ) {
            //        case ShareBioparent.Yes:
            //            return MarkerEvidence.Supports;
            //        case ShareBioparent.No:
            //            return MarkerEvidence.Disconfirms;
            //        case ShareBioparent.DoNotKnow:
            //        case ShareBioparent.NotSure:
            //        case ShareBioparent.Refusal:
            //            return MarkerEvidence.Ambiguous;
            //        case ShareBioparent.NonInterview:
            //        case ShareBioparent.ValidSkip:
            //            return MarkerEvidence.Irrelevant;
            //        default:
            //            throw new ArgumentOutOfRangeException("share", share, "The enum value for share is not recognized.");
            //    }
            //}
        }
    }
}
