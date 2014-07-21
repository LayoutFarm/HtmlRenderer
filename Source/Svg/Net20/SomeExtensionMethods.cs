//using System.Collections.Generic;

//namespace System
//{
//    public static class SomeExtensionMethods
//    {
//        public static T[] Concat<T>(this T[] a1, T[] a2)
//        {
//            return null;
//        }
//        public static T First<T>(this T[] a)
//        {
//            return default(T);
//        }
//        public static T[] ToArray<T>(this T[] a)
//        {
//            return null;
//        }
//        public static bool Contains<T>(this T[] a, T data)
//        {
//            return false;
//        }
//        public static IEnumerable<T> Select<T>(this T[] a, Func<T, bool> f)
//        {
//            return null;
//        }
//        public static T[] Join<T>(T[] a1, T[] a2)
//        {
//            return null;
//        }
//        public static bool IsNullOrWhiteSpace(this string s)
//        {
//            return string.IsNullOrEmpty(s);
//        }
//        public static void Clear(System.Text.StringBuilder stbuilder)
//        {
//            stbuilder.Length = 0;
//        }
//        public static T FirstOrDefault<T>(this  IList<T> list)
//        {
//            if (list == null)
//            {
//                return default(T);
//            }
//            else if (list.Count == 0)
//            {
//                return default(T);
//            }
//            return list[0];
//        }
//    }
//}