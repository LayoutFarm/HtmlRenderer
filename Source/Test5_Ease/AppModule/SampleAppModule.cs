//MIT, 2015-2016, WinterDev

using LayoutFarm.Scripting;
using Espresso;

namespace Test5_Ease
{
    //just a sample only***
    //
    abstract class SampleAppModule
    {
        JsEngine myengine;
        //4. root js context
        JsContext myCtx;
        public SampleAppModule()
        {
        }
        public virtual string GetInitPage()
        {
            return null;
        }
        public virtual string RootDir
        {
            get { return ""; }
        }
        public LayoutFarm.WebDom.IHtmlDocument HtmlDoc
        {
            get;
            set;
        }
        internal MyWebConsole Console
        {
            get;
            set;
        } 
        internal void InitJsEngine()
        {
            if (myengine == null)
            {
                var jstypeBuilder = new LayoutFarm.Scripting.MyJsTypeDefinitionBuilder();
                myengine = new JsEngine();
                myCtx = myengine.CreateContext(jstypeBuilder);
            }
            myCtx.SetVariableAutoWrap("document", HtmlDoc);
            myCtx.SetVariableAutoWrap("console", Console);
        }
        public virtual object ExecuteJs(string js)
        {
            object testResult = myCtx.Execute(js);
            return testResult;
        }
    }
    class MyTestHtmlAppModule : SampleAppModule
    {
        public override string GetInitPage()
        {
            return "<html><script>function doc_ready(){console.log('doc_ready');}</script><body onload=\"doc_ready()\"><div id=\"a\">A</div><div id=\"b\" style=\"background-color:yellow\">B</div><div id=\"c\">c_node</div></body></html>";
        }
    }

}