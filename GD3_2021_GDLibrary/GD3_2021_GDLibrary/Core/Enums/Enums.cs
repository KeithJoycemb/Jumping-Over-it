namespace GDLibrary.Core
{
    /// <summary>
    /// Possible status types for an actor within the game (e.g. Update | Drawn, Update, Drawn, Off)
    /// </summary>
    /// <see cref="GDLibrary.GameObject.GameObject(string, GameObjectType, StatusType)"/>
    public enum StatusType
    {
        Off = 0,            //turn object off
        Drawn = 1,           //e.g. a game or ui object a renderer but no components
        Updated = 2,         //e.g. a camera
        /*
        * Q. Why do we use powers of 2? Will it allow us to do anything different?
        * A. StatusType.Updated | StatusType.Drawn - See ObjectManager::Update() or Draw()
        */
    }

    /// <summary>
    /// Event categories within the game that a subscriber can subscribe to in the EventDispatcher
    /// </summary>
    public enum EventCategoryType
    {
        Camera,
        Player,
        NonPlayer,
        Pickup,
        Sound,
        Menu,
        UI,
        GameObject,
        UiObject,
        Opacity,
        Picking

        //add more here...
    }

    /// <summary>
    /// Event actions that can occur within a category (e.g. EventCategoryType.Sound with EventActionType.OnPlay)
    /// </summary>
    public enum EventActionType
    {
        OnPlay,
        OnPlay2D,
        OnPlay3D,

        OnPause,
        OnResume,
        OnRestart,
        OnExit,
        OnStop,
        OnStopAll,

        OnVolumeDelta,
        OnVolumeSet,
        OnMute,
        OnUnMute,

        OnClick,
        OnHover,

        OnCameraSetActive,
        OnCameraCycle,

        OnLose,
        OnWin,
        OnPickup,

        OnAddObject,
        OnRemoveObject,
        OnEnableObject,
        OnDisableObject,
        OnSpawnObject,
        OnObjectPicked,
        OnNoObjectPicked,

        OnJump,

        //add more here...
    }

    /// <summary>
    /// Used by SoundManager to set volume etc on a category of sounds e.g. all explosion sounds
    /// </summary>
    public enum SoundCategoryType : sbyte
    {
        WinLose,
        Explosion,
        BackgroundMusic,
        Alarm,
        Jump
    }
}