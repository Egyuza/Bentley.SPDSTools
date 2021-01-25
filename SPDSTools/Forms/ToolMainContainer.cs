using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.MstnPlatformNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SPDSTools
{
public partial class ToolMainContainer : UserControl
{
    public delegate void ButtonEventHandler(bool stateOn);
    public delegate void EventHandler();   

    public ToolTypeEnum ToolType
    {
        get
        {
            if (ToolCtrl is HeightToolControl)
                return ToolTypeEnum.HeightTool;
            else
                return ToolTypeEnum.LeaderTool;
        }
    }
    
    // встраиваемый в форму контрол специализированного тула
    public IToolControl ToolCtrl { get; private set; }

    private bool ChangeModeInProgress;
    private ToolMode _toolMode;
    public ToolMode ActiveToolMode
    {
        get
        {
            return _toolMode;
        }
        set
        {
            // TODO !
                        
            if (_toolMode == value || ChangeModeInProgress)
                return;

            ChangeModeInProgress = true;

            try
            {
                _toolMode = value;

                // цвет подложки для видимость режима Редактирования
                this.BackColor = (_toolMode == ToolMode.Edit) ?
                    Color.LightSteelBlue : Color.Transparent;

                chbxDraw.Checked = _toolMode == ToolMode.Draw; // НВС
                chbxLocateDraw.Checked = _toolMode == ToolMode.LocateDraw; // НВС


                tlpSelection.Visible =
                tlpApplyReset.Visible = 
                tlpApplyReset.Enabled = _toolMode == ToolMode.Edit;

                switch (_toolMode)
                {
                case ToolMode.None:                               
                    chbxDraw.Checked = 
                    chbxLocateDraw.Checked = false;

                    if (HeightTool.Instance != null &&
                        HeightTool.Instance.IsActiveTool())
                    {
                        HeightTool.Instance.OnCleaningUpEvent -= 
                            HeightTool_OnCleaningUpEvent;  
                        HeightTool.Instance.StopTool();
                    }                    
                    break;

                case ToolMode.Draw:
                    chbxDraw.Checked = true;
                    chbxLocateDraw.Checked = false;

                    if (ToolType == ToolTypeEnum.HeightTool)
                    {
                        HeightTool.InstallNewInstance();
                        HeightTool.Instance.OnCleaningUpEvent += 
                            HeightTool_OnCleaningUpEvent;
                        HeightToolControl.Instance?.LoadToolData(
                            new HeightToolData(Session.Instance.GetActiveDgnModel()));
                    }
                    break;

                case ToolMode.LocateDraw:
                    chbxLocateDraw.Checked = true;

                    if (ToolType == ToolTypeEnum.HeightTool)
                    {
                        HeightTool.InstallNewInstance(true);
                        HeightTool.Instance.OnCleaningUpEvent += 
                            HeightTool_OnCleaningUpEvent;
                        HeightToolControl.Instance?.LoadToolData(
                            new HeightToolData(Session.Instance.GetActiveDgnModel()));
                    }
                    break;

                case ToolMode.Edit:
                    chbxDraw.Checked = false;
                    ModifyTool.InstallNewInstance();
                    ModifyTool.Instance.OnCleaningUpEvent +=
                         ModifyTool_OnCleaningUpEvent;
                    break;
                }
            }
            finally
            {
                ChangeModeInProgress = false;
            }
        }
    }

    public ToolMainContainer(IToolControl ctrl)
    {
        if (ctrl == null)
            throw new ArgumentNullException("toolControl");

        ToolCtrl = ctrl;
        
        InitializeComponent();

        this.Location = new System.Drawing.Point(0,0);
                
        //this.Dock = DockStyle.Fill;

        this.Margin =
        ctrl.Control.Margin = new Padding(0);
        
        this.Dock = DockStyle.None;

        var coord = tlpContainer.GetPositionFromControl(markToInsert);
        tlpContainer.Controls.Remove(markToInsert);

        int irow = 1;
        tlpContainer.Controls.Add(ctrl.Control, coord.Column, coord.Row);
        tlpContainer.RowStyles[irow].SizeType = SizeType.AutoSize;

        this.AutoSizeMode = 
        tlpContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;

        this.AutoSize =
        tlpContainer.AutoSize = true;

        this.Size = this.PreferredSize;

        foreach (ButtonBase btn in 
            new List<ButtonBase>() {chbxDraw, chbxLocateDraw, btnUpdate})
        {
            btn.MouseEnter += Button_MouseEnter;
            btn.MouseLeave += Button_MouseLeave;
            if (btn is CheckBox) {
                (btn as CheckBox).CheckedChanged += Chbox_CheckedChanged;
            }
        }

        SelectionHelper.Instance.SelectionChanged += Instance_SelectionChanged;
        
        //this.AutoSize = true;

        //this.AutoSize = 
        //tlpContainer.AutoSize = false;
        //tlpContainer.Dock = DockStyle.Top;
        //ctrl.Dock = DockStyle.Fill;
        //this.Refresh();

        ActiveToolMode = ToolMode.None;;
    }

    private void Instance_SelectionChanged(
        IEnumerable<ToolData> selection, uint filePosition)
    {
        //if (ActiveToolMode == ToolMode.Draw)
        //    return;
        
        List<HeightToolData> listHData = new List<HeightToolData>(
            selection.CastDown<HeightToolData, ToolData>());
               
         lbSelCount.Text = listHData.Count.ToString();
        
        if (listHData.Count == 0)
        {
            ActiveToolMode = ToolMode.None;
            lbSelCount.Text = "0";
            return;
        }
        else 
        {
            ActiveToolMode = ToolMode.Edit;
            tlpSelection.Font = new Font(
                    tlpSelection.Font, 
                    listHData.Count == 1 ? FontStyle.Regular : FontStyle.Bold
                );
                
            var HToolCtrl =  ToolCtrl.Control as HeightToolControl;

            if (listHData.Count == 1)
            {
                HToolCtrl?.LoadToolData(listHData[0]);
            }
            else
            {
                // TODO через ElementCache + FilePosition можно попробовать найти 
                // конкретный элемент из выделенной группы, по которому был 
                // произведен двойной клик чтобы именно его свойства вывести в форму

                HToolCtrl?.LoadToolData(listHData[0]);
            }
        }
    }

    private void ToolMainContainer_DockChanged(object sender, EventArgs e)
    {
        // Важно!
        tlpContainer.Dock = ToolCtrl.Control.Dock = 
            (this.Dock == DockStyle.None)
                ? DockStyle.None
                : DockStyle.Top;
    }

    private void Model_SelectionChangedEvent(AddIn sender, 
            AddIn.SelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Action != AddIn.SelectionChangedEventArgs.ActionKind.SetChanged &&
            eventArgs.Action != AddIn.SelectionChangedEventArgs.ActionKind.SetEmpty)
        {
            return;
        }

    }

#region SpecialButtonsOptions
    private void Chbox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chbox = sender as CheckBox;
        if (chbox == null)
            return;
        
        if (chbox.Checked)
            chbox.FlatStyle = FlatStyle.Popup;
        else if (chbox.Focused == false)
            chbox.FlatStyle = FlatStyle.Flat;
    }

    private void Button_MouseLeave(object sender, EventArgs e)
    {
        ButtonBase btn = sender as ButtonBase;
        if (btn == null)
            return;
        
        if (!(btn is CheckBox) || (btn as CheckBox).Checked == false) 
            btn.FlatStyle = FlatStyle.Flat;           
    }

    private void Button_MouseEnter(object sender, EventArgs e)
    {
        ButtonBase button = sender as ButtonBase;
        if (button == null)
            return;
        
        button.FlatStyle = FlatStyle.Popup;
    }
