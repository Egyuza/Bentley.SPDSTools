using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCOM = Bentley.Interop.MicroStationDGN;
using BMI = Bentley.MstnPlatformNET.InteropServices;

namespace SPDSTools
{
public partial class LeaderToolControl : UserControl, IToolControl
{
    private ListViewItem lvItem;
    public Dictionary<string, string> m_dictionary;
    public long m_numPoint;
    public long idElem;
    public long idModelElem;
    public int typeOrient;
    public int typeLeader;       
    public int m_typePlaceLeader;
    public List<BCOM.Element> m_elemDelete = new List<BCOM.Element>();  
    public int flagSelect;
    //public static PlaceLeaderForm s_current;
    //public LeaderData sdata = new LeaderData();  

    public LeaderToolControl()
    {                     
        m_dictionary = new Dictionary<string, string>();
        typeLeader = 0;
        typeOrient = 0;
        InitializeComponent();

        cbxOrientation.SelectedIndex = 0;
        m_numPoint = 0;
        m_typePlaceLeader = 0;
            
        flagSelect = 1;
        Stream _imageStream;
        Assembly _assembly = Assembly.GetExecutingAssembly();

        // список указателей:
        {
            cbxPointer.ImageList = imageList1;
            for (int i = 1; i <= 7; i++)
            {
                _imageStream = _assembly.GetManifestResourceStream(
                    string.Format("ManagedReportsTools.line{0}1.bmp", i));
                imageList1.Images.Add(Image.FromStream(_imageStream));
                cbxPointer.Items.Add("");
            }
            cbxPointer.SelectedIndex = 0;
        }

        // список типов выносок:
        cbxLeaderType.ImageList = imageList2;
        for (int i = 1; i <= 4; i++)
        {
            _imageStream = _assembly.GetManifestResourceStream(
                string.Format("ManagedReportsTools.res.type{0}.bmp", i));
            imageList2.Images.Add(Image.FromStream(_imageStream));
            cbxLeaderType.Items.Add("");
        }
        cbxLeaderType.SelectedIndex = 0;

        BCOM.Application m_app;
        m_app = BMI.Utilities.ComApp;

        cbxTextStyle.Items.Add("Стиль(нет)");


        Bentley.DgnPlatformNET.DgnFile m_dgnFile = Bentley.MstnPlatformNET.Session.Instance.GetActiveDgnFile();
        TextStyleCollection collTextStyle = m_dgnFile.GetTextStyles();
        IEnumerator<DgnTextStyle> ee = collTextStyle.GetEnumerator();
        while (ee.MoveNext())
        {              
            cbxTextStyle.Items.Add(ee.Current.Name);
        }

        DgnTextStyle textStyle = DgnTextStyle.GetSettings(m_dgnFile);
        cbxTextStyle.Text = textStyle.Name;
        if (textStyle.Name == "")
            cbxTextStyle.Text = "Стиль(нет)";

        this.Text = "Выноски";

        cbxLeaderType.Visible = true;
        cbxLeaderType.Visible = true;


        this.lvText.View = View.Details;
          
        this.lvText.FullRowSelect = true;

       
        ColumnHeader columnheader;
        ListViewItem lvitem;

        columnheader = new ColumnHeader();
        columnheader.Text = "Текст";
        columnheader.Width = -2;

        this.lvText.Columns.Add(columnheader);

        string str = "";
        lvitem = new ListViewItem(str);
        this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);

        //lvitem = new ListViewItem(str);
        //this.lvText.Items.Add(lvitem);
    }


    internal static void CloseForm()
    {
        //if (s_current == null)
        //    return;

        //s_current = null;

    }

    private void cbListViewCombo_SelectedValueChanged(object sender, EventArgs e)
    {
        // Set text of ListView item to match the ComboBox.
        lvItem.SubItems[0].Text = this.cbListViewCombo.Text;

        // Hide the ComboBox.
        this.cbListViewCombo.Visible = false;
    }

    private void cbListViewCombo_KeyPress(object sender, KeyPressEventArgs e)
    {
        switch (e.KeyChar)
        {
            case (char)(int)Keys.Escape:
                {
                    // Reset the original text value, and then hide the ComboBox.
                    this.cbListViewCombo.Text = lvItem.SubItems[0].Text;
                    this.cbListViewCombo.Visible = false;
                    break;
                }

            case (char)(int)Keys.Enter:
                {
                    // Hide the ComboBox.
                    this.cbListViewCombo.Visible = false;
                    break;
                }
        }
    }

