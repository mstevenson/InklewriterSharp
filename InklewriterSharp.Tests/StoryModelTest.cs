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
				new FlagValue ("test a", 1),
				new FlagValue ("test b", 2),
				new FlagValue ("test c", 3),
			};

			Assert.AreEqual (1, StoryModel.GetIndexOfFlag ("Test B", flags));
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
				new FlagValue ("test a", 10),
				new FlagValue ("test b", 20),
				new FlagValue ("test c", 30),
			};

			Assert.AreEqual (20, StoryModel.GetValueOfFlag ("Test B", flags));
		}

		[Test]
		public void ProcessFlagSetting ()
		{
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

			StoryModel.ProcessFlagSetting (stitch, flags);

			Assert.AreEqual (0, flags [0].value);
			Assert.AreEqual (2, flags [1].value);
			Assert.AreEqual (11, flags [2].value);
			Assert.AreEqual (7, flags [3].value);
			Assert.AreEqual (2, flags [4].value);
			Assert.AreEqual (1, flags [5].value);
			Assert.AreEqual (1, flags [6].value);
		}

		[Test]
		public void Test ()
		{
			List<FlagValue> flags = new List<FlagValue> {
				new FlagValue ("a", false),
				new FlagValue ("b", true),
				new FlagValue ("c", 10),
			};
			Assert.IsTrue (StoryModel.Test ("a == false", flags));
			Assert.IsFalse (StoryModel.Test ("a == true", flags));
			Assert.IsTrue (StoryModel.Test ("b == true", flags));
			Assert.IsFalse (StoryModel.Test ("b == false", flags));
			Assert.IsTrue (StoryModel.Test ("b == 1", flags));
			Assert.IsFalse (StoryModel.Test ("b == 2", flags));
			Assert.IsTrue (StoryModel.Test ("c == 10", flags));
			Assert.IsFalse (StoryModel.Test ("c != 10", flags));
			Assert.IsTrue (StoryModel.Test ("c < 11", flags));
			Assert.IsTrue (StoryModel.Test ("c > 9", flags));
			Assert.IsFalse (StoryModel.Test ("c > 11", flags));
			Assert.IsFalse (StoryModel.Test ("c < 9", flags));
			Assert.IsTrue (StoryModel.Test ("c <= 10", flags));
			Assert.IsTrue (StoryModel.Test ("c >= 10", flags));

			// TODO create a custom exception type
			Assert.Throws<System.Exception> (() => StoryModel.Test ("a < true", flags));
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

