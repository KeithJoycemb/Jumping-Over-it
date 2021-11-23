using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Components
{
    /// <summary>
    /// Adds simple 1st person controller to camera using keyboard and mouse input
    /// </summary>
    public class FirstPersonController : Controller
    {
        protected Vector3 translation = Vector3.Zero;
        protected Vector3 rotation = Vector3.Zero;
        private float moveSpeed = 0.05f;
        private float strafeSpeed = 0.025f;
        private float rotationSpeed = 0.00009f;

        public FirstPersonController(float moveSpeed, float strafeSpeed, float rotationSpeed)
        {
            this.moveSpeed = moveSpeed;
            this.strafeSpeed = strafeSpeed;
            this.rotationSpeed = rotationSpeed;
        }
        public override void Update()
        {
            HandleInputs();
        }

        protected override void HandleInputs()
        {
            HandleMouseInput();
            HandleKeyboardInput();
            //    HandleGamepadInput(); //not using for this controller implementation
            //   base.Update(); //nothing happens so dont call this
        }

        protected override void HandleKeyboardInput()
        {
            translation = Vector3.Zero;

            if (Input.Keys.IsPressed(Keys.W))
                translation += transform.Forward * moveSpeed * Time.Instance.DeltaTimeMs;
            else if (Input.Keys.IsPressed(Keys.S))
                translation -= transform.Forward * moveSpeed * Time.Instance.DeltaTimeMs;

            if (Input.Keys.IsPressed(Keys.A))
                translation += transform.Left * strafeSpeed * Time.Instance.DeltaTimeMs;
            else if (Input.Keys.IsPressed(Keys.D))
                translation += transform.Right * strafeSpeed * Time.Instance.DeltaTimeMs;

            transform.Translate(ref translation);
        }

        protected override void HandleMouseInput()
        {
            rotation = Vector3.Zero;
            rotation.Y -= Input.Mouse.Delta.X * rotationSpeed * Time.Instance.DeltaTimeMs;
            rotation.X -= Input.Mouse.Delta.Y * rotationSpeed * Time.Instance.DeltaTimeMs;
            transform.Rotate(ref rotation);  //converts value type to a reference
        }

        #region Unused

        protected override void HandleGamepadInput()
        {
        }

        #endregion Unused
    }
}