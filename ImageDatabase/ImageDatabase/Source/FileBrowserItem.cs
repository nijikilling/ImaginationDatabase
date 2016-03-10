using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImageDatabase.Source
{
    public sealed class FileBrowserItem : Panel
    {
        public int Type;
        //0 => folder
        //1 => file
        //2 => go up
        //3 => disk drive

        public string FullPath;
        public FileBrowser Owner;
        public Image Icon;

        public new Label Text;
        public PictureBox PictureBlock;

        public FileBrowserItem()
        {
            
        }

        public FileBrowserItem(int tp, string path, int size, float fsize, Image img, FileBrowser parent, bool dmode)
        {
            Type = tp;
            FullPath = path;
            Owner = parent;
            Margin = new Padding(0);
            BackgroundImage = img;
            BackgroundImageLayout = ImageLayout.None;
            Click += ClickEvent;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;

            Text = new Label();
            Text.Parent = this;
            Text.AutoSize = false;
            Text.AutoEllipsis = true;

            SetVariables(size, fsize, img, parent, dmode);
            Text.BackColor = Color.Transparent;
            if (Type < 2)
                Text.Text = Path.GetFileName(path);
            if (Type == 3)
                Text.Text = FullPath;
            Text.BringToFront();
            Text.Click += ClickEvent;
            Text.MouseEnter += OnMouseEnter;
            Text.MouseLeave += OnMouseLeave; //TRINITY!!1!
        }

        public void SetVariables(int size, float fsize, Image backimg, FileBrowser parent, bool dmode)
        {
            Width = parent.CurrentLayout.GetElementWidth();
            Height = parent.CurrentLayout.GetElementHeight();
            BackgroundImage = backimg;
            Text.Location = parent.CurrentLayout.GetLocation();
            Text.Width = parent.CurrentLayout.GetTextWidth();
            Text.Height = parent.CurrentLayout.GetTextHeight();
            Text.Font = new Font(Text.Font.FontFamily, parent.CurrentLayout.GetFontSize());
            Text.TextAlign = parent.CurrentLayout.GetAlign();
        }

        public void ClickEvent(object sender, EventArgs e)
        {
            Owner.ClickedItem(this);
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.LightGray;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            BackColor = Color.Transparent;
        }

        public void Deconstruct()
        {
            Text.Dispose();
            Dispose();
        }
    }
}
