using Android.App;
using Android.Widget;
using Android.OS;
using Test_Android_Glyph;
using Android.Content.PM;
using Android.Content.Res;
using System.IO;


namespace TestApp01.Droid
{
    [Activity(Label = "TestApp01", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        GLView1 view;
        private static AssetManager s_assetManager;
        public static AssetManager AssetManager => s_assetManager;

        static void LoadFonts(Typography.FontManagement.InstalledTypefaceCollection fontCollection)
        {
            LoadBundleFont(fontCollection, "DroidSans.ttf");
            LoadBundleFont(fontCollection, "tahoma.ttf");
            LoadBundleFont(fontCollection, "SOV_Thanamas.ttf");
        }
        static Stream LoadFont(string fontfile)
        {
            using (Stream s = MainActivity.AssetManager.Open(fontfile))
            {
                // This is a simple hack because on Xamarin.Android, a `Stream` created by `AssetManager.Open` is not seekable.
                var ms = new MemoryStream();
                s.CopyTo(ms);
                ms.Position = 0;
                return ms;
            }
        }
        static void LoadBundleFont(Typography.FontManagement.InstalledTypefaceCollection fontCollection, string fontfile)
        {
            // This is a simple hack because on Xamarin.Android, a `Stream` created by `AssetManager.Open` is not seekable.
            
            using (Stream s = MainActivity.AssetManager.Open(fontfile)) 
            {
                var ms = new MemoryStream();
                s.CopyTo(ms);
                ms.Position = 0;
                fontCollection.AddFontStreamSource(new BundleResourceFontStreamSource(ms, fontfile));
            }
        }

        class BundleResourceFontStreamSource : Typography.FontManagement.IFontStreamSource
        {
            MemoryStream _ms;
            string _pathName;
            public BundleResourceFontStreamSource(MemoryStream ms, string pathName)
            {
                _ms = ms;
                _pathName = pathName;
            }
            public string PathName => _pathName;
            public Stream ReadFontStream()
            {
                return _ms;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            s_assetManager = this.Assets;

            Typography.FontManagement.InstalledTypefaceCollectionExtensions.CustomSystemFontListLoader = LoadFonts;
            Typography.FontManagement.InstalledTypefaceCollectionExtensions.CustomFontStreamLoader = LoadFont;

            view = new GLView1(this);
            SetContentView(view);
        }
        protected override void OnPause()
        {
            base.OnPause();
            view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            view.Resume();
        }
    }
}

