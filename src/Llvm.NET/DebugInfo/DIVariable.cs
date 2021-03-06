﻿// <copyright file="DIVariable.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET.DebugInfo
{
    /// <summary>Debug information for a variable</summary>
    public class DIVariable
        : DINode
    {
        /* TODO: UInt32 Line => NativeMethods.LLVMDIVariableGetLine( MetadataHandle ); */

        /// <summary>Gets the Debug information scope for this variable</summary>
        public DIScope Scope => Operands[ 0 ]?.Metadata as DIScope;

        /// <summary>Gets the Debug information name for this variable</summary>
        public string Name => ( Operands[ 1 ]?.Metadata as MDString )?.ToString( ) ?? string.Empty;

        /// <summary>Gets the Debug information file for this variable</summary>
        public DIFile File => Operands[ 2 ]?.Metadata as DIFile;

        /// <summary>Gets the Debug information type for this variable</summary>
        public DIType DIType => Operands[ 3 ]?.Metadata as DIType;

        internal DIVariable( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }
}
