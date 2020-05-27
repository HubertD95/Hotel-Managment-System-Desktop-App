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
    /// Interaction logic for UzytkownicyZmienHasloWindow.xaml
    /// </summary>
    public partial class UzytkownicyZmienHasloWindow : Window
    {
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";

        int Id;
        string Login;
        public UzytkownicyZmienHasloWindow(int id, string login)
        {
            InitializeComponent();
            this.Id = id;
            this.Login = login;
            Btn_ZmienHaslo.IsEnabled = false;

        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            if(PB_NHaslo.Password.Length != 0 && PB_PNHaslo.Password.Length != 0)
            {
                Btn_ZmienHaslo.IsEnabled = true;
            }
        }

        private void Btn_anuluj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_ZmienHaslo_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();

            if (PB_NHaslo.Password != PB_PNHaslo.Password)
            {
                MessageBox.Show("Wprowadzone hasła nie są takie same", "Błąd");
                PB_PNHaslo.Password = "";
            }
            
            else
            {
                var message = Application.Current.Properties["Login"] + " zmienił hasło użytkownika: " + Login;
                Logi l = new Logi(message);

                command.CommandText = "Update uzytkownicy SET Haslo='" + PB_NHaslo.Password.ToString() + "' WHERE id='" + Id + "'";

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
        }
    }
}
