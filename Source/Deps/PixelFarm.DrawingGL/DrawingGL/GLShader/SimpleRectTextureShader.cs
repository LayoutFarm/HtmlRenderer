//MIT, 2016-2017, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    abstract class SimpleRectTextureShader : ShaderBase
    {
        protected ShaderVtxAttrib3f a_position;
        protected ShaderVtxAttrib2f a_texCoord;
        protected ShaderUniformMatrix4 u_matrix;
        protected ShaderUniformVar1 s_texture;
        protected static readonly ushort[] indices = new ushort[] { 0, 1, 2, 3 };
        public SimpleRectTextureShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
        }

        int orthoviewVersion = -1;
        protected void CheckViewMatrix()
        {
            int version = 0;
            if (orthoviewVersion != (version = _canvasShareResource.OrthoViewVersion))
            {
                orthoviewVersion = version;
                u_matrix.SetData(_canvasShareResource.OrthoView.data);
            }
        }


        public void RenderSubImage(GLBitmap bmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {
            //TODO: review float array here,use buffer instead
            RenderSubImage(bmp, new float[]{
                srcLeft,srcTop,
                srcW,srcH,
                targetLeft,
                targetTop
            }, 1);
        }

        public void RenderSubImage(GLBitmap bmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop, float scale)
        {
            //TODO: review float array here,use buffer instead
            RenderSubImage(bmp, new float[]{
                srcLeft,srcTop,
                srcW,srcH,
                targetLeft,
                targetTop
            }, scale);
        }
        public void RenderSubImage(GLBitmap bmp, float[] srcDestList, float scale)
        {
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(0);
            OnSetVarsBeforeRenderer();

            int j = srcDestList.Length;
            for (int i = 0; i < j;)
            {

                float srcLeft = srcDestList[i];
                float srcTop = srcDestList[i + 1];
                float srcW = srcDestList[i + 2];
                float srcH = srcDestList[i + 3];
                float targetLeft = srcDestList[i + 4];
                float targetTop = srcDestList[i + 5];

                i += 6;
                //-------------------------------
                float srcBottom = srcTop - srcH;
                float srcRight = srcLeft + srcW;
                float orgBmpW = bmp.Width;
                float orgBmpH = bmp.Height;
                unsafe
                {
                    if (bmp.IsInvert)
                    {

                        float* imgVertices = stackalloc float[5 * 4];
                        {
                            imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)
                            imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                            //---------------------
                            imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                            imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcTop / orgBmpH; //texture coord 1  (left,top)

                            //---------------------
                            imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                            imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcBottom / orgBmpH; //texture coord 2  (right,bottom)

                            //---------------------
                            imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                            imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcTop / orgBmpH; //texture coord 3 (right,top)
                        }
                        a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                        a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                    }
                    else
                    {
                        float* imgVertices = stackalloc float[5 * 4];
                        {
                            imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                            imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcTop / orgBmpH; //texture coord 0 (left,top)

                            //---------------------
                            imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                            imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcBottom / orgBmpH; //texture coord 1 (left,bottom)

                            //---------------------
                            imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                            imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcTop / orgBmpH; //texture coord 2 (right,top)

                            //---------------------
                            imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                            imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcBottom / orgBmpH; //texture coord 3  (right,bottom)
                        }
                        a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                        a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                    }
                }
                GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
            }

        }
        public void Render(GLBitmap bmp, float left, float top, float w, float h)
        {
            unsafe
            {


                if (bmp.IsInvert)
                {

                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                        //imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                        //---------------------
                        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1  (left,top)

                        //---------------------
                        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2  (right,bottom)

                        //---------------------
                        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 (right,top)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                else
                {
                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                        imgVertices[3] = 0; imgVertices[4] = 1; //texture coord 0 (left,top)

                        //---------------------
                        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = 0; imgVertices[9] = 0; //texture coord 1 (left,bottom)

                        //---------------------
                        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = 1; imgVertices[14] = 1; //texture coord 2 (right,top)

                        //---------------------
                        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = 1; imgVertices[19] = 0; //texture coord 3  (right,bottom)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }




                //if (bmp.IsInvert)
                //{

                //    float* imgVertices = stackalloc float[5 * 4];
                //    {
                //        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0
                //        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 

                //        //---------------------
                //        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1
                //        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1 

                //        //---------------------
                //        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2
                //        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2 

                //        //---------------------
                //        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3
                //        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 
                //    };

                //    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                //    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                //}
                //else
                //{
                //    float* imgVertices = stackalloc float[5 * 4];
                //    {
                //        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0
                //        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 

                //        //---------------------
                //        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1
                //        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1 

                //        //---------------------
                //        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2
                //        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2 

                //        //---------------------
                //        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3
                //        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 
                //    };

                //    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                //    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                //}
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(0);
            OnSetVarsBeforeRenderer();
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
        public void Render(int textureId, float left, float top, float w, float h)
        {
            //ratio 1:1
            unsafe
            {
                float* imgVertices = stackalloc float[5 * 4];
                {
                    imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0
                    imgVertices[3] = 0; imgVertices[4] = 0; //texture coord  0 
                    //---------------------
                    imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1
                    imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1 
                    imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2
                    imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2 
                    imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3
                    imgVertices[18] = 1; imgVertices[19] = 1; //texture  coord 3
                };
                a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                a_texCoord.UnsafeLoadMixedV2f((imgVertices + 3), 5);
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(0);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
        protected bool BuildProgram(string vs, string fs)
        {
            //---------------------
            if (!shaderProgram.Build(vs, fs))
            {
                return false;
            }
            //-----------------------
            a_position = shaderProgram.GetAttrV3f("a_position");
            a_texCoord = shaderProgram.GetAttrV2f("a_texCoord");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            s_texture = shaderProgram.GetUniform1("s_texture");
            OnProgramBuilt();
            return true;
        }
        protected virtual void OnSetVarsBeforeRenderer()
        {
        }
        protected virtual void OnProgramBuilt()
        {
        }
    }

    class GdiImageTextureShader : SimpleRectTextureShader
    {
        public GdiImageTextureShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
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
            //we need to switch color component ***
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
            BuildProgram(vs, fs);
        }
    }

    class OpenGLESTextureShader : SimpleRectTextureShader
    {
        public OpenGLESTextureShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
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
            //this case we not need to swap 
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      void main()
                      {
                         gl_FragColor = texture2D(s_texture, v_texCoord);        
                      }
                ";
            BuildProgram(vs, fs);
        }
    }


    class GdiImageTextureWithWhiteTransparentShader : SimpleRectTextureShader
    {
        public GdiImageTextureWithWhiteTransparentShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
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
                         if((c[2] ==1.0) && (c[1]==1.0) && (c[0]== 1.0) && (c[3] == 1.0)){
                            discard;
                         }else{                                                   
                            gl_FragColor =  vec4(c[2],c[1],c[0],c[3]);  
                         }
                      }
                ";
            BuildProgram(vs, fs);
        }
    }
}