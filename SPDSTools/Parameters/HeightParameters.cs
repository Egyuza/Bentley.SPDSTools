using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bentley.DgnPlatformNET;

namespace SPDSTools.Parameters
{
    using HP = HeightParametersEnum;
    using CLVL = Bentley.DgnPlatformNET.ConfigurationVariableLevel;
    using Bentley.MstnPlatformNET;

public enum HeightParametersEnum
{
//! у всех префикс "AEP_SPDS_HEIGHT_"

/// COMMON:
    [StringValue("STYLE")]
    STYLE,
    [StringValue("LEVEL")]
    LEVEL,
    [StringValue("TEXTSTYLE")]
    TEXTSTYLE,
    [StringValue("TEXTSTYLE_EDIT")]
    TEXTSTYLE_EDIT,
    [StringValue("DECIMALSEPARATOR")]
    DECIMALSEPARATOR,
    [StringValue("ZEROPREFIX")]
    ZEROPREFIX,
    [StringValue("TEXT_MARGIN_BEFORE")]
    TEXT_MARGIN_BEFORE,
    [StringValue("TEXT_MARGIN_TOP")]
    TEXT_MARGIN_TOP,
    [StringValue("TEXT_MARGIN_AFTER")]
    TEXT_MARGIN_AFTER,
    [StringValue("TEXT_MARGIN_BOTTOM")]
    TEXT_MARGIN_BOTTOM,
    
/// USER
    [StringValue("TYPE")]
    TYPE,
    [StringValue("TEXT")]
    TEXT,
    [StringValue("TEXT2")]
    TEXT2,
    [StringValue("AUTOCALC")]
    AUTOCALC,
    [StringValue("CALCBYITEM")]
    CALCBYITEM,
    [StringValue("SHOW_ITEMHEIGHT")]
    SHOW_ITEMHEIGHT,
    [StringValue("ALIGNMENT")]
    ALIGNMENT,
    [StringValue("AS_LEADER")]
    AS_LEADER,
    [StringValue("USE_FILLING")]
    USE_FILLING,
    [StringValue("SECTOR_ARROW")]
    SECTOR_ARROW,

/// DEFAULT:
    [StringValue("DEFAULT_SECTION_ORIGIN_GAP")]
    DEF_SEC_ORIGIN_GAP,
    [StringValue("DEFAULT_SECTION_ARROW_GAP_AFTER")]
    DEF_SEC_ARROW_GAP_AFTER,
    [StringValue("DEFAULT_SECTION_LENGTH_BEFORE_ARROW")]
    DEF_SEC_LENGTH_BEFORE_ARROW,
    [StringValue("DEFAULT_SECTION_ARROW_DX")]
    DEF_SEC_ARROW_DX,
    [StringValue("DEFAULT_SECTION_ARROW_DY")]
    DEF_SEC_ARROW_DY,
    [StringValue("DEFAULT_SECTION_LANDING_HEIGHT")]
    DEF_SEC_LANDING_HEIGHT,
    [StringValue("DEFAULT_PLAN_ARROW_FILLCOLOR")]
    DEF_PLAN_ARROW_FILLCOLOR,
    [StringValue("DEFAULT_PLAN_POINTRADIUS")]
    DEF_PLAN_POINTRADIUS,

/// PAKS:
    [StringValue("PAKS_ARROW_FILLCOLOR")]
    PAKS_ARROW_FILLCOLOR,
    [StringValue("PAKS_PLAN_ARROW_RADIUS")]
    PAKS_PLAN_ARROW_RADIUS,
    [StringValue("PAKS_PLAN_ARROW_RADIUS2")]
    PAKS_PLAN_ARROW_RADIUS2,
    [StringValue("PAKS_PLAN_LANDING_LENGTH")]
    PAKS_PLAN_LANDING_LENGTH,
    [StringValue("PAKS_SECTION_ARROW_DX")]
    PAKS_SEC_ARROW_DX,
    [StringValue("PAKS_SECTION_ARROW_DY")]
    PAKS_SEC_ARROW_DY,
    [StringValue("PAKS_SECTION_LINESTYLE")]
    PAKS_SEC_LINESTYLE,
    [StringValue("PAKS_SECTION_LANDING_LENGTH")]
    PAKS_SEC_LANDING_LENGTH,
}


public class HeightParameters : CfgParameters
{
    private static HeightParameters _inst;

