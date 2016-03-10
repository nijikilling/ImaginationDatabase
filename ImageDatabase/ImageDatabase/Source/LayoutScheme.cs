using System.Drawing;
using System.Windows.Forms;

namespace ImageDatabase.Source
{
    public class LayoutScheme
    {
        public Control Parent;

        protected float MinFontSize;
        protected float MaxFontSize;

        protected float MinItemSize;
        protected float MaxItemSize;

        protected float FontSize;
        protected int ItemSize;

        public LayoutScheme()
        {

        }

        public LayoutScheme(Control parent)
        {
            Parent = parent;
        }

        public virtual Point GetLocation() { return new Point(0, 0); }
        public virtual int GetElementWidth() { return ItemSize; }
        public virtual int GetElementHeight() { return ItemSize; }
        public virtual int GetTextWidth() { return ItemSize; }
        public virtual int GetTextHeight() { return ItemSize; }
        public virtual float GetFontSize() { return FontSize; }
        public virtual ContentAlignment GetAlign() { return ContentAlignment.MiddleCenter; }
        public void RecalcCurrentItemSize(float value) { ItemSize = (int)(MinItemSize + (MaxItemSize - MinItemSize) * value); }
        public void RecalcCurrentFontSize(float value) { FontSize = (MinFontSize + (MaxFontSize - MinFontSize) * value); }
    }

    public class FlatScheme : LayoutScheme
    {
        public FlatScheme(Control parent)
        {
            Parent = parent;
            MinFontSize = 8;
            MaxFontSize = 32;
            MinItemSize = 32;
            MaxItemSize = 128;
        }

        public override Point GetLocation() { return new Point(0, (int)(0.6 * ItemSize)); }
        public override int GetElementWidth() { return ItemSize; }
        public override int GetTextWidth() { return ItemSize; }
        public override int GetTextHeight() { return (int)(0.4 * ItemSize); }

    }
    public class ListScheme : LayoutScheme
    {
        public ListScheme(Control parent)
        {
            Parent = parent;
            MinFontSize = 4;
            MaxFontSize = 32;
            MinItemSize = 8;
            MaxItemSize = 32;
        }

        public override Point GetLocation() { return new Point(ItemSize, 0); }
        public override int GetElementWidth() { return Parent.Width; }
        public override int GetTextWidth() { return Parent.Width - ItemSize; }
        public override int GetTextHeight() { return ItemSize; }
        public override ContentAlignment GetAlign() { return ContentAlignment.MiddleLeft; }
    }
}
