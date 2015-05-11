/*
	inklewriter Copyright (c) 2012 inkle Ltd
	C# port Copyright (c) 2015 Michael Stevenson

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/

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
