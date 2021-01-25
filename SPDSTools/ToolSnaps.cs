using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPDSTools
{
internal class ToolSnaps
{
    int? _FocusedSnapIndex;
    List<Element> _elements;
    
    readonly RedrawElems _redrawElems;
    readonly uint colorFocused, colorCommon;

    readonly ToolData _data;
    public ToolData ToolData { get { return _data; } }

    public bool SnapIsFocused
    {
        get { return _FocusedSnapIndex.HasValue; }
    }

    public ToolSnaps(ToolData data)
    {        
        if (data == null)
            throw new ArgumentNullException("data");

        _data = data;
        _elements = new List<Element>();

        _redrawElems = new RedrawElems();
        _redrawElems.SetDynamicsViewsFromActiveViewSet(Session.GetActiveViewport());
        _redrawElems.DrawMode = DgnDrawMode.TempDraw;
        _redrawElems.DrawPurpose = DrawPurpose.Dynamics;
        _redrawElems.IgnoreHilite = false;

        DgnFile file = Session.Instance.GetActiveDgnFile();
        colorFocused = file.GetClosestColor(System.Drawing.Color.Red);
        colorCommon = file.GetClosestColor(System.Drawing.Color.Blue);
    }

    public void Redraw(DgnButtonEvent ev)
    {
        Update(ev);
        foreach (Element element in _elements)
            _redrawElems.DoRedraw(element);        
    }
    
    public void Update(DgnButtonEvent ev) 
    {
        _FocusedSnapIndex = null;
        this._elements = null;

        if (_data == null)
            return;

        DgnModel model = ev.Viewport.GetRootModel();
        DgnFile file = model.GetDgnFile();

        _elements = new  List<Element>();

        // зависимость от текущего масштаба камеры:

        ViewInformation info = ev.Viewport.GetViewInformation();
        double k = 0.0045 * info.Delta.Magnitude; //0.4 * toolData.Scale            

        int index = -1;
        foreach (DPoint3d p in _data.Points)
        {
            ++index;
            var ellips = new EllipseElement(model, null, p, k, k, 0.0);
            
            if (ellips.GetRange().Contains(ev.Point))
            {
                _FocusedSnapIndex = index;

                ellips.AddSolidFill(colorFocused, false);
 
                var line =  new LineStringElement(model, null, new DPoint3d[]
                    { p.Shift(dY: 4*k), p.Shift(dY: -4*k)});            
                _elements.Add(line);
                line =  new LineStringElement(model, null, new DPoint3d[]
                    { p.Shift(4*k), p.Shift(-4*k)});
                _elements.Add(line);                
            }
            else
            {
                ellips.AddSolidFill(colorCommon, false);
            }
             
            _elements.Add(ellips);
        }
    }

    public IEnumerable<DPoint3d> GetExpandedPoints(DPoint3d newPoint)
    {
        return (_FocusedSnapIndex.HasValue)
            ? _GetExpandedPoints(_data.Points, newPoint, _FocusedSnapIndex.Value)
            : _data.Points;
    }

    private static IEnumerable<DPoint3d> _GetExpandedPoints(
        IEnumerable<DPoint3d> arr, DPoint3d varPoint, int ind)
    {
        int cnt;
        if (arr == null || ind + 1 > (cnt = arr.Count()))
            return arr;        

        DPoint3d[] res = new DPoint3d[cnt];

        // осуществляем смещение точек после указанной:
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
}
}
