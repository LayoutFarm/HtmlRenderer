//BSD, 2010-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.WebDom
{
    public class UniqueStringTable
    {
        Dictionary<string, int> _dic;
        List<string> _list;
        public UniqueStringTable()
        {
            _dic = new Dictionary<string, int>();
            _list = new List<string>();
            _dic.Add(string.Empty, 0);//empty string
            _list.Add(string.Empty);
        }
        /// <summary>
        /// get index for specific str, if not found return -1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetStringIndex(string str)
        {
            if (str == null)
            {
                return 0;
            }
            int foundIndex;
            if (_dic.TryGetValue(str, out foundIndex))
            {
                return foundIndex;
            }
            else
            {
                return -1;
            }
        }

        public int AddStringIfNotExist(string str)
        {
            if (str == null)
            {
                return 0;
            }
            //---------------------------------------
            int foundIndex;
            if (_dic.TryGetValue(str, out foundIndex))
            {
                return foundIndex;
            }
            else
            {
                int index = _dic.Count;
                _dic.Add(str, index);
                _list.Add(str);
                return index;
            }
        }
        //
        public bool Contains(string str) => _dic.ContainsKey(str);
        //
        public int Count => _dic.Count;
        //
        public string GetString(int index) => _list[index];
        //
        public IEnumerable<string> WordIter
        {
            get
            {
                foreach (string str in _dic.Keys)
                {
                    yield return str;
                }
            }
        }
        //
        internal List<string> GetStringList() => _list;



        public UniqueStringTable Clone()
        {
            UniqueStringTable newClone = new UniqueStringTable();
            Dictionary<string, int> cloneDic = newClone._dic;
            cloneDic.Clear();
            foreach (KeyValuePair<string, int> kp in _dic)
            {
                cloneDic.Add(kp.Key, kp.Value);
            }
            newClone._list.Clear();
            newClone._list.AddRange(_list);
            return newClone;
        }
    }
}