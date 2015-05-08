/*
	Copyright (c) 2015 Michael Stevenson

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/

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
		/// Is this the final chunk in the story?
		/// </summary>
		public bool IsEnd {
			get {
				foreach (var o in Options) {
					if (o.IsVisible) {
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Is the beginning of a new story section?
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

