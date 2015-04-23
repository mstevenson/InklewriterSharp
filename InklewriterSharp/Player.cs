using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Inklewriter
{
	public class Player
	{
		public event Action<PlayChunk> OnChunkShown;


		public class PlayChunk
		{
			public List<Stitch> stitches = new List<Stitch> ();
			public List<string> flagsCollected = new List<string> ();
			
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






//		List<PlayChunk> e = new List<PlayChunk> ();
//		List<string> flagsCollected = new List<string> ();
//		List<Stitch> stitches = new List<Stitch> ();
//		PlayChunk playChunk;
//		PlayChunk prevChunk;
//		int wordCount = 0;
//		bool hadSectionHeading;
//
		StoryModel storyModel;
//
//
//		// initialize method?
//		void r (Stitch r)
//		{
//			var i = this;
//			var t = false;
//
//			this.playChunk = null;
////			this.stitches = [];
////			this.optionBoxes = [];
////			this.flagsCollected = [];
//			this.wordCount = 0;
//			this.hadSectionHeading = false;
////			this.flags = this.jqPlayChunk.find (".flags");
////			this.jqFlags.hide ();
////			this.prevChunk = e.last();
//			if (this.prevChunk) {
//				for (var s = 0; s < this.prevChunk.flagsCollected.Count; s++) {
//					i.flagsCollected.Add (this.prevChunk.flagsCollected [s]);
//				}
//			}
//			if (r != null) {
//				if (t) {
////					this.jqPlayChunk.html("<div class='the_end'>End</div>")
//				} else {
////					n(); // remove expired page elements
////					this.jqPlayChunk.html("This page intentionally left blank.<br>(<a href='javascript:EditorMenu.enterEditMode();'>Continue the story from here</a>.)");
//				}
////				$("#read_area").append(this.jqPlayChunk);
//				return;
//			}
//			var o = r;
////			this.jqTextBlock = this.jqPlayChunk.find(".stitch_block");
//			var a = "";
//			var f = "";
//			while (o) {
//				this.stitches.Add (o);
//				if (o.PageNumber >= 1) {
//					this.hadSectionHeading = true;
//				}
//				if (storyModel.DoesArrayMeetConditions (o.IfConditions, o.NotIfConditions, this.flagsCollected)) {
//					if (!string.IsNullOrEmpty (o.Image)) {
//						(a += "\n%|%|%" + o.Image + "$|$|$\n");
//					}
//					f += o.Text.Replace("\n", " ") + " ";
//					if (Regex.IsMatch (o.Text, @"\[\.\.\.\]") && !o.RunOn || !o.DivertStitch) {
//						a += c (f, this.flagsCollected) + "\n";
//					}
//					f = "";
//					if (o.Flags.Count > 0) {
//						storyModel.ProcessFlagSetting (o, this.flagsCollected);
//						if (!t) {
////							var l = this.jqFlags.find("ul");
//							for (var s = 0; s < o.Flags.Count; s++) {
//								var h = o.FlagByIndex(s),
//								p = @"^(.*?)\s*(\+|\-)\s*(\b.*\b)\s*$";
//								var matchSet = Regex.Match (h, p);
//								if (matchSet.Success) {
//									h += " (now " + storyModel.GetValueOfFlag(matchSet.Groups[1]);
//								}
//								this.flagsCollected) + ")";
////								l.append("<li>" + h + "</li>")
//							}
////							this.jqFlags.show()
//						}
//					}
//				}
//				o = o.DivertStitch;
//			}
//			this.wordCount += wordCountOf(a);
//			this.jqTextBlock.html(u(a));
//			this.jqPlayChunk.append(this.jqTextBlock);
//			$("#read_area").append(this.jqPlayChunk);
//			this.createOptionBlock();
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
//		}

//		void o (Stitch t)
//		{
//			if (t.Text != "..." && storyModel.Story.OptionMirroring && e.last().jqPlayChunk.prepend('<div class="option_chosen">' + p(t.text()) + "</div>")
//		}


		void ParseInLineConditionals ()
		{
//			var n = /\{([^\~\{]*?)\:([^\{]*?)(\|([^\{]*?))?\}/,
//				r = /(^\s*|\s*$)/g,
//				i = /\s*(&&|\band\b)\s*/,
//				s = /\s*(\!|\bnot\b)\s*(.+?)\s*$/,
//				o, u = 0;
//				while (o = e.match(n)) {
//					u++;
//					if (u > 1e3) {
//						alert("Error in conditional!");
//						break
//					}
//					if (o.length > 0) {
//						var a = "",
//						f = [],
//						l = [],
//						c = o[1].split(i);
//						for (var h = 0; h < c.length; h++)
//							if (c[h] != "&&" && c[h] != "and") {
//								var p = c[h].match(s);
//								p ? l.push(p[2].replace(r, "")) : f.push(c[h].replace(r, ""))
//							}
//						StoryModel.doesArrayMeetConditions(f, l, t) ? a = o[2] : o[4] !== undefined && (a = o[4]), e = e.replace(n, " " + a + " ")
//					}
//				}
//				return e
		}

		public void ShuffleRandomOptions ()
		{
//			var t = /\{\~([^\{\}]*?)\}/,
//			n;
//			while (n = e.match(t)) {
//				var r = n[1].split("|"),
//				i = parseInt(Math.random() * r.length);
//				e = e.replace(t, r[i])
//			}
//			return e
		}

		public void ReplaceBoldAndItalicMarkup ()
		{
//			return e = e.replace(/\*\-(.*?)\-\*/g, "<b>$1</b>"), e = e.replace(/\/\=(.*?)\=\//g, "<i>$1</i>"), e = e.replace(/(\/\=|\=\/|\*\-|\-\*)/g, ""), e
		}

		public void ReplaceRunOnMarker ()
		{
//			return e.replace(/\[\.\.\.\]/g, " ")
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

		public string ConvertNumberToWords (string text, List<FlagValue> flags)
		{
			var pattern = @"\[\s*(number|value)\s*\:\s*(.*?)\s*\]";
			var matchSet = Regex.Match (text, pattern);
			foreach (var match in matchSet.Groups) {
				int number = StoryModel.GetValueOfFlag (matchSet.Groups[2].Value, flags);
				string numberWords = number.ToString ();
				if (matchSet.Groups[1].Value == "value") {
					numberWords = NumToWords.Convert (number);
				}
				text = Regex.Replace (text, pattern, numberWords);
			}
			return text;
		}

	}
	
}