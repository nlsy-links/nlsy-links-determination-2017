using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nls.Tests.BaseFixture {
	public static class Helpers {
		static public void CompareArray ( double[] expected, double[] actual, double tolerance ) {
			Assert.AreEqual(expected.Length, actual.Length, "The array should have the correct number of elements.");

			for ( Int32 rowIndex = 0; rowIndex < expected.GetLength(0); rowIndex++ ) {
				Assert.AreEqual(expected[rowIndex], actual[rowIndex], tolerance, "The element at index ({0}) (zero-based) should be correct.", rowIndex);
			}
		}
		static public void CompareArray ( Int16[] expected, Int16[] actual ) {
			Assert.AreEqual(expected.Length, actual.Length, "The array should have the correct number of elements.");

			for ( Int32 rowIndex = 0; rowIndex < expected.GetLength(0); rowIndex++ ) {
				Assert.AreEqual(expected[rowIndex], actual[rowIndex], "The element at index ({0}) (zero-based) should be correct.", rowIndex);
			}
		}
	}
}
