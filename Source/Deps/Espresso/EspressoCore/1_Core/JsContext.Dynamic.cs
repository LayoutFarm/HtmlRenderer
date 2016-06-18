//2013 MIT, Federico Di Gregorio <fog@initd.org>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace VroomJs
{
    partial class JsContext
    {
        public IEnumerable<string> GetMemberNames(JsObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            CheckDisposed();
            if (obj.Handle == IntPtr.Zero)
                throw new JsInteropException("wrapped V8 object is empty (IntPtr is Zero)");
            JsValue v = jscontext_get_property_names(_context, obj.Handle);
            object res = _convert.FromJsValue(v);
            jsvalue_dispose(v);
            Exception e = res as JsException;
            if (e != null)
                throw e;
            object[] arr = (object[])res;
            string[] strArr = new string[arr.Length];
            for (int i = arr.Length - 1; i >= 0; --i)
            {
                strArr[i] = arr[i].ToString();
            }
            return strArr;
        }


        public object GetPropertyValue(JsObject obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (name == null)
                throw new ArgumentNullException("name");
            CheckDisposed();
            if (obj.Handle == IntPtr.Zero)
                throw new JsInteropException("wrapped V8 object is empty (IntPtr is Zero)");
            JsValue v = jscontext_get_property_value(_context, obj.Handle, name);
            object res = _convert.FromJsValue(v);
            jsvalue_dispose(v);
            Exception e = res as JsException;
            if (e != null)
                throw e;
            return res;
        }

        public void SetPropertyValue(JsObject obj, string name, object value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (name == null)
                throw new ArgumentNullException("name");
            CheckDisposed();
            if (obj.Handle == IntPtr.Zero)
                throw new JsInteropException("wrapped V8 object is empty (IntPtr is Zero)");
            JsValue a = _convert.AnyToJsValue(value);
            JsValue v = jscontext_set_property_value(_context, obj.Handle, name, a);
            object res = _convert.FromJsValue(v);
            jsvalue_dispose(v);
            jsvalue_dispose(a);
            Exception e = res as JsException;
            if (e != null)
                throw e;
        }

        public object InvokeProperty(JsObject obj, string name, object[] args)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (name == null)
                throw new ArgumentNullException("name");
            CheckDisposed();
            if (obj.Handle == IntPtr.Zero)
                throw new JsInteropException("wrapped V8 object is empty (IntPtr is Zero)");
            JsValue a = JsValue.Null; // Null value unless we're given args.
            if (args != null)
                a = _convert.AnyToJsValue(args);
            JsValue v = jscontext_invoke_property(_context, obj.Handle, name, a);
            object res = _convert.FromJsValue(v);
            jsvalue_dispose(v);
            jsvalue_dispose(a);
            Exception e = res as JsException;
            if (e != null)
                throw e;
            return res;
        }
    }
}
