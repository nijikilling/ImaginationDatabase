using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ImageDatabase.Layout;

namespace ImageDatabase.Source
{
    public sealed class FileBrowser : FlowLayoutPanel
    {
        public string CurrentDirectory;
        public List<FileBrowserItem> Items;
        public int ItemSize;
        public float FontSize;
        public int Mode;
        public LayoutScheme LayoutFlat, LayoutList, CurrentLayout;
        //0 -> browsing collection
        //1 -> browsing filesystem(collections)
        //2 -> browsing filesystem(folders and files)
        public bool DisplayMode;
        //0 -> browsing in a win8 way
        //1 -> browsing LayoutList

        private Bitmap _folderPicture;
        private Bitmap _returnFolder;

        private float _fv, _iv;

        public FileBrowser()
        {
            Items = new List<FileBrowserItem>();
        }

        public Bitmap FolderIconWorkaround(Bitmap sampler)
        {
            return new Bitmap(sampler, new Size(CurrentLayout.GetElementHeight(), CurrentLayout.GetElementHeight()));
        } 

        public FileBrowser(string initialDir, int itemSize, int mod, Form1 parent)
        {
            Parent = parent;
            Dock = DockStyle.Fill;
            CurrentDirectory = initialDir;
            Items = new List<FileBrowserItem>();
            ItemSize = itemSize;
            Mode = mod;
            AutoScroll = true;
            _folderPicture = FolderIconWorkaround(Properties.Resources.Folder);
            _returnFolder = FolderIconWorkaround(Properties.Resources.ReturnFolder);
            LayoutFlat = new FlatScheme(this);
            LayoutList = new ListScheme(this);
            CurrentLayout = LayoutFlat;
        }

        public void ClearBrowser()
        {
            Debug.Assert(Items != null, "Items != null");
            foreach (FileBrowserItem c in Items)
                c.Deconstruct();
            Items = new List<FileBrowserItem>();
        }

        public void AddItem(int type, string pth, Image img)
        {
            FileBrowserItem item = new FileBrowserItem(type, pth, ItemSize, FontSize, img, this, DisplayMode);
            Items.Add(item);
        }

        public void BuildFolder()
        {
            ClearBrowser();
            if (CurrentDirectory == null)
            {
                foreach(string dd in Directory.GetLogicalDrives())
                {
                    AddItem(3, dd, _folderPicture);
                }
                // ReSharper disable once CoVariantArrayConversion
                Controls.AddRange(Items.ToArray());
                VerticalScroll.Value = 0;
                return;
            }
            AddItem(2, CurrentDirectory, _returnFolder);
            foreach (string p in Directory.GetDirectories(CurrentDirectory))
            {
                AddItem(0, p, _folderPicture);
            }
            // ReSharper disable once CoVariantArrayConversion
            Controls.AddRange(Items.ToArray());
            VerticalScroll.Value = 0;
            UpdateContent();
        }
        public void ClickedItem(FileBrowserItem item)
        {
            if (item.Type == 0 || item.Type == 3)
            {
                CurrentDirectory = item.FullPath;
                SuspendLayout();
                ClearBrowser();
                BuildFolder();
                ResumeLayout();
            }
            if (item.Type == 2)
            {
                CurrentDirectory = Path.GetDirectoryName(item.FullPath);
                SuspendLayout();
                ClearBrowser();
                BuildFolder();
                ResumeLayout();
            }
        }
        public void RecalcFontAndItemSize(float fontVal, float itemVal)
        {
            _fv = fontVal;
            _iv = itemVal;
            LayoutFlat = new FlatScheme(this);
            LayoutList = new ListScheme(this);
            Database db = Database.Load("test.txt");
            db.AddPicture("cursor2.png");
            db.Save();
            UpdateContent();
        }
        public void UpdateContent()
        { 
            CurrentLayout = (DisplayMode ? LayoutList : LayoutFlat);
            CurrentLayout.RecalcCurrentFontSize(_fv);
            CurrentLayout.RecalcCurrentItemSize(_iv);
            _folderPicture = FolderIconWorkaround(Properties.Resources.Folder);
            _returnFolder = FolderIconWorkaround(Properties.Resources.ReturnFolder);
            SuspendLayout();
            foreach (FileBrowserItem item in Items)
            {
                item.SetVariables(ItemSize, FontSize, (item.Type == 2 ? _returnFolder : _folderPicture), this, DisplayMode);
            }
            ResumeLayout();
        }
    }
}
