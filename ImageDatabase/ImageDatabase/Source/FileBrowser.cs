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
        public bool displayMode;
        //0 -> browsing in a win8 way
        //1 -> browsing list

        private float minFontSize = 6;
        private float maxFontSize = 32;

        private float minItemSize = 32;
        private float maxItemSize = 128;
        private float minItemListSize = 8;
        private float maxItemListSize = 32;

        private Bitmap folder;
        private Bitmap return_folder;

        public FileBrowser()
        {
            items = new List<FileBrowserItem>();
        }

        public Bitmap FolderIconWorkaround(Bitmap sampler)
        {
            return new Bitmap(sampler, new Size(Math.Min(ItemSize, ItemSize), Math.Min(ItemSize, ItemSize)));
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
            folder = FolderIconWorkaround(Properties.Resources.Folder);
            return_folder = FolderIconWorkaround(Properties.Resources.ReturnFolder);
            displayMode = false;
        }

        public void ClearBrowser()
        {
            foreach (FileBrowserItem c in items)
                c.Deconstruct();
            items = new List<FileBrowserItem>();
        }

        public void AddItem(int type, string pth, Image img)
        {
            FileBrowserItem item = new FileBrowserItem(type, pth, ItemSize, FontSize, img, this, displayMode);
            items.Add(item);
        }

        public void BuildFolder()
        {
            ClearBrowser();
            if (CurrentDirectory == null)
            {
                foreach(string dd in Directory.GetLogicalDrives())
                {
                    AddItem(3, dd, folder);
                }
                Controls.AddRange(items.ToArray());
                VerticalScroll.Value = 0;
                return;
            }
            AddItem(2, CurrentDirectory, return_folder);
            foreach (string p in Directory.GetDirectories(CurrentDirectory))
            {
                AddItem(0, p, folder);
            }
            Controls.AddRange(items.ToArray());
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
            if (!displayMode)
            {
                FontSize = fontVal * (maxFontSize - minFontSize) + minFontSize;
                ItemSize = (int)(itemVal * (maxItemSize - minItemSize) + minItemSize);
            }
            else
            {
                FontSize = fontVal * (maxFontSize - minFontSize) + minFontSize;
                ItemSize = (int)(itemVal * (maxItemListSize - minItemListSize) + minItemListSize);
            }
            folder = FolderIconWorkaround(Properties.Resources.Folder);
            return_folder = FolderIconWorkaround(Properties.Resources.ReturnFolder);
            SuspendLayout();
            foreach (FileBrowserItem item in items)
            {
                item.SetVariables(ItemSize, FontSize, (item.type == 2 ? return_folder : folder), this, displayMode);
            }
            ResumeLayout();
        }
    }
}
