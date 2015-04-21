using System;
using System.IO;
using Inklewriter;

namespace Inklewriter.Examples
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string storyJson = File.ReadAllText ("Stories/tutorial.json");
			StoryModel model = new StoryModel ();
			model.ImportStory (storyJson);

			Console.Clear ();
			Console.WriteLine ();
			Console.WriteLine (model.Story.Title);
			Console.WriteLine ("by " + model.Story.EditorData.AuthorName);

			Stitch selectedStitch = model.Story.InitialStitch;
			while (selectedStitch != null) {
				Console.WriteLine ();
				HorizontalLine ();
				Console.WriteLine ();

				selectedStitch = DisplayAllStitches (selectedStitch);
			}
		}

		/// <summary>
		/// Display a series of stitches up to a branch.
		/// </summary>
		static Stitch DisplayAllStitches (Stitch stitch)
		{
			Stitch divert = stitch;
			Stitch lastStitch = null;
			while (divert != null) {
				lastStitch = DisplayStitch (divert);
				divert = divert.DivertStitch;
			}
			return lastStitch;
		}

		/// <summary>
		/// Takes a single stitch to display, and returns the stitch
		// /referenced by the player selected option if one is available.
		/// </summary>
		static Stitch DisplayStitch (Stitch stitch)
		{
			if (stitch.RunOn) {
				Console.Write (" " + stitch.Text);
			} else {
				Console.WriteLine (stitch.Text);
				Console.WriteLine ();
			}
			return PresentOptions (stitch);
		}

		/// <summary>
		/// Presents a series of branching options for the given stitch, if available.
		/// </summary>
		static Stitch PresentOptions (Stitch stitch)
		{
			// Check for options
			int count = stitch.Options.Count;
			if (count == 0) {
				return null;
			}
			// Draw options
			for (int i = 0; i < count; i++) {
				var opt = stitch.Options [i];
				Console.WriteLine (string.Format ("{0}. {1}", (i + 1), opt.Text));
			}
			// Wait for input, return stitch for valid selected option
			int choice = -1;
			while (choice < 1 || choice > stitch.Options.Count) {
				var key = Console.ReadKey (true);
				int.TryParse (key.KeyChar.ToString (), out choice);
			}
			return stitch.Options [choice - 1].LinkStitch;
		}

		static void HorizontalLine ()
		{
			for (int i = 0; i < Console.BufferWidth; i++) {
				Console.Write ("─");
			}
			Console.Write ("\n");
		}
	}
}
