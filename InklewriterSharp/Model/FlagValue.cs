namespace Inklewriter
{
	public class FlagValue
	{
		public string flagName;
		public int value;
		public bool isBoolean;

		public FlagValue ()
		{
		}

		public FlagValue (string name, bool isTrue)
		{
			flagName = name;
			value = isTrue ? 1 : 0;
			isBoolean = true;
		}

		public FlagValue (string name, int number)
		{
			flagName = name;
			value = number;
		}
	}
}

