using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>
    /// Weighted randomised list of items.
    /// </summary>
    /// <seealso cref="RandomList{T}"/>
    /// <typeparam name="T"></typeparam>
    [System.Obsolete("Rist has been renamed to RandomList")]
    public class Rist<T> : RandomList<T> { }

    /// <summary>
    /// A randomised list. It will store a collection of values with specified weights and provide functionallity to select randomly from the list.
    /// </summary>
    /// <typeparam name="T">Type to store as the value.</typeparam>
    [System.Serializable]
    public class RandomList<T> : IEnumerable<T>
    {
        private List<T> _list;
        private List<float> _weights;
        private float _weight = 0;

        /// <summary>
        /// Number of elements currently in the table.
        /// </summary>
        public int Count { get { return _list.Count; } }

        /// <summary>
        /// The total tally of the weights.
        /// </summary>
        public float TotalWeight { get { return _weight; } }

        /// <summary>
        /// Creates a new Random List
        /// </summary>
        public RandomList()
        {
            _list = new List<T>();
            _weights = new List<float>();
        }

        /// <summary>
        /// Creates a new Random List with a set capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public RandomList(int capacity)
        {
            _list = new List<T>(capacity);
            _weights = new List<float>(capacity);
        }

        /// <summary>
        /// Clears the random table. 
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            _weights.Clear();
            _weight = 0;
        }

        /// <summary>
        /// Adds a new item with a specified weight to the table and increments the total weight.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="weight">Weight this item has</param>
        public void Add(T item, float weight = 1.0f)
        {
            _list.Add(item);
            _weights.Add(weight);
            _weight += weight;
        }

        /// <summary>
        /// Removes an item and its weight
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void Remove(T item)
        {
            Debug.Assert(_list.Count == _weights.Count);

            var index = _list.IndexOf(item);
            if (index >= 0)
            {
                _list.RemoveAt(index);
                _weight -= _weights[index];
                _weights.RemoveAt(index);
            }
        }

        /// <summary>
        /// Updates the weight of a specific item
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <param name="weight">Weight of the item</param>
        /// <returns>If the item exists and was updated</returns>
        public bool SetWeight(T item, float weight)
        {
            Debug.Assert(_list.Count == _weights.Count);
            
            var index = _list.IndexOf(item);
            if (index < 0) return false;

            _weight -= _weights[index];
            _weight += weight;
            _weights[index] = weight;
            return true;
        }

        /// <summary>
        /// Gets the weight of a specific item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public float GetWeight(T item)
        {
            var index = _list.IndexOf(item);
            if (index < 0)
                throw new System.ArgumentOutOfRangeException("item", "The item does not exist within the collection");
            return _weights[index];
        }

        /// <summary>
        /// Tries to get the weight of a specific item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool TryGetWeight(T item, out float weight)
        {
            weight = 0;
            var index = _list.IndexOf(item);
            if (index < 0) return false;
            weight = _weights[index];
            return true;
        }

        /// <summary>
        /// Attempts to pick a random element from the table based of weighting.
        /// </summary>
        /// <param name="random">A random value between 0 and 1. This is done so System.Random or Unity.Random can be used.</param>
        /// <param name="result">The random element that was fetched.</param>
        /// <returns>false if we are unable to find a random element.</returns>
        public bool Randomise(float random, out T result)
        {
            Debug.Assert(_list.Count == _weights.Count);

            float rand = random * TotalWeight;
            for (int i = 0; i < _list.Count; i++)
            {
                rand -= _weights[i];
                if (rand < 0)
                {
                    result = _list[i];
                    return true;
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Recalculates the total weights
        /// </summary>
        protected float RecalculateWeights()
        {
            Debug.Assert(_list.Count == _weights.Count);

            _weight = 0;
            for (int i = 0; i < _list.Count; i++) 
                _weight += _weights[i];

            return _weight;
        }

        /// <summary>
        /// Gets a enumerator of items in the Rist. Does not apply the random function too it.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets a enumerator of items in the Rist. Does not apply the random function too it.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}