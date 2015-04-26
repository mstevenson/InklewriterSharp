using System;
using System.Text.RegularExpressions;

namespace Inklewriter.MarkupConverters
{
	public class HtmlConverter : IMarkupConverter
	{
		public string ReplaceLinkUrlMarkup (string url, string label)
		{
			return string.Format ("<a href=\"{0}\">{1}</a>", url, label);
		}

		public string ReplaceImageUrlMarkup (string url)
		{
			return string.Format ("<div id=\"illustration\"><img class=\"pic\" src=\"{0}\"/></div>", url);
		}

		public string ReplaceBoldStyleMarkup (string text)
		{
			return string.Format ("<b>{0}</b>", text);
		}

		public string ReplaceItalicStyleMarkup (string text)
		{
			return string.Format ("<i>{0}</i>", text);
		}
	}
}
