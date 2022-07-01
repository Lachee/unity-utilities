using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>
    /// This class creates a Singleton GameObject that will either be lazily initialized when it is referenced for the first time or, grabbed from the scene if an instance already exists.
    /// <para>It derives from MonoBehaviour allowing for all of the usual Unity systems to be used.</para>
    /// <para>NOTE: A subclasses must pass in its own Type as the T parameter, this is so the singleton can typecast the instance member variable to the corrent class.</para>
    /// </summary>
    /// <example>
    /// public class GameManager : Singleton<GameManager>
    /// {
    ///     public void StartGame() { }
    /// }
    /// 
    /// public class UIButton : MonoBehaviour 
    /// {
    ///     public void OnButtonPress() {
    ///         GameManager.instance.StartGame();
    ///     }
    /// }
    /// </example>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        internal static T _instance;
        private static bool _isquitting = false;

        /// <summary>
        /// The current type that belongs to this singleton. Alias of <code>typeof(T)</code>.
        /// </summary>
        public static System.Type type { get { return typeof(T); } }

        /// <summary>Waits for the singleton to be available in the scene.</summary>
        /// <seealso cref="available"/>
        public static readonly WaitWhile wait = new WaitWhile(() => !available);
        /// <summary>Waits for the singleton to be referenced.</summary>
        /// <seealso cref="referenced"/>
        public static readonly WaitWhile waitForReference = new WaitWhile(() => !referenced);

        /// <summary>The current logger</summary>
        public static readonly Logger logger = new Logger(typeof(T).Name);

        /// <summary>
        /// The singleton instance of the type. Will create a new object with type if it is not available within the scene.
        /// </summary>    
        public static T instance
        {
            get
            {

                //Util.Assert(Object.FindObjectsOfType<T>().Length <= 1); // there should not be more than one instance of any manager component

                if (!available)
                {
                    if (_isquitting)
                        Debug.LogError("Creating a new instance of " + type + " while a OnApplicationQuit has been detected.");

                    //We do not have one available, lets create it as a new gameobject.
                    if (Application.isPlaying)
                    {
                        GameObject obj = new GameObject($"[ {type} INSTANCE ]");
                        _instance = obj.AddComponent<T>();
                        Debug.LogWarning("Singleton " + type + " does not exist. A new instance has been created instead.", _instance);
                    }
                }

                //Return the instance.
                return _instance;
            }

            set
            {
                Debug.LogError("Someone is trying to manually set the singleton for " + type, _instance);
                _instance = value;
            }
        }


        /// <summary>
        /// The singleton instance of the type. Similar to <see cref="instance"/>, however null will be returned if the instance does not exist instead of trying to create a new gameobject. It will still try to find the gameobject in the scene.
        /// </summary>
        public static T available
        {
            get
            {
                //We do not have a instance yet, try and find one in the scene.
                if (_instance == null)
                {
                    //Get all objects of the type
                    var objects = FindObjectsOfType<T>();

                    //There is no object of this type, so return null.
                    if (objects.Length == 0)
                    {
                        _instance = null;
                    }
                    else
                    {
                        //assign the object to the first element
                        _instance = objects[0];

                        //Make sure we only got one result
                        if (objects.Length > 1)
                        {
                            Debug.LogError("Singleton " + type + " has multiple instances.", _instance);
                            Debug.Break();
                        }
                    }
                }

                //Return what ever we found, even if we found nothing.
                return _instance;
            }
        }

        /// <summary>
        /// Has the instance been assigned / referenced? Will calling <see cref="instance"/> or <see cref="available"/> call a FindObjectOfType? Use this to check the validity of the instance before creating one or even locating one.
        /// <para>
        /// See also <see cref="available"/> for a nullable instance that does not create any objects.
        /// </para>
        /// </summary>
        public static bool referenced { get { return _instance != null; } }

        /// <summary>
        /// Does an instance of this singleton exist in the world? Use this to check the validity of the instance before creating one.  Short hand for available != null.
        /// <para>
        /// See also <see cref="available"/> for a nullable instance that does not create any objects.
        /// </para>
        /// </summary>
        [System.Obsolete("Use available or referenced instead", true)]
        public static bool exists { get { return available != null; } }

        /// <summary>
        /// 
        /// <para>
        ///     When Unity quits, it destroys objects in a random order.
        ///     In principle, a Singleton is only destroyed when application quits.
        /// </para>
        /// <para>
        ///     If any script calls <seealso cref="instance"/> after it have been destroyed, 
        ///     it will create a buggy ghost object that will stay on the Editor scene
        ///     when the player stops. This override prevents this from happening and must be called.
        ///</para>
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            _isquitting = true;
        }

        /// <summary>
        /// Called when the object is first initialized in the scene.
        /// <para>
        /// Singleton overrides this to ensure the object does not get destroyed on Load.
        /// </para>
        /// </summary>
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
