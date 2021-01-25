using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.ECObjects.Instance;
using Bentley.GeometryNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using SPDSTools.Parameters;
using HP = SPDSTools.Parameters.HeightParametersEnum;
using Bentley.MstnPlatformNET;

namespace SPDSTools
{
public class HeightToolElement : ToolElement
{
    private HeightToolData hData { 
        get { return base.Data as HeightToolData; } 
    }

    private double Pi { get { return Math.PI; } }

    protected HeightParameters HPars
    {
        get { return HeightParameters.Instance; }
    }

    public DPoint3d Origin { get; protected set; }

    public override int ConstractionPointsCount
    {
        get
        {           
            if (hData.ToolStyle == ToolStyleEnum.Default &&
                hData.Type == HeightToolTypeEnum.Planned)
            {
                return hData.AsLeader ? 2 : 1;
            }
            else
            {
                return 2;
            }            
        }
    }

    public override ECClassTypeEnum ToolECClassType
    {
        get { return ECClassTypeEnum.HeightTool; }
    }

    public string Text { get; private set; }

    public string Text2 { get; private set; }

    public DgnTextStyle TextStyle { get; private set; }
    public TextMargin TextMargin { get; private set; }    

    /// **** КОНСТРУКТОР ***
    public HeightToolElement(HeightToolData data): base(data)
    {
        if (data == null)        
            throw new ArgumentNullException("data");
    }

    private const string errtxt = "Error on read <{0}> config variable \"{1}\"";
    
    OrientHorEnum OrH;
    OrientVertEnum OrV;

    int kH { get { return OrH == OrientHorEnum.Left ? -1 : 1; }  }
    int kV { get { return OrV == OrientVertEnum.Bottom ? -1 : 1; }  }
    

    /// <summary>
    ///  Построение по точкам
    /// </summary>
    /// <param name="points">входные точки</param>
    /// <param name="pivots">выходные коодинаты точек-манипуляторов</param>
    /// <returns></returns>
    protected override CellHeaderElement OnLoadByPoints(List<DPoint3d> points)
    {
        // ! элемент следует строить даже при одной точке,
        // используя значения по умолчанию

        TextStyle = TextStyleHelper.GetTextStyle(
            HPars[HP.TEXTSTYLE].StringValue, model, true);

        TextMargin = new TextMargin(
            Scaled(HPars[HP.TEXT_MARGIN_BEFORE].DoubleValue),
            Scaled(HPars[HP.TEXT_MARGIN_TOP].DoubleValue),
            Scaled(HPars[HP.TEXT_MARGIN_AFTER].DoubleValue),
            Scaled(HPars[HP.TEXT_MARGIN_BOTTOM].DoubleValue));

        if (points == null || points.Count() == 0) // НВС
            return null;

        var elements = new List<Element>();        
        Origin = points[0]; // в качестве базовой точки указываем points[0]

        Text = GetHeight_FormatValue(Origin);
        
        // TODO добавить проверку на плиту - Slab

        if (hData.ShowItemHeight && Target != null)        
            Text2 = getSlabHeight();
        else
            Text2 = hData.Text2;
        
        DPoint3d p1 = (points.Count == 1)
            ? p1 = points[0]
            : p1 = points[1];

        // вторая точка (если есть) задаёт ориентацию элементов тула:
        OrH = p1.OrientH(points[0], hData.Alignment);
        OrV = p1.OrientV(points[0], hData.Alignment);

        try
        {
            CellHeaderElement cell = null;
                
            switch (hData.ToolStyle)
            {
            case ToolStyleEnum.Default: // ПО УМОЛЧАНИЮ:
            {
                elements = (hData.Type == HeightToolTypeEnum.Planned) ?
                    GetElements_Default_Planned(points) :
                    GetElements_Default_Section(points);

                break;
            } 
            case ToolStyleEnum.Paks:   // ПАКШ:
            {
                uint fillColor = hData.UseFilling ? 
                    HPars[HP.PAKS_ARROW_FILLCOLOR].UIntValue :
                    COLOR_AS_BACKGROUND;

                elements = (hData.Type == HeightToolTypeEnum.Planned) ?
                    GetElements_Paks_Planned(points, fillColor) :
                    GetElements_Paks_Section(points, fillColor);

                break;
            }
            case ToolStyleEnum.ED: // EL-DAABE:
            {
                uint fillColor = hData.UseFilling
                    ? HPars[HP.PAKS_ARROW_FILLCOLOR].UIntValue
                    : COLOR_AS_BACKGROUND;

                elements = (hData.Type == HeightToolTypeEnum.Section) ?
                    // для Разрезной - Default
                    GetElements_Default_Section(points) :
                    // для Плановой - Пакш
                    GetElements_Paks_Planned(points, fillColor);

                break;
            }
            default:
                throw new SPDSException("Unimplemented ToolStyle");
            }                  
            

            cell = new CellHeaderElement(
                model, ToolName, Origin, DMatrix3d.Zero, elements);

            // устанавливаем слой
            ElementPropertiesSetter setter = new ElementPropertiesSetter();
       
            setter.SetLevel(HPars[HP.LEVEL].IntValue);
            setter.Apply(cell);
            return cell;
        }
        catch (Exception)
        {
            return null;
        }
    }  

