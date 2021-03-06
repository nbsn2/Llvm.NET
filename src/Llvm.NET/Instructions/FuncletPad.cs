﻿// <copyright file="FuncletPad.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Llvm.NET.Native;
using Llvm.NET.Values;

namespace Llvm.NET.Instructions
{
    /// <summary>Funclet pad for exception handling</summary>
    public class FuncletPad
        : Instruction
    {
        /// <summary>Gets the outer EH-pad this funclet is nested withing</summary>
        /// <remarks>
        /// <note type="note">This returns the associated <see cref="CatchSwitch"/> if this
        /// <see cref="FuncletPad"/> is a <see cref="CleanupPad"/> instruction.</note>
        /// </remarks>
        public Value ParentPad => GetOperand<Value>( -1 );

        /// <summary>Gets the argument operands for this <see cref="FuncletPad"/>.</summary>
        public IReadOnlyList<Value> ArgOperands => new UserOperandList( this, 1 );

        internal FuncletPad( LLVMValueRef valueRef )
            : base( valueRef )
        {
        }
    }
}
