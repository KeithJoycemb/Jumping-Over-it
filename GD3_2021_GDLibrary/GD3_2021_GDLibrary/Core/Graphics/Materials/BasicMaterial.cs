using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class BasicMaterial : Material
    {
        #region Fields

        protected Vector3 diffuseColor;
        protected Texture2D texture;

        #endregion Fields

        #region Properties

        public Vector3 DiffuseColor { get => diffuseColor; set => diffuseColor = value; }
        public Texture2D Texture { get => texture; set => texture = value; }

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
            clone.texture = texture;  //shallow
            clone.shader = shader; //shallow
            return clone;
        }
    }
}