using System;
using System.Collections.Generic;
using Librarian.Models;

namespace Librarian.DataStructures
{
    // custom hash table implementation for fast resource lookup
    // time complexity: O(1) average case, O(n) worst case
    public class CustomHashTable<TKey, TValue>
    {
        private class HashNode
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public HashNode Next { get; set; }

            public HashNode(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Next = null;
            }
        }

        private HashNode[] buckets;
        private int size;
        private int count;
        private const double LoadFactor = 0.75;

        public int Count => count;

        public CustomHashTable(int initialCapacity = 16)
        {
            size = initialCapacity;
            buckets = new HashNode[size];
            count = 0;
        }

        // simple hash function for integers and strings
        private int GetHashCode(TKey key)
        {
            if (key == null)
                return 0;

            int hash = key.GetHashCode();
            return Math.Abs(hash) % size;
        }

        // add or update key-value pair
        // time complexity: O(1) average case
        public void Put(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int index = GetHashCode(key);
            HashNode current = buckets[index];

            // check if key already exists
            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    current.Value = value; // update existing
                    return;
                }
                current = current.Next;
            }

            // add new node at beginning of chain
            HashNode newNode = new HashNode(key, value);
            newNode.Next = buckets[index];
            buckets[index] = newNode;
            count++;

            // resize if load factor exceeded
            if ((double)count / size > LoadFactor)
            {
                Resize();
            }
        }

        // get value by key
        // time complexity: O(1) average case
        public TValue Get(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int index = GetHashCode(key);
            HashNode current = buckets[index];

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    return current.Value;
                }
                current = current.Next;
            }

            throw new KeyNotFoundException($"Key '{key}' not found");
        }

        // check if key exists
        public bool ContainsKey(TKey key)
        {
            if (key == null)
                return false;

            int index = GetHashCode(key);
            HashNode current = buckets[index];

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    return true;
                }
                current = current.Next;
            }

            return false;
        }

        // remove key-value pair
        // time complexity: O(1) average case
        public bool Remove(TKey key)
        {
            if (key == null)
                return false;

            int index = GetHashCode(key);
            HashNode current = buckets[index];
            HashNode previous = null;

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    if (previous == null)
                    {
                        buckets[index] = current.Next;
                    }
                    else
                    {
                        previous.Next = current.Next;
                    }
                    count--;
                    return true;
                }
                previous = current;
                current = current.Next;
            }

            return false;
        }

        // get all values for iteration
        public IEnumerable<TValue> GetAllValues()
        {
            for (int i = 0; i < size; i++)
            {
                HashNode current = buckets[i];
                while (current != null)
                {
                    yield return current.Value;
                    current = current.Next;
                }
            }
        }

        // resize hash table when load factor exceeded
        private void Resize()
        {
            HashNode[] oldBuckets = buckets;
            int oldSize = size;

            size = size * 2;
            buckets = new HashNode[size];
            count = 0;

            // rehash all existing nodes
            for (int i = 0; i < oldSize; i++)
            {
                HashNode current = oldBuckets[i];
                while (current != null)
                {
                    Put(current.Key, current.Value);
                    current = current.Next;
                }
            }
        }
    }
}
