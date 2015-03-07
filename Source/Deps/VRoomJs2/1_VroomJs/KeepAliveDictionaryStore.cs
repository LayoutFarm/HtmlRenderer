// This file is part of the VroomJs library.
//
// Author:
//     Federico Di Gregorio <fog@initd.org>
//
// Copyright (c) 2013 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace VroomJs
{
    public class KeepAliveDictionaryStore : IKeepAliveStore
    {
        Dictionary<int,object> _store = new Dictionary<int,object>();
        int _store_index = 1;

        public int MaxSlots {
            get { return Int32.MaxValue; }
        }

        public int AllocatedSlots {
            get { return _store.Count; }
        }

        public int UsedSlots {
            get { return _store.Count; }
        }

        public int Add(object obj)
        {
            _store.Add(_store_index, obj);
            return _store_index++;
        }

        public object Get(int slot)
        {
            object obj;
            if (_store.TryGetValue(slot, out obj))
                return obj;
            return null;
        }

        public void Remove(int slot)
        {
            var obj = Get(slot);
            if (obj != null) {
                var disposable = obj as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
                _store.Remove(slot);
            }
        }

        public void Clear()
        {
            _store.Clear();
        }
    }
}
