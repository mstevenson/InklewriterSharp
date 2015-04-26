using System.Collections.Generic;

namespace Inklewriter.Player
{
	/// <summary>
	/// A series of stitches ending in a block of selectable options. Includes an optional illustration image.
	/// </summary>
	public class PlayChunk
	{
		/// <summary>
		/// In-line illustration image URL.
		/// </summary>
		public string image;

		/// <summary>
		/// All stitches belonging to this play chunk. Stitches that pass flag validation
		/// will have the isVisible value set to true.
		/// </summary>
		public List<BlockContent<Stitch>> stitches = new List<BlockContent<Stitch>> ();

		public List<BlockContent<Option>> options = new List<BlockContent<Option>> ();

		/// <summary>
		/// All flags recorded during play up to and including to this chunk.
		/// </summary>
		public List<FlagValue> flagsCollected = new List<FlagValue> ();

		/// <summary>
		/// Body text compiled from all stitches belonging to this chunk, post-styling.
		/// Stitches that do not pass flag validation will not be included in this text.
		/// </summary>
		public string text;
	}
}

