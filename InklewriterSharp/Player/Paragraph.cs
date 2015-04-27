using System;

namespace Inklewriter.Player
{
	/// <summary>
	/// A single paragraph of styled text which is composed of one or more stitches.
	/// Optionally includes the url of an image that should be displayed
	/// before the paragraph text.
	/// </summary>
	public class Paragraph
	{
		public string Text { get; private set; }
		public string Image { get; private set; }
		public string PageLabel { get; private set; }

		public Paragraph (string text, string image = null, string pageLabel = null)
		{
			this.Text = text;
			this.Image = image;
			this.PageLabel = pageLabel;
		}

		public override string ToString ()
		{
			return Text;
		}
	}
}

