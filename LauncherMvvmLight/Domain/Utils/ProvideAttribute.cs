using System;

namespace LauncherMvvmLight.Infrastructure.Util
{
    /// <summary>
    /// Attribute used by view models to define a property that can send a value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ProvideAttribute : Attribute
    {
        /// <summary>
        /// Well known property name. Should be the same on the source and target
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Target navigation action defines what target this property can be sent to
        /// </summary>
        public string TargetNavigationAction { get; set; }
    }
}
