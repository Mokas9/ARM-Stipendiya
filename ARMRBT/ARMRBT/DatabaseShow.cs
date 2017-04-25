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
    public partial class DatabaseShow : Form
    {
        public DatabaseShow(TableType tableType, string nameDataBase, Database db, EditFormTable editTable, string[] names, string query)
        {
            InitializeComponent();
            _tableType = tableType;  //Таблица
            _editFormTable = editTable;  //Поля для добавления данных
            _database = db;  //Соединение с БД
            _query = query;  //Запрос
            _captionsColumns = names;  //Заголовки таблицы
            _addNewRowForm = new AddNewRowForm(tableType, _editFormTable, _database);  //Динамическое создание таблиц
            _choiceDataForm = new ChoiceDataForm(_editFormTable, _database, dataGridView1);
            
            _addNewRowForm.FillControls(panel1);
            _choiceDataForm.FillControls(panel2);

            //Подписывание обработчиков события
            _addNewRowForm.OnClick += _addNewRowForm_OnClick;
            dataGridView1.CellMouseDown += _choiceDataForm.SelectedRow;
            _choiceDataForm.OnChangedValue += _choiceDataForm_OnChangedValue;

            LoadData(_captionsColumns, _query);
            SetElements(nameDataBase);

            comboBox1.DropDownWidth = 210;

            dataGridView1.Columns[0].ReadOnly = true;
        }

        private void _choiceDataForm_OnChangedValue(object sender, EventArgs arg)
        {
            if ((sender as ComboBox).SelectedIndex == -1)
                return;

            FieldForm ff = (sender as ComboBox).Tag as FieldForm;
            string querychangedate = string.Format("UPDATE {0} SET {1} = '{2}' WHERE {3} = '{4}'", _editFormTable.NameTable, ff.FieldFullNames[0].Split('.')[1], _choiceDataForm.GetValueFieldComboBox((sender as ComboBox), ff), _dataTable.Columns[0].Caption, (int)dataGridView1.CurrentRow.Cells[0].Value);
            if (checkBox1.Checked)
            {
                DialogResult dr = MessageBox.Show("Подтвердить изменения?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    _database.Query(querychangedate); // ChoiceQueryUpdate(_tableType);
                    (sender as ComboBox).SelectedIndex = -1;
                }
            }
            else
            {
                _database.Query(querychangedate);// ChoiceQueryUpdate(_tableType);
                (sender as ComboBox).SelectedIndex = -1;
            }
            LoadDataToTable(_query);
        }

        private void _addNewRowForm_OnClick(object sender, EventArgs arg)  //Обработчик события 
        {
            LoadDataToTable(_query);
        }

        private AddNewRowForm _addNewRowForm;  //Объект для добавления данных
        private ChoiceDataForm _choiceDataForm;  //Объект для изменения данных 
        private EditFormTable _editFormTable;  //Поля
        private Database _database;  //Соединение
        private DataTable _dataTable;  //Результат выборки
        private Type _currentType;  //Тип для поиска
        private string _query;  //Запрос
        private string[] _captionsColumns;  //Заголовки таблицы
        private string _tempEditValue;  //Редактируемое значенмие
        private TableType _tableType;  //Таблица

        private void DeleteRowButton_Click(object sender, EventArgs e) //Удаление записи
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Необходимо выбрать запись!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox1.Checked)
            {
                DialogResult dr = MessageBox.Show("Вы точно хотите удалить запись?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                    _database.Query(string.Format("DELETE FROM {0} WHERE {1} = '{2}'", _editFormTable.NameTable, _dataTable.Columns[0].Caption, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
            }
            else
                _database.Query(string.Format("DELETE FROM {0} WHERE {1} = '{2}'", _editFormTable.NameTable, _dataTable.Columns[0].Caption, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
            LoadDataToTable(_query);
        }

        private void LoadData(string[] names, string query)  //Настройка DataGridView
        {
            LoadDataToTable(query);

            try
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    dataGridView1.Columns[i].HeaderText = names[i];

            }
            catch { }
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void SetElements(string nametable)  //Настройка формы и поиска по критерий
        {
            label1.Text = nametable;
            pictureBoxHeader.Controls.Add(label1);
            this.Controls.Remove(label1);
            this.WindowState = FormWindowState.Maximized;
            pictureBoxHeader.Width = this.Width;
            dateTimePickerSearch.CustomFormat = "yyyy-MMM-dd";
            foreach (string val in _captionsColumns)
                comboBox1.Items.Add(val);
        }

        #region ChangedSize
        private void DatabaseShow_Resize(object sender, EventArgs e)
        {
            pictureBoxHeader.Width = this.Width;

            dataGridView1.Width = this.Width - 380;
            dataGridView1.Height = this.Height - 120;
        }

        private void pictureBoxHeader_Resize(object sender, EventArgs e)
        {
            label1.Location = new Point(pictureBoxHeader.Width / 2 - label1.Width / 2, label1.Location.Y);
        }
        #endregion

        #region SearchCritChanged
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)  //Выбор критерия
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            DataView dv = new DataView(_dataTable);
            dataGridView1.DataSource = dv;

            if (_dataTable.Columns[comboBox1.SelectedIndex].DataType == typeof(int) || _dataTable.Columns[comboBox1.SelectedIndex].DataType == typeof(string))
            {
                dateTimePickerSearch.Visible = false;
                checkBoxSearch.Visible = false;
                textBoxSearch.Visible = true;
            }
            else if (_dataTable.Columns[comboBox1.SelectedIndex].DataType == typeof(DateTime))
            {
                dateTimePickerSearch.Visible = true;
                checkBoxSearch.Visible = false;
                textBoxSearch.Visible = false;
            }
            else
            {
                dateTimePickerSearch.Visible = false;
                checkBoxSearch.Visible = true;
                textBoxSearch.Visible = false;
            }

            _currentType = _dataTable.Columns[comboBox1.SelectedIndex].DataType;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)  //Ввод значения для поиска
        {
            DataView dv = new DataView(_dataTable);

            if (_currentType == typeof(int))
            {
                if (textBoxSearch.Text.Length == 0)
                {
                    dataGridView1.DataSource = dv;
                    return;
                }

                if (!Char.IsDigit(textBoxSearch.Text[textBoxSearch.Text.Length - 1]))
                {
                    MessageBox.Show("Должны вводиться числа!", "Ошибка!", MessageBoxButtons.OK);
                    textBoxSearch.Text = "";
                    return;
                }

                dv.RowFilter = string.Format("{0} = '{1}'", _dataTable.Columns[comboBox1.SelectedIndex].ColumnName, textBoxSearch.Text);
            }
            else
            {
                dv.RowFilter = string.Format("{0} LIKE '%{1}%'", _dataTable.Columns[comboBox1.SelectedIndex].ColumnName, textBoxSearch.Text);
            }

            dataGridView1.DataSource = dv;
        }

        private void dateTimePickerSearch_ValueChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(_dataTable);
            dv.RowFilter = string.Format("{0} = '{1}'", _dataTable.Columns[comboBox1.SelectedIndex].ColumnName, dateTimePickerSearch.Value.Year + "." + dateTimePickerSearch.Value.Month + "." + dateTimePickerSearch.Value.Day);
            dataGridView1.DataSource = dv;
        }

        private void checkBoxSearch_CheckedChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(_dataTable);
            dv.RowFilter = string.Format("{0} = '{1}'", _dataTable.Columns[comboBox1.SelectedIndex].ColumnName, checkBoxSearch.Checked);
            dataGridView1.DataSource = dv;
        }
        #endregion

        private void LoadDataToTable(string query)  //Загрузка данных в таблицу dataGridView
        {
            try
            {
                _dataTable = _database.SelectQuery(query);
                dataGridView1.DataSource = _dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
        #region CellEdit
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)  //Редактировние ячейки
        {
            _tempEditValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)  //Конец редактиирования ячейки
        {
            try
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == DBNull.Value)  //Поле пустое
                {
                    MessageBox.Show("Поле пустое!");
                    LoadDataToTable(_query);
                    return;
                }

                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == _tempEditValue)  //Значение не изменилось
                    return;

                if (checkBox1.Checked)
                {
                    DialogResult dr = MessageBox.Show("Подтвердить изменения?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == DialogResult.Yes)
                    {
                        ChoiceQueryUpdate(_tableType);
                    }
                    else
                        ChoiceQueryUpdate(_tableType);
                }
                else
                    ChoiceQueryUpdate(_tableType);

                LoadDataToTable(_query);
                _addNewRowForm.CellEndEdit();
                _choiceDataForm.CellEndEdit((int)dataGridView1.Rows[e.RowIndex].Cells[0].Value);


                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            }
            catch { };
        }
        #endregion

        private void ChoiceQueryUpdate(TableType tableType)  //ИЗМЕНИТЬ !!! Запросы на изменение данных
        {
            switch (tableType)
            {
                case TableType.Faculteti:
                    _database.Query(string.Format("UPDATE faculteti SET faculteti.facultet = '{0}' WHERE id_faculteta = {1}", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    //_database.Query(string.Format("UPDATE faculteti SET faculteti.facultet = '{0}' WHERE id_faculteta = {1}", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Gruppi:
                    _database.Query(string.Format("UPDATE faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN gruppi ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta SET gruppi.nomer_gruppi = '{0}', gruppi.kurs = '{1}', specialnosti.specialnost = '{2}', kafedri.kafedra = '{3}', faculteti.facultet = '{4}' WHERE id_gruppi = '{5}';", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    //_database.Query(string.Format("UPDATE faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN gruppi ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta SET gruppi.nomer_gruppi = '{0}', specialnosti.specialnost = '{1}', kafedri.kafedra = '{2}', faculteti.facultet = '{3}' WHERE id_gruppi = '{4}'", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Kafedri:
                    _database.Query(string.Format("UPDATE faculteti INNER JOIN kafedri ON faculteti.id_faculteta = kafedri.id_faculteta SET kafedri.kafedra = '{0}', faculteti.facultet = '{1}' WHERE id_kafedri = '{2}'", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Predmeti:
                    _database.Query(string.Format("UPDATE predmeti SET predmeti.predmet = '{0}' WHERE id_predmeta = '{1}'", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Sessii:
                    _database.Query(string.Format("UPDATE faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN (gruppi INNER JOIN (student INNER JOIN sessii ON student.id_studenta = sessii.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta SET sessii.mark_itog = '{0}', sessii.data_sdachi = '{1}', student.familiya = '{2}', student.imya = '{3}', student.otchestvo = '{4}', student.nomer_zach_knijki = '{5}', student.inogodniy = '{6}', student.adress_propiski = '{7}', student.adress_projivaniya = '{8}', gruppi.nomer_gruppi = '{9}', gruppi.kurs = '{10}', specialnosti.specialnost = '{11}', kafedri.kafedra = '{12}', faculteti.facultet = '{13}' WHERE id_sessii = '{14}';", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[9].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[10].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[11].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[12].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[13].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[14].Value, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    //_database.Query(string.Format("UPDATE sessii SET sessii.mark_itog = '{0}', sessii.data_sdachi = '{1}' WHERE id_sessii = {2}", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Specialnosti:
                    _database.Query(string.Format("UPDATE faculteti INNER JOIN (kafedri INNER JOIN specialnosti ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta SET specialnosti.specialnost = '{0}', kafedri.kafedra = '{1}', faculteti.facultet = '{2}' WHERE id_specialnosti = {3}", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Stipendiya:
                    _database.Query(string.Format("UPDATE faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN (gruppi INNER JOIN (student INNER JOIN stipendiya ON student.id_studenta = stipendiya.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta SET stipendiya.summa_stipendii = '{0}', student.familiya = '{1}', student.imya = '{2}', student.otchestvo = '{3}', student.nomer_zach_knijki = '{4}', student.inogodniy = '{5}', student.adress_propiski = '{6}', student.adress_projivaniya = '{7}', gruppi.nomer_gruppi = '{8}', gruppi.kurs = '{9}', specialnosti.specialnost = '{10}', kafedri.kafedra = '{11}', faculteti.facultet = '{12}' WHERE id_stipendii = '{13}';", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[9].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[10].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[11].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[12].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[13].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    //_database.Query(string.Format("UPDATE sessii INNER JOIN (faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN (gruppi INNER JOIN (student INNER JOIN stipendiya ON student.id_studenta = stipendiya.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta) ON sessii.id_sessii = student.id_sessii SET stipendiya.summa_stipendii = '{0}', gruppi.nomer_gruppi = '{1}', specialnosti.specialnost = '{2}', kafedri.kafedra = '{3}', faculteti.facultet = '{4}', sessii.mark_itog = '{5}', sessii.data_sdachi = '{6}', student.familiya = '{7}', student.imya = '{8}', student.otchestvo = '{9}', student.nomer_zach_knijki = '{10}', student.inogodniy = '{11}', student.adress_propiski = '{12}', student.adress_projivaniya = '{13}' WHERE id_stipendii = '{14}'", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[9].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[10].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[11].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[12].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[13].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[14].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Student:
                    _database.Query(string.Format("UPDATE faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN (gruppi INNER JOIN student ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta SET student.familiya = '{0}', student.imya = '{1}', student.otchestvo = '{2}', student.nomer_zach_knijki = '{3}', student.inogodniy = '{4}', student.adress_propiski = '{5}', student.adress_projivaniya = '{6}', gruppi.nomer_gruppi = '{7}', gruppi.kurs = '{8}', specialnosti.specialnost = '{9}', kafedri.kafedra = '{10}', faculteti.facultet = '{11}' WHERE id_studenta = '{12}';", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[9].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[10].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[11].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[12].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    //_database.Query(string.Format("UPDATE sessii INNER JOIN (faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN (gruppi INNER JOIN student ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta) ON sessii.id_sessii = student.id_sessii SET student.familiya = '{0}', student.imya = '{1}', student.otchestvo = '{2}', student.nomer_zach_knijki = '{3}', student.inogodniy = '{4}', student.adress_propiski = '{5}', student.adress_projivaniya = '{6}', gruppi.nomer_gruppi = '{7}', specialnosti.specialnost = '{8}', kafedri.kafedra = '{9}', faculteti.facultet = '{10}', sessii.mark_itog = '{11}', sessii.data_sdachi = '{12}' WHERE id_studenta = '{13}'", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[9].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[10].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[11].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[12].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
                case TableType.Zacheti:
                    _database.Query(string.Format("UPDATE (faculteti INNER JOIN kafedri ON faculteti.id_faculteta = kafedri.id_faculteta) INNER JOIN (specialnosti INNER JOIN (gruppi INNER JOIN (student INNER JOIN (predmeti INNER JOIN zacheti ON predmeti.id_predmeta = zacheti.id_predmeta) ON student.id_studenta = zacheti.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri SET zacheti.zach_mark = '{0}', predmeti.predmet = '{1}', student.familiya = '{2}', student.imya = '{3}', student.otchestvo = '{4}', student.nomer_zach_knijki = '{5}', student.inogodniy = '{6}', student.adress_propiski = '{7}', student.adress_projivaniya = '{8}', gruppi.nomer_gruppi = '{9}', gruppi.kurs = '{10}', specialnosti.specialnost = '{11}', kafedri.kafedra = '{12}', faculteti.facultet = '{13}' WHERE id_zacheta = '{14}'; ", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[9].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[10].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[11].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[12].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[13].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[14].Value, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    //_database.Query(string.Format("UPDATE sessii INNER JOIN (faculteti INNER JOIN (kafedri INNER JOIN (specialnosti INNER JOIN (gruppi INNER JOIN (student INNER JOIN (predmeti INNER JOIN zacheti ON predmeti.id_predmeta = zacheti.id_predmeta) ON student.id_studenta = zacheti.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta) ON sessii.id_sessii = student.id_sessii SET zacheti.zach_mark = '{0}', predmeti.predmet = '{1}', student.familiya = '{2}', student.imya = '{3}', student.otchestvo = '{4}', student.nomer_zach_knijki = '{5}', student.inogodniy = '{6}', student.adress_propiski = '{7}', student.adress_projivaniya = '{8}', gruppi.nomer_gruppi = '{9}', specialnosti.specialnost = '{10}', kafedri.kafedra = '{11}', faculteti.facultet = '{12}', sessii.mark_itog = '{13}', sessii.data_sdachi = '{14}' WHERE id_zacheta = '{15}'", dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[9].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[10].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[11].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[12].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[13].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[14].Value, dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[15].Value, (int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value));
                    break;
            }
        }
    }
}