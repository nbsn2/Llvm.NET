﻿// <copyright file="Generated.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

using static Llvm.NET.Native.NativeMethods;

namespace Llvm.NET.Native
{
    internal struct LLVMValueRef
        : IEquatable<LLVMValueRef>
    {
        public override int GetHashCode( ) => Handle.GetHashCode( );

        public override bool Equals( object obj ) => !( obj is null ) && ( obj is LLVMValueRef r ) && r.Handle == Handle;

        public bool Equals( LLVMValueRef other ) => Handle == other.Handle;

        public static bool operator ==( LLVMValueRef lhs, LLVMValueRef rhs )
            => EqualityComparer<LLVMValueRef>.Default.Equals( lhs, rhs );

        public static bool operator !=( LLVMValueRef lhs, LLVMValueRef rhs ) => !( lhs == rhs );

        // FIXME: Move this out of here to the Value as it is a layering violation
        public Context Context
        {
            get
            {
                if( Handle.IsNull() )
                {
                    return null;
                }

                var hType = LLVMTypeOf( this );
                Debug.Assert( hType != default, "Should not get a null pointer from LLVM" );
                return hType.Context;
            }
        }

        internal LLVMValueRef( IntPtr pointer )
        {
            Handle = pointer;
        }

        private readonly IntPtr Handle;
    }
}
