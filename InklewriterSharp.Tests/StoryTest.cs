using NUnit.Framework;
using System;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class StoryTest
	{
		[SetUp]
		public void Setup ()
		{
		}

		[Test]
		public void EmptyConstructor ()
		{
			DateTime now = DateTime.Now;
			Story story = new Story ();

			Assert.IsFalse (story.AllowCheckpoints);
			var nowString = now.ToString ();
			var createdString = story.CreatedAt.ToString ();
			var updatedString = story.UpdatedAt.ToString ();
			Assert.AreEqual (createdString, nowString,
				string.Format ("Dates are not equal:  {0}  {1}", createdString, nowString));
			Assert.AreEqual (updatedString, nowString,
				string.Format ("Dates are not equal:  {0}  {1}", updatedString, nowString));
			Assert.IsNotNull (story.EditorData);
			Assert.IsNull (story.InitialStitch);
			Assert.IsEmpty (story.Stitches);
			Assert.IsNull (story.Title);
		}
	}
}

