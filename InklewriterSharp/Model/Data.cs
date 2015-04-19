using System.Collections.Generic;

namespace Inklewriter
{
	[System.Serializable]
	public class Data
	{
		public bool allowCheckpoints;

		public string initial;

		public bool optionMirroring;

		public EditorData editorData;

		public Dictionary<string, Stitch> stitches;
	}
}

