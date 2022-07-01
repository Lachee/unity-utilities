using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Lachee.Utilities.Serialization 
{
    ///<summary>Binary Formatter with support for Unity types.
    /// Current supports:
    ///     - Vector2, Vector3, Vector4
    ///     - Color
    ///     - Quaternion
    ///     - Texture2D
    ///</summary>
    ///<remarks>
    /// This class is intended for simple saving game state and not to replace Unity's serialization. This means this will be
    /// limited to basic data types and not more complicated structures like GameObject or MonoBehaviour.
    ///</remarks>
    ///<see>https://forum.unity.com/threads/vector3-is-not-marked-serializable.435303/#post-2814558</see>
    ///<example>
    ///var binaryFormatter = new BinaryFormatter();
    ///BinaryFormatterSurrogate.Use(binaryFormatter);
    ///binaryFormatter.Serialize(MyObject);
    ///</example>
    public static class BinaryFormatterSurrogate
    {
        ///<summary>Creates the surrogates to serialize unity types</summary>
        public static SurrogateSelector Create() {
            SurrogateSelector surrogateSelector = new SurrogateSelector();

            surrogateSelector.AddSurrogate(typeof(Vector2),new StreamingContext(StreamingContextStates.All), new VectorSurrogate(2));
            surrogateSelector.AddSurrogate(typeof(Vector3),new StreamingContext(StreamingContextStates.All), new VectorSurrogate(3));
            surrogateSelector.AddSurrogate(typeof(Vector4),new StreamingContext(StreamingContextStates.All), new VectorSurrogate(4));
            surrogateSelector.AddSurrogate(typeof(Quaternion),new StreamingContext(StreamingContextStates.All), new VectorSurrogate(4));
            surrogateSelector.AddSurrogate(typeof(Color),new StreamingContext(StreamingContextStates.All), new ColorSurrogate());
            surrogateSelector.AddSurrogate(typeof(Texture2D), new StreamingContext(StreamingContextStates.All), new Texture2DSurrogate());

            return surrogateSelector;
        }

        /// <summary>
        /// Shortcut method that just applies the selector to the formatter.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static SurrogateSelector Use(BinaryFormatter formatter) {
            var selector = Create();
            formatter.SurrogateSelector = selector;
            return selector;
        }
    }
} 