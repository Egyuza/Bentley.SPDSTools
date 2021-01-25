namespace SPDSTools
{
    /// <summary>
    /// Значения связаны со значениями в EC-схеме данных
    /// </summary>
    public enum AlignmentEnum
    {
        Auto = 0,
        Bottom = 1,
        BottomLeft = 2,
        BottomRight = 3,
        Left = 4,
        Right = 5,
        Top = 6,
        TopLeft = 7,
        TopRight = 8
    }

    //public static AlignmentEnum GetAlignment(bool left, bool top, bool right, bool bottom)
    //{       
    //    if (!left && !top && !right && bottom)
    //        return AlignmentEnum.Bottom;
    //    if (left && !top && !right && bottom)
    //        return AlignmentEnum.BottomLeft;
    //    if (!left && !top && right && bottom)
    //        return AlignmentEnum.BottomRight;
    //    if (left && !top && !right && !bottom)
    //        return AlignmentEnum.Left;
    //    if (!left && !top && right && !bottom)
    //        return AlignmentEnum.Right;
    //    if (!left && top && !right && !bottom)
    //        return AlignmentEnum.Top;
    //    if (left && top && !right && !bottom)
    //        return AlignmentEnum.TopLeft;
    //    if (!left && top && right && !bottom)
    //        return AlignmentEnum.TopRight;
        
    //    return AlignmentEnum.Auto;
    //}

}
