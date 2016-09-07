//MIT 2016, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    abstract class ShaderBase
    {
        protected readonly CanvasToShaderSharedResource _canvasShareResource;
        protected readonly MiniShaderProgram shaderProgram = new MiniShaderProgram();
        public ShaderBase(CanvasToShaderSharedResource canvasShareResource)
        {
            _canvasShareResource = canvasShareResource;
        }
        /// <summary>
        /// set as current shader
        /// </summary>
        protected void SetCurrent()
        {
            if (_canvasShareResource._currentShader != this)
            {
                shaderProgram.UseProgram();
                _canvasShareResource._currentShader = this;
                this.OnSwithToThisShader();
            }
        }
        protected virtual void OnSwithToThisShader()
        {
        }
    }
}