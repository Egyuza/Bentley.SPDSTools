using System;
using System.Linq;
using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET;

using System.Collections.Generic;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET.XDataTree;
using Bentley.DgnPlatformNET.XDataTree;

using System.Globalization;
using System.Threading;
using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;

namespace SPDSTools
{
[Bentley.MstnPlatformNET.AddInAttribute(MdlTaskID = "SPDSTools")]
public sealed class SPDSTools: Bentley.MstnPlatformNET.AddIn                 
{
    private static SPDSTools s_app;       
    
    // КОНСТРУКТОР
    public SPDSTools(System.IntPtr mdlDesc)
        : base(mdlDesc)
    {
        s_app = this;
            
        this.ModelChangedEvent += SPDSTools_ModelChangedEvent;
        this.ElementChangedEvent += SPDSTools_ElementChangedEvent;

        this.ReferenceDetachedEvent += SPDSTools_ReferenceDetachedEvent;
        this.BeforeNewDesignFileEvent += SPDSTools_BeforeNewDesignFileEvent;
        this.ReloadEvent += SPDSTools_ReloadEvent;
        
        this.SelectionChangedEvent += SPDSTools_SelectionChangedEvent;
        
        // todo : найти способ определить язык приложения
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

        try
        {
            //Bentley.ECObjects.ECObjects.Initialize();
            //var type = Bentley.ECObjects.ECObjects.DateTimeType;
            //Bentley.ECObjects.ECObjects.in
            //DateTime date;
            //Bentley.ECObjects.ECObjects.StringToDateTime(out date, "2459275.9130568979");
            ;
        }
        catch (Exception)
        {

            
        }
    }

    private void SPDSTools_SelectionChangedEvent(
        AddIn sender, SelectionChangedEventArgs eventArgs)
    {
        // TODO REMOVE AFTER TEST
        Element element = null;
        DgnModelRef modelRef = eventArgs.DgnModelRef;

        if (SelectionSetManager.NumSelected() == 1 &&
            StatusInt.Success == SelectionSetManager.GetElement(0,
                ref element, ref modelRef))
        {
            ECPropertyReader.Test(element);
        }

        if (ToolFormHelper.ToolWindow == null &&
            eventArgs.Action == SelectionChangedEventArgs.ActionKind.DoubleClickElement)
        {
            uint index = eventArgs.FilePosition;


            if (SelectionSetManager.NumSelected() == 1 &&
                StatusInt.Success == SelectionSetManager.GetElement(0,
                    ref element, ref modelRef))
            {
                ECPropertyReader reader =
                    ECPropertyReader.TryGet(element, ECClassTypeEnum.HeightTool);
                if (reader != null)
                {
                    SelectionSetManager.EmptyAll();

                    // запуск тула Высотной отметки:
                    Keyin.cmd_HeightTool();

                    // после выделения одного элемента должен автоматически
                    // запуститься режим его редактирования:
                    SelectionHelper.Instance.setIsActive(true);
                    SelectionHelper.Select(element, false);
                    //SelectionSetManager.AddElement(element, modelRef);
                    // ModifyTool.Instance?.TrySelect(element);
                }
            }
        }
    }

    private void SPDSTools_ReloadEvent(AddIn sender, ReloadEventArgs eventArgs)
    {
        clearForMergeIntoMaster();
    }

    private void SPDSTools_BeforeNewDesignFileEvent(AddIn sender, BeforeNewDesignFileEventArgs eventArgs)
    {
       clearForMergeIntoMaster();
    }

    private void SPDSTools_ReferenceDetachedEvent(AddIn senderIn, ReferenceDetachedEventArgs eventArgsIn)
    {
        if ((int)eventArgsIn.Cause == 6) // MergeIntoMasterCause
        {               
            IECSchema schema = 
                ECHelper.GetSchema(eventArgsIn.DgnAttachment.GetDgnFile());

            foreach (CellHeaderElement cell in MergeIntoMasterElements)
            {
                if (!cell.IsSPDSElement()) // НВС
                    continue;
                    
                ECPropertyWriter writer = 
                    ECPropertyWriter.Get(cell, schema, ECClassTypeEnum.Common);
                // запись текущего масштаба аннотаций модели
                writer.SetValue(ECPropertyName.Common.IsMerged, true);
                writer.SaveOnElement();                
            }
            clearForMergeIntoMaster();
        }
    }

