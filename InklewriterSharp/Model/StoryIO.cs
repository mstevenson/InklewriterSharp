using System.Collections.Generic;
using LitJson;

namespace Inklewriter
{
	public class StoryIO
	{
		public static Story Read (string data)
		{
			JsonReader reader = new JsonReader (data);
			Story story = new Story ();

			ReadStoryRoot (reader, story);
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
					story.CreatedAt = (string)reader.Value;
					break;
				case "data":
					ReadData (reader, story);
					break;
				case "title":
					story.Title = (string)reader.Value;
					break;
				case "updated_at":
					story.UpdatedAt = (string)reader.Value;
					break;
				case "url_key":
					story.UrlKey = (string)reader.Value;
					break;
				}
			}
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
					story.EditorData.authorName = (string)reader.Value;
					break;
				case "libraryVisible":
					story.EditorData.libraryVisible = (bool)reader.Value;
					break;
				case "playPoint":
					story.EditorData.playPoint = (string)reader.Value;
					break;
				case "textSize":
					story.EditorData.textSize = (int)reader.Value;
					break;
				}
			}
		}

		static void ReadStitches (JsonReader reader, Story story)
		{
			story.Stitches = new Dictionary<string, Stitch> ();

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
					stitch.PageNum = pageNum.Value;
				}
				stitch.PageLabel = pageLabel;
				if (runOn.HasValue) {
					stitch.RunOn = runOn.Value;
				}
				stitch.Divert = GetOrCreateStitch (story, divert);
				stitch.FlagNames = flagNames;
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
			if (story.Stitches == null) {
				story.Stitches = new Dictionary<string, Stitch> ();
			}
			Stitch s = null;
			if (!story.Stitches.TryGetValue (stitchName, out s)) {
				s = new Stitch ();
				story.Stitches.Add (stitchName, s);
			}
			return s;
		}


		public static string Write (Story story)
		{
			JsonWriter writer = new JsonWriter ();
			writer.PrettyPrint = true;
			writer.WriteObjectStart ();

			writer.WritePropertyName ("created_at");
			// FIXME doesn't match correct date formatting
			writer.Write (story.CreatedAt);

			// Data
			writer.WritePropertyName ("data");
			writer.WriteObjectStart ();

			writer.WritePropertyName ("allowCheckpoints");
			writer.Write (story.AllowCheckpoints);

			writer.WritePropertyName ("editorData");
			{
				// Editor data
				writer.WriteObjectStart ();

				writer.WritePropertyName ("authorName");
				writer.Write (story.EditorData.authorName);

				writer.WritePropertyName ("libraryVisible");
				writer.Write (story.EditorData.libraryVisible);

				writer.WritePropertyName ("playPoint");
				writer.Write (story.EditorData.playPoint);

				writer.WritePropertyName ("textSize");
				writer.Write (story.EditorData.textSize);

				writer.WriteObjectEnd ();
			}

			writer.WritePropertyName ("initial");
			writer.Write (story.InitialStitch.Name);

			writer.WritePropertyName ("optionMirroring");
			writer.Write (story.OptionMirroring);

			writer.WritePropertyName ("stitches");
			{
				// Stitches
				writer.WriteObjectStart ();
				foreach (var kvp in story.Stitches) {
					writer.WritePropertyName (kvp.Key);
					writer.WriteObjectStart ();
					writer.WritePropertyName ("content");
					writer.WriteArrayStart ();
					{
						var stitch = kvp.Value;
						writer.Write (stitch.Text);

						if (stitch.Divert != null) {
							writer.WriteObjectStart ();
							writer.WritePropertyName ("divert");
							writer.Write (stitch.Divert.Name);
							writer.WriteObjectEnd ();
						}
						if (stitch.FlagNames != null) {
							foreach (var flag in stitch.FlagNames) {
								writer.WriteObjectStart ();
								writer.WritePropertyName ("flagName");
								writer.Write (flag);
								writer.WriteObjectEnd ();
							}
						}
						if (stitch.Image != null) {
							writer.WriteObjectStart ();
							writer.WritePropertyName ("image");
							writer.Write (stitch.Image);
							writer.WriteObjectEnd ();
						}
						if (stitch.Options != null) {
							foreach (var opt in stitch.Options) {
								writer.WriteObjectStart ();

								writer.WritePropertyName ("ifConditions");
								if (opt.IfConditions != null) {
									foreach (var condition in opt.IfConditions) {
										writer.WriteObjectStart ();
										writer.WritePropertyName ("ifCondition");
										writer.Write (condition);
										writer.WriteObjectEnd ();
									}
								} else {
									writer.Write (null);
								}

								writer.WritePropertyName ("linkPath");
								writer.Write (opt.LinkStitch != null ? opt.LinkStitch.Name : null);

								writer.WritePropertyName ("notIfConditions");
								if (opt.NotIfConditions != null) {
									foreach (var condition in opt.NotIfConditions) {
										writer.WriteObjectStart ();
										writer.WritePropertyName ("notIfCondition");
										writer.Write (condition);
										writer.WriteObjectEnd ();
									}
								} else {
									writer.Write (null);
								}

								writer.WritePropertyName ("option");
								writer.Write (opt.Text);

								writer.WriteObjectEnd ();
							}
						}
						if (stitch.PageLabel != null) {
							writer.WriteObjectStart ();
							writer.WritePropertyName ("pageLabel");
							writer.Write (stitch.PageLabel);
							writer.WriteObjectEnd ();
						}
						if (stitch.PageNum != -1) {
							writer.WriteObjectStart ();
							writer.WritePropertyName ("pageNum");
							writer.Write (stitch.PageNum);
							writer.WriteObjectEnd ();
						}
						if (stitch.RunOn) {
							writer.WriteObjectStart ();
							writer.WritePropertyName ("runOn");
							writer.Write (stitch.RunOn);
							writer.WriteObjectEnd ();
						}
						if (stitch.IfConditions != null) {
							writer.WriteObjectStart ();
							foreach (var condition in stitch.IfConditions) {
								writer.WritePropertyName ("ifCondition");
								writer.Write (condition);
							}
							writer.WriteObjectEnd ();
						}
						if (stitch.NotIfConditions != null) {
							writer.WriteObjectStart ();
							foreach (var condition in stitch.NotIfConditions) {
								writer.WritePropertyName ("notIfCondition");
								writer.Write (condition);
							}
							writer.WriteObjectEnd ();
						}
					}
					writer.WriteArrayEnd ();
					writer.WriteObjectEnd ();
				}
				writer.WriteObjectEnd ();
			}

			writer.WriteObjectEnd ();

			writer.WritePropertyName ("title");
			writer.Write (story.Title);

			writer.WritePropertyName ("updated_at");
			writer.Write (story.UpdatedAt);

			writer.WritePropertyName ("url_key");
			writer.Write (story.UrlKey);

			writer.WriteObjectEnd ();

			return writer.ToString ();
		}
	}
}

