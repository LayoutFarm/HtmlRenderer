//2014 ,BSD, WinterDev

using System;
using System.Text;
using System.Collections.Generic;

namespace HtmlRenderer
{
  

    class ValueMap<T>
    {
        static Type mapNameAttrType = typeof(MapAttribute);

        readonly Dictionary<string, T> stringToValue;
        readonly Dictionary<T, string> valueToString;
        public ValueMap()
        {

            LoadAndAssignValues(out stringToValue, out valueToString);
        }


        static void LoadAndAssignValues<T>(out Dictionary<string, T> stringToValue, out Dictionary<T, string> valueToString)
        {
            stringToValue = new Dictionary<string, T>();
            valueToString = new Dictionary<T, string>();

            var fields = typeof(T).GetFields();

            for (int i = fields.Length - 1; i >= 0; --i)
            {
                var field = fields[i];
                MapAttribute cssNameAttr = null;
                var customAttrs = field.GetCustomAttributes(mapNameAttrType, false);
                if (customAttrs != null && customAttrs.Length > 0 &&
                   (cssNameAttr = customAttrs[0] as MapAttribute) != null)
                {
                    T value = (T)field.GetValue(null);
                    stringToValue.Add(cssNameAttr.Name, value);//1.
                    valueToString.Add(value, cssNameAttr.Name);//2.                    

                }
            }

        }
        public string GetStringFromValue(T value)
        {
            string found;
            valueToString.TryGetValue(value, out found);
            return found;
        }
        public T GetValueFromString(string str, T defaultIfNotFound)
        {
            T found;
            if (stringToValue.TryGetValue(str, out found))
            {
                return found;
            }
            return defaultIfNotFound;
        }
        public int Count
        {
            get
            {
                return this.valueToString.Count;
            }
        }
    }


}