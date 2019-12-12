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
    public partial class FormTableAttributes : Form
    {
        public FormTableAttributes(String[] columnNames,String[,] cellValues)
        {
             
            InitializeComponent();
            InitValues(columnNames,cellValues);
        }

        public void InitValues(String[] columnNames,String[,] cellValues)
        {
            int numberColumns = columnNames.Length;
            // Add Header Columns:
            for (int i=0; i<numberColumns;i++)
            {
                listAttributes.Columns.Add(columnNames[i],80, HorizontalAlignment.Center);
            }

            
            // Load Data
            for (int i = 0; i < cellValues.GetLength(0); i++)
            {
                String[] rowData = new String[numberColumns];
                for (int j = 0; j < cellValues.GetLength(1); j++)
                {
                    rowData[j] = cellValues[i, j];
                }
                ListViewItem item = new ListViewItem(rowData);
                listAttributes.Items.Add(item);
            }
            
        }
    }
}
