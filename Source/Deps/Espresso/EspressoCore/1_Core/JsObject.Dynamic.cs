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
    public class DynamicObject
    {
        //member cache
        Dictionary<string, object> members = new Dictionary<string, object>();
        public object this[string name]
        {
            get
            {
                object foundMember;
                if (!members.TryGetValue(name, out foundMember))
                {
                    if (this.TryGetMember(name, out foundMember))
                    {
                        //found
                        members[name] = foundMember;
                        return foundMember;
                    }
                    return null;
                }
                return foundMember;
            }
            set
            {
                this.TrySetMember(name, value);
                //this.members[name] = value;
            }
        }
 
        public virtual bool TryGetMember(string mbname, out object result)
        {
            result = null;
            return false;
        } 
        public virtual bool TrySetMember(string mbname, object value)
        {
            return false;
        }
        public virtual IEnumerable<string> GetDynamicMemberNames()
        {
            return null;
        }
    }
     

    public class JsObject : DynamicObject, IDisposable
    {
        public JsObject(JsContext context, IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("can't wrap an empty object (ptr is Zero)", "ptr");

            _context = context;
            _handle = ptr;
        }

        readonly JsContext _context;
        readonly IntPtr _handle;

        public IntPtr Handle
        {
            get { return _handle; }
        }
        public virtual bool TryInvokeMember(string name, object[] args, out object result)
        {
            result = _context.InvokeProperty(this, name, args);
            return true;
        }
       
        public override bool TryGetMember(string mbname, out object result)
        {
            return (result = _context.GetPropertyValue(this, mbname)) != null;

        }
        
        public override bool TrySetMember(string mbname, object value)
        {
            _context.SetPropertyValue(this, mbname, value);
            return true;
        }
        
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _context.GetMemberNames(this);
        }

        #region IDisposable implementation

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                throw new ObjectDisposedException("JsObject:" + _handle);

            _disposed = true;

            _context.Engine.DisposeObject(this.Handle);
        }

        ~JsObject()
        {
            if (!_disposed)
                Dispose(false);
        }

        #endregion
    }

}


