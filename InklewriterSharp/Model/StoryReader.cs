using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using System.Linq;

namespace Inklewriter
{
	public class StoryReader
	{
		public static Story Read (string data)
		{
			JsonReader reader = new JsonReader (data);
			Story story = new Story ();

			ReadStoryRoot (reader, story);

			// Post-process
			var allStitches = story.Stitches;
			foreach (var stitch in allStitches) {
//				if (stitch.DivertStitch != null) {
//					var target = allStitches [stitch.DivertStitch.Name];
//					stitch.DivertTo (target);
//				}
			}

			// TODO wire up options
//			T.text(x.option), T.writeModeOnly = x.writeModeOnly, x.linkPath && T.linkStitch(r[x.linkPath].storyStitch), T._parentStitch = y.storyStitch;
//			if (x.ifConditions)
//				for (var E = 0; E < x.ifConditions.length; ++E) T._ifConditions.push(x.ifConditions[E].ifCondition);
//			if (x.notIfConditions)
//				for (var N = 0; N < x.notIfConditions.length; ++N) T._notIfConditions.push(x.notIfConditions[N].notIfCondition)



//			if (r[t.initial]) StoryModel.initialStitch = r[t.initial].storyStitch;
//			else {
//				StoryModel.initialStitch = StoryModel.stitches[0];
//				for (var C = 0; C < StoryModel.stitches.length; C++) {
//					var k = StoryModel.stitches[C].pageNumberLabel();
//					k > 0 && StoryModel.stitches[C].pageNumberLabel(k + 1)
//				}
//				StoryModel.initialStitch.pageNumberLabel(1)
//			}
//			return StoryModel.optionMirroring =
//				t.optionMirroring !== undefined ? t.optionMirroring : !0,
//			StoryModel.allowCheckpoints = t.allowCheckpoints !== undefined ? t.allowCheckpoints : !1, t.editorData
//			&& (t.editorData.playPoint
//				&& r[t.editorData.playPoint] ?
//				n.playPoint = r[t.editorData.playPoint].storyStitch :
//				n.playPoint = StoryModel.initialStitch,
//				n.libraryVisible = t.editorData.libraryVisible,
//				t.editorData.textSize !== undefined ?
//				n.textSize = t.editorData.textSize :
//				n.textSize = 0, t.editorData.authorName
//				&& StoryModel.setAuthorName(t.editorData.authorName)),
//			StoryModel.loading = !1, StoryModel.updateGraphModel(),
//			StoryModel.collateFlags(), n

			return story;
		}

		static void ReadStoryRoot (JsonReader reader, Story story)
		{
			// read object start
			reader.Read ();

			while (reader.Read ()) {
				if (reader.Token == JsonToken.ObjectEnd) {
					return;
				}
				if (reader.Token != JsonToken.PropertyName) {
					continue;
				}
				string propertyName = (string)reader.Value;
				reader.Read ();
				switch (propertyName) {
				case "created_at":
					story.CreatedAt = GetDateTime ((string)reader.Value);
					break;
				case "data":
					ReadData (reader, story);
					break;
				case "title":
					story.Title = (string)reader.Value;
					break;
				case "updated_at":
					story.UpdatedAt = GetDateTime ((string)reader.Value);
					break;
				case "url_key":
					story.UrlKey = (string)reader.Value;
					break;
				}
			}
		}

		static System.DateTime GetDateTime (string s)
		{
			string dateString = s;
			string format = "YYYY-MM-DD'T'HH:mm:ss'Z'";
			return System.DateTime.ParseExact (dateString, format, System.Globalization.CultureInfo.InvariantCulture);
		}

		static void ReadData (JsonReader reader, Story story)
		{
			while (reader.Read ()) {
				if (reader.Token == JsonToken.ObjectEnd) {
					break;
				}
				if (reader.Token != JsonToken.PropertyName) {
					continue;
				}
				string propertyName = (string)reader.Value;
				reader.Read ();
				switch (propertyName) {
				case "allowCheckpoints":
					story.AllowCheckpoints = (bool)reader.Value;
					break;
				case "editorData":
					ReadDataEditor (reader, story);
					break;
				case "initial":
					var stitchName = (string)reader.Value;
					story.InitialStitch = GetOrCreateStitch (story, stitchName);
					break;
				case "optionMirroring":
					story.OptionMirroring = (bool)reader.Value;
					break;
				case "stitches":
					ReadStitches (reader, story);
					break;
				}
			}
		}

		static void ReadDataEditor (JsonReader reader, Story story)
		{
			story.EditorData = new EditorData ();

			while (reader.Read ()) {
				if (reader.Token == JsonToken.ObjectEnd) {
					break;
				}
				string propertyName = (string)reader.Value;
				reader.Read ();

				switch (propertyName) {
				case "authorName":
					story.EditorData.AuthorName = (string)reader.Value;
					break;
				case "libraryVisible":
					story.EditorData.LibraryVisible = (bool)reader.Value;
					break;
				case "playPoint":
					story.EditorData.PlayPoint = (string)reader.Value;
					break;
				case "textSize":
					story.EditorData.TextSize = (int)reader.Value;
					break;
				}
			}
		}

