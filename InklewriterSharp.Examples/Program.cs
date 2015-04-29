using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.IO;
using Inklewriter;
using Inklewriter.Player;

namespace Inklewriter.Examples
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// Load story file
			string storyJson = File.ReadAllText ("Stories/musgraveritual.json");
			StoryModel model = StoryModel.Create (storyJson);

			// Load story into player
			StoryPlayer player = new StoryPlayer (model, new Inklewriter.MarkupConverters.ConsoleMarkupConverter ());

			// Display header
			Console.Clear ();
			Console.WriteLine ();
			Console.WriteLine (player.Title);
			Console.WriteLine ("by " + player.Author);

			// Main loop
			Stitch lastStitch = player.InitialStitch;
			while (lastStitch != null) {
				Console.WriteLine ();
				DrawHorizontalLine ();
				Console.WriteLine ();

				var nextChunk = player.GetChunkFromStitch (lastStitch);
				lastStitch = DisplayChunk (nextChunk);
			}
		}

		/// <summary>
		/// Takes a single stitch to display, and returns the stitch
		// /referenced by the player selected option if one is available.
		/// </summary>
		static Stitch DisplayChunk (PlayChunk chunk)
		{
			foreach (var paragraph in chunk.Paragraphs) {
				if (!string.IsNullOrEmpty (paragraph.Image)) {
					WriteImage (paragraph.Image);
					Console.WriteLine ();
				}
				WriteWrappedText (paragraph.Text);
				Console.WriteLine ();
			}

			// Find all available options
			var visibleOptions = chunk.Options.Where (o => o.isVisible).Select (o => o.content).ToList ();

			// Bail out if no options were visible and we have nowhere to go
			if (visibleOptions.Count == 0) {
				return null;
			}

			// Display options
			for (int i = 0; i < visibleOptions.Count; i++) {
				Console.WriteLine (string.Format ("{0}. {1}", (i + 1), visibleOptions[i].Text));
			}

			// Wait for input, return selected option's linked stitch
			int choice = -1;
			while (choice < 1 || choice > visibleOptions.Count) {
				var key = Console.ReadKey (true);
				int.TryParse (key.KeyChar.ToString (), out choice);
			}
			return visibleOptions [choice - 1].LinkStitch;
		}

		static void DrawHorizontalLine ()
		{
			for (int i = 0; i < Console.BufferWidth; i++) {
				Console.Write ("─");
			}
			Console.Write ("\n");
		}

		public static void WriteImage (string url)
		{
			Console.WriteLine (string.Format ("[Image: {0}]", url));
		}

		public static void WriteWrappedText (string text)
		{
			// Word wrap
			text = Regex.Replace (text, @"(.{" + (Console.BufferWidth - 20) + @"}[^\s]*)\s+", "$1\n");
			Console.WriteLine (text);
		}
	}
}
