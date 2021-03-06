﻿// <copyright file="DICompositeType.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using JetBrains.Annotations;
using Llvm.NET.Native;

namespace Llvm.NET.DebugInfo
{
    /// <summary>Debug information for a composite type</summary>
    /// <seealso href="xref:llvm_langref#dicompositetype">LLVM DICompositeType</seealso>
    public class DICompositeType
        : DIType
    {
        /// <summary>Gets the base type for this type, if any</summary>
        [property: CanBeNull]
        public DIType BaseType => GetOperand<DIType>( 3 );

        /// <summary>Gets the elements of this <see cref="DICompositeType"/></summary>
        public DINodeArray Elements => new DINodeArray( GetOperand<MDTuple>( 4 ) );

        /// <summary>Gets the type that holds the VTable for this type, if any</summary>
        [property: CanBeNull]
        public DIType VTableHolder => GetOperand<DIType>( 5 );

        /// <summary>Gets the template parameters for this type, if any</summary>
        [property: CanBeNull]
        public DITemplateParameterArray TemplateParameters => new DITemplateParameterArray( GetOperand<MDTuple>( 6 ));

        /// <summary>Gets the identifier for this type</summary>
        public string Identifier => GetOperand<MDString>( 7 ).ToString( );

        /// <summary>Initializes a new instance of the <see cref="DICompositeType"/> class from an LLVM-C API Metadata handle</summary>
        /// <param name="handle">LLVM handle to wrap</param>
        internal DICompositeType( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }
}
