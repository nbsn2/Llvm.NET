﻿// <copyright file="MemoryBuffer.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Llvm.NET.Native;
using Llvm.NET.Native.Handles;
using Ubiquity.ArgValidators;

using static Llvm.NET.Native.NativeMethods;

namespace Llvm.NET
{
    /// <summary>LLVM MemoryBuffer</summary>
    public sealed class MemoryBuffer
        : IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="MemoryBuffer"/> class from a file</summary>
        /// <param name="path">Path of the file to load</param>
        public MemoryBuffer( string path )
        {
            path.ValidateNotNullOrWhiteSpace( nameof( path ) );

            if( LLVMCreateMemoryBufferWithContentsOfFile( path, out BufferHandle_, out string msg ).Failed )
            {
                throw new InternalCodeGeneratorException( msg );
            }
        }

        /// <summary>Gets the size of the buffer</summary>
        public int Size
        {
            get
            {
                if( BufferHandle == default )
                {
                    return 0;
                }

                return LLVMGetBufferSize( BufferHandle ).Pointer.ToInt32();
            }
        }

        /// <inheritdoc/>
        public void Dispose( )
        {
            if( BufferHandle == default )
            {
                LLVMDisposeMemoryBuffer( BufferHandle );
                BufferHandle_ = default;
            }
        }

        /// <summary>Gets an array of bytes from the buffer</summary>
        /// <returns>Array of bytes copied from the buffer</returns>
        public byte[] ToArray()
        {
            var bufferStart = LLVMGetBufferStart( BufferHandle );
            var retVal = new byte[ Size ];
            Marshal.Copy( bufferStart, retVal, 0, Size );
            return retVal;
        }

        internal MemoryBuffer( LLVMMemoryBufferRef bufferHandle )
        {
            bufferHandle.ValidateNotDefault( nameof( bufferHandle ) );

            BufferHandle_ = bufferHandle;
        }

        internal LLVMMemoryBufferRef BufferHandle => BufferHandle_;

        // keep as a private field so this is usable as an out parameter in constructor
        // do not write to it directly, treat it as readonly.
        [SuppressMessage( "StyleCop.CSharp.NamingRules"
                        , "SA1310:Field names must not contain underscore"
                        , Justification = "Trailing _ indicates should not be written to directly even internally"
                        )
        ]
        private LLVMMemoryBufferRef BufferHandle_;
    }
}
