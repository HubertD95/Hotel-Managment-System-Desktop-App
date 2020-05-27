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
using MySql.Data.MySqlClient;

namespace inz_vol._2
{
    /// <summary>
    /// Interaction logic for FirstWindow.xaml
    /// </summary>
    public partial class FirstWindow : Window
    {
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        DateTime datadopokoi = DateTime.Today;
        public List<int> pokoj { get; set; } = new List<int>();
        public List<Daty> daty { get; set; } = new List<Daty>();
        public ObservableCollection<Pokoje> pokoje { get; set; }
        public ObservableCollection<Rezerwacja> rezerwacje { get; set; }
        private bool przelacznik = false;
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";
        int typ = Convert.ToInt16(Application.Current.Properties["Typ"]);

        public FirstWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
            dispatcherTimer.Start();

            rezerwacje = new ObservableCollection<Rezerwacja>();
            pokoje = new ObservableCollection<Pokoje>();
            DataContext = this;

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Rezerwacje"; //Wypis z bazy
            string stringg = "string";

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
                rezerwacje.Add(new Rezerwacja
                    (Convert.ToInt32(reader["id"]),
                    reader["Nazwisko"].ToString(),
                    reader["Imie"].ToString(),
                    Convert.ToInt32(reader["Nr_pokoju"]),
                    reader["Data_poczatek"].ToString(),
                    reader["Data_koniec"].ToString(),
                    Convert.ToInt32(reader["Potwierdzone"]),
                    Convert.ToInt32(reader["Posilki"]),
                    reader["Opis"].ToString(),
                    Convert.ToInt32(reader["Oplacone"])
                    ));
            }

