using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPDSTools
{
public class LeaderTool : Tool
{
    // КОНСТРУТОР
    public LeaderTool() 
    : base()
    {
       
    }

    //public override StatusInt StartTool()
    //{
    //    //ShowForm(new LeaderToolControl(), "LeaderTool");
    //    return this.InstallTool();
    //}

    public override StatusInt OnElementModify(Element element)
    {
        return StatusInt.Error;
    }

}
}
