﻿// <copyright file="BinaryOperator.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET.Instructions
{
    /// <summary>Base class for a binry operator</summary>
    public class BinaryOperator
        : Instruction
    {
        internal BinaryOperator( LLVMValueRef valueRef )
            : base( valueRef )
        {
        }
    }
}
