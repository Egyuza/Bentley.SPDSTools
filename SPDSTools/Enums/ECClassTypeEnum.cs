namespace SPDSTools
{
public enum ECClassTypeEnum
{
    [StringValue("Unknown")]
    Unknown,
    [StringValue("Tool")]
    Common,
    [StringValue("HeightTool")]
    HeightTool,
    [StringValue("LeaderTool")]
    LeaderTool,
    [StringValue("MstnHeader")]
    MstnHeader,
    [StringValue("NormalCellElement")]
    NormalCell,
    [StringValue("UIDElement")]
    UIDElement,
}
}