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
    public partial class FormBandCombination : Form
    {
        public int redIndex;
        public int greenIndex;
        public int blueIndex;
        public FormBandCombination(List<int> bandIndex)
        {
            InitializeComponent();
            InitUI(bandIndex);
        }
        private void InitUI(List<int> bandIndex)
        {
            for (int i = 0; i < bandIndex.Count; i++)
            {

                cbRed.Items.Add("Band " + (i + 1).ToString());
                cbGreen.Items.Add("Band " + (i + 1).ToString());
                cbBlue.Items.Add("Band " + (i + 1).ToString());
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            redIndex = Convert.ToInt16(cbRed.Text.Split(' ')[1])-1;
            greenIndex = Convert.ToInt16(cbGreen.Text.Split(' ')[1])-1;
            blueIndex = Convert.ToInt16(cbBlue.Text.Split(' ')[1])-1;
            this.Close();
        }
    }
}