            conn.Close();

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }

            command.CommandText = "Select * from Pokoje"; //Wypis z bazy
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                pokoje.Add(new Pokoje
                    (Convert.ToInt32(reader["id"]),
                    Convert.ToInt32(reader["Nr_pok"]),
                    Convert.ToInt32(reader["Czy_zajety"]),
                    Convert.ToInt32(reader["Do_sprzatania"]),
                    reader["Opis"].ToString(),
                    reader["Typ"].ToString(),
                    Convert.ToInt32(reader["Cena"])
                    ));
            }
            conn.Close();

            if (Convert.ToInt16(Application.Current.Properties["Typ"]) == 1)
            {
                TypUser();
            }
            else if (Convert.ToInt16(Application.Current.Properties["Typ"]) == 2)
            {
                TypSerwis();
            }

            
            Btn_dzisiaj.Content = datadopokoi.ToString("yyyy-MM-dd");
            WypelnijListe(Convert.ToDateTime(Btn_dzisiaj.Content));

            //bool flaga = true;
            foreach (Rezerwacja x in rezerwacje)
            {
                //x.Data_koniec
                DateTime oDate = Convert.ToDateTime(x.Data_koniec);
                if (datadopokoi > oDate)
                {
                    int indeks = x.Id;

                    command.CommandText = "Insert into archiwum(id, Imie, Nazwisko, Nr_pokoju, Data_poczatek, Data_koniec, Cena) " +
                                          "values ( '" + x.Id + "', '" + x.Imie + "', '" + x.Nazwisko.ToString() + "', '" + x.Nr_Pok + "', '" + x.Data_poczatek.ToString() + "', '" + x.Data_koniec.ToString() + "', '" + x.Cena + "')";
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

                    command.CommandText = "DELETE FROM rezerwacje WHERE id='" + x.Id + "'";
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        //flaga = false;
                        MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
                    }
                    //if (flaga) { MessageBox.Show("Dane zostały przeniesione do archiwum", "Zapisano"); }

                    command.ExecuteNonQuery();
                    conn.Close();

                    
                }
            }
            object sender = new object();
            EventArgs ea = new EventArgs();
            dispatcherTimer_Tick(sender, ea);
            //ListViewRezerwacje.Items.Refresh();
        }

        public void TypUser()
        {
            Btn_Uzytkownicy.Visibility = Visibility.Hidden;
            Btn_Logi.Visibility = Visibility.Hidden;
            //Label_cena.Visibility = Visibility.Hidden;
            // Label_nrpok.Visibility = Visibility.Hidden;
            // Label_opis.Visibility = Visibility.Hidden;
            // Label_Pokoje.Visibility = Visibility.Hidden;
            // Label_typ.Visibility = Visibility.Hidden;
            Btn_dodaj_pokoj.Visibility = Visibility.Hidden;
            Btn_usun_pokoj.Visibility = Visibility.Hidden;
            // TB_nrpokojuPokoje.Visibility = Visibility.Hidden;
            // TB_Typpokoju.Visibility = Visibility.Hidden;
            //  TB_Cenapokoju.Visibility = Visibility.Hidden;
            //  TB_Pokojopis.Visibility = Visibility.Hidden;
            //Application.Current.MainWindow.Height = 294;
            //  Btn_zapis_pokoj.Margin = new Thickness(301,346,0,0);
        }

        private void TypSerwis()
        {
            ListViewPokoje.Visibility = Visibility.Hidden;
            Btn_Uzytkownicy.Visibility = Visibility.Hidden;
            Label_cena.Visibility = Visibility.Hidden;
            Label_dosprzatania.Visibility = Visibility.Hidden;
            Label_nrpok.Visibility = Visibility.Hidden;
            Label_opis.Visibility = Visibility.Hidden;
            Label_Pokoje.Visibility = Visibility.Hidden;
            Label_typ.Visibility = Visibility.Hidden;
            Label_zajety.Visibility = Visibility.Hidden;
            Btn_zapis_pokoj.Visibility = Visibility.Hidden;
            Btn_dodaj_pokoj.Visibility = Visibility.Hidden;
            Btn_usun_pokoj.Visibility = Visibility.Hidden;
            TB_nrpokojuPokoje.Visibility = Visibility.Hidden;
            TB_Typpokoju.Visibility = Visibility.Hidden;
            TB_Cenapokoju.Visibility = Visibility.Hidden;
            TB_Pokojopis.Visibility = Visibility.Hidden;
            CB_zajety.Visibility = Visibility.Hidden;
            CB_Dosprzatania.Visibility = Visibility.Hidden;
            Btn_Logi.Visibility = Visibility.Hidden;

            Application.Current.MainWindow.Height = 294;
        }

        //sortowanie listview
        private void GridViewColumn_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader naglowek = sender as GridViewColumnHeader;
            string nazwaKolumny = naglowek.Content.ToString();

            CollectionView colView = CollectionViewSource.GetDefaultView(ListViewRezerwacje.ItemsSource) as CollectionView;

            if (colView != null)
            {
                colView.SortDescriptions.Clear();
                colView.SortDescriptions.Add(new SortDescription(nazwaKolumny, przelacznik ? ListSortDirection.Descending : ListSortDirection.Ascending));
                przelacznik = !przelacznik;
            }
        }

        //dodawanie rezerwacji
        private void Btn_dodaj_rezerwacje_Click(object sender, RoutedEventArgs e)
        {
            DodajRezerwacjeWindow dodajwindow = new DodajRezerwacjeWindow();
            dodajwindow.ShowDialog();

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Rezerwacje"; //Wypis z bazy

            try
            {
                conn.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }

            rezerwacje.Clear();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rezerwacje.Add(new Rezerwacja
                    (Convert.ToInt32(reader["id"]),
                    reader["Nazwisko"].ToString(),
                    reader["Imie"].ToString(),
                    Convert.ToInt32(reader["Nr_pokoju"]),
                    reader["Data_poczatek"].ToString(),
                    reader["Data_koniec"].ToString(),
                    Convert.ToInt32(reader["Potwierdzone"]),
                    Convert.ToInt32(reader["Posilki"]),
                    reader["Opis"].ToString(),
                    Convert.ToInt32(reader["Oplacone"])
                    ));
            }

            conn.Close();

            ListViewRezerwacje.Items.Refresh();



        }

        //Usuwanie z bazy 
        private void Btn_usun_rezerwacje_Click(object sender, RoutedEventArgs e)
        {
            int indeks = rezerwacje.IndexOf(ListViewRezerwacje.SelectedItem as Rezerwacja);

            if (indeks != -1)
            {

                MySqlConnection conn = new MySqlConnection(connString);
                MySqlCommand command = conn.CreateCommand();

                command.CommandText = "DELETE FROM rezerwacje WHERE id='" + (ListViewRezerwacje.SelectedItem as Rezerwacja).Id + "'";

                var message = Application.Current.Properties["Login"] + " usuną rezerwację " + (ListViewRezerwacje.SelectedItem as Rezerwacja).Imie + " " + (ListViewRezerwacje.SelectedItem as Rezerwacja).Nazwisko + ", pokój: " + (ListViewRezerwacje.SelectedItem as Rezerwacja).Nr_Pok + ", data: " + (ListViewRezerwacje.SelectedItem as Rezerwacja).Data_poczatek + " -> " + (ListViewRezerwacje.SelectedItem as Rezerwacja).Data_koniec;
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

                rezerwacje.RemoveAt(indeks);
            }
        }

        //Wypelnienie textboxow po kliknieciu w listviewRezerwacje
        private void SelectionChanged_ListView(object sender, SelectionChangedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            int cena = 0;

            if (ListViewRezerwacje.SelectedItem != null)
            {
                TB_Imie.IsEnabled = true;
                TB_Naziwko.IsEnabled = true;
                TB_nr_pok.IsEnabled = true;
                TB_Poczatek.IsEnabled = true;
                TB_Koniec.IsEnabled = true;
                TB_Posilki_Opis.IsEnabled = true;
                CB_Potwierdzone.IsEnabled = true;
                CB_Posilki.IsEnabled = true;
                TB_Dozaplaty.IsEnabled = true;
                Btn_zapis_rezerwacje.IsEnabled = true;
                Btn_zaplac.IsEnabled = true;
                Btn_usun_rezerwacje.IsEnabled = true;
                if (typ == 0) { CB_Oplacone.IsEnabled = true; }


                TB_Imie.Text = (ListViewRezerwacje.SelectedItem as Rezerwacja).Imie;
                TB_Naziwko.Text = (ListViewRezerwacje.SelectedItem as Rezerwacja).Nazwisko;
                TB_nr_pok.Text = (ListViewRezerwacje.SelectedItem as Rezerwacja).Nr_Pok.ToString();
                TB_Poczatek.Text = (ListViewRezerwacje.SelectedItem as Rezerwacja).Data_poczatek;
                TB_Koniec.Text = (ListViewRezerwacje.SelectedItem as Rezerwacja).Data_koniec;
                TB_Posilki_Opis.Text = (ListViewRezerwacje.SelectedItem as Rezerwacja).Opis;
                if ((ListViewRezerwacje.SelectedItem as Rezerwacja).potwierdzone == "Potwierdzone") { CB_Potwierdzone.SelectedIndex = 0; }
                else { CB_Potwierdzone.SelectedIndex = 1; }
                if ((ListViewRezerwacje.SelectedItem as Rezerwacja).Oplacone == "Tak") { CB_Oplacone.SelectedIndex = 0; }
                else { CB_Oplacone.SelectedIndex = 1; }

                if ((ListViewRezerwacje.SelectedItem as Rezerwacja).Posilki == 0) { CB_Posilki.SelectedIndex = 0; }
                else if ((ListViewRezerwacje.SelectedItem as Rezerwacja).Posilki == 1) { CB_Posilki.SelectedIndex = 1; }
                else if ((ListViewRezerwacje.SelectedItem as Rezerwacja).Posilki == 2) { CB_Posilki.SelectedIndex = 2; }

                TB_nrpokojuPokoje.IsEnabled = false;
                TB_Typpokoju.IsEnabled = false;
                TB_Cenapokoju.IsEnabled = false;
                TB_Pokojopis.IsEnabled = false;
                CB_Dosprzatania.IsEnabled = false;
                CB_zajety.IsEnabled = false;
                Btn_zapis_pokoj.IsEnabled = false;
                Btn_usun_pokoj.IsEnabled = false;

                TB_nrpokojuPokoje.Text = "";
                TB_Typpokoju.Text = "";
                TB_Cenapokoju.Text = "";
                TB_Pokojopis.Text = "";
                CB_Dosprzatania.SelectedIndex = -1;
                CB_zajety.SelectedIndex = -1;


                command.CommandText = "Select Cena from pokoje Where Nr_pok = '" + (ListViewRezerwacje.SelectedItem as Rezerwacja).Nr_Pok + "' "; //Wypis z bazy

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
                    cena = Convert.ToInt32(reader["Cena"]);
                }

                conn.Close();


                TB_Dozaplaty.Text = (cena * Convert.ToInt32((ListViewRezerwacje.SelectedItem as Rezerwacja).Ilosc_Dni)).ToString();

            }
            else
            {
                TB_Imie.Text = "";
                TB_Naziwko.Text = "";
                TB_nr_pok.Text = "";
                TB_Koniec.Text = "";
                TB_Poczatek.Text = "";
                TB_Posilki_Opis.Text = "";
                CB_Potwierdzone.SelectedIndex = -1;
                TB_Dozaplaty.Text = "";
                CB_Posilki.SelectedIndex = -1;
            }


        }

        // aktualizuj rezerwacje
        private void Btn_zapisz_rezerwacje_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            int potwierdzenie, posilki, oplacone;

            if (CB_Potwierdzone.SelectedIndex == 0) { potwierdzenie = 1; }
            else { potwierdzenie = 0; }

            if (CB_Posilki.SelectedIndex == 0) { posilki = 0; }
            else if (CB_Posilki.SelectedIndex == 1) { posilki = 1; }
            else if (CB_Posilki.SelectedIndex == 2) { posilki = 2; }
            else if (CB_Posilki.SelectedIndex == 3) { posilki = 3; }
            else { posilki = 0; }
            if (CB_Oplacone.SelectedIndex == 0) { oplacone = 1; }
            else { oplacone = 0; }

            command.CommandText = "Update Rezerwacje SET Imie='" + TB_Imie.Text.ToString() + "', Nazwisko='" + TB_Naziwko.Text.ToString() + "', Nr_pokoju='" + TB_nr_pok.Text.ToString() + "', Potwierdzone='" + potwierdzenie + "', Data_poczatek='" + TB_Poczatek.Text.ToString() + "', Data_koniec='" + TB_Koniec.Text.ToString() + "', Posilki='" + posilki + "', Opis='" + TB_Posilki_Opis.Text.ToString() + "', Oplacone='" + oplacone + "' WHERE id='" + (ListViewRezerwacje.SelectedItem as Rezerwacja).Id + "'";

            var message = Application.Current.Properties["Login"] + " edytował rezerwację na nazwisko " + TB_Naziwko.Text.ToString() + " " + TB_Imie.Text.ToString() + ", pokój: " + TB_nr_pok.Text.ToString() + ", data: " + TB_Poczatek.Text.ToString() + " -> " + TB_Koniec.Text.ToString();
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

            foreach (Rezerwacja x in rezerwacje)
            {
                if (x.Id == (ListViewRezerwacje.SelectedItem as Rezerwacja).Id)
                {
                    x.Imie = TB_Imie.Text.ToString();
                    x.Nazwisko = TB_Naziwko.Text.ToString();
                    x.Nr_Pok = Convert.ToInt32(TB_nr_pok.Text);
                    x.Opis = TB_Posilki_Opis.Text.ToString();
                    if (CB_Potwierdzone.SelectedIndex == 0) { x.potwierdzone = "Potwierdzone"; }
                    else { x.potwierdzone = "Nie potwierdzone"; }

                    if (CB_Posilki.SelectedIndex == 0) { x.Posilki = 0; }
                    else if (CB_Posilki.SelectedIndex == 1) { x.Posilki = 1; }
                    else if (CB_Posilki.SelectedIndex == 2) { x.Posilki = 2; }
                    else if (CB_Posilki.SelectedIndex == 3) { x.Posilki = 3; }
                    if (CB_Oplacone.SelectedIndex == 0) { x.Oplacone = "Tak"; }
                    else { x.Oplacone = "Nie"; }
                }
            }

            TB_Imie.Text = "";
            TB_Naziwko.Text = "";
            TB_nr_pok.Text = "";
            TB_Koniec.Text = "";
            TB_Poczatek.Text = "";
            CB_Potwierdzone.SelectedIndex = -1;
            CB_Posilki.SelectedIndex = -1;
            TB_Posilki_Opis.Text = "";
            CB_Oplacone.SelectedIndex = -1;
            TB_Dozaplaty.Text = "";

            ListViewRezerwacje.Items.Refresh();

        }

        //Dodaj pokoj
        private void Btn_dodaj_pokoj_Click(object sender, RoutedEventArgs e)
        {
            DodajPokojWindow dodajpokojwindow = new DodajPokojWindow();
            dodajpokojwindow.ShowDialog();
            pokoje.Clear();

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Pokoje"; //Wypis z bazy

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
                pokoje.Add(new Pokoje
                    (Convert.ToInt32(reader["id"]),
                    Convert.ToInt32(reader["Nr_pok"]),
                    Convert.ToInt32(reader["Czy_zajety"]),
                    Convert.ToInt32(reader["Do_sprzatania"]),
                    reader["Opis"].ToString(),
                    reader["Typ"].ToString(),
                    Convert.ToInt32(reader["Cena"])
                    ));
            }

            conn.Close();

        }

        //Usun pokoj
        private void Btn_usun_pokoj_Click(object sender, RoutedEventArgs e)
        {
            int indeks = pokoje.IndexOf(ListViewPokoje.SelectedItem as Pokoje);

            if (indeks != -1)
            {

                MySqlConnection conn = new MySqlConnection(connString);
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "DELETE FROM pokoje WHERE id='" + (ListViewPokoje.SelectedItem as Pokoje).Id + "'";

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

                var message = Application.Current.Properties["Login"] + " usuną pokój: " + (ListViewPokoje.SelectedItem as Pokoje).Nr_pok.ToString();
                Logi l = new Logi(message);

                pokoje.RemoveAt(indeks);
            }
        }

        //Wypelnianie TextBoxow pokojowych
        private void SelectionChanged_ListViewPokoje(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewPokoje.SelectedItem != null)
            {
                if (Convert.ToInt16(Application.Current.Properties["Typ"]) == 1)
                {
                    CB_Dosprzatania.IsEnabled = true;
                    Btn_zapis_pokoj.IsEnabled = true;
                }
                else
                {
                    TB_nrpokojuPokoje.IsEnabled = true;
                    TB_Typpokoju.IsEnabled = true;
                    TB_Cenapokoju.IsEnabled = true;
                    TB_Pokojopis.IsEnabled = true;
                    CB_Dosprzatania.IsEnabled = true;
                    CB_zajety.IsEnabled = true;
                    Btn_zapis_pokoj.IsEnabled = true;
                    Btn_usun_pokoj.IsEnabled = true;
                }

                TB_nrpokojuPokoje.Text = (ListViewPokoje.SelectedItem as Pokoje).Nr_pok.ToString();
                TB_Typpokoju.Text = (ListViewPokoje.SelectedItem as Pokoje).Typ;
                TB_Cenapokoju.Text = (ListViewPokoje.SelectedItem as Pokoje).Cena.ToString();
                TB_Pokojopis.Text = (ListViewPokoje.SelectedItem as Pokoje).Opis;
                if ((ListViewPokoje.SelectedItem as Pokoje).Zajety == "Nie") { CB_zajety.SelectedIndex = 1; }
                else { CB_zajety.SelectedIndex = 0; }
                if ((ListViewPokoje.SelectedItem as Pokoje).Do_sprzatania == "Nie") { CB_Dosprzatania.SelectedIndex = 1; }
                else { CB_Dosprzatania.SelectedIndex = 0; }

                TB_Imie.Text = "";
                TB_Naziwko.Text = "";
                TB_nr_pok.Text = "";
                TB_Koniec.Text = "";
                TB_Poczatek.Text = "";
                TB_Posilki_Opis.Text = "";
                CB_Potwierdzone.SelectedIndex = -1;
                TB_Dozaplaty.Text = "";
                CB_Posilki.SelectedIndex = -1;

                TB_Imie.IsEnabled = false;
                TB_Naziwko.IsEnabled = false;
                TB_nr_pok.IsEnabled = false;
                TB_Poczatek.IsEnabled = false;
                TB_Koniec.IsEnabled = false;
                TB_Posilki_Opis.IsEnabled = false;
                CB_Potwierdzone.IsEnabled = false;
                CB_Posilki.IsEnabled = false;
                TB_Dozaplaty.IsEnabled = false;
                Btn_zapis_rezerwacje.IsEnabled = false;
                Btn_zaplac.IsEnabled = false;
                Btn_usun_rezerwacje.IsEnabled = false;

            }
            else
            {
                TB_nrpokojuPokoje.Text = "";
                TB_Typpokoju.Text = "";
                TB_Cenapokoju.Text = "";
                TB_Pokojopis.Text = "";
                CB_Dosprzatania.SelectedIndex = -1;
                CB_zajety.SelectedIndex = -1;
            }
        }


        //Aktualizacja pokoju
        private void Btn_zapisz_pokoj_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            int zajety, dosprzatania;

            if (CB_zajety.SelectedIndex == 0) { zajety = 1; }
            else { zajety = 0; }

            if (CB_Dosprzatania.SelectedIndex == 0) { dosprzatania = 1; }
            else { dosprzatania = 0; }


            //command.CommandText = "Update Pokoje SET Nr_pok='" +  + "',         Czy_zajety='" +  + "',      Do_sprzatania='" +  + "',       Opis='" + + "',         Typ='" +  + "',         Cena='" +  + "' WHERE id='" + (ListViewPokoje.SelectedItem as Pokoje).Id + "'";
            var message = Application.Current.Properties["Login"] + " edytował pokój: " + TB_nrpokojuPokoje.Text.ToString();
            Logi l = new Logi(message);


            command.CommandText = "Update Pokoje SET Nr_pok='" + Convert.ToInt32(TB_nrpokojuPokoje.Text) + "', Czy_zajety='" + zajety + "', Do_sprzatania='" + dosprzatania + "', Opis='" + TB_Pokojopis.Text.ToString() + "', Typ='" + TB_Typpokoju.Text.ToString() + "', Cena='" + Convert.ToInt32(TB_Cenapokoju.Text) + "' WHERE id='" + (ListViewPokoje.SelectedItem as Pokoje).Id + "'";
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

            foreach (Pokoje x in pokoje)
            {
                if (x.Id == (ListViewPokoje.SelectedItem as Pokoje).Id)
                {
                    x.Nr_pok = Convert.ToInt32(TB_nrpokojuPokoje.Text);
                    x.Typ = TB_Typpokoju.Text.ToString();
                    x.Opis = TB_Pokojopis.Text.ToString();
                    x.Cena = Convert.ToInt32(TB_Cenapokoju.Text);
                    if (CB_zajety.SelectedIndex == 0) { x.Zajety = "Tak"; }
                    else { x.Zajety = "Nie"; }
                    if (CB_Dosprzatania.SelectedIndex == 0) { x.Do_sprzatania = "Tak"; }
                    else { x.Do_sprzatania = "Nie"; }

                }
            }

            TB_nrpokojuPokoje.Text = "";
            TB_Typpokoju.Text = "";
            TB_Cenapokoju.Text = "";
            TB_Pokojopis.Text = "";
            CB_Dosprzatania.SelectedIndex = -1;
            CB_zajety.SelectedIndex = -1;

            ListViewPokoje.Items.Refresh();


        }

        //Archuwim przejscie
        private void MenuBar_Arch_Click(object sender, RoutedEventArgs e)
        {
            ArchiwumWindow archwindow = new ArchiwumWindow();
            archwindow.ShowDialog();
        }

        //Przycisk zOplacone
        private void Btn_zaplac_Click(object sender, RoutedEventArgs e)
        {
            //bool flaga = true;
            //MySqlConnection conn = new MySqlConnection(connString);
            //MySqlCommand command = conn.CreateCommand();
            //int indeks = rezerwacje.IndexOf(ListViewRezerwacje.SelectedItem as Rezerwacja);


            //command.CommandText = "Insert into archiwum(id, Imie, Nazwisko, Nr_pokoju, Data_poczatek, Data_koniec, Cena) " +
            //                      "values ( '" + (ListViewRezerwacje.SelectedItem as Rezerwacja).Id + "', '" + TB_Imie.Text.ToString() + "', '" + TB_Naziwko.Text.ToString() + "', '" + Convert.ToInt64(TB_nr_pok.Text) + "', '" + TB_Poczatek.Text.ToString() + "', '" + TB_Koniec.Text.ToString() + "', '" + Convert.ToInt32(TB_Dozaplaty.Text) + "')";
            //try
            //{
            //    conn.Open();
            //}
            //catch (Exception ex)
            //{
            //    flaga = false;
            //    MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            //}

            //command.ExecuteNonQuery();
            //conn.Close();

            //command.CommandText = "DELETE FROM rezerwacje WHERE id='" + (ListViewRezerwacje.SelectedItem as Rezerwacja).Id + "'";
            //try
            //{
            //    conn.Open();
            //}
            //catch (Exception ex)
            //{
            //    flaga = false;
            //    MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            //}
            //if (flaga) { MessageBox.Show("Dane zostały przeniesione do archiwum", "Zapisano"); }
            //command.ExecuteNonQuery();
            //conn.Close();

            //rezerwacje.RemoveAt(indeks);
            //ListViewRezerwacje.Items.Refresh();

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            int indeks = rezerwacje.IndexOf(ListViewRezerwacje.SelectedItem as Rezerwacja);
            int tmp = 1;
           // "Update Rezerwacje SET Opis='' WHERE id='" + (ListViewRezerwacje.SelectedItem as Rezerwacja).Id + "'";
            command.CommandText = "Update Rezerwacje SET Oplacone = '" + tmp + "' WHERE id='" + (ListViewRezerwacje.SelectedItem as Rezerwacja).Id + "'";
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                //flaga = false;
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }

            command.ExecuteNonQuery();
            conn.Close();

            foreach (Rezerwacja x in rezerwacje)
            {
                if (x.Id == (ListViewRezerwacje.SelectedItem as Rezerwacja).Id)
                {
                    //x.Oplacone = CB_Oplacone.Text.ToString();
                    if (CB_Oplacone.SelectedIndex == 0) { x.Oplacone = "Tak"; }
                    else { x.Oplacone = "Nie"; }
                    (ListViewRezerwacje.SelectedItem as Rezerwacja).Oplacone = "Tak";
                }
            }

            ListViewRezerwacje.Items.Refresh();

            TB_Imie.Text = "";
            TB_Naziwko.Text = "";
            TB_nr_pok.Text = "";
            TB_Koniec.Text = "";
            TB_Poczatek.Text = "";
            TB_Posilki_Opis.Text = "";
            CB_Potwierdzone.SelectedIndex = -1;
            TB_Dozaplaty.Text = "";
            CB_Posilki.SelectedIndex = -1;
            CB_Oplacone.SelectedIndex = -1;

            TB_Imie.IsEnabled = false;
            TB_Naziwko.IsEnabled = false;
            TB_nr_pok.IsEnabled = false;
            TB_Poczatek.IsEnabled = false;
            TB_Koniec.IsEnabled = false;
            TB_Posilki_Opis.IsEnabled = false;
            CB_Potwierdzone.IsEnabled = false;
            CB_Posilki.IsEnabled = false;
            TB_Dozaplaty.IsEnabled = false;
            Btn_zapis_rezerwacje.IsEnabled = false;
            Btn_zaplac.IsEnabled = false;
            Btn_usun_rezerwacje.IsEnabled = false;
            CB_Oplacone.IsEnabled = false;

            var message = Application.Current.Properties["Login"] + " oznaczył rezerwacje jako opłaconą: " + TB_Imie.Text.ToString() + " " + TB_Naziwko.Text.ToString() + " numer pokoju: " + TB_nr_pok.Text.ToString() + ", daty: " + TB_Poczatek.Text.ToString() + " -> " + TB_Koniec.Text.ToString();
            Logi l = new Logi(message);
        }

        private void Wyloguj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Properties["wyloguj"] = true;

            var message = Application.Current.Properties["Login"] + " wylogował się";
            Logi l = new Logi(message);
        }

        private void ZmienHaslo_Click(object sender, RoutedEventArgs e)
        {
            ZmienHasloWindow zhwindow = new ZmienHasloWindow();
            zhwindow.Show();
        }

        private void Btn_dodaj_uzytkownika_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_usun_uzytkownika_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectionChanged_ListViewUzytkownicy(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Uzytkownicy_Click(object sender, RoutedEventArgs e)
        {
            UzytkownicyWindow uwindow = new UzytkownicyWindow();
            uwindow.Show();
        }

        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            foreach (Rezerwacja x in rezerwacje)
            {
                //x.Data_koniec
                DateTime oDate = Convert.ToDateTime(x.Data_koniec);
                if (datadopokoi > oDate)
                {
                    int indeks = x.Id;

                    command.CommandText = "Insert into archiwum(id, Imie, Nazwisko, Nr_pokoju, Data_poczatek, Data_koniec, Cena) " +
                                          "values ( '" + x.Id + "', '" + x.Imie + "', '" + x.Nazwisko.ToString() + "', '" + x.Nr_Pok + "', '" + x.Data_poczatek.ToString() + "', '" + x.Data_koniec.ToString() + "', '" + x.Cena + "')";
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

                    command.CommandText = "DELETE FROM rezerwacje WHERE id='" + x.Id + "'";
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        //flaga = false;
                        MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
                    }
                    //if (flaga) { MessageBox.Show("Dane zostały przeniesione do archiwum", "Zapisano"); }

                    command.ExecuteNonQuery();
                    conn.Close();


                }
            }



            rezerwacje.Clear();
            pokoje.Clear();

            
            command.CommandText = "Select * from Rezerwacje";

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
                rezerwacje.Add(new Rezerwacja
                    (Convert.ToInt32(reader["id"]),
                    reader["Nazwisko"].ToString(),
                    reader["Imie"].ToString(),
                    Convert.ToInt32(reader["Nr_pokoju"]),
                    reader["Data_poczatek"].ToString(),
                    reader["Data_koniec"].ToString(),
                    Convert.ToInt32(reader["Potwierdzone"]),
                    Convert.ToInt32(reader["Posilki"]),
                    reader["Opis"].ToString(),
                    Convert.ToInt32(reader["Oplacone"])
                    ));
            }

            conn.Close();

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
            }

            command.CommandText = "Select * from Pokoje";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                pokoje.Add(new Pokoje
                    (Convert.ToInt32(reader["id"]),
                    Convert.ToInt32(reader["Nr_pok"]),
                    Convert.ToInt32(reader["Czy_zajety"]),
                    Convert.ToInt32(reader["Do_sprzatania"]),
                    reader["Opis"].ToString(),
                    reader["Typ"].ToString(),
                    Convert.ToInt32(reader["Cena"])
                    ));
            }
            conn.Close();

            ListViewPokoje.Items.Refresh();
            ListViewRezerwacje.Items.Refresh();
        }

        private void Btn_Odswierz_click(object sender, RoutedEventArgs e)
        {
            EventArgs ea = new EventArgs();
            dispatcherTimer_Tick(sender, ea);
            MessageBox.Show("Listy zostały wypełnione", "OK");
        }



        private void Calendar_dzin_pokoje_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar.SelectedDate.HasValue)
            {
                Btn_dzisiaj.Content = calendar.SelectedDate.Value.ToString("yyyy-MM-dd");
                Calendar_dzin_pokoje.Visibility = Visibility.Hidden;
                WypelnijListe(Convert.ToDateTime(Btn_dzisiaj.Content));
            }
            //ListViewRezerwacje.Items.Refresh();
        }

        private void Btn_dzisiaj_Click(object sender, RoutedEventArgs e)
        {
            Calendar_dzin_pokoje.Visibility = Visibility.Visible;
        }

        public void WypelnijListe(DateTime data)
        {
            DateTime przekazanadata = data;
            bool flag = false, flagaprzeciecie = false;
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Rezerwacje"; //Wypis z bazy rezerwacji

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
                flag = false;
                foreach (var x in daty)
                {
                    if (x.Nr_pok == Convert.ToInt32(reader["Nr_pokoju"]))
                    {
                        flag = true;
                        x.datypoczatekkoniec.Add(new DatyPoczatekKoniec(Convert.ToDateTime(reader["Data_poczatek"]), Convert.ToDateTime(reader["Data_koniec"]), Convert.ToString(reader["Nazwisko"])));
                    }
                }
                if (flag == false)
                {
                    daty.Add(new Daty(Convert.ToInt32(reader["Nr_pokoju"]), Convert.ToDateTime(reader["Data_poczatek"]), Convert.ToDateTime(reader["Data_koniec"]), Convert.ToString(reader["Nazwisko"])));
                }
            }
            conn.Close();

            command.CommandText = "Select * from Pokoje"; //Wypis z bazy pokoi 

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
                pokoj.Add(Convert.ToInt32(reader["Nr_pok"]));
            }
            conn.Close();

            foreach (int x in pokoj)
            {
                foreach (Daty d in daty)
                {
                    if (x == d.Nr_pok)
                    {
                        foreach (var dpk in d.datypoczatekkoniec)
                        {

                            if (dpk.Koniec >= przekazanadata && dpk.Poczatek <= przekazanadata)
                            {
                                foreach (Pokoje p in pokoje)
                                {
                                    if (p.Nr_pok == x) { p.Zajety = "TAK"; p.Nazwisko = dpk.Nazwisko; }
                                }
                            }
                        }
                    }
                }

                if (flagaprzeciecie == false) { }
                flagaprzeciecie = false;
            }

            ListViewPokoje.Items.Refresh();


        }

        private void Logi_Click(object sender, RoutedEventArgs e)
        {
            LogiWindow uwindow = new LogiWindow();
            uwindow.Show();
        }
    }


    //prototyp rezerwacji
    public class Rezerwacja
    {
        public int Id { get; set; }
        public string Nazwisko { get; set; }
        public string Imie { get; set; }
        public int Nr_Pok { get; set; }
        public string potwierdzone { get; set; }
        public string Data_poczatek { get; set; }
        public string Data_koniec { get; set; }
        public string Ilosc_Dni { get; set; }
        public int Cena { get; set; }
        public int Posilki { get; set; }
        public string Opis { get; set; }
        public string Oplacone { get; set; }

        public Rezerwacja() { }

        public Rezerwacja(int id, string nazwisko, string imie, int nrPok, string data_poczatek, string data_koniec, int potw, int posilki, string opis, int opl)
        {
            this.Id = id;
            this.Nazwisko = nazwisko;
            this.Imie = imie;
            this.Nr_Pok = nrPok;
            this.Opis = opis;
            this.Posilki = posilki;
            this.Data_poczatek = data_poczatek;
            this.Data_koniec = data_koniec;

            if (potw == 0)
            {
                this.potwierdzone = "Nie potwierdzone";
            }
            else
            {
                this.potwierdzone = "Potwierdzone";
            }
            if (opl == 0)
            {
                this.Oplacone= "Nie";
            }
            else
            {
                this.Oplacone = "Tak";
            }
            TimeSpan ts = DateTime.Parse(data_koniec) - DateTime.Parse(data_poczatek);
            this.Ilosc_Dni = ts.TotalDays.ToString();


        }
    }

    //Prototyp Pokoji
    public class Pokoje
    {
        public int Id { get; set; }
        public int Nr_pok { get; set; }
        public string Zajety { get; set; }
        public string Do_sprzatania { get; set; }
        public string Opis { get; set; }
        public string Typ { get; set; }
        public int Cena { get; set; }
        public string Nazwisko { get; set; }

        public Pokoje() { }

        public Pokoje(int id, int nr_pok, int zajety, int do_sprzatania, string opis, string typ, int cena)
        {
            this.Id = id;
            this.Nr_pok = nr_pok;
            this.Opis = opis;
            this.Typ = typ;
            this.Cena = cena;
            this.Nazwisko = "";

            if (zajety == 0) { this.Zajety = "Nie"; }
            else { this.Zajety = "Tak"; }

            if (do_sprzatania == 0) { this.Do_sprzatania = "Nie"; }
            else { this.Do_sprzatania = "Tak"; }

        }
    }

    public class Daty
    {
        public int Nr_pok { get; set; }
        public List<DatyPoczatekKoniec> datypoczatekkoniec { get; set; }


        public Daty() { }

        public Daty(int nr_pok, DateTime poczatek, DateTime koniec, string nazwisko)
        {
            datypoczatekkoniec = new List<DatyPoczatekKoniec>();
            this.Nr_pok = nr_pok;
            datypoczatekkoniec.Add(new DatyPoczatekKoniec(poczatek, koniec, nazwisko));
        }
    }

    public class DatyPoczatekKoniec
    {
        public DateTime Poczatek { get; set; }
        public DateTime Koniec { get; set; }
        public string Nazwisko { get; set; }


        public DatyPoczatekKoniec() { }

        public DatyPoczatekKoniec(DateTime poczatek, DateTime koniec, string nazwisko)
        {
            this.Poczatek = poczatek;
            this.Koniec = koniec;
            this.Nazwisko = nazwisko;
        }
    }

}
