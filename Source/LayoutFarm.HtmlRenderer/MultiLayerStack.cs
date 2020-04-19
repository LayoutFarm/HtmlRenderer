//BSD, 2014-present, WinterDev 


using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
{
    class MultiLayerStack<T>
    {
        struct LevelInfo
        {
            public readonly int currentLayerStartAt;
            public readonly int currentLeyerCount;
            public LevelInfo(int startAt, int count)
            {
                this.currentLayerStartAt = startAt;
                this.currentLeyerCount = count;
            }
        }

        List<T> _itemCollection = new List<T>();
        Stack<LevelInfo> _levelInfoStack = new Stack<LevelInfo>();
        int _currentLayerStartAt = 0;
        int _currentLayerItemCount = 0;
        public void AddLayerItem(T item)
        {
            _itemCollection.Add(item);
            _currentLayerItemCount++;
        }
        public void ClearLayerItems()
        {
            //clear in current layer  
            for (int i = 0; i < _currentLayerItemCount; ++i)
            {
                _itemCollection.RemoveAt(i);
            }
            _currentLayerItemCount = 0;
        }
        public int CurrentLayerItemCount => _currentLayerItemCount;
        //
        public T GetItem(int index) => _itemCollection[_currentLayerStartAt + index];
        //
        public void EnterNewContext()
        {
            //store last info
            _levelInfoStack.Push(new LevelInfo(_currentLayerStartAt, _currentLayerItemCount));
            _currentLayerItemCount = 0;
            _currentLayerStartAt = _itemCollection.Count;
        }
        public void ExitCurrentContext()
        {
            //clear all item in prev layer
            LevelInfo prevLayerInfo = _levelInfoStack.Pop();
            _currentLayerStartAt = prevLayerInfo.currentLayerStartAt;
            _currentLayerItemCount = prevLayerInfo.currentLeyerCount;
        }
    }
}