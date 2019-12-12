using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GISApp
{
    public partial class FormRasterValue : Form
    {
        public FormRasterValue(double[] vals)
        {
            InitializeComponent();
            InitValues(vals);
        }

        private void InitValues(double[] vals)
        {
            for (int i = 1; i <= vals.Length; i++)
            {
                ListViewItem item = new ListViewItem(new String[] { "Band " + i, vals[i - 1].ToString() });
                listValue.Items.Add(item);
            }
        }

    }
}
