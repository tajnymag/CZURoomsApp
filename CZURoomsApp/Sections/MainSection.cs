using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CZURoomsApp.Models;
using CZURoomsApp.Services;
using Eto.Forms;

namespace CZURoomsApp.Sections
{
    public class MainSection: Panel
    {
        private ListBox _eventsBox { get; set; }
        private DateTimePicker _fromDatePicker { get; set; }
        private DateTimePicker _toDatePicker { get; set; }
        
        public MainSection(Form mainForm)
        {
            _eventsBox = new ListBox();

            _fromDatePicker = new DateTimePicker {Mode = DateTimePickerMode.Date, Value = DateTime.Today};
            _toDatePicker = new DateTimePicker {Mode = DateTimePickerMode.Date, Value = DateTime.Today.AddDays(7)};
            
            Content = new StackLayout
            {
                Padding = 10,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = {_fromDatePicker, _toDatePicker}
                    },
                    new StackLayoutItem(_eventsBox, expand: true)
                }
            };
        }

        public async void LoadEvents()
        {
            var from = _fromDatePicker.Value.GetValueOrDefault();
            var to = _toDatePicker.Value.GetValueOrDefault();

            try
            {
                await Shared.Uis.Login();

                List<TimetableEvent> timetableEvents = null;
                List<ClassRoom> classRooms = null;

                
                    var html = Shared.Uis.GetRoomPage(new ClassRoom(0, "-- všechny místnosti --"), from, to, DayOfWeek.Wednesday).Result;
                    var parser = new CZUParser(html);
                    timetableEvents = parser.GetTimetableEvents(parser.GetAllRows());
                

                
                    var html2 = Shared.Uis.GetEnumsPage().Result;
                    var parser2 = new CZUParser(html);
                    classRooms = parser.GetClassRooms();
                

                foreach (var classRoom in classRooms)
                {
                    Shared.AddClassRoom(classRoom.Name, classRoom);
                }

                _eventsBox.Items.Clear();
                foreach (var timetableEvent in timetableEvents)
                {
                    Shared.AddEventToClassRoom(timetableEvent.Room.Name, timetableEvent);
                }

                foreach (var classRoom in classRooms)
                {
                    _eventsBox.Items.Add(new ListItem {Text = classRoom.GetFreeIntervals(from, to).ToString() });
                }
            }
            catch (NoEventsFoundException e)
            {
                MessageBox.Show(Application.Instance.MainForm,
                    "V daném rozsahu nebyla nalezena žádná událost v rozvrhu!");
                return;
            }
            catch (LoginErrorException e)
            {
                MessageBox.Show(Application.Instance.MainForm,
                    "Nepodařilo se přihlásit. Zkontrolujte své přihlašovací údaje!");
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Instance.MainForm, "Nepodařilo se naparsovat html z UIS!");
                MessageBox.Show(Application.Instance.MainForm, e.Message);
                return;
            }
        }
    }
}