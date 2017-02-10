//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.RenderBoxes
{
    public struct RenderElementRequest
    {
        public RenderElement ve;
        public RequestCommand req;
        public object parameters;
        public RenderElementRequest(RenderElement ve, RequestCommand req)
        {
            this.ve = ve;
            this.req = req;
            this.parameters = null;
        }
        public RenderElementRequest(RenderElement ve, RequestCommand req, object parameters)
        {
            this.ve = ve;
            this.req = req;
            this.parameters = parameters;
        }
    }
    public enum RequestCommand
    {
        DoFocus,
        AddToWindowRoot,
        InvalidateArea,
        NotifySizeChanged,
    }
}