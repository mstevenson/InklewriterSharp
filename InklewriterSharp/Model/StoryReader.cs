using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Inklewriter
{
	public class StoryReader
	{
		public static Story Read (string jsonData)
		{
			Story story = new Story ();
			var obj = (JsonObject)SimpleJson.DeserializeObject (jsonData);
			ReadStoryRoot (obj, story);
			return story;

//
//			ReadStoryRoot (reader, story);
//
//			// Post-process
////			foreach (var stitch in story.Stitches) {
////				if (stitch.DivertStitch != null) {
////					var target = story.Stitches [stitch.DivertStitch.Name];
////					stitch.DivertTo (target);
////				}
////			}
//
//			// TODO wire up options
////			T.text(x.option), T.writeModeOnly = x.writeModeOnly, x.linkPath && T.linkStitch(r[x.linkPath].storyStitch), T._parentStitch = y.storyStitch;
////			if (x.ifConditions)
////				for (var E = 0; E < x.ifConditions.length; ++E) T._ifConditions.push(x.ifConditions[E].ifCondition);
////			if (x.notIfConditions)
////				for (var N = 0; N < x.notIfConditions.length; ++N) T._notIfConditions.push(x.notIfConditions[N].notIfCondition)
//
//
//
////			if (r[t.initial]) StoryModel.initialStitch = r[t.initial].storyStitch;
////			else {
////				StoryModel.initialStitch = StoryModel.stitches[0];
////				for (var C = 0; C < StoryModel.stitches.length; C++) {
////					var k = StoryModel.stitches[C].pageNumberLabel();
////					k > 0 && StoryModel.stitches[C].pageNumberLabel(k + 1)
////				}
////				StoryModel.initialStitch.pageNumberLabel(1)
////			}
////			return StoryModel.optionMirroring =
////				t.optionMirroring !== undefined ? t.optionMirroring : !0,
////			StoryModel.allowCheckpoints = t.allowCheckpoints !== undefined ? t.allowCheckpoints : !1, t.editorData
////			&& (t.editorData.playPoint
////				&& r[t.editorData.playPoint] ?
////				n.playPoint = r[t.editorData.playPoint].storyStitch :
////				n.playPoint = StoryModel.initialStitch,
////				n.libraryVisible = t.editorData.libraryVisible,
////				t.editorData.textSize !== undefined ?
////				n.textSize = t.editorData.textSize :
////				n.textSize = 0, t.editorData.authorName
////				&& StoryModel.setAuthorName(t.editorData.authorName)),
////			StoryModel.loading = !1, StoryModel.updateGraphModel(),
////			StoryModel.collateFlags(), n
//
//			return story;
		}

		static void ReadStoryRoot (JsonObject obj, Story story)
		{
			story.Title = (string)obj ["title"];
			JsonObject data = (JsonObject)obj ["data"];
			JsonObject editorData = (JsonObject)data ["editorData"];
			story.EditorData.AuthorName = (string)editorData["authorName"];

			foreach (var kvp in obj) {
				string property = kvp.Key;
				object value = kvp.Value;
				switch (property) {
				case "created_at":
					story.CreatedAt = System.DateTime.Parse ((string)value);
					break;
				case "data":
					ReadData ((JsonObject)value, story);
					break;
				case "title":
					story.Title = (string)value;
					break;
				case "updated_at":
					story.UpdatedAt = System.DateTime.Parse ((string)value);
					break;
				case "url_key":
					story.UrlKey = (string)value;
					break;
				}
			}
		}

		static void ReadData (JsonObject obj, Story story)
		{
			foreach (var kvp in obj) {
				string property = kvp.Key;
				object value = kvp.Value;
				switch (property) {
				case "allowCheckpoints":
					story.AllowCheckpoints = (bool)value;
					break;
				case "editorData":
					ReadDataEditor ((JsonObject)value, story);
					break;
				case "initial":
					var stitchName = (string)value;
					story.InitialStitch = GetOrCreateStitch (story, stitchName);
					break;
				case "optionMirroring":
					story.OptionMirroring = (bool)value;
					break;
				case "stitches":
					ReadStitches ((JsonObject)value, story);
					break;
				}
			}
		}

		static void ReadDataEditor (JsonObject obj, Story story)
		{
			story.EditorData = new EditorData ();
			foreach (var kvp in obj) {
				string property = kvp.Key;
				object value = kvp.Value;
				switch (property) {
				case "authorName":
					story.EditorData.AuthorName = (string)value;
					break;
				case "libraryVisible":
					story.EditorData.LibraryVisible = (bool)value;
					break;
				case "playPoint":
					story.EditorData.PlayPoint = (string)value;
					break;
				case "textSize":
					story.EditorData.TextSize = (EditorData.TextSizeType)(ParseInt(value));
					break;
				}
			}
		}

		static int ParseInt (object obj)
		{
			return System.Convert.ToInt32 ((long)obj);
		}

		static void ReadStitches (JsonObject obj, Story story)
		{
			foreach (var kvp in obj) {
				string name = (string)kvp.Key;
				JsonArray content = (JsonArray)(((JsonObject)obj [name])["content"]);

				var stitch = GetOrCreateStitch (story, name);

				// Set body text
				stitch.Text = (string)content[0];
				if (stitch.Text.EndsWith ("[...]")) {
					stitch.Text = Regex.Replace (stitch.Text, @"\[\.\.\.\]", "");
					stitch.RunOn = true;
				}

				// Parse content objects
				if (content.Count > 1) {
					for (int i = 1; i < content.Count; i++) {
						ReadStitchContent ((JsonObject)content [i], story, stitch);
					}
				}
			}
		}

		static void ReadStitchContent (JsonObject obj, Story story, Stitch stitch)
		{
			// Parse option content object
			if (obj.ContainsKey ("option")) {
				Option option = stitch.AddOption ();
				foreach (var kvp in obj) {
					string property = kvp.Key;
					object value = kvp.Value;
					switch (property) {
					case "option":
						option.Text = (string)value;
						break;
					case "linkPath":
						option.LinkStitch = GetOrCreateStitch (story, (string)value);
						break;
					case "ifConditions":
						var ifConditionsArray = (JsonArray)value;
						if (ifConditionsArray == null) {
							break;
						}
						foreach (var c in ifConditionsArray) {
							var val = (string)((JsonObject)c) ["ifCondition"];
							option.IfConditions.Add (val);
						}
						break;
					case "notIfConditions":
						var notIfConditionsArray = (JsonArray)value;
						if (notIfConditionsArray == null) {
							break;
						}
						foreach (var c in notIfConditionsArray)
						{
							var val = (string)((JsonObject)c)["notIfCondition"];
							option.NotIfConditions.Add (val);
						}
						break;
					}
				}
			} else {
				foreach (var kvp in obj) {
					string property = (string)kvp.Key;
					object value = kvp.Value;
					switch (property) {
					case "runOn":
						stitch.RunOn = (bool)value;
						break;
					case "pageNum":
						stitch.PageNumber = ParseInt (value);
						break;
					case "pageLabel":
						stitch.PageLabel = (string)value;
						break;
					case "divert":
						var divertStitch = GetOrCreateStitch (story, (string)value);
						stitch.DivertTo (divertStitch);
						break;
					case "image":
						stitch.Image = (string)value;
						break;
					case "flagName":
						stitch.Flags.Add ((string)value);
						break;
					case "ifCondition":
						stitch.IfConditions.Add ((string)value);
						break;
					case "notIfCondition":
						stitch.NotIfConditions.Add ((string)value);
						break;
					}
				}
			}
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