    private List<Element> GetElements_Default_Planned(List<DPoint3d> points)
    {
        var elements = new List<Element>();

        DPoint3d p0 = points[0];
        TextHandlerBase tmpText = null;

        if (!hData.AsLeader) // без выносной линии:
        {
            tmpText = GetTextElement(Text, TextStyle,
                p0, TextElementJustification.CenterMiddle);
        }
        else // выносной элемент:
        ///        _______
        ///  -----| +4,86 |
        ///  
        {
            DPoint3d p1;
            if (points.Count == 1)
                p1 = p0;
            else // > 1:
            {
                p1 = points[1];
                elements.Add(createLine(p0, p1));
            }
                                     
            { // точка указателя:
                double r = Scaled(HPars[HP.DEF_PLAN_POINTRADIUS].DoubleValue);
                uint fcolor = HPars[HP.DEF_PLAN_ARROW_FILLCOLOR].UIntValue;

                var cirlce = new EllipseElement(model, null, p0, r, r, 0.0);
                cirlce.AddSolidFill(fcolor, true);
                elements.Add(cirlce);
            }

            // отступ текста от рамки:
            double dX_txt, dY_txt;

            int kH, kV;

            if (p1.OrientH(p0, hData.Alignment) == OrientHorEnum.Left)
            {
                kH = -1;
                tmpText = GetTextElement(Text, TextStyle, p1, 
                    TextElementJustification.RightMarginMiddle);
                dX_txt = Math.Abs(p1.X - tmpText.GetRange().High.X);
                dY_txt = Math.Abs(p1.Y - tmpText.GetRange().High.Y);          
            }
            else
            {
                kH = 1;
                tmpText = GetTextElement(Text, TextStyle, p1, 
                    TextElementJustification.LeftMarginMiddle);
                dX_txt = Math.Abs(p1.X - tmpText.GetRange().Low.X);
                dY_txt = Math.Abs(p1.Y - tmpText.GetRange().Low.Y);
            }

            kV = p1.OrientV(p0, hData.Alignment) == OrientVertEnum.Bottom
                ? -1 : 1;

            if (p1 != p0)
            {
                DPoint3d trOrig;
                tmpText.GetTransformOrigin(out trOrig);
                DTransform3d tr = DTransform3d.FromTranslation(
                    kH * dX_txt, kV * dY_txt, .0);
                tmpText.ApplyTransform(new TransformInfo(tr));                    
            }
        }
             
        elements.Add(tmpText);
        DRange3d range = tmpText.GetRange();
            
        // рамка вокруг текста:
        var line = createLine(range.Low, range.Low.Shift(dY: range.YSize), 
            range.High, range.High.Shift(dY: -range.YSize), range.Low);
            
        elements.Add(line);
        return elements;
    }

