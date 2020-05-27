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
    /// Interaction logic for DodajPokojWindow.xaml
    /// </summary>
    public partial class DodajPokojWindow : Window
    {

        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";

        public DodajPokojWindow()
        {
            InitializeComponent();
            Btn_dodaj.IsEnabled = false;
        }

        private void Btn_dodaj_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Insert into Pokoje (Nr_pok, Czy_zajety, Do_sprzatania, Opis, Typ, Cena) " +
                                  "values ( '" + Convert.ToInt32(TB_nrpokoju.Text) + "', '0', '0', '" + TB_opispokoju.Text.ToString() + "', '" + TB_typpokoju.Text.ToString() + "', '" + Convert.ToInt32(TB_cenapokoju.Text) + "' )";

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

            var message = Application.Current.Properties["Login"] + " dodał pokój nr: " + TB_nrpokoju.Text.ToString();
            Logi l = new Logi(message);

            this.Close();

        }

        private void Btn_anuluj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            if(TB_nrpokoju.Text.Length != 0 && TB_typpokoju.Text.Length != 0 && TB_cenapokoju.Text.Length != 0)
            {
                Btn_dodaj.IsEnabled = true;
            }
        }
    }
}
