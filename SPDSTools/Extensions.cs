using Bentley.GeometryNET;
using Bentley.DgnPlatformNET.Elements;
using System;
using System.Collections.Generic;
using Bentley.DgnPlatformNET;
using Bentley.ECObjects.Instance;
using System.Linq;
using System.Globalization;

namespace SPDSTools
{
public static class Extensions
{
    public static DPoint3d Shift(this DPoint3d p3d, double dX = 0.0, double dY = 0.0, double dZ = 0.0)
    {
        return new DPoint3d(p3d.X + dX, p3d.Y + dY, p3d.Z + dZ);
    }

    public static DPoint3d GetCenter(this DisplayableElement element)
    {
        DRange3d rng = element.GetRange();
        return DPoint3d.Add(rng.Low, rng.DiagonalVector, 0.5);
       // return rng.Low.Shift(rng.XSize/2, rng.YSize/2);
    }

    public static DPoint3d GetOrigin(this DisplayableElement element)
    {
        DPoint3d origin;
        element.GetSnapOrigin(out origin);
        return origin;
    }

    public static DPoint3d OverSet(this DPoint3d pt, double? X = null, 
        double? Y = null, double? Z = null)
    {
        return new DPoint3d(X ?? pt.X, Y ?? pt.Y, Z ?? pt.Z);
    }

    /// <summary>
    /// Ориентация относително указазанной точки по горизонтали
    /// </summary>
    /// <param name="origin">базовая точка</param>
    public static OrientHorEnum OrientH(this DPoint3d point, DPoint3d origin, AlignmentEnum sets)
    {
        if (sets == AlignmentEnum.Right || sets == AlignmentEnum.TopRight
            || sets == AlignmentEnum.BottomRight)
        {
            return OrientHorEnum.Right;
        }
        else if (sets == AlignmentEnum.Left || sets == AlignmentEnum.TopLeft
            || sets == AlignmentEnum.BottomLeft)
        {
            return OrientHorEnum.Left;
        }
    
        if (point.X > origin.X)
            return OrientHorEnum.Right;
        else if (point.X < origin.X)
            return OrientHorEnum.Left;
        else
            return OrientHorEnum.Middle;
    }

    /// <summary>
    /// Ориентация относително указазанной точки по вертикали
    /// </summary>
    /// <param name="origin">базовая точка</param>
    /// <returns></returns>
    public static OrientVertEnum OrientV(this DPoint3d point, DPoint3d origin, AlignmentEnum sets)
    {
        if (sets == AlignmentEnum.Top || sets == AlignmentEnum.TopRight
            || sets == AlignmentEnum.TopLeft)
        {
            return OrientVertEnum.Top;
        }
        else if (sets == AlignmentEnum.Bottom || sets == AlignmentEnum.BottomRight
            || sets == AlignmentEnum.BottomLeft)
        {
            return OrientVertEnum.Bottom ;
        }    
    
        if (point.Y > origin.Y)
            return OrientVertEnum.Top;
        else if (point.Y < origin.Y)
            return OrientVertEnum.Bottom;
        else
            return OrientVertEnum.Middle;
    }

    

    //public static OrientationEnum GetOrientation(this DPoint3d basePoint, 
    //    DPoint3d horPoint, DPoint3d vertPoint)
    //{
    //    OrientHorEnum orH = basePoint.OrientH(horPoint, OrientationEnum.Auto);
    //    OrientVertEnum orV = basePoint.OrientV(vertPoint, OrientationEnum.Auto);

    //    if (orH == OrientHorEnum.Left)
    //    {
    //        return orV == OrientVertEnum.Top ? OrientationEnum.TopLeft
    //            : orV == OrientVertEnum.Bottom ? OrientationEnum.BottomLeft
    //            : OrientationEnum.Left;
    //    }
    //    else if (orH == OrientHorEnum.Right)
    //    {
    //        return chbxOrient_Top.Checked ? OrientationEnum.TopRight
    //            : chbxOrient_Bottom.Checked ? OrientationEnum.BottomRight
    //            : OrientationEnum.Right;
    //    }
    //    else if (chbxOrient_Top.Checked)
    //    {
    //        return chbxOrient_Left.Checked ? OrientationEnum.TopLeft
    //            : chbxOrient_Right.Checked ? OrientationEnum.TopRight
    //            : OrientationEnum.Top;
    //    }
    //    else if (chbxOrient_Bottom.Checked)
    //    {
    //        return chbxOrient_Left.Checked ? OrientationEnum.BottomLeft
    //            : chbxOrient_Right.Checked ? OrientationEnum.BottomRight
    //            : OrientationEnum.Bottom;
    //    }       
    //    else
    //        return OrientationEnum.Auto; 
    //}

