using System;
using LitJson;

namespace Inklewriter
{
	public class StoryWriter
	{
		static string GetDateTimeString (System.DateTime date)
		{
			string format = "YYYY-MM-DD'T'HH:mm:ss'Z'";
			return date.ToUniversalTime ().ToString (format, System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string Write (Story story)
		{
			JsonWriter writer = new JsonWriter ();
			writer.PrettyPrint = true;
			writer.WriteObjectStart ();

			writer.WritePropertyName ("created_at");
			// FIXME doesn't match correct date formatting
			writer.Write (GetDateTimeString (story.CreatedAt));

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
				writer.Write (story.EditorData.AuthorName);

				writer.WritePropertyName ("libraryVisible");
				writer.Write (story.EditorData.LibraryVisible);

				writer.WritePropertyName ("playPoint");
				writer.Write (story.EditorData.PlayPoint);

				writer.WritePropertyName ("textSize");
				writer.Write ((int)story.EditorData.TextSize);

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
				foreach (var stitch in story.Stitches) {
					writer.WritePropertyName (stitch.Name);
					writer.WriteObjectStart ();
					writer.WritePropertyName ("content");
					writer.WriteArrayStart ();
					{
						writer.Write (stitch.Text);

						if (stitch.DivertStitch != null) {
							writer.WriteObjectStart ();
							writer.WritePropertyName ("divert");
							writer.Write (stitch.DivertStitch.Name);
							writer.WriteObjectEnd ();
						}
						if (stitch.Flags != null) {
							foreach (var flag in stitch.Flags) {
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
						if (stitch.PageNumber != -1) {
							writer.WriteObjectStart ();
							writer.WritePropertyName ("pageNum");
							writer.Write (stitch.PageNumber);
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
			writer.Write (GetDateTimeString (story.UpdatedAt));

			writer.WritePropertyName ("url_key");
			writer.Write (story.UrlKey);

			writer.WriteObjectEnd ();

			return writer.ToString ();
		}
	}
}

