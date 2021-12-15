using GDLibrary;
using GDLibrary.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GDApp
{
    /// <summary>
    /// This component will check for win/lose logic in the game
    /// </summary>
    public class MyStateManager : PausableGameComponent
    {
        public List<string> inventory;
        private float timeSinceLastStateCheck;

        public MyStateManager(Game game)
            : base(game)
        {
            inventory = new List<string>();
        }

        protected override void SubscribeToEvents()
        {
            //add more events here...
            EventDispatcher.Subscribe(EventCategoryType.Player,
                HandleEvent);

            EventDispatcher.Subscribe(EventCategoryType.Inventory,
                HandleInventory);

            //dont forget to call base otherwise no play/pause support
            base.SubscribeToEvents();
        }

        private void HandleInventory(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnAddInventory)
            {
                //add something to list
            }
            else if (eventData.EventActionType == EventActionType.OnRemoveInventory)
            {
                //remove something from list
            }
        }

        protected override void HandleEvent(EventData eventData)
        {
            //add more event handlers here...
            if (eventData.EventActionType == EventActionType.OnPlay)
            {
                EventDispatcher.Raise(new EventData(EventCategoryType.Menu, EventActionType.OnPlay));
                object[] parameters = { "backgroundMusic" };
                EventDispatcher.Raise(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, parameters));
            }
            //dont forget to call base otherwise no play/pause support
            base.HandleEvent(eventData);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceLastStateCheck += Time.Instance.DeltaTimeMs;

            if (timeSinceLastStateCheck >= 500)
            {
                timeSinceLastStateCheck -= 500;

                //do we need to periodically check something like win/lose state?
                if (inventory.Contains("key"))
                {
                }

                if (inventory.Count == 5)
                {
                    //play sound
                    //change camera Application.SceneManager.ActiveScene.SetMainCamera("dfdf");
                    //event for ui element?
                    //event for showing main menu
                }
            }

            base.Update(gameTime);
        }
    }
}