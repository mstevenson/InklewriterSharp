using System;
using Eto.Forms;
using Eto.Drawing;
using Inklewriter;
using Inklewriter.Player;
using System.IO;

namespace InklewriterEditor
{
	public class MainForm : Form
	{
		StoryModel model;
		bool isDirty;
		string filePath;

		public MainForm ()
		{
			Title = "My Eto Form";
			ClientSize = new Size (400, 350);

			// scrollable region as the main content
			Content = new Scrollable {
				// table with three rows
				Content = new TableLayout (
					null,
					// row with three columns
					new TableRow (null, new Label { Text = "Hello World!" }, null),
					null
				)
			};

			var newStory = new Command {
				MenuText = "New Story",
				Shortcut = Application.Instance.CommonModifier | Keys.N
			};
			newStory.Executed += (sender, e) => {
				if (isDirty) {
					// TODO save before new
				}
				NewStory ();
			};

			// create a few commands that can be used for the menu and toolbar
			var saveStory = new Command {
				MenuText = "Save",
				Shortcut = Application.Instance.CommonModifier | Keys.S
//				ToolBarText = "New Story"
			};
			saveStory.Executed += (sender, e) => {
				var s = new SaveFileDialog {
					Title = "Save Story File",
					Filters = new[] { new FileDialogFilter ("json", "json") },
				};
				s.ShowDialog (this);
				if (!string.IsNullOrEmpty (s.FileName)) {
					SaveStory (s.FileName);
				}
			};

			var openStory = new Command {
				MenuText = "Open Story",
				Shortcut = Application.Instance.CommonModifier | Keys.O
			};
			openStory.Executed += (sender, e) => {
				var o = new OpenFileDialog {
					MultiSelect = false,
					Title = "Open Story File",
					Filters = new[] { new FileDialogFilter ("json", "json") },
					CheckFileExists = true
				};
				o.ShowDialog (this);
				if (!string.IsNullOrEmpty (o.FileName)) {
					OpenStory (o.FileName);
				}
			};

			var quitCommand = new Command {
				MenuText = "Quit",
				Shortcut = Application.Instance.CommonModifier | Keys.Q
			};
			quitCommand.Executed += (sender, e) => Application.Instance.Quit ();

			var aboutCommand = new Command { MenuText = "About..." };
			aboutCommand.Executed += (sender, e) => MessageBox.Show (this, "About my app...");

			// create menu
			Menu = new MenuBar {
				Items = {
					// File submenu
					new ButtonMenuItem { Text = "&File", Items = { newStory, saveStory, openStory } },
					new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
				ApplicationItems = {
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
				},
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};

			// create toolbar			
//			ToolBar = new ToolBar { Items = { clickMe } };
		}

		void NewStory ()
		{
			var story = new Story ();
			model = StoryModel.Create (story);
			filePath = null;
		}

		void SaveStory (string path)
		{
			filePath = path;
			var sw = new StreamWriter (path);
			var writer = new JsonStoryWriter (sw);
			writer.Write (model.Story);
		}

		void OpenStory (string path)
		{
			filePath = path;
			var data = File.ReadAllText (path);
			model = StoryModel.Create (data);
		}
	}
}
