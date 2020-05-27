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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace inz_vol._2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";
        int tmpp = 0;
        public List<Uzytkownicy> Uzytkownicy { get; set; } = new List<Uzytkownicy>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_Zaloguj_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            int tmp = -1;
            bool flaga = false;

            command.CommandText = "Select * from Uzytkownicy";

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
                Uzytkownicy.Add(new Uzytkownicy
                    (reader["Login"].ToString(),
                    reader["Haslo"].ToString(),
                    Convert.ToInt32(reader["Typ"])
                    ));
            }

            conn.Close();

            if (Uzytkownicy.Any()) {
                foreach (Uzytkownicy u in Uzytkownicy)
                {
                    if (TBLogin.Text.ToString() == u.Login && TBHaslo.Password.ToString() == u.Haslo)
                    {
                        tmpp = 1;
                        Application.Current.Properties["Login"] = TBLogin.Text.ToString();
                        Application.Current.Properties["Haslo"] = TBHaslo.Password.ToString();
                        Application.Current.Properties["Typ"] = u.Typ;

                        this.Hide();

                        var message = Application.Current.Properties["Login"] + " zalogował się";
                        Logi l = new Logi(message);

                        switch (tmp)
                        {
                            case 2:
                                {
                                    SerwisWindow window = new SerwisWindow();
                                    window.ShowDialog();
                                    flaga = Convert.ToBoolean(Application.Current.Properties["wyloguj"]);
                                    if (flaga == true)
                                    {
                                        this.Show();
                                        TBLogin.Text = "";
                                        TBHaslo.Password = "";
                                        Application.Current.Properties["wyloguj"] = false;
                                    }
                                    else
                                    {
                                        this.Close();
                                    }
                                    break;
                                }
                            default:
                                {
                                    FirstWindow firstwindow = new FirstWindow();
                                    firstwindow.ShowDialog();
                                    flaga = Convert.ToBoolean(Application.Current.Properties["wyloguj"]);
                                    if (flaga == true)
                                    {
                                        this.Show();
                                        TBLogin.Text = "";
                                        TBHaslo.Password = "";
                                        Application.Current.Properties["wyloguj"] = false;
                                    }
                                    else
                                    {
                                        this.Close();
                                    }
                                    break;
                                }
                        }
                    }
                    
                }
                if (tmpp != 1)
                {
                    MessageBox.Show("Błędne dane logowania", "Błąd");
                    TBLogin.Text = "";
                    TBHaslo.Password = "";
                }
            }
            else
            {
                command.CommandText = "Insert into Uzytkownicy (Login, Haslo, Typ) values ( 'admin', 'admin', '1' )";

                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                }
                command.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Lista uzytkowników jest pusta, dodano konto Superadministratora", "Błąd");
            }

            //command.CommandText = "Select Typ From uzytkownicy Where Login = '" + TBLogin.Text.ToString() + "' And Haslo = '" + TBHaslo.Password.ToString() + "' ";
            //Application.Current.Properties["Login"] = TBLogin.Text.ToString();
            //Application.Current.Properties["Haslo"] = TBHaslo.Password.ToString();
            //try
            //{
            //    conn.Open();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Nie udało się połączyć z bazą użytkowników", "Błąd");
            //}

            //MySqlDataReader reader = command.ExecuteReader();
            //while (reader.Read())
            //{
            //    Application.Current.Properties["Typ"] = Convert.ToInt16(reader["Typ"]);
            //}
            //tmp = Convert.ToInt16(Application.Current.Properties["Typ"]);
            //conn.Close();

            //if (tmp != -1)
            //{
            //    this.Hide();

            //    var message = Application.Current.Properties["Login"] + " zalogował się";
            //    Logi l = new Logi(message);

            //    switch (tmp)
            //    {
            //        case 2:
            //            {
            //                SerwisWindow window = new SerwisWindow();
            //                window.ShowDialog();
            //                flaga = Convert.ToBoolean(Application.Current.Properties["wyloguj"]);
            //                if (flaga == true)
            //                {
            //                    this.Show();
            //                    TBLogin.Text = "";
            //                    TBHaslo.Password = "";
            //                    Application.Current.Properties["wyloguj"] = false;
            //                }
            //                else
            //                {
            //                    this.Close();
            //                }
            //                break;
            //            }
            //        default:
            //            {
            //                FirstWindow firstwindow = new FirstWindow();
            //                firstwindow.ShowDialog();
            //                flaga = Convert.ToBoolean(Application.Current.Properties["wyloguj"]);
            //                if (flaga == true)
            //                {
            //                    this.Show();
            //                    TBLogin.Text = "";
            //                    TBHaslo.Password = "";
            //                    Application.Current.Properties["wyloguj"] = false;
            //                }
            //                else
            //                {
            //                    this.Close();
            //                }
            //                break;
            //            }
            //    }

            //}
            //else
            //{
            //    MessageBox.Show("Błędne dane logowania", "Błąd");
            //    TBLogin.Text = "";
            //    TBHaslo.Password = "";
            //}



        }

        //nacisniecie enter podczas wpisywania
        private void KeyDown(object sender, KeyEventArgs e)
        {
            object o = new object();
            RoutedEventArgs k = new RoutedEventArgs();
            if (e.Key == Key.Return)
            {
                Btn_Zaloguj_Click(o, k);
            }
        }
    }

    public class Uzytkownicy
    {
        public string Login{ get; set; }
        public string Haslo { get; set; }
        public int Typ{ get; set; }


        public Uzytkownicy() { }

        public Uzytkownicy(string login, string haslo, int typ)
        {
            this.Login = login;
            this.Haslo = haslo;
            this.Typ = typ;
        }
    }
}
