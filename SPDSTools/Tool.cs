using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Bentley.DgnPlatformNET.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bentley.MstnPlatformNET;
using Bentley.Windowing;
using BCOM = Bentley.Interop.MicroStationDGN;
using BMI = Bentley.MstnPlatformNET.InteropServices;

using SPDSTools.Parameters;

namespace SPDSTools
{

public enum ToolActionMode
{
    CreateNew,
    FindEdit,
    Edit,
    EditByPoints,
    LocateFixedTarget
}



public abstract class Tool: DgnElementSetTool, IDisposable
{
    public event EventHandler OnCleaningUpEvent;

    protected DgnModel _model { get; private set; }
    protected int _dynamicPoint;
    protected List<DPoint3d> _points;

    /// <summary> Наименование инструмента построения </summary>
    
    private RedrawElems redrawElems;
    private ElementAgendaDisplayable agendaDisp;

    private ToolActionMode actionMode_;
    private SnapMode snapModeSets_;
    protected ToolActionMode ActionMode 
    {
        get { return actionMode_; }
        set
        {
            actionMode_ = value;
            // отключаем привязки AccuSnap, чтобы они не мешали
            // выбору точки-манипулятора
            AccuDraw.Active = AccuSnap.SnapEnabled = (actionMode_ != ToolActionMode.FindEdit);
        }
    }

    protected bool isTargetFixed;

    //public HitPath CurHitPath { get; private set; }

    /// <summary>
    /// Флаг необходимости считывания элемента модели под курсором
    /// </summary>
    protected bool RequiresToEnsureLocatingElements;

       
    // КОНСТРУКТОР
    public Tool()
    : base()
    {
        _model = Session.Instance.GetActiveDgnModel();
        _points = new List<DPoint3d>();
        ElementAgenda.SetCapacity(1);
        
        foreach (DgnAttachment dgnAtt in _model.GetDgnAttachments())
        {
            DTransform3d transform3d;
            dgnAtt.GetTransformToParent(out transform3d, false);
            dgnAtt.GetTransformToParent(out transform3d, true);
        }
        
        AccuDraw.Active = true;
        AccuSnap.SnapEnabled = AccuSnap.LocateEnabled = true;

        SPDSTools.Instance.ModelChangedEvent += Instance_ModelChangedEvent;
    }

    public new virtual void Dispose()
    {
        SPDSTools.Instance.ModelChangedEvent -= Instance_ModelChangedEvent;       
        agendaDisp?.Dispose();
        base.Dispose();
    }

    private void Instance_ModelChangedEvent(AddIn senderIn, AddIn.ModelChangedEventArgs eventArgsIn)
    {
        if (eventArgsIn == null)
            return;

        switch (eventArgsIn.Change)
        {
        case AddIn.ModelChangedEventArgs.ChangeType.PropagateAnnotationScale:
        case AddIn.ModelChangedEventArgs.ChangeType.Settings:
            break;
        }
    }

    /// <summary> Инициализация Tool, запуск формы настроек </summary>
    //public abstract StatusInt StartTool();

    public void StopTool()
    {
        if (this.IsActiveTool() && !OnCleanUpInProgress)
            ExitTool();
    }
    
    public override void ResetStop()
    {
        base.ResetStop();
    }

    protected override void OnRestartTool()
    {
        _points.Clear();
    }
        
    bool OnCleanUpInProgress;

    protected override void OnCleanup()
    {
        if (OnCleanUpInProgress)
            return;

        OnCleanUpInProgress = true;
        try
        {          
            Settings.SnapMode = snapModeSets_;
            OnCleaningUpEvent?.Invoke(this, new EventArgs());            
        }
        finally
        {
            OnCleanUpInProgress = false;
            Dispose();
        }
    }

    public override StatusInt OnElementModify(Element element)
    {
        // todo check
        return StatusInt.Success;
    }

