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
			List<FlagValue> flags = new List<FlagValue> {
				new FlagValue { flagName = "test a", value = 1 },
				new FlagValue { flagName = "test b", value = 2 },
				new FlagValue { flagName = "test c", value = 3 },
			};
			StoryModel model = new StoryModel ();

			Assert.AreEqual (1, model.GetIndexOfFlag ("Test B", flags));
		}

		[Test]
		public void ExtractFlagNameFromExpression ()
		{
			Assert.AreEqual ("test", StoryModel.ExtractFlagNameFromExpression ("Test = 3"));
		}

		[Test]
		public void GetValueOfFlag ()
		{
			List<FlagValue> flags = new List<FlagValue> {
				new FlagValue { flagName = "test a", value = 10 },
				new FlagValue { flagName = "test b", value = 20 },
				new FlagValue { flagName = "test c", value = 30 },
			};
			StoryModel model = new StoryModel ();

			Assert.AreEqual (20, model.GetValueOfFlag ("Test B", flags));
		}

		[Test]
		public void ProcessFlagSetting ()
		{
			StoryModel model = new StoryModel ();
			List<FlagValue> flags = new List<FlagValue> {
				new FlagValue ("a", true),
				new FlagValue ("b", 5),
				new FlagValue ("c", 10),
				new FlagValue ("d", false),
				new FlagValue ("e", true),
				new FlagValue ("f", 1),
				new FlagValue ("g", true),
			};
			Stitch stitch = new Stitch {
				Flags = new List<string> {
					"a = false", // set a boolean
					"b = 2", // set a number
					"c + 1", // add to a number
					"d = 7", // convert a boolean to a number
					"e + 1", // add a number to a boolean, which is weird, but valid
					"f + true", // add a boolean to a number, invalid
					"g + true" // add a boolean to a boolean, invalid
				}
			};

			model.ProcessFlagSetting (stitch, flags);

			Assert.AreEqual (0, flags [0].value);
			Assert.AreEqual (2, flags [1].value);
			Assert.AreEqual (11, flags [2].value);
			Assert.AreEqual (7, flags [3].value);
			Assert.AreEqual (2, flags [4].value);
			Assert.AreEqual (1, flags [5].value);
			Assert.AreEqual (1, flags [6].value);
		}

		[Test]
		[Ignore]
		public void CollateFlags ()
		{
			StoryModel model = new StoryModel ();
			model.CollateFlags ();
		}
	}

}

