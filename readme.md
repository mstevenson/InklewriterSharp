InkleSharp
==========

An unofficial C# reimplementation of [inklewriter](https://writer.inklestudios.com), an interactive story system created by [inkle studios](http://www.inklestudios.com).

Instructions
------------

- Write a story with [inklewriter](https://writer.inklestudios.com).
- Generate a share link for your story and append *.json* to the end of the URL. Download this file.
- Read your story file into a string and import it into a StoryModel:

    string storyJson = File.ReadAllText ("story.json");
    StoryModel model = new StoryModel ();
    model.ImportStory (storyJson);

License
-------

The InkleSharp source code is covered under the MIT License. Inkle story files may be used commercially with a credit to inkle studios.