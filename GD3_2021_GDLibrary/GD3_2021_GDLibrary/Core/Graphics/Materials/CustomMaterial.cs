using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class CustomMaterial : BasicMaterial
    {
        #region Fields

        protected Vector2 tiling;

        protected Texture2D normalTexture;

        #endregion Fields

        #region Properties

        public Vector2 Tiling { get => tiling; set => tiling = value; }
        public Texture2D NormalTexture { get => normalTexture; set => normalTexture = value; }

        #endregion Properties

        #region Constructors

        public CustomMaterial(string name) : base(name)
        {
        }

        #endregion Constructors

        public override object Clone()
        {
            var clone = new CustomMaterial($"Clone - {name}");
            clone.DiffuseColor = diffuseColor;
            clone.Texture = diffuseTexture;
            clone.tiling = tiling;
            clone.normalTexture = normalTexture;
            clone.shader = shader;
            return clone;
        }
    }
}