using System;
using System.Collections.Generic;

using Bentley.DgnPlatformNET;
using CLVL = Bentley.DgnPlatformNET.ConfigurationVariableLevel;
using System.Collections;

namespace SPDSTools.Parameters
{
public abstract class CfgParameters: IEnumerable
{
    protected Dictionary<Enum, ICfgParameter> map;

    private readonly string _prefix;

    protected CfgParameters(string prefix)
    {
        _prefix = prefix;
        map = new Dictionary<Enum, ICfgParameter>();
    }

    /// <summary>
    /// param name="level" == User - для декларации переменных, имеющих временный характер и
    /// используемых только во время сеасна приложения, 
    /// переменные не сохраняются в файлах настроек конфиг. переменных
    /// </summary>
    private void Add<T>(Enum paramId, T defaultValue, 
        CfgParameterDelegate<T> func = null, bool isOptional = false)
    {
        string prePrefix = isOptional ? "OPTIONAL_" : "";
        map.Add(
            paramId, 
            new CfgParameter<T>(
                prePrefix + _prefix + EnumString.ToString(paramId), 
                defaultValue, func, isOptional
            )
        );
    }

    /// Специальные методы Add для ограничения и конкретизации 
    /// поддерживаемых типов:

    protected void Add(Enum paramId, bool defaultValue, 
        CfgParameterDelegate<bool> func = null, bool isOptional = false)
    {
        Add<bool>(paramId, defaultValue, func, isOptional);
    }

    protected void Add(Enum paramId, int defaultValue, 
        CfgParameterDelegate<int> func = null, bool isOptional = false)
    {
        Add<int>(paramId, defaultValue, func, isOptional);
    }

    protected void Add(Enum paramId, double defaultValue, 
        CfgParameterDelegate<double> func = null, bool isOptional = false)
    {
        Add<double>(paramId, defaultValue, func, isOptional);
    }

    protected void Add(Enum paramId, string defaultValue, 
        CfgParameterDelegate<string> func = null, bool isOptional = false)
    {
        Add<string>(paramId, defaultValue, func, isOptional);
    }

    protected void Add(Enum paramId, Enum defaultValue, 
        CfgParameterDelegate<Enum> func = null, bool isOptional = false)
    {
        Add<Enum>(paramId, defaultValue, func, isOptional);
    }

    public ICfgParameter this[Enum id]
    {
        get
        { 
            if (!map.ContainsKey(id))
            {
                SPDSException.Alert(string.Format(
                    "Конфигурационный параметр <{0}> не обрабатывается", id));
            }
            return map[id]; 
        }
   }
   
    public static void ExportInfoToFile(string filePath)
    {
        /// TODO реализовать выгрузку информации о всех используемых 
        /// конфиг. переменных в указанный текстовый файл
    }

    public static void setValue<T>(ICfgParameter parameter, T value)
    {
        if (parameter.IsOptional ||
            ConfigurationManager.GetVariable(parameter.Name, CLVL.User) != null)
        {
            try
            {
                //if (ConfigurationManager.IsVariableDefined(parameter.Name))
                //    ConfigurationManager.UndefineVariable(parameter.Name);

                ConfigurationManager.DefineVariable(
                    parameter.Name, value.ToString(), CLVL.User);
            }
            catch (Exception) {}
        }
        else
        {
            //if (ConfigurationManager.IsVariableDefined(parameter.Name))
            //    ConfigurationManager.UndefineVariable(parameter.Name);
            ConfigurationManager.DefineVariable(parameter.Name, 
                value.ToString(), CLVL.User);
        }
        }

    public IEnumerator GetEnumerator()
    {
        return map.Values.GetEnumerator();
    }
}

}