    private List<Element> GetElements_Default_Section(List<DPoint3d> points)
    {
        var elements = new List<Element>();        

        //              __#,###__
        //             |   
        // ___________\|/_                        
        //

        OrientHorEnum orH;
        OrientVertEnum orV;
        int kH, kV;
        // Ориентация:
        {
            DPoint3d pp0 = points[0];
            DPoint3d pp1 = (points.Count > 1) ? pp1 = points[1] : pp0;
            orH = pp1.OrientH(pp0, hData.Alignment);
            orV = pp1.OrientV(pp0, hData.Alignment);
            kH = orH == OrientHorEnum.Left ? -1 : 1;
            kV = orV == OrientVertEnum.Bottom ? -1 : 1;
        }

        // начало базовой линии выноски с учётом сдвига
        double GapStart = Scaled(HPars[HP.DEF_SEC_ORIGIN_GAP].DoubleValue);
        DPoint3d p0 = points[0].Shift(kH * GapStart);

        elements.Add(new PointStringElement(model, null, new [] {points[0]}, true));

        DPoint3d p1;
        if (points.Count > 1)
            p1 = points[1];
        else
        {                
            p1 = p0.Shift(Scaled(
                HPars[HP.DEF_SEC_LENGTH_BEFORE_ARROW].DoubleValue));
        }

        // высота второй точки не меняется:
        p1.Y = p0.Y;           

        double Arrow_dX = Scaled(HPars[HP.DEF_SEC_ARROW_DX].DoubleValue);
        double Arrow_dY = Scaled(HPars[HP.DEF_SEC_ARROW_DY].DoubleValue);
        double GapArrowAfter = Scaled(HPars[HP.DEF_SEC_ARROW_GAP_AFTER].DoubleValue);

        elements.Add(createLine( p0, p1, p1.Shift(kH * GapArrowAfter)));
        
        // стрелка:
        var lineArrow = createLine( 
            p1.Shift(Arrow_dX, kV * Arrow_dY), 
            p1,
            p1.Shift(-Arrow_dX, kV * Arrow_dY));

        ElementPropertiesSetter pSetter = new ElementPropertiesSetter();
        pSetter.SetWeight((uint)2);
        pSetter.Apply(lineArrow);
        elements.Add(lineArrow);                  

        double LandingHeight = Scaled(HPars[HP.DEF_SEC_LANDING_HEIGHT].DoubleValue);

        // высота полки:
        double h = kV > 0 ? LandingHeight:  
            System.Math.Max(LandingHeight,
                TextStyle.GetHeight() + TextMargin.Bottom + TextMargin.Top);

        DPoint3d p2 = p1.Shift(dY: kV * h);

        // todo ориентация текста

        TextElementJustification txtJust;

        // выравнивание текста в зависимости от гор. расположения:
        txtJust = orH == OrientHorEnum.Left
            ? TextElementJustification.RightBaseline
            : TextElementJustification.LeftBaseline;           

        TextHandlerBase txtElem = GetTextElement(Text, TextStyle,
            p2.Shift(kH * TextMargin.Before, TextMargin.Bottom),
            txtJust
        );
            
        DRange3d range = txtElem.GetRange();
            
        //// высота полки в зависимости от верт. расположения:
        //def_LandingHeight = orV == OrientVertEnum.Bottom
        //    ? def_GapTextTop + range.YSize + def_GapTextBottom
        //    : def_LandingHeight;

        elements.Add(txtElem);

        // полка:
        elements.Add(createLine(p1, p2, p2.Shift(
            kH * (TextMargin.Before + range.XSize + TextMargin.After))));

        return elements;
    }

