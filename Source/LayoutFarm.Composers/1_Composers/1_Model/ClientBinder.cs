// 2015,2014 ,BSD, WinterDev  

namespace LayoutFarm
{
    public class ClientImageBinder : ImageBinder
    {
        UI.IEventListener listener;
        public ClientImageBinder(string src)
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