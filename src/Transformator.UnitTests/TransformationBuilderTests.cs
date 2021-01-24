using System;
using FakeItEasy;
using NUnit.Framework;
using Transformator.Models;
using Transformator.Transformators;
using Transformator.Transformers;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests
{
    [TestFixture]
    public class TransformationBuilderTests
    {
        class MyAbstractTransformation : AbstractTransformation<Foo, Bar>
        {
            public TransformationBuilder<Foo, Bar> GetBuilder()
            {
                return Builder;
            }

            public override Bar Transform(Foo source, Bar destination, TransformationContext context)
            {
                throw new NotImplementedException();
            }
        }

        TransformationBuilder<Foo, Bar> _builder;

        [SetUp]
        public void Setup()
        {
            _builder = A.Fake<TransformationBuilder<Foo, Bar>>()
                .AsPartial();
        }

        [Test]
        public void Ctor_PassedSettingsAreAccessibleViaSettingsProperty()
        {
            var settings = new TransformationSettings();

            var builder = new TransformationBuilder<Foo, Bar>(settings);

            Assert.AreEqual(settings, builder.Settings);
        }

        [Test]
        public void Ctor_TransformationsListIsInitiatedWithEmptyList()
        {
            var builder = new TransformationBuilder<Foo, Bar>();

            Assert.IsNotNull(builder.Transformations);
            Assert.AreEqual(0, builder.Transformations.Count);
        }

        [Test]
        public void Transformations_WhenAddTransformation_ItIsAttachedToThisBuilder()
        {
            var transformer = new MyAbstractTransformation();

            _builder.Transformations.Add(transformer);

            Assert.AreEqual(1, _builder.Transformations.Count);
            Assert.AreEqual(transformer, _builder.Transformations[0]);
            Assert.AreEqual(_builder, transformer.GetBuilder());
        }

        [Test]
        public void Transformations_WhenRemoveTransformation_ItIsDetachedFromThisBuilder()
        {
            var transformer = new MyAbstractTransformation();

            _builder.Transformations.Add(transformer);

            Assert.AreEqual(1, _builder.Transformations.Count);
            Assert.AreEqual(transformer, _builder.Transformations[0]);

            _builder.Transformations.Remove(transformer);

            Assert.AreEqual(0, _builder.Transformations.Count);
            Assert.IsNull(transformer.GetBuilder());
        }

        [Test]
        public void CreateInstance_InstanceFactoryIsNull_UseActivator()
        {
            _builder.WithSettings(null);

            var result = _builder.CreateInstance<Foo>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateInstance_InstanceFactoryIsNotNull_CallFactory_AndReturnInstance()
        {
            var instance = new Foo();
            _builder.WithSettings(new TransformationSettings { InstanceFactory = t => instance });

            var result = _builder.CreateInstance<Foo>();

            Assert.IsNotNull(result);
            Assert.AreEqual(instance, result);
        }

        [Test]
        public void WithSettings_SetSettingsProperty()
        {
            var settings = new TransformationSettings();

            var builderInstance = _builder.WithSettings(settings);

            Assert.AreEqual(settings, _builder.Settings);
            Assert.AreEqual(_builder, builderInstance);
        }

        [Test]
        public void WithInitialValue_SetInitialInstanceFactoryProperty()
        {
            var initialValue = new Bar();

            var builderInstance = _builder.WithInitialValue(initialValue);
            var result = _builder.InitialInstanceFactory(null);

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(initialValue, result);
        }

        [Test]
        public void Build_ReturnSingleResultTransformatorInstance()
        {
            var result = _builder.Build();

            Assert.IsNotNull(result);
            Assert.IsTrue(result is SingleResultTransformator<Foo, Bar>);
        }

        [Test]
        public void BuildMulti_ReturnMultiResultTransformatorInstance()
        {
            var result = _builder.BuildMulti();

            Assert.IsNotNull(result);
            Assert.IsTrue(result is MultiResultTransformator<Foo, Bar>);
        }

        [Test]
        public void Do_ActionTransformerWithPassedActionAddedToTransformationList()
        {
            var source = new Foo();
            var destination = new Bar();
            TransformationContext context = new SomeTransformationContext();
            var customDestination = new Bar();

            var builderInstance = _builder.Do((s, d, c) =>
            {
                Assert.AreEqual(source, s);
                Assert.AreEqual(destination, d);
                Assert.AreEqual(context, c);

                return customDestination;
            });

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            var transformer = _builder.Transformations[0] as ActionTransformer<Foo, Bar>;
            Assert.IsNotNull(transformer);
            var result = transformer.Transform(source, destination, context);
            Assert.AreEqual(customDestination, result);
        }
    }
}