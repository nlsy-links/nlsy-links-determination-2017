using System;
using System.Collections.Generic;

namespace Nls.BaseAssembly.Trend {
	public sealed class TrendLineGen0InHH {
		#region Fields
		private readonly bool _hasAnyRecords;
		private readonly bool? _everAtHome;
		private readonly Int32 _yob;
		private readonly Int16[] _years;
		private readonly bool?[] _values;
		private readonly byte[] _ages;
		private readonly Int16[] _jumps;
		//private readonly Int32 _countNonnull=0;
		//private readonly bool? _firstNonnullValue = null;
		//private readonly bool? _lastNonnullValue = null;
		#endregion
		#region Properties
		public Int32 CountAll { get { return _years.Length; } }
		//public Int32 CountNonnull { get { return _countNonnull; } }
		public bool? EverAtHome { get { return _everAtHome; } }
		public bool HasAnyRecords { get { return _hasAnyRecords; } }
		public Int16[] Jumps { get { return _jumps; } }
		public Int32 JumpCount { get { return _jumps.Length; } }
		public Int16[] Years { get { return _years; } }
		public bool?[] Values { get { return _values; } }
		public Int32 Yob { get { return _yob; } }
		#endregion
		#region Constructor
		public TrendLineGen0InHH ( Int32 yob, bool hasAnyRecords, bool? everAtHome, Int16[] years, bool?[] values, byte[] ages ) {
			_yob = yob;
			_hasAnyRecords = hasAnyRecords;
			_everAtHome = everAtHome;

			if ( !_hasAnyRecords ) {
				_years = new Int16[] { };
				_ages = new byte[] { };
				_values = new bool?[] { };
				_jumps = new Int16[] { };
			}
			else {
				if ( years == null ) throw new ArgumentNullException("years");
				if ( values == null ) throw new ArgumentNullException("values");
				if ( ages == null ) throw new ArgumentNullException("ages");
				Validate(years, values, ages);

				_years = years;
				_values = values;
				_ages = ages;

				Int32 pointCount = years.Length;
				if ( pointCount <= 1 ) {
					_jumps = new Int16[] { };
				}
				else {
					List<Int16> jumps = new List<Int16>();
					bool? previous = values[0];
					for ( Int32 i = 1; i < pointCount; i++ ) {//Notice it doesn't start at i=0;
						if ( (values[i].HasValue) && (!values[i].Equals(previous)) ) {
							if ( previous.HasValue ) {
								//_countNonnull += 1;
								jumps.Add(years[i]);
								//_lastNonnullValue = values[i];

								//if ( !_firstNonnullValue.HasValue )
								//   _firstNonnullValue = values[i];
							}
							previous = values[i];
						}
					}
					_jumps = jumps.ToArray();
				}
			}
		}
		#endregion
		#region Static Methods
		public static void Validate<TValidate> ( Int16[] years, TValidate[] points, byte[] ages ) {
			if ( years == null ) throw new ArgumentNullException("surveyYears");
			if ( points == null ) throw new ArgumentNullException("points");
			if ( ages == null ) throw new ArgumentNullException("ages");
			if ( years.Length != points.Length ) throw new ArgumentException("The years and points arrays should have equal number of elements.");
			if ( years.Length != ages.Length ) throw new ArgumentException("The years and ages arrays should have equal number of elements.");
		}
		#endregion
	}
}