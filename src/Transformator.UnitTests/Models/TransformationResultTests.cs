using NUnit.Framework;
using Transformator.Models;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests.Models
{
    [TestFixture]
    public class TransformationResultTests
    {
        TransformationResult<Foo> _transformationResult;

        [SetUp]
        public void Setup()
        {
            _transformationResult = new TransformationResult<Foo>();
        }

        [Test]
        public void DestinationProperty_ContainsNotNullAndEmptyListAfterInstantiation()
        {
            Assert.NotNull(_transformationResult.Destinations);
            Assert.AreEqual(0, _transformationResult.Destinations.Count);
        }

        [Test]
        public void HasNotNullDestination_NoNotNullItems_ReturnFalse()
        {
            _transformationResult.Add(null);
            _transformationResult.Add(null);

            var result = _transformationResult.HasNotNullDestination();

            Assert.IsFalse(result);
        }

        [Test]
        public void HasNotNullDestination_WithNotNullItems_ReturnTrue()
        {
            _transformationResult.Add(null);
            _transformationResult.Add(new Foo());

            var result = _transformationResult.HasNotNullDestination();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsEmpty_ListIsEmpty_ReturnTrue()
        {
            _transformationResult.Destinations.Clear();

            var result = _transformationResult.IsEmpty();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsEmpty_ListIsNotEmpty_ReturnFalse()
        {
            _transformationResult.Add(new Foo());

            var result = _transformationResult.IsEmpty();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsEmpty_ListContainsOnlyNullItems_ReturnFalse()
        {
            _transformationResult.Add(null);
            _transformationResult.Add(null);

            var result = _transformationResult.IsEmpty();

            Assert.IsFalse(result);
        }

        [Test]
        public void Add_ItemAddedToTheList()
        {
            var item = new Foo();

            _transformationResult.Add(item);

            Assert.AreEqual(1, _transformationResult.Destinations.Count);
            Assert.AreEqual(item, _transformationResult.Destinations[0]);
        }

        [Test]
        public void GetSingleOrDefault_NoItemsInList_ReturnNull()
        {
            _transformationResult.Destinations.Clear();

            var result = _transformationResult.GetSingleOrDefault();

            Assert.IsNull(result);
        }

        [Test]
        public void GetSingleOrDefault_OneItemInList_ReturnThisSinglelItem()
        {
            var item = new Foo();

            _transformationResult.Add(item);

            var result = _transformationResult.GetSingleOrDefault();

            Assert.AreEqual(item, result);
        }

        [Test]
        public void GetSingleOrDefault_ManyItemsInList_ReturnNull()
        {
            _transformationResult.Add(null);
            _transformationResult.Add(new Foo());

            var result = _transformationResult.GetSingleOrDefault();

            Assert.IsNull(result);
        }

        [Test]
        public void RemoveAt_RemoveItem()
        {
            _transformationResult.Add(null);
            _transformationResult.Add(new Foo());

            _transformationResult.RemoveAt(1);

            Assert.AreEqual(1, _transformationResult.Destinations.Count);
            Assert.IsNull(_transformationResult.Destinations[0]);
        }

        [Test]
        public void Insert_InsertItemAtSpecifiedIndex()
        {
            var newItem = new Foo();
            var oldItem = new Foo();

            _transformationResult.Add(oldItem);

            _transformationResult.Insert(1, newItem);

            Assert.AreEqual(2, _transformationResult.Destinations.Count);
            Assert.AreEqual(oldItem, _transformationResult.Destinations[0]);
            Assert.AreEqual(newItem, _transformationResult.Destinations[1]);
        }
    }
}