    protected override void OnReinitialize()
    {
        // todo ?
    }

    protected override bool OnFlick(DgnFlickEvent ev)
    {
        return true;
    }

    //protected override void ExitTool()
    //{
    //    if (toolWindow != null)
    //        closeToolWindow();
    //    else
    //        base.ExitTool(); // инициирует CleanUp
    //    this.Dispose();
    //}

    private static bool _InstallNewToolInProgress;
    protected override bool OnInstall()
    {
        _InstallNewToolInProgress = true;
        snapModeSets_ = Settings.SnapMode;
        return true;
    }

    protected override void OnPostInstall()
    {
        try
        {           
            AccuSnap.SnapEnabled = true;
            AccuSnap.LocateEnabled = true;

            base.OnPostInstall();

            // todo найти эквивалент на .net
            BCOM.Application m_app = BMI.Utilities.ComApp;
            BCOM.LocateCriteria lc = m_app.CommandState.CreateLocateCriteria(false);
           
            m_app.CommandState.SetLocateCriteria(lc);
            m_app.ShowPrompt("Select element");

            cacheColl.Clear();

            if (ActionMode == ToolActionMode.LocateFixedTarget)
            {
                SetLocateCursor(true);
            }
            else
            {
                SetLocateCursor(false);
                BeginDynamics();                
            }
        }
        finally
        {
            _InstallNewToolInProgress = false;
        }
    }
    
    private struct PreselectData
    {
        public readonly DRange3d rng;
        public readonly IEnumerable<DPoint3d> points;

        public PreselectData(ToolElement toolElement)
        {
            rng = toolElement.GetExRange();
            points = toolElement.SnapPoints;
        }
    }

    private Dictionary<IntPtr, PreselectData> cacheColl = 
        new Dictionary<IntPtr, PreselectData>();    
    
    // todo ! оптимизировать
    private bool _isAnySnapFocused;
    private DPoint3d _FocusedSnapPoint;
    private int _SelectedSnapInd;

    private List<Element> GetSnapsElemens(IEnumerable<DPoint3d> points, 
        DPoint3d cursor) 
    {
        DgnFile file = Session.Instance.GetActiveDgnFile();

        var list = new  List<Element>();
        _isAnySnapFocused = false;

              Viewport viewPort = Session.GetActiveViewport();
        ViewInformation info = viewPort.GetViewInformation();
        double k = 0.0045 * info.Delta.Magnitude; //0.4 * toolData.Scale            

        foreach (DPoint3d point in points)
        {
            var ellips = new EllipseElement(_model, null, point, k, k, 0.0);
            
            if (ellips.GetRange().Contains(cursor))
            {
                ellips.AddSolidFill(file.GetClosestColor(
                    System.Drawing.Color.Red), false);
                if (!_isAnySnapFocused)
                {
                    _FocusedSnapPoint = point;
                    _isAnySnapFocused = true;
                }
                var line =  new LineStringElement(_model, null, new DPoint3d[] { point.Shift(dY: 4*k), point.Shift(dY: -4*k)});            
                list.Add(line);
                line =  new LineStringElement(_model, null, new DPoint3d[] { point.Shift(4*k), point.Shift(-4*k)});   
                list.Add(line);                
            }
            else
            {
                ellips.AddSolidFill(file.GetClosestColor(
                    System.Drawing.Color.Blue), false);
            }
             
            list.Add(ellips);
        }
        return list;
    }

    //protected override bool OnPostLocate(HitPath path, out string cantAcceptReason)
    //{
    //    //CurHitPath = path ?? CurHitPath;
    //    return base.OnPostLocate(path, out cantAcceptReason);
    //}

    private void _correctDgnButtonEvent(DgnButtonEvent ev) 
    {
        // корректируем баг системы для работы в моделе 3D Sheet:
        ev.Point = new DPoint3d(ev.Point.X, ev.Point.Y, 0.0);
    }

