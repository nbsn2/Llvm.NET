// <copyright file="MDString.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET
{
    /// <summary>Stores a string in Metadata</summary>
    public class MDString
        : LlvmMetadata
    {
        /// <summary>Gets the string from the metadata node</summary>
        /// <returns>String this node wraps</returns>
        public override string ToString( )
        {
            return NativeMethods.LLVMGetMDStringText( MetadataHandle, out uint len );
        }

        internal MDString( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }
}
