﻿// <copyright file="DILocalVariable.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET.DebugInfo
{
    /// <summary>Debug information for a local variable</summary>
    /// <seealso href="xref:llvm_langref#dilocalvariable">LLVM DILocalVariable</seealso>
    public class DILocalVariable
        : DIVariable
    {
        /* TODO: non-operand properties
        public DebugInfoFlags => LLVMDILocalVariableGetFlags( MetadataHandle );
        public UInt16 ArgIndex => LLVMDILocalVariableGetArg( MetadataHandle );
        public bool IsParameter => ArgIndex != 0;
        */

        /// <summary>Gets the local scope containing this variable</summary>
        public new DILocalScope Scope => base.Scope as DILocalScope;

        internal DILocalVariable( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }
}
