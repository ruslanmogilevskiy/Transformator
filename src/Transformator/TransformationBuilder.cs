using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Transformator.Interfaces;
using Transformator.Models;
using Transformator.Transformators;
using Transformator.Transformers;

namespace Transformator
{
    /// <summary>
    /// Builds the transformation flow.
    /// </summary>
    /// <typeparam name="TSource">Source data type of transformation.</typeparam>
    /// <typeparam name="TDestination">Destination data type of transformation.</typeparam>
    public class TransformationBuilder<TSource, TDestination>
    {
        /// <summary>Transformations list.</summary>
        public IList<IAbstractTransformation<TSource, TDestination>> Transformations { get; }

        /// <summary>Initial destination instance factory method.</summary>
        /// <remarks>Used to dynamically customize the destination type instantiation when transformation starts. It not supplied, the destination
        /// instance will be created via Activator.</remarks>
        public Func<TransformationContext, TDestination> InitialInstanceFactory { get; set; }

        /// <summary>Transformation settings.</summary>
        protected internal TransformationSettings Settings { get; private set; }

        public TransformationBuilder(TransformationSettings settings = null)
        {
            Settings = settings;
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

        /// <summary>Abstracts the required instance creation.</summary>
        /// <typeparam name="T">The type which instance to create.</typeparam>
        protected internal virtual T CreateInstance<T>()
        {
            if (Settings?.InstanceFactory != null)
            {
                return (T)Settings.InstanceFactory(typeof(T));
            }

            return Activator.CreateInstance<T>();
        }

        /// <summary>Builds the single-result transformation.</summary>
        public ITransformator<TSource, TDestination> Build()
        {
            return new SingleResultTransformator<TSource, TDestination>(this);
        }

        /// <summary>Builds the multi-result transformation.</summary>
        public IMultiTransformator<TSource, TDestination> BuildMulti()
        {
            return new MultiResultTransformator<TSource, TDestination>(this);
        }

        /// <summary>Sets transformation settings that will be used for the transformation flow.</summary>
        /// <param name="settings">Transformation settings.</param>
        public TransformationBuilder<TSource, TDestination> WithSettings(TransformationSettings settings)
        {
            Settings = settings;
            return this;
        }

        /// <summary>Sets initial destination value that is passed into the first transformer in the transformation flow.</summary>
        /// <param name="destinationInitialValue">Destination initial value.</param>
        public TransformationBuilder<TSource, TDestination> WithInitialValue(TDestination destinationInitialValue)
        {
            InitialInstanceFactory = _ => destinationInitialValue;
            return this;
        }

        /// <summary>Executes the specified action on the transformation.</summary>
        /// <param name="action">An action to execute.</param>
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
            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, transformation.Transform));
            return this;
        }

        public TransformationBuilder<TSource, TDestination> IfApplyIsolated(Func<TSource, TDestination, TransformationContext, bool> condition,
            IAbstractTransformation<TSource, TDestination> transformation)
        {
            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, transformation.Transform, true));
            return this;
        }
    }
}