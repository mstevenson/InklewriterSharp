using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

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
		Story story;
		Stitch currentStitch;



		public Player (Story story)
		{
			this.story = story;
		}

		public void Begin ()
		{
			ShowStitch (story.data.initial);
		}

		void ShowStitch (string id)
		{
			Stitch stitch;
			if (story.data.stitches.TryGetValue (id, out stitch)) {
				currentStitch = stitch;
//				if (stitch.flagNames != null) {
//					foreach (var f in stitch.flagNames) {
//						if (!flags.Contains (stitch);
//					}
//				}
//				if (stitch.options && stitch.options.Count > 0) {
//					ShowOptions (stitch.options);
//				}
				if (!string.IsNullOrEmpty (stitch.divert)) {
					ShowStitch (stitch.divert);
				}
			}
		}





		void ProcessFlags (List<string> flags)
		{
			foreach (var flag in flags) {
				var f = flag.ToLower ();
				var regex = new Regex (@"^(.*?)\s*(\=|\+|\-)\s*(\b.*\b)\s*$");
				regex.Match (flag);
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