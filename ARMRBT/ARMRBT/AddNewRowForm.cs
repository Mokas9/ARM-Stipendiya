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
    public class AddNewRowForm
    {
        public delegate void Click(object sender, EventArgs arg);
        public event Click OnClick;

        public TableType TableType;
        public EditFormTable EditFormTable;
        public Database Database;
        public List<Control> CreatedControls = new List<Control>();

        public AddNewRowForm(TableType tableType, EditFormTable editFormTable, Database database)
        {
            this.TableType = tableType;
            this.EditFormTable = editFormTable;
            this.Database = database;
        }

        public void FillControls(Panel panel)
        {
            CreateControlElements(panel, EditFormTable);
        }

        public void CellEndEdit()
        {
            foreach(Control control in CreatedControls)
                if (control is ComboBox)
                {
                    (control as ComboBox).Items.Clear();
                    List<string> values = GetValuesForComboBox((control as ComboBox).Tag as FieldForm);
                    foreach (string val in values)
                        (control as ComboBox).Items.Add(val);
                }
        }

        private void CreateControlElements(Panel panel1, EditFormTable eft)
        {
            int X = 10;
            int Y = 30;
            int maxwidth = 0;
            bool havecombobox = false;

            for (int i = 0; i < eft.Fields.Count; i++)
            {
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
                    UpdateComboBox(comboBox, eft.Fields[i]);
                    panel1.Controls.Add(comboBox);

                    CreatedControls.Add(comboBox);
                }
                else if (eft.Fields[i].Date)
                {
                    DateTimePicker dateTimePicker = new DateTimePicker();
                    dateTimePicker.Tag = eft.Fields[i];
                    dateTimePicker.CustomFormat = "yyyy-MMM-dd";
                    dateTimePicker.Format = DateTimePickerFormat.Custom;
                    dateTimePicker.Name = "dateTimePicker" + i;
                    dateTimePicker.Width = 200;
                    dateTimePicker.Location = new Point(maxwidth + X, Y);
                    panel1.Controls.Add(dateTimePicker);
                    CreatedControls.Add(dateTimePicker);
                }
                else if (eft.Fields[i].Check)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Tag = eft.Fields[i];
                    checkBox.Name = "checkBox" + i;
                    checkBox.Width = 200;
                    checkBox.Location = new Point(maxwidth + X, Y);
                    panel1.Controls.Add(checkBox);
                    CreatedControls.Add(checkBox);
                }
                else
                {

                    TextBox textBox = new TextBox();
                    textBox.Tag = eft.Fields[i];
                    if (eft.Fields[i].CheckInt)
                        textBox.TextChanged += delegate (object sender, EventArgs arg)
                        {
                            if (textBox.Text.Length == 0)
                                return;

                            if (!Char.IsDigit(textBox.Text[textBox.Text.Length - 1]))
                            {
                                MessageBox.Show("Должны вводиться числа!", "Ошибка!", MessageBoxButtons.OK);
                                textBox.Text = "";
                                return;
                            }
                        };
                    textBox.Name = "textBox" + i;
                    textBox.Width = 200;
                    textBox.Location = new Point(maxwidth + X, Y);
                    panel1.Controls.Add(textBox);
                    CreatedControls.Add(textBox);
                }

                Y += 25;
            }

            Button button = new Button();
            button.Cursor = Cursors.Hand;
            button.Click += SendQuery;
            button.Name = "ButtonOk";
            button.Text = "OK";
            button.Width = 100;
            button.Location = new Point(panel1.Width / 2 - button.Width / 2, Y + 10);

            panel1.Controls.Add(button);
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
        private void SendQuery(object sender, EventArgs e)  //Выполняем запрос
        {
            if (EmptyValueInField())
                return;

            string query;
            if (TableType == TableType.Sessii)
            {
                query = CreateInsertQuerySessii();
            }
            else if (TableType == TableType.Stipendiya)
            {
                query = CreateInsertQueryStipendiya();
            }
            else
                query = CreateInsertQuery();

            Database.Query(query);
            ClearValues();
            ClickButton(); 
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
        private string CreateInsertQuerySessii()
        {
            int IDStudenta = GetValueFieldComboBox((CreatedControls[0] as ComboBox), (CreatedControls[0] as ComboBox).Tag as FieldForm);
            string que = string.Format("SELECT AVG(zach_mark) AS Average FROM zacheti WHERE id_studenta = '{0}'", IDStudenta);
            DataTable datat = Database.SelectQuery(que);

            int mark = 0;

            try
            {
                mark = (int)Math.Round(Convert.ToDouble(datat.Rows[0][0].ToString()));
            }
            catch { };

            string query = string.Format("INSERT INTO {0} ({1},{2},{3}) values ('{4}', '{5}', '{6}')", EditFormTable.NameTable, "id_studenta", "mark_itog", "data_sdachi", IDStudenta, mark, (CreatedControls[1] as TextBox).Text);

            return query;
        }
        private string CreateInsertQueryStipendiya()
        {
            int IDStudenta = GetValueFieldComboBox((CreatedControls[0] as ComboBox), (CreatedControls[0] as ComboBox).Tag as FieldForm);
            int stipendiya = 0;
            string que = string.Format("SELECT mark_itog FROM sessii WHERE id_studenta = '{0}'", IDStudenta);
            DataTable datat = Database.SelectQuery(que);
            int mark = 0;
            if (datat.Rows.Count != 0)
            {
                try
                {
                    mark = (int)Math.Round(Convert.ToDouble(datat.Rows[0][0].ToString()));
                }
                catch { };
            }
            

            if (5 >= mark && mark >= 0)
                stipendiya = 0;
            else if (mark >= 6 && 8 >= mark) //if (avr >= 6 && 8 <= avr)
            {
                stipendiya = 60;
            }
            else if (mark >= 9 && 10 >= mark)
            {
                stipendiya = 70;
            }

            string query = string.Format("INSERT INTO {0} ({1},{2}) values ('{3}', '{4}')", EditFormTable.NameTable, "id_studenta", "summa_stipendii", IDStudenta, stipendiya);

            return query;
        }

        /*
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
            int IDStudenta = 0;
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
                    if ((TableType == TableType.Sessii || TableType == TableType.Stipendiya) && ((control as ComboBox).Tag as FieldForm).FieldCaption == "Студент")
                        IDStudenta = GetValueFieldComboBox((control as ComboBox), (control as ComboBox).Tag as FieldForm);
                    
                    query += GetValueFieldComboBox((control as ComboBox), (control as ComboBox).Tag as FieldForm) + "' ";
                }
                else if (control is TextBox)
                {
                    if (TableType == TableType.Sessii && (((control as TextBox).Tag as FieldForm).FieldCaption == "Итоговая оценка" || ((control as TextBox).Tag as FieldForm).FieldCaption == "Сумма стипендии"))
                    {
                        string que = string.Format("SELECT AVG(zach_mark) AS Average FROM zacheti WHERE id_studenta = '{0}'", IDStudenta);
                        DataTable datat = Database.SelectQuery(que);
                        int avr = (int)Math.Round(Convert.ToDouble(datat.Rows[0][0].ToString()));
                        query += avr + "' ";
                    }
                    else if (TableType == TableType.Stipendiya && ((control as TextBox).Tag as FieldForm).FieldCaption == "Студент")
                    {
                        int stipendiya = 0;
                        string que = string.Format("SELECT mark_itog FROM sessii WHERE id_studenta = '{0}'", IDStudenta);
                        DataTable datat = Database.SelectQuery(que);
                        int mark = (int)Math.Round(Convert.ToDouble(datat.Rows[0][0].ToString()));
                        if (5 >= mark && mark <= 0)
                            stipendiya = 0;
                        else if (mark >= 6 && 8 <= mark)///if (avr >= 6 && 8 <= avr)
                        {
                            stipendiya = 60;
                        }
                        else if (mark >= 9 && 10 <= mark)
                        {
                            stipendiya = 70;
                        }

                        query += stipendiya + "' ";
                    }
                    else
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
        */
        private int GetValueFieldComboBox(ComboBox cb, FieldForm fieldf)  //Получаем значение ID из другой таблицы
        {
            return fieldf.IDs[cb.SelectedIndex];
        }
        private bool EmptyValueInField()
        {
            foreach (Control c in CreatedControls)
            {
                if (c is ComboBox)
                {
                    if ((c as ComboBox).SelectedIndex == -1)
                    {
                        FieldForm ff = (c as ComboBox).Tag as FieldForm;
                        MessageBox.Show("В поле '" + ff.FieldCaption + "' отсутствует значение!", "Ошибка", MessageBoxButtons.OK);
                        return true;
                    }
                }
                else if (c is TextBox)
                {
                    if ((c as TextBox).Text == "")
                    {
                        FieldForm ff = (c as TextBox).Tag as FieldForm;
                        MessageBox.Show("В поле '" + ff.FieldCaption + "' отсутствует значение!", "Ошибка", MessageBoxButtons.OK);
                        return true;
                    }
                }
            }

            return false;
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
        protected virtual void ClickButton()
        {
            if (OnClick != null)
                OnClick(this, new EventArgs());
        }
    }
}
