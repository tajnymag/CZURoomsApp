using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CZURoomsApp.Models;
using CZURoomsApp.Services;
using Eto.Forms;

namespace CZURoomsApp.Sections
{
    public class MainSection: Panel
    {
        private ListBox _eventsBox { get; set; }
        private DateTimePicker _whenDatePicker { get; set; }

        public MainSection(Form mainForm)
        {
            _eventsBox = new ListBox();

            _whenDatePicker = new DateTimePicker {Mode = DateTimePickerMode.Date, Value = DateTime.Today};
            
            Content = new StackLayout
            {
                Padding = 10,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = {_whenDatePicker}
                    },
                    new StackLayoutItem(new ProgressBar()),
                    new StackLayoutItem(_eventsBox, expand: true)
                }
            };
        }

        public async void LoadEvents()
        {
            var when = _whenDatePicker.Value.GetValueOrDefault();
            var whenInterval = TimeHelpers.GetDayInterval(when.DayOfWeek, when, when.AddHours(23));
            
            Shared.ResetClassRooms();

            try
            {
                await Shared.Uis.Login();

                List<TimetableEvent> timetableEvents = null;
                List<ClassRoom> classRooms = null;
                
                var html2 = await Shared.Uis.GetEnumsPage();
                var parser2 = new CZUParser(html2);
                classRooms = parser2.GetClassRooms();
                

                foreach (var classRoom in classRooms)
                {
                    Shared.AddClassRoom(classRoom.Name, classRoom);
                }
                
                var html = await Shared.Uis.GetRoomPage(new ClassRoom(0, "-- všechny místnosti --"), whenInterval.From, whenInterval.To, when.DayOfWeek);
                var parser = new CZUParser(html);
                timetableEvents = parser.GetTimetableEvents(parser.GetAllRows());
                
                _eventsBox.Items.Clear();
                foreach (var timetableEvent in timetableEvents)
                {
                    Shared.AddEventToClassRoom(timetableEvent.Room.Name, timetableEvent);
                }

                foreach (var classRoom in classRooms)
                {
                    if (classRoom.Id == 0)
                    {
                        continue;
                    }
                    
                    foreach (var freeInterval in classRoom.GetFreeIntervals(whenInterval.From, whenInterval.To))
                    {
                        string text = $"{classRoom.Name} je volná mezi {freeInterval.From.ToString()} a {freeInterval.To.ToString()}";
                        _eventsBox.Items.Add(new ListItem {Text = text });
                    }
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