using NUnit.Framework;
using System;
using Inklewriter;

namespace Inklewriter.Tests
{
	[TestFixture]
	public class EditorDataTest
	{
		[Test]
		[Ignore]
		public void Constructor ()
		{
			EditorData data = new EditorData ();

			Assert.IsNull (data.AuthorName);
			Assert.IsFalse (data.LibraryVisible);
			Assert.IsNull (data.PlayPoint);
			Assert.AreEqual (0, data.TextSize);
		}
	}
}
