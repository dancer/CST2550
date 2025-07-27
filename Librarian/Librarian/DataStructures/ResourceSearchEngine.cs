using System;
using System.Collections.Generic;
using System.Linq;
using Librarian.Models;

namespace Librarian.DataStructures
{
    // specialized search engine for library resources
    // combines multiple data structures for efficient searching
    public class ResourceSearchEngine
    {
        // primary storage - fast lookup by id
        private readonly CustomHashTable<int, LibraryResource> resourcesById;

        // secondary indexes for different search types
        private readonly CustomHashTable<string, List<LibraryResource>> resourcesByTitle;
        private readonly CustomHashTable<string, List<LibraryResource>> resourcesByAuthor;
        private readonly CustomHashTable<string, List<LibraryResource>> resourcesByGenre;

        // sorted tree for range searches
        private readonly BinarySearchTree<ResourceWrapper> sortedByTitle;

        public int Count => resourcesById.Count;

        public ResourceSearchEngine()
        {
            resourcesById = new CustomHashTable<int, LibraryResource>();
            resourcesByTitle = new CustomHashTable<string, List<LibraryResource>>();
            resourcesByAuthor = new CustomHashTable<string, List<LibraryResource>>();
            resourcesByGenre = new CustomHashTable<string, List<LibraryResource>>();
            sortedByTitle = new BinarySearchTree<ResourceWrapper>();
        }

        // add resource to all indexes
        // time complexity: O(log n) due to tree insertion
        public void AddResource(LibraryResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            // add to primary index
            resourcesById.Put(resource.Id, resource);

            // add to secondary indexes
            AddToIndex(resourcesByTitle, resource.Title.ToLower(), resource);
            AddToIndex(resourcesByAuthor, resource.Author.ToLower(), resource);
            AddToIndex(resourcesByGenre, resource.Genre.ToLower(), resource);

            // add to sorted tree
            sortedByTitle.Insert(new ResourceWrapper(resource));
        }

        // remove resource from all indexes
        // time complexity: O(log n)
        public bool RemoveResource(int id)
        {
            if (!resourcesById.ContainsKey(id))
                return false;

            LibraryResource resource = resourcesById.Get(id);

            // remove from primary index
            resourcesById.Remove(id);

            // remove from secondary indexes
            RemoveFromIndex(resourcesByTitle, resource.Title.ToLower(), resource);
            RemoveFromIndex(resourcesByAuthor, resource.Author.ToLower(), resource);
            RemoveFromIndex(resourcesByGenre, resource.Genre.ToLower(), resource);

            // remove from sorted tree
            sortedByTitle.Remove(new ResourceWrapper(resource));

            return true;
        }

        // get resource by id
        // time complexity: O(1)
        public LibraryResource GetById(int id)
        {
            return resourcesById.Get(id);
        }

        // check if resource exists
        // time complexity: O(1)
        public bool Contains(int id)
        {
            return resourcesById.ContainsKey(id);
        }

        // search by title - exact match
        // time complexity: O(1) average case
        public List<LibraryResource> SearchByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                return new List<LibraryResource>();

            string key = title.ToLower();
            return resourcesByTitle.ContainsKey(key)
                ? resourcesByTitle.Get(key)
                : new List<LibraryResource>();
        }

        // search by title - partial match
        // time complexity: O(n) - needs to check all resources
        public List<LibraryResource> SearchByTitlePartial(string partialTitle)
        {
            if (string.IsNullOrEmpty(partialTitle))
                return new List<LibraryResource>();

            List<LibraryResource> results = new List<LibraryResource>();
            string searchTerm = partialTitle.ToLower();

            foreach (LibraryResource resource in GetAllResources())
            {
                if (resource.Title.ToLower().Contains(searchTerm))
                {
                    results.Add(resource);
                }
            }

            return results;
        }

        // search by author
        // time complexity: O(1) average case
        public List<LibraryResource> SearchByAuthor(string author)
        {
            if (string.IsNullOrEmpty(author))
                return new List<LibraryResource>();

            string key = author.ToLower();
            return resourcesByAuthor.ContainsKey(key)
                ? resourcesByAuthor.Get(key)
                : new List<LibraryResource>();
        }

        // search by genre
        // time complexity: O(1) average case
        public List<LibraryResource> SearchByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
                return new List<LibraryResource>();

            string key = genre.ToLower();
            return resourcesByGenre.ContainsKey(key)
                ? resourcesByGenre.Get(key)
                : new List<LibraryResource>();
        }

        // get all resources sorted by title
        // time complexity: O(n)
        public List<LibraryResource> GetAllResourcesSorted()
        {
            return sortedByTitle.InOrderTraversal().Select(wrapper => wrapper.Resource).ToList();
        }

        // get all resources (unsorted)
        // time complexity: O(n)
        public List<LibraryResource> GetAllResources()
        {
            return resourcesById.GetAllValues().ToList();
        }

        // search resources by publication year range
        // time complexity: O(n) - linear search through all resources
        public List<LibraryResource> SearchByYearRange(int minYear, int maxYear)
        {
            List<LibraryResource> results = new List<LibraryResource>();

            foreach (LibraryResource resource in GetAllResources())
            {
                if (resource.PublicationYear >= minYear && resource.PublicationYear <= maxYear)
                {
                    results.Add(resource);
                }
            }

            return results;
        }

        // helper method to add resource to secondary index
        private void AddToIndex(
            CustomHashTable<string, List<LibraryResource>> index,
            string key,
            LibraryResource resource
        )
        {
            if (index.ContainsKey(key))
            {
                index.Get(key).Add(resource);
            }
            else
            {
                List<LibraryResource> list = new List<LibraryResource> { resource };
                index.Put(key, list);
            }
        }

        // helper method to remove resource from secondary index
        private void RemoveFromIndex(
            CustomHashTable<string, List<LibraryResource>> index,
            string key,
            LibraryResource resource
        )
        {
            if (index.ContainsKey(key))
            {
                List<LibraryResource> list = index.Get(key);
                list.RemoveAll(r => r.Id == resource.Id);
                if (list.Count == 0)
                {
                    index.Remove(key);
                }
            }
        }

        // wrapper class for tree comparison by title
        private class ResourceWrapper : IComparable<ResourceWrapper>
        {
            public LibraryResource Resource { get; }

            public ResourceWrapper(LibraryResource resource)
            {
                Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            }

            public int CompareTo(ResourceWrapper other)
            {
                if (other == null)
                    return 1;
                return string.Compare(
                    Resource.Title,
                    other.Resource.Title,
                    StringComparison.OrdinalIgnoreCase
                );
            }
        }
    }
}
