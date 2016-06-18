
#region Using Directives

using System;
using OpenTK.Graphics.ES20;
using Mini;
#endregion


namespace OpenTkEssTest
{
    [Info(OrderCode = "053")]
    [Info("T53_Viewport")]
    public class T53_Viewport : PrebuiltGLControlDemoBase
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib a_position;
        ShaderVtxAttrib a_color;
        ShaderVtxAttrib a_textureCoord;
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
            attribute vec2 a_texcoord;
            
            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
            uniform int u_useSolidColor;            

            varying vec4 v_color;
            varying vec2 v_texcoord;
             
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
                v_texcoord= a_texcoord;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color; 
                varying vec2 v_texcoord;                 
                void main()
                {       
                    
                    //gl_FragColor = vec4(1,v_texcoord.y,0,1);
                    gl_FragColor= v_color;
                }
            ";
            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }


            a_position = shaderProgram.GetVtxAttrib("a_position");
            a_color = shaderProgram.GetVtxAttrib("a_color");
            a_textureCoord = shaderProgram.GetVtxAttrib("a_texcoord");
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

            float[] vertices =
            {
                 50,  100f, //2d coord
                 1, 0, 0, 0.5f,//r
                 200f, 100f,  //2d coord
                 0,1,0,0.5f,//g
                 150f, 200,  //2d corrd
                 0,0,1,0.5f, //b
            };
            //---------------------------------------------------------


            GL.Clear(ClearBufferMask.ColorBufferBit);
            shaderProgram.UseProgram();
            //---------------------------------------------------------  
            u_matrix.SetData(orthoView.data);
            //---------------------------------------------------------  
            //triangle shape 
            FillPolygonWithVertexColor(vertices, 6);
            //---------------------------------------------------------
            //rect shape 


            //float[] quadVertices3 = CreateRectCoords(250f, 450f, 100f, 100f);
            //FillPolygonWithSolidColor(quadVertices3, 6, PixelFarm.Drawing.Color.Red);

            //float[] quadVertices2 = CreateRectCoords(250f, 450f, 100f, 100f);
            float[] quadVertices2 = CreateRectCoords(260f, 160f, 100f, 100f);
            float[] textureCoords = CreateRectTextureCoords();
            //FillPolygonWithSolidColor2(quadVertices2, textureCoords, 6, PixelFarm.Drawing.Color.Blue);
            FillPolygonWithSolidColor(quadVertices2, 6, PixelFarm.Drawing.Color.Blue);
            //---------------------------------------------------------
            float[] quadVertices3 = CreateRectCoords(280f, 180f, 100f, 100f);
            float[] textureCoords3 = CreateRectTextureCoords();
            FillPolygonWithSolidColor2(quadVertices3, textureCoords3, 6, PixelFarm.Drawing.Color.Blue);
            //---------------------------------------------------------
            float[] quadVertices = CreateRectCoords(250f, 150f, 100f, 100f);
            FillPolygonWithSolidColor(quadVertices, 6, PixelFarm.Drawing.Color.Yellow);
            //---------------------------------------------------------

            // DrawLines(0, 0, 300, 300);
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

        void FillPolygonWithSolidColor2(float[] onlyCoords, float[] textureCoords,
            int numVertices, PixelFarm.Drawing.Color c)
        {
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)c.R / 255f, (float)c.G / 255f, (float)c.B / 255f, (float)c.A / 255f);//use solid color  
            a_position.LoadV2f(onlyCoords, 2, 0);
            a_textureCoord.LoadV2f(textureCoords, 2, 0);
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
        static float[] CreateRectTextureCoords()
        {
            float[] vertices = new float[]{
               0.5f,0.5f,
               0.5f,0.5f,
               0.5f,0.5f,
               0.7f,0.7f,
               0.7f,0.7f,
               0.7f,0.7f};
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

