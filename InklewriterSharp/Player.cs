using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System;
using Inklewriter.MarkupConverters;

namespace Inklewriter
{
	public class BlockContent<T>
	{
		public T content;
		public bool isVisible;

		public BlockContent (T content, bool isVisible)
		{
			this.content = content;
			this.isVisible = isVisible;
		}
	}

	/// <summary>
	/// A series of stitches ending in a block of selectable options. Includes an optional illustration image.
	/// </summary>
	public class PlayChunk
	{
		/// <summary>
		/// In-line illustration image URL.
		/// </summary>
		public string image;

		/// <summary>
		/// All stitches belonging to this play chunk. Stitches that pass flag validation
		/// will have the isVisible value set to true.
		/// The text from all visible stitches will be processed, styled, and stored in compiledText.
		/// </summary>
		public List<BlockContent<Stitch>> stitches = new List<BlockContent<Stitch>> ();

		/// <summary>
		/// All flags recorded during play up to and including to this chunk.
		/// </summary>
		public List<FlagValue> flagsCollected = new List<FlagValue> ();

		/// <summary>
		/// Body text compiled from all stitches belonging to this chunk, post-styling.
		/// Stitches that do not pass flag validation will not be included in this text.
		/// </summary>
		public string compiledText;
	}

	public class Player
	{
		public List<FlagValue> AllFlagsCollected { get; private set; }

		StoryModel model;
		IMarkupConverter markupConverter;

		public Player (StoryModel model, IMarkupConverter markupConverter)
		{
			this.model = model;
			this.markupConverter = markupConverter;
			AllFlagsCollected = new List<FlagValue> ();
		}

		List<PlayChunk> e = new List<PlayChunk> (); // these should be stitches, not play chunks?
		List<Stitch> visitedStitches = new List<Stitch> ();
		PlayChunk prevChunk;
		int wordCount = 0;
		bool hadSectionHeading;

		PlayChunk TraverseStitch (Stitch stitch)
		{
			PlayChunk chunk = new PlayChunk ();

			this.AllFlagsCollected = new List<FlagValue> ();
			this.wordCount = 0;
			this.hadSectionHeading = false;
			if (this.prevChunk != null) {
				for (var s = 0; s < this.prevChunk.flagsCollected.Count; s++) {
					AllFlagsCollected.Add (this.prevChunk.flagsCollected [s]);
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
				bool isStitchVisible = false;
				// This stitch passes flag tests and should be included in this chunk
				if (StoryModel.DoesArrayMeetConditions (currentStitch.IfConditions, currentStitch.NotIfConditions, AllFlagsCollected)) {

					isStitchVisible = true;

					// Embed illustration image url
					if (!string.IsNullOrEmpty (currentStitch.Image)) {
						chunk.image = currentStitch.Image;
					}

					// Replace newlines with spaces
					newlineStripped += currentStitch.Text.Replace("\n", " ") + " ";

					// Stitch is not a run-on, or has no stitch to link to
					if (Regex.IsMatch (currentStitch.Text, @"\[\.\.\.\]") && !currentStitch.RunOn || currentStitch.DivertStitch == null) {
						// if there is no more text to display in this chunk...
						compiledText += ApplyRuleSubstitutions (newlineStripped, AllFlagsCollected) + "\n";
						newlineStripped = "";
					}

					// Process flags
					if (currentStitch.Flags.Count > 0) {
						StoryModel.ProcessFlagSetting (currentStitch, this.AllFlagsCollected);
					}
				}
				// Add stitch to chunk
				chunk.stitches.Add (new BlockContent<Stitch> (currentStitch, isStitchVisible));
				currentStitch = currentStitch.DivertStitch;
			}
			this.wordCount += WordCountOf (compiledText);

			chunk.compiledText = compiledText;

			CreateOptionBlock ();

			return chunk;
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
					var a = StoryModel.DoesArrayMeetConditions(u.IfConditions, u.NotIfConditions, this.AllFlagsCollected);
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
			text = Regex.Replace (text, @"\*\-(.*?)\-\*", markupConverter.ReplaceBoldStyleMarkup ("$1"));
			text = Regex.Replace (text, @"\/\=(.*?)\=\/", markupConverter.ReplaceItalicStyleMarkup ("$1"));
			// Remove inkle style markup
			text = Regex.Replace (text, @"(\/\=|\=\/|\*\-|\-\*)", "");
			return text;
		}

		public string ReplaceUrlMarkup (string e)
		{
			e = Regex.Replace (e, @"\[(.*?)\|(.*?)\]", markupConverter.ReplaceLinkUrlMarkup ("$1", "$2"));
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
			text = Regex.Replace (text, @"\%\|\%\|\%(.*?)\$\|\$\|\$", markupConverter.ReplaceImageUrlMarkup ("$1"));
			return text;
		}

	}
	
}