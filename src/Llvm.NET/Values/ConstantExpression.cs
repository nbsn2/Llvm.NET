﻿// <copyright file="ConstantExpression.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Llvm.NET.Instructions;
using Llvm.NET.Native;
using Llvm.NET.Types;
using Ubiquity.ArgValidators;

namespace Llvm.NET.Values
{
    /// <summary>While technically a type in LLVM, ConstantExpression is primarily a static factory for Constants</summary>
    public class ConstantExpression
        : Constant
    {
        /// <summary>Gets the constant instruction expression op code</summary>
        public OpCode OpCode => ( OpCode )NativeMethods.LLVMGetConstOpcode( ValueHandle );

        /// <summary>Gets an IntToPtr expression to convert an integral value to a pointer</summary>
        /// <param name="value">Constant value to cast to a pointer</param>
        /// <param name="type">Type of the pointer to cast <paramref name="value"/> to</param>
        /// <returns>New pointer constant</returns>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific type required by interop call" )]
        public static Constant IntToPtrExpression( Constant value, ITypeRef type )
        {
            value.ValidateNotNull( nameof( value ) );

            if( value.NativeType.Kind != TypeKind.Integer )
            {
                throw new ArgumentException( "Integer Type expected", nameof( value ) );
            }

            if( !( type is IPointerType ) )
            {
                throw new ArgumentException( "pointer type expected", nameof( type ) );
            }

            return FromHandle<Constant>( NativeMethods.LLVMConstIntToPtr( value.ValueHandle, type.GetTypeRef( ) ) );
        }

        /// <summary>Creates a constant bit cast expression</summary>
        /// <param name="value">value to cast</param>
        /// <param name="toType">Type to cast to</param>
        /// <returns>Constant cast expression</returns>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific type required by interop call" )]
        public static Constant BitCast( Constant value, ITypeRef toType )
        {
            value.ValidateNotNull( nameof( value ) );

            var handle = NativeMethods.LLVMConstBitCast( value.ValueHandle, toType.GetTypeRef( ) );
            return FromHandle<Constant>( handle );
        }

        /// <summary>Creates a constant GetElementPtr expression</summary>
        /// <param name="value">Constant value to get the element pointer for</param>
        /// <param name="args">Pointer index args</param>
        /// <returns>GetElementPtr expression</returns>
        public static Constant GetElementPtr( Constant value, params Constant[ ] args )
            => GetElementPtr( value, ( IEnumerable<Constant> )args );

        /// <summary>Creates a constant GetElementPtr expression</summary>
        /// <param name="value">Constant value to get the element pointer for</param>
        /// <param name="args">Pointer index args</param>
        /// <returns>GetElementPtr expression</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific type required by interop call")]
        public static Constant GetElementPtr(Constant value, IEnumerable<Constant> args)
        {
            var llvmArgs = InstructionBuilder.GetValidatedGEPArgs(value, args);
            var handle = NativeMethods.LLVMConstGEP( value.ValueHandle, out llvmArgs[0], (uint)llvmArgs.Length);
            return FromHandle<Constant>(handle);
        }

        internal ConstantExpression( LLVMValueRef valueRef )
            : base( valueRef )
        {
        }
    }
}
