using NUnit.Framework;
using System;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class ReadTest
	{
		string data;
		Story story;

		[SetUp]
		public void CreateStoryData ()
		{
			data = @"{""updated_at"":""2015-04-21T03:36:04Z"",""url_key"":""bhwj"",""data"":{""editorData"":{""textSize"":2,""libraryVisible"":true,""playPoint"":""thisIsTheThirdPa1"",""authorName"":""Inkle""},""stitches"":{""thisIsThePageLin"":{""content"":[""This is the page linked from Option One, and joins back to the second paragraph of the story."",{""divert"":""thisIsTheSecondP""}]},""thisIsTheThirdPa1"":{""content"":[""This is the third paragraph of the story, and displays an image above it."",{""image"":""https://archive.org/images/WaybackLogoSmall.png""},{""ifConditions"":null,""option"":""Option One with condition"",""linkPath"":""thisIsThePageLin"",""notIfConditions"":[{""notIfCondition"":""first marker""}]},{""ifConditions"":null,""option"":""Option Two"",""linkPath"":""thisOptionSetsTh"",""notIfConditions"":null},{""ifConditions"":null,""option"":""Disconnected option"",""linkPath"":null,""notIfConditions"":null}]},""thisIsTheSecondP"":{""content"":[""This is the second paragraph, and runs on to the third paragraph."",{""runOn"":true},{""divert"":""thisIsTheThirdPa""}]},""thisIsAnUnconnec"":{""content"":[""This is an unconnected stitch.""]},""thisIsTheInitial"":{""content"":[""This is the *-initial paragraph-* in the story, and is followed by /=three=/ additional paragraphs."",{""divert"":""thisIsTheSecondP""},{""pageNum"":1}]},""thisOptionSetsTh"":{""content"":[""This option sets the \""First Marker\"" flag. { first marker: When set, this text is shown. }"",{""divert"":""thisIsTheSecondP""},{""flagName"":""first marker""}]},""thisIsTheThirdPa"":{""content"":[""This is the third paragraph and contains a condition for it to be shown."",{""divert"":""thisIsTheThirdPa1""},{""ifCondition"":""first marker""}]}},""optionMirroring"":true,""initial"":""thisIsTheInitial"",""allowCheckpoints"":true},""created_at"":""2015-04-21T03:09:41Z"",""title"":""Feature Test""}";
			story = StoryReader.Read (data);
		}

		[Test]
		public void StitchCount ()
		{
			int stitchesExpected = 7;
			int stitchesFound = story.Stitches.Count;

			Assert.AreEqual (stitchesExpected, story.Stitches.Count);
		}

		[Test]
		public void CreatedAt ()
		{
			DateTime date = new DateTime (2015, 04, 21, 3, 09, 41, DateTimeKind.Utc);
			Assert.AreEqual (date, story.CreatedAt.ToUniversalTime ());
		}

		[Test]
		public void UpdatedAt ()
		{
			DateTime date = new DateTime (2015, 04, 21, 3, 36, 4, DateTimeKind.Utc);
			Assert.AreEqual (date, story.UpdatedAt.ToUniversalTime ());
		}

		[Test]
		public void Title ()
		{
			Assert.AreEqual ("Feature Test", story.Title);
		}

		[Test]
		public void Author ()
		{
			Assert.AreEqual ("Inkle", story.EditorData.AuthorName);
		}

		[Test]
		public void UrlKey ()
		{
			Assert.AreEqual ("bhwj", story.UrlKey);
		}

		[Test]
		public void AllowCheckpoints ()
		{
			Assert.IsTrue (story.AllowCheckpoints);
		}

		[Test]
		public void OptionMirroring ()
		{
			Assert.IsTrue (story.OptionMirroring);
		}

		[Test]
		[Ignore]
		public void InitialStitch ()
		{
			
		}

		[Test]
		[Ignore]
		public void PlayPoint ()
		{
			// Test the a stitch object referenced by PlayPoint
		}

		[Test]
		public void TextSize ()
		{
			Assert.AreEqual (EditorData.TextSizeType.Dense, story.EditorData.TextSize);
		}

		[Test]
		public void LibraryVisible ()
		{
			Assert.IsTrue (story.EditorData.LibraryVisible);
		}
	}
}

