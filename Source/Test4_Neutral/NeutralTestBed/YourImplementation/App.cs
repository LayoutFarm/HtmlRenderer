//Apache2, 2014-present, WinterDev


namespace LayoutFarm
{

    public abstract class App
    {
        internal void StartApp(AppHost host)
        {
            OnStart(host);
        }
        protected virtual void OnStart(AppHost host)
        {
        }
        public virtual void OnClosing()
        {

        }
        public virtual void OnClosed()
        {

        }
        //
        public virtual string Desciption => "";
        //
        protected virtual System.IO.Stream ReadStream(string url)
        {
            if (s_readStreamDelegate != null)
            {
                return s_readStreamDelegate(url);
            }
            return null;
        }

        static System.Func<string, System.IO.Stream> s_readStreamDelegate;
        static System.Func<string, System.IO.Stream> s_writeStreamDelegate;
        static System.Func<string, System.IO.Stream, bool> s_uploadStreamDelegate;

        public static void RegisterUploadStreamDelegate(System.Func<string, System.IO.Stream, bool> uploadStreamDel)
        {
            s_uploadStreamDelegate = uploadStreamDel;
        }
        public static void RegisterReadStreamDelegate(System.Func<string, System.IO.Stream> getReadStreamDel)
        {
            s_readStreamDelegate = getReadStreamDel;
        }
        public static void RegisterGetWriteStreamDelegate(System.Func<string, System.IO.Stream> getWriteStreamDel)
        {
            s_writeStreamDelegate = getWriteStreamDel;
        }
        public static System.IO.Stream ReadStreamS(string url)
        {
            return s_readStreamDelegate(url);
        }
        public static System.IO.Stream GetWriteStream(string url)
        {
            if (s_writeStreamDelegate != null)
            {
                return s_writeStreamDelegate(url);
            }
            return null;
        }
        public static bool UploadStream(string url, System.IO.Stream uploadstream)
        {
            return s_uploadStreamDelegate(url, uploadstream);
        }
    }

}