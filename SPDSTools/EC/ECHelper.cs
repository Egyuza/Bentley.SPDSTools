using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bentley.DgnPlatformNET;
using Bentley.ECObjects.Schema;
using Bentley.DgnPlatformNET.DgnEC;
using Bentley.DgnPlatformNET.Elements;
using Bentley.ECObjects;
using Bentley.EC.Persistence.Query;
using Bentley.ECObjects.Instance;
using Bentley.MstnPlatformNET;

namespace SPDSTools
{
// todo ! проверить нужен ли?
internal static class ECHelper
{
    // todo максимально перенести работу с EC-свойствами в этот класс

    public const string SCHEMA_NAME = "SPDSTools";
    public const string ECCLASS_COMMON_NAME = "Tool";
    public const string ECPROP_COMMON_TYPE = "Type";

    public const int VrnMajor = 1;
    public const int VrnMinor = 0;
        
    static ECHelper()
    {
        //Schema = new ECSchema("SPDSTool", 1, 0, _appName);
        //Common = new ECClass("Common");
        //Schema.AddClass(Common);
        //Common.Add(new ECProperty("ToolType", ECObjects.StringType));
    }
    public static string SchemaFullName
    {
        get
        {
            return string.Format("{0}.{1:d2}.{2:d2}", SCHEMA_NAME,
                VrnMajor, VrnMinor);
        }
    }

    public static SchemaImportStatus ImportSchema(ECSchema schema, DgnFile file)
    {
        return DgnECManager.Manager.ImportSchema(schema, file, 
            new ImportSchemaOptions());       
    }

    public static Element TryGetElement(DgnModelRepositoryConnection modelConn, string instanceId)
    {
        Element element;
        DgnECManager.TryGetElementInfo(modelConn, instanceId, out element);
        return element;
    }
    
    private static DgnFile cachedFile;
    private static IECSchema cachedSchema;

    public static IECSchema GetSchema(Element element)
    {
        if (element == null)
            throw new ArgumentNullException();

        return GetSchema(element.DgnModel.GetDgnFile());
    }

    public static IECSchema GetSchema(DgnModel model)
    {
        if (model == null)
            throw new ArgumentNullException();

        return GetSchema(model.GetDgnFile());
    }

    public static IECSchema GetSchema(DgnFile file)
    {
        if (file == null)
            throw new ArgumentNullException();

        if (file == cachedFile && ECHelper.cachedSchema != null)
            return ECHelper.cachedSchema;
        
        cachedSchema = null;
        cachedFile = file;
        { // todo Правильная инициализация схемы:
            // в.1
            cachedSchema = DgnECManager.Manager.LocateDeliveredSchema(SCHEMA_NAME, 
                VrnMajor, VrnMinor, SchemaMatchType.LatestCompatible, file);
            // в.2
            //FindInstancesScope scope = FindInstancesScope.CreateScope(file, new FindInstancesScopeOption());
            //Schema = DgnECManager.Manager.LocateSchemaInScope(scope, "SPDSTools", 1, 0, SchemaMatchType.LatestCompatible);
        }

        if (cachedSchema == null)
            return null;

        IEnumerable<string> schemas = DgnECManager.Manager.DiscoverSchemas(
            file, ReferencedModelScopeOption.All, true);
        
        if (!schemas.Contains(cachedSchema.FullName))
        { // схема не подготовлена для использования в файле:
            SchemaImportStatus impStat = DgnECManager.Manager.ImportSchema(
                cachedSchema, file, new ImportSchemaOptions(
                   isExternal: true, 
                   importReferencedSchemas: false,
                   registerHighPriorityLocater: true));

            if (impStat != SchemaImportStatus.Success)
            { // todo !
                throw new SPDSException("Failed to initialize ECSchema: {0}", SchemaFullName);
            }
        }
        return cachedSchema;
    }
    
