using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Components
{
    public class OLD_FirstPersonCameraController //: Controller
    {
        //private float moveSpeedPerSecond = 3;
        //private float strafeSpeedPerSecond = 2; //you can never strafe as fast as you move
        //private float rotateDegreesPerSecond = 0.1f; //4 secs = a full rotation
        //private Keys[] moveKeys;
        //private Keys[] turnKeys;

        //public FirstPersonCameraController(Keys[] moveKeys, Keys[] turnKeys)
        //{
        //    this.moveKeys = moveKeys;
        //    this.turnKeys = turnKeys;
        //}
        //public override void Update()
        //{
        //    //forward/backward
        //    if (Input.Keys.IsPressed(moveKeys[0]))
        //    {
        //        transform.Translate(transform.Forward * moveSpeedPerSecond
        //            * Time.Time.Instance.DeltaTimeMs / 1000.0f);
        //    }
        //    else if (Input.Keys.IsPressed(moveKeys[1]))
        //    {
        //        transform.Translate(-transform.Forward * moveSpeedPerSecond * Time.Time.Instance.DeltaTimeMs / 1000.0f);
        //    }

        //    //strafe left/right
        //    if (Input.Keys.IsPressed(moveKeys[2]))
        //    {
        //        transform.Translate(transform.Left * strafeSpeedPerSecond * Time.Time.Instance.DeltaTimeMs / 1000.0f);
        //    }
        //    else if (Input.Keys.IsPressed(moveKeys[3]))
        //    {
        //        transform.Translate(transform.Right * strafeSpeedPerSecond * Time.Time.Instance.DeltaTimeMs / 1000.0f);
        //    }

        //    //rotate left/right
        //    //if (Input.Keys.IsPressed(turnKeys[0]))
        //    //{
        //    //    transform.Rotate(transform.Up * rotateDegreesPerSecond
        //    //        * Time.Time.Instance.DeltaTimeMs / 1000.0f);
        //    //}
        //    //else if (Input.Keys.IsPressed(turnKeys[1]))
        //    //{
        //    //    transform.Rotate(-transform.Up * rotateDegreesPerSecond
        //    //        * Time.Time.Instance.DeltaTimeMs / 1000.0f);
        //    //}

        //    Vector2 mouseDelta = Input.Mouse.GetDeltaFromCentre(new Vector2(512, 384)); //REFACTOR - NMCG
        //    mouseDelta *= rotateDegreesPerSecond * Time.Time.Instance.DeltaTimeMs;

        //    if (mouseDelta.Length() != 0)
        //    {
        //        transform.SetRotation(new Vector3(mouseDelta, 0));
        //    }

        //    //do we need to call the base method? does it actually do anything?
        //    // base.Update();
        //}
    }
}