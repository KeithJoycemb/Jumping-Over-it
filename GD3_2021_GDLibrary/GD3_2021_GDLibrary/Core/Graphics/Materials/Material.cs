using System;

namespace GDLibrary.Graphics
{
    public abstract class Material : IDisposable, ICloneable
    {
        #region Fields

        protected string name;
        protected bool isOpaque = true;
        protected float alpha = 1;
        protected Shader shader;

        #endregion Fields

        #region Properties

        public string Name { get => name; set => name = value.Trim(); }
        public bool IsOpaque { get => isOpaque; }

        public float Alpha
        {
            get => alpha;
            set
            {
                alpha = value >= 0 && value <= 1 ? value : 1;
                //if < 1 then we have a semi-transparent object
                if (alpha < 1)
                    isOpaque = false;
            }
        }

        public Shader Shader { get => shader; set => shader = value; }

        #endregion Properties

        #region Constructors

        public Material(string name)
        {
            Name = name;
            Alpha = 1;
        }

        #endregion Constructors

        #region Actions - Housekeeeping

        public virtual void Dispose()
        {
            //Overridden in child classes
        }

        public abstract object Clone();

        #endregion Actions - Housekeeeping
    }
}