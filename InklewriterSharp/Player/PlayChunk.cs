using System.Collections.Generic;

namespace Inklewriter.Player
{
	/// <summary>
	/// A series of stitches ending in a block of selectable options. Includes an optional illustration image.
	/// </summary>
	public class PlayChunk
	{
		/// <summary>
		/// Paragraphs of styled text that compose the chunk.
		/// This is a processed and display-ready form of the Stitches list.
		/// </summary>
		public List<Paragraph> Paragraphs { get; internal set; }

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
		/// Is the final chunk in the story.
		/// </summary>
		public bool IsEnd {
			get {
				foreach (var o in Options) {
					if (o.isVisible) {
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Is the beginning of a new story section.
		/// </summary>
		public bool HasSectionHeading { get; internal set; }

		public PlayChunk ()
		{
			Stitches = new List<BlockContent<Stitch>> ();
			Options = new List<BlockContent<Option>> ();
			FlagsCollected = new List<FlagValue> ();
			Paragraphs = new List<Paragraph> ();
		}
	}
}

