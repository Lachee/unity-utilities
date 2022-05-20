using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>Instantiates an object in a particular way.</summary>
    public class Instantiator<T> where T : Object  
    {
        public GameObject prefab { get; }
        public Transform parent { get; set; }
        public Vector3 localPosition { get; set; }

        private System.Func<GameObject, T> callback;

        public Instantiator(GameObject prefab, System.Func<GameObject, T> callback) {
            this.prefab = prefab;
            this.callback = callback;
        }

        /// <summary>Spawns the object and returns the result of the post event</summary>
        public T Instantiate()
        {
            // Create the instance
            var instance = GameObject.Instantiate(prefab);

            // Set the parent
            if (parent != null)
                instance.transform.SetParent(parent);

            // Set local coordinate
            instance.transform.localPosition = localPosition;

            // Invoke the post event
            return callback?.Invoke(instance);
        }
    }
}
