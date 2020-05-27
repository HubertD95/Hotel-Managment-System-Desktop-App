using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inz_vol._2
{
    class Logi
    {
        string connString = "Server=localhost;Port=3306;Database=inzynierka;Uid=root;Password=;";


        private string Message { get; set; }

        public Logi() { }
        public Logi ( string message )
        {
            DateTime now = DateTime.Now;

            Message = now.ToString() + "  " + message;
            //Message = now.ToString() + "  " + Message;
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "Insert into Logi (Wiadomosc) " + "values ( '" + Message.ToString() + "')";

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
            }
            command.ExecuteNonQuery();
            conn.Close();

        }
    }
}