    // HeightToolData toolDataEdit;
    CellHeaderElement cellEdit;

    protected override bool OnDataButton(DgnButtonEvent ev)
    {
        _correctDgnButtonEvent(ev);

        AccuSnap.LocateEnabled = true;
        AccuSnap.SnapEnabled = true;
        double z = ev.Point.Z;
        if (ActionMode == ToolActionMode.LocateFixedTarget)
        {
            Target = LocateTarget(ev);
            if (Target != null)
            {
                ActionMode = ToolActionMode.CreateNew;
                SetLocateCursor(false);
                BeginDynamics();
            }
        }
        else if (ActionMode == ToolActionMode.FindEdit || ActionMode == ToolActionMode.Edit)
        {
            foreach (IntPtr elId in cacheColl.Keys.Reverse())
            {
                PreselectData prdata = cacheColl[elId];
                if (prdata.rng.ContainsXY(ev.Point))
                {
                    // todo
                    cellEdit = 
                        (CellHeaderElement)Element.GetFromElementRef(elId);     
                    
                    // TODO !
                    // загружаем данные и оповещаем о загрузке:
                    var heightData = new HeightToolData(cellEdit);

                    // toolData.LoadFromCellForEdit(cell, true);

                    ActionMode = ToolActionMode.Edit;

                    foreach (Element elem in 
                        GetSnapsElemens(prdata.points, ev.Point))
                    {
                        redrawElems.DoRedraw(elem);
                    }
                    
                    if (_isAnySnapFocused)
                    {                       
                        if (agendaDisp != null)
                            agendaDisp.ClearHilite();
                        agendaDisp = new ElementAgendaDisplayable();
                        agendaDisp.Insert(cellEdit, true);
                        agendaDisp.Hilite();

                        int i = 0;
                        // вычисляем индекс точки-манипулятора:
                        foreach (DPoint3d pt in prdata.points)
                        {                           
                            if (pt.EqualsXY(_FocusedSnapPoint))
                            {
                                _SelectedSnapInd = i;
                                ActionMode = ToolActionMode.EditByPoints;
                                _points = new List<DPoint3d>(heightData.Points);                                
                                return true;
                            }
                            ++i;
                        }
                    }
                    return true;
                }
            }
        }
        else if (ActionMode == ToolActionMode.EditByPoints)
        {
            var pts = new List<DPoint3d>(_points);
            pts.Add(ev.Point);

            IntPtr cellPtr = IntPtr.Zero;
            if (cellEdit != null) 
            {
                cellPtr = cellEdit.GetNativeElementRef();
                var heightElem = new HeightToolElement(new HeightToolData(cellEdit));
                heightElem.LoadByPoints(pts);
                heightElem.AddToModel();
            }
            cacheColl.Remove(cellPtr);
            ActionMode = ToolActionMode.FindEdit;                
        }
        else if (ActionMode == ToolActionMode.CreateNew)
        {
            _points = _points ?? new List<DPoint3d>();
            _points.Add(ev.Point);
            
            var hdata = new HeightToolData(ev.Viewport.GetRootModel());
            var toolElem = new HeightToolElement(hdata);
            toolElem.setTarget(Target);

            toolElem.LoadByPoints(_points); // !важно т.к. задаёт координату Z

            if (_points.Count >= toolElem.ConstractionPointsCount)
            {
                toolElem.AddToModel();
                this.OnResetButton(ev);
            }
        }
        return true;
    }

    private IEnumerable<DPoint3d> expandPoints(IEnumerable<DPoint3d> arr, 
        DPoint3d varPoint, int ind)
    {
        int cnt;

        if (arr == null || ind + 1 > (cnt = arr.Count()))
            return arr;        

        DPoint3d[] res = new DPoint3d[cnt];

        double dx = 0.0, dy = 0.0;
        for (int i = 0; i < cnt; i++)
        {
            if (i < ind)
                res[i] = arr.ElementAt(i);
            else if (i == ind)
            {
                res[i] = varPoint;
                dx =  varPoint.X - arr.ElementAt(i).X;
                dy = varPoint.Y - arr.ElementAt(i).Y;
            }
            else            
                res[i] = arr.ElementAt(i).Shift(dx, dy);
        }
        return res;   
    }


