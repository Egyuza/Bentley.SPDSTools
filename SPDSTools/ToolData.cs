using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;

using SPDSTools.Parameters;
using Bentley.ECObjects.Instance;
using System.Collections.Generic;
using Bentley.GeometryNET;

namespace SPDSTools
{

public struct TargetId
{
    public readonly ElementId AttachmentId;
    public readonly ElementId ElementId;

    public TargetId(ElementId attachmentId, ElementId elemId)
    {
        AttachmentId = attachmentId;
        ElementId = elemId;
    }
}

public abstract class ToolData
{
    public CellHeaderElement OriginalCell { get; private set; }
    public List<DPoint3d> Points { get; protected set; }
    public virtual List<DPoint3d> SnapPoints 
    {
        get { return Points; }
    }

    public bool IsAnnotaion { get; private set; }
    public bool IsMerged { get; protected set; }
    public TargetId TargetId { get; private set; }

    protected abstract void OnReadPropertiesFromCell(ECPropertyReader reader);

    public ToolData(CfgParameters pars)
    {
        IsAnnotaion = true;
        IsMerged = false;

    }
    public ToolData(CellHeaderElement cell)
    {
        OriginalCell = cell;
        readPropertiesFromCell(cell);
    }

    private void readPropertiesFromCell(CellHeaderElement cell)
    {
        ECPropertyReader reader = 
            ECPropertyReader.TryGet(cell, ECClassTypeEnum.Common);

        if (reader == null)
            return;

        IECPropertyValue propVal = reader.Get(ECPropertyName.Common.TargetId);

        if (propVal != null && !propVal.IsNull)
        {
            long id = long.Parse(propVal.StringValue);
            var elementId = new ElementId(ref id);

            long att_id = -1;
            ElementId attachmentId = new ElementId();
            {
                propVal = reader.Get(ECPropertyName.Common.TargetReferenceId);
                if (propVal != null && !propVal.IsNull 
                    && !string.IsNullOrWhiteSpace(propVal.StringValue))
                {
                    att_id = long.Parse(propVal.StringValue);
                    attachmentId = new ElementId(ref att_id);
                }        
            }

            TargetId = new TargetId(attachmentId, elementId);
        }

        { // IsAnnotaion
            propVal = reader.Get(ECPropertyName.Common.IsAnnotation);
            IsAnnotaion = (propVal == null)
                ? true
                : propVal.BooleanValue();
        }

        { // IsMerged
            propVal = reader.Get(ECPropertyName.Common.IsMerged);
            IsMerged = (propVal == null) ? false : propVal.BooleanValue();
        }
        
        OnReadPropertiesFromCell(
            ECPropertyReader.TryGet(cell, ECClassTypeEnum.HeightTool));
    }

    public void setPoints(IEnumerable<DPoint3d> points) {
        Points = new List<DPoint3d>(points);
    }

    public static bool ExtractFrom(ToolElement toolElement, out ToolData data)
    {
        data = null;

        if (toolElement == null)
            return false;

        if (toolElement is HeightToolElement)        
            data = new HeightToolData(toolElement.Cell);
        else
            data = null;

        return data != null;
    }
}

}
