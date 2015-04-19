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
		public string option;

		/// <summary>
		/// The stitch to display after selecting this option.
		/// </summary>
		public string linkPath;

		/// <summary>
		/// Display this option only if the specified markers have been set.
		/// </summary>
		public List<string> ifConditions = new List<string> ();

		/// <summary>
		/// Display this option only if the specified markers are not set.
		/// </summary>
		public List<string> notIfConditions = new List<string> ();


		public Stitch LinkStitch { get; set; }

		public Stitch ParentStitch { get; set; }

		public string Text { get; set; }

		public Option (Stitch parent = null)
		{
			Text = "";
			ParentStitch = parent;
		}

		public void CreateLinkStitch (Stitch target)
		{
			if (target == null) {
				return;
			}
			if (LinkStitch != target) {
				if (LinkStitch != null) {
					LinkStitch.RefCount--;
				}
				LinkStitch = target;
				LinkStitch.RefCount++;
			}
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

