//#define DEMO

using GDLibrary;
using GDLibrary.Collections;
using GDLibrary.Components;
using GDLibrary.Components.UI;
using GDLibrary.Core;
using GDLibrary.Core.Demo;
using GDLibrary.Graphics;
using GDLibrary.Inputs;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using GDLibrary.Renderers;
using GDLibrary.Utilities;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace GDApp
{
    public class Main : Game
    {
        #region Fields

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Stores and updates all scenes (which means all game objects i.e. players, cameras, pickups, behaviours, controllers)
        /// </summary>
        private SceneManager sceneManager;

        /// <summary>
        /// Draws all game objects with an attached and enabled renderer
        /// </summary>
        private RenderManager renderManager;

        /// <summary>
        /// Updates and Draws all ui objects
        /// </summary>
        private UISceneManager uiSceneManager;

        /// <summary>
        /// Updates and Draws all menu objects
        /// </summary>
        private MyMenuManager uiMenuManager;

        /// <summary>
        /// Plays all 2D and 3D sounds
        /// </summary>
        private SoundManager soundManager;

        private MyStateManager stateManager;
        private PickingManager pickingManager;

        /// <summary>
        /// Handles all system wide events between entities
        /// </summary>
        private EventDispatcher eventDispatcher;

        /// <summary>
        /// Applies physics to all game objects with a Collider
        /// </summary>
        private PhysicsManager physicsManager;

        /// <summary>
        /// Quick lookup for all textures used within the game
        /// </summary>
        private Dictionary<string, Texture2D> textureDictionary;

        /// <summary>
        /// Quick lookup for all fonts used within the game
        /// </summary>
        private ContentDictionary<SpriteFont> fontDictionary;

        /// <summary>
        /// Quick lookup for all models used within the game
        /// </summary>
        private ContentDictionary<Model> modelDictionary;

        //temps
        private Scene activeScene;

        private UITextObject nameTextObj;
        private Collider collider;

        #endregion Fields

        /// <summary>
        /// Construct the Game object
        /// </summary>
        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Set application data, input, title and scene manager
        /// </summary>
        private void InitializeEngine(string gameTitle, int width, int height)
        {
            //set game title
            Window.Title = gameTitle;

            //the most important element! add event dispatcher for system events
            eventDispatcher = new EventDispatcher(this);

            //add physics manager to enable CD/CR and physics
            physicsManager = new PhysicsManager(this);

            //instanciate scene manager to store all scenes
            sceneManager = new SceneManager(this);

            //create the ui scene manager to update and draw all ui scenes
            uiSceneManager = new UISceneManager(this, _spriteBatch);

            //create the ui menu manager to update and draw all menu scenes
            uiMenuManager = new MyMenuManager(this, _spriteBatch);

            //add support for playing sounds
            soundManager = new SoundManager(this);

            //this will check win/lose logic
            stateManager = new MyStateManager(this);

            //picking support using physics engine
            //this predicate lets us say ignore all the other collidable objects except interactables and consumables
            Predicate<GameObject> collisionPredicate =
                (collidableObject) =>
                {
                    if (collidableObject != null)
                        return collidableObject.GameObjectType
                        == GameObjectType.Interactable
                        || collidableObject.GameObjectType == GameObjectType.Consumable;

                    return false;
                };
            pickingManager = new PickingManager(this, 2, 100, collisionPredicate);

            //initialize global application data
            Application.Main = this;
            Application.Content = Content;
            Application.GraphicsDevice = _graphics.GraphicsDevice;
            Application.GraphicsDeviceManager = _graphics;
            Application.SceneManager = sceneManager;
            Application.PhysicsManager = physicsManager;
            Application.StateManager = stateManager;

            //instanciate render manager to render all drawn game objects using preferred renderer (e.g. forward, backward)
            renderManager = new RenderManager(this, new ForwardRenderer(), false, true);

            //instanciate screen (singleton) and set resolution etc
            Screen.GetInstance().Set(width, height, true, true);

            //instanciate input components and store reference in Input for global access
            Input.Keys = new KeyboardComponent(this);
            Input.Mouse = new MouseComponent(this);
            Input.Mouse.Position = Screen.Instance.ScreenCentre;
            Input.Gamepad = new GamepadComponent(this);

            //************* add all input components to component list so that they will be updated and/or drawn ***********/

            //add event dispatcher
            Components.Add(eventDispatcher);

            //add time support
            Components.Add(Time.GetInstance(this));

            //add input support
            Components.Add(Input.Keys);
            Components.Add(Input.Mouse);
            Components.Add(Input.Gamepad);

            //add physics manager to enable CD/CR and physics
            Components.Add(physicsManager);

            //add support for picking using physics engine
            Components.Add(pickingManager);

            //add scene manager to update game objects
            Components.Add(sceneManager);

            //add render manager to draw objects
            Components.Add(renderManager);

            //add ui scene manager to update and drawn ui objects
            Components.Add(uiSceneManager);

            //add ui menu manager to update and drawn menu objects
            Components.Add(uiMenuManager);

            //add sound
            Components.Add(soundManager);

            //add state
            Components.Add(stateManager);
        }

        /// <summary>
        /// Not much happens in here as SceneManager, UISceneManager, MenuManager and Inputs are all GameComponents that automatically Update()
        /// Normally we use this to add some temporary demo code in class - Don't forget to remove any temp code inside this method!
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {

            #region jumping sounds
            if (Input.Keys.WasJustPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {

                Random rnd = new Random();
                int rand = rnd.Next(1, 3);   // creates a number between 1 and 6

                if (rand == 1)
                {
                    object[] parameters = { "jump1" };
                    EventDispatcher.Raise(new EventData(EventCategoryType.Sound,
                        EventActionType.OnPlay2D, parameters));
                }
                else if (rand == 2)
                {
                    object[] parameters = { "jump2" };
                    EventDispatcher.Raise(new EventData(EventCategoryType.Sound,
                        EventActionType.OnPlay2D, parameters));
                }
                else if (rand == 3)
                {
                    object[] parameters = { "jump3" };
                    EventDispatcher.Raise(new EventData(EventCategoryType.Sound,
                        EventActionType.OnPlay2D, parameters));
                }
                rand = rnd.Next(1, 100);
                if (rand <= 8)
                {
                    if (rand <= 4)
                    {
                        object[] parameters = { "croak1" };
                        EventDispatcher.Raise(new EventData(EventCategoryType.Sound,
                            EventActionType.OnPlay2D, parameters));
                    }
                    else
                    {
                        object[] parameters = { "croak2" };
                        EventDispatcher.Raise(new EventData(EventCategoryType.Sound,
                            EventActionType.OnPlay2D, parameters));
                    }

                }

            }

            if (Input.Keys.WasJustPressed(Microsoft.Xna.Framework.Input.Keys.C))
                Application.SceneManager.ActiveScene.CycleCameras();

            base.Update(gameTime);
            #endregion

            #region Walking Sounds

            if (Input.Keys.WasJustPressed(Microsoft.Xna.Framework.Input.Keys.W))
            {
                object[] parameters = { "walking" };
                EventDispatcher.Raise(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, parameters));
            }

            if (Input.Keys.WasJustPressed(Microsoft.Xna.Framework.Input.Keys.A))
            {
                object[] parameters = { "walking" };
                EventDispatcher.Raise(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, parameters));
            }

            if (Input.Keys.WasJustPressed(Microsoft.Xna.Framework.Input.Keys.S))
            {
                object[] parameters = { "walking" };
                EventDispatcher.Raise(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, parameters));
            }

            if (Input.Keys.WasJustPressed(Microsoft.Xna.Framework.Input.Keys.D))
            {
                object[] parameters = { "walking" };
                EventDispatcher.Raise(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, parameters));
            }

            #endregion Walking sounds


        }

        /// <summary>
        /// Not much happens in here as RenderManager, UISceneManager and MenuManager are all DrawableGameComponents that automatically Draw()
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        /******************************** Student Project-specific ********************************/
        /******************************** Student Project-specific ********************************/
        /******************************** Student Project-specific ********************************/

        #region Student/Group Specific Code

        /// <summary>
        /// Initialize engine, dictionaries, assets, level contents
        /// </summary>
        protected override void Initialize()
        {
            //move here so that UISceneManager can use!
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //data, input, scene manager
            InitializeEngine(AppData.GAME_TITLE_NAME,
                AppData.GAME_RESOLUTION_WIDTH,
                AppData.GAME_RESOLUTION_HEIGHT);

            //load structures that store assets (e.g. textures, sounds) or archetypes (e.g. Quad game object)
            InitializeDictionaries();

            //load assets into the relevant dictionary
            LoadAssets();

            //level with scenes and game objects
            InitializeLevel();

            //add menu and ui
            InitializeUI();

            //TODO - remove hardcoded mouse values - update Screen class to centre the mouse with hardcoded value - remove later
            Input.Mouse.Position = Screen.Instance.ScreenCentre;

            //turn on/off debug info
            InitializeDebugUI(false,false);

            //to show the menu we must start paused for everything else!
            EventDispatcher.Raise(new EventData(EventCategoryType.Menu, EventActionType.OnPause));

            base.Initialize();
        }

        /******************************* Load/Unload Assets *******************************/

        private void InitializeDictionaries()
        {
            textureDictionary = new Dictionary<string, Texture2D>();

            //why not try the new and improved ContentDictionary instead of a basic Dictionary?
            fontDictionary = new ContentDictionary<SpriteFont>();
            modelDictionary = new ContentDictionary<Model>();
        }

        private void LoadAssets()
        {
            LoadModels();
            LoadTextures();
            LoadSounds();
            LoadFonts();
        }

        /// <summary>
        /// Load models to dictionary
        /// </summary>
        private void LoadModels()
        {
            //notice with the ContentDictionary we dont have to worry about Load() or a name (its assigned from pathname)
            modelDictionary.Add("Assets/Models/sphere");
            modelDictionary.Add("Assets/Models/cube");
            modelDictionary.Add("Assets/Models/teapot");
            modelDictionary.Add("Assets/Models/monkey1");
            modelDictionary.Add("Assets/Models/Frog");
            modelDictionary.Add("Assets/Models/Tree");
            modelDictionary.Add("Assets/Models/TreeEverGreen");
            modelDictionary.Add("Assets/Models/levelForImport");
            modelDictionary.Add("Assets/Models/rock1");
            modelDictionary.Add("Assets/Models/rock2");
            modelDictionary.Add("Assets/Models/platform");
            modelDictionary.Add("Assets/Models/SIGNFINISHED");
            modelDictionary.Add("Assets/Models/crown");
            modelDictionary.Add("Assets/Models/frog");
        }

        /// <summary>
        /// Load fonts to dictionary
        /// </summary>
        private void LoadFonts()
        {
            fontDictionary.Add("Assets/Fonts/ui");
            fontDictionary.Add("Assets/Fonts/menu");
            fontDictionary.Add("Assets/Fonts/debug");
        }

        /// <summary>
        /// Load sound data used by sound manager
        /// </summary>
        private void LoadSounds()
        {
            var soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/smokealarm1");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "smokealarm",
                soundEffect,
                SoundCategoryType.Alarm,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/jumping1");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "jump1",
                soundEffect,
                SoundCategoryType.Jump,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/jumping2");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "jump2",
                soundEffect,
                SoundCategoryType.Jump,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/jumping3");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "jump3",
                soundEffect,
                SoundCategoryType.Jump,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/jumping4");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "jump4",
                soundEffect,
                SoundCategoryType.Jump,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/fall1");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "fall1",
                soundEffect,
                SoundCategoryType.Fall,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/ambience");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "ambience",
                soundEffect,
                SoundCategoryType.Ambience,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/croak1");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "croak1",
                soundEffect,
                SoundCategoryType.Jump,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/Frog-sound-ribbit");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "croak2",
                soundEffect,
                SoundCategoryType.Jump,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/BackGroundMusic");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "backgroundMusic",
                soundEffect,
                SoundCategoryType.Ambience,
                new Vector3(1, 0, 0),
                false));

            soundEffect = Content.Load<SoundEffect>("Assets/Sounds/Effects/walking");
            //add the new sound effect
            soundManager.Add(new GDLibrary.Managers.Cue(
                "walking",
                soundEffect,
                SoundCategoryType.Ambience,
                new Vector3(1, 0, 0),
                false));


        }

        /// <summary>
        /// Load texture data from file and add to the dictionary
        /// </summary>
        private void LoadTextures()
        {

            //debug
            textureDictionary.Add("checkerboard", Content.Load<Texture2D>("Assets/Demo/Textures/checkerboard"));
            textureDictionary.Add("mona lisa", Content.Load<Texture2D>("Assets/Demo/Textures/mona lisa"));

            //skybox
            textureDictionary.Add("skybox_front", Content.Load<Texture2D>("Assets/Textures/Skybox/front"));
            textureDictionary.Add("skybox_left", Content.Load<Texture2D>("Assets/Textures/Skybox/left"));
            textureDictionary.Add("skybox_right", Content.Load<Texture2D>("Assets/Textures/Skybox/right"));
            textureDictionary.Add("skybox_back", Content.Load<Texture2D>("Assets/Textures/Skybox/back"));
            textureDictionary.Add("skybox_sky", Content.Load<Texture2D>("Assets/Textures/Skybox/sky"));

            //environment
            textureDictionary.Add("grass", Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"));
            textureDictionary.Add("crate1", Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"));
            textureDictionary.Add("grass2", Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass2"));

            //ui
            textureDictionary.Add("ui_progress_32_8", Content.Load<Texture2D>("Assets/Textures/UI/Controls/ui_progress_32_8"));
            textureDictionary.Add("progress_white", Content.Load<Texture2D>("Assets/Textures/UI/Controls/progress_white"));
            textureDictionary.Add("map", Content.Load<Texture2D>("Assets/Textures/UI/Controls/map"));

            //menu
            textureDictionary.Add("mainmenu", Content.Load<Texture2D>("Assets/Textures/UI/Backgrounds/mainmenu"));
            textureDictionary.Add("audiomenu", Content.Load<Texture2D>("Assets/Textures/UI/Backgrounds/audiomenu"));
            textureDictionary.Add("controlsmenu", Content.Load<Texture2D>("Assets/Textures/UI/Backgrounds/controlsmenu"));
            textureDictionary.Add("exitmenuwithtrans", Content.Load<Texture2D>("Assets/Textures/UI/Backgrounds/exitmenuwithtrans"));
            textureDictionary.Add("genericbtn", Content.Load<Texture2D>("Assets/Textures/UI/Progress/genericbtn"));
            textureDictionary.Add("exit", Content.Load<Texture2D>("Assets/Textures/UI/Progress/exit"));

            //models
            textureDictionary.Add("gray", Content.Load<Texture2D>("Assets/Textures/Models/gray"));
            textureDictionary.Add("mountain", Content.Load<Texture2D>("Assets/Textures/Models/mountain"));
            textureDictionary.Add("platform", Content.Load<Texture2D>("Assets/Textures/Models/platform"));
            textureDictionary.Add("crown", Content.Load<Texture2D>("Assets/Textures/Models/crown"));
            textureDictionary.Add("sign", Content.Load<Texture2D>("Assets/Textures/Models/Wood_01_07_low6SG-color"));
            textureDictionary.Add("tree", Content.Load<Texture2D>("Assets/Textures/Models/tree"));
            textureDictionary.Add("frog", Content.Load<Texture2D>("Assets/Textures/Models/frog"));
            textureDictionary.Add("frog1", Content.Load<Texture2D>("Assets/Textures/Models/frog1"));
            textureDictionary.Add("frog2", Content.Load<Texture2D>("Assets/Textures/Models/frog2"));
            textureDictionary.Add("frog3", Content.Load<Texture2D>("Assets/Textures/Models/frog3"));



            //map
            textureDictionary.Add("0", Content.Load<Texture2D>("Assets/Textures/UI/map/0"));
            textureDictionary.Add("1", Content.Load<Texture2D>("Assets/Textures/UI/map/1"));
            textureDictionary.Add("2", Content.Load<Texture2D>("Assets/Textures/UI/map/2"));
            textureDictionary.Add("3", Content.Load<Texture2D>("Assets/Textures/UI/map/3"));
            textureDictionary.Add("4", Content.Load<Texture2D>("Assets/Textures/UI/map/4"));
            textureDictionary.Add("5", Content.Load<Texture2D>("Assets/Textures/UI/map/5"));
            textureDictionary.Add("6", Content.Load<Texture2D>("Assets/Textures/UI/map/6"));
            textureDictionary.Add("7", Content.Load<Texture2D>("Assets/Textures/UI/map/7"));
            textureDictionary.Add("8", Content.Load<Texture2D>("Assets/Textures/UI/map/8"));
            textureDictionary.Add("9", Content.Load<Texture2D>("Assets/Textures/UI/map/9"));
            textureDictionary.Add("10", Content.Load<Texture2D>("Assets/Textures/UI/map/10"));


         
        }

        /// <summary>
        /// Free all asset resources, dictionaries, network connections etc
        /// </summary>
        protected override void UnloadContent()
        {
            //TODO - add graceful dispose for content

            //remove all models used for the game and free RAM
            modelDictionary?.Dispose();
            fontDictionary?.Dispose();

            base.UnloadContent();
        }

        /******************************* UI & Menu *******************************/

        /// <summary>
        /// Create a scene, add content, add to the scene manager, and load default scene
        /// </summary>
        private void InitializeLevel()
        {
            float worldScale = 1000;
            activeScene = new Scene("level 1");

            InitializeCameras(activeScene);

            InitializeSkybox(activeScene, worldScale);

            //remove because now we are interested only in collidable things!
            //InitializeCubes(activeScene);
            //InitializeModels(activeScene);

            InitializeCollidables(activeScene, worldScale);

            sceneManager.Add(activeScene);
            sceneManager.LoadScene("level 1");
        }

        /// <summary>
        /// Adds menu and UI elements
        /// </summary>
        private void InitializeUI()
        {
            InitializeGameMenu();
            InitializeGameUI();
        }

        /// <summary>
        /// Adds main menu elements
        /// </summary>
        private void InitializeGameMenu()
        {
            //a re-usable variable for each ui object
            UIObject menuObject = null;

            #region Main Menu

            /************************** Main Menu Scene **************************/
            //make the main menu scene
            var mainMenuUIScene = new UIScene(AppData.MENU_MAIN_NAME);

            /**************************** Background Image ****************************/

            //main background
            var texture = textureDictionary["mainmenu"];
            //get how much we need to scale background to fit screen, then downsizes a little so we can see game behind background
            var scale = _graphics.GetScaleForTexture(texture,
                new Vector2(1f, 1f));

            menuObject = new UITextureObject("main background",
                UIObjectType.Texture,
                new Transform2D(Screen.Instance.ScreenCentre, scale, 0), //sets position as center of screen
                0,
                new Color(255, 255, 255, 400),
                texture.GetOriginAtCenter(), //if we want to position image on screen center then we need to set origin as texture center
                texture);

            //add ui object to scene
            mainMenuUIScene.Add(menuObject);

            /**************************** Play Button ****************************/

            var btnTexture = textureDictionary["genericbtn"];
            var sourceRectangle
                = new Microsoft.Xna.Framework.Rectangle(0, 0,
                btnTexture.Width, btnTexture.Height);
            var origin = new Vector2(1430, 500);

            var playBtn = new UIButtonObject(AppData.MENU_PLAY_BTN_NAME, UIObjectType.Button,
                new Transform2D(AppData.MENU_PLAY_BTN_POSITION, .6f * Vector2.One, 0),
                .7f,
                Color.White,
                SpriteEffects.None,
                origin,
                btnTexture,
                null,
                sourceRectangle,
                "",
                fontDictionary["menu"],
                Color.Black,
                Vector2.Zero);

            //demo button color change
            var comp = new UIColorMouseOverBehaviour(Color.Orange, Color.White);
            playBtn.AddComponent(comp);

            mainMenuUIScene.Add(playBtn);

            /**************************** Controls Button ****************************/

            //same button texture so we can re-use texture, sourceRectangle and origin

            //demo button color change


            /**************************** Exit Button ****************************/

            //same button texture so we can re-use texture, sourceRectangle and origin

            //use a simple/smaller version of the UIButtonObject constructor
            var btnTexture2 = textureDictionary["exit"];
            var exitBtn = new UIButtonObject(AppData.MENU_EXIT_BTN_NAME, UIObjectType.Button,
                new Transform2D(AppData.MENU_EXIT_BTN_POSITION, .6f * Vector2.One, 0),
                0.7f,
                Color.White,
                SpriteEffects.None,
                origin,
                btnTexture2,
                null,
                sourceRectangle,
                "",
                fontDictionary["menu"],
                Color.Black,
                Vector2.Zero);

            //demo button color change
            exitBtn.AddComponent(new UIColorMouseOverBehaviour(Color.Orange, Color.White));

            mainMenuUIScene.Add(exitBtn);

            #endregion Main Menu

            //add scene to the menu manager
            uiMenuManager.Add(mainMenuUIScene);

            /************************** Controls Menu Scene **************************/

            /************************** Options Menu Scene **************************/

            /************************** Exit Menu Scene **************************/

            //finally we say...where do we start
            uiMenuManager.SetActiveScene(AppData.MENU_MAIN_NAME);
        }

        /// <summary>
        /// Adds ui elements seen in-game (e.g. health, timer)
        /// </summary>
        private void InitializeGameUI()
        { //create the scene
            var mainGameUIScene = new UIScene(AppData.UI_SCENE_MAIN_NAME);

            #region Add World map
            var mapTextureObj = new UITextureObject("0",
               UIObjectType.Texture,
               new Transform2D(new Vector2(1700, 880),
               Vector2.One, 0),
               0, textureDictionary["0"]);

            //add a progress controller
            //healthTextureObj.AddComponent(new UIProgressBarController(4, 8));
            
            
            //add the ui element to the scene
            mainGameUIScene.Add(mapTextureObj);

            //add a health bar in the centre of the game window

            var texture = textureDictionary["0"];
            var position = new Vector2(1750, 880);
            var origin = new Vector2(texture.Width / 2, texture.Height / 2);

            //create the UI element
            var healthTextureObj = new UITextureObject("health",
                UIObjectType.Texture,
                new Transform2D(position, new Vector2(.15f, .15f), 0),
                0,
                Color.White,
                origin,
                texture);




            //add a progress controller
            healthTextureObj.AddComponent(new UIProgressBarController(10, 10));

            //add the ui element to the scene
            mainGameUIScene.Add(healthTextureObj);

            #endregion Add Health Bar

            #region Add Scene To Manager & Set Active Scene

            //add the ui scene to the manager
            uiSceneManager.Add(mainGameUIScene);

            //set the active scene
            uiSceneManager.SetActiveScene(AppData.UI_SCENE_MAIN_NAME);

            #endregion Add Scene To Manager & Set Active Scene

  
        }

        /// <summary>
        /// Adds component to draw debug info to the screen
        /// </summary>
        private void InitializeDebugUI(bool showDebugInfo, bool showCollisionSkins = false)
        {
            if (showDebugInfo)
            {
                Components.Add(new GDLibrary.Utilities.GDDebug.PerfUtility(this,
                    _spriteBatch, fontDictionary["debug"],
                    new Vector2(40, _graphics.PreferredBackBufferHeight - 80),
                    Color.White));
            }

            if (showCollisionSkins)
                Components.Add(new GDLibrary.Utilities.GDDebug.PhysicsDebugDrawer(this, Color.Red));
        }

        /******************************* Non-Collidables *******************************/

        /// <summary>
        /// Set up the skybox using a QuadMesh
        /// </summary>
        /// <param name="level">Scene Stores all game objects for current...</param>
        /// <param name="worldScale">float Value used to scale skybox normally 250 - 1000</param>
        private void InitializeSkybox(Scene level, float worldScale = 500)
        {
            #region Reusable - You can copy and re-use this code elsewhere, if required

            //re-use the code on the gfx card
            var shader = new BasicShader(Application.Content, true, true);
            //re-use the vertices and indices of the primitive
            var mesh = new QuadMesh();
            //create an archetype that we can clone from
            var archetypalQuad = new GameObject("quad", GameObjectType.Skybox, true);

            #endregion Reusable - You can copy and re-use this code elsewhere, if required

            GameObject clone = null;
            //back
            clone = archetypalQuad.Clone() as GameObject;
            clone.Name = "skybox_back";
            clone.Transform.Translate(0, 0, -worldScale / 2.0f);
            clone.Transform.Scale(worldScale, worldScale, 1);
            clone.AddComponent(new MeshRenderer(mesh, new BasicMaterial("skybox_back_material", shader, Color.White, 1, textureDictionary["skybox_back"])));
            level.Add(clone);

            //left
            clone = archetypalQuad.Clone() as GameObject;
            clone.Name = "skybox_left";
            clone.Transform.Translate(-worldScale / 2.0f, 0, 0);
            clone.Transform.Scale(worldScale, worldScale, null);
            clone.Transform.Rotate(0, 90, 0);
            clone.AddComponent(new MeshRenderer(mesh, new BasicMaterial("skybox_left_material", shader, Color.White, 1, textureDictionary["skybox_left"])));
            level.Add(clone);

            //right
            clone = archetypalQuad.Clone() as GameObject;
            clone.Name = "skybox_right";
            clone.Transform.Translate(worldScale / 2.0f, 0, 0);
            clone.Transform.Scale(worldScale, worldScale, null);
            clone.Transform.Rotate(0, -90, 0);
            clone.AddComponent(new MeshRenderer(mesh, new BasicMaterial("skybox_right_material", shader, Color.White, 1, textureDictionary["skybox_right"])));
            level.Add(clone);

            //front
            clone = archetypalQuad.Clone() as GameObject;
            clone.Name = "skybox_front";
            clone.Transform.Translate(0, 0, worldScale / 2.0f);
            clone.Transform.Scale(worldScale, worldScale, null);
            clone.Transform.Rotate(0, -180, 0);
            clone.AddComponent(new MeshRenderer(mesh, new BasicMaterial("skybox_front_material", shader, Color.White, 1, textureDictionary["skybox_front"])));
            level.Add(clone);

            //top
            clone = archetypalQuad.Clone() as GameObject;
            clone.Name = "skybox_sky";
            clone.Transform.Translate(0, worldScale / 2.0f, 0);
            clone.Transform.Scale(worldScale, worldScale, null);
            clone.Transform.Rotate(90, -90, 0);
            clone.AddComponent(new MeshRenderer(mesh, new BasicMaterial("skybox_sky_material", shader, Color.White, 1, textureDictionary["skybox_sky"])));
            level.Add(clone);
        }

        /// <summary>
        /// Initialize the camera(s) in our scene
        /// </summary>
        /// <param name="level"></param>
        public void InitializeCameras(Scene level)
        {
            #region First Person Camera - Non Collidable


            //add camera game object
            var camera = new GameObject(AppData.CAMERA_FIRSTPERSON_NONCOLLIDABLE_NAME, GameObjectType.Camera);
            // trying to get the height of the camera so we can change the minimap 
            var height = new Vector3(0,10,0);
            if (camera.Transform.LocalTranslation.Y>=height.Y)
            {

            }
            //add components
            //here is where we can set a smaller viewport e.g. for split screen
            //e.g. new Viewport(0, 0, _graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight)
            camera.AddComponent(new Camera(_graphics.GraphicsDevice.Viewport));
            camera.AddComponent(new FirstPersonController(0.05f, 0.025f, new Vector2(0.006f, 0.004f)));
            IsMouseVisible = false;
            //set initial position
            camera.Transform.SetTranslation(-200, 20, 240);
            
            
            level.Add(camera);

            #endregion First Person Camera - Non Collidable

            #region Curve Camera - Non Collidable

            //add curve for camera translation
            var translationCurve = new Curve3D(CurveLoopType.Cycle);
            translationCurve.Add(new Vector3(0, 2, 10), 0);
            translationCurve.Add(new Vector3(0, 8, 15), 1000);
            translationCurve.Add(new Vector3(0, 8, 20), 2000);
            translationCurve.Add(new Vector3(0, 6, 25), 3000);
            translationCurve.Add(new Vector3(0, 4, 25), 4000);
            translationCurve.Add(new Vector3(0, 2, 10), 6000);

            //add camera game object
            var curveCamera = new GameObject(AppData.CAMERA_CURVE_NONCOLLIDABLE_NAME, GameObjectType.Camera);

            //add components
            curveCamera.AddComponent(new Camera(_graphics.GraphicsDevice.Viewport));
            curveCamera.AddComponent(new CurveBehaviour(translationCurve));
            curveCamera.AddComponent(new FOVOnScrollController(MathHelper.ToRadians(2)));

            //add to level
            level.Add(curveCamera);

            #endregion Curve Camera - Non Collidable

            #region First Person Camera - Collidable

            //add camera game object
            camera = new GameObject(AppData.CAMERA_FIRSTPERSON_COLLIDABLE_NAME, GameObjectType.Camera);

            //set initial position - important to set before the collider as collider capsule feeds off this position
            camera.Transform.SetTranslation(-160, 2.8f, 96.2f);

            //add components
            camera.AddComponent(new Camera(_graphics.GraphicsDevice.Viewport));

            //adding a collidable surface that enables acceleration, jumping
            //var collider = new CharacterCollider(2, 2, true, false);

            var collider = new MyHeroCollider(2, 2, true, false);
            camera.AddComponent(collider);
            collider.AddPrimitive(new Capsule(camera.Transform.LocalTranslation,
                Matrix.CreateRotationX(MathHelper.PiOver2), 2.5f, 3.6f),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            collider.Enable(false, 2);

            //add controller to actually move the collidable camera
            camera.AddComponent(new MyCollidableFirstPersonController(25,
                        0.5f, 0.3f, new Vector2(0.006f, 0.004f)));

            //add to level
            level.Add(camera);

            #endregion First Person Camera - Collidable

            //set the main camera, if we dont call this then the first camera added will be the Main
            level.SetMainCamera(AppData.CAMERA_FIRSTPERSON_COLLIDABLE_NAME);

            //allows us to scale time on all game objects that based movement on Time
            // Time.Instance.TimeScale = 0.1f;
        }

        /******************************* Collidables *******************************/

        /// <summary>
        /// Demo of the new physics manager and collidable objects
        /// </summary>
        private void InitializeCollidables(Scene level, float worldScale = 500)
        {
            InitializeCollidableGround(level, worldScale);
           //InitializeCollidableCubes(level);

            //InitializeCollidableModels(level);
            //InitializeCollidableTriangleMeshes(level);
            InitializeMountain(level);
            //InitializeWorldAssests(level);
            InitializeTrees(level);
            InitializeRocks(level);
            InitializeStaticPlatforms(level);
            InitializeSigns(level);
            InitializeCrown(level);
            InitializeFrog(level);
            //InitializemovingPlatforms(level);

        }

        private void InitializeStaticPlatforms(Scene level)
        {
            #region Reusable - You can copy and re-use this code elsewhere, if required


            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);

            //create the platform
            var platformArchetype = new GameObject("platform",
                GameObjectType.Platform, true);


            #endregion Reusable - You can copy and re-use this code elsewhere, if required


            #region First platform
            GameObject clone = null;


            clone = platformArchetype.Clone() as GameObject;
            int move = -80;
            for (int i = 0; i < 10000; i++)
            { move =move- 1;
                
            }
                clone.Name = "platform1";
            clone.Transform.Translate(-59, 13, 48);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f)),
                new MaterialProperties(1f, 1f, 1f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion
            #region copy platforms
            #region Second Platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform2";
            clone.Transform.Translate(-52, 13, 54);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 3rd platform
            clone = null;


            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform3";
            clone.Transform.Translate(-44, 13, 54);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 4th platform
            clone = null;


            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform4";
            clone.Transform.Translate(1, 25, 8);
            clone.Transform.SetScale(1, 1, 1);
            clone.Transform.SetRotation(0,90,0);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 5th platform
            clone = null;


            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform5";
            clone.Transform.Translate(3, 25, -6);
            clone.Transform.SetScale(1, 1, 1);
            clone.Transform.SetRotation(0, 90, 0);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 6th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform6";
            clone.Transform.Translate(-45, 35, -47);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 7th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform7";
            clone.Transform.Translate(-51, 39, -47);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 8th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform8";
            clone.Transform.Translate(-60, 37, -47);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 9th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform9";
            clone.Transform.Translate(-101, 49, -5);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 10th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform10";
            clone.Transform.Translate(-106, 53, 8);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 11th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform11";
            clone.Transform.Translate(-69, 61, 36);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 12th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform12";
            clone.Transform.Translate(-62, 64, 36);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 13th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform13";
            clone.Transform.Translate(-54, 68, 36);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 14th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform14";
            clone.Transform.Translate(-47, 63, 36);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 15th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform15";
            clone.Transform.Translate(-40, 65, 36);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 16th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-32, 64, 36);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 17th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-15, 73, 3);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 18th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-15, 73, -10);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 19th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-22, 73, -12);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 20th platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-22, 73, 5);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 21 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-28, 73, 5);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 22 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-28, 73, -17);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 23 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-47, 84, -31);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 24 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-58, 87, -31);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 25 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-74, 90, -31);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 26 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-79, 94, -6);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 27 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-89, 94, -6);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 28 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-99, 94, -6);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 29 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-79, 94, 8);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 30 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-89, 94, 8);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 31 platform
            clone = null;

            clone = platformArchetype.Clone() as GameObject;

            //clone the archetypal cube
            clone.Name = "platform16";
            clone.Transform.Translate(-99, 94, 8);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["platform"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion
        }
        #endregion

        private void InitializemovingPlatforms(Scene level)
        {
            #region Reusable - You can copy and re-use this code elsewhere, if required


            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);

            //create the platform
            var platformArchetype = new GameObject("platform",
                GameObjectType.Platform, true);


            #endregion Reusable - You can copy and re-use this code elsewhere, if required


            #region First platform
            GameObject clone = null;


            clone = platformArchetype.Clone() as GameObject;

            clone.Name = "platform1";
            var translationCurve = new Curve3D(CurveLoopType.Cycle);
            translationCurve.Add(new Vector3(-60, 12, 49), 0);
            translationCurve.Add(new Vector3(-60, 12, 20), 10000);
            translationCurve.Add(new Vector3(-60, 12, 49),10000);
            translationCurve.Add(new Vector3(-60, 12, 20), 10000);
            translationCurve.Add(new Vector3(-60, 12, 49), 10000);

            clone.AddComponent(new CurveBehaviour(translationCurve));
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["platform"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["platform"])));

            //add Collision Surface(s)
            
            clone.AddComponent(new CurveBehaviour(translationCurve));
           
            //add To Scene Manager
            level.Add(clone);
            #endregion
        }

            
           

        private void InitializeMountain(Scene level)
        {
            #region Reusable - You can copy and re-use this code elsewhere, if required


            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);

            //create the sphere
            var mountainArchetype = new GameObject("levelForImport",
                GameObjectType.Interactable, true);


            #endregion Reusable - You can copy and re-use this code elsewhere, if required

            //clone the archetypal cube
          

            mountainArchetype.Name = "Mountain";
            mountainArchetype.Transform.Translate(1, 12, 1);
            mountainArchetype.Transform.SetScale(2, 2, 2);
            mountainArchetype.AddComponent(new ModelRenderer(modelDictionary["levelForImport"],
                new BasicMaterial("sphere_material",
                shader, Color.White, 1, textureDictionary["mountain"])));

            //add Collision Surface(s)
            collider = new Collider();
            mountainArchetype.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["levelForImport"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(2f, 2f, 2f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(mountainArchetype);

        }


        private void InitializeCollidableGround(Scene level, float worldScale)
        {
            #region Reusable - You can copy and re-use this code elsewhere, if required

            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);
            //re-use the vertices and indices of the model
            var mesh = new QuadMesh();

            #endregion Reusable - You can copy and re-use this code elsewhere, if required

            //create the ground
            var ground = new GameObject("ground", GameObjectType.Ground, true);
            ground.Transform.SetRotation(-90, 0, 0);
            ground.Transform.SetScale(worldScale, worldScale, 1);
            ground.AddComponent(new MeshRenderer(mesh, new BasicMaterial("grass_material", shader, Color.White, 1, textureDictionary["grass2"])));

            //add Collision Surface(s)
            collider = new Collider();
            ground.AddComponent(collider);
            collider.AddPrimitive(new JigLibX.Geometry.Plane(
                ground.Transform.Up, ground.Transform.LocalTranslation),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(ground);
        }

        private void InitializeSigns(Scene level)
        {
            #region Signs
            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);
            var sign = new GameObject("SIGNFINISHED", GameObjectType.Platform, true);

            GameObject clone = null;

            clone = sign.Clone() as GameObject;
            clone.Name = "sign1";
            clone.Transform.Translate(-70, 12, 56);
            clone.Transform.SetScale(1, 1, 1);
            clone.Transform.SetRotation(0,130,0);
            clone.AddComponent(new ModelRenderer(modelDictionary["SIGNFINISHED"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["sign"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["SIGNFINISHED"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            
            collider.Enable(true, 1);

           
            level.Add(clone);
            

            #region 2nd Sign
            clone = null;
            clone = sign.Clone() as GameObject;

            
            clone.Name = "sign2";
            clone.Transform.Translate(4, 24, 20);
            clone.Transform.SetScale(1, 1, 1);
            clone.Transform.SetRotation(0, 300, 0);
            clone.AddComponent(new ModelRenderer(modelDictionary["SIGNFINISHED"],new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["sign"])));

           
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["SIGNFINISHED"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            clone.Transform.SetRotation(0, 180, 0);
            collider.Enable(true, 1);

     
            level.Add(clone);
            #endregion

            #region 3rd Sign
            clone = null;
            clone = sign.Clone() as GameObject;

            clone.Name = "sign3";
            clone.Transform.Translate(-33, 35, -54);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["SIGNFINISHED"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["sign"])));

           
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["SIGNFINISHED"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

        
            level.Add(clone);
            #endregion

            #region 4th sign
            clone = null;
            clone = sign.Clone() as GameObject;

            clone.Name = "sign4";
            clone.Transform.Translate(-107, 47, -18);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["SIGNFINISHED"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["sign"])));

            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["SIGNFINISHED"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region 5th sign
            clone = null;
            clone = sign.Clone() as GameObject;

            clone.Name = "sign5";
            clone.Transform.Translate(-81.4f, 58.5f, 38);
            clone.Transform.SetScale(1, 1, 1);
            clone.Transform.SetRotation(0, 180, 0);
            clone.AddComponent(new ModelRenderer(modelDictionary["SIGNFINISHED"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["sign"])));

            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["SIGNFINISHED"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 6th sign
            clone = null;
            clone = sign.Clone() as GameObject;

            clone.Name = "sign6";
            clone.Transform.Translate(-38.6f, 81.5f, -46.9f);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["SIGNFINISHED"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["sign"])));

            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["SIGNFINISHED"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #region 7th sign
            clone = null;
            clone = sign.Clone() as GameObject;

            clone.Name = "sign7";
            clone.Transform.Translate(-102.8f, 92.7f, -13.6f);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["SIGNFINISHED"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["sign"])));

            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["SIGNFINISHED"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion

            #endregion signs
        }
        private void InitializeCrown(Scene level)
        {
            #region Signs


            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);
            var crown = new GameObject("crown", GameObjectType.Consumable, true);

            GameObject clone = null;

            clone = crown.Clone() as GameObject;
            clone.Name = "crown";
            clone.Transform.Translate(-55, 123, -27);
            clone.Transform.SetScale(1, 1, 1);
            clone.AddComponent(new ModelRenderer(modelDictionary["crown"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["crown"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["crown"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 1f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            //add To Scene Manager
            level.Add(clone);
            #endregion
        }

        private void InitializeFrog(Scene level)
        {
            #region frog spectators


            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);
            var frog = new GameObject("frog", GameObjectType.Tree, true);

            GameObject clone = null;
            //creates 3 rows of frogs in an incline for the crowd
            for (int j = 0; j < 6; j += 2)
            {
                for (int i = 5; i < 40; i += 5)
                {
                    clone = frog.Clone() as GameObject;
                    clone.Name = "frog";
                    clone.Transform.Translate(-100+i, j, 80+j);
                    clone.Transform.SetScale(1, 1, 1);
                    clone.Transform.SetRotation(0, 180, 0);
                    Random rnd = new Random();
                    int rand = rnd.Next(1, 4);   // creates a number between 1 and 6

                   // if (rand == 1)
                   // {
                        clone.AddComponent(new ModelRenderer(modelDictionary["frog"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["frog"])));

                   // }
                   // else if (rand == 2)
                   // {
                   //     clone.AddComponent(new ModelRenderer(modelDictionary["frog1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["frog1"])));

                   // }
                   // else if (rand == 3)
                   // {
                   //     clone.AddComponent(new ModelRenderer(modelDictionary["frog2"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["frog2"])));

                   // }
                   // else if (rand == 4)
                    //{
                   //     clone.AddComponent(new ModelRenderer(modelDictionary["frog3"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["frog3"])));

                   // }
                    //add Collision Surface(s)
                    collider = new Collider();
                    clone.AddComponent(collider);
                    collider.AddPrimitive(
                       CollisionUtility.GetTriangleMesh(modelDictionary["frog"],
                        new Vector3(0, 0, 0), new Vector3(0, 180, 0), new Vector3(1f, 1f, 1f)),
                        new MaterialProperties(1f, 1f, 1f));
                    collider.Enable(true, 1);


                    //add To Scene Manager
                    level.Add(clone);
                }
            }
            #endregion
        }

        private void InitializeTrees(Scene level)
        {
             #region trees
            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);
            var Tree = new GameObject("Tree", GameObjectType.Platform, true);

            GameObject clone = null;

            clone = Tree.Clone() as GameObject;
            clone.Name = "Tree";
            clone.Transform.Translate(-98, -5, 40);
            clone.Transform.SetScale(0.1f, 0.1f,0.1f);
            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);
//tree2
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree2";
            clone.Transform.Translate(-15,7, 54);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);
            
            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);
            //tree3
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree3";
            clone.Transform.Translate(4, 18, -33);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);

            //tree4
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree4";
            clone.Transform.Translate(-78, 30, -54);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);

            

            //tree5
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree5";
            clone.Transform.Translate(-14, 53, 34);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);
            //tree6
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree6";
            clone.Transform.Translate(-39, 114, -40);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);

            //tree7
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree7";
            clone.Transform.Translate(-68, 114, -40);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);
            #endregion trees

            #region Level Trees

            #region LDTree1
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree8";
            clone.Transform.Translate(-160, -18, 163);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0,90,0);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree2
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree9";
            clone.Transform.Translate(-72, -18, 197);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0,-35,0);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree3
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree10";
            clone.Transform.Translate(2, -18, 190);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree4
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree11";
            clone.Transform.Translate(84, -18, 143);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0,235,0);
            

            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree5
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree12";
            clone.Transform.Translate(-233, -15, 56);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree6
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree13";
            clone.Transform.Translate(-222, -15, -21);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree7
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree14";
            clone.Transform.Translate(-390, 35, -65);
            clone.Transform.SetScale(0.4f, 0.4f, 0.4f);
            clone.Transform.SetRotation(0, 30, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree8
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree15";
            clone.Transform.Translate(-212, -15, -90);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree9
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree16";
            clone.Transform.Translate(-149, -15, -125);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree10
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree17";
            clone.Transform.Translate(-62, -15, -156);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree11
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree18";
            clone.Transform.Translate(-16,-15,-151);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree12
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree20";
            clone.Transform.Translate(63, -15, -115);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree13
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree21";
            clone.Transform.Translate(110, -15, -38);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree14
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree22";
            clone.Transform.Translate(109, -15, -2);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree15
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree23";
            clone.Transform.Translate(106, -14, 43);
            clone.Transform.SetScale(0.2f, 0.2f, 0.2f);
            clone.Transform.SetRotation(0, 20, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree16
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree24";
            clone.Transform.Translate(-343, -15, 195);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 75, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree17
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree25";
            clone.Transform.Translate(-343, -15, -190);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 175, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree18
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree25";
            clone.Transform.Translate(-163, -15, -409);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 175, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree19
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree30";
            clone.Transform.Translate(343, -15, 195);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 75, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree20
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(343, -15, -190);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 175, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree21
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(386, 40, -15);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 60, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree22
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(143, 35, -388);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 175, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree23
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(143, 35, 388);
            clone.Transform.SetScale(0.3f, 0.3f, 0.3f);
            clone.Transform.SetRotation(0, 60, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree24
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(-190, 60, 340);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);
            clone.Transform.SetRotation(0, 60, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree25
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(37, -5, 325);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);
            clone.Transform.SetRotation(0, 175, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree26
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(244, -10, 14);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);
            clone.Transform.SetRotation(0, 275, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region LDTree27
            clone = null;
            clone = Tree.Clone() as GameObject;


            clone.Name = "tree31";
            clone.Transform.Translate(180, -5, -252);
            clone.Transform.SetScale(0.1f, 0.1f, 0.1f);
            clone.Transform.SetRotation(0, 45, 0);


            clone.AddComponent(new ModelRenderer(modelDictionary["Tree"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["tree"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["Tree"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17f, 17f, 17f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion
            #endregion
        }

        private void InitializeRocks(Scene level)
        {
             #region Rocks
            //re-use the code on the gfx card, if we want to draw multiple objects using Clone
            var shader = new BasicShader(Application.Content, false, true);
            var rock = new GameObject("rock1", GameObjectType.Platform, true);

            GameObject clone = null;

            clone = rock.Clone() as GameObject;
            clone.Name = "rock1";
            clone.Transform.Translate(-102,3, 36);
            clone.Transform.SetScale(4, 4, 4);
            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));

            //add Collision Surface(s)
            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f,4f, 4f)),
                new MaterialProperties(1f, 1f, 1f));
            collider.Enable(true, 1);

           
            level.Add(clone);


//rock2
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "rock2";
            clone.Transform.Translate(-106,3,33);
            clone.Transform.SetScale(4, 4, 4);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);
//rock3
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "rock2";
            clone.Transform.Translate(-109, 3, 24);
            clone.Transform.SetScale(4, 4, 4);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);


            level.Add(clone);

            #region level design rocks

            #region level rocks 1
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock1";
            clone.Transform.Translate(-117, 5, 177);
            clone.Transform.SetScale(20, 20, 20);
            clone.Transform.SetRotation(0,155,0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region level rocks 2
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock2";
            clone.Transform.Translate(-217, 5, 125);
            clone.Transform.SetScale(25, 25, 25);
            clone.Transform.SetRotation(0, 210, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region level rocks 3
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock2";
            clone.Transform.Translate(-217,5, 125);
            clone.Transform.SetScale(25, 25, 25);
            clone.Transform.SetRotation(0, 210, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region level rocks 4
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock4";
            clone.Transform.Translate(100, 6, -93);
            clone.Transform.SetScale(20, 20, 20);
            clone.Transform.SetRotation(0, 145, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #endregion level design rocks

            #region Background Rocks
            #region background rock 1
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock2";
            clone.Transform.Translate(-422, 5, -2);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, 0, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 2
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock2";
            clone.Transform.Translate(-422, 1, -70);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, 180, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 3
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock3";
            clone.Transform.Translate(-337, 1, -322);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, 65, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 4
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock3";
            clone.Transform.Translate(-210, 1, -350);
            clone.Transform.SetScale(25, 25, 25);
            clone.Transform.SetRotation(0, 125, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 5
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock2";
            clone.Transform.Translate(422, 5, -2);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, 0, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 6
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock2";
            clone.Transform.Translate(422, 1, -70);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, 180, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 7
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock7";
            clone.Transform.Translate(165, 1, -412);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, 80, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 8
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock7";
            clone.Transform.Translate(135, 1, -412);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, -80, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 9
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock7";
            clone.Transform.Translate(165, 1, 412);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, 80, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 10
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock10";
            clone.Transform.Translate(135, 1, 412);
            clone.Transform.SetScale(70, 70, 70);
            clone.Transform.SetRotation(0, -80, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 11
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock10";
            clone.Transform.Translate(-200, 25, 356);
            clone.Transform.SetScale(45, 45, 45);
            clone.Transform.SetRotation(0, -55, 0);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 12
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock10";
            clone.Transform.Translate(-67, 4, 369);
            clone.Transform.SetScale(35, 35, 35);
            clone.Transform.SetRotation(20, -125, -10);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 13
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock10";
            clone.Transform.Translate(255, 7, 296);
            clone.Transform.SetScale(35, 35, 35);
            clone.Transform.SetRotation(45, -155, -30);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 14
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock10";
            clone.Transform.Translate(-388, 17, 356);
            clone.Transform.SetScale(35, 35, 35);
            clone.Transform.SetRotation(45, -155, -30);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #region background rock 15
            clone = null;
            clone = rock.Clone() as GameObject;


            clone.Name = "LDRock10";
            clone.Transform.Translate(376, 19, -340);
            clone.Transform.SetScale(45, 45, 45);
            clone.Transform.SetRotation(45, -155, -30);

            clone.AddComponent(new ModelRenderer(modelDictionary["rock1"], new BasicMaterial("sphere_material", shader, Color.White, 1, textureDictionary["gray"])));


            collider = new Collider();
            clone.AddComponent(collider);
            collider.AddPrimitive(
               CollisionUtility.GetTriangleMesh(modelDictionary["rock1"],
                new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(4f, 4f, 4f)),
                new MaterialProperties(0.1f, 0.8f, 0.7f));
            collider.Enable(true, 1);

            level.Add(clone);
            #endregion

            #endregion Background Rocks

            #endregion rocks
        }

        #endregion Student/Group Specific Code

    }
}
