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
using OpenTK;
using OpenTK.Graphics.ES20;
using Mini;
#endregion

namespace OpenTkEssTest
{
    [Info(OrderCode = "044")]
    [Info("T44_SimpleVertexShader")]
    public class T44_SimpleVertexShader : PrebuiltGLControlDemoBase
    {
        bool isGLInit;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
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
            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                //return false
                throw new NotSupportedException();
            }


            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Get the attribute locations

            mPositionLoc = GL.GetAttribLocation(mProgram, "a_position");
            mTexCoordLoc = GL.GetAttribLocation(mProgram, "a_texcoord");
            // Get the uniform locations
            mMVPMatrixLoc = GL.GetUniformLocation(mProgram, "u_mvpMatrix");
            cube = new CubeGeometry();
            GeometryUtils.GenerateCubeGeometry(-0.5f, cube);
            mRotation = 45.0f;
            GL.ClearColor(0, 0, 0, 0);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.CullFace);
            this.EnableAnimationTimer = true;
            isGLInit = true;
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //// Set the viewport
            //glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Viewport(0, 0, this.Width, this.Height);
            //// Clear the color buffer
            //glClear(GL_COLOR_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //// Use the program object
            //glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            //// Load the vertex position
            //glVertexAttribPointer(mPositionLoc, 3, GL_FLOAT, GL_FALSE, 0, mCube.positions.data());
            GL.VertexAttribPointer(mPositionLoc, 3, VertexAttribPointerType.Float, false, 0, cube.positions);
            //glEnableVertexAttribArray(mPositionLoc);
            GL.EnableVertexAttribArray(mPositionLoc);
            //// Load the texcoord data
            //glVertexAttribPointer(mTexcoordLoc, 2, GL_FLOAT, GL_FALSE, 0, mCube.texcoords.data());
            GL.VertexAttribPointer(mTexCoordLoc, 2, VertexAttribPointerType.Float, false, 0, cube.texcoords);
            //glEnableVertexAttribArray(mTexcoordLoc);
            GL.EnableVertexAttribArray(mTexCoordLoc);
            //// Draw the cube
            //glDrawElements(GL_TRIANGLES, mCube.indices.size(), GL_UNSIGNED_SHORT, mCube.indices.data());
            GL.DrawElements(BeginMode.Triangles, cube.indices.Length, DrawElementsType.UnsignedShort, cube.indices);
            this.miniGLControl.SwapBuffers();
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
            mProgram = 0;
        }
        // Handle to a program object
        int mProgram;
        // Attribute locations
        int mPositionLoc;
        int mTexCoordLoc;
        // Uniform locations
        int mMVPMatrixLoc;
        // Current rotation
        float mRotation;
        // Geometry data
        CubeGeometry cube;
        //CubeGeometry mCube;
    }
}