    private List<Element> GetElements_Paks_Planned(
        List<DPoint3d> points, uint fillColor)
    {
        var elements = new List<Element>();

        Angle angleXY = Angle.Zero;
        
        DPoint3d p0 = points[0];
        DPoint3d p1 = (points.Count == 1)
            ? p1 = p0
            : p1 = points[1];
                          
        // считаем, что текст всегда над полкой
        // вторая точка задаёт угол вращения
        angleXY = new DVector3d(p0, p1).AngleXY;
          
        //p1 = p0.Shift(p1.Distance(p0));


        double r = Scaled(HPars[HP.PAKS_PLAN_ARROW_RADIUS].DoubleValue);
        double r2 = Scaled(HPars[HP.PAKS_PLAN_ARROW_RADIUS2].DoubleValue);

        { // Подложка:
            //double bg_r = Math.Max(r, r2);
            //if (bg_r == r) {
            //    bg_r *= 1.075; // эмпир. коэф
            //}
                         
            //var bg_ellips = new EllipseElement(
            //    model, null, DEllipse3d.FromCenterRadiusXY(p0, bg_r)); // p0, bg_r, bg_r, 0.0);

            //bg_ellips.AddSolidFill(COLOR_AS_BACKGROUND, true);
                
            //ElementPropertiesSetter setter = new ElementPropertiesSetter();
            //setter.SetColor(COLOR_AS_BACKGROUND);
            //setter.Apply(bg_ellips);

            // ОТКЛ:
            // elements.Add(bg_ellips);
        }

        //! вводим ОГРАНИЧЕНИЯ:
        TextElementJustification txtJust;
        TextElementJustification txtJust2;
        int _kV = 1;
        int _kH = 1;

        if (angleXY.Degrees >= 45.0 && angleXY.Degrees < 135)
        {
            angleXY = Angle.FromDegrees(90.0);
               
            p1 = !hData.AsLeader
                ? p0.Shift(Scaled(HPars[HP.PAKS_PLAN_LANDING_LENGTH].DoubleValue))
                : p0.Shift(p1.Y - p0.Y);

            txtJust = TextElementJustification.LeftBaseline;
            txtJust2 = TextElementJustification.LeftTop;
            _kH = 1;
        }
        else if (angleXY.Degrees > -135.0 && angleXY.Degrees <= -45)
        {
            angleXY = Angle.FromDegrees(90.0);
               
            p1 = !hData.AsLeader
                ? p0.Shift(-Scaled(HPars[HP.PAKS_PLAN_LANDING_LENGTH].DoubleValue))
                : p0.Shift(p1.Y - p0.Y);

            txtJust = TextElementJustification.RightBaseline;
            txtJust2 = TextElementJustification.RightTop;
            _kH = -1;
        }     
        else
        {
            angleXY = Angle.Zero;
            _kH = kH;

            p1 = !hData.AsLeader
                ? p0.Shift(kH * Scaled(HPars[HP.PAKS_PLAN_LANDING_LENGTH].DoubleValue))
                : p1.OverSet(Y: p0.Y);

            txtJust = OrH == OrientHorEnum.Left ? 
                TextElementJustification.RightBaseline :
                TextElementJustification.LeftBaseline;

            txtJust2 = OrH == OrientHorEnum.Left ? 
                TextElementJustification.RightTop :
                TextElementJustification.LeftTop;
        }

        var tranRotate = new TransformInfo(
        DTransform3d.FromRotationAroundLine(
            p0, new DVector3d(p0, p0.Shift(dZ:1.0)), angleXY));

        {                
        // ТЕКСТ:
            TextHandlerBase txtElem = GetTextElement(Text, TextStyle,
                p1.Shift(_kH * TextMargin.Before, _kV * TextMargin.Bottom), 
                txtJust, !hData.UseFilling);
                
        // ТЕКСТ2: высота указанного объекта
            TextHandlerBase txtElementHeight = null;
            if (hData.ShowItemHeight)
            {
                // ТЕКСТ - толщина плиты для стиля El-Daabe              
                txtElementHeight = GetTextElement(
                    Text2, 
                    TextStyle,
                    p1.Shift(_kH * TextMargin.Before, -_kV * TextMargin.Bottom), 
                    txtJust2, 
                    !hData.UseFilling
                );
            }

        // ПОЛКА:
            DRange3d rngText = DRange3d.NullRange;
            txtElem.CalcElementRange(out rngText);

            DRange3d rngText2 = DRange3d.NullRange;
            txtElementHeight?.CalcElementRange(out rngText2);

            double width = Math.Max(rngText.XSize, rngText2.XSize);

            DVector3d circle_vector = new DVector3d(p0, p0.Shift(r));
            circle_vector = circle_vector.RotateXY(new DVector3d(p0, p1).AngleXY);
            // точка пересечения отрезка и окружности
            DPoint3d _p0 = DPoint3d.Add(p0, circle_vector);                

            LineStringElement lPolka = createLine(_p0, p1, p1.Shift(
                _kH * (TextMargin.Before + width + TextMargin.After)));
                
            // поворот на заданный угол:
            //txtElem.ApplyTransform(tranRotate);
            //lPolka.ApplyTransform(tranRotate);               

            elements.Add(txtElem);
            if (txtElementHeight != null)
                elements.Add(txtElementHeight);
            elements.Add(lPolka);
        }   

        // УКАЗАТЕЛЬ
        if (hData.SectorArrow)
        {
            ArcElement arc;
            if (_kH > 0)   
                arc = new ArcElement(model, null, p0, r, -r, Pi, -Pi/2, -Pi);
            else
                arc = new ArcElement(model, null, p0, r, -r, -Pi, -Pi/2, Pi);
            elements.Add(arc);
        }
        else
        {
            var ellips = new EllipseElement(model, null,
                DEllipse3d.FromCenterRadiusXY(p0, r));
            ellips.AddSolidFill(COLOR_AS_BACKGROUND, true);
            elements.Add(ellips);
        } 

        { // перекрестие
            var lineH = new LineElement(model, null, 
                hData.SectorArrow
                    ? new DSegment3d(p0, p0.Shift(_kH * r2))
                    : new DSegment3d(p0.Shift(-r2), p0.Shift(r2))
            );
            var lineV = new LineElement(model, null, 
                new DSegment3d(p0.Shift(dY:r2), p0.Shift(dY:-r2)));
                elements.Add(lineH);
                elements.Add(lineV);
            }

        { // перекрёстные сектора:

            if (hData.SectorArrow)
            {
                elements.Add(
                    createShapeSolid(
                        fillColor,
                        createLine(p0.Shift(_kH*r), p0, p0.Shift(dY:-r)),
                        new ArcElement(model, null, p0, _kH*r, r, 
                            -_kH*Pi/2, 0, Pi/2)
                     )
                );
            }
            else
            {
                ComplexShapeElement solidShape1 = createShapeSolid(
                    fillColor,
                    createLine(p0.Shift(-r), p0, p0.Shift(dY: r)),
                    new ArcElement(model, null, p0, _kH*r, -r, 
                        -Pi/2, Pi, -Pi/2)
                );            
                ComplexShapeElement solidShape2 = createShapeSolid(
                    fillColor,
                    createLine(p0.Shift(dY:-r), p0, p0.Shift(r)),
                    new ArcElement(model, null, p0, _kH*r, -r, 
                        Pi/2, Pi/2, Pi/2)
                );
                elements.Add(solidShape1);
                elements.Add(solidShape2);
            }
        }        

        // ПОВОРОТ на заданный угол
        foreach (Element el in elements)        
            el.ApplyTransform(tranRotate);        
      
        return elements;
    }

