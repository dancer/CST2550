using Librarian.DataStructures;
using Xunit;

namespace Librarian.Tests
{
    public class BinarySearchTreeTests
    {
        [Fact]
        public void Insert_ValidItems_ShouldIncreaseCount()
        {
            var tree = new BinarySearchTree<int>();

            tree.Insert(10);
            tree.Insert(5);
            tree.Insert(15);

            Assert.Equal(3, tree.Count);
        }

        [Fact]
        public void Contains_ExistingItem_ShouldReturnTrue()
        {
            var tree = new BinarySearchTree<int>();
            tree.Insert(42);

            Assert.True(tree.Contains(42));
        }

        [Fact]
        public void Contains_NonExistentItem_ShouldReturnFalse()
        {
            var tree = new BinarySearchTree<int>();
            tree.Insert(10);

            Assert.False(tree.Contains(99));
        }

        [Fact]
        public void InOrderTraversal_ShouldReturnSortedOrder()
        {
            var tree = new BinarySearchTree<int>();
            int[] values = { 50, 30, 70, 20, 40, 60, 80 };

            foreach (int value in values)
            {
                tree.Insert(value);
            }

            var sorted = tree.InOrderTraversal();
            var expected = new[] { 20, 30, 40, 50, 60, 70, 80 };

            Assert.Equal(expected, sorted);
        }

        [Fact]
        public void SearchRange_ValidRange_ShouldReturnItemsInRange()
        {
            var tree = new BinarySearchTree<int>();
            int[] values = { 10, 5, 15, 3, 7, 12, 18 };

            foreach (int value in values)
            {
                tree.Insert(value);
            }

            var rangeResults = tree.SearchRange(6, 14);

            Assert.Contains(7, rangeResults);
            Assert.Contains(10, rangeResults);
            Assert.Contains(12, rangeResults);
            Assert.DoesNotContain(5, rangeResults);
            Assert.DoesNotContain(15, rangeResults);
        }

        [Fact]
        public void Remove_ExistingItem_ShouldReturnTrueAndDecreaseCount()
        {
            var tree = new BinarySearchTree<int>();
            tree.Insert(100);
            tree.Insert(50);

            bool removed = tree.Remove(50);

            Assert.True(removed);
            Assert.Equal(1, tree.Count);
            Assert.False(tree.Contains(50));
        }

        [Fact]
        public void Clear_ShouldRemoveAllItems()
        {
            var tree = new BinarySearchTree<int>();
            tree.Insert(1);
            tree.Insert(2);
            tree.Insert(3);

            tree.Clear();

            Assert.Equal(0, tree.Count);
            Assert.False(tree.Contains(1));
        }
    }
}
