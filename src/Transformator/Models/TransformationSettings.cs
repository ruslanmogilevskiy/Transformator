using System;
using System.Diagnostics.CodeAnalysis;

namespace Transformator.Models
{
    [ExcludeFromCodeCoverage]
    public class TransformationSettings
    {
        /// <summary>Abstracts the instantiation of the required type.</summary>
        public Func<Type, object> InstanceFactory { get; set; }
    }
}