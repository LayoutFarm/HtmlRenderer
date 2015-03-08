//2015 MIT, WinterDev

// This file is part of the VroomJs library.
//
// Author:
//     Federico Di Gregorio <fog@initd.org>
//
// Copyright © 2013 Federico Di Gregorio <fog@initd.org>
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Timers;
using VroomJs;

namespace VroomJs
{

    public partial class JsContext : IDisposable
    {



        readonly int _id;
        readonly JsEngine _engine;
        readonly ManagedMethodCallDel engineMethodCallbackDel;

        List<JsMethodDefinition> registerMethods = new List<JsMethodDefinition>();
        List<JsPropertyDefinition> registerProperties = new List<JsPropertyDefinition>();

        Dictionary<Type, JsTypeDefinition> mappingJsTypeDefinition = new Dictionary<Type, JsTypeDefinition>();

        NativeObjectProxyStore proxyStore;

        JsTypeDefinitionBuilder jsTypeDefBuilder;

        internal JsContext(int id,
            JsEngine engine,
            HandleRef engineHandle,
            Action<int> notifyDispose,
            JsTypeDefinitionBuilder jsTypeDefBuilder)
        {

            _id = id;
            _engine = engine;
            _notifyDispose = notifyDispose;

            _keepalives = new KeepAliveDictionaryStore();
            _context = new HandleRef(this, jscontext_new(id, engineHandle));
            _convert = new JsConvert(this);

            this.jsTypeDefBuilder = jsTypeDefBuilder;

            engineMethodCallbackDel = new ManagedMethodCallDel(EngineListener_MethodCall);
            NativeV8JsInterOp.CtxRegisterManagedMethodCall(this, engineMethodCallbackDel);
            registerMethods.Add(null);//first is null
            registerProperties.Add(null); //first is null


            proxyStore = new NativeObjectProxyStore(this);

        }

        internal INativeRef GetObjectProxy(int index)
        {
            return this.proxyStore.GetProxyObject(index);
        }

        internal JsConvert Converter
        {
            get { return this._convert; }
        }
        internal void CollectionTypeMembers(JsTypeDefinition jsTypeDefinition)
        {

            List<JsMethodDefinition> methods = jsTypeDefinition.GetMethods();
            int j = methods.Count;
            for (int i = 0; i < j; ++i)
            {
                JsMethodDefinition met = methods[i];
                met.SetMemberId(registerMethods.Count);
                registerMethods.Add(met);
            }

            List<JsPropertyDefinition> properties = jsTypeDefinition.GetProperties();
            j = properties.Count;
            for (int i = 0; i < j; ++i)
            {
                var p = properties[i];
                p.SetMemberId(registerProperties.Count);
                registerProperties.Add(p);
            }

        }

        void EngineListener_MethodCall(int mIndex, int methodKind, IntPtr metArgs)
        {
            switch (methodKind)
            {
                case 1:
                    {
                        //property get        
                        if (mIndex == 0) return;
                        //------------------------------------------
                        JsMethodDefinition getterMethod = registerProperties[mIndex].GetterMethod;

                        if (getterMethod != null)
                        {
                            getterMethod.InvokeMethod(new ManagedMethodArgs(this, metArgs));
                        }

                    } break;
                case 2:
                    {
                        //property set
                        if (mIndex == 0) return;
                        //------------------------------------------
                        JsMethodDefinition setterMethod = registerProperties[mIndex].SetterMethod;
                        if (setterMethod != null)
                        {
                            setterMethod.InvokeMethod(new ManagedMethodArgs(this, metArgs));
                        }
                    } break;
                default:
                    {
                        if (mIndex == 0) return;
                        JsMethodDefinition foundMet = registerMethods[mIndex];
                        if (foundMet != null)
                        {
                            foundMet.InvokeMethod(new ManagedMethodArgs(this, metArgs));
                        }
                    } break;
            }


        }
        public JsEngine Engine
        {
            get { return _engine; }
        }
        readonly HandleRef _context;

        public HandleRef Handle
        {
            get { return _context; }
        }

        readonly JsConvert _convert;

