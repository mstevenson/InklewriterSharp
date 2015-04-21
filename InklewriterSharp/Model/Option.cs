using System;
using System.Collections.Generic;

namespace Inklewriter
{
	[System.Serializable]
	public class Option
	{
		/// <summary>
		/// The option text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Display this option only if the specified markers have been set.
		/// </summary>
		public List<string> IfConditions { get; set; }

		/// <summary>
		/// Display this option only if the specified markers are not set.
		/// </summary>
		public List<string> NotIfConditions = new List<string> ();

		public Stitch LinkStitch { get; set; }

		public Stitch ParentStitch { get; set; }

		public Option ()
		{
			Text = "";
			IfConditions = new List<string> ();
			NotIfConditions = new List<string> ();
		}

		public Option (Stitch parent) : this()
		{
			ParentStitch = parent;
		}

		public void CreateLinkStitch (Stitch target)
		{
			if (target == null) {
				return;
			}
			if (LinkStitch == target) {
				return;
			}
			if (LinkStitch != null) {
				LinkStitch.RefCount--;
			}
			LinkStitch = target;
			LinkStitch.RefCount++;
		}

		public void Unlink ()
		{
			if (LinkStitch == null) {
				return;
			}
			LinkStitch.RefCount--;
			if (StoryModel.WatchRefCounts ()) {
				Console.WriteLine ("Unlinking " + Text + " - option on " + ParentStitch.Name);
			}
			LinkStitch = null;
		}
	}
}

