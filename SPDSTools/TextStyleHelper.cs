using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SPDSTools
{
public static class TextStyleHelper
{    
    const int SUCCESS = 0;   

    [DllImport("stdmdlbltin.dll")]
    public static extern int mdlTextStyle_addToFile(
        ref int pNewTableEntryId, IntPtr pStyle, IntPtr pStyleName, int bLockEntry); 

    [DllImport("stdmdlbltin.dll")]
    public static extern int mdlTextStyle_getByName(
        ref IntPtr pStyle, ref IntPtr pTextStyleId, IntPtr pStyleName, IntPtr modelRef,
            int SearchLibrary);

    public static bool AddTextStyle(DgnModel model, string name, 
        bool searchLib, bool lockEntry = false)
   {
        IntPtr styleAddress = new IntPtr();
        IntPtr styleIdAddress = new IntPtr(-1);

        IntPtr pntrName = Marshal.StringToCoTaskMemUni(name);        
    
        if (SUCCESS == mdlTextStyle_getByName(ref styleAddress, ref styleIdAddress, 
            pntrName, model.GetNative(), searchLib ? 1 : 0))
        {
            int newId = 0;
            int LockEntry = lockEntry ? 1 : 0 ; // TODO проверить что даёт
            return (SUCCESS == mdlTextStyle_addToFile(ref newId, styleAddress, 
                pntrName, LockEntry));
        }
        return false;
    }

    public static DgnTextStyle GetTextStyle(string name, DgnModel model, 
        bool activateLibStyles)
    {
        DgnFile file = model.GetDgnFile();
        DgnTextStyle textStyle = DgnTextStyle.GetByName(name, file);

        if (textStyle == null && activateLibStyles)
        {
            // пробуем активировать библиотечный стиль
            Session.Instance.Keyin(
                string.Format("textstyle active {0}", name));
            textStyle = DgnTextStyle.GetByName(name, file);

            if (textStyle == null && // пробуем добавить библиотечный стиль
                TextStyleHelper.AddTextStyle(model, name, true, false))
            {
                textStyle = DgnTextStyle.GetByName(name, file);
            }
        }

        return textStyle != null ? textStyle : DgnTextStyle.GetSettings(file);
    }


    //public static DgnTextStyle GetTextStyle(string name, DgnModel model, 
    //    bool searchLib)
    //{
    //    var textStyle = DgnTextStyle.GetByName(name, model.GetDgnFile());

    //    if (textStyle == null) // в файле стиля нет
    //    {
    //        // пробуем через библиотеку
    //        IntPtr stylePtr = new IntPtr();
    //        IntPtr styleIdAddress = new IntPtr(-1);
    //        IntPtr pntrName = Marshal.StringToCoTaskMemUni(name); 

    //        if (SUCCESS == mdlTextStyle_getByName(ref stylePtr, ref styleIdAddress, 
    //        pntrName, model.GetNative(), searchLib ? 1 : 0))
    //        {
    //            return DgnTextStyle.GetFromNativeDgnTextStyle(stylePtr);
                
    //            //int newId = 0;
    //            //int LockEntry = 0 ; // ' No idea what that does!
    //            //return (SUCCESS == mdlTextStyle_addToFile(ref newId, stylePtr, 
    //            //    pntrName, LockEntry));
    //        }
    //    }

    //    return textStyle;
    //    //return textStyle != null ? 
    //    //    textStyle : DgnTextStyle.GetSettings(model.GetDgnFile());
    //}
}
}
