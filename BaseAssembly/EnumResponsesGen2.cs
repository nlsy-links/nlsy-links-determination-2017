using System;

namespace Nls.BaseAssembly {
	namespace EnumResponsesGen2 {
		public enum TypicalItem : int {
			Missing = -7,
			ValidSkip = -4,
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
		}
		public enum ShareBiodadGen2 : int {
			Missing = -7,
			DoNotKnow = -2,
			Refusal = -1,
			Yes = 1,
			No = 2,
			NotSure = 3,
		}
		public enum FatherOfGen2LiveInHH : short {  
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
		}
		public enum FatherOfGen2Living : short {   
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
		}
		public enum FatherAsthma : int {
			Missing = -7,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
		}
		public enum KidBioCount : int {//Y21486.00
			Missing = -7,
			Zero = 0,
			One = 1,
			Two = 2,
			Three = 3,
			Four = 4,
			Five = 5,
			Six = 6,
			Seven = 7,
			Eight = 8,
			Nine = 9,
		}
	}
}
//public enum DateOfBirthMonthNonResponses : int {//C00055.00
//}
