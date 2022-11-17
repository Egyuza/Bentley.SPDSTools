using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using Bentley.ECObjects.Schema;
using System.Globalization;
using Bentley.ECObjects.Instance;
using System.Runtime.InteropServices;

namespace SPDSTools
{

public abstract class ToolElement : IDisposable
{
   [DllImport("stdmdlbltin.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int mdlTextStyle_addToFile(
        ref int pNewTableEntryId, IntPtr pStyle, IntPtr pStyleName, int bLockEntry); 

    /// <summary>
    /// Цвет заимствуется от цвета фона модели
    /// </summary>
    public const int COLOR_AS_BACKGROUND = 255;
    
    public ToolData Data { get; private set; }

    public event EventHandler SelectedToolCellForEdit;

    public abstract ECClassTypeEnum ToolECClassType {get;}       
    protected abstract void OnWriteProperties(ECPropertyWriter writer);
    //protected abstract void OnReadProperties(ECPropertyReader reader);
    

    /// <summary> 
    /// Требуемое количество точек для построения элемента 
    /// </summary>
    public abstract int ConstractionPointsCount { get; }
    
    // TODO ? можно ли отказаться от хранения model и файл?
    public DgnModel model { get; private set; }
    public DgnFile file { get; private set; }

    public string ToolName { get { return EnumString.ToString(ToolECClassType); } }

    private DisplayableElement _target;
    public DisplayableElement Target 
    {
        get 
        {
            if (_target != null)
                return _target;
            if (Data?.TargetId != null)
            {
                if (Data.TargetId.AttachmentId != null)
                {
                    var attachment = model.FindDgnAttachmentByElementId(
                        Data.TargetId.AttachmentId);
                    if (attachment != null)
                    {
                        return attachment.GetDgnModel().FindElementById(
                            Data.TargetId.ElementId) as DisplayableElement; 
                    }
                    return model.FindElementById( 
                        Data.TargetId.ElementId) as DisplayableElement;
                }
            }            
            return null;
        }
        set { _target = value; }
    }

    public void setTarget(DisplayableElement target)
    {
        this.Target = target;
    }
    
    /// <summary>
    /// Cell элемент, публикуемый в модель чертежа
    /// </summary>
    public CellHeaderElement Cell { get; private set; }
    public CellHeaderElement OriginalCell { get; private set; }
    
    // КОНСТРУКТОР
    protected ToolElement(ToolData data)
    {        
        // todo правильное получение модели
        model = Session.Instance.GetActiveDgnModel();
        file = Session.Instance.GetActiveDgnFile();
        OriginalCell = data.OriginalCell;
        this.Data = data;
        if (data.Points?.Count > 0)
            LoadByPoints(data.Points);
    }

    public void Dispose()
    {
        // TODO если потребуется
        
    }
   

    /// <summary>
    /// Диапазон элемента тула, увеличенный относительно реального на 5%
    /// </summary>
    /// <returns></returns>
    public DRange3d GetExRange()
    {
        if (Cell == null)
            return DRange3d.NullRange;

        DRange3d range;
        Cell.CalcElementRange(out range);

        foreach(DPoint3d point in Points)
        {
            if (!range.ContainsXY(point))
                range.Extend(point);
        }

        range.ScaleAboutCenter(1.05);
        return range;
    }

    protected List<DPoint3d> Points { get; private set; }
    public List<DPoint3d> SnapPoints { get; protected set; }

    protected abstract CellHeaderElement OnLoadByPoints(List<DPoint3d> points);

    public CellHeaderElement LoadByPoints(IEnumerable<DPoint3d> points)
    {    
        Points = new List<DPoint3d> (points);
        Cell = OnLoadByPoints(Points);
        Data?.setPoints(points);
        return Cell;
    }

    private RedrawElems _redrawElems;
    
    public void DoRedraw(Viewport viewPort)
    {
        if (Cell == null)
            return;
        
        if (_redrawElems == null)
        {
            _redrawElems = new RedrawElems();
            _redrawElems.DrawMode = DgnDrawMode.TempDraw;
            _redrawElems.DrawPurpose = DrawPurpose.Dynamics;
            // todo продумать по поводу ViewPort
            _redrawElems.SetDynamicsViewsFromActiveViewSet(viewPort);
        }
        
        _redrawElems.DoRedraw(Cell);        
    }
            
    //protected abstract void OnLoadFromCell(CellHeaderElement cell,
    //    out List<DPoint3d> points);

    public delegate void EventHandler();
    /// <summary>
    /// Событие загрузки тула из ранее опубликованого
    /// </summary>
    public event EventHandler LoadedFromCellForEditEvent;

    private ElementAgendaDisplayable agenda;


    // todo удалить?
    //public ComplexHeaderElement LoadFromCellForEdit(CellHeaderElement cell, 
    //    bool notificate = false)
    //{
    //    OriginalCell = cell;
    //    List<DPoint3d> points = null;
    //    Target = null;

    //    // readPropertiesFromCell(cell);
        
    //    OnLoadFromCell(cell, out points);
    //    _cell = LoadByPoints(points);

    //    if (notificate)
    //    {
    //        if (agenda != null)
    //            agenda.ClearHilite();
    //        agenda = new ElementAgendaDisplayable();
    //        agenda.Insert(cell, true);
    //        agenda.Hilite();

    //        if (LoadedFromCellForEditEvent?.GetInvocationList().Length > 0)
    //            LoadedFromCellForEditEvent.Invoke();
    //    }

    //    return _cell;
    //}
       
    //private void readPropertiesFromCell(CellHeaderElement cell)
    //{
    //    ECPropertyReader reader = ECPropertyReader.TryGet(cell, ECClassTypeEnum.Common);
    //    IECPropertyValue propVal = reader.Get(ECPropertyName.Common.TargetId);

    //    if (propVal != null && !propVal.IsNull)
    //    {
    //        long id = long.Parse(propVal.StringValue);
    //        var elementId = new ElementId(ref id);

    //        long att_id = -1;
    //        ElementId attachmentId = new ElementId();
    //        {
    //            propVal = reader.Get(ECPropertyName.Common.TargetReferenceId);
    //            if (propVal != null && !propVal.IsNull 
    //                && !string.IsNullOrWhiteSpace(propVal.StringValue))
    //            {
    //                att_id = long.Parse(propVal.StringValue);
    //                attachmentId = new ElementId(ref att_id);
    //            }        
    //        }

    //        Element targetEl = model.FindElementById(elementId);
    //        if (targetEl == null)
    //        {
    //            DgnAttachmentCollection attachments = model.GetDgnAttachments();
    //            foreach (DgnAttachment attachment in attachments)
    //            {
    //                if (att_id >= 0 && attachment.GetElementId() != attachmentId)
    //                    continue;

    //                // TODO проверить!
    //                targetEl = attachment.GetDgnModel().FindElementById(elementId);
    //                if (targetEl != null)
    //                    break;                    
    //            }
    //            if (targetEl != null && targetEl is DisplayableElement)
    //                Target = targetEl as DisplayableElement;
    //            else
    //                Target = null;
    //        }
    //    }

    //    { // IsAnnotaion
    //        propVal = reader.Get(ECPropertyName.Common.IsAnnotation);
    //        IsAnnotaion = (propVal == null)
    //            ? true
    //            : propVal.BooleanValue();
    //    }

    //    { // IsMerged
    //        propVal = reader.Get(ECPropertyName.Common.IsMerged);
    //        IsMerged = (propVal == null) ? false : propVal.BooleanValue();
    //    }

    //    OnReadProperties(ECPropertyReader.TryGet(cell, ToolECClassType));
    //}

        /// <summary> Model AnnotationScaleFactor </summary>
    protected double GetAnnScale 
    { 
        get 
        { 
            return !Settings.UseAnnotationScale ? 
                1 :
                model.GetModelInfo().AnnotationScaleFactor; 
        } 
    }

    /// <summary> 
    /// Коэффициент перевода измерения с учётом установленного разрешения 
    /// </summary>
    protected double UorPerMeter 
    { 
        get { return model.GetModelInfo().UorPerMeter; } 
    }

    /// <summary> 
    /// Результирующий масштабный коэффициент для перевода геометрии в 
    /// миллиметрах на модель чертежа
    /// </summary>
    /// UOR - Units of resolution - разрешение dgn-файла, на основе кот.
    /// расчитываются все координаты модели
    public double Scale
    {
        get { return GetAnnScale * (model.GetModelInfo().UorPerMeter / 1000) /*UorPerMillimetr*/; }
    }


    /// <summary>
    /// Приведение значения к текущему масштабу аннотаций модели
    /// </summary>
    public double Scaled(double value)
    {
        return value*Scale;
    }

    /// <summary>
    /// Получение действительного значения
    /// </summary>
    public double UnScaled(double value)
    {
        return value / Scale;
    }
                
    protected virtual void OnAfterAddToModel(CellHeaderElement cell) {}  

    public StatusInt ReplaceInModel(CellHeaderElement replacement)
    {
        OriginalCell = replacement;
        return AddToModel();
    }

    public StatusInt AddToModel()
    {   
        StatusInt addToModelStatus;

        if (Cell == null)
        {
            SPDSException.AlertIfDebug("Попытка добавить в модель пустой элемент");
            return StatusInt.Error;
        }

        if (OriginalCell != null) {
            SPDSTools.Set_ElementChangeProcessing(true);
            addToModelStatus = Cell.ReplaceInModel(OriginalCell);
            SPDSTools.Set_ElementChangeProcessing(false);
        }
        else
        {
            addToModelStatus = Cell.AddToModel();
        }

        if (addToModelStatus != StatusInt.Success)
            return addToModelStatus;
        
        //writeTest();
        writeProperties();

        Element root = Target;
        if (root != null)
        {
            Element rootEl = null;
            bool success = GetRootAssociated(Cell, out rootEl);
            if (!success)
                makeAssociated(Cell, root, model);

#if DEBUG
            // проверка
            // todo delete test
            {
                success = GetRootAssociated(Cell, out rootEl);
            }
#endif
            
        }

        OnAfterAddToModel(Cell);
        OriginalCell = null; // cброс

        return addToModelStatus;
    }

    private static void makeAssociated(Element element, Element root, DgnModel model)
    {
        ElementId referenceId;
        DgnAttachment attachment = root.DgnModel.AsDgnAttachmentOf(element.DgnModel);
        if (attachment == null)
            referenceId = new ElementId(); // = 0 - значит в текущей модели
        else // целевой элемент принадлежит ссылочной модели
            referenceId = attachment.GetElementId();

        AssociativePoint assPoint = new AssociativePoint();
        assPoint.InitializeOrigin(0);

        BentleyStatus status = assPoint.SetRoot(root.ElementId, referenceId, 0);
        if (status == BentleyStatus.Success)
        {
            StatusInt res = assPoint.InsertPoint(element, 0, 1);
            //res = assPoint.InitializeFromElement(element, 0, 1);
            //res =  element.ReplaceInModel(element);
        }
    }

    private bool GetRootAssociated(Element element, out Element root)
    {        
        root = null;
        var assPoint = new AssociativePoint();
        if (assPoint.InitializeFromElement(element, 0, 1) == StatusInt.Success)
        {
            DisplayPath dispPath = new DisplayPath();
            int numRoots;
            assPoint.GetRoot(dispPath, out numRoots, element.DgnModel, 0);
            root = dispPath.GetHeadElement();            
        }
        return root != null;
    }

    private void writeTest()
    {
        try
        {
            //IECSchema schema = Bentley.DgnPlatformNET.DgnEC.DgnECManager.Manager.LocateDeliveredSchema(
            //    "IGN_General", 1, 0, SchemaMatchType.LatestCompatible, file);

            IECSchema schema = ECHelper.GetSchema(file, "IGN_General");
        
            var writer = ECPropertyWriter.Get(
                Cell, schema,  ECClassTypeEnum.UIDElement);
        
            writer?.SetValue("UID", "1234");
            writer?.SaveOnElement();
        }
        catch (Exception)
        {

        }
    }

    private void writeProperties()
    {
        IECSchema schema =  ECHelper.GetSchema(file);    
                           
        // Записываем общие свойства для всех типов тула:
        // тип:               
        var commonPropWriter = ECPropertyWriter.Get(
            Cell, schema,  ECClassTypeEnum.Common);
        
        var toolPropWriter = ECPropertyWriter.Get(
            Cell, schema, ToolECClassType);
                
        if (commonPropWriter == null || toolPropWriter == null)        
            throw new SPDSException("Couldn't write properties on element");        

        commonPropWriter.SetValue(
            ECPropertyName.Common.Type, EnumString.ToString(ToolECClassType));

        try
        {
            // записываем тип
            commonPropWriter.SetValue(ECPropertyName.Common.Type, 
                ToolECClassType.ToString());

            if (Target != null)
            {
                commonPropWriter.SetValue(ECPropertyName.Common.TargetId, 
                    (long)Target.ElementId);

                DgnAttachment att;
                if ((att = Target.DgnModel.AsDgnAttachmentOf(model)) != null) 
                {
                    commonPropWriter.SetValue(
                        ECPropertyName.Common.TargetReferenceId, 
                        (long)att.GetElementId());
                }            
            }
            
            commonPropWriter.SetValue(ECPropertyName.Common.IsAnnotation,
                Data.IsAnnotaion);
            commonPropWriter.SetValue(ECPropertyName.Common.AnnotationScale, 
                GetAnnScale);
            commonPropWriter.SetValue(ECPropertyName.Common.IsMerged, 
                Data.IsMerged);

            // запись специальных для тула совойств:
            OnWriteProperties(toolPropWriter);
            // привязка свойств к элементу
            toolPropWriter.SaveOnElement();
            commonPropWriter.SaveOnElement();
        }
        catch (Exception)
        {
            // todo продумать обработку исключения
            MessageCenter.Instance.ShowErrorMessage("SPDSTools error",
                "Error on write EC-properties to cell", true);
        }   
    }

    public static bool Load(ToolData data, out ToolElement toolElement)
    {
        toolElement = null;
        if (data is HeightToolData)
            toolElement = new HeightToolElement(data as HeightToolData);
        else
            toolElement = null;

        return toolElement != null;
    }
  
}
}