    private List<Element> GetElements_Paks_Section(List<DPoint3d> points, uint fillColor)
    {
        var elements = new List<Element>();

        ///        ±0.00
        /// _ _ _ _▼_
        /// 
            
        DPoint3d p0 = points[0];            
        DPoint3d p1 = (points.Count == 1)
            ? p1 = p0.Shift(Scaled(HPars[HP.PAKS_SEC_LANDING_LENGTH].DoubleValue))
            : p1 = points[1];
            
        // фиксируем вторую точку по высоте:
        p1 = p1.OverSet(Y: p0.Y);

        // высота треугольного указателя
        double tr_dX = Scaled(HPars[HP.PAKS_SEC_ARROW_DX].DoubleValue);
        double tr_dY = Scaled(HPars[HP.PAKS_SEC_ARROW_DY].DoubleValue);
            
        // линия высотного уровня:
        int lineStyle = HPars[HP.PAKS_SEC_LINESTYLE].IntValue;
        elements.Add(createLineString(lineStyle, p0, p1, p1.Shift(kH *tr_dX)));
            
        ComplexShapeElement trShape = null;
        TextHandlerBase txtBase = null;

        DPoint3d txtOrigin = p1.Shift(
            kH*(TextMargin.Before - tr_dX), kV*(tr_dY + TextMargin.Bottom));        

        if (OrV == OrientVertEnum.Bottom)
        {
            // треугольный указатель:
            trShape = createShapeSolid(fillColor, 
                createLine(p1, p1.Shift(-tr_dX, -tr_dY), p1.Shift(tr_dX, -tr_dY), p1));
            // текст
            txtBase = OrH == OrientHorEnum.Left
                ? GetTextElement(Text, TextStyle, txtOrigin,
                        TextElementJustification.RightMarginTop)
                : GetTextElement(Text, TextStyle, txtOrigin, 
                        TextElementJustification.LeftMarginTop);
        }
        else
        {
            // треугольный указатель:                    
            trShape = createShapeSolid(fillColor, 
                createLine(p1, p1.Shift(-tr_dX, tr_dY), p1.Shift(tr_dX, tr_dY), p1));

            // текст
            txtBase = OrH == OrientHorEnum.Left 
                ? GetTextElement(Text, TextStyle, txtOrigin, 
                        TextElementJustification.RightMarginBaseline)
                : GetTextElement(Text, TextStyle, txtOrigin, 
                        TextElementJustification.LeftMarginBaseline);
        }
                                    
        elements.Add(trShape);                
        elements.Add(txtBase);
      
        return elements;
    }


