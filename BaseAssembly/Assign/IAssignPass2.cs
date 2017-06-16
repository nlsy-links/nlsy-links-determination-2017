using System;

namespace Nls.BaseAssembly {
	interface IAssignPass2 {
		Int32 IDLeft { get; }
		//Int32 IDRight { get; }
		float? RImplicit { get; }
		float? RImplicitSubject { get; }
		float? RImplicitMother { get; }
		float? RExplicit { get; }
		float? R { get; }
		float? RFull { get; }
		float? RPeek { get; }
	}
}
