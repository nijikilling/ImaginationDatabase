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
            //Database db = Database.Load("test.idb");
            //db.AddPicture("cursor2.png");
            //db.Save();
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
            _imageExtensions = imageExtensions;
            AutoScroll = true;
            _folderPicture = FolderIconWorkaround(Resources.Folder);
            _returnFolder = FolderIconWorkaround(Resources.ReturnFolder);
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

        private void AddItem(ItemType type, string pth, Bitmap img)
        {
            FileBrowserItem item = new FileBrowserItem(type, pth, img, this);
            _items.Add(item);
        }

        private void BuildCollection()
        {
            Database db = Database.Load(CurrentDirectory);
            foreach (Element el in db.Data)
            {
                el.GetPictureData(db.CacheFolder);
                AddItem(ItemType.File, el.Identifier, el.picture);
            }
            // ReSharper disable once CoVariantArrayConversion
            Controls.AddRange(_items.ToArray());
            VerticalScroll.Value = 0;
            UpdateContent();
        }

        public void BuildFolder()
        {
            //ToDo make async - very slow, especially on big folders. 
            ClearBrowser();
            if (CurrentDirectory == null) //if should write disks
            {
                foreach(var dd in Directory.GetLogicalDrives())
                {
                    AddItem(ItemType.Drive, dd, _folderPicture);
                }
                // ReSharper disable once CoVariantArrayConversion
                Controls.AddRange(_items.ToArray());
                VerticalScroll.Value = 0;
                return;
            }
            AddItem(ItemType.Return, CurrentDirectory, _returnFolder);
            foreach (string p in Directory.GetDirectories(CurrentDirectory))
            {
                AddItem(ItemType.Folder, p, _folderPicture);
            }
            foreach (string p in Directory.GetFiles(CurrentDirectory))
            {
                if (IsImageFile(p))
                    AddItem(ItemType.File, p, new Bitmap(p));
                if (IsCollectionFile(p))
                    AddItem(ItemType.Collection, p, Resources.Collection);
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
            if (item.Type == ItemType.Folder || item.Type == ItemType.Drive)
            {
                CurrentDirectory = item.FullPath;
                ClearBrowser();
                BuildFolder();
            }
            else if (item.Type == ItemType.Return)
            {
                CurrentDirectory = Path.GetDirectoryName(item.FullPath);
                ClearBrowser();
                BuildFolder();
            }
            else if (item.Type == ItemType.Collection)
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
            UpdateContent();
        }
        public void UpdateContent()
        { 
            RedrawControl.SuspendDrawing(this);
            CurrentLayout = DisplayMode ? _layoutList : _layoutFlat;
            CurrentLayout.RecalcCurrentFontSize(_fv);
            CurrentLayout.RecalcCurrentItemSize(_iv);
            _folderPicture = Resources.Folder;
            _returnFolder = Resources.ReturnFolder;
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
