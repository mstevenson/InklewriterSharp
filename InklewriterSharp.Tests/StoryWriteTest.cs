using NUnit.Framework;
using System;
using System.Collections.Generic;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class StoryWriteTest
	{
		Story story;

		[SetUp]
		public void CreateStory ()
		{
			var firstStitch = new Stitch () {
				Name = "firstStitch",
				Text = "This is the first stitch.",
				Image = "http://test.com/image.jpg",
				PageNumber = 1,
				PageLabel = "Beginning",
				RunOn = true,
//				DivertStitch = "secondStitch",
				Flags = new List<string> { "flag a", "flag b" }
			};

			var secondStitch = new Stitch () {
				Name = "secondStitch",
				Text = "Second stitch text.",
				IfConditions = new List<string> { "flag a" },
				NotIfConditions = new List<string> { "flag c" },
				Options = new List<Option> {
					new Option {
						Text = "Go To First",
						LinkStitch = firstStitch,
						IfConditions = new List<string> { "flag c" },
						NotIfConditions = null
					},
					new Option {
						Text = "Go To Third",
//						LinkStitch = thirdStitch,
					}
				}
			};

			var thirdStitch = new Stitch () {
				Name = "thirdStitch",
				Text = "This is stitch three.",
				Flags = new List<string> { "flag c" },
				PageNumber = 2,
				PageLabel = "Middle",
			};

			firstStitch.DivertStitch = secondStitch;
			secondStitch.Options [1].LinkStitch = thirdStitch;
		
			this.story = new Story {
				Title = "Test Story",
				UrlKey = "abcd",
				CreatedAt = new DateTime (2015, 01, 02, 15, 10, 5, DateTimeKind.Utc),
				UpdatedAt = new DateTime (2015, 02, 03, 4, 10, 20, DateTimeKind.Utc),
				EditorData = new EditorData {
					AuthorName = "inkle",
					LibraryVisible = true,
					PlayPoint = secondStitch,
					TextSize = EditorData.TextSizeType.Dense
				},
				InitialStitch = firstStitch,
				AllowCheckpoints = true,
				OptionMirroring = true,
				Stitches = new List<Stitch> { firstStitch, secondStitch, thirdStitch }
			};
		}

		[Test]
		public void Title ()
		{
			string data = StoryWriter.Write (story);
			JsonObject obj = (JsonObject)SimpleJson.DeserializeObject (data);

			Assert.AreEqual ("Test Story", obj ["title"]);
		}

		[Test]
		public void UrlKey ()
		{
			string data = StoryWriter.Write (story);
			JsonObject obj = (JsonObject)SimpleJson.DeserializeObject (data);

			Assert.AreEqual ("abcd", obj ["url_key"]);
		}

		[Test]
		public void Author ()
		{
			string data = StoryWriter.Write (story);
			JsonObject obj = (JsonObject)SimpleJson.DeserializeObject (data);

			var d = (JsonObject)obj["data"];
			var e = (JsonObject)d["editorData"];
			Assert.AreEqual ("inkle", e["authorName"]);
		}
	}
}
