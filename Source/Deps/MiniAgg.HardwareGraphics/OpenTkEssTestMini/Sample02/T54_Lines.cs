
#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using Examples.Tutorial;
using Mini;

#endregion


namespace OpenTkEssTest
{
    [Info(OrderCode = "054")]
    [Info("T54_Viewport")]
    public class T54_Lines : PrebuiltGLControlDemoBase
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib a_position;
        ShaderVtxAttrib a_color;

        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 u_useSolidColor;
        ShaderUniformVar4 u_solidColor;

        MyMat4 orthoView;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {

            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position;
            attribute vec4 a_color; 

            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
            uniform int u_useSolidColor;              

            varying vec4 v_color;
 
            void main()
            {
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                if(u_useSolidColor !=0)
                {
                    v_color= u_solidColor;
                }
                else
                {
                    v_color = a_color;
                }
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

            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");

            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int max = Math.Max(this.Width, this.Height);
            //square viewport
            GL.Viewport(0, 0, max, max);
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            //--------------------------------------------------------------------------------

            //load image


        }
        protected override void DemoClosing()
        {
            shaderProgram.DeleteMe();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //------------------------------------------------------------------------------------------------
             
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shaderProgram.UseProgram();
            //---------------------------------------------------------  
            u_matrix.SetData(orthoView.data); 
            //---------------------------------------------------------  
            
            DrawLines(0, 0, 300, 300);
            //---------------------------------------------------------

            miniGLControl.SwapBuffers();
        }

        //-------------------------------
        void FillPolygonWithSolidColor(float[] onlyCoords,
               int numVertices, PixelFarm.Drawing.Color c)
        {

            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)c.R / 255f, (float)c.G / 255f, (float)c.B / 255f, (float)c.A / 255f);//use solid color 
            a_position.LoadV2f(onlyCoords, 2, 0);
            GL.DrawArrays(BeginMode.Triangles, 0, numVertices);

        }
        void FillPolygonWithSolidColor(float[] onlyCoords,
               int numVertices, float r, float g, float b, float a)
        {

            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue(r, g, b, a);//use solid color 
            a_position.LoadV2f(onlyCoords, 2, 0);
            GL.DrawArrays(BeginMode.Triangles, 0, numVertices);

        }
        void FillPolygonWithVertexColor(float[] vertices, int numVertices)
        {
            //x,y,r,g,b,a 
            u_useSolidColor.SetValue(0);
            a_position.LoadV2f(vertices, 6, 0);
            a_color.LoadV4f(vertices, 6, 2);
            GL.DrawArrays(BeginMode.Triangles, 0, numVertices);
        }

        static float[] CreateRectCoords(float x, float y, float w, float h)
        {

            float[] vertices = new float[]{
                x, y,  
                x+w,y, 
                x+w,y-h, 
                x+w,y-h, 
                x, y - h,  
                x, y
            };
            return vertices;
        }
        //------------------------------- 
        void DrawLines(float x1, float y1, float x2, float y2)
        {
            float[] vtxs = new float[] { x1, y1, x2, y2 };
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue(0f, 0f, 0f, 1f);//use solid color 
            a_position.LoadV2f(vtxs, 2, 0);
            GL.DrawArrays(BeginMode.Lines, 0, 2);
        }
        void DrawImage(float x, float y)
        {
            
        }

    }


}
