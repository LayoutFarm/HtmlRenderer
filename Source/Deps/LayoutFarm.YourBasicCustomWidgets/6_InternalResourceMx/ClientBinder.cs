//BSD, 2014-2017, WinterDev

namespace LayoutFarm
{
    class MyClientImageBinder : ImageBinder
    {
        UI.IEventListener listener;
        public MyClientImageBinder(string src)
            : base(src)
        {
        }
        protected override void OnImageChanged()
        {
            if (listener != null)
            {
                listener.HandleContentUpdate();
            }
        }
        public void SetOwner(UI.IEventListener listener)
        {
            this.listener = listener;
        }
    }
}