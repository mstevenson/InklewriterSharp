using System;

namespace Inklewriter
{
	public class StoryWriter
	{
		static string GetDateTimeString (System.DateTime date)
		{
			string format = "yyyy-MM-dd'T'HH:mm:ss'Z'";
			return date.ToUniversalTime ().ToString (format, System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string Write (Story story)
		{
			JsonObject rootObj = new JsonObject ();

			rootObj ["title"] = story.Title;
			rootObj ["url_key"] = story.UrlKey;
			rootObj ["created_at"] = GetDateTimeString (story.CreatedAt);
			rootObj ["updated_at"] = GetDateTimeString (story.UpdatedAt);

			var dataObj = new JsonObject ();
			rootObj ["data"] = dataObj;

			dataObj ["allowCheckpoints"] = story.AllowCheckpoints;

			var editorDataObj = new JsonObject ();
			dataObj ["editorData"] = editorDataObj;

			editorDataObj ["authorName"] = story.EditorData.AuthorName;
			editorDataObj ["libraryVisible"] = story.EditorData.LibraryVisible;
			editorDataObj ["playPoint"] = story.EditorData.PlayPoint.Name;
			editorDataObj ["textSize"] = story.EditorData.TextSize;

			dataObj ["initial"] = story.InitialStitch.Name;
			dataObj ["optionMirroring"] = story.OptionMirroring;

			var stitchesObj = new JsonObject ();
			dataObj ["stitches"] = stitchesObj;

			foreach (var s in story.Stitches) {
				var stitch = new JsonObject ();
				stitchesObj [s.Name] = stitch;
				var contentArray = new JsonArray ();
				stitch ["content"] = contentArray;

				// Stitch content items
				contentArray.Add (s.Text);
				if (s.DivertStitch != null) {
					var divertObj = new JsonObject ();
					contentArray.Add (divertObj);
					divertObj ["divert"] = s.DivertStitch.Name;
				}
				if (s.Flags != null) {
					foreach (var flag in s.Flags) {
						var flagsObj = new JsonObject ();
						contentArray.Add (flagsObj);
						flagsObj ["flagName"] = flag;
					}
				}
				if (s.Image != null) {
					var imageObj = new JsonObject ();
					contentArray.Add (imageObj);
					imageObj ["image"] = s.Image;
				}
				if (s.Options != null) {
					foreach (var option in s.Options) {
						var optionsObj = new JsonObject ();
						optionsObj ["option"] = option.Text;
						optionsObj ["linkPath"] = option.LinkStitch != null ? option.LinkStitch.Name : null;

						if (option.IfConditions != null) {
							var ifConditionsArray = new JsonArray ();
							optionsObj ["ifConditions"] = ifConditionsArray;
							foreach (var cond in option.IfConditions) {
								var conditionObj = new JsonObject ();
								ifConditionsArray.Add (conditionObj);
								conditionObj ["ifCondition"] = cond;
							}
						}

						if (option.NotIfConditions != null) {
							var notIfConditionsArray = new JsonArray ();
							optionsObj ["notIfConditions"] = notIfConditionsArray;
							foreach (var cond in option.NotIfConditions) {
								var conditionObj = new JsonObject ();
								notIfConditionsArray.Add (conditionObj);
								conditionObj ["notIfCondition"] = cond;
							}
						}
					}
				}
				if (s.PageLabel != null && s.PageNumber != -1) {
					var pageLabelObj = new JsonObject ();
					contentArray.Add (pageLabelObj);
					pageLabelObj ["pageLabel"] = s.PageLabel;
				}
				if (s.PageNumber != -1) {
					var pageNumberObj = new JsonObject ();
					contentArray.Add (pageNumberObj);
					pageNumberObj ["pageNumber"] = s.PageNumber;
				}
				if (s.RunOn) {
					var runOnObj = new JsonObject ();
					contentArray.Add (runOnObj);
					runOnObj ["runOn"] = s.RunOn;
				}
				if (s.IfConditions != null) {
					foreach (var condition in s.IfConditions) {
						var ifConditionsObj = new JsonObject ();
						contentArray.Add (ifConditionsObj);
						ifConditionsObj ["ifCondition"] = condition;
					}
				}
				if (s.NotIfConditions != null) {
					foreach (var condition in s.NotIfConditions) {
						var notIfConditionsObj = new JsonObject ();
						contentArray.Add (notIfConditionsObj);
						notIfConditionsObj ["notIfCondition"] = condition;
					}
				}
			}

			return SimpleJson.SerializeObject (rootObj);
		}
	}
}

