﻿// <copyright file="Fence.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET.Instructions
{
    public class Fence
        : Instruction
    {
        internal Fence( LLVMValueRef valueRef )
            : base( valueRef )
        {
        }
    }
}
