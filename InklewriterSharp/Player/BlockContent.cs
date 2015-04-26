namespace Inklewriter.Player
{
	public class BlockContent<T>
	{
		public T content;
		public bool isVisible;

		public BlockContent (T content, bool isVisible)
		{
			this.content = content;
			this.isVisible = isVisible;
		}
	}
}

