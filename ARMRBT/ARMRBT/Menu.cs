using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace ARMRBT
{
    public partial class Menu : Form
    {
        public Menu(Authorization auth)
        {
            InitializeComponent();
            this.auth = auth;
            auth.Hide();
            CreateFormsForTables();
        }

        private Authorization auth;

        ///ДЛЯ ДОБАВЛЕНИЯ И РЕДАКТИРОВАНИЯ///
        EditFormTable eftStipendiya;
        EditFormTable eftSessii;
        EditFormTable eftStudents;
        EditFormTable eftZacheti;
        EditFormTable eftGruppi;
        EditFormTable eftPredmeti;
        EditFormTable eftSpecialnosti;
        EditFormTable eftKafedri;
        EditFormTable eftFaculteti;
        ////////////////////////////////////


        private void Menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            auth.Close();
        }

        public void CreateFormsForTables()
        {
            eftStipendiya = new EditFormTable() { NameForm = "стипендии", NameTable = "stipendiya" };
            eftSessii = new EditFormTable() { NameForm = "сессии", NameTable = "sessii" };
            eftStudents = new EditFormTable() { NameForm = "студента", NameTable = "student" };
            eftZacheti = new EditFormTable() { NameForm = "зачёта", NameTable = "zacheti" };
            eftGruppi = new EditFormTable() { NameForm = "группы", NameTable = "gruppi" };
            eftPredmeti = new EditFormTable() { NameForm = "предмета", NameTable = "predmeti" };
            eftSpecialnosti = new EditFormTable() { NameForm = "специальности", NameTable = "specialnosti" };
            eftKafedri = new EditFormTable() { NameForm = "кафедры", NameTable = "kafedri" };
            eftFaculteti = new EditFormTable() { NameForm = "факультета", NameTable = "faculteti" };


            ////////////////Стипендии/////////////////
            eftStipendiya.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.id_studenta", "student.familiya", "student.imya" }, FieldCaption = "Студент", HaveLink = true, IDs = new List<int>()});
            //eftStipendiya.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "stipendiya.summa_stipendii"}, FieldCaption = "Сумма стипендии"});

            /////////////////СЕССИИ///////////////////
            eftSessii.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.id_studenta", "student.familiya", "student.imya" }, FieldCaption = "Студент", HaveLink = true, IDs = new List<int>() });
            //eftSessii.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "sessii.mark_itog" }, FieldCaption = "Итоговая оценка" });
            eftSessii.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "sessii.data_sdachi" }, FieldCaption = "Дата сдачи" });
            //////////////СТУДЕНТЫ/////////////
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "gruppi.id_gruppi", "gruppi.nomer_gruppi" }, FieldCaption = "Группа", HaveLink = true, IDs = new List<int>() });
            //eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "sessii.id_sessii", "sessii.data_sdachi" }, FieldCaption = "Сессия", HaveLink = true, IDs = new List<int>() });
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.familiya" }, FieldCaption = "Фамилия" });
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.imya" }, FieldCaption = "Имя" });
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.otchestvo" }, FieldCaption = "Отчество" });
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.nomer_zach_knijki" }, FieldCaption = "Номер зач.книжки" });
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.inogodniy" }, FieldCaption = "Иногородний" });
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.adress_propiski" }, FieldCaption = "Адрес прописки" });
            eftStudents.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.adress_projivaniya" }, FieldCaption = "Адрес проживания" });
            ////////////////ЗАЧЁТЫ////////////////
            eftZacheti.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "student.id_studenta", "student.familiya", "student.imya" }, FieldCaption = "Студент", HaveLink = true, IDs = new List<int>() });
            eftZacheti.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "predmeti.id_predmeta", "predmeti.predmet" }, FieldCaption = "Предмет", HaveLink = true, IDs = new List<int>() });            
            eftZacheti.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "zacheti.zach_mark" }, FieldCaption = "Оценка" });
            ////////////////Группы//////////////
            eftGruppi.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "specialnosti.id_specialnosti", "specialnosti.specialnost" }, FieldCaption = "Специальность", HaveLink = true, IDs = new List<int>() });
            eftGruppi.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "gruppi.nomer_gruppi" }, FieldCaption = "Номер группы" });
            eftGruppi.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "gruppi.kurs" }, FieldCaption = "Курс" });
            ////////////////Предметы//////////////
            eftPredmeti.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "predmeti.predmet"}, FieldCaption = "Предмет" });
            ///////////////////Специальности//////////////////
            eftSpecialnosti.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "kafedri.id_kafedri", "kafedri.kafedra" }, FieldCaption = "Кафедра", HaveLink = true, IDs = new List<int>() });
            eftSpecialnosti.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "specialnosti.specialnost" }, FieldCaption = "Специальность" });
            ////////////////Кафедры///////////////
            eftKafedri.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "faculteti.id_faculteta", "faculteti.facultet" }, FieldCaption = "Факультет", HaveLink = true, IDs = new List<int>() });
            eftKafedri.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "kafedri.kafedra" }, FieldCaption = "Кафедра" });
            ////////////////Факультеты///////////////
            eftFaculteti.Fields.Add(new FieldForm() { FieldFullNames = new List<string>() { "faculteti.facultet" }, FieldCaption = "Факультет" });
        }




        private void button10_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "Help.chm");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] names = {"ID Сессии", "Итоговая оценка", "Дата сдачи", "Фамилия", "Имя", "Отчество", "Номер зачётной книжки", "Иногородний", "Адрес прописки", "Адрес проживания", "Номер группы", "Курс", "Специальность", "Кафедра", "Факультет" };
            new DatabaseShow(TableType.Sessii ,"Сессии", auth.database, eftSessii, names, "SELECT sessii.id_sessii, sessii.mark_itog, sessii.data_sdachi, student.familiya, student.imya, student.otchestvo, student.nomer_zach_knijki, student.inogodniy, student.adress_propiski, student.adress_projivaniya, gruppi.nomer_gruppi, gruppi.kurs, specialnosti.specialnost, kafedri.kafedra, faculteti.facultet FROM faculteti INNER JOIN(kafedri INNER JOIN(specialnosti INNER JOIN(gruppi INNER JOIN(student INNER JOIN sessii ON student.id_studenta = sessii.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta; ").ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] names = { "ID Стипендии", "Сумма стипендии", "Фамилия", "Имя", "Отчество", "Номер зачетной книжки", "Иногородний", "Адрес прописки", "Адрес проживания", "Группа", "Курс", "Специальность", "Кафедра", "Факультет"};
            new DatabaseShow(TableType.Stipendiya,"Стипендии", auth.database, eftStipendiya, names, "SELECT stipendiya.id_stipendii, stipendiya.summa_stipendii, student.familiya, student.imya, student.otchestvo, student.nomer_zach_knijki, student.inogodniy, student.adress_propiski, student.adress_projivaniya, gruppi.nomer_gruppi, gruppi.kurs, specialnosti.specialnost, kafedri.kafedra, faculteti.facultet FROM faculteti INNER JOIN(kafedri INNER JOIN(specialnosti INNER JOIN(gruppi INNER JOIN(student INNER JOIN stipendiya ON student.id_studenta = stipendiya.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta; ").ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] names = { "ID Студента", "Фамилия", "Имя", "Отчество", "Номер зачётной книжки", "Иногородний", "Адрес прописки", "Адрес проживания", "Номер группы", "Курс", "Специальность", "Кафедра", "Факультет"};
            new DatabaseShow(TableType.Student, "Студенты", auth.database, eftStudents, names, "SELECT student.id_studenta, student.familiya, student.imya, student.otchestvo, student.nomer_zach_knijki, student.inogodniy, student.adress_propiski, student.adress_projivaniya, gruppi.nomer_gruppi, gruppi.kurs, specialnosti.specialnost, kafedri.kafedra, faculteti.facultet FROM faculteti INNER JOIN(kafedri INNER JOIN(specialnosti INNER JOIN(gruppi INNER JOIN student ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta; ").ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] names = { "ID Группы", "Номер группы", "Курс", "Специальность", "Кафедра", "Факультет"};
            new DatabaseShow(TableType.Gruppi, "Группы", auth.database, eftGruppi, names, "SELECT gruppi.id_gruppi, gruppi.nomer_gruppi, gruppi.kurs, specialnosti.specialnost, kafedri.kafedra, faculteti.facultet FROM faculteti INNER JOIN(kafedri INNER JOIN(specialnosti INNER JOIN gruppi ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta; ").ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string[] names = { "ID Кафедры", "Кафедра", "Факультет"};
            new DatabaseShow(TableType.Kafedri, "Кафедры", auth.database, eftKafedri, names, "SELECT kafedri.id_kafedri, kafedri.kafedra, faculteti.facultet FROM faculteti INNER JOIN kafedri ON faculteti.id_faculteta = kafedri.id_faculteta; ").ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] names = { "ID Зачёты", "Оценка зачёт", "Предмет", "Фамилия", "Имя", "Отчество", "Номер зачётной книжки", "Иногородний", "Адрес прописки", "Адрес проживания", "Номер группы", "Курс", "Специальность", "Кафедра", "Факультет"};
            new DatabaseShow(TableType.Zacheti, "Зачёты", auth.database, eftZacheti, names, "SELECT zacheti.id_zacheta, zacheti.zach_mark, predmeti.predmet, student.familiya, student.imya, student.otchestvo, student.nomer_zach_knijki, student.inogodniy, student.adress_propiski, student.adress_projivaniya, gruppi.nomer_gruppi, gruppi.kurs, specialnosti.specialnost, kafedri.kafedra, faculteti.facultet FROM predmeti INNER JOIN(faculteti INNER JOIN(kafedri INNER JOIN(specialnosti INNER JOIN(gruppi INNER JOIN(student INNER JOIN zacheti ON student.id_studenta = zacheti.id_studenta) ON gruppi.id_gruppi = student.id_gruppi) ON specialnosti.id_specialnosti = gruppi.id_specialnosti) ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta) ON predmeti.id_predmeta = zacheti.id_predmeta; ").ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string[] names = {"ID Факультета", "Факультет"};
            new DatabaseShow(TableType.Faculteti, "Факультеты", auth.database, eftFaculteti, names, "SELECT * FROM faculteti ").ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string[] names = {"ID Предметы", "Предмет"};
            new DatabaseShow(TableType.Predmeti, "Предметы", auth.database, eftPredmeti, names, "SELECT * FROM predmeti ").ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string[] names = {"ID Специальности", "Специальность", "Кафедра", "Факультет"};
            new DatabaseShow(TableType.Specialnosti, "Специальности", auth.database, eftSpecialnosti, names, "SELECT specialnosti.id_specialnosti, specialnosti.specialnost, kafedri.kafedra, faculteti.facultet FROM faculteti INNER JOIN(kafedri INNER JOIN specialnosti ON kafedri.id_kafedri = specialnosti.id_kafedri) ON faculteti.id_faculteta = kafedri.id_faculteta; ").ShowDialog();
        }
    }
}
