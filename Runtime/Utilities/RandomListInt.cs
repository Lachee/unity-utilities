using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>
    /// A randomised list. It will store a collection of values with specified weights and provide functionallity to select randomly from the list.
    /// </summary>
    /// <typeparam name="T">Type to store as the value.</typeparam>
    [System.Serializable]
    public class RandomListInt<T> : IEnumerable<T>, IRandomList<T, int>
    {
        [SerializeField] private List<T> _list;
        [SerializeField] private List<int> _weights;
        [SerializeField] private int _sumWeight = 0;

        /// <summary>
        /// Number of elements currently in the table.
        /// </summary>
        public int Count { get { return _list.Count; } }

        /// <summary>
        /// The total tally of the weights.
        /// </summary>
        public int TotalWeight { get { return _sumWeight; } }

        /// <summary>
        /// If false, then the weight will slowly decrement with every random, eventually landing on zero.
        /// </summary>
        public bool WithReplacement { get; set; } = true;

        /// <summary>
        /// Creates a new Random List
        /// </summary>
        public RandomListInt()
        {
            _list = new List<T>();
            _weights = new List<int>();
        }

        /// <summary>
        /// Creates a new Random List with a set capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public RandomListInt(int capacity)
        {
            _list = new List<T>(capacity);
            _weights = new List<int>(capacity);
        }

        public RandomListInt(IEnumerable<KeyValuePair<T, int>> collection) : this()
        {
            foreach (var kp in collection)
                Add(kp);
        }

        /// <summary>
        /// Clears the random table. 
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            _weights.Clear();
            _sumWeight = 0;
        }

        /// <summary>
        /// Adds a new item with a specified weight to the table and increments the total weight.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="weight">Non-negative weight this item has</param>
        public void Add(T item, int weight = 1)
        {
            if (weight < 0)
            {
                Debug.LogWarning("Weight for item " + item.ToString() + " is negative");
                weight = 0;
            }

            _list.Add(item);
            _weights.Add(weight);
            _sumWeight += weight;
        }

        /// <summary>Adds a new key value pair</summary>
        public void Add(KeyValuePair<T, int> pair)
        {
            Add(pair.Key, pair.Value);
        }

        /// Adds a random list to this one
        public void Add(RandomListInt<T> other)
        {
            _list.AddRange(other._list);
            _weights.AddRange(other._weights);
            _sumWeight += other._sumWeight;
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
                _sumWeight -= _weights[index];
                _weights.RemoveAt(index);
            }
        }

        /// <summary>
        /// Updates the weight of a specific item
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <param name="weight">Weight of the item</param>
        /// <returns>If the item exists and was updated</returns>
        public bool SetWeight(T item, int weight)
        {
            Debug.Assert(_list.Count == _weights.Count);
            
            var index = _list.IndexOf(item);
            if (index < 0) return false;

            _sumWeight -= _weights[index];
            _sumWeight += weight;
            _weights[index] = weight;
            return true;
        }

        /// <summary>
        /// Gets the weight of a specific item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetWeight(T item)
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
        public bool TryGetWeight(T item, out int weight)
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
                    if (!WithReplacement && _weights[i] > 0)
                        _weights[i] -= 1;
                    
                    return true;
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Recalculates the total weights
        /// </summary>
        protected int RecalculateWeights()
        {
            Debug.Assert(_list.Count == _weights.Count);

            _sumWeight = 0;
            for (int i = 0; i < _list.Count; i++) 
                _sumWeight += _weights[i];

            return _sumWeight;
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