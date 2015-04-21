using NUnit.Framework;
using System;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class OptionTest
	{
		[Test]
		public void EmptyConstructor ()
		{
			Option option = new Option ();

			Assert.IsNullOrEmpty (option.Text);
			Assert.IsNotNull (option.IfConditions);
			Assert.IsNotNull (option.NotIfConditions);
			Assert.IsNull (option.LinkStitch);
		}

		[Test]
		public void ConstructorWithStitch ()
		{
			Stitch stitch = new Stitch ();
			Option option = new Option (stitch);

			Assert.AreSame (stitch, option.ParentStitch);
		}

		[Test]
		public void CreateLinkStitch ()
		{
			Option option = new Option ();

			Stitch linkStitch = new Stitch ();
			option.CreateLinkStitch (linkStitch);

			Assert.AreSame (linkStitch, option.LinkStitch);
		}

		[Test]
		public void CreateLinkStitchIncrementsRefCount ()
		{
			Option option = new Option ();

			Stitch linkStitch = new Stitch ();
			option.CreateLinkStitch (linkStitch);

			Assert.AreEqual (1, linkStitch.RefCount);
		}

		[Test]
		public void Unlink ()
		{
			Option option = new Option ();

			Stitch linkStitch = new Stitch ();
			option.CreateLinkStitch (linkStitch);
			option.Unlink ();

			Assert.IsNull (option.LinkStitch);
		}

		[Test]
		public void UnlinkDecrementsRefCount ()
		{
			Option option = new Option ();

			Stitch linkStitch = new Stitch ();
			option.CreateLinkStitch (linkStitch);
			option.Unlink ();

			Assert.AreEqual (0, linkStitch.RefCount);
		}
	}
}

