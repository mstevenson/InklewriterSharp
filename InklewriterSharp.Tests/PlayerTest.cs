using NUnit.Framework;
using System;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class PlayerTest
	{
		[Test]
		public void NumToWordsConvert ()
		{
			Assert.AreEqual ("zero", NumToWords.Convert (0));
			Assert.AreEqual ("minus ten", NumToWords.Convert (-10));
			Assert.AreEqual ("fifteen", NumToWords.Convert (15));
			Assert.AreEqual ("three hundred and eighty-two", NumToWords.Convert (382));
			Assert.AreEqual ("one thousand and twenty", NumToWords.Convert (1020));
			Assert.AreEqual ("forty-two thousand, one hundred and sixty-five", NumToWords.Convert (42165));
			Assert.AreEqual ("ninety million", NumToWords.Convert (90000000));
			Assert.AreEqual ("minus seven hundred and thirty-three trillion, two hundred billion, fifty-eight thousand and one", NumToWords.Convert (-733200000058001));
		}
	}
}

