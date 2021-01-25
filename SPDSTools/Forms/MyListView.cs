using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPDSTools
{
    public partial class MyListView : ListView
    {
        public MyListView()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref Message msg)
        {
            // Look for the WM_VSCROLL or the WM_HSCROLL messages.
            if ((msg.Msg == WM_VSCROLL) || (msg.Msg == WM_HSCROLL) || msg.Msg == 0x020A)
            {
                // Move focus to the ListView to cause ComboBox to lose focus.
                this.Focus();
            }

            // Pass message to default handler.
            base.WndProc(ref msg);
        }
    }
}
