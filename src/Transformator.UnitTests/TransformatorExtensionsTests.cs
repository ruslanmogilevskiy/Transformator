using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using Transformator.Interfaces;
using Transformator.Models;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests
{
    [TestFixture]
    public class TransformatorExtensionsTests : TestBase
    {
        [Test]
        public void Transform_ContextIsPassed_CallTransformatorTransformMethodWithContext_AndReturnResult()
        {
            var transformator = A.Fake<ITransformator<Foo, Bar>>();
            var source = new Foo();
            var context = A.Fake<TransformationContext>();
            var destination = new Bar();

            A.CallTo(() => transformator.Transform(source, null, context))
                .Returns(destination);

            var result = transformator.Transform(source, context);

            Assert.AreEqual(destination, result);
        }

        [Test]
        public void Transform_ContextIsNotPassed_CallTransformatorTransformMethodWithNullContext_AndReturnResult()
        {
            var transformator = A.Fake<ITransformator<Foo, Bar>>();
            var source = new Foo();
            var destination = new Bar();

            A.CallTo(() => transformator.Transform(source, null, null))
                .Returns(destination);

            var result = transformator.Transform(source);

            Assert.AreEqual(destination, result);
        }

        [Test]
        public void TransformMulti_ContextIsPassed_CallTransformatorTransformMultiMethodWithContext_AndReturnResult()
        {
            var transformator = A.Fake<IMultiTransformator<Foo, Bar>>();
            var source = new Foo();
            var context = A.Fake<TransformationContext>();
            IEnumerable<Bar> destination = new []{new Bar()};

            A.CallTo(() => transformator.TransformMulti(source, null, context))
                .Returns(destination);

            var result = transformator.TransformMulti(source, context);

            Assert.AreEqual(destination, result);
        }

        [Test]
        public void TransformMulti_ContextIsNotPassed_CallTransformatorTransformMultiMethodWithNullContext_AndReturnResult()
        {
            var transformator = A.Fake<IMultiTransformator<Foo, Bar>>();
            var source = new Foo();
            IEnumerable<Bar> destination = new[] { new Bar() };

            A.CallTo(() => transformator.TransformMulti(source, null, null))
                .Returns(destination);

            var result = transformator.TransformMulti(source);

            Assert.AreEqual(destination, result);
        }
    }
}