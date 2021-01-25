using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;

namespace SPDSTools
{
public struct TextMargin
{
    public readonly double
        Before,
        Top,
        After,
        Bottom;

    public TextMargin (double before, double top, double after, double bottom)
    {
        this.Before = before;
        this.Top = top;
        this.After = after;
        this.Bottom = bottom;
    }
}
}