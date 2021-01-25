using System;
using System.Collections.Generic;
using System.Linq;

using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;

using SPDSTools.Parameters;
using Bentley.GeometryNET;
using Bentley.ECObjects.Instance;

using HP = SPDSTools.Parameters.HeightParametersEnum;
using Bentley.MstnPlatformNET;

namespace SPDSTools
{

public struct DefaultData
{  
public uint
    Plan_PointFillColor;
public double
    GapArrowAfter, // Выступ направляющей от стрелки
    GapStart, // Отступ до нач. линии уровня высотной отметки
    Arrow_dX, // половина ширины указателя стрелки
    Arrow_dY, // высота указателя стрелки
    LandingHeight, // высота полки
    Base_LenBeforeArrow, // базовая длина до стрелки
    Plan_PointRadius; // радиус точки - указателя выносной плановой отметки
}

public struct PaksData
{
public uint ArrowFillColor;
public int Sec_LineStyle;
public double 
    Plan_ArrowRadius,
    Plan_ArrowRadius2,
    Sec_Arrow_dX,
    Sec_Arrow_dY, // высота треугольного указателя
    LandingLength;
}

public class HeightToolData: ToolData
{
    public ToolStyleEnum ToolStyle { get; private set; }
    public HeightToolTypeEnum Type { get; private set; }

    /// <summary> Текст на полке </summary>
    public string Text { get; private set; }
    /// <summary> Текст под полкой </summary>
    public string Text2 { get; private set; }

    public bool IsLoadedFromCell { get; private set; }
    
    public void setText(string text) 
    { 
        Text = text; 
    }
    public void setText2(string text) 
    {
        Text2 = text; 
    }
       

    public AlignmentEnum Alignment { get; private set; }

    // индивидуальные для цела:
    public bool AutoCalc { get; private set; }
    public bool CalcByItem { get; private set; }
    public bool ShowItemHeight { get; private set; }
    public bool AsLeader { get; private set; }
    public bool UseFilling { get; private set; }
    public bool SectorArrow { get; private set; }

    public override List<DPoint3d> SnapPoints 
    {
        get 
        {
            return Points;
        }
    }
    
    private double GetScale(DgnModel model)
    {
        double annScaleFactor = Settings.UseAnnotationScale
        ? model.GetModelInfo().AnnotationScaleFactor
        : 1;
        return annScaleFactor * (model.GetModelInfo().UorPerMeter / 1000);
    }

    public HeightToolData(DgnModel model) : base(HeightParameters.Instance)
    {
    ///! Определяются параметры тула только на основе конгфиг. параметров!

        IsLoadedFromCell = false;

        double scale = GetScale(model);

        HeightParameters pars = HeightParameters.Instance;

        ToolStyle = (ToolStyleEnum)pars[HP.STYLE].EnumValue;
        UpdatFromParameters();
    }

    public void UpdatFromParameters()
    {
        HeightParameters pars = HeightParameters.Instance;
        
        Type = (HeightToolTypeEnum)pars[HP.TYPE].EnumValue;
        Text = pars[HP.TEXT].StringValue;
        Text2 = pars[HP.TEXT2].StringValue;

        AutoCalc = pars[HP.AUTOCALC].BoolValue;
        CalcByItem = pars[HP.CALCBYITEM].BoolValue;
        Alignment = (AlignmentEnum)pars[HP.ALIGNMENT].EnumValue;
        AsLeader = pars[HP.AS_LEADER].BoolValue;
        UseFilling = pars[HP.USE_FILLING].BoolValue;
        SectorArrow = pars[HP.SECTOR_ARROW].BoolValue;

        ShowItemHeight = pars[HP.SHOW_ITEMHEIGHT].BoolValue;
    }

