using Bentley.DgnPlatformNET.DgnEC;
using Bentley.ECObjects.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPDSTools
{
 /// <summary> Абстрактный, базовый класс для автоматизации создания 
 /// перечисления уникальных имён EC-свойств </summary> 
public abstract class ToolProperty
{
    private struct _ToolSets
    {
        public ECClass ECClass;
        public Dictionary<int,string> valToType;

        public _ToolSets(string toolName)
        {
            valToType = new Dictionary<int, string>();
            ECClass = new ECClass(toolName);
            ECClass.IsCustomAttribute = true; // todo ? зачем нужен CustomAttribute
        }
    }

    public string Name { get; private set; }
    public string ToolName { get; private set; }
    public IECPrimitiveType ValueType { get; private set; }

    protected readonly int intVal;

    private static Dictionary<string, _ToolSets>  _toolSets = 
        new Dictionary<string, _ToolSets>();

    // КОНСТРУКТОР
    protected ToolProperty(string toolName, string propName, IECPrimitiveType valueType)
    {
        if (!_toolSets.ContainsKey(toolName))
        {
            _toolSets.Add(toolName, new _ToolSets(toolName));
        }
        else if (_toolSets[toolName].valToType.ContainsValue(propName))
        {
            throw new ArgumentException(string.Format(
                "value \"{0}\" is already exists in <{1}>", propName, 
                this.GetType().ToString()));
        }

        _ToolSets sets = _toolSets[toolName];

        ToolName = toolName;
        Name = propName;
        ValueType = valueType;
        intVal = sets.valToType.Count;

        sets.valToType.Add(intVal, propName);

        // каждое объявленное свойство записывается в EC класс для поддержания
        // целестности EC схемы
        ECProperty prop = new ECProperty(Name, ValueType) 
        { IsReadOnly = true, DisplayLabel = null };
        sets.ECClass.Add(prop);
    }        

    /// <summary>
    /// Получает EC-класс, ассациированный с именем SPDS-инструмента
    /// </summary>
    /// <param name="toolName">имя SPDS-инструмента</param>
    /// <returns></returns>
    public static ECClass GetClass(string toolName)
    {
        return _toolSets.ContainsKey(toolName) ? 
            _toolSets[toolName].ECClass : null;
    }

    /// <summary>
    /// Получает EC-класс, которому принадлежит свойство
    /// </summary>
    /// <param name="toolName">имя SPDS-инструмента</param>
    /// <returns></returns>
    public ECClass GetClass()
    {
        return _toolSets.ContainsKey(ToolName) ? 
            _toolSets[ToolName].ECClass : null;
    }
  
    public override string ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return intVal;
    }
}    
}
