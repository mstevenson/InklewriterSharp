using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Inklewriter
{
	public class FlagValue
	{
		public string flagName;
		public int value;
		public bool isBoolean;

		public FlagValue ()
		{
		}

		public FlagValue (string name, bool isTrue)
		{
			flagName = name;
			value = isTrue ? 1 : 0;
			isBoolean = true;
		}

		public FlagValue (string name, int number)
		{
			flagName = name;
			value = number;
		}
	}

	public class StoryModel
	{
		public const string defaultStoryName = "Untitled Story";
		public const string defaultAuthorName = "Anonymous";
		public const int maxPreferredPageLength = 8;

		public int MaxPage { get; set; }

		public Story Story { get; private set; }

		public List<string> FlagIndex { get; set; }

		public int EndCount { get; set; }

		public int LooseEndCount { get; set; }

		public bool Loading { get; set; }

		public StoryModel ()
		{
			FlagIndex = new List<string> ();
		}

		#region IO

		public void ImportStory (string data)
		{
			Story = StoryReader.Read (data);
		}

		public string ExportStory ()
		{
			if (Story != null) {
				NameStitches ();
				var data = StoryWriter.Write (Story);
				return data;
			}
			return null;
		}

		#endregion

//		public void RebuildBacklinks ()
//		{
//			EndCount = 0;
//			var stitches = Stitches;
//			for (int e = 0; e < stitches.Count; e++) {
//				stitches [e].Backlinks = new List<Stitch> ();
//			}
//			for (int e = 0; e < stitches.Count; e++)
//				if (stitches [e].Options.Count > 0) {
//					for (var t = 0; t < stitches [e].Options.Count; t++) {
//						if (stitches [e].Options [t].LinkStitch != null) {
//							stitches [e].Options [t].LinkStitch.Backlinks.Add (stitches [e]);
//						} else {
//							LooseEndCount++;
//						}
//					}
//				} else {
//					if (stitches [e].DivertStitch != null) {
//						stitches [e].DivertStitch.Backlinks.Add (stitches [e]);
//					} else {
//						EndCount++;
//					}
//				}
//			if (WatchRefCounts ()) {
//				for (int e = 0; e < stitches.Count; e++) {
//					if (stitches[e].Backlinks.Count != stitches[e].RefCount) {
//						throw new System.Exception ("Stitch with text '" + stitches[e].Text + "' has invalid ref-count!");
//					}
//				}
//			}
//		}

		public static bool WatchRefCounts ()
		{
			// FIXME
			return false;
		}

		#region Flags

		/// <summary>
		/// Preprocesses and stores all flags set in all stitches.
		/// </summary>
		public void CollateFlags ()
		{
			FlagIndex = new List<string> ();
			foreach (var stitch in Story.Stitches) {
				foreach (var flag in stitch.Flags) {
					AddFlagToIndex (flag);
				}
				foreach (var option in stitch.Options) {
					foreach (var ifCondition in option.IfConditions) {
						AddFlagToIndex (ifCondition);
					}
					foreach (var notIfCondition in option.NotIfConditions) {
						AddFlagToIndex (notIfCondition);
					}
				}
				foreach (var ifCondition in stitch.IfConditions) {
					AddFlagToIndex (ifCondition);
				}
				foreach (var notIfCondition in stitch.NotIfConditions) {
					AddFlagToIndex (notIfCondition);
				}
			}
		}

		/// <summary>
		/// Adds a flag name to the index, but strips off the value in a flag expression.
		/// </summary>
		public void AddFlagToIndex (string flag)
		{
			Console.WriteLine ("Adding flag string " + flag);
			var name = ExtractFlagNameFromExpression (flag);
			if (!FlagIndex.Contains (name)) {
				FlagIndex.Add (name);
			}
		}

		public int GetIndexOfFlag (string flag, List<FlagValue> allFlags)
		{
			for (var i = 0; i < allFlags.Count; i++) {
				if (allFlags[i].flagName == flag.ToLower ()) {
					return i;
				}
			}
			return -1;
		}

		public static string ExtractFlagNameFromExpression (string expression)
		{
			var regex = new Regex (@"^(.*?)\s*(\=|\+|\-|\>|\<|\!\=|$)");
			var match = regex.Match (expression);
			var name = match.Groups [1].Value.ToLower ();
			return name;
		}

		public int GetValueOfFlag (string flag, List<FlagValue> allFlags)
		{
			var n = GetIndexOfFlag (flag, allFlags);
			return n >= 0 ? allFlags[n].value : 0;
		}

		public void ProcessFlagSetting (Stitch stitch, List<FlagValue> allFlags)
		{
			for (int n = 0; n < stitch.Flags.Count; n++) {
				string flag = stitch.FlagByIndex (n);
				int newValue = 1; // true

				Console.WriteLine ("Flag directive: " + flag);
				var match = Regex.Match (flag, @"^(.*?)\s*(\=|\+|\-)\s*(\b.*\b)\s*$");

				int flagIndex = -1;
				bool isBoolean = false;

				if (match.Success) {
					flag = match.Groups [1].ToString ();
					flagIndex = GetIndexOfFlag(flag, allFlags);
					var matchedOperator = match.Groups [2].ToString ();
					var matchedValue = match.Groups [3].ToString ();

					bool isValueNumerical = Regex.IsMatch (matchedValue, @"\d+");
					if (isValueNumerical) {
						// Handle numerical value
						isBoolean = false;
						if (matchedOperator == "=") {
							newValue = int.Parse (matchedValue);
						} else {
							newValue = (flagIndex < 0) ? 0 : allFlags [flagIndex].value;
							if (matchedOperator == "+") {
								newValue += int.Parse (matchedValue);
							} else if (matchedOperator == "-") {
								newValue -= int.Parse (matchedValue);
							}
						}
					} else {
						// Handle boolean value
						isBoolean = true;
						if (matchedOperator == "=") {
							newValue = ConvertStringToBooleanIfAppropriate (matchedValue);
						} else {
							Console.WriteLine ("Can't add/subtract a boolean.");
						}
					}
				} else {
					flagIndex = GetIndexOfFlag(flag, allFlags);
				}

				Console.WriteLine ("Assigning value: " + newValue);
				allFlags [flagIndex].isBoolean = isBoolean;
				allFlags [flagIndex].value = newValue;
			}
		}

		public bool Test (string expression, List<FlagValue> allFlags)
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

		#endregion

		#region Stitches

		public Stitch CreateStitch (string text)
		{
			Stitch s = new Stitch (text);
			Story.Stitches.Add (s);
			return s;
		}

		public void RemoveStitch (Stitch e)
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
			for (int t = e.Options.Count - 1; t >= 0; t--) {
				e.RemoveOption (e.Options [t]);
			}
			RemovePageNumber(e, true);
			for (var t = 0; t < Story.Stitches.Count; ++t) {
				if (Story.Stitches [t] == e) {
					Story.Stitches.RemoveAt (t);
					return;
				}
			}
		}

		public void RepointStitchToStitch (Stitch source, Stitch target)
		{
			if (WatchRefCounts ()) {
				Console.WriteLine ("Repointing stitch links from " + source.Name + " to " + (target != null ? target.Name : "to null."));
			}
			var stitches = Story.Stitches;
			for (int n = 0; n < stitches.Count; n++) {
				var r = stitches[n];
				if (r.DivertStitch == source) {
					r.Undivert ();
					if (target != null) {
						r.DivertTo (target);
					}
				}
				for (var i = 0; i < r.Options.Count; i++) {
					if (r.Options[i].LinkStitch == source) {
						r.Options [i].Unlink ();
						if (target != null) {
							r.Options [i].CreateLinkStitch (target);
						}
					}
				}
			}
		}

		public void NameStitches ()
		{
			HashSet<string> usedShortNames = new HashSet<string> ();
			foreach (var currentStitch in Story.Stitches) {
				string shortName = currentStitch.CreateShortName ();
				string incrementedShortName = shortName;
				for (int num = 1; usedShortNames.Contains (shortName); num++) {
					incrementedShortName = shortName + num;
				}
				shortName = incrementedShortName;
				usedShortNames.Add (shortName);
				currentStitch.Name = shortName;
			}
		}

		#endregion

		#region Options

		public Option CreateOption (Stitch stitch)
		{
			var t = stitch.AddOption ();
			return t;
		}

		public void RemoveOption (Stitch stitch, Option opt)
		{
			stitch.RemoveOption (opt);
		}

		#endregion

		#region Page Numbers

		public void InsertPageNumber (Stitch e)
		{
			if (Loading || e.VerticalDistanceFromPageNumberHeader < 2
				|| PageSize (e.PageNumber) < StoryModel.maxPreferredPageLength / 2
				|| HeaderWithinDistanceOfStitch (3, e))
			{
				return;
			}
			if (e.PageNumber != 0) {
				return;
			}
			var n = e.PageNumber + 1;
			var stitches = Story.Stitches;
			for (var r = 0; r < stitches.Count; r++) {
				var i = stitches[r].PageNumber;
				if (i >= n) {
					stitches [r].SetPageNumberLabel (this, i + 1);
				}
			}
			e.SetPageNumberLabel (this, n);
			ComputePageNumbers ();
		}

		public void RemovePageNumber (Stitch e, bool doIt)
		{
//			var n = e.pageNumberLabel();
//			if (n <= 0) return;
//			e.pageNumberLabel(-1);
//			for (var r = 0; r < StoryModel.stitches.length; r++) {
//				var i = StoryModel.stitches[r].pageNumberLabel();
//				i > n && StoryModel.stitches[r].pageNumberLabel(i - 1)
//			}
//			t || StoryModel.computePageNumbers()
		}

		public void ComputePageNumbers ()
		{
			var e = new List<Stitch> ();
			var t = 0;
			//			var n = {}		;
			//			var r = {};
			var stitches = Story.Stitches;
			for (var i = 0; i < stitches.Count; i++) {
				var s = stitches[i].PageNumber;
				if (s > 0) {
					e.Add (stitches [i]);
					if (s > t) {
						t = s;
					}
					stitches [i].SetPageNumberLabel (this, s);
					//					n[s] = [];
					//					r[s] = !0;
				} else {
					stitches [i].SetPageNumberLabel (this, 0);
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

		#endregion

		public void Purge ()
		{
			if (Story.Stitches.Count == 0) {
				return;
			}
			var stitches = Story.Stitches;
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

		public int ConvertStringToBooleanIfAppropriate (string s)
		{
			if (s.ToLower () == "true") {
				return 1;
			}
			return 0;
		}

		public bool HeaderWithinDistanceOfStitch (int distance, Stitch stitch)
		{
//			var n = [],
//			r = [];
//			n.push(t);
//			for (var i = 0; i <= e; i++) {
//				for (var s = 0; s < n.length; s++) {
//					var o = n[s];
//					if (o) {
//						if (o.pageNumberLabel() > 0) return !0;
//						r.push(o.divertStitch);
//						for (var u = 0; u < o.options.length; u++) r.push(o.options[u]._linkStitch)
//						}
//				}
//				n = r, r = []
//			}
//			return !1

			// FIXME
			return false;
		}

		public int PageSize (int pageNumber)
		{
			var t = 0;
			var stitches = Story.Stitches;
			for (var n = 0; n < stitches.Count; n++) {
				if (stitches [n].PageNumber == pageNumber) {
					t++;
				}
			}
			return t;
		}

		public void ComputeVerticalHeuristic ()
		{
			if (Story.InitialStitch == null) {
				return;
			}
//			var e = [];
//			t = [];
			var stitches = Story.Stitches;
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
	}
}
