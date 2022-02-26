using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoReader.Window
{
    public partial class GeneralSelectListWindow : Form
    {
        public GeneralSelectListWindow()
        {
            InitializeComponent();
        }

        private List<string> _selections = new List<string>();

        public string GetSplitSections(char delimiter)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _selections.Count; i++)
            {
                builder.Append(_selections[i]);
                if (i < _selections.Count - 1)
                {
                    builder.Append(delimiter);
                }
            }
            return builder.ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Show();
            foreach (var selectedItem in chkListBox_selections.SelectedItems)
            {
                _selections.Add(selectedItem.ToString());
            }
        }
    }
}
