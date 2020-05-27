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

    public partial class DodajRezerwacjeWindow : Window
    {
        bool poczatek_click = false, koniec_click = false;

        public List<Daty> daty { get; set; } = new List<Daty>();
        public List<Pokoj> pokoje { get; set; }
        DateTime pDate, kDate;

        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";

        public DodajRezerwacjeWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Btn_zapisz_rezerwacje.IsEnabled = false;
            pokoje = new List<Pokoj>();
            //Kalendarz.Visibility = Visibility.Hidden;
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
                pokoje.Add(new Pokoj(Convert.ToInt32(reader["Nr_pok"]),
                    reader["Typ"].ToString()
                    ));
            }
            conn.Close();

            //foreach (int x in pokoje)
            //{
                //CB_nr_pok.Items.Add(x);
            //}

            
        }


        //Kalendarze
        private void Btn_poczatek_Click(object sender, RoutedEventArgs e)
        {
            poczatek_click = true;
            Kalendarz.Visibility = Visibility.Visible;
            if (poczatek_click && koniec_click) { Enable(); }
        }

        private void Kalendarz_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar.SelectedDate.HasValue)
            {
                Label_Poczatek.Content = calendar.SelectedDate.Value.ToString("yyyy-MM-dd");
                Kalendarz.Visibility = Visibility.Hidden;
            }
            if(poczatek_click && koniec_click)
            {
                pDate = Convert.ToDateTime(Label_Poczatek.Content.ToString());
                kDate = Convert.ToDateTime(Label_Koniec.Content.ToString());
                if (pDate < kDate)
                {
                    WypelnijListe();
                }
                else
                {
                    MessageBox.Show("Błędne daty", "Błąd");
                    poczatek_click = false;
                    koniec_click = false;
                    Label_Poczatek.Content = "";
                    Label_Koniec.Content = "";
                }
            }
        }

        private void Btn_koniec_Click(object sender, RoutedEventArgs e)
        {
            koniec_click = true;
            Kalendarz2.Visibility = Visibility.Visible;
            if (poczatek_click && koniec_click) { Enable(); }
        }

        private void Kalendarz2_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar.SelectedDate.HasValue)
            {
                Label_Koniec.Content = calendar.SelectedDate.Value.ToString("yyyy-MM-dd");
                Kalendarz2.Visibility = Visibility.Hidden;
            }
            if (poczatek_click && koniec_click)
            {
                pDate = Convert.ToDateTime(Label_Poczatek.Content.ToString());
                kDate = Convert.ToDateTime(Label_Koniec.Content.ToString());
                if (pDate < kDate)
                {
                    WypelnijListe();
                }
                else
                {
                    MessageBox.Show("Błędne daty", "Błąd");
                    poczatek_click = false;
                    koniec_click = false;
                    Label_Poczatek.Content = "";
                    Label_Koniec.Content = "";
                }
            }
        }


        //Zapisz Btn
        private void Btn_zapisz_rezerwacje_Click(object sender, RoutedEventArgs e)
        {
            bool tmp = true;
            string nr_pok = null ;
            int posilki;
            if (CB_Posilki.SelectedIndex == 0) { posilki = 0; }
            if (CB_Posilki.SelectedIndex == 1) { posilki = 1; }
            if (CB_Posilki.SelectedIndex == 2) { posilki = 2; }
            if (CB_Posilki.SelectedIndex == 3) { posilki = 3; }
            else { posilki = 0; }

            foreach(char ch in CB_nr_pok.SelectedItem.ToString())
            {
                if(ch == '-') { tmp = false; }
                if(tmp) { nr_pok += ch; }
            }

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Insert into Rezerwacje (Nazwisko,Imie,Nr_pokoju, Potwierdzone, Data_poczatek, Data_koniec, Posilki, Opis) " +
                                  "values ('" + TB_Nazwisko.Text.ToString() + "', '" + TB_Imie.Text.ToString() + "', '" + Convert.ToInt32(nr_pok) + "', '0', '" + Label_Poczatek.Content.ToString() + "', '" + Label_Koniec.Content.ToString() + "', '" + posilki + "', '" + TB_Opis.Text.ToString() + "')";

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

            var message = Application.Current.Properties["Login"] + " dodał rezerwacje na nazwisko: " + TB_Nazwisko.Text.ToString() + " " + TB_Imie.Text.ToString() + ", na pokój nr: " + nr_pok.ToString() + ", data: " + Label_Poczatek.Content.ToString() + " -> " + Label_Koniec.Content.ToString();
            Logi l = new Logi(message);

            this.Close();

        }

        //Anuluj Btn
        private void Btn_anuluj_rezerwacje_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        public class Daty
        {
            public int Nr_pok { get; set; }
            public List<DatyPoczatekKoniec> datypoczatekkoniec { get; set; } 


            public Daty() { }

            public Daty(int nr_pok, DateTime poczatek, DateTime koniec)
            {
                datypoczatekkoniec = new List<DatyPoczatekKoniec>();
                this.Nr_pok = nr_pok;
                datypoczatekkoniec.Add(new DatyPoczatekKoniec(poczatek, koniec));
            }
        }

        public class DatyPoczatekKoniec
        {
            public DateTime Poczatek { get; set; }
            public DateTime Koniec { get; set; }


            public DatyPoczatekKoniec() { }

            public DatyPoczatekKoniec(DateTime poczatek, DateTime koniec)
            {
                this.Poczatek = poczatek;
                this.Koniec = koniec;
            }
        }

        public class Pokoj
        {
            public int Nr_pok { get; set; }
            public string Typ { get; set; }


            public Pokoj() { }

            public Pokoj(int nr_pok, string Typ)
            {
                this.Nr_pok = nr_pok;
                this.Typ = Typ;
            }
        }


        private void Enable()
        {
            TB_Imie.IsEnabled = true;
            TB_Nazwisko.IsEnabled = true;
            TB_Opis.IsEnabled = true;
            CB_nr_pok.IsEnabled = true;
            CB_Posilki.IsEnabled = true;
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            if (TB_Imie.Text.Length != 0 && TB_Nazwisko.Text.Length != 0 && CB_nr_pok.SelectedIndex != -1 && CB_Posilki.SelectedIndex != -1)
            {
                Btn_zapisz_rezerwacje.IsEnabled = true;
            }
        }

        private void SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (TB_Imie.Text.Length != 0 && TB_Nazwisko.Text.Length != 0 && CB_nr_pok.SelectedIndex != -1 && CB_Posilki.SelectedIndex != -1)
            {
                Btn_zapisz_rezerwacje.IsEnabled = true;
            }
        }

        public void WypelnijListe()
        {
            CB_nr_pok.Items.Clear();
            bool flag = false, flagaprzeciecie = false;
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

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                foreach (var x in daty)
                {
                    if (x.Nr_pok == Convert.ToInt32(reader["Nr_pokoju"]))
                    {
                        flag = true;
                        x.datypoczatekkoniec.Add(new DatyPoczatekKoniec (Convert.ToDateTime(reader["Data_poczatek"]), Convert.ToDateTime(reader["Data_koniec"])));
                    }
                }
                if (flag == false)
                {
                    daty.Add(new Daty(Convert.ToInt32(reader["Nr_pokoju"]), Convert.ToDateTime(reader["Data_poczatek"]), Convert.ToDateTime(reader["Data_koniec"])));
                }
            }
            conn.Close();

            foreach (Pokoj x in pokoje)
            {
                foreach (Daty d in daty)
                {
                    if (x.Nr_pok == d.Nr_pok)
                    {
                        foreach (var dpk in d.datypoczatekkoniec)
                        {
                            if (dpk.Poczatek >= Convert.ToDateTime(Label_Poczatek.Content) && dpk.Poczatek <= Convert.ToDateTime(Label_Koniec.Content)) { flagaprzeciecie = true; }
                            if (dpk.Koniec >= Convert.ToDateTime(Label_Poczatek.Content) && dpk.Poczatek <= Convert.ToDateTime(Label_Koniec.Content)) { flagaprzeciecie = true; }
                        }
                    }
                }

                if(flagaprzeciecie == false) { CB_nr_pok.Items.Add((x.Nr_pok.ToString() + "-" + x.Typ).ToString()); }
                flagaprzeciecie = false;
            }



                //if (arrayList.get(i).data.after(xxx) && arrayList.get(i).data.before(yyy))
                  //  System.out.println(arrayList.get(i).nazwa);
        }
    }
}


