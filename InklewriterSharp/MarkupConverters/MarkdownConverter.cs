namespace Inklewriter.MarkupConverters
{
	public class MarkdownConverter : IMarkupConverter
	{
		public string ReplaceLinkUrlMarkup (string url, string label)
		{
			return string.Format ("[{0}]({1})", label, url);
		}

		public string ReplaceImageUrlMarkup (string url)
		{
			return string.Format ("![{0}]({1})", "", url);
		}

		public string ReplaceBoldStyleMarkup (string text)
		{
			return string.Format ("**{0}**", text);
		}

		public string ReplaceItalicStyleMarkup (string text)
		{
			return string.Format ("_{0}_", text);
		}
	}
}

