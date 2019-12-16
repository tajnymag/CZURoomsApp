using System;
using CZURoomsApp.Data;
using Eto.Forms;

namespace CZURoomsApp.Dialogs
{
    /// <summary>
    /// Okno sloužící k zobrazení a editaci uživatelského nastavení
    /// </summary>
    public class SettingsDialog : Dialog
    {
        private DateTimePicker _dayBeginPicker;
        private DateTimePicker _dayEndPicker;
        private CheckBox _includeBreaksBox;

        private Button _saveButton;
        private Button _cancelButton;

        public SettingsDialog()
        {
            // vytvoření základního kontejneru
            var layout = new TableLayout();

            // nastavení výběru počatečního času
            _dayBeginPicker = new DateTimePicker {Mode = DateTimePickerMode.Time};
            _dayBeginPicker.Value = Store.Settings.DayBeginTime;

            // nastavení výběru koncového času
            _dayEndPicker = new DateTimePicker {Mode = DateTimePickerMode.Time};
            _dayEndPicker.Value = Store.Settings.DayEndTime;

            // nastavení výběru zahrnutí přestávek
            _includeBreaksBox = new CheckBox();
            _includeBreaksBox.Checked = Store.Settings.IncludeBreaks;

            // vytvoření tlačíka "Uložit"
            _saveButton = new Button {Text = "Uložit"};
            // uložení zadaného nastavení do storu a souboru, uzavření okna
            _saveButton.Click += (sender, args) =>
            {
                Store.Settings.DayBeginTime = _dayBeginPicker.Value ?? Store.Settings.DayBeginTime;
                Store.Settings.DayEndTime = _dayEndPicker.Value ?? Store.Settings.DayEndTime;
                Store.Settings.IncludeBreaks = _includeBreaksBox.Checked ?? Store.Settings.IncludeBreaks;
                Store.Settings.SaveToDisk();
                Close();
            };

            // nastavení tlačítka "Storno"
            _cancelButton = new Button {Text = "Storno"};
            _cancelButton.Click += (sender, args) => { Close(); };

            // vložení prvků do hlavního kontejneru
            layout.Rows.Add(new Label {Text = "Školní den začíná:"});
            layout.Rows.Add(_dayBeginPicker);

            layout.Rows.Add(new Label {Text = "Školní den končí:"});
            layout.Rows.Add(_dayEndPicker);

            layout.Rows.Add(new Label {Text = "Zobrazovat i přestávky:"});
            layout.Rows.Add(_includeBreaksBox);

            Title = "Nastavení";
            Content = layout;
            Padding = 10;

            DefaultButton = _saveButton;
            AbortButton = _cancelButton;

            PositiveButtons.Add(_saveButton);
            NegativeButtons.Add(_cancelButton);
        }
    }
}