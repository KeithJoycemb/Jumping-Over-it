using GDLibrary.Collections;
using GDLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Components
{
    /// <summary>
    /// Stores and updates all game objects for a level or part of a level
    /// </summary>
    public class Scene : GameObject
    {
        #region Statics

        private static Scene current;
        public static Scene Current { get => current; set => current = value; }

        #endregion Statics

        #region Fields

        protected GameObjectList gameObjects;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a list of all renderers attached to game objects in this scene
        /// </summary>
        public List<Renderer> Renderers => gameObjects.Renderers;

        /// <summary>
        /// Gets  a list of all materials attached to renderers in this scene
        /// </summary>
        public List<Material> Materials => gameObjects.Materials;

        /// <summary>
        /// Gets a list of all cameras in this scene
        /// </summary>
        public List<Camera> Cameras => gameObjects.Cameras;

        #endregion Properties

        #region Constructors

        public Scene(string name) : base(name)
        {
            gameObjects = new GameObjectList();
        }

        #endregion Constructors

        #region Update, Unload, Clear

        public override void Update()
        {
            //update any components attached to this scene
            base.Update();

            //update all the static and dynamic game objects in this scene
            gameObjects.Update();
        }

        public virtual void Unload()
        {
            gameObjects.Unload();
        }

        #endregion Update, Unload, Clear

        #region Actions - Add, Remove, Find, Check

        public void Add(GameObject gameObject)
        {
            gameObjects.Add(this, gameObject);
        }

        public GameObject Find(Predicate<GameObject> predicate)
        {
            return gameObjects.Find(predicate);
        }

        public List<GameObject> FindAll(Predicate<GameObject> predicate)
        {
            return gameObjects.FindAll(predicate);
        }

        /// <summary>
        /// Returns a list of all the cameras in this scene
        /// </summary>
        /// <returns></returns>
        /// <see cref="GDLibrary.Managers.RenderManager.Draw(Microsoft.Xna.Framework.GameTime)"/>
        public List<Camera> GetAllActiveSceneCameras()
        {
            return gameObjects.Cameras;
        }

        /// <summary>
        /// Sets the Main camera for the game using an appropriate predicate
        /// </summary>
        /// <param name="predicate">Predicate of type GameObject</param>
        /// <returns>True if set, otherwise false</returns>
        public bool SetMainCamera(Predicate<GameObject> predicate)
        {
            //look for the cameras game object
            var cameraObject = gameObjects.Find(predicate);

            //if not null then look for a camera component
            var camera = cameraObject?.GetComponent<Camera>();

            //if no camera then null
            if (camera == null)
                throw new ArgumentException("Predicate did not return a valid camera!");

            //set this camera as Main in the game
            Camera.Main = camera;

            return true;
        }

        /// <summary>
        /// Sets the Main camera for the game using the Name of the parent game object
        /// </summary>
        /// <param name="name">String name of the parent GameObject</param>
        /// <returns>True if set, otherwise false</returns>
        public bool SetMainCamera(string name)
        {
            return SetMainCamera(gameObject => gameObject.Name == name);
        }

        #endregion Actions - Add, Remove, Find, Check
    }
}

