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

        List<T> itemCollection = new List<T>();
        Stack<LevelInfo> levelInfoStack = new Stack<LevelInfo>();
        int currentLayerStartAt = 0;
        int currentLayerItemCount = 0;
        public void AddLayerItem(T item)
        {
            itemCollection.Add(item);
            this.currentLayerItemCount++;
        }
        public void ClearLayerItems()
        {
            //clear in current layer  
            for (int i = 0; i < currentLayerItemCount; ++i)
            {
                itemCollection.RemoveAt(i);
            }
            this.currentLayerItemCount = 0;
        }
        public int CurrentLayerItemCount
        {
            get { return this.currentLayerItemCount; }
        }
        public T GetItem(int index)
        {
            //get item at current layer
            return itemCollection[currentLayerStartAt + index];
        }
        public void EnterNewContext()
        {
            //store last info
            levelInfoStack.Push(new LevelInfo(this.currentLayerStartAt, this.currentLayerItemCount));
            this.currentLayerItemCount = 0;
            this.currentLayerStartAt = itemCollection.Count;
        }
        public void ExitCurrentContext()
        {
            //clear all item in prev layer
            LevelInfo prevLayerInfo = this.levelInfoStack.Pop();
            this.currentLayerStartAt = prevLayerInfo.currentLayerStartAt;
            this.currentLayerItemCount = prevLayerInfo.currentLeyerCount;
        }
    }
}