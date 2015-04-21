using System;

namespace Inklewriter
{
	[System.Serializable]
	public class EditorData
	{
		public string AuthorName { get; set; }

		public bool LibraryVisible { get; set; }

		public string PlayPoint { get; set; }

		public enum TextSizeType { Unknown = -1, Normal = 0, Compact = 1, Dense = 2 }

		public TextSizeType TextSize { get; set; }
	}
}

