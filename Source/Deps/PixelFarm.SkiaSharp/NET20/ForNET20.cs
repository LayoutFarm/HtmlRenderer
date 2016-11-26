#if NET20
namespace System.Runtime.CompilerServices
{
    public partial class ExtensionAttribute : Attribute { }
}
namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this String str)
        {
            return str == null || str == " ";
        }
    }

    public class WeakReference<T> : WeakReference
        where T : class
    {
        public WeakReference(T obj)
            : base(obj)
        {

        }
        public bool TryGetTarget(out T target)
        {
            object savedTarget = this.Target;
            if (savedTarget != null)
            {
                target = (T)savedTarget;
                return true;
            }
            else
            {
                target = null;
                return false;
            }
        }
    }
}
#endif