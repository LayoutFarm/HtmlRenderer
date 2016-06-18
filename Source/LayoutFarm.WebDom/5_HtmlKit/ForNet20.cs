#define NET20
#if NET20 
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