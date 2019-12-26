using System;
using CZURoomsApp.Exceptions;
using Eto.Forms;

namespace CZURoomsApp.Dialogs
{
    /// <summary>
    /// Struktura sloužící ke komunikaci zadaných dat rodiči
    /// </summary>
    public struct LoginDialogResult
    {
        public string Username;
        public string Password;
        public bool Canceled;
    }

    /// <summary>
    /// Okno sloužící k získání přihlašovacích údajů od uživatele
    /// </summary>
    public class LoginDialog : Dialog<LoginDialogResult>
    {
        private TextBox _usernameTextBox;
        private PasswordBox _passwordTextBox;

        private Button _submitButton;
        private Button _cancelButton;

        public LoginDialog(string username = "", string password = "")
        {
            // vytvoření základního kontejneru
            var layout = new TableLayout();

            // nastavení pole uživatelského jména
            _usernameTextBox = new TextBox();
            _usernameTextBox.Text = username;

            // nastavení pole hesla
            _passwordTextBox = new PasswordBox();
            _passwordTextBox.Text = password;

            // vytvoření tlačítka "OK"
            _submitButton = new Button {Text = "OK"};
            // uzavření okna a odeslání zadaných údajů rodiči
            _submitButton.Click += (sender, args) =>
            {
                if (String.IsNullOrWhiteSpace(_usernameTextBox.Text) ||
                    String.IsNullOrWhiteSpace(_passwordTextBox.Text))
                {
                    Close();
                    throw new NoCredentialsGivenException();
                }

                var result = GetCurrentValues();
                result.Canceled = false;

                Close(result);
            };

            // vytvoření tlačítka "Storno"
            _cancelButton = new Button {Text = "Storno"};
            // uzavření okna a odeslání informace o stornu rodiči
            _cancelButton.Click += (sender, args) =>
            {
                var result = GetCurrentValues();
                result.Canceled = true;

                Close(result);
            };

            // vložení prvků do hlavního kontejneru
            layout.Rows.Add(new Label {Text = "Uživatelské jméno:"});
            layout.Rows.Add(_usernameTextBox);

            layout.Rows.Add(new Label {Text = "Heslo:"});
            layout.Rows.Add(_passwordTextBox);

            Title = "Přihlášení do UIS";
            Content = layout;
            Padding = 10;

            DefaultButton = _submitButton;
            AbortButton = _cancelButton;

            PositiveButtons.Add(_submitButton);
            NegativeButtons.Add(_cancelButton);
        }

        /// <summary>
        /// Vrátí strukturu s aktuálně zadanými daty
        /// </summary>
        /// <returns>
        /// Struktura se zadanými daty
        /// </returns>
        private LoginDialogResult GetCurrentValues()
        {
            var result = new LoginDialogResult();
            result.Username = _usernameTextBox.Text;
            result.Password = _passwordTextBox.Text;

            return result;
        }
    }
}