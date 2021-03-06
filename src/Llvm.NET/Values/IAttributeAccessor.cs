﻿// <copyright file="IAttributeAccessor.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Llvm.NET.Values
{
    /// <summary>Interface for raw attribute access</summary>
    /// <remarks>
    /// As of LLVM v3.9x and later, Functions and call sites use distinct LLVM-C API sets for
    /// manipulating attributes. Fortunately they have consistent signatures so these
    /// are used to abstract the difference via derived types specialized for each case.
    /// Going forward this is the simplest and most direct way to manipulate attributes
    /// on a value as everything ultimately comes down to this interface.
    /// </remarks>
    public interface IAttributeAccessor
        : IAttributeContainer
    {
        /// <summary>Gets the count of attributes on a given index</summary>
        /// <param name="index">Index to get the count for</param>
        /// <returns>Number of attributes on the specified index</returns>
        uint GetAttributeCountAtIndex( FunctionAttributeIndex index );

        /// <summary>Gets the attributes on a given index</summary>
        /// <param name="index">index to get the attributes for</param>
        /// <returns>Attributes for the index</returns>
        IEnumerable<AttributeValue> GetAttributesAtIndex( FunctionAttributeIndex index );

        /// <summary>Gets a specific attribute at a given index</summary>
        /// <param name="index">Index to get the the attribute from</param>
        /// <param name="kind"><see cref="AttributeKind"/> to get</param>
        /// <returns>The specified attribute or the default <see cref="AttributeValue"/></returns>
        AttributeValue GetAttributeAtIndex( FunctionAttributeIndex index, AttributeKind kind );

        /// <summary>Gets a named attribute at a given index</summary>
        /// <param name="index">Index to get the the attribute from</param>
        /// <param name="name">name of the attribute to get</param>
        /// <returns>The specified attribute or the default <see cref="AttributeValue"/></returns>
        AttributeValue GetAttributeAtIndex( FunctionAttributeIndex index, string name );

        /// <summary>Adds an <see cref="AttributeValue"/> at a specified index</summary>
        /// <param name="index">Index to add the attribute to</param>
        /// <param name="attrib">Attribute to add</param>
        void AddAttributeAtIndex( FunctionAttributeIndex index, AttributeValue attrib );

        /// <summary>Removes an <see cref="AttributeKind"/> at a specified index</summary>
        /// <param name="index">Index to add the attribute to</param>
        /// <param name="kind">Attribute to Remove</param>
        void RemoveAttributeAtIndex( FunctionAttributeIndex index, AttributeKind kind );

        /// <summary>Removes a named attribute at a specified index</summary>
        /// <param name="index">Index to add the attribute to</param>
        /// <param name="name">Name of the attribute to remove</param>
        void RemoveAttributeAtIndex( FunctionAttributeIndex index, string name );
    }
}
