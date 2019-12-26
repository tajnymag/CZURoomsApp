using System;
using CZURoomsApp.Sections;
using CZURoomsApp.Services;
using Eto.Drawing;
using Eto.Forms;

namespace CZURoomsApp
{
    /// <summary>
    /// Třída hlavního okna aplikace
    /// </summary>
    public sealed class MainForm : Form
    {
        public MainForm()
        {
            // Nastavení názvu hlavního okna a jeho velikosti
            Title = "CZU Rooms";
            ClientSize = new Size(480, 320);

            // Načtení hodnot sdíleného storu (možné načtení hodnot z disku)
            MainFormController.SetupStore();
            // Zobrazení přihlašovacího okna
            MainFormController.ShowLoginDialog(true);

            // Vytvoření instance hlavní sekce aplikace
            var mainSection = new MainSection();

            // Nastavení výše vytvořené instance jako hlavního obsahu okna
            Content = mainSection;

            // Vytvoření příkazů/tlačítek, jež poté můžeme přidat do toolbarů
            var loadTimetable = new Command {MenuText = "Načíst rozvrh", ToolBarText = "Načíst rozvrh"};
            loadTimetable.Executed += (sender, e) => mainSection.LoadEvents();

            var updateCredentials = new Command {MenuText = "Přehlásit se", ToolBarText = "Přehlásit se"};
            updateCredentials.Executed += (sender, e) => MainFormController.ShowLoginDialog();

            var openSettings = new Command {MenuText = "Nastavení", ToolBarText = "Nastavení"};
            openSettings.Executed += (sender, e) => MainFormController.ShowSettingsDialog();

            var quitCommand = new Command
                {MenuText = "Ukončit", Shortcut = Application.Instance.CommonModifier | Keys.Q};
            quitCommand.Executed += (sender, e) => MainFormController.Quit();

            var aboutCommand = new Command {MenuText = "O aplikaci"};
            aboutCommand.Executed += (sender, e) => MainFormController.ShowAboutDialog();

            // Nastavení prvků ve standardním toolbaru (hlavička)
            Menu = new MenuBar
            {
                Items =
                {
                    // Podmenu "soubor"
                    new ButtonMenuItem {Text = "&File", Items = {updateCredentials, openSettings}}
                },
                QuitItem = quitCommand,
                AboutItem = aboutCommand
            };

            // Nastavení prvků v toolbaru uvnitř okna
            ToolBar = new ToolBar {Items = {loadTimetable}};
        }
    }
}