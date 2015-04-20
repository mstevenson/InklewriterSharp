using NUnit.Framework;
using System;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class StitchTest
	{
		[SetUp]
		public void Setup ()
		{
		}

		[Test]
		public void EmptyConstructor ()
		{
			Stitch stitch = new Stitch ();
			
			Assert.IsEmpty (stitch.Backlinks);
			Assert.IsNull (stitch.DivertStitch);
			Assert.IsEmpty (stitch.Flags);
			Assert.IsEmpty (stitch.IfConditions);
			Assert.IsNull (stitch.Image);
			Assert.IsNull (stitch.Name);
			Assert.IsEmpty (stitch.NotIfConditions);
			Assert.IsEmpty (stitch.Options);
			Assert.IsNull (stitch.PageLabel);
			Assert.AreEqual (-1, stitch.PageNumber);
			Assert.AreEqual (0, stitch.RefCount);
			Assert.IsFalse (stitch.RunOn);
			Assert.IsNull (stitch.Text);
			Assert.AreEqual (0, stitch.VerticalDistance);
			Assert.AreEqual (0, stitch.VerticalDistanceFromPageNumberHeader);
			Assert.AreEqual (0, stitch.WordCount);
		}

		[Test]
		public void ConstructorWithString ()
		{
			string body = "This is a stitch.";
			Stitch stitch = new Stitch (body);
			Assert.AreEqual (stitch.Text, body);
		}

		[Test]
		public void AutomaticPageLabel ()
		{
			Stitch stitch = new Stitch ();
			stitch.PageNumber = 3;
			Assert.AreEqual ("Section 3", stitch.PageLabel);
		}

		[Test]
		public void ExplicitPageLabel ()
		{
			Stitch stitch = new Stitch ();
			string originalLabel = "Section 3";
			string customLabel = "Custom Label";
			stitch.PageNumber = 3;
			stitch.PageLabel = customLabel;
			Assert.AreEqual (customLabel, stitch.PageLabel);
			stitch.PageLabel = "";
			Assert.AreEqual (originalLabel, stitch.PageLabel);
			stitch.PageLabel = null;
			Assert.AreEqual (originalLabel, stitch.PageLabel);
		}

		[Test]
		public void WordCount ()
		{
			Stitch stitch = new Stitch ();
			stitch.Text = "One two three.";
			Assert.AreEqual (stitch.WordCount, 3);
		}

		[Test]
		public void IsDead ()
		{
			Stitch stitch = new Stitch ();
			Assert.IsTrue (stitch.IsDead);
			stitch.Text = "Text";
			Assert.IsFalse (stitch.IsDead);
		}

		[Test]
		public void ShortName ()
		{
			Stitch stitch = new Stitch ();
			string expected = "blankStitch";
			string name = stitch.CreateShortName ();
			Assert.AreEqual (expected, name);

			stitch.Text = "!!!";
			expected = "punctuatedStitch";
			name = stitch.CreateShortName ();
			Assert.AreEqual (expected, name);

			stitch.Text = "A brand! NEW short name.";
			expected = "aBrandNewShortNa";
			name = stitch.CreateShortName ();
			Assert.AreEqual (expected, name,
				string.Format ("Expected short name '{0}', found '{1}'", expected, stitch.Name));
		}
	}
}