    private void cbListViewCombo_Leave(object sender, EventArgs e)
    {
        // Set text of ListView item to match the ComboBox.
        lvItem.SubItems[0].Text = this.cbListViewCombo.Text;

        if (cbListViewCombo.Text.Length > 0)
        {
            if(this.cbListViewCombo.Items.Contains(cbListViewCombo.Text) == false)
            {
                this.cbListViewCombo.Items.Add(cbListViewCombo.Text);
            }
        }

        // Hide the ComboBox.
        this.cbListViewCombo.Visible = false;
    }

    private void button1_Click(object sender, EventArgs e)
    {

        m_dictionary.Clear();
        // SelectComponentTool.InstallNewInstance(this);            
      
    }

    private void m_comboBoxListParam_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cm = (ComboBox)sender;
         
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        typeOrient = cbxOrientation.SelectedIndex;
    }

    private void m_comboBoxArr_SelectedIndexChanged(object sender, EventArgs e)
    {
        typeLeader = cbxPointer.SelectedIndex;
    }

    private void AnnotationForm_FormClosed(object sender, FormClosedEventArgs e)
    {
           
        //if (s_current == null)
        //    return;
       
        ////s_current.Close();
        //s_current = null;
    }

    private void myListView_MouseUp(object sender, MouseEventArgs e)
    {
        // Get the item on the row that is clicked.
        lvItem = this.lvText.GetItemAt(e.X, e.Y);

        // Make sure that an item is clicked.
        if (lvItem != null)
        {
                                  
            // Get the bounds of the item that is clicked.
            Rectangle ClickedItem = lvItem.Bounds;

            //ClickedItem.X = this.myListView.Columns[0].Width;

            // Verify that the column is completely scrolled off to the left.
            if ((ClickedItem.Left + this.lvText.Columns[0].Width) < 0)
            {                    
                return;
            }
            else 
            {
                ClickedItem.Width = this.lvText.Width - 2;
                ClickedItem.X = 2;
            }

            // Adjust the top to account for the location of the ListView.
            ClickedItem.Y += this.lvText.Top;
            ClickedItem.X += this.lvText.Left;

            // Assign calculated bounds to the ComboBox.
            this.cbListViewCombo.Bounds = ClickedItem;

            // Set default text for ComboBox to match the item that is clicked.
            this.cbListViewCombo.Text = lvItem.SubItems[0].Text;

            // Display the ComboBox, and make sure that it is on top with focus.
            this.cbListViewCombo.Visible = true;
            this.cbListViewCombo.BringToFront();
            this.cbListViewCombo.Focus();
        }
    }

    private void m_ComboBoxType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (flagSelect == 1)
        {
            m_typePlaceLeader = cbxLeaderType.SelectedIndex;
            BCOM.Application m_app;
            m_app = BMI.Utilities.ComApp;

            if (m_numPoint > 1)
            {
                BCOM.Element elem;
                for (int i = 0; i < m_elemDelete.Count; i++)
                {
                    elem = (BCOM.Element)m_elemDelete[i];
                    m_app.ActiveModelReference.AddElement(elem);
                }

                m_app.CommandState.StartDefaultCommand();
            }
        }
    }

    public void SetSelectedIndex(int index)
    {
        flagSelect = 0;
        cbxLeaderType.SelectedIndex = index;
        flagSelect = 1;
    }

    private void AnnotationForm_Move(object sender, EventArgs e)
    {
        Form dlg = (Form)sender;
        Point ds = dlg.DesktopLocation;
    }

    private void AnnotationForm_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        //List<BCOM.Element> elemDel = new List<BCOM.Element>();
        //List<DPoint3d> pts = new List<DPoint3d>();

        //PlaceLeaderTool.InstallNewInstance(pts, s_current, -1, elemDel, 0, -1, 0);

    }

    UserControl IToolControl.Control
    {
        get { return this; }
    }

    void IToolControl.ApplyChanges()
    {
        throw new NotImplementedException();
    }

    void IToolControl.ResetChanges()
    {
        throw new NotImplementedException();
    }

        public void LoadToolData(ToolData data)
        {
            throw new NotImplementedException();
        }
    }
}
