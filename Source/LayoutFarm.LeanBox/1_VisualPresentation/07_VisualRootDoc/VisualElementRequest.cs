//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;




namespace LayoutFarm.Presentation
{

    public struct VisualElementRequest
    {
        public ArtVisualElement ve;
        public RequestCommand req;
        public object parameters;

        public VisualElementRequest(ArtVisualElement ve, RequestCommand req)
        {
            this.ve = ve;
            this.req = req;
            this.parameters = null;
        }
        public VisualElementRequest(ArtVisualElement ve, RequestCommand req, object parameters)
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