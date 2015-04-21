using System;
using System.Collections.Generic;

namespace Inklewriter
{
	[System.Serializable]
	public class Story
	{
		public string Title { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }

		public string UrlKey { get; set; }

		/// <summary>
		/// Provides more rewind points.
		/// </summary>
		public bool AllowCheckpoints { get; set; }

		public Stitch InitialStitch { get; set; }

		/// <summary>
		/// Displays an option once chosen.
		/// </summary>
		public bool OptionMirroring { get; set; }

		public EditorData EditorData { get; set; }

		public List<Stitch> Stitches { get; set; }

		public Story ()
		{
			Stitches = new List<Stitch> ();
			CreatedAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;
			EditorData = new EditorData ();
		}
	}
}

