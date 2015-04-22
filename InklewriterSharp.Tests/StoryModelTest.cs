using NUnit.Framework;
using System;
using System.Collections.Generic;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class StoryModelTest
	{
		[Test]
		public void AddFlagToIndex ()
		{
			StoryModel model = new StoryModel ();
			model.AddFlagToIndex ("Test = 1");

			Assert.AreEqual (1, model.FlagIndex.Count);
			Assert.AreEqual ("test", model.FlagIndex [0]);
		}

		[Test]
		public void GetIndexOfFlag ()
		{
			List<StoryModel.FlagValue> flags = new List<StoryModel.FlagValue> {
				new StoryModel.FlagValue { flagName = "test a", value = 1 },
				new StoryModel.FlagValue { flagName = "test b", value = 2 },
				new StoryModel.FlagValue { flagName = "test c", value = 3 },
			};
			StoryModel model = new StoryModel ();

			Assert.AreEqual (1, model.GetIndexOfFlag ("Test B", flags));
		}

		[Test]
		public void ExtractFlagNameFromExpression ()
		{
			Assert.AreEqual ("test", StoryModel.ExtractFlagNameFromExpression ("Test = 3"));
		}
	}

}

