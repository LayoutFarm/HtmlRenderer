//2013 MIT, Federico Di Gregorio <fog@initd.org>
using System;
using System.Collections.Generic;
using System.Reflection;

namespace VroomJs
{
  

    public class JsFunction : IDisposable
    {
        readonly JsContext _context;
        readonly IntPtr _funcPtr;
        readonly IntPtr _thisPtr;

        static MethodInfo myInvokeMethodInfo;

        static JsFunction()
        {
            myInvokeMethodInfo = typeof(JsFunction).GetMethod("Invoke");

        }
        public JsFunction(JsContext context, IntPtr funcPtr, IntPtr thisPtr)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (funcPtr == IntPtr.Zero)
                throw new ArgumentException("can't wrap an empty function (ptr is Zero)", "ptr");

            _context = context;
            _funcPtr = funcPtr;
            _thisPtr = thisPtr; 
        } 
        public object Invoke(params object[] args)
        {
            object result = _context.Invoke(_funcPtr, _thisPtr, args);
            return result;
        } 
        public object MakeDelegate(Type targetDelegateType)
        {   
            if (targetDelegateType.BaseType != typeof(MulticastDelegate))
            {
                throw new ApplicationException("Not a delegate.");
            }
            return Delegate.CreateDelegate(targetDelegateType, this, myInvokeMethodInfo); 
        }
        //public object MakeDelegate(Type type, object[] args)
        //{ 
        //    return new SimpleDelegate(this.Invoke);
        //    // return null;

        //    //if (type.BaseType != typeof(MulticastDelegate))
        //    //{
        //    //    throw new ApplicationException("Not a delegate.");
        //    //}

        //    //MethodInfo invoke = type.GetMethod("Invoke");
        //    //if (invoke == null)
        //    //{
        //    //    throw new ApplicationException("Not a delegate.");
        //    //}

        //    //ParameterInfo[] invokeParams = invoke.GetParameters();
        //    //Type returnType = invoke.ReturnType;

        //    //List<ParameterExpression> parameters = new List<ParameterExpression>();
        //    //List<Expression> arrayInitExpressions = new List<Expression>();

        //    //for (int i = 0; i < invokeParams.Length; i++)
        //    //{
        //    //    ParameterExpression param = Expression.Parameter(invokeParams[i].ParameterType, invokeParams[i].Name);
        //    //    parameters.Add(param);
        //    //    arrayInitExpressions.Add(Expression.Convert(param, typeof(object)));
        //    //}

        //    //Expression array = Expression.NewArrayInit(typeof(object), arrayInitExpressions);

        //    //Expression me = Expression.Constant(this);
        //    //MethodInfo myInvoke = GetType().GetMethod("Invoke");
        //    //Expression callExpression = Expression.Call(me, myInvoke, array);

        //    //if (returnType != typeof(void))
        //    //{
        //    //    callExpression = Expression.Convert(callExpression, returnType);
        //    //}

        //    //return Expression.Lambda(type, callExpression, parameters).Compile();
        //}

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
                throw new ObjectDisposedException("JsObject:" + _funcPtr);

            _disposed = true;

            _context.Engine.DisposeObject(this._funcPtr);
            if (_thisPtr != IntPtr.Zero)
            {
                _context.Engine.DisposeObject(this._thisPtr);
            }
        }

        ~JsFunction()
        {
            if (!_disposed)
                Dispose(false);
        }

        #endregion
    }
}
