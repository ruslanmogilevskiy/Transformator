using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Rumo.Transformator.Models;
using Rumo.Transformator.Transformers;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests.Transformers
{
    [TestFixture]
    public class TypedAbstractMultiTransformerTests : TestBase
    {
        public class MyTypedAbstractMultiTransformer : TypedAbstractMultiTransformer<Foo, Bar, TransformationContext>
        {
            public IEnumerable<Bar> CustomDestinations { get; set; }
            
            public TransformAction? CustomTransformAction { get; set; }

            protected override TransformAction CanTransform(Foo source, Bar destination, TransformationContext context)
            {
                return CustomTransformAction ?? base.CanTransform(source, destination, context);
            }

            protected override IEnumerable<Bar> DoMultiTransform(Foo source, Bar destination, TransformationContext context)
            {
                return DoMultiTransformPublic(source, destination, context);
            }

            public virtual IEnumerable<Bar> DoMultiTransformPublic(Foo source, Bar destination, TransformationContext context)
            {
                return CustomDestinations ?? new []{destination};
            }
        }

        MyTypedAbstractMultiTransformer _transformer;

        [SetUp]
        public void Setup()
        {
            _transformer = A.Fake<MyTypedAbstractMultiTransformer>().AsPartial();
        }

        [Test]
        public void Transform_IsNotSupport_AndThrowException()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();

            var exception = Assert.Throws<NotSupportedException>(()=>_transformer.Transform(source, destination, context));

            Assert.IsNotNull(exception);
            Assert.AreEqual($"Only multi-transformation is supported via TransformMulti method", exception.Message);
        }

        [Test]
        public void TransformMulti_ByDefaultItCallsDoMultiTransform_AndReturnResult()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();

            var result = _transformer.TransformMulti(source, destination, context);

            Assert.IsNotNull(result);
            var resultItems = result.ToList();
            Assert.AreEqual(1, resultItems.Count);
            Assert.AreEqual(destination, resultItems[0]);
            A.CallTo(() => _transformer.DoMultiTransformPublic(source, destination, context))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void TransformMulti_CanTransformReturnTransform_CallDoMultiTransform_AndReturnResult()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();
            var customerDestination2 = new Bar();

            _transformer.CustomTransformAction = TransformAction.Transform;
            _transformer.CustomDestinations = new []{customerDestination, customerDestination2};

            var result = _transformer.TransformMulti(source, destination, context);

            Assert.IsNotNull(result);
            var resultItems = result.ToList();
            Assert.AreEqual(2, resultItems.Count);
            Assert.AreEqual(customerDestination, resultItems[0]);
            Assert.AreEqual(customerDestination2, resultItems[1]);
        }

        [Test]
        public void TransformMulti_CanTransformReturnPassThrough_DoNotCallDoTransform_AndReturnUnchangedDestination()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();
            var customerDestination2 = new Bar();

            _transformer.CustomTransformAction = TransformAction.PassThrough;
            _transformer.CustomDestinations = new[] { customerDestination, customerDestination2 };

            var result = _transformer.TransformMulti(source, destination, context);

            Assert.IsNotNull(result);
            var resultItems = result.ToList();
            Assert.AreEqual(1, resultItems.Count);
            Assert.AreEqual(destination, resultItems[0]);
            A.CallTo(() => _transformer.DoMultiTransformPublic(source, destination, context))
                .MustNotHaveHappened();
        }

        [Test]
        public void TransformMulti_CanTransformReturnBreakTransformation_DoNotCallDoTransform_AndBreakTransformation()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();
            var customerDestination2 = new Bar();

            _transformer.CustomTransformAction = TransformAction.BreakTransformation;
            _transformer.CustomDestinations = new[] { customerDestination, customerDestination2 };

            var result = _transformer.TransformMulti(source, destination, context);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.ToList().Count);
            A.CallTo(() => _transformer.DoMultiTransformPublic(source, destination, context))
                .MustNotHaveHappened();
        }

        [Test]
        public void TransformMulti_CanTransformReturnUnknownAction_DoNotCallDoTransform_AndReturnNullDestination()
        {
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var customerDestination = new Bar();
            var customerDestination2 = new Bar();

            _transformer.CustomTransformAction = (TransformAction?) 345;
            _transformer.CustomDestinations = new[] { customerDestination, customerDestination2 };

            var result = _transformer.TransformMulti(source, destination, context);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.ToList().Count);
            A.CallTo(() => _transformer.DoMultiTransformPublic(source, destination, context))
                .MustNotHaveHappened();
        }
    }
}