using System;
using System.Globalization;
using Bentley.DgnPlatformNET;

using CLVL = Bentley.DgnPlatformNET.ConfigurationVariableLevel;

namespace SPDSTools.Parameters
{

public delegate T CfgParameterDelegate<T>(string item);

public class CfgParameter<T> : ICfgParameter
    {
    readonly string _name;
    readonly bool _isOptional;
    readonly T _defValule;
    readonly ConfigurationVariableMonitor _monitor;
    readonly CfgParameterDelegate<T> _func;

    string _strValue;
    T _Value;

    public string DefaultValue
    {
        get 
        {
            if (typeof(T) == typeof(Enum))
                return Convert.ChangeType(_defValule, typeof(int)).ToString();            
            else
                return _defValule.ToString();
        }
    }

    public bool IsOptional { get { return _isOptional; } }

    public CfgParameter(string name, T defaultValue,
        CfgParameterDelegate<T> func, bool isUserValue = false)
    {
        _name = name;
        _defValule = defaultValue;
        _strValue = defaultValue.ToString();

        _func = func;
        _isOptional = isUserValue;

        if (_isOptional && !ConfigurationManager.IsVariableDefined(_name))
        {
            //if (ConfigurationManager.IsVariableDefined(_name))
            //{
            //    // TODO Assert ошибка!
            //    throw new SPDSException(
            //        "Попытка задекларировать уже задекларированную" + 
            //        " пользовательскую конф. переменную: \"" + name + "\"");
            //}

            // декларируем только временные польз. переменные для собств. нужд

            ConfigurationManager.DefineVariable(_name, _strValue, CLVL.User);
            //ConfigurationManager.DefineVariable(_name, _strValue);
        }

        _monitor = new ConfigurationVariableMonitor(_name);
        _monitor.VariableHasChanged += Monitor_VariableHasChanged;

        Monitor_VariableHasChanged(_name);
    }

    private void Monitor_VariableHasChanged(string variableName)
    {
        _strValue = ConfigurationManager.GetVariable(_name);
        //_isOptional
        //    ? ConfigurationManager.GetVariable(_name, CLVL.User)
        //    : ConfigurationManager.GetVariable(_name);

        _Value = parseValue(_strValue, _defValule);
    }

    public string Name { get { return _name; } }
    public string StringValue { get { return _strValue; } }
    public char CharValue { get { return convert<string>()[0]; } }
    public bool BoolValue { get { return convert<bool>(); } }
    public int IntValue { get { return convert<int>(); } }
    public uint UIntValue { get { return convert<uint>(); } }
    public double DoubleValue { get { return convert<double>(); } }
    public Enum EnumValue { get { return convert<Enum>(); } }
    public TVal GetValue<TVal>() { return convert<TVal>(); }

    private TVal convert<TVal>()
    {
        if (_func != null) // функцию вычисляем каждый раз
        {
             _Value = parseValue(_strValue, _defValule);
        }

        try
        {
            return (TVal)Convert.ChangeType(_Value, typeof(TVal));
        }
        catch (Exception)
        {
            return default(TVal);
        }
    }

    private T parseValue(string strVal, T defVal)
    {    
        try
        {
            if (typeof(T) == typeof(double))
            {
                // корректируем разделитель в соответствии с языковой средой
                string delimeter = CultureInfo.CurrentCulture.NumberFormat
                    .CurrencyDecimalSeparator;
                strVal = strVal.Replace(".", delimeter).Replace(
                    ",", delimeter);
            }
            
            if (_func != null)
            {                
                return _func(strVal);
            }
            else
            {
                T res;
                if (typeof(T) == typeof(Enum))
                    res = EnumString.Parse<T>(strVal, defVal.GetType());
                else
                    res =  (T)Convert.ChangeType(strVal, typeof(T));

                if (res != null)
                    return res;
            }
        }
        catch (Exception)
        {
            // todo localize exception message
        }

        // значение по умолчанию:
        return _defValule;
    }    

    public void Dispose()
    {
       ConfigurationManager.UndefineVariable(_name);
        _monitor.VariableHasChanged -= Monitor_VariableHasChanged;
    }
}
}
