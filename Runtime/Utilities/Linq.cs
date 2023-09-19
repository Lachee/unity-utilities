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
    public static class Linq
    {
         #region Find
        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire enumeration</summary>
        /// <param name="item">The item to search for</param>
        /// <remarks>
        /// The enumerable is searched forward starting at the first item.
        /// <para>The <see cref="object.Equals(object)"/> is used to validate equality</para>
        /// </remarks>
        /// <returns>The index. If none is found then -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> arr, T item)
            => IndexOf<T>(arr, item, 0, int.MaxValue);
        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire enumeration</summary>
        /// <param name="item">The item to search for</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <remarks>
        /// The enumerable is searched forward starting at the first item.
        /// <para>The <see cref="object.Equals(object)"/> is used to validate equality</para>
        /// </remarks>
        /// <returns>The index. If none is found then -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> arr, T item, int index)
            => IndexOf<T>(arr, item, index, int.MaxValue);
        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire enumeration</summary>
        /// <param name="item">The item to search for</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <remarks>
        /// The enumerable is searched forward starting at the first item.
        /// <para>The <see cref="object.Equals(object)"/> is used to validate equality</para>
        /// </remarks>
        /// <returns>The index. If none is found then -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> arr, T item, int index, int count)
        {
            int cnt = 0;
            foreach (var a in arr.Skip(index))
            {
                if ((a == null && item == null) || a.Equals(item))
                    return cnt + index;
                
                if (++cnt >= count) 
                    break;
            }
            return -1;

        }

        /// <summary>Searches untill the predicate is true and returns the zero-based index of the first occurrence within the entire enumeration</summary>
        /// <param name="predicate">Method invoked for each item to check for equality</param>
        /// <remarks>
        /// The enumerable is searched forward starting at the first item.
        /// </remarks>
        /// <returns>The index. If none is found then -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> arr, System.Predicate<T> predicate)
            => IndexOf(arr, predicate, 0, int.MaxValue, out var _);
        /// <summary>Searches untill the predicate is true and returns the zero-based index of the first occurrence within the entire enumeration</summary>
        /// <param name="predicate">Method invoked for each item to check for equality</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <remarks>
        /// The enumerable is searched forward starting at the first item.
        /// </remarks>
        /// <returns>The index. If none is found then -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> arr, System.Predicate<T> predicate, int index)
            => IndexOf(arr, predicate, index, int.MaxValue, out var _);
        /// <summary>Searches untill the predicate is true and returns the zero-based index of the first occurrence within the entire enumeration</summary>
        /// <param name="predicate">Method invoked for each item to check for equality</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <remarks>
        /// The enumerable is searched forward starting at the first item.
        /// </remarks>
        /// <returns>The index. If none is found then -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> arr, System.Predicate<T> predicate, int index, int count)
            => IndexOf(arr, predicate, index, count, out var _);
        /// <summary>Searches untill the predicate is true and returns the zero-based index of the first occurrence within the entire enumeration</summary>
        /// <param name="predicate">Method invoked for each item to check for equality</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="item">The item that is at the given index</param>
        /// <remarks>
        /// The enumerable is searched forward starting at the first item.
        /// </remarks>
        /// <returns>The index. If none is found then -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> arr, System.Predicate<T> predicate, int index, int count, out T item)
        {
            item = default;
            int cnt = 0;
            foreach (var a in arr.Skip(index))
            {
                if (predicate.Invoke(a))
                {
                    item = a;
                    return cnt+index;
                }

                if (++cnt >= count) 
                    break;
            }
            return -1;
        }
        #endregion
        
        #region Random
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
            var enumerator = source.GetEnumerator();

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

        ///<summary>Shuffles the list in place</summary>
        ///<remarks>The list will be updated</remarks>
        public static List<T> Shuffle<T>(this List<T> src)
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < src.Count; t++)
            {
                T tmp = src[t];
                int r = UnityEngine.Random.Range(t, src.Count);
                src[t] = src[r];
                src[r] = tmp;
            }
            return src;
        }
        ///<summary>Shuffles the array in place</summary>
        ///<remarks>The list will be updated</remarks>
        public static T[] Shuffle<T>(this T[] src)
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < src.Length; t++)
            {
                T tmp = src[t];
                int r = UnityEngine.Random.Range(t, src.Length);
                src[t] = src[r];
                src[r] = tmp;
            }
            return src;
        }
        #endregion

        #region HashSet

        /// <summary>
        /// Adds a range to the HashSet. Duplicates are ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashSet"></param>
        /// <param name="items">Enumerable list of items</param>
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
                hashSet.Add(item);
        }

        #endregion

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
