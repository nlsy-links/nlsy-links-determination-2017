using System;

namespace Nls.BaseAssembly {
	interface IAssignPass1 {
		Int32 IDLeft { get; }
		//Int32 IDRight { get; }
		MultipleBirth MultipleBirthIfSameSex { get; }
		Tristate IsMZ { get; }
		//Tristate IsRelatedInMzManual { get; }
		//Int16 RosterAssignmentID { get; }
		//float? RRoster { get; }

		Tristate ImplicitShareBiomomPass1 { get; }
		Tristate ImplicitShareBiodadPass1 { get; }
		Tristate ExplicitShareBiomomPass1 { get; }
		Tristate ExplicitShareBiodadPass1 { get; }
		Tristate ShareBiomomPass1 { get; }
		Tristate ShareBiodadPass1 { get; }

		float? RImplicitPass1 { get; }
		float? RImplicit2004 { get; }
		float? RExplicitOldestSibVersion { get; }
		float? RExplicitYoungestSibVersion { get; }
		float? RExplicitPass1 { get; }
		float? RPass1 { get; }
	}
}
