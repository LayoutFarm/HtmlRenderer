#region License
// Copyright (c) 2013 Antonie Blom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Collections.Generic;
namespace Pencil.Gaming
{
    public sealed class KeyboardState
    {
        private Dictionary<Key, bool> keys = new Dictionary<Key, bool>(52);
        private static Key[] allKeys;
        static KeyboardState()
        {
            Array allKeysa = Enum.GetValues(typeof(Key));
            allKeys = new Key[allKeysa.Length];
            for (int i = 0; i < allKeysa.Length; ++i)
            {
                allKeys[i] = (Key)allKeysa.GetValue(i);
            }
        }

        private KeyboardState()
        {
        }

        public static KeyboardState GetState(GlfwWindowPtr window)
        {
            KeyboardState result = new KeyboardState();
            for (int i = 0; i < allKeys.Length; ++i)
            {
                Key k = allKeys[i];
                result.keys[k] = Glfw.GetKey(window, k);
            }

            return result;
        }

        public bool this[Key k]
        {
            get
            {
                return keys[k];
            }
        }
    }
}

