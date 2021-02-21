using System.Linq;
using NUnit.Framework;
using Rumo.Transformator.Configuration;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests.Transformators
{
    [TestFixture]
    public class MultiResultTransformatorTests : TestBase
    {
        readonly TransformationConfiguration _defaultConfig = new() { AutoCreateDestination = true };

        [Test]
        public void Transform_MultipleResultsAreProduced_ReturnSingleLastResult()
        {
            var source = new Foo { SourceProperty = "source" };
            var dest1 = new Bar();
            var dest2 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(dest1, dest2));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.Transform(source);

            Assert.AreEqual(dest2, result);
        }

        [Test]
        public void Transform_NoResultsAreProduced_ReturnNull()
        {
            var source = new Foo { SourceProperty = "source" };
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer());
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.Transform(source);

            Assert.IsNull(result);
        }

        [Test]
        public void TransformMulti_OnlySimpleTransformersInChain_ReturnSingleResult()
        {
            var sourceValue = "source";
            var source = new Foo { SourceProperty = sourceValue };
            var dest1 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(dest1))
                .Apply<SingleTestTransformer>();

            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(dest1, items[0]);
            Assert.AreEqual(sourceValue, items[0]
                .DestinationProperty);
        }

        [Test]
        public void TransformMulti_SingleResult_SimpleNonIsolatedTransformer_FollowingBySimpleIsolated_FirstReturnNonIsolatedResult_ThenIsolatedResultSeparately()
        {
            var source = new Foo { SourceProperty = "source" };
            var value1 = "111111";
            var value2 = "222";
            var builder = Transformation.For<Foo, Bar>()
                .WithConfiguration(_defaultConfig)
                .Apply(new SingleTestTransformer(value1))
                .ApplyIsolated(new SingleTestTransformer(value2));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            var item = items[0];
            Assert.IsNotNull(item);
            Assert.AreEqual(value1, item.DestinationProperty);
            item = items[1];
            Assert.IsNotNull(item);
            Assert.AreEqual(value2, item.DestinationProperty);
        }

        [Test]
        public void TransformMulti_SingleResult_InitialDestinationIsIsolatedForTransformer_ButNotIsolatedGlobally_ReturnSeparatedDestinationObjects()
        {
            var source = new Foo { SourceProperty = "source" };
            var builder = Transformation.For<Foo, Bar>()
                .WithConfiguration(new TransformationConfiguration { IsolateInitialDestination = false })
                .Apply(new SingleTestTransformer("111111"))
                .ApplyIsolated(new SingleTestTransformer("222"), false);
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            var item1 = items[0];
            var item2 = items[1];
            Assert.AreNotEqual(item1, item2);
        }

        [Test]
        public void TransformMulti_SingleResult_InitialDestinationIsIsolatedForTransformer_AndIsolatedGlobally_ReturnSeparatedDestinationObject()
        {
            var source = new Foo { SourceProperty = "source" };
            var builder = Transformation.For<Foo, Bar>()
                .WithConfiguration(new TransformationConfiguration { IsolateInitialDestination = true })
                .Apply(new SingleTestTransformer("111111"))
                .ApplyIsolated(new SingleTestTransformer("222"), false);
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            var item1 = items[0];
            var item2 = items[1];
            Assert.AreNotEqual(item1, item2);
        }

        [Test]
        public void TransformMulti_SingleResult_InitialDestinationIsNotIsolatedForTransformer_ButIsolatedGlobally_ReturnExistingDestinationObject()
        {
            var source = new Foo { SourceProperty = "source" };
            var builder = Transformation.For<Foo, Bar>()
                .WithConfiguration(new TransformationConfiguration { IsolateInitialDestination = true })
                .Apply(new SingleTestTransformer("111111"))
                .ApplyIsolated(new SingleTestTransformer("222"), true);
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            var item1 = items[0];
            var item2 = items[1];
            Assert.AreEqual(item1, item2);
        }

        [Test]
        public void TransformMulti_SingleResult_InitialDestinationIsNotIsolatedForTransformer_AndNotIsolatedGlobally_ReturnExistingDestinationObject()
        {
            var source = new Foo { SourceProperty = "source" };
            var builder = Transformation.For<Foo, Bar>()
                .WithConfiguration(new TransformationConfiguration { IsolateInitialDestination = false })
                .Apply(new SingleTestTransformer("111111"))
                .ApplyIsolated(new SingleTestTransformer("222"), true);
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            var item1 = items[0];
            var item2 = items[1];
            Assert.AreEqual(item1, item2);
        }

        [Test]
        public void TransformMulti_SingleResult_SomeOfSimpleTransformerReturnsNull_DoNotCallAllFurtherSimpleTransformers_AndReturnNoResults()
        {
            var source = new Foo { SourceProperty = "source" };
            var value1 = "1";
            var value3 = "3";
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer(value1) { ReturnNullDestination = true })
                .Apply(new SingleTestTransformer(value3));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.ToList().Count);
        }

        [Test]
        public void TransformMulti_SingleResult_SimpleTransformerReturnsNull_DoNotCallAllFurtherSimpleTransformers_ButCallIsolatedTransformers_AndReturnNoResults()
        {
            var source = new Foo { SourceProperty = "source" };
            var value1 = "1";
            var isolatedValue = "2";
            var value3 = "3";
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer(value1) { ReturnNullDestination = true })
                .ApplyIsolated(new SingleTestTransformer(isolatedValue))
                .Apply(new SingleTestTransformer(value3));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(isolatedValue, items[0]
                .DestinationProperty);
        }

        [Test]
        public void TransformMulti_SingleResult_IsolatedTransformerReturnsNull_StillCallAllFurtherTransformations()
        {
            var source = new Foo { SourceProperty = "source" };
            var value1 = new Bar();
            var isolatedValue2 = new Bar();
            var value2 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer(value1))
                .ApplyIsolated(new SingleTestTransformer { ReturnNullDestination = true })
                .ApplyIsolated(new SingleTestTransformer(isolatedValue2))
                .Apply(new SingleTestTransformer(value2));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(3, items.Count);
            Assert.AreEqual(value1, items[0]);
            Assert.AreEqual(isolatedValue2, items[1]);
            Assert.AreEqual(value2, items[2]);
        }

        [Test]
        public void TransformMulti_SingleResult_IsolatedTransformerBetweenSimpleOnes_ReturnSeparateResultForBothSimpleTransformers()
        {
            var source = new Foo { SourceProperty = "source" };
            var value1 = new Bar();
            var isolatedValue1 = new Bar();
            var value2 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer(value1))
                .ApplyIsolated(new SingleTestTransformer(isolatedValue1))
                .Apply(new SingleTestTransformer(value2));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(3, items.Count);
            Assert.AreEqual(value1, items[0]);
            Assert.AreEqual(isolatedValue1, items[1]);
            Assert.AreEqual(value2, items[2]);
        }

        [Test]
        public void TransformMulti_MixResult_SingleNotIsolatedTransformer_ReturnMultipleResults()
        {
            var source = new Foo { SourceProperty = "source" };
            var value1 = new Bar();
            var value2 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(value1, value2));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(value1, items[0]);
            Assert.AreEqual(value2, items[1]);
        }

        [Test]
        public void TransformMulti_MixResult_NotIsolatedTransformer_ThenIsolated_ReturnMultipleIsolatedResults()
        {
            var source = new Foo();
            var value1 = new Bar();
            var value2 = new Bar();
            var value3 = new Bar();
            var value4 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(value1, value2))
                .ApplyIsolated(new MultiTestTransformer(value3, value4));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(4, items.Count);
            Assert.AreEqual(value1, items[0]);
            Assert.AreEqual(value2, items[1]);
            Assert.AreEqual(value3, items[2]);
            Assert.AreEqual(value4, items[3]);
        }

        [Test]
        public void TransformMulti_MixResult_MultiTransformer_ThenSingleTransformer_ReturnResultsCountAsMultiTransformerReturned()
        {
            var source = new Foo();
            var value1 = new Bar();
            var value2 = new Bar();
            var destProperty = "12";
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(value1, value2))
                .Apply(new SingleTestTransformer(destProperty));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(value1, items[0]);
            Assert.AreEqual(destProperty, items[0]
                .DestinationProperty);
            Assert.AreEqual(value2, items[1]);
            Assert.AreEqual(destProperty, items[1]
                .DestinationProperty);
        }

        [Test]
        public void TransformMulti_MixResult_MultiTransformer_ThenSingleTransformerWithOwnResults_ReturnSecondTransformerResults()
        {
            var source = new Foo();
            var value1 = new Bar();
            var value2 = new Bar();
            var value3 = new Bar();
            var value4 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(value1, value2))
                .Apply(new SingleTestTransformer(d =>
                {
                    if (d == value1)
                        return value3;
                    if (d == value2)
                        return value4;
                    return null;
                }));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            var items = result.ToList();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(value3, items[0]);
            Assert.AreEqual(value4, items[1]);
        }

        [Test]
        public void TransformMulti_MixResult_MultiTransformer_ThenSingleIsolated_ThenSingleIsolated_ReturnMultiResult_AndIsolatedResultSeparately()
        {
            var source = new Foo();
            var destinationProperty1 = "initial 1";
            var destinationProperty2 = "initial 2";
            var value1 = new Bar { DestinationProperty = destinationProperty1 };
            var value2 = new Bar { DestinationProperty = destinationProperty2 };
            var newDestinationProperty = "12";
            var isolatedProperty = "isolated";
            var isolatedValue = new Bar { DestinationProperty = isolatedProperty };
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(value1, value2))
                .ApplyIsolated(new SingleTestTransformer(isolatedValue))
                .Apply(new SingleTestTransformer(newDestinationProperty));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            int i = 0;
            foreach (var item in result)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(value1, item);
                        Assert.AreEqual(destinationProperty1, item.DestinationProperty);
                        break;
                    case 1:
                        Assert.AreEqual(value2, item);
                        Assert.AreEqual(destinationProperty2, item.DestinationProperty);
                        break;
                    case 2:
                        Assert.AreEqual(isolatedValue, item);
                        Assert.AreEqual(isolatedProperty, item.DestinationProperty);
                        break;
                    case 3:
                        Assert.AreEqual(value1, item);
                        Assert.AreEqual(newDestinationProperty, item.DestinationProperty);
                        break;
                    case 4:
                        Assert.AreEqual(value2, item);
                        Assert.AreEqual(newDestinationProperty, item.DestinationProperty);
                        break;
                }
            }

            var items = result.ToList();
            Assert.AreEqual(5, items.Count);
        }

        [Test]
        public void TransformMulti_MixResult_MultiTransformer_ThenAgainMultiTransformer_ReturnMultipliedResults()
        {
            var source = new Foo();
            var destinationProperty1 = "initial 1";
            var destinationProperty2 = "initial 2";
            var value1 = new Bar { DestinationProperty = destinationProperty1 };
            var value2 = new Bar { DestinationProperty = destinationProperty2 };
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new MultiTestTransformer(value1, value2))
                .Apply(new MultiplicatorTestTransformer(2));
            var transformator = new MyMultiResultTransformator(builder);

            var result = transformator.TransformMulti(source);

            Assert.IsNotNull(result);
            int i = 0;
            foreach (var item in result)
            {
                switch (i++)
                {
                    case 0:
                    case 1:
                        Assert.AreEqual(value1, item);
                        Assert.AreEqual(destinationProperty1 + "_1_2", item.DestinationProperty);
                        break;
                    case 2:
                    case 3:
                        Assert.AreEqual(value2, item);
                        Assert.AreEqual(destinationProperty2+"_3_4", item.DestinationProperty);
                        break;
                }
            }
            var items = result.ToList();
            Assert.AreEqual(4, items.Count);
        }
    }
}