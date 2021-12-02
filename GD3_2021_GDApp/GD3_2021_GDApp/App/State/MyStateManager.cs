using GDLibrary.Core;
using Microsoft.Xna.Framework;

namespace GDApp.App.State
{
    /// <summary>
    /// This component will check for win/lose logic in the game
    /// </summary>
    public class MyStateManager : PausableGameComponent
    {
        //      private List<string> inventory;

        public MyStateManager(Game game)
            : base(game)
        {
        }

        protected override void SubscribeToEvents()
        {
            //add more events here...
            EventDispatcher.Subscribe(EventCategoryType.Player,
                HandleEvent);

            //dont forget to call base otherwise no play/pause support
            base.SubscribeToEvents();
        }

        protected override void HandleEvent(EventData eventData)
        {
            //add more event handlers here...
            if (eventData.EventActionType == EventActionType.OnPickup)
            {
                var objectName = eventData.Parameters[0] as string;
            }
            //dont forget to call base otherwise no play/pause support
            base.HandleEvent(eventData);
        }

        public override void Update(GameTime gameTime)
        {
            //do we need to periodically check something like win/lose state?

            base.Update(gameTime);
        }
    }
}