using System.Text;

using Bentley.MstnPlatformNET;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;

using SPDSTools.Parameters;
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SPDSTools
{
public sealed class Keyin
{
    public static Tool ActiveTool {get; private set;}

    public const string KEY_EDITMODE = "EditMode";

    public static void cmd_HeightTool(string unparsed = "")
    {
        DgnModel model = Session.Instance.GetActiveDgnModel();

        if (model.Is3d && model.ModelType != DgnModelType.Sheet)
        {
            MessageCenter.Instance.ShowMessage(MessageType.Warning,
                SPDSTools.Instance.MdlTaskId + 
                " works only for 2D models or for 3D sheets", "", MessageAlert.Dialog);
            return;
        }
        
        bool editMode = unparsed.Contains(KEY_EDITMODE);
        ToolFormHelper.Show(ToolTypeEnum.HeightTool, editMode);
    }

    public static void cmd_Help_Configuration(string unparsed = "")
    {
        StringBuilder bldr = new StringBuilder();
        bldr.AppendLine(string.Format(
            "# {0} = '{1}'   [{2}]",
            "param_name", "current_value", "default_value")).AppendLine();

        foreach (ICfgParameter p in HeightParameters.Instance)
        {
            if (p.IsOptional)
                continue;

            bldr.AppendLine(string.Format(
                "{0} = {1}   [{2}]", 
                p.Name, 
                string.IsNullOrEmpty(p.StringValue) ? "NULL" : p.StringValue, 
                p.DefaultValue));
        }

        MessageCenter.Instance.ShowInfoMessage("Help configuration parameters",
            bldr.ToString(), true);
    }

    public static void cmd_RefreshAllSPDS_InModel(string unparsed = "")
    {
        refreshAllSPDS_InModel();
    }
    public static void refreshAllSPDS_InModel(ProgressBar pbar = null)
    {
        DgnModel model = Session.Instance.GetActiveDgnModel();
    
        List<CellHeaderElement> cells = 
                ECHelper.FindSPDSElementsByInstance(model);

        //if (false) // (pbar != null)
        //{
        //    pbar.Maximum = (int)cells.Count;
        //    pbar.Value = 0;
        //    pbar.Enabled =
        //    pbar.Visible = true;
            
        //    var t = new System.Threading.Thread(new System.Threading.ThreadStart(
        //        delegate
        //        {
        //            //DgnModel model = Session.Instance.GetActiveDgnModel();
                    
        //            foreach (CellHeaderElement cell in cells)
        //            {
        //                pbar.FindForm().Invoke(new System.Threading.ThreadStart(delegate
        //                {
        //                    ECPropertyReader reader =
        //                        ECPropertyReader.TryGet(cell, ECClassTypeEnum.HeightTool);
        //                    if (reader != null)
        //                    {
        //                        try 
        //                        {
        //                            HeightToolData data = new HeightToolData(cell);
        //                            var toolEl = new HeightToolElement(data);
        //                            toolEl.AddToModel();
        //                        }
        //                        catch(Exception) {}
        //                    }
        //                    ++pbar.Value;                            
        //                }));                        
        //            }
        //        }
        //    ));
        //    t.Start();
            
        //    while(pbar.Value < pbar.Maximum)
        //    {
        //    }
        //    if (pbar != null)
        //    {
        //        pbar.Step = pbar.Maximum;
        //        pbar.Visible =
        //        pbar.Enabled = false;
        //        pbar.Value = 0;
        //    }
        //}
        //else
        //{      
                   
        foreach (CellHeaderElement cell in cells)
        {
            //if (pbar != null) {
            //    ++pbar.Value;
            //}

            ECPropertyReader reader =
                ECPropertyReader.TryGet(cell, ECClassTypeEnum.HeightTool);
            if (reader != null)
            {
                try
                {
                    HeightToolData data = new HeightToolData(cell);
                    var toolEl = new HeightToolElement(data);
                    toolEl.AddToModel();
                }
                catch(Exception) {}
            }
        }

// }
    }
}
}