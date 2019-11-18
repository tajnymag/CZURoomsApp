using System;
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

            var view = new WebView {Width = 400, Height = 350};

            var uis = new CZUApi("xlukm014", "NaserSiCZU5");
            Test(uis, view);

            Content = new MainSection(view);

            // create a few commands that can be used for the menu and toolbar
            var clickMe = new Command {MenuText = "Click Me!", ToolBarText = "Click Me!"};
            clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

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
            ToolBar = new ToolBar {Items = {clickMe, clickMe}};
        }

        private async void Test(CZUApi uis, WebView view)
        {
            await Task.Run(async () =>
            {
                await uis.Login();
                var html = await uis.GetRoomPage(ClassRoom.PEFEII, DateTime.Today, DateTime.Today.AddDays(7),
                    DayOfWeek.Wednesday);

                view.LoadHtml(html);

                var parser = new CZUParser(html);
                var timetableEvent = parser.GetTimetableEvent(parser.GetAllRows()[0]);

                MessageBox.Show(timetableEvent.Interval.ToString());
            });
        }
    }
}