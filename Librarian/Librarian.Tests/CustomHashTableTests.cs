using Librarian.DataStructures;
using Xunit;

namespace Librarian.Tests
{
    public class CustomHashTableTests
    {
        [Fact]
        public void Put_ValidKeyValue_ShouldStoreSuccessfully()
        {
            var hashTable = new CustomHashTable<string, int>();

            hashTable.Put("key1", 100);

            Assert.Equal(100, hashTable.Get("key1"));
            Assert.Equal(1, hashTable.Count);
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            var hashTable = new CustomHashTable<string, int>();

            Assert.Throws<KeyNotFoundException>(() => hashTable.Get("nonexistent"));
        }

        [Fact]
        public void ContainsKey_ExistingKey_ShouldReturnTrue()
        {
            var hashTable = new CustomHashTable<string, int>();
            hashTable.Put("test", 42);

            Assert.True(hashTable.ContainsKey("test"));
        }

        [Fact]
        public void ContainsKey_NonExistentKey_ShouldReturnFalse()
        {
            var hashTable = new CustomHashTable<string, int>();

            Assert.False(hashTable.ContainsKey("missing"));
        }

        [Fact]
        public void Remove_ExistingKey_ShouldReturnTrueAndRemoveItem()
        {
            var hashTable = new CustomHashTable<string, int>();
            hashTable.Put("remove_me", 123);

            bool removed = hashTable.Remove("remove_me");

            Assert.True(removed);
            Assert.False(hashTable.ContainsKey("remove_me"));
            Assert.Equal(0, hashTable.Count);
        }

        [Fact]
        public void Put_UpdateExistingKey_ShouldUpdateValue()
        {
            var hashTable = new CustomHashTable<string, int>();
            hashTable.Put("update", 1);

            hashTable.Put("update", 999);

            Assert.Equal(999, hashTable.Get("update"));
            Assert.Equal(1, hashTable.Count);
        }

        [Fact]
        public void GetAllValues_MultipleItems_ShouldReturnAllValues()
        {
            var hashTable = new CustomHashTable<string, int>();
            hashTable.Put("a", 1);
            hashTable.Put("b", 2);
            hashTable.Put("c", 3);

            var values = hashTable.GetAllValues().ToList();

            Assert.Equal(3, values.Count);
            Assert.Contains(1, values);
            Assert.Contains(2, values);
            Assert.Contains(3, values);
        }
    }
}