    /// <summary>
    ///  Реанимация возможности обнаружения элементов модели
    /// </summary>
    /// <param name="ev"></param>
    private void EnsureLocatingElements(DgnButtonEvent ev)
    {
        /// отключение динамики и вызов процедуры DoLocate
        /// позволяет реанимировать возможность обнаружения элементов модели
        /// даже при активной динамике
        EndDynamics();

        HitPath hitpath = DoLocate(ev, true, ComponentMode.Innermost);
        //Settings.SnapMode = SnapMode.Nearest;  // привязка к ближайшей точке
        //Session.Instance.Keyin("SNAP NEAREST"); // привязка к ближайшей точке
        RequiresToEnsureLocatingElements = false;

        // восстанавливаем диманику
        BeginDynamics();        
    }

    private DisplayableElement Target;
    private DisplayableElement LocateTarget(DgnButtonEvent ev)
    {       
        HitPath hitpath = DoLocate(ev, true, ComponentMode.Innermost);
        if (hitpath != null)        
            return hitpath.GetHeadElement() as DisplayableElement;
        else
            return null;   
    }

    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
         _correctDgnButtonEvent(ev);

        if (RequiresToEnsureLocatingElements)
        {
            EnsureLocatingElements(ev);
            return;
        }
        
        DgnModel model = ev.Viewport.GetRootModel();

        // сброс
         _isAnySnapFocused = false;

        if (_points.Count == 0 && !isTargetFixed)
        {
            // Целевой элемент расчитывается по 1-ой точке           
            Target = LocateTarget(ev);
        }

        if (ActionMode == ToolActionMode.CreateNew)
        {   // РЕЖИМ РИСОВАНИЯ
            var pts = new List<DPoint3d>(_points);
            pts.Add(ev.Point);

            var heightElem = new HeightToolElement(new HeightToolData(model));
            heightElem.setTarget(Target);            
            heightElem.LoadByPoints(pts);
            heightElem.DoRedraw(ev.Viewport);
        }
        else if (ActionMode == ToolActionMode.EditByPoints)
        {
            // todo ! проверить автоориентацию
            _points = new List<DPoint3d>(
                expandPoints(_points, ev.Point, _SelectedSnapInd));

            var heightElem = new HeightToolElement(new HeightToolData(cellEdit));
            heightElem.setTarget(Target);
            heightElem.LoadByPoints(_points);
            heightElem.DoRedraw(ev.Viewport);
        }
        else if (ActionMode == ToolActionMode.Edit)
        {
            if (cellEdit != null)
            {
                ElementPropertiesSetter set = new ElementPropertiesSetter();
                set.SetTransparency(80.0);
                set.SetChangeEntireElement(true);
                set.Apply(cellEdit);
            }            
        }

