using System;

namespace SPDSTools.Parameters
{
public interface ICfgParameter : IDisposable
{
    string Name { get; }
    bool BoolValue { get; }
    char CharValue { get; }
    double DoubleValue { get; }
    Enum EnumValue { get; }
    int IntValue { get; }
    uint UIntValue { get; }
    string StringValue { get; }
    string DefaultValue { get; }
    bool IsOptional { get; }
}
}