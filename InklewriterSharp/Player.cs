using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Inklewriter
{
	public class Player
	{
		public event Action OnReachedEnd;

		// In-line text styling callbacks
		public Func<string, string> onStyledBold; // arg: text to style
		public Func<string, string> onStyledItalic; // arg: text to style
		public Func<string, string, string> onReplacedLinkUrl; // args: url, link text
		public Func<string, string> onReplacedImageUrl; // args: url

		public class StitchBlock
		{
			public string image;
			public List<Stitch> stitches = new List<Stitch> ();
			public List<FlagValue> flagsCollected = new List<FlagValue> ();
			public string compiledText;
		}

		StoryModel model;

		public List<FlagValue> FlagsCollected { get; private set; }

		public Player (StoryModel model)
		{
			this.model = model;
			FlagsCollected = new List<FlagValue> ();
		}


		List<StitchBlock> e = new List<StitchBlock> (); // these should be stitches, not play chunks?
		List<Stitch> visitedStitches = new List<Stitch> ();
		StitchBlock prevChunk;
		int wordCount = 0;
		bool hadSectionHeading;

		StitchBlock TraverseStitch (Stitch stitch)
		{
			StitchBlock chunk = new StitchBlock ();

			this.FlagsCollected = new List<FlagValue> ();
			this.wordCount = 0;
			this.hadSectionHeading = false;
			if (this.prevChunk != null) {
				for (var s = 0; s < this.prevChunk.flagsCollected.Count; s++) {
					FlagsCollected.Add (this.prevChunk.flagsCollected [s]);
				}
			}

			var currentStitch = stitch;
			var compiledText = "";
			var newlineStripped = "";

			// Loop through complete series of stitches.
			while (currentStitch != null) {
				this.visitedStitches.Add (currentStitch);
				if (currentStitch.PageNumber >= 1) {
					this.hadSectionHeading = true;
				}
				// This stitch passes flag tests and should be included in this chunk
				if (StoryModel.DoesArrayMeetConditions (currentStitch.IfConditions, currentStitch.NotIfConditions, FlagsCollected)) {
					
					// Embed illustration image url
					if (!string.IsNullOrEmpty (currentStitch.Image)) {
						chunk.image = currentStitch.Image;
					}

					// Replace newlines with spaces
					newlineStripped += currentStitch.Text.Replace("\n", " ") + " ";

					// Stitch is not a run-on, or has no stitch to link to
					if (Regex.IsMatch (currentStitch.Text, @"\[\.\.\.\]") && !currentStitch.RunOn || currentStitch.DivertStitch == null) {
						// if there is no more text to display in this chunk...
						compiledText += ApplyRuleSubstitutions (newlineStripped, FlagsCollected) + "\n";
						newlineStripped = "";
					}

					// Process flags
					if (currentStitch.Flags.Count > 0) {
						StoryModel.ProcessFlagSetting (currentStitch, this.FlagsCollected);
					}

					// Add valid stitch to chunk
					chunk.stitches.Add (currentStitch);
				}
				currentStitch = currentStitch.DivertStitch;
			}
			this.wordCount += WordCountOf (compiledText);

			chunk.compiledText = compiledText;

			CreateOptionBlock ();
		}

		public void CreateOptionBlock ()
		{
//			var r = "<div class='option-divider'></div>";
//			this.jqOptBlock = $("<div class='option_block'>" + r + "</div>");
			if (visitedStitches [visitedStitches.Count - 1].Options.Count == 0) {
//				this.jqTextBlock.append('<div class="the_end">End</div>');
//				this.jqTextBlock.find(".the_end").append("<div class='back_to_top'></div>";
//				this.jqTextBlock.find(".back_to_top").bind("click tap", function() { b(e.first().jqPlayChunk); });
//				$("#read_area").append("<div id='madeby'>Text &copy; the author. <a href='http://www.inklestudios.com/inklewriter'><strong>inklewriter</strong></a> &copy; <a href='http://www.inklestudios.com'><strong>inkle</strong></a></div>"))
			} else {
				var i = visitedStitches [visitedStitches.Count - 1].Options;
				for (var o = 0; o < i.Count; o++) {
					var u = i [o];
					var a = StoryModel.DoesArrayMeetConditions(u.IfConditions, u.NotIfConditions, this.FlagsCollected);
					if (a) {
						var f = CreateOptionButton (u, a);
//						this.optionBoxes.push(f), this.jqOptBlock.append(f.jqPlayOption), this.jqOptBlock.append(r)
					}
				}
//				this.jqPlayChunk.append(this.jqOptBlock)
			}
//			$(".expired").remove()
		}

		// Should return the option button
		public static string CreateOptionButton (Option n, bool arrayMeetsConditions) // s method
		{
			return null;

//			this.jqPlayOption = $('<div class="option_button">' + p(n.text()) + "</div>");
//			this.linkTo = n.linkStitch();
//			var s = this.linkTo;
//			if (!n.writeModeOnly) {
//				if (i) {
//					this.jqPlayOption.bind("click tap", function() {
//						if (!t || e.last().hadSectionHeading && StoryModel.allowCheckpoints) {
//							e.last().jqRewindButton.show();
//						} else {
//							e.first().jqRewindButton.show();
//						}
//						var i = s;
//						$(".option_block").addClass("expired");
//						e.push(new r(i)), k(), y(), o(n);
//					});
//				}
//			} else {
//				this.jqPlayOption.addClass("disabled");
//				if (n.writeModeOnly) {
//					this.jqPlayOption.attr("tooltip", "Switch to write mode to continue.")
//				} else {
//					this.jqPlayOption.attr("tooltip", "This option has been disallowed by conditions.")	
//				}
//			}
		}

		public static int WordCountOf (string s)
		{
			if (!string.IsNullOrEmpty (s)) {
				return Regex.Matches (s, @"\S+").Count;
			}
			return 0;
		}
			
		string ApplyMarkupSubstitutions (string text)
		{
			text = ReplaceQuotes (text);
			text = ReplaceUrlMarkup (text);
			text = ReplaceImageMarkup (text);
			return text;
		}

		string ApplyRuleSubstitutions (string text, List<FlagValue> flags)
		{
			text = ReplaceRunOnMarker (text);
			text = ConvertNumberToWords (text, flags);
			string n = "";
			while (n != text) {
				n = text;
				text = ParseInLineConditionals (text, flags);
				text = ShuffleRandomElements (text);
			}
			text = ReplaceStyleMarkup (text);
			return text;
		}

		public static string ParseInLineConditionals (string text, List<FlagValue> flags)
		{
			var conditionBoundsPattern = @"\{([^\~\{]*?)\:([^\{]*?)(\|([^\{]*?))?\}";
			var orPattern = @"(^\s*|\s*$)";
			var andPattern = @"\s*(&&|\band\b)\s*";
			var notPattern = @"\s*(\!|\bnot\b)\s*(.+?)\s*$";
			var count = 0;
			var matches = Regex.Matches (text, conditionBoundsPattern);
			foreach (Match match in matches) {
				count++;
				if (count > 1000) {
					throw new System.Exception ("Error in conditional!");
				}
				if (matches.Count > 0) {
					var conditions = new List<string> ();
					var notConditions = new List<string> ();
					// Search "and" conditions
					var conditionMatches = Regex.Split (matches[1].Value, andPattern);
					for (var i = 0; i < conditionMatches.Length; i++) {
						// Is not an "and" condition
						if (conditionMatches [i] != "&&" && conditionMatches [i] != "and") {
							// Search "not" conditions
							var notPatternMatches = Regex.Match (conditionMatches [i], notPattern);
							// Is a "not condition"
							if (notPatternMatches.Success) {
								notConditions.Add (notPatternMatches.Groups [2].Value.Replace (orPattern, ""));
							} else {
								conditions.Add (conditionMatches [i].Replace (orPattern, ""));
							}
						}
					}
					var replacementValue = "";
					if (StoryModel.DoesArrayMeetConditions (conditions, notConditions, flags)) {
						replacementValue = matches [2].Value;
					} else if (!string.IsNullOrEmpty (matches [4].Value)) {
						replacementValue = matches [4].Value;
					}
					text = Regex.Replace (text, conditionBoundsPattern, " " + replacementValue + " ");
				}
			}
			return text;
		}

		public string ShuffleRandomElements (string text)
		{
			var pattern = @"\{\~([^\{\}]*?)\}";
			var matches = Regex.Matches (text, pattern);
			foreach (Match match in matches) {
				var group = match.Groups[1];
				var r = group.Value.Split ('|');
				var rand = new Random ();
				int i = rand.Next (0, r.Length);
				text = Regex.Replace (text, pattern, r [i]);
			}
			return text;
		}

		public string ReplaceRunOnMarker (string text)
		{
			text = Regex.Replace (text, @"\[\.\.\.\]", " ");
			return text;
		}

		public string ReplaceQuotes (string text)
		{
			// straight quotes to curly quotes
			text = Regex.Replace (text, @"\""([^\n]*?)\""", "“$1”");
			text = Regex.Replace (text, @"(\s|^|\n|<b>|<i>|\(|\“)\'", "$1‘");
			// straight apostrophe to curly apostrophe
			text = Regex.Replace (text, @"\'", "’");
			text = Regex.Replace (text, @"(^|\n)\""", "$1“");
			return text;
		}

		public string ReplaceStyleMarkup (string text)
		{
			// Replace inkle style markup with delegate method's output, or default to HTML tags
			text = Regex.Replace (text, @"\*\-(.*?)\-\*", onStyledBold != null ? onStyledBold ("$1") : "<b>$1</b>");
			text = Regex.Replace (text, @"\/\=(.*?)\=\/", onStyledItalic != null ? onStyledItalic ("$1") : "<i>$1</i>");
			// Remove inkle style markup
			text = Regex.Replace (text, @"(\/\=|\=\/|\*\-|\-\*)", "");
			return text;
		}

		public string ReplaceUrlMarkup (string e)
		{
			e = Regex.Replace (e, @"\[(.*?)\|(.*?)\]", onReplacedLinkUrl != null ? onReplacedLinkUrl ("$1", "$2") : "<a href=\"$1\">$2</a>");
			return e;
		}

		public string ConvertNumberToWords (string text, List<FlagValue> flags)
		{
			var pattern = @"\[\s*(number|value)\s*\:\s*(.*?)\s*\]";
			var matchSet = Regex.Matches (text, pattern);
			foreach (Match match in matchSet) {
				int number = StoryModel.GetValueOfFlag (match.Groups[2].Value, flags);
				string numberWords = number.ToString ();
				if (match.Groups[1].Value == "value") {
					numberWords = NumToWords.Convert (number);
				}
				text = Regex.Replace (text, pattern, numberWords);
			}
			return text;
		}

		public static int CalculateApproximateWordCount (List<Stitch> stitches)
		{
			var wordCount = 0;
			for (int i = 0; i < stitches.Count; i++) {
				wordCount += stitches [i].WordCount;
			}
			if (wordCount <= 100) {
				wordCount = wordCount - wordCount % 10 + 10;
			} else {
				wordCount = wordCount - wordCount % 100 + 100;
//				$("#wordcount").text("About " + commadString(n) + " words");
			}
			return wordCount;
		}

		public string ReplaceImageMarkup (string text)
		{
			text = Regex.Replace (text, @"\%\|\%\|\%(.*?)\$\|\$\|\$", (onReplacedImageUrl != null) ? onReplacedImageUrl ("$1") : "<div id=\"illustration\"><img class=\"pic\" src=\"$1\"/></div>");
			return text;
		}

	}
	
}