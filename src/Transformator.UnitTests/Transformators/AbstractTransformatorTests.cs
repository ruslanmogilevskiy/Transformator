using System.Collections.Generic;
using NUnit.Framework;
using Rumo.Transformator.Models;
using Rumo.Transformator.Transformators;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests.Transformators
{
    [TestFixture]
    public class AbstractTransformatorTests : TestBase
    {
        class MyAbstractTransformator : AbstractTransformator<Foo, Bar>
        {
            public MyAbstractTransformator(TransformationBuilder<Foo, Bar> builder = null) : base(builder)
            {
            }

            public override Bar Transform(Foo source, Bar destination, TransformationContext context)
            {
                return destination;
            }

            public TransformationBuilder<Foo, Bar> GetBuilder()
            {
                return Builder;
            }

            public new IList<AbstractTransformation<Foo, Bar>> GetTransformations()
            {
                return base.GetTransformations();
            }
        }

        TransformationBuilder<Foo, Bar> _builder;
        MyAbstractTransformator _transformator;

        public override void Setup()
        {
            base.Setup();

            _builder = Transformation.For<Foo, Bar>();
            _transformator = new MyAbstractTransformator();
            _transformator.AttachTo(_builder);
        }

        [Test]
        public void Ctor_PassedBuilderIsNotNull_AttachToBuilder_Success()
        {
            var builder = new TransformationBuilder<Foo, Bar>();

            var transformator = new MyAbstractTransformator(builder);

            Assert.AreEqual(builder, transformator.GetBuilder());
        }

        [Test]
        public void Ctor_PassedBuilderIsNull_DontAttachToBuilder()
        {
            var transformator = new MyAbstractTransformator();

            Assert.IsNull(transformator.GetBuilder());
        }

        [Test]
        public void GetTransformations_BuilderIsNotNull_ReturnBuilderTransformations()
        {
            var builder = new TransformationBuilder<Foo, Bar>();
            var transformator = new MyAbstractTransformator(builder);

            var result = transformator.GetTransformations();

            Assert.AreEqual(builder.Transformations, result);
        }

        [Test]
        public void GetTransformations_BuilderIsNull_ReturnEmptyList()
        {
            var transformator = new MyAbstractTransformator();

            var result = transformator.GetTransformations();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetDestinationInstance_PassedDestinationNotNull_AndIsNotIsolatedTransformer_ReturnOriginalDestination()
        {
            var context = new SomeTransformationContext();
            var destination = new Bar();

            _transformator.IsIsolatedResult = false;

            var result = _transformator.GetDestinationInstance(context, destination);

            Assert.AreEqual(destination, result);
        }

        [Test]
        public void GetDestinationInstance_PassedDestinationNotNull_AndIsIsolatedTransformer_NoFactoriesConfigured_ReturnNull()
        {
            var context = new SomeTransformationContext();
            var destination = new Bar();

            _transformator.IsIsolatedResult = true;
            _builder.Configuration.AutoCreateDestination = false;

            var result = _transformator.GetDestinationInstance(context, destination);

            Assert.IsNull(result);
        }

        [Test]
        public void GetDestinationInstance_InitialDestinationFactoryIsSet_UseItAndReturnCreatedInstance()
        {
            Bar newDestination = new Bar();
            var context = new SomeTransformationContext();
            var destination = new Bar();

            _transformator.IsIsolatedResult = true;
            _builder.InitialDestinationFactory = c => newDestination;

            var result = _transformator.GetDestinationInstance(context, destination);

            Assert.AreEqual(newDestination, result);
        }

        [Test]
        public void GetDestinationInstance_AutoCreateDestinationIsTrue_ButConfigurationInstanceFactoryIsNull_CreateInstanceWithActivatorAndReturn()
        {
            var context = new SomeTransformationContext();
            var destination = new Bar();

            _transformator.IsIsolatedResult = true;
            _builder.Configuration.AutoCreateDestination = true;
            _builder.Configuration.InstanceFactory = null;

            var result = _transformator.GetDestinationInstance(context, destination);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(destination, result);
        }

        [Test]
        public void
            GetDestinationInstance_AutoCreateDestinationIsTrue_AndConfigurationInstanceFactoryIsSet_CreateInstanceWithInstanceFactoryAndReturn()
        {
            var context = new SomeTransformationContext();
            var destination = new Bar();
            var newDestination = new Bar();

            _transformator.IsIsolatedResult = true;
            _builder.Configuration.AutoCreateDestination = true;
            _builder.Configuration.InstanceFactory = t => t == typeof(Bar) ? newDestination : null;

            var result = _transformator.GetDestinationInstance(context, destination);

            Assert.AreEqual(newDestination, result);
        }

        [Test]
        public void GetDestinationInstance_NoneFactoryIsConfigured_ReturnNull()
        {
            var context = new SomeTransformationContext();
            var destination = new Bar();
            var newDestination = new Bar();

            _transformator.IsIsolatedResult = true;
            _builder.Configuration.AutoCreateDestination = false;
            _builder.Configuration.InstanceFactory = null;

            var result = _transformator.GetDestinationInstance(context, destination);

            Assert.IsNull(result);
        }
    }
}