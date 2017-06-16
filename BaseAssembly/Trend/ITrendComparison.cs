using System;
namespace Nls.BaseAssembly.Trend {
	public interface ITrendComparison {
		Int32 AgreementCountExcludingNulls { get; }
		Int32 AgreementCountOfNulls { get; }
		double AgreementProportionExcludingNulls { get; }
		Int32 Count { get; }
		Int32 CountOfNullZeroes { get; }
		Int32 CountOfNullSingles { get; }
		Int32 CountOfNullDoubles { get; }
		Int32 DisagreementCountExcludingNulls { get; }
		Int32 DisagreementCountIncludingNulls { get; }
		bool JumpsAgreePerfectly { get; }
		bool? LastMutualNonNullPointsAgree { get; }
		Int16? LastNonMutualNullPointsYear { get; }
		bool PointsAgreePerfectly { get; }
		//MarkerEvidence Evidence { get; }
		//MarkerEvidence Evidence { get; }

	}
}