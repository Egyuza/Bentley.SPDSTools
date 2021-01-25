using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using Bentley.Windowing;

namespace SPDSTools
{
public class ToolFormHelper
{
    
    private static WindowContent _toolWindow;
    public static WindowContent ToolWindow { get { return _toolWindow; } }
    //private static ToolMainContainer toolContainer;
    private static IToolControl _toolCtrl;

    //private ToolForm(Control content, object id, string caption, WindowContentType type) 
    //    : base(content, id, caption, type)
    //{        
    //}

    /// <summary>
    /// Активный режим работы тула
    /// </summary>
    public static ToolMode ActiveToolMode
    {
        get
        {
            try
            {
                var toolContainer = _toolWindow?.Content as ToolMainContainer;
                return   toolContainer != null 
                    ? toolContainer.ActiveToolMode 
                    : ToolMode.None;
            }
            catch (Exception) { return ToolMode.None; }
        }
        set
        {
            try
            {
                var toolContainer = _toolWindow?.Content as ToolMainContainer;
                if (toolContainer != null)
                    toolContainer.ActiveToolMode = value;                    
            }
            catch (Exception) { }
        }
    }

    public static void Show(ToolTypeEnum type, bool forceEditMode)
    {

        if (_toolWindow != null)
        {
            _toolWindow.Show();
            return;
        }

        _toolCtrl = LoadByType(type);

        if (_toolCtrl == null)
           return;   
        
        // по умолчанию при запуске
        ToolMode mode = ToolMode.Draw;

        if (forceEditMode)
            mode = ToolMode.Edit;

        loadWindowContent(out _toolWindow, _toolCtrl, mode);
        _toolWindow.Show();
    }

    public static IToolControl LoadByType(ToolTypeEnum type)
    {
        IToolControl res = null;
        switch (type)
        {
        case ToolTypeEnum.HeightTool:
            res = HeightToolControl.InstallNewInstance();
            break;
        case ToolTypeEnum.LeaderTool:
            break;
        }
        ActiveToolMode = ToolMode.None;

        return res;
    }

    public static void LoadToolDataForEdit(ToolData toolData)
    {
        if (toolData is HeightToolData)
        {
            if (false == _toolCtrl is HeightToolControl)
            {
                HeightToolControl.InstallNewInstance();
                _toolCtrl = HeightToolControl.Instance;
            }

            HeightToolControl.Instance.LoadToolData(toolData as HeightToolData);
        }
        ActiveToolMode = ToolMode.Edit;
    }

    private static void loadWindowContent(out WindowContent wcontent, 
        IToolControl toolCtrl, ToolMode toolMode)
    {
        ToolTypeEnum type = (toolCtrl is HeightToolControl) ? 
            ToolTypeEnum.HeightTool : ToolTypeEnum.LeaderTool;

        var container = new ToolMainContainer(toolCtrl);
        container.ActiveToolMode = toolMode;
        //System.Drawing.Size prefSize = container.Size;

        System.Drawing.Size remSize = container.Size;
        // container.MinimumSize = container.Size;

        WindowManager winMngr = WindowManager.GetForMicroStation();
        string caption = "SPDS: " + EnumString.ToString(type);
        
        wcontent = winMngr.DockPanel(container, caption,
            caption, DockLocation.Floating); // здесь вызов FrmMain_Load
        wcontent.FloatingHostForm.SizeChanged += FloatingHostForm_SizeChanged;
        wcontent.FloatingHostForm.Disposed += Form_Disposed;

        //wcontent.FloatingHostForm.AcceptButton = container.btnApply;
        //wcontent.FloatingHostForm.CancelButton = container.btnCancel;

        if (wcontent.State != Bentley.Windowing.Docking.ZoneState.Floating)
        {
            container.Dock = DockStyle.Top;
        }
        else
        {
            container.Dock = DockStyle.None;
            container.Size = remSize;
        }
        
        container.SizeChanged += ToolContainer_SizeChanged;
        
        // сброс для корректировки:
        wcontent.FloatingHostForm.Size = new System.Drawing.Size();
    }

    static bool _Form_SizeChanging;
    private static void FloatingHostForm_SizeChanged(object sender, EventArgs e)
    {
        Form form = sender as Form;

        if (_Form_SizeChanging || form == null)
            return;

        _Form_SizeChanging = true;

        try
        {        
            if (_toolWindow.State != Bentley.Windowing.Docking.ZoneState.Floating)
                return;

            //form.Size = form.PreferredSize;
            form.Width = _toolWindow.Content.Width + 8;
            form.Height = _toolWindow.Content.Height + form.PreferredSize.Height;
        }
        finally
        {
            _Form_SizeChanging = false;          
        }
    }

    private static bool _SizeChanged_Susspended;
    private static void ToolContainer_SizeChanged(object sender, EventArgs e)
    {
        ToolMainContainer container = sender as ToolMainContainer;
        if (container == null)
            return;

        if (_SizeChanged_Susspended || 
            _toolWindow.State != Bentley.Windowing.Docking.ZoneState.Floating)
        {
            return;
        }

        _SizeChanged_Susspended = true;

        Form form = _toolWindow.FloatingHostForm;
        form.SuspendLayout();
        try
        {
           // form.AutoSize = false;
            //form.AutoSize = true;
            container.Refresh();
            form.Size = new System.Drawing.Size();

            //// toolContainer.Visible = false;
            //form.AutoSize = true;

            //System.Drawing.Size delta = form.Size;
            ////toolContainer.Size = new System.Drawing.Size();
            //container.Dock = DockStyle.Top;
            //container.Refresh();
            //delta = container.Size - container.PreferredSize;
            
            // //new System.Drawing.Size(
            // //   form.Width - toolContainer.ClientSize.Width,
            // //   toolContainer.Height + delta.Height);           

         
            //form.AutoSize = false;

            //form.MinimumSize = new System.Drawing.Size(
            //    container.Size.Width + 12,
            //    form.Height + delta.Height);


            //form.AutoSize = true;
            //form.AutoSize = false;
            //toolContainer.Visible = true;


                //form.Padding = new Padding(0);
                ////  toolContainer.Size = toolContainer.PreferredSize;
                //System.Drawing.Size prefSize = toolContainer.Size;
                //form.MinimumSize = new System.Drawing.Size(
                //    prefSize.Width + 12,
                //    prefSize.Height + form.PreferredSize.Height);

                /// заставит принять оптимальные размеры
                //form.Size = new System.Drawing.Size(0, form.PreferredSize.Height);
                //form.AutoSize = true;
                //form.AutoSize = false;
            }
        finally
        {
            form.ResumeLayout();
            _SizeChanged_Susspended = false;
        }
    }    
    
    private static void Form_Disposed(object sender, EventArgs e)
    {
        if (HeightTool.Instance != null)
            HeightTool.Instance.StopTool();
        if (ModifyTool.Instance != null)
            ModifyTool.Instance.StopTool();
        
       _toolWindow?.Close();
       _toolWindow = null;

       SelectionHelper.Instance?.Dispose();
    }
}
}