    internal static SPDSTools Instance
    {
        get { return s_app; }
    }

    /*------------------------------------------------------------------------------------**/
    /// <summary>Required Run method of AddIn class.</summary>
    /// <author>BentleySystems</author>
    /*--------------+---------------+---------------+---------------+---------------+------*/
    protected override int Run(string[] commandLine)
    {
        return 0;
    }

#region MODEL_CHANGED_EVENT
    
    double previousAnnScale;
    static bool ModelChangedEvent_Processing;
    static ModelChangedEventArgs modelChangedEventArgs;
    private void SPDSTools_ModelChangedEvent(AddIn senderIn, ModelChangedEventArgs eventArgsIn)
    {
        clearForMergeIntoMaster();

        if (ModelChangedEvent_Processing)
            return;
        try
        {
            ModelChangedEvent_Processing = true;
            modelChangedEventArgs = eventArgsIn;
            OnModelChangedEvent(eventArgsIn);
        }
        catch(Exception ex)
        {
            MessageCenter.Instance.ShowErrorMessage(
                "SPDSTools: error on processing ModelChangedEvent", 
                ex.Message, false);
        }
        finally
        {
            ModelChangedEvent_Processing = false;
        }    
    }

    private void OnModelChangedEvent(ModelChangedEventArgs eventArgsIn)
    {
        if (eventArgsIn == null)
            return;

        DgnModel model = eventArgsIn.DgnModelRef?.GetDgnModel();
        DgnFile file = eventArgsIn.DgnModelRef?.GetDgnFile();
                
        if (model == null) // НВС
            return;

        if (eventArgsIn.Change == AddIn.ModelChangedEventArgs.ChangeType.BeforeProperties)
            previousAnnScale = model.GetModelInfo().AnnotationScaleFactor;
        
        if (eventArgsIn.Change == 
                AddIn.ModelChangedEventArgs.ChangeType.PropagateAnnotationScale
            && Settings.UseAnnotationScale)
        {   
            // Событие изменения масштаба анотаций:

            List<CellHeaderElement> cells = 
                ECHelper.FindSPDSElementsByInstance(model);           

            // НВС
            List<ElementId> doneList = new List<ElementId>();

            IECSchema schema = ECHelper.GetSchema(file);

            foreach(CellHeaderElement cell in cells)
            {
                if (doneList.Contains(cell.ElementId))
                    continue;

                processScalingCell(cell, model, schema);
                doneList.Add(cell.ElementId);                
            }      
        }
    }

