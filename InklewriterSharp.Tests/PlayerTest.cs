using NUnit.Framework;
using System;
using System.Collections.Generic;
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

		[Test]
		public void ReplaceQuotes ()
		{
			Player player = new Player (null);

			Assert.AreEqual ("This is “quoted” text.", player.ReplaceQuotes ("This is \"quoted\" text."));
			Assert.AreEqual ("It’s an apostrophe.", player.ReplaceQuotes ("It's an apostrophe."));
		}

		[Test]
		public void ConvertNumberToWords ()
		{
			Player player = new Player (null);
			List<FlagValue> flags = new List<FlagValue> {
				new FlagValue ("a", 5),
				new FlagValue ("b", 10),
			};

			Assert.AreEqual ("The number five.", player.ConvertNumberToWords ("The number [value:a].", flags));
			Assert.AreEqual ("The number 5.", player.ConvertNumberToWords ("The number [number:a].", flags));
		}

		[Test]
		public void ReplaceStyleMarkup ()
		{
			Player player = new Player (null);

			Assert.AreEqual ("This is <b>bold</b> text.", player.ReplaceStyleMarkup ("This is *-bold-* text."));
			Assert.AreEqual ("This is <i>italic</i> text.", player.ReplaceStyleMarkup ("This is /=italic=/ text."));
		}
	}
}

