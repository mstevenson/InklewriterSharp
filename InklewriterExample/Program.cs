using System;
using System.IO;
using Inklewriter;

namespace InklewriterExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string storyJson = File.ReadAllText ("tutorial.json");
			StoryModel model = new StoryModel ();
			Story story = model.ImportStory (storyJson);

			Console.WriteLine ("Loaded story file: " + story.title);

			Player player = new Player (story);
			player.Begin ();

			Console.WriteLine ("Began story.");

		}
	}
}