    /// <summary>
    /// обработка изменения маштаба spds-элемента
    /// </summary>
    private void processScalingCell(CellHeaderElement cell, DgnModel model, 
        IECSchema schema)
    {
        if (cell == null)
            return;       
        

        
        DPoint3d origin;
        cell.GetSnapOrigin(out origin);   
                
        double? currentAnnScale = null;                

        { // для поддержки совместимости с элементами с аннотативным текстом:
            TextElement txtEl = 
                cell.GetChildren().First(x => x is TextElement) as TextElement;

            if (txtEl != null)
            {
                double value;
                if (txtEl.HasAnnotationScale(out value))    
                    currentAnnScale = value;
            }
        }

        if (!currentAnnScale.HasValue) // читаем из EC-свойств
        {
            ECPropertyReader reader =
                ECPropertyReader.TryGet(cell, ECClassTypeEnum.Common);
            if (reader != null)
            {
                IECPropertyValue propVal =
                    reader.Get(ECPropertyName.Common.AnnotationScale);
                if (propVal != null && !propVal.IsNull && propVal.DoubleValue != 0)
                    currentAnnScale = propVal.DoubleValue;
            }
        }

        double modelAnnScale = model.GetModelInfo().AnnotationScaleFactor;
        double reqScale = modelAnnScale;

        if (currentAnnScale.HasValue)        
            reqScale /= currentAnnScale.Value; // корректируем
                
        bool expose = cell.ExposeChildren(ExposeChildrenReason.Edit);                        
        List<Element> listElements = new List<Element>();
                        
        foreach (Element el in cell.GetChildren())
        {
            if (!(el is TextElement))
                listElements.Add(el);
            else
            {
                TextElement txtEl = el as TextElement;

                //TextQueryOptions opt = new TextQueryOptions();                            
                //TextPartIdCollection tpcoll = txtEl.GetTextPartIds(opt);
                //TextBlock txtBlock =  txtEl.GetTextPart(tpcoll[0]).Clone();
                //txtBlock.GetProperties().ApplyTextStyle(txtEl.GetTextStyle(), annScale, true);
                                
                //TextHandlerBase textBase = TextHandlerBase.CreateElement(null, txtBlock);
                //DPoint3d txtOrigin;
                //    textBase.GetSnapOrigin(out txtOrigin);
                
                double textAnnScale;
                if (txtEl.HasAnnotationScale(out textAnnScale))
                {
                    DPoint3d txtOrigin;
                    txtEl.GetSnapOrigin(out txtOrigin);
                    var tr3d = DTransform3d.FromUniformScaleAndFixedPoint(
                        txtOrigin, 1/textAnnScale);
                    txtEl.ApplyTransform(new TransformInfo(tr3d));
                }
                listElements.Add(txtEl);
            }
        }
                    
        CellHeaderElement newCell = new CellHeaderElement(model, 
            cell.CellName, origin, DMatrix3d.Identity, listElements);

        //newCell.ReplaceInModel(cell);

        { // ИЗМЕНЕНИЕ МАСШТАБА тула:
            var tr3d = DTransform3d.FromUniformScaleAndFixedPoint(
                origin, reqScale);
            newCell.ApplyTransform(new TransformInfo(tr3d));
        }

        //newCell.ReplaceInModel(newCell);
        newCell.ReplaceInModel(cell);

        ECPropertyWriter writer =
            ECPropertyWriter.Get(newCell, schema, ECClassTypeEnum.Common);
        // запись текущего масшатаба аннотаций модели
        writer.SetValue(ECPropertyName.Common.AnnotationScale, modelAnnScale);
        writer.SaveOnElement();
        //newCell.ReplaceInModel(newCell);
    }

#endregion

#region ELEMENT_CHANGED_EVENT

    static bool OnElementChangedEvent_Processing;
    private void SPDSTools_ElementChangedEvent(AddIn sender, AddIn.ElementChangedEventArgs eventArgs)
    {
        if (OnElementChangedEvent_Processing)
            return;
                      
        OnElementChangedEvent_Processing = true;
        try
        {
            OnElementChangedEvent(eventArgs);
        }
        finally
        {
            OnElementChangedEvent_Processing = false;
        }
    }

    internal static void Set_ElementChangeProcessing(bool value)
    {
        OnElementChangedEvent_Processing = value;
    }

    private void clearForMergeIntoMaster()
    {
        WaitingMergeIntoMasterElemIds.Clear();
        MergeIntoMasterElements.Clear();
    }

    /// <summary>
    /// Список id элементов, подготовленных к операции MergeToMaster из референса
    /// </summary>
    private List<ElementId> WaitingMergeIntoMasterElemIds = new List<ElementId>();
    private List<CellHeaderElement> MergeIntoMasterElements = new List<CellHeaderElement>();
    
