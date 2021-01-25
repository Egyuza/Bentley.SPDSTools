using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPDSTools
{
/// <summary>
/// Интерфейс нужен из-за проблемы многоуровневой имплементации класса 
/// UserControl, после чего он повреждает дизайнер формы, где используется
/// </summary>
public interface IToolControl
{
    UserControl Control { get; }

    void LoadToolData (ToolData data);

    void ApplyChanges();
    void ResetChanges();
}
}
