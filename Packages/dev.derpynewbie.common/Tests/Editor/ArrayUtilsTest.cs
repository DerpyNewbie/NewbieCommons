using NUnit.Framework;

namespace DerpyNewbie.Common.Tests.Editor
{
    public class ArrayUtilsTest
    {
        private string[] _preMadeStrings;
        private string[] _preMadeWithDuplicate;
        private string[] _preMadeWithNull;
        private string[] _strings;

        [SetUp]
        public void SetUp()
        {
            _strings = new string[0];
            _preMadeStrings = new[] { "a", "b", "c", "d", "e" };
            _preMadeWithNull = new[] { "a", "b", null, "d", "e" };
            _preMadeWithDuplicate = new[] { "a", "a", "a", "b", "c", "c" };
        }

        [TearDown]
        public void TearDown()
        {
            _strings = null;
            _preMadeStrings = null;
            _preMadeWithNull = null;
            _preMadeWithDuplicate = null;
        }

        // A Test behaves as an ordinary method
        [Test]
        public void AddAsListTest()
        {
            // add on empty array
            _strings = _strings.AddAsList("a");

            Assert.That(_strings.Length == 1);
            Assert.That(_strings[0] == "a");

            // add on existing array
            _preMadeStrings = _preMadeStrings.AddAsList("f");

            Assert.That(_preMadeStrings.Length == 6);
            Assert.That(_preMadeStrings[5] == "f");

            // add on null-containing array
            _preMadeWithNull = _preMadeWithNull.AddAsList("f");

            Assert.That(_preMadeWithNull.Length == 6);
            Assert.That(_preMadeWithNull[5] == "f");

            // add an existing item
            _preMadeStrings = _preMadeStrings.AddAsList("a");

            Assert.That(_preMadeStrings.Length == 7);
            Assert.That(_preMadeStrings[0] == "a");
            Assert.That(_preMadeStrings[6] == "a");

            // add an null as item
            _preMadeWithNull = _preMadeWithNull.AddAsList(null);

            Assert.That(_preMadeWithNull.Length == 7);
            Assert.That(_preMadeWithNull[0] == "a");
            Assert.That(_preMadeWithNull[6] == null);
        }

        [Test]
        public void InsertAtFirstTest()
        {
            _strings = _strings.InsertItemAtIndex(0, "a", out var result);

            Assert.That(result, Is.True, "insert an empty array at 0: Result");
            Assert.That(_strings.Length, Is.EqualTo(1), "insert an empty array at 0: Length");
            Assert.That(_strings[0], Is.SameAs("a"), "insert an empty array at 0: Value");

            _preMadeStrings = _preMadeStrings.InsertItemAtIndex(0, "f", out result);

            Assert.That(result, Is.True, "insert on existing array at 0: Result");
            Assert.That(_preMadeStrings.Length, Is.EqualTo(6), "insert on existing array at 0: Length");
            Assert.That(_preMadeStrings[0], Is.SameAs("f"), "insert on existing array at 0: Value");
            Assert.That(_preMadeStrings[1], Is.SameAs("a"), "insert on existing array at 0: Original Value");
        }

        [Test]
        public void InsertAtLastTest()
        {
            _preMadeStrings = _preMadeStrings.InsertItemAtIndex(_preMadeStrings.Length, "g", out var result);

            Assert.That(result, Is.True, "insert on existing array at last: Result");
            Assert.That(_preMadeStrings.Length, Is.EqualTo(6), "insert on existing array at last: Length");
            Assert.That(_preMadeStrings[_preMadeStrings.Length - 1], Is.SameAs("g"),
                "insert on existing array at last: Value");
            Assert.That(_preMadeStrings[_preMadeStrings.Length - 2], Is.SameAs("e"),
                "insert on existing array at last: Original Value");
        }

        [Test]
        public void InsertAtMiddleTest()
        {
            _preMadeStrings = _preMadeStrings.InsertItemAtIndex(3, "g", out var result);

            Assert.That(result, Is.True, "insert on existing array at middle: Result");
            Assert.That(_preMadeStrings.Length, Is.EqualTo(6), "insert on existing array at middle: Length");
            Assert.That(_preMadeStrings[3], Is.SameAs("g"), "insert on existing array at middle: Value");
            Assert.That(_preMadeStrings[2], Is.SameAs("c"),
                "insert on existing array at middle: Original Value Before");
            Assert.That(_preMadeStrings[4], Is.SameAs("d"),
                "insert on existing array at middle: Original Value After");
        }

