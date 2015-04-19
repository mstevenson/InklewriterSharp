using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Inklewriter
{

	public class FlagValue {
		public string flagName;
		public int value;
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
				return null;
//				return Story.data.stitches;
			}
		}

		public void CollateFlags ()
		{
//			Story story = CurrentStory
			// FIXME
			Story story = null;


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
				int i = 0;
				Console.WriteLine ("Flag directive: " + r);
				var o = new Regex (@"^(.*?)\s*(\=|\+|\-)\s*(\b.*\b)\s*$");
				var s = o.Matches (r);
				int u = -1;
				if (s.Count > 0) {
					r = s[1].ToString ();
					u = GetIdxOfFlag(r, allFlags);
					var m = new Regex (@"\d+");
					if (m.IsMatch (s [3].ToString ())) {
						if (s [2].ToString () == "=") {
							i = ParseInt(s[3].ToString ());
						}
					} else {
						if (u < 0) {
							i = 0;
						} else {
							i = allFlags[u].value;
						}
						if (s[2].ToString () == "+") {
							i += ParseInt(s[3].ToString ());
						} else {
							i -= ParseInt(s[3].ToString ());
						}
					}
					if (s[2].ToString () == "=") {
						i = ConvertStringToBooleanIfAppropriate (s[3].ToString ());
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
			// FIXME
			return 0;
		}

		// FIXME should return a bool, but bools and ints are used interchangebly
		public bool Test (string expression, List<FlagValue> allFlags) // e == flag
		{
			Regex regex = new Regex (@"^(.*?)\s*(\<|\>|\<\=|\>\=|\=|\!\=|\=\=)\s*(\b.*\b)\s*$");
			bool result = false;
			MatchCollection matches = regex.Matches (expression);
			if (regex.IsMatch (expression)) {
				string flag = matches [1].ToString ();
				string op = matches [2].ToString ();
				string valueString = matches [3].ToString ();
				int value = int.Parse (valueString);

				int flagValue = GetValueOfFlag (flag, allFlags);
				Console.WriteLine ("Testing " + flagValue + " " + op + " " + value);
				if (op == "==" || op == "=") {
					result = flagValue == value;
				} else {
					if (op == "!=" || op == "<>") {
						result = flagValue != value;
					} else {
						if (Regex.IsMatch (valueString, @"\d+")) {
							throw new System.Exception ("Error - Can't perform an order-test on a boolean.");
						}
						if (op == "<") {
							result = flagValue < value;
						} else if (op == "<=") {
							result = flagValue <= value;
						} else if (op == ">") {
							result = flagValue > value;
						} else if (op == ">=") {
							result = flagValue >= value;
						}
					}
				}
			} else {
//				result = GetValueOfFlag (expression, allFlags) == 1;
//				result = ConvertStringToBooleanIfAppropriate(result) == 1;
//				if (result == false || result == -1) {
//					result = false;
//				}
//				result = true; // FIXME is this right?
			}
			return result;
		}

		Stitch CreateStitch (string text)
		{
			Stitch s = new Stitch (text);
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

		Option CreateOption (Stitch stitch)
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
				if (n.IsDead ()) {
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

		int GetValueOfFlag (string flag, List<FlagValue> allFlags)
		{
			var n = GetIdxOfFlag (flag, allFlags);
			return n >= 0 ? allFlags[n].value : 0;
		}

		int ConvertStringToBooleanIfAppropriate (string s)
		{
			// FIXME bools represented as 0 and 1
			return 0;
		}

		public void AddFlagToIndex (string flag)
		{
			Console.WriteLine ("Adding flag string " + flag);
			var name = ExtractFlagNameFromExpression (flag);
			if (!flagIndex.Contains (name)) {
				flagIndex.Add (name);
			}
		}

		public void InsertPageNumber (Stitch e)
		{
			if (Loading || e.VerticalDistanceFromHeader < 2
			    || PageSize (e.PageNumber) < StoryModel.maxPreferredPageLength / 2
			    || HeaderWithinDistanceOfStitch (3, e))
			{
				return;
			}
			if (e.PageNumber != 0) {
				return;
			}
			var n = e.PageNumber + 1;
			var stitches = Stitches;
			for (var r = 0; r < stitches.Count; r++) {
				var i = stitches[r].PageNumber;
				if (i >= n) {
					stitches [r].SetPageNumberLabel (i + 1);
				}
			}
			e.SetPageNumberLabel (n);
			ComputePageNumbers ();
		}

		void ComputePageNumbers ()
		{
			var e = new List<Stitch> ();
			var t = 0;
//			var n = {}		;
//			var r = {};
			var stitches = Stitches;
			for (var i = 0; i < stitches.Count; i++) {
				var s = stitches[i].PageNumber;
				if (s > 0) {
					e.Add (stitches [i]);
					if (s > t) {
						t = s;
					}
					stitches [i].SetPageNumberLabel (s);
//					n[s] = [];
//					r[s] = !0;
				} else {
					stitches [i].SetPageNumberLabel (0);
					stitches[i].SectionStitches = new List<Stitch> ();
				}
			}
//			e.sort(function(e, t) {
//				return e.pageNumberLabel() - t.pageNumberLabel()
//				});
			for (var i = e.Count - 1; i >= 0; i--) {
//				var o = function(t, r, s) {
//					if (!t) return;
//					if (!r && t.pageNumber() > 0) {
//						t.verticalDistanceFromHeader() > s && t.pageNumber() == e[i].pageNumber() && t.verticalDistanceFromHeader(s), n[e[i].pageNumber()].push(t.pageNumber());
//						return
//						}
//					t.pageNumber(e[i].pageNumber()), t.headerStitch(e[i]), e[i].sectionStitches.push(t), t.verticalDistanceFromHeader(s), o(t.divertStitch, !1, s + .01);
//					for (var u = 0; u < t.options.length; u++) o(t.options[u].linkStitch(), !1, s + 1 + .1 * u)
//					};
//				o(e[i], !0, 0)
			}
//			var u = [];
//			u.push(initialStitch.pageNumber());
//			while (u.length > 0) {
//				var a = [];
//				for (var i = 0; i < u.length; i++)
//					if (r[u[i]]) {
//						r[u[i]] = !1;
//						for (var f = 0; f < n[u[i]].length; f++) a.push(n[u[i]][f])
//						}
//				u = a
//			}
//			for (var i = 0; i < StoryModel.stitches.length; i++) {
//				var l = StoryModel.stitches[i].pageNumber();
//				l && r[l] && (StoryModel.stitches[i].pageNumber(0), StoryModel.stitches[i].sectionStitches = [])
//			}
		}

		bool HeaderWithinDistanceOfStitch (int distance, Stitch stitch)
		{
			// FIXME
			return false;
		}

		int PageSize (int pageNumber)
		{
			var t = 0;
			var stitches = Stitches;
			for (var n = 0; n < stitches.Count; n++) {
				if (stitches [n].PageNumber == pageNumber) {
					t++;
				}
			}
			return t;
		}


		Stitch InitialStitch {
			get;
			set;
		}

		void ComputeVerticalHeuristic ()
		{
			if (InitialStitch == null) {
				return;
			}
//			var e = [];
//			t = [];
			var stitches = Stitches;
//			for (var n = 0; n < stitches.Count; n++) {
//				var r = stitches[n];
//				r.VerticalDistance (-1);
//			}
//			e.push(StoryModel.initialStitch), StoryModel.initialStitch.verticalDistance(1);
//			while (e.length > 0) {
//				for (var n = 0; n < e.length; n++) {
//					var r = e[n];
//					if (r.divertStitch) {
//						var i = r.divertStitch;
//						i.verticalDistance() == -1 && (i.verticalDistance(r.verticalDistance() + .01), t.push(i))
//					} else
//						for (var s = 0; s < r.options.length; s++)
//							if (r.options[s].linkStitch()) {
//								var i = r.options[s].linkStitch();
//								i.verticalDistance() == -1 && (i.verticalDistance(r.verticalDistance() + 1 + .1 * s), t.push(i))
//							}
//				}
//				e = t, t = []
//			}
//			for (var n = 0; n < StoryModel.stitches.length; n++) {
//				var r = StoryModel.stitches[n];
//				r.verticalDistance() == -1 && r.verticalDistance(StoryModel.stitches.length + 1)
//			}
		}

		bool Loading {
			get;
			set;
		}
	}
}
