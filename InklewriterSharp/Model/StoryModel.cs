using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Inklewriter
{
	public class StoryModel
	{
		const string defaultStoryName = "Untitled Story";
		const string defaultAuthorName = "Anonymous";
		const int maxPreferredPageLength = 8;

		public Story Story { get; private set; }

		public void ImportStory (string data)
		{
			Story = StoryIO.Read (data);
		}

		public string ExportStory ()
		{
			if (Story) {
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
				usedShortNames [shortName] = true;
				currentStitch.Name (shortName);
			}
		}

		public List<Stitch> Stitches {
			get {
				return Story.data.stitches ?? new List<Stitch> ();
			}
		}
	}
}
