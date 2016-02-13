using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageDatabase
{
    public partial class Form1 : Form
    {
        //public FileBrowser fileBrowser1;
        public Form1()
        {
            InitializeComponent();
           
        }

        public void ClickedItem(FileBrowserItem item)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            fileBrowser1.CurrentDirectory = null;
            fileBrowser1.RecalcFontAndItemSize(trackBar1.Value / 100.0F, trackBar2.Value / 100.0F);
            fileBrowser1.BuildFolder();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            fileBrowser1.RecalcFontAndItemSize(trackBar1.Value / 100.0F, trackBar2.Value / 100.0F);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            fileBrowser1.RecalcFontAndItemSize(trackBar1.Value / 100.0F, trackBar2.Value / 100.0F);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.CheckState == CheckState.Checked)
                checkBox1.CheckState = CheckState.Unchecked;
            else
                checkBox1.CheckState = CheckState.Checked;
            fileBrowser1.displayMode = checkBox1.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
                checkBox2.CheckState = CheckState.Unchecked;
            else
                checkBox2.CheckState = CheckState.Checked;
            fileBrowser1.displayMode = checkBox1.Checked;
        }
    }
}
