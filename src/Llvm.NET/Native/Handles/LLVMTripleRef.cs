﻿// <copyright file="LLVMTripleRef.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Llvm.NET.Native
{
    // typedef struct LLVMOpaqueTriple* LLVMTripleRef;
    [SecurityCritical]
    internal class LLVMTripleRef
        : LlvmObjectRef
    {
        internal LLVMTripleRef( )
            : base( true )
        {
        }

        internal LLVMTripleRef( IntPtr handle, bool owner )
            : base( owner )
        {
            SetHandle( handle );
        }

        [SecurityCritical]
        protected override bool ReleaseHandle( )
        {
            LLVMDisposeTriple( handle );
            return true;
        }

        [DllImport( NativeMethods.LibraryPath, CallingConvention = CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        private static extern void LLVMDisposeTriple( IntPtr triple );
    }
}
