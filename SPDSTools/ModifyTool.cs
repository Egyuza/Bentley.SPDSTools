using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.MstnPlatformNET;
using Bentley.GeometryNET;
using System.Windows.Forms;

namespace SPDSTools
{

public class ModifyTool : DgnElementSetTool
{
    const int KEY_SHIFT = 4;
    const int KEY_CTRL = 8;
    const int KEY_ALT = 10;

    public event EventHandler OnCleaningUpEvent;

    public void ApplyChangesToSelection(ProgressBar pbar = null) 
    {
        // TODO можно поместить метод в класс ToolMainContainer и запускать
        // там делегат функции обработки элемента, определённый в этом классе

        uint count = SelectionSetManager.NumSelected();

        if (pbar != null)
        {
            pbar.Maximum = (int)count;
            pbar.Step = 0;
            pbar.Enabled =
            pbar.Visible = true;            
            pbar.Refresh();
        }            

        DgnModelRef modelRef = Session.Instance.GetActiveDgnModel();
        for (uint i = 0; i < SelectionSetManager.NumSelected(); ++i)
        {
            if (pbar != null)
                pbar.Step = (int)i;

            Element element = null; 
            if (StatusInt.Success ==
                SelectionSetManager.GetElement(i, ref element, ref modelRef))
            {
                CellHeaderElement cell = element as CellHeaderElement;
                if (cell == null)
                    continue;
                
                ECPropertyReader reader =
                    ECPropertyReader.TryGet(element, ECClassTypeEnum.HeightTool);

                if (reader != null)
                {
                    var hData = new HeightToolData(element as CellHeaderElement);
                    hData.UpdatFromParameters();
                    var toolEl = new HeightToolElement(hData);
                    toolEl?.LoadByPoints(hData.Points);

                    toolEl?.ReplaceInModel(cell);
                }                
            }
        }

        if (pbar != null)
        {
            pbar.Step = pbar.Maximum;
            pbar.Visible =
            pbar.Enabled = false;
            pbar.Step = 0;
        }
    }
    
    // манипуляторы выделенного тула
    ToolSnaps toolSnap;

    bool _ModifyProcessing;
    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
        Element element = LocateElement(ev);
        hiliteTemp(element);

        if (toolSnap == null)
            _ModifyProcessing = false; // НВС

        if (toolSnap != null)
        {
            if (_ModifyProcessing)
            {
                AccuSnap.SnapEnabled = true;
                // todo ! проверить автоориентацию
                var points = toolSnap.GetExpandedPoints(ev.Point);
                
                ToolElement toolElem;
                if (ToolElement.Load(toolSnap.ToolData, out toolElem))
                {
                    toolElem.LoadByPoints(points);
                    toolElem.DoRedraw(ev.Viewport);
                }
            }
            else
            {
                AccuSnap.SnapEnabled = false;
                toolSnap.Redraw(ev);
                BeginPickElements();
            }
        }
    }

    private void hiliteTemp(Element element)
    {
        if (element == null)
            return;  
                 
        var _redrawElems = new RedrawElems();
        _redrawElems.SetDynamicsViewsFromActiveViewSet(Session.GetActiveViewport());
        _redrawElems.DrawMode = DgnDrawMode.TempDraw;
        _redrawElems.DrawPurpose = DrawPurpose.Dynamics;
        _redrawElems.IgnoreHilite = true;

        element.ExposeChildren(ExposeChildrenReason.Edit);
            DgnFile file = Session.Instance.GetActiveDgnFile();
            uint  colorHilite = 53; // 53 // file.GetClosestColor(System.Drawing.Color.Red);
        ElementPropertiesSetter setter = new ElementPropertiesSetter();
        //setter.SetFillColor(53);
        setter.SetChangeEntireElement(true);
        setter.SetColor(colorHilite);
             
        setter.SetWeight(1);
        setter.Apply(element);
 
        //List<Element> mchildren = new List<Element>();
        //foreach(var child in element.GetChildren())
        //{
        //    setter.Apply(child);
        //    mchildren.Add(child);
        //}

        //CellHeaderElement modCell = new CellHeaderElement(
        //    ev.Viewport.GetRootModel(),
        //    "temp",
        //    (element as DisplayableElement).GetOrigin(),
        //    DMatrix3d.Identity,
        //    mchildren);

        _redrawElems.DoRedraw(element);
    }
    
    protected override bool OnDataButton(DgnButtonEvent ev)
    {
        int modKey;
        bool modKeyDown;
        uint qMask;       

        GetModifierKeyTransitionState(out modKey, out modKeyDown, out qMask);
        bool modCtrlKey = (qMask == KEY_CTRL);
        
        // TODO проверка активных манипуляторов
        
        if (_ModifyProcessing && toolSnap != null)
        {
            _ModifyProcessing = false;
            var points = toolSnap.GetExpandedPoints(ev.Point);

            ToolElement toolElem;
            if (ToolElement.Load(toolSnap.ToolData, out toolElem))
            {
                toolElem.LoadByPoints(points);
                toolElem.AddToModel();
                SelectionHelper.Select(null, false);
                ExitTool();
            } 
        }
        else 
        {
            if (toolSnap != null && toolSnap.SnapIsFocused)
            {
                _ModifyProcessing = true;
                return true;
            }

             _ModifyProcessing = false;

            SelectionHelper.Select(LocateElement(ev), modCtrlKey);

            int selCount = SelectionHelper.Instance.SelectedData.Count();

            toolSnap = (selCount == 1)
                ? new ToolSnaps(SelectionHelper.Instance.SelectedData.First())
                : null;

            if (selCount == 0)
                ExitTool();
        }

        return true;
    }

    private Element LocateElement(DgnButtonEvent ev) {
        HitPath hitPath = DoLocate(ev, true, 1);
        return hitPath?.GetHeadElement();
    }

    public override StatusInt OnElementModify(Element element)
    {
        // All modification is done in OnDataButton method, 
        // just return Error from this method.
        return StatusInt.Error;
    }

    protected override void OnPostInstall()
    {
        this.SetLocateCursor(true);
        this.BeginDynamics();

        if (SelectionHelper.Instance.SelectedData.Count() == 1)
        {
            toolSnap = new ToolSnaps(
                SelectionHelper.Instance.SelectedData.First());
            _ModifyProcessing = false;
        }        
    }

    bool OnCleanUpInProgress_;
    protected override void OnCleanup()
    {
        if (OnCleanUpInProgress_)
            return;
        
        OnCleanUpInProgress_ = true;
        try
        {
            SelectionHelper.Instance.setIsActive(false);
           // SelectionSetManager.EmptyAll();
            // OnCleaningUpEvent?.Invoke(this, new EventArgs());
            OnCleaningUpEvent?.Invoke(this, new EventArgs());
        }
        finally
        {
            OnCleanUpInProgress_ = false;
        }
    }

    protected override bool OnResetButton(DgnButtonEvent ev)
    {
        // return base.OnResetButton(ev);
        ExitTool();
        SelectionSetManager.EmptyAll();
        return true;
    }

    protected override void OnRestartTool()
    {
       // InstallNewInstance();
       StopTool();
    }

    public void StopTool()
    {
        if (this.IsActiveTool() && !OnCleanUpInProgress_) {
            ExitTool();
        }
    }
    
    public static ModifyTool Instance { get; private set; }

    public static void InstallNewInstance ()
    {   
        if (Instance != null) {
            Instance.Dispose();            
        }    
        Instance = new ModifyTool();
        Instance.InstallTool();
    }

    protected override void Dispose(bool A_0)
    {        
        base.Dispose(A_0);
    }
}
}
