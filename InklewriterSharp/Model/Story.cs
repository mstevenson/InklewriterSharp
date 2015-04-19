using System;
using System.Collections.Generic;

namespace Inklewriter
{
	[System.Serializable]
	public class Story
	{
		public string Title { get; set; }

		public string CreatedAt { get; set; }

		public string UpdatedAt { get; set; }

		public string UrlKey { get; set; }


		public bool AllowCheckpoints { get; set; }

		public Stitch InitialStitch { get; set; }

		public bool OptionMirroring { get; set; }

		public EditorData EditorData { get; set; }

		public Dictionary<string, Stitch> Stitches { get; set; }
	}
}