        [Test]
        public void InsertAtOutOfRangeTest()
        {
            _preMadeStrings = _preMadeStrings.InsertItemAtIndex(_preMadeStrings.Length + 1, "g", out var result);

            Assert.That(result, Is.False, "Should return false when provided index was > arr.Length");
            Assert.That(_preMadeStrings.Length, Is.EqualTo(5), "Length should not be changed when result is false@upper");

            _preMadeStrings = _preMadeStrings.InsertItemAtIndex(-1, "g", out result);
            
            Assert.That(result, Is.False, "Should return false when provided index was < 0");
            Assert.That(_preMadeStrings.Length, Is.EqualTo(5), "Length should not be change when result is false@lower");
        }

        [Test]
        public void RemoveAtFirstTest()
        {
            _preMadeStrings = _preMadeStrings.RemoveItemAtIndex(0, out var result);
            
            Assert.IsTrue(result, "Should be able to remove item at 0");
            Assert.AreSame("b", _preMadeStrings[0] , "All values should be moved down by one");
        }

        [Test]
        public void RemoveAtMiddleTest()
        {
            _preMadeStrings = _preMadeStrings.RemoveItemAtIndex(2, out var result);
            
            Assert.IsTrue(result, "Should be able to remove item at 3");
            Assert.AreSame("b", _preMadeStrings[1], "Values < index should not be moved");
            Assert.AreSame("d", _preMadeStrings[2], "Values >= index should be moved down by one");
        }

        [Test]
        public void RemoveItemTest()
        {
            // Remove middle
            _preMadeStrings = _preMadeStrings.RemoveItem("b", out var result);

            Assert.That(result);
            Assert.That(_preMadeStrings.Length, Is.EqualTo(4));
            Assert.That(_preMadeStrings[0], Is.SameAs("a"));
            Assert.That(_preMadeStrings[1], Is.SameAs("c"));

            // Remove nothing
            _preMadeStrings = _preMadeStrings.RemoveItem("none", out result);

            Assert.That(result, Is.False);
            Assert.That(_preMadeStrings.Length, Is.EqualTo(4));

            // Remove from first
            _preMadeStrings = _preMadeStrings.RemoveItem("a", out result);

            Assert.That(result, Is.True);
            Assert.That(_preMadeStrings.Length, Is.EqualTo(3));
            Assert.That(_preMadeStrings[0], Is.SameAs("c"));

            // Remove from last
            _preMadeStrings = _preMadeStrings.RemoveItem("e", out result);

            Assert.That(result, Is.True);
            Assert.That(_preMadeStrings.Length, Is.EqualTo(2));
            Assert.That(_preMadeStrings[1], Is.SameAs("d"));

            // Remove from one
            _preMadeStrings = _preMadeStrings.RemoveItem("c", out result).RemoveItem("d", out result);

            Assert.That(result, Is.True);
            Assert.That(_preMadeStrings.Length, Is.Zero);

            // Remove from empty
            _strings = _strings.RemoveItem("invalid", out result);

            Assert.That(!result);
            Assert.That(_strings.Length, Is.Zero);
        }

        [Test]
        public void ContainsItemTest()
        {
            // do have "a" in array
            Assert.That(_preMadeStrings.ContainsItem("a"), Is.True);
            // does not have "no" in array
            Assert.That(_preMadeStrings.ContainsItem("no"), Is.False);
            // does not have "null" in array
            Assert.That(_preMadeStrings.ContainsItem(null), Is.False);

            // does not have "invalid" in empty array
            Assert.That(_strings.ContainsItem("invalid"), Is.False);
            // does not have "null" in empty array
            Assert.That(_strings.ContainsItem(null), Is.False);
        }

        [Test]
        public void FindItemTest()
        {
            // do have "b" in array at index of 1
            Assert.That(_preMadeStrings.FindItem("a"), Is.EqualTo(0));
            Assert.That(_preMadeStrings.FindItem("b"), Is.EqualTo(1));
            Assert.That(_preMadeStrings.FindItem("c"), Is.EqualTo(2));

            // find from first
            Assert.That(_preMadeWithDuplicate.FindItem("a"), Is.EqualTo(0));
            Assert.That(_preMadeWithDuplicate.FindItem("c"), Is.EqualTo(4));

            // do not have
            Assert.That(_preMadeStrings.FindItem("invalid"), Is.EqualTo(-1));
            Assert.That(_preMadeStrings.FindItem(null), Is.EqualTo(-1));

            Assert.That(_strings.FindItem("invalid"), Is.EqualTo(-1));
            Assert.That(_strings.FindItem(null), Is.EqualTo(-1));
        }
    }
}