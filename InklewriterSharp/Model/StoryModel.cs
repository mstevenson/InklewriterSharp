using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Inklewriter
{

	public class FlagValue {
		public string flagName;
		public bool value;
	}


	public class StoryModel
	{
		const string defaultStoryName = "Untitled Story";
		const string defaultAuthorName = "Anonymous";
		const int maxPreferredPageLength = 8;

		public Story Story { get; private set; }

		List<string> flagIndex = new List<string> ();

		public void ImportStory (string data)
		{
			Story = StoryIO.Read (data);
		}

		public string ExportStory ()
		{
			if (Story != null) {
				var data = StoryIO.Write (Story);
				return data;
			}
			return null;
		}

		public void NameStitches ()
		{
			HashSet<string> usedShortNames = new HashSet<string> ();
			var stitches = Stitches;
			foreach (var currentStitch in stitches) {
				string shortName = currentStitch.CreateShortName ();
				string incrementedShortName = shortName;
				for (int num = 1; usedShortNames.Contains (shortName); num++) {
					incrementedShortName = shortName + num;
				}
				shortName = incrementedShortName;
				usedShortNames.Add (shortName);
				currentStitch.SetName (shortName);
			}
		}

		int EndCount { get; set; }

		public void RebuildBacklinks ()
		{
			EndCount = 0;
			var stitches = Stitches;
			for (int e = 0; e < stitches.Count; e++) {
				stitches [e].Backlinks = new List<Stitch> ();
			}
			for (int e = 0; e < stitches.Count; e++)
				if (stitches [e].options.Count > 0) {
					for (var t = 0; t < stitches [e].options.Count; t++) {
						if (stitches [e].options [t].LinkStitch != null) {
							stitches [e].options [t].LinkStitch.Backlinks.Add (stitches [e]);
						} else {
							LooseEndCount++;
						}
					}
				} else {
					if (stitches [e].DivertedStitch != null) {
						stitches [e].DivertedStitch.Backlinks.Add (stitches [e]);
					} else {
						EndCount++;
					}
				}
			if (WatchRefCounts ()) {
				for (int e = 0; e < stitches.Count; e++) {
					if (stitches[e].Backlinks.Count != stitches[e].RefCount) {
						throw new System.Exception ("Stitch with text '" + stitches[e].text + "' has invalid ref-count!");
					}
				}
			}
		}

		int LooseEndCount { get; set; }

		public void RepointStitchToStitch (Stitch source, Stitch target)
		{
			if (WatchRefCounts ()) {
				Console.WriteLine ("Repointing stitch links from " + source.Name + " to " + (target != null ? target.Name : "to null."));
			}
			var stitches = Stitches;
			for (int n = 0; n < stitches.Count; n++) {
				var r = stitches[n];
				if (r.DivertedStitch == source) {
					r.Undivert ();
					if (target != null) {
						r.Divert (target, true);
					}
				}
				for (var i = 0; i < r.options.Count; i++) {
					if (r.options[i].LinkStitch == source) {
						r.options [i].Unlink ();
						if (target != null) {
							r.options [i].CreateLinkStitch (target);
						}
					}
				}
			}
		}

		public static bool WatchRefCounts ()
		{
			// FIXME
			return false;
		}

		public static string ExtractFlagNameFromExpression (string expression)
		{
			var regex = new Regex (@"^(.*?)\s*(\=|\+|\-|\>|\<|\!\=|$)");
			var match = regex.Match (expression);
			return match.Captures[1].Value;
		}

		public List<Stitch> Stitches {
			get {
				return Story.data.stitches;
			}
		}

		public void CollateFlags (Story story)
		{
			flagIndex = new List<string> ();
			var stitches = Stitches;
			for (int e = 0; e < stitches.Count; e++) {
				var t = stitches[e];
				for (int n = 0; n < t.flagNames.Count; n++) {
					AddFlagToIndex (t.flagNames[n]);
				}
				for (int r = 0; r < t.options.Count; r++) {
					for (int n = 0; n < t.options [r].ifConditions.Count; n++) {
						AddFlagToIndex (t.options[r].ifConditions[n]);
					}
					for (int n = 0; n < t.options [r].notIfConditions.Count; n++) {
						AddFlagToIndex (t.options [r].notIfConditions [n]);
					}
				}
				for (var n = 0; n < t.ifConditions.Count; n++) {
					AddFlagToIndex (t.ifConditions[n]);
				}
				for (var n = 0; n < t.notIfConditions.Count; n++) {
					AddFlagToIndex (t.notIfConditions [n]);
				}
			}
		}

		public int GetIdxOfFlag (string flag, List<FlagValue> allFlags)
		{
			// FIXME allFlags should be special flag objects that have a 'value'

			for (var n = 0; n < allFlags.Count; n++) {
				if (allFlags[n].flagName == flag) {
					return n;
				}
			}
			return -1;
		}

		public void ProcessFlagSetting (Stitch stitch, List<FlagValue> allFlags) // t == all flags
		{
			for (int n = 0; n < stitch.NumberOfFlags; n++) {
				string r = stitch.FlagByIndex (n);
				bool i = true;
				bool s = false;
				Console.WriteLine ("Flag directive: " + r);
				var o = new Regex (@"^(.*?)\s*(\=|\+|\-)\s*(\b.*\b)\s*$");
				s = o.IsMatch (r);
				int u = -1;
				if (s) {
					r = s[1];
					u = GetIdxOfFlag(r, allFlags);
					var m = new Regex (@"\d+");
					if (m.IsMatch (s [3])) {
						if (s [2] == "=") {
							i = ParseInt(s[3]);
						}
					} else {
						if (u < 0) {
							i = 0;
						} else {
							i = allFlags[u].value;
						}
						if (s[2] == "+") {
							i += ParseInt(s[3]);
						} else {
							i -= ParseInt(s[3]);
						}
					}
					if (s[2] == "=") {
						i = ConvertStringToBooleanIfAppropriate (s[3]);
					} else {
						Console.WriteLine ("Can't add/subtract a boolean.");
					}
				} else {
					u = GetIdxOfFlag(r, allFlags);
				}
				Console.WriteLine ("Assigning value: " + i);
				if (u >= 0) {
					allFlags.RemoveAt(u);
				}
				var a = new FlagValue {
					flagName = r,
					value = i
				};
				allFlags.Add(a);
			}
		}

		int ParseInt (string s)
		{
		}

		public bool Test (string expression, List<FlagValue> allFlags) // e == flag
		{
			var regex = new Regex (@"^(.*?)\s*(\<|\>|\<\=|\>\=|\=|\!\=|\=\=)\s*(\b.*\b)\s*$");
			var result = false;
			var matches = regex.Matches (expression);
			if (regex.IsMatch (expression)) {
				var flag = matches [1];
				var op = matches [2];
				var value = matches [3];

				var flagValue = GetValueOfFlag (flag, allFlags);
				Console.WriteLine ("Testing " + flagValue + " " + op + " " + value);
				if (op == "==" || op == "=") {
					result = flagValue == value;
				} else {
					if (op == "!=" || op == "<>") {
						result = flagValue != matches[3];
					} else {
						if (Regex.IsMatch (value, @"\d+")) {
							throw new System.Exception ("Error - Can't perform an order-test on a boolean.");
						}
						if (matches[2] == "<") {
							result = flagValue < value;
						} else if (matches[2] == "<=") {
							result = flagValue <= value;
						} else if (matches[2] == ">") {
							result = flagValue > value;
						} else if (matches[2] == ">=") {
							result = flagValue >= value;
						}
					}
				}
			} else {
				result = StoryModel.GetValueOfFlag (expression, allFlags);
				result = ConvertStringToBooleanIfAppropriate(result);
				if (result == 0 || result == -1) {
					result = !1;
				}
				result = true; // FIXME is this right?
			}
			return result;
		}

		Stitch CreateStitch (Stitch parent = null)
		{
			Stitch s = new Stitch (parent);
			Stitches.Add (s);
			return s;
		}

		void RemoveStitch (Stitch e)
		{
			if (WatchRefCounts ()) {
				Console.WriteLine ("Removing " + e.Name + " entirely.");
			}
			if (e.RefCount != 0) {
				RepointStitchToStitch (e, null);
				Console.WriteLine ("Deleting stitch with references, so first unpointing stitches from this stitch.");
				if (e.RefCount != 0) {
					throw new System.Exception ("Fixing ref-count on stitch removal failed.");
				}
			}
			e.Undivert ();
			for (int t = e.options.Count - 1; t >= 0; t--) {
				e.RemoveOption (e.options [t]);
			}
			RemovePageNumber(e, true);
			for (var t = 0; t < Stitches.Count; ++t) {
				if (Stitches [t] == e) {
					Stitches.RemoveAt (t);
					return;
				}
			}
		}

		void CreateOption (Stitch stitch)
		{
			var t = stitch.AddOption ();
			return t;
		}

		void RemoveOption (Stitch stitch, Option opt)
		{
			stitch.RemoveOption (opt);
		}

		void Purge ()
		{
			if (Stitches.Count == 0) {
				return;
			}
			var stitches = Stitches;
			List<Stitch> stitchesToRemove = new List<Stitch> ();
			for (var t = 0; t < stitches.Count; ++t) {
				var n = stitches[t];
				if (n.IsDead) {
					stitchesToRemove.Add (n);
				}
			}
			for (var t = 0; t < stitchesToRemove.Count; t++) {
				RemoveStitch (stitchesToRemove [t]);
			}
		}

		void RemovePageNumber (Stitch e, bool doIt)
		{
		}

		bool GetValueOfFlag (string flag, List<FlagValue> allFlags)
		{
			var n = GetIdxOfFlag (flag, allFlags);
			return n >= 0 ? allFlags[n].value : false;
		}

		bool ConvertStringToBooleanIfAppropriate (string s)
		{
		}

		public void AddFlagToIndex (string flag)
		{
			Console.WriteLine ("Adding flag string " + flag);
			var name = ExtractFlagNameFromExpression (flag);
			if (!flagIndex.Contains (name)) {
				flagIndex.Add (name);
			}
		}
	}
}
