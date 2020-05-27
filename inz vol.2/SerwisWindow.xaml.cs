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
using System.Windows.Threading;

namespace inz_vol._2
{
    /// <summary>
    /// Interaction logic for SerwisWindow.xaml
    /// </summary>
    public partial class SerwisWindow : Window
    {
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public ObservableCollection<Pokoje> pokoje { get; set; }
        private bool przelacznik = false;
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";

        public SerwisWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
            dispatcherTimer.Start();

            pokoje = new ObservableCollection<Pokoje>();

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Pokoje";

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
                if (Convert.ToInt32(reader["Do_sprzatania"]) == 1){
                    pokoje.Add(new Pokoje
                        (Convert.ToInt32(reader["id"]),
                        Convert.ToInt32(reader["Nr_pok"]),
                        Convert.ToInt32(reader["Czy_zajety"]),
                        Convert.ToInt32(reader["Do_sprzatania"])
                        ));
                }
            }

            conn.Close();
            ListViewSprzatanie.Items.Refresh();

            Btn_zapisz.IsEnabled = false;
            CB_sprzatanie.IsEnabled = false;
            ListViewSprzatanie.ItemsSource = pokoje;
        }

        private void GridViewColumn_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader naglowek = sender as GridViewColumnHeader;
            string nazwaKolumny = naglowek.Content.ToString();

            CollectionView colView = CollectionViewSource.GetDefaultView(ListViewSprzatanie.ItemsSource) as CollectionView;
            if (colView != null)
            {
                colView.SortDescriptions.Clear();
                colView.SortDescriptions.Add(new SortDescription(nazwaKolumny, przelacznik ? ListSortDirection.Descending : ListSortDirection.Ascending));
                przelacznik = !przelacznik;
            }
        }

        private void Btn_zapisz_Click(object sender, RoutedEventArgs e)
        {
            int dosprzatania;
            CB_sprzatanie.SelectedIndex = -1;
            CB_sprzatanie.IsEditable = false;
            Btn_zapisz.IsEnabled = false;

            if (CB_sprzatanie.SelectedIndex == 0) { dosprzatania = 1; }
            else { dosprzatania = 0; }

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Update Pokoje SET Do_sprzatania='" + dosprzatania + "' WHERE id='" + (ListViewSprzatanie.SelectedItem as Pokoje).Id + "'";

            var message = Application.Current.Properties["Login"] + " edytował pole Do_sprzątania w pokoju: " + (ListViewSprzatanie.SelectedItem as Pokoje).Nr_pok;
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

            foreach(Pokoje p in pokoje)
            {
                if ((ListViewSprzatanie.SelectedItem as Pokoje).Id == p.Id)
                {
                    p.Do_sprzatania = "Nie";
                }
            }

            ListViewSprzatanie.Items.Refresh();
        }

        private void SelectionChanged_ListViewSprzatanie(object sender, SelectionChangedEventArgs e)
        {
            Btn_zapisz.IsEnabled = true;
            CB_sprzatanie.IsEnabled = true;

            if((ListViewSprzatanie.SelectedItem as Pokoje).Do_sprzatania == "Nie") { CB_sprzatanie.SelectedIndex = 1; }
            else { CB_sprzatanie.SelectedIndex = 0; }
        }

        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            pokoje.Clear();

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Pokoje";

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
                if (Convert.ToInt32(reader["Do_sprzatania"]) == 1)
                {
                    pokoje.Add(new Pokoje
                        (Convert.ToInt32(reader["id"]),
                        Convert.ToInt32(reader["Nr_pokoju"]),
                        Convert.ToInt32(reader["Zajety"]),
                        Convert.ToInt32(reader["Do_sprzatania"])
                        ));
                }
            }

            conn.Close();

            Btn_zapisz.IsEnabled = false;
            CB_sprzatanie.IsEnabled = false;
        }

        public class Pokoje
        {
            public int Id { get; set; }
            public int Nr_pok { get; set; }
            public string Zajety { get; set; }
            public string Do_sprzatania { get; set; } 

            public Pokoje() { }

            public Pokoje(int id, int nr_pok, int zajety, int do_sprzatania)
            {
                this.Id = id;
                this.Nr_pok = nr_pok;

                if (zajety == 0) { this.Zajety = "Nie"; }
                else { this.Zajety = "Tak"; }

                if (do_sprzatania == 0) { this.Do_sprzatania = "Nie"; }
                else { this.Do_sprzatania = "Tak"; }
            }
        }

        private void ZmienHaslo_Click(object sender, RoutedEventArgs e)
        {
            ZmienHasloWindow zhwindow = new ZmienHasloWindow();
            zhwindow.Show();
        }

        private void Wyloguj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Properties["wyloguj"] = true;

            var message = Application.Current.Properties["Login"] + " wylogował się";
            Logi l = new Logi(message);
        }
    }
}
