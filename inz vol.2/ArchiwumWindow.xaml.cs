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
    /// Interaction logic for ArchiwumWindow.xaml
    /// </summary>
    public partial class ArchiwumWindow : Window
    {
        private bool przelacznik = false;
        int id = -1;
        public ObservableCollection<Rezerwacja> rezerwacjee { get; set; }
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";
        public ArchiwumWindow()
        {
            //ListViewRezerwacje.Items.Clear();
            InitializeComponent();
            rezerwacjee = new ObservableCollection<Rezerwacja>();
            DataContext = this;

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();

            command.CommandText = "Select * from archiwum"; //Wypis z bazy
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
                rezerwacjee.Add(new Rezerwacja
                    (Convert.ToInt32(reader["id"]),
                    reader["Nazwisko"].ToString(),
                    reader["Imie"].ToString(),
                    Convert.ToInt32(reader["Nr_pokoju"]),
                    reader["Data_poczatek"].ToString(),
                    reader["Data_koniec"].ToString(),
                    Convert.ToInt32(reader["Cena"])
                    ));
            }
            conn.Close();

            command.CommandText = "Select * from danefaktura"; //Wypis z bazy
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }
            
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = Convert.ToInt32(reader["id"]);
                TB_Nazwa.Text = reader["Nazwa"].ToString();
                TB_Adres.Text = reader["Adres"].ToString();
                TB_Poczta.Text = reader["Poczta"].ToString();
                TB_Nip.Text = reader["NIP"].ToString();
            }
            conn.Close();

            //ListViewRezerwacje.ItemsSource = null;
            ListViewRezerwacjee.ItemsSource = rezerwacjee;

            if (Convert.ToInt32(Application.Current.Properties["Typ"]) != 0)
            {
                TB_Adres.IsEnabled = false;
                TB_Nazwa.IsEnabled = false;
                TB_Nip.IsEnabled = false;
                TB_Poczta.IsEnabled = false;
                Btn_zapisz.Visibility = Visibility.Hidden;
            }
        }

        private void MenuBar_Rezerwacje_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_generuj_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Btn_zapisz_Click(object sender, RoutedEventArgs e)
        {
            bool flaga = true;
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();

            if (id != -1)
            {
                command.CommandText = "Update danefaktura SET Nazwa='" + TB_Nazwa.Text.ToString() + "', Adres='" + TB_Adres.Text.ToString() + "', Poczta='" + TB_Poczta.Text.ToString() + "', NIP='" + Convert.ToInt64(TB_Nip.Text) + "' WHERE id='" + id + "'";
            }
            else
            {
                command.CommandText = "Insert into danefaktura(Nazwa, Adres, Poczta, NIP) " +
                                  "values ( '" + TB_Nazwa.Text.ToString() + "', '" + TB_Adres.Text.ToString() + "', '" + TB_Poczta.Text.ToString() + "', '" + Convert.ToInt64(TB_Nip.Text) + "' )";
            }
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                flaga = false;
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }

            var message = Application.Current.Properties["Login"] + " zmienił nasze dane do faktury";
            Logi l = new Logi(message);

            if (flaga) { MessageBox.Show("Dane zostały zapisane w bazie danych", "Zapisano"); }
            
            command.ExecuteNonQuery();
            conn.Close();
        }
    

        private void SelectionChanged_ListView(object sender, SelectionChangedEventArgs e)
        {
            TB_nazwa_nabywca.IsEnabled = true;
            TB_Adres_nabywca.IsEnabled = true;
            TB_Poczta_nabywca.IsEnabled = true;
            TB_Nip_nabywca.IsEnabled = true;

            TB_Nazwa_odbiorca.IsEnabled = true;
            TB_Adres_odbiorca.IsEnabled = true;
            TB_Poczta_odbiorca.IsEnabled = true;
            TB_Nip_odbiorca.IsEnabled = true;
            
        }

        private void GridViewColumn_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader naglowek = sender as GridViewColumnHeader;
            string nazwaKolumny = naglowek.Content.ToString();

            CollectionView colView = CollectionViewSource.GetDefaultView(ListViewRezerwacjee.ItemsSource) as CollectionView;

            if (colView != null)
            {
                colView.SortDescriptions.Clear();
                colView.SortDescriptions.Add(new SortDescription(nazwaKolumny, przelacznik ? ListSortDirection.Descending : ListSortDirection.Ascending));
                przelacznik = !przelacznik;
            }
        }

        public class Rezerwacja
        {
            public int Id { get; set; }
            public string Nazwisko { get; set; }
            public string Imie { get; set; }
            public int Nr_Pok { get; set; }
            public string Data_poczatek { get; set; }
            public string Data_koniec { get; set; }
            public string Ilosc_Dni { get; set; }
            public int Cena { get; set; }

            public Rezerwacja() { }

            public Rezerwacja(int id, string nazwisko, string imie, int nrPok, string data_poczatek, string data_koniec, int cena)
            {
                this.Id = id;
                this.Nazwisko = nazwisko;
                this.Imie = imie;
                this.Nr_Pok = nrPok;
                this.Cena = cena;
                this.Data_poczatek = data_poczatek;
                this.Data_koniec = data_koniec;

                TimeSpan ts = DateTime.Parse(data_koniec) - DateTime.Parse(data_poczatek);
                this.Ilosc_Dni = ts.TotalDays.ToString();

            }
        }
    }
}
