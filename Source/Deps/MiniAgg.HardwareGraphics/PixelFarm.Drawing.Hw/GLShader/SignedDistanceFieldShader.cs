//MIT 2016, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class SingleChannelSdf : SimpleRectTextureShader
    {
        //note not correct
        //TODO: fix 

        ShaderUniformVar4 _u_color;
        ShaderUniformVar1 _u_buffer;
        ShaderUniformVar1 _u_gamma;
        public SingleChannelSdf(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            //credit: https://www.mapbox.com/blog/text-signed-distance-fields/
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
            string fs = @"
                precision mediump float;

                uniform sampler2D s_texture;
                uniform vec4 u_color;
                uniform float u_buffer;
                uniform float u_gamma;

                varying vec2 v_texCoord;

                void main() {
                    float dist = texture2D(s_texture, v_texCoord).r;
                    float alpha = smoothstep(u_buffer - u_gamma, u_buffer + u_gamma, dist);
                    gl_FragColor = vec4(u_color.rgb, alpha * u_color.a); 
                } 
             ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            _u_color = shaderProgram.GetUniform4("u_color");
            _u_buffer = shaderProgram.GetUniform1("u_buffer");
            _u_gamma = shaderProgram.GetUniform1("u_gamma");
        }
        protected override void OnSetVarsBeforeRenderer()
        {
            PixelFarm.Drawing.Color fgColor = ForegroundColor;
            _u_color.SetValue((float)fgColor.R / 255f, (float)fgColor.G / 255f, (float)fgColor.B / 255f, (float)fgColor.A / 255f);
            _u_buffer.SetValue(192f / 256f);
            _u_gamma.SetValue(1f);
        }
        public PixelFarm.Drawing.Color ForegroundColor;
    }



    class MultiChannelSdf : SimpleRectTextureShader
    {

        ShaderUniformVar4 _fgColor;
        public MultiChannelSdf(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            //credit: https://github.com/Chlumsky/msdfgen 

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
            //enable derivative extension  for fwidth() function
            //see 
            //https://www.khronos.org/registry/gles/extensions/OES/OES_standard_derivatives.txt
            //https://github.com/AnalyticalGraphicsInc/cesium/issues/745
            //https://developer.mozilla.org/en-US/docs/Web/API/OES_standard_derivatives
            //https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/Using_Extensions
            string fs = @"
                        #ifdef GL_OES_standard_derivatives
                            #extension GL_OES_standard_derivatives : enable
                        #endif  
                        precision mediump float; 
                        varying vec2 v_texCoord;                
                        uniform sampler2D s_texture; //msdf texture 
                        uniform vec4 fgColor;

                        float median(float r, float g, float b) {
                            return max(min(r, g), min(max(r, g), b));
                        }
                        void main() {
                            vec4 sample = texture2D(s_texture, v_texCoord);
                            float sigDist = median(sample[0], sample[1], sample[2]) - 0.5;
                            float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0); 
                            vec4 finalColor=vec4(fgColor[0],fgColor[1],fgColor[2],opacity);
                            //mix(bgColor, fgColor, opacity);  
                            gl_FragColor= finalColor;
                        }
             ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {

            _fgColor = shaderProgram.GetUniform4("fgColor");
        }

        public PixelFarm.Drawing.Color ForegroundColor;
        protected override void OnSetVarsBeforeRenderer()
        {

            PixelFarm.Drawing.Color fgColor = ForegroundColor;
            _fgColor.SetValue((float)fgColor.R / 255f, (float)fgColor.G / 255f, (float)fgColor.B / 255f, (float)fgColor.A / 255f);
        }
    }

    class MultiChannelSubPixelRenderingSdf : SimpleRectTextureShader
    {
        ShaderUniformVar4 _bgColor;
        ShaderUniformVar4 _fgColor;
        public MultiChannelSubPixelRenderingSdf(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            BuildProgramV1();
            //BuildProgramV2();
        }

        void BuildProgramV1()
        {
            //credit: https://github.com/Chlumsky/msdfgen 

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
            //enable derivative extension  for fwidth() function
            //see 
            //https://www.khronos.org/registry/gles/extensions/OES/OES_standard_derivatives.txt
            //https://github.com/AnalyticalGraphicsInc/cesium/issues/745
            //https://developer.mozilla.org/en-US/docs/Web/API/OES_standard_derivatives
            //https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/Using_Extensions
            string fs = @"
                        #ifdef GL_OES_standard_derivatives
                            #extension GL_OES_standard_derivatives : enable
                        #endif  
                        precision mediump float; 
                        varying vec2 v_texCoord;                
                        uniform sampler2D s_texture; //msdf texture
                        uniform vec4 bgColor;
                        uniform vec4 fgColor;

                        float median(float r, float g, float b) {
                            return max(min(r, g), min(max(r, g), b));
                        }
                        void main() {
                            vec4 sample = texture2D(s_texture, v_texCoord);
                            float sigDist = median(sample[0], sample[1], sample[2]) - 0.5;
                            float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);
                            float ddx= dFdx(sigDist);
                            float ddy= dFdy(sigDist);
                            //gl_FragColor = mix(bgColor, fgColor, opacity);//original
 
                            //for study ***
                            /*if(ddx>0.0){
                                //uphill
                                gl_FragColor = mix(bgColor, vec4(1.0,0,0,opacity), opacity);
                            } else if(ddx<0.0){
                                //downhill
                                gl_FragColor = mix(bgColor, vec4(0.0,0,1.0,opacity), opacity);                                
                            }else{
                                //stable
                                gl_FragColor = mix(bgColor, fgColor, opacity);
                            }  */
                                
                            
                            if(opacity == 1.0){
                                //100%
                                //gl_FragColor = mix(bgColor, fgColor, opacity);//original
                                //gl_FragColor = mix(bgColor, fgColor, opacity);//original        
                                gl_FragColor = vec4(fgColor[0],fgColor[1],fgColor[2],opacity);//original            
                            }else if(opacity<= (1.0/3.0)){
                                  if(ddx>0.0){
                                     //uphill
                                     float  c_r = bgColor[0];
                                     float  c_g = (mix(bgColor[1],fgColor[1], opacity/2.0)); //*
                                     float  c_b = (mix(bgColor[2],fgColor[2], opacity));                                    
                                     //gl_FragColor = mix(bgColor, vec4(c_r,c_g,c_b,1.0), opacity); 
                                     gl_FragColor =  vec4(c_r,c_g,c_b, opacity); 
                                  }else{
                                     float  c_r = (mix(bgColor[0],fgColor[0], opacity));
                                     float  c_g = (mix(bgColor[1],fgColor[1], opacity/2.0)); //***
                                     float  c_b = bgColor[2];
                                     //gl_FragColor = mix(bgColor, vec4(c_r,c_g,c_b,1.0), opacity); 
                                     gl_FragColor =  vec4(c_r,c_g,c_b, opacity); 
                                  }
                            }else if(opacity<= (2.0/3.0)){
                                  if(ddx>0.0){
                                     //uphill
                                     float  c_r = (mix(bgColor[0],fgColor[0], opacity/2.0));
                                     float  c_g = (mix(bgColor[1],fgColor[1], opacity));
                                     float  c_b = (mix(bgColor[2],fgColor[2], 1.0));
                                    
                                     //gl_FragColor = mix(bgColor, vec4(c_r,c_g,c_b,1.0), opacity);   
                                     gl_FragColor =  vec4(c_r,c_g,c_b, opacity);    
                                  }else{
                                     float  c_r = (mix(bgColor[0],fgColor[0], 1.0));
                                     float  c_g = (mix(bgColor[1],fgColor[1], opacity));
                                     float  c_b = (mix(bgColor[1],fgColor[1], opacity/2.0));
                                     //gl_FragColor = mix(bgColor, vec4(c_r,c_g,c_b,1.0), opacity); 
                                     gl_FragColor =  vec4(c_r,c_g,c_b, opacity); 
                                  }
                            }else{
                                  if(ddx>0.0){
                                     //uphill
                                     float  c_r = (mix(bgColor[0],fgColor[0], opacity));
                                     float  c_g = (mix(bgColor[1],fgColor[1], 1.0));
                                     float  c_b = (mix(bgColor[2],fgColor[2], 1.0));                                    
                                     //gl_FragColor = mix(bgColor, vec4(c_r,c_g,c_b,1.0), opacity); 
                                    gl_FragColor = vec4(c_r,c_g,c_b, opacity);
                                  }else{
                                     float  c_r = (mix(bgColor[0],fgColor[0], 1.0));
                                     float  c_g = (mix(bgColor[1],fgColor[1], 1.0));
                                     float  c_b = (mix(bgColor[2],fgColor[2], opacity));                                    
                                     //gl_FragColor = mix(bgColor, vec4(c_r,c_g,c_b,1.0), opacity); 
                                     gl_FragColor =  vec4(c_r,c_g,c_b, opacity); 
                                  }
                            }
                        }
             ";
            BuildProgram(vs, fs);
        }

        void BuildProgramV2()
        {
            //credit: https://github.com/Chlumsky/msdfgen 

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
            //enable derivative extension  for fwidth() function
            //see 
            //https://www.khronos.org/registry/gles/extensions/OES/OES_standard_derivatives.txt
            //https://github.com/AnalyticalGraphicsInc/cesium/issues/745
            //https://developer.mozilla.org/en-US/docs/Web/API/OES_standard_derivatives
            //https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/Using_Extensions
            string fs = @"
                        #ifdef GL_OES_standard_derivatives
                            #extension GL_OES_standard_derivatives : enable
                        #endif  
                        precision mediump float; 
                        varying vec2 v_texCoord;                
                        uniform sampler2D s_texture; //msdf texture
                        //uniform vec4 bgColor;
                        uniform vec4 fgColor;

                        float median(float r, float g, float b) {
                            return max(min(r, g), min(max(r, g), b));
                        }
                        void main() {
                            vec4 sample = texture2D(s_texture, v_texCoord);
                            float dist = texture2D(s_texture, v_texCoord).r;
                            //float alpha = smoothstep(u_buffer - u_gamma, u_buffer + u_gamma, dist);
                            //float alpha = smoothstep(1.0 - 0.5, 1.0 + 0.5, dist);
                            float opacity = smoothstep(0.1, 1.0, dist);
                            //gl_FragColor = vec4(u_color.rgb, alpha * u_color.a); 
                            //float opacity = clamp(dist + 0.5, 0.0, 1.0);
                            gl_FragColor = vec4(0.0,0.0,0.0,opacity);
                        }
             ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            _bgColor = shaderProgram.GetUniform4("bgColor");
            _fgColor = shaderProgram.GetUniform4("fgColor");
        }
        public PixelFarm.Drawing.Color BackgroundColor;
        public PixelFarm.Drawing.Color ForegroundColor;
        protected override void OnSetVarsBeforeRenderer()
        {
            PixelFarm.Drawing.Color bgColor = BackgroundColor;
            PixelFarm.Drawing.Color fgColor = ForegroundColor;
           // _bgColor.SetValue((float)bgColor.R / 255f, (float)bgColor.G / 255f, (float)bgColor.B / 255f, (float)bgColor.A / 255f);
            _fgColor.SetValue((float)fgColor.R / 255f, (float)fgColor.G / 255f, (float)fgColor.B / 255f, (float)fgColor.A / 255f);
        }
    }
}