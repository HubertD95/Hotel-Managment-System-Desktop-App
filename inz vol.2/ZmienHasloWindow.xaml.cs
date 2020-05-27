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
    /// Interaction logic for ZmienHasloWindow.xaml
    /// </summary>
    public partial class ZmienHasloWindow : Window
    {
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";

        public ZmienHasloWindow()
        {
            InitializeComponent();
            Btn_ZmienHaslo.IsEnabled = false;
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
            else if (PB_SHaslo.Password.ToString() != Application.Current.Properties["Haslo"].ToString())
            {
                MessageBox.Show("Stare hasło wprowadzone błędnie", "Błąd");
                PB_SHaslo.Password = "";
            }
            else
            {
                command.CommandText = "Update uzytkownicy SET Haslo='" + PB_NHaslo.Password.ToString() + "' WHERE Login='" + Application.Current.Properties["Login"].ToString() + "'";

                var message = Application.Current.Properties["Login"] + " zmienił swoje hasło";
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
        }

        private void Btn_Anuluj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            if (PB_NHaslo.Password.Length != 0 && PB_PNHaslo.Password.Length != 0 && PB_SHaslo.Password.Length != 0)
            {
                Btn_ZmienHaslo.IsEnabled = true;
            }
        }

        
    }
}
