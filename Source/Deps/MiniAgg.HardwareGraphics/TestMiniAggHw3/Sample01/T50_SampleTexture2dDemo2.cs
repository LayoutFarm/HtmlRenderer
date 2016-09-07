//
// Copyright (c) 2014 The ANGLE Project Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//

//            Based on Hello_Triangle.c from
// Book:      OpenGL(R) ES 2.0 Programming Guide
// Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
// ISBN-10:   0321502795
// ISBN-13:   9780321502797
// Publisher: Addison-Wesley Professional
// URLs:      http://safari.informit.com/9780321563835
//            http://www.opengles-book.com


#region Using Directives

using System;
using OpenTK.Graphics.ES20;
using Mini;
#endregion


namespace OpenTkEssTest
{
    [Info(OrderCode = "050")]
    [Info("T50_SampleTexture2dDemo2")]
    public class T50_SampleTexture2dDemo2 : PrebuiltGLControlDemoBase
    {
        int u_matrix;
        protected override void OnInitGLProgram(object sender, EventArgs handler)
        {
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform mat4 u_mvpMatrix; 
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = u_mvpMatrix* a_position;
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //in fs, angle on windows 
            //we need to switch color component
            //because we store value in memory as BGRA
            //and gl expect input in RGBA
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      void main()
                      {
                         vec4 c = texture2D(s_texture, v_texCoord);                            
                         gl_FragColor =  vec4(c[2],c[1],c[0],c[3]);
                      }
                ";
            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                //return false
            }

            // Get the attribute locations
            mPositionLoc = GL.GetAttribLocation(mProgram, "a_position");
            mTexCoordLoc = GL.GetAttribLocation(mProgram, "a_texCoord");
            u_matrix = GL.GetUniformLocation(mProgram, "u_mvpMatrix");
            // Get the sampler location
            mSamplerLoc = GL.GetUniformLocation(mProgram, "s_texture");
            //// Load the texture
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap("d:\\WImageTest\\test001.png");
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap("d:\\WImageTest\\test001.png");
            int bmpW = bmp.Width;
            int bmpH = bmp.Height;
            mTexture = LoadTexture(bmp);
            GL.ClearColor(0, 0, 0, 0);
            //================================================================================

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int max = Math.Max(this.Width, this.Height);
            orthoViewMat = MyMat4.ortho(0, max, 0, max, 0, 1).data;
            //square viewport
            GL.Viewport(0, 0, max, max);
            imgVertices = new float[]
            {
                0, bmpH,0,
                0,0,
                //---------------------
                0,0,0,
                0,1,
                //---------------------
                bmpW ,bmpH ,0,
                1,0,
                //---------------------
                bmpW,0,0,
                1,1
           };
        }

        float[] imgVertices;
        ushort[] indices = new ushort[] { 0, 1, 2, 3 };
        float[] orthoViewMat;
        protected override void OnGLRender(object sender, EventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(mProgram);
            unsafe
            {
                fixed (float* h = &imgVertices[0])
                {
                    GL.VertexAttribPointer(mPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)h);
                    GL.VertexAttribPointer(mTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)(h + 3));
                }
            }

            GL.EnableVertexAttribArray(mPositionLoc);
            GL.EnableVertexAttribArray(mTexCoordLoc);
            GL.UniformMatrix4(u_matrix, 1, false, orthoViewMat);
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mTexture);
            // Set the texture sampler to texture unit to 0             
            GL.Uniform1(mSamplerLoc, 0);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
            miniGLControl.SwapBuffers();
        }
        protected override void DemoClosing()
        {
            GL.DeleteProgram(mProgram);
            GL.DeleteTexture(mTexture);
            mProgram = mTexture = 0;
        }
        static int LoadTexture(System.Drawing.Bitmap bmp)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            //     glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
            //GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, static_cast<GLsizei>(image.width), static_cast<GLsizei>(image.height), 0,
            //             GL_RGBA, GL_UNSIGNED_BYTE, image.data.data());
            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bmpdata.Scan0);
            bmp.UnlockBits(bmpdata);
            //glGenerateMipmap(GL_TEXTURE_2D);
            GL.GenerateMipmap(TextureTarget.Texture2D);
            return texture;
        }
        int mProgram;
        // Attribute locations
        int mPositionLoc;
        int mTexCoordLoc;
        // Sampler location
        int mSamplerLoc;
        // Texture handle
        int mTexture;
    }
}
