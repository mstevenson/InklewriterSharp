namespace Inklewriter.MarkupConverters
{
	public class HtmlConverter : IMarkupConverter
	{
		public string ReplaceLinkUrlMarkup (string url, string label)
		{
			if (string.IsNullOrEmpty (url) && string.IsNullOrEmpty (label)) {
				return "";
			}
			return string.Format ("<a href=\"{0}\">{1}</a>", url, label);
		}

		public string ReplaceImageUrlMarkup (string url)
		{
			if (string.IsNullOrEmpty (url)) {
				return "";
			}
			return string.Format ("<div id=\"illustration\"><img class=\"pic\" src=\"{0}\"/></div>", url);
		}

		public string ReplaceBoldStyleMarkup (string text)
		{
			if (string.IsNullOrEmpty (text)) {
				return "";
			}
			return string.Format ("<b>{0}</b>", text);
		}

		public string ReplaceItalicStyleMarkup (string text)
		{
			if (string.IsNullOrEmpty (text)) {
				return "";
			}
			return string.Format ("<i>{0}</i>", text);
		}
	}
}
