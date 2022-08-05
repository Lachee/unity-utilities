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
    }
}