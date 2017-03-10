//BSD, 2014-2017, WinterDev 

using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing.WinGdi
{
    public static class WinGdiPlusPlatform
    {

        static WinGdiPlusPlatform()
        {

            PixelFarm.Agg.AggBuffMx.SetNaiveBufferImpl(new Win32AggBuffMx());
            //3. set default encoing
            WinGdiTextService.SetDefaultEncoding(System.Text.Encoding.ASCII);
        }

        public static void SetFontEncoding(System.Text.Encoding encoding)
        {
            WinGdiTextService.SetDefaultEncoding(encoding);
        }
        public static void SetFontLoader(IFontLoader fontLoader)
        {
            WinGdiFontFace.SetFontLoader(fontLoader);
        }
    }



    class Win32AggBuffMx : PixelFarm.Agg.AggBuffMx
    {

        protected override void InnerMemCopy(byte[] dest_buffer, int dest_startAt, byte[] src_buffer, int src_StartAt, int len)
        {
            unsafe
            {
                fixed (byte* head_dest = &dest_buffer[dest_startAt])
                fixed (byte* head_src = &src_buffer[src_StartAt])
                {
                    Win32.MyWin32.memcpy(head_dest, head_src, len);
                }
            }
        }
        protected override void InnerMemSet(byte[] dest, int startAt, byte value, int count)
        {
            unsafe
            {
                fixed (byte* head = &dest[startAt])
                {
                    Win32.MyWin32.memset(head, value, count);
                }
            }
        }

    }



}