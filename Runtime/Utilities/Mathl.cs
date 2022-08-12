using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>
    /// Wet Maths a.k.a MathLake MathLachee, or MathLoony... however you want it.
    /// Bunch of static maths functions that are useful, like Cyclic Modulo
    /// </summary>
    public static class Mathl
    {
        /// <summary>
        /// Maps a value from it a range to a new range
        /// </summary>
        /// <param name="value">the value to map</param>
        /// <param name="fromMin">the minimum the value can be</param>
        /// <param name="fromMax">the maximum the value can be</param>
        /// <param name="toMin">the value minimum gets mapped into</param>
        /// <param name="toMax">the value maximum gets mapped into</param>
        /// <returns>value mapped from toMin to toMax</returns>
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax) {
            return ((value - fromMin) * (toMax - toMin)) / (fromMax - fromMin) + toMin;
        }

        /// <summary>
        /// Cyclic modulo
        /// <para>Thanks SaroVati :3</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int Mod(int x, int m) { 
            return (x % m + m) % m; 
        }
    
        /// <summary>
        /// Counts the number of bits set in the bitflag.
        /// </summary>
        /// <param name="i">The bitflag</param>
        /// <returns></returns>
        public static int BitsSet(int i) {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        /// <summary>
        /// Returns true if the other rectangle overlaps this rectangle.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="overlap">The inner overlap rectangle</param>
        /// <returns></returns>
        public static bool Overlaps(this Rect self, Rect other, out Rect overlap)
        {
            if (!self.Overlaps(other))
            {
                overlap = other;
                return false;
            }

            float left = Mathf.Max(self.xMin, other.xMin);
            float width = Mathf.Min(self.xMax, other.xMax) - left;
            float top = Mathf.Max(self.yMin, other.yMin);
            float height = Mathf.Min(self.yMax, other.yMax) - top;
            overlap = new Rect(left, top, width, height);
            return true;
        }
    
        /// <summary>
        /// Extrudes the vector a distance away from the center
        /// </summary>
        /// <param name="vector">The starting vector</param>
        /// <param name="center">The center to extrude away from</param>
        /// <param name="distance">The distance to exture</param>
        /// <returns></returns>
        public static Vector3 Extrude(this Vector3 vector, Vector3 center, float distance) {
            if (distance == 0) return vector;
            Vector3 norm = (vector-center).normalized;
            return vector + (norm * distance);
        }
    }

}