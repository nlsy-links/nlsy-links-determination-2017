using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nls.BaseAssembly.Trend {
	public sealed class TrendLineDate {
		#region Fields
		private readonly Int16[] _surveyYears;
		private readonly DateTime?[] _dates;
		private readonly Int16[] _jumps;
		//private readonly Int32 _countNonnull = 0;
		private readonly DateTime? _firstNonnullValue = null;
		private readonly DateTime? _lastNonnullValue = null;
		#endregion
		#region Properties
		public Int32 CountAll { get { return _surveyYears.Length; } }
		//public Int32 CountNonnull { get { return _countNonnull; } }
		public Int16[] Jumps { get { return _jumps; } }
		public Int32 JumpCount { get { return _jumps.Length; } }
		public Int16[] SurveyYears { get { return _surveyYears; } }
		public DateTime?[] Dates { get { return _dates; } }
		#endregion
		#region Constructor
		public TrendLineDate ( Int16[] surveyYears, DateTime?[] dates ) {
			if ( surveyYears == null ) throw new ArgumentNullException("surveyYears");
			if ( dates == null ) throw new ArgumentNullException("dates");
			Validate(surveyYears, dates);

			_surveyYears = surveyYears;
			_dates = dates;

			Int32 pointCount = surveyYears.Length;
			if ( pointCount <= 1 ) {
				_jumps = new Int16[] { };
			}
			else {
				List<Int16> jumps = new List<Int16>();
				DateTime? previous = dates[0];
				for ( Int32 i = 1; i < pointCount; i++ ) {//Notice it doesn't start at i=0;
					if ( (dates[i] != null) && (!dates[i].Equals(previous)) ) {
						if ( previous != null ) {
							//_countNonnull += 1;
							jumps.Add(surveyYears[i]);
							_lastNonnullValue = dates[i];

							if ( !_firstNonnullValue.HasValue )
								_firstNonnullValue = dates[i];
						}

						previous = dates[i];
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
//public static Int16[] FindJumps ( TrendLine<T> trend ) {
//   return FindJumps<T>(trend.SurveyYears, trend.Points);
//}
//public static Int16[] FindJumps<TStatic> ( Int16[] surveyYears, TStatic[] points ) {
//   Validate<TStatic>(surveyYears, points);
//   Int32 pointCount = surveyYears.Length;
//   if ( pointCount <= 1 ) return new Int16[] { };

//   List<Int16> jumps = new List<Int16>();
//   TStatic previous = points[0];
//   //TStatic firstNonnullValue=null;
//   //TStatic lastNonnullValue=null;

//   for ( Int32 i = 1; i < pointCount; i++ ) {//Notice it doesn't start at i=0;
//      if ( (points[i] != null) && (!points[i].Equals(previous)) ) {
//         if ( previous != null ) {
//            jumps.Add(surveyYears[i]);
//            //lastNonnullValue = points[i];
//         }

//         previous = points[i];
//      }
//   }
//   return jumps.ToArray();
//}
