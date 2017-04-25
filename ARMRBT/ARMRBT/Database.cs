using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.IO;


namespace ARMRBT
{
    public enum TableType
    {
        Stipendiya,
        Sessii,
        Student,
        Zacheti,
        Gruppi,
        Predmeti,
        Specialnosti,
        Kafedri,
        Faculteti
    }

    public class Database
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NameDatabase { get; set; }
        public string Port { get; set; }

        public MySqlConnection mysqlconnection;
        public MySqlCommand mysqlcommand;
        public MySqlDataAdapter mysqladapter;

        public string NetConnectionString
        {
            get
            {
                return string.Format("server = localhost;  PORT = {0} ;userid = '{1}'; password = '{2}'; database = '{3}';charset=utf8", Port, Username, Password, NameDatabase);
            }
        }

        public bool OpenConnect()
        {
            mysqlconnection = new MySqlConnection(NetConnectionString);


            try
            {
                mysqlconnection.Open();
                mysqlcommand = new MySqlCommand();
                mysqladapter = new MySqlDataAdapter();
                return true;
            }
            catch
            {
                MessageBox.Show("Неправильно введены логин или пароль!", "Ошибка!");
                return false;
            }
        }

        public DataTable SelectQuery(string query)
        {
            DataTable dataTable = new DataTable();
            mysqlcommand.Connection = mysqlconnection;
            mysqlcommand.CommandText = query;
            mysqladapter.SelectCommand = mysqlcommand;
            mysqladapter.Fill(dataTable);
            return dataTable;
        }

        public void Query(string query)
        {
            try
            {
                mysqlcommand.CommandText = query;
                mysqlcommand.Connection = mysqlconnection;
                mysqlcommand.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK);
            }
        }

        public Database(string server, string port, string username, string password, string namedatabase = "podschet")
        {
            this.Server = server;
            this.Port = port;
            this.Username = username;
            this.Password = password;
            this.NameDatabase = namedatabase;
        }
    }
}
