using System.ComponentModel;
using Eto.Forms;

namespace CZURoomsApp.Dialogs
{
    public struct LoginDialogResult
    {
        public string Username;
        public string Password;
    }
    
    public class LoginDialog: Dialog<LoginDialogResult>
    {
        private TextBox _usernameTextBox;
        private PasswordBox _passwordTextBox;
        private Button _submitButton;
        private Button _cancelButton;

        public LoginDialogResult Credentials;
        
        public LoginDialog()
        {
            var layout = new TableLayout();
            
            Credentials = new LoginDialogResult { Username = "", Password = "" };

            _usernameTextBox = new TextBox();
            _usernameTextBox.Bind(c => c.Text, Binding.Delegate(() => Credentials.Username, (string val) => Credentials.Username = val));
            
            _passwordTextBox = new PasswordBox();
            _passwordTextBox.Bind(c => c.Text, Binding.Delegate(() => Credentials.Password, (string val) => Credentials.Password = val));
            
            _submitButton = new Button { Text = "OK" };
            _submitButton.Click += (sender, args) => Close(Credentials);
            
            _cancelButton = new Button { Text = "Storno" };
            _cancelButton.Click += (sender, args) => Close();

            layout.Rows.Add(new Label { Text = "Uživatelské jméno:" });
            layout.Rows.Add(_usernameTextBox);
            
            layout.Rows.Add(new Label { Text = "Heslo:" });
            layout.Rows.Add(_passwordTextBox);

            Content = layout;
            Padding = 10;

            DefaultButton = _submitButton;
            AbortButton = _cancelButton;
            
            PositiveButtons.Add(_submitButton);
            NegativeButtons.Add(_cancelButton);
        }
    }
}