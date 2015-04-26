InkleSharp
==========

An unofficial C# reimplementation of [inklewriter](https://writer.inklestudios.com), an interactive story system created by [inkle studios](http://www.inklestudios.com).

Instructions
------------

To generate story content:

1. Write a story with [inklewriter](https://writer.inklestudios.com).
2. Generate a share link for your story and append *.json* to the end of the URL. Download this file.

You can load your story into your application with just a few lines of code:

    string data = File.ReadAllText ("story.json");
    StoryModel model = new StoryModel ();
    model.ImportStory (storyJson);
    StoryPlayer player = new StoryPlayer (model, new Inklewriter.MarkupConverters.ConsoleMarkupConverter ());

It's now up to you to display your story's content and to advance the story state when options are selected. Here's the general approach to implementing a custom story player:

1. Create a class that implements `IMarkupConverter` and pass this into your `StoryPlayer` constructor. Markup converters transform inklewriter's image, url, and styled text markup into something suitable for your specific application. A few sample converters are provided in the Inklewriter.MarkupConverters namespace.
2. Call `storyPlayer.GetChunkFromStitch(storyPlayer.InitialStitch)` to grab the first chunk of the story. A `PlayChunk` contains a block of text, and optionally may contain a link to an image and a series of selectable options.
3. Display `chunk.Image` if it exists.
4. Display each option in `chunk.Options` for which option.isVisible is true. If the option is not visible it means that one or more required flags ('markers' in inklewriter) have not been satisfied.
5. Wait for player input. When the player selects an option, call `storyPlayer.GetChunkFromStitch(option.LinkStitch)` to grab the next chunk.
6. Repeat this process until you display a chunk that contains no options. With nowhere else to go, the story has come to an end.

See InkleSharp.Examples.Program.cs for a complete example implementation that runs in the console.

License
-------

The InkleSharp source code is licensed under the MIT License. Inkle story files may be used commercially with a credit to inkle studios.