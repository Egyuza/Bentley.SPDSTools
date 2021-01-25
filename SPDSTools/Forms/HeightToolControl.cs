using System;
using System.Windows.Forms;
using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET;
using System.Xml.Serialization;

using System.Runtime.InteropServices;
using SPDSTools.Parameters;
using HP = SPDSTools.Parameters.HeightParametersEnum;

namespace SPDSTools
{

/// <summary>
/// ! ПОДСКАЗКА:
/// Чтобы включить дизайнер, необходимо указать базовым классом - UserContol
/// </summary>
public partial class HeightToolControl : UserControl, IToolControl
{
    const char _plusMinus = '±';
    const string thiknessPrefix = "h=";

    private Properties.Settings Sets
    {
        get { return global::SPDSTools.Properties.Settings.Default; }
    }

    private HeightParameters pars
    {
        get { return HeightParameters.Instance; }
    }
    
    public static HeightToolControl Instance { get; private set; }
    
    public static HeightToolControl InstallNewInstance() {
        return (Instance = new HeightToolControl());
    }

    // КОНСТРУКТОР
    public HeightToolControl()
        : base()
    {
        InitializeComponent();

        this.MinimumSize = tlpMain.Size;
        this.Size = this.PreferredSize; // мин. высота контрола
              
        // стили текста в открытом файле
        loadActiveDgnTextStyles();
                
        LoadToolData(new HeightToolData(Session.Instance.GetActiveDgnModel()));
    }
    
    private void loadAlignment(AlignmentEnum alignment)
    {
        switch (alignment)
        {
        case AlignmentEnum.Auto:
            chbxOrient_Auto.Checked = true; break;
        case AlignmentEnum.Left:
            chbxOrient_Left.Checked = true; break;
        case AlignmentEnum.TopLeft:
            chbxOrient_Left.Checked = true;
            chbxOrient_Top.Checked = true; break;
        case AlignmentEnum.Top:
            chbxOrient_Top.Checked = true; break;
        case AlignmentEnum.TopRight:
            chbxOrient_Top.Checked = true; 
            chbxOrient_Right.Checked = true; break;
        case AlignmentEnum.Right:
            chbxOrient_Right.Checked = true; break;
        case AlignmentEnum.BottomRight:
            chbxOrient_Right.Checked = true; 
            chbxOrient_Bottom.Checked = true; break;
        case AlignmentEnum.Bottom:
            chbxOrient_Bottom.Checked = true; break;
        case AlignmentEnum.BottomLeft:
            chbxOrient_Left.Checked = true;
            chbxOrient_Bottom.Checked = true; break;
        }
    }    

