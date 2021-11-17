using GDLibrary.Components;
using GDLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Collections
{
    /// <summary>
    /// Actions that we can apply to a game object in a list
    /// </summary>
    public enum ComponentChangeType
    {
        Add, Update, Remove
    }

    /// <summary>
    /// Provides list storage for a gameobject in either a static or dynamic list
    /// The list itself is not static or dynamic, rather the game object may be static (e.g. a wall) or dynamic (e.g. a pickup spawned in game)
    /// </summary>
    public class GameObjectList
    {
        #region Fields

        /// <summary>
        /// Indicate the likely number of static objects in your game scene
        /// </summary>
        private static readonly int STATIC_LIST_DEFAULT_SIZE = 20;

        /// <summary>
        /// Indicate the likely number of dynamic objects in your game scene
        /// </summary>
        private static readonly int DYNAMIC_LIST_DEFAULT_SIZE = 10;

        protected List<GameObject> staticList;
        protected List<GameObject> dynamicList;
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

        public GameObjectList()
        {
            staticList = new List<GameObject>(STATIC_LIST_DEFAULT_SIZE);
            dynamicList = new List<GameObject>(DYNAMIC_LIST_DEFAULT_SIZE);
            renderers = new List<Renderer>(STATIC_LIST_DEFAULT_SIZE);
            controllers = new List<Controller>(STATIC_LIST_DEFAULT_SIZE);
            behaviours = new List<Behaviour>(STATIC_LIST_DEFAULT_SIZE);

            //TODO - replace hardcoded with some reasonable constants
            materials = new List<Material>(5);
            cameras = new List<Camera>(5);
        }

        #endregion Constructors

        #region Actions - Update, Unload, Clear

        public virtual void Update()
        {
            for (int i = 0; i < staticList.Count; i++)
            {
                if (staticList[i].IsEnabled)
                    staticList[i].Update();
            }

            for (int i = 0; i < dynamicList.Count; i++)
            {
                if (dynamicList[i].IsEnabled)
                    dynamicList[i].Update();
            }
        }

        public virtual void Unload()
        {
            foreach (GameObject gameObject in staticList)
                gameObject.Dispose();

            foreach (GameObject gameObject in dynamicList)
                gameObject.Dispose();

            //TODO - add Dispose for materials, behaviours etc

            Clear();
        }

        protected void Clear()
        {
            staticList.Clear();
            dynamicList.Clear();
            controllers.Clear();
            behaviours.Clear();
            renderers.Clear();
            materials.Clear();
            cameras.Clear();
        }

        #endregion Actions - Update, Unload, Clear

        #region Actions - Add, Remove, Find, CheckComponents

        public void Add(Scene scene, GameObject gameObject)
        {
            //add to the appropriate list of objects in the scene
            if (gameObject.IsStatic)
                staticList.Add(gameObject);
            else
                dynamicList.Add(gameObject);

            //set the scene that object belongs to
            gameObject.Scene = scene;

            //TODO - add root transform set and notify change
            //gameObject.Transform.Root = transform;

            //if object enabled then add to appropriate list
            if (gameObject.IsEnabled)
                CheckComponents(gameObject, ComponentChangeType.Add);

            //if object isn't initialized then init
            if (!gameObject.IsRunning)
                gameObject.Initialize();
        }

        public void Remove(GameObject obj)
        {
            if (obj.IsStatic)
                staticList.Add(obj);
            else
                dynamicList.Add(obj);
        }

        public GameObject Find(Predicate<GameObject> predicate)
        {
            GameObject found = staticList.Find(predicate);
            if (found == null)
                found = dynamicList.Find(predicate);

            return found;
        }

        public List<GameObject> FindAll(Predicate<GameObject> predicate)
        {
            List<GameObject> found = staticList.FindAll(predicate);
            if (found == null)
                found = dynamicList.FindAll(predicate);

            return found;
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

        #endregion Actions - Add, Remove, Find, CheckComponents
    }
}