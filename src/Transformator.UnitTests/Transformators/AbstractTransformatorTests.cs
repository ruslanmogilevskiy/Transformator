using System.Collections.Generic;
using NUnit.Framework;
using Transformator.Interfaces;
using Transformator.Models;
using Transformator.Transformators;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests.Transformators
{
    [TestFixture]
    public class AbstractTransformatorTests
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

            public new IList<IAbstractTransformation<Foo, Bar>> GetTransformations()
            {
                return base.GetTransformations();
            }
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
    }
}