    private void OnElementChangedEvent(ElementChangedEventArgs eventArgs)
    {
        if (eventArgs.NewElement == null)
            return;
         AssociativePoint assPoint = new AssociativePoint();
        
        DependencyManager.ProcessAffected(); //RootChanged(eventArgs.NewElement);
        
        CellHeaderElement cell = eventArgs.NewElement as CellHeaderElement;
        if (cell == null)
            return;

        short funcName = eventArgs.Info.FunctionName;

        /// ! Из эмпирического опыта: операции Attachment Merge Into Master
        /// референс-модели соответствуют след. значения "флагов"
        ///     • 1 этап - {FunctionName = 2, Change = Add}
        ///     • 2 этап - {FunctionName = 2, Change = 4}        
    
        // from MicroStationAPI DgnPlatform.h header file:
        /// enum class ChangeTrackAction
        /// {
        ///     Delete            = 1,  //!< An element was deleted from the file.
        ///     Add               = 2,  //!< An element was added to the file.
        ///     Modify            = 3,  //!< An existing element was changed and rewritten to the file in place.
        ///     AddComplete       = 4,  //!< An element (and its xattributes) were added to the file.
        ///     ModifyFence       = 5,  //!< The fence was modified.
        ///     Mark              = 7,  //!< Used to delineate commands in undo buffer.
        ///     ModelAdd          = 9,  //!< A model was added to the file.
        ///     ModelDelete       = 10, //!< A model was deleted from the file.
        ///     XAttributeAdd     = 11, //!< An XAttribute was addeed to an element.
        ///     XAttributeDelete  = 12, //!< An XAttribute was deleted from an element.
        ///     XAttributeModify  = 13, //!< some part of the XAttribute was modified.
        ///     XAttributeReplace = 14, //!< An XAttribute was replaced.
        ///     ModelPropModify   = 15, //!< A model's properties were modified.
        ///     CustomEntry       = 16, //!< Application data held in the undo buffer (\em not an element-level change to the file).
        ///     ModifyComplete    = 17, //!< An existing element (and its xattributes) were changed and rewritten to the file in place.
        ///     Last              = ChangeTrackAction::ModifyComplete,
        /// };

        // ADD:
        if (eventArgs.Change == ChangeTrackKind.Add && funcName == 2)
        {
            if (!WaitingMergeIntoMasterElemIds.Contains(cell.ElementId))
            {
                WaitingMergeIntoMasterElemIds.Add(cell.ElementId);
                return;
            }
        }
        else if ((int)eventArgs.Change == 4 && funcName == 2)
        {
             /// funcName == 25 - здесь считаем, что над элементом
             /// выполняется операция MergeIntoMaster:
            if (WaitingMergeIntoMasterElemIds.Contains(cell.ElementId))
            {
                /// 
                if (ECHelper.IsSPDSElement(cell))
                    MergeIntoMasterElements.Add(cell);
                
                WaitingMergeIntoMasterElemIds.Remove(cell.ElementId);
                return;
            }
        }
        // MODIFY:
        else if (eventArgs.Change == ChangeTrackKind.Modify || 
            ((int)eventArgs.Change == 4 && (funcName == 25 || funcName == 52)))
        {
        // funcName == 25 - функция копирования
        // funcName == 52 - массив

            if (!ModelChangedEvent_Processing ||  modelChangedEventArgs.Change !=
                ModelChangedEventArgs.ChangeType.PropagateAnnotationScale)
            {
                if (!ECHelper.IsSPDSElement(cell))
                    return;                
                
                    // TODO разделение
                ToolElement toolElement = null;
                Tool tool = Keyin.ActiveTool ?? new HeightTool();

                switch (ECHelper.GetToolType(cell))
                {
                case ECClassTypeEnum.HeightTool:
                    toolElement = new HeightToolElement(new HeightToolData(cell)); 
                    break;
                case ECClassTypeEnum.LeaderTool:
                    toolElement = new LeaderToolElement(new HeightToolData(cell)); 
                    break;  // TODO
                }

                if (toolElement != null)
                {         
                    // toolElement.LoadFromCellForEdit(cell, true);
                    toolElement.AddToModel();
                }
                else
                {
                    MessageCenter.Instance.ShowErrorMessage(
                        "SPDSTools element changing error",
                        "SPDSTools element was changed, but without interception and processing",
                        true);
                }                
            }
        }

        if (WaitingMergeIntoMasterElemIds.Contains(cell.ElementId))
            WaitingMergeIntoMasterElemIds.Remove(cell.ElementId);       
    }
#endregion
    

}
}
