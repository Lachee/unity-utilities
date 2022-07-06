using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>
    /// Extends GameObject functionality with a collection of utlities
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        /// Sets the layer of the game object and its children
        /// </summary>
        /// <param name="obj">The game object</param>
        /// <param name="layer">The new layer</param>
        /// <param name="mask">Ignores the object if it doesn't match the mask</param>
        /// <param name="exitEarly">Stops recursion if the object doesn't match the mask</param>
        public static void SetLayerRecursive(this GameObject obj, int layer, LayerMask mask = default(LayerMask), bool exitEarly = false)
        {
            if (!obj.SetLayer(layer, mask) && exitEarly)
                return;

            foreach(Transform child in obj.transform)
                child.gameObject.SetLayerRecursive(layer, mask);
        }

        /// <summary>
        /// Sets the layer of the game object
        /// </summary>
        /// <param name="obj">The game object</param>
        /// <param name="layer">The new layer</param>
        /// <param name="mask">Ignores the object if it doesn't match the mask</param>
        /// <returns>True if it matches and was updated</returns>
        public static bool SetLayer(this GameObject obj, int layer, LayerMask mask = default(LayerMask))
        {
            if (mask == (mask | (1 << obj.layer)))
            {
                obj.layer = layer;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Extends RectRectangle
    /// </summary>
    public static class RectTransformExtension
    {
        /// <summary>Converts this rect transform to screen space</summary>
        public static Rect ToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (size * 0.5f), size);
        }
    }

    /// <summary>
    /// Extends Transform
    /// </summary>
    public static class TransformExtension 
    {
        /// <summary>
        /// Rotates around a pivot with the given rotation
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="pivotPoint"></param>
        /// <param name="rotation"></param>
        public static void RotateAround (this Transform transform, Vector3 pivotPoint, Quaternion rotation)
        {
            transform.position = rotation * (transform.position - pivotPoint) + pivotPoint;
            transform.rotation = rotation * transform.rotation;
        }
    }

    /// <summary>
    /// Extends Particle Systems
    /// </summary>
    public static class ParticleSystemExtension
    {
        /// <summary>Sets the max number of particles</summary>
        public static void SetMaxParticles(this ParticleSystem system, int count)
        {
            // Structs are referenced!?
            var module = system.main;
            module.maxParticles = count;
        }
    }
}
