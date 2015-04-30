namespace Inklewriter.Player
{
	/// <summary>
	/// Content displayed in series.
	/// A BlockContent instance is likely to be either paragraph
	/// text or an image retrieved from a Stitch object.
	/// </summary>
	public class BlockContent<T>
	{
		public T Content { get; private set; }
		public bool IsVisible { get; private set; }

		public BlockContent (T content, bool isVisible)
		{
			this.Content = content;
			this.IsVisible = isVisible;
		}
	}
}

