//BSD, 2014-2017, WinterDev 

using System.Runtime.InteropServices;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing.WinGdi
{
    public class WinGdiPlusPlatform : GraphicsPlatform
    {

        static WinGdiPlusPlatform()
        {
             
            PixelFarm.Agg.AggBuffMx.SetNaiveBufferImpl(new Win32AggBuffMx());
            //3. set default encoing
            WinGdiTextService.SetDefaultEncoding(System.Text.Encoding.ASCII);
        }
        public WinGdiPlusPlatform()
        {
        }

      
        public override Canvas CreateCanvas(int left, int top, int width, int height, CanvasInitParameters canvasInitPars = new CanvasInitParameters())
        {
            return new MyGdiPlusCanvas(0, 0, left, top, width, height);
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
                fixed (byte* head = &dest[0])
                {
                    Win32.MyWin32.memset(head, 0, 100);
                }
            }
        }
        
    }



}