    /// <summary>
    /// Определение данных на основании чтения спдс-элемента
    /// </summary>
    public HeightToolData(CellHeaderElement cell) : base(cell)
    {
        ///! Определяются параметры тула вне зависимости от конгфиг. параметров!
        // а только из параметров прочитанного цела              

        IsLoadedFromCell = true;

        HeightParameters pars = HeightParameters.Instance;

        Alignment = AlignmentEnum.Auto;

        var points = new List<DPoint3d>();
        switch (ToolStyle)
        {
        case ToolStyleEnum.Default:
            loadFromCell_Default(cell, out points); break;

        case ToolStyleEnum.Paks:
            loadFromCell_Paks(cell, out points); break;

        case ToolStyleEnum.ED: 
             
            if (Type == HeightToolTypeEnum.Planned)
                 // Плановая отметка наследует настройки от ПАКШ
                loadFromCell_Paks(cell, out points);
            else // Разрезная наследует от Default
                loadFromCell_Default(cell, out points);

            break; 
        default:
            throw new SPDSException("Unimpelemented ToolStyle");
        }

        Points = points;

        if (!AutoCalc)
        { // Изменеённый текст:
            int i = 0;
            foreach (Element element in cell.GetChildren().Where(
                x => x is TextElement))
            {
                if (i == 0)    
                {
                    Text = ((TextElement)element).GetText();
                    ++i;
                }            
                else if (ShowItemHeight)
                {
                    Text2 = ((TextElement)element).GetText();
                    break;
                }
            }
        }
    }

#region Чтение СПДС-элемента
    protected override void OnReadPropertiesFromCell(ECPropertyReader reader)
    {
        IECPropertyValue propVal = null;

        if ((propVal = reader.Get(ECPropertyName.Height.Style)) != null)
        {
            ToolStyle = (ToolStyleEnum)propVal.IntValue;
            switch (ToolStyle)
            {
            case ToolStyleEnum.Default:
                readProperties_Default(); break;
            case ToolStyleEnum.Paks:
            case ToolStyleEnum.ED: // наследует настройки от ПАКШ
                readProperties_Paks(); break;
            default:
                 throw new SPDSException("Unsuspected ToolStyle");
            }
        }

        // todo можно заменить на: bool Get(string name, out PropertyValue propVal)
        if ((propVal = reader.Get(ECPropertyName.Height.Text)) != null)
            Text = propVal.StringValue;
        if ((propVal = reader.Get(ECPropertyName.Height.Text2)) != null)
            Text2 = propVal.StringValue;
        if ((propVal =  reader.Get(ECPropertyName.Height.AutoCalc)) != null)
            AutoCalc = propVal.BooleanValue();
        if ((propVal =  reader.Get(ECPropertyName.Height.CalcByItem)) != null)
            CalcByItem = propVal.BooleanValue();
        if ((propVal =  reader.Get(ECPropertyName.Height.ShowItemHeight)) != null)
            ShowItemHeight = propVal.BooleanValue();
        if ((propVal = reader.Get(ECPropertyName.Height.Type)) != null)        
            Type = (HeightToolTypeEnum)propVal.IntValue;
        if ((propVal = reader.Get(ECPropertyName.Height.AsLeader)) != null)
            AsLeader = propVal.BooleanValue();
        if ((propVal = reader.Get(ECPropertyName.Height.UseFilling)) != null)
            UseFilling = propVal.BooleanValue();
        if ((propVal = reader.Get(ECPropertyName.Height.SectorArrow)) != null)
            SectorArrow = propVal.BooleanValue();

        // !
        if (IsMerged)
        {
            AutoCalc = // выключаем
            IsMerged = false; // сбрасываем
        }
    }
    private void readProperties_Paks()
    {
        // todo readProperties_Paks
    }
    private void readProperties_Default()
    {
        // todo readProperties_Default        
    }
    
    private void loadFromCell_Default(CellHeaderElement cell, out List<DPoint3d> points)
    {
        /// Принцип чтения элемента заключается в:
        /// 1. определении точек построения, стиля текста и ориентации;
        /// 2. остальные параметры строятся в соответствии с 
        ///    текущими настройками;        
        
        DgnModel model = cell.DgnModel;
        

        // базовая точка - точка указателя
        DPoint3d origin = cell.GetOrigin();

        // Точки построения
        points = new List<DPoint3d>() { origin };
        List<Element> childs = cell.GetChildren().ToList();        
        
        //DefaultData defData = new DefaultData();

        if (Type == HeightToolTypeEnum.Planned)
        {   // PLANNED

            ///   _______
            ///  | +4,86 | 
            ///   ‾‾‾‾‾‾‾

            if (AsLeader)
            {
            ///        _______
            ///  -----| +4,86 | 
            ///        ‾‾‾‾‾‾‾
            // по двум точкам
                LineStringElement line = childs.FirstCast<LineStringElement>();
                if (line != null)
                {
                    List<DPoint3d> pts = new List<DPoint3d>();
                    line.GetCurveVector().GetPrimitive(0).TryGetLineString(pts);
                    points.Add(pts[1]);
                }

                //EllipseElement circle  = childs.FirstCast<EllipseElement>(); 
                //if (circle != null)
                //{
                //    bool filled; uint color;
                //    circle.GetSolidFill(out color, out filled);
                //    defData.Plan_PointFillColor = color;
                //    defData.Plan_PointRadius = circle.GetRange().XSize / 2;
                //}
            }
        }
        else
        { // SECTION

            ///                #,###
            ///             |‾‾‾‾‾‾‾‾‾
            ///            \|/           
            /// ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾

            var strLines = childs.Where(
                x => x is LineStringElement).Cast<LineStringElement>();
            List<DPoint3d>[] lpoints = new List<DPoint3d>[strLines.Count()];

            int i = -1;
            foreach (LineStringElement line in strLines)
            {
                var pts = new List<DPoint3d>();
                line.GetCurveVector().GetPrimitive(0).TryGetLineString(pts);
                lpoints[++i] = pts;
            }

            // ! 2-ая точка 3-й линии задаёт ориентацию
            points.Add(lpoints[2][1]);
        }
    }
    private void loadFromCell_Paks(CellHeaderElement cell, out List<DPoint3d> points)
    {
        /// Принцип чтения элемента заключается в:
        /// 1. определении точек построения;
        /// 2. остальные параметры строятся в соответствии с 
        ///    текущими настройками;        
        
        // Точки построения
        points = new List<DPoint3d>();
        // базовую точку
        points.Add(cell.GetOrigin());

        // var paksData = new PaksData();
        
        ChildElementCollection childs = cell.GetChildren();
        if (Type == HeightToolTypeEnum.Planned)
        {
            // по двум точкам
            LineStringElement line = null;                    
            var strLines = childs.Where(x => x is LineStringElement);               
            if (strLines.Count() == 1) 
                line = strLines.First() as LineStringElement;            
            else 
            { // поддержка старого алгоритма (при большом желании можно удалить)
                line = strLines.LastOrDefault() as LineStringElement;
            }
                
            if (line != null)
            {
                List<DPoint3d> pts = new List<DPoint3d>();
                line.GetCurveVector().GetPrimitive(0).TryGetLineString(pts);
                points.Add(pts[1]); // вторая точка - колено полки
            }
        }
        else // Section
        {        
            ComplexShapeElement cxEl  = childs.FirstCast<ComplexShapeElement>();                
            points.Add(cxEl.GetCenter());
        }       
    }
     
#endregion

}
}
