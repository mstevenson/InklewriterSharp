using System;

namespace Inklewriter
{
	[System.Serializable]
	public class Option
	{
		/// <summary>
		/// The option text.
		/// </summary>
		public string option;

		/// <summary>
		/// The stitch to display after selecting this option.
		/// </summary>
		public string linkPath;

		/// <summary>
		/// Display this option only if the specified markers have been set.
		/// </summary>
		public List<string> ifConditions;

		/// <summary>
		/// Display this option only if the specified markers are not set.
		/// </summary>
		public List<string> notIfConditions;


		public bool LinkSwitch ()
		{
			// TODO implement
			return false;
		}
	}
}

