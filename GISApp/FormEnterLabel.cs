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
    public partial class FormEnterLabel : Form
    {
        public string label;
        public FormEnterLabel(string id, string type)
        {
            InitializeComponent();
            txtID.Text = id;
            txtType.Text = type;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            label = txtLabel.Text;
            this.Close();
        }
    }
}
