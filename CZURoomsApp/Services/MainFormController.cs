using System;
using CZURoomsApp.Data;
using CZURoomsApp.Dialogs;
using CZURoomsApp.Exceptions;
using Eto.Forms;
using AboutDialog = CZURoomsApp.Dialogs.AboutDialog;

namespace CZURoomsApp.Services
{
    /// <summary>
    ///     Controller hlavního okna.
    ///     Obsahuje metody obstarávající logické části nesouvisející přímo s grafickou vrstvou
    /// </summary>
    public static class MainFormController
    {
        /// <summary>
        ///     Zavolá načtení uložených hodnot do storu a případně uživatele informuje o chybě s tím spojené
        /// </summary>
        public static void SetupStore()
        {
            try
            {
                Store.LoadFromDisk();
            }
            catch (ParseErrorException e)
            {
                MessageBox.Show(Application.Instance.MainForm,
                    "Cache seznamu místností je poškozena a bude načtena znovu.");
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Instance.MainForm, "Nastala neočekávaná chyba při načítání storu z disku.");
            }
        }

        /// <summary>
        ///     Ukončí okamžitě celý proces aplikace
        /// </summary>
        /// <param name="statusCode">
        ///     Nepovinný parametr určující návratový kód programu
        /// </param>
        public static void Quit(int statusCode = 0)
        {
            Application.Instance.Quit();
            Environment.Exit(statusCode);
        }

        /// <summary>
        ///     Zobrazí přihlašovací okno a uloží vložené údaje do storu
        /// </summary>
        /// <param name="required">
        ///     Nepovinný parametr určující, zda od uživatele vyžadujeme zadání údajů
        /// </param>
        public static void ShowLoginDialog(bool required = false)
        {
            try
            {
                // vytvoření instance přihlašovacího okna
                var dialog = new LoginDialog(Store.Uis.GetUsername(), Store.Uis.GetPassword());

                // zobrazení okna
                var result = dialog.ShowModal();

                // ukončení programu při stisknutí storno
                if (required && result.Canceled)
                {
                    Quit();
                }

                if (result.Canceled)
                {
                    if (required)
                    {
                        Quit();
                    }
                }
                else
                {
                    Store.Uis.UpdateCredentials(result.Username, result.Password);
                }
            }
            catch (NoCredentialsGivenException e)
            {
                // zobrazení znovu přihlašovacího okna, pokud uživatel nezadaná své údaje
                MessageBox.Show("Zadejte, prosím, své přihlašovací údaje do UIS!");
                ShowLoginDialog(required);
            }
        }

        /// <summary>
        ///     Zobrazí okno s informacemi o programu
        /// </summary>
        public static void ShowAboutDialog()
        {
            var dialog = new AboutDialog();
            dialog.ShowDialog(Application.Instance.MainForm);
        }

        /// <summary>
        ///     Zobraí okno s nastavením
        /// </summary>
        public static void ShowSettingsDialog()
        {
            var dialog = new SettingsDialog();
            dialog.ShowModal(Application.Instance.MainForm);
        }
    }
}