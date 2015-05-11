InklewriterSharp
================

An unofficial C# reimplementation of the [inklewriter](https://writer.inklestudios.com) player, an interactive story system created by [inkle studios](http://www.inklestudios.com).

Instructions
------------

To create a story:

1. Write a story with [inklewriter](https://writer.inklestudios.com).
2. Generate a share link for your story and append *.json* to the end of the URL. Download this file and include it in your project.

You can load a story into your application with just a few lines of code:

```csharp
string jsonData = System.IO.File.ReadAllText("story.json");
StoryModel model = StoryModel.Create(jsonData);
StoryPlayer player = new StoryPlayer(model, new Inklewriter.MarkupConverters.ConsoleMarkupConverter());
```

In the above example, **StoryModel** is the C# object representation of the inklewriter story. **StoryPlayer** tracks internal story state and available options, and generates blocks of content to be displayed.

The process of reading player input and displaying images and text will vary widely between applications, so it's up to you to complete this loop in your own code. Here's the general approach to implementing a custom story player:

1. Create a class that implements the `IMarkupConverter` interface and pass this into your `StoryPlayer` constructor. Markup converters transform inklewriter's image, url, and styled text markup into something suitable for your specific application. A few sample converters are provided in the Inklewriter.MarkupConverters namespace.
2. Call `player.CreateFirstChunk()` to generate the first block of content in the story. A `PlayChunk` is composed of a list of paragraphs and a list of user-selectable options for advancing the story. 
3. Display each `Paragraph` object in the chunk. A `Paragraph` is composed of a string and an optional image URL. Images are intended to be displayed above text.
4. Display each `Option` object for which `IsVisible` is true. If an option is not visible this indicates that one or more required flags have not been satisfied. The flags tracked by StoryPlayer are modified each time a PlayChunk is generated.
5. Wait for player input. When the player selects an option, call `player.CreateChunkForOption(option)` to grab the next chunk of the story.
6. Repeat steps 3 through 5 until you receive a chunk for which `IsEnd` is true, indicating that the chunk has no visible options. With nowhere else to go, the story has come to an end.

Examples
--------

- **Console App** - see InkleSharp.Examples.Program.cs included in this project for a complete text-only implementation that will run in a Windows command prompt or OS X/Linux terminal.
- **Unity App** - available as a separate project, the [InklewriterUnity library](https://github.com/mstevenson/InklewriterUnity) plays inklewriter stories through the Unity game engine.

License
-------

The InkleSharp source code is licensed under the MIT License. Inklewriter story files may be used commercially with a credit to inkle studios.