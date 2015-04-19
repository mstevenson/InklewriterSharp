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
			model.ImportStory (storyJson);

			Console.WriteLine ("Loaded story file: " + model.Story.Title);

			Player player = new Player (model);
			player.Begin ();

			Console.WriteLine ("Began story.");
		}
	}
}
