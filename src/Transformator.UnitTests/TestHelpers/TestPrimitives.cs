using System;
using System.Collections.Generic;
using Rumo.Transformator.Models;
using Rumo.Transformator.Transformators;
using Rumo.Transformator.Transformers;

namespace Rumo.Transformator.UnitTests.TestHelpers
{
    public class Foo
    {
        public string SourceProperty { get; set; }
    }

    public class Bar
    {
        public string DestinationProperty { get; set; }
    }

    public class SomeTransformationContext : TransformationContext
    {
    }

    public class MyMultiResultTransformator : MultiResultTransformator<Foo, Bar>
    {
        public MyMultiResultTransformator(TransformationBuilder<Foo, Bar> builder = null) : base(builder)
        {
        }
    }

    public class MySingleResultTransformator : SingleResultTransformator<Foo, Bar>
    {
        public MySingleResultTransformator(TransformationBuilder<Foo, Bar> builder = null) : base(builder)
        {
        }
    }

    public class SingleTestTransformer : AbstractTransformer<Foo, Bar>
    {
        readonly Func<Bar, Bar> _transformer;
        readonly string _customDestinationValue;

        public bool ReturnNullDestination { get; set; }
        
        public Bar CustomDestination { get; set; }

        public SingleTestTransformer()
        {
            
        }

        public SingleTestTransformer(Bar customDestination)
        {
            CustomDestination = customDestination;
        }

        public SingleTestTransformer(string customDestinationValue = null)
        {
            _customDestinationValue = customDestinationValue;
        }

        public SingleTestTransformer(Func<Bar, Bar> transformer)
        {
            _transformer = transformer;
        }

        protected override Bar DoTransform(Foo source, Bar destination, TransformationContext context)
        {
            if (_transformer != null)
            {
                return _transformer(destination);
            }
            if (ReturnNullDestination)
                return null;
            if (CustomDestination != null)
                return CustomDestination;

            destination.DestinationProperty = _customDestinationValue ?? source.SourceProperty;
            return destination;
        }
    }

    public class MultiTestTransformer : TypedAbstractMultiTransformer<Foo, Bar, TransformationContext>
    {
        readonly Bar[] _destinationObjects;

        public MultiTestTransformer(params Bar[] destinationObjects)
        {
            _destinationObjects = destinationObjects;
        }

        protected override IEnumerable<Bar> DoMultiTransform(Foo source, Bar destination, TransformationContext context)
        {
            foreach (var destinationObject in _destinationObjects)
            {
                yield return destinationObject;
            }
        }
    }

    public class MultiplicatorTestTransformer : TypedAbstractMultiTransformer<Foo, Bar, TransformationContext>
    {
        readonly int _multiplyTimes;
        int _currentIndex;

        public MultiplicatorTestTransformer(int multiplyTimes)
        {
            _multiplyTimes = multiplyTimes;
        }

        protected override IEnumerable<Bar> DoMultiTransform(Foo source, Bar destination, TransformationContext context)
        {
            for (int i = 0; i < _multiplyTimes; i++)
            {
                destination.DestinationProperty += $"_{++_currentIndex}";
                yield return destination;
            }
        }
    }
}