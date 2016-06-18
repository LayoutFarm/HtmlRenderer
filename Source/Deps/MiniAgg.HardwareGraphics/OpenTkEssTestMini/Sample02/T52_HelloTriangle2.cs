
#region Using Directives

using System;
using OpenTK.Graphics.ES20;
using Mini;
#endregion


namespace OpenTkEssTest
{
    [Info(OrderCode = "052")]
    [Info("T52_HelloTriangle2")]
    public class T52_HelloTriangle2 : PrebuiltGLControlDemoBase
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //----------------
            //vertex shader source
            string vs = @"      
             
            attribute vec2 a_position;
            attribute vec4 a_color;
            
            varying vec4 v_color;
 
            void main()
            {   
                
                gl_Position = vec4(a_position[0],a_position[1],0,1);  
                v_color = a_color;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color;                 
                void main()
                { 
                    gl_FragColor = v_color;
                }
            ";
            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }


            a_position = shaderProgram.GetVtxAttrib("a_position");
            a_color = shaderProgram.GetVtxAttrib("a_color");
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.ClearColor(0, 0, 0, 0);
            GL.ClearColor(1, 1, 1, 1);
        }
        protected override void DemoClosing()
        {
            shaderProgram.DeleteMe();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //------------------------------------------------------------------------------------------------
            int width = miniGLControl.Width;
            int height = miniGLControl.Height;
            float[] vertices =
            {
                     0.0f,  0.5f, //2d coord
                     1, 0, 0, 0.1f,//r
                    -0.5f, -0.5f,  //2d coord
                     0,1,0,0.1f,//g
                     0.5f, -0.5f,  //2d corrd
                     0,0,1,0.1f, //b
            };
            GL.Viewport(0, 0, width, height);
            // Set the viewport 
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shaderProgram.UseProgram();
            // Load the vertex data 
            a_position.LoadV2f(vertices, 6, 0);
            a_color.LoadV3f(vertices, 6, 2);
            //GL.DrawArrays(BeginMode.Triangles, 0, 3);
            GL.DrawArrays(BeginMode.Points, 0, 3);
            miniGLControl.SwapBuffers();
        }
        //-------------------------------
        ShaderVtxAttrib a_position;
        ShaderVtxAttrib a_color;
    }
}
