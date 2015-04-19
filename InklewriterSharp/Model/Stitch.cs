using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Inklewriter
{
	[System.Serializable]
	public class Stitch
	{
		/// <summary>
		/// Short name, a unique identifier.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Body text.
		/// </summary>
		public string Text {
			get {
				return _text;
			}
			set {
				_text = value;
				if (string.IsNullOrEmpty (_text)) {
					WordCount = 0;
				} else {
					WordCount = Regex.Matches (_text, @"[\S]+").Count;
				}
			}
		}
		string _text;

		/// <summary>
		/// A URL for an image to display at the top of the stitch.
		/// </summary>
		public string Image { get; set; }

		public int PageNumber { get; set; }

		/// <summary>
		/// Section label.
		/// </summary>
		public string PageLabel {
			get {
				if (string.IsNullOrEmpty (_pageLabel)) {
					return "Section " + PageNumber;
				}
				return _pageLabel;
			}
			set {
				_pageLabel = value;
			}
		}
		string _pageLabel;

		/// <summary>
		/// Indicates that this stitch should connect to the 'divert' stitch
		/// without printing a paragraph break.
		/// </summary>
		public bool RunOn { get; set; }

		/// <summary>
		/// The next stitch to display. If runOn is false, the 'divert' stitch
		/// will be appended directly to this stitch without a paragraph break.
		/// </summary>
		public Stitch DivertStitch { get; set; }

		public List<Option> Options { get; set; }

		/// <summary>
		/// Markers that will be set before displaying this stitch.
		/// </summary>
		public List<string> Flags { get; set; }

		/// <summary>
		/// Display this stitch only if the specified markers have been set.
		/// </summary>
		public List<string> IfConditions { get; set; }

		/// <summary>
		/// Display this stitch only if the specified markers are not set.
		/// </summary>
		public List<string> NotIfConditions { get; set; }

		/// <summary>
		/// The number of stitches that link to this stitch.
		/// </summary>
		public int RefCount { get; set; }

		public Stitch ()
		{
			PageNumber = -1;
			Options = new List<Option> ();
			Flags = new List<string> ();
			Backlinks = new List<Stitch> ();
			IfConditions = new List<string> ();
			NotIfConditions = new List<string> ();
		}

		public Stitch (string text) : this()
		{
			Text = text;
		}

		public List<Stitch> Backlinks { get; set; }

		public bool IsDead (Story story = null)
		{
			return Text.Trim ().Length == 0
				&& Flags.Count == 0
				&& RefCount == 0
				&& this != story.InitialStitch;
		}

		#region Layout

		public int VerticalDistanceFromHeader {
			get {
				return VerticalDistanceFromPageNumberHeader;
			}
		}

		public int VerticalDistanceFromPageNumberHeader { get; set; }

		public int VerticalDistance { set; get; }

		#endregion


		#region Text

		public int WordCount { get; set; }

		public int SetPageNumberLabel (StoryModel model, int num)
		{
			if (num < this.PageNumber && model.MaxPage == PageNumber) {
				model.MaxPage--;
				PageNumber = num;
				if (num > model.MaxPage) {
					model.MaxPage = num;
				}
			}
			return PageNumber;
		}

		public string CreateShortName ()
		{
			if (string.IsNullOrEmpty (Text)) {
				return "blankStitch";
			}
			string shortName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase (Text.ToLower ());
			shortName = Regex.Replace(shortName, @"[^\w]", string.Empty);
			if (shortName.Length == 0) {
				return "punctuatedStitch";
			}
			shortName.Substring (0, System.Math.Min (16, Text.Length));
			var result = char.ToLowerInvariant (shortName [0]).ToString ();
			if (shortName.Length > 1) {
				result += shortName.Substring (1);
			}
			return result;
		}

		#endregion

		#region Diversion

		public void DivertTo (Stitch stitch)
		{
			if (stitch == this) {
				throw new System.Exception ("Diverted a stitch back to itself");
			}
			if (DivertStitch != null) {
				DivertStitch.RefCount--;
			}
			DivertStitch = stitch;
			DivertStitch.RefCount++;
		}

		public void Undivert ()
		{
			if (DivertStitch == null) {
				return;
			}
			DivertStitch.RefCount--;
			DivertStitch = null;
		}

		#endregion

		#region Options

		public Option AddOption ()
		{
			var newOption = new Option (this);
			Options.Add (newOption);
			return newOption;
		}

		public void RemoveOption (Option option)
		{
			option.Unlink ();
			Options.Remove (option);
		}

		#endregion

		#region Stats

		public class StitchStats
		{
			public bool deadEnd;
			public int numOptions;
			public List<Option> looseEnds = new List<Option> ();
			public int numLooseEnds;
			public int numLinked;
		}

		public StitchStats GetStats ()
		{
			var stats = new StitchStats ();
			stats.numOptions = Options.Count;
			if (Options.Count > 0) {
				stats.numLinked = 0;
				for (int i = 0; i < stats.numOptions; i++) {
					Option n = this.Options [i];
					if (n.LinkStitch != null) {
						stats.numLinked++;
					} else {
						stats.looseEnds.Add (n);
					}
				}
				stats.numLooseEnds = stats.numOptions - stats.numLinked;
			}
			return stats;
		}

		#endregion

		#region Flags

		bool FlagIsUsed (string expression)
		{
			string flag = StoryModel.ExtractFlagNameFromExpression (expression);
			for (int i = 0; i < Flags.Count; i++) {
				string n = StoryModel.ExtractFlagNameFromExpression (Flags[i]);
				if (n == flag) {
					return true;
				}
			}
			return false;
		}

		public string FlagByIndex (int index)
		{
			if (index < 0 || index >= Flags.Count) {
				return "";
			}
			return Flags [index];
		}

		public void EditFlag (string flag, StoryModel model)
		{
			int index = Flags.IndexOf (flag);
			if (index == -1) {
				Flags.Add (flag);
				model.AddFlagToIndex (flag);
			} else {
				Flags.RemoveAt (index);
				model.CollateFlags ();
			}
		}

		#endregion

//		public int NumberOfConditionals {
//			get {
//				
//			}
//		}
	}
}

