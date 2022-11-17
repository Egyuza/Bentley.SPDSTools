using System;
using System.Linq;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.ECObjects.Schema;
using Bentley.DgnPlatformNET.DgnEC;
using Bentley.ECObjects.Instance;

namespace SPDSTools
{
/// <summary>
/// Класс - средство записи EC-свойств в элемент
/// </summary>
public class ECPropertyWriter
{
    private readonly Element element;
    private readonly StandaloneECDInstance ecInst;
    private readonly IECClass instClass;
    
    private ECPropertyWriter(Element element, IECSchema schema,
        ECClassTypeEnum instType)
    {
        if (element == null)
            throw new ArgumentNullException("element");
        if (schema == null)
            throw new ArgumentNullException("schema");

        // !проверка, что элемент добавлен в модель
        if (element.DgnModel == null || element.ElementId == null)
            throw new ArgumentException(string.Format(
                "Couldn't write EC-properties to non-model element"));

        this.element = element;
        DgnFile file = element.DgnModel.GetDgnFile();

        this.instClass = schema.GetClass(EnumString.ToString(instType));
        if (this.instClass == null)
        {
            //throw new Exception(
            //    "Couldn't get instance of ECSchema for writing properties");
        }
        
        var mgr = DgnECManager.Manager;

        this.ecInst = instClass == null ? null :
            mgr.ObtainInstanceEnabler(file, instClass)?.SharedWipInstance;

        using (DgnECInstanceCollection ecInstances =
        mgr.GetElementProperties(element, ECQueryProcessFlags.SearchAllClasses))
        {
            IDgnECInstance inst = ecInstances.FirstOrDefault(x =>
                x.ClassDefinition.Name == EnumString.ToString(instType));
            if (inst != null)
            {
                // this.ecInst = mgr.ObtainInstanceEnabler(file, inst.ClassDefinition)?.SharedWipInstance;

                //foreach (IECPropertyValue propVal in inst)
                //{
                //    if (ecInst.ClassDefinition.Contains(propVal.Property))
                //        SetValue(propVal);
                //}
            }
        }
    }

    /// <summary>
    ///  Получение экземпляра средства записи EC-свойств в dgn-элемент
    /// </summary>
    public static ECPropertyWriter Get(Element element, IECSchema schema,
        ECClassTypeEnum instType)
    {       
        var writter = new ECPropertyWriter(element, schema, instType);        
        return writter.ecInst != null ? writter : null;
    }
    
    public void SetValue(IECPropertyValue propValue)
    {
        if (propValue == null)
            throw new ArgumentNullException("propValue");        

        SetValue(propValue.Property.Name, propValue.NativeValue);       
    }

    public void SetValue<T>(string propName, T value)
    {
        if (string.IsNullOrWhiteSpace(propName))
            throw new ArgumentException("propName");
        if (value == null)
            return; // пропускаем null

        ECDMemoryBuffer memBuff = ecInst.MemoryBuffer;        
        
        if (value is string)
            memBuff.SetStringValue(propName, -1, Convert.ToString(value));
        else if (value is bool)
            memBuff.SetBooleanValue(propName, -1, Convert.ToBoolean(value));
        else if (value is int || value is Enum)
             memBuff.SetIntegerValue(propName, -1, Convert.ToInt32(value));
        else if (value is long)
            memBuff.SetLongValue(propName, -1, Convert.ToInt32(value));
        else if (value is double || value is decimal)
            memBuff.SetDoubleValue(propName, -1, Convert.ToDouble(value));
        else if (value is DateTime)
            memBuff.SetDateTimeTickValue(propName, -1, Convert.ToDateTime(value).Ticks);
        else
        {
            throw new SPDSException("Unexpected type of Property \"{0}\"",
                propName);
        }
           
    }

    /// <summary>
    /// Сохранение записей EC-свойств на элементе.
    /// Если заданный EC-класс уже существовал ранее - он удалится.
    /// </summary>
    public void SaveOnElement()
    {
        try
        {
            DgnECInstanceCollection oldInstColl = 
                DgnECManager.Manager.GetElementProperties(element, 
                ECQueryProcessFlags.SearchAllClasses);

            foreach (IDgnECInstance oldInst in oldInstColl.Where(
                x => x.ClassDefinition.Name == instClass.Name))
            {
                oldInst.Delete(); // удаляем старый класс с тем же имененем
            }

            // привязка свойств к элементу:
            var propInst = (ecInst.Enabler as DgnECInstanceEnabler).
                CreateInstanceOnElement(element, ecInst, true);

        }
        catch (Exception ex)
        {
            throw new SPDSException(ex, string.Format(
                "Couldn't write EC-properties on Element (Id={0})", 
                element.ElementId));
        }
    }
}
}
