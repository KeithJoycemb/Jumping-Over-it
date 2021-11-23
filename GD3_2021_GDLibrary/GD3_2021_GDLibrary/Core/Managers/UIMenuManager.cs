using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Stores a dictionary of MENU-SPECIFIC ui scenes and updates and draws the currently active scene
    /// </summary>
    public class UIMenuManager : UISceneManager
    {
        public UIMenuManager(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
        }

        public override void Update(GameTime gameTime)
        {
            //loop through all the ui objects in the active scene

            //test if the mouse is over the ui button object

            //react if mouse over and/or mouse click over ui button

            base.Update(gameTime);
        }
    }
}