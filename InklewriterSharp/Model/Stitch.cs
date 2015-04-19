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


		public int ComputedPageNumber {
			get {
				return 0;
			}
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

		public void Divert (Stitch stitch)
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

		public void AddOption ()
		{
			//			var newOption = new Option (this);
			//			options.Add (newOption);
		}

		public void RemoveOption (Option option)
		{
			//			option.Unlink ();
			//			options.Remove (option);
		}

		public bool IsDead (Story story)
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

		public StitchStats Stats ()
		{
			var stats = new StitchStats ();
			stats.numOptions = options.Count;
			if (options.Count > 0) {
				stats.numLinked = 0;
				for (int i = 0; i < stats.numOptions; i++) {
					Option n = this.options [i];
					if (n.LinkSwitch ()) {
						stats.numLinked++;
					} else {
						stats.looseEnds.Add (n);
					}
				}
				stats.numLooseEnds = stats.numOptions - stats.numLinked;
			}
			return stats;
		}

		int NumberOfFlags {
			get {

				// FIXME


				return 0;
			}
		}

		public void Name (string shortName)
		{
			// TODO
		}

		public Stitch DivertedStitch { get; private set; }
	}
}

