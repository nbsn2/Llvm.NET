﻿// <copyright file="ConstantAggregateZero.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET.Values
{
    public class ConstantAggregateZero
        : ConstantData
    {
        internal ConstantAggregateZero( LLVMValueRef valueRef )
            : base( valueRef )
        {
        }
    }
}