    public static void Write(this WriteDataBlock data, object value)
    {
        if (value is string)
            data.WriteString((string)value);
        else if (value is double)
            data.WriteDouble((double)value);
        else if (value is float)
            data.WriteFloat((float)value);
        else if (value is Int16)
            data.WriteInt16((Int16)value);
        else if (value is Int32)
            data.WriteInt32((Int32)value);
        else if (value is Int64)
            data.WriteInt64((Int64)value);
    }

    public static uint GetClosestColor(this DgnFile file, System.Drawing.Color color)
    {                       
        RgbColorDef def = new RgbColorDef(color.R, color.G, color.B);
        return file.GetColorMap().FindClosestMatch(def);
    }

    public static DRange3d GetRange(this DisplayableElement elem)
    {
        DRange3d range;
        StatusInt status = elem.CalcElementRange(out range);
        return range;
    }

    public static double GetHeight(this DgnTextStyle style)
    {
        double value;    
        style.GetProperty(TextStyleProperty.Height, out value);
        return value;
    }

    public static DgnTextStyle GetTextStyle(this TextElement text)
    {
        DgnTextStyle style = null;
        DgnFile file = text.DgnModel.GetDgnFile();
        TextQueryOptions opt = new TextQueryOptions();
        opt.ShouldIncludeEmptyParts = false;
        TextPartIdCollection tpcoll = text.GetTextPartIds(opt);

        if (text.GetTextPart(tpcoll[0]).GetProperties().HasTextStyle == true)
        {
            long stId = text.GetTextPart(tpcoll[0]).GetProperties().TextStyleId;
            ElementId styleId = new ElementId(ref stId);
            style = DgnTextStyle.GetById(styleId, file);
        }
        else
        {
            style = DgnTextStyle.ExtractFromElement(text, 0);
        }
        return style ?? DgnTextStyle.GetSettings(file);
    }

    public static TextElementJustification GetJustification(this TextElement text)
    {
        TextQueryOptions opt = new TextQueryOptions();
        opt.ShouldIncludeEmptyParts = false;
        TextPartIdCollection tpcoll = text.GetTextPartIds(opt);

        ParagraphProperties pp = text.GetTextPart(tpcoll[0]).GetParagraphPropertiesForAdd();
        return pp.Justification;
    }
           
    public static string GetText(this TextElement text)
    {
        TextQueryOptions opt = new TextQueryOptions();
        opt.ShouldIncludeEmptyParts = false;
        TextPartIdCollection tpcoll = text.GetTextPartIds(opt);

        return text.GetTextPart(tpcoll[0]).ToString();
    }

    public static IEnumerable<T1>CastDown<T1, T>(this IEnumerable<T> enu) where T1: T
    {
        if (enu == null)
            return null;

        List<T1> list = new List<T1>();
        foreach(T item in enu)
        {
            if (item is T1)
                list.Add((T1)item);
        }
        return list;
    }

    public static DgnAttachment AsDgnAttachmentOf(this DgnModel model, DgnModel owner)
    {
        if (owner == null || model == owner)
            return null;

        foreach (DgnAttachment attach in owner.GetDgnAttachments())
        {
            if (attach.GetDgnModel() == model)
                return attach;
        }
        return null;
    }

    public static bool BooleanValue(this IECPropertyValue prop)
    {
        return bool.Parse(prop.StringValue);
    }
        
    /// <summary>
    /// Получает первый элемент в последовательности типа "TResult"
    /// </summary>
    public static TResult FirstCast<TResult> (
        this IEnumerable<Element> sequence) where TResult : Element
    {
        object el = sequence.OfType<TResult>().First() as object;
        return el != null ? el as TResult : null;
    }


    public static void setLineStyle (this Element element, int style)
    {
        var propSetter = new ElementPropertiesSetter();
        propSetter.SetLinestyle(style, null);
        propSetter.Apply(element);
    }

    public static double Scaled(this double value, double scale)
    {
        return value * scale;
    }


    public static bool IsActiveTool(this DgnPrimitiveTool tool)
    {
        return tool == DgnTool.GetActivePrimitiveTool();
    }

    public static string Formatted(this string txt, params object[] args)
    {
        return string.Format(txt, args);
    }
    
    public static bool TryParseToDouble(this string s, out double result)
    {
        string dltr = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        s = s.Replace(".", dltr).Replace(",", dltr);

        return double.TryParse(s, out result);
    }
}
}