        // Keep objects passed to V8 alive even if no other references exist.
        readonly IKeepAliveStore _keepalives;

        public JsEngineStats GetStats()
        {
            return new JsEngineStats
            {
                KeepAliveMaxSlots = _keepalives.MaxSlots,
                KeepAliveAllocatedSlots = _keepalives.AllocatedSlots,
                KeepAliveUsedSlots = _keepalives.UsedSlots
            };
        }

        public object Execute(JsScript script, TimeSpan? executionTimeout = null)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            CheckDisposed();

            bool executionTimedOut = false;
            Timer timer = null;
            if (executionTimeout.HasValue)
            {
                timer = new Timer(executionTimeout.Value.TotalMilliseconds);
                timer.Elapsed += (sender, args) =>
                {
                    timer.Stop();
                    executionTimedOut = true;
                    _engine.TerminateExecution();
                };
                timer.Start();
            }
            object res;
            try
            {
                JsValue v = jscontext_execute_script(_context, script.Handle);
                res = _convert.FromJsValue(v);
#if DEBUG_TRACE_API
        	Console.WriteLine("Cleaning up return value from execution");
#endif
                jsvalue_dispose(v);
            }
            finally
            {
                if (executionTimeout.HasValue)
                {
                    timer.Dispose();
                }
            }

            if (executionTimedOut)
            {
                throw new JsExecutionTimedOutException();
            }

            Exception e = res as JsException;
            if (e != null)
                throw e;
            return res;
        }

        public object Execute(string code, string name = null, TimeSpan? executionTimeout = null)
        {
            Stopwatch watch1 = new Stopwatch();
            Stopwatch watch2 = new Stopwatch();

            watch1.Start();
            if (code == null)
                throw new ArgumentNullException("code");

            CheckDisposed();

            bool executionTimedOut = false;
            Timer timer = null;
            if (executionTimeout.HasValue)
            {
                timer = new Timer(executionTimeout.Value.TotalMilliseconds);
                timer.Elapsed += (sender, args) =>
                {
                    timer.Stop();
                    executionTimedOut = true;
                    _engine.TerminateExecution();
                };
                timer.Start();
            }
            object res = null;
            try
            {
                watch2.Start();

                int ver = getVersion();
                JsValue v = jscontext_execute(_context, code, name ?? "<Unnamed Script>");

                watch2.Stop();
                res = _convert.FromJsValue(v);
#if DEBUG_TRACE_API
        	Console.WriteLine("Cleaning up return value from execution");
#endif
                jsvalue_dispose(v);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (executionTimeout.HasValue)
                {
                    timer.Dispose();
                }
            }

            if (executionTimedOut)
            {
                throw new JsExecutionTimedOutException();
            }

            Exception e = res as JsException;
            if (e != null)
                throw e;
            watch1.Stop();

            // Console.WriteLine("Execution time " + watch2.ElapsedTicks + " total time " + watch1.ElapsedTicks);
            return res;
        }

        public object GetGlobal()
        {
            CheckDisposed();
            JsValue v = jscontext_get_global(_context);
            object res = _convert.FromJsValue(v);
            jsvalue_dispose(v);

            Exception e = res as JsException;
            if (e != null)
                throw e;
            return res;
        }

        public object GetVariable(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue v = jscontext_get_variable(_context, name);
            object res = _convert.FromJsValue(v);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value get variable.");
#endif
            jsvalue_dispose(v);

            Exception e = res as JsException;
            if (e != null)
                throw e;
            return res;
        }



        public void SetFunction(string name, Delegate func)
        {
            WeakDelegate del;
            if (func.Target != null)
            {
                del = new BoundWeakDelegate(func.Target, func.Method.Name);
            }
            else
            {
                del = new BoundWeakDelegate(func.Method.DeclaringType, func.Method.Name);
            }
            this.SetVariableFromAny(name, del);
        }

        public void Flush()
        {
            jscontext_force_gc();
        }

        #region Keep-alive management and callbacks.

        internal int KeepAliveAdd(object obj)
        {
            return _keepalives.Add(obj);
        }

        internal object KeepAliveGet(int slot)
        {
            return _keepalives.Get(slot);
        }

