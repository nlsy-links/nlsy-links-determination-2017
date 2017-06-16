using System;

namespace Nls.BaseAssembly {
	namespace EnumResponsesGen1 {
		public enum TypicalItem : int {
			NonInterview = -5,
			ValidSkip = -4,
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
		}
		public enum ShareBioparentGen1 : int {
			//DoesNotExistInNls = -99,
			NonInterview = -5,
			ValidSkip = -4,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
			NotSure = 2,
		}
		public enum Gen1BioparentBirthCountry : short { //eg, R00061.00
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			US = 1,
			NotUS = 2,
			DidNotKnowParent = 3,
		}
		public enum Gen1BioparentAlive : short { //eg, H00024.00
			ValidSkip = -4,
			//InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
		}
		public enum Gen1BioparentDeathCause : short { //eg, H00137.00
			ValidSkipOrNoInterviewOrNotInSurvey=-6,
			////NonInterview = -5,
			//ValidSkip = -4,
			//InvalidSkip = -3,
			//DoNotKnow = -2,
			//Refusal = -1,
			MIOrStroke = 1,
			Accident = 2,
			Cancer = 3,
			OldAge = 4,
			Emphysema = 5,
			Other = 6,
		}
		public enum BioparentOfGen1InHH : short { //eg, R28372.00
			NonInterview = -5,
			ValidSkip = -4,
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
		}
		public enum BabyDaddyLiveInHH : short {//eg, R42768.00    
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
		}
		public enum BabyDaddyLiving : short {// eg, R42769.00    
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			No = 0,
			Yes = 1,
		}
		public enum BabyDaddyLeftHH : short {//eg, R42770.00    
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			Yes = 1,
			NeverLivedInHH = 2,
		}
		public enum BabyDaddyLeftHHMonthDoubleCoded : short {//eg, R42770.00    
			InvalidSkip = -3,
			DoNotKnow = -2,
			Refusal = -1,
			January = 1,
			February = 2,
			March = 3,
			April = 4,
			May = 5,
			June = 6,
			July = 7,
			August = 8,
			September = 9,
			October = 10,
			November = 11,
			December = 12,
			NeverLivedInHH = 96,
		}
		//public enum BabyDaddyDistance : short {//eg, R42770.00    
		//   InvalidSkip = -3,
		//   DoNotKnow = -2,
		//   Refusal = -1,
		//   Yes = 1,
		//   NeverLivedInHH = 2,
		//}
		public enum Gen1SiblingIsATwinOrTrip1994 : short {
			NoTwinOrTripListed = 0,
			TwinOrTripListed = 1,
		}
		public enum Gen1MultipleSiblingType1994 : short {
			NoTwinOrTrip = 0,
			Twin = 1,
			Trip = 2,
		}
		public enum Gen1ListedTwinOrTripCorrect1994 : short {
			No = 0,
			Yes= 1,
		}
		public enum Gen1SiblingIsMzOrDz1994 : short {
			MZ = 1,
			DZ = 2,
		}
		public enum Gen1Roster : short {//Int16
			ValidSkip = -4,
			InvalidSkip = -3,
			Refusal = -1,

			Respondent = 0,
			Spouse = 1,
			Son = 2,
			Daughter = 3,
			Father = 4,
			Mother = 5,
			Brother = 6,
			Sister = 7,
			Grandfather = 8,
			Grandmother = 9,
			Grandson = 10,
			Granddaughter = 11,
			UncleEtc = 12,
			AuntEtc = 13,
			GreatUncle = 14,
			GreatAunt = 15,
			Cousin = 16,
			Nephew = 17,
			Niece = 18,
			OtherBloodRelative = 19,
			AdoptedOrStepson = 20,
			AdoptedOrStepdaughter = 21,
			SonInLaw = 22,
			DaughterInLaw = 23,
			FatherInLaw = 24,
			MotherInLaw = 25,
			BrotherInLaw = 26,
			SisterInLaw = 27,
			GrandfatherInLaw = 28,
			GrandmotherInLaw = 29,
			GrandsonInLaw = 30,
			GranddaughterInLaw = 31,
			OtherInLawRelative = 32,
			Partner = 33,
			Boarder = 34,
			FosterChild = 35,
			OtherNonRelative = 36,
			Stepfather = 37,
			Stepmother = 38,
			Stepbrother = 39,
			Stepsister = 40,
			GreatGrandson = 41,
			GreatGranddaughter = 42,
			StepGrandson = 43,
			StepGranddaughter = 44,
			FosterSon = 45,
			FosterDaughter = 46,
			Parents = 47,
			Grandparents = 48,
			AuntOrUncle = 49,
			FosterFather = 50,
			FosterMother = 51,
			FosterBrother = 52,
			FosterSister = 53,
			Guardian = 54,
			HusbandOrBrotherInLaw = 57,
			WifeOrSisterInLaw = 58,
			AdoptedOrStepbrother = 59,
			AdoptedOrStepsister = 60,//Watch out, this is listed at the bottom of the NlsInvestigator
			BrotherOrCousin = 62,
			SisterOrCousin = 63,
			BrotherNaturalStepOrAdopted = 64,
			SisterNaturalStepOrAdopted = 65,
			SiblingOrSpouseOfInLaws = 66,
		}
	}
}
//public enum MultipleBirthType : int {//'These values come from NLS Gen1 items such as R48260.00 and R81067.00
//   NonInterview = -5,
//   ValidSkip = -4,
//   DoNotKnow = -2,
//   Refusal = -1,
//   Identical = 1,
//   Fraternal = 2,
//}
//public enum RHasSiblings : int {
//   Missing = -7,
//   InvalidSkip = -3,
//   No = 0,
//   Yes = 1,
//}
//public enum TwinsTripsListed : int {
//   NonInterview = -5,
//   ValidSkip = -4,
//   DoNotKnow = -2,
//   Refusal = -1,
//   No = 0,
//   OneSetOfTwinsOnRoster = 1,
//   OneSetOfTripsOnRoster = 2,
//   IncorrectTwinsTripsOnRoster = 3,
//}
