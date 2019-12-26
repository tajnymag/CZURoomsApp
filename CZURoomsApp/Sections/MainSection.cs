using System;
using System.Threading.Tasks;
using CZURoomsApp.Exceptions;
using CZURoomsApp.Services;
using Eto.Forms;

namespace CZURoomsApp.Sections
{
    /// <summary>
    /// Obsah hlavního okna obsahující nejdůležitější
    /// </summary>
    public class MainSection : Panel
    {
        private GridView _eventsTable { get; }
        private DateTimePicker _whenDatePicker { get; }
        private ProgressBar _progressBar { get; }

        public MainSection()
        {
            // vytvoření instancí prvků
            _eventsTable = new GridView();
            _whenDatePicker = new DateTimePicker {Mode = DateTimePickerMode.Date, Value = DateTime.Today};
            _progressBar = new ProgressBar();

            // nastavení sloupce s názvem místnosti
            var tableRoomColumn = new GridColumn
            {
                HeaderText = "Místnost",
                DataCell = new TextBoxCell
                {
                    // napojení hodnoty buňky na správnou hodnotu z objektu volných intervalů
                    Binding = Binding.Property<MainController.FreeIntervalItem, string>(item =>
                        item.Room.Name.ToString())
                }
            };
            // nastavení sloupce se začátkem intervalu, kdy je místnost volná
            var tableFromColumn = new GridColumn
            {
                HeaderText = "Od",
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<MainController.FreeIntervalItem, string>(item =>
                        item.From.ToString("HH:mm"))
                }
            };
            // nastavení sloupce s koncem intervalu, kdy je místnost volná
            var tableToColumn = new GridColumn
            {
                HeaderText = "Do",
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<MainController.FreeIntervalItem, string>(item =>
                        item.To.ToString("HH:mm"))
                }
            };
            // zaregistrování výše vytvořených sloupců do gridview
            _eventsTable.Columns.Add(tableRoomColumn);
            _eventsTable.Columns.Add(tableFromColumn);
            _eventsTable.Columns.Add(tableToColumn);

            // finální sestavení a přiřazení rozložení do obsahu panelu
            Content = new StackLayout
            {
                Padding = 10,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = {_whenDatePicker, new StackLayoutItem(_progressBar, true)}
                    },
                    new StackLayoutItem(_eventsTable, true)
                }
            };
        }

        /// <summary>
        /// Načtení seznamu volných místností a vložení jej do tabulky/gridview v tomto panelu
        /// </summary>
        public async void LoadEvents()
        {
            // indikace načítání
            _progressBar.Indeterminate = true;
            // získání aktuálně vybrané hodnoty data
            var when = _whenDatePicker.Value.GetValueOrDefault();

            try
            {
                // spouštíme načítání v odděleném Tasku, abychom nezablokovali UI
                var items = await Task.Run(async () => await MainController.GetEvents(when));

                // vložení získaného seznamu do tabulky/gridview
                _eventsTable.DataStore = items;
            }
            catch (NoEventsFoundException e)
            {
                MessageBox.Show(Application.Instance.MainForm,
                    "V daném rozsahu nebyla nalezena žádná událost v rozvrhu!");
            }
            catch (LoginErrorException e)
            {
                MessageBox.Show(Application.Instance.MainForm,
                    "Nepodařilo se přihlásit. Zkontrolujte své přihlašovací údaje a připojení k internetu!");
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Instance.MainForm, "Nepodařilo se naparsovat html z UIS!");
            }
            finally
            {
                // neutralizace progressbaru
                _progressBar.Indeterminate = false;
                _progressBar.Value = 0;
            }
        }
    }
}