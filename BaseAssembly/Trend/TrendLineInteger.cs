using System;
using System.Collections.Generic;

namespace Nls.BaseAssembly.Trend {
	public sealed class TrendLineInteger {
		#region Fields
		private readonly Int16[] _surveyYears;
		private readonly Int16?[] _values;
		private readonly Int16[] _jumps;
		//private readonly Int32 _countNonnull=0;
		private readonly Int16? _firstNonnullValue = null;
		private readonly Int16? _lastNonnullValue = null;
		#endregion
		#region Properties
		public Int32 CountAll { get { return _surveyYears.Length; } }
		//public Int32 CountNonnull { get { return _countNonnull; } }
		public Int16[] Jumps { get { return _jumps; } }
		public Int32 JumpCount { get { return _jumps.Length; } }
		public Int16[] SurveyYears { get { return _surveyYears; } }
		public Int16?[] Values { get { return _values; } }
		#endregion
		#region Constructor
		public TrendLineInteger ( Int16[] surveyYears, Int16?[] values ) {
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( values == null ) throw new ArgumentNullException("values");
			Validate(surveyYears, values);

			_surveyYears = surveyYears;
			_values = values;

			Int32 pointCount = surveyYears.Length;
			if ( pointCount <= 1 ) {
				_jumps = new Int16[] { };
			}
			else {
				List<Int16> jumps = new List<Int16>();
				Int16? previous = values[0];
				for ( Int32 i = 1; i < pointCount; i++ ) {//Notice it doesn't start at i=0;
					if ( (values[i] != null) && (!values[i].Equals(previous)) ) {
						if ( previous != null ) {
							//_countNonnull += 1;
							jumps.Add(surveyYears[i]);
							_lastNonnullValue = values[i];

							if ( !_firstNonnullValue.HasValue )
								_firstNonnullValue = values[i];
						}
						previous = values[i];
					}
				}
				_jumps = jumps.ToArray();
			}
		}
		#endregion
		#region Static Methods
		private static void Validate<TValidate> ( Int16[] surveyYears, TValidate[] points ) {
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( points == null ) throw new ArgumentNullException("points");
			if ( surveyYears.Length != points.Length ) throw new ArgumentException("The surveyYears and points arrays should have equal number of elements.");
		}
		#endregion
	}
}