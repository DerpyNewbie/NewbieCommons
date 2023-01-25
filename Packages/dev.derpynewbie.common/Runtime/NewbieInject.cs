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
    }
}