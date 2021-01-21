using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Transformator.Interfaces;
using Transformator.Transformators;
using Transformator.Transformers;

namespace Transformator
{
    public class TransformationBuilder<TSource, TDestination>
    {
        public IList<IAbstractTransformation<TSource, TDestination>> Transformations { get; }

        public Func<TransformationContext, TDestination> InitialInstanceProvider { get; set; }

        /// <summary>Callback to abstract the transformers instantiation.</summary>
        public static Func<Type, object> GlobalInstanceProvider { get; set; }

        public TransformationBuilder()
        {
            var list = new ObservableCollection<IAbstractTransformation<TSource, TDestination>>();
            list.CollectionChanged += List_CollectionChanged;
            Transformations = list;
        }

        void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is AbstractTransformation<TSource, TDestination> abstractTransformation)
                        abstractTransformation.AttachTo(this);
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is AbstractTransformation<TSource, TDestination> abstractTransformation)
                        abstractTransformation.AttachTo(null);
                }
            }
        }

        protected virtual T CreateInstance<T>()
        {
            if (GlobalInstanceProvider != null)
            {
                return (T)GlobalInstanceProvider(typeof(T));
            }

            return Activator.CreateInstance<T>();
        }

        public ITransformator<TSource, TDestination> Build()
        {
            return new SingleResultTransformator<TSource, TDestination>(this);
        }

        public IMultiTransformator<TSource, TDestination> BuildMulti()
        {
            return new MultiResultTransformator<TSource, TDestination>(this);
        }

        public TransformationBuilder<TSource, TDestination> WithInitialValue(TDestination destinationInitialValue)
        {
            InitialInstanceProvider = _ => destinationInitialValue;
            return this;
        }

        public TransformationBuilder<TSource, TDestination> Do(Func<TSource, TDestination, TransformationContext, TDestination> action)
        {
            Transformations.Add(new ActionTransformer<TSource, TDestination>(action));
            return this;
        }

        public TransformationBuilder<TSource, TDestination> IfDo(Func<TSource, TDestination, TransformationContext, bool> condition,
            Func<TSource, TDestination, TransformationContext, TDestination> action)
        {
            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, action));
            return this;
        }

        /// <summary>Add new transformer to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> Apply(IAbstractTransformation<TSource, TDestination> transformation)
        {
            Transformations.Add(transformation);
            return this;
        }

        /// <summary>Add new transformer to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> Apply<T>() where T : IAbstractTransformation<TSource, TDestination>
        {
            var transformation = CreateInstance<T>();
            Transformations.Add(transformation);
            return this;
        }

        /// <summary>Add new transformer to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> ApplyIsolated(IAbstractTransformation<TSource, TDestination> transformation)
        {
            transformation.IsIsolatedResult = true;
            Transformations.Add(transformation);
            return this;
        }

        /// <summary>Add new transformer to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> ApplyIsolated<T>()
            where T : IAbstractTransformation<TSource, TDestination>
        {
            var transformation = CreateInstance<T>();
            transformation.IsIsolatedResult = true;
            Transformations.Add(transformation);
            return this;
        }

        public TransformationBuilder<TSource, TDestination> IfApply(Func<TSource, TDestination, TransformationContext, bool> condition,
            IAbstractTransformation<TSource, TDestination> transformation)
        {
            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, transformation));
            return this;
        }

        public TransformationBuilder<TSource, TDestination> IfApplyIsolated(Func<TSource, TDestination, TransformationContext, bool> condition,
            IAbstractTransformation<TSource, TDestination> transformation)
        {
            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, transformation, true));
            return this;
        }
    }
}