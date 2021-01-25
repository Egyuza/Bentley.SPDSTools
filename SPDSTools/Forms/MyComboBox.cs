using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SPDSTools
{
    public partial class MyComboBox : System.Windows.Forms.ComboBox
    {
        private ImageList _imageList;

        public MyComboBox()
        {
           DrawMode = DrawMode.OwnerDrawFixed;          
        }

        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }
        
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
         
            Rectangle bounds_ = e.Bounds;
            SolidBrush brush_ = new SolidBrush(e.ForeColor);

            e.DrawBackground();
            e.DrawFocusRectangle();

            try
            {
                if(e.Index != -1)
                {
                   _imageList.Draw(e.Graphics, bounds_.Left, bounds_.Top, e.Index);
                }
            }
            catch
            {
            
            }
            base.OnDrawItem(e);
        }
    }
}




