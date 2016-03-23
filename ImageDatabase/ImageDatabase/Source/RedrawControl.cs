using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageDatabase.Source
{
    static class RedrawControl
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

        private const int WmSetredraw = 11;
        private static bool _suspended;
        public static void SuspendDrawing(Control parent)
        {
            if (!_suspended)
            {
                SendMessage(parent.Handle, WmSetredraw, false, 0);
                _suspended = true;
            }
        }

        public static void ResumeDrawing(Control parent)
        {
            if (_suspended)
            {
                SendMessage(parent.Handle, WmSetredraw, true, 0);
                parent.Refresh();
                _suspended = false;
            }
        }
    }
}
