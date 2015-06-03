//#define NET20 

namespace System
{
    public class MyNotImplementException : Exception
    {
        //TODO: find this exception in the code 
        //and reimplement it again
    }

}

#if NET20
 
namespace System
{

    public delegate void Action();
    //public delegate void Action<in T>(T obj);
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void Action<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);


    public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T arg);
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult Func<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);


}
namespace System.Runtime.CompilerServices
{

    public class ExtensionAttribute : Attribute { }

}


namespace System
{
    namespace Collections.Generic
    {
        public interface ISet<T> : IEnumerable<T>, ICollection<T>
        {
           
        }
        //public class SortedSet<T>
        //{

        //}
    }

    
    namespace Collections.Generic
    {
        public static class CollectionExtensionMethods
        {
            public static List<T> ToList<T>(this IEnumerable<T> iter)
            {
                List<T> list = new List<T>(iter);
                return list;
            }
            public static T[] ToArray<T>(this IEnumerable<T> iter)
            {
                List<T> list = new List<T>(iter);
                return list.ToArray();
            }
        }
        public class HashSet<T>
        {
            public HashSet()
            {
            }
            public HashSet(IEnumerable<T> iter)
            {

            }
            public void Add(T t)
            {

            }
            public bool Contains(T t)
            {
                throw new MyNotImplementException();
            }
        }

    }

    //namespace Collections.Concurrent
    //{
    //    class dummy { }
    //}
    //namespace Linq
    //{
    //    class dummy { }
    //}
    //namespace Xml
    //{
    //    class dummy { }
    //}
}
namespace CsQuery
{
    //namespace Implementation
    //{
    //    class dummy { }
    //}
    //namespace ExtensionMethods
    //{
    //    class dummy { }
    //}
    //namespace ExtensionMethods.Internal
    //{
    //    class dummy { }
    //}
    //namespace Utility
    //{
    //    class dummy { }
    //}
    //namespace Engine
    //{
    //    class dummy { }
    //}
    //namespace HtmlParser
    //{
    //    class dummy { }
    //}
    //namespace Output
    //{
    //    class dummy { }
    //}
    //namespace StringScanner
    //{
    //    class dummy { }
    //}
    //namespace StringScanner.Implementation
    //{
    //    class dummy { }
    //}
}
#endif