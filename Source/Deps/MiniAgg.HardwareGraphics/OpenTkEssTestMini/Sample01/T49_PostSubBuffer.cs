//
// Copyright (c) 2014 The ANGLE Project Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//

//            Based on Simple_VertexShader.c from
// Book:      OpenGL(R) ES 2.0 Programming Guide
// Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
// ISBN-10:   0321502795
// ISBN-13:   9780321502797
// Publisher: Addison-Wesley Professional
// URLs:      http://safari.informit.com/9780321563835
//            http://www.opengles-book.com



#region Using Directives

using System;
using System.Text;
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
    using EGLNativeDisplayType = IntPtr;
    using EGLNativePixmapType = IntPtr;
    using EGLConfig = IntPtr;
    using EGLContext = IntPtr;
    using EGLDisplay = IntPtr;
    using EGLSurface = IntPtr;
    using EGLClientBuffer = IntPtr;

    [Info(OrderCode = "049")]
    [Info("T49_PostSubBuffer")]
    public class T49_PostSubBuffer : PrebuiltGLControlDemoBase
    {
        //EGLBoolean (EGLAPIENTRYP PFNEGLPOSTSUBBUFFERNVPROC) (EGLDisplay dpy, EGLSurface surface, EGLint x, EGLint y, EGLint width, EGLint height);

        delegate bool PFNEGLPOSTSUBBUFFERNVPROC(EGLDisplay dpy, EGLSurface surface, int x, int y, int width, int height);
       
        public T49_PostSubBuffer()
        {
            this.Width = 1280;
            this.Height = 720;
        }

        bool isGLInit;
        
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {


          
            IntPtr eglPostSubBufferNVFuncPtr = OpenTK.Platform.Egl.EglFuncs.GetProcAddress("eglPostSubBufferNV");
         
            if (eglPostSubBufferNVFuncPtr == IntPtr.Zero)
            {
                throw new NotSupportedException();
            }

            mPostSubBufferNV = (PFNEGLPOSTSUBBUFFERNVPROC)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(eglPostSubBufferNVFuncPtr, typeof(PFNEGLPOSTSUBBUFFERNVPROC));
            
            //   mPostSubBufferNV = (PFNEGLPOSTSUBBUFFERNVPROC)eglGetProcAddress("eglPostSubBufferNV");
            //if (!mPostSubBufferNV)
            //{
            //    std::cerr << "Could not load eglPostSubBufferNV.";
            //    return false;
            //}
            string vs = @"
                 uniform mat4 u_mvpMatrix;
                 attribute vec4 a_position;
                 attribute vec2 a_texcoord;
                 varying vec2 v_texcoord;
                 void main()
                 {
                        gl_Position = u_mvpMatrix * a_position;
                        v_texcoord = a_texcoord;
                 }
            ";

            string fs = @"
                precision mediump float;
                varying vec2 v_texcoord;
                void main()
                {
                      gl_FragColor = vec4(v_texcoord.x, v_texcoord.y, 1.0, 1.0);
                }
            ";

            //mProgram = CompileProgram(vs, fs);
            //if (!mProgram)
            //{
            //    return false;
            //}
            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                throw new NotSupportedException();
            }
            //// Get the attribute locations
            //mPositionLoc = glGetAttribLocation(mProgram, "a_position");          
            mPositionLoc = GL.GetAttribLocation(mProgram, "a_position");
            //mTexcoordLoc = glGetAttribLocation(mProgram, "a_texcoord");
            mTexcoordLoc = GL.GetAttribLocation(mProgram, "a_texcoord");
            //// Get the uniform locations
            //mMVPMatrixLoc = glGetUniformLocation(mProgram, "u_mvpMatrix");
            mMVPMatrixLoc = GL.GetUniformLocation(mProgram, "u_mvpMatrix");
            //// Generate the geometry data
            //GenerateCubeGeometry(0.5f, &mCube);
            mCube = new CubeGeometry();
            GeometryUtils.GenerateCubeGeometry(0.5f, mCube);

            //// Set an initial rotation
            //mRotation = 45.0f;
            mRotation = 45.0f;
            //// Clear the whole window surface to blue.
            //glClearColor(0.0f, 0.0f, 1.0f, 0.0f); 
            GL.ClearColor(0, 0, 1, 0);
            //glClear(GL_COLOR_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //SampleApplication::swap(); 
           
            //this.miniGLControl.SwapBuffers();

            //glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.ClearColor(0, 0, 0, 0); 
            //glCullFace(GL_BACK);
            GL.CullFace(CullFaceMode.Back);
            //glEnable(GL_CULL_FACE);
            GL.Enable(EnableCap.CullFace);
            //return true; 
            isGLInit = true;
            this.EnableAnimationTimer = true;
        }

        void CustomSwap()
        {

            // Instead of letting the application call eglSwapBuffers, call eglPostSubBufferNV here instead
            int windowWidth = this.Width;
            int windowHeight = this.Height;
            EGLDisplay display = getDisplay();
            EGLSurface surface = getSurface();

            //test drop draw target to (60,200) and resize 
            mPostSubBufferNV(display, surface, 60, 200, windowWidth - 120, windowHeight - 120);
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {

            if (!isGLInit)
            {
                return;
            }

            GL.Viewport(0, 0, this.Width, this.Height);

            //// Clear the color buffer
            //glClear(GL_COLOR_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //// Use the program object
            //glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            //// Load the vertex position
            //glVertexAttribPointer(mPositionLoc, 3, GL_FLOAT, GL_FALSE, 0, mCube.positions.data());
            GL.VertexAttribPointer(mPositionLoc, 3, VertexAttribPointerType.Float, false, 0, mCube.positions);

            //glEnableVertexAttribArray(mPositionLoc);
            GL.EnableVertexAttribArray(mPositionLoc);
            //// Load the texcoord data
            //glVertexAttribPointer(mTexcoordLoc, 2, GL_FLOAT, GL_FALSE, 0, mCube.texcoords.data());
            GL.VertexAttribPointer(mTexcoordLoc, 2, VertexAttribPointerType.Float, false, 0, mCube.texcoords);
            //glEnableVertexAttribArray(mTexcoordLoc);
            GL.EnableVertexAttribArray(mTexcoordLoc);
            //// Draw the cube
            //glDrawElements(GL_TRIANGLES, mCube.indices.size(), GL_UNSIGNED_SHORT, mCube.indices.data());
            GL.DrawElements(BeginMode.Triangles, mCube.indices.Length, DrawElementsType.UnsignedShort, mCube.indices);

            // this.miniGLControl.SwapBuffers(); 
            // Instead of letting the application call eglSwapBuffers, call eglPostSubBufferNV here instead 
            CustomSwap();
        }

        static double fmod(double numer, double denom)
        {
            return numer - ((int)(numer / denom));
        }
        protected override void OnTimerTick(object sender, EventArgs e)
        {
            if (!isGLInit)
            {
                return;
            }


            float dt = 0.5f;
            mRotation = (float)fmod(mRotation + (dt * 40.0f), 360.0f);


            //Matrix4 perspectiveMatrix = Matrix4::perspective(60.0f, float(getWindow()->getWidth()) / getWindow()->getHeight(),
            //                                                 1.0f, 20.0f);
            MyMat4 perspectiveMat = MyMat4.perspective(60.0f, (float)Width / (float)Height, 1.0f, 20.0f);
            //Matrix4 modelMatrix = Matrix4::translate(Vector3(0.0f, 0.0f, -2.0f)) *
            //                      Matrix4::rotate(mRotation, Vector3(1.0f, 0.0f, 1.0f));

            MyMat4 modelMat = MyMat4.translate(new Vector3(0f, 0f, -2f)) *
                              MyMat4.rotate(mRotation, new Vector3(1, 0, 1));

            MyMat4 viewMatrix = MyMat4.GetIdentityMat();

            MyMat4 mvpMatrix = perspectiveMat * viewMatrix * modelMat;

            //// Load the matrices
            //glUniformMatrix4fv(mMVPMatrixLoc, 1, GL_FALSE, mvpMatrix.data);
            GL.UniformMatrix4(mMVPMatrixLoc, 1, false, mvpMatrix.data);


        }
        protected override void DemoClosing()
        {
            GL.DeleteProgram(mProgram);
        }
        // Handle to a program object
        int mProgram;

        // Attribute locations
        int mPositionLoc;
        int mTexcoordLoc;

        // Uniform locations
        int mMVPMatrixLoc;

        // Current rotation
        float mRotation;

        // Geometry data
        CubeGeometry mCube;


        //// eglPostSubBufferNV entry point
        PFNEGLPOSTSUBBUFFERNVPROC mPostSubBufferNV;
    }
}