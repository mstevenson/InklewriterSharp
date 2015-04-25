﻿using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Inklewriter
{
	public class Player
	{
		public event Action OnReachedEnd;

		public Func<string, string> onStyledBold; // arg: text to style
		public Func<string, string> onStyledItalic; // arg: text to style
		public Func<string, string, string> onReplacedLinkUrl; // args: url, link text
		public Func<string, string> onReplacedImageUrl; // args: url

		public class PlayChunk
		{
			public List<Stitch> stitches = new List<Stitch> ();
			public List<FlagValue> flagsCollected = new List<FlagValue> ();
			
			internal void AddStitch (Stitch stitch)
			{
				stitches.Add (stitch);
			}
		}

		StoryModel model;

		public List<FlagValue> FlagsCollected { get; private set; }

		public Player (StoryModel model)
		{
			this.model = model;
			FlagsCollected = new List<FlagValue> ();
		}


		List<PlayChunk> e = new List<PlayChunk> (); // these should be stitches, not play chunks?
//		List<string> flagsCollected = new List<string> ();
		List<Stitch> visitedStitches = new List<Stitch> ();
//		PlayChunk playChunk;
//		PlayChunk prevChunk;
		int wordCount = 0;
		bool hadSectionHeading;

		bool isReadOnly;

		void VisitStitch (Stitch r)
		{
			var i = this;
			var t = false;

//			this.playChunk = null;
//			this.stitches = [];
//			this.optionBoxes = [];
			this.FlagsCollected = new List<FlagValue> ();
			this.wordCount = 0;
			this.hadSectionHeading = false;
//			this.flags = this.jqPlayChunk.find (".flags");
//			this.jqFlags.hide ();
//			this.prevChunk = e.last();
			if (this.prevChunk != null) {
				for (var s = 0; s < this.prevChunk.flagsCollected.Count; s++) {
					FlagsCollected.Add (this.prevChunk.flagsCollected [s]);
				}
			}
			if (r != null) {
				if (t) {
					// Display "the end"
					if (OnReachedEnd != null) {
						OnReachedEnd ();
					}
//					this.jqPlayChunk.html("<div class='the_end'>End</div>")
				} else {
					
					// remove expired page elements

//					$(".expired").remove()
//					this.jqPlayChunk.html("This page intentionally left blank.<br>(<a href='javascript:EditorMenu.enterEditMode();'>Continue the story from here</a>.)");
				}

				// Append play chunk
//				$("#read_area").append(this.jqPlayChunk);

				return;
			}

			var o = r;
//			this.jqTextBlock = this.jqPlayChunk.find(".stitch_block");
			var a = "";
			var f = "";

			// Loop through complete series of diverted stitches
			while (o != null) {
				this.visitedStitches.Add (o);
				if (o.PageNumber >= 1) {
					this.hadSectionHeading = true;
				}
				if (StoryModel.DoesArrayMeetConditions (o.IfConditions, o.NotIfConditions, FlagsCollected)) {
					if (!string.IsNullOrEmpty (o.Image)) {
						// embed image
						a += "\n%|%|%" + o.Image + "$|$|$\n";
					}
					// replace newlines with spaces
					f += o.Text.Replace("\n", " ") + " ";
					if (Regex.IsMatch (o.Text, @"\[\.\.\.\]") && !o.RunOn || o.DivertStitch == null) {
						// if there is no more text to display...
						a += ApplyRuleSubstitutions (f, FlagsCollected) + "\n";
						f = "";
					}
					if (o.Flags.Count > 0) {
						StoryModel.ProcessFlagSetting (o, this.FlagsCollected);
						if (!isReadOnly) {
//							var l = this.jqFlags.find("ul");
							for (var s = 0; s < o.Flags.Count; s++) {
								var h = o.FlagByIndex (s);
								var p = @"^(.*?)\s*(\+|\-)\s*(\b.*\b)\s*$";
								var matchSet = Regex.Match (h, p);
								if (matchSet.Success) {
									h += " (now " + StoryModel.GetValueOfFlag (matchSet.Groups[1].Value, this.FlagsCollected) + ")";
								}
//								l.append("<li>" + h + "</li>")
							}
//							this.jqFlags.show()
						}
					}
				}
				o = o.DivertStitch;
			}
			this.wordCount += WordCountOf(a);
//			this.jqTextBlock.html(u(a));
//			this.jqPlayChunk.append(this.jqTextBlock);
//			$("#read_area").append(this.jqPlayChunk);
			CreateOptionBlock ();
//			this.jqRewindButton = $('<div class="rewindButton" tooltip="Rewind to here"></div>');
//			this.jqPlayChunk.append(this.jqRewindButton);
//			this.jqRewindButton.bind("mousedown tap";
//				function() {
//					i.rewindToHere();
//					k();
//				});
//			this.jqRewindButton.hide();
//			if (e.length >= 1) {
//				b(this.jqPlayChunk);
//				if (t) {
//					this.jqRewindButton.addClass("noText");
//				}
//			} else {
//				this.jqRewindButton.addClass("initial");
//				t && this.jqRewindButton.text("Start again");
//			}
		}

		public void CreateOptionBlock ()
		{
//			var r = "<div class='option-divider'></div>";
//			this.jqOptBlock = $("<div class='option_block'>" + r + "</div>");
			if (visitedStitches [visitedStitches.Count - 1].Options.Count == 0) {
//				this.jqTextBlock.append('<div class="the_end">End</div>');
				if (isReadOnly) {
//					this.jqTextBlock.find(".the_end").append("<div class='back_to_top'></div>";
//					this.jqTextBlock.find(".back_to_top").bind("click tap", function() { b(e.first().jqPlayChunk); });
//					$("#read_area").append("<div id='madeby'>Text &copy; the author. <a href='http://www.inklestudios.com/inklewriter'><strong>inklewriter</strong></a> &copy; <a href='http://www.inklestudios.com'><strong>inkle</strong></a></div>"))
				} else {
//					this.jqTextBlock.append('<br>(<a href="javascript:EditorMenu.enterEditMode();">Go back to Write mode to continue</a>.)</div>');
				}
			} else {
				var i = visitedStitches [visitedStitches.Count - 1].Options;
				for (var o = 0; o < i.Count; o++) {
					var u = i [o];
					var a = StoryModel.DoesArrayMeetConditions(u.IfConditions, u.NotIfConditions, this.FlagsCollected);
					if (a || !isReadOnly) {
						if (a) {
							var f = CreateOptionButton (u, a);
//							this.optionBoxes.push(f), this.jqOptBlock.append(f.jqPlayOption), this.jqOptBlock.append(r)
						}
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