    /// <summary>
    /// Отформатированное значение высоты
    /// </summary>
    private string GetHeight_FormatValue(DPoint3d heightPoint)
    {
        if (!hData.AutoCalc)
            return hData.Text;

        double? height = null;
        if (hData.Type == HeightToolTypeEnum.Planned)
        {

        // ez 2019-06-10 временное решение:
        // т.к. у Атомпроекта при работе через ProjectWise теряется связь с 
        // целевым элементом.
        // Пересчёт отключаем только для Плановых отметок

            if (hData.IsLoadedFromCell)
            {
                double value;
                if (!hData.Text.TryParseToDouble(out value))
                    return hData.Text;
                else
                    height = value;
            }
            else if (hData.CalcByItem && Target != null) 
            { 
            // расчёт по элементу
                height = Target.GetRange().High.Z / UorPerMeter; // приводим к метрам
            }
        }
        else if (hData.Type == HeightToolTypeEnum.Section)
        {
            if (!hData.CalcByItem) // расчёт по уровню
                height = heightPoint.Y  / UorPerMeter; // приводим к метрам
        }
        
        string txt = "";

        string ZeroPrefix = HPars[HP.ZEROPREFIX].StringValue;
        char DecimalSeparator = HPars[HP.DECIMALSEPARATOR].CharValue;

        if (height == null)
            txt = ZeroPrefix +  string.Format("#{0}###", DecimalSeparator);
        else
        {
            string prefix = 
                Math.Round(height.Value, 3) == 0.0 ? 
                    ZeroPrefix :
                height > 0 ?
                    "+" : "";
            txt = string.Format("{0}{1:f3}", prefix, height);
            txt = txt.Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
                DecimalSeparator.ToString());
        }
        
        hData.setText(txt);
        return txt;
    }
    
    /// <summary>
    /// Значение высоты целевого объекта
    /// </summary>
    private string getSlabHeight(bool setAsToolText2 = true)
    {
        if (!hData.ShowItemHeight)
            return "";

        // ez 2019-06-10 временное решение:
        // т.к. у Атомпроекта при работе через ProjectWise теряется связь с 
        // целевым элементом.

        if (!hData.AutoCalc || hData.IsLoadedFromCell)
            return hData.Text2;
        
        double? height = null;

        if (hData.CalcByItem && Target != null) // расчёт по элементу
            height = Target.GetRange().ZSize;
        
        string ZeroPrefix = HPars[HP.ZEROPREFIX].StringValue;
        char DecimalSeparator = HPars[HP.DECIMALSEPARATOR].CharValue;

        string txt = "";
        if (!height.HasValue)
            txt = "";
        else
        {
            // приводим к миллиметрам
            height = Math.Round((height.Value * 1000)/ UorPerMeter);
            txt = "h=" + height.ToString();
            //txt = txt.Replace(
            //    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
            //    DecimalSeparator.ToString());
        }
               
        hData.setText2(txt);
        
        return txt;
    }
    

