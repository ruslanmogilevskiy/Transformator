using System;
using FakeItEasy;
using NUnit.Framework;
using Rumo.Transformator.Configuration;
using Rumo.Transformator.Models;
using Rumo.Transformator.Transformators;
using Rumo.Transformator.Transformers;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests
{
    [TestFixture]
    public class TransformationBuilderTests : TestBase
    {
        public class MyAbstractTransformation : AbstractTransformation<Foo, Bar>
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
            var settings = new TransformationConfiguration();

            var builder = new TransformationBuilder<Foo, Bar>(settings);

            Assert.AreEqual(settings, builder.Configuration);
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
            _builder.WithConfiguration(new TransformationConfiguration());

            var result = _builder.CreateInstance<Foo>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateInstance_InstanceFactoryIsNotNull_CallFactory_AndReturnInstance()
        {
            var instance = new Foo();
            _builder.WithConfiguration(new TransformationConfiguration { InstanceFactory = t => instance });

            var result = _builder.CreateInstance<Foo>();

            Assert.IsNotNull(result);
            Assert.AreEqual(instance, result);
        }

        [Test]
        public void WithConfiguration_SetConfigurationProperty()
        {
            var settings = new TransformationConfiguration();

            var builderInstance = _builder.WithConfiguration(settings);

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(settings, _builder.Configuration);
        }

        [Test]
        public void WithInitialValue_InitialValueIsNotNull_SetInitialInstanceFactoryProperty()
        {
            var initialValue = new Bar();

            var builderInstance = _builder.WithInitialValue(initialValue);
            var result = _builder.InitialDestinationFactory(null);

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(initialValue, result);
        }

        [Test]
        public void WithInitialValue_InitialValueIsNull_SetInitialInstanceFactoryPropertyToReturnNull()
        {
            var builderInstance = _builder.WithInitialValue(null);
            var result = _builder.InitialDestinationFactory(null);

            Assert.AreEqual(_builder, builderInstance);
            Assert.IsNull(result);
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

        [Test]
        public void Do_ActionParameterIsNull_ThrowException()
        {
            var resultException = Assert.Throws<ArgumentNullException>(()=>_builder.Do(null));

            Assert.IsNotNull(resultException);
            Assert.AreEqual("action".GetArgumentNullExceptionMessage(), resultException.Message);
        }

        [Test]
        public void IfDo_ConditionIsTrue_ExecuteTransformation()
        {
            var source = new Foo();
            var destination = new Bar();
            var customDestination = new Bar();

            var builderInstance = _builder.IfDo((s, d, c) => c == null, (s, d, c) =>
            {
                Assert.AreEqual(source, s);
                Assert.AreEqual(destination, d);
                Assert.IsNull(c);

                return customDestination;
            });

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            var transformer = _builder.Transformations[0] as ConditionalTransformer<Foo, Bar>;
            Assert.IsNotNull(transformer);
            var result = transformer.Transform(source, destination, null);
            Assert.AreEqual(customDestination, result);
        }

        [Test]
        public void IfDo_PassedConditionIsNull_ThrowException()
        {
            var resultException = Assert.Throws<ArgumentNullException>(() => _builder.IfDo(null, (s, d, c) => null));

            Assert.IsNotNull(resultException);
            Assert.AreEqual("condition".GetArgumentNullExceptionMessage(), resultException.Message);
        }

        [Test]
        public void IfDo_PassedActionIsNull_ThrowException()
        {
            var resultException = Assert.Throws<ArgumentNullException>(()=>_builder.IfDo((_,_,_)=>true, null));

            Assert.IsNotNull(resultException);
            Assert.AreEqual("action".GetArgumentNullExceptionMessage(), resultException.Message);
        }

        [Test]
        public void IfDo_ConditionIsFalse_DoNotExecuteTransformation()
        {
            var source = new Foo();
            var destination = new Bar();
            var customDestination = new Bar();
            var context = new SomeTransformationContext();

            var builderInstance = _builder.IfDo((s, d, c) => c == null, (s, d, c) =>
            {
                Assert.AreEqual(source, s);
                Assert.AreEqual(destination, d);
                Assert.IsNull(c);

                return customDestination;
            });

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            var transformer = _builder.Transformations[0] as ConditionalTransformer<Foo, Bar>;
            Assert.IsNotNull(transformer);
            var result = transformer.Transform(source, destination, context);
            Assert.AreEqual(destination, result);
        }

        [Test]
        public void Apply_NotNullTransformationPassed_AddPassedTransformerToTransformationsList()
        {
            var transformation = new MyAbstractTransformation();

            var builderInstance = _builder.Apply(transformation);

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            Assert.AreEqual(transformation, _builder.Transformations[0]);
        }

        [Test]
        public void Apply_NullTransformationPassed_ThrowException()
        {
            var thrownException = Assert.Throws<ArgumentNullException>(() => _builder.Apply(null));

            Assert.IsNotNull(thrownException);
            Assert.AreEqual("transformation".GetArgumentNullExceptionMessage(), thrownException.Message);
        }

        [Test]
        public void Apply_Generic_CreateTransformer_AndAddItToTransformationsList()
        {
            var transformation = new MyAbstractTransformation();
            
            A.CallTo(() => _builder.CreateInstance<MyAbstractTransformation>())
                .Returns(transformation);

            var builderInstance = _builder.Apply<MyAbstractTransformation>();

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            Assert.AreEqual(transformation, _builder.Transformations[0]);
        }

        [Test]
        public void ApplyIsolated_NotNullTransformationPassed_AddPassedTransformerToTransformationsList()
        {
            var transformation = new MyAbstractTransformation();
            Assert.IsFalse(transformation.IsIsolatedResult);

            var builderInstance = _builder.ApplyIsolated(transformation);

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            Assert.AreEqual(transformation, _builder.Transformations[0]);
            Assert.IsTrue(transformation.IsIsolatedResult);
            Assert.AreEqual(!TransformationConfiguration.Default.IsolateInitialDestination, transformation.KeepInitialDestination);
        }

        [Test]
        public void ApplyIsolated_KeepInitialDestinationParameterPassed_SetItOnTransformer()
        {
            var transformation = new MyAbstractTransformation();
            Assert.IsFalse(transformation.IsIsolatedResult);

            var builderInstance = _builder.ApplyIsolated(transformation, false);

            Assert.AreEqual(_builder, builderInstance);
            Assert.IsFalse(transformation.KeepInitialDestination);
        }

        [Test]
        public void ApplyIsolated_NullTransformationPassed_ThrowException()
        {
            var thrownException = Assert.Throws<ArgumentNullException>(() => _builder.ApplyIsolated(null));

            Assert.IsNotNull(thrownException);
            Assert.AreEqual("transformation".GetArgumentNullExceptionMessage(), thrownException.Message);
        }

        [Test]
        public void ApplyIsolated_Generic_CreateTransformer_AndAddItToTransformationsList()
        {
            var transformation = new MyAbstractTransformation();
            Assert.IsFalse(transformation.IsIsolatedResult);

            A.CallTo(() => _builder.CreateInstance<MyAbstractTransformation>())
                .Returns(transformation);

            var builderInstance = _builder.ApplyIsolated<MyAbstractTransformation>();

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            Assert.AreEqual(transformation, _builder.Transformations[0]);
            Assert.IsTrue(transformation.IsIsolatedResult);
            Assert.AreEqual(!TransformationConfiguration.Default.IsolateInitialDestination, transformation.KeepInitialDestination);
        }

        [Test]
        public void ApplyIsolated_Generic_KeepInitialDestinationParameterPassed_SetItOnTransformer()
        {
            var transformation = new MyAbstractTransformation();
            Assert.IsFalse(transformation.IsIsolatedResult);

            A.CallTo(() => _builder.CreateInstance<MyAbstractTransformation>())
                .Returns(transformation);

            var builderInstance = _builder.ApplyIsolated<MyAbstractTransformation>(false);

            Assert.AreEqual(_builder, builderInstance);
            Assert.IsFalse(transformation.KeepInitialDestination);
        }

        [Test]
        public void IfApply_AddConditionalTransformer_WithSpecifiedConditionAndTransformation_AndNonIsolatedResult()
        {
            var transformation = A.Fake<MyAbstractTransformation>();
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var newDestination = new Bar();

            A.CallTo(() => transformation.Transform(source, destination, context))
                .Returns(newDestination);

            var builderInstance = _builder.IfApply((s, d, c) => true, transformation);

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            var conditionalTransformer = _builder.Transformations[0] as ConditionalTransformer<Foo, Bar>;
            Assert.IsNotNull(conditionalTransformer);
            Assert.IsFalse(conditionalTransformer.IsIsolatedResult);
            var transformationResult = conditionalTransformer.Transform(source, destination, context);
            Assert.AreEqual(newDestination, transformationResult);
        }

        [Test]
        public void IfApply_NullConditionPassed_ThrowException()
        {
            var transformation = A.Fake<MyAbstractTransformation>();

            var thrownException = Assert.Throws<ArgumentNullException>(() => _builder.IfApply(null, transformation));

            Assert.IsNotNull(thrownException);
            Assert.AreEqual("condition".GetArgumentNullExceptionMessage(), thrownException.Message);
        }

        [Test]
        public void IfApply_NullTransformationPassed_ThrowException()
        {
            var thrownException = Assert.Throws<ArgumentNullException>(() => _builder.IfApply((s, d, c) => true, null));

            Assert.IsNotNull(thrownException);
            Assert.AreEqual("transformation".GetArgumentNullExceptionMessage(), thrownException.Message);
        }

        [Test]
        public void IfApplyIsolated_AddConditionalTransformer_WithSpecifiedConditionAndTransformation_AndIsolatedResult()
        {
            var transformation = A.Fake<MyAbstractTransformation>();
            var source = new Foo();
            var destination = new Bar();
            var context = new SomeTransformationContext();
            var newDestination = new Bar();
            var transformedDestination = new Bar();

            _builder.Configuration.InstanceFactory = t =>
            {
                if (t == typeof(Bar))
                {
                    return newDestination;
                }

                return null;
            };
            A.CallTo(() => transformation.Transform(source, newDestination, context))
                .Returns(transformedDestination);

            var builderInstance = _builder.IfApplyIsolated((s, d, c) => true, transformation);

            Assert.AreEqual(_builder, builderInstance);
            Assert.AreEqual(1, _builder.Transformations.Count);
            var conditionalTransformer = _builder.Transformations[0] as ConditionalTransformer<Foo, Bar>;
            Assert.IsNotNull(conditionalTransformer);
            Assert.IsTrue(conditionalTransformer.IsIsolatedResult);

            var transformationResult = conditionalTransformer.Transform(source, destination, context);
            Assert.AreEqual(transformedDestination, transformationResult);
        }

        [Test]
        public void IfApplyIsolated_NullConditionPassed_ThrowException()
        {
            var transformation = A.Fake<MyAbstractTransformation>();

            var thrownException = Assert.Throws<ArgumentNullException>(() => _builder.IfApplyIsolated(null, transformation));

            Assert.IsNotNull(thrownException);
            Assert.AreEqual("condition".GetArgumentNullExceptionMessage(), thrownException.Message);
        }

        [Test]
        public void IfApplyIsolated_NullTransformationPassed_ThrowException()
        {
            var thrownException = Assert.Throws<ArgumentNullException>(() => _builder.IfApplyIsolated((s, d, c) => true, null));

            Assert.IsNotNull(thrownException);
            Assert.AreEqual("transformation".GetArgumentNullExceptionMessage(), thrownException.Message);
        }
    }
}