using UnityEngine;

namespace Lachee.Utilities
{
    public static class GameObjectExtensions
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

    public static class RectTransformExtensions
    {
        /// <summary>Converts this rect transform to screen space</summary>
        public static Rect ToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (size * 0.5f), size);
        }
    }
}