    /// <summary>
    /// Чтение свойств в Cell-элемент после публикации
    /// </summary>
    //protected override void OnReadProperties(ECPropertyReader reader)
    //{
    //    IECPropertyValue propVal = null;           

    //    if ((propVal = reader.Get(ECPropertyName.Height.Style)) != null)
    //    {
    //        ToolStyle = (ToolStyleEnum)propVal.IntValue;

    //        if (ToolStyle == ToolStyleEnum.Default)
    //            readProperties_Default();
    //        else if (ToolStyle == ToolStyleEnum.Paks)
    //            readProperties_Paks();
    //        else
    //            throw new SPDSException("Unsuspected ToolStyle");
    //    }

    //    // todo можно заменить на: bool Get(string name, out PropertyValue propVal)
    //    if ((propVal = reader.Get(ECPropertyName.Height.Text)) != null)
    //        Text = propVal.StringValue;
    //    if ((propVal =  reader.Get(ECPropertyName.Height.AutoCalc)) != null)
    //        AutoCalc = propVal.BooleanValue();
    //    if ((propVal =  reader.Get(ECPropertyName.Height.CalcByItem)) != null)
    //        CalcByItem = propVal.BooleanValue();
    //    if ((propVal = reader.Get(ECPropertyName.Height.Type)) != null)        
    //        Type = (HeighToolTypeEnum)propVal.IntValue;
    //    if ((propVal = reader.Get(ECPropertyName.Height.AsLeader)) != null)
    //        AsLeader = propVal.BooleanValue();
    //    if ((propVal = reader.Get(ECPropertyName.Height.UseFilling)) != null)
    //        UseFilling = propVal.BooleanValue();

    //    // !
    //    if (IsMerged)
    //    {
    //        AutoCalc = // выключаемы             
    //        IsMerged = false; // сбрасываем
    //    }
    //}

    //private void readProperties_Paks()
    //{
    //    // todo readProperties_Paks
    //}

    //private void readProperties_Default()
    //{
    //    // todo readProperties_Default        
    //}
    
    /// <summary>
    /// Запись свойств в Cell-элемент после публикации
    /// </summary>
    protected override void OnWriteProperties(ECPropertyWriter writer)
    {       
        DgnTextStyle textStyle = TextStyleHelper.GetTextStyle(
            HPars[HP.TEXTSTYLE].StringValue, model, true);

        writer.SetValue(ECPropertyName.Height.Style, (int)hData.ToolStyle);
        writer.SetValue(ECPropertyName.Height.TextStyle, textStyle.Name);
        writer.SetValue(ECPropertyName.Height.Text, hData.Text ?? "");
        writer.SetValue(ECPropertyName.Height.Text2, hData.Text2 ?? "");
        writer.SetValue(ECPropertyName.Height.AutoCalc, hData.AutoCalc);
        writer.SetValue(ECPropertyName.Height.CalcByItem, hData.CalcByItem);
        writer.SetValue(ECPropertyName.Height.ShowItemHeight, hData.ShowItemHeight);
        writer.SetValue(ECPropertyName.Height.Type, hData.Type);
        writer.SetValue(ECPropertyName.Height.AsLeader, hData.AsLeader);
        writer.SetValue(ECPropertyName.Height.UseFilling, hData.UseFilling);       
        writer.SetValue(ECPropertyName.Height.SectorArrow, hData.SectorArrow);       
        
        if (hData.ToolStyle == ToolStyleEnum.Default)
            writeProperties_Default();
        else if (hData.ToolStyle == ToolStyleEnum.Paks || hData.ToolStyle == ToolStyleEnum.ED)
            writeProperties_Paks();
        else
            throw new SPDSException("Unsuspected ToolStyle");    
    }

    private void writeProperties_Default()
    {
        // отступ точки-"указателя"
        // WriteToolProperty(HeightToolProperty.AlignLevelLine, _alignLevelLine);

    }

