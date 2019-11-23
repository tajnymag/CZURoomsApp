using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CZURoomsApp.Models;
using CZURoomsApp.Sections;
using CZURoomsApp.Services;
using Eto.Forms;
using Eto.Drawing;

namespace CZURoomsApp
{
	public sealed class MainForm : Form
	{
		public MainForm()
		{
			Title = "My Eto Form";
			ClientSize = new Size(400, 350);

			var view = new ListBox();

			var uis = new CZUApi("xlukm014", "NaserSiCZU5");

			Content = new MainSection(view);
			
			Test(uis, view);

			// create a few commands that can be used for the menu and toolbar
			var clickMe = new Command {MenuText = "Click Me!", ToolBarText = "Click Me!"};
			clickMe.Executed += (sender, e) => Test(uis, view);

			var quitCommand = new Command {MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q};
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			var aboutCommand = new Command {MenuText = "About..."};
			aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					// File submenu
					new ButtonMenuItem {Text = "&File", Items = {clickMe}}
				},
				ApplicationItems =
				{
					// application (OS X) or file menu (others)
					new ButtonMenuItem {Text = "&Preferences..."}
				},
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};

			// create toolbar			
			ToolBar = new ToolBar {Items = {clickMe}};
		}

		private async void Test(CZUApi uis, ListBox view)
		{
			await uis.Login();
			var html = await uis.GetRoomPage(ClassRoom.ALL, DateTime.Today, DateTime.Today.AddDays(7),
				DayOfWeek.Wednesday);

			List<TimetableEvent> timetableEvents = null;

			await Task.Run(() =>
			{
				var parser = new CZUParser(html);
				timetableEvents = parser.GetTimetableEvents(parser.GetAllRows());
			});

			foreach (var timetableEvent in timetableEvents)
			{
				view.Items.Add(new ListItem { Text = $"{timetableEvent.Room} {timetableEvent.Interval}" });
			}
		}
	}
}