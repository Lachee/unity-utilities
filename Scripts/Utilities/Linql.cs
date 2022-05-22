using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Lachee.Utilities
{
    ///<summary>
    /// LINQ for Lachee's. Provides a collection of LINQ and Enumeration related utilities
    ///</summary>
    public static class Linql
    {
        /// <summary>
        /// Picks a random item from the enumerator by enumerating over a random amount.
        /// Do not use this on fixed length arrays or lists, as it is less efficient than a direct lookup.
        /// This isn't perfect, and using a flat lookup will have a more distributed random, but this is a useful utility regardless.
        /// For more evenly distributed random selection of items, use a Rist
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="upperBounds">The maximum length the enumeration can be. For an accurate random, this has to be >= length of source.</param>
        /// <returns>A random item</returns>
        public static TSource Random<TSource>(this IEnumerable<TSource> source, int upperBounds = int.MaxValue)
        {
            // The enumerator to iterate over
            using var enumerator = source.GetEnumerator();

            // The number of items we have found.
            // If we loop over, then we will reset the counter to a random number within this range.
            bool counting = true;
            int count = 0;

            // A random number to the upper bounds.
            int rand = UnityEngine.Random.Range(0, upperBounds);
            while (rand-- >= 0)
            {
                // Count how many items there are
                if (counting)
                    count++;

                // Move back
                if (!enumerator.MoveNext())
                {
                    // Stop counting and reset the enumerator (so it loops back)
                    counting = false;
                    enumerator.Reset();

                    // We are garuanteed to land on a item
                    rand = UnityEngine.Random.Range(0, count);
                }
            }

            // Return the item.
            return enumerator.Current;
        }

        #region Array Segments
        /// <summary>
        /// Segments and the copies the segmented data into the new array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] Cut<T>(this T[] array, int from, int count) 
            => GetSegment(array, from, count).ToArray();

        /// <summary>
        /// Creates a segment from the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ArraySegment<T> GetSegment<T>(this T[] array, int from, int count)
        {
            return new ArraySegment<T>(array, from, count);
        }

        /// <summary>
        /// Creates a segment from the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static ArraySegment<T> GetSegment<T>(this T[] array, int from)
        {
            return GetSegment(array, from, array.Length - from);
        }

        /// <summary>
        /// Creates a segment from the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ArraySegment<T> GetSegment<T>(this T[] array)
        {
            return new ArraySegment<T>(array);
        }

        /// <summary>
        /// Creates an enumeration of the array segment from the offset to the count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arraySegment"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this ArraySegment<T> arraySegment)
        {
            return arraySegment.Array.Skip(arraySegment.Offset).Take(arraySegment.Count);
        }

        /// <summary>
        /// Copies the array segment into a new array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arraySegment"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this ArraySegment<T> arraySegment)
        {
            T[] array = new T[arraySegment.Count];
            Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
            return array;
        }
        #endregion
    }
}
