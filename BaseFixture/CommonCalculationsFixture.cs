using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nls.BaseAssembly;

namespace Nls.Tests.BaseFixture {
    [TestClass()]
    public class CommonCalculationsFixture {
        [TestMethod()]
        public void ConvertItemsToStringTest( ) {
            Item[] items = { Item.AgeAtInterviewDateMonths, Item.AgeAtInterviewDateYears, 
							Item.DateOfBirthMonth, Item.DateOfBirthYearGen1, Item.DateOfBirthYearGen2,
							Item.InterviewDateDay,Item.InterviewDateMonth,Item.InterviewDateYear};
            const string expected = "17,16,13,14,15,20,21,22";
            string actual = CommonCalculations.ConvertItemsToString(items);
            Assert.AreEqual(expected, actual, "The Item IDs should be correct.");
        }
    }
}
