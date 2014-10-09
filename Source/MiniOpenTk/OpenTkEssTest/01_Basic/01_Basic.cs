#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

#region --- Using Directives ---

using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using OpenTK;
using OpenTK.Graphics.OpenGL;

#endregion --- Using Directives ---

namespace OpenTkEssTest
{
    //[Example("Display Lists", ExampleCategory.OpenGL, "1.x", 2, Documentation = "DisplayLists")]
    public class T01_Basic : GameWindow
    {
        #region --- Fields ---

        const int num_lists = 13;
        int[] lists = new int[num_lists];

        #endregion

        #region --- Constructor ---

        public T01_Basic()
            : base(800, 600)
        {
        }

        #endregion

        #region OnLoad

        protected override void OnLoad(EventArgs e)
        {

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            //GL.glMatrixMode(GL.GL_PROJECTION);
            //GL.glLoadIdentity();
            float aspect = this.ClientSize.Width / (float)this.ClientSize.Height;

            Matrix4 projection_matrix;
            Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspect, 0.1f, 100f, out projection_matrix);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection_matrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            GL.DeleteLists(lists[0], num_lists);
        }

        #endregion

        #region OnResize

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(ClientRectangle);

            float aspect = this.ClientSize.Width / (float)this.ClientSize.Height;

            Matrix4 projection_matrix;
            Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspect, 1, 64, out projection_matrix);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection_matrix);
        }

        #endregion

        #region OnUpdateFrame

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                this.Exit();
            }
        }

        #endregion

        #region OnRenderFrame

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //Matrix4 lookat = Matrix4.LookAt(0, 0, 16, 0, 0, 0, 0, 1, 0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref lookat);
            float rotate = 0;
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.CallLists(num_lists, ListNameType.Int, lists);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(-1.5f, 0f, -6f);

            //GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);		// Clear the Screen and the Depth Buffer
            //GL.glMatrixMode(GL.GL_MODELVIEW);				// Modelview Matrix
            // GL.glLoadIdentity();							// reset the current modelview matrix
            // GL.glTranslatef(-1.5f, 0.0f, -6.0f);				// move 1.5 Units left and 6 Units into the screen
            // GL.glRotatef(rtri, 0.0f, 1.0f, 0.0f);				// rotate the triangle on the Y-axis
            rotate += 0.2f;
            // increase the rotation variable

            GL.Begin(BeginMode.Triangles);
            GL.Color3(1f, 0, 0); GL.Vertex3(0.0f, 1.0f, 0.0f);
            GL.Color3(0f, 1, 0); GL.Vertex3(-1f, -1f, 0.0f);
            GL.Color3(0f, 0, 1); GL.Vertex3(1f, -1f, 0.0f);
            GL.End();

            GL.LoadIdentity();
            //rotate += 50f;
            GL.Translate(1.5f, 0.0f, -6.0f);
            //GL.Rotate(rotate, 1.0f, 0.0f, 0.0f);
            GL.Color3(0.5f, 0.5f, 1.0f);

            //-------------------------------
            DrawQuads(-1, 1, .5f, .5f);

            //-------------------------------
            float cx = -1;
            float cy = 0;
            for (int i = 0; i < 10; i++)
            {
                DrawQuads(cx, cy, .5f, .5f);
                cx += 2;
                cy += 2;
            }


            SwapBuffers();
        }
        #endregion

        static void DrawQuads(float x, float y, float w, float h)
        {
            //clock-wise 
            GL.Begin(BeginMode.Triangles);

            GL.Vertex3(x, y, 0);//1
            GL.Vertex3(x + w, y, 0);//2
            GL.Vertex3(x + w, y - h, 0);//3

            GL.Vertex3(x + w, y - h, 0);//3
            GL.Vertex3(x, y - h, 0);//4
            GL.Vertex3(x, y, 0);//1

            GL.End();
        }


    }



}