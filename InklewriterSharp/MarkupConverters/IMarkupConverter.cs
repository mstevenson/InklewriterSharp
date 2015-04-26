using System;

namespace Inklewriter.MarkupConverters
{
	/// <summary>
	/// Interface for methods called by Player that convert inkelwriter styling markup
	/// to a another markup type.
	/// </summary>
	public interface IMarkupConverter
	{
		string ReplaceLinkUrlMarkup (string url, string label);

		string ReplaceImageUrlMarkup (string url);

		string ReplaceBoldStyleMarkup (string text);

		string ReplaceItalicStyleMarkup (string text);
	}
}

