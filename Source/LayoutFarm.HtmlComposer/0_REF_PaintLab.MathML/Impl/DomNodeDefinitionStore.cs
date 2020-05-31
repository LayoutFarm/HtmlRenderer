//MIT, 2020, Brezza92
using LayoutFarm.Composers;
using System;
using System.Collections.Generic;
namespace MathLayout
{

    partial class DomNodeDefinitionStore
    {
        delegate MathNode MathNodeCreatorDelegate();

        //another part of this is on autogen side
        public DomNodeDefinitionStore()
        {
            LoadNodeDefinition();
        }
        partial void LoadNodeDefinition();//this is partial method, the implementation is on autogen side

        Dictionary<string, MathNodeCreatorDelegate> _nodeCreatorDic = new Dictionary<string, MathNodeCreatorDelegate>();
        void Register(string nodeName, MathNodeCreatorDelegate creatorFunc)
        {
            _nodeCreatorDic.Add(nodeName, creatorFunc);
        }
        public MathNode CreateMathNode(string nodename)
        {
            if (!_nodeCreatorDic.TryGetValue(nodename, out MathNodeCreatorDelegate creator))
            {
                System.Diagnostics.Debugger.Break();//???
            }
            //if found this
            MathNode newNode = creator();
            return newNode;
        }
    }
}