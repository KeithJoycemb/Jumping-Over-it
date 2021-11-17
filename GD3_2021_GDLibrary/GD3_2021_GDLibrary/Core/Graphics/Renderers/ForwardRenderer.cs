using GDLibrary.Components;
using GDLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Renderers
{
    /// <summary>
    /// Renders the scene using a forward lighting technique and related effects
    /// </summary>
    public class ForwardRenderer : IRenderScene
    {
        //temps used in Render
        private Renderer renderer;
        private Material material;
        private Shader shader;
        private Scene scene;

        public virtual void Render(GraphicsDevice graphicsDevice, Camera camera)
        {
            //until first update this will be null - then in SceneManager the activeScene will be set
            if (scene == null)
                scene = Application.SceneManager.ActiveScene;

            //set depth and blend state

            //set viewport
            graphicsDevice.Viewport = camera.Viewport;

            //render game objects
            var length = scene.Renderers.Count;

            for (var i = 0; i < length; i++)
            {
                renderer = scene.Renderers[i];
                material = renderer.Material;

                if (material == null)
                    throw new NullReferenceException("This game object has no material set for its renderer!");

                //access the shader (e.g. BasicEffect) for this rendered game object
                shader = material.Shader;

                //Set View and Projection
                shader.PrePass(camera);

                //Set World matrix
                shader.Pass(renderer);

                //draw scene contents
                renderer.Draw(graphicsDevice);
            }

            //render post processing

            //render ui
        }
    }
}