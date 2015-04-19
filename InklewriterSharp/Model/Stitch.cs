using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Inklewriter
{
	[System.Serializable]
	public class Stitch
	{
		public string text;

		/// <summary>
		/// A URL for an image to display at the top of the stitch.
		/// </summary>
		public string image;

		public int pageNum = -1;

		/// <summary>
		/// Section label.
		/// </summary>
		public string pageLabel;

		/// <summary>
		/// Indicates that this stitch should connect to the 'divert' stitch
		/// without printing a paragraph break.
		/// </summary>
		public bool runOn;

		/// <summary>
		/// The next stitch to display. If runOn is false, the 'divert' stitch
		/// will be appended directly to this stitch without a paragraph break.
		/// </summary>
		public string divert;

		public List<Option> options;

		/// <summary>
		/// Markers that will be set before displaying this stitch.
		/// </summary>
		public List<string> flagNames;

		/// <summary>
		/// Display this stitch only if the specified markers have been set.
		/// </summary>
		public List<string> ifConditions;

		/// <summary>
		/// Display this stitch only if the specified markers are not set.
		/// </summary>
		public List<string> notIfConditions;


		public Stitch ()
		{
			options = new List<Option> ();
			Backlinks = new List<Stitch> ();
		}

		public Stitch (string text)
		{
			RefCount = 0;
			this.text = text;
			WordCount = WordCountOf (text);
		}

		int WordCountOf (string e) {
			if (!string.IsNullOrEmpty (e)) {
				var t = Regex.Matches (e, @"[\S]+");
				return t.Count;
			}
			return 0;
		}

		public int WordCount {
			get; set;
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

		public int VerticalDistanceFromPageNumberHeader {
			get;
			set;
		}

		public int VerticalDistance {
			set;
			get;
		}


		public string PageLabelText {
			get {
				if (string.IsNullOrEmpty (pageLabel)) {
					return "Section " + PageNumberLabel;
				}
				return pageLabel;
			}
		}

		public string PageNumberLabel {
			get {
				// FIXME implement
				// return e && (e < this._pageNumberHeader && StoryModel.maxPage == this._pageNumberHeader && StoryModel.maxPage--, this._pageNumberHeader = e, e > StoryModel.maxPage && (StoryModel.maxPage = e)), this._pageNumberHeader
				return "";
			}
		}

		public List<Stitch> SectionStitches {
			get;
			set;
		}

		public void SetPageNumberLabel (int number)
		{
			// TODO
		}

		public int PageNumber {
			get {
				return ComputedPageNumber;
			}
		}

		public int RefCount { get; set; }

		public string CreateShortName ()
		{
			if (string.IsNullOrEmpty (text)) {
				return "blankStitch";
			}
			string shortName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase (text.ToLower ());
			shortName = Regex.Replace(shortName, @"[^\w]", string.Empty);
			if (shortName.Length == 0) {
				return "punctuatedStitch";
			}
			shortName.Substring (0, System.Math.Min (16, text.Length));
			var result = char.ToLowerInvariant (shortName [0]).ToString ();
			if (shortName.Length > 1) {
				result += shortName.Substring (1);
			}
			return result;
		}

		public void Divert (Stitch stitch, bool doDivert)
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
			options.Add (newOption);
			return newOption;
		}

		public void RemoveOption (Option option)
		{
			option.Unlink ();
			options.Remove (option);
		}

		public bool IsDead (Story story = null)
		{
			return false;

			//			return text.Trim ().Length == 0
			//				&& NumberOfFlags == 0
			//				&& RefCount == 0
			//				&& this != story.InitialStitch;
		}

		public bool RunOn ()
		{
			// TODO not sure what argument this requires
			return false;
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
			stats.numOptions = options.Count;
			if (options.Count > 0) {
				stats.numLinked = 0;
				for (int i = 0; i < stats.numOptions; i++) {
					Option n = this.options [i];
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
				return flagNames.Count;
			}
		}

		bool FlagIsUsed (string expression)
		{
			string flag = StoryModel.ExtractFlagNameFromExpression (expression);
			for (int i = 0; i < flagNames.Count; i++) {
				string n = StoryModel.ExtractFlagNameFromExpression (flagNames[i]);
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
			if (index < 0 || index >= flagNames.Count) {
				return "";
			}
			return flagNames [index];
		}

		public void EditFlag (string flag, StoryModel model)
		{
			int index = flagNames.IndexOf (flag);
			if (index == -1) {
				flagNames.Add (flag);
				model.AddFlagToIndex (flag);
			} else {
				flagNames.RemoveAt (index);
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

