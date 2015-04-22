using NUnit.Framework;
using System;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class StoryModelTest
	{
		[Test]
		public void ExtractFlagNameFromExpression ()
		{
			Assert.AreEqual ("test", StoryModel.ExtractFlagNameFromExpression ("Test = 3"));
		}
	}

}

