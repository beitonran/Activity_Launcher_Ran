using System;

namespace LauncherMvvmLight.Infrastructure.Util
{
    /// <summary>
    /// Attribute used by view models to define a property that can receive a value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConsumeAttribute : Attribute
    {
        /// <summary>
        /// Well known property name. Should be the same on the source and target
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Source navigation action defines what sources this property can be received from
        /// </summary>
        public string SourceNavigationAction { get; set; }

        /// <summary>
        /// Clears the consumer property if no value was provided by the previous view. This only works if the 
        /// property is nullable.
        /// </summary>
        public bool ClearIfNoValue { get; set; }
    }
}
