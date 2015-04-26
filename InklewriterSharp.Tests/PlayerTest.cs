using NUnit.Framework;
using System;
using System.Collections.Generic;
using Inklewriter;
using Inklewriter.Player;
using Inklewriter.MarkupConverters;

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
			StoryPlayer player = new StoryPlayer (null, new HtmlConverter ());

			Assert.AreEqual ("This is “quoted” text.", player.ReplaceQuotes ("This is \"quoted\" text."));
			Assert.AreEqual ("It’s an apostrophe.", player.ReplaceQuotes ("It's an apostrophe."));
		}

		[Test]
		public void ConvertNumbersToWords ()
		{
			StoryPlayer player = new StoryPlayer (null, new HtmlConverter ());
			List<FlagValue> flags = new List<FlagValue> {
				new FlagValue ("a", 5),
				new FlagValue ("b", 10),
			};

			Assert.AreEqual ("The number five.", player.ConvertNumbersToWords ("The number [value:a].", flags));
			Assert.AreEqual ("The number 5.", player.ConvertNumbersToWords ("The number [number:a].", flags));
		}

		[Test]
		public void ReplaceStyleMarkup ()
		{
			StoryPlayer player = new StoryPlayer (null, new HtmlConverter ());

			Assert.AreEqual ("This is <b>bold</b> text.", player.ReplaceStyleMarkup ("This is *-bold-* text."));
			Assert.AreEqual ("This is <i>italic</i> text.", player.ReplaceStyleMarkup ("This is /=italic=/ text."));
		}

		[Test]
		[Ignore]
		public void ReplaceStyleMarkupWithDelegates ()
		{
//			StoryPlayer player = new StoryPlayer (null);
//			// Convert to markdown syntax
//			player.onStyledBold = text => {
//				return "**" + text + "**";
//			};
//			player.onStyledItalic = text => {
//				return "_" + text + "_";
//			};
//
//			Assert.AreEqual ("This is **bold** text.", player.ReplaceStyleMarkup ("This is *-bold-* text."));
//			Assert.AreEqual ("This is _italic_ text.", player.ReplaceStyleMarkup ("This is /=italic=/ text."));
		}

		[Test]
		public void ReplaceUrlMarkup ()
		{
			StoryPlayer player = new StoryPlayer (null, new HtmlConverter ());

			Assert.AreEqual ("<a href=\"http://inklestudios.com\">Inkle</a>", player.ReplaceUrlMarkup ("[http://inklestudios.com|Inkle]"));
		}

		[Test]
		[Ignore]
		public void ReplaceUrlMarkupWithDelegate ()
		{
//			StoryPlayer player  = new StoryPlayer (null);
//			// Convert to markdown syntax
//			player.onReplacedLinkUrl = (url, title) => {
//				return string.Format ("[{0}]({1})", title, url);
//			};
//
//			Assert.AreEqual ("[Inkle](http://inklestudios.com)", player.ReplaceUrlMarkup ("[http://inklestudios.com|Inkle]"));
		}

		[Test]
		public void ReplaceRunOnMarker ()
		{
			StoryPlayer player = new StoryPlayer (null, new HtmlConverter ());
			Assert.AreEqual ("Run on ", player.ReplaceRunOnMarker ("Run on[...]"));
		}

		[Test]
		public void ShuffleRandomElements ()
		{
			StoryPlayer player = new StoryPlayer (null, new HtmlConverter ());
			List<string> validResults = new List<string> { "Random red color.", "Random blue color.", "Random green color." };
			string result = player.ShuffleRandomElements ("Random {~red|green|blue} color.");

			Assert.IsTrue (validResults.Contains (result));
		}

		[Test]
		public void ParseInLineConditionals ()
		{
			string input = "Moving { speed > 5 : quickly | slowly }.";
//			string slowResult = StoryPlayer.ParseInLineConditionals (input, new List<FlagValue> { new FlagValue ("speed", 1) });
			string quickResult = StoryPlayer.ParseInLineConditionals (input, new List<FlagValue> { new FlagValue ("speed", 8) });

//			Assert.AreEqual ("Moving slowly.", slowResult);
			Assert.AreEqual ("Moving quickly.", quickResult);
		}

		[Test]
		public void CalculateApproximateWordCount ()
		{
			Stitch a = new Stitch ("The quick brown fox");
			Stitch b = new Stitch ("jumped over the lazy dog.");
			int wordCount = StoryPlayer.CalculateApproximateWordCount (new List<Stitch> { a, b });

			// Word count is rounded to the nearest 10 when under 100 words, otherwise rounded to 100
			Assert.AreEqual (10, wordCount);
		}

		[Test]
		public void ReplaceImageMarkup ()
		{
			StoryPlayer player = new StoryPlayer (null, new HtmlConverter ());
			var result = player.ReplaceImageMarkup (@"%|%|%image.jpg$|$|$");

			Assert.AreEqual (@"<div id=""illustration""><img class=""pic"" src=""image.jpg""/></div>", result);
		}

		[Test]
		[Ignore]
		public void ReplaceImageMarkupWithDelegate ()
		{
		}


		public void SaveGame (StoryModel storyModel)
		{
		}

		public void LoadGame ()
		{
		}
	}
}

