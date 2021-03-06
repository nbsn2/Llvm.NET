﻿// <copyright file="Switch.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;
using Llvm.NET.Values;

using static Llvm.NET.Native.NativeMethods;

namespace Llvm.NET.Instructions
{
    public class Switch
        : Terminator
    {
        /// <summary>Gets the default <see cref="BasicBlock"/> for the switch</summary>
        public BasicBlock Default => BasicBlock.FromHandle( NativeMethods.LLVMGetSwitchDefaultDest( ValueHandle ) );

        /// <summary>Adds a new case to the <see cref="Switch"/> instruction</summary>
        /// <param name="onVal">Value for the case to match</param>
        /// <param name="destination">Destination <see cref="BasicBlock"/> if the case matches</param>
        public void AddCase( Value onVal, BasicBlock destination )
        {
            if( onVal == null )
            {
                throw new System.ArgumentNullException( nameof( onVal ) );
            }

            if( destination == null )
            {
                throw new System.ArgumentNullException( nameof( destination ) );
            }

            LLVMAddCase( ValueHandle, onVal.ValueHandle, destination.BlockHandle );
        }

        internal Switch( LLVMValueRef valueRef )
            : base( valueRef )
        {
        }
    }
}
