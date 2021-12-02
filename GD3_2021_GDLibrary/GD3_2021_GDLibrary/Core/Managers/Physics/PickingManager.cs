using GDLibrary.Components;
using GDLibrary.Core;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Managers
{
    public class PickingManager : PausableGameComponent
    {
        private float pickStartDistance;
        private float pickEndDistance;
        private Predicate<GameObject> collisionPredicate;
        private GameObject pickedObject;

        public PickingManager(Game game,
           float pickStartDistance, float pickEndDistance,
           Predicate<GameObject> collisionPredicate)
           : base(game)
        {
            this.pickStartDistance = pickStartDistance;
            this.pickEndDistance = pickEndDistance;
            this.collisionPredicate = collisionPredicate;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsUpdated)
                HandleMouse(gameTime);

            base.Update(gameTime);
        }

        protected virtual void HandleMouse(GameTime gameTime)
        {
            if (Input.Mouse.WasJustClicked(Inputs.MouseButton.Left))
                GetPickedObject();
        }

        private void GetPickedObject()
        {
            Vector3 pos;
            Vector3 normal;

            pickedObject =
                Input.Mouse.GetPickedObject(Camera.Main,
                pickStartDistance, pickEndDistance,
                out pos, out normal) as GameObject;

            if (collisionPredicate(pickedObject))
            {
                var behaviour = pickedObject.GetComponent<PickupBehaviour>();

                System.Diagnostics.Debug.WriteLine($"{behaviour.Desc} " +
                    $"- {behaviour.Value}");

                ////OnRemove
                ////OnPlay3D/2D

                //object[] parameters = { pickedObject };
                ////OnPickup
                //EventDispatcher.Raise(new EventData(
                //    EventCategoryType.GameObject,
                //    EventActionType.OnRemoveObject,
                //    parameters));

                //System.Diagnostics.Debug.WriteLine($"{pickedObject.Name} - {pickedObject.ID}");
            }
        }
    }
}