    private void writeProperties_Paks()
    {    
        // todo writeProperties_Paks

    }

      /// <summary>
    /// Адаптивное создание текстового элемента
    /// </summary>
    /// <param name="heightPoint"></param>
    /// <param name="textOrigin"></param>
    /// <param name="textJust"></param>
    /// <returns></returns>
    protected TextElement GetTextElement(string txt, DgnTextStyle textStyle,
        DPoint3d textOrigin, TextElementJustification textJust, 
        bool italics = false, bool upsidedown = false, bool rightToLeft = false)
    {
        // TODO проверки аргументов функции    
        
        if (textStyle == null)
        {
            SPDSException.AlertIfDebug(
            "Попытка создания текстового поля с неинициализированным стилем.");
            return null;
        }
        DgnModel model = Session.Instance.GetActiveDgnModel();

        DgnTextStyle style = textStyle.Copy();
        style.SetProperty(TextStyleProperty.Justification, (uint)textJust);     
        style.SetProperty(TextStyleProperty.Italics, italics);
        style.SetProperty(TextStyleProperty.Upsidedown, upsidedown);
        style.SetProperty(TextStyleProperty.RightToLeft, rightToLeft);
        var txtBlock = new TextBlock(style, model);
        TextBlockProperties txtprops = new TextBlockProperties(style, model);

        txt = string.IsNullOrWhiteSpace(txt) ? " " : txt;
        txtBlock.AppendText(txt);
        
        txtBlock.SetOrientation(DMatrix3d.Identity);
        if (textOrigin != null)
            txtBlock.SetUserOrigin(textOrigin);

        var rng1 = txtBlock.GetNominalRange();
        txtBlock.GetNominalRange().Extend(2.0 , 2.0, 0.0);
        var rng2 = txtBlock.GetNominalRange();

        //! даункаст
        TextElement textEl = (TextElement)TextHandlerBase.CreateElement(null, txtBlock);

        TextQueryOptions opt = new TextQueryOptions();
        opt.ShouldIncludeEmptyParts = false;
        
        TextPartIdCollection tpcoll = textEl.GetTextPartIds(opt);
        textEl.GetTextPart(tpcoll[0]).
            GetProperties().ApplyTextStyle(style, 1, true);
        textEl.GetTextPart(tpcoll[0]).GetProperties().ClearAnnotationScale();

        //textBase.GetTextPart(tpcoll[0]).GetProperties().n
        // записываем в элемент используемый стиль текста, важно
        // для последующего корректного его распознавания
        StatusInt status = style.AddToElement(textEl, 0);

        // удаляем масштаб аннотации для самостоятельного его задания
        textEl.RemoveAnnotationScale();

        var tr3d = 
            DTransform3d.FromUniformScaleAndFixedPoint(textOrigin, GetAnnScale);    
        textEl.ApplyTransform(new TransformInfo(tr3d));

        return textEl;
    }
        
    protected LineStringElement createLine(params DPoint3d[] points)
    {
        return new LineStringElement(model, null, points);
    }

    protected LineStringElement createLineString(int style, params DPoint3d[] points)
    {
        var propSetter = new ElementPropertiesSetter();
        propSetter.SetLinestyle(style, null);

        var line = new LineStringElement(model, null, points);
        propSetter.Apply(line); 
        return line;
    }
    
    protected ArcElement createArcCircle(DPoint3d center, double r)
    {
        return new ArcElement(model, null, center, r, -r, -Math.PI/2, Math.PI, -Math.PI/2);
    }
    
    protected ComplexShapeElement createShapeSolid(uint solidColor, 
        params Element[] elements)
    {
        ComplexShapeElement solidShape = createShape(elements);
        solidShape.AddSolidFill(solidColor, true);
        return solidShape;
    }
    
    protected ComplexShapeElement createShape(params Element[] elements)
    {
        ComplexShapeElement solidShape = new ComplexShapeElement(model, null);
        {
            foreach (Element el in elements)
                solidShape.AddComponentElement(el);
        }
        return solidShape;
    }
}
}
