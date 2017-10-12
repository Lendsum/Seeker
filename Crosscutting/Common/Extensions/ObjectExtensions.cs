//-----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Mediatech Solutions">
//      Copyright (c) Mediatech Solutions. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Lendsum.Crosscutting.Common.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for every object
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Return if both objects are equal or both are null
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public static bool EqualsOrBothNull(this object target, object other)
        {
            return Object.Equals(target, other) || (target == null && other == null);
        }
    }
}