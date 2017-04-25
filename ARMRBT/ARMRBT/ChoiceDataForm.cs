using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MySql.Data.MySqlClient;

namespace ARMRBT
{
    public class ChoiceDataForm
    {
        public delegate void ChangedValue(object sender, EventArgs arg);
        public event ChangedValue OnChangedValue;

        public EditFormTable EditFormTable;
        public Database Database;
        public List<Control> CreatedControls = new List<Control>();

        private DataGridView _dataGridView;

        public ChoiceDataForm(EditFormTable editFormTable, Database database, DataGridView dataGridView)
        {
            this.EditFormTable = editFormTable;
            this.Database = database;
            this._dataGridView = dataGridView;
        }

        public void FillControls(Panel panel)
        {
            CreateControlElements(panel, EditFormTable);
        }

        public void CellEndEdit(int ID)
        {
            foreach(Control control in CreatedControls)
                if (control is ComboBox)
                {
                    (control as ComboBox).Items.Clear();
                    List<string> values = GetValuesForComboBox((control as ComboBox).Tag as FieldForm);
                    foreach (string val in values)
                        (control as ComboBox).Items.Add(val);
                }

            //FillValues(ID);
        }

        private void CreateControlElements(Panel panel1, EditFormTable eft)
        {
            int X = 10;
            int Y = 30;
            int maxwidth = 0;
            bool havecombobox = false;

            for (int i = 0; i < eft.Fields.Count; i++)
            {
                if (!eft.Fields[i].HaveLink)
                    break;

                Label label = new Label();
                label.Text = eft.Fields[i].FieldCaption;
                maxwidth = (maxwidth < label.Width ? label.Width : maxwidth);
                label.Location = new Point(X, Y);
                panel1.Controls.Add(label);
                Y += 25;
            }

            Y = 30;

            for (int i = 0; i < eft.Fields.Count; i++)
            {
                if (eft.Fields[i].HaveLink)
                {
                    havecombobox = true;
                    ComboBox comboBox = new ComboBox();
                    comboBox.Tag = eft.Fields[i];
                    comboBox.Name = "comboBox" + i;
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBox.Width = 200;
                    comboBox.Location = new Point(maxwidth + X, Y);
                    comboBox.SelectedIndexChanged += delegate (object sender, EventArgs arg)
                    {
                        if (_dataGridView.RowCount == 0)
                        {
                            comboBox.SelectedIndex = -1;
                            return;
                        }

                        ChangedValueCombobox(sender);
                    };
                    UpdateComboBox(comboBox, eft.Fields[i]);
                    panel1.Controls.Add(comboBox);

                    CreatedControls.Add(comboBox);
                }
               
                Y += 25;
            }
        }
        private List<string> GetValuesForComboBox(FieldForm fieldf)
        {
            fieldf.IDs.Clear();
            List<string> stringsvalues = new List<string>();
            DataTable dt;
            string query = "SELECT";
            for (int i = 0; i < fieldf.FieldFullNames.Count; i++)
                query += (i == 0 ? " " : ", ") + fieldf.FieldFullNames[i];
            query += " FROM " + fieldf.FieldFullNames[0].Split('.')[0];

            dt = Database.SelectQuery(query);

            bool firstid;
            string str = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                str = "";
                firstid = true;
                foreach (DataColumn column in dt.Columns)
                {
                    if (firstid)
                        fieldf.IDs.Add(int.Parse(row[column].ToString()));
                    else
                        str += row[column] + " ";
                    firstid = false;
                }
                stringsvalues.Add(str);
            }

            return stringsvalues;
        }
        private void UpdateComboBox(ComboBox cb, FieldForm fieldf)  //Заполняем ComboBox данными
        {
            cb.Items.Clear();
            List<string> valcombobox = GetValuesForComboBox(fieldf);
            for (int q = 0; q < valcombobox.Count; q++)
                cb.Items.Add(valcombobox[q]);
        }
        private string CreateInsertQuery() //Создание запроса на добавление запись в таблицу
        {
            string query = "INSERT INTO " + EditFormTable.NameTable + " (";
            DataTable dt = Database.SelectQuery("SELECT * FROM " + EditFormTable.NameTable);
            bool first = true;
            bool cont = true;
            foreach (DataColumn dc in dt.Columns)
            {
                if (cont)
                {
                    cont = false;
                    continue;
                }

                if (first)
                {
                    query += dc.ColumnName;
                    first = false;
                }
                else
                    query += ", " + dc.ColumnName;
            }
            query += ") values(";
            first = true;
            //CreatedControls
            foreach (Control control in CreatedControls)
            {
                if (control is Label || control is Button)
                    continue;

                if (first)
                {
                    query += " '";
                    //query += (control is ComboBox) ? " '" + GetValueFieldComboBox((control as ComboBox), ((control as ComboBox).Tag as FieldForm)) + "' " : " '"+(control as TextBox).Text+"' ";
                    first = false;
                }
                else
                    query += ", '";
                //query += (control is ComboBox) ? ", '"+GetValueFieldComboBox((control as ComboBox), ((control as ComboBox).Tag as FieldForm))+"' " : ", '" + (control as TextBox).Text + "' ";
                //MessageBox.Show(control.Name);

                if (control is ComboBox)
                {
                    query += GetValueFieldComboBox((control as ComboBox), (control as ComboBox).Tag as FieldForm) + "' ";
                }
                else if (control is TextBox)
                {
                    query += (control as TextBox).Text + "' ";
                }
                else if (control is DateTimePicker)
                {
                    //query += (control as DateTimePicker).Value.Date + "' ";
                    query += string.Format("{0}.{1}.{2}", (control as DateTimePicker).Value.Year, (control as DateTimePicker).Value.Month, (control as DateTimePicker).Value.Date.Day) + "' ";
                }
                else
                {
                    query += ((control as CheckBox).Checked ? 1 : 0) + "' ";
                }

            }
            query += ");";

            return query;
        }
        public int GetValueFieldComboBox(ComboBox cb, FieldForm fieldf)  //Получаем значение ID из другой таблицы
        {
            return fieldf.IDs[cb.SelectedIndex];
        }
        private void ClearValues()
        {
            foreach(Control control in CreatedControls)
            {
                if (control is ComboBox)
                    (control as ComboBox).SelectedIndex = -1;

                if (control is TextBox)
                    (control as TextBox).Text = "";
            }
        }
        private void FillValues(int ID)  //Заполнение значений в combobox
        {
            DataTable dt = Database.SelectQuery(string.Format("SELECT * FROM {0}", EditFormTable.NameTable));
            dt = Database.SelectQuery(string.Format("SELECT * FROM {0} WHERE {1} = {2}", EditFormTable.NameTable, dt.Columns[0].ColumnName, ID));

            for (int i = 0; i < CreatedControls.Count; i++)
            {
                FieldForm ff = (CreatedControls[i] as ComboBox).Tag as FieldForm;
                (CreatedControls[i] as ComboBox).SelectedIndex = ff.IDs.IndexOf((int)dt.Rows[0][dt.Columns[i+1]]);
            }
        }
        public void SelectedRow (object sender, DataGridViewCellMouseEventArgs e)
        {
            //FillValues((int)_dataGridView.Rows[e.RowIndex].Cells[0].Value);
        }
        protected virtual void ChangedValueCombobox(object sender)
        {
            if (OnChangedValue != null)
                OnChangedValue(sender, new EventArgs());
        }
    }
}
