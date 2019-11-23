using Eto.Forms;

namespace CZURoomsApp.Sections
{
    public class MainSection: Panel
    {
        public MainSection(Control webview)
        {
            Content = new StackLayout
            {
                Padding = 10,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = {new DateTimePicker { Mode = DateTimePickerMode.Date }, new DateTimePicker { Mode = DateTimePickerMode.Date }}
                    },
                    new StackLayoutItem(webview, expand: true)
                }
            };
        }
    }
}