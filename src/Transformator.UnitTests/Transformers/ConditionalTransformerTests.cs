using System;
using FakeItEasy;
using NUnit.Framework;
using Transformator.Models;
using Transformator.Transformers;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests.Transformers
{
    [TestFixture]
    public class ConditionalTransformerTests
    {
        [Test]
        public void DoTransform_ConditionReturnFalse_PassedActionIsNotCalled_AndReturnDestination()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var isActionCalled = false;

            var transformer = new ConditionalTransformer<Foo, Bar>((_, _, _) => false, (_, _, _) =>
            {
                isActionCalled = true;
                return null;
            });

            var result = transformer.Transform(source, destination, context);

            Assert.IsFalse(isActionCalled);
            Assert.AreEqual(destination, result);
        }

        [Test]
        public void DoTransform_ConditionReturnTrue_CreateDestinationInstance_CallPassedAction_AndReturnResult()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var isActionCalled = false;
            var customDestination = new Bar();
            var newDestination = new Bar();
            Func<Foo, Bar, TransformationContext, bool> condition = (_, _, _) => true;
            Func<Foo, Bar, TransformationContext, Bar> action = (s, d, c) =>
            {
                Assert.AreEqual(source, s);
                Assert.AreEqual(newDestination, d);
                Assert.AreEqual(context, c);
                isActionCalled = true;
                return customDestination;
            };
            var transformer = A.Fake<ConditionalTransformer<Foo, Bar>>(o=>o.WithArgumentsForConstructor(new object[]
            {
              condition, action, false
            })).AsPartial();

            A.CallTo(() => transformer.GetDestinationInstance(context, destination))
                .Returns(newDestination);

            var result = transformer.Transform(source, destination, context);

            Assert.AreEqual(customDestination, result);
            Assert.IsTrue(isActionCalled);
        }
    }
}