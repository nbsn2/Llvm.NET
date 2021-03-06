﻿// <copyright file="TypeRef.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System;
using Llvm.NET.Native;
using Llvm.NET.Values;

// Interface+interal type matches file name
#pragma warning disable SA1649

namespace Llvm.NET.Types
{
    /// <summary>LLVM Type</summary>
    internal class TypeRef
        : ITypeRef
        , ITypeHandleOwner
    {
        /// <inheritdoc/>
        public LLVMTypeRef TypeHandle => TypeRefHandle;

        /// <inheritdoc/>
        public bool IsSized
        {
            get
            {
                if( Kind == TypeKind.Function )
                {
                    return false;
                }

                return NativeMethods.LLVMTypeIsSized( TypeRefHandle );
            }
        }

        /// <inheritdoc/>
        public TypeKind Kind => ( TypeKind )NativeMethods.LLVMGetTypeKind( TypeRefHandle );

        /// <inheritdoc/>
        public bool IsInteger=> Kind == TypeKind.Integer;

        /// <inheritdoc/>
        public bool IsFloat => Kind == TypeKind.Float32;

        /// <inheritdoc/>
        public bool IsDouble => Kind == TypeKind.Float64;

        /// <inheritdoc/>
        public bool IsVoid => Kind == TypeKind.Void;

        /// <inheritdoc/>
        public bool IsStruct => Kind == TypeKind.Struct;

        /// <inheritdoc/>
        public bool IsPointer => Kind == TypeKind.Pointer;

        /// <inheritdoc/>
        public bool IsSequence => Kind == TypeKind.Array || Kind == TypeKind.Vector || Kind == TypeKind.Pointer;

        /// <inheritdoc/>
        public bool IsFloatingPoint
        {
            get
            {
                switch( Kind )
                {
                case TypeKind.Float16:
                case TypeKind.Float32:
                case TypeKind.Float64:
                case TypeKind.X86Float80:
                case TypeKind.Float128m112:
                case TypeKind.Float128:
                    return true;

                default:
                    return false;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsPointerPointer
        {
            get
            {
                var ptrType = this as IPointerType;
                return ptrType != null && ptrType.ElementType.Kind == TypeKind.Pointer;
            }
        }

        /// <inheritdoc/>
        public Context Context => TypeRefHandle.Context;

        /// <inheritdoc/>
        public uint IntegerBitWidth
        {
            get
            {
                if( Kind != TypeKind.Integer )
                {
                    return 0;
                }

                return NativeMethods.LLVMGetIntTypeWidth( TypeRefHandle );
            }
        }

        /// <inheritdoc/>
        public Constant GetNullValue() => Constant.NullValueFor( this );

        /// <inheritdoc/>
        public IArrayType CreateArrayType( uint count ) => FromHandle<IArrayType>( NativeMethods.LLVMArrayType( TypeRefHandle, count ) );

        /// <inheritdoc/>
        public IPointerType CreatePointerType( ) => CreatePointerType( 0 );

        /// <inheritdoc/>
        public IPointerType CreatePointerType( uint addressSpace )
        {
            if( IsVoid )
            {
                throw new InvalidOperationException( "Cannot create pointer to void in LLVM, use i8* instead" );
            }

            return FromHandle<IPointerType>( NativeMethods.LLVMPointerType( TypeRefHandle, addressSpace ) );
        }

        public bool TryGetExtendedPropertyValue<T>( string id, out T value ) => ExtensibleProperties.TryGetExtendedPropertyValue<T>( id, out value );

        public void AddExtendedPropertyValue( string id, object value ) => ExtensibleProperties.AddExtendedPropertyValue( id, value );

        /// <summary>Builds a string representation for this type in LLVM assembly language form</summary>
        /// <returns>Formatted string for this type</returns>
        public override string ToString( ) => NativeMethods.LLVMPrintTypeToString( TypeRefHandle );

        internal TypeRef( LLVMTypeRef typeRef )
        {
            TypeRefHandle = typeRef;
            if( typeRef == default )
            {
                throw new ArgumentNullException( nameof( typeRef ) );
            }
        }

        internal static TypeRef FromHandle( LLVMTypeRef typeRef ) => FromHandle<TypeRef>( typeRef );

        internal static T FromHandle<T>( LLVMTypeRef typeRef )
            where T : class, ITypeRef
        {
            if( typeRef == default )
            {
                return null;
            }

            var ctx = typeRef.Context;
            return ( T )ctx.GetTypeFor( typeRef, StaticFactory );
        }

        protected LLVMTypeRef TypeRefHandle { get; }

        private static ITypeRef StaticFactory( LLVMTypeRef typeRef )
        {
            var kind = ( TypeKind )NativeMethods.LLVMGetTypeKind( typeRef );
            switch( kind )
            {
            case TypeKind.Struct:
                return new StructType( typeRef );

            case TypeKind.Array:
                return new ArrayType( typeRef );

            case TypeKind.Pointer:
                return new PointerType( typeRef );

            case TypeKind.Vector:
                return new VectorType( typeRef );

            case TypeKind.Function: // NOTE: This is a signature rather than a Function, which is a Value
                return new FunctionType( typeRef );

            // other types not yet supported in Object wrappers
            // but the pattern for doing so should be pretty obvious...
            case TypeKind.Void:
            case TypeKind.Float16:
            case TypeKind.Float32:
            case TypeKind.Float64:
            case TypeKind.X86Float80:
            case TypeKind.Float128m112:
            case TypeKind.Float128:
            case TypeKind.Label:
            case TypeKind.Integer:
            case TypeKind.Metadata:
            case TypeKind.X86MMX:
            default:
                return new TypeRef( typeRef );
            }
        }

        private readonly ExtensiblePropertyContainer ExtensibleProperties = new ExtensiblePropertyContainer( );
    }
}
