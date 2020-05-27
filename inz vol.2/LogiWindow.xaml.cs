using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LogiWindow.xaml
    /// </summary>
    public partial class LogiWindow : Window
    {
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";

        public ObservableCollection<Logi> logi{ get; set; } = new ObservableCollection<Logi>();
        public LogiWindow()
        {
            InitializeComponent();
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Logi"; //Wypis z bazy
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
                logi.Add(new Logi(Convert.ToInt32(reader["id"]), reader["Wiadomosc"].ToString()));
            }
            conn.Close();

            int i = 0;
            string datadzis = null;
            foreach ( Logi l in logi)
            {
                datadzis = null;
                i = 0;
                while(l.Log[i] != ' ') { datadzis += l.Log[i]; i++; }
                TimeSpan result = DateTime.Today - DateTime.Parse(datadzis);
                 if ( result.TotalDays > 31)
                {
                    command.CommandText = "Delete From Logi Where id='"+ l.Id +"'"; //Wypis z bazy
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Nie udało się połączyć z bazą danych", "Błąd");
                    }
                    command.ExecuteNonQuery();

                    logi.RemoveAt(l.Id);
                }
            }

            conn.Close();
            ListViewRezerwacje.ItemsSource = logi;
        }
        public class Logi
        {
            public int Id { get; set; }
            public string Log { get; set; }
            public Logi() { }
            public Logi(int id, string w)
            {
                this.Log = w;
                this.Id = id;
            }

            
        }

        private void TB_wyszukajTextChange(object sender, TextChangedEventArgs e)
        {
            logi.Clear();
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "SELECT* FROM `logi` WHERE `Wiadomosc` LIKE '%" + TB_wyszukaj.Text.ToString() + "%'"; //Wypis z bazy
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
                logi.Add(new Logi(Convert.ToInt32(reader["id"]), reader["Wiadomosc"].ToString()));
            }

            conn.Close();
            ListViewRezerwacje.ItemsSource = logi;
            ListViewRezerwacje.Items.Refresh();
        }
    }

    
}
