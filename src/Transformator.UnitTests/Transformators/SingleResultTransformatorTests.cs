using NUnit.Framework;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests.Transformators
{
    [TestFixture]
    public class SingleResultTransformatorTests : TestBase
    {
        [Test]
        public void Transform_SingleResultTransformersInChain_ReturnSingleLastResult()
        {
            var source = new Foo { SourceProperty = "source" };
            var dest1 = new Bar();
            var dest2 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer(dest1))
                .Apply(new SingleTestTransformer(dest2));
            var transformator = new MySingleResultTransformator(builder);

            var result = transformator.Transform(source);

            Assert.AreEqual(dest2, result);
        }

        [Test]
        public void Transform_SimpleAndIsolatedTransformersInChain_ReturnSingleLastResult()
        {
            var source = new Foo { SourceProperty = "source" };
            var dest1 = new Bar();
            var dest2 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer(dest1))
                .ApplyIsolated(new SingleTestTransformer(dest2));
            var transformator = new MySingleResultTransformator(builder);

            var result = transformator.Transform(source);

            Assert.AreEqual(dest2, result);
        }

        [Test]
        public void Transform_SimpleThenIsolatedThenSimpleTransformersInChain_ReturnSingleLastResult()
        {
            var source = new Foo { SourceProperty = "source" };
            var dest1 = new Bar();
            var dest2 = new Bar();
            var dest3 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer(dest1))
                .ApplyIsolated(new SingleTestTransformer(dest2))
                .Apply(new SingleTestTransformer(dest3));
            var transformator = new MySingleResultTransformator(builder);

            var result = transformator.Transform(source);

            Assert.AreEqual(dest3, result);
        }

        [Test]
        public void Transform_TransformerReturnNull_ReturnNull()
        {
            var source = new Foo { SourceProperty = "source" };
            var dest2 = new Bar();
            var builder = Transformation.For<Foo, Bar>()
                .Apply(new SingleTestTransformer() { ReturnNullDestination = true})
                .Apply(new SingleTestTransformer(dest2));
            var transformator = new MySingleResultTransformator(builder);

            var result = transformator.Transform(source);

            Assert.IsNull(result);
        }

        [Test]
        public void Transform_NoTransformersInChain_AndDestinationAutoCreationDisabled_ReturnNull()
        {
            var source = new Foo { SourceProperty = "source" };
            var builder = Transformation.For<Foo, Bar>();
            builder.Configuration.AutoCreateDestination = false;
            var transformator = new MySingleResultTransformator(builder);

            var result = transformator.Transform(source);

            Assert.IsNull(result);
        }
    }
}