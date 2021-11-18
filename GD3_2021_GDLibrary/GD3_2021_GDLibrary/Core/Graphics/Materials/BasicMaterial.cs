using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class BasicMaterial : Material
    {
        #region Fields

        protected Vector3 diffuseColor;
        protected Texture2D diffuseTexture;

        #endregion Fields

        #region Properties

        public Vector3 DiffuseColor { get => diffuseColor; set => diffuseColor = value; }
        public Texture2D Texture { get => diffuseTexture; set => diffuseTexture = value; }

        #endregion Properties

        #region Constructors

        public BasicMaterial(string name) : base(name)
        {
            DiffuseColor = Color.White.ToVector3();
        }

        #endregion Constructors

        public override object Clone()
        {
            var clone = new BasicMaterial($"Clone - {name}");
            clone.diffuseColor = diffuseColor; //deep
            clone.diffuseTexture = diffuseTexture;  //shallow
            clone.shader = shader; //shallow
            return clone;
        }
    }
}