using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMRBT
{
    public enum EditType
    {
        Add,
        Edit
    }

    public class FieldForm
    {
        public List<string> FieldFullNames { get; set; }
        public string FieldCaption { get; set; }
        public bool HaveLink { get; set; }
        public bool Date { get; set; }
        public bool Check { get; set; }
        public bool CheckInt { get; set; }
        public EditFormTable TableField { get; set; }
        public List<int> IDs { get; set; }
        public FieldForm()
        {
            FieldFullNames = new List<string>();
        }
    }

    public class EditFormTable
    {
        public string NameForm { get; set; }  //Название формы
        public string NameTable { get; set; }  //Название таблицы
        public EditType Type { get; set; }
        public List<FieldForm> Fields { get; set; }
        public EditFormTable()
        {
            Fields = new List<FieldForm>();
        }
    }
}
