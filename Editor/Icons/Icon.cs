using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Lachee.Editor.Icons
{
    internal static class Icon
    {
        private const string BASE_PATH = "Packages/com.lachee.utilities/Editor/Icons/";
        private static Dictionary<string, Texture> _cache = new Dictionary<string, Texture>(6);

        public static Texture equals    => Load("equals_100");
        public static Texture link      => Load("auto_32");
        public static Texture link_sm   => Load("auto_16");
        public static Texture rotate    => Load("rotate");
        public static Texture singleton => Load("singleton");

        public static Texture ristBar    => Load("ristbar");
        public static Texture @namespace => Load("NamespaceConfigurationIcon");

        /// <summary>
        /// Gets a texture and stores it in a cache if requried
        /// </summary>
        /// <param name="name"></param>
        /// <param name="recache"></param>
        /// <returns></returns>
        public static Texture Load(string name, bool recache = false)
        {
            //Check the cache
            if (!recache && _cache.TryGetValue(name, out var t))
                return t;


            //Load the file
            string filePath = BASE_PATH + name;
            if (string.IsNullOrEmpty(Path.GetExtension(filePath)))
                filePath += ".png";

            if (!File.Exists(filePath))
            {
                Debug.LogError("The icon " + name + " does not exist!");
                return null;
            }

            //Load the texture and store
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(filePath);
            _cache[name] = texture;
            return texture;
        }

        /// <summary>
        /// Draws a preview of a sprite
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sprite"></param>
        public static void DrawSpritePreview(Rect position, Sprite sprite)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
    }
}