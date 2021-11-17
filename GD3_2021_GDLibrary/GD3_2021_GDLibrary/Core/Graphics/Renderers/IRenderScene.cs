using GDLibrary.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Renderers
{
    public interface IRenderScene
    {
        public void Render(GraphicsDevice graphicsDevice, Camera camera);
    }
}