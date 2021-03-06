﻿// <copyright file="DIObjCProperty.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET.DebugInfo
{
    /// <summary>Objective-C Property</summary>
    public class DIObjCProperty
        : DINode
    {
        /*
        public uint Line {get;}
        public uint Attributes {get;}
        */

        /// <summary>Gets teh Debug information for the file containing this property</summary>
        public DIFile File => GetOperand<DIFile>( 1 );

        /// <summary>Gets the name of the property</summary>
        public string Name => GetOperand<MDString>( 0 ).ToString( );

        /// <summary>Gets the name of the getter method for the property</summary>
        public string GetterName => GetOperand<MDString>( 2 ).ToString( );

        /// <summary>Gets the name of the setter method for the property</summary>
        public string SetterName => GetOperand<MDString>( 3 ).ToString( );

        /// <summary>Gets the type of the property</summary>
        public DIType Type => GetOperand<DIType>( 4 );

        internal DIObjCProperty( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }
}
