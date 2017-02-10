//MIT, 2016-2017, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class BlurShader : SimpleRectTextureShader
    {
        ShaderUniformVar1 _horizontal;
        ShaderUniformVar1 _isBigEndian;
        public BlurShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            BuildShaderV3();
        }
        void BuildShaderV3()
        {
            //glsl credit: http://xissburg.com/faster-gaussian-blur-in-glsl/
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
                     
                      uniform sampler2D s_texture;
                      uniform int isBigEndian;
                      uniform int blur_horizontal; 
                      varying vec2 v_texCoord; 
                      void main()
                      {                         
                        vec4 c = vec4(0.0);
                        float v_texCoord1= v_texCoord[1];
                        float v_texCoord0 =v_texCoord[0];
                        if(blur_horizontal==1){
                            c += texture2D(s_texture,vec2(v_texCoord0-0.028,v_texCoord1))*0.0044299121055113265;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.024,v_texCoord1))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.020,v_texCoord1))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.016,v_texCoord1))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.012,v_texCoord1))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.008,v_texCoord1))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.004,v_texCoord1))*0.147308056121;
                            c += texture2D(s_texture, v_texCoord         )*0.159576912161;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.004,v_texCoord1))*0.147308056121;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.008,v_texCoord1))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.012,v_texCoord1))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.016,v_texCoord1))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.020,v_texCoord1))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.024,v_texCoord1))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.028,v_texCoord1))*0.0044299121055113265;
                        }else{
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.028))*0.0044299121055113265;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.024))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.020))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.016))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.012))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.008))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.004))*0.147308056121;
                            c += texture2D(s_texture, v_texCoord         )*0.159576912161;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.004))*0.147308056121;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.008))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.012))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.016))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.020))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0,v_texCoord1+0.024))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.028))*0.0044299121055113265; 
                        }
                        if(isBigEndian ==1){
                            gl_FragColor = c;
                        }else{
                            gl_FragColor =  vec4(c[2],c[1],c[0],c[3]);
                        }
                      }
                ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            _horizontal = shaderProgram.GetUniform1("blur_horizontal");
            //TODO: review here
            //temp fixed for big vs little-endian
            _isBigEndian = shaderProgram.GetUniform1("isBigEndian");
        }
        public bool IsHorizontal { get; set; }
        public bool IsBigEndian { get; set; }
        protected override void OnSetVarsBeforeRenderer()
        {
            _horizontal.SetValue(IsHorizontal ? 1 : 0);
            _isBigEndian.SetValue(IsBigEndian ? 1 : 0);
        }
    }



    public static class Mat3x3ConvGen
    {
        //credit: http://webglfundamentals.org/webgl/webgl-2d-image-3x3-convolution.html

        public static readonly
            float[] normal = new float[] {
              0,0,0,
              0,1,0,
              0,0,0
        };
        public static readonly
            float[] gaussianBlur = new float[] {
              0.045f, 0.122f, 0.045f,
              0.122f, 0.332f, 0.122f,
              0.045f, 0.122f, 0.045f
        };
        public static readonly
            float[] gaussianBlur2 = new float[] {
              1, 2, 1,
              2, 4, 2,
              1, 2, 1
        };
        public static readonly
            float[] gaussianBlur3 = new float[] {
              0.045f, 0.122f, 0.045f,
              0.122f, 0.332f, 0.122f,
              0.045f, 0.122f, 0.045f
        };
        public static readonly
            float[] unsharpen = new float[]
        {
              -1, -1, -1,
              -1,  9, -1,
              -1, -1, -1
        };
        public static readonly
            float[] sharpness = new float[]
            {
              -1, -1, -1,
              -1,  5, -1,
              -1, -1, -1
            };
        public static readonly
            float[] sharpen = new float[]
          {
              -1, -1, -1,
              -1,  16, -1,
              -1, -1, -1
          };
        public static readonly
          float[] edgeDetect = new float[]
        {
              -0.125f, -0.125f, -0.125f,
             -0.125f,  1,     -0.125f,
             -0.125f, -0.125f, -0.125f
        };
        public static readonly
          float[] edgeDetect2 = new float[]
          {
               -1, -1, -1,
               -1,  8, -1,
               -1, -1, -1
          };
        public static readonly
          float[] edgeDetect3 = new float[]
          {
              -5, 0, 0,
               0, 0, 0,
               0, 0, 5
          };
        public static readonly
          float[] edgeDetect4 = new float[]
          {
              -1,-1,-1,
               0, 0, 0,
               1, 1, 1
          };
        public static readonly
          float[] edgeDetect5 = new float[]
          {
              -1,-1,-1,
               2, 2,2,
              -1,-1,-1,
          };
        public static readonly
         float[] edgeDetect6 = new float[]
         {
              -5, -5, -5,
              -5, 39, -5,
              -5, -5, -5
         };
        public static readonly
        float[] sobelHorizontal = new float[]
        {
              1,  2,  1,
              0,  0,  0,
             -1, -2, -1
        };
        public static readonly
         float[] sobelVertical = new float[]
         {
             1,  0, -1,
             2,  0, -2,
             1,  0, -1
         };
        public static readonly
         float[] previtHorizontal = new float[]
         {
             1,  1,  1,
             0,  0,  0,
             -1, -1, -1
         };
        public static readonly
          float[] previtVertical = new float[]
          {
            1,  0, -1,
            1,  0, -1,
            1,  0, -1
          };
        public static readonly
          float[] boxBlur = new float[]
          {
            0.111f, 0.111f, 0.111f,
            0.111f, 0.111f, 0.111f,
            0.111f, 0.111f, 0.111f
          };
        public static readonly
          float[] triangleBlur = new float[]
          {
              0.0625f, 0.125f, 0.0625f,
              0.125f,  0.25f,  0.125f,
              0.0625f, 0.125f, 0.0625f
          };
        public static readonly
         float[] emboss = new float[]
         {
               -2, -1,  0,
               -1,  1,  1,
                0,  1,  2
         };
    }



    class Conv3x3TextureShader : SimpleRectTextureShader
    {
        //credit: http://webglfundamentals.org/webgl/webgl-2d-image-3x3-convolution.html
        ShaderUniformVar1 _isBigEndian;
        ShaderUniformVar2 _onepix_xy;
        ShaderUniformMatrix3 _convKernel;
        ShaderUniformVar1 _kernelWeight;
        float[] kernels;
        float kernelWeight;
        float toDrawImgW = 1, toDrawImgH = 1;
        public Conv3x3TextureShader(CanvasToShaderSharedResource canvasShareResource)
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
            //we need to switch color component
            //because we store value in memory as BGRA
            //and gl expect input in RGBA
            string fs = @"
                      precision mediump float;
                     
                      uniform sampler2D s_texture;
                      uniform int isBigEndian;
                      uniform mat3 convKernel;
                      uniform float kernelWeight;  
                      uniform vec2 onepix_xy;
                        
                      varying vec2 v_texCoord; 
                      void main()
                      {                         
                        vec4 c = vec4(0.0);
                        float v_texCoord1= v_texCoord[1];
                        float v_texCoord0 =v_texCoord[0];
                        float one_x=onepix_xy[0];               
                        float one_y=onepix_xy[1];

                        c += texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1+one_y))*convKernel[0][0];
                        c += texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1+one_y))*convKernel[0][1];
                        c += texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1+one_y))*convKernel[0][2];

                        c += texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1))*convKernel[1][0];
                        c += texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1))*convKernel[1][1];
                        c += texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1))*convKernel[1][2];

                        c += texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1-one_y))*convKernel[2][0];
                        c += texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1-one_y))*convKernel[2][1];
                        c += texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1-one_y))*convKernel[2][2];
                         
                        c= c / kernelWeight;

                        if(isBigEndian ==1){
                            gl_FragColor = vec4(c[0],c[1],c[2],1);
                        }else{
                            gl_FragColor =  vec4(c[2],c[1],c[0],1);
                        }
                      }
                ";
            BuildProgram(vs, fs);
            SetConvolutionKernel(Mat3x3ConvGen.gaussianBlur);
        }
        public void SetConvolutionKernel(float[] kernels)
        {
            this.kernels = kernels;
            //calculate kernel weight
            float total = 0;
            for (int i = kernels.Length - 1; i >= 0; --i)
            {
                total += kernels[i];
            }
            if (total <= 0)
            {
                total = 1;
            }
            kernelWeight = total;
        }
        public bool IsBigEndian { get; set; }
        protected override void OnProgramBuilt()
        {
            _isBigEndian = shaderProgram.GetUniform1("isBigEndian");
            _convKernel = shaderProgram.GetUniformMat3("convKernel");
            _onepix_xy = shaderProgram.GetUniform2("onepix_xy");
            _kernelWeight = shaderProgram.GetUniform1("kernelWeight");
        }
        protected override void OnSetVarsBeforeRenderer()
        {
            _isBigEndian.SetValue(IsBigEndian ? 1 : 0);
            _convKernel.SetData(kernels);
            _onepix_xy.SetValue(1f / toDrawImgW, 1f / toDrawImgH);
            _kernelWeight.SetValue(kernelWeight);
        }
        public void SetBitmapSize(int w, int h)
        {
            this.toDrawImgW = w;
            this.toDrawImgH = h;
        }
    }
}