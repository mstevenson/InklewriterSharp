using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Inklewriter
{
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

		public StoryModel ()
		{
			FlagIndex = new List<string> ();
		}

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

		public void RebuildBacklinks ()
		{
			EndCount = 0;
			for (int e = 0; e < Story.Stitches.Count; e++) {
				Story.Stitches [e].Backlinks = new List<Stitch> ();
			}
			for (int e = 0; e < Story.Stitches.Count; e++) {
				if (Story.Stitches [e].Options.Count > 0) {
					for (var t = 0; t < Story.Stitches [e].Options.Count; t++) {
						if (Story.Stitches [e].Options [t].LinkStitch != null) {
							Story.Stitches [e].Options [t].LinkStitch.Backlinks.Add (Story.Stitches [e]);
						} else {
							LooseEndCount++;
						}
					}
				} else {
					if (Story.Stitches [e].DivertStitch != null) {
						Story.Stitches [e].DivertStitch.Backlinks.Add (Story.Stitches [e]);
					} else {
						EndCount++;
					}
				}
			}
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
		/// Adds a flag name to the story model's flag index.
		/// The flag index contains flag names, but no values.
		/// </summary>
		public void AddFlagToIndex (string flag)
		{
			var name = ExtractFlagNameFromExpression (flag);
			if (!FlagIndex.Contains (name)) {
				FlagIndex.Add (name);
			}
		}

		/// <summary>
		/// Returns the numerical index of the given flag from the given flags list.
		/// </summary>
		public static int GetIndexOfFlag (string flag, List<FlagValue> allFlags)
		{
			for (var i = 0; i < allFlags.Count; i++) {
				if (allFlags[i].flagName == flag.ToLower ()) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Strips operators and values from an expression and returns its flag name.
		/// </summary>
		/// <example>
		/// "cunning >= 5" returns "cunning"
		/// </example>
		public static string ExtractFlagNameFromExpression (string expression)
		{
			var regex = new Regex (@"^(.*?)\s*(\=|\+|\-|\>|\<|\!\=|$)");
			var match = regex.Match (expression);
			var name = match.Groups [1].Value.ToLower ();
			return name;
		}

		/// <summary>
		/// Finds the given flag in an array of flags and returns its value.
		/// </summary>
		public static int GetValueOfFlag (string flag, List<FlagValue> allFlags)
		{
			var i = GetIndexOfFlag (flag, allFlags);
			if (i >= 0) {
				return allFlags [i].value;
			}
			return 0;
		}

		/// <summary>
		/// Modify the values in the given flags array based on the flags contained in the given stitch.
		/// Each flag in the stitch must already exist in the flags array.
		/// </summary>
		public static void ProcessFlagSetting (Stitch stitch, List<FlagValue> allFlags)
		{
			for (int n = 0; n < stitch.Flags.Count; n++) {
				string flag = stitch.FlagByIndex (n);
				int newValue = 1; // true

				var match = Regex.Match (flag, @"^(.*?)\s*(\=|\+|\-)\s*(\b.*\b)\s*$");

				int flagIndex = -1;
				bool isBoolean = false;

				if (match.Success) {
					flag = match.Groups [1].Value;
					flagIndex = GetIndexOfFlag(flag, allFlags);
					var matchedOperator = match.Groups [2].Value;
					var matchedValue = match.Groups [3].Value;

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
						// Can't add or subtract a boolean, can only check equality
						isBoolean = true;
						if (matchedOperator == "=") {
							newValue = ConvertStringToBoolean (matchedValue) ? 1 : 0;
						}
					}
				} else {
					flagIndex = GetIndexOfFlag(flag, allFlags);
				}
					
				if (isBoolean) {
					allFlags.Add (new FlagValue (flag, newValue == 0 ? false : true));
				} else {
					allFlags.Add (new FlagValue (flag, newValue));
				}
			}
		}

		/// <summary>
		/// Tests a given flag expression against a given list of flags.
		/// Returns true on success.
		/// </summary>
		/// <example>>
		/// The expression "fruit > 1 && nuts == 7" will return true if
		/// the allFlags list contains FlagValue("fruit", 3) and FlagValue("nuts", 7).
		/// </example>
		public static bool Test (string expression, List<FlagValue> allFlags)
		{
			bool result = false;
			string pattern = @"^(.*?)\s*(\<|\>|\<\=|\>\=|\=|\!\=|\=\=)\s*(\b.*\b)\s*$";
			var match = Regex.Match (expression, pattern);
			if (match.Success) {
				string flag = match.Groups [1].Value;
				string op = match.Groups [2].Value;
				string value = match.Groups [3].Value;

				int numberValue = -1;
				bool isBool = !int.TryParse (value, out numberValue);

				if (isBool) {
					numberValue = ConvertStringToBoolean (value) ? 1 : 0;
				}
				
				int flagValue = GetValueOfFlag (flag, allFlags);
				if (op == "==" || op == "=") {
					result = flagValue == numberValue;
				} else if (op == "!=" || op == "<>") {
					result = flagValue != numberValue;
				} else {
					if (isBool) {
						throw new System.Exception ("Error - Can't perform an order-test on a boolean.");
					}
					switch (op) {
					case "<":
						result = flagValue < numberValue;
						break;
					case "<=":
						result = flagValue <= numberValue;
						break;
					case ">":
						result = flagValue > numberValue;
						break;
					case ">=":
						result = flagValue >= numberValue;
						break;
					}
				}
			} else {
				result = GetValueOfFlag (expression, allFlags) == 1;
			}
			return result;
		}

		#endregion

		#region Stitches

		/// <summary>
		/// Creates a new Stitch with the given body text and adds it
		/// to the current Story.
		/// </summary>
		public Stitch CreateStitch (string text)
		{
			Stitch s = new Stitch (text);
			Story.Stitches.Add (s);
			return s;
		}

		/// <summary>
		/// Deletes the given stitch and cleans up stitch references.
		/// </summary>
		public void RemoveStitch (Stitch stitch)
		{
			// Stitch has references, so unpoint all stitches from this stitch
			if (stitch.RefCount != 0) {
				RepointStitchToStitch (stitch, null);
				if (stitch.RefCount != 0) {
					throw new System.Exception ("Fixing ref-count on stitch removal failed.");
				}
			}
			stitch.Undivert ();
			// Detach options
			for (int t = stitch.Options.Count - 1; t >= 0; t--) {
				stitch.RemoveOption (stitch.Options [t]);
			}
			RemovePageNumber (stitch);
			// Remove the stitch from its Story
			for (var t = 0; t < Story.Stitches.Count; ++t) {
				if (Story.Stitches [t] == stitch) {
					Story.Stitches.RemoveAt (t);
					return;
				}
			}
		}

		/// <summary>
		/// Replaces each reference to the source stitch with a reference
		/// to the target stitch. All stitches and options that link to
		/// the source stitch will be modified.
		/// </summary>
		public void RepointStitchToStitch (Stitch source, Stitch target)
		{
			foreach (var stitch in Story.Stitches) {
				// Find all stitches that link to the source stitch
				// and swap it with the target stitch
				if (stitch.DivertStitch == source) {
					stitch.Undivert ();
					if (target != null) {
						stitch.DivertTo (target);
					}
				}
				foreach (var option in stitch.Options) {
					// Find all options that link to the source stitch
					// and relink them to the target stitch
					if (option.LinkStitch == source) {
						option.Unlink ();
						if (target != null) {
							option.CreateLinkStitch (target);
						}
					}
				}
			}
		}

		/// <summary>
		/// Creates globally unique short names for all stitches
		/// in the current Story.
		/// Stitches are not guaranteed to retain the same
		/// short name throughout their lifetime.
		/// </summary>
		public void NameStitches ()
		{
			HashSet<string> usedShortNames = new HashSet<string> ();
			foreach (var stitch in Story.Stitches) {
				string shortName = stitch.CreateShortName ();
				// Enforce globally unique names by appending a number
				// to stitches that return an existing short name.
				string incrementedShortName = shortName;
				for (int num = 1; usedShortNames.Contains (shortName); num++) {
					incrementedShortName = shortName + num;
				}
				shortName = incrementedShortName;
				usedShortNames.Add (shortName);
				stitch.Name = shortName;
			}
		}

		#endregion

		#region Options

		/// <summary>
		/// Appends a new and empty Option to the given Stitch.
		/// </summary>
		public Option CreateOption (Stitch stitch)
		{
			var t = stitch.AddOption ();
			return t;
		}

		/// <summary>
		/// Removes an option from the given stitch.
		/// </summary>
		public void RemoveOption (Stitch stitch, Option option)
		{
			stitch.RemoveOption (option);
		}

		#endregion

		#region Page Numbers

		public void InsertPageNumber (Stitch stitch)
		{
			if (stitch.VerticalDistanceFromPageNumberHeader < 2
				|| PageSize (stitch.PageNumber) < StoryModel.maxPreferredPageLength / 2
				|| HeaderWithinDistanceOfStitch (3, stitch))
			{
				return;
			}
			if (stitch.PageNumber != 0) {
				return;
			}
			var number = stitch.PageNumber + 1;
			var stitches = Story.Stitches;
			for (var r = 0; r < stitches.Count; r++) {
				var i = stitches[r].PageNumber;
				if (i >= number) {
					stitches [r].SetPageNumberLabel (this, i + 1);
				}
			}
			stitch.SetPageNumberLabel (this, number);
			ComputePageNumbers ();
		}

		public void RemovePageNumber (Stitch stitch)
		{
			var currentNumber = stitch.PageNumber;
			if (currentNumber <= 0) {
				return;
			}
			stitch.SetPageNumberLabel (this, -1);
			foreach (var s in Story.Stitches) {
				if (s.PageNumber > currentNumber) {
					s.SetPageNumberLabel (this, s.PageNumber - 1);
				}
			}
			ComputePageNumbers ();
		}

		public void ComputePageNumbers ()
		{
//			List<Stitch> e = new List<Stitch> ();
//			Stitch t = null;
////			var n = new List<Stitch> ();
//			var r = new List<Stitch> ();
//			var stitches = Story.Stitches;
//			for (var i = 0; i < stitches.Count; i++) {
//				var s = stitches[i].PageNumber;
//				if (s > 0) {
//					e.Add (stitches [i]);
//					if (s > t) {
//						t = s;
//					}
//					stitches [i].SetPageNumberLabel (this, s);
//					//					n[s] = [];
//					//					r[s] = !0;
//				} else {
//					stitches [i].SetPageNumberLabel (this, 0);
//				}
//			}
//			//			e.sort(function(e, t) {
//			//				return e.pageNumberLabel() - t.pageNumberLabel()
//			//				});
//			for (var i = e.Count - 1; i >= 0; i--) {
//				ComputePageNumbersSubProcess (e[i], true, 0);
//			}
//			List<int> u = new List<int> ();
////			u.Add (initialStitch.pageNumber());
//			while (u.Count > 0) {
////				var a = [];
////				for (var i = 0; i < u.length; i++) {
////					if (r[u[i]]) {
////						r[u[i]] = !1;
////						for (var f = 0; f < n[u[i]].length; f++) {
////							a.Add (n [u [i]] [f]);
////						}
////					}
////				}
////				u = a;
//			}
////			for (var i = 0; i < StoryModel.stitches.length; i++) {
////				var l = StoryModel.stitches[i].pageNumber();
////				l && r[l] && (StoryModel.stitches[i].pageNumber(0), StoryModel.stitches[i].sectionStitches = [])
////			}
		}

//		void ComputePageNumbersSubProcess (Stitch t2, bool r2, int s) {
//			if (!t2) {
//				return;
//			}
//
//			if (!r2 && t2.PageNumber > 0) {
//				if (t2.VerticalDistanceFromPageNumberHeader > s && t2.PageNumber == e[i].PageNumber && t2.verticalDistanceFromHeader(s)) {
//					n[e[i].pageNumber()].push(t.pageNumber());
//				}
//				return;
//			}
//			//					t.pageNumber(e[i].PageNumber);
//			//					t.headerStitch (e[i]);
//			//					e[i].sectionStitches.push(t);
//			//					t.verticalDistanceFromHeader(s);
//			o(t.DivertStitch, false, s + .01);
//			for (var u = 0; u < t.Options.Count; u++) {
//				o (t.Options[u].LinkStitch, false, s + 1 + .1 * u);
//			}
//		}

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

		public static bool ConvertStringToBoolean (string s)
		{
			if (s.ToLower () == "true") {
				return true;
			}
			return false;
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

		/// <summary>
		/// Returns true if the 'if' and 'not if' conditions are satisfied by the given list of flags.
		/// </summary>
		public static bool DoesArrayMeetConditions (List<string> ifConditions, List<string> notIfConditions, List<FlagValue> flags) // n type is unknown
		{
			var success = false;
			for (var i = 0; i < ifConditions.Count && !success; i++) {
				success = !StoryModel.Test (ifConditions [i], flags);
			}
			for (var i = 0; i < notIfConditions.Count && !success; i++) {
				success = StoryModel.Test (notIfConditions [i], flags);
			}
			return !success;
		}
	}
}
