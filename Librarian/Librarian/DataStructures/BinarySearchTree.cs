using System;
using System.Collections.Generic;
using Librarian.Models;

namespace Librarian.DataStructures
{
    // binary search tree for sorted resource searches
    // time complexity: O(log n) average case, O(n) worst case
    public class BinarySearchTree<T>
        where T : IComparable<T>
    {
        private class TreeNode
        {
            public T Data { get; set; }
            public TreeNode Left { get; set; }
            public TreeNode Right { get; set; }

            public TreeNode(T data)
            {
                Data = data;
                Left = null;
                Right = null;
            }
        }

        private TreeNode root;
        private int count;

        public int Count => count;

        public BinarySearchTree()
        {
            root = null;
            count = 0;
        }

        // insert new item
        // time complexity: O(log n) average case
        public void Insert(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            root = InsertRecursive(root, item);
        }

        private TreeNode InsertRecursive(TreeNode node, T item)
        {
            if (node == null)
            {
                count++;
                return new TreeNode(item);
            }

            int comparison = item.CompareTo(node.Data);
            if (comparison < 0)
            {
                node.Left = InsertRecursive(node.Left, item);
            }
            else if (comparison > 0)
            {
                node.Right = InsertRecursive(node.Right, item);
            }
            // ignore duplicates (comparison == 0)

            return node;
        }

        // search for specific item
        // time complexity: O(log n) average case
        public bool Contains(T item)
        {
            if (item == null)
                return false;
            return SearchRecursive(root, item) != null;
        }

        private TreeNode SearchRecursive(TreeNode node, T item)
        {
            if (node == null)
                return null;

            int comparison = item.CompareTo(node.Data);
            if (comparison == 0)
            {
                return node;
            }
            else if (comparison < 0)
            {
                return SearchRecursive(node.Left, item);
            }
            else
            {
                return SearchRecursive(node.Right, item);
            }
        }

        // find items within a range
        // time complexity: O(log n + k) where k is number of results
        public List<T> SearchRange(T min, T max)
        {
            if (min == null || max == null)
                return new List<T>();

            List<T> results = new List<T>();
            SearchRangeRecursive(root, min, max, results);
            return results;
        }

        private void SearchRangeRecursive(TreeNode node, T min, T max, List<T> results)
        {
            if (node == null)
                return;

            // check if current node is in range
            if (node.Data.CompareTo(min) >= 0 && node.Data.CompareTo(max) <= 0)
            {
                results.Add(node.Data);
            }

            // search left subtree if needed
            if (node.Data.CompareTo(min) > 0)
            {
                SearchRangeRecursive(node.Left, min, max, results);
            }

            // search right subtree if needed
            if (node.Data.CompareTo(max) < 0)
            {
                SearchRangeRecursive(node.Right, min, max, results);
            }
        }

        // get all items in sorted order
        // time complexity: O(n)
        public List<T> InOrderTraversal()
        {
            List<T> result = new List<T>();
            InOrderRecursive(root, result);
            return result;
        }

        private void InOrderRecursive(TreeNode node, List<T> result)
        {
            if (node != null)
            {
                InOrderRecursive(node.Left, result);
                result.Add(node.Data);
                InOrderRecursive(node.Right, result);
            }
        }

        // remove item from tree
        // time complexity: O(log n) average case
        public bool Remove(T item)
        {
            if (item == null)
                return false;

            int initialCount = count;
            root = RemoveRecursive(root, item);
            return count < initialCount;
        }

        private TreeNode RemoveRecursive(TreeNode node, T item)
        {
            if (node == null)
                return null;

            int comparison = item.CompareTo(node.Data);

            if (comparison < 0)
            {
                node.Left = RemoveRecursive(node.Left, item);
            }
            else if (comparison > 0)
            {
                node.Right = RemoveRecursive(node.Right, item);
            }
            else
            {
                // node to delete found
                count--;

                // case 1: no children
                if (node.Left == null && node.Right == null)
                {
                    return null;
                }
                // case 2: one child
                else if (node.Left == null)
                {
                    return node.Right;
                }
                else if (node.Right == null)
                {
                    return node.Left;
                }
                // case 3: two children
                else
                {
                    // find inorder successor (smallest in right subtree)
                    TreeNode successor = FindMin(node.Right);
                    node.Data = successor.Data;
                    node.Right = RemoveRecursive(node.Right, successor.Data);
                    count++; // adjust count since we decremented above
                }
            }

            return node;
        }

        private TreeNode FindMin(TreeNode node)
        {
            while (node.Left != null)
            {
                node = node.Left;
            }
            return node;
        }

        // clear all nodes
        public void Clear()
        {
            root = null;
            count = 0;
        }
    }
}
