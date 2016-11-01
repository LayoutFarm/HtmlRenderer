//MIT 2016, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class InvertAlphaLineSmoothShader : ShaderBase
    {
        //for stencil buffer ***

        ShaderVtxAttrib4f a_position;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar4 u_solidColor;
        ShaderUniformVar1 u_linewidth;
        Drawing.Color _strokeColor;
        float _strokeWidth = 0.5f;
        public InvertAlphaLineSmoothShader(CanvasToShaderSharedResource canvasShareResource)
             : base(canvasShareResource)
        {
            //-------------------------------------------------------------------------------
            string vs = @"                   
            attribute vec4 a_position;    

            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor; 
            uniform float u_linewidth;

            varying vec4 v_color; 
            varying float v_distance;
            varying float p0;
            
            void main()
            {   
                
                float rad = a_position[3];
                v_distance= a_position[2];

                float n_x = sin(rad); 
                float n_y = cos(rad);  

                vec4 delta;
                if(v_distance <1.0){                                         
                    delta = vec4(-n_x * u_linewidth,n_y * u_linewidth,0,0);                       
                }else{                      
                    delta = vec4(n_x * u_linewidth,-n_y * u_linewidth,0,0);
                }
    
                if(u_linewidth <= 0.5){
                    p0 = 0.5;      
                }else if(u_linewidth <=1.0){
                    p0 = 0.45;  
                }else if(u_linewidth>1.0 && u_linewidth<3.0){
                    
                    p0 = 0.25;  
                }else{
                    p0= 0.1;
                }
                
                vec4 pos = vec4(a_position[0],a_position[1],0,1) + delta;                 
                gl_Position = u_mvpMatrix* pos;                

                v_color= u_solidColor;
            }
            ";
            //fragment source
            //this is invert fragment shader *** 
            //so we 
            string fs = @"
                precision mediump float;
                varying vec4 v_color;  
                varying float v_distance;
                varying float p0;                
                void main()
                {
                    float d0= v_distance; 
                    float p1= 1.0-p0;
                    float factor= 1.0 /p0;
            
                    if(d0 < p0){                        
                        gl_FragColor = vec4(v_color[0],v_color[1],v_color[2], 1.0 -(v_color[3] *(d0 * factor)));
                    }else if(d0> p1){                         
                        gl_FragColor= vec4(v_color[0],v_color[1],v_color[2],1.0-(v_color[3] *((1.0-d0)* factor)));
                    }
                    else{ 
                       gl_FragColor = vec4(0,0,0,0);                        
                    } 
                }
            ";
            //---------------------
            if (!shaderProgram.Build(vs, fs))
            {
                return;
            }
            //-----------------------

            a_position = shaderProgram.GetAttrV4f("a_position");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
            u_linewidth = shaderProgram.GetUniform1("u_linewidth");
            _strokeColor = Drawing.Color.Black;
        }
        int orthoviewVersion = -1;
        void CheckViewMatrix()
        {
            int version = 0;
            if (orthoviewVersion != (version = _canvasShareResource.OrthoViewVersion))
            {
                orthoviewVersion = version;
                u_matrix.SetData(_canvasShareResource.OrthoView.data);
            }
        }

        public void DrawTriangleStrips(float[] coords, int ncount)
        {
            SetCurrent();
            CheckViewMatrix();
            //-----------------------------------
            u_solidColor.SetValue(
                  _strokeColor.R / 255f,
                  _strokeColor.G / 255f,
                  _strokeColor.B / 255f,
                  _strokeColor.A / 255f);
            a_position.LoadPureV4f(coords);
            u_linewidth.SetValue(_strokeWidth);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, ncount);
        }
    }
}