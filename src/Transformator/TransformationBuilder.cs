using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Rumo.Transformator.Configuration;
using Rumo.Transformator.Helpers;
using Rumo.Transformator.Interfaces;
using Rumo.Transformator.Models;
using Rumo.Transformator.Transformators;
using Rumo.Transformator.Transformers;

namespace Rumo.Transformator
{
    /// <summary>
    /// Builds the transformation flow.
    /// </summary>
    /// <typeparam name="TSource">Source data type of transformation.</typeparam>
    /// <typeparam name="TDestination">Destination data type of transformation.</typeparam>
    public class TransformationBuilder<TSource, TDestination>
    {
        /// <summary>Transformations list.</summary>
        public IList<AbstractTransformation<TSource, TDestination>> Transformations { get; }

        /// <summary>Initial destination instance factory method.</summary>
        /// <remarks>Used to dynamically customize the destination type instantiation when transformation starts. It not supplied, the destination
        /// instance will be created via Activator.</remarks>
        public Func<TransformationContext, TDestination> InitialDestinationFactory { get; set; }

        /// <summary>Transformation configuration to use on the transformation.</summary>
        /// <remarks>Could be change via <see cref="WithConfiguration"/> method.</remarks>
        protected internal TransformationConfiguration Configuration { get; private set; }

        public TransformationBuilder(TransformationConfiguration configuration = null)
        {
            Configuration = configuration;
            var list = new ObservableCollection<AbstractTransformation<TSource, TDestination>>();
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
            var result = Configuration.CreateInstanceSafe<T>();
            return result;
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

        /// <summary>Sets transformation configuration that will be used for the transformation flow.</summary>
        /// <param name="configuration">Transformation configuration.</param>
        public TransformationBuilder<TSource, TDestination> WithConfiguration(TransformationConfiguration configuration)
        {
            ArgGuard.NotNull(configuration, nameof(configuration));

            Configuration = configuration;
            return this;
        }

        /// <summary>Sets initial destination value that is passed into the first transformer in the transformation flow.</summary>
        /// <param name="destinationInitialValue">Destination initial value.</param>
        public TransformationBuilder<TSource, TDestination> WithInitialValue(TDestination destinationInitialValue)
        {
            InitialDestinationFactory = _ => destinationInitialValue;
            return this;
        }

        /// <summary>Executes the specified action on the transformation.</summary>
        /// <param name="action">An action to execute.</param>
        public TransformationBuilder<TSource, TDestination> Do(Func<TSource, TDestination, TransformationContext, TDestination> action)
        {
            ArgGuard.NotNull(action, nameof(action));

            Transformations.Add(new ActionTransformer<TSource, TDestination>(action));
            return this;
        }

        /// <summary>Applies the transformation <paramref name="action"/> only when <paramref name="condition"/> returns true.</summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="action">The action to execute when condition is true.</param>
        public TransformationBuilder<TSource, TDestination> IfDo(Func<TSource, TDestination, TransformationContext, bool> condition,
            Func<TSource, TDestination, TransformationContext, TDestination> action)
        {
            ArgGuard.NotNull(condition, nameof(condition));
            ArgGuard.NotNull(action, nameof(action));
          
            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, action));
            return this;
        }

        /// <summary>Add the transformation step to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> Apply(AbstractTransformation<TSource, TDestination> transformation)
        {
            ArgGuard.NotNull(transformation, nameof(transformation));
           
            Transformations.Add(transformation);
            return this;
        }

        /// <summary>Add the transformation step to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> Apply<T>() 
            where T : AbstractTransformation<TSource, TDestination>
        {
            return Apply(CreateInstance<T>());
        }

        /// <summary>Add the isolated transformation step to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> ApplyIsolated(AbstractTransformation<TSource, TDestination> transformation, bool? keepInitialDestination = null)
        {
            ArgGuard.NotNull(transformation, nameof(transformation));
          
            transformation.IsIsolatedResult = true;
            transformation.KeepInitialDestination = keepInitialDestination ?? !Configuration.IsolateInitialDestination;
            Transformations.Add(transformation);
            return this;
        }

        /// <summary>Add the isolated transformation step to the transformation chain.</summary>
        public TransformationBuilder<TSource, TDestination> ApplyIsolated<T>(bool? keepInitialDestination = null)
            where T : AbstractTransformation<TSource, TDestination>
        {
            return ApplyIsolated(CreateInstance<T>(), keepInitialDestination);
        }

        /// <summary>Add the specified transformation step to the transformation chain if the specified condition returns true.</summary>
        /// <param name="condition">The condition function that defines whether to apply the specified transformation. It's evaluated in run-time when the
        /// transformation is going.</param>
        /// <param name="transformation">The transformation to apply.</param>
        public TransformationBuilder<TSource, TDestination> IfApply(Func<TSource, TDestination, TransformationContext, bool> condition,
            AbstractTransformation<TSource, TDestination> transformation)
        {
            ArgGuard.NotNull(condition, nameof(condition));
            ArgGuard.NotNull(transformation, nameof(transformation));
          
            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, transformation.Transform));
            return this;
        }

        /// <summary>Add the specified isolated transformation step to the transformation chain if the specified condition returns true.</summary>
        /// <param name="condition">The condition function that defines whether to apply the specified transformation. It's evaluated in run-time when the
        /// transformation is going.</param>
        /// <param name="transformation">The transformation to apply.</param>
        public TransformationBuilder<TSource, TDestination> IfApplyIsolated(Func<TSource, TDestination, TransformationContext, bool> condition,
            AbstractTransformation<TSource, TDestination> transformation)
        {
            ArgGuard.NotNull(condition, nameof(condition));
            ArgGuard.NotNull(transformation, nameof(transformation));

            Transformations.Add(new ConditionalTransformer<TSource, TDestination>(condition, transformation.Transform, true));
            return this;
        }
    }
}