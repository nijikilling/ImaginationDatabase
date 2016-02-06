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
    public class FileBrowserItem : Button
    {
        public int type;
        //0 => folder
        //1 => file
        //2 => go up
        //3 => disk drive

        public string fullPath;
        public FileBrowser owner;

        new public Label Text;

        public FileBrowserItem()
        {
            
        }

        public FileBrowserItem(int tp, string path, int size, float fsize, Image img, FileBrowser parent)
        {
            type = tp;
            fullPath = path;
            Width = size;
            Height = size;
            BackgroundImage = img;
            owner = parent;
            Parent = parent;
            FlatStyle = FlatStyle.Flat;
            TextAlign = ContentAlignment.BottomCenter;
            FlatAppearance.BorderSize = 0;
            BackgroundImageLayout = ImageLayout.Stretch;

            Text = new Label();
            Text.Parent = this;
            Text.AutoSize = false;
            Text.AutoEllipsis = true;
            Text.Location = new Point(0, (int)(Height * 0.6));
            Text.Width = Width;
            Text.Height = (int)(Height * 0.4);
            Text.Font = new Font(Text.Font.FontFamily, fsize);
            Text.TextAlign = ContentAlignment.MiddleCenter;
            Text.BackColor = Color.Transparent;
            Text.Click += ClickEvent;

            if (type < 2)
                Text.Text = Path.GetFileName(path);
            if (type == 3)
                Text.Text = fullPath;
        }

        public void ClickEvent(object sender, EventArgs e)
        {
            owner.ClickedItem(this);
        }

        protected override void OnClick(EventArgs e)
        {
            owner.ClickedItem(this);
            base.OnClick(e);
        }
    }
}
