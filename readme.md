InklewriterSharp
================

An unofficial C# reimplementation of [inklewriter](https://writer.inklestudios.com), an interactive story system created by [inkle studios](http://www.inklestudios.com).

Instructions
------------

To create a story:

1. Write a story with [inklewriter](https://writer.inklestudios.com).
2. Generate a share link for your story and append *.json* to the end of the URL. Download this file.

You can load your story into your application with just a few lines of code:

```csharp
string jsonData = System.IO.File.ReadAllText("story.json");
StoryModel model = StoryModel.Create(jsonData);
StoryPlayer player = new StoryPlayer(model, new Inklewriter.MarkupConverters.ConsoleMarkupConverter ());
```

It's now up to you to display your story's content and to advance the story state when options are selected. Here's the general approach to implementing a custom story player:

1. Create a class that implements `IMarkupConverter` and pass this into your `StoryPlayer` constructor. Markup converters transform inklewriter's image, url, and styled text markup into something suitable for your specific application. A few sample converters are provided in the Inklewriter.MarkupConverters namespace.
2. Call `storyPlayer.GetChunkFromStitch(storyPlayer.InitialStitch)` to grab the first chunk of the story. A `PlayChunk` is composed of a list of paragraphs and a list of user-selectable options for advancing the story. 
3. Display each `Paragraph` object. A `Paragraph` is composed of a string and an optional image URL. Images are intended to be displayed before text.
4. Display each `Option` object for which `isVisible` is true. If an option is not visible it indicates that one or more required flags/markers have not been satisfied.
5. Wait for player input. When the player selects an option, call `storyPlayer.GetChunkFromStitch(option.LinkStitch)` to grab the next chunk.
6. Repeat this process until chunk.IsEnd is true, indicating a chunk that has no visible options. With nowhere else to go, the story has come to an end.

Examples
--------

- **Console App** - see InkleSharp.Examples.Program.cs included in this project for a complete text-only implementation that runs in a Windows command prompt or OS X/Linux terminal.
- **Unity App** - the InklewriterUnity project runs inklewriter stories in the Unity game engine, mimicking the appearance and functionality of the inklewriter web app.

License
-------

The InkleSharp source code is licensed under the MIT License. Inkle story files may be used commercially with a credit to inkle studios.