using System;

namespace Nls.BaseAssembly {
	public sealed class TimePoint<T> {

		private readonly Int16 _surveyYear = Int16.MinValue;
		private readonly T _point;

		internal Int16 SurveyYear { get { return _surveyYear; } }
		public T Point { get { return _point; } }
		
		internal TimePoint ( Int16 surveyYear, T point ) {
			_surveyYear = surveyYear;
			_point = point;
		}
	}
}