		static void ReadStitches (JsonReader reader, Story story)
		{
			while (reader.Read ()) {
				if (reader.Token == JsonToken.ObjectEnd) {
					break;
				}

				string key = (string)reader.Value;
				//Begin object containing only 'content' array
				reader.Read ();

				if (reader.Token == JsonToken.ObjectStart) {
					// reader 'content'
					reader.Read ();
					// enter 'content' array start
					reader.Read ();
					// read stitch's raw text
					reader.Read ();

					var stitch = GetOrCreateStitch (story, key);
					stitch.Text = (string)reader.Value;
					if (stitch.Text.EndsWith ("[...]")) {
						stitch.Text = Regex.Replace (stitch.Text, @"\[\.\.\.\]", "");
						stitch.RunOn = true;
					}

					// Read all stitch options. Exits when the array end token is read.
					bool haveMoreStitchContent = false;
					do {
						haveMoreStitchContent = ReadStitchContent (reader, story, stitch);
					} while (haveMoreStitchContent);

					// Read stitch's object end token, save the stitch
					reader.Read ();
				}
			}
		}

		static bool ReadStitchContent (JsonReader reader, Story story, Stitch stitch)
		{
			// Start object
			reader.Read ();
			if (reader.Token != JsonToken.ObjectStart) {
				return false;
			}

			string linkPath = null;
			string option = null;

			List<string> ifConditions = null;
			List<string> notIfConditions = null;

			string image = null;
			int? pageNum = null;
			string pageLabel = null;
			bool? runOn = null;
			string divert = null;
			List<string> flagNames = null;

			while (reader.Read ()) {
				if (reader.Token == JsonToken.ObjectEnd) {
					break;
				}
				string propertyName = reader.Value as string;
				reader.Read ();

				if (reader.Token == JsonToken.Null) {
					continue;
				}

				switch (propertyName) {
				case "option":
					option = (string)reader.Value;
					break;
				case "linkPath":
					linkPath = (string)reader.Value;
					break;
				case "ifCondition": // belongs to stitch
					if (ifConditions == null) {
						ifConditions = new List<string> ();
					}
					string ifCondition = (string)reader.Value;
					ifConditions.Add (ifCondition);
					break;
				case "notIfCondition": // belongs to stitch
					if (notIfConditions == null) {
						notIfConditions = new List<string> ();
					}
					string notIfCondition = (string)reader.Value;
					notIfConditions.Add (notIfCondition);
					break;
				case "ifConditions": // belongs to option
					ifConditions = ReadOptionConditions (reader);
					break;
				case "notIfConditions": // belongs to option
					notIfConditions = ReadOptionConditions (reader);
					break;
				case "image":
					image = (string)reader.Value;
					break;
				case "pageNum":
					pageNum = (int)reader.Value;
					break;
				case "pageLabel":
					pageLabel = (string)reader.Value;
					break;
				case "runOn":
					runOn = (bool)reader.Value;
					break;
				case "divert":
					divert = (string)reader.Value;
					break;
				case "flagName":
					if (flagNames == null) {
						flagNames = new List<string> ();
					}
					flagNames.Add ((string)reader.Value);
					break;
				}
			}

			if (option != null) { // is an option object
				var newOption = new Option ();
				newOption.Text = option;
				newOption.IfConditions = ifConditions;
				newOption.NotIfConditions = notIfConditions;

				newOption.LinkStitch = GetOrCreateStitch (story, linkPath);

				if (stitch.Options == null) {
					stitch.Options = new List<Option> ();
				}
				stitch.Options.Add (newOption);
			} else {
				stitch.IfConditions = ifConditions;
				stitch.NotIfConditions = notIfConditions;
				stitch.Image = image;
				if (pageNum.HasValue) {
					stitch.PageNumber = pageNum.Value;
				}
				stitch.PageLabel = pageLabel;
				if (runOn.HasValue) {
					stitch.RunOn = runOn.Value;
				}
				stitch.DivertStitch = GetOrCreateStitch (story, divert);
				stitch.Flags = flagNames;
			}
			return true;
		}

		static List<string> ReadOptionConditions (JsonReader reader)
		{
			if (reader.Token == JsonToken.Null) {
				return null;
			}

			// Read array start
			reader.Read ();

			// Read first object start
			reader.Read ();

			List<string> conditions = new List<string> ();

			while (reader.Token != JsonToken.ArrayEnd) {
				reader.Read ();
				string condition = (string)reader.Value;
				conditions.Add (condition);
				reader.Read ();
				// Read next object start, or the end of the array
				reader.Read ();
			}
			return conditions;
		}

		static Stitch GetOrCreateStitch (Story story, string stitchName)
		{
			if (string.IsNullOrEmpty (stitchName)) {
				return null;
			}
			Stitch stitch = story.Stitches.FirstOrDefault (s => s.Name == stitchName);
			if (stitch == null) {
				stitch = new Stitch ();
				stitch.Name = stitchName;
				story.Stitches.Add (stitch);
			}
			return stitch;
		}
	}
}

