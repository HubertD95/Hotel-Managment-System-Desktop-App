using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for UzytkownicyWindow.xaml
    /// </summary>
    public partial class UzytkownicyWindow : Window
    {
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";
        public ObservableCollection<Uzytkownik> uzytkownicy { get; set; }
        private bool przelacznik = false;


        public UzytkownicyWindow()
        {
            InitializeComponent();
            uzytkownicy = new ObservableCollection<Uzytkownik>();

            DataContext = this;

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from uzytkownicy"; //Wypis z bazy
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }

            //INSERT I UPDATE
            //command.ExecuteNonQuery();

            // WYŚWIETLANIE CAŁEJ LISTY
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                uzytkownicy.Add(new Uzytkownik
                    (Convert.ToInt32(reader["id"]),
                    reader["Login"].ToString(),
                    reader["Haslo"].ToString(),
                    Convert.ToInt32(reader["Typ"])
                    ));
            }

            conn.Close();
        }


        private void Btn_DodajUzytkownika_Click(object sender, RoutedEventArgs e)
        {
            DodajUzytkownikaWindow window = new DodajUzytkownikaWindow();
            window.ShowDialog();
            uzytkownicy.Clear();

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from uzytkownicy"; //Wypis z bazy
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }

            
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                uzytkownicy.Add(new Uzytkownik
                    (Convert.ToInt32(reader["id"]),
                    reader["Login"].ToString(),
                    reader["Haslo"].ToString(),
                    Convert.ToInt32(reader["Typ"])
                    ));
            }

            conn.Close();

            ListViewUzytkownicy.Items.Refresh();

            TB_Login.IsEnabled = false;
            CB_Typ.IsEnabled = false;
            Btn_Zapisz.IsEnabled = false;
            Btn_usun_uzytkownika.IsEnabled = false;
            Btn_ZmienHaslo.IsEnabled = false;
        }

        private void Btn_usun_uzytkownika_Click(object sender, RoutedEventArgs e)
        {
            int indeks = uzytkownicy.IndexOf(ListViewUzytkownicy.SelectedItem as Uzytkownik);

            if (indeks != -1)
            {

                MySqlConnection conn = new MySqlConnection(connString);
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "DELETE FROM uzytkownicy WHERE id='" + (ListViewUzytkownicy.SelectedItem as Uzytkownik).Id + "'";

                var message = Application.Current.Properties["Login"] + " usuną użytkownika: " + (ListViewUzytkownicy.SelectedItem as Uzytkownik).Login;
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

                uzytkownicy.RemoveAt(indeks);
                ListViewUzytkownicy.Items.Refresh();
            }

            TB_Login.IsEnabled = false;
            CB_Typ.IsEnabled = false;
            Btn_Zapisz.IsEnabled = false;
            Btn_usun_uzytkownika.IsEnabled = false;
            Btn_ZmienHaslo.IsEnabled = false;
        }

        private void Btn_ZmienHaslo_Click(object sender, RoutedEventArgs e)
        {
            UzytkownicyZmienHasloWindow window = new UzytkownicyZmienHasloWindow((ListViewUzytkownicy.SelectedItem as Uzytkownik).Id, (ListViewUzytkownicy.SelectedItem as Uzytkownik).Login);
            window.Show();

            TB_Login.IsEnabled = false;
            CB_Typ.IsEnabled = false;
            Btn_Zapisz.IsEnabled = false;
            Btn_usun_uzytkownika.IsEnabled = false;
            Btn_ZmienHaslo.IsEnabled = false;
        }

        private void Btn_Zapisz_Click(object sender, RoutedEventArgs e)
        {
            int typ = 1;

            if (CB_Typ.SelectedIndex == 0) { typ = 0; }
            else if (CB_Typ.SelectedIndex == 1) { typ = 1; }
            else if (CB_Typ.SelectedIndex == 2) { typ = 2; }
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Update Uzytkownicy SET Login='" + TB_Login.Text.ToString() + "', Typ='" + typ + "' WHERE id='" + (ListViewUzytkownicy.SelectedItem as Uzytkownik).Id + "'";
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

            foreach (Uzytkownik u in uzytkownicy)
            {
                if (u.Id == (ListViewUzytkownicy.SelectedItem as Uzytkownik).Id)
                {
                    u.Login = TB_Login.Text.ToString();
                    if (typ == 0) { u.Typ = "Administrator"; }
                    else if (typ == 1) { u.Typ = "User"; }
                    else if (typ == 2) { u.Typ = "Serwis"; }
                    else { u.Typ = "User"; }
                }
            }
            ListViewUzytkownicy.Items.Refresh();


            TB_Login.IsEnabled = false;
            CB_Typ.IsEnabled = false;
            Btn_Zapisz.IsEnabled = false;
            Btn_usun_uzytkownika.IsEnabled = false;
            Btn_ZmienHaslo.IsEnabled = false;
        }

        private void SelectionChanged_ListViewUzytkownicy(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewUzytkownicy.SelectedItem != null)
            {
                TB_Login.Text = (ListViewUzytkownicy.SelectedItem as Uzytkownik).Login;
                if ((ListViewUzytkownicy.SelectedItem as Uzytkownik).Typ.ToString() == "Administrator") { CB_Typ.SelectedIndex = 0; }
                else if ((ListViewUzytkownicy.SelectedItem as Uzytkownik).Typ.ToString() == "User") { CB_Typ.SelectedIndex = 1; }
                else if ((ListViewUzytkownicy.SelectedItem as Uzytkownik).Typ.ToString() == "Serwis") { CB_Typ.SelectedIndex = 2; }
            }
            if (ListViewUzytkownicy.SelectedItem != null)
            {
                if ((ListViewUzytkownicy.SelectedItem as Uzytkownik).Id != 1)
                {
                    TB_Login.IsEnabled = true;
                    CB_Typ.IsEnabled = true;
                    Btn_Zapisz.IsEnabled = true;
                    Btn_usun_uzytkownika.IsEnabled = true;
                    Btn_ZmienHaslo.IsEnabled = true;
                }
                else
                {
                    TB_Login.IsEnabled = false;
                    CB_Typ.IsEnabled = false;
                    Btn_Zapisz.IsEnabled = false;
                    Btn_usun_uzytkownika.IsEnabled = false;
                    Btn_ZmienHaslo.IsEnabled = false;
                }
            }
        }

        private void GridViewColumn_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader naglowek = sender as GridViewColumnHeader;
            string nazwaKolumny = naglowek.Content.ToString();

            CollectionView colView = CollectionViewSource.GetDefaultView(ListViewUzytkownicy.ItemsSource) as CollectionView;

            if (colView != null)
            {
                colView.SortDescriptions.Clear();
                colView.SortDescriptions.Add(new SortDescription(nazwaKolumny, przelacznik ? ListSortDirection.Descending : ListSortDirection.Ascending));
                przelacznik = !przelacznik;
            }
        }

        public class Uzytkownik
        {
            public int Id { get; set; }
            public string Login { get; set; }
            public string Haslo { get; set; }
            public string Typ { get; set; }

            public Uzytkownik() { }

            public Uzytkownik(int id, string login, string haslo, int typ)
            {
                this.Id = id;
                this.Login = login;
                this.Haslo = haslo;

                if (typ == 0) { this.Typ = "Administrator"; }
                else if (typ == 1) { this.Typ = "User"; }
                else if (typ == 2) { this.Typ = "Serwis"; }
                else { this.Typ = "User"; }
            }
        }
    }
}
