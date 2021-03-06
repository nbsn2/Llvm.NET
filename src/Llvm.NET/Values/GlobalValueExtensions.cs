﻿// <copyright file="GlobalValueExtensions.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

namespace Llvm.NET.Values
{
    /// <summary>Fluent style extensions for modifying properties of a <see cref="GlobalValue"/></summary>
    public static class GlobalValueExtensions
    {
        /// <summary>Visibility of this global value</summary>
        /// <typeparam name="T">Type of the value (Must be <see cref="GlobalValue"/> or a type derived from it)</typeparam>
        /// <param name="self">Value to modify</param>
        /// <param name="value">New value to set</param>
        /// <returns><paramref name="self"/> for fluent usage patterns</returns>
        /// <seealso cref="GlobalValue.Visibility"/>
        public static T Visibility<T>( this T self, Visibility value )
            where T : GlobalValue
        {
            self.Visibility = value;
            return self;
        }

        /// <summary>Linkage specification for this symbol</summary>
        /// <typeparam name="T">Type of the value (Must be <see cref="GlobalValue"/> or a type derived from it)</typeparam>
        /// <param name="self">Value to modify</param>
        /// <param name="value">New value to set</param>
        /// <returns><paramref name="self"/> for fluent usage patterns</returns>
        /// <seealso cref="GlobalValue.Linkage"/>
        public static T Linkage<T>( this T self, Linkage value )
            where T : GlobalValue
        {
            self.Linkage = value;
            return self;
        }

        /// <summary>Sets the UnnamedAddress property of a value</summary>
        /// <typeparam name="T">Type of the value (Must be <see cref="GlobalValue"/> or a type derived from it)</typeparam>
        /// <param name="self">Value to modify</param>
        /// <param name="value">New value to set</param>
        /// <returns><paramref name="self"/> for fluent usage patterns</returns>
        /// <seealso cref="GlobalValue.UnnamedAddress"/>
        public static T UnnamedAddress<T>( this T self, bool value )
            where T : GlobalValue
        {
            self.UnnamedAddress = value;
            return self;
        }
    }
}
