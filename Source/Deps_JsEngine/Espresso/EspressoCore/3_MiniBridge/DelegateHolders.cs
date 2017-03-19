//MIT, 2015-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Espresso
{
    //create more if you want

    class DelegateTemplate
    {
        public readonly Type delHolderType;
        public readonly DelegateHolder holder;

        public DelegateTemplate(Type delHolderType, DelegateHolder holder)
        {
            this.delHolderType = delHolderType;
            this.holder = holder;
        }

        public Delegate CreateNewDelegate(Type targetDelegateType, JsFunction jsfunc)
        {
            DelegateHolder newHolder = this.holder.New();
            newHolder.jsFunc = jsfunc;
#if NET20
            return Delegate.CreateDelegate(targetDelegateType,
                newHolder,
                this.holder.InvokeMethodInfo);
#else
            return this.holder.InvokeMethodInfo.CreateDelegate(targetDelegateType, newHolder);
#endif

        }
    }

    static class Helper
    {
        public static MethodInfo GetInvokeMethod<T>()
        {
#if NET20
            return typeof(T).GetMethod("Invoke");
#else
      return typeof(T).GetRuntimeMethod("Invoke", null);//.GetMethod("Invoke");
#endif
        }
    }

    abstract class DelegateHolder
    {


        public JsFunction jsFunc;
        /// <summary>
        /// create new black delegate holder with the same kind
        /// </summary>
        /// <returns></returns>
        public abstract DelegateHolder New();
        public abstract MethodInfo InvokeMethodInfo { get; }

    }

    class FuncDelegateHolder<TResult> : DelegateHolder
    {

        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<FuncDelegateHolder<TResult>>();

        public TResult Invoke()
        {
            return (TResult)this.jsFunc.Invoke(new object[0]);
        }
        public override DelegateHolder New()
        {
            return new FuncDelegateHolder<TResult>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }
    class FuncDelegateHolder<T, TResult> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<FuncDelegateHolder<T, TResult>>();

        public TResult Invoke(T arg)
        {
            return (TResult)this.jsFunc.Invoke(arg);
        }
        public override DelegateHolder New()
        {
            return new FuncDelegateHolder<T, TResult>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }
    class FuncDelegateHolder<T1, T2, TResult> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<FuncDelegateHolder<T1, T2, TResult>>();

        public TResult Invoke(T1 arg1, T2 arg2)
        {
            return (TResult)this.jsFunc.Invoke(arg1, arg2);
        }
        public override DelegateHolder New()
        {
            return new FuncDelegateHolder<T1, T2, TResult>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }
    class FuncDelegateHolder<T1, T2, T3, TResult> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<FuncDelegateHolder<T1, T2, T3, TResult>>();
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return (TResult)this.jsFunc.Invoke(arg1, arg2, arg3);
        }
        public override DelegateHolder New()
        {
            return new FuncDelegateHolder<T1, T2, T3, TResult>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }
    class FuncDelegateHolder<T1, T2, T3, T4, TResult> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<FuncDelegateHolder<T1, T2, T3, T4, TResult>>();

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return (TResult)this.jsFunc.Invoke(arg1, arg2, arg3, arg4);
        }
        public override DelegateHolder New()
        {
            return new FuncDelegateHolder<T1, T2, T3, T4, TResult>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }


    class ActionDelegateHolder : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<ActionDelegateHolder>();
        public void Invoke()
        {
            this.jsFunc.Invoke(new object[0]);
        }
        public override DelegateHolder New()
        {
            return new ActionDelegateHolder();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }

    }
    class ActionDelegateHolder<T> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<ActionDelegateHolder<T>>();
        public void Invoke(T arg)
        {
            this.jsFunc.Invoke(arg);
        }
        public override DelegateHolder New()
        {
            return new ActionDelegateHolder<T>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }
    class ActionDelegateHolder<T1, T2> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<ActionDelegateHolder<T1, T2>>();
        public void Invoke(T1 arg1, T2 arg2)
        {
            this.jsFunc.Invoke(arg1, arg2);
        }
        public override DelegateHolder New()
        {
            return new ActionDelegateHolder<T1, T2>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }

    class ActionDelegateHolder<T1, T2, T3> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<ActionDelegateHolder<T1, T2, T3>>();
        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            this.jsFunc.Invoke(arg1, arg2, arg3);
        }
        public override DelegateHolder New()
        {
            return new ActionDelegateHolder<T1, T2, T3>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }
    class ActionDelegateHolder<T1, T2, T3, T4> : DelegateHolder
    {
        static MethodInfo invokeMethodInfo = Helper.GetInvokeMethod<ActionDelegateHolder<T1, T2, T3, T4>>();
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            this.jsFunc.Invoke(arg1, arg2, arg3, arg4);
        }
        public override DelegateHolder New()
        {
            return new ActionDelegateHolder<T1, T2, T3, T4>();
        }
        public override MethodInfo InvokeMethodInfo
        {
            get { return invokeMethodInfo; }
        }
    }


}