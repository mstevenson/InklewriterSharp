/*
	inklewriter Copyright (c) 2012 inkle Ltd
	C# port Copyright (c) 2015 Michael Stevenson

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

using System;
using System.Collections.Generic;

namespace Inklewriter
{
	public class Option
	{
		/// <summary>
		/// The option text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Display this option only if the specified markers have been set.
		/// </summary>
		public List<string> IfConditions { get; set; }

		/// <summary>
		/// Display this option only if the specified markers are not set.
		/// </summary>
		public List<string> NotIfConditions = new List<string> ();

		public Stitch LinkStitch { get; set; }

		public Stitch ParentStitch { get; set; }

		public Option ()
		{
			Text = "";
			IfConditions = new List<string> ();
			NotIfConditions = new List<string> ();
		}

		public Option (Stitch parent) : this()
		{
			ParentStitch = parent;
		}

		public void CreateLinkStitch (Stitch target)
		{
			if (target == null) {
				return;
			}
			if (LinkStitch == target) {
				return;
			}
			if (LinkStitch != null) {
				LinkStitch.RefCount--;
			}
			LinkStitch = target;
			LinkStitch.RefCount++;
		}

		public void Unlink ()
		{
			if (LinkStitch == null) {
				return;
			}
			LinkStitch.RefCount--;
			LinkStitch = null;
		}
	}
}

