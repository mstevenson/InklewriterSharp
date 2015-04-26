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
		public string Image { get; internal set; }

		/// <summary>
		/// All stitches belonging to this play chunk. Stitches that pass flag validation
		/// will have the isVisible value set to true.
		/// </summary>
		public List<BlockContent<Stitch>> Stitches { get; private set; }

		public List<BlockContent<Option>> Options { get; private set; }

		/// <summary>
		/// All flags recorded during play up to and including to this chunk.
		/// </summary>
		public List<FlagValue> FlagsCollected { get; private set; }

		/// <summary>
		/// Body text compiled from all stitches belonging to this chunk, post-styling.
		/// Stitches that do not pass flag validation will not be included in this text.
		/// </summary>
		public string Text { get; internal set; }

		/// <summary>
		/// Is the final chunk in the story.
		/// </summary>
		public bool IsEnd { get; internal set; }

		/// <summary>
		/// Is the beginning of a new story section.
		/// </summary>
		public bool HasSectionHeading { get; internal set; }

		public PlayChunk ()
		{
			Stitches = new List<BlockContent<Stitch>> ();
			Options = new List<BlockContent<Option>> ();
			FlagsCollected = new List<FlagValue> ();
		}
	}
}

