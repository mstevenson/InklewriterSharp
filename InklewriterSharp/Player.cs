using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Inklewriter
{
	public class NumToWords
	{
		string[] digits = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
		string[] tens = new[] { "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
		string[] teens = new[] { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
		string[] illions = new[] { "thousand", "million", "billion", "trillion" };

		public void Convert (int num)
		{
			string text = "";
			if (num == 0) {
				text = "zero";
			} else if (num < 0) {
				text = "minus ";
				num = -num;
			}
		}
	}


	public class Player
	{
		public event Action<PlayChunk> OnChunkShown;

		public class PlayChunk
		{
			public List<Stitch> stitches;
			
			internal void AddStitch (Stitch stitch)
			{
				stitches.Add (stitch);
			}
		}

		StoryModel model;

		public List<string> FlagsCollected { get; private set; }

		public Player (StoryModel model)
		{
			this.model = model;
			FlagsCollected = new List<string> ();
		}

		public void Begin ()
		{
			ShowChunk (model.Story.InitialStitch);
		}

		PlayChunk ShowChunk (Stitch stitch)
		{
			PlayChunk chunk = new PlayChunk ();
			Stitch current = stitch;
			while (current != null) {
				ProcessFlags (current.Flags);
				chunk.AddStitch (current);
				current = current.DivertStitch;
			}
			return chunk;
		}


		public void SelectOption (int index)
		{
		}


		void ProcessFlags (List<string> flags)
		{
			if (flags == null) {
				return;
			}
			foreach (var flag in flags) {
				var f = flag.ToLower ();
				var regex = new Regex (@"^(.*?)\s*(\=|\+|\-)\s*(\b.*\b)\s*$");
				regex.Match (f);
//				bool equality = flag.Contains ("=");
//				bool addition = flag.Contains ("+");
//				bool subtraction = flag.Contains ("-");

//				if (equality || addition || subtraction) {
//					var f = flag.Replace (" ", "");
//					if (f.EndsWith ("=false")) {
//						flags.Remove (flag);
//					} else {
////						if (f.Contains ("+");
//					}
//				} else {
//
//				}
			}
		}

		void Test (string expression)
		{
			var regex = new Regex (@"^(.*?)\s*(\<|\>|\<\=|\>\=|\=|\!\=|\=\=)\s*(\b.*\b)\s*$");
			var match = regex.Match (expression);
			if (match.Success) {

			} else {
			}
		}

		string GetValueOfFlag (string expression)
		{
			return "";
		}

		void ShowOptions (List<Option> options)
		{
		}
		
	}
	
}