/*
 using GDLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Components
{
    /// <summary>
    /// Actions that we can apply to a game object in a list
    /// </summary>
    public enum ComponentChangeType
    {
        Add, Update, Remove
    }

    /// <summary>
    /// Stores and updates all game objects for a level or part of a level
    /// </summary>
    public class Scene : GameObject
    {
        #region Statics

        private static Scene current;
        public static Scene Current { get => current; set => current = value; }

        #endregion Statics

        #region Fields

        protected List<GameObject> gameObjects;
        protected List<Renderer> renderers;
        protected List<Controller> controllers;
        protected List<Behaviour> behaviours;
        protected List<Material> materials;
        protected List<Camera> cameras;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a list of all renderers attached to game objects in this scene
        /// </summary>
        public List<Renderer> Renderers => renderers;

        /// <summary>
        /// Gets  a list of all materials attached to renderers in this scene
        /// </summary>
        public List<Material> Materials => materials;

        /// <summary>
        /// Gets a list of all cameras in this scene
        /// </summary>
        public List<Camera> Cameras => cameras;

        #endregion Properties

        #region Constructors

        public Scene(string name) : base(name)
        {
            //TODO - add default readonly variables instead of magic numbers!
            gameObjects = new List<GameObject>(10);
            renderers = new List<Renderer>(10);
            controllers = new List<Controller>(10);
            behaviours = new List<Behaviour>(10);
            materials = new List<Material>(5);
            cameras = new List<Camera>(4);
        }

        #endregion Constructors

        #region Update

        public override void Update()
        {
            base.Update();

            //TODO - Add remove of destroyed objects

            for (int i = 0; i < gameObjects.Count; i++)
            {
                //do not update something that is disabled (e.g. a controller for a door that wont be enabled until X time or we have Y key)
                if (gameObjects[i].IsEnabled)
                    gameObjects[i].Update();
            }
        }

        #endregion Update

        #region Actions - Add, Remove, Find, Check

        public void Add(GameObject gameObject)
        {
            //add to the list of objects in the scene
            gameObjects.Add(gameObject);

            //set the scene that object belongs to
            gameObject.Scene = this;

            //TODO - add root transform set and notify change
            //gameObject.Transform.Root = transform;

            //if object enabled then add to appropriate list
            if (gameObject.IsEnabled)
                CheckComponents(gameObject, ComponentChangeType.Add);

            //if object isn't initialized then init
            if (!gameObject.IsRunning)
                gameObject.Initialize();
        }

        public GameObject Find(Predicate<GameObject> predicate)
        {
            return gameObjects.Find(predicate);
        }

        public List<GameObject> FindAll(Predicate<GameObject> predicate)
        {
            return gameObjects.FindAll(predicate);
        }

        protected void CheckComponents(GameObject gameObject, ComponentChangeType type)
        {
            for (int i = 0; i < gameObject.Components.Count; i++)
            {
                var component = gameObject.Components[i];

                if (component is Renderer renderer)
                {
                    if (type == ComponentChangeType.Add)
                        AddRenderer(renderer);
                    else if (type == ComponentChangeType.Remove)
                        RemoveRenderer(renderer);
                }
                else if (component is Camera camera)
                {
                    if (type == ComponentChangeType.Add && !cameras.Contains(camera))
                        AddCamera(camera);
                    else if (type == ComponentChangeType.Remove)
                        RemoveCamera(camera);
                }
                else if (component is Behaviour behaviour)
                {
                    if (type == ComponentChangeType.Add && !behaviours.Contains(behaviour))
                        AddBehaviour(behaviour);
                    else if (type == ComponentChangeType.Remove)
                        RemoveBehaviour(behaviour);
                }
                else if (component is Controller controller)
                {
                    if (type == ComponentChangeType.Add && !controllers.Contains(controller))
                        AddController(controller);
                    else if (type == ComponentChangeType.Remove)
                        RemoveController(controller);
                }
            }
        }

        protected int AddCamera(Camera camera)
        {
            var index = cameras.IndexOf(camera);

            if (index == -1)
            {
                cameras.Add(camera);
                cameras.Sort();
                index = cameras.Count - 1;

                if (Camera.Main == null)
                    Camera.Main = camera;
            }

            return index;
        }

        /// <summary>
        /// Sets the Main camera for the game using an appropriate predicate
        /// </summary>
        /// <param name="predicate">Predicate of type GameObject</param>
        /// <returns>True if set, otherwise false</returns>
        public bool SetMainCamera(Predicate<GameObject> predicate)
        {
            //look for the cameras game object
            var cameraObject = gameObjects.Find(predicate);

            //if not null then look for a camera component
            var camera = cameraObject?.GetComponent<Camera>();

            //if no camera then null
            if (camera == null)
                throw new ArgumentException("Predicate did not return a valid camera!");

            //set this camera as Main in the game
            Camera.Main = camera;

            return true;
        }

        /// <summary>
        /// Sets the Main camera for the game using the Name of the parent game object
        /// </summary>
        /// <param name="name">String name of the parent GameObject</param>
        /// <returns>True if set, otherwise false</returns>
        public bool SetMainCamera(string name)
        {
            return SetMainCamera(gameObject => gameObject.Name == name);
        }

        protected void RemoveCamera(Camera camera)
        {
            if (cameras.Contains(camera))
                cameras.Remove(camera);
        }

        protected void AddRenderer(Renderer renderer)
        {
            if (renderers.Contains(renderer))
                return;

            renderers.Add(renderer);
            renderers.Sort();
        }

        protected void RemoveRenderer(Renderer renderer)
        {
            if (renderers.Contains(renderer))
                renderers.Remove(renderer);
        }

        protected void AddController(Controller controller)
        {
            if (controllers.Contains(controller))
                return;

            controllers.Add(controller);
            controllers.Sort();
        }

        protected void RemoveController(Controller controller)
        {
            if (controllers.Contains(controller))
                controllers.Remove(controller);
        }

        protected void AddBehaviour(Behaviour behaviour)
        {
            if (behaviours.Contains(behaviour))
                return;

            behaviours.Add(behaviour);
            behaviours.Sort();
        }

        protected void RemoveBehaviour(Behaviour behaviour)
        {
            if (behaviours.Contains(behaviour))
                behaviours.Remove(behaviour);
        }

        public virtual void Unload()
        {
            foreach (GameObject gameObject in gameObjects)
                gameObject.Dispose();

            //TODO - add Dispose for materials, behaviours etc

            Clear();
        }

        protected void Clear()
        {
            gameObjects.Clear();
            controllers.Clear();
            behaviours.Clear();
            renderers.Clear();
            materials.Clear();
            cameras.Clear();
        }

        #endregion Actions - Add, Remove, Find, Check

        //TODO - Clear, Dispose
    }
}

 */