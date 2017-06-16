using System;

namespace Nls.BaseAssembly {
	public static class RCoefficients {
		public const float ParentChild = 0.5f;
		public const float MzTrue = 1.0f;
		public const float MzAmbiguous = 0.75f;
		public const float SiblingFull = 0.5f;
		public const float SiblingAmbiguous = 0.375f;
		public const float SiblingHalf = 0.25f;
		public const float SiblingHalfOrLess = 0.0625f;
		public const float NotRelated = 0f;
	}
}

////R=.5
//public static bool IsSiblingFull ( float value ) {
//   return Convert.ToBoolean(Math.Abs(value - SiblingFull) < Single.Epsilon);
//}
////R=.375
//public static bool IsSiblingAmbiguous ( float value ) {
//   return Convert.ToBoolean(Math.Abs(value - SiblingAmbiguous) < Single.Epsilon);
//}
////R=.25
//public static bool IsSiblingHalf ( float value ) {
//   return Convert.ToBoolean(Math.Abs(value - SiblingHalf) < Single.Epsilon);
//}
////R=0
//public static bool IsNotRelated ( float value ) {
//   return Convert.ToBoolean(Math.Abs(value - NotRelated) < Single.Epsilon);
//}