#endregion

    private void btnApply_Click(object sender, EventArgs e)
    {
        if (ActiveToolMode != ToolMode.Edit)
        {   // НВС
            tlpApplyReset.Visible =
            tlpApplyReset.Enabled = false;
            return;
        }
        uint num = SelectionSetManager.NumSelected();
        if (num > 1)
        {
            DialogResult res = MessageBox.Show(
                "You are going to modify " + num + " elements.\nContinue?",
                "Edit SPDS-elements", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Exclamation);
            if (res == DialogResult.No)
                return;
        }

        // TODO with progbar
        // ModifyTool.Instance?.ApplyChangesToSelection(progBar);
        ModifyTool.Instance?.ApplyChangesToSelection();
        ActiveToolMode = ToolMode.None;
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        //tlpApplyReset.Enabled = false;
        if (ModifyTool.Instance != null)
        {
            ModifyTool.Instance.StopTool();
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        Keyin.refreshAllSPDS_InModel(progBar);
    }

    
    private void chbxDraw_CheckedChanged(object sender, EventArgs e)
    {
        ActiveToolMode = chbxDraw.Checked ? ToolMode.Draw : ToolMode.None;
    }

    private void chbxLocateDraw_CheckedChanged(object sender, EventArgs e)
    {
        ActiveToolMode = chbxLocateDraw.Checked ? ToolMode.LocateDraw : ToolMode.None;
    }

    private void HeightTool_OnCleaningUpEvent(object sender, EventArgs e)
    {
        ActiveToolMode = ToolMode.None;
    }

    private void ModifyTool_OnCleaningUpEvent(object sender, EventArgs e)
    {
        //chbxEdit.Checked = false;
        ActiveToolMode = ToolMode.None;
    }

}
}