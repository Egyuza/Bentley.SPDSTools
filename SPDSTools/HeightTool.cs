using System;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;

namespace SPDSTools
{
public class HeightTool : Tool, IDisposable
{   
    
    private HeightToolElement _element;
    
    // КОНСТРУКТОР
    public HeightTool()
    : base()
    {
    }

    protected override bool NeedPointForDynamics() {
        return true;
    }

    //public override StatusInt StartTool()
    //{
    //    // ShowForm(new HeightToolControl(), "HeightTool");
    //    return this.InstallTool();
    //}

    private void _data_ToolTypeChangedEvent(HeightToolTypeEnum type)
    {
        if (type == HeightToolTypeEnum.Planned)
        {
            //this.EndDynamics();
            RequiresToEnsureLocatingElements = true;
            // todo настроить параметры привязок
        }
    }

    public override StatusInt OnElementModify(Element element)
    {
        // todo check
        return base.OnElementModify(element);
    }

    public override void Dispose()
    {
        
        _element?.Dispose();
        _element = null;
        base.Dispose();
    }

    protected override bool OnDataButton(DgnButtonEvent ev)
    {
        var retVal = base.OnDataButton(ev);

        if (_points.Count == 0)
            RequiresToEnsureLocatingElements = true;

        return retVal;
    }

    //protected override void OnDynamicFrame(DgnButtonEvent ev)
    //{
    //    if (!RequiresToEnsureLocatingElements)
    //        base.OnDynamicFrame(ev);
    //    else
    //    {
    //        EndDynamics();
    //        // позволяет реанимировать возможность локации элементов модели при
    //        // активной динамике
    //        HitPath hitpath = DoLocate(ev, true, ComponentMode.Innermost);
    //        AccuSnap.SetComponentMode(ComponentMode.NormalChild);
    //        Session.Instance.Keyin("SNAP NEAREST");

    //        RequiresToEnsureLocatingElements = false;
    //        BeginDynamics();
    //    }
    //}
    
    public static HeightTool Instance { get; private set; }

    public static void InstallNewInstance (bool isTargetFixed = false)
    {
        Instance = new HeightTool();
        if (isTargetFixed)
        {
            Instance.isTargetFixed = isTargetFixed;
            Instance.ActionMode = ToolActionMode.LocateFixedTarget;
        }
        Instance.InstallTool();
    }
}
}