//BSD 2010-2014 ,WinterCore

using System;
using System.Collections.Generic;
using System.Text;  

namespace HtmlRenderer.WebDom
{
     
    class UniqueStringTable
    {
        Dictionary<string, int> dic;
        List<string> list;
        public UniqueStringTable()
        {
            dic = new Dictionary<string, int>();
            list = new List<string>();
            dic.Add(string.Empty, 0);//empty string
            list.Add(string.Empty);
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
            if (dic.TryGetValue(str, out foundIndex))
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
            if (dic.TryGetValue(str, out foundIndex))
            {
                return foundIndex;
            }
            else
            {   
                int index = dic.Count;
                dic.Add(str, index);
                list.Add(str);
                return index;
            }
        }
        public bool Contains(string str)
        {
            return dic.ContainsKey(str);
        }
        public int Count
        {
            get
            {
                return dic.Count;
            }
        }
        public string GetString(int index)
        {
            return list[index];
        }
        public IEnumerable<string> WordIter
        {
            get
            {
                foreach (string str in dic.Keys)
                {
                    yield return str;
                }
            }
        }
        internal List<string> GetStringList()
        {
            return list;
        }

        
        public UniqueStringTable Clone()
        {
            UniqueStringTable newClone = new UniqueStringTable();
            Dictionary<string, int> cloneDic = newClone.dic;
            cloneDic.Clear();
            foreach (KeyValuePair<string, int> kp in this.dic)
            {
                cloneDic.Add(kp.Key, kp.Value);
            }
            newClone.list.Clear();
            newClone.list.AddRange(list);

            return newClone;
        }
    }
}