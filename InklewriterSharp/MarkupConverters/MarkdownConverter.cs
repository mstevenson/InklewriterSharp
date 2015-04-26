namespace Inklewriter.MarkupConverters
{
	public class MarkdownConverter : IMarkupConverter
	{
		public string ReplaceLinkUrlMarkup (string url, string label)
		{
			if (string.IsNullOrEmpty (url) && string.IsNullOrEmpty (label)) {
				return "";
			}
			return string.Format ("[{0}]({1})", label, url);
		}

		public string ReplaceImageUrlMarkup (string url)
		{
			if (string.IsNullOrEmpty (url)) {
				return "";
			}
			return string.Format ("![{0}]({1})", "", url);
		}

		public string ReplaceBoldStyleMarkup (string text)
		{
			if (string.IsNullOrEmpty (text)) {
				return "";
			}
			return string.Format ("**{0}**", text);
		}

		public string ReplaceItalicStyleMarkup (string text)
		{
			if (string.IsNullOrEmpty (text)) {
				return "";
			}
			return string.Format ("_{0}_", text);
		}
	}
}

