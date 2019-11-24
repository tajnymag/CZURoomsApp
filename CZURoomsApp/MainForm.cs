using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CZURoomsApp.Dialogs;
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
			Title = "CZU Rooms";
			ClientSize = new Size(400, 350);

			var loginDialog = new LoginDialog();
			var credentials = loginDialog.ShowModal();

			if (credentials.Username == null || credentials.Password == null)
			{
				Application.Instance.Quit();
			}
			
			Shared.Uis = new CZUApi(credentials.Username, credentials.Password);

			var mainSection = new MainSection(this);

			Content = mainSection;

			// create a few commands that can be used for the menu and toolbar
			var loadTimetable = new Command {MenuText = "Načíst rozvrh", ToolBarText = "Načíst rozvrh"};
			loadTimetable.Executed += (sender, e) => mainSection.LoadEvents();
			
			var updateCredentials = new Command {MenuText = "Přehlásit se", ToolBarText = "Přehlásit se"};
			updateCredentials.Executed += (sender, e) =>
			{
				var ld = new LoginDialog();
				var cr = ld.ShowModal();

				if (cr.Username != null && cr.Password != null)
				{
					Shared.Uis.UpdateCredentials(cr.Username, cr.Password);
				}
			};

			var quitCommand = new Command {MenuText = "Ukončit", Shortcut = Application.Instance.CommonModifier | Keys.Q};
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			var aboutCommand = new Command {MenuText = "O aplikaci"};
			aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					// File submenu
					new ButtonMenuItem {Text = "&File", Items = {updateCredentials}}
				},
				ApplicationItems =
				{
					// application (OS X) or file menu (others)
					new ButtonMenuItem {Text = "&Nastavení"}
				},
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};

			// create toolbar			
			ToolBar = new ToolBar {Items = {updateCredentials, loadTimetable}};
		}
	}
}