    public static HeightParameters Instance
    {
        get { return _inst ?? (_inst = new HeightParameters()); }
    }

    private HeightParameters() : base("AEP_SPDS_HEIGHT_")
    {
        /// ОПЦИОНАЛЬНЫЕ:
        Add(HP.TYPE, HeightToolTypeEnum.Section, null, true);
        Add(HP.TEXT, "", null, true);
        Add(HP.TEXT2, "", null, true);
        Add(HP.AUTOCALC, true, null, true);
        Add(HP.CALCBYITEM, true, null, true);
        Add(HP.ALIGNMENT, AlignmentEnum.Auto, null, true);
        Add(HP.AS_LEADER, false, null, true);
        Add(HP.SHOW_ITEMHEIGHT, false, null, true);
        Add(HP.USE_FILLING, true, null, true);
        Add(HP.SECTOR_ARROW, false, null, true);

        // COMMON:
        Add(HP.STYLE, ToolStyleEnum.Default);
        Add(HP.LEVEL, 0, f_ParseLevelId);
        Add(HP.TEXTSTYLE, "");
        Add(HP.TEXTSTYLE_EDIT, true);
        Add(HP.DECIMALSEPARATOR, ".");
        Add(HP.ZEROPREFIX, "±");        
        {
            double before = 0, top = 0, after = 0, bottom = 0;
            switch ((ToolStyleEnum)this[HP.STYLE].EnumValue)
            {
            case ToolStyleEnum.Default:
                before = 3.0; top = bottom = 0.4; after = 1.1; break;
            case ToolStyleEnum.Paks:
            case ToolStyleEnum.ED:
                before = top = after = bottom = 0.3; break;
            }
            Add(HP.TEXT_MARGIN_BEFORE, before);
            Add(HP.TEXT_MARGIN_TOP, top);
            Add(HP.TEXT_MARGIN_AFTER, after);
            Add(HP.TEXT_MARGIN_BOTTOM, bottom);
        }
        
    /// DEFAULT:
        Add(HP.DEF_SEC_ARROW_DX, 2.8);
        Add(HP.DEF_SEC_ARROW_DY, 2.8);
        Add(HP.DEF_SEC_ARROW_GAP_AFTER, 2.4);
        Add(HP.DEF_SEC_ORIGIN_GAP, 1.0);
        Add(HP.DEF_SEC_LENGTH_BEFORE_ARROW, 25.0);
        Add(HP.DEF_SEC_LANDING_HEIGHT, 7.0);
        Add(HP.DEF_PLAN_ARROW_FILLCOLOR, 0); // цвет заливки
        Add(HP.DEF_PLAN_POINTRADIUS, 0.3);


    /// PAKS:
        Add(HP.PAKS_SEC_LINESTYLE, 0, f_ParseLineStyleIndex);
        Add(HP.PAKS_PLAN_ARROW_RADIUS, 1.35);
        Add(HP.PAKS_PLAN_ARROW_RADIUS2, 1.89);
        Add(HP.PAKS_PLAN_LANDING_LENGTH, 5.0);
        Add(HP.PAKS_ARROW_FILLCOLOR, 0); // цвет заливки указателя
        Add(HP.PAKS_SEC_ARROW_DX, 1.98);
        Add(HP.PAKS_SEC_ARROW_DY, 1.98);
        Add(HP.PAKS_SEC_LANDING_LENGTH, 5.0);



    }

    private int f_ParseLevelId(string levelName)
    {
        int res;
        if (int.TryParse(levelName, out res))
            return res;
        else
        {
            LevelHandle lh = Session.Instance.GetActiveDgnModel()
                .GetLevelCache().GetLevelByName(levelName, true);
            return lh.LevelId;
        }
    }

    public int f_ParseLineStyleIndex(string styleName)
    {
        int index = 0;
        styleName = styleName ?? "None";
        if (!int.TryParse(styleName, out index))
        {            
            try
            {
                LineStyleManager.Reinitialize();
                DgnFile file = Session.Instance.GetActiveDgnFile();
                index = LineStyleManager.GetNumberFromName(styleName, file, true, true);
                string checkName = LineStyleManager.GetNameFromNumber(index, file);
            } 
            catch(Exception) { }                
        }
        return index;
    }
}
}
