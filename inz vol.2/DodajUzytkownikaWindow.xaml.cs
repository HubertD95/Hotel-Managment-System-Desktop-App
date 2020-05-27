using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace inz_vol._2
{
    /// <summary>
    /// Interaction logic for DodajUzytkownikaWindow.xaml
    /// </summary>
    public partial class DodajUzytkownikaWindow : Window
    {
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";

        public DodajUzytkownikaWindow()
        {
            InitializeComponent();
            Btn_Dodaj.IsEnabled = false;
        }

        private void Btn_Anuluj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Dodaj_Click(object sender, RoutedEventArgs e)
        {
            int typ = 1;
            if (CB_Typ.SelectedIndex == 0) { typ = 0; }
            else if(CB_Typ.SelectedIndex == 1) { typ = 1; }
            else if (CB_Typ.SelectedIndex == 2) { typ = 2; }
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Insert into Uzytkownicy (Login, Haslo, Typ) " +
                                  "values ( '" + TB_Login.Text.ToString() + "', '" + PB_Haslo.Password.ToString() + "', '" + typ + "' )";

            var message = Application.Current.Properties["Login"] + " dodał użytkownika: " + TB_Login.Text.ToString();
            Logi l = new Logi(message);

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }
            command.ExecuteNonQuery();
            conn.Close();

            this.Close();
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            if(TB_Login.Text.Length != 0 && PB_Haslo.Password.Length != 0 && CB_Typ.SelectedIndex != -1)
            {
                Btn_Dodaj.IsEnabled = true;
            }
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            if (TB_Login.Text.Length != 0 && PB_Haslo.Password.Length != 0 && CB_Typ.SelectedIndex != -1)
            {
                Btn_Dodaj.IsEnabled = true;
            }
        }

        private void SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (TB_Login.Text.Length != 0 && PB_Haslo.Password.Length != 0 && CB_Typ.SelectedIndex != -1)
            {
                Btn_Dodaj.IsEnabled = true;
            }
        }
    }
}
