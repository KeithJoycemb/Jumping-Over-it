using GDLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Components
{
    /// <summary>
    /// Draws a user-defined array of VertexPositionNormalTexture vertices
    /// </summary>
    public class MeshRenderer : Renderer
    {
        //TODO - generalise for any IVertexType
        protected Mesh mesh;

        //TODO - If mesh data is changed then we must re-set the data in the buffers. Same for ModelRenderer too.
        public Mesh Mesh
        {
            get
            {
                return mesh;
            }

            set
            {
                if (value != mesh && value != null)
                {
                    mesh = value;
                    //TODO - update if transform changes
                    //BUG - on clone
                    //  SetBoundingVolume();
                }
            }
        }

        public override void Draw(GraphicsDevice device)
        {
            device.SetVertexBuffer(mesh.VertexBuffer);
            device.Indices = mesh.IndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                0, 0, mesh.IndexBuffer.IndexCount / 3);
        }

        public override void SetBoundingVolume()
        {
            if (mesh == null)
                throw new NullReferenceException("No mesh has been set for this renderer!");

            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var vertices = mesh.GetVertices();

            foreach (var vertex in vertices)
            {
                min.X = Math.Min(vertex.X, min.X);
                min.Y = Math.Min(vertex.Y, min.Y);
                min.Z = Math.Min(vertex.Z, min.Z);
                max.X = Math.Max(vertex.X, max.X);
                max.Y = Math.Max(vertex.Y, max.Y);
                max.Z = Math.Max(vertex.Z, max.Z);
            }

            boundingBox.Min = min;
            boundingBox.Max = max;

            var dx = max.X - min.X;
            var dy = max.Y - min.Y;
            var dz = max.Z - min.Z;

            boundingSphere.Radius = (float)Math.Max(Math.Max(dx, dy), dz) / 2.0f;
            boundingSphere.Center = transform.LocalTranslation;
        }

        #region Actions - Housekeeping

        //TODO - Dispose

        public override object Clone()
        {
            var clone = new MeshRenderer();
            clone.material = material.Clone() as Material;
            clone.Mesh = mesh.Clone() as Mesh;
            return clone;
        }

        #endregion Actions - Housekeeping
    }
}