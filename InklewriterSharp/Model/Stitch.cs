using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Inklewriter
{
	[System.Serializable]
	public class Stitch
	{
		public string Text { get; set; }

		/// <summary>
		/// A URL for an image to display at the top of the stitch.
		/// </summary>
		public string Image { get; set; }

		public int PageNum { get; set; }

		/// <summary>
		/// Section label.
		/// </summary>
		public string PageLabel {
			get {
				if (string.IsNullOrEmpty (_pageLabel)) {
					return "Section " + PageNumberLabel;
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
		public Stitch Divert { get; set; }

		public List<Option> Options { get; set; }

		/// <summary>
		/// Markers that will be set before displaying this stitch.
		/// </summary>
		public List<string> FlagNames { get; set; }

		/// <summary>
		/// Display this stitch only if the specified markers have been set.
		/// </summary>
		public List<string> IfConditions { get; set; }

		/// <summary>
		/// Display this stitch only if the specified markers are not set.
		/// </summary>
		public List<string> NotIfConditions { get; set; }

		public List<Stitch> SectionStitches { get; set; }


		public Stitch ()
		{
			PageNum = -1;
			Options = new List<Option> ();
			FlagNames = new List<string> ();
			Backlinks = new List<Stitch> ();
			IfConditions = new List<string> ();
			NotIfConditions = new List<string> ();
		}

		public Stitch (string text) : this()
		{
			RefCount = 0;
			Text = text;
		}

		public int WordCountOf (string e) {
			if (!string.IsNullOrEmpty (e)) {
				var t = Regex.Matches (e, @"[\S]+");
				return t.Count;
			}
			return 0;
		}

		public int WordCount {
			get {
				return WordCountOf (Text);
			}
		}

		public int ComputedPageNumber {
			get {
				return 0;
			}
		}

		public int VerticalDistanceFromHeader {
			get {
				return VerticalDistanceFromPageNumberHeader;
			}
		}

		public int VerticalDistanceFromPageNumberHeader { get; set; }

		public int VerticalDistance { set; get; }

//		public string PageLabelText {
//			get {
//				if (string.IsNullOrEmpty (PageLabel)) {
//					return "Section " + PageNumberLabel;
//				}
//				return PageLabel;
//			}
//		}

		public int SetPageNumberLabel (StoryModel model, int num)
		{
			if (num < this.PageNumberLabel && model.MaxPage == PageNumberLabel) {
				model.MaxPage--;
				PageNumberLabel = num;
				if (num > model.MaxPage) {
					model.MaxPage = num;
				}
			}
			return PageNumberLabel;
		}

		public int PageNumberLabel { get; set; }

		public int PageNumber {
			get {
				return ComputedPageNumber;
			}
		}

		public int RefCount { get; set; }

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

		public void DivertTo (Stitch stitch)
		{
			if (stitch == this) {
				throw new System.Exception ("Diverted a stitch back to itself");
			}
			if (DivertedStitch != null) {
				DivertedStitch.RefCount--;
			}
			DivertedStitch = stitch;
			DivertedStitch.RefCount++;
		}

		public void Undivert ()
		{
			if (DivertedStitch == null) {
				return;
			}
			DivertedStitch.RefCount--;
			DivertedStitch = null;
		}

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

		public bool IsDead (Story story = null)
		{
			return Text.Trim ().Length == 0
				&& NumberOfFlags == 0
				&& RefCount == 0
				&& this != story.InitialStitch;
		}

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

		public int NumberOfFlags {
			get {
				return FlagNames.Count;
			}
		}

		bool FlagIsUsed (string expression)
		{
			string flag = StoryModel.ExtractFlagNameFromExpression (expression);
			for (int i = 0; i < FlagNames.Count; i++) {
				string n = StoryModel.ExtractFlagNameFromExpression (FlagNames[i]);
				if (n == flag) {
					return true;
				}
			}
			return false;
		}

		public void SetName (string shortName)
		{
			// TODO
		}

		public void DivertStitch ()
		{
		}

		public string Name {
			get;
			set;
		}

		public string FlagByIndex (int index)
		{
			if (index < 0 || index >= FlagNames.Count) {
				return "";
			}
			return FlagNames [index];
		}

		public void EditFlag (string flag, StoryModel model)
		{
			int index = FlagNames.IndexOf (flag);
			if (index == -1) {
				FlagNames.Add (flag);
				model.AddFlagToIndex (flag);
			} else {
				FlagNames.RemoveAt (index);
				model.CollateFlags ();
			}
		}

//		public int NumberOfConditionals {
//			get {
//				
//			}
//		}

		public Stitch DivertedStitch { get; private set; }

		public List<Stitch> Backlinks { get; set; }
	}
}

