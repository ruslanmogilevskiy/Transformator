using FakeItEasy;
using NUnit.Framework;
using Rumo.Transformator.Models;
using Rumo.Transformator.Transformers;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests.Transformers
{
    [TestFixture]
    public class TypedAbstractTransformerTests : TestBase
    {
        public class MyTypedAbstractTransformer : TypedAbstractTransformer<Foo, Bar, TransformationContext>
        {
            public Bar CustomDestination { get; set; }
            
            public TransformAction? CustomTransformAction { get; set; }

            protected override TransformAction CanTransform(Foo source, Bar destination, TransformationContext context)
            {
                return CustomTransformAction ?? base.CanTransform(source, destination, context);
            }

            protected override Bar DoTransform(Foo source, Bar destination, TransformationContext context)
            {
                return DoTransformPublic(source, destination, context);
            }

            public virtual Bar DoTransformPublic(Foo source, Bar destination, TransformationContext context)
            {
                return CustomDestination ?? destination;
            }
        }

        MyTypedAbstractTransformer _transformer;

        [SetUp]
        public void Setup()
        {
            _transformer = A.Fake<MyTypedAbstractTransformer>().AsPartial();
        }

        [Test]
        public void Transform_ByDefaultItCallsDoTransform_AndReturnDestination()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();

            var result = _transformer.Transform(source, destination, context);

            Assert.AreEqual(destination, result);
            A.CallTo(() => _transformer.DoTransformPublic(source, destination, context))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Transform_CanTransformReturnTransform_CallDoTransform_AndReturnTransformedDestination()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();

            _transformer.CustomTransformAction = TransformAction.Transform;
            _transformer.CustomDestination = customerDestination;

            var result = _transformer.Transform(source, destination, context);

            Assert.AreEqual(customerDestination, result);
        }

        [Test]
        public void Transform_CanTransformReturnPassThrough_DoNotCallDoTransform_AndReturnUnchangedDestination()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();

            _transformer.CustomTransformAction = TransformAction.PassThrough;
            _transformer.CustomDestination = customerDestination;

            var result = _transformer.Transform(source, destination, context);

            Assert.AreEqual(destination, result);
            A.CallTo(() => _transformer.DoTransformPublic(source, destination, context))
                .MustNotHaveHappened();
        }

        [Test]
        public void Transform_CanTransformReturnBreakTransformation_DoNotCallDoTransform_AndBreakTransformation()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();

            _transformer.CustomTransformAction = TransformAction.BreakTransformation;
            _transformer.CustomDestination = customerDestination;

            var result = _transformer.Transform(source, destination, context);

            Assert.IsNull(result);
            A.CallTo(() => _transformer.DoTransformPublic(source, destination, context))
                .MustNotHaveHappened();
        }

        [Test]
        public void Transform_CanTransformReturnUnknownAction_DoNotCallDoTransform_AndReturnNullDestination()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();

            _transformer.CustomTransformAction = (TransformAction)3454;
            _transformer.CustomDestination = customerDestination;

            var result = _transformer.Transform(source, destination, context);

            Assert.IsNull(result);
            A.CallTo(() => _transformer.DoTransformPublic(source, destination, context))
                .MustNotHaveHappened();
        }
    }
}