    public static List<CellHeaderElement> FindSPDSElementsByInstance(DgnModel model)
    {
        if (model == null)
            throw new ArgumentNullException();
        
        DgnFile file = model.GetDgnFile();
        DgnFileOwner fileOwner = new DgnFileOwner(file);
        
        IECSchema Schema = null;
        { // todo Правильная инициализация схемы:
            Schema = DgnECManager.Manager?.LocateDeliveredSchema(
                ECHelper.SCHEMA_NAME,
                VrnMajor, VrnMinor, SchemaMatchType.LatestCompatible, file);
        }

        if (Schema == null)
            return null;
        
        IECClass commonClass = Schema.GetClass(ECCLASS_COMMON_NAME);        
        
        ECQuery query = new ECQuery(commonClass);
        
        var modelConn = DgnModelRepositoryConnection.CreateConnection(
            new DgnECConnectionOptions(), fileOwner, model);

        var instColl = DgnECManager.Manager.FindDgnECInstances(modelConn, query);

        if (instColl == null)
            return null;

        List<CellHeaderElement> resList = new List<CellHeaderElement>();

        foreach(var item in instColl)
        {
            Element element;
            if (DgnECManager.TryGetElementInfo(modelConn, item.InstanceId,
                out element))
            {
                if (element is CellHeaderElement)
                    resList.Add(element as CellHeaderElement);
            }
        }
        return resList;
    }

    /// <summary>
    /// Проверка является ли элемент SPDS-элементом
    /// </summary>
    public static bool IsSPDSElement(this Element element)
    {
        CellHeaderElement cell = element as CellHeaderElement;
        if (cell == null)
            return false;
        
        var manager = DgnECManager.Manager;
        using (DgnECInstanceCollection ecInstances =
            manager.GetElementProperties(cell, ECQueryProcessFlags.SearchAllClasses))
        {
            return ecInstances.FirstOrDefault(x =>
                x.ClassDefinition.Name == ECCLASS_COMMON_NAME &&
                x.ClassDefinition.Schema.Name == SCHEMA_NAME) != null;
        }
    }

    public static bool IsSPDSHeightElem(this Element element)
    {
        if (!element.IsSPDSElement())
            return false;
        var reader = ECPropertyReader.TryGet(element, ECClassTypeEnum.HeightTool);
        return reader != null;
    }

        /// <summary>
    /// Проверка является ли элемент SPDS-элементом
    /// </summary>
    public static bool AsSPDSElement(this Element element)
    {
        if (!element.IsSPDSElement())
            return false;

        


        CellHeaderElement cell = element as CellHeaderElement;
        if (cell == null)
            return false;
        


        var manager = DgnECManager.Manager;
        using (DgnECInstanceCollection ecInstances =
            manager.GetElementProperties(cell, ECQueryProcessFlags.SearchAllClasses))
        {
            return ecInstances.FirstOrDefault(x =>
                x.ClassDefinition.Name == ECCLASS_COMMON_NAME &&
                x.ClassDefinition.Schema.Name == SCHEMA_NAME) != null;
        }
    }

    /// <summary>
    /// Проверка является ли элемент SPDS-элементом
    /// </summary>
    public static ECClassTypeEnum GetToolType(this CellHeaderElement cell)
    {
        if (cell == null)
            throw new ArgumentNullException("cell");
        
        var manager = DgnECManager.Manager;
        using (DgnECInstanceCollection ecInstances = 
        manager.GetElementProperties(cell, ECQueryProcessFlags.SearchAllClasses))
        {
            IDgnECInstance inst = ecInstances.FirstOrDefault(x =>
                x.ClassDefinition.Name == ECCLASS_COMMON_NAME &&
                x.ClassDefinition.Schema.Name == SCHEMA_NAME);

            if (inst == null)
                return ECClassTypeEnum.Unknown;

            IECPropertyValue propVal;
            try
            {
                if ((propVal = inst.FirstOrDefault(
                    x => x.Property.Name == ECPropertyName.Common.Type)) != null)
                {
                    return EnumString.Parse<ECClassTypeEnum>(propVal.StringValue);
                }
            }
            catch (Exception) {}
        }
        return ECClassTypeEnum.Unknown;
    }
}
}
