namespace Inklewriter
{
	public struct FlagValue
	{
		public readonly string flagName;
		public readonly int value;
		public readonly bool isBoolean;

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
			isBoolean = false;
		}
	}
}

