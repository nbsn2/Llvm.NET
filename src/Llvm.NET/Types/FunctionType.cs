﻿// <copyright file="FunctionType.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Llvm.NET.Native;

// Interface+interal type matches file name
#pragma warning disable SA1649

namespace Llvm.NET.Types
{
    /// <summary>Interface to represent the LLVM type of a function (e.g. a signature)</summary>
    public interface IFunctionType
        : ITypeRef
    {
        /// <summary>Gets a value indicating whether this signature is for a variadic function</summary>
        bool IsVarArg { get; }

        /// <summary>Gets the return type of the function</summary>
        ITypeRef ReturnType { get; }

        /// <summary>Gets the types of the parameters for the function</summary>
        IReadOnlyList<ITypeRef> ParameterTypes { get; }
    }

    /// <summary>Class to represent the LLVM type of a function (e.g. a signature)</summary>
    internal class FunctionType
        : TypeRef
        , IFunctionType
    {
        /// <inheritdoc/>
        public bool IsVarArg => NativeMethods.LLVMIsFunctionVarArg( TypeRefHandle );

        /// <inheritdoc/>
        public ITypeRef ReturnType => FromHandle<ITypeRef>( NativeMethods.LLVMGetReturnType( TypeRefHandle ) );

        /// <inheritdoc/>
        public IReadOnlyList<ITypeRef> ParameterTypes
        {
            get
            {
                uint paramCount = NativeMethods.LLVMCountParamTypes( TypeRefHandle );
                if( paramCount == 0 )
                {
                    return new List<TypeRef>().AsReadOnly();
                }

                var paramTypes = new LLVMTypeRef[ paramCount ];
                NativeMethods.LLVMGetParamTypes( TypeRefHandle, out paramTypes[ 0 ] );
                return paramTypes.Select( FromHandle<TypeRef> )
                                 .ToList( )
                                 .AsReadOnly( );
            }
        }

        internal FunctionType( LLVMTypeRef typeRef )
            : base( typeRef )
        {
        }
    }
}
