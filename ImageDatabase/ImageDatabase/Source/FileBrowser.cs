using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ImageDatabase.Layout;
using ImageDatabase.Properties;

namespace ImageDatabase.Source
{
    public sealed class FileBrowser : FlowLayoutPanel
    {
        public string CurrentDirectory;
        private List<FileBrowserItem> _items;
        private int _itemSize;
        private float _fontSize;
        private LayoutScheme _layoutFlat;
        private LayoutScheme _layoutList;
        public LayoutScheme CurrentLayout;
        //0 -> browsing collection
        //1 -> browsing filesystem(collections)
        //2 -> browsing filesystem(folders and files)
        public bool DisplayMode;
        //0 -> browsing in a win8 way
        //1 -> browsing LayoutList

        private Bitmap _folderPicture;
        private Bitmap _returnFolder;

        private float _fv, _iv;
        private readonly HashSet<string> _imageExtensions;

        public FileBrowser()
        {
            _imageExtensions = new HashSet<string> {".bmp", ".gif", ".png", ".jpg", ".jpeg"};
            _items = new List<FileBrowserItem>();
        }

        private Bitmap FolderIconWorkaround(Bitmap sampler)
        {
            double shrinkCoeff = Math.Max(sampler.Width, sampler.Height) * 1.0F / CurrentLayout.GetElementHeight();
            return new Bitmap(sampler, new Size(Convert.ToInt32(sampler.Width / shrinkCoeff), Convert.ToInt32(sampler.Height / shrinkCoeff)));
        } 

        public FileBrowser(string initialDir, int itemSize, Form1 parent, HashSet<string> imageExtensions)
        {
            Parent = parent;
            Dock = DockStyle.Fill;
            CurrentDirectory = initialDir;
            _items = new List<FileBrowserItem>();
            _itemSize = itemSize;
            _imageExtensions = imageExtensions;
            AutoScroll = true;
            _folderPicture = FolderIconWorkaround(Properties.Resources.Folder);
            _returnFolder = FolderIconWorkaround(Properties.Resources.ReturnFolder);
            _layoutFlat = new FlatScheme(this);
            _layoutList = new ListScheme(this);
            CurrentLayout = _layoutFlat;
        }

        private void ClearBrowser()
        {
            Debug.Assert(_items != null, "Items != null");
            foreach (FileBrowserItem c in _items)
                c.Deconstruct();
            _items = new List<FileBrowserItem>();
        }

        private void AddItem(int type, string pth, Bitmap img)
        {
            FileBrowserItem item = new FileBrowserItem(type, pth, _itemSize, _fontSize, img, this, DisplayMode);
            _items.Add(item);
        }

        private void BuildCollection()
        {
            Database db = Database.Load(CurrentDirectory);
            
        }

        public void BuildFolder()
        {
            //ToDo make async - very slow, especially on big folders. 
            ClearBrowser();
            if (CurrentDirectory == null) //if should write disks
            {
                foreach(var dd in Directory.GetLogicalDrives())
                {
                    AddItem(3, dd, _folderPicture);
                }
                // ReSharper disable once CoVariantArrayConversion
                Controls.AddRange(_items.ToArray());
                VerticalScroll.Value = 0;
                return;
            }
            AddItem(2, CurrentDirectory, _returnFolder);
            foreach (string p in Directory.GetDirectories(CurrentDirectory))
            {
                AddItem(0, p, _folderPicture);
            }
            foreach (string p in Directory.GetFiles(CurrentDirectory))
            {
                if (IsImageFile(p))
                    AddItem(1, p, new Bitmap(p));
                if (IsCollectionFile(p))
                    AddItem(4, p, Resources.Collection);
            }
            // ReSharper disable once CoVariantArrayConversion
            Controls.AddRange(_items.ToArray());
            VerticalScroll.Value = 0;
            UpdateContent();
        }

        private bool IsCollectionFile(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            return ext == ".idb";
        }

        private bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            return _imageExtensions.Contains(ext);
        }

        public void ClickedItem(FileBrowserItem item)
        {
            RedrawControl.SuspendDrawing(this);
            SuspendLayout();
            if (item.Type == 0 || item.Type == 3)
            {
                CurrentDirectory = item.FullPath;
                ClearBrowser();
                BuildFolder();
            }
            else if (item.Type == 2)
            {
                CurrentDirectory = Path.GetDirectoryName(item.FullPath);
                ClearBrowser();
                BuildFolder();
            }
            else if (item.Type == 4)
            {
                CurrentDirectory = item.FullPath;
                ClearBrowser();
                BuildCollection();
            }
            ResumeLayout();
            RedrawControl.ResumeDrawing(this);
        }
        public void RecalcFontAndItemSize(float fontVal, float itemVal)
        {
            _fv = fontVal;
            _iv = itemVal;
            _layoutFlat = new FlatScheme(this);
            _layoutList = new ListScheme(this);
            Database db = Database.Load("test.txt");
            db.AddPicture("cursor2.png");
            db.Save();
            UpdateContent();
        }
        public void UpdateContent()
        { 
            RedrawControl.SuspendDrawing(this);
            CurrentLayout = DisplayMode ? _layoutList : _layoutFlat;
            CurrentLayout.RecalcCurrentFontSize(_fv);
            CurrentLayout.RecalcCurrentItemSize(_iv);
            _folderPicture = Properties.Resources.Folder;
            _returnFolder = Properties.Resources.ReturnFolder;
            SuspendLayout();
            foreach (FileBrowserItem item in _items)
            {
                item.SetVariables(FolderIconWorkaround(item.FullImage), this);
            }
            ResumeLayout();
            RedrawControl.ResumeDrawing(this);
        }
    }
}
