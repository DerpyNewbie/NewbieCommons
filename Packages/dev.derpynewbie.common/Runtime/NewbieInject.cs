using System;

namespace DerpyNewbie.Common
{
    /// <summary>
    /// Marks serialized field as build-time injected field.
    /// </summary>
    /// <remarks>
    /// Field attributed with this class will be searched at VRCSDK BuildRequestedCallback time,
    /// then injected with first-found component of field type.
    /// </remarks>
    public sealed class NewbieInject : Attribute
    {
        public readonly SearchScope Scope;

        public NewbieInject()
        {
            Scope = SearchScope.Scene;
        }

        public NewbieInject(SearchScope scope)
        {
            Scope = scope;
        }
    }

    public enum SearchScope
    {
        /// <summary>
        /// Searches from whole Scene
        /// </summary>
        Scene,
        /// <summary>
        /// Searches from attached GameObject
        /// </summary>
        Self,
        /// <summary>
        /// Searches from children of attached GameObject
        /// </summary>
        Children,
        /// <summary>
        /// Searches from parents of attached GameObject
        /// </summary>
        Parents,
    }
}