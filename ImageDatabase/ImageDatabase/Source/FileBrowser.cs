using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace ImageDatabase
{
    public class FileBrowser : FlowLayoutPanel
    {
        public string CurrentDirectory;
        public List<FileBrowserItem> items;
        public int ItemSize;
        public float FontSize;
        public int mode;
        //0 -> browsing collection
        //1 -> browsing filesystem(collections)
        //2 -> browsing filesystem(folders and files)

        private float minFontSize = 6;
        private float maxFontSize = 32;
        private float minItemSize = 32;
        private float maxItemSize = 128;

        public FileBrowser()
        {
            items = new List<FileBrowserItem>();
        }

        public FileBrowser(string initialDir, int itemSize, int mod, Form1 parent)
        {
            Parent = parent;
            Dock = DockStyle.Fill;
            CurrentDirectory = initialDir;
            items = new List<FileBrowserItem>();
            ItemSize = itemSize;
            mode = mod;
            AutoScroll = true;
        }

        public void ClearBrowser()
        {
            foreach (Control c in items)
                c.Dispose();
            items = new List<FileBrowserItem>();
        }

        public void AddItem(int type, string pth, Image img)
        {
            items.Add(new FileBrowserItem(type, pth, ItemSize, FontSize, img, this));
        }

        public void BuildFolder()
        {
            ClearBrowser();
            if (CurrentDirectory == null)
            {
                //It means that we've moved outside of disk and now must return list of disks
                foreach(string dd in Directory.GetLogicalDrives())
                {
                    AddItem(3, dd, Properties.Resources.Folder);
                }
                return;
            }
            AddItem(2, CurrentDirectory, Properties.Resources.ReturnFolder);
            foreach (string p in Directory.GetDirectories(CurrentDirectory))
            {
                AddItem(0, p, Properties.Resources.Folder);
            }
            VerticalScroll.Value = 0;
        }
        public void ClickedItem(FileBrowserItem item)
        {
            if (item.type == 0 || item.type == 3)
            {
                CurrentDirectory = item.fullPath;
                SuspendLayout();
                ClearBrowser();
                BuildFolder();
                ResumeLayout();
            }
            if (item.type == 2)
            {
                CurrentDirectory = Path.GetDirectoryName(item.fullPath);
                SuspendLayout();
                ClearBrowser();
                BuildFolder();
                ResumeLayout();
            }
        }
        public void RecalcFontAndItemSize(float fontVal, float itemVal)
        {
            FontSize = fontVal * (maxFontSize - minFontSize) + minFontSize;
            ItemSize = (int)(itemVal * (maxItemSize - minItemSize) + minItemSize);
            SuspendLayout();
            foreach (FileBrowserItem item in items)
            {
                item.Width = ItemSize;
                item.Height = ItemSize;
            }
            ResumeLayout();
            SuspendLayout();
            foreach (FileBrowserItem item in items)
            {
                item.Text.Location = new Point(0, (int)(item.Height * 0.6));
                item.Text.Width = item.Width;
                item.Text.Height = (int)(item.Height * 0.4);
                item.Text.Font = new Font(item.Font.FontFamily, FontSize);
                item.TextAlign = ContentAlignment.TopCenter;
                item.Refresh();
            }
            ResumeLayout();
        }
    }
}
