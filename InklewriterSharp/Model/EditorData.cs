using System;

namespace Inklewriter
{
	[System.Serializable]
	public class EditorData
	{
		public string AuthorName { get; set; }

		public bool LibraryVisible { get; set; }

		public string PlayPoint { get; set; }

		public int TextSize { get; set; }
	}
}