        if (ActionMode == ToolActionMode.Edit || ActionMode == ToolActionMode.FindEdit) // РЕЖИМ ПОИСКА элемента для редактирования
        {
            // todo оптимизировать
            if (_points.Count == 0 && redrawElems == null)
            {
                redrawElems = new RedrawElems();
                redrawElems.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redrawElems.DrawMode = DgnDrawMode.TempDraw;
                redrawElems.DrawPurpose = DrawPurpose.Dynamics;
                redrawElems.IgnoreHilite = false;
            }

            // Проверяем элемент под курсором:
            HitPath hitPath = AccuSnap.CurrentHit; // DoLocate(ev, true, (int)ComponentMode.Innermost);
            if (hitPath != null)
            {               
                Element element = hitPath.GetHeadElement();

                //if (agendaDisp != null)
                    //agendaDisp.ClearHilite();
                //agendaDisp = new ElementAgendaDisplayable();
                //agendaDisp.Insert(element, true);
                //agendaDisp.ClearHilite();
                // todo preselect                     

                CellHeaderElement cell;
                IntPtr elRef = element.GetNativeElementRef();

                if ((cell = element as CellHeaderElement) != null && 
                    !cacheColl.ContainsKey(elRef) && ECHelper.IsSPDSElement(cell))
                {
                    if (!cacheColl.ContainsKey(elRef))
                    {
                        // todo
                        /// читаем для правильного определения диапазона и 
                        /// и точек манипулирования:

                        //if (toolData.ToolECClassType == ECHelper.GetToolType(cell))
                        //{
                        //    toolData.LoadFromCellForEdit(cell);
                        //    cacheColl.Add(elRef, new PreselectData(toolData));
                        //}
                    }
                }
            }
        
            foreach (IntPtr elId in cacheColl.Keys.Reverse())
            {                
                PreselectData prdata = cacheColl[elId];
                DRange3d rng = prdata.rng;
                if (!prdata.rng.ContainsXY(ev.Point))
                    continue;                 
                foreach (Element elem in GetSnapsElemens(prdata.points, ev.Point))
                {
                    redrawElems.DoRedraw(elem);                      
                }
            }            
        }   
    }

    protected override bool OnResetButton(DgnButtonEvent ev)
    {
        if (isTargetFixed)
        {
            this.ActionMode = ToolActionMode.LocateFixedTarget;
            SetLocateCursor(true);
        }

        if (_points.Count > 0) // в процессе построения - сбрасываем на начало
            _points.Clear();
        else
            ExitTool();
    
        return true;
    }

    //protected void ShowForm(ToolControl toolCtrl, string caption)
    //{
    //    if (toolWindow != null)
    //    {
    //        toolWindow.Show();
    //        return;
    //    }
    
    //    if (toolCtrl == null)
    //        throw new ArgumentNullException("toolCtrl");
        
    //    toolCtrl.Location = new System.Drawing.Point(0,0);        
    //    toolContainer = new ToolContainer(toolCtrl);
    //    toolContainer.OnToolEdit += ToolContainer_OnToolEdit;
        
    //    System.Drawing.Size prefSize = toolContainer.Size;        
       
    //    WindowManager winMngr = WindowManager.GetForMicroStation();
    //    caption = "SPDS: " + caption ?? ""; // toolData.ToolName;
    //    toolWindow = winMngr.DockPanel(toolContainer, caption, 
    //        caption, DockLocation.Floating); // здесь вызов FrmMain_Load    
    //    toolContainer.Size =prefSize;
    //    toolContainer.Dock = DockStyle.Top;
        
    //    if (toolWindow.State == Bentley.Windowing.Docking.ZoneState.Floating)
    //    {
    //        Form form = toolWindow.FloatingHostForm;
    //        form.Padding = new Padding(0);
    //        form.MinimumSize =  new System.Drawing.Size(
    //            prefSize.Width + 12,
    //            prefSize.Height + form.PreferredSize.Height);
            
    //        /// заставит принять оптимальные размеры
    //        form.Size = new System.Drawing.Size(0, 0);
    //        form.Disposed += Form_Disposed;
    //    }
    //    toolWindow.Show();
    //}

    bool ToolWindowClosingInProgress;

    /// <summary> Переключение режима редактирования </summary>
    private void ToolContainer_OnToolEdit(bool active)
    {
        this.SetLocateCursor(active);
        ActionMode = active ? ToolActionMode.FindEdit : ToolActionMode.CreateNew;
        _points?.Clear();
    }

    public HitPath DoLocate(DgnButtonEvent ev, bool newSearch, ComponentMode mode)
    {
        return this.DoLocate(ev, newSearch, (int)mode);
    }

}

}