    /// <summary>
    /// Загрузка на форму списка доступных текстовых стилей открытого dgn-файла
    /// </summary>
    private void loadActiveDgnTextStyles()
    {
        cbxTextStyle.Items.Clear();      
        cbxTextStyle.Items.Add("None"); // ru: Стиль(нет)


        // из открытого файла:
        DgnFile file = Session.Instance.GetActiveDgnFile();

        if (file != null) // НВС
        {
            foreach(DgnTextStyle txtStyle in file.GetTextStyles())
                cbxTextStyle.Items.Add(txtStyle.Name);            

            // стиль в настройках файла:
            //DgnTextStyle textStyle = DgnTextStyle.GetSettings(_data.file);
            //int ind;
            //if ((ind = cbxTextStyle.Items.IndexOf(Sets.LastTextStyle)) >= 0)
            //    cbxTextStyle.SelectedIndex = ind;
            //else if (string.IsNullOrEmpty(textStyle.Name))
            //    cbxTextStyle.SelectedIndex = 0;
            //else
            //    cbxTextStyle.Text = textStyle.Name;
        }
        else
        {
            //cbxTextStyle.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Обработка события чтения данных тула для последующего редактирования
    /// </summary>
    private void _data_LoadedFromCellForEditEvent()
    {
        this.Enabled = true;

        // TODO

        //elementCacheData = GetFormDataFromTool();
        //setFormData(elementCacheData);
    }

    /// <summary>
    /// Изменения стиля текста
    /// </summary>
    private void cbxTextStyle_SelectedIndexChanged(object sender, EventArgs e)
    {        
        CfgParameters.setValue(pars[HP.TEXTSTYLE], cbxTextStyle.Text);
    }

    private void chbxAutoCalc_CheckedChanged(object sender, EventArgs e)
    {
        // TODO remove?
        
        if (chbxAutoCalc.Checked)
        {
            chbxPlusMinus.Enabled = 
            chbxPlusMinus.Checked = false;
        }
        else
        {
            chbxPlusMinus.Enabled = true;
        }

        CfgParameters.setValue(pars[HP.AUTOCALC], chbxAutoCalc.Checked);

        var type = (HeightToolTypeEnum)pars[HP.TYPE].EnumValue;
        CfgParameters.setValue(
            pars[HP.CALCBYITEM], type == HeightToolTypeEnum.Planned);
        
        txtText.Enabled =
        txtThikness.Enabled = !chbxAutoCalc.Checked;
        if (txtText.Enabled)
            txtText.Focus();
    }

    private void chbxAsLeader_CheckedChanged(object sender, EventArgs e)
    {
        CfgParameters.setValue(pars[HP.AS_LEADER], chbxAsLeader.Checked);   
    }

    private void chbxCalcByItem_CheckedChanged(object sender, EventArgs e)
    {
        // TODO ? CALCBYITEM
        // пока управление только через AutoCalc
        // CfgParameters.setValue(pars[HP.CALCBYITEM], chbxCalcByItem.Checked);
    }

    private void txtText_TextChanged(object sender, EventArgs e)
    {
        if (sender != txtText)
            return;

        char dltr = pars[HP.DECIMALSEPARATOR].CharValue;
        if (!char.IsWhiteSpace(dltr))
        {
            int index = txtText.SelectionStart;
            txtText.Text = txtText.Text.Replace('.', dltr).Replace(',', dltr);
            txtText.SelectionStart = index;
        }

        if (chbxPlusMinus.Checked && !txtText.Text.StartsWith(_plusMinus.ToString()))
        {
            txtText.SuspendLayout();
            int index = txtText.SelectionStart;
            txtText.Text = chbxPlusMinus.Text + txtText.Text;
            txtText.SelectionStart = index + 1;
            txtText.ResumeLayout();
        }

        CfgParameters.setValue(pars[HP.TEXT], txtText.Text);
    }

    private bool _suspend;
    private void chbxOrient_CheckedChanged(object sender, EventArgs e)
    {
        if (_suspend)
            return;
        _suspend = true;

        try
        {
            if (sender == chbxOrient_Auto && chbxOrient_Auto.Checked)
            {
                chbxOrient_Left.Checked =
                chbxOrient_Top.Checked =
                chbxOrient_Right.Checked =
                chbxOrient_Bottom.Checked = false;
            }
            else if (sender == chbxOrient_Left && chbxOrient_Left.Checked)
                chbxOrient_Right.Checked = false;
            else if (sender == chbxOrient_Right && chbxOrient_Right.Checked)
                chbxOrient_Left.Checked = false;
            else if (sender == chbxOrient_Top && chbxOrient_Top.Checked)
                chbxOrient_Bottom.Checked = false;
            else if (sender == chbxOrient_Bottom && chbxOrient_Bottom.Checked)
                chbxOrient_Top.Checked = false;
        
            chbxOrient_Auto.Checked = !(chbxOrient_Left.Checked || 
                chbxOrient_Top.Checked || chbxOrient_Right.Checked ||
                chbxOrient_Bottom.Checked);

            CfgParameters.setValue(pars[HP.ALIGNMENT], _GetAlignment());
        }
        finally
        {
            _suspend = false;
        }
    }
    
    private void rbtnType_CheckedChanged(object sender, EventArgs e)
    {
        HeightToolTypeEnum type = rbtnType_Section.Checked ? 
            HeightToolTypeEnum.Section : HeightToolTypeEnum.Planned;
        
        CfgParameters.setValue(pars[HP.TYPE], type);
        CfgParameters.setValue(
            pars[HP.CALCBYITEM], type == HeightToolTypeEnum.Planned);
                    
        var style = (ToolStyleEnum)pars[HP.STYLE].EnumValue;

        chbxAsLeader.Visible = 
        chbxSectorArrow.Visible = (type == HeightToolTypeEnum.Planned);
        
        txtThikness.Visible =
        chbxThikness.Visible =
        chbxThikness.Enabled = 
            (style == ToolStyleEnum.ED && type == HeightToolTypeEnum.Planned);

        chbxThikness.Checked = !chbxThikness.Visible ? 
            false : chbxThikness.Checked;
        
        // TODO !
        //if (_data.Type == HeighToolTypeEnum.Planned)
        //    chbxAutoCalc.Enabled = chbxAutoCalc.Checked = false;
        //else
        //    chbxAutoCalc.Enabled = chbxAutoCalc.Checked = true;
    }

    private AlignmentEnum _GetAlignment()
    {
        if (chbxOrient_Left.Checked)
        {
            return chbxOrient_Top.Checked ? AlignmentEnum.TopLeft
                : chbxOrient_Bottom.Checked ? AlignmentEnum.BottomLeft
                : AlignmentEnum.Left;
        }
        else if (chbxOrient_Right.Checked)
        {
            return chbxOrient_Top.Checked ? AlignmentEnum.TopRight
                : chbxOrient_Bottom.Checked ? AlignmentEnum.BottomRight
                : AlignmentEnum.Right;
        }
        else if (chbxOrient_Top.Checked)
            return AlignmentEnum.Top;
        else if (chbxOrient_Bottom.Checked)
            return AlignmentEnum.Bottom;
        else
            return AlignmentEnum.Auto; 
    }

    private void chbxPlusMinus_CheckedChanged(object sender, EventArgs e)
    {
        chbxPlusMinus.FlatStyle = chbxPlusMinus.Checked 
            ? FlatStyle.Popup : FlatStyle.Flat;

        txtText.SuspendLayout();
        bool prefExists = txtText.Text.StartsWith(_plusMinus.ToString());
        if (chbxPlusMinus.Checked)
        {
            if (!prefExists) // доб. префикс
                txtText.Text = _plusMinus + txtText.Text;
        }
        else if (prefExists) // удаляем префикс
        {
            txtText.Text = txtText.Text.TrimStart(_plusMinus);
        }

        txtText.ResumeLayout();
    }
    
    private void chbxUseFilling_CheckedChanged(object sender, EventArgs e)
    {
       CfgParameters.setValue(pars[HP.USE_FILLING], chbxUseFilling.Checked);
    }
    private void chbxSectorArrow_CheckedChanged(object sender, EventArgs e)
    {
        CfgParameters.setValue(pars[HP.SECTOR_ARROW], chbxSectorArrow.Checked);
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

    /// IToolControl
    public void LoadToolData(ToolData data)
    {
        this.SuspendLayout();
        try
        {
            _loadToolData(data);
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    private void _loadToolData(ToolData data)
    {        
        var hData = data as HeightToolData;
        if (hData == null)
            return;
        
        grbxAdditions.Visible = 
        grbxAdditions.Enabled =
        txtThikness.Visible =
        chbxThikness.Visible =
        chbxThikness.Enabled = false;
        
        switch (hData.ToolStyle)
        {
        case ToolStyleEnum.Paks:
        case ToolStyleEnum.ED: // наследует настройки от ПАКШ
        {
            grbxAdditions.Visible =
            chbxUseFilling.Enabled =
            grbxAdditions.Enabled = true;

            if (hData.ToolStyle == ToolStyleEnum.ED && 
                hData.Type == HeightToolTypeEnum.Planned)
            {
                txtThikness.Visible =
                chbxThikness.Visible =
                chbxThikness.Enabled = true;
            }
        } break;
        }

        chbxSectorArrow.Checked = hData.SectorArrow;
        chbxAutoCalc.Checked = hData.AutoCalc;
        chbxCalcByItem.Checked = hData.CalcByItem;        
        chbxUseFilling.Checked = hData.UseFilling;

        txtText.Text = hData.Text;
        txtThikness.Text = hData.Text2.TrimStart(thiknessPrefix.ToCharArray());
        txtThikness.Enabled =
        txtText.Enabled = !chbxAutoCalc.Checked;

        DgnTextStyle textStyle = TextStyleHelper.GetTextStyle(
            pars[HP.TEXTSTYLE].StringValue, 
            Session.Instance.GetActiveDgnModel(), true);
        {
            cbxTextStyle.Text = textStyle.Name;
            if (string.IsNullOrWhiteSpace(cbxTextStyle.Text) || cbxTextStyle.Text == "None")
                cbxTextStyle.Enabled = true;
            else
                cbxTextStyle.Enabled = false;  
        }

        if (hData.Type == HeightToolTypeEnum.Planned)
        {
            rbtnType_Planned.Checked = true;
            chbxAsLeader.Enabled = true;
            chbxAsLeader.Checked = hData.AsLeader;
            chbxThikness.Checked = hData.ShowItemHeight;
        }
        else
        {
            rbtnType_Section.Checked = true;
            chbxAsLeader.Visible = false;   
        }

        loadAlignment(hData.Alignment);
        // todo Alignment
    }

    private void txtText_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (ToolFormHelper.ActiveToolMode == ToolMode.None)
                ToolFormHelper.ActiveToolMode = ToolMode.Draw;
        }
    }

    private void chbxItemHeight_CheckedChanged(object sender, EventArgs e)
    {
        CfgParameters.setValue(pars[HP.SHOW_ITEMHEIGHT], chbxThikness.Checked);
    }

    private void txtThikness_TextChanged(object sender, EventArgs e)
    {
        if (sender != txtThikness)     
            return;

        char dltr = pars[HP.DECIMALSEPARATOR].CharValue;
        if (!char.IsWhiteSpace(dltr))
        {
            int index = txtThikness.SelectionStart;
            txtThikness.Text = 
                txtThikness.Text.Replace('.', dltr).Replace(',', dltr);
            txtThikness.SelectionStart = index;
        }

        string text = txtThikness.Text;
        text = text.Length > 0 ? thiknessPrefix + text : text;
        CfgParameters.setValue(pars[HP.TEXT2], text);
    }
}
}
