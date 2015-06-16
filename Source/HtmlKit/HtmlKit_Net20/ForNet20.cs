#define NET20
#if NET20
namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}
namespace System.Text
{
    public static class StringBuilderExtension
    {
        public static void Clear(this StringBuilder stbuilder)
        {
            stbuilder.Length = 0;
        }
    }
}
#endif