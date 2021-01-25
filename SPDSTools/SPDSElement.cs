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
    /// TODO идея : инкапсулировать всю возможную обработку spds-элементов 
    /// в классе SPDSElement

    public class SPDSElement : CellHeaderElement
    {
        protected SPDSElement(DgnModel dgnModel, string cellName, DPoint3d origin, 
                DMatrix3d rotation, IList<Element> children)
            :base(dgnModel, cellName, origin, DMatrix3d.Identity, children)
        {
            throw new NotImplementedException();
        }        
    }
}
