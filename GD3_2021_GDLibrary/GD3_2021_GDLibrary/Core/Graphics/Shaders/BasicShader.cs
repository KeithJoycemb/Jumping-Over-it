using GDLibrary.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class BasicShader : Shader
    {
        #region Fields

        protected EffectParameter diffuseColorParameter;
        protected EffectParameter textureEnabledParameter;
        protected EffectParameter textureParameter;
        protected EffectParameter alphaParameter;

        //temp
        private BasicEffect basicEffect;

        #endregion Fields

        #region Constructors

        public BasicShader() : base()
        {
        }

        #endregion Constructors

        #region Initialization

        public override void LoadEffect()
        {
            effect = new BasicEffect(Application.GraphicsDevice);

            //TODO - remove this lazy code - just here to demo lighting
            //(effect as BasicEffect).LightingEnabled = true;
            //(effect as BasicEffect).EnableDefaultLighting();
        }

        #endregion Initialization

        #region Actions - Pass

        public override void PrePass(Camera camera)
        {
            basicEffect = effect as BasicEffect;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;
        }

        public override void Pass(Renderer renderer)
        {
            //access the game objects material (i.e. diffuse color, alpha, texture)
            var material = renderer.Material as BasicMaterial;

            //set color
            basicEffect.DiffuseColor = material.DiffuseColor;

            //set transparency
            basicEffect.Alpha = material.Alpha;

            //TODO - add bool to material to disable/enable texture
            basicEffect.TextureEnabled = true;

            //set texture
            basicEffect.Texture = material.Texture;

            //set ambient
            basicEffect.AmbientLightColor = ambientLightColor;

            //set world for game object
            basicEffect.World = renderer.Transform.WorldMatrix;

            //TODO - add support for multiple passes
            //set pass
            effect.CurrentTechnique.Passes[0].Apply();
        }

        #endregion Actions - Pass
    }
}