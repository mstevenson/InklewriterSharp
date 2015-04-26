﻿using System.Collections.Generic;
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
					story.EditorData.PlayPoint = GetOrCreateStitch (story, (string)value);
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
						var contentItem = (JsonObject)content [i];
						if (contentItem.ContainsKey ("option")) {
							ReadOptionContentItem (contentItem, story, stitch);
						} else {
							ReadContentItem (contentItem, story, stitch);
						}
					}
				}
			}
		}

		static void ReadOptionContentItem (JsonObject obj, Story story, Stitch stitch)
		{
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
		}

		static void ReadContentItem (JsonObject obj, Story story, Stitch stitch)
		{
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

