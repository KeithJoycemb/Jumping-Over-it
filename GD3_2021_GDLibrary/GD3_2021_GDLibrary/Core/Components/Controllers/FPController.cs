using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Components
{
    /// <summary>
    /// Adds simple 1st person controller to camera using keyboard and mouse input
    /// </summary>
    public class FPController : Controller
    {
        protected Vector3 translation = Vector3.Zero;
        protected Vector3 rotation = Vector3.Zero;
        private float lastX;
        private float lastY;
        private float moveSpeed = 0.05f;
        private float strafeSpeed = 0.025f;
        private float rotationSpeed = 0.00009f;
        private float sensitivity = 100;
        private bool autoMove = false;
        public FPController(float moveSpeed, float strafeSpeed, float rotationSpeed)
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
            if (autoMove)
            {
                autoMove = false;
                lastX = Input.Mouse.Delta.X;
                lastY = Input.Mouse.Delta.Y;
            }
            else
            {
                rotation = Vector3.Zero;
                rotation.Y -= (Input.Mouse.Delta.X - lastX) * rotationSpeed * sensitivity * Time.Instance.DeltaTimeMs;
                rotation.X -= (Input.Mouse.Delta.Y - lastY) * rotationSpeed * sensitivity * Time.Instance.DeltaTimeMs;
                transform.Rotate(ref rotation);  //converts value type to a reference
                lastX = Input.Mouse.Delta.X;
                lastY = Input.Mouse.Delta.Y;
            }
                if (Input.Mouse.X > 1917 || Input.Mouse.X < 3)
                {
                    Input.Mouse.Position = new Vector2(960, 540);
                    autoMove = true;
                }
                
        }

        #region Unused

        protected override void HandleGamepadInput()
        {
        }

        #endregion Unused
    }
}