        internal void KeepAliveRemove(int slot)
        {
            _keepalives.Remove(slot);
        }

        #endregion

        #region IDisposable implementation

        private readonly Action<int> _notifyDispose;
        bool _disposed;

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            CheckDisposed();

            _disposed = true;

            jscontext_dispose(_context);

            if (disposing)
            {
                _keepalives.Clear();
            }

            _notifyDispose(_id);
        }

        void CheckDisposed()
        {
            if (_engine.IsDisposed)
            {
                throw new ObjectDisposedException("JsContext: engine has been disposed");
            }
            if (_disposed)
                throw new ObjectDisposedException("JsContext:" + _context.Handle);
        }

        ~JsContext()
        {
            if (!_engine.IsDisposed && !_disposed)
                Dispose(false);
        }

        #endregion

        internal bool TrySetMemberValue(Type type, object obj, string name, JsValue value)
        {
            // dictionaries.
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dictionary = (IDictionary)obj;
                dictionary[name] = _convert.FromJsValue(value);
                return true;
            }

            BindingFlags flags;
            if (type == obj)
            {
                flags = BindingFlags.Public | BindingFlags.Static;
            }
            else
            {
                flags = BindingFlags.Public | BindingFlags.Instance;
            }

            PropertyInfo pi = type.GetProperty(name, flags | BindingFlags.SetProperty);
            if (pi != null)
            {
                pi.SetValue(obj, _convert.FromJsValue(value), null);
                return true;
            }

            return false;
        }

        internal JsValue KeepAliveSetPropertyValue(int slot, string name, JsValue value)
        {
#if DEBUG_TRACE_API
			Console.WriteLine("setting prop " + name);
#endif
            // TODO: This is pretty slow: use a cache of generated code to make it faster.

            var obj = KeepAliveGet(slot);
            if (obj != null)
            {
                Type type;
                if (obj is Type)
                {
                    type = (Type)obj;
                }
                else
                {
                    type = obj.GetType();
                }
#if DEBUG_TRACE_API
				Console.WriteLine("setting prop " + name + " type " + type);
#endif
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        var upperCamelCase = Char.ToUpper(name[0]) + name.Substring(1);
                        if (TrySetMemberValue(type, obj, upperCamelCase, value))
                        {
                            return JsValue.Null;
                        }
                        if (TrySetMemberValue(type, obj, name, value))
                        {
                            return JsValue.Null;
                        }
                    }

                    return JsValue.Error(KeepAliveAdd(
                        new InvalidOperationException(String.Format("property not found on {0}: {1} ", type, name))));
                }
                catch (Exception e)
                {
                    return JsValue.Error(KeepAliveAdd(e));
                }
            }

            return JsValue.Error(KeepAliveAdd(new IndexOutOfRangeException("invalid keepalive slot: " + slot)));
        }

        internal bool TryGetMemberValue(Type type, object obj, string name, out JsValue value)
        {
            object result;

            // dictionaries.
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dictionary = (IDictionary)obj;
                if (dictionary.Contains(name))
                {
                    result = dictionary[name];
                    value = _convert.AnyToJsValue(result);
                }
                else
                {
                    value = JsValue.Null;
                }
                return true;
            }

            BindingFlags flags;
            if (type == obj)
            {
                flags = BindingFlags.Public | BindingFlags.Static;
            }
            else
            {
                flags = BindingFlags.Public | BindingFlags.Instance;
            }

            // First of all try with a public property (the most common case).
            PropertyInfo pi = type.GetProperty(name, flags | BindingFlags.GetProperty);
            if (pi != null)
            {
                result = pi.GetValue(obj, null);
                value = _convert.AnyToJsValue(result);
                return true;
            }

            // try field.
            FieldInfo fi = type.GetField(name, flags | BindingFlags.GetProperty);
            if (fi != null)
            {
                result = fi.GetValue(obj);
                value = _convert.AnyToJsValue(result);
                return true;
            }

            // Then with an instance method: the problem is that we don't have a list of
            // parameter types so we just check if any method with the given name exists
            // and then keep alive a "weak delegate", i.e., just a name and the target.
            // The real method will be resolved during the invokation itself.
            BindingFlags mFlags = flags | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy;

            // TODO: This is probably slooow.
            foreach (var met in type.GetMembers(flags))
            {
                if (met.Name == name)
                {
                    if (type == obj)
                    {
                        result = new WeakDelegate(type, name);
                    }
                    else
                    {
                        result = new WeakDelegate(obj, name);
                    }
                    value = _convert.AnyToJsValue(result);
                    return true;
                }
            }
            //if (type.GetMethods(mFlags).Any(x => x.Name == name))
            //{
            //    if (type == obj)
            //    {
            //        result = new WeakDelegate(type, name);
            //    }
            //    else
            //    {
            //        result = new WeakDelegate(obj, name);
            //    }
            //    value = _convert.ToJsValue(result);
            //    return true;
            //}

            value = JsValue.Null;
            return false;
        }

        internal JsValue KeepAliveGetPropertyValue(int slot, string name)
        {
#if DEBUG_TRACE_API
			Console.WriteLine("getting prop " + name);
#endif
            // we need to fall back to the prototype verison we set up because v8 won't call an object as a function, it needs
            // to be from a proper FunctionTemplate.
            if (!string.IsNullOrEmpty(name) && name.Equals("valueOf", StringComparison.InvariantCultureIgnoreCase))
            {
                return JsValue.Empty;
            }

            // TODO: This is pretty slow: use a cache of generated code to make it faster.
            var obj = KeepAliveGet(slot);
            if (obj != null)
            {
                Type type;
                if (obj is Type)
                {
                    type = (Type)obj;
                }
                else
                {
                    type = obj.GetType();
                }
#if DEBUG_TRACE_API
				Console.WriteLine("getting prop " + name + " type " + type);
#endif
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        var upperCamelCase = Char.ToUpper(name[0]) + name.Substring(1);
                        JsValue value;
                        if (TryGetMemberValue(type, obj, upperCamelCase, out value))
                        {
                            return value;
                        }
                        if (TryGetMemberValue(type, obj, name, out value))
                        {
                            return value;
                        }
                    }

                    // Else an error.
                    return JsValue.Error(KeepAliveAdd(
                        new InvalidOperationException(String.Format("property not found on {0}: {1} ", type, name))));
                }
                catch (TargetInvocationException e)
                {
                    // Client code probably isn't interested in the exception part related to
                    // reflection, so we unwrap it and pass to V8 only the real exception thrown.
                    if (e.InnerException != null)
                        return JsValue.Error(KeepAliveAdd(e.InnerException));
                    throw;
                }
                catch (Exception e)
                {
                    return JsValue.Error(KeepAliveAdd(e));
                }
            }

            return JsValue.Error(KeepAliveAdd(new IndexOutOfRangeException("invalid keepalive slot: " + slot)));
        }

        internal JsValue KeepAliveValueOf(int slot)
        {
            var obj = KeepAliveGet(slot);
            if (obj != null)
            {
                Type type = obj.GetType();
                MethodInfo mi = type.GetMethod("valueOf") ?? type.GetMethod("ValueOf");
                if (mi != null)
                {
                    object result = mi.Invoke(obj, new object[0]);
                    return _convert.AnyToJsValue(result);
                }
                return _convert.AnyToJsValue(obj);
            }
            return JsValue.Error(KeepAliveAdd(new IndexOutOfRangeException("invalid keepalive slot: " + slot)));
        }

        internal JsValue KeepAliveInvoke(int slot, JsValue args)
        {
            // TODO: This is pretty slow: use a cache of generated code to make it faster.
#if DEBUG_TRACE_API
			Console.WriteLine("invoking");
#endif
            //   Console.WriteLine(args);

            var obj = KeepAliveGet(slot);
            if (obj != null)
            {
                Type constructorType = obj as Type;
                if (constructorType != null)
                {
#if DEBUG_TRACE_API
					Console.WriteLine("constructing " + constructorType.Name);
#endif
                    object[] constructorArgs = (object[])_convert.FromJsValue(args);
                    return _convert.AnyToJsValue(Activator.CreateInstance(constructorType, constructorArgs));
                }

                WeakDelegate func = obj as WeakDelegate;
                if (func == null)
                {
                    throw new Exception("not a function.");
                }

                Type type = func.Target != null ? func.Target.GetType() : func.Type;
#if DEBUG_TRACE_API
				Console.WriteLine("invoking " + obj.Target + " method " + obj.MethodName);
#endif
                object[] a = (object[])_convert.FromJsValue(args);

                BindingFlags flags = BindingFlags.Public
                        | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy;

                if (func.Target != null)
                {
                    flags |= BindingFlags.Instance;
                }
                else
                {
                    flags |= BindingFlags.Static;
                }

                if (obj is BoundWeakDelegate)
                {
                    flags |= BindingFlags.NonPublic;
                }

                // need to convert methods from JsFunction's into delegates?
                foreach (var a_elem in a)
                {
                    if (a.GetType() == typeof(JsFunction))
                    {
                        CheckAndResolveJsFunctions(type, func.MethodName, flags, a);
                        break;
                    }
                }
                //if (a.Any(z => z != null && z.GetType() == typeof(JsFunction)))
                //{
                //    CheckAndResolveJsFunctions(type, func.MethodName, flags, a);
                //}

                try
                {
                    object result = type.InvokeMember(func.MethodName, flags, null, func.Target, a);
                    return _convert.AnyToJsValue(result);
                }
                catch (TargetInvocationException e)
                {
                    return JsValue.Error(KeepAliveAdd(e.InnerException));
                }
                catch (Exception e)
                {
                    return JsValue.Error(KeepAliveAdd(e));
                }
            }

            return JsValue.Error(KeepAliveAdd(new IndexOutOfRangeException("invalid keepalive slot: " + slot)));
        }

        private static void CheckAndResolveJsFunctions(Type type, string methodName, BindingFlags flags, object[] args)
        {
            MethodInfo mi = type.GetMethod(methodName, flags);
            ParameterInfo[] paramTypes = mi.GetParameters();

            for (int i = Math.Min(paramTypes.Length, args.Length) - 1; i >= 0; --i)
            {
                if (args[i] != null && args[i].GetType() == typeof(JsFunction))
                {
                    JsFunction function = (JsFunction)args[i];
                    args[i] = function.MakeDelegate(paramTypes[i].ParameterType, args);
                }
            }

        }

        internal JsValue KeepAliveDeleteProperty(int slot, string name)
        {
#if DEBUG_TRACE_API
			Console.WriteLine("deleting prop " + name);
#endif
            // TODO: This is pretty slow: use a cache of generated code to make it faster.
            var obj = KeepAliveGet(slot);
            if (obj != null)
            {
#if DEBUG_TRACE_API
				Console.WriteLine("deleting prop " + name + " type " + type);
#endif
                if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
                {
                    IDictionary dictionary = (IDictionary)obj;
                    if (dictionary.Contains(name))
                    {
                        dictionary.Remove(name);
                        return _convert.ToJsValue(true);
                    }
                }
                return _convert.ToJsValue(false);
            }
            return JsValue.Error(KeepAliveAdd(new IndexOutOfRangeException("invalid keepalive slot: " + slot)));
        }

        internal JsValue KeepAliveEnumerateProperties(int slot)
        {
#if DEBUG_TRACE_API
			Console.WriteLine("deleting prop " + name);
#endif
            // TODO: This is pretty slow: use a cache of generated code to make it faster.
            var obj = KeepAliveGet(slot);
            if (obj != null)
            {
#if DEBUG_TRACE_API
				Console.WriteLine("deleting prop " + name + " type " + type);
#endif

                if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
                {
                    IDictionary dictionary = (IDictionary)obj;
                    //string[] keys = dictionary.Keys.Cast<string>().ToArray();

                    var keys01 = new System.Collections.Generic.List<string>();
                    foreach (var k in dictionary.Keys)
                    {
                        keys01.Add(k.ToString());
                    }

                    return _convert.ToJsValue(keys01.ToArray());
                }

                var mbNameList = new System.Collections.Generic.List<string>();
                foreach (var mb in obj.GetType().GetMembers(BindingFlags.Public |
                    BindingFlags.Instance))
                {
                    var met = mb as MethodBase;
                    if (met != null && !met.IsSpecialName)
                    {
                        mbNameList.Add(mb.Name);
                    }
                }
                //string[] values = obj.GetType().GetMembers(
                //    BindingFlags.Public |
                //    BindingFlags.Instance).Where(m =>
                //    {
                //        var method = m as MethodBase;
                //        return method == null || !method.IsSpecialName;
                //    }).Select(z => z.Name).ToArray();
                return _convert.ToJsValue(mbNameList.ToArray());
            }
            return JsValue.Error(KeepAliveAdd(new IndexOutOfRangeException("invalid keepalive slot: " + slot)));
        }

        public object Invoke(IntPtr funcPtr, IntPtr thisPtr, object[] args)
        {
            CheckDisposed();

            if (funcPtr == IntPtr.Zero)
                throw new JsInteropException("wrapped V8 function is empty (IntPtr is Zero)");

            JsValue a = JsValue.Null; // Null value unless we're given args.
            if (args != null)
            {
                a = _convert.AnyToJsValue(args);
            }


            JsValue v = jscontext_invoke(_context, funcPtr, thisPtr, a);
            object res = _convert.FromJsValue(v);
            jsvalue_dispose(v);
            jsvalue_dispose(a);

            Exception e = res as JsException;
            if (e != null)
                throw e;
            return res;
        }

        public INativeScriptable CreateWrapper(object o, JsTypeDefinition jsTypeDefinition)
        {
            return proxyStore.CreateProxyForObject(o, jsTypeDefinition);
        }
        public void RegisterTypeDefinition(JsTypeDefinition jsTypeDefinition)
        {
            proxyStore.CreateProxyForTypeDefinition(jsTypeDefinition);
        }

        public void SetVariableFromAny(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.AnyToJsValue(value);
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
            // TODO: Check the result of the operation for errors.
        }

        public void SetVariable(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.ToJsValue(value);
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
        }
        public void SetVariable(string name, int value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.ToJsValue(value);
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
        }
        public void SetVariable(string name, double value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.ToJsValue(value);
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
        }
        public void SetVariable(string name, long value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.ToJsValue(value);
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
        }
        public void SetVariable(string name, DateTime value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.ToJsValue(value);
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
        }
        public void SetVariable(string name, INativeScriptable proxy)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.ToJsValue(proxy);
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
            // TODO: Check the result of the operation for errors.
        }
        public void SetVariableNull(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            CheckDisposed();

            JsValue a = _convert.ToJsValueNull();
            JsValue b = jscontext_set_variable(_context, name, a);
#if DEBUG_TRACE_API
			Console.WriteLine("Cleaning up return value from set variable");
#endif
            jsvalue_dispose(a);
            jsvalue_dispose(b);
        }

        public void SetVariableAutoWrap<T>(string name, T result)
             where T : class
        {
            var jsTypeDef = this.GetJsTypeDefinition<T>(result);
            var proxy = this.CreateWrapper(result, jsTypeDef);
            this.SetVariable(name, proxy);
        }
        //------------------------------------------------------------------

        public JsTypeDefinition GetJsTypeDefinition<T>(T t)
            where T : class
        {
            Type type = typeof(T);
            JsTypeDefinition found;
            if (this.mappingJsTypeDefinition.TryGetValue(type, out found))
                return found;

            //if not found
            //just create it
            found = this.jsTypeDefBuilder.BuildTypeDefinition(type);
            this.mappingJsTypeDefinition.Add(type, found);
            this.RegisterTypeDefinition(found);

            return found;
        }


        public JsTypeDefinition GetJsTypeDefinition2(Type type)
        {

            JsTypeDefinition found;
            if (this.mappingJsTypeDefinition.TryGetValue(type, out found))
                return found;

            //if not found
            //just create it
            found = this.jsTypeDefBuilder.BuildTypeDefinition(type);
            this.mappingJsTypeDefinition.Add(type, found);
            this.RegisterTypeDefinition(found);

            return found;
        }
    }
}
