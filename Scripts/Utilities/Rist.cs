using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>
    /// A randomised list. It will store a collection of values with specified weights and provide functionallity to select randomly from the list.
    /// </summary>
    /// <typeparam name="T">Type to store as the value.</typeparam>
    public class Rist<T> : IEnumerable<T>
    {
        private List<T> _list;
        private List<float> _weights;
        private float _weight = 0;

        /// <summary>
        /// Number of elements currently in the table.
        /// </summary>
        public int Count { get { return _list.Count; } }

        /// <summary>
        /// The total tally of the weights. Use RecalculateWeights(); to update this value.
        /// </summary>
        public float TotalWeight { get { return _weight; } }

        /// <summary>
        /// Creates a new Random List
        /// </summary>
        public Rist()
        {
            _list = new List<T>();
            _weights = new List<float>();
        }

        /// <summary>
        /// Creates a new Random List with a set capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public Rist(int capacity)
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
        public float RecalculateWeights()
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