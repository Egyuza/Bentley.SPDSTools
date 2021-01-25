using System;
using System.Collections.Generic;
using System.Linq;

using Bentley.MstnPlatformNET;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using static Bentley.MstnPlatformNET.AddIn.SelectionChangedEventArgs;

namespace SPDSTools
{
public delegate void 
SelectionChagedHandler(IEnumerable<ToolData> selection, uint filePosition);

class SelectionHelper : IDisposable
{
    public event SelectionChagedHandler SelectionChanged;

    // КОНСТРУКТОР
    private SelectionHelper()
    {
        elemsData_ = new Dictionary<Element, ToolData>();
        SPDSTools.Instance.SelectionChangedEvent += Instance_SelectionChangedEvent;
    }
    public void Dispose()
    {
        SPDSTools.Instance.SelectionChangedEvent -= Instance_SelectionChangedEvent;
        instance_ = null;
    }

    public static SelectionHelper Instance
    {
        get { return instance_ = instance_ ?? new SelectionHelper(); }
    }

    public static void Select(Element element, bool modCtrlKey)
    {
        if (element == null) // при Ctrl не сбрасываем
        {
            if (!modCtrlKey) {
                SelectionSetManager.EmptyAll();
            }
            return;
        }   
         
        DgnModel model = Session.Instance.GetActiveDgnModel();
        if (modCtrlKey) 
            SelectionSetManager.InvertElement(element, model);
        else
            SelectionSetManager.ReplaceWithElement(element, model);
    }

    // TODO можно оптимизировать с помощью отражения ElementId - ulong 
    // private SortedSet<ulong> _selectedIds;

    public IEnumerable<ToolData> SelectedData
    { 
        get { return elemsData_.Values; }
    }

    public List<HeightToolData> GetSelectedHeightData()
    {
        List<HeightToolData> res = new List<HeightToolData>();
        foreach (ToolData data in elemsData_.Values)
        {
            HeightToolData hdata = data as HeightToolData;
            if (hdata != null)
                res.Add(hdata);
        }
        return res;
    }

    public void setIsActive(bool value)
    {
        isActive_ = value;
    }

    private static SelectionHelper instance_;
    private bool isInProcess_;
    private bool isActive_;
    
    private Dictionary<Element, ToolData> elemsData_;

    private void Instance_SelectionChangedEvent(
        AddIn sender, AddIn.SelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Action == ActionKind.DoubleClickElement)
        {
            isActive_ = true;
        }
        else if (eventArgs.Action == ActionKind.SetEmpty)
        {
            isActive_ = false;
        }
        else if (eventArgs.Action != ActionKind.SetChanged &&
            eventArgs.Action != ActionKind.SetEmpty)
        {
            return;
        }
        
        if (isInProcess_)
            return;
        
        isInProcess_ = true;

        try
        {
            synchronize();

            if (isActive_) 
            {
                SelectionChanged?.Invoke(SelectedData, eventArgs.FilePosition);
            }
        }
        finally
        {
            isInProcess_ = false;
        }        
    }
    
    private static List<Element> getSelectedElements()
    {
        var res = new List<Element>();
        uint nums = SelectionSetManager.NumSelected();
        if (nums == 0)
            return res;

        Element element = null;
        
        DgnModelRef modelRef = null;
        for (uint i = 0; i < nums; ++i)
        {
            if (StatusInt.Success ==
                SelectionSetManager.GetElement(i, ref element, ref modelRef))
            {
                res.Add(element); 
            }
        }
        return res;
    }

    /// <summary>
    /// Синхронизация с системным селектором объектов
    /// </summary>
    private void synchronize()
    {
        if (SelectionSetManager.NumSelected() == 0)
        {
            elemsData_.Clear();
            return;
        }
        
        // SortedSet нельзя, т.к. Element не реализует IComparable
        List<Element> selected = getSelectedElements();

        {   // удаляем старые:
            Stack<Element> removeStack = new Stack<Element>();
            foreach (Element element in elemsData_.Keys)
            {
                if (false == selected.Contains(element))
                    removeStack.Push(element);                
            }
            while (removeStack.Count > 0)
            {
                elemsData_.Remove(removeStack.Pop());
            }
        }        
        {   // добавляем новые:
            foreach (Element element in selected)
            {
                if (elemsData_.ContainsKey(element))
                    continue;
                
                CellHeaderElement cell = element as CellHeaderElement;
                if (cell == null || !cell.IsSPDSElement())
                {                                       
                    //SelectionSetManager.EmptyAll(); // 2019-04-25 - откл.
                    isActive_ = false;
                    return;
                }

                if (element.IsSPDSHeightElem()) {
                    elemsData_.Add(element, new HeightToolData(cell));
                }
            }
        }
        // проверка на выделение элементов только одного типа:
        // можно 
        if (selected.Count != GetSelectedHeightData().Count)
        {
            SelectionSetManager.EmptyAll();
            elemsData_.Clear();
        }
    }

}
}
