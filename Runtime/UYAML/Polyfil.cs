using System.Collections.Generic;

namespace Lachee.UYAML
{
    #if !NET_STANDARD
    internal static class NetStandardPolyfill {
        public static bool TryPop<T>(this Stack<T> stack, out T result) {
            result = default;
            if (stack.Count == 0)
                return false;
            result = stack.Pop();
            return true;
        }

        public static bool TryAdd<T, K>(this Dictionary<T, K> dict, T key, K value) {
            if (dict.ContainsKey(key))
                return false;
            dict.Add(key, value);
            return true;
        }

        public static string[] Split(this string str, char separator, int count)
        {
            return str.Split(new char[] { separator }, count);
        }
    }
    #endif

    #if UNITY_5_3_OR_NEWER 
    public static class UExtensions {
        /// <summary>
        /// Replaces all fileIDs that match the given mapping
        /// </summary>
        /// <param name="property"></param>
        /// <param name="map"></param>
        public static void ReplaceFileID(this UProperty property, Dictionary<long, long> map) {
            if (property.value is UObject obj)
            {
                foreach (var kp in obj.properties.Values)
                    ReplaceFileID(kp, map);
            }
            else if (property.value is UArray arr)
            {
                foreach (var item in arr.items)
                    ReplaceFileID(new UProperty(string.Empty, item), map);
            } 
            else if (property.name == "fileID" && property.value is UValue value && long.TryParse(value.value, out var fileID))
            {
                if (map.TryGetValue(fileID, out var dest)) {
                    value.value = dest.ToString();
                }
            } 
   
        }

        /// <summary>
        /// Replaces all GUIDs inside the map.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="map"></param>
        public static void ReplaceGUID(this UProperty property, Dictionary<string, string> map) {
            if (property.name == "guid" && property.value is UValue value)
            {
                if (map.TryGetValue(value.value, out var dest)) 
                    value.value = dest.ToString();
            } 
            else if (property.value is UObject obj)
            {
                foreach (var kp in obj.properties.Values)
                    ReplaceGUID(kp, map);
            }
            else if (property.value is UArray arr)
            {
                foreach (var item in arr.items)
                    ReplaceGUID(new UProperty(string.Empty, item), map);
            }
        }
       
    
    }
    #endif
}
