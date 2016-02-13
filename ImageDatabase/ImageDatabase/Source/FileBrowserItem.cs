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
    public class FileBrowserItem : Panel
    {
        public int type;
        //0 => folder
        //1 => file
        //2 => go up
        //3 => disk drive

        public string fullPath;
        public FileBrowser owner;
        public Image icon;

        new public Label Text;
        public PictureBox button;

        public FileBrowserItem()
        {
            
        }

        public FileBrowserItem(int tp, string path, int size, float fsize, Image img, FileBrowser parent, bool dmode)
        {
            this.type = tp;
            this.fullPath = path;
            this.owner = parent;
            this.Margin = new Padding(0);
            this.BackgroundImage = img;
            this.BackgroundImageLayout = ImageLayout.None;
            this.Click += ClickEvent;
            this.MouseEnter += onMouseEnter;
            this.MouseLeave += onMouseLeave;
            this.Paint += onPaint;

            Text = new Label();
            Text.Parent = this;
            Text.AutoSize = false;
            Text.AutoEllipsis = true;

            SetVariables(size, fsize, img, parent, dmode);
            Text.TextAlign = ContentAlignment.MiddleCenter;
            Text.BackColor = Color.Transparent;
            if (type < 2)
                Text.Text = Path.GetFileName(path);
            if (type == 3)
                Text.Text = fullPath;
            Text.BringToFront();
            Text.Click += ClickEvent;
            Text.MouseEnter += onMouseEnter;
            Text.MouseLeave += onMouseLeave; //TRINITY!!1!
        }

        public void SetVariables(int size, float fsize, Image backimg, FileBrowser parent, bool dmode)
        {
            this.Width = (dmode ? parent.Width : size);
            this.Height = size;
            this.BackgroundImage = backimg;
            Text.Location = new Point((dmode ? size : 0), (int)((dmode ? 0 : size * 0.6)));
            Text.Width = (dmode ? parent.Width - size : size);
            Text.Height = (dmode ? size : (int)(size * 0.4));
            Text.Font = new Font(Text.Font.FontFamily, fsize);
        }

        public void onPaint(object sender, EventArgs e)
        {
            //CreateGraphics().DrawImage(icon, 0, 0, Math.Min(Width, Height), Math.Min(Width, Height));
        }

        public void ClickEvent(object sender, EventArgs e)
        {
            owner.ClickedItem(this);
        }

        public void onMouseHover(object sender, EventArgs e)
        {
            //BackColor = Color.LightGray;
        }

        public void onMouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.LightGray;
        }

        public void onMouseLeave(object sender, EventArgs e)
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
