using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reports.Tests
{
/// <summary>
/// Класс для тестирования Key-in программного модуля TOOLSREPORTS
/// </summary>
[TestClass()]
public class KeyInsTests
{
private Process StartKeyInProc(string keyInArg)
{
    var proccess = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            //@"D:\REPOS\AEPSalvator\SALVATOR\AEPSalvator\bin\Debug\AEPSalvator.exe"
            FileName = @"c:\Work\АЭП\SALVATOR-master\AEPSalvator\bin\Debug\AEPSalvator.exe",
            Arguments = String.Concat("KEYIN ", string.Format(
                " \"mdl load SPDSTOOLS; SPDSTOOLS {0}\"", keyInArg))
        }
    };
    proccess.Start();
    proccess.WaitForExit();
    return proccess;
}

[TestMethod()]
public void TOOLSREPORTS_PLACE_HEIGHT_Test()
{
    // todo TOOLSREPORTS_PLACE_HEIGHT_Test
    Process proccess = StartKeyInProc("PLACE HEIGHT");
}

}
}
