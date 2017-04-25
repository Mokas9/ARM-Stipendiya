using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ARMRBT
{
    public partial class Authorization : Form
    {
        public Authorization()
        {
            InitializeComponent();
        }

        public Database database;

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Заполните поля!");
                return;
            }

            database = new Database("127.0.0.1", textBox3.Text, textBox1.Text, textBox2.Text);

            if (database.OpenConnect())
                (new Menu(this)).Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MySqlConnection mysqlconn = new MySqlConnection(string.Format("server = localhost;  PORT = {0} ;userid = {1}; password = {2}; database = {3}", textBox3.Text ,textBox1.Text, textBox2.Text, "podschet"));
            try
            {
                mysqlconn.Open();
                MessageBox.Show("Соединение прошло успешно!");
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Ошибка соединения!\n"+ex.ErrorCode+"\n"+ex.Message